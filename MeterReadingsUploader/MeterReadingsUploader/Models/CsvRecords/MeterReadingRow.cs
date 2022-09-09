using CsvHelper.Configuration.Attributes;

namespace MeterReadingsUploader.Models.CsvRecords
{
    public record MeterReadingRow(
        [Name("AccountId")] string? AccountId,
        [Name("MeterReadingDateTime")] string? MeterReadingDateTime,
        [Name("MeterReadValue")] string? MeterReadValue);
}
