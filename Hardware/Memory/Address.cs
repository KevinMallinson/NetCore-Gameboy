namespace Hardware.Memory
{
    public enum AddressSource
    {
        Immediate,
        Register
    }
    
    public class FullAddress
    {
        public ushort Value { get; }
        public AddressSource Source { get; }

        public FullAddress(ushort address, AddressSource source)
        {
            Value = address;
            Source = source;
        }
    }
    
    public class HalfAddress
    {
        public byte Value { get; }
        public AddressSource Source { get; }
        
        public HalfAddress(byte address, AddressSource source)
        {
            Value = address;
            Source = source;
        }
    }
}