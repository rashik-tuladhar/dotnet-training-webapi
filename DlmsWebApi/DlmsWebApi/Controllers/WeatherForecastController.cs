using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("get-string")]
        public async Task<IActionResult> GetAsync()
        {
            return Ok("Hello, World!");
        }

        [HttpPost]
        [Route("add-string")]
        public async Task<IActionResult> PostAsync(string value)
        {
            return Created($"Hello, {value}!", value);
        }


        [HttpPost]
        [Route("add-string-from-body")]
        public async Task<IActionResult> PostWithBodyAsync([FromBody] string value)
        {
            return Created($"Hello, {value}!", value);
        }


    }
}


/*
 HTTP VERBS
 * GET: read/fetch
 * POST: create
 * PUT: update -> whole replace/ update
 * PATCH: update -> certain part or values
 * DELETE: delete
 * 
 * 
 * 
    [HttpGet]
    [HttpDelete]
    [HttpPost]
    [HttpPatch]
    [HttpPut]

    [HttpHead]
    [HttpOptions]
 * 
 */


/*
 STATUS CODES 
    200 -> Success -> GET
    201 -> Created -> POST
    204 -> Success with no body or no response -> DELETE
    400 -> Bad request (form validation) -> POST/PUT/PATCH (validation error)
    401 -> Unauthorized (invalid credentials) -> POST
    403 -> Forbidden (Authorization failed) -> POST
    404 -> Page or url not found -> EVERY
    405 -> Method not allowed
    500 -> Internal server error -> Exception
 */