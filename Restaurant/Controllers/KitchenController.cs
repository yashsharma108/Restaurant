using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models.Orders;
using Restaurant.Hubs;
using Microsoft.AspNetCore.Authorization;
using Restaurant.Models.ViewModels; // Add this for KitchenOrderViewModel

[Authorize(Roles = "Chef,Manager")]
public class KitchenController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ILogger<KitchenController> _logger;

    public KitchenController(
        ApplicationDbContext context,
        IHubContext<OrderHub> hubContext,
        ILogger<KitchenController> logger)
    {
        _context = context;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<IActionResult> KitchenDisplay()
    {
        try
        {
            var orders = await _context.OrderDetails
                .Include(od => od.Order)
                    .ThenInclude(o => o.Table)
                .Include(od => od.MenuItem)
                .Where(od => od.Status != OrderDetail.OrderItemStatus.Served) // Enum comparison
                .OrderBy(od => od.Order.OrderDate)
                .Select(od => new KitchenOrderViewModel
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.Order.OrderId,
                    TableNumber = od.Order.Table.TableNumber,
                    MenuItemName = od.MenuItem.Name,
                    Quantity = od.Quantity,
                    SpecialInstructions = od.SpecialInstructions,
                    Status = od.Status, // Direct enum assignment
                    PreparationTime = od.MenuItem.PreparationTime,
                    OrderTime = od.Order.OrderDate
                })
                .AsNoTracking()
                .ToListAsync();

            return View(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading kitchen display");
            return StatusCode(500, "Error loading orders");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderDetail.OrderItemStatus status) // Change parameter type
    {
        try
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                    .ThenInclude(o => o.Table)
                .Include(od => od.MenuItem)
                .FirstOrDefaultAsync(od => od.OrderDetailId == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            orderDetail.Status = status; // Direct enum assignment
            orderDetail.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var update = new
            {
                OrderDetailId = orderDetail.OrderDetailId,
                OrderId = orderDetail.OrderId,
                NewStatus = status.ToString(), // Convert to string for client
                MenuItem = orderDetail.MenuItem.Name,
                TableNumber = orderDetail.Order.Table.TableNumber,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group("KitchenGroup")
                .SendAsync("OrderStatusUpdated", update);

            if (status == OrderDetail.OrderItemStatus.Served)
            {
                await CheckOrderCompletion(orderDetail.OrderId);
            }

            return Ok(update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating status for order detail {id}");
            return StatusCode(500, "Error updating status");
        }
    }

    private async Task CheckOrderCompletion(int orderId)
    {
        var pendingItems = await _context.OrderDetails
            .AnyAsync(od => od.OrderId == orderId &&
                          od.Status != OrderDetail.OrderItemStatus.Served); // Remove ToString()

        if (!pendingItems)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order != null)
            {
                order.Status = Order.OrderStatus.Completed; // Direct enum assignment
                order.CompletedTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await _hubContext.Clients.Group("KitchenGroup")
                    .SendAsync("OrderCompleted", new
                    {
                        OrderId = orderId,
                        TableNumber = order.Table.TableNumber,
                        CompletionTime = DateTime.UtcNow
                    });
            }
        }
    }
}