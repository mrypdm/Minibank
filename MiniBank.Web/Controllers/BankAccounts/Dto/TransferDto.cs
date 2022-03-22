namespace MiniBank.Web.Controllers.BankAccounts.Dto
{
    public class TransferCreateDto
    {
        public double Amount { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
    }
}