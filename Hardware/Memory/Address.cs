namespace Hardware.Memory
{
    public class FullAddress
    {
        public ushort Value { get; }

        public FullAddress(ushort address)
        {
            Value = address;
        }
    }
    
    public class HalfAddress
    {
        public ushort Value { get; }

        public HalfAddress(byte address)
        {
            Value = (ushort)(0xFF + address);
        }
    }
}