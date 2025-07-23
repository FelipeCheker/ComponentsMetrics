namespace ComponentsMetrics.DTOs;

public class MetricsData
{
    public float CpuUsagePercent { get; set; }
    public float AvailableMemoryMB { get; set; }
    public float DiskUsagePercent { get; set; }
    public float? GpuUsagePercent { get; set; }
    public DateTime Timestamp { get; set; }
}
