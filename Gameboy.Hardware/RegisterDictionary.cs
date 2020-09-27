using System.Collections.Generic;

namespace Gameboy.Hardware
{
    public class RegisterDictionary : Dictionary<RegisterId, Register>
    {
        public RegisterDictionary()
        {
            Add(RegisterId.B, new Register(RegisterId.B));
            Add(RegisterId.C, new Register(RegisterId.C));
            Add(RegisterId.D, new Register(RegisterId.D));
            Add(RegisterId.E, new Register(RegisterId.E));
            Add(RegisterId.H, new Register(RegisterId.H));
            Add(RegisterId.L, new Register(RegisterId.L));
            Add(RegisterId.F, new Register(RegisterId.F, x => x & 0xF0, x => x & 0xF0));
            Add(RegisterId.A, new Register(RegisterId.A));
            
            Add(RegisterId.AF, new Register
            (
                RegisterId.AF, 
                x => this[RegisterId.A].Data << 8 | this[RegisterId.F].Data,
                x =>
                {
                    //Flags last 4 bits always zero (ZNHC0000)
                    this[RegisterId.A].Data = x >> 8;
                    this[RegisterId.F].Data = x & 0x00F0;
                    return x & 0xFFF0;
                }, 
                isByte: false
            ));
            
            Add(RegisterId.BC, new Register
            (
                RegisterId.BC,
                x => this[RegisterId.B].Data << 8 | this[RegisterId.C].Data,
                x =>
                {
                    this[RegisterId.B].Data = x >> 8;
                    this[RegisterId.C].Data = x & 0x00FF;
                    return x;
                },
                isByte: false
            ));
            
            Add(RegisterId.DE, new Register
            (
                RegisterId.DE,
                x => this[RegisterId.D].Data << 8 | this[RegisterId.E].Data,
                x =>
                {
                    this[RegisterId.D].Data = x >> 8;
                    this[RegisterId.E].Data = x & 0x00FF;
                    return x;
                },
                isByte: false
            ));
            
            Add(RegisterId.HL, new Register
            (
                RegisterId.HL,
                x => this[RegisterId.H].Data << 8 | this[RegisterId.L].Data,
                x =>
                {
                    this[RegisterId.H].Data = x >> 8;
                    this[RegisterId.L].Data = x & 0x00FF;
                    return x;
                },
                isByte: false
            ));
        }
    }
}