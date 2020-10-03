using Hardware.System;
using Interfaces;
using Xunit;

namespace UnitTests.CPUTests
{
    public class LDTests
    {
        private readonly IRegisterIO _registers;

        public LDTests()
        {
            Bus.Init();
            _registers = Bus.CPU.Registers;
        }
        
        [Theory]
        [InlineData(0x40, "LD", "B", "B", 1, 4, "- - - -")]
        [InlineData(0x41, "LD", "B", "C", 1, 4, "- - - -")]
        [InlineData(0x42, "LD", "B", "D", 1, 4, "- - - -")]
        [InlineData(0x43, "LD", "B", "E", 1, 4, "- - - -")]
        [InlineData(0x44, "LD", "B", "H", 1, 4, "- - - -")]
        [InlineData(0x45, "LD", "B", "L", 1, 4, "- - - -")]
        [InlineData(0x47, "LD", "B", "A", 1, 4, "- - - -")]
        [InlineData(0x48, "LD", "C", "B", 1, 4, "- - - -")]
        [InlineData(0x49, "LD", "C", "C", 1, 4, "- - - -")]
        [InlineData(0x4A, "LD", "C", "D", 1, 4, "- - - -")]
        [InlineData(0x4B, "LD", "C", "E", 1, 4, "- - - -")]
        [InlineData(0x4C, "LD", "C", "H", 1, 4, "- - - -")]
        [InlineData(0x4D, "LD", "C", "L", 1, 4, "- - - -")]
        [InlineData(0x4F, "LD", "C", "A", 1, 4, "- - - -")]
        [InlineData(0x50, "LD", "D", "B", 1, 4, "- - - -")]
        [InlineData(0x51, "LD", "D", "C", 1, 4, "- - - -")]
        [InlineData(0x52, "LD", "D", "D", 1, 4, "- - - -")]
        [InlineData(0x53, "LD", "D", "E", 1, 4, "- - - -")]
        [InlineData(0x54, "LD", "D", "H", 1, 4, "- - - -")]
        [InlineData(0x55, "LD", "D", "L", 1, 4, "- - - -")]
        [InlineData(0x57, "LD", "D", "A", 1, 4, "- - - -")]
        [InlineData(0x58, "LD", "E", "B", 1, 4, "- - - -")]
        [InlineData(0x59, "LD", "E", "C", 1, 4, "- - - -")]
        [InlineData(0x5A, "LD", "E", "D", 1, 4, "- - - -")]
        [InlineData(0x5B, "LD", "E", "E", 1, 4, "- - - -")]
        [InlineData(0x5C, "LD", "E", "H", 1, 4, "- - - -")]
        [InlineData(0x5D, "LD", "E", "L", 1, 4, "- - - -")]
        [InlineData(0x5F, "LD", "E", "A", 1, 4, "- - - -")]
        [InlineData(0x60, "LD", "H", "B", 1, 4, "- - - -")]
        [InlineData(0x61, "LD", "H", "C", 1, 4, "- - - -")]
        [InlineData(0x62, "LD", "H", "D", 1, 4, "- - - -")]
        [InlineData(0x63, "LD", "H", "E", 1, 4, "- - - -")]
        [InlineData(0x64, "LD", "H", "H", 1, 4, "- - - -")]
        [InlineData(0x65, "LD", "H", "L", 1, 4, "- - - -")]
        [InlineData(0x67, "LD", "H", "A", 1, 4, "- - - -")]
        [InlineData(0x68, "LD", "L", "B", 1, 4, "- - - -")]
        [InlineData(0x69, "LD", "L", "C", 1, 4, "- - - -")]
        [InlineData(0x6A, "LD", "L", "D", 1, 4, "- - - -")]
        [InlineData(0x6B, "LD", "L", "E", 1, 4, "- - - -")]
        [InlineData(0x6C, "LD", "L", "H", 1, 4, "- - - -")]
        [InlineData(0x6D, "LD", "L", "L", 1, 4, "- - - -")]
        [InlineData(0x6F, "LD", "L", "A", 1, 4, "- - - -")]
        [InlineData(0x78, "LD", "A", "B", 1, 4, "- - - -")]
        [InlineData(0x79, "LD", "A", "C", 1, 4, "- - - -")]
        [InlineData(0x7A, "LD", "A", "D", 1, 4, "- - - -")]
        [InlineData(0x7B, "LD", "A", "E", 1, 4, "- - - -")]
        [InlineData(0x7C, "LD", "A", "H", 1, 4, "- - - -")]
        [InlineData(0x7D, "LD", "A", "L", 1, 4, "- - - -")]
        [InlineData(0x7F, "LD", "A", "A", 1, 4, "- - - -")]
        public static void LoadRegisterToRegister(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags)
        {
            var value = 33;

            tester
                .SetFromByteRegister(from)
                .SetFromValue(value)
                .SetToByteRegister(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }


        [Theory]
        [InlineData(0xF9, "LD", "SP", "HL", 1, 8, "- - - -")]
        public static void Method2(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        { 
            var value = 33;

            tester
                .SetFromRegisterPair(from)
                .SetFromValue(value)
                .SetToWordRegister(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }


        [Theory]
        [InlineData(0x46, "LD", "B", "(HL)",  1, 8 , "- - - -")]
        [InlineData(0x4E, "LD", "C", "(HL)",  1, 8,  "- - - -")]
        [InlineData(0x56, "LD", "D", "(HL)",  1, 8,  "- - - -")]
        [InlineData(0x5E, "LD", "E", "(HL)",  1, 8,  "- - - -")]
        [InlineData(0x66, "LD", "H", "(HL)",  1, 8,  "- - - -")]
        [InlineData(0x6E, "LD", "L", "(HL)",  1, 8,  "- - - -")]
        [InlineData(0x7E, "LD", "A", "(HL)",  1, 8,  "- - - -")]
        [InlineData(0xF2, "LD", "A", "(C)",   2, 8,  "- - - -")]
        [InlineData(0x0A, "LD", "A", "(BC)",  1, 8,  "- - - -")]
        [InlineData(0x1A, "LD", "A", "(DE)",  1, 8,  "- - - -")]
        [InlineData(0x2A, "LD", "A", "(HL+)", 1, 8,  "- - - -")]
        [InlineData(0x3A, "LD", "A", "(HL-)", 1, 8,  "- - - -")]
        [InlineData(0xFA, "LD", "A", "(a16)", 3, 16, "- - - -")]
        public static void Method3(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        {
            tester
                .SetFromMemoryRegister(from)
                .SetFromValue(value)
                .SetToByteRegister(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }


        [Theory]
        [InlineData(0x02, "LD", "(BC)",  "A",   1, 8,  "- - - -")]
        [InlineData(0x12, "LD", "(DE)",  "A",   1, 8,  "- - - -")]
        [InlineData(0x22, "LD", "(HL+)", "A",   1, 8,  "- - - -")]
        [InlineData(0x32, "LD", "(HL-)", "A",   1, 8,  "- - - -")]
        [InlineData(0x70, "LD", "(HL)",  "B",   1, 8,  "- - - -")]
        [InlineData(0x71, "LD", "(HL)",  "C",   1, 8,  "- - - -")]
        [InlineData(0x72, "LD", "(HL)",  "D",   1, 8,  "- - - -")]
        [InlineData(0x73, "LD", "(HL)",  "E",   1, 8,  "- - - -")]
        [InlineData(0x74, "LD", "(HL)",  "H",   1, 8,  "- - - -")]
        [InlineData(0x75, "LD", "(HL)",  "L",   1, 8,  "- - - -")]
        [InlineData(0x77, "LD", "(HL)",  "A",   1, 8,  "- - - -")]
        [InlineData(0xE2, "LD", "(C)",   "A",   2, 8,  "- - - -")]
        [InlineData(0xEA, "LD", "(a16)", "A",   3, 16, "- - - -")]
        public static void Method4(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        { 
            tester
                .SetFromByteRegister(from)
                .SetFromValue(value)
                .SetToMemoryRegister(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }

        [Theory]
        [InlineData(0x08, "LD", "(a16)", "SP",  3, 20, "- - - -")]
        public static void Method10(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        { 
            tester
                .SetFromWordRegister(from)
                .SetFromValue(value)
                .SetToMemoryAddress(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }

        [Theory]
        [InlineData(0x36, "LD", "(HL)", "d8", 2, 12, "- - - -")]
        public static void Method5(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        { 
            tester
                .SetFromImmediateByte(from)
                .SetFromValue(value)
                .SetToMemoryRegister(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }


        [Theory]
        [InlineData(0x06, "LD", "B", "d8", 2, 8, "- - - -")]
        [InlineData(0x16, "LD", "D", "d8", 2, 8, "- - - -")]
        [InlineData(0x26, "LD", "H", "d8", 2, 8, "- - - -")]
        [InlineData(0x0E, "LD", "C", "d8", 2, 8, "- - - -")]
        [InlineData(0x1E, "LD", "E", "d8", 2, 8, "- - - -")]
        [InlineData(0x2E, "LD", "L", "d8", 2, 8, "- - - -")]
        [InlineData(0x3E, "LD", "A", "d8", 2, 8, "- - - -")]
        public static void Method6(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        { 
            tester
                .SetFromImmediateByte(from)
                .SetFromValue(value)
                .SetToByteRegister(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }

        [Theory]
        [InlineData(0x01, "LD", "BC", "d16",   3, 12, "- - - -")]
        [InlineData(0x11, "LD", "DE", "d16",   3, 12, "- - - -")]
        [InlineData(0x21, "LD", "HL", "d16",   3, 12, "- - - -")]
        public static void Method7(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        {
            tester
                .SetFromImmediateWord(from)
                .SetFromValue(value)
                .SetToRegisterPair(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }
        
        [Theory]
        [InlineData(0x31, "LD", "SP", "d16",   3, 12, "- - - -")]
        public static void Method7(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        {
            tester
                .SetFromImmediateWord(from)
                .SetFromValue(value)
                .SetToWordRegister(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);
        }

        [Theory]
        [InlineData(0xF8, "LD", "HL", "SP+r8", 2, 12, "0 0 H C")]
        public static void Method7(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags)
        {
            tester
                .SetFromWordRegister(from)
                .SetFromOffset(offset)
                .SetFromValue(value)
                .SetToRegisterPair(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);          
        }
        
        [Theory]
        [InlineData(0xE0, "LDH", "(a8)", "A", 2, 12, "- - - -")]
        public static void Method8(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        { 
            tester
                .SetFromByteRegister(from)
                .SetFromValue(value)
                .SetToUpperMemoryAddress(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);    
        }

        [Theory]
        [InlineData(0xF0, "LDH", "A", "(a8)", 2, 12, "- - - -")]
        public static void Method9(ushort opcode, string mnemonic, string to, string from, int length, int cycles, string flags) 
        { 
            tester
                .SetFromUpperMemoryAddress(from)
                .SetFromValue(value)
                .SetToByteRegister(to)
                .Load()
                .AssertValue(value)
                .AssertPC(length)
                .AssertCycles(cycles)
                .AssertFlags(flags);    
        }
    }
}