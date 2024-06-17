using System.Reflection;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NestF.Application.DTOs.Generic;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Infrastructure.Implements.Repositories;
using NestF.Infrastructure.Implements.Services;
using Npgsql;
using Quartz;
using Quartz.Impl.AdoJobStore;
using StackExchange.Redis;

namespace NestF.Infrastructure;

public static class DepsInject
{
    public static void AddInfra(this IServiceCollection services, IConfiguration config)
        {
            // services.Configure<AppConfig>(GetAppConfig());
            var mailConfig = config.GetSection("SMTP").Get<EmailConfig>();
            services.AddSingleton(mailConfig!);
            //services.AddLocalization(opt => opt.ResourcesPath = GetFilePath(typeof(AppMessage)));
            //services.AddOptions<AppConfig>();
            services.AddOptions<EmailConfig>();
            //services.AddValidatorsFromAssemblyContaining<TravelerCreateValidator>();
            services.AddMapsterConfig();
            //ConfigJsonSerializer();
            services.AddSingleton<ITimeService, TimeService>();
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(config.GetConnectionString("CloudDb"));
            var dataSource = dataSourceBuilder.Build();
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource, o =>
            {
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                //o.EnableRetryOnFailure(3);
            }));
            services.Configure<QuartzOptions>(opt =>
            {
                opt.Scheduling.IgnoreDuplicates = true;
                opt.Scheduling.OverWriteExistingData = true;
            });
            services.AddQuartz(q =>
            {
               
                q.UsePersistentStore(
                    x =>
                    {
                        
                        x.UseClustering(c =>
                        {
                            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                        });
                        x.UsePostgres(
                            opt =>
                            {
                                opt.UseDriverDelegate<PostgreSQLDelegate>();
                                opt.ConnectionString = config.GetConnectionString("CloudDb")!;
                                opt.TablePrefix = "\"quartz\".qrtz_";
                            }, "default");
                        x.UseNewtonsoftJsonSerializer();
                    });
                q.SchedulerId = "NestF Scheduler";
                q.MisfireThreshold = TimeSpan.FromSeconds(15);
                q.MaxBatchSize = 100;
                q.UseDefaultThreadPool(250);
                //q.SetProperty("quartz.jobStore.acquireTriggersWithinLock", "true");
                //q.SetProperty("quartz.scheduler.batchTriggerAcquisitionMaxCount", "40");
                q.SetProperty("quartz.jobStore.maxMisfiresToHandleAtATime", "200");
                q.SetProperty("quartz.scheduler.idleWaitTime", "5000");
                q.SetProperty("quartz.plugin.recentHistory.type", "Quartz.Plugins.RecentHistory.ExecutionHistoryPlugin, Quartz.Plugins.RecentHistory");
                q.SetProperty("quartz.plugin.recentHistory.storeType", "Quartz.Plugins.RecentHistory.Impl.InProcExecutionHistoryStore, Quartz.Plugins.RecentHistory");
            });
            services.AddSingleton<IJobListener, JobFailureListener>();
            services.AddStackExchangeRedisCache(opts =>
            {
                opts.Configuration = config.GetConnectionString("Redis");
                opts.InstanceName = "Redis cache";
            });
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            services.AddScoped<IBackgroundService, QuartzBackgroundService>();
            services.AddScoped<QuartzBackgroundService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
        }
        // private static string GetFilePath(Type targetType)
        // {
        //     return Path.GetDirectoryName(Assembly.GetAssembly(targetType)!.Location)!;
        // }
        // private static IConfiguration GetAppConfig()
        // {
        //     var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
        //                                             .AddJsonFile(GlobalConstants.CONFIG_PATH, false, true);
        //     return builder.Build();
        // }
        private static void AddMapsterConfig(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
        }
        // private static void ConfigJsonSerializer()
        // {
        //     JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        //     {
        //         ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //         Converters = [new GeometryConverter(), new StringEnumConverter()],
        //         ContractResolver = new CamelCasePropertyNamesContractResolver()
        //     };
        // }
}