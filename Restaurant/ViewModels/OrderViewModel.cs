using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Restaurant.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        [Display(Name = "Table")]
        [Required]
        public int TableId { get; set; }
        public SelectList TableOptions { get; set; }

        [Display(Name = "Staff Member")]
        public string StaffId { get; set; }
        public SelectList StaffOptions { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public List<SelectListItem> AvailableMenuItems { get; set; }
    }

    public class OrderItemViewModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string SpecialInstructions { get; set; }
    }
}