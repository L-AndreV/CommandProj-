namespace Contracts
{
    public class GetDataRequest : BaseContract //Запрос информации(если нужно, можно добавить свойство или даже enum для конкретики получаемой инфы)
    {
        public bool isEmployee { get; set; } //true - если данные запрашивает сотрудник
        public GetDataRequest(bool isEmployee = false)//По умолчанию нет
        {
            ContractType = nameof(GetDataRequest);
            this.isEmployee = isEmployee;
        }
    }
}
