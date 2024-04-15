using Microsoft.EntityFrameworkCore;
using Server.Model.Models;

namespace Server.Context.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Meeting?> Meetings { get; set; }
}