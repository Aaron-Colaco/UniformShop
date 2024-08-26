using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopUnifromProject.Models
{
    public class Order
    {
        // Unique identifier for the order
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderId { get; set; }

        // Time the order was placed, required and defaulted to current time
        [Required]
        public DateTime OrderTime { get; set; } = DateTime.Now;

        // Total price of the order, required to be between 1 and 1000, formatted as currency
        [DataType(DataType.Currency),Range(1,1000)]
        public decimal TotalPrice { get; set; }

      

        // Collection of items in the order
        public ICollection<OrderItem> OrderItems { get; set; }


        // Foreign key referencing the order status sets up one to many relasitonship 
        [ForeignKey("Status")]
        public int StatusId { get; set; } = 1;
        public Status Status { get; set; }

        //Foreign key referencing the customer who placed the order sets up one to many relasitonship 
        [ForeignKey("Customer")]
        public string  CustomerId { get; set; }

        public Customer Customers { get; set; }

    }
}
