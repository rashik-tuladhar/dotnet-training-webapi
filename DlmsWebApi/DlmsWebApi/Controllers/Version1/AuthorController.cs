using System.Text.Json;
using Asp.Versioning;
using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Filters;
using DlmsWebApi.Shared;
using DlmsWebApi.Shared.AuthorData;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore.Attributes;

namespace DlmsWebApi.Controllers.Version1
{
    //[ServiceFilter(typeof(BasicAuthFilter))]
    [Deprecated("This API version is deprecated. Please use v2.0 for new features and improvements.")]
    [SecurityAuthentication("AuthorController")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/author")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorBusiness _authorBusiness;

        public AuthorController(IAuthorBusiness authorBusiness)
        {
            _authorBusiness = authorBusiness;
        }

        [HttpGet]
        [Route("get-author-list")]
        public async Task<IActionResult> GetList()
        {
            var authorList = await _authorBusiness.GetList();
            // ApiResponse<List<AuthorDetails>> response = new ApiResponse<List<AuthorDetails>>();
            // response.Success = true;
            // response.Message = "Success";
            // response.Data = authorList;
            // return Ok(response);
            
            return Ok(ApiResponse<List<AuthorDetails>>.SuccessMessage(authorList,"this is success message"));
            
            
        }
        
        [HttpGet]
        [Route("get-author-list-repository-pattern")]
        public async Task<IActionResult> GetListRepositoryPattern()
        {
            var authorList = await _authorBusiness.GetListRepositoryPattern();
            return Ok(ApiResponse<List<AuthorDetails>>.SuccessMessage(authorList,"this is success message"));
        }
        
        /// <summary>
        /// Pagination Response with generic response
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-author-list-paginated")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination,
            CancellationToken ct)
        {
            var result = await _authorBusiness.GetListPaginated(pagination, ct);

            // Add pagination metadata to response headers
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                result.Data.TotalCount,
                result.Data.TotalPages,
                result.Data.HasPreviousPage,
                result.Data.HasNextPage
            }));

            return Ok(result);
        }

        [HttpPost]
        [Route("add-author")]
        public async Task<IActionResult> Add([FromBody] AuthorDetails author)
        {
            bool isAdded = await _authorBusiness.Add(author);
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
        [Route("get-author-details")]
        public async Task<IActionResult> Edit([FromQuery] string id)
        {
            ApiResponse<AuthorDetails> response = new ApiResponse<AuthorDetails>();
            try
            {
                var authorId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
                var authorDetails = await _authorBusiness.GetDetails(authorId);
                authorDetails.AuthorIdString = EncryptionHelper.Encrypt(authorDetails.AuthorId.ToString());
                response.Success = true;
                response.Message = "Success";
                response.Data = authorDetails;
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Failed to get author details";
                response.Data = null;
                List<string> errors = new List<string>();
                errors.Add(e.Message);
                response.Errors = errors;
                return BadRequest(response);
            }
            
        }

        [HttpPut]
        [Route("update-author-details")]
        public async Task<IActionResult> Edit([FromBody] AuthorDetails author)
        {
            author.AuthorId = Convert.ToInt32(EncryptionHelper.Decrypt(author.AuthorIdString));
            var details = await _authorBusiness.Edit(author);
            if (details)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update author details");
            }
        }


        [HttpPatch]
        [Route("update-author-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string id)
        {
            var authorId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
            var user = "admin";
            var isUpdated = await _authorBusiness.UpdateStatus(authorId, user);
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
