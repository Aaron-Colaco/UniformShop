using System.ComponentModel.DataAnnotations;

namespace ShopUnifromProject.Models
{
    public class Category
    {
        //Primary key feild unique identifier
        [Key]      
        public int CategoryId { get; set; }

        //sets the max lenght to 30 and maxs feild required
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        //collections of items in catergory
        public ICollection<Item> Items { get; set; }


    }
}
