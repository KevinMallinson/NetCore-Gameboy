using System;
using System.Collections;
using System.Collections.Generic;
using Gameboy.Extensions;
using Gameboy.Interfaces;

namespace Gameboy.Hardware
{
    //B, C, D, E, H, L, F, A
    public enum Register
    {
        B = 0,
        C = 1,
        D = 2,
        E = 3,
        H = 4,
        L = 5,
        F = 6,
        A = 7
    };

    public class CPU : ICentralProcessingUnit
    {
        
        private byte _f;
        private ushort _pc;
        private ushort _sp;
        private ExecutedOpcode? _lastExecutedOpcode;
        
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }

        public byte F
        {
            //Flags last 4 bits always zero (ZNHC0000)
            get => (byte)(_f & 0xF0);
            set => _f = (byte)(value & 0xF0);
        }
        public byte H { get; set; }
        public byte L { get; set; }

        public ushort AF
        {
            get => (ushort)(A << 8 | F);
            set
            {
                //Flags last 4 bits always zero (ZNHC0000)
                A = (byte)(value >> 8);
                F = (byte)(value & 0x00F0);
            }
        }
        public ushort BC
        {
            get => (ushort)(B << 8 | C);
            set
            {
                B = (byte)(value >> 8);
                C = (byte)(value & 0x00FF);

            }
        }
        public ushort DE
        {
            get => (ushort)(D << 8 | E);
            set
            {
                D = (byte)(value >> 8);
                E = (byte)(value & 0x00FF);
            }
        }

        public ushort HL
        {
            get => (ushort)(H << 8 | L);
            set
            {
                H = (byte)(value >> 8);
                L = (byte)(value & 0x00FF);
            }
        }

        /// <summary>
        /// Access the registers via its index in the following order:
        /// B, C, D, E, H, L, F, A
        /// </summary>
        /// <param name="key">Register Index</param>
        public int this[Register key]
        {
            get
            {
                return key switch
                {
                    Register.B => B,
                    Register.C => C,
                    Register.D => D,
                    Register.E => E,
                    Register.H => H,
                    Register.L => L,
                    Register.F => F,
                    Register.A => A,
                    _ => throw new Exception($"Can only access registers with index 0 through 7. The index attempted to access was: {key}")
                };
            }

            set
            {
                Action<byte> assignRegister = key switch
                {
                    Register.B => (val) => B = val,
                    Register.C => (val) => C = val,
                    Register.D => (val) => D = val,
                    Register.E => (val) => E = val,
                    Register.H => (val) => H = val,
                    Register.L => (val) => L = val,
                    Register.F => (val) => F = val,
                    Register.A => (val) => A = val,
                    _ => throw new Exception($"Can only access registers with index 0 through 7. The index attempted to access was: {key}")
                };

                assignRegister((byte) value);
            }
        }
        
        public ExecutedOpcode LD(Register dest, Register src)
        {
            this[dest] = this[src];
            _pc++;
            
            var opcode = $"LD {dest.ToString()}, {src.ToString()}";
            return new ExecutedOpcode(4, opcode, _pc, _sp, OpcodeType.LOAD);
        }
        
        public ExecutedOpcode LD(Register dest, GBMemory memory)
        {
            if (memory.IsByte)
            {
                this[dest] = memory.Data;
            }
            else
            {
                throw new NotImplementedException();
            }
            
            _pc += 2;
            
            var opcode = $"LD {dest.ToString()}, {memory.Data.ToLittleEndian()}";
            return new ExecutedOpcode(8, opcode, _pc, _sp, OpcodeType.LOAD);
        }

        public ExecutedOpcode JMP(GBMemory memory)
        {
            _pc = memory.Data;
            
            var opcode = $"JMP {memory.Data.ToLittleEndian()}";
            return new ExecutedOpcode(12, opcode, _pc, _sp, OpcodeType.JUMP);
        }
        
        public ExecutedOpcode Step()
        {
            var opcode = Bus.MMU.GetByte(_pc).Data;
            switch (opcode)
            {
                //------------LD INSTRUCTIONS FOR REGISTERS (NON PAIR), AND NON MEMORY (HL)------------//
                //LD instructions from 0x40 to 0x7F
                //Excluding 0x46, 0x56, 0x66                                due to --> LD reg, (HL)
                //Excluding 0x4E, 0x5E, 0x6E, 0x7E                          due to --> LD reg, (HL)
                //Excluding 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x77        due to --> LD (HL), reg
                //Excluding 0x76 because it's a HALT instruction.
                //------------------------------------------------------------------------------------//
                case 0x40: case 0x41: case 0x42: case 0x43: case 0x44: case 0x45: case 0x47: // ld b,reg
                case 0x48: case 0x49: case 0x4a: case 0x4b: case 0x4c: case 0x4d: case 0x4f: // ld c,reg
                case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x57: // ld d,reg
                case 0x58: case 0x59: case 0x5a: case 0x5b: case 0x5c: case 0x5d: case 0x5f: // ld e,reg
                case 0x60: case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x67: // ld h,reg
                case 0x68: case 0x69: case 0x6a: case 0x6b: case 0x6c: case 0x6d: case 0x6f: // ld l,reg
                case 0x78: case 0x79: case 0x7a: case 0x7b: case 0x7c: case 0x7d: case 0x7f: // ld a,reg
                {
                    var dest = (Register)(opcode >> 3 & 0b00000111);
                    var src = (Register)(opcode & 0b00000111);
                    _lastExecutedOpcode = LD(dest, src);
                    break;
                }
                case 0x06:
                {
                    var data = Bus.MMU.GetByte((ushort)(_pc + 1));
                    _lastExecutedOpcode = LD( Register.B, data);
                    break;
                }
                case 0xC3:
                {
                    var data = Bus.MMU.GetWord((ushort)(_pc + 1));
                    _lastExecutedOpcode = JMP(data);
                    break;
                }
            }
            
            return _lastExecutedOpcode;
        }

        public void Reset()
        {
            _pc = 0x100;
        }
    }
}