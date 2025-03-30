using Restaurant.Models.Orders;

namespace Restaurant.Models.ViewModels
{
    public class KitchenOrderViewModel
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public string TableNumber { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; }
        public OrderDetail.OrderItemStatus Status { get; set; } // Changed to enum
        public int PreparationTime { get; set; }
        public DateTime OrderTime { get; set; }
    }
}