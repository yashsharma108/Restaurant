//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using Restaurant.Data;
//using Restaurant.Models.Orders;
//using Restaurant.Hubs;
//using Microsoft.AspNetCore.Authorization;

//[Authorize(Roles = "Chef,Manager")]
//public class KitchenController : Controller
//{
//    private readonly ApplicationDbContext _context;
//    private readonly IHubContext<OrderHub> _hubContext;
//    private readonly ILogger<KitchenController> _logger;

//    public KitchenController(
//        ApplicationDbContext context,
//        IHubContext<OrderHub> hubContext,
//        ILogger<KitchenController> logger)
//    {
//        _context = context;
//        _hubContext = hubContext;
//        _logger = logger;
//    }

//    public async Task<IActionResult> KitchenDisplay()
//    {
//        try
//        {
//            var orders = await _context.OrderDetails
//                .Include(od => od.Order)
//                    .ThenInclude(o => o.Table)
//                .Include(od => od.MenuItem)
//                .Where(od => od.Status != OrderDetail.OrderItemStatus.Served.ToString())
//                .OrderBy(od => od.Order.OrderDate)
//                .AsNoTracking()
//                .Select(od => new KitchenOrderViewModel
//                {
//                    OrderDetailId = od.OrderDetailId,
//                    OrderId = od.OrderId,
//                    TableNumber = od.Order.Table.TableNumber,
//                    MenuItemName = od.MenuItem.Name,
//                    Quantity = od.Quantity,
//                    SpecialInstructions = od.SpecialInstructions,
//                    Status = od.Status,
//                    PreparationTime = od.MenuItem.PreparationTime,
//                    OrderTime = od.Order.OrderDate
//                })
//                .ToListAsync();

//            return View(orders);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error loading kitchen display");
//            return StatusCode(500, "Error loading orders");
//        }
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> UpdateStatus(int id, string status)
//    {
//        if (!Enum.TryParse<OrderDetail.OrderItemStatus>(status, out var statusEnum))
//        {
//            return BadRequest("Invalid status value");
//        }

//        try
//        {
//            var orderDetail = await _context.OrderDetails
//                .Include(od => od.Order)
//                .Include(od => od.MenuItem)
//                .FirstOrDefaultAsync(od => od.OrderDetailId == id);

//            if (orderDetail == null)
//            {
//                return NotFound();
//            }

//            orderDetail.Status = statusEnum.ToString();
//            orderDetail.LastUpdated = DateTime.UtcNow;

//            await _context.SaveChangesAsync();

//            // Prepare real-time update payload
//            var update = new
//            {
//                OrderDetailId = orderDetail.OrderDetailId,
//                OrderId = orderDetail.OrderId,
//                NewStatus = status,
//                MenuItem = orderDetail.MenuItem.Name,
//                TableNumber = orderDetail.Order.Table.TableNumber,
//                Timestamp = DateTime.UtcNow
//            };

//            // Notify all kitchen displays
//            await _hubContext.Clients.Group("KitchenGroup")
//                .SendAsync("OrderStatusUpdated", update);

//            // Handle order completion if all items served
//            if (statusEnum == OrderDetail.OrderItemStatus.Served)
//            {
//                await CheckOrderCompletion(orderDetail.OrderId);
//            }

//            return Ok(update);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, $"Error updating status for order detail {id}");
//            return StatusCode(500, "Error updating status");
//        }
//    }

//    private async Task CheckOrderCompletion(int orderId)
//    {
//        var pendingItems = await _context.OrderDetails
//            .AnyAsync(od => od.OrderId == orderId &&
//                           od.Status != OrderDetail.OrderItemStatus.Served.ToString());

//        if (!pendingItems)
//        {
//            var order = await _context.Orders.FindAsync(orderId);
//            if (order != null)
//            {
//                order.Status = Order.OrderStatus.Completed.ToString();
//                order.CompletedTime = DateTime.UtcNow;
//                await _context.SaveChangesAsync();

//                await _hubContext.Clients.Group("KitchenGroup")
//                    .SendAsync("OrderCompleted", new
//                    {
//                        OrderId = orderId,
//                        TableNumber = order.Table.TableNumber,
//                        CompletionTime = DateTime.UtcNow
//                    });
//            }
//        }
//    }
//}