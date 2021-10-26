using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CRLogAPI.Models
{
    // This class represents LoginDBContext with inheritance from DbContext!
    public class LoginDBContext : DbContext
    {

        public LoginDBContext(DbContextOptions<LoginDBContext> options) : base(options)
        {
            this.Database.Migrate();
        }

        // Properties to be tables in the database!
        public DbSet<DbUser> Users { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
    }
}

