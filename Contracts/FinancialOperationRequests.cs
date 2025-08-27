namespace Contracts
{
    public class LoanApplicationRequest : BaseContract //Запрос на одобрение кредита
    {
        public LoanApplicationRequest() => ContractType = nameof(LoanApplicationRequest);
        public int Amount { get; set; }
        public int AccountId { get; set; }
        public int BranchId { get; set; }
    }
        public class CreateAccountRequest : BaseContract //Запрос для создания счёта
    {
        public CreateAccountRequest() => ContractType = nameof(CreateAccountRequest);
    }
    public class CreateDepositRequest : BaseContract
    {
        public CreateDepositRequest() => ContractType = nameof(CreateDepositRequest); //Запрос для для создания вклада
        public decimal Amount { get; set; }
        public int AccountId { get; set; } //Id счёта снятия(обязательно заполнить, иначе не сработает)
        public int BranchId { get; set; }
    }
}
