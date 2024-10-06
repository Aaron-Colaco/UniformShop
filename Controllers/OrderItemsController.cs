﻿using ShopUnifromProject.Data;
using ShopUnifromProject.Models;
using ShopUnifromProject.Stripe_Payment_API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;
using static System.Net.WebRequestMethods;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using System.Drawing;

namespace ShopUnifromProject.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly StripeSettings _stripeSettings;


        public OrderItemsController(IOptions<StripeSettings> stripeSettings, ApplicationDbContext context)
        {
            _stripeSettings = stripeSettings.Value;
            _context = context;


        }


        public async Task<IActionResult> OpenCart()
        {
            //Calls the GetOrder Method and stores the return value in a Variable called order.
            var Order = await GetOrder();
            //returns the index view pasiing the Order Variable
            return View("Index", Order);
        }







        public async Task<IActionResult> AddToCart(int itemId, string size)
        {

            //If user is not logined in redrict to to Register page
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectPermanent("/Identity/Account/Register");
            }




            // Call the CheckUserOrders method and store the return vlaue in a string called orderId
            string orderId = await CheckUserOrders();

            // Call the Items In order method and store the return values in a var.
            var itemsInOrder = await GetOrder();

            //If the sum of the items in a user order is >= 35
            if (itemsInOrder.Sum(a => a.Quantity) >= 35)
            {
                ViewBag.CartFull = 1;
                //return to Index method passing in the order id as well as true for the cartfull parameter
                return RedirectToAction("Index", new { id = orderId, CartFull = true });
            }

            //Find if any items in the order where the itemId is the same as the itemid passed into the method
            var ExistingItem = itemsInOrder.Where(a => a.ItemId == itemId && a.Size == size).FirstOrDefault();



            //If item Exist in order
            if (ExistingItem != null)
            {
                //increase the quantity by 1
                ExistingItem.Quantity++;
            }
            else // If item is not in order add new item
            {

                var OrderItem = new OrderItem
                {
                    //Set the OrderId to the orderId string, the ItemId to the itemId passed into the method and the quantity to one.
                    OrderId = orderId,
                    ItemId = itemId,
                    Quantity = 1,
                    Size = size
                };
                // add the new Orderitem details to the database
                _context.OrderItem.Add(OrderItem);
            }
            // Save changes
            await _context.SaveChangesAsync();

            //Find the Order where the OrderId is equal to the orderId string.
            var Order = _context.Order.Where(a => a.OrderId == orderId).First();

            // Call the Items In order method again to update the var itemsInOrder
            itemsInOrder = await GetOrder();

            //Set the total Price of the order to the sum of the( items Price * Items Quantity) in the var itemsInOrder.
            Order.TotalPrice = itemsInOrder.Sum(a => a.Items.Price * a.Quantity);

            //save changes
            await _context.SaveChangesAsync();

            //Redirect to the items index action passing true for the display pop up parameter and ItemId for the item parrameter.
            return RedirectToAction("Index", "Items", new { displayPopUp = true, item = itemId });
        }



        [HttpPost]
        //make sure user is logged in before accesing the method
        [Authorize]
        //This method returns a string
        public async Task<string> CheckUserOrders()
        {
            //Find the id of the Cusotomer that is logged in
            var Customer = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Find a order in the database where the cusotmer id of the order equals the id of the logged in user.
            var UserOrder = _context.Order.Where(a => a.CustomerId == Customer && a.StatusId == 1).FirstOrDefault();

            //If user has no orders, or no order with the status of one
            if (UserOrder == null)
            {
                //Create new order
                var NewOrder = new Models.Order
                {
                    //set the customer id to the id of the cureently logined in user.
                    CustomerId = Customer,
                    StatusId = 1,// set the status id to one.
                    OrderTime = DateTime.Now // set the order time to the current time now
                };
                //Add the new order to the database and save changes.
                _context.Order.Add(NewOrder);
                await _context.SaveChangesAsync();

                //return the string Order Id of the new order
                return NewOrder.OrderId;
            }
            else
            // if the user already as a order where the statusid is one 
            {
                //return the stirng OrderId of the exisitng Order.
                return (UserOrder.OrderId);
            }
        }

        public async Task<IActionResult> Index(string id, bool cartFull = false)
        {

            //Find order where the order id == the id passed into the method.
            var order = _context.Order.Where(a => a.OrderId == id).FirstOrDefault();

            //check that the Order belongs to the currently logins in use for security purpose or that the user is admin
            if (order.CustomerId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Admin"))
            {
                // if order dose not belong to user return Error Message
                return View("Cant find Order that Belongs to you");

            }

            //Store the cart full parameter in the view bag as well as the order status.
            ViewBag.CartFull = cartFull;
            ViewBag.StatusId = order.StatusId;
            ViewBag.TotalRrice = order.TotalPrice;

            //retrieves all OrderItems related to the order from the database, including their items and returns it to the index view
            var OrderItem = await _context.OrderItem.Where(a => a.OrderId == order.OrderId).Include(a => a.Items).Include(a => a.Orders).ThenInclude(a => a.Status).ToListAsync();
            return View(OrderItem);
        }

        public async Task<IActionResult> ProcessOrder(string fullName, string DOB, int Year, string Id)
        {
            //Calls the CheckUserOrders method and stores the return Value in a string.
            string orderId = await CheckUserOrders();
            //Find the Order the belong to the user by serahcing the database for an order that as the same order id as the vlaue stored in the OrderId string.
            var orderToProcess = _context.Order.Where(a => a.OrderId.Equals(orderId)).First();
            //Find the customer the belogns to that Order.
            var Customer = _context.Customer.Where(a => a.Id.Equals(orderToProcess.CustomerId)).First();

            //Set the details such as the full name and phone number of the cusotmer to realted parameters passed in.
            Customer.FullName = fullName;
            Customer.DOB = DOB;
            Customer.yearLevel = Year;
            Customer.StudentNumber = Id;

            _context.SaveChanges();
        /*    //Future code to verfiy student id against school database

            This will run a linq query in the verfiy the sutdnet in the future, but for privacy reasons I cant access the School DB. 
            if (_context.Customer.Where(a => a.DOB == DOB && a.yearLevel == Year && a.Id == Id && a.FullName == fullName) == null)
            {
                return View("Checkout");
            }
*/



            //Set the delivery details of orderToProccess realted parameters passed in.
            string removedItems = "";
            var order = await GetOrder();
            bool itemsRemoved = false;
            foreach (var item in order)
            {
                if (item.Items.YearLevelNeededtobuy > Customer.yearLevel)
                {
                    _context.OrderItem.Remove(item);
                    _context.SaveChanges();

                    
                    itemsRemoved = true;

                    if (!removedItems.Contains(item.Items.Name)){
                        
                        removedItems = removedItems + item.Items.Name;

                    }
          
                    


                }
            }

            if (itemsRemoved == true)
            {
                ViewBag.RemoveItems = removedItems;

                return View();
            }






            //Create a new gift using the infromation in the realted parameters passed in.

            //add the gift Recipent and save changes




            //Redirect to the Pyament Method passing in the order Id
            return RedirectToAction("Payment", new { OrderId = orderId });


        }

        //This method returns a lsit of type orderItem
        public async Task<List<OrderItem>> GetOrder()
        {
            //gets the order id from the return method CheckUserOrders.
            string orderId = await CheckUserOrders();
            // Find all ordered Items with that orderId, including their related items and orders.                 
            var listOrderItems = await _context.OrderItem.Include(o => o.Items).Include(o => o.Orders).Where(a => a.OrderId == orderId).ToListAsync();

            //return this list to the caller Method.
            return listOrderItems;
        }




        public async Task<IActionResult> Success()
        {
            //Calls the checkUserOrders method and finds the user's order whith the string order id the method returns.
            string orderId = await CheckUserOrders();
            var userOrder = _context.Order.Where(a => a.OrderId == orderId).Include(a => a.Customers).FirstOrDefault();

            /// Set the statusId of user order to 2 and the Order time to the current date time.
            userOrder.StatusId = 2;
            userOrder.OrderTime = DateTime.Now;

            await _context.SaveChangesAsync();

            //Create a Message for the email body, and pass that message to the Homecotntroller send email mehtod. along with passing the Cusotmer email and a subject. 
            string EmailBody = "<h1>Dear " + userOrder.Customers.FullName + ",</h1><p>Thank you for your order. We will work on processing your order as soon as we can. you wil recvice an email when it is ready to be collected</p><p>Your total cost was $" + userOrder.TotalPrice.ToString() + ".</p>";
            HomeController.SendEmailToCustomer(userOrder.Customers.Email, EmailBody, "Thanks " + userOrder.Customers.FullName);

            //Return action to the Order Controller index method.
            return RedirectToAction("Details", "Orders", new {id = orderId});
        }



        public async Task<IActionResult> Cancel()
        {
            //Calls the checkUserOrders method and finds the user's order whith the string order id the method returns.
            string orderId = await CheckUserOrders();
            var userOrder = _context.Order.Where(a => a.OrderId == orderId).First();

            //Remove the User's Order from the database and ave changes
            _context.Remove(userOrder);
            await _context.SaveChangesAsync();
            //Return action to the Order Controller index method.
            return RedirectToAction("Index", "Home");

        }



        public IActionResult Payment(string orderId)
        {
            // Set the Stripe API key using the secret key from the `_stripeSettings` class.
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            //gets the items in the order with the orderId passed in.
            var itemsInOrder = _context.OrderItem.Where(a => a.OrderId == orderId).Include(a => a.Items);

            // Create options for the Stripe session.
            var Options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                CustomerEmail = User.Identity.Name,
                SuccessUrl = "https://localhost:7055/OrderItems/Success",
                CancelUrl = "https://localhost:7055/OrderItems/Cancel",
                Mode = "payment",
                ClientReferenceId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            };


            // Add each ordered item to the session line items for the stripe API.
            foreach (var item in itemsInOrder)
            {
                var orderedItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Items.Price * 100,
                        Currency = "nzd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {

                            Description = "Size " + item.Size,
                            Name = item.Items.Name //pases the item name
                        }
                    },
                    Quantity = item.Quantity //pases the quantity
                };
                Options.LineItems.Add(orderedItem);

            }

            // Create a Stripe session using the specified options.
            var service = new SessionService();
            var session = service.Create(Options);

            // Redirect the user to the Stripe portal for payment.
            return Redirect(session.Url);

        }


        public async Task<IActionResult> Delete(int itemId)
        {
            //Gets the orderItems of the users order from the return method
            var OrderItems = await GetOrder();
            //Finds the item to remove in the user order, bassed on the item passed into the method.
            var OrderItemToRemove = OrderItems.Where(a => a.ItemId == itemId).FirstOrDefault();
            //removes item from order and save changes to database,
            _context.OrderItem.Remove(OrderItemToRemove);
            _context.SaveChanges();

            // Redirect action to Open Cart
            return RedirectToAction("OpenCart");
        }
    }
}
