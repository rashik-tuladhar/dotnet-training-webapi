using DlmsWebApi.Business.MemberBusiness;
using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Shared.MemberData;
using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers
{
    [ApiController]
    [Route("api/member")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberBusiness _memberBusiness;

    public MemberController(IMemberBusiness memberBusiness)
        {
            _memberBusiness = memberBusiness;
        }

        [HttpGet]
        [Route("get-member-list")]
        public async Task<IActionResult> GetList()
        {
            var memberList = await _memberBusiness.GetList();
            return Ok(memberList);
        }

        [HttpPost]
        [Route("add-member")]
        public async Task<IActionResult> Add([FromBody] MemberDetails member)
        {
            bool isAdded = await _memberBusiness.Add(member);

            if (isAdded)
            {
                return Created("", "Member added successfully");
            }
            else
            {
                return BadRequest("Failed to add member");
            }
        }

        [HttpGet]
        [Route("get-member-details")]
        public async Task<IActionResult> Edit([FromQuery] string id)
        {
            var memberId = Convert.ToInt32(EncryptionHelper.Decrypt(id));

            var memberDetails = await _memberBusiness.GetDetails(memberId);

            if (memberDetails == null)
            {
                return NotFound("Member not found");
            }

            memberDetails.MemberIdString =
                EncryptionHelper.Encrypt(memberDetails.MemberId.ToString());

            return Ok(memberDetails);
        }

        [HttpPut]
        [Route("update-member-details")]
        public async Task<IActionResult> Edit([FromBody] MemberDetails member)
        {
            member.MemberId =
                Convert.ToInt32(EncryptionHelper.Decrypt(member.MemberIdString));

            var details = await _memberBusiness.Edit(member);

            if (details)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update member details");
            }
        }

        [HttpPatch]
        [Route("update-member-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string id)
        {
            var memberId = Convert.ToInt32(EncryptionHelper.Decrypt(id));

            var user = "admin";

            var isUpdated = await _memberBusiness.UpdateStatus(memberId, user);

            if (isUpdated)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update member status");
            }
        }
    }

}
