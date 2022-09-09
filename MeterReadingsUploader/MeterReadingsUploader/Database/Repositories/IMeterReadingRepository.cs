using MeterReadingsUploader.Database.EntityFramework.Entities;

namespace MeterReadingsUploader.Database.Repositories;

public interface IMeterReadingRepository
{
    Task<bool> CreateMany(IEnumerable<MeterReadingEntity> meterReadings, CancellationToken cancellationToken);

    IEnumerable<MeterReadingEntity> GetForAccountId(int accountId);
    IEnumerable<MeterReadingEntity> GetAll();
}