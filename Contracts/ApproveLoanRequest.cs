namespace Contracts
{
    public class ApproveLoanRequest : BaseContract //Запрос для одобрения кредита(для сотрудника)
    {
        public ApproveLoanRequest() => ContractType = nameof(ApproveLoanRequest);
        public int LoanId { get; set; }
        public string Status {  get; set; }
    }
}
