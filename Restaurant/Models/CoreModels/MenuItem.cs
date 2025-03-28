using Restaurant.Models.Core;
using Restaurant.Models.CoreModels;
using Restaurant.Models.Orders;
using RestaurantManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models.Core
{
    public class MenuItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 1000)]
        public decimal Price { get; set; }

        [Range(1, 120)]
        public int PreparationTime { get; set; } // in minutes

        public bool IsAvailable { get; set; } = true;

        // Navigation properties
        public virtual MenuCategory Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}