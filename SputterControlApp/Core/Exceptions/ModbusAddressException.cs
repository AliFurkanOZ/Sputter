using System;

namespace SputterControlApp.Core.Exceptions
{
    public sealed class ModbusAddressException : Exception
    {
        public ushort StartAddress { get; }
        public ushort Quantity { get; }

        public ModbusAddressException(ushort startAddress, ushort quantity, Exception? inner = null)
            : base("Yanlış / olmayan register adresi hatası.", inner)
        {
            StartAddress = startAddress;
            Quantity = quantity;
        }

        public override string ToString()
            => $"{Message} (StartAddress={StartAddress}, Quantity={Quantity})";
    }
}
