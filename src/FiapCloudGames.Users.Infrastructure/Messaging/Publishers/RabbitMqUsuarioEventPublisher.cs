using FiapCloudGames.Users.Application.Usuarios.Interfaces.Messaging;
using FiapCloudGames.Users.Domain.Usuarios.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FiapCloudGames.Users.Infrastructure.Messaging.Publishers
{
    public class RabbitMqUsuarioEventPublisher : IUsuarioEventPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqUsuarioEventPublisher(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("RabbitMq:Host").Value!,
                UserName = configuration.GetSection("RabbitMq:Username").Value!,
                Password = configuration.GetSection("RabbitMq:Password").Value!,
                Port = Int32.Parse(configuration.GetSection("RabbitMq:Port").Value!)
            };
        }

        public async Task PublicarUsuarioCriadoAsync(
            UsuarioCriadoEvent evento)
        {
            var connection = await _factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "usuario-criado",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var payload = JsonSerializer.Serialize(evento);

            var body = Encoding.UTF8.GetBytes(payload);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "usuario-criado",
                mandatory: false,
                basicProperties: properties,
                body: body);
        }
    }
}
