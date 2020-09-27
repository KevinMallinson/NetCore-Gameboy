// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using Gameboy.Extensions;
using Gameboy.Interfaces;

namespace Gameboy.Hardware
{
    public class CPU : ICentralProcessingUnit
    {
        public ushort ProgramCounter { get; set; }
        public ushort StackPointer { get; set; }
        public ExecutedOpcode LastExecutedOpcode { get; private set; }
        public RegisterDictionary Registers { get; } = new RegisterDictionary();

        public CPU()
        {
            
        }
        
        public ExecutedOpcode LD(RegisterId dest, RegisterId src)
        {
            Registers[dest] = Registers[src];
            ProgramCounter++;
            
            var opcode = $"LD {dest.ToString()}, {src.ToString()}";
            return new ExecutedOpcode(4, opcode, ProgramCounter, StackPointer, OpcodeType.LOAD);
        }
        
        public ExecutedOpcode LD(RegisterId dest, GBMemory memory)
        {
            if (memory.IsByte)
            {
                Registers[dest].Data = memory.Data;
            }
            else
            {
                throw new NotImplementedException();
            }
            
            ProgramCounter += 2;
            
            var opcode = $"LD {dest.ToString()}, {memory.Data.ToLittleEndian()}";
            return new ExecutedOpcode(8, opcode, ProgramCounter, StackPointer, OpcodeType.LOAD);
        }

        public ExecutedOpcode JMP(GBMemory memory)
        {
            ProgramCounter = memory.Data;
            
            var opcode = $"JMP {memory.Data.ToLittleEndian()}";
            return new ExecutedOpcode(12, opcode, ProgramCounter, StackPointer, OpcodeType.JUMP);
        }
        
        public ExecutedOpcode Step()
        {
            var opcode = Bus.MMU.GetByte(ProgramCounter).Data;
            switch (opcode)
            {
                case 0x40: case 0x41: case 0x42: case 0x43: case 0x44: case 0x45: case 0x47: // ld b,reg
                case 0x48: case 0x49: case 0x4a: case 0x4b: case 0x4c: case 0x4d: case 0x4f: // ld c,reg
                case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x57: // ld d,reg
                case 0x58: case 0x59: case 0x5a: case 0x5b: case 0x5c: case 0x5d: case 0x5f: // ld e,reg
                case 0x60: case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x67: // ld h,reg
                case 0x68: case 0x69: case 0x6a: case 0x6b: case 0x6c: case 0x6d: case 0x6f: // ld l,reg
                case 0x78: case 0x79: case 0x7a: case 0x7b: case 0x7c: case 0x7d: case 0x7f: // ld a,reg
                {
                    var dest = (RegisterId)(opcode >> 3 & 0b00000111);
                    var src = (RegisterId)(opcode & 0b00000111);
                    LastExecutedOpcode = LD(dest, src);
                    break;
                }
                case 0x06:
                {
                    var data = Bus.MMU.GetByte((ushort)(ProgramCounter + 1));
                    LastExecutedOpcode = LD( RegisterId.B, data);
                    break;
                }
                case 0xC3:
                {
                    var data = Bus.MMU.GetWord((ushort)(ProgramCounter + 1));
                    LastExecutedOpcode = JMP(data);
                    break;
                }
            }
            
            return LastExecutedOpcode;
        }
    }
}