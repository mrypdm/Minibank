using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.BankAccounts;
using MiniBank.Core.BankAccounts.Services;
using MiniBank.Core.Transfers;
using MiniBank.Web.Controllers.BankAccounts.Dto;

namespace MiniBank.Web.Controllers.BankAccounts
{
    [ApiController]
    [Authorize]
    [Route("Account")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _service;

        public BankAccountController(IBankAccountService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create new account
        /// </summary>
        [HttpPost]
        public Task CreateAccount(BankAccountCreateDto newAccountInfo, CancellationToken token)
        {
            return _service.Create(new BankAccount
            {
                UserId = newAccountInfo.UserId,
                CurrencyCode = newAccountInfo.CurrencyCode,
                Amount = newAccountInfo.Amount
            }, token);
        }

        /// <summary>
        /// Calculate transfer commission
        /// </summary>
        [HttpGet("calc-commission")]
        public Task<double> CalculateTransferCommission(double amount, string fromAccountId, string toAccountId,
            CancellationToken token)
        {
            return _service.CalculateTransferCommission(new Transfer
            {
                Amount = amount,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            }, token);
        }

        /// <summary>
        /// Make transfer between accounts
        /// </summary>
        [HttpPost("transfer")]
        public Task MakeTransfer(TransferCreateDto transferInfo, CancellationToken token)
        {
            return _service.MakeTransfer(new Transfer
            {
                Amount = transferInfo.Amount,
                FromAccountId = transferInfo.FromAccountId,
                ToAccountId = transferInfo.ToAccountId
            }, token);
        }

        /// <summary>
        /// Close account
        /// </summary>
        [HttpDelete("{accountId}")]
        public Task CloseAccountById(string accountId, CancellationToken token)
        {
            return _service.CloseById(accountId, token);
        }
    }
}