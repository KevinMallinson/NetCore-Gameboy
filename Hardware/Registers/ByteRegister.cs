using System;
using Interfaces;

namespace Hardware.Registers
{
    public class ByteRegister : IByteRegister
    {
        public RegisterId Id { get; }
        private byte _data;

        public ByteRegister(RegisterId id)
        {
            Id = id;
        }
        
        object IRegister<byte>.Get() => Get();
        public byte Get() => _data;

        public void Set(byte value)
        {
            _data = value;
            if (Id == RegisterId.F)
            {
                _data = (byte) (_data & 0xF0);
            }
        }

        public void Increment(byte by = 1) => Set((byte)(Get() + by));

        public void Decrement(byte by = 1) => Set((byte)(Get() - by));
    }
}