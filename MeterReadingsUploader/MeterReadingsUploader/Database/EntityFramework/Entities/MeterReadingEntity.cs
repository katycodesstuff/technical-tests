using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace MeterReadingsUploader.Database.EntityFramework.Entities
{
    public class MeterReadingEntity
    {
        [Key] 
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public int MeterReadValue { get; set; }

        [ForeignKey("AccountId")]
        public virtual AccountEntity? Account { get; set; }

        public static MeterReadingEntity Create(string accountId, string dateTime, string meterReadValue)
        {
            return new MeterReadingEntity
            {
                AccountId = int.Parse(accountId),
                MeterReadValue = int.Parse(meterReadValue),
                MeterReadingDateTime = DateTime.ParseExact(dateTime, "dd/MM/yyyy HH:mm", null, DateTimeStyles.AdjustToUniversal)
            };
        }
    }
}
