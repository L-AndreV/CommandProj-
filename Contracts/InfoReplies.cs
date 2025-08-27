namespace Contracts
{
    //Это примеры ответов. Их нужно самому заполнить, чтобы я знал, какую информацию отправлять
    public class AuthReply : BaseContract
    {
        public AuthReply() => ContractType = nameof(AuthReply);
        public bool IsAuthorized { get; set; }
        public string errorMessage { get; set; }
        //Поле с токеном сессии вшито в базовый контракт(сюда добавлять не надо)
    }
    public class ClientInfoReply : BaseContract
    {
        public ClientInfoReply() => ContractType = nameof(ClientInfoReply); 
    }
    public class EmployeeInfoReply : BaseContract
    {
        public EmployeeInfoReply() => ContractType = nameof(EmployeeInfoReply);
    }
    public class LoanInfoReply : BaseContract
    {
        public LoanInfoReply() => ContractType = nameof(LoanInfoReply);
    }
    //Здесь не все ответы. Если нужны ещё какие-то, нужно добавить и заполнить самому
    //В целом если нужно, то можно редактировать все контракты, только нужно сообщить об этом мне
}
