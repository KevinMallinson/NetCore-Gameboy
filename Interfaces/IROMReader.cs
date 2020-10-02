using System.Collections.Generic;

namespace Interfaces
{
    public interface IROMReader
    {
        List<byte> ReadRom(string source);
    }
}