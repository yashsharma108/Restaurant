using Restaurant.Models.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models.CoreModels
{
    public class MenuCategory
    {
        public MenuCategory()
        {
            MenuItems = new HashSet<MenuItem>();
        }

        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation property
        public virtual ICollection<MenuItem> MenuItems { get; set; }
    }
}