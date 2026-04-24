using Microsoft.EntityFrameworkCore;
using APPLICATION_BACKEND.Database;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly CampusConnectDbContext _context;

        public ProductCategoryService(CampusConnectDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductCategoryResponseDto>> GetAllProductCategoriesAsync()
        {
            var categories = await _context.ProductCategories.ToListAsync();
            return categories.Select(MapToProductCategoryResponseDto);
        }

        public async Task<ProductCategoryResponseDto?> GetProductCategoryByIdAsync(long productCategoryId)
        {
            var category = await _context.ProductCategories.FindAsync(productCategoryId);
            return category != null ? MapToProductCategoryResponseDto(category) : null;
        }

        public async Task<ProductCategoryResponseDto> CreateProductCategoryAsync(ProductCategoryCreateDto productCategoryCreateDto)
        {
            var category = new ProductCategory
            {
                Name = productCategoryCreateDto.Name,
                Description = productCategoryCreateDto.Description,
                IsActive = productCategoryCreateDto.IsActive,
                DateAdded = DateTime.UtcNow,
                DateUpdated = null
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return MapToProductCategoryResponseDto(category);
        }

        public async Task<ProductCategoryResponseDto?> UpdateProductCategoryAsync(long productCategoryId, ProductCategoryUpdateDto productCategoryUpdateDto)
        {
            var existingCategory = await _context.ProductCategories.FindAsync(productCategoryId);
            if (existingCategory == null)
                return null;

            existingCategory.Name = productCategoryUpdateDto.Name;
            
            if (productCategoryUpdateDto.Description != null)
                existingCategory.Description = productCategoryUpdateDto.Description;

            if (productCategoryUpdateDto.IsActive.HasValue)
                existingCategory.IsActive = productCategoryUpdateDto.IsActive.Value;

            existingCategory.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToProductCategoryResponseDto(existingCategory);
        }

        public async Task<bool> DeleteProductCategoryAsync(long productCategoryId)
        {
            var category = await _context.ProductCategories.FindAsync(productCategoryId);
            if (category == null)
                return false;

            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ProductCategoryResponseDto>> GetActiveProductCategoriesAsync()
        {
            var categories = await _context.ProductCategories
                .Where(c => c.IsActive)
                .ToListAsync();
            
            return categories.Select(MapToProductCategoryResponseDto);
        }

        private static ProductCategoryResponseDto MapToProductCategoryResponseDto(ProductCategory category)
        {
            return new ProductCategoryResponseDto
            {
                ProductCategoryId = category.ProductCategoryId,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                DateAdded = category.DateAdded,
                DateUpdated = category.DateUpdated
            };
        }
    }
}
