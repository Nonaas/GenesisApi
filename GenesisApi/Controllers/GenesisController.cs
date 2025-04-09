using GenesisApi.Interfaces;
using GenesisApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace GenesisApi.Controllers
{
    [ApiController]
    [Route("api/genesis")]
    public class GenesisController(IHttpClientFactory _httpClientFactory, ITableParserService _tableParserService) : Controller
    {
        private readonly HttpClient _httpClient = _httpClientFactory.CreateClient();

        public GenesisController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("check-login")]
        public async Task<IActionResult> LoginCheck([FromBody] LoginRequest request)
        {
            Dictionary<string, string> values = new()
        {
            { "username", request.Username },
            { "language", request.Language }
        };

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                values.Add("password", request.Password);
            }

            FormUrlEncodedContent content = new(values);

            HttpResponseMessage responseMessage = await _httpClient.PostAsync(
                "https://www-genesis.destatis.de/genesisWS/rest/2020/helloworld/logincheck", 
                content);

            string responseBody = await responseMessage.Content.ReadAsStringAsync();

            return Content(responseBody, "application/json");
        }
    }
}
