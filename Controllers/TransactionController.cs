using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using KPMGTask.Services.TransactionServices;
using Microsoft.AspNetCore.Authorization;

namespace KPMGTask.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly TransactionServices _transactionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionController(TransactionServices transactionService, IHttpContextAccessor httpContextAccessor)
        {
            _transactionService = transactionService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("deposit")]
        public async Task<IActionResult> Deposit([FromBody] RequestDto request)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var result = await _transactionService.DepositAsync(request.Amount);
            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(500, "An error occurred during the deposit process.");
        }

        [HttpPost]
        [Route("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] RequestDto request)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var result = await _transactionService.WithdrawalAsync(request.Amount);
            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(500, "An error occurred during the withdrawal process.");
        }
    }
}
