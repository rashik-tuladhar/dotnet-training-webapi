using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestRouteController : ControllerBase
    {
        [HttpGet]
        [Route("from-query-string")]
        public async Task<IActionResult> GetAsync(string value)
        {
            return Ok($"Hello, {value}!");
        }
        
        [HttpGet]
        [Route("from-dynamic-routing/{value}")]
        public async Task<IActionResult> GetFromRoutingAsync(string value)
        {
            return Ok($"Hello, {value}!");
        }
        
        [HttpGet]
        [Route("get-from-body")]
        public async Task<IActionResult> GetFromBodyAsync([FromBody] BookHello bookHello)
        {
            return Ok($"Hello, world!");
        }
        
        [HttpGet]
        [Route("get-from-form")]
        public async Task<IActionResult> GetFromFormAsync([FromForm] BookHello bookHello)
        {
            return Ok($"Hello, world!");
        }
        
        [HttpGet]
        [Route("get-from-header")]
        public async Task<IActionResult> GetFromHeaderAsync([FromHeader] BookHello bookHello)
        {
            return Ok($"Hello, world!");
        }
        
        [HttpPost]
        [Route("payload-check-security")]
        public async Task<IActionResult> GetAsync([FromBody] BookHello bookHello)
        {
            
            /*
             * {
                 "name": "hello",
                 "author": "world"
               }
               
               Header:
                EncryptedValue: aGVsbG8sd29ybGQ=
             */
            
            var headerValue = HttpContext.Request.Headers["EncryptedValue"].ToString();
            
            var requestDetails = $"{bookHello.Name},{bookHello.Author}";
            byte[] stringBytes = Encoding.UTF8.GetBytes(requestDetails);
            var requestString = Convert.ToBase64String(stringBytes);
            
            if(headerValue == requestString)
            {
                return Ok($"Hello, {bookHello.Name}! Your request is valid.");
            }
            else
            {
                return BadRequest("Invalid request. Header value does not match the expected value.");
            }
        }
    }

    public class BookHello
    {
        public string Name { get; set; }
        public string Author { get; set; }
    }
}

