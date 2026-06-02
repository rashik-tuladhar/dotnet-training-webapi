using DlmsWebApi.Business.CategoryBusiness;
using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Shared.CategoryData;
using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryBusiness _categoryBusiness;

        public CategoryController(ICategoryBusiness categoryBusiness)
        {
            _categoryBusiness = categoryBusiness;
        }

        [HttpGet]
        [Route("get-category-list")]
        public async Task<IActionResult> GetList()
        {
            var categoryList = await _categoryBusiness.GetList();
            return Ok(categoryList);
        }

        [HttpPost]
        [Route("add-category")]
        public async Task<IActionResult> Add([FromBody] CategoryDetails category)
        {
            category.User ??= "admin";

            var isAdded = await _categoryBusiness.Add(category);
            if (!isAdded)
            {
                return BadRequest("Failed to add category");
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        [Route("get-category-details")]
        public async Task<IActionResult> GetDetails([FromQuery] string id)
        {
            if (!TryReadCategoryId(id, out var categoryId))
            {
                return BadRequest("Invalid category id");
            }

            var categoryDetails = await _categoryBusiness.GetDetails(categoryId);
            if (categoryDetails == null)
            {
                return NotFound("Category not found");
            }

            return Ok(categoryDetails);
        }

        [HttpPut]
        [Route("update-category-details")]
        public async Task<IActionResult> Edit([FromBody] CategoryDetails category)
        {
            if (!string.IsNullOrWhiteSpace(category.CategoryIdString))
            {
                if (!TryReadCategoryId(category.CategoryIdString, out var categoryId))
                {
                    return BadRequest("Invalid category id");
                }

                category.CategoryId = categoryId;
            }

            if (category.CategoryId <= 0)
            {
                return BadRequest("Category id is required");
            }

            category.User ??= "admin";

            var isUpdated = await _categoryBusiness.Edit(category);
            if (!isUpdated)
            {
                return BadRequest("Failed to update category details");
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPatch]
        [Route("update-category-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string id)
        {
            if (!TryReadCategoryId(id, out var categoryId))
            {
                return BadRequest("Invalid category id");
            }

            var isUpdated = await _categoryBusiness.UpdateStatus(categoryId, "admin");
            if (!isUpdated)
            {
                return BadRequest("Failed to update category status");
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        private static bool TryReadCategoryId(string id, out int categoryId)
        {
            categoryId = 0;

            if (int.TryParse(id, out categoryId))
            {
                return categoryId > 0;
            }

            var decryptedId = EncryptionHelper.Decrypt(id);
            return int.TryParse(decryptedId, out categoryId) && categoryId > 0;
        }
    }
}
