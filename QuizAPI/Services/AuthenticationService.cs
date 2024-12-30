using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace QuizAPI.Services
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthenticationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //Fix later!!!
        public async Task Authorize(HttpContext httpContext)
        {
            var result = await ApiOperation(httpContext, "/AuthCheck");
        }

        public async Task<string?> GetUser(string userName)
        {
            //Fix this line
            if (userName == null)
            {
                return null;
            }
            var result = await ApiOperation(userName!, $"/GetUser?userName={userName}");
            return result.ToString();
        }

        private async Task<object> ApiOperation(object data, string apiPath)
        {
            var jsonLog = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonLog, Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient("Auth");

            var response = await client.GetAsync(apiPath);
            response.EnsureSuccessStatusCode();

            var apiContent = await response.Content.ReadAsStringAsync();
            return apiContent;
        }
    }
}
