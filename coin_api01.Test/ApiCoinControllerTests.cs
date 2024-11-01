using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using coin_api01.Controllers;
using coin_api01.Services;
using coin_api01.Models;
using coin_api01.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace coin_api01.Tests
{
    [TestFixture]
    public class ApiCoinControllerTests
    {
        private Mock<ILogger<ApiCoinController>> _loggerMock;
        private Mock<ICoinService> _coinServiceMock;
        private Mock<IOptions<CoinApiOptions>> _optionsMock;
        private ApiCoinController _controller;
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ApiCoinController>>();
            _coinServiceMock = new Mock<ICoinService>();
            _optionsMock = new Mock<IOptions<CoinApiOptions>>();
            _optionsMock.Setup(o => o.Value).Returns(new CoinApiOptions { DefaultTimeZone = 0 });

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(options);

            _controller = new ApiCoinController(_loggerMock.Object, _optionsMock.Object, _coinServiceMock.Object, _context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // 1. Test Get method

        [Test]
        public void Get_ShouldReturnServiceUnavailable_WhenCoinServiceReturnsNull()
        {
            _coinServiceMock.Setup(cs => cs.GetCurrentPrice()).Returns((CurrentPriceModel)null);

            var result = _controller.Get() as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(503));
        }

        [Test]
        public void Get_ShouldReturnOkResultWithCorrectData_WhenCoinServiceReturnsValidData()
        {
            var currentPriceModel = new CurrentPriceModel
            {
                time = new CurrentPriceTime { updatedISO = DateTimeOffset.Parse("2024-11-01T05:42:55+00:00") },
                bpi = new Dictionary<string, CurrentPriceInfoModel>
                {
                    { "USD", new CurrentPriceInfoModel { code = "USD", rate_float = 69586.8726M } },
                    { "GBP", new CurrentPriceInfoModel { code = "GBP", rate_float = 53951.0503M } },
                    { "EUR", new CurrentPriceInfoModel { code = "EUR", rate_float = 63973.9955M } }
                }
            };
            _coinServiceMock.Setup(cs => cs.GetCurrentPrice()).Returns(currentPriceModel);

            _context.CoinLang.AddRange(
                new CoinLangModel { Code = "USD", Name = "US Dollar" },
                new CoinLangModel { Code = "GBP", Name = "British Pound" },
                new CoinLangModel { Code = "EUR", Name = "Euro" }
            );
            _context.SaveChanges();

            var result = _controller.Get() as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var responseData = result.Value as ResponseCoinPriceModel;
            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.update, Is.EqualTo(currentPriceModel.time.updatedISO));
            Assert.That(responseData.data.Any(c => c.Code == "USD" && c.Rate == 69586.8726M), Is.True);
        }

        // 2. Test GetSupport method

        [Test]
        public void GetSupport_ShouldReturnSupportedCodes()
        {
            var result = _controller.GetSupport() as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var responseData = result.Value as CoinSupportModel;
            Assert.That(responseData, Is.Not.Null);
            Assert.That(responseData.Codes, Is.EquivalentTo(new List<string> { "USD", "GBP", "EUR" }));
        }

        // 3. Test AddLang method

        [Test]
        public void AddLang_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            var invalidCoin = new CoinLangModel { Code = "", Name = "" };

            var result = _controller.AddLang(invalidCoin) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void AddLang_ShouldReturnBadRequest_WhenCodeNotSupported()
        {
            var unsupportedCoin = new CoinLangModel { Code = "JPY", Name = "Japanese Yen" };

            var result = _controller.AddLang(unsupportedCoin) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void AddLang_ShouldReturnBadRequest_WhenCoinCodeAlreadyExists()
        {
            var existingCoin = new CoinLangModel { Code = "USD", Name = "US Dollar" };
            _context.CoinLang.Add(existingCoin);
            _context.SaveChanges();

            var result = _controller.AddLang(existingCoin) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void AddLang_ShouldReturnOk_WhenModelIsValid()
        {
            var coin = new CoinLangModel { Code = "GBP", Name = "British Pound" };

            var result = _controller.AddLang(coin);

            Assert.That(result, Is.TypeOf<OkResult>());
            Assert.That(_context.CoinLang.Any(c => c.Code == "GBP"), Is.True);
        }

        // 4. Test UpdateLang method

        [Test]
        public void UpdateLang_ShouldReturnBadRequest_WhenCodeNotFound()
        {
            var updateCoin = new CoinLangModel { Name = "Updated Coin" };

            var result = _controller.UpdateLang("NonExistingCode", updateCoin) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void UpdateLang_ShouldReturnBadRequest_WhenInputIsInvalid()
        {
            var coin = new CoinLangModel { Code = "USD", Name = "US Dollar" };
            _context.CoinLang.Add(coin);
            _context.SaveChanges();

            var result = _controller.UpdateLang("USD", null) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void UpdateLang_ShouldReturnOk_WhenCodeExists()
        {
            var coin = new CoinLangModel { Code = "USD", Name = "US Dollar" };
            _context.CoinLang.Add(coin);
            _context.SaveChanges();

            var updateCoin = new CoinLangModel { Name = "Updated Dollar" };

            var result = _controller.UpdateLang("USD", updateCoin);

            Assert.That(result, Is.TypeOf<OkResult>());
            Assert.That(_context.CoinLang.FirstOrDefault(c => c.Code == "USD")?.Name, Is.EqualTo("Updated Dollar"));
        }

        // 5. Test DeleteLang method

        [Test]
        public void DeleteLang_ShouldReturnBadRequest_WhenCodeNotFound()
        {
            var result = _controller.DeleteLang("NonExistingCode") as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void DeleteLang_ShouldReturnOk_WhenCodeExists()
        {
            var coin = new CoinLangModel { Code = "USD", Name = "US Dollar" };
            _context.CoinLang.Add(coin);
            _context.SaveChanges();

            var result = _controller.DeleteLang("USD");

            Assert.That(result, Is.TypeOf<OkResult>());
            Assert.That(_context.CoinLang.Any(c => c.Code == "USD"), Is.False);
        }
    }
}
