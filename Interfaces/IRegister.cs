using System;

namespace Interfaces
{
    public enum RegisterId
    {
        B = 0,
        C = 1,
        D = 2,
        E = 3,
        H = 4,
        L = 5,
        F = 6,
        A = 7,
        AF = 8,
        BC = 9,
        DE = 10,
        HL = 11,
        SP = 12,
        PC = 13
    };

    public enum PointerId
    {
        C = 1,
        AF = 8,
        BC = 9,
        DE = 10,
        HL = 11
    };

    public interface IRegister<in T>
    {
        RegisterId Id { get; }
        object Get();
        void Set(T value);
        void Increment(T by);
        void Decrement(T by);
    }

    public interface  IByteRegister : IRegister<byte>
    {
        new byte Get();
    }

    public interface IRegisterPair : IRegister<ushort>
    {
        new ushort Get();
    }

    public interface IWordRegister : IRegister<ushort>
    {
        new ushort Get();
    }
}