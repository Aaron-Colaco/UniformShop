using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopUnifromProject.Data;
using ShopUnifromProject.Models;

using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Net.Mail;
using Microsoft.Extensions.Hosting.Internal;
using Tesseract;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;


namespace ShopUnifromProject.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        HostingEnvironment _hostingEnvironment = new HostingEnvironment();

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult SubmitId(IFormFile Idfile)
        {
            // Check if no file was uploaded
            if (Idfile == null || Idfile.Length == 0)
            {
                ViewBag.Result = "None";
            }


            var uploadsPath = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads");

            // Create the full file path (including the file name)
            string filePath = Path.Combine(uploadsPath, Idfile.FileName);

            // If the uploads folder doesn't exist, create it
            Directory.CreateDirectory(uploadsPath);


            //code from tesset lbary 
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Idfile.CopyTo(stream);
            }

            // Define the path to the Tesseract data files (tessdata)
            var tessDataPath = Path.Combine(_hostingEnvironment.ContentRootPath, "IdUpload");

            // Create a Tesseract engine to process the image
            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            {
                // Load the image file from the server
                using (Pix pix = Pix.LoadFromFile(filePath))
                {
                    // Process the image to extract text
                    using (var page = engine.Process(pix))
                    {

                        string res = page.GetText();



                        string idPattern = @"\b(?<id>\d{6})\b"; // Matches any standalone 6-digit number
                        string yearPattern = @"ss:\s*(?<year>\d{2})"; // Matches 'Year/class: XX'
                        string dobPattern = @"DOB:\s*(?<dob>\d{2}/\d{2}/\d{4})"; // Matches 'DOB: MM/DD/YYYY'

                        string namePattern = @"land\s*\n\s*(?<name>[A-Z][a-z]*\s+[A-Z][a-z]*)\s*\n";

                        // Extract the name using regex
                        string name = Regex.Match(res, namePattern).Groups["name"].Value.Trim();


                        string id = Regex.Match(res, idPattern).Groups["id"].Value.Trim();
                        string year = Regex.Match(res, yearPattern).Groups["year"].Value.Trim();
                        string dob = Regex.Match(res, dobPattern).Groups["dob"].Value.Trim();

                        if (id.IsNullOrEmpty() || year.IsNullOrEmpty() || name.IsNullOrEmpty())
                        {
                            ViewBag.Results = "f";
                        }
                        else
                        {
                            var customer = _context.Customer.Where(a => a.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)).FirstOrDefault();
                            customer.yearLevel = Convert.ToInt32(year);
                            _context.SaveChanges();
                            ViewBag.Name = name;
                            ViewBag.id = id;
                            ViewBag.year = year;
                            ViewBag.dob = dob;


                            ViewBag.Results = "t";
                        }

                    }
                }
            }

            return View("CheckOut");
        }
        



            //Restricts Method to Admin Only
            [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchByCustomer(string CustomerName)
      {
            //searches for orders where the customer's full name or email contains the CustomerName vlaue passed into the method.
            // Excludes orders with a status ID of 1 as these order have no yet been payed for.
            var Results = _context.Order.Where(a => a.Customers.FullName.Contains(CustomerName) || a.Customers.Email.Contains(CustomerName) && a.StatusId != 1).Include(a => a.Customers).Include(a => a.Status);
             //Returns reuslts to the index page.
            return View("Index", await Results.ToListAsync());
       }


        [Authorize(Roles = "Admin")]//Restricts Method to Admin Only
        public async Task< IActionResult> FilterOrdersByDate(DateTime Date1, DateTime Date2)
        {

            //  Find Orders with dates between the specified start date (Date1) and end date (Date2) passed into the method. Excludes orders with a status ID of 1 as these order have no yet been payed for.

            var OrderData = _context.Order.Where(a => a.OrderTime >= Date1 && a.OrderTime <= Date2 && a.StatusId != 1).Include(a => a.Status).Include(a => a.Customers);
            //retuns OrderData to the index view 
            return View("Index", await OrderData.ToListAsync());
        }

           
            public async Task<IActionResult> Index(string sortOrder, int Page = 1)
        {

            var OrderData = from a in _context.Order
                            select a;

            // Set up sorting parameters for the view.
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            // Switch statement to handle different sorting options.
            switch (sortOrder)
            {
              
                case "Date":
                    // Sort orders by ascending order time.
                    OrderData = OrderData.OrderBy(a => a.OrderTime);
                    break;
                case "date_desc":
                    // Sort orders by descending order time.
                    OrderData = OrderData.OrderByDescending(s => s.OrderTime);
                    break;
                default:
                    // Default sorting (descending order time).
                    OrderData = OrderData.OrderByDescending(s => s.OrderTime);
                    break;
            }

            //If user is a admin return all orders where th status id is not one and inlcude the related customers.
            if (User.IsInRole("Admin"))
            {
                OrderData = OrderData.Where(a => a.StatusId != 1).Include(o => o.Status).Include(a => a.Customers);
          
            }
            else // if user is not admin only display their order, by using thier id to find only orders that belong to them(the logged in user).
            {
                OrderData = OrderData.Include(o => o.Status).Include(a => a.Customers).Where(a => a.CustomerId == User.FindFirstValue(ClaimTypes.NameIdentifier));
           
            }


            const int ITEMSPERPAGE = 20;


            //Calculates the number of pages needed based on the number of orders in the database.
            ViewBag.Pages = (int)Math.Ceiling((double)OrderData.Count() / ITEMSPERPAGE);
            //Stores the number of pages needed in the view bag.
            ViewBag.PageNumber = Page;
            //Return the index view, skip the (page number passed into the method) -1 * 6 amount of orders.  Then take 20 orders to list on the items view/page.
            return View(await OrderData.Skip((Page - 1) * ITEMSPERPAGE).Take(20).AsNoTracking().ToListAsync());

  

        }



        public IActionResult CheckOut()
        {
            return View();
        }
      

        public async Task<IActionResult> Details(string id)
        {

            var test = _context.Order.Where(a => a.OrderId == id && a.CustomerId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if(!User.IsInRole("Admin") && test == null)
            {
                throw new Exception();
            }

            //find order bassed on the id passed into the method
      
            var order = await _context.Order
                .Include(o => o.Status)
                .Include(a => a.Customers)
             
                .FirstOrDefaultAsync(m => m.OrderId == id);
          
           // return order to details page
            return View(order);
        }

        //restricts method to admin use only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StatusUpdate(string id)
        {
            //stores informtion about the order based on the id passed into the method, in the view bag so it an be accesed from the view.
            ViewBag.OrderId = id;
            
            var order = _context.Order.Where(a => a.OrderId == id).Include(a => a.Customers).First();
            ViewBag.Customer = order.Customers.FullName;
            ViewBag.Date = order.OrderTime;
            //return status in the database to the view.
            ViewBag.Status = _context.Status;
            ViewBag.StatId = order.StatusId;
            //returns the status update view.
            return View("StatusUpdate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder(int status, string orderId)
        {
            //Find order bassed on id passed into the method and also include the customer related to the order.
            var order = _context.Order.Where(a => a.OrderId == orderId).Include(a => a.Status).Include(a => a.Customers).FirstOrDefault();
            //set statusId of the order to the statusId passed into the method.
            order.StatusId = status;
            string OrderStatus = _context.Status.Where(a => a.StatusId == status).Select(a => a.Name).First();
            //save changes
            _context.SaveChanges();
            //Call the SendEmailToCustomer method and pass in the customers email, a subject and body to email the customer about their status update.
            HomeController.SendEmailToCustomer(order.Customers.Email, "<h1>Dear " + order.Customers.FullName + "<h1><h5>Your order is now " + OrderStatus + "</h5>", "<h5>Update on" + order.Customers.FullName + "<p><b>Pick up Address</b></p>                <p>                  51 Victor Street, Avondale<br>                   Auckland 1026                   <br>                  New Zealand<br>              </p>             <p>phone: <a>+64 9 828 7024</a></p>                <p><a>email: Uniform@avcol.school.nz</a></p>           </div>           <p>Come meet our friendly staff at the avondale college uniform shop during these times</p>           <div>         <p> Monday to Friday during term time, from 8.15 am - 1.30 pm, selling the Avondale College uniform and some stationery items.</p>          </div>           <p>When you place an order, you will receive an email detailing the pickup time and date.</p>        <div>                  <button onclick=\"window.location.href='www.google.com/maps/place/Avondale+College/@@-36.8911528,174.6896728,16.36z/data=!4m8!3m7!1s0x6d0d46cd23ece1e9:0xe57882263b141a5e!8m2!3d-36.8917835!4d174.6908506!9m1!1b1!16zL20vMGRmOHBk?hl=en-NZ&entry=ttu'\">DIRECTIONS</button>            </div>        </div>      <div class=\"map-section\">          <iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3190.9401849489336!2d174.6882756760394!3d-36.89177918191069!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x6d0d46cd23ece1e9%3A0xe57882263b141a5e!2sAvondale%20College!5e0!3m2!1sen!2sus!4v1720241713704!5m2!1sen!2sus\" style=\"border:0;\" width=\"680px\"                  height=\"560px\" allowfullscreen=\"\" loading=\"lazy\" referrerpolicy=\"no-referrer-when-downgrade\"></iframe>        </div> ");
                //return to the index action.
            return RedirectToAction("StatusUpdate", new {id = orderId});

        }


        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            //Find Order bassed on the id passed into the method.
            var order = await _context.Order
                .Include(o => o.Customers)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            // return Delete view passing in the order.
            return View(order);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //find the order bassed on the id passed In.
            var order = await _context.Order.FindAsync(id);
           
         //Removes the Order from the database and saves changes
             _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            //return to the index action.
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(string id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
