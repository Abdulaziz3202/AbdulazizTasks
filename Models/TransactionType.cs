namespace KPMGTask.Models
{
    public class TransactionType
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public ICollection<TransactionHistory> TransactionHistories { get; set; }
    }
}
