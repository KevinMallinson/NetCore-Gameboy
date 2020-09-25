namespace Gameboy.Interfaces
{
    public enum OpcodeType
    {
        JUMP,
        UNSPECIFIED,
        LOAD
    }

    public class ExecutedOpcode
    {
        public ExecutedOpcode(int cycles, string opcode, int newProgramCounter, int newStackPointer, OpcodeType opcodeType)
        {
            Cycles = cycles;
            Opcode = opcode;
            NewProgramCounter = newProgramCounter;
            NewStackPointer = newStackPointer;
            OpcodeType = opcodeType;
        }


        public string Opcode { get; }
        public int Cycles { get; }
        public int NewProgramCounter { get; }
        public int NewStackPointer { get; }
        public OpcodeType OpcodeType { get; }

        public override string ToString()
        {
            return Opcode;
        }
    }
}