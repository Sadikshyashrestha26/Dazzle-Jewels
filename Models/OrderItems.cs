using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DazzleJewels.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        // Foreign Keys
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        // Navigation Properties
        public Order? Order { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
    }
}