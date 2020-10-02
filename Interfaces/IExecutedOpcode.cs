namespace Interfaces
{
    public enum OpcodeType
    {
        JP,
        UNSPECIFIED,
        LD,
        LDH
    }

    public interface IExecutedOpcode
    {
        public string Opcode { get; }
        public int Cycles { get; }
        public int NewProgramCounter { get; }
        public int NewStackPointer { get; }
        public OpcodeType OpcodeType { get; }
        public string ToString() => Opcode;
    }
}