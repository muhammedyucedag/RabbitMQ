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
            Uri = !string.IsNullOrEmpty(amqpUrl) ? new Uri(amqpUrl) : null
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

                //Random kuyruk oluşturma

                var randomQueueName = channel.QueueDeclare().QueueName;

                // Fanout exchange ile kuyruğu bağla
                channel.QueueBind(randomQueueName, "logs-fanout", "", null);

                // Ön yükleme ayarını yap
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                int sleepTime = args.Length > 0 ? int.Parse(args[0]) : 1;
                Console.WriteLine("Waiting for messages. Sleep time : {0} sec", sleepTime);

                // Tüketici oluştur
                var consumer = new EventingBasicConsumer(channel);

                // False ile silme işlemini hemen yapmıyoruz. BasicAck() ile ulaşan mesajı sileceğiz
                // Kuyruktan mesajları al
                channel.BasicConsume(randomQueueName, false, consumer);

                Console.WriteLine("Log dinleniyor");

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"Received: {message}");

                    Thread.Sleep(sleepTime * 1000);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
            }

        }

        Console.ReadLine();
    }

}
