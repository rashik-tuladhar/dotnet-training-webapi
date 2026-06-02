using DlmsWebApi.Business.CategoryBusiness;
using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.CategoryData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : Controller
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
            bool isAdded = await _categoryBusiness.Add(category);
            if (isAdded)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to add author");
            }
        }

        [HttpGet]
        [Route("get-category-details")]
        public async Task<IActionResult> Edit([FromQuery] string id)
        {
            var categoryId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
            var categoryDetails = await _categoryBusiness.GetDetails(categoryId);
            categoryDetails.CategoryIdString = EncryptionHelper.Encrypt(categoryDetails.CategoryId.ToString());
            return Ok(categoryDetails);
        }

        [HttpPatch]
        [Route("update-category-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string id)
        {
            var categoryId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
            var user = "admin";
            var isUpdated = await _categoryBusiness.UpdateStatus(categoryId, user);
            if (isUpdated)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update author status");
            }
        }



    }
}
