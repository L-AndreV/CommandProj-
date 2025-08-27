using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Contracts;
using CommandProj.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace BankServer
{
    public class Server : IHostedService, IDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ILogger<Server> _logger;
        private ServerOptions _options;
        private BankContext _context;
        private readonly DateTime _startTime;
        private bool _isPaused = false;
        private long _processedMessages = 0;
        private readonly object _lock = new object();
        private readonly object _configLock = new object();
        private readonly TokenManager _tokenManager;
        private readonly Random _random = new Random();
        public Server(ILogger<Server> logger, IOptions<ServerOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _startTime = DateTime.UtcNow;
            _context = new(_options.Provider);
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(options: null, cancellationToken: cancellationToken);

                await _channel.QueueDeclareAsync("order_queue", false, false, false, null);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                        await ProcessMessageAsync(json, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                };

                await _channel.BasicConsumeAsync("order_queue", true, consumer);
                _logger.LogInformation("Consumer слушает очередь order_queue...");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Критическая ошибка при запуске Consumer");
                throw;
            }
        }
        private async Task SendReplyAsync(BaseContract reply, BaseContract originalRequest)
        {
            try
            {

                var json = JsonConvert.SerializeObject(reply);
                var body = Encoding.UTF8.GetBytes(json);

                // Используем очередь из оригинального запроса
                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: originalRequest.ReplyQueue,  // ← Очередь клиента!
                    mandatory: true,
                    body: body
                );

                _logger.LogInformation("Ответ отправлен в очередь {Queue}: {MessageType}",
                    originalRequest.ReplyQueue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки ответа: {MessageType}");

            }
        }
        private async Task ProcessMessageAsync(string json, CancellationToken ct)
        {
            BaseContract? contract;
            try
            {
                contract = JsonConvert.DeserializeObject<BaseContract>(json);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка десериализации JSON: {Json}", json);
                contract = null;
            }
            if (contract != null)
            {
                switch (contract.ContractType)
                {
                    case "CreateAccountRequest":
                        {
                            await HandleCreateAccountAsync((CreateAccountRequest)contract);
                            break;
                        }
                    case "CreateDepositRequest":
                        {
                            await HandleCreateDepositAsync((CreateDepositRequest)contract);
                            break;
                        }
                    case "LoanApplicationRequest":
                        {
                            await HandleLoanApplicationAsync((LoanApplicationRequest)contract);
                            break;
                        }
                    case "RegisterRequest":
                        {
                            await HandleRegisterAsync((RegisterRequest)contract);
                            break;
                        }
                    case "LoginRequest":
                        {
                            await HandleLoginAsync((LoginRequest)contract);
                            break;
                        }
                    default:
                        {

                            break;
                        }
                }
            }
        }
        private async Task HandleLoanApplicationAsync(LoanApplicationRequest request)
        {
            //OperationReply reply;
            var id = _tokenManager.GetClientId(request.SessionToken);
            if (id != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == id);
                if (user != null)
                {
                    var statement = new CreditStatement()
                    {
                        UserId = (int)id,
                        Amount = request.Amount,
                        BranchId = _context.Branches.Select(b => b.BranchId).ToList()[_random.Next(_context.Branches.Count())],
                        Date = DateTime.UtcNow,
                        Status = "На рассмотрении"
                    };
                    _context.CreditStatements.Add(statement);
                    //reply = new AuthReply
                    //{
                    //    
                    //};
                    //await SendReplyAsync(reply, request);
                    //return;
                }
            }
            //reply = new AuthReply
            //{
            //    
            //};
            //await SendReplyAsync(reply, request);
            return;
        }
        private async Task HandleCreateAccountAsync(CreateAccountRequest request)
        {
            //OperationReply reply;
            var id = _tokenManager.GetClientId(request.SessionToken);
            if (id != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == id);
                if (user != null)
                {
                    var account = new Account()
                    {
                        UserId = (int)id,
                        Balance = 0,
                    };
                    _context.Accounts.Add(account);
                    //reply = new AuthReply
                    //{
                    //    
                    //};
                    //await SendReplyAsync(reply, request);
                    //return;
                }
            }
            //reply = new AuthReply
            //{
            //    
            //};
            //await SendReplyAsync(reply, request);
            return;
        }
        private async Task HandleCreateDepositAsync(CreateDepositRequest request)
        {
            //OperationReply reply;
            var id = _tokenManager.GetClientId(request.SessionToken);
            if (id != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == id);
                if (user != null)
                {
                    var account = _context.Accounts.FirstOrDefault(a => a.AccountId == request.AccountId);
                    if (account != null && account.UserId == id && account.Balance >= request.Amount)
                    {
                        var deposit = new Deposit()
                        {
                            UserId = (int)id,
                            Amount = request.Amount,
                            InterestRate = _random.Next(50, 101) / 10.0m
                        };
                        _context.Deposits.Add(deposit);
                        account.Balance -= request.Amount;
                        //reply = new AuthReply
                        //{
                        //    
                        //};
                        //await SendReplyAsync(reply, request);
                        //return;
                    }
                }
            }
            //reply = new AuthReply
            //{
            //    
            //};
            //await SendReplyAsync(reply, request);
            return;
        }
        private async Task HandleRegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Country) ||
            string.IsNullOrWhiteSpace(request.Phone) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Country.Length > 50 ||
            request.FirstName.Length > 100 ||
            request.LastName.Length > 100 ||
            request.Phone.Length > 50 ||
            request.Password.Length < 6 ||
            request.Password.Length > 100)
            {
                var reply = new AuthReply
                {
                    IsAuthorized = false,
                    errorMessage = "Некорректные данные регистрации"
                };
                await SendReplyAsync(reply, request);
                return;
            }
            else
            {
                AuthReply reply;
                if (request.isEmployee == false)
                {
                    var user = new User()
                    {
                        UserId = GetFirstAvailableId(_context.Users.Distinct().Select(u => u.UserId).ToList()),
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Phone = request.Phone,
                        Country = request.Country,
                    };
                    _context.Users.Add(user);
                    var authData = new ClientAuthData()
                    {
                        Phone = request.Phone,
                        Password = request.Password
                    };
                    _context.ClientAuthData.Add(authData);
                    reply = new AuthReply
                    {
                        IsAuthorized = true,
                        SessionToken = _tokenManager.CreateClientToken(user.UserId, TimeSpan.FromMinutes(10))
                    };
                }
                else
                {
                    var employee = new Employee()
                    {
                        EmployeeId = GetFirstAvailableId(_context.Employees.Distinct().Select(e => e.EmployeeId).ToList()),
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        AccessLevel = _random.Next(1, 4).ToString(),
                        Phone = request.Phone,
                        BranchId = _context.Branches.Select(b => b.BranchId).ToList()[_random.Next(_context.Branches.Count())],
                        Country = request.Country,
                        HireDate = DateTime.UtcNow
                    };
                    _context.Employees.Add(employee);
                    var authData = new EmployeeAuthData()
                    {
                        Phone = request.Phone,
                        Password = request.Password
                    };
                    _context.EmployeeAuthData.Add(authData);
                    reply = new AuthReply
                    {
                        IsAuthorized = true,
                        SessionToken = _tokenManager.CreateEmployeeToken(employee.EmployeeId, TimeSpan.FromMinutes(10))
                    };
                }
                await SendReplyAsync(reply, request);
                return;
            }
        }
        private async Task HandleLoginAsync(LoginRequest request)
        {
            if (request.isEmployee)
            {
                var auth = _context.EmployeeAuthData.FirstOrDefault(e => e.Phone == request.Phone);
                var employee = _context.Employees.FirstOrDefault(e => e.Phone == request.Phone);
                if (auth == null ||
                    auth.Password != request.Password ||
                    employee == null)
                {
                    var reply = new AuthReply
                    {
                        IsAuthorized = false,
                        errorMessage = "Некорректные данные регистрации"
                    };
                    await SendReplyAsync(reply, request);
                    return;
                }
                else
                {
                    var reply = new AuthReply
                    {
                        IsAuthorized = true,
                        SessionToken = _tokenManager.CreateEmployeeToken(employee.EmployeeId, TimeSpan.FromMinutes(10))
                    };
                    await SendReplyAsync(reply, request);
                    return;
                }
            }
            else
            {
                var auth = _context.ClientAuthData.FirstOrDefault(e => e.Phone == request.Phone);
                var client = _context.Users.FirstOrDefault(e => e.Phone == request.Phone);
                if (auth == null ||
                    auth.Password != request.Password ||
                    client == null)
                {
                    var reply = new AuthReply
                    {
                        IsAuthorized = false,
                        errorMessage = "Некорректные данные регистрации"
                    };
                    await SendReplyAsync(reply, request);
                    return;
                }
                else
                {
                    var reply = new AuthReply
                    {
                        IsAuthorized = true,
                        SessionToken = _tokenManager.CreateClientToken(client.UserId, TimeSpan.FromMinutes(10))
                    };
                    await SendReplyAsync(reply, request);
                    return;
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Остановка Consumer...");
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _context.SaveChanges();
            _context.Dispose();
            _logger.LogInformation("Остановка канала и соединения...");
            _channel?.DisposeAsync().GetAwaiter().GetResult();
            _connection?.DisposeAsync().GetAwaiter().GetResult();
        }
        public ServerStatus GetStatus()
        {
            return new ServerStatus
            {
                IsRunning = true,
                IsPaused = _isPaused,
                ProcessedMessages = _processedMessages,
                Provider = _options.Provider,
                Uptime = DateTime.UtcNow - _startTime
            };
        }

        public async Task<HealthStatus> GetHealth()
        {
            var health = new HealthStatus { IsHealthy = true };

            try
            {
                using var context = new BankContext(_options.Provider);
                await context.Database.CanConnectAsync();
                health.DatabaseStatus = "OK";
            }
            catch (Exception ex)
            {
                health.DatabaseStatus = $"ERROR: {ex.Message}";
                health.IsHealthy = false;
            }

            health.RabbitMQStatus = _connection?.IsOpen == true ? "OK" : "Disconnected";

            return health;
        }

        public void PauseQueue()
        {
            if (_isPaused != true)
            {
                lock (_lock)
                {
                    _isPaused = true;
                    _logger.LogInformation("Обработка очереди приостановлена");
                }
            }
        }

        public void ResumeQueue()
        {
            if (_isPaused != false)
            {
                lock (_lock)
                {
                    _isPaused = false;
                    _logger.LogInformation("Обработка очереди возобновлена");
                }
            }
        }


        public async Task ChangeProviderAsync(ServerOptions options)
        {
            var newProvider = options.Provider;
            var connectionString = options.ConnectionString;
            _logger.LogInformation($"Смена провайдера с {_options.Provider} на {newProvider}");
            try
            {
                using var testContext = new BankContext(newProvider);
                await testContext.Database.CanConnectAsync();
                _logger.LogInformation("Новое подключение успешно протестировано");

                lock (_lock)
                {
                    _options.Provider = newProvider;
                    _options.ConnectionString = connectionString;

                    _context?.Dispose();
                    _context = new BankContext(newProvider);
                }

                _logger.LogInformation("Провайдер успешно изменен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка смены провайдера");
                throw;
            }
        }
        private int GetFirstAvailableId(List<int> ids)
        {

            if (!ids.Any())
                return 1;

            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] != i + 1)
                    return i + 1;
            }

            return ids.Max() + 1;
        }
    }

    public class ServerStatus
    {
        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }
        public long ProcessedMessages { get; set; }
        public DbProvider Provider { get; set; }
        public TimeSpan Uptime { get; set; }
    }

    public class HealthStatus
    {
        public bool IsHealthy { get; set; }
        public string DatabaseStatus { get; set; } = "Unknown";
        public string RabbitMQStatus { get; set; } = "Unknown";
    }
}
