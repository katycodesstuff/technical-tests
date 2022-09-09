using System.Globalization;
using System.Text.RegularExpressions;
using MeterReadingsUploader.Database.Repositories;
using MeterReadingsUploader.Models.CsvRecords;

namespace MeterReadingsUploader.Domain
{
    public class MeterReadingValidator : IMeterReadingValidator
    {
        private const string ValidDateFormat = "dd/MM/yyyy HH:mm";
        private const string ValidMeterReadRegex = "^\\d{5}$";

        private readonly IAccountRepository _accountRepository;
        private readonly IMeterReadingRepository _meterReadingRepository;

        public MeterReadingValidator(IAccountRepository accountRepository, IMeterReadingRepository meterReadingRepository)
        {
            _accountRepository = accountRepository;
            _meterReadingRepository = meterReadingRepository;
        }

        public bool Validate(MeterReadingRow meterReadingRow)
        {
            if (!IsValidAccountId(meterReadingRow, out var accountId))
            {
                return false;
            }

            if (!IsValidEntryDate(meterReadingRow, out var entryDate))
            {
                return false;
            }

            if (!IsValidMeterReading(meterReadingRow, out _))
            {
                return false;
            }

            var existingMeterReadingsForAccount = _meterReadingRepository.GetForAccountId(accountId).ToArray();
            if (existingMeterReadingsForAccount.Length > 0 
                && (existingMeterReadingsForAccount.Any(mr => mr.MeterReadingDateTime == entryDate)
                || existingMeterReadingsForAccount.Max(mr => mr.MeterReadingDateTime > entryDate)))
            {
                return false;
            }

            return true;
        }

        private static bool IsValidEntryDate(MeterReadingRow meterReadingRow, out DateTime entryDate)
        {
            entryDate = DateTime.MinValue;
            return meterReadingRow.MeterReadingDateTime is not null
                    && DateTime.TryParseExact(meterReadingRow.MeterReadingDateTime, ValidDateFormat, null, DateTimeStyles.None, out entryDate);
        }

        private bool IsValidAccountId(MeterReadingRow meterReadingRow, out int accountId)
        {
            accountId = 0;
            return meterReadingRow.AccountId is not null 
                     && int.TryParse(meterReadingRow.AccountId, out accountId)
                     && AccountExists(accountId);
        }

        private bool IsValidMeterReading(MeterReadingRow meterReadingRow, out int meterReading)
        {
            meterReading = 0;
            return meterReadingRow.MeterReadValue is not null
                   && Regex.Match(meterReadingRow.MeterReadValue, ValidMeterReadRegex).Success
                   && int.TryParse(meterReadingRow.MeterReadValue, out meterReading);
        }

        private bool AccountExists(int accountId)
        {
            return _accountRepository.Get(accountId) is not null;
        }
    }
}
