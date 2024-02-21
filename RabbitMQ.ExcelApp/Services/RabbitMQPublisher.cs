using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.ExcelApp.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        // Ürün resmi oluşturulduğunda çağrılan yayınlama metodu
        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel = _rabbitMQClientService.Connect(); // RabbitMQ kanalını alıyoruz

            var bodyString = JsonSerializer.Serialize(createExcelMessage); // Ürün resmni JSON formatına dönüştürüyoruz

            var bodyByte = Encoding.UTF8.GetBytes(bodyString); // JSON verisini byte dizisine dönüştürüyoruz

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: RabbitMQClientService.ExchangeName,
                routingKey: RabbitMQClientService.RoutingExcel,
                basicProperties: properties,
                body: bodyByte);
        }
    }

}
