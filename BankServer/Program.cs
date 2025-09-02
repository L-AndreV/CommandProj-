using System;
using System.Text;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommandProj.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;


namespace BankServer//Здесь находиться интерфейс для взаимодействия админа с сервером
{
    public class Program
    {
        IHost? host = null;
        BankServer.CommandHandler commandHandler = null;
        static async Task Main()//Здесь настраиваются основные компоненты и происходит обработка команд админа
        {
            ServerOptions options = new();
            string[] parts;
            string command;
            bool isWorking = false;
            Program program = new Program();

            Console.WriteLine("Банковский сервер");
            Console.WriteLine("Введите 'help' для списка команд");
            while (true)
            {
                parts = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                command = parts.FirstOrDefault()?.ToLower();
                var args = parts.Skip(1).ToArray();
                switch (command)
                {
                    case "help":
                        {
                            program.ShowHelp();
                            break;
                        }

                    case "start":
                        {
                            if (isWorking)
                            {
                                Console.WriteLine("Сервер уже работает");
                            }
                            else
                            {
                                _ = Task.Run(async () =>
                                {
                                    try
                                    {
                                        if (program.host == null) await program.ConfigureHost(options);
                                        isWorking = true;
                                        await program.host.RunAsync();
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        Console.WriteLine("Сервер остановлен");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Ошибка сервера: {ex.Message}");
                                    }
                                });
                                await Task.Delay(1000);
                                Console.WriteLine("Сервер запущен");
                            }
                            break;
                        }
                    case "stop":
                        {
                            {
                                if (isWorking)
                                {
                                    program.commandHandler.Stop();
                                    isWorking = false;
                                    Console.WriteLine("Сервер остановлен");
                                }
                                else
                                {
                                    Console.WriteLine("Сервер уже выключен");
                                }
                                break;
                            }
                        }
                    case "status":
                        {
                            if (isWorking)
                            {
                                var status = program.commandHandler.GetStatus();
                                Console.WriteLine("татус сервера:");
                                Console.WriteLine($"  Состояние:     {(status.IsRunning ? "Запущен" : "Остановлен")}");
                                Console.WriteLine($"  Очередь:       {(status.IsPaused ? "Приостановлена" : "Активна")}");
                                Console.WriteLine($"  Сообщений:     {status.ProcessedMessages:N0}");
                                Console.WriteLine($"  Провайдер:     {options.Provider}");
                                Console.WriteLine($"  Аптайм:        {status.Uptime}");
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine("Статус сервера:");
                                Console.WriteLine($"  Состояние: выключен");
                                Console.WriteLine($"  Очередь: -");
                                Console.WriteLine($"  Обработано сообщений: -");
                                Console.WriteLine($"  Провайдер: {options.Provider}");
                                Console.WriteLine($"  Аптайм: -");
                                Console.WriteLine();
                            }
                            break;
                        }

                    case "health":
                        {
                            if (isWorking)
                            {
                                Console.WriteLine("Состояние системы");
                                var health = await program.commandHandler.GetHealth();

                                Console.WriteLine($"  Общее состояние: {(health.IsHealthy ? "OK" : "Ошибка")}");
                                Console.WriteLine($"  База данных:     {health.DatabaseStatus}");
                                Console.WriteLine($"  RabbitMQ:        {health.RabbitMQStatus}");

                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine("Сервер не запущен. Невозможно проверить состояние БД.");
                            }
                            break;
                        }

                    case "pause":
                        {
                            if (isWorking)
                            {
                                program.commandHandler.PauseResume(false);
                            }
                            else
                            {
                                Console.WriteLine("Сервер не запущен. Невозможно приостановить обработку.");
                            }
                            break;
                        }

                    case "resume":
                        {
                            if (isWorking)
                            {
                                program.commandHandler.PauseResume(true);
                            }
                            else
                            {
                                Console.WriteLine("Сервер не запущен. Невозможно возобновить обработку.");
                            }
                            break;
                        }

                    case "provider":
                        {
                            if (Enum.TryParse<DbProvider>(args[0], true, out var provider))
                            {
                                Console.WriteLine($"Провайдер изменён с {options.Provider} на {provider}");
                                options.Provider = provider;
                                if (isWorking)
                                {
                                    program.commandHandler.ChangeProvider(options);
                                }
                                else
                                {
                                    await program.ConfigureHost(options);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Неизвестный провайдер: {args[0]}");
                                Console.WriteLine($"Доступные провайдеры: {string.Join(",", Enum.GetNames<DbProvider>)}");
                            }
                            break;
                        }

                    case "":
                        break;

                    default:
                        Console.WriteLine($"?Неизвестная команда: {command}");
                        Console.WriteLine("Введите 'help' для списка доступных команд");
                        break;
                }
            }
        }
        async Task ConfigureHost(ServerOptions serverOptions)//Тут интереснее. Здесь происходит настройка самого сервера
        {
            //Её можно провести только до его запуска
            //Здесь используются только те классы, которые сделаны по определённому шаблону
            var logsDir = Path.Combine("Logging", "logs");
            if (!Directory.Exists(logsDir))
                Directory.CreateDirectory(logsDir);
            var logFilePath = Path.Combine("Logging", "logs", $"consumer-{DateTime.Today:yyyy-MM-dd}.log");

            host = Host.CreateDefaultBuilder()//Создание хоста
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ServerOptions>(options =>//Настройка того, что передаётся серверу при старте
                    {
                        options.Provider = serverOptions.Provider;
                        options.ConnectionString = serverOptions.ConnectionString;
                    });
                    services.AddHostedService<Server>();//Добавление сервера в хост
                    services.AddSingleton<Server>();
                    services.AddSingleton<TokenManager>();//Добавление менеджера токенов в хост
                    services.AddSingleton<CommandHandler>();//Добавление команд для управления сервером в хост
                })
                .ConfigureLogging(logging =>//Добавление логера в хост
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddSimpleFile(logFilePath);
                })
                .Build();
            commandHandler = host.Services.GetRequiredService<CommandHandler>();
        }

        private void ShowHelp()
        {
            Console.WriteLine("  Доступные команды:");
            Console.WriteLine("  start                - Показать статус сервера");
            Console.WriteLine("  stop                 - Остановить сервер");
            Console.WriteLine("  status               - Показать статус сервера");
            Console.WriteLine("  health               - Проверить состояние системы");
            Console.WriteLine("  pause                - Приостановить обработку очереди");
            Console.WriteLine("  resume               - Возобновить обработку очереди");
            Console.WriteLine("  provider <провайдер> - Сменить провайдер БД");
            Console.WriteLine();
        }
    }
    public class ServerOptions//Класс для передачи серверу при старте
    {
        public DbProvider Provider { get; set; } = DbProvider.SQLite;
        public string ConnectionString { get; set; } = "default_connection";
    }
}