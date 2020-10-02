using System;

namespace Extensions
{
    public static class UshortExtensions
    {
        public static byte ToByteWithAssert(this ushort src)
        {
            if (src > 0xFF)
            {
                throw new Exception($"{src} is too big to fit in a byte.");
            }

            return (byte) src;
        }
    }
}