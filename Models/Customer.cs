using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShopUnifromProject.Models
{

    // Represents a customer inheriting from default idenity IdentityUser class
    public class Customer: IdentityUser
    {
       // limits to 100 characters
        [MaxLength(100)]
        public string FullName { get; set; }

        /// Phone number of the customer, validated with a regular expression
        [RegularExpression(@"^\+?\d{1,3}[- ]?\(?\d{3}\)?[- ]?\d{3}[- ]?\d{4}$", ErrorMessage = "Invalid phone number format")]
        public int PhoneNumebr { get; set; }


        public DateOnly DBO { get; set; }

        public int yearLevel { get; set; }
        // List of orders associated with the customer
        public List <Order> Orders{ get; set; }
    }
}
