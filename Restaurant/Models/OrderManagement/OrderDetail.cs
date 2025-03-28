using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restaurant.Models.Core;    // For MenuItem
using Restaurant.Models.Orders; // For Order

namespace Restaurant.Models.Orders
{
    public class OrderDetail
    {
        public OrderDetail()
        {
            Status = OrderItemStatus.Pending; // Initialize with enum default
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("MenuItem")]
        public int ItemId { get; set; }

        [Required]
        [Range(1, 20)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Price at time of ordering

        [StringLength(500)]
        public string SpecialInstructions { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")] // Store enum as string
        public OrderItemStatus Status { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual MenuItem MenuItem { get; set; }

        // Computed property
        [NotMapped]
        public decimal LineTotal => Quantity * UnitPrice;

        // Enum definition
        public enum OrderItemStatus
        {
            Pending,
            Preparing,
            Ready,
            Served,
            Cancelled,
            Modified
        }

        // Display property for UI
        [NotMapped]
        public string StatusDisplay => Status.ToString();
    }
}