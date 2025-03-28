using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models.Inventory
{
    public class InventoryItem
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } // kg, liter, piece, etc.

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityInStock { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumRequired { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPerUnit { get; set; }

        [StringLength(100)]
        public string Supplier { get; set; }
    }
}