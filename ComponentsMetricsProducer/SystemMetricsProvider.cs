using LibreHardwareMonitor.Hardware;

public class MetricsReader
{
    private readonly Computer _computer;

    public MetricsReader()
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsMemoryEnabled = true,
            IsGpuEnabled = true
        };
        _computer.Open();
    }

    public void PrintMetrics()
    {
        foreach (var hardware in _computer.Hardware)
        {
            hardware.Update();
            Console.WriteLine($"Hardware: {hardware.Name} ({hardware.HardwareType})");

            foreach (var sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.Load && sensor.Value != null)
                {
                    Console.WriteLine($"  Sensor: {sensor.Name} - {sensor.Value}%");
                }
                if (sensor.SensorType == SensorType.Temperature && sensor.Value != null)
                {
                    Console.WriteLine($"  Temp: {sensor.Name} - {sensor.Value}°C");
                }
            }
        }
    }
}