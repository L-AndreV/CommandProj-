using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankServer
{
    public class CommandHandler
    {
        private readonly Server _server;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<CommandHandler> _logger;

        public CommandHandler(Server server, IHostApplicationLifetime lifetime, ILogger<CommandHandler> logger)
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
