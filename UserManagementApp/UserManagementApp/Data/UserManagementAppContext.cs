using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.Models;

namespace UserManagementApp.Data
{
    public class UserManagementAppContext : DbContext
    {

        public UserManagementAppContext (DbContextOptions<UserManagementAppContext> options)
            : base(options)
        {
        }

        public DbSet<UserManagementApp.Models.User> User { get; set; } = default!;
    }
}
