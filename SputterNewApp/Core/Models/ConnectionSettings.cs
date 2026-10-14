namespace SputterNewApp.Core.Models
{
    public sealed class ConnectionSettings
    {
        public string IpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 502;
        public byte UnitId { get; set; } = 1;
        public string RegistrationNo { get; set; } = "1234";
    }
}
