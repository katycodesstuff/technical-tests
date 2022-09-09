using CsvHelper;
using System.Globalization;
using MeterReadingsUploader.Database.EntityFramework.Entities;
using MeterReadingsUploader.Database.Repositories;
using MeterReadingsUploader.Models.CsvRecords;

namespace MeterReadingsUploader.Database.EntityFramework
{
    public class SeedDataService : ISeedDataService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<SeedDataService> _logger;
        private static bool _hasSeeded = false;

        public SeedDataService(IAccountRepository accountRepository, ILogger<SeedDataService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task SeedAccountsData(string accountDataCsvFilePath, CancellationToken cancellationToken)
        {
            if (_hasSeeded)
            {
                return;
            }

            if (!File.Exists(accountDataCsvFilePath))
            {
                _logger.LogWarning("Could not find seed data at file path {FilePath}", accountDataCsvFilePath);
                return;
            }
            await using var file = File.OpenRead(accountDataCsvFilePath);
            using var streamReader = new StreamReader(file);
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<AccountRow>()?.ToArray();

            if (records is null)
            {
                _hasSeeded = true;
                return;
            }
            var entities = records
                .Where(IsValidAccountRow)
                .Select(r => new AccountEntity
            {
                FirstName = r.FirstName,
                LastName = r.LastName,
                Id = int.Parse(r.AccountId!)
            });

            if (_accountRepository.GetAll().Any())
            {
                _hasSeeded = true;
                return;
            }
            await _accountRepository.CreateMany(entities, cancellationToken);
            _logger.LogInformation("Successfully seeded accounts table with data from {FilePath}", accountDataCsvFilePath);
            _hasSeeded = true;
        }

        private bool IsValidAccountRow(AccountRow accountRow)
        {
            return accountRow.AccountId is not null 
                   && accountRow.FirstName is not null 
                   && accountRow.LastName is not null
                   && int.TryParse(accountRow.AccountId, out _);
        }
    }
}
