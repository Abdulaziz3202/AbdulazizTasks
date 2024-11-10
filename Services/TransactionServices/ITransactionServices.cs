using KPMGTask.Services.TransactionServices.Dto;

namespace KPMGTask.Services.TransactionServices
{
    public interface ITransactionServices
    {
        public Task<TransactionResultDto> WithdrawalAsync(double amount);

        public Task<TransactionResultDto> DepositAsync(double amount);

    }
}
