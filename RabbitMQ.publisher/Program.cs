using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
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

        channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);


        Random random = new Random();
        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            LogNames logName1 = (LogNames)random.Next(1, 5);
            LogNames logName2 = (LogNames)random.Next(1, 5);
            LogNames logName3 = (LogNames)random.Next(1, 5);

            var routeKey = $"{logName1}.{logName2}.{logName3}";

            string message = $"log-type: {logName1}-{logName2}-{logName3}";

            var messageBody = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("logs-direct", routeKey, null, messageBody);

            Console.WriteLine($"Log gönderilmiştir : {message}");
        });

        Console.ReadLine();
    }
}