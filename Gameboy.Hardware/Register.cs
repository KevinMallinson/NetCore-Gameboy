using System;

namespace Gameboy.Hardware
{
    public class Register
    {
        public RegisterId Id { get; }
        public int Data
        {
            get => AssertCorrectSize(_getter(_data));
            set => _data = AssertCorrectSize(_setter(value));
        }

        public bool IsRegisterGroup =>
            Id == RegisterId.AF || Id == RegisterId.BC || Id == RegisterId.DE || Id == RegisterId.HL;

        private int _data;
        private readonly Func<int, int> _setter;
        private readonly Func<int, int> _getter;
        private readonly bool _isByte;

        public Register(RegisterId id, Func<int, int> getModifier = null, Func<int, int> setModifier = null, bool isByte = true)
        {
            Id = id;
            
            _getter = getModifier ?? (x => x);
            _setter = setModifier ?? (x => x);
            _isByte = isByte;
        }

        private int AssertCorrectSize(int value)
        {
            if (_isByte && value > 0xFF)
                throw new Exception($"The value {value} exceeds the maximum size of a byte.");
            
            if (!_isByte && value > 0xFFFF)
                throw new Exception($"The value {value} exceeds the maximum size of a ushort.");

            return value;
        }
    }
}