using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TopUpService.Application.Services;

namespace TopUp.Application.ExternalServices
{
    public class ExternalBalanceService : IExternalBalanceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BeneficiaryService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public ExternalBalanceService(HttpClient httpClient, ILogger<BeneficiaryService> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<decimal> GetBalanceAsync(string username)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_configuration["ApiGateway:BaseUrl"]}/api/balance");
                response.EnsureSuccessStatusCode();
                var balance = await response.Content.ReadAsStringAsync();
                return decimal.Parse(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching balance for the user: {Username}", username);
                throw new Exception("An error occurred while fetching balance.");
            }
        }

        public async Task<bool> DebitBalanceAsync(string username, decimal amount)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var payload =  amount;
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_configuration["ApiGateway:BaseUrl"]}/api/balance/debit", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Debit balance for the user: {Username}", username);
                throw new Exception("An error occurred while debit balance.");
            }
        }

        public async Task<bool> CreditBalanceAsync(string username, decimal amount)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var payload = amount;
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_configuration["ApiGateway:BaseUrl"]}/api/balance/credit", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Credit balance for the user: {Username}", username);
                throw new Exception("An error occurred while Credit balance.");
            }

        }
    }
}
