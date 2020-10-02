using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using Hardware.Memory;
using Hardware.System;
using Xunit;

namespace UnitTests.ApprovalTests.MMUTests
{
    [UseReporter(typeof(DiffReporter))]
    public class MMUTests
    {
        public MMUTests()
        {
            Bus.Init();
        }
        
        [Fact]
        public void MMU_ValidateHumanReadableDump()
        {
            FillMMU();
            Approvals.Verify(((MMU) Bus.MMU).Dump());
        }
        
        [Fact]
        public void MMU_ValidateHexDump()
        {
            FillMMU();
            Approvals.Verify(((MMU) Bus.MMU).HexDump());
        }

        private static void FillMMU()
        {
            Bus.Init();
            var expectedValues = new byte[0xFFFF + 1];

            ushort i = 0;
            while (i < 0xFFFF)
            {
                if (i == 0xE000) // Skip Echo RAM
                    i = 0xFE00;

                expectedValues[i] = (byte)(i % 256);
                Bus.MMU.SetByte(i, expectedValues[i]);
                i++;
            }

            Array.ConstrainedCopy(
                expectedValues,
                0xC000,
                expectedValues,
                0xE000,
                7680
            ); // Copy the work ram into the echo ram
            
            Array.Fill<byte>(expectedValues, 0, 0xFEA0, 96); // Unusable Address Space

            expectedValues[0xFFFF] = 1; // Interrupt Enable Flag
            Bus.MMU.SetByte(0xFFFF, expectedValues[0xFFFF]);
        }
    }
}