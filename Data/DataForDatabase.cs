using ShopUnifromProject.Models;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace ShopUnifromProject.Data
{

    public class DataForDatabase
    {


        public static void AddData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var Context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                Context.Database.EnsureCreated();


                //check if any items already exist


                if (Context.Category.Any() || Context.Item.Any() || Context.Status.Any() || Context.Order.Any() || Context.OrderItem.Any())
                {
                    return;
                }

                else {

                    var Categories = new Category[]
                    {
new Category { Name = "Tops"},
new Category { Name = "Bottoms"},
new Category { Name = "Others"},
new Category { Name = "Seasonal"}
                    };

                    Context.Category.AddRange(Categories);

                    Context.SaveChanges();

                    var ItemsData = new Item[]
                    {
// Tops

new Item{Name = "Senior Shirt (long sleeve) ", CategoryId = 1, ImageURL = "../Images/long.png", CostToProduce = 60, Price = 60, Description = "Long sleeve white shirt for senior students"},
new Item{Name = "Polo shirt (long sleeve)", CategoryId = 1, ImageURL = "../Images/longshirt.png", CostToProduce = 40, Price = 40, Description = "Long sleeve polo shirt for cooler days."},
new Item{Name = "Polo shirt (short sleeve)", CategoryId = 1, ImageURL = "../Images/shirtshort.png", CostToProduce = 35, Price = 35, Description = "Comfortable short sleeve polo shirt, ideal for summer."},
new Item{Name = "Senior Shirt (short sleeve) ", CategoryId = 1, ImageURL = "../Images/shortt.png", CostToProduce = 60, Price = 60, Description = "Short sleeve white shirt for students"},
new Item{Name = "White Senior blouse (short sleeve)", CategoryId = 1, ImageURL = "https://www.nzuniforms.com/imagecache/pi_1_1116813_5.png", CostToProduce = 56, Price = 56, Description = "Short sleeve white blouse for senior students."},
new Item{Name = "White Senior blouse (long sleeve)", CategoryId = 1, ImageURL = "https://www.nzuniforms.com/imagecache/pi_1_1116830_5.jpg", CostToProduce = 59, Price = 59, Description = "Long sleeve white blouse for senior students."},

new Item{Name = "PE top (unisex)", CategoryId = 1, ImageURL = "../Images/pe.png", CostToProduce = 60, Price = 60, Description = "Unisex PE top for physical education classes."},




new Item{Name = "Long skirt", CategoryId = 2, ImageURL = "https://th.bing.com/th/id/OIP._U1cxfly-03eOu35SWpmkgAAAA?pid=ImgDet&w=196&h=262&c=7&dpr=1.5", CostToProduce = 80, Price = 80, Description = "Elegant long skirt for formal occasions."},
new Item{Name = "Short skirt", CategoryId = 2, ImageURL = "https://th.bing.com/th/id/OIP._U1cxfly-03eOu35SWpmkgAAAA?pid=ImgDet&w=196&h=262&c=7&dpr=1.5", CostToProduce = 69, Price = 69, Description = "Junior short skirt, ideal for summer."},
new Item{Name = "Grey Shorts", CategoryId = 2, ImageURL = "https://cdn.theschoollocker.com.au/media/catalog/product/cache/1/image/9df78eab33525d08d6e5fb8d27136e95/s/h/short_tkis.jpg", CostToProduce = 50, Price = 50, Description = "Comfortable grey pants, usually worn by boys"},
new Item{Name = "Grey Pants", CategoryId = 2, ImageURL = "https://th.bing.com/th/id/OIP.r_XVt_QOQKyhbA9pJ6gOeQHaHa?w=600&h=600&rs=1&pid=ImgDetMain", CostToProduce = 50, Price = 50, Description = "Comfortable grey pants, usually worn by boys"},
new Item{Name = "Black Pants", CategoryId = 2, ImageURL = "https://www.nzuniforms.com/imagecache/ii_1_1003627_5.jpg", CostToProduce = 50, Price = 50, Description = "Comfortable black pants, usually worn by y13 "},



// Others
new Item{Name = "Black crew socks 1 pair", CategoryId = 3, ImageURL = "https://th.bing.com/th/id/OIP.sw4l-ft9FrvNaib2xFuzlQHaIx?rs=1&pid=ImgDetMain",CostToProduce = 12, Price = 12, Description = "Three pairs of comfortable black crew socks."},
new Item{Name = "Black crew socks 3 pair", CategoryId = 3, ImageURL = "https://th.bing.com/th/id/OIP.sw4l-ft9FrvNaib2xFuzlQHaIx?rs=1&pid=ImgDetMain", CostToProduce = 24, Price = 24, Description = "Three pairs of comfortable black crew socks."},
new Item{Name = "Long black socks", CategoryId = 3, ImageURL = "https://th.bing.com/th/id/OIP.Xzzv8CwcOBDVcibpspo6jQHaJ_?rs=1&pid=ImgDetMain", CostToProduce = 10, Price = 10, Description = "Long black socks for formal wear."},

// Seasonal
new Item{Name = "Scarf", CategoryId = 4, ImageURL = "https://th.bing.com/th/id/OIP.ElbA-S4e-XJF8RAsI1P9NwHaHf?w=190&h=191&c=7&r=0&o=5&dpr=1.5&pid=1.7", CostToProduce = 20, Price = 20, Description = "Cozy scarf for cold weather."},
new Item{Name = "Black/white jersey", CategoryId = 4, ImageURL = "../Images/jump.png", CostToProduce = 99, Price = 99, Description = "Warm black and white jersey for winter."},
new Item{Name = "Jacket", CategoryId = 4, ImageURL = "../Images/unifromshop.png", CostToProduce = 120, Price = 120, Description = "Warm jacket, ideal for winter."},
new Item{Name = "Hat", CategoryId = 4, ImageURL = "../Images/unifromshop.png", CostToProduce = 120, Price = 120, Description = "Warm jacket, ideal for winter."}


};




                    Context.Item.AddRange(ItemsData);

                    Context.SaveChanges();





                    var StatusData = new Status[]
                    {
                    new Status{Name = "Pending"},
                    new Status{Name = "Processing" },
                    new Status{Name ="Ready To Collect"},
                    new Status{Name ="Fullfilled"}

                    };




               



                   

           






                   



                }


            }



        }



    }

}










