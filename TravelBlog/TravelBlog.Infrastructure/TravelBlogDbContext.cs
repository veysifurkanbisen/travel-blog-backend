using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelBlog.Shared;

namespace TravelBlog.Infrastructure;

public class TravelBlogDbContext : DbContext
{
    private readonly ConnectionStrings _connectionStrings;

    public TravelBlogDbContext(DbContextOptions options, IOptions<ConnectionStrings> connectionStrings): base(options)
    {
        _connectionStrings = connectionStrings.Value;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionStrings.DefaultConnection);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}