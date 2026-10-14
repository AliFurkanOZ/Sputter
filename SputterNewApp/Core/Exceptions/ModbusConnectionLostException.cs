using System;

namespace SputterNewApp.Core.Exceptions
{
    public sealed class ModbusConnectionLostException : Exception
    {
        public ModbusConnectionLostException(string message) : base(message) { }
        public ModbusConnectionLostException(string message, Exception? innerException) : base(message, innerException) { }
    }
}
