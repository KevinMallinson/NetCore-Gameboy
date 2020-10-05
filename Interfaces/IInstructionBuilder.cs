using System;

namespace Interfaces
{
    public interface IInstructionBuilder
    {
        int InstructionLength { get; }
        Func<ushort, IInstruction> Build { get; }
    }
}