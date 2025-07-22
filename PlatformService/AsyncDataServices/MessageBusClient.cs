using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly IConnection? _connection;
        private readonly IModel? _channel;

        public MessageBusClient(IConfiguration config)
        {
            _config = config;
            var host = _config["RabbitMQHost"];
            var port = _config["RabbitMQPort"];
            Console.WriteLine($"--> MessageBus Host: {host}, port: {port}");

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
            {
                throw new ArgumentException("Missing Message Bus config values");
            }

            var factory = new ConnectionFactory()
            {
                HostName = host,
                Port = int.Parse(port)
            };

            try
            {
                Console.WriteLine("--> Trying to create connection");
                _connection = factory.CreateConnection();
                Console.WriteLine("--> Trying to create channel");
                _channel = _connection.CreateModel();
                Console.WriteLine("--> Trying to create exchange");
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                Console.WriteLine("--> Adding shutdown subscribtion");
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to Message Bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection to Message Bus ShutDown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection!.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection Closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger",
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);

            Console.WriteLine($"--> We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("--> Message Bus Disposed");
            if (_channel!.IsOpen)
            {
                _channel.Close();
                _connection?.Close();
            }
        }
    }
}