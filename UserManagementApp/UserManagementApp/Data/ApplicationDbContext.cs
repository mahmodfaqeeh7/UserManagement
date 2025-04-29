using UserManagementApp.Models;

namespace UserManagementApp.Data;
using System.Data.Entity;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() : base("DefaultConnection") { }
    public DbSet<User> Users { get; set; }
}
