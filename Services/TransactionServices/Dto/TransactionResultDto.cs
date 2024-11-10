namespace KPMGTask.Services.TransactionServices.Dto
{
    public class TransactionResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public double Amount { get; set; }
    }
}
