namespace Mango.Services.OrderAPI.Models.Dto
{
    public class TransactionLogDto
    {
        public long StartTime { get; set; }
        public int TimeSpent { get; set; }
        public int Attempts { get; set; }
        public int Errors { get; set; }
        public bool Success { get; set; }
        public bool Mobile { get; set; }
        public List<object> Input { get; set; }
        public List<LogHistoryDto> History { get; set; }
    }
}
