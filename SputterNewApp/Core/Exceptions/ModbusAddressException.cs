using System;

namespace SputterNewApp.Core.Exceptions
{
    public sealed class ModbusAddressException : Exception
    {
        public ushort StartAddress { get; }
        public ushort NumberOfPoints { get; }

        public ModbusAddressException(ushort startAddress, ushort numberOfPoints, Exception innerException)
            : base($"Modbus exception reading/writing address {startAddress} (count {numberOfPoints}): {innerException.Message}", innerException)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }
    }
}
