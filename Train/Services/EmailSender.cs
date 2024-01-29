using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Train.Models;

namespace Train.Services { 
public class EmailSender : IEmailSender
    {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public EmailSender(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Read API settings from appsettings.json
            var apiUrl = _configuration["EmailSettings:ApiUrl"];
            var apiToken = _configuration["EmailSettings:ApiToken"];

            var emailRequest = new EmailRequest
            {
                ApiToken = apiToken,
                Message = new EmailMessage
                {
                    To = email,
                    Subject = subject,
                    Body = htmlMessage
                }
            };

            // Set up the request headers
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Serialize the email request to JSON
            var jsonContent = JsonConvert.SerializeObject(emailRequest);

            using (var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
            {
                // Make the POST request
                var response = await _httpClient.PostAsync(apiUrl, content);

                // Check the response status
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    // Process the successful response if needed
                    Console.WriteLine(result);
                }
                else
                {
                    // Handle unsuccessful response
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorMessage}");
                    // You may log the error message or perform other actions if needed
                }
            }
        }
    }
}
