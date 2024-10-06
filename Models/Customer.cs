using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace ShopUnifromProject.Models
{

    // Represents a customer inheriting from default idenity IdentityUser class
    public class Customer: IdentityUser
    {
       // limits to 100 characters
        [MaxLength(100)]
        public string FullName { get; set; }

        public string DOB { get; set; }

        public string StudentNumber { get; set; }

        public int yearLevel { get; set; }
        // List of orders associated with the customer
        public List <Order> Orders{ get; set; }
    }
}
