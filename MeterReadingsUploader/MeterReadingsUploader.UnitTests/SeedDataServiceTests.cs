using MeterReadingsUploader.Database.EntityFramework;
using MeterReadingsUploader.Database.EntityFramework.Entities;
using MeterReadingsUploader.Database.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MeterReadingsUploader.UnitTests
{
    public class SeedDataServiceTests : IDisposable
    {
        private readonly IAccountRepository _mockAccountRepository;
        public SeedDataServiceTests()
        {
            _mockAccountRepository = Substitute.For<IAccountRepository>();
        }

        [Fact]
        public async Task GivenCsvFile_WhenSeedAccountsData_ThenDatabaseIsSeeded()
        {
            var seedDataService = new SeedDataService(_mockAccountRepository, Substitute.For<ILogger<SeedDataService>>());

            var csvData = @"AccountId,FirstName,LastName
                            2344,Tommy,Test
                            2233,Barry,Test
                            8766,Sally,Test";
            await File.WriteAllTextAsync("Test_Accounts.csv", csvData);

            var addedEntities = new List<AccountEntity>();
            _mockAccountRepository
                .WhenForAnyArgs(repo => repo.CreateMany(null!, CancellationToken.None))
                .Do(c =>
                {
                    addedEntities.AddRange(c.Arg<IEnumerable<AccountEntity>>());
                });

            await seedDataService.SeedAccountsData("Test_Accounts.csv", CancellationToken.None);

            await _mockAccountRepository.Received(1)
                .CreateMany(Arg.Any<IEnumerable<AccountEntity>>(), CancellationToken.None);
            Assert.Equal(3, addedEntities.Count);
            Assert.Equal(2344, addedEntities[0].Id);
            Assert.Equal(2233, addedEntities[1].Id);
            Assert.Equal(8766, addedEntities[2].Id);
        }

        public void Dispose()
        {
            if (File.Exists("Test_Accounts.csv"))
            {
                File.Delete("Test_Accounts.csv");
            }
        }
    }
}
