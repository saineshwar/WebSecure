using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebSecure.Models;

namespace WebSecure.Repository
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<UserTokens> UserTokens { get; set; }
        public DbSet<RegisterVerification> RegisterVerification { get; set; }
        public DbSet<ResetPasswordVerification> ResetPasswordVerification { get; set; }
    }
}
