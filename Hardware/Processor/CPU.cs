using System;
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
            var instructionBuilder = _instructionTable.Get(opcode);
            
            var param = instructionBuilder.InstructionLength switch
            {
                1 => 0, //unused param
                2 => Bus.MMU.GetByte((ushort)(Registers.PC.Get() + 1)).Data,
                3 => Bus.MMU.GetWord((ushort)(Registers.PC.Get() + 1)).Data,
                _ => throw new Exception($"Cannot handle {opcode} instruction with length of {instructionBuilder.InstructionLength}")
            };
            
            return instructionBuilder.Build((ushort)param).Execute();
        }

        public string Flags => "- - - -";
    }
}