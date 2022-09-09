using MeterReadingsUploader.Database.EntityFramework.Entities;

namespace MeterReadingsUploader.Database.EntityFramework;

public interface ISeedDataService
{
    Task SeedAccountsData(string accountDataCsvFilePath, CancellationToken cancellationToken);
}