using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text;
using System.Text.Json;

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

        // RabbitMQ serverına bağlanma
        using var connection = factory.CreateConnection();
        Console.WriteLine("RabbitMQ serverına bağlanıldı.");
        Console.WriteLine("Bağlantı URL'si: " + amqpUrl);

        var channel = connection.CreateModel();
        channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);


        channel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(channel);

        var queueName = channel.QueueDeclare().QueueName;

        Dictionary<string, object> headers = new Dictionary<string, object>();

        headers.Add("format", "pdf");
        headers.Add("shape", "a4");
        headers.Add("x-match", "all");


        channel.QueueBind(queueName, "header-exchange", String.Empty, headers);

        channel.BasicConsume(queueName, false, consumer);

        Console.WriteLine("Loglar dinleniyor.");

        consumer.Received += (model, e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            Product product = JsonSerializer.Deserialize<Product>(message);

            Thread.Sleep(1000);
            Console.WriteLine($"Gelen Mesaj: + {product.Id}-{product.Name}-{product.Price}-{product.Stock}");


            channel.BasicAck(e.DeliveryTag, false);
        };

        Console.ReadLine();
    }

}
