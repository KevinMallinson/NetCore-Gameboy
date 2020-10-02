using System;
using Interfaces;

namespace Hardware.Registers
{
    public class RegisterIO : IRegisterIO
    {
        public IByteRegister B { get; }
        public IByteRegister C { get; }
        public IByteRegister D { get; }
        public IByteRegister E { get; }
        public IByteRegister H { get; }
        public IByteRegister L { get; }
        public IByteRegister F { get; }
        public IByteRegister A { get; }
        public IRegisterPair AF { get; }
        public IRegisterPair BC { get; }
        public IRegisterPair DE { get; }
        public IRegisterPair HL { get; }
        public IWordRegister PC { get; }
        public IWordRegister SP { get; }

        public RegisterIO()
        {
            B = new ByteRegister(RegisterId.B);
            C = new ByteRegister(RegisterId.C);
            D = new ByteRegister(RegisterId.D);
            E = new ByteRegister(RegisterId.E);
            H = new ByteRegister(RegisterId.H);
            L = new ByteRegister(RegisterId.L);
            F = new ByteRegister(RegisterId.F);
            A = new ByteRegister(RegisterId.A);
            
            AF = new RegisterPair(RegisterId.AF, A, F);
            BC = new RegisterPair(RegisterId.BC, B, C);
            DE = new RegisterPair(RegisterId.DE, D, E);
            HL = new RegisterPair(RegisterId.HL, H, L);

            PC = new WordRegister(RegisterId.PC);
            SP = new WordRegister(RegisterId.SP);
        }
        
        public IByteRegister GetByteRegister(RegisterId id)
        {
            return id switch
            {
                RegisterId.B => B,
                RegisterId.C => C,
                RegisterId.D => D,
                RegisterId.E => E,
                RegisterId.H => H,
                RegisterId.L => L,
                RegisterId.F => F,
                RegisterId.A => A,
                _ => throw new Exception($"Could not find the register with ID: {id}")
            };
        }
        
        public IRegisterPair GetRegisterPair(RegisterId id)
        {
            return id switch
            {
                RegisterId.AF => AF,
                RegisterId.BC => BC,
                RegisterId.DE => DE,
                RegisterId.HL => HL,
                _ => throw new Exception($"Could not find the register with ID: {id}")
            };
        }
        
        public IWordRegister GetWordRegister(RegisterId id)
        {
            return id switch
            {
                RegisterId.SP => SP,
                RegisterId.PC => PC,
                _ => throw new Exception($"Could not find the register with ID: {id}")
            };
        }
        
        public void SetByteRegister(RegisterId id, byte value)
        {
            var register = id switch
            {
                RegisterId.B => B,
                RegisterId.C => C,
                RegisterId.D => D,
                RegisterId.E => E,
                RegisterId.H => H,
                RegisterId.L => L,
                RegisterId.F => F,
                RegisterId.A => A,
                _ => throw new Exception($"Could not find the register with ID: {id}")
            };

            register.Set(value);
        }

        public void SetByteRegister(RegisterId id, RegisterId from)
        {
            var toRegister = GetByteRegister(id);
            var fromData = GetByteRegister(from).Get();
            toRegister.Set(fromData);
        }

        public void SetRegisterPair(RegisterId id, ushort value)
        {
            var register = id switch
            {
                RegisterId.AF => AF,
                RegisterId.BC => BC,
                RegisterId.DE => DE,
                RegisterId.HL => HL,
                _ => throw new Exception($"Could not find the register with ID: {id}")
            };

            register.Set(value);
        }
        
        public void SetWordRegister(RegisterId id, ushort value)
        {
            var register = id switch
            {
                RegisterId.SP => SP,
                RegisterId.PC => PC,
                _ => throw new Exception($"Could not find the register with ID: {id}")
            };

            register.Set(value);
        }
    }
}