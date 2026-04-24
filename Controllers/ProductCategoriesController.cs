using Microsoft.AspNetCore.Mvc;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Controllers
{
    [Route("api/[controller]")]
    public class ProductCategoriesController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoriesController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductCategories()
        {
            try
            {
                var categories = await _productCategoryService.GetAllProductCategoriesAsync();
                return SuccessResponse(categories, "Product categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving product categories: {ex.Message}");
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProductCategories()
        {
            try
            {
                var categories = await _productCategoryService.GetActiveProductCategoriesAsync();
                return SuccessResponse(categories, "Active product categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving active product categories: {ex.Message}");
            }
        }

        [HttpGet("{productCategoryId}")]
        public async Task<IActionResult> GetProductCategoryById(long productCategoryId)
        {
            try
            {
                var category = await _productCategoryService.GetProductCategoryByIdAsync(productCategoryId);
                if (category == null)
                    return ErrorResponse($"Product category with ID {productCategoryId} not found.");

                return SuccessResponse(category, "Product category retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving the product category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategory([FromBody] ProductCategoryCreateDto productCategoryCreateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid product category data provided");

            try
            {
                var createdCategory = await _productCategoryService.CreateProductCategoryAsync(productCategoryCreateDto);
                return SuccessResponse(createdCategory, "Product category created successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while creating the product category: {ex.Message}");
            }
        }

        [HttpPut("{productCategoryId}")]
        public async Task<IActionResult> UpdateProductCategory(long productCategoryId, [FromBody] ProductCategoryUpdateDto productCategoryUpdateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid product category data provided");

            try
            {
                var updatedCategory = await _productCategoryService.UpdateProductCategoryAsync(productCategoryId, productCategoryUpdateDto);
                if (updatedCategory == null)
                    return ErrorResponse($"Product category with ID {productCategoryId} not found.");

                return SuccessResponse(updatedCategory, "Product category updated successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while updating the product category: {ex.Message}");
            }
        }

        [HttpDelete("{productCategoryId}")]
        public async Task<IActionResult> DeleteProductCategory(long productCategoryId)
        {
            try
            {
                var deleted = await _productCategoryService.DeleteProductCategoryAsync(productCategoryId);
                if (!deleted)
                    return ErrorResponse($"Product category with ID {productCategoryId} not found.");

                return SuccessResponse(true, "Product category deleted successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while deleting the product category: {ex.Message}");
            }
        }
    }
}
