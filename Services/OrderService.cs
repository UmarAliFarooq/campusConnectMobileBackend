using System;
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

        public async Task<ShopkeeperDashboardStatsDto> GetShopkeeperDashboardStatsAsync(long shopkeeperId)
        {
            var shopkeeperExists = await _context.Users.AnyAsync(u =>
                u.UserId == shopkeeperId &&
                u.IsActive &&
                u.RoleName != null &&
                u.RoleName.ToUpper() == "SHOPKEEPER");
            if (!shopkeeperExists)
                throw new ArgumentException("Shopkeeper not found or inactive.");

            var orders = await _context.Orders
                .Where(o => o.ShopkeeperId == shopkeeperId)
                .ToListAsync();

            bool IsNewOrder(string? s) =>
                !string.IsNullOrWhiteSpace(s) && (
                    s.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                    s.Equals("New", StringComparison.OrdinalIgnoreCase) ||
                    s.Equals("PLACED", StringComparison.OrdinalIgnoreCase));

            bool IsTerminal(string? s) =>
                !string.IsNullOrWhiteSpace(s) && (
                    s.Equals("Delivered", StringComparison.OrdinalIgnoreCase) ||
                    s.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) ||
                    s.Equals("Completed", StringComparison.OrdinalIgnoreCase));

            int newOrders = orders.Count(o => IsNewOrder(o.OrderStatus));
            int processing = orders.Count(o =>
                !string.IsNullOrWhiteSpace(o.OrderStatus) &&
                !IsTerminal(o.OrderStatus) &&
                !IsNewOrder(o.OrderStatus));

            // Income: delivered / completed orders, or paid (each order counted once)
            var incomeOrderIds = orders
                .Where(o => !string.IsNullOrWhiteSpace(o.OrderStatus) &&
                            !o.OrderStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) &&
                            !o.OrderStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                .Where(o =>
                    o.OrderStatus.Equals("Delivered", StringComparison.OrdinalIgnoreCase) ||
                    o.OrderStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(o.PaymentStatus) &&
                     (o.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase) ||
                      o.PaymentStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))))
                .Select(o => o.OrderId)
                .Distinct()
                .ToList();

            int income = 0;
            if (incomeOrderIds.Count > 0)
            {
                income = await _context.OrderItems
                    .Where(oi => incomeOrderIds.Contains(oi.OrderId))
                    .SumAsync(oi =>
                        (oi.UnitPrice ?? 0) * (oi.Quantity ?? 0) - (oi.Discount ?? 0));
            }

            return new ShopkeeperDashboardStatsDto
            {
                NewOrdersCount = newOrders,
                ProcessingOrdersCount = processing,
                TotalIncome = income
            };
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByShopkeeperAsync(long shopkeeperId)
        {
            await EnsureShopkeeperExistsAsync(shopkeeperId);

            var orderIds = await _context.Orders
                .Where(o => o.ShopkeeperId == shopkeeperId)
                .OrderByDescending(o => o.OrderId)
                .Select(o => o.OrderId)
                .ToListAsync();

            var list = new List<OrderResponseDto>();
            foreach (var id in orderIds)
            {
                var dto = await GetOrderByIdAsync(id);
                if (dto != null)
                    list.Add(dto);
            }

            return list;
        }

        public async Task<OrderResponseDto?> AcceptOrderAsync(long orderId, long shopkeeperId)
        {
            await EnsureShopkeeperExistsAsync(shopkeeperId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null || order.ShopkeeperId != shopkeeperId)
                    return null;

                if (!IsNewOrderStatus(order.OrderStatus))
                    throw new ArgumentException("Only new / pending orders can be accepted.");

                var lines = await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
                foreach (var oi in lines)
                {
                    var product = await _context.ProductCategoryItems.FindAsync(oi.ProductCategoryItemId);
                    if (product == null)
                        throw new ArgumentException($"Product {oi.ProductCategoryItemId} not found.");

                    var need = (int)(oi.Quantity ?? 0);
                    if (need <= 0)
                        continue;

                    if (product.Quantity < need)
                        throw new ArgumentException($"Insufficient stock for \"{product.Name}\". Have {product.Quantity}, need {need}.");

                    product.Quantity -= need;
                    product.DateUpdated = DateTime.UtcNow;
                }

                order.OrderStatus = "Processing";
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetOrderByIdAsync(orderId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderResponseDto?> RejectOrderAsync(long orderId, long shopkeeperId)
        {
            await EnsureShopkeeperExistsAsync(shopkeeperId);

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.ShopkeeperId != shopkeeperId)
                return null;

            if (!IsNewOrderStatus(order.OrderStatus))
                throw new ArgumentException("Only new / pending orders can be rejected.");

            order.OrderStatus = "Cancelled";
            await _context.SaveChangesAsync();

            return await GetOrderByIdAsync(orderId);
        }

        public async Task<OrderResponseDto?> ShopkeeperSetOrderStatusAsync(long orderId, long shopkeeperId, string nextStatus)
        {
            await EnsureShopkeeperExistsAsync(shopkeeperId);

            if (string.IsNullOrWhiteSpace(nextStatus))
                throw new ArgumentException("Next status is required.");

            var normalized = nextStatus.Trim();
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.ShopkeeperId != shopkeeperId)
                return null;

            var current = order.OrderStatus ?? string.Empty;

            // Allowed: Processing -> ReadyForDelivery; ReadyForDelivery -> Delivered; Processing -> Delivered
            if (IsNewOrderStatus(current))
                throw new ArgumentException("Accept the order first before changing kitchen status.");

            if (current.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) ||
                current.Equals("Delivered", StringComparison.OrdinalIgnoreCase) ||
                current.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("This order cannot be updated.");

            var next = NormalizeShopkeeperStatus(normalized);

            if (next.Equals("ReadyForDelivery", StringComparison.OrdinalIgnoreCase))
            {
                if (!current.Equals("Processing", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Can only move to Ready for delivery from Processing.");
                order.OrderStatus = "ReadyForDelivery";
            }
            else if (next.Equals("Delivered", StringComparison.OrdinalIgnoreCase) ||
                     next.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                if (!current.Equals("Processing", StringComparison.OrdinalIgnoreCase) &&
                    !current.Equals("ReadyForDelivery", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Invalid transition to delivered.");

                order.OrderStatus = "Delivered";
                order.PaymentStatus = "Paid";
            }
            else
                throw new ArgumentException($"Unsupported status: {normalized}");

            await _context.SaveChangesAsync();
            return await GetOrderByIdAsync(orderId);
        }

        private static string NormalizeShopkeeperStatus(string s)
        {
            var t = s.Trim();
            if (t.Equals("Ready for delivery", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("ReadyForPickup", StringComparison.OrdinalIgnoreCase))
                return "ReadyForDelivery";
            if (t.Equals("Complete", StringComparison.OrdinalIgnoreCase))
                return "Delivered";
            return t;
        }

        private static bool IsNewOrderStatus(string? s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            return s.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("New", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("PLACED", StringComparison.OrdinalIgnoreCase);
        }

        private async Task EnsureShopkeeperExistsAsync(long shopkeeperId)
        {
            var ok = await _context.Users.AnyAsync(u =>
                u.UserId == shopkeeperId &&
                u.IsActive &&
                u.RoleName != null &&
                u.RoleName.ToUpper() == "SHOPKEEPER");
            if (!ok)
                throw new ArgumentException("Shopkeeper not found or inactive.");
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

            var qty = orderItem.Quantity ?? 0;
            var unit = orderItem.UnitPrice ?? 0;
            var disc = orderItem.Discount ?? 0;
            var totalPrice = unit * qty - disc;

            return new OrderItemResponseDto
            {
                OrderItemId = orderItem.OrderItemId,
                OrderId = orderItem.OrderId,
                ProductCategoryItemId = orderItem.ProductCategoryItemId,
                ProductCategoryItemName = product?.Name ?? "Unknown",
                ProductCategoryItemPrice = product?.Price ?? 0,
                Quantity = qty,
                Discount = disc,
                UnitPrice = unit,
                TotalPrice = totalPrice
            };
        }
    }
}
