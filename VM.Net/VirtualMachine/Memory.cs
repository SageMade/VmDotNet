using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.VirtualMachine
{
    public class Memory
    {
        private byte[] myData;

        public byte this[int index]
        {
            get { return myData[index]; }
            set { myData[index] = value; }
        }
        
        public Memory(int size)
        {
            if (size > UInt16.MaxValue)
                throw new InvalidOperationException("Cannot create a memory module exceeding the limit of 16k addressing");

            myData = new byte[size];
        }

        public void ClearSector(ushort screenMemoryLocation, ushort screenMemorySize)
        {
            for (int index = screenMemoryLocation; index < screenMemoryLocation + screenMemorySize; index++)
                myData[index] = 0;
        }
    }
}
