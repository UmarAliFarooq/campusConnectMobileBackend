using APPLICATION_BACKEND.DTOs;

namespace APPLICATION_BACKEND.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
        Task<OrderResponseDto?> GetOrderByIdAsync(long orderId);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(long customerId);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByStatusAsync(string orderStatus);
        Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto orderCreateDto);
        Task<OrderResponseDto?> UpdateOrderAsync(long orderId, OrderUpdateDto orderUpdateDto);
        Task<bool> DeleteOrderAsync(long orderId);
        Task<bool> UpdateOrderStatusAsync(long orderId, string orderStatus);
        Task<bool> UpdatePaymentStatusAsync(long orderId, string paymentStatus);
        Task<OrderResponseDto?> AddOrderItemAsync(long orderId, OrderItemCreateDto orderItemCreateDto);
        Task<bool> UpdateOrderItemAsync(long orderId, long orderItemId, OrderItemUpdateDto orderItemUpdateDto);
        Task<bool> RemoveOrderItemAsync(long orderId, long orderItemId);
        Task<ShopkeeperDashboardStatsDto> GetShopkeeperDashboardStatsAsync(long shopkeeperId);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByShopkeeperAsync(long shopkeeperId);
        Task<OrderResponseDto?> AcceptOrderAsync(long orderId, long shopkeeperId);
        Task<OrderResponseDto?> RejectOrderAsync(long orderId, long shopkeeperId);
        Task<OrderResponseDto?> ShopkeeperSetOrderStatusAsync(long orderId, long shopkeeperId, string nextStatus);
    }
}
