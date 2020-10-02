using System;
using Interfaces;

namespace Hardware.Registers
{
    public class RegisterPair : IRegisterPair
    {
        public RegisterId Id { get; }
        private readonly IByteRegister _hi;
        private readonly IByteRegister _lo;

        public RegisterPair(RegisterId id, IByteRegister hi, IByteRegister lo)
        {
            Id = id;
            _hi = hi;
            _lo = lo;
        }

        object IRegister<ushort>.Get() => Get();

        public ushort Get()
        {
            var msb = _hi.Get();
            var lsb = _lo.Get();
            return (ushort) (msb << 8 | lsb);
        }

        public void Set(ushort value)
        {
            _hi.Set((byte)(value >> 8));
            _lo.Set((byte)(value & 0x00FF));
        }

        public void Increment(ushort by = 1) => Set((ushort)(Get() + by));

        public void Decrement(ushort by = 1) => Set((ushort)(Get() - by));
    }
}