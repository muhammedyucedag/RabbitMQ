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

        //var factory = new ConnectionFactory();
        //factory.Uri = new Uri("amqps://irldprra:pvfLdiVq_X1fS2uKIaz9NPHHRG5PENIo@gull.rmq.cloudamqp.com/irldprra");

        // RabbitMQ serverına bağlanma
        using var connection = factory.CreateConnection();
        Console.WriteLine("RabbitMQ serverına bağlanıldı.");
        Console.WriteLine("Bağlantı URL'si: " + amqpUrl);

        var channel = connection.CreateModel();

        // Kuyruk Oluşturma
        channel.QueueDeclare("hello-queue", true, false, false);

        channel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(channel);

        channel.BasicConsume("hello-queue", false, consumer);

        consumer.Received += (model, e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            Thread.Sleep(1500);
            Console.WriteLine("Gelen Mesaj:" + message);

            channel.BasicAck(e.DeliveryTag, false);
        };

        Console.ReadLine();
    }


}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        var builder = new ConfigurationBuilder();
//        builder.SetBasePath(Directory.GetCurrentDirectory())
//           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//        IConfiguration config = builder.Build();

//        var amqpUrl = config["AMQP-URL"];

//        var factory = new ConnectionFactory()
//        {
//            Uri = !string.IsNullOrEmpty(amqpUrl) ? new Uri(amqpUrl) : null
//        };

//        // RabbitMQ serverına bağlanma
//        using (var connection = factory.CreateConnection())
//        {
//            // Bağlantı kurulduğunda yapılabilecek işlemler burada gerçekleştirilir
//            Console.WriteLine("RabbitMQ serverına bağlanıldı.");
//            Console.WriteLine("Bağlantı URL'si: " + amqpUrl);

//            // RabbitMQ kanal üzerinden bağlanma
//            using (var channel = connection.CreateModel())
//            {
//                // Kuyruk Oluşturma
//                //channel.QueueDeclare("hello-queue", true, false, true);

//                //True dersek her subscriber'e bölüştürür mesajları.
//                //False dersek her subscriber'e 1 1 1 dağıtacak.

//                //Random kuyruk oluşturma

//                var randomQueueName = "log-database-save-queue"; //channel.QueueDeclare().QueueName;

//                // Kuyruğu oluştur
//                channel.QueueDeclare(randomQueueName, true, false, false);

//                // Fanout exchange ile kuyruğu bağla
//                channel.QueueBind(randomQueueName, "logs-fanout", "", null);

//                // Ön yükleme ayarını yap
//                channel.BasicQos(0, 10, false);

//                Console.WriteLine("Waiting for messages");

//                // Tüketici oluştur
//                var consumer = new EventingBasicConsumer(channel);

//                // False ile silme işlemini hemen yapmıyoruz. BasicAck() ile ulaşan mesajı sileceğiz
//                // Kuyruktan mesajları al
//                channel.BasicConsume(randomQueueName, false, consumer);

//                Console.WriteLine("Log dinleniyor");

//                consumer.Received += (model, e) =>
//                {
//                    var message = Encoding.UTF8.GetString(e.Body.ToArray());

//                    Thread.Sleep(1000);

//                    Console.WriteLine($"Received: {message}");

//                    channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
//                };
//            }

//        }

//        Console.ReadLine();
//    }

//}
