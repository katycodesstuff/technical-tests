using MeterReadingsUploader.Database.EntityFramework;
using MeterReadingsUploader.Database.EntityFramework.Entities;

namespace MeterReadingsUploader.Database.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly DatabaseContext _dbContext;

    public AccountRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateMany(IEnumerable<AccountEntity> accounts, CancellationToken cancellationToken)
    {
        await _dbContext.Accounts.AddRangeAsync(accounts, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public AccountEntity? Get(int accountId)
    {
        return _dbContext.Accounts.Find(accountId);
    }

    public IEnumerable<AccountEntity> GetAll()
    {
        return _dbContext.Accounts.AsEnumerable();
    }
}