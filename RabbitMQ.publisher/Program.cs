using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

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
                // Kuyruk Oluşturma
                channel.QueueDeclare("hello-queue", true, false, false);

                Enumerable.Range(1, 50).ToList().ForEach(x =>
                {
                    string message = $"Message {x}";

                    // Mesajı Byte halinde alıyoruz
                    var messageBody = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

                    Console.WriteLine($"Mesaj Gönderilmiştir : {message}");

                });
            }
        }

        Console.ReadLine();
    }
}
