using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.VirtualMachine
{
    public class ProcessorCache
    {
        private byte myRegisterA;
        private byte myRegisterB;
        private ushort myRegisterX;
        private ushort myRegisterY;
        private ushort myRegisterD;

        public event EventHandler OnRegistersUpdated;

        public byte RegisterA
        {
            get { return myRegisterA; }
            set
            {
                myRegisterA = value;
                myRegisterD = (ushort)((myRegisterA << 8) + myRegisterB);
                UpdateRegisterStatus();
            }
        }
        public byte RegisterB
        {
            get { return myRegisterB; }
            set
            {
                myRegisterB = value;
                myRegisterD = (ushort)((myRegisterA << 8) + myRegisterB);
                UpdateRegisterStatus();
            }
        }

        public ushort RegisterX
        {
            get { return myRegisterX; }
            set
            {
                myRegisterX = value;
                UpdateRegisterStatus();
            }
        }
        public ushort RegisterY
        {
            get { return myRegisterY; }
            set
            {
                myRegisterY = value;
                UpdateRegisterStatus();
            }
        }
        public ushort RegisterD
        {
            get { return myRegisterD; }
            set
            {
                myRegisterD = value;
                myRegisterA = (byte)(myRegisterD >> 8);
                myRegisterB = (byte)(myRegisterD & 0xFF);
            }
        }

        public ProcessorCache()
        {
            Clear();
        }
        
        private void UpdateRegisterStatus()
        {
            OnRegistersUpdated?.Invoke(this, EventArgs.Empty);
        }

        public  void Clear()
        {
            myRegisterA = 0;
            myRegisterB = 0;
            myRegisterX = 0;
            myRegisterY = 0;
            myRegisterD = 0;
            UpdateRegisterStatus();
        }
    }
}
