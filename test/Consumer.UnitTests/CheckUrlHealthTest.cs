using Microsoft.Extensions.Logging;

using Moq;
using Moq.Protected;

using URLHealthChecker.Consumer.Services;

namespace URLHealthCheck.Consumer.UnitTests
{
    public class CheckUrlHealthTest
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<HealthChecker>> _mockLogger;

        public CheckUrlHealthTest()
        {
            Mock<HttpMessageHandler> handlerMock = new(MockBehavior.Strict);
            HttpResponseMessage response = new();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response)
                .Verifiable();

            HttpClient httpClient = new(handlerMock.Object)
            {
                BaseAddress = new Uri("https://www.google.com")
            };

            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _ = _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _mockLogger = new Mock<ILogger<HealthChecker>>();
        }

        [Fact]
        public void InvalidURLReturnsInternalServiceError()
        {
            HealthChecker healthChecker = new(_mockHttpClientFactory.Object, _mockLogger.Object);
            healthChecker.LogURLStatus("https://www.google.com").Wait();
        }
    }
}