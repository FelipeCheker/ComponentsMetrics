using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace ComponentsMetrics;

public class ServerEventPublisher : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task InitializeAsync(string hostName = "localhost")
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
            UserName = "guest",
            Password = "guest"
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        
        // Configuración para eventos de usuarios
        const string metrisExchangeName = "exchange-metrics";
        const string metrisQueueName = "metrics-events";
        
        await _channel.ExchangeDeclareAsync(
            exchange: metrisExchangeName,
            type: ExchangeType.Direct,
            durable: true,  // ✅ Cambiado a true para consistencia
            autoDelete: false
        );
        
        await _channel.QueueDeclareAsync(
            queue: metrisQueueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false
        );
        
        // ✅ AGREGAR BINDING - esto faltaba!
        await _channel.QueueBindAsync(
            queue: metrisQueueName,
            exchange: metrisExchangeName,
            routingKey: "components"
        );
        
        Console.WriteLine("ServerEventPublisher inicializado correctamente");
    }

    public async Task PublishEventAsync(string routingKey, object data, string exchangeName)
    {
        if (_channel is null)
            throw new InvalidOperationException("Channel is not initialized.");

        try
        {
            string message = data is string str ? str : JsonSerializer.Serialize(data);
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(exchangeName, routingKey, body);
            Console.WriteLine($"[Evento Publicado → {routingKey}] {message} en {exchangeName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publicando evento: {ex.Message}");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
            
            Console.WriteLine("ServerEventPublisher cerrado correctamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cerrando ServerEventPublisher: {ex.Message}");
        }
    }
}