namespace Contracts
{
    public class FinancialOperationRequest : BaseContract
    {
        public enum OperationType
        {
            CreateAccount,
            CreateDeposit,
            RequestLoan
        }
        public FinancialOperationRequest() => RequestType = nameof(AuthRequest);
        public OperationType Operation { get; set; }
        public string AccountTypeId { get; set; } // Условия кредита, вклада или счёта
        public int? WithdrawalAccountId { get; set; } // Для вклада
        public decimal Amount { get; set; }
        public int? TermMonths { get; set; }
        public decimal? InterestRate { get; set; }
    }
}
