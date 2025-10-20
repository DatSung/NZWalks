using System.Data.Common;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NZWalks.DataAccess.Data;
using NZWalks.DataAccess.Repository;
using NZWalks.DataAccess.Repository.IRepository;
using NZWalks.Service.Mappings;
using NZWalks.Service.Service.Interface;
using Respawn;
using Testcontainers.MsSql;

namespace Service.Tests.TestBase;

public class StartUpTestBase : IAsyncLifetime
{
    public NZWalksDbContext NZWalksDbContext { get; private set; } = null!;
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    private readonly MsSqlContainer _msSqlContainer;
    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;

    public StartUpTestBase()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1433, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .WithCleanUp(true) // Clear the container after test
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        
        var options = new DbContextOptionsBuilder<NZWalksDbContext>()
            .UseSqlServer(_msSqlContainer.GetConnectionString())
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;
        
        NZWalksDbContext = new NZWalksDbContext(options);
        await NZWalksDbContext.Database.MigrateAsync();
        
        var services = new ServiceCollection();

        // Register your services here
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var assemblies = new[]
        {
            typeof(AutoMapperProfiles).Assembly,
        };
        
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddLogging();
        services.AddSingleton(NZWalksDbContext);
        services.AddSingleton<IConfiguration>(configuration);
        services.AddAutoMapper(assemblies);
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IWalksService, NZWalks.Service.Service.Implement.WalksService>();

        ServiceProvider = services.BuildServiceProvider();
        
        await InitializeRespawner();
    }
    
    private async Task InitializeRespawner()
    {
        _dbConnection = NZWalksDbContext.Database.GetDbConnection();

        if (_dbConnection.State != System.Data.ConnectionState.Open)
        {
            await _dbConnection.OpenAsync();
        }
        
       

        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = ["dbo"]
            });
    }
    
    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
        NZWalksDbContext.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
        await NZWalksDbContext.DisposeAsync();
        await _msSqlContainer.DisposeAsync();
    }
}