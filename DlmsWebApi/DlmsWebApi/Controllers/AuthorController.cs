using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Shared.AuthorData;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    [ApiController]
    [Route("api/author")]
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
            return Ok(authorList);
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

        //[HttpGet]
        //[Route("get-author-details")]
        //public async Task<IActionResult> Edit([FromQuery] string id)
        //{
        //    var authorId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
        //    var authorDetails = await _authorBusiness.GetDetails(authorId);
        //    authorDetails.AuthorIdString = EncryptionHelper.Encrypt(authorDetails.AuthorId.ToString());
        //    return Ok(authorDetails);
        //}



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
