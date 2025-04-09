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

        [HttpPost("fetch-table-data")]
        public async Task<IActionResult> TableRequest([FromBody] TableRequest request)
        {
            Dictionary<string, string> requestParams = new()
            {
                { "username", request.Token },
                { "name", request.TableCode },
                { "area", request.Area },
                { "language", request.Language }
            };

            if (!string.IsNullOrEmpty(request.StartYear))
            {
                requestParams.Add("startyear", request.StartYear);
            }

            if (!string.IsNullOrEmpty(request.EndYear))
            {
                requestParams.Add("endyear", request.EndYear);
            }

            string queryString = string.Join("&", requestParams.Select(kv =>
                $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            string requestUrl = $"https://www-genesis.destatis.de/genesisWS/rest/2020/data/table?{queryString}";

            HttpResponseMessage responseMessage = await _httpClient.GetAsync(requestUrl);
            string responseJson = await responseMessage.Content.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
            {
                return StatusCode((int)responseMessage.StatusCode, responseJson);
            }

            // Get json root object
            using var doc = JsonDocument.Parse(responseJson);
            JsonElement root = doc.RootElement;

            // Get content block
            string? content = root
                .GetProperty("Object")
                .GetProperty("Content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("No data content found.");
            }

            List<ParsedTableRow> parsedRows = _tableParserService.ParseTableContent(content);

            return Ok(parsedRows);
        }

    }
}
