using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopUnifromProject.Data;
using ShopUnifromProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Buffers.Text;
using Humanizer;
using Microsoft.CodeAnalysis.Options;
using System.Drawing;
using Microsoft.CodeAnalysis.CSharp;

namespace ShopUnifromProject.Controllers
{
    //Controller for managing Items
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        //Constructor to Initialaize the Database Context.
        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> SizePicker(int itemId)
        {

            return RedirectToAction("Index", "Items", new { displaySizePicker = true, itemId = itemId });
        }


        // Decalres the constant number of Items Per Page as 6
        public const int ITEMSPERPAGE = 6;

        
        //Index Method takes in Page Number and ItemId which is set to one by default as parameters as well as boolean called display up which is set to false by deafult.
      
        public async Task<IActionResult> Index(int page = 1, int itemId = 1, bool displayPopUp = false, bool displaySizePicker = false)
        {
            ViewBag.displaySizePicker = displaySizePicker;
          
            




            //Finds the item in the database where the item has an id that matches the ItemId passed into the method and stores it the he view bag
            ViewBag.Item = _context.Item.Where(a => a.ItemId == itemId).FirstOrDefault();
            //Stores the value of the display pop-up boolean in the view bag 
            ViewBag.displayPopUp = displayPopUp;
            //Find all the items in the database as well as their category stored in a variable called items
            var Items = _context.Item.Include(i => i.Categorys);

            //Calculates the number of pages needed based on the number of products in the database.
            ViewBag.Pages = (int)Math.Ceiling((double)Items.Count() / ITEMSPERPAGE);

            //Stores the number of pages needed in the view bag.
            ViewBag.PageNUmber = page;

            //Stores the categorys in the database in the viewbag.
            ViewBag.Category = _context.Category;

            //Return the index view, skip the (page number passed into the method) -1 * 6 amount of items. Then take 6 items to list on the items view/page.
            return View(await Items.Skip((page - 1) * ITEMSPERPAGE).Take(ITEMSPERPAGE).ToListAsync());

        }




        public async Task<IActionResult> Search(string searchTerm)
        {

            //Finds all the products in the database whose name contains the search term which is passed into the search method storing it in a variable.
            var Results = _context.Item.Where(i => i.Name.Contains(searchTerm) || i.Description == searchTerm || i.Categorys.Name == searchTerm).Include(i => i.Categorys);
            //Stores the categorys in the database in the viewbag.
            ViewBag.Category = _context.Category;
            //reutrn the Index view passing the reuslts.
            return View("Index", await Results.ToListAsync());
        }



        public async Task<IActionResult> FilterByCategroy(string category)
        {
            
            //Finds all the products in the database whose category name matches the category passed into the method 
            var Results = _context.Item.Where(i => i.Categorys.Name == category).Include(i => i.Categorys);
            //Stores the categorys in the database in the viewbag.
            ViewBag.Category = _context.Category;
            //reutrn the Index view passing the reuslts.
            return View("Index", await Results.ToListAsync());

       
        }

        public async Task<IActionResult> Filter(int minPrice, int maxPrice)
        {
           
            //Finds all the products in the database whose price's are between the min and max price passed into the method
            var Results = _context.Item.Where(i => i.Price >= minPrice && i.Price <= maxPrice).Include(i => i.Categorys);

            //Stores the categorys in the database in the viewbag.
            ViewBag.Category = _context.Category;
            //reutrn the Index view passing the reuslts.
            return View("Index", await Results.ToListAsync());
        }



        //only let Users In the Admin Role Access this Method
        [Authorize(Roles = "Admin")]

        
        public IActionResult Create()
        {
            //Stores the Categorys in ViewData 
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "Name");
            //Reuturns the create View
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]


        //Only let Users In the Admin Role Access this Method
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ItemId,Name,Price,CostToProduce,ImageURL,Description,CategoryId,YearLevelNeededtobuy")] Item item)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                //Add the new item to the database and save changes
                _context.Add(item);
                await _context.SaveChangesAsync();
                // Redirect to the Index method
                return RedirectToAction(nameof(Index));
            }
            //deafualt mvc code used for populating the drop down list for categorys.
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId", item.Categorys.Name);
            //Return the index view
            return View(item);
        }



        //only lets Users In the Admin Role Access this Method
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            //if id parameter null return not found
            if (id == null)
            {
                return NotFound();
            }
           // Find the item by the id passed into the method.
            var item = await _context.Item.FindAsync(id);

            //deafualt mvc code used for populating the drop down list for categorys.
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "Name");
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]


        //only lets Users In the Admin Role Access this Method
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,Name,Price,CostToProduce,ImageURL,Description,CategoryId,YearLevelNeededtobuy")] Item item)
        {
   
            if (!ModelState.IsValid)
            {
                try
                {
                    //Update the item in the context and save changes
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                  
                }
                // Redirect to the Index action
                return RedirectToAction(nameof(Index));
            }
            //deafualt mvc code used for populating the drop down list for categorys.
            ViewData["CategoryList"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryId", item.Categorys.Name);
            return View(item);
        }

         //restricts access to Admin role only
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {


            // Find the item b the id passed into the method.
            var item = await _context.Item
                .Include(i => i.Categorys)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            // Return the view with the item details
            return View(item);
        }

        // POST: Items/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           // Find the item by the id passed into the method
            var item = await _context.Item.FindAsync(id);

            // If item is found, remove it from the database
            if (item != null)
            {
                _context.Item.Remove(item);
            }
            // Save changes
            await _context.SaveChangesAsync();
            // Redirect to the Index method
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ItemId == id);
        }
    }
}
