using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
namespace TaskManager.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<TaskItem> TaskItems { get; set; } = null!;
    }
}
