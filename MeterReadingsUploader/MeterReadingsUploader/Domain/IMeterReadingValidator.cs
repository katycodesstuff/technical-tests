using MeterReadingsUploader.Models.CsvRecords;

namespace MeterReadingsUploader.Domain;

public interface IMeterReadingValidator
{
    bool Validate(MeterReadingRow meterReadingRow);
}