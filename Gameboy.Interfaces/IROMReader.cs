using System.Collections.Generic;

namespace Gameboy.Interfaces
{
    public interface IROMReader
    {
        List<byte> ReadRom(string source);
    }
}