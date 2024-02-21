using RabbitMQ.Client;

namespace RabbitMQ.ExcelApp.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;  // RabbitMQ bağlantı fabrikası
        private IConnection _connection; // RabbitMQ bağlantısı
        private IModel _channel; // RabbitMQ kanalı
        private readonly ILogger<RabbitMQClientService> _logger; // Mesaj yazmak için kullanılan logger


        // RabbitMQ için değişkenlerin tanımlandığı sınıf dışında erişilebilir statik değişkenler
        public static string ExchangeName = "ExcelDirectExchange";
        public static string RoutingExcel = "excel-route-file";
        public static string QueueName = "queue-excel-file";

        // Constructor metot
        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        // RabbitMQ sunucusuna bağlanan metot
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection(); // Bağlantı oluştur

            if (_channel is { IsOpen: true }) // Kanal zaten açıksa mevcut kanalı dön
                return _channel;

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, type: "direct", true, false);

            _channel.QueueDeclare(QueueName, true, false, false, null);

            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingExcel);

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu");

            return _channel;

        }

        // Nesne imha edildiğinde çağrılan metot
        public void Dispose()
        {
            _channel?.Close(); // Kanalı kapat
            _channel?.Dispose(); // Kanalı imha et

            _connection?.Close(); // Bağlantıyı kapat
            _connection?.Dispose(); // Bağlantıyı imha et

            _logger.LogInformation("RabbitMQ ile bağlantı koptu"); // Loga bağlantının koptuğu bilgisini yaz

        }


    }

}
