using System.ComponentModel.DataAnnotations;

namespace ShopUnifromProject.Models
{
    public class Status
    {
        [Key]
        //Primary key feild unique identifier
        public int StatusId { get; set; }

        //Sets the field for status name to required and limits the max length to 50 characters.
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        //Collection of orders that can belong to one status. 
        public ICollection<Order> Orders { get; set; }
    }
}
