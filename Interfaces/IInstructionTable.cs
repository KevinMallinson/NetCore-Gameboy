using System;

namespace Interfaces
{
    public interface IInstructionTable
    {
        Func<ushort, IInstruction> Get(int opcode);
    }
}