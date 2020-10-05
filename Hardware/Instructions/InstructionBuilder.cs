using System;
using Interfaces;
using NotImplementedException = System.NotImplementedException;

namespace Hardware.Instructions
{
    public class InstructionBuilder : IInstructionBuilder
    {
        public int InstructionLength { get; }
        public Func<ushort, IInstruction> Build { get; }

        public InstructionBuilder(int length, Func<ushort, IInstruction> instruction)
        {
            Build = instruction;
            InstructionLength = length;
        }
        
    }
}