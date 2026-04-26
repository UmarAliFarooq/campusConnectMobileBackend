using Microsoft.AspNetCore.Mvc;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Controllers
{
    /// <summary>
    /// Deliverer-only endpoints (pool, accept, complete) under a dedicated prefix.
    /// </summary>
    [Route("api/deliverer")]
    [ApiController]
    public class DelivererPortalController : BaseController
    {
        private readonly IOrderService _orderService;

        public DelivererPortalController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{delivererId:long}/dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats(long delivererId)
        {
            try
            {
                var stats = await _orderService.GetDelivererDashboardStatsAsync(delivererId);
                return SuccessResponse(stats, "Deliverer dashboard statistics retrieved successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving deliverer statistics: {ex.Message}");
            }
        }

        /// <summary>
        /// Global delivery pool: pickup type Delivery, status ReadyForDelivery, no rider assigned (DelivererId null/0).
        /// Does not take delivererId — the list is the same for every rider.
        /// </summary>
        [HttpGet("open-delivery-pool")]
        public async Task<IActionResult> GetOpenDeliveryPool()
        {
            try
            {
                var orders = await _orderService.GetOpenDeliveryPoolAsync();
                return SuccessResponse(orders, "Unassigned delivery orders retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving the delivery pool: {ex.Message}");
            }
        }

        private async Task<IActionResult> UnassignedDeliveryPoolAsync(long delivererId)
        {
            try
            {
                var orders = await _orderService.GetUnassignedDeliveryPoolAsync(delivererId);
                return SuccessResponse(orders, "Unassigned delivery orders retrieved successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving the delivery pool: {ex.Message}");
            }
        }

        [HttpGet("{delivererId:long}/delivery-pool")]
        public Task<IActionResult> GetDeliveryPool(long delivererId) => UnassignedDeliveryPoolAsync(delivererId);

        /// <summary>Same payload as open-delivery-pool; delivererId only validates the rider account exists.</summary>
        [HttpGet("{delivererId:long}/new-orders")]
        public Task<IActionResult> GetNewOrdersPool(long delivererId) => UnassignedDeliveryPoolAsync(delivererId);

        [HttpGet("{delivererId:long}/active-orders")]
        public async Task<IActionResult> GetActiveOrders(long delivererId)
        {
            try
            {
                var orders = await _orderService.GetDelivererActiveOrdersAsync(delivererId);
                return SuccessResponse(orders, "Active delivery orders retrieved successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving active orders: {ex.Message}");
            }
        }

        [HttpPost("{delivererId:long}/orders/{orderId:long}/accept")]
        public async Task<IActionResult> AcceptDelivery(long delivererId, long orderId)
        {
            try
            {
                var updated = await _orderService.DelivererAcceptOrderAsync(orderId, delivererId);
                if (updated == null)
                    return ErrorResponse("Order is no longer available or is not in the delivery pool.");

                return SuccessResponse(updated, "Order accepted — you are now assigned for this delivery.");
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

        /// <summary>
        /// Rider confirms they paid the shopkeeper the food amount at pickup; order moves to EnRouteToCustomer.
        /// </summary>
        [HttpPost("{delivererId:long}/orders/{orderId:long}/confirm-shopkeeper-cash")]
        public async Task<IActionResult> ConfirmShopkeeperCash(long delivererId, long orderId)
        {
            try
            {
                var updated = await _orderService.DelivererConfirmShopkeeperCashAsync(orderId, delivererId);
                if (updated == null)
                    return ErrorResponse($"Order with ID {orderId} not found or not assigned to you.");

                return SuccessResponse(updated, "Shopkeeper cash recorded — continue to the customer.");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while confirming shopkeeper payment: {ex.Message}");
            }
        }

        [HttpPost("{delivererId:long}/orders/{orderId:long}/complete-delivery")]
        public async Task<IActionResult> CompleteDelivery(long delivererId, long orderId)
        {
            try
            {
                var updated = await _orderService.DelivererCompleteDeliveryAsync(orderId, delivererId);
                if (updated == null)
                    return ErrorResponse($"Order with ID {orderId} not found or not assigned to you.");

                return SuccessResponse(updated, "Delivery marked as completed.");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while completing delivery: {ex.Message}");
            }
        }
    }
}
