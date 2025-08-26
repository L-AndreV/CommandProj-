using Contracts;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



namespace BankUser
{
    public class User
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string replyQueueName;

        public event Action<BaseContract> OnReplyReceived;

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            replyQueueName = $"client_reply_{Guid.NewGuid()}";
            await _channel.QueueDeclareAsync(
                queue: replyQueueName,
                durable: false,
                exclusive: true,
                autoDelete: false,
                arguments: null);

            await StartListeningForReplies();
        }
        private async Task StartListeningForReplies()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += (sender, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var reply = JsonConvert.DeserializeObject<BaseContract>(json);
                OnReplyReceived?.Invoke(reply);

                return Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(replyQueueName, autoAck: true, consumer);
        }
        public async Task SendOrderMessage(BaseContract request)
        {
            if (_channel is not { IsOpen: true })
                throw new InvalidOperationException("Канал RabbitMQ закрыт");
            request.ReplyQueue = replyQueueName;
            var json = JsonConvert.SerializeObject(request);
            var body = System.Text.Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "order_queue",
                body: body);
        }
    }
}
