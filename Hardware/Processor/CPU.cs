using Interfaces;
using Hardware.Registers;
using Hardware.System;

namespace Hardware.Processor
{
    public class CPU : ICentralProcessingUnit
    {
        public IRegisterIO Registers { get; }
        private readonly IInstructionTable _instructionTable;

        public CPU()
        {
            Registers = new RegisterIO();
            _instructionTable = new InstructionTable(Registers);
        }
        
        public IExecutedOpcode Step()
        {
            var opcode = Bus.MMU.GetByte(Registers.PC.Get()).Data;
            var instruction = _instructionTable.Get(opcode)(0);
            return instruction.Execute();
        }
    }
}