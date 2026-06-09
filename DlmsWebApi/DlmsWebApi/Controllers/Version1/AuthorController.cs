using System.Text.Json;
using Asp.Versioning;
using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Caching;
using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Filters;
using DlmsWebApi.Shared;
using DlmsWebApi.Shared.AuthorData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Scalar.AspNetCore.Attributes;
using Microsoft.AspNetCore.RateLimiting;

namespace DlmsWebApi.Controllers.Version1
{
    //[ServiceFilter(typeof(BasicAuthFilter))]
    [Deprecated("This API version is deprecated. Please use v2.0 for new features and improvements.")]
    //[SecurityAuthentication("AuthorController")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/author")]
    public class AuthorController : ControllerBase
    {
        private static readonly Dictionary<string, (int Count, DateTime WindowStart)> _requestCounts = new();
        private static readonly object _rateLimitLock = new();

        private readonly IAuthorCacheInvalidator _authorCacheInvalidator;
        private readonly IAuthorBusiness _authorBusiness;

        public AuthorController(
            IAuthorBusiness authorBusiness,
            IAuthorCacheInvalidator authorCacheInvalidator)
        {
            _authorBusiness = authorBusiness;
            _authorCacheInvalidator = authorCacheInvalidator;
        }

        [HttpGet]
        [EnableRateLimiting("Author:TokenBucket")]
        [Route("get-author-list")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "api-version" })]
        [OutputCache(PolicyName = AuthorCacheKeys.AuthorListOutputCachePolicy)]
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
        [EnableRateLimiting("Author:SlidingWindow")]
        [Route("get-author-list-repository-pattern")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "api-version" })]
        [OutputCache(PolicyName = AuthorCacheKeys.AuthorListOutputCachePolicy)]
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
        [EnableRateLimiting("Author:FixedWindow")]
        [Route("get-author-list-paginated")]
        //[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize", "api-version" })]
        //[OutputCache(Duration = 30, VaryByQueryKeys = new[] { "page", "pageSize", "api-version" }, Tags = new[] { AuthorCacheKeys.AuthorTag })]
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
        [EnableRateLimiting("Author:Concurrency")]
        [Route("add-author")]
        public async Task<IActionResult> Add([FromBody] AuthorDetails author)
        {
            bool isAdded = await _authorBusiness.Add(author);
            if (isAdded)
            {
                await _authorCacheInvalidator.ClearAsync(HttpContext.RequestAborted);
                return Created();
            }
            else
            {
                return BadRequest("Failed to add author");
            }
        }

        [HttpGet]
        [EnableRateLimiting("Author:PerApiKeyFixedWindow")]
        [Route("get-author-details")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "id", "api-version" })]
        [OutputCache(PolicyName = AuthorCacheKeys.AuthorDetailsOutputCachePolicy)]
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
        [EnableRateLimiting("Author:Concurrency")]
        [Route("update-author-details")]
        public async Task<IActionResult> Edit([FromBody] AuthorDetails author)
        {
            author.AuthorId = Convert.ToInt32(EncryptionHelper.Decrypt(author.AuthorIdString));
            var details = await _authorBusiness.Edit(author);
            if (details)
            {
                await _authorCacheInvalidator.ClearAsync(HttpContext.RequestAborted);
                return Created();
            }
            else
            {
                return BadRequest("Failed to update author details");
            }
        }


        [HttpPatch]
        [EnableRateLimiting("Author:Concurrency")]
        [Route("update-author-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string id)
        {
            var authorId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
            var user = "admin";
            var isUpdated = await _authorBusiness.UpdateStatus(authorId, user);
            if (isUpdated)
            {
                await _authorCacheInvalidator.ClearAsync(HttpContext.RequestAborted);
                return Created();
            }
            else
            {
                return BadRequest("Failed to update author status");
            }
        }

        /// <summary>
        /// A very simple, manual rate limiter without using ASP.NET Core policies.
        /// Excellent for teaching how rate limiting works under the hood.
        /// </summary>
        [HttpGet]
        [Route("simple-manual-rate-limit")]
        public IActionResult SimpleManualRateLimit()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;
            var windowSize = TimeSpan.FromSeconds(30); // 30-second window
            var maxRequests = 3; // Allow 3 requests per window

            lock (_rateLimitLock)
            {
                if (_requestCounts.TryGetValue(ipAddress, out var info))
                {
                    if (now - info.WindowStart > windowSize)
                    {
                        // Window has expired, reset the counter for this IP
                        _requestCounts[ipAddress] = (1, now);
                    }
                    else if (info.Count >= maxRequests)
                    {
                        // Still within window and limit exceeded -> Reject
                        Response.Headers.RetryAfter = (windowSize - (now - info.WindowStart)).TotalSeconds.ToString("F0");
                        return StatusCode(StatusCodes.Status429TooManyRequests, "Too Many Requests. Please wait and try again.");
                    }
                    else
                    {
                        // Within window and under limit -> Increment
                        _requestCounts[ipAddress] = (info.Count + 1, info.WindowStart);
                    }
                }
                else
                {
                    // First request ever from this IP
                    _requestCounts[ipAddress] = (1, now);
                }
            }

            return Ok(new { Message = "Success! You are within the rate limit. You have used " + _requestCounts[ipAddress].Count + " out of " + maxRequests + " requests." });
        }
    }
}
