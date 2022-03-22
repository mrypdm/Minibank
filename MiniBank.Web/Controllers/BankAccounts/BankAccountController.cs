using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.BankAccounts;
using MiniBank.Core.BankAccounts.Services;
using MiniBank.Core.Transfers;
using MiniBank.Web.Controllers.BankAccounts.Dto;

namespace MiniBank.Web.Controllers.BankAccounts
{
    [ApiController]
    [Route("Account")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _service;

        public BankAccountController(IBankAccountService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all accounts
        /// </summary>
        [HttpGet]
        public IEnumerable<BankAccountDto> GetAll()
        {
            return _service.GetAll().Select(ac => new BankAccountDto
            {
                Id = ac.Id,
                UserId = ac.UserId,
                Amount = ac.Amount,
                CurrencyCode = ac.CurrencyCode,
                OpeningDate = ac.OpeningDate,
                ClosingDate = ac.ClosingDate,
                IsClosed = ac.IsClosed,
            });
        }

        /// <summary>
        /// Create new account
        /// </summary>
        [HttpPost]
        public void CreateAccount(BankAccountCreateDto newAccountInfo)
        {
            _service.Create(new BankAccount
            {
                UserId = newAccountInfo.UserId,
                CurrencyCode = newAccountInfo.CurrencyCode,
                Amount = newAccountInfo.Amount
            });
        }

        /// <summary>
        /// Calculate transfer commission
        /// </summary>
        [HttpGet("calc-commission")]
        public double CalculateTransferCommission(double amount, string fromAccountId, string toAccountId)
        {
            return _service.CalculateTransferCommission(new Transfer
            {
                Amount = amount,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            });
        }

        /// <summary>
        /// Make transfer between accounts
        /// </summary>
        [HttpPost("transfer")]
        public void MakeTransfer(TransferCreateDto transferInfo)
        {
            _service.MakeTransfer(new Transfer
            {
                Amount = transferInfo.Amount,
                FromAccountId = transferInfo.FromAccountId,
                ToAccountId = transferInfo.ToAccountId
            });
        }

        /// <summary>
        /// Close account
        /// </summary>
        [HttpDelete("{accountId}")]
        public void CloseAccountById(string accountId)
        {
            _service.CloseById(accountId);
        }
    }
}