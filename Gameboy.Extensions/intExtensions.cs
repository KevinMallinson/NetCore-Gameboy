using System;

namespace Gameboy.Extensions
{
    public static class IntExtensions
    {
        public static string ToLittleEndian(this int src)
        {
            if (src <= 255)
            {
                return $"0x{src:X2}";
            }

            if (src > 0xFFFF)
            {
                throw new Exception("Must be less than or equal to 0xFFFF");
            }
            
            var msb = src >> 8;
            var lsb = src & 0x00FF;

            return $"0x{lsb:X2}{msb:x2}";
        }
    }
}