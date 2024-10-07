using ShopUnifromProject.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;


using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using ShopUnifromProject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace ShopUnifromProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //Void method that will send an email, takes in 3 parameters: CusotmersEmail, the HTML text for the body and the subject. 
        public static void SendEmailToCustomer(string userEmail, string htmlForBody = null, string subject = null)
        {

            // Variables to store the Ceridantles for the Outlook Account as well as the Customers Email
            var adminEmail = new MailAddress("shopapp@zohomail.com.au", "Uniform Shop App");
            var Cusotmer = new MailAddress(userEmail, "Dear Customer");
            var adminpassword = "LrwFBdvz2ns9";



            try
            {
                //sets up Smtp Client for the Mail Kit API.
                var smtp = new SmtpClient
                {
                    //Set up to matacth the Outlook Smtp Settings
                    Host = "smtp.zoho.com.au",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(adminEmail.Address, adminpassword)
                };
                // Create a varaible called email content using the Mail kit Api And pass in Body and Subject 
                using (var emailcontent = new MailMessage(adminEmail, Cusotmer)
                {
                    Subject = subject,
                    Body = htmlForBody,
                    //Lets html and css code be used to cusotmise the email.
                    IsBodyHtml = true


                })
                {
                    //send the emailcontent to the user.
                    smtp.Send(emailcontent);
                }
            }
            //If error occurs dont send email and return action to the method that initially called the send email method
            catch (Exception error)
            {
                return;
            }
           
                return;
            
        }
        public IActionResult SendEmail (string userEmail, string htmlForBody = null, string subject = null)
        {

            // Variables to store the Ceridantles for the Outlook Account as well as the Customers Email
            var adminEmail = new MailAddress("shopapp@zohomail.com.au", "Uniform Shop App");
           
            var adminpassword = "LrwFBdvz2ns9";



            try
            {
                //sets up Smtp Client for the Mail Kit API.
                var smtp = new SmtpClient
                {
                    //Set up to matacth the Outlook Smtp Settings
                    Host = "smtp.zoho.com.au",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(adminEmail.Address, adminpassword)
                };
                // Create a varaible called email content using the Mail kit Api And pass in Body and Subject 
                using (var emailcontent = new MailMessage(adminEmail, adminEmail)
                {
                    Subject = subject,
                    Body = htmlForBody,
                    //Lets html and css code be used to cusotmise the email.
                    IsBodyHtml = true


                })
                {
                    //send the emailcontent to the user.
                    smtp.Send(emailcontent);
                }
            }
            //If error occurs dont send email and return action to the method that initially called the send email method
            catch (Exception error)
            {
                return RedirectToAction("Index");

            }

            // return action to the method that initially called the send email method

            return RedirectToAction("Index");
           
        }

            //Deafualt MVC Code
            public IActionResult Index()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
