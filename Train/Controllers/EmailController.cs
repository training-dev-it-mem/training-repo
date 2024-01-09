using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Train.Models;

namespace Train.Controllers
{
    public class EmailController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EmailController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        [HttpGet("/emails")]
        public async Task<IActionResult> SendEmail()
        {
            // Read API settings from appsettings.json
            var apiUrl = _configuration["EmailSettings:ApiUrl"];
            var apiToken = _configuration["EmailSettings:ApiToken"];

            var emailRequest = new EmailRequest
            {
                ApiToken = apiToken,
                Messages = new List<EmailMessage>
            {
                new EmailMessage
                {
                    To = "ahmed.s.alwahaibi@mem.gov.om",
                    Subject = "Test Subject 1",
                    Body = "Test Body 1"
                },
                  new EmailMessage
                {
                    To = "ahmed.s.alwahaibi@mem.gov.om",
                    Subject = "Test Subject 2",
                    Body = "Test Body 2"
                }
            }
            };

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    // Set up the request headers
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Serialize the email request to JSON
                    var jsonContent = JsonConvert.SerializeObject(emailRequest);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Make the POST request
                    var response = await client.PostAsync(apiUrl, content);

                    // Check the response status
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        // Process the successful response if needed
                        return Ok(result);
                    }
                    else
                    {
                        // Handle unsuccessful response
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
