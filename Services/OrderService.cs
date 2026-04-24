using Microsoft.EntityFrameworkCore;
using APPLICATION_BACKEND.Database;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Services
{
    public class OrderService : IOrderService
    {
        private readonly CampusConnectDbContext _context;

        public OrderService(CampusConnectDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            var orderResponseDtos = new List<OrderResponseDto>();

            foreach (var order in orders)
            {
                var orderResponseDto = await MapToOrderResponseDto(order);
                orderResponseDtos.Add(orderResponseDto);
            }

            return orderResponseDtos;
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(long orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return null;

            return await MapToOrderResponseDto(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(long customerId)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            var orderResponseDtos = new List<OrderResponseDto>();
            foreach (var order in orders)
            {
                var orderResponseDto = await MapToOrderResponseDto(order);
                orderResponseDtos.Add(orderResponseDto);
            }

            return orderResponseDtos;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByStatusAsync(string orderStatus)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderStatus == orderStatus)
                .ToListAsync();

            var orderResponseDtos = new List<OrderResponseDto>();
            foreach (var order in orders)
            {
                var orderResponseDto = await MapToOrderResponseDto(order);
                orderResponseDtos.Add(orderResponseDto);
            }

            return orderResponseDtos;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto orderCreateDto)
        {
            // Validate customer exists
            var customerExists = await _context.Users.AnyAsync(u => u.UserId == orderCreateDto.CustomerId);
            if (!customerExists)
                throw new ArgumentException($"Customer with ID {orderCreateDto.CustomerId} not found.");

            // Validate shopkeeper if provided
            if (orderCreateDto.ShopkeeperId.HasValue)
            {
                var shopkeeperExists = await _context.Users.AnyAsync(u => u.UserId == orderCreateDto.ShopkeeperId.Value);
                if (!shopkeeperExists)
                    throw new ArgumentException($"Shopkeeper with ID {orderCreateDto.ShopkeeperId.Value} not found.");
            }

            // Validate deliverer if provided
            if (orderCreateDto.DelivererId.HasValue)
            {
                var delivererExists = await _context.Users.AnyAsync(u => u.UserId == orderCreateDto.DelivererId.Value);
                if (!delivererExists)
                    throw new ArgumentException($"Deliverer with ID {orderCreateDto.DelivererId.Value} not found.");
            }

            // Validate order items and product availability
            foreach (var item in orderCreateDto.OrderItems)
            {
                var productExists = await _context.ProductCategoryItems.AnyAsync(p => p.ProductCategoryItemId == item.ProductCategoryItemId && p.IsAvailable);
                if (!productExists)
                    throw new ArgumentException($"Product category item with ID {item.ProductCategoryItemId} not found or not available.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create order
                var order = new Order
                {
                    CustomerId = orderCreateDto.CustomerId,
                    ShopkeeperId = orderCreateDto.ShopkeeperId ?? 0,
                    DelivererId = orderCreateDto.DelivererId ?? 0,
                    OrderStatus = orderCreateDto.OrderStatus,
                    PaymentMethod = orderCreateDto.PaymentMethod,
                    PaymentStatus = orderCreateDto.PaymentStatus,
                    OrderPickupType = orderCreateDto.OrderPickupType,
                    Destination = orderCreateDto.Destination,
                    PickupPoint = orderCreateDto.PickupPoint,
                    EstimatedTimeMin = orderCreateDto.EstimatedTimeMin ?? 0,
                    SpecialNotes = orderCreateDto.SpecialNotes,
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order items
                foreach (var itemDto in orderCreateDto.OrderItems)
                {
                    var product = await _context.ProductCategoryItems.FindAsync(itemDto.ProductCategoryItemId);
                    if (product == null)
                        throw new ArgumentException($"Product category item with ID {itemDto.ProductCategoryItemId} not found.");

                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductCategoryItemId = itemDto.ProductCategoryItemId,
                        Quantity = itemDto.Quantity,
                        Discount = itemDto.Discount,
                        UnitPrice = product.Price
                    };

                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return complete order with items
                return await GetOrderByIdAsync(order.OrderId) ?? throw new Exception("Failed to retrieve created order.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderResponseDto?> UpdateOrderAsync(long orderId, OrderUpdateDto orderUpdateDto)
        {
            var existingOrder = await _context.Orders.FindAsync(orderId);
            if (existingOrder == null)
                return null;

            // Validate deliverer if provided
            if (orderUpdateDto.DelivererId.HasValue)
            {
                var delivererExists = await _context.Users.AnyAsync(u => u.UserId == orderUpdateDto.DelivererId.Value);
                if (!delivererExists)
                    throw new ArgumentException($"Deliverer with ID {orderUpdateDto.DelivererId.Value} not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Update order fields
                if (orderUpdateDto.DelivererId.HasValue)
                    existingOrder.DelivererId = orderUpdateDto.DelivererId.Value;

                if (!string.IsNullOrEmpty(orderUpdateDto.OrderStatus))
                    existingOrder.OrderStatus = orderUpdateDto.OrderStatus;

                if (!string.IsNullOrEmpty(orderUpdateDto.PaymentStatus))
                    existingOrder.PaymentStatus = orderUpdateDto.PaymentStatus;

                if (!string.IsNullOrEmpty(orderUpdateDto.OrderPickupType))
                    existingOrder.OrderPickupType = orderUpdateDto.OrderPickupType;

                if (!string.IsNullOrEmpty(orderUpdateDto.Destination))
                    existingOrder.Destination = orderUpdateDto.Destination;

                if (!string.IsNullOrEmpty(orderUpdateDto.PickupPoint))
                    existingOrder.PickupPoint = orderUpdateDto.PickupPoint;

                if (orderUpdateDto.EstimatedTimeMin.HasValue)
                    existingOrder.EstimatedTimeMin = orderUpdateDto.EstimatedTimeMin.Value;

                if (!string.IsNullOrEmpty(orderUpdateDto.SpecialNotes))
                    existingOrder.SpecialNotes = orderUpdateDto.SpecialNotes;

                // Update order items if provided
                if (orderUpdateDto.OrderItems != null)
                {
                    foreach (var itemUpdate in orderUpdateDto.OrderItems)
                    {
                        var existingItem = await _context.OrderItems
                            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.OrderItemId == itemUpdate.OrderItemId);
                        
                        if (existingItem != null)
                        {
                            if (itemUpdate.Quantity.HasValue)
                                existingItem.Quantity = itemUpdate.Quantity.Value;

                            if (itemUpdate.Discount.HasValue)
                                existingItem.Discount = itemUpdate.Discount.Value;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await MapToOrderResponseDto(existingOrder);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteOrderAsync(long orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            // Remove order items first
            var orderItems = await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            _context.OrderItems.RemoveRange(orderItems);

            // Remove order
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(long orderId, string orderStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.OrderStatus = orderStatus;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdatePaymentStatusAsync(long orderId, string paymentStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.PaymentStatus = paymentStatus;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<OrderResponseDto?> AddOrderItemAsync(long orderId, OrderItemCreateDto orderItemCreateDto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return null;

            var productExists = await _context.ProductCategoryItems.AnyAsync(p => p.ProductCategoryItemId == orderItemCreateDto.ProductCategoryItemId && p.IsAvailable);
            if (!productExists)
                throw new ArgumentException($"Product category item with ID {orderItemCreateDto.ProductCategoryItemId} not found or not available.");

            var product = await _context.ProductCategoryItems.FindAsync(orderItemCreateDto.ProductCategoryItemId);
            if (product == null)
                throw new ArgumentException($"Product category item with ID {orderItemCreateDto.ProductCategoryItemId} not found.");

            var orderItem = new OrderItem
            {
                OrderId = orderId,
                ProductCategoryItemId = orderItemCreateDto.ProductCategoryItemId,
                Quantity = orderItemCreateDto.Quantity,
                Discount = orderItemCreateDto.Discount,
                UnitPrice = product.Price
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return await GetOrderByIdAsync(orderId);
        }

        public async Task<bool> UpdateOrderItemAsync(long orderId, long orderItemId, OrderItemUpdateDto orderItemUpdateDto)
        {
            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.OrderItemId == orderItemId);

            if (orderItem == null)
                return false;

            if (orderItemUpdateDto.Quantity.HasValue)
                orderItem.Quantity = orderItemUpdateDto.Quantity.Value;

            if (orderItemUpdateDto.Discount.HasValue)
                orderItem.Discount = orderItemUpdateDto.Discount.Value;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveOrderItemAsync(long orderId, long orderItemId)
        {
            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.OrderItemId == orderItemId);

            if (orderItem == null)
                return false;

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<OrderResponseDto> MapToOrderResponseDto(Order order)
        {
            // Get order items manually
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToListAsync();

            var orderItemResponseDtos = new List<OrderItemResponseDto>();
            foreach (var item in orderItems)
            {
                var itemResponse = await MapToOrderItemResponseDto(item);
                orderItemResponseDtos.Add(itemResponse);
            }

            // Get user information manually
            var customer = await _context.Users.FindAsync(order.CustomerId);
            var shopkeeper = order.ShopkeeperId != 0 ? await _context.Users.FindAsync(order.ShopkeeperId) : null;
            var deliverer = order.DelivererId != 0 ? await _context.Users.FindAsync(order.DelivererId) : null;

            // Calculate totals
            int totalAmount = orderItemResponseDtos.Sum(oi => oi.TotalPrice);
            int discountAmount = orderItemResponseDtos.Sum(oi => oi.Discount * oi.Quantity);
            int finalAmount = totalAmount - discountAmount;

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
                ShopkeeperId = order.ShopkeeperId,
                ShopkeeperName = shopkeeper != null ? $"{shopkeeper.FirstName} {shopkeeper.LastName}" : null,
                DelivererId = order.DelivererId,
                DelivererName = deliverer != null ? $"{deliverer.FirstName} {deliverer.LastName}" : null,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                OrderPickupType = order.OrderPickupType,
                Destination = order.Destination,
                PickupPoint = order.PickupPoint,
                EstimatedTimeMin = order.EstimatedTimeMin,
                SpecialNotes = order.SpecialNotes,
                OrderItems = orderItemResponseDtos,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount
            };
        }

        private async Task<OrderItemResponseDto> MapToOrderItemResponseDto(OrderItem orderItem)
        {
            // Get product category item manually
            var product = await _context.ProductCategoryItems.FindAsync(orderItem.ProductCategoryItemId);

            var totalPrice = (orderItem.UnitPrice * orderItem.Quantity) - orderItem.Discount;

            return new OrderItemResponseDto
            {
                OrderItemId = orderItem.OrderItemId,
                OrderId = orderItem.OrderId,
                ProductCategoryItemId = orderItem.ProductCategoryItemId,
                ProductCategoryItemName = product?.Name ?? "Unknown",
                ProductCategoryItemPrice = product?.Price ?? 0,
                Quantity = (int)orderItem.Quantity,
                Discount = (int)orderItem.Discount,
                UnitPrice = (int)orderItem.UnitPrice,
                TotalPrice = (int)totalPrice
            };
        }
    }
}
