namespace Contracts
{
    public abstract class BaseContract //Базовый контракт(и запрос, и ответ)
    {
        public string ContractType { get; set; } //Тип контракта(обязательно заполнять в каждом классе, нужен для обработки)
        public string ReplyQueue { get; set; } //Очередь для ответа(только если нужен ответ)
        public Guid SessionToken { get; set; } //Токен сессии(обязательно заполнять для идентификации пользователя)
    }
}

