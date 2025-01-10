using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using QuizAPI.UserAccessor;
using Newtonsoft.Json.Linq;

namespace QuizAPI.Services
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserAccessor _userAccessor;
        public AuthenticationService(IHttpClientFactory httpClientFactory, IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
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
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _userAccessor.Token);

            var response = await client.GetAsync(apiPath);
            response.EnsureSuccessStatusCode();

            var apiContent = await response.Content.ReadAsStringAsync();
            return apiContent;
        }
    }
}
