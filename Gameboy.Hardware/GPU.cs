using System;
using Gameboy.Interfaces;

namespace Gameboy.Hardware
{
    public class GPU : IGraphicsProcessingUnit
    {
        //Start		End		Description
        //8000		9FFF	8KB Video RAM(VRAM)
        //FE00		FE9F	Sprite attribute table(OAM)

        private int[] _videoRam = new int[0x2000]; //8kb
        private int[] _spriteAttributeTable = new int[0x00A0]; //160 bytes

        public GBMemory GetByte(int absoluteAddress, MemoryRegion region)
        {
            VerifyAddress(absoluteAddress, region);

            if (region == MemoryRegion.VIDEO_RAM)
            {
                return new GBMemory(MemoryRegion.VIDEO_RAM, _videoRam[absoluteAddress & 0x1FFF], absoluteAddress);
            }

            if (region == MemoryRegion.SPRITE_ATTRIBUTE_TABLE)
            {
                return new GBMemory(MemoryRegion.SPRITE_ATTRIBUTE_TABLE, _spriteAttributeTable[absoluteAddress & 0x00FF], absoluteAddress);
            }

            throw new Exception($"{region} is not valid: must be {MemoryRegion.VIDEO_RAM} or {MemoryRegion.SPRITE_ATTRIBUTE_TABLE}");
        }

        public void SetByte(int absoluteAddress, int val, MemoryRegion region)
        {
            VerifyAddress(absoluteAddress, region);

            if (region == MemoryRegion.VIDEO_RAM)
            {
                _videoRam[absoluteAddress & 0x1FFF] = val;
                return;
            }

            if (region == MemoryRegion.SPRITE_ATTRIBUTE_TABLE)
            {
                _spriteAttributeTable[absoluteAddress & 0x00FF] = val;
                return;
            }

            throw new Exception($"{region} is not valid: must be {MemoryRegion.VIDEO_RAM} or {MemoryRegion.SPRITE_ATTRIBUTE_TABLE}");
        }

        private static void VerifyAddress(int absoluteAddress, MemoryRegion region)
        {
            if (region != MemoryRegion.VIDEO_RAM && region != MemoryRegion.SPRITE_ATTRIBUTE_TABLE)
            {
                throw new Exception($"{region} is not valid: must be {MemoryRegion.VIDEO_RAM} or {MemoryRegion.SPRITE_ATTRIBUTE_TABLE}");
            }

            if (region == MemoryRegion.VIDEO_RAM && (absoluteAddress < 0x8000 || absoluteAddress > 0x9FFF))
            {
                throw new IndexOutOfRangeException($"The address {absoluteAddress} is out of range (0x8000 to 0x9FFF)");
            }

            if (region == MemoryRegion.SPRITE_ATTRIBUTE_TABLE && (absoluteAddress < 0xFE00 || absoluteAddress > 0xFE9F))
            {
                throw new IndexOutOfRangeException($"The address {absoluteAddress} is out of range (0xFE00 to 0xFE9F)");
            }

            // If got here, it's valid.
        }
    }
}