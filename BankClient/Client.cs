using BankUser;
using Contracts;



namespace BankClient
{
    public class Client : User
    {
        public int ClientId { get; }

        public Client(int ClientId)
        {
            this.ClientId = ClientId;
        }

        // Ищет только по своему Id
        public bool MatchesId(int id) => id == ClientId;

        public async Task RequestCreateAccountAsync(string accountTypeId, decimal initialAmount = 0m, string currency = null)
        {
            var req = new FinancialOperationRequest
            {
                Operation = FinancialOperationRequest.OperationType.CreateAccount,
                AccountTypeId = accountTypeId,
                Amount = initialAmount,
            };
            // унаследованный метод из User устанавливает ReplyQueue и отправляет
            await SendOrderMessage(req);
        }

        public async Task RequestCreateDepositAsync(string accountTypeId, int withdrawalAccountId, decimal amount, int termMonths, decimal interestRate)
        {
            var req = new FinancialOperationRequest
            {
                Operation = FinancialOperationRequest.OperationType.CreateDeposit,
                AccountTypeId = accountTypeId,
                WithdrawalAccountId = withdrawalAccountId,
                Amount = amount,
                TermMonths = termMonths,
                InterestRate = interestRate
            };
            await SendOrderMessage(req);
        }

        public async Task RequestLoanAsync(string accountTypeId, decimal amount, int termMonths, decimal? interestRate = null)
        {
            var req = new FinancialOperationRequest
            {
                Operation = FinancialOperationRequest.OperationType.RequestLoan,
                AccountTypeId = accountTypeId,
                Amount = amount,
                TermMonths = termMonths,
                InterestRate = interestRate
            };
            await SendOrderMessage(req);
        }
    }

}