using APPLICATION_BACKEND.DTOs;

namespace APPLICATION_BACKEND.Interfaces
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryResponseDto>> GetAllProductCategoriesAsync();
        Task<ProductCategoryResponseDto?> GetProductCategoryByIdAsync(long productCategoryId);
        Task<ProductCategoryResponseDto> CreateProductCategoryAsync(ProductCategoryCreateDto productCategoryCreateDto);
        Task<ProductCategoryResponseDto?> UpdateProductCategoryAsync(long productCategoryId, ProductCategoryUpdateDto productCategoryUpdateDto);
        Task<bool> DeleteProductCategoryAsync(long productCategoryId);
        Task<IEnumerable<ProductCategoryResponseDto>> GetActiveProductCategoriesAsync();
    }
}
