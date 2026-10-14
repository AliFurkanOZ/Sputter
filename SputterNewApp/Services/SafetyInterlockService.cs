using System;

namespace SputterNewApp.Services
{
    public sealed class SafetyInterlockService
    {
        public bool IsChillerOn { get; set; }
        public bool IsMatchBoxOn { get; set; }
        public bool IsRf1PowerOn { get; set; }
        public bool IsRf2PowerOn { get; set; }
        public bool IsDcPowerOn { get; set; }
        public bool IsEmergencyStopActive { get; set; }

        public bool CanStartTurboVacuum() => IsChillerOn && !IsEmergencyStopActive;

        public bool CanStartDcPower() => IsChillerOn && !IsEmergencyStopActive;

        public bool CanStartRf1Power() => IsChillerOn && IsMatchBoxOn && !IsEmergencyStopActive;

        public bool CanStartRf2Power() => IsChillerOn && IsMatchBoxOn && !IsEmergencyStopActive;

        public bool CanTurnOffMatchBox() => !IsRf1PowerOn && !IsRf2PowerOn;

        public bool CanStartLayer() => IsChillerOn && !IsEmergencyStopActive;

        public string? GetStartBlockReason(string target)
        {
            if (IsEmergencyStopActive)
                return "Acil Stop (Emergency Stop) devrede!";

            switch (target)
            {
                case "Turbo":
                    return !IsChillerOn ? "Önce Chiller çalıştırılmalıdır." : null;
                case "DC":
                    return !IsChillerOn ? "Önce Chiller çalıştırılmalıdır." : null;
                case "RF1":
                    if (!IsChillerOn) return "Önce Chiller çalıştırılmalıdır.";
                    if (!IsMatchBoxOn) return "Önce MatchBox Power açılmalıdır.";
                    return null;
                case "RF2":
                    if (!IsChillerOn) return "Önce Chiller çalıştırılmalıdır.";
                    if (!IsMatchBoxOn) return "Önce MatchBox Power açılmalıdır.";
                    return null;
                case "MatchBoxOff":
                    if (IsRf1PowerOn || IsRf2PowerOn)
                        return "MatchBox'ı kapatmadan önce RF1 ve RF2 kapatılmalıdır.";
                    return null;
                case "Layer":
                    return !IsChillerOn ? "Önce Chiller çalıştırılmalıdır." : null;
                default:
                    return null;
            }
        }
    }
}
