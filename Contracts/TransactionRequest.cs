namespace Contracts
{
    public class TransactionRequest : BaseContract // Запрос для проведения транзакции
    {
        public TransactionRequest() => ContractType = nameof(TransactionRequest);
        public int SendertAccountId { get; set; }
        public int RecipientAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
    }
}
