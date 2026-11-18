using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DazzleJewels.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [StringLength(30)]
        public string Status { get; set; } = "Pending";
    }
}