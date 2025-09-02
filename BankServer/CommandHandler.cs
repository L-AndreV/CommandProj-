using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankServer
{
    public class CommandHandler//Как я понял, так как сервер работает через хост
    {
        //Напрямую мы его экземпляр получить не можем
        //И поэтому для управления им мы используем команды
        //А так команды здесь - это простой вызов методов сервера
        private readonly Server _server;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<CommandHandler> _logger;

        public CommandHandler(Server server, IHostApplicationLifetime lifetime, ILogger<CommandHandler> logger)//Сюда данные попадают непосредственно из хоста
        {
            _server = server;
            _lifetime = lifetime;
            _logger = logger;
        }
        public void ChangeProvider(ServerOptions options)
        {
            _ = _server.ChangeProviderAsync(options);
        }
        public ServerStatus GetStatus()
        {
            return _server.GetStatus();
        }
        public async Task<HealthStatus> GetHealth()
        {
            return await _server.GetHealth();
        }
        public void PauseResume(bool isResume)
        {
            if (isResume) _server.PauseQueue();
            else _server.ResumeQueue();
        }
        public void Stop()
        {
            _lifetime.StopApplication();
        }
    }
}
