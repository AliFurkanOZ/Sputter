using System;
using System.Text.Json.Serialization;

namespace SputterControlApp.Logging
{
    public enum LogLevel { Info, Warn, Error, Alarm }

    public enum LogSource
    {
        System,
        PLC,
        VacuumControl,
        SampleControl,
        GasControl,
        MotionControl
    }

    public sealed class LogEvent
    {
        public DateTime Time { get; init; } = DateTime.Now; // local time
        public LogLevel Level { get; init; } = LogLevel.Info;
        public string Code { get; init; } = "0";
        public string Message { get; init; } = "";
        public LogSource Source { get; init; } = LogSource.System;
        public string RegistrationNo { get; init; } = "";

        // Grid için hazır string alanlar
        [JsonIgnore]
        public string TimeText => Time.ToString("dd-MMM-yy HH:mm:ss");
        [JsonIgnore]
        public string LevelText => Level.ToString().ToUpperInvariant();
    }
}
