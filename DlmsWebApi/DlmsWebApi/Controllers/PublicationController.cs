using DlmsWebApi.Business.PublicationBusiness;
using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Shared.PublicationData;
using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers
{
    [ApiController]
    [Route("api/publication")]
    public class PublicationController : ControllerBase
    {
        private readonly IPublicationBusiness _publicationBusiness;

        public PublicationController(IPublicationBusiness publicationBusiness)
        {
            _publicationBusiness = publicationBusiness;
        }

        [HttpGet]
        [Route("get-publication-list")]
        public async Task<IActionResult> GetList()
        {
            var publicationList = await _publicationBusiness.GetList();
            return Ok(publicationList);
        }

        [HttpPost]
        [Route("add-publication")]
        public async Task<IActionResult> Add([FromBody] PublicationDetails publication)
        {
            bool isAdded = await _publicationBusiness.Add(publication);

            if (isAdded)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to add publication");
            }
        }

        [HttpGet]
        [Route("get-publication-details")]
        public async Task<IActionResult> Edit([FromQuery] string id)
        {
            var publicationId = Convert.ToInt32(EncryptionHelper.Decrypt(id));

            var publicationDetails = await _publicationBusiness.GetDetails(publicationId);

            publicationDetails.PublicationIdString =
                EncryptionHelper.Encrypt(publicationDetails.PublicationId.ToString());

            return Ok(publicationDetails);
        }

        [HttpPut]
        [Route("update-publication-details")]
        public async Task<IActionResult> Edit([FromBody] PublicationDetails publication)
        {
            publication.PublicationId =
                Convert.ToInt32(EncryptionHelper.Decrypt(publication.PublicationIdString));

            var details = await _publicationBusiness.Edit(publication);

            if (details)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update publication details");
            }
        }

        [HttpPatch]
        [Route("update-publication-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string id)
        {
            var publicationId = Convert.ToInt32(EncryptionHelper.Decrypt(id));

            var user = "admin";

            var isUpdated = await _publicationBusiness.UpdateStatus(publicationId, user);

            if (isUpdated)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update publication status");
            }
        }
    }
}