using coin_api01.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;

namespace coin_api01.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CoinLangModel> CoinLang { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
