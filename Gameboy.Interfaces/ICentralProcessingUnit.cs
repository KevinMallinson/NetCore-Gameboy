using System.Collections;
using System.Collections.Generic;

namespace Gameboy.Interfaces
{
    public interface ICentralProcessingUnit
    {
        ExecutedOpcode Step();
    }
}