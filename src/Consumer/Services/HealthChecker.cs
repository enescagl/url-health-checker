using System.Net;

using Microsoft.Extensions.Logging;

using URLHealthChecker.Consumer.Entity;
using URLHealthChecker.Consumer.Interfaces;

namespace URLHealthChecker.Consumer.Services
{
    public class HealthChecker : IHealthChecker
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HealthChecker> _logger;

        public HealthChecker(IHttpClientFactory httpClientFactory, ILogger<HealthChecker> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task LogURLStatus(string url)
        {
            HttpStatusCode statusCode = await CheckURL(url);
            URLLog urlLog = new() { CallingService = nameof(HealthChecker), URL = url.Trim(), StatusCode = statusCode.ToString() };
            _logger.LogInformation("{@URLLog}", urlLog);
        }

        private async Task<HttpStatusCode> CheckURL(string url)
        {
            bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                return response.StatusCode;
            }
            return HttpStatusCode.InternalServerError;
        }
    }
}