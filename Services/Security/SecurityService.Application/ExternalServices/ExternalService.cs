using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace SecurityService.Application.ExternalServices
{
    public class ExternalService : IExternalServices
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ExternalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task AddUserBalanceAsync(string username)
        {
            var balanceUser = new { Username = username, InitialBalance = 0 };
            var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiGateway:BaseUrl"]}/api/balance/create", balanceUser);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddUserTopUpAsync(string username, bool isVerified)
        {
            var topUpUser = new { Username = username, IsVerified = 0 };
            var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiGateway:BaseUrl"]}/api/user/create", topUpUser);
            response.EnsureSuccessStatusCode();
        }
    }
}
