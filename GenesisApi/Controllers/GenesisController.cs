using GenesisApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GenesisApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenesisController : Controller
    {
        private readonly HttpClient _httpClient;

        public GenesisController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("logincheck")]
        public async Task<IActionResult> LoginCheck([FromBody] LoginRequest request)
        {
            var values = new Dictionary<string, string>
        {
            { "username", request.Username },
            { "language", request.Language }
        };

            if (!string.IsNullOrWhiteSpace(request.Password))
                values.Add("password", request.Password);

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync("https://www-genesis.destatis.de/genesisWS/rest/2020/helloworld/logincheck", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return Content(responseBody, "application/json");
        }
    }
}
