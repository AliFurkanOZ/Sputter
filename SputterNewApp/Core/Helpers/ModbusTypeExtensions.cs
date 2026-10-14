using System;

namespace SputterNewApp.Core.Helpers
{
    public static class ModbusTypeExtensions
    {
        // WORD ORDER: Low-High (CDAB) implementation
        // Registers: [Reg0: LowWord (CD)], [Reg1: HighWord (AB)]

        public static float ToFloat32_CDAB(this ushort[] registers)
        {
            if (registers == null || registers.Length < 2) return 0.0f;

            ushort lowWord = registers[0];  // CD
            ushort highWord = registers[1]; // AB

            var bytes = new byte[4];

            // Low Word (CD) -> D, C
            bytes[0] = (byte)(lowWord & 0xFF);        // D
            bytes[1] = (byte)((lowWord >> 8) & 0xFF); // C

            // High Word (AB) -> B, A
            bytes[2] = (byte)(highWord & 0xFF);       // B
            bytes[3] = (byte)((highWord >> 8) & 0xFF);// A

            return BitConverter.ToSingle(bytes, 0);
        }

        public static ushort[] ToRegisters_CDAB(this float value)
        {
            byte[] bytes = BitConverter.GetBytes(value); // D, C, B, A (Little Endian sistemde)

            // Low Word (CD) oluştur
            ushort lowWord = (ushort)((bytes[1] << 8) | bytes[0]);

            // High Word (AB) oluştur
            ushort highWord = (ushort)((bytes[3] << 8) | bytes[2]);

            // CDAB sırası: Önce Low, Sonra High
            return new ushort[] { lowWord, highWord };
        }

        public static short ToInt16(this ushort register)
        {
            return unchecked((short)register);
        }

        public static ushort ToUshort(this short value)
        {
            return unchecked((ushort)value);
        }

        /// <summary>
        /// Decode IEEE754 float32 from two holding registers using CDAB (Low-High) word order.
        /// </summary>
        public static float ToFloat32_CDAB(ushort lowWord_CD, ushort highWord_AB)
        {
            byte c = (byte)(lowWord_CD >> 8);
            byte d = (byte)(lowWord_CD & 0xFF);

            byte a = (byte)(highWord_AB >> 8);
            byte b = (byte)(highWord_AB & 0xFF);

            byte[] abcd = { a, b, c, d };

            if (BitConverter.IsLittleEndian)
                Array.Reverse(abcd);

            return BitConverter.ToSingle(abcd, 0);
        }

        /// <summary>
        /// Encode IEEE754 float32 to two holding registers using CDAB (Low-High) word order.
        /// Returns (lowWord_CD, highWord_AB) to be written at (Address, Address+1).
        /// </summary>
        public static (ushort lowWord_CD, ushort highWord_AB) FromFloat32_CDAB(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            byte a = bytes[0];
            byte b = bytes[1];
            byte c = bytes[2];
            byte d = bytes[3];

            ushort lowWord = (ushort)((c << 8) | d); // CD
            ushort highWord = (ushort)((a << 8) | b); // AB

            return (lowWord, highWord);
        }

        public static ushort FromInt16(short value) => unchecked((ushort)value);
    }
}
