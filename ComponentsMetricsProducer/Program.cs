using System.Configuration;
using ComponentsMetrics.DTOs;

namespace ComponentsMetrics;

class Program
{
    private static ServerEventPublisher? _publisher;

    static async Task Main(string[] args)
    {
        var ipServer = ConfigurationManager.AppSettings["ServerIP"] ?? "0.0.0.0";
        var serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"] ?? "10000");
        _publisher = new ServerEventPublisher();
        
        try
        {
            await _publisher.InitializeAsync();
            Console.WriteLine("Publisher de eventos inicializado correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inicializando publisher: {ex.Message}");
            Console.WriteLine("Continuando sin publisher de eventos...");
            _publisher = null;
        }
        
        //if (_publisher is null)
         //   return;
        
        string exchangeName = "exchange-metrics";
        
        var reader = new MetricsReader();
        while (true)
        {
            reader.PrintMetrics();
        }
    }
}