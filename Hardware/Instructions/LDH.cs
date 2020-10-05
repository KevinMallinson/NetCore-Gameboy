using System;
using Interfaces;
using Hardware.Memory;
using Hardware.Processor;
using Hardware.System;

namespace Hardware.Instructions
{
    public class LDH : IInstruction
    {
        private readonly Func<IExecutedOpcode> _executor;
        
        public LDH(IByteRegister to, HalfAddress from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                to.Set(Bus.MMU.GetByte((ushort)(0xFF00 + from.Value)).Data);
                Bus.CPU.Registers.PC.Increment((ushort)(from.Source == AddressSource.Register ? 1 : 2));
                opcodeStr = opcodeStr.Replace("a8", $"0x{from.Value:X}");
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LDH);
            };
        }

        public LDH(HalfAddress to, IByteRegister from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                Bus.MMU.SetByte((ushort)(0xFF00 + to.Value), from.Get());
                Bus.CPU.Registers.PC.Increment((ushort)(to.Source == AddressSource.Register ? 1 : 2));
                opcodeStr = opcodeStr.Replace("a8", $"0x{to.Value:X}");
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public IExecutedOpcode Execute() => _executor();
    }
}