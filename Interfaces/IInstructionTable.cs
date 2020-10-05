using System;

namespace Interfaces
{
    public interface IInstructionTable
    {
        IInstructionBuilder Get(int opcode);
    }
}