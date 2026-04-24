using Microsoft.AspNetCore.Mvc;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Controllers
{
    [Route("api/[controller]")]
    public class ProductCategoryItemsController : BaseController
    {
        private readonly IProductCategoryItemService _productCategoryItemService;

        public ProductCategoryItemsController(IProductCategoryItemService productCategoryItemService)
        {
            _productCategoryItemService = productCategoryItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductCategoryItems()
        {
            try
            {
                var items = await _productCategoryItemService.GetAllProductCategoryItemsAsync();
                return SuccessResponse(items, "Product category items retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving product category items: {ex.Message}");
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableItems()
        {
            try
            {
                var items = await _productCategoryItemService.GetAvailableItemsAsync();
                return SuccessResponse(items, "Available product category items retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving available product category items: {ex.Message}");
            }
        }

        [HttpGet("category/{productCategoryId}")]
        public async Task<IActionResult> GetItemsByCategory(long productCategoryId)
        {
            try
            {
                var items = await _productCategoryItemService.GetItemsByCategoryAsync(productCategoryId);
                return SuccessResponse(items, $"Product category items for category {productCategoryId} retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving product category items: {ex.Message}");
            }
        }

        [HttpGet("category/{productCategoryId}/available")]
        public async Task<IActionResult> GetAvailableItemsByCategory(long productCategoryId)
        {
            try
            {
                var items = await _productCategoryItemService.GetAvailableItemsByCategoryAsync(productCategoryId);
                return SuccessResponse(items, $"Available product category items for category {productCategoryId} retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving available product category items: {ex.Message}");
            }
        }

        [HttpGet("shopkeeper/{shopkeeperId}")]
        public async Task<IActionResult> GetItemsByShopkeeper(long shopkeeperId)
        {
            try
            {
                var items = await _productCategoryItemService.GetItemsByShopkeeperAsync(shopkeeperId);
                return SuccessResponse(items, $"Product items for shopkeeper {shopkeeperId} retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving shopkeeper products: {ex.Message}");
            }
        }

        [HttpGet("{productCategoryItemId}")]
        public async Task<IActionResult> GetProductCategoryItemById(long productCategoryItemId)
        {
            try
            {
                var item = await _productCategoryItemService.GetProductCategoryItemByIdAsync(productCategoryItemId);
                if (item == null)
                    return ErrorResponse($"Product category item with ID {productCategoryItemId} not found.");

                return SuccessResponse(item, "Product category item retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving the product category item: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategoryItem([FromBody] ProductCategoryItemCreateDto productCategoryItemCreateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid product category item data provided");

            try
            {
                var createdItem = await _productCategoryItemService.CreateProductCategoryItemAsync(productCategoryItemCreateDto);
                return SuccessResponse(createdItem, "Product category item created successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while creating the product category item: {ex.Message}");
            }
        }

        [HttpPut("{productCategoryItemId}")]
        public async Task<IActionResult> UpdateProductCategoryItem(long productCategoryItemId, [FromBody] ProductCategoryItemUpdateDto productCategoryItemUpdateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid product category item data provided");

            try
            {
                var updatedItem = await _productCategoryItemService.UpdateProductCategoryItemAsync(productCategoryItemId, productCategoryItemUpdateDto);
                if (updatedItem == null)
                    return ErrorResponse($"Product category item with ID {productCategoryItemId} not found.");

                return SuccessResponse(updatedItem, "Product category item updated successfully");
            }
            catch (ArgumentException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while updating the product category item: {ex.Message}");
            }
        }

        [HttpDelete("{productCategoryItemId}")]
        public async Task<IActionResult> DeleteProductCategoryItem(long productCategoryItemId)
        {
            try
            {
                var deleted = await _productCategoryItemService.DeleteProductCategoryItemAsync(productCategoryItemId);
                if (!deleted)
                    return ErrorResponse($"Product category item with ID {productCategoryItemId} not found.");

                return SuccessResponse(true, "Product category item deleted successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while deleting the product category item: {ex.Message}");
            }
        }
    }
}
