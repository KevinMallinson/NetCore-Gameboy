namespace Interfaces
{
    public interface IRegisterIO
    {
        IByteRegister B { get; }
        IByteRegister C { get; }
        IByteRegister D { get; }
        IByteRegister E { get; }
        IByteRegister H { get; }
        IByteRegister L { get; }
        IByteRegister F { get; }
        IByteRegister A { get; }
        IRegisterPair AF { get; }
        IRegisterPair BC { get; }
        IRegisterPair DE { get; }
        IRegisterPair HL { get; }
        IWordRegister PC { get; }
        IWordRegister SP { get; }
        IByteRegister GetByteRegister(RegisterId id);
        IRegisterPair GetRegisterPair(RegisterId id);
        IWordRegister GetWordRegister(RegisterId id);
        void SetByteRegister(RegisterId id, byte value);
        void SetByteRegister(RegisterId id, RegisterId from);
        void SetRegisterPair(RegisterId id, ushort value);
        void SetWordRegister(RegisterId id, ushort value);
    }
}