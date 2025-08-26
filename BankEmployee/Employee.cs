using BankUser;
using Contracts;



namespace BankClient
{
    public class Client : User
    {
        public int BranchId { get; }

        public Client(int branchId) => BranchId = branchId;

        // Ищет по BranchId
        public bool MatchesBranch(int branchId) => branchId == BranchId;

        // Одобрение кредитной заявки: отправляет ответный контракт (простое одобрение)
        // Использует тот же канал/очередь, что и User.SendOrderMessage
        public async Task ApproveLoanAsync(int loanId, int userId)
        {
            if (loanId <= 0) throw new ArgumentException(nameof(loanId));
            if (userId <= 0) throw new ArgumentException(nameof(userId));

            var approval = new FinancialOperationRequest
            {
                // Помечаем тип операции как RequestLoan — это ответ/подтверждение
                Operation = FinancialOperationRequest.OperationType.RequestLoan,
                // В поле AccountTypeId можно положить id заявки как строку или специальный маркер
                AccountTypeId = loanId.ToString(),
                Amount = 0m, // не обязательно
                TermMonths = null,
                InterestRate = null
            };

            // Если нужен явный маркер "approved", можно использовать ReplyQueue или добавить поле в BaseContract.
            // Здесь отправим в поле Amount = 0 и положим в ReplyQueue (родительский User установит его при отправке).
            await SendOrderMessage(approval);
        }
    }
}