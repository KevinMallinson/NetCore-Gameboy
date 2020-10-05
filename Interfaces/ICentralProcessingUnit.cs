using System.Collections;
using System.Collections.Generic;

namespace Interfaces
{
    public interface ICentralProcessingUnit
    {
        public IRegisterIO Registers { get; }  
        IExecutedOpcode Step();
        string Flags { get; }
    }
}