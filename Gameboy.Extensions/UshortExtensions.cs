using System;

namespace Gameboy.Extensions
{
    public static class UshortExtensions
    {
        public static string ToLittleEndian(this ushort src)
        {
            if (src <= 255)
            {
                return $"0x{src:X2}";
            }
            
            var msb = src >> 8;
            var lsb = src & 0x00FF;

            return $"0x{lsb:X2}{msb:x2}";
        }
    }
}