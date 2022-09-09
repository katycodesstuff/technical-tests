using CsvHelper.Configuration.Attributes;

namespace MeterReadingsUploader.Models.CsvRecords
{
    public record AccountRow(
        [Name("AccountId")] string? AccountId,
        [Name("FirstName")] string? FirstName,
        [Name("LastName")] string? LastName);
}
