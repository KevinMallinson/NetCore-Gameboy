using System;
using Interfaces;

namespace Hardware.Registers
{
    public class WordRegister : IWordRegister
    {
        public RegisterId Id { get; }
        private ushort _data = 0;

        public WordRegister(RegisterId id)
        {
            Id = Id;
        }
        
        object IRegister<ushort>.Get() => Get();
        public ushort Get() => _data;
        public void Set(ushort value) => _data = value;

        public void Increment(ushort by = 1) => Set((ushort)(Get() + by));

        public void Decrement(ushort by = 1) => Set((ushort)(Get() - by));
    }
}