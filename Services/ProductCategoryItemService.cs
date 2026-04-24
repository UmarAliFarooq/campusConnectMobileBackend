using Microsoft.EntityFrameworkCore;
using APPLICATION_BACKEND.Database;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Services
{
    public class ProductCategoryItemService : IProductCategoryItemService
    {
        private readonly CampusConnectDbContext _context;

        public ProductCategoryItemService(CampusConnectDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductCategoryItemResponseDto>> GetAllProductCategoryItemsAsync()
        {
            var items = await _context.ProductCategoryItems.ToListAsync();
            var responseDtos = new List<ProductCategoryItemResponseDto>();

            foreach (var item in items)
            {
                var responseDto = await MapToProductCategoryItemResponseDto(item);
                responseDtos.Add(responseDto);
            }

            return responseDtos;
        }

        public async Task<ProductCategoryItemResponseDto?> GetProductCategoryItemByIdAsync(long productCategoryItemId)
        {
            var item = await _context.ProductCategoryItems.FindAsync(productCategoryItemId);
            return item != null ? await MapToProductCategoryItemResponseDto(item) : null;
        }

        public async Task<ProductCategoryItemResponseDto> CreateProductCategoryItemAsync(ProductCategoryItemCreateDto productCategoryItemCreateDto)
        {
            // Verify if the product category exists
            var categoryExists = await _context.ProductCategories.AnyAsync(c => c.ProductCategoryId == productCategoryItemCreateDto.ProductCategoryId);
            if (!categoryExists)
                throw new ArgumentException($"Product category with ID {productCategoryItemCreateDto.ProductCategoryId} not found.");

            var item = new ProductCategoryItem
            {
                Name = productCategoryItemCreateDto.Name,
                Price = productCategoryItemCreateDto.Price,
                Quantity = productCategoryItemCreateDto.Quantity,
                PreperationTimeMinutes = productCategoryItemCreateDto.PreperationTimeMinutes,
                IsAvailable = productCategoryItemCreateDto.IsAvailable,
                ImageUrl = productCategoryItemCreateDto.ImageUrl,
                ProductCategoryId = productCategoryItemCreateDto.ProductCategoryId,
                DateAdded = DateTime.UtcNow,
                DateUpdated = null
            };

            _context.ProductCategoryItems.Add(item);
            await _context.SaveChangesAsync();

            return await MapToProductCategoryItemResponseDto(item);
        }

        public async Task<ProductCategoryItemResponseDto?> UpdateProductCategoryItemAsync(long productCategoryItemId, ProductCategoryItemUpdateDto productCategoryItemUpdateDto)
        {
            var existingItem = await _context.ProductCategoryItems.FindAsync(productCategoryItemId);
            if (existingItem == null)
                return null;

            // If ProductCategoryId is being updated, verify it exists
            if (productCategoryItemUpdateDto.ProductCategoryId.HasValue)
            {
                var categoryExists = await _context.ProductCategories.AnyAsync(c => c.ProductCategoryId == productCategoryItemUpdateDto.ProductCategoryId.Value);
                if (!categoryExists)
                    throw new ArgumentException($"Product category with ID {productCategoryItemUpdateDto.ProductCategoryId.Value} not found.");
            }

            existingItem.Name = productCategoryItemUpdateDto.Name;

            if (productCategoryItemUpdateDto.Price.HasValue)
                existingItem.Price = productCategoryItemUpdateDto.Price.Value;

            if (productCategoryItemUpdateDto.Quantity.HasValue)
                existingItem.Quantity = productCategoryItemUpdateDto.Quantity.Value;

            if (productCategoryItemUpdateDto.PreperationTimeMinutes.HasValue)
                existingItem.PreperationTimeMinutes = productCategoryItemUpdateDto.PreperationTimeMinutes.Value;

            if (productCategoryItemUpdateDto.IsAvailable.HasValue)
                existingItem.IsAvailable = productCategoryItemUpdateDto.IsAvailable.Value;

            if (productCategoryItemUpdateDto.ImageUrl != null)
                existingItem.ImageUrl = productCategoryItemUpdateDto.ImageUrl;

            if (productCategoryItemUpdateDto.ProductCategoryId.HasValue)
                existingItem.ProductCategoryId = productCategoryItemUpdateDto.ProductCategoryId.Value;

            existingItem.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await MapToProductCategoryItemResponseDto(existingItem);
        }

        public async Task<bool> DeleteProductCategoryItemAsync(long productCategoryItemId)
        {
            var item = await _context.ProductCategoryItems.FindAsync(productCategoryItemId);
            if (item == null)
                return false;

            _context.ProductCategoryItems.Remove(item);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ProductCategoryItemResponseDto>> GetItemsByCategoryAsync(long productCategoryId)
        {
            var items = await _context.ProductCategoryItems
                .Where(item => item.ProductCategoryId == productCategoryId)
                .ToListAsync();
            
            var responseDtos = new List<ProductCategoryItemResponseDto>();
            foreach (var item in items)
            {
                var responseDto = await MapToProductCategoryItemResponseDto(item);
                responseDtos.Add(responseDto);
            }

            return responseDtos;
        }

        public async Task<IEnumerable<ProductCategoryItemResponseDto>> GetAvailableItemsAsync()
        {
            var items = await _context.ProductCategoryItems
                .Where(item => item.IsAvailable)
                .ToListAsync();
            
            var responseDtos = new List<ProductCategoryItemResponseDto>();
            foreach (var item in items)
            {
                var responseDto = await MapToProductCategoryItemResponseDto(item);
                responseDtos.Add(responseDto);
            }

            return responseDtos;
        }

        public async Task<IEnumerable<ProductCategoryItemResponseDto>> GetAvailableItemsByCategoryAsync(long productCategoryId)
        {
            var items = await _context.ProductCategoryItems
                .Where(item => item.ProductCategoryId == productCategoryId && item.IsAvailable)
                .ToListAsync();
            
            var responseDtos = new List<ProductCategoryItemResponseDto>();
            foreach (var item in items)
            {
                var responseDto = await MapToProductCategoryItemResponseDto(item);
                responseDtos.Add(responseDto);
            }

            return responseDtos;
        }

        private async Task<ProductCategoryItemResponseDto> MapToProductCategoryItemResponseDto(ProductCategoryItem item)
        {
            // Get category information manually
            var category = await _context.ProductCategories.FindAsync(item.ProductCategoryId);

            return new ProductCategoryItemResponseDto
            {
                ProductCategoryItemId = item.ProductCategoryItemId,
                Name = item.Name,
                Price = (decimal)item.Price,
                Quantity = item.Quantity,
                PreperationTimeMinutes = item.PreperationTimeMinutes,
                IsAvailable = item.IsAvailable,
                ImageUrl = item.ImageUrl,
                ProductCategoryId = item.ProductCategoryId,
                CategoryName = category?.Name ?? "Unknown",
                DateAdded = item.DateAdded,
                DateUpdated = item.DateUpdated
            };
        }
    }
}
