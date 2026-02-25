using Json_Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace Json_Demo.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<People> People { get; set; }
    }
}
