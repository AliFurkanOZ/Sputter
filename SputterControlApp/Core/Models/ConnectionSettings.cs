namespace SputterControlApp.Core.Models
{
    public sealed class ConnectionSettings
    {
        public string IpAddress { get; set; } = "192.168.1.10";
        public int Port { get; set; } = 502;
        public byte UnitId { get; set; } = 1; // Modbus slave/unit id
    }
}
