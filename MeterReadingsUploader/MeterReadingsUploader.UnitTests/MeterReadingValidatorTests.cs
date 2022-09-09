using MeterReadingsUploader.Database.EntityFramework.Entities;
using MeterReadingsUploader.Database.Repositories;
using MeterReadingsUploader.Domain;
using MeterReadingsUploader.Models.CsvRecords;
using NSubstitute;

namespace MeterReadingsUploader.UnitTests
{
    public class MeterReadingValidatorTests
    {
        private readonly IAccountRepository _mockAccountRepository;
        private readonly IMeterReadingRepository _mockMeterReadingRepository;

        public MeterReadingValidatorTests()
        {
            _mockMeterReadingRepository = Substitute.For<IMeterReadingRepository>();
            _mockAccountRepository = Substitute.For<IAccountRepository>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("string")]
        public void GivenInvalidAccountId_WhenValidate_ThenReturnsFalse(string? accountId)
        {
            var invalidMeterReadingRow = new MeterReadingRow(accountId, "08/09/2022 15:52", "12345");

            SetUpAccountRepository();

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(invalidMeterReadingRow);

            Assert.False(result);
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("9876")]
        public void GivenValidAccountId_WhenValidate_ThenReturnsTrue(string? accountId)
        {
            var validMeterReading = new MeterReadingRow(accountId, "08/09/2022 15:52", "12345");

            SetUpAccountRepository(int.Parse(accountId!));

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(validMeterReading);

            Assert.True(result);
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("9876")]
        public void GivenValidAccountId_ButNoAssociatedAccount_WhenValidate_ThenReturnsFalse(string? accountId)
        {
            var validMeterReading = new MeterReadingRow(accountId, "08/09/2022 15:52", "12345");

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(validMeterReading);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("2022-05-04 12:45")]
        public void GivenInvalidDateTime_WhenValidate_ThenReturnsFalse(string? dateTime)
        {
            var invalidMeterReadingRow = new MeterReadingRow("1234", dateTime, "12345");

            SetUpAccountRepository(1234);

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(invalidMeterReadingRow);

            Assert.False(result);
        }

        [Theory]
        [InlineData("04/05/2021 12:45")]
        public void GivenValidDateTime_WhenValidate_ThenReturnsFalse(string? dateTime)
        {
            var meterReadingRow = new MeterReadingRow("1234", dateTime, "12345");

            SetUpAccountRepository(1234);

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(meterReadingRow);

            Assert.True(result);
        }

        [Fact]
        public void GivenMeterReadingAlreadyExistsForAccountWhichIsNewer_WhenValidate_ThenReturnsFalse()
        {
            var meterReadingRow = new MeterReadingRow("1234", "01/01/2022 12:00", "12345");

            SetUpAccountRepository(1234);
            SetUpMeterReadingForAccount(1234, new MeterReadingRow("1234", "02/01/2022 12:00", "13456"));

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(meterReadingRow);

            Assert.False(result);
        }

        [Fact]
        public void GivenMeterReadingAlreadyExistsForAccountWhichIsOlder_WhenValidate_ThenReturnsTrue()
        {
            var meterReadingRow = new MeterReadingRow("1234", "03/01/2022 12:00", "12345");

            SetUpAccountRepository(1234);
            SetUpMeterReadingForAccount(1234, new MeterReadingRow("1234", "02/01/2022 12:00", "13456"));

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(meterReadingRow);

            Assert.True(result);
        }

        [Fact]
        public void GivenMeterReadingAlreadyExistsForAccountWhichIsTheSameDate_WhenValidate_ThenReturnsFalse()
        {
            var meterReadingRow = new MeterReadingRow("1234", "01/01/2022 12:00", "12345");

            SetUpAccountRepository(1234);
            SetUpMeterReadingForAccount(1234, new MeterReadingRow("1234", "01/01/2022 12:00", "13456"));

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(meterReadingRow);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1000")]
        public void GivenInvalidMeterReadingValue_WhenValidate_ThenReturnsFalse(string? meterReadingValue)
        {
            var invalidMeterReadingRow = new MeterReadingRow("1234", "01/01/2022 12:00", meterReadingValue);

            SetUpAccountRepository(1234);

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(invalidMeterReadingRow);

            Assert.False(result);
        }

        [Theory]
        [InlineData("00001")]
        [InlineData("12345")]
        [InlineData("01005")]
        public void GivenValidMeterReadingValue_WhenValidate_ThenReturnsTrue(string? meterReadingValue)
        {
            var meterReadingRow = new MeterReadingRow("1234", "01/01/2022 12:00", meterReadingValue);

            SetUpAccountRepository(1234);

            var validator = new MeterReadingValidator(_mockAccountRepository, _mockMeterReadingRepository);

            var result = validator.Validate(meterReadingRow);

            Assert.True(result);
        }

        private void SetUpMeterReadingForAccount(int accountId, MeterReadingRow meterReading)
        {
            _mockMeterReadingRepository.GetForAccountId(accountId)
                .Returns(new List<MeterReadingEntity> 
                { 
                    MeterReadingEntity.Create(accountId.ToString(), meterReading.MeterReadingDateTime, meterReading.MeterReadValue)
                });
        }

        private void SetUpAccountRepository(int? accountNumber = null)
        {
            if (accountNumber.HasValue)
            {
                _mockAccountRepository.Get(accountNumber.Value).Returns(c => new AccountEntity
                {
                    Id = accountNumber.Value,
                    FirstName = "John",
                    LastName = "Test"
                });
                return;
            }

            _mockAccountRepository.Get(Arg.Any<int>()).Returns(c => new AccountEntity
            {
                Id = c.Arg<int>(),
                FirstName = "John",
                LastName = "Test"
            });
        }
    }
}