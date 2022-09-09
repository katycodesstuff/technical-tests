using MeterReadingsUploader.Models.CsvRecords;

namespace MeterReadingsUploader.Domain;

public interface IMeterReadingsWriter
{
    Task<(int Succeeded, int Failed)> AddToDatabase(ICollection<MeterReadingRow> meterReadingRows,
        CancellationToken cancellationToken);
}