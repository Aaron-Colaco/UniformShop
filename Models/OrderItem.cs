
using ShopUnifromProject.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace ShopUnifromProject.Models
{

    public class OrderItem
    {
        //bridging tabel to set up many to many relastionship bettween items and orders

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]


        //Primary key feild
        public string OrderItemId { get; set; }

        //sets the defualt quantity to 1
        public int Quantity { get; set; } = 1;

        //foreign key to set up the relastonship with the order table(many to many)
        [ForeignKey("Order")]
        public string OrderId { get; set; }
        public Order Orders { get; set; }

        //foreign key to set up the relastonship with the item table(many to many)
        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public Item Items { get; set; }

        public string  Size  { get; set; }




    }
}