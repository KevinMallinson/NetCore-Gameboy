using System;

namespace Gameboy.Hardware
{
    //B, C, D, E, H, L, F, A
    public enum Registers
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

    public class CPU
    {
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
        public int this[int key]
        {
            get
            {
                return key switch
                {
                    0 => B,
                    1 => C,
                    2 => D,
                    3 => E,
                    4 => H,
                    5 => L,
                    6 => F,
                    7 => A,
                    _ => throw new Exception($"Can only access registers with index 0 through 7. The index attempted to access was: {key}")
                };

            }

            set
            {
                switch (key)
                {
                    case 0:
                        B = (byte)value;
                        break;
                    case 1:
                        C = (byte)value;
                        break;
                    case 2:
                        D = (byte)value;
                        break;
                    case 3:
                        E = (byte)value;
                        break;
                    case 4:
                        H = (byte)value;
                        break;
                    case 5:
                        L = (byte)value;
                        break;
                    case 6:
                        F = (byte)value;
                        break;
                    case 7:
                        A = (byte)value;
                        break;
                    default:
                        throw new Exception($"Can only access registers with index 0 through 7. The index attempted to access was: {key}");
                };
            }
        }

        private byte _f;
        private ushort _pc;
        private ushort _sp;
    }
}