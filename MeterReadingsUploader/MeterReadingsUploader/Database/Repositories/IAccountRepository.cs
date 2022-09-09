using MeterReadingsUploader.Database.EntityFramework.Entities;

namespace MeterReadingsUploader.Database.Repositories;

public interface IAccountRepository
{
    Task<bool> CreateMany(IEnumerable<AccountEntity> accounts, CancellationToken cancellationToken);

    AccountEntity? Get(int accountId);

    IEnumerable<AccountEntity> GetAll();
}