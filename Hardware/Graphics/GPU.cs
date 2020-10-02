using System;
using Interfaces;
using Hardware.Memory;

namespace Hardware.Graphics
{
    public class GPU : IGraphicsProcessingUnit
    {
        //Start		End		Description
        //8000		9FFF	8KB Video RAM(VRAM)
        //FE00		FE9F	Sprite attribute table(OAM)

        private readonly byte[] _videoRam = new byte[0x2000]; //8kb
        private readonly byte[] _spriteAttributeTable = new byte[0x00A0]; //160 bytes

        public IMemory<byte> GetByte(ushort address, MemoryRegion region)
        {
            AssertAddress(address, region);

            return region switch
            {
                MemoryRegion.VIDEO_RAM => new ByteMemory(MemoryRegion.VIDEO_RAM, _videoRam[address & 0x1FFF], address),
                MemoryRegion.SPRITE_ATTRIBUTE_TABLE => new ByteMemory(MemoryRegion.SPRITE_ATTRIBUTE_TABLE, _spriteAttributeTable[address & 0x00FF], address),
                _ => throw new Exception($"{region} is not valid: must be {MemoryRegion.VIDEO_RAM} or {MemoryRegion.SPRITE_ATTRIBUTE_TABLE}")
            };
        }

        public void SetByte(ushort address, byte value, MemoryRegion region)
        {
            AssertAddress(address, region);

            if (region == MemoryRegion.VIDEO_RAM)
            {
                _videoRam[address & 0x1FFF] = value;
                return;
            }

            if (region == MemoryRegion.SPRITE_ATTRIBUTE_TABLE)
            {
                _spriteAttributeTable[address & 0x00FF] = value;
                return;
            }

            throw new Exception($"{region} is not valid: must be {MemoryRegion.VIDEO_RAM} or {MemoryRegion.SPRITE_ATTRIBUTE_TABLE}");
        }

        private static void AssertAddress(int address, MemoryRegion region)
        {
            if (region != MemoryRegion.VIDEO_RAM && region != MemoryRegion.SPRITE_ATTRIBUTE_TABLE)
            {
                throw new Exception($"{region} is not valid: must be {MemoryRegion.VIDEO_RAM} or {MemoryRegion.SPRITE_ATTRIBUTE_TABLE}");
            }

            if (region == MemoryRegion.VIDEO_RAM && (address < 0x8000 || address > 0x9FFF))
            {
                throw new IndexOutOfRangeException($"The address {address} is out of range (0x8000 to 0x9FFF)");
            }

            if (region == MemoryRegion.SPRITE_ATTRIBUTE_TABLE && (address < 0xFE00 || address > 0xFE9F))
            {
                throw new IndexOutOfRangeException($"The address {address} is out of range (0xFE00 to 0xFE9F)");
            }
        }
    }
}