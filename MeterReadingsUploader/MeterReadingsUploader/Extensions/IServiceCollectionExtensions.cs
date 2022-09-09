using MeterReadingsUploader.Database.EntityFramework;
using MeterReadingsUploader.Database.Repositories;
using MeterReadingsUploader.Domain;

namespace MeterReadingsUploader.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IMeterReadingsWriter, MeterReadingsWriter>();
            serviceCollection.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
            serviceCollection.AddScoped<IAccountRepository, AccountRepository>();
            serviceCollection.AddScoped<IMeterReadingValidator, MeterReadingValidator>();
            serviceCollection.AddScoped<DatabaseContext>();
            serviceCollection.AddScoped<ISeedDataService, SeedDataService>();
        }

        public static void EnsureDatabaseIsCreated(this WebApplication app)
        {
            using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.Database.EnsureCreated();

            var filepath = app.Configuration.GetValue<string>("AccountDetailsSeedDataFilepath");
            serviceScope.ServiceProvider.GetRequiredService<ISeedDataService>()
                .SeedAccountsData(filepath, CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
