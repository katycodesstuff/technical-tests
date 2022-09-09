using MeterReadingsUploader.Database.EntityFramework.Entities;
using MeterReadingsUploader.Database.Repositories;
using MeterReadingsUploader.Domain;
using MeterReadingsUploader.Models.CsvRecords;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MeterReadingsUploader.UnitTests
{
    public class MeterReadingsWriterTests
    {
        private readonly IMeterReadingRepository _mockMeterReadingRepository;
        private readonly IMeterReadingValidator _mockMeterReadingValidator;

        public MeterReadingsWriterTests()
        {
            _mockMeterReadingRepository = Substitute.For<IMeterReadingRepository>();
            _mockMeterReadingValidator = Substitute.For<IMeterReadingValidator>();
        }

        [Fact]
        public async Task GivenTwoConflictingMeterReadings_WhenAddToDatabase_ThenOnlyFirstOccurrenceIsPersisted()
        {
            var writer = new MeterReadingsWriter(_mockMeterReadingRepository, _mockMeterReadingValidator, Substitute.For<ILogger<MeterReadingsWriter>>());
            var addedEntities = new List<MeterReadingEntity>();

            _mockMeterReadingValidator.Validate(Arg.Any<MeterReadingRow>()).Returns(true);
            _mockMeterReadingRepository
                .When(repo => repo.CreateMany(Arg.Any<ICollection<MeterReadingEntity>>(), Arg.Any<CancellationToken>()))
                .Do(c =>
                {
                    var entities = c.Arg<ICollection<MeterReadingEntity>>();
                    addedEntities.AddRange(entities);
                });
            var result = await writer.AddToDatabase(new List<MeterReadingRow>
            {
                new ("1234", "01/01/2022 12:00", "12345"),
                new ("1234", "01/01/2022 12:00", "23456")
            }, CancellationToken.None);

            await _mockMeterReadingRepository.Received(1).CreateMany(
                Arg.Any<ICollection<MeterReadingEntity>>(),
                Arg.Any<CancellationToken>());

            Assert.Equal(1, result.Failed);
            Assert.Equal(1, result.Succeeded);
            Assert.Single(addedEntities);
            Assert.Equal(12345, addedEntities[0].MeterReadValue);
        }

        [Fact]
        public async Task GivenAListOfValidMeterReadings_WhenAddToDatabase_ThenReadingsArePersisted()
        {
            var writer = new MeterReadingsWriter(_mockMeterReadingRepository, _mockMeterReadingValidator, Substitute.For<ILogger<MeterReadingsWriter>>());
            var addedEntities = new List<MeterReadingEntity>();

            _mockMeterReadingValidator.Validate(Arg.Any<MeterReadingRow>()).Returns(true);
            _mockMeterReadingRepository
                .When(repo => repo.CreateMany(Arg.Any<ICollection<MeterReadingEntity>>(), Arg.Any<CancellationToken>()))
                .Do(c =>
                {
                    var entities = c.Arg<ICollection<MeterReadingEntity>>();
                    addedEntities.AddRange(entities);
                });
            var result = await writer.AddToDatabase(new List<MeterReadingRow>
            {
                new ("2345", "01/01/2022 12:00", "12345"),
                new ("1234", "01/01/2022 12:00", "23456"),
                new ("3456", "02/01/2022 13:00", "00001"),
            }, CancellationToken.None);

            await _mockMeterReadingRepository.Received(1).CreateMany(
                Arg.Any<ICollection<MeterReadingEntity>>(),
                Arg.Any<CancellationToken>());

            Assert.Equal(0, result.Failed);
            Assert.Equal(3, result.Succeeded);
            Assert.Equal(3, addedEntities.Count);
        }
    }
}
