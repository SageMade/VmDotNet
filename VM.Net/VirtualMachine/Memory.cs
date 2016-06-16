using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.VirtualMachine
{
    public class Memory
    {
        private byte[] myData;

        public uint Size
        {
            get { return (uint)myData.Length; }
        }

        public byte this[uint index]
        {
            get { return myData[index]; }
            set { myData[index] = value; }
        }
        
        public Memory(uint size)
        {
            if (size > UInt32.MaxValue)
                throw new InvalidOperationException("Cannot create a memory module exceeding the limit of 16k addressing");

            myData = new byte[size];
        }

        public void SetUInt32(uint location, uint value)
        {
            myData[location] = (byte)((value << 24) >> 24);
            myData[location + 1] = (byte)((value << 16) >> 24);
            myData[location + 2] = (byte)((value << 8) >> 24);
            myData[location + 3] = (byte)(value >> 24);
        }

        public void DumpText(string fileName, uint min, uint max)
        {
            FileStream fs = File.OpenWrite(fileName);
            StreamWriter writer = new StreamWriter(fs);
            
            for (uint index = min; index < max; index ++)
            {
                if (index % 8 == 0)
                    writer.Write(string.Format("\n 0x{0:X2}", index));

                writer.Write(string.Format(" {0:X2} ", myData[index]));
            }
            writer.Close();
            fs.Close();

            writer.Dispose();
            fs.Dispose();
        }

        public float GetFloat32(uint location)
        {
            return BitConverter.ToSingle(myData, (int)location);
        }

        public uint GetUInt32(uint location)
        {
            return BitConverter.ToUInt32(myData, (int)location);
        }

        public void ClearSector(uint startLocation, uint size)
        {
            for (uint index = startLocation; index < startLocation + size; index++)
                myData[index] = 0;
        }

        public byte[] GetBytes()
        {
            return myData;
        }
    }
}
