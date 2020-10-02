using Interfaces;
using Hardware.System;

namespace Hardware.Processor
{
    public class ExecutedOpcode : IExecutedOpcode
    {
        public ExecutedOpcode(int cycles, string opcode, OpcodeType opcodeType)
        {
            Cycles = cycles;
            Opcode = opcode;
            NewProgramCounter = Bus.CPU.Registers.PC.Get();
            NewStackPointer = Bus.CPU.Registers.SP.Get();
            OpcodeType = opcodeType;
        }


        public string Opcode { get; }
        public int Cycles { get; }
        public int NewProgramCounter { get; }
        public int NewStackPointer { get; }
        public OpcodeType OpcodeType { get; }
    }
}