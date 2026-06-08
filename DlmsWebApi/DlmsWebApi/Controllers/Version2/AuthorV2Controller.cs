using Asp.Versioning;
using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Filters;
using DlmsWebApi.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers.Version2
{
    [SecurityAuthentication("AuthorController")]
    [ApiVersion("2.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/author")]
    public class AuthorV2Controller : ControllerBase
    {
        private readonly IAuthorBusiness _authorBusiness;

        public AuthorV2Controller(IAuthorBusiness authorBusiness)
        {
            _authorBusiness = authorBusiness;
        }

        [HttpGet]
        [Route("get-author-list")]
        public async Task<IActionResult> GetList()
        {
            var authorList = await _authorBusiness.GetList();

            var v2List = authorList.Select(author => new AuthorDetailsV2
            {
                AuthorIdString = author.AuthorIdString,
                FullName = string.IsNullOrWhiteSpace(author.MiddleName)
                    ? $"{author.FirstName} {author.LastName}"
                    : $"{author.FirstName} {author.MiddleName} {author.LastName}",
                Bio = author.Bio,
                DateOfBirth = author.DateOfBirth,
                Status = author.Status
            }).ToList();

            return Ok(ApiResponse<List<AuthorDetailsV2>>.SuccessMessage(v2List, "Success (V2 API)"));
        }
    }

    public class AuthorDetailsV2
    {
        public string? AuthorIdString { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Status { get; set; }
    }
}
