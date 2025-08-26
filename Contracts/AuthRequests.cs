namespace Contracts
{
    public class RegisterRequest : BaseContract //Запрос для регистрации
    {
        public RegisterRequest(bool isEmployee = false) //По умолчанию - false
        {
            ContractType = nameof(RegisterRequest);
            this.isEmployee = isEmployee;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int CountryId { get; set; }
        public bool isEmployee { get; set; } //true - если данные запрашивает сотрудник
    }
    public class LoginRequest : BaseContract //Запрос для входа
    {
        public LoginRequest(bool isEmployee = false) //По умолчанию - false
        {
            ContractType = nameof(LoginRequest);
            this.isEmployee = isEmployee;
        }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool isEmployee { get; set; } //true - если данные запрашивает сотрудник
    }
}
