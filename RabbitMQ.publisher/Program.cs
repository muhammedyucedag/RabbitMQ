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

        channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

        Dictionary<string,object> headers = new Dictionary<string, object>();

        headers.Add("format", "pdf");
        headers.Add("shape", "a4");

        var properties = channel.CreateBasicProperties();
        properties.Headers = headers;

        channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("Header Mesajım"));

        Console.WriteLine("Mesaj gönderilmiştir.");

        Console.ReadLine();
    }
}