using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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

        channel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(channel);

        var queueName = channel.QueueDeclare().QueueName;

        var routekey = "Information.#";

        channel.QueueBind(queueName, "logs-topic", routekey);

        channel.BasicConsume(queueName, false, consumer);

        Console.WriteLine("Loglar dinleniyor.");

        consumer.Received += (model, e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            Thread.Sleep(1000);
            Console.WriteLine("Gelen Mesaj:" + message);

            //File.AppendAllText("direct-queue-Critical.txt", message + "\n");

            channel.BasicAck(e.DeliveryTag, false);
        };

        Console.ReadLine();
    }

}
