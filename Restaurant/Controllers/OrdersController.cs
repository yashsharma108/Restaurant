//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using Restaurant.Data;
//using Restaurant.Models.Orders;
//using Restaurant.Hubs; // Make sure this namespace matches your Hub location

//public class KitchenController : Controller
//{
//    private readonly ApplicationDbContext _context;
//    private readonly IHubContext<OrderHub> _hubContext;

//    public KitchenController(ApplicationDbContext context, IHubContext<OrderHub> hubContext)
//    {
//        _context = context;
//        _hubContext = hubContext;
//    }

//    public async Task<IActionResult> KitchenDisplay()
//    {
//        var orders = await _context.OrderDetails
//            .Include(od => od.Order)
//                .ThenInclude(o => o.Table)
//            .Include(od => od.MenuItem)
//            .Where(od => od.Status != OrderDetail.OrderItemStatus.Served.ToString())
//            .OrderBy(od => od.Order.OrderDate)
//            .AsNoTracking() // Recommended for read-only operations
//            .ToListAsync();

//        return View(orders);
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> UpdateStatus(int id, string status)
//    {
//        if (!Enum.TryParse<OrderDetail.OrderItemStatus>(status, out _))
//        {
//            return BadRequest("Invalid status value");
//        }

//        var orderDetail = await _context.OrderDetails
//            .Include(od => od.Order)
//            .FirstOrDefaultAsync(od => od.OrderDetailId == id);

//        if (orderDetail == null)
//        {
//            return NotFound();
//        }

//        orderDetail.Status = status;
//        await _context.SaveChangesAsync();

//        // Notify all kitchen displays via SignalR
//        await _hubContext.Clients.Group("KitchenGroup")
//            .SendAsync("ReceiveOrderUpdate", id, status);

//        // Additional check if all items are served
//        var order = orderDetail.Order;
//        if (status == OrderDetail.OrderItemStatus.Served.ToString())
//        {
//            var allItemsServed = !await _context.OrderDetails
//                .AnyAsync(od => od.OrderId == order.OrderId &&
//                               od.Status != OrderDetail.OrderItemStatus.Served.ToString());

//            if (allItemsServed)
//            {
//                order.Status = Order.OrderStatus.Completed.ToString();
//                await _context.SaveChangesAsync();
//                await _hubContext.Clients.Group("KitchenGroup")
//                    .SendAsync("OrderCompleted", order.OrderId);
//            }
//        }

//        return Ok();
//    }
//}