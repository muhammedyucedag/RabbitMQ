using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
                //channel.QueueDeclare("hello-queue", true, false, true);

                //True dersek her subscriber'e bölüştürür mesajları.
                //False dersek her subscriber'e 1 1 1 dağıtacak.

                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);

                //False ile silme işlemini hemen yapmıyoruz. BasicAck() ile ulaşan mesajı sileceğiz
                channel.BasicConsume("hello-queue", false, consumer);

                consumer.Received += (object sender, BasicDeliverEventArgs e) =>
                {
                    var message = Encoding.UTF8.GetString(e.Body.ToArray());
                    Thread.Sleep(1000);
                    Console.WriteLine("Gelen Mesaj:" + message);

                    // Mesaj başarıyla işlendiğinde kuyruktan sil
                    channel.BasicAck(e.DeliveryTag, false);
                
                };
            }

        }

        Console.ReadLine();
    }

}
