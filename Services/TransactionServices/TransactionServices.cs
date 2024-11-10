using AutoMapper;
using KPMGTask.EntityFrameworkCore;
using KPMGTask.Models;
using KPMGTask.Services.TransactionServices.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KPMGTask.Services.TransactionServices
{
    public class TransactionServices : ITransactionServices
    {

        private readonly AppDBContextContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly KPMGTask.Middlewares.WebSocketManager _socketManager; // Inject WebSocketManager

        public TransactionServices(IHttpContextAccessor httpContextAccessor, AppDBContextContext context, IMapper mapper, KPMGTask.Middlewares.WebSocketManager socketManager)
        {
            _httpContextAccessor = httpContextAccessor;

            _context = context;
            _mapper = mapper;
            _socketManager = socketManager;
        }


        public async Task<TransactionResultDto> DepositAsync(double amount)
        {
            try
            {

                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;


                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }
                var user = _context.Users.Include(x => x.FinancialAccount).FirstOrDefault(u => u.Id.ToString().Equals(userId));

                if (user == null)
                {
                    throw new Exception("User not found.");
                }
                var TransactionDate= DateTime.Now;
                var transactionHistory=new TransactionHistory
                {

                    AmountBeforeTransaction = user.FinancialAccount.BalanceAmount,
                    AmountAfterTransaction = (user.FinancialAccount.BalanceAmount + amount),
                    FinancialAccountId = user.FinancialAccount.Id,
                    TransactionDate = TransactionDate,
                    TransactionTypeId = 1
                };

                await _context.TransactionHistory.AddAsync(transactionHistory);


                user.FinancialAccount.BalanceAmount += amount;
                var financialAccount = user.FinancialAccount;

                _context.FinancialAccount.Update(financialAccount);
               

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var notificationMessage = $"Deposit of {amount} successful. New Balance: {user.FinancialAccount.BalanceAmount}";
                    await _socketManager.SendToUser(userId, notificationMessage); // Send notification to user via WebSocket
                    await AddUserNotification(new Guid(userId), notificationMessage);

                    return new TransactionResultDto
                    {
                        Amount = user.FinancialAccount.BalanceAmount,
                        TransactionType = "Deposite",
                        TransactionDate= TransactionDate

                    };
                }
                else
                {
                    var notificationMessage = $"Deposit failed. Please try again.";
                    await _socketManager.SendToUser(userId, notificationMessage); // Send notification to user via WebSocket
                    await AddUserNotification(new Guid(userId), notificationMessage);


                    return new TransactionResultDto
                    {
                        Amount = (user.FinancialAccount.BalanceAmount-amount),
                        TransactionType = "Failed Deposite",
                        TransactionDate = TransactionDate

                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TransactionResultDto> WithdrawalAsync(double amount)
        {
            try
            {

                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }


                var user = _context.Users.Include(x => x.FinancialAccount).FirstOrDefault(u => u.Id.ToString().Equals(userId));

                if (user.FinancialAccount.BalanceAmount < amount)
                {
                    var notificationMessage = "Withdrawal declined: Insufficient balance.";
                    await _socketManager.SendToUser(userId, notificationMessage); // Send notification to user via WebSocket
                    await AddUserNotification(new Guid(userId), notificationMessage);

                    return new TransactionResultDto
                    {
                        Success = false,
                        Message = "Withdrawal declined: Insufficient balance."
                    };
                }


                if (user == null)
                {
                    throw new Exception("User not found.");
                }
                var TransactionDate = DateTime.Now;
                var transactionHistory = new TransactionHistory
                {

                    AmountBeforeTransaction = user.FinancialAccount.BalanceAmount,
                    AmountAfterTransaction = (user.FinancialAccount.BalanceAmount - amount),
                    FinancialAccountId = user.FinancialAccount.Id,
                    TransactionDate = TransactionDate,
                    TransactionTypeId = 2
                };

                await _context.TransactionHistory.AddAsync(transactionHistory);


                user.FinancialAccount.BalanceAmount -= amount;


                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {

                    var notificationMessage = $"Withdrawal of {amount} successful. New Balance: {user.FinancialAccount.BalanceAmount}";
                    await _socketManager.SendToUser(userId, notificationMessage); // Send notification to user via WebSocket
                    await AddUserNotification(new Guid(userId), notificationMessage);


                    return new TransactionResultDto
                    {
                        Amount = user.FinancialAccount.BalanceAmount,
                        TransactionType = "Withdraw",
                        TransactionDate = TransactionDate

                    };
                }
                else
                {
                    var notificationMessage = $"Withdrawal failed. Please try again.";
                    await _socketManager.SendToUser(userId, notificationMessage); // Send notification to user via WebSocket
                    await AddUserNotification(new Guid(userId), notificationMessage);


                    return new TransactionResultDto
                    {
                        Amount = (user.FinancialAccount.BalanceAmount + amount),
                        TransactionType = "Failed Withdraw",
                        TransactionDate = TransactionDate

                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        private async Task AddUserNotification(Guid userId, string message)
        {
            try
            {

           
            var notification = new UserNotification
            {
               
                Text = message,
                CreationDateTime = DateTime.Now,
                UserId=userId
            };

            await _context.UserNotification.AddAsync(notification);
            await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
