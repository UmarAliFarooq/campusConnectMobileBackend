using APPLICATION_BACKEND.DTOs;

namespace APPLICATION_BACKEND.Interfaces
{
    public interface IProductCategoryItemService
    {
        Task<IEnumerable<ProductCategoryItemResponseDto>> GetAllProductCategoryItemsAsync();
        Task<ProductCategoryItemResponseDto?> GetProductCategoryItemByIdAsync(long productCategoryItemId);
        Task<ProductCategoryItemResponseDto> CreateProductCategoryItemAsync(ProductCategoryItemCreateDto productCategoryItemCreateDto);
        Task<ProductCategoryItemResponseDto?> UpdateProductCategoryItemAsync(long productCategoryItemId, ProductCategoryItemUpdateDto productCategoryItemUpdateDto);
        Task<bool> DeleteProductCategoryItemAsync(long productCategoryItemId);
        Task<IEnumerable<ProductCategoryItemResponseDto>> GetItemsByCategoryAsync(long productCategoryId);
        Task<IEnumerable<ProductCategoryItemResponseDto>> GetAvailableItemsAsync();
        Task<IEnumerable<ProductCategoryItemResponseDto>> GetAvailableItemsByCategoryAsync(long productCategoryId);
        Task<IEnumerable<ProductCategoryItemResponseDto>> GetItemsByShopkeeperAsync(long shopkeeperId);
    }
}
