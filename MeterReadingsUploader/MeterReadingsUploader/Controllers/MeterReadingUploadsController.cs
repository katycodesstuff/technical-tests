using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Mime;
using MeterReadingsUploader.Database.Repositories;
using MeterReadingsUploader.Domain;
using MeterReadingsUploader.Models.ApiResponse;
using MeterReadingsUploader.Models.CsvRecords;

namespace MeterReadingsUploader.Controllers
{
    [ApiController]
    [Route("meter-reading-uploads")]
    [Produces(MediaTypeNames.Application.Json)]
    public class MeterReadingUploadsController : ControllerBase
    {
        private readonly ILogger<MeterReadingUploadsController> _logger;

        public MeterReadingUploadsController(ILogger<MeterReadingUploadsController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "meter-reading-uploads")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post(
            [FromForm] IFormFile meterReadingCsvFile, 
            [FromServices]IMeterReadingsWriter writer,
            CancellationToken cancellationToken)
        {
            await using var file = meterReadingCsvFile.OpenReadStream();
            var fileName = meterReadingCsvFile.FileName;

            using var streamReader = new StreamReader(file);
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<MeterReadingRow>()?.ToArray();

            if (records is null || !records.Any())
            {
                return ValidationProblem(detail: $"No records were found in the uploaded CSV file \"{fileName}\"");
            }

            var result = await writer.AddToDatabase(records, cancellationToken);
            
            return Ok(new
            {
                result.Succeeded,
                result.Failed
            });
        }

        [HttpGet]
        [Route("{accountId}")]
        public IActionResult Get(int accountId, [FromServices]IMeterReadingRepository meterReadingRepository)
        {
            var meterReadingsForAccount = meterReadingRepository.GetForAccountId(accountId);
            return Ok(new
            {
                MeterReadings = meterReadingsForAccount
                    .Select(mr => new MeterReadingsDetails(mr.MeterReadingDateTime.ToString("f", CultureInfo.CurrentUICulture), mr.MeterReadValue)).OrderByDescending(mr => mr.EntryDate)
            });
        }

        [HttpGet]
        public IActionResult GetAll([FromServices] IMeterReadingRepository meterReadingRepository)
        {
            var meterReadingsForAccount = meterReadingRepository.GetAll();
            var accountMeterReadings = meterReadingsForAccount
                .GroupBy(mr => mr.AccountId)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(mr => 
                    new MeterReadingsDetails(
                        mr.MeterReadingDateTime.ToString("f", CultureInfo.CurrentUICulture), 
                        mr.MeterReadValue)).ToList());

            var response = new
            {
                AccountMeterReadings = accountMeterReadings
            };

            return Ok(response);
        }
    }
}