using FluentValidation;
using MassTransit;
using Newtonsoft.Json;
using TravelBlog.Infrastructure;
using TravelBlog.Shared;
using TravelBlog.Shared.Extentions;

namespace TravelBlog.API;

public class Startup
{
    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }
    
    public Startup(
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddBearerAuthentication(Configuration);
        //services.AddAuthorization();
        //services.AddDataProtection();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        //services.AddOptions();


        services.AddHttpClient();
        
        //services.AddTransient<AuthHttpClientFactory>();
        //services.AddSingleton<AuthHttpClientAuthorization>();

        #region Context
        services.AddDatabaseContext<TravelBlogDbContext, Startup>(Configuration);
        #endregion

        #region Configurations
        services.Configure<ConnectionStrings>(Configuration.GetSection(nameof(ConnectionStrings)));
        //services.Configure<ApiSettings>(Configuration.GetSection("ApiSettings"));
        //services.Configure<RabbitMQDetails>(Configuration.GetSection(nameof(RabbitMQDetails)));
        //services.Configure<AnalyticResultConfig>(Configuration.GetSection(nameof(AnalyticResultConfig)));
        //services.Configure<DataFlowThresholdConfig>(Configuration.GetSection(nameof(DataFlowThresholdConfig)));


        //MapsterConfig.ConfigureMapping();
        #endregion

        #region Repositories
        /*
        services.AddScoped<IAssetGroupRepository, AssetGroupRepository>();
        services.AddScoped<IAssetConfigRepository, AssetConfigRepository>();
        services.AddScoped<IAssetModelConfigRepository, AssetModelConfigRepository>();
        services.AddScoped<IATGroupHistoryRepository, ATGroupHistoryRepository>();
        services.AddScoped<IATConfigHistoryRepository, ATConfigHistoryRepository>();
        services.AddScoped<IATModelConfigHistoryRepository, ATModelConfigHistoryRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IScoresRepository, ScoresRepository>();
        services.AddScoped<IParameterRepository, ParameterRepository>();
        services.AddScoped<IActionRepository, ActionRepository>();
        services.AddScoped<IActionHistoryRepository, ActionHistoryRepository>();
        // !
        services.AddScoped<ITelemetryRawDataReadonlyRepository, TelemetryRawDataReadonlyRepository>();
        */
        #endregion

        #region Services
        /*
        services.AddScoped<IAssetGroupService, AssetGroupService>();
        services.AddScoped<IAssetConfigService, AssetConfigService>();
        services.AddScoped<IAssetModelConfigService, AssetModelConfigService>();
        services.AddScoped<IAssetTelemetryHelperService, AssetTelemetryHelperService>();
        services.AddScoped<IATGroupHistoryService, ATGroupHistoryService>();
        services.AddScoped<IATConfigHistoryService, ATConfigHistoryService>();
        services.AddScoped<IATModelConfigHistoryService, ATModelConfigHistoryService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IAttributeService, AttributeService>();
        services.AddScoped<IScoresService, ScoresService>();
        services.AddScoped<IActionService, ActionService>();
        services.AddScoped<IConnectorService, ConnectorService>();
        services.AddScoped<IMasterBoardService, MasterBoardService>();
        services.AddScoped<ITelemetryRawDataService, TelemetryRawDataService>();
        services.AddScoped<IReportService, ReportService>();
        */
        
        #endregion

        #region FluentValidation
        // services.AddScoped<IAnalyticsAssetModelConfigDtoValidator, AnalyticsAssetModelConfigDtoValidator>();
        // services.AddTransient<ValidateHeaderHandler>();
        // services.AddScoped(typeof(IValidator<CarbonBaseRequestDto>), typeof(BaseRequestValidator<CarbonBaseRequestDto>));

        // AssetConfig
        /*
        services.AddScoped(typeof(IValidator<AssetConfigCreateDto>), typeof(AssetConfigCreateDtoValidator));
        services.AddScoped(typeof(IValidator<AssetConfigDeleteDto>), typeof(AssetConfigDeleteDtoValidator));
        services.AddScoped(typeof(IValidator<AssetConfigUpdateDto>), typeof(AssetConfigUpdateDtoValidator));
        */

        // AssetGroup
        /*
        services.AddScoped(typeof(IValidator<AssetGroupCreateDto>), typeof(AssetGroupCreateDtoValidator));
        services.AddScoped(typeof(IValidator<AssetGroupDeleteDto>), typeof(AssetGroupDeleteDtoValidator));
        services.AddScoped(typeof(IValidator<AssetGroupUpdateDto>), typeof(AssetGroupUpdateDtoValidator));
        services.AddScoped(typeof(IValidator<AssetGroupUpsertRequestDto>), typeof(AssetGroupUpsertRequestDtoValidator));
        */

        // AssetModelConfig
        /*
        services.AddScoped(typeof(IValidator<AssetModelConfigCreateDto>), typeof(AssetModelConfigCreateDtoValidator));
        services.AddScoped(typeof(IValidator<AssetModelConfigDeleteDto>), typeof(AssetModelConfigDeleteDtoValidator));
        services.AddScoped(typeof(IValidator<AssetModelConfigUpdateDto>), typeof(AssetModelConfigUpdateDtoValidator));
        */
        
        //MasterBoard
        //services.AddScoped(typeof(IValidator<MasterBoardRequestDto>), typeof(MasterBoardRequestDtoValidator));
        #endregion

        #region ApiClients
        // services.AddScoped<IDatalogManagementApiClient, DatalogManagementApiClient>();
        // services.AddScoped<IAssetManagementApiClient, AssetManagementApiClient>();
        // services.AddScoped<IAnalyticsApiClient, AnalyticsApiClient>();
        #endregion
        
        /*
        #region MassTransit

        var massTransitSettings = Configuration.GetSection("MassTransit").Get<MassTransitSettings>();
        services.AddSingleton<MassTransitSettings>(massTransitSettings);
        services.AddMassTransitBus(cfg =>
            {
                var rabbitMQDetails = Configuration.GetSection("RabbitMQDetails").Get<RabbitMQDetails>();

                cfg.AddConsumer<ConfigLogConsumer>();
                cfg.AddConsumer<ModelConfigLogConsumer>();
                cfg.AddConsumer<RiskScoreConsumer>();

                cfg.AddRabbitMqBus(Configuration, (provider, busFactoryConfig) =>
                {

                    busFactoryConfig.ReceiveEndpoint(rabbitMQDetails.MaiConfigLogQueueName, configurator =>
                    {
                        configurator.ClearMessageDeserializers();
                        configurator.UseRawJsonSerializer();
                        configurator.Consumer<ConfigLogConsumer>(provider);
                        Retry.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                        configurator.PrefetchCount = 1;
                    });

                    busFactoryConfig.ReceiveEndpoint(rabbitMQDetails.MaiModelConfigLogQueueName, configurator =>
                    {
                        configurator.ClearMessageDeserializers();
                        configurator.UseRawJsonSerializer();
                        configurator.Consumer<ModelConfigLogConsumer>(provider);
                        Retry.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                        configurator.PrefetchCount = 1;
                    });

                    busFactoryConfig.ReceiveEndpoint(rabbitMQDetails.MaiSetRiskScoreQueueName, configurator =>
                    {
                        configurator.ClearMessageDeserializers();
                        configurator.UseRawJsonSerializer();
                        configurator.Consumer<RiskScoreConsumer>(provider);
                        Retry.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                        configurator.PrefetchCount = 1;
                    });
                });
            });
        #endregion
        */
    }
    

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            Console.Out.Write("Development Environmnet MAHOOOOOOOOOOOOOOOOOOOOOOOOOO");
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.MigrateDatabase<TravelBlogDbContext>();

    }
}
