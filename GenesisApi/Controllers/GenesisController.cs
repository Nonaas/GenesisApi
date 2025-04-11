using GenesisApi.Interfaces;
using GenesisApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace GenesisApi.Controllers
{
    [ApiController]
    [Route("api/genesis")]
    public class GenesisController(IOptions<GenesisConf> conf, IHttpClientFactory _httpClientFactory, ITableParserService _tableParserService) : Controller
    {
        private readonly HttpClient _httpClient = _httpClientFactory.CreateClient();


        [HttpPost("check-login")]
        public async Task<IActionResult> LoginCheck([FromBody] LoginRequest request)
        {
            string token = !string.IsNullOrEmpty(request.Username) ? request.Username : conf.Value.Token;

            Dictionary<string, string> values = new()
            {
                { "username", token },
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
            string token = !string.IsNullOrEmpty(request.Token) ? request.Token : conf.Value.Token;

            Dictionary<string, string> requestParams = new()
            {
                { "username", token },
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

            // Get content
            string? content;
            try
            {
                using var doc = JsonDocument.Parse(responseJson);
                JsonElement root = doc.RootElement;

                // Get content block
                content = root.GetProperty("Object")
                              .GetProperty("Content")
                              .GetString();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("No data found.");
            }
            else
            {
                string jsonResult = string.Empty;

                try
                {
                    jsonResult = _tableParserService.ParseTableContentToJson(content);

                    return Ok(jsonResult);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error: {ex.Message}");
                }
            }
        }

        [HttpPost("fetch-table-data-raw")]
        public async Task<IActionResult> TableRequestRaw([FromBody] TableRequest request)
        {
            string token = !string.IsNullOrEmpty(request.Token) ? request.Token : conf.Value.Token;

            Dictionary<string, string> requestParams = new()
            {
                { "username", token },
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

            // Get content
            string? content;
            try
            {
                using var doc = JsonDocument.Parse(responseJson);
                JsonElement root = doc.RootElement;

                // Get content block
                content = root.GetProperty("Object")
                              .GetProperty("Content")
                              .GetString();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("No data found.");
            }
            else
            {
                return Ok(content);
            }
        }

    }
}
