
using ShopUnifromProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopUnifromProject.Data;
using System.Globalization;

namespace AaronColacoAsp.NETProject.Controllers
{
    public class SalesDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SalesDataController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DatePicker()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SalesDashboard(DateTime Date1, DateTime Date2)
        {
            // Retrieve orders bettwen date 1 and date 2 which values are passed into the method. Exclude orders with status id of one as thse ordr have not been paid for yet.
            var orderData = _context.Order.Where(a => a.OrderTime >= Date1 && a.OrderTime <= Date2).Where(a => a.StatusId != 1);
            
            // Retrieve ordersitems data where order date are bettwen date 1 and date 2 which are passed into the method. Exclude orders with status id of one as thse ordr have not been paid for yet.
            var orderItemData = _context.OrderItem.Where(a => a.Orders.OrderTime >= Date1 && a.Orders.OrderTime <= Date2).Where(a => a.Orders.StatusId != 1);

            //Calculate total sales by summing the product of price * quantity
            decimal totalSales = orderItemData.Sum(a => a.Items.Price * a.Quantity);
            ViewBag.totalSales = totalSales.ToString("F2");
            // Calculate total expenses by summing up the (product of cost to produce * quantity)
            decimal totalExpense = orderItemData.Sum(a => a.Items.CostToProduce * a.Quantity);
            ViewBag.totalExpense = totalExpense.ToString("F2");

            // Calculate profit the difference between total sales and total expenses
            decimal profit = (totalSales - totalExpense);
            ViewBag.profit = profit.ToString("F2");


            // Calculate the total number of items sold
            var totalProductsSold = orderItemData.Sum(a => a.Quantity);
            ViewBag.TotalProductsSold = totalProductsSold;

            // Calculate the average order cost (if pitems were sold)
            decimal averageOrderCost;
            if (totalProductsSold == 0)
            {
                //if not products are sold set average cost to 0.
                averageOrderCost = 0;
            }
            else
            {
                averageOrderCost = orderData.Average(a => a.TotalPrice);
            }

            ViewBag.averageOrderCost = averageOrderCost.ToString("F2"); 
            //store the data in the view bag so it can be accesed in the view.

            //return the salesDashboard view
            return View();


        }
    }
}