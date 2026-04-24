using Microsoft.AspNetCore.Mvc;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return SuccessResponse(orders, "Orders retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving orders: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomer(long customerId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
                return SuccessResponse(orders, $"Orders for customer {customerId} retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving customer orders: {ex.Message}");
            }
        }

        [HttpGet("status/{orderStatus}")]
        public async Task<IActionResult> GetOrdersByStatus(string orderStatus)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(orderStatus);
                return SuccessResponse(orders, $"Orders with status '{orderStatus}' retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving orders by status: {ex.Message}");
            }
        }

        [HttpGet("shopkeeper/{shopkeeperId}/dashboard-stats")]
        public async Task<IActionResult> GetShopkeeperDashboardStats(long shopkeeperId)
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

        [HttpGet("shopkeeper/{shopkeeperId}/orders")]
        public async Task<IActionResult> GetOrdersByShopkeeper(long shopkeeperId)
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

        [HttpPost("shopkeeper/{shopkeeperId}/orders/{orderId}/accept")]
        public async Task<IActionResult> AcceptShopkeeperOrder(long shopkeeperId, long orderId)
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

        [HttpPost("shopkeeper/{shopkeeperId}/orders/{orderId}/reject")]
        public async Task<IActionResult> RejectShopkeeperOrder(long shopkeeperId, long orderId)
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

        [HttpPost("shopkeeper/{shopkeeperId}/orders/{orderId}/status")]
        public async Task<IActionResult> ShopkeeperSetOrderStatus(long shopkeeperId, long orderId, [FromBody] ShopkeeperOrderStatusDto body)
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

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(long orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(order, "Order retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving the order: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid order data provided");

            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(orderCreateDto);
                return SuccessResponse(createdOrder, "Order created successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while creating the order: {ex.Message}");
            }
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(long orderId, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid order data provided");

            try
            {
                var updatedOrder = await _orderService.UpdateOrderAsync(orderId, orderUpdateDto);
                if (updatedOrder == null)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(updatedOrder, "Order updated successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while updating the order: {ex.Message}");
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(long orderId)
        {
            try
            {
                var deleted = await _orderService.DeleteOrderAsync(orderId);
                if (!deleted)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(true, "Order deleted successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while deleting the order: {ex.Message}");
            }
        }

        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(long orderId, [FromBody] string orderStatus)
        {
            if (string.IsNullOrEmpty(orderStatus))
                return ErrorResponse("Order status is required");

            try
            {
                var updated = await _orderService.UpdateOrderStatusAsync(orderId, orderStatus);
                if (!updated)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(true, $"Order status updated to '{orderStatus}' successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while updating order status: {ex.Message}");
            }
        }

        [HttpPatch("{orderId}/payment-status")]
        public async Task<IActionResult> UpdatePaymentStatus(long orderId, [FromBody] string paymentStatus)
        {
            if (string.IsNullOrEmpty(paymentStatus))
                return ErrorResponse("Payment status is required");

            try
            {
                var updated = await _orderService.UpdatePaymentStatusAsync(orderId, paymentStatus);
                if (!updated)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(true, $"Payment status updated to '{paymentStatus}' successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while updating payment status: {ex.Message}");
            }
        }

        [HttpPost("{orderId}/items")]
        public async Task<IActionResult> AddOrderItem(long orderId, [FromBody] OrderItemCreateDto orderItemCreateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid order item data provided");

            try
            {
                var updatedOrder = await _orderService.AddOrderItemAsync(orderId, orderItemCreateDto);
                if (updatedOrder == null)
                    return ErrorResponse($"Order with ID {orderId} not found.");

                return SuccessResponse(updatedOrder, "Order item added successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while adding order item: {ex.Message}");
            }
        }

        [HttpPut("{orderId}/items/{orderItemId}")]
        public async Task<IActionResult> UpdateOrderItem(long orderId, long orderItemId, [FromBody] OrderItemUpdateDto orderItemUpdateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid order item data provided");

            try
            {
                var updated = await _orderService.UpdateOrderItemAsync(orderId, orderItemId, orderItemUpdateDto);
                if (!updated)
                    return ErrorResponse($"Order item with ID {orderItemId} not found in order {orderId}.");

                return SuccessResponse(true, "Order item updated successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while updating order item: {ex.Message}");
            }
        }

        [HttpDelete("{orderId}/items/{orderItemId}")]
        public async Task<IActionResult> RemoveOrderItem(long orderId, long orderItemId)
        {
            try
            {
                var removed = await _orderService.RemoveOrderItemAsync(orderId, orderItemId);
                if (!removed)
                    return ErrorResponse($"Order item with ID {orderItemId} not found in order {orderId}.");

                return SuccessResponse(true, "Order item removed successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while removing order item: {ex.Message}");
            }
        }
    }
}
