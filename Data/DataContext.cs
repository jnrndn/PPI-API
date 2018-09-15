using Microsoft.EntityFrameworkCore;
using PPI.API.Models;

namespace PPI.API.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        
    }
}