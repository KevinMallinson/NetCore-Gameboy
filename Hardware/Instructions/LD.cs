using System;
using Interfaces;
using Hardware.Memory;
using Hardware.Processor;
using Hardware.System;

namespace Hardware.Instructions
{
    public class LD : IInstruction
    {
        private readonly Func<IExecutedOpcode> _executor;

        public LD(IByteRegister to, IByteRegister from, string opcodeStr, int cycles, Action customLogic = null)
        {
            _executor = () =>
            {
                to.Set(from.Get());
                customLogic?.Invoke();
                Bus.CPU.Registers.PC.Increment(1);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(IByteRegister to, byte from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                to.Set(from);
                Bus.CPU.Registers.PC.Increment(2);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(IRegisterPair to, ushort from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                to.Set(from);
                Bus.CPU.Registers.PC.Increment(3);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(FullAddress to, IRegisterPair from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                Bus.MMU.SetWord(to.Value, from.Get());
                Bus.CPU.Registers.PC.Increment(3);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(FullAddress to, byte from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                Bus.MMU.SetByte(to.Value, from);
                Bus.CPU.Registers.PC.Increment(2);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(FullAddress to, IByteRegister from, string opcodeStr, int cycles, Action customLogic = null)
        {
            _executor = () =>
            {
                Bus.MMU.SetByte(to.Value, from.Get());
                customLogic?.Invoke();
                Bus.CPU.Registers.PC.Increment(3);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(IByteRegister to, FullAddress from, string opcodeStr, int cycles, Action customLogic = null)
        {
            _executor = () =>
            {
                to.Set(Bus.MMU.GetByte(from.Value).Data);
                customLogic?.Invoke();
                Bus.CPU.Registers.PC.Increment(3);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(IWordRegister to, IRegisterPair from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                to.Set(from.Get());
                Bus.CPU.Registers.PC.Increment(1);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(IWordRegister to, ushort from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                to.Set(from);
                Bus.CPU.Registers.PC.Increment(3);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public LD(FullAddress to, IWordRegister from, string opcodeStr, int cycles)
        {
            _executor = () =>
            {
                Bus.MMU.SetWord(to.Value, from.Get());
                Bus.CPU.Registers.PC.Increment(3);
                return new ExecutedOpcode(cycles, opcodeStr, OpcodeType.LD);
            };
        }

        public IExecutedOpcode Execute() => _executor();
    }
}