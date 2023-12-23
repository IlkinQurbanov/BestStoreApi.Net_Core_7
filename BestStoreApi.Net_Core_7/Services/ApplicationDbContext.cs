using BestStoreApi.Net_Core_7.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreApi.Net_Core_7.Services
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { 

        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }

    }
}
