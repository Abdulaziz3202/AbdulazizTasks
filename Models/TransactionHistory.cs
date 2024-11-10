namespace KPMGTask.Models
{
    public class TransactionHistory
    {
        public Guid Id { get; set; }
        public double AmountBeforeTransaction { get; set; }
        public double AmountAfterTransaction { get; set; }
        public Guid FinancialAccountId { get; set; }
        public FinancialAccount FinancialAccount { get; set; }
        public DateTime TransactionDate { get; set; }
        public long TransactionTypeId { get; set; }
        public TransactionType TransactionType { get; set; } 



    }
}
