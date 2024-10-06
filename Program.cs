
using ShopUnifromProject.Data;
using ShopUnifromProject.Models;
using ShopUnifromProject.Stripe_Payment_API;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



namespace ShopUnifromProkect
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
            var key = builder.Configuration.GetValue<string>("StripeSettings:SecretKey");

            builder.Services.AddDefaultIdentity<Customer>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            DataForDatabase.AddData(app);


            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();



                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

            }

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Customer>>();



                string adminID = "87612";
                string AdminPassword = "AdminPassword@2024";


                if (await userManager.FindByEmailAsync("Admin@Uniform.co.nz") == null)
                {
                    var user = new Customer();
                    user.Id = adminID;
                    user.UserName = "Admin@Uniform.co.nz";
                    user.Email = "Admin@Uniform.co.nz";
                    user.yearLevel = "13";

                    await userManager.CreateAsync(user, AdminPassword);
                    await userManager.AddToRoleAsync(user, "Admin");

                }




            }

            app.Run();





        }
    }
}