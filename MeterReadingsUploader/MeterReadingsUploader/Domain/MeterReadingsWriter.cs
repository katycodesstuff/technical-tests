using MeterReadingsUploader.Database.EntityFramework.Entities;
using MeterReadingsUploader.Database.Repositories;
using MeterReadingsUploader.Models.CsvRecords;

namespace MeterReadingsUploader.Domain
{
    public class MeterReadingsWriter : IMeterReadingsWriter
    {
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IMeterReadingValidator _validator;
        private readonly ILogger<MeterReadingsWriter> _logger;

        public MeterReadingsWriter(IMeterReadingRepository meterReadingRepository, 
            IMeterReadingValidator validator,
            ILogger<MeterReadingsWriter> logger)
        {
            _meterReadingRepository = meterReadingRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<(int Succeeded, int Failed)> AddToDatabase(
            ICollection<MeterReadingRow> meterReadingRows,
            CancellationToken cancellationToken)
        {
            var meterReadingsToAdd = new List<MeterReadingEntity>();
            foreach (var meterReading in meterReadingRows)
            {
                if (!_validator.Validate(meterReading))
                {
                    continue;
                }
                var entity = MeterReadingEntity.Create(
                    meterReading.AccountId!, 
                    meterReading.MeterReadingDateTime!,
                    meterReading.MeterReadValue!);

                var existingEntry = meterReadingsToAdd
                    .SingleOrDefault(mr =>
                        mr.MeterReadingDateTime == entity.MeterReadingDateTime 
                        && mr.AccountId == entity.AccountId);

                if (existingEntry is not null)
                {
                    _logger.LogWarning("Entry already processed for account ID {AccountId} on date {EntryDate} with value {MeterReadValue}. Ignoring repeated row with value {MeterReadValue}.", entity.AccountId, entity.MeterReadingDateTime.ToString("g"), existingEntry.MeterReadValue, entity.MeterReadValue);
                }
                else
                {
                    meterReadingsToAdd.Add(entity);
                }
            }

            await _meterReadingRepository.CreateMany(meterReadingsToAdd, cancellationToken);

            return (meterReadingsToAdd.Count, meterReadingRows.Count - meterReadingsToAdd.Count);
        }
    }
}
