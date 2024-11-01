using NUnit.Framework;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using coin_api01.Services;
using coin_api01.Models;
using System.Text.Json;
using Moq.Protected;
using System.Collections.Generic;

namespace coin_api01.Tests
{
    [TestFixture]
    public class CoinDeskServiceTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<ILogger<CoinDeskService>> _loggerMock;
        private CoinDeskService _coinDeskService;

        [SetUp]
        public void Setup()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<CoinDeskService>>();
            _coinDeskService = new CoinDeskService(_loggerMock.Object, _httpClientFactoryMock.Object);
        }

        [Test]
        public void GetCurrentPrice_ShouldReturnValidData_WhenApiResponseIsSuccessful()
        {
            // Arrange
            var expectedPriceModel = new CurrentPriceModel
            {
                chartName = "Bitcoin",
                disclaimer = "Sample Disclaimer",
                time = new CurrentPriceTime
                {
                    updated = "Nov 1, 2024 04:36:18 UTC",
                    updatedISO = DateTimeOffset.Parse("2024-11-01T04:36:18+00:00"),
                    updateduk = "Nov 1, 2024 at 04:36 GMT"
                },
                bpi = new Dictionary<string, CurrentPriceInfoModel>
                {
                    { "USD", new CurrentPriceInfoModel { code = "USD", rate = "64,000.00", description = "United States Dollar", rate_float = 64000.00M } }
                }
            };
            var jsonResponse = JsonSerializer.Serialize(expectedPriceModel);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var client = new HttpClient(httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = _coinDeskService.GetCurrentPrice();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.chartName, Is.EqualTo(expectedPriceModel.chartName));
            Assert.That(result.bpi["USD"].rate, Is.EqualTo(expectedPriceModel.bpi["USD"].rate));
        }

        [Test]
        public void GetCurrentPrice_ShouldReturnNull_WhenApiResponseIsEmpty()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                });

            var client = new HttpClient(httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = _coinDeskService.GetCurrentPrice();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetCurrentPrice_ShouldReturnNull_WhenApiResponseFails()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError // 模擬 API 失敗
                });

            var client = new HttpClient(httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = _coinDeskService.GetCurrentPrice();

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
