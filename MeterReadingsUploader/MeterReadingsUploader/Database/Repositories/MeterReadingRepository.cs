using MeterReadingsUploader.Database.EntityFramework;
using MeterReadingsUploader.Database.EntityFramework.Entities;

namespace MeterReadingsUploader.Database.Repositories;

public class MeterReadingRepository : IMeterReadingRepository
{
    private readonly DatabaseContext _dbContext;

    public MeterReadingRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateMany(IEnumerable<MeterReadingEntity> meterReadings, CancellationToken cancellationToken)
    {
        await _dbContext.MeterReadings.AddRangeAsync(meterReadings, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public IEnumerable<MeterReadingEntity> GetForAccountId(int accountId)
    {
        return _dbContext.MeterReadings.Where(mr => mr.AccountId == accountId).AsEnumerable();
    }

    public IEnumerable<MeterReadingEntity> GetAll()
    {
        return _dbContext.MeterReadings.AsEnumerable();
    }
}