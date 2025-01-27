using Microsoft.EntityFrameworkCore;
using CL8.DAL.Entities;

namespace CL8.DAL
{
    public class MyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Database=CL8;Trusted_Connection=True;Trust Server Certificate=True;");
        }
    }
}