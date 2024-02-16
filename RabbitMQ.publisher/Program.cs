using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;


public enum LogNames
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Information = 4,
}

class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();

        var amqpUrl = config["AMQP-URL"];

        var factory = new ConnectionFactory()
        {
            Uri = !string.IsNullOrEmpty(amqpUrl) ? new Uri(amqpUrl) : null
        };

        using var connection = factory.CreateConnection();

        var channel = connection.CreateModel();

        channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

        Enum.GetNames(typeof(LogNames)).ToList().ForEach(name =>
        {
            var routeKey = $"route-{name}";

            var queueName = $"direct-queue-{name}";
            channel.QueueDeclare(queueName, true, false, false);

            channel.QueueBind(queueName, "logs-direct", routeKey, null);
        });

        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            LogNames log = (LogNames)new Random().Next(1, 5);

            string message = $"log-type: {log}";

            var messageBody = Encoding.UTF8.GetBytes(message);

            var routeKey = $"route-{log}";

            channel.BasicPublish("logs-direct", routeKey, null, messageBody);

            Console.WriteLine($"Log gönderilmiştir : {message}");

        });

        Console.ReadLine();
    }
}