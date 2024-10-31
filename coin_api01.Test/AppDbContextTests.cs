using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using coin_api01.Data;
using coin_api01.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace coin_api01.Tests
{
    [TestFixture]
    public class AppDbContextTests
    {
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // �ϥ� InMemoryDatabase
            .Options;

            _context = new AppDbContext(options);

            // Add���ռƾ�
            SeedTestData();
        }

        private void SeedTestData()
        {
            var testData = new List<CoinLangModel>
            {
                new CoinLangModel { Id = 1, Code="USD",Name="����" },
                new CoinLangModel { Id = 2, Code="EUR",Name="�ڤ�" },
                new CoinLangModel { Id = 3, Code="GBP",Name="�^��" }
            };
            _context.CoinLang.AddRange(testData);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllCoinLangs_ShouldReturnAllData()
        {
            // Act
            var results = await _context.CoinLang.ToListAsync();

            // Assert
            Assert.That(results, Has.Count.EqualTo(3));
        }

        [Test]
        public async Task FindCoinLang_ById_ShouldReturnCorrectData()
        {
            // Act
            var result = await _context.CoinLang.FirstOrDefaultAsync(x => x.Code == "EUR");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("�ڤ�"));
        }

        [Test]
        public async Task UpdateCoinLang_ShouldModifyData()
        {
            // Arrange
            var coinLang = await _context.CoinLang.FirstOrDefaultAsync(x => x.Code == "USD");
            coinLang.Name = "USD����";

            // Act
            _context.CoinLang.Update(coinLang);
            await _context.SaveChangesAsync();

            var updatedCoinLang = await _context.CoinLang.FirstOrDefaultAsync(x => x.Code == "USD");

            // Assert
            Assert.That(updatedCoinLang.Name, Is.EqualTo("USD����"));
        }

        [Test]
        public async Task DeleteCoinLang_ShouldDecreaseCount()
        {
            // Arrange
            var coinLang = await _context.CoinLang.FirstOrDefaultAsync(x => x.Code == "GBP");

            // Act
            _context.CoinLang.Remove(coinLang);
            await _context.SaveChangesAsync();

            // Assert
            Assert.That(_context.CoinLang.Count(), Is.EqualTo(2));
        }
    }
}