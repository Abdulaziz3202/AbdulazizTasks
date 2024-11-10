namespace KPMGTask.Models
{
    public class FinancialAccount
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }                // Foreign Key
        public User User { get; set; }
        public double BalanceAmount { get; set; }
        public DateTime CreationDateTime { get; set; }
        // Navigation property: Each FinancialAmount can have many transactions
        public ICollection<TransactionHistory> TransactionHistories { get; set; }
    }
}
