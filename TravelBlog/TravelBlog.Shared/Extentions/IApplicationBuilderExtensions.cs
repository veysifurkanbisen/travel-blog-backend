using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace TravelBlog.Shared.Extentions;

public static class IApplicationBuilderExtensions
{
    public static void MigrateDatabase<TContext>(
      this 
      #nullable disable
      IApplicationBuilder app,
      bool EnableLegacyTimestampBehavior = true)
      where TContext : DbContext
    {
      if (EnableLegacyTimestampBehavior)
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
      using (IServiceScope scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        scope.ServiceProvider.GetRequiredService<TContext>().Database.Migrate();
    }
    
    public static void AddDatabaseContext<TContext, TStartup>(
      this IServiceCollection services,
      IConfiguration configuration)
      where TContext : DbContext
      where TStartup : class
    {
      string target = configuration.GetConnectionString("ConnectionTarget") ?? "MSSQL";
      string connectionString = configuration.GetConnectionString("DefaultConnection");
      string migrationsAssembly = typeof (TStartup).GetTypeInfo().Assembly.GetName().Name;
      string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      switch (target.ToLower())
      {
        case "postgresql":
          services.AddDbContext<TContext>((Action<DbContextOptionsBuilder>) (options => options.UseNpgsql(connectionString, (Action<NpgsqlDbContextOptionsBuilder>) (sql => sql.MigrationsAssembly(migrationsAssembly)))));
          break;
        case "mssql":
          //services.AddDbContext<TContext>((Action<DbContextOptionsBuilder>) (options => options.UseSqlServer(connectionString, (Action<SqlServerDbContextOptionsBuilder>) (sql => sql.MigrationsAssembly(migrationsAssembly)))));
          break;
        default:
          throw new Exception("No Valid Connection Target Found");
      }
      services.AddDatabaseContextHealthCheck(target, connectionString);
      try
      {
        AssemblyLoadContext assemblyLoadContext = AssemblyLoadContext.Default;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
        interpolatedStringHandler.AppendFormatted(directoryName);
        interpolatedStringHandler.AppendFormatted<char>(Path.AltDirectorySeparatorChar);
        interpolatedStringHandler.AppendFormatted(migrationsAssembly);
        interpolatedStringHandler.AppendLiteral(".dll");
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        assemblyLoadContext.LoadFromAssemblyPath(stringAndClear);
      }
      catch
      {
        Console.WriteLine("No Migration Assembly Loaded!");
      }
    }
    
    public static void AddDatabaseContextHealthCheck(
      this IServiceCollection services,
      string target,
      string connectionString,
      HealthStatus failureStatus = HealthStatus.Unhealthy)
    {
      switch (target.ToLower())
      {
        case "mssql":
          IHealthChecksBuilder builder1 = services.AddHealthChecks();
          string connectionString1 = connectionString;
          HealthStatus? nullable1 = new HealthStatus?(failureStatus);
          string name1 = target ?? "";
          HealthStatus? failureStatus1 = nullable1;
          TimeSpan? timeout1 = new TimeSpan?();
          //builder1.AddSqlServer(connectionString1, name: name1, failureStatus: failureStatus1, timeout: timeout1);
          break;
        case "postgresql":
          IHealthChecksBuilder builder2 = services.AddHealthChecks();
          string connectionString2 = connectionString;
          HealthStatus? nullable2 = new HealthStatus?(failureStatus);
          string name2 = target ?? "";
          HealthStatus? failureStatus2 = nullable2;
          TimeSpan? timeout2 = new TimeSpan?();
          builder2.AddNpgSql(connectionString2, name: name2, failureStatus: failureStatus2, timeout: timeout2);
          break;
        default:
          throw new Exception("No Valid Connection Target Found");
      }
    }
}