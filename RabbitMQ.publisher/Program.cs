using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();

        var amqpUrl = config["AMQP-URL"];

        var factory = new ConnectionFactory()
        {
            Uri = new Uri(amqpUrl)
        };

        // RabbitMQ serverına bağlanma
        using (var connection = factory.CreateConnection())
        {
            // Bağlantı kurulduğunda yapılabilecek işlemler burada gerçekleştirilir
            Console.WriteLine("RabbitMQ serverına bağlanıldı.");
            Console.WriteLine("Bağlantı URL'si: " + amqpUrl);


            // RabbitMQ kanal üzerinden bağlanma
            using (var channel = connection.CreateModel())
            {
                //channel.QueueDeclare("hello-queue", true, false, false);
                channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

                Enumerable.Range(1, 50).ToList().ForEach(x =>
                {
                    string message = $"Log {x}";

                    var messageBody = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("logs-fanout", "", null, messageBody);

                    Console.WriteLine($"Message: {message}");
                });
            }
        }

        Console.ReadLine();
    }
}
