using Microsoft.AspNetCore.Mvc;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Controllers
{
    /// <summary>
    /// Shopkeeper-only endpoints under a dedicated prefix so they never collide with
    /// <c>GET /api/Orders/{orderId}</c> or other variable routes on <see cref="OrdersController"/>.
    /// </summary>
    [Route("api/shopkeeper")]
    [ApiController]
    public class ShopkeeperPortalController : BaseController
    {
        private readonly IOrderService _orderService;

        public ShopkeeperPortalController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{shopkeeperId:long}/dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats(long shopkeeperId)
        {
            try
            {
                var stats = await _orderService.GetShopkeeperDashboardStatsAsync(shopkeeperId);
                return SuccessResponse(stats, "Dashboard statistics retrieved successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving dashboard statistics: {ex.Message}");
            }
        }

        [HttpGet("{shopkeeperId:long}/orders")]
        public async Task<IActionResult> GetOrders(long shopkeeperId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByShopkeeperAsync(shopkeeperId);
                return SuccessResponse(orders, "Shopkeeper orders retrieved successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving shopkeeper orders: {ex.Message}");
            }
        }

        [HttpPost("{shopkeeperId:long}/orders/{orderId:long}/accept")]
        public async Task<IActionResult> AcceptOrder(long shopkeeperId, long orderId)
        {
            try
            {
                var updated = await _orderService.AcceptOrderAsync(orderId, shopkeeperId);
                if (updated == null)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(updated, "Order accepted and moved to processing");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while accepting the order: {ex.Message}");
            }
        }

        [HttpPost("{shopkeeperId:long}/orders/{orderId:long}/reject")]
        public async Task<IActionResult> RejectOrder(long shopkeeperId, long orderId)
        {
            try
            {
                var updated = await _orderService.RejectOrderAsync(orderId, shopkeeperId);
                if (updated == null)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(updated, "Order rejected");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while rejecting the order: {ex.Message}");
            }
        }

        [HttpPost("{shopkeeperId:long}/orders/{orderId:long}/status")]
        public async Task<IActionResult> SetOrderStatus(long shopkeeperId, long orderId, [FromBody] ShopkeeperOrderStatusDto? body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.NextStatus))
                return ErrorResponse("NextStatus is required");

            try
            {
                var updated = await _orderService.ShopkeeperSetOrderStatusAsync(orderId, shopkeeperId, body.NextStatus);
                if (updated == null)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(updated, "Order status updated");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while updating order status: {ex.Message}");
            }
        }
    }
}
