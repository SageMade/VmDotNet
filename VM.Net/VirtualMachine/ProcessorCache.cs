using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.VirtualMachine
{
    public class ProcessorCache
    {
        private uint myRegister_IP;
        private uint myRegister_SP;
        private uint myRegister_BP;
        private uint myRegister_DI;
        private uint myRegister_SI;

        private uint myRegister_IAX;
        private uint myRegister_IBX;
        private uint myRegister_ICX;
        private uint myRegister_IDX;
        private uint myRegister_IEX;
        private uint myRegister_IFX;

        private float myRegister_FAX;
        private float myRegister_FBX;
        private float myRegister_FCX;

        private uint myRegister_RIA;
        private float myRegister_RFA;

        public event EventHandler OnRegistersUpdated;

        public uint Register_IP
        {
            get { return myRegister_IP; }
            set
            {
                myRegister_IP = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_SP
        {
            get { return myRegister_SP; }
            set
            {
                myRegister_SP = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_BP
        {
            get { return myRegister_BP; }
            set
            {
                myRegister_BP = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_DI
        {
            get { return myRegister_DI; }
            set
            {
                myRegister_DI = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_SI
        {
            get { return myRegister_SI; }
            set
            {
                myRegister_SI = value;
                UpdateRegisterStatus();
            }
        }

        public uint Register_IAX
        {
            get { return myRegister_IAX; }
            set
            {
                myRegister_IAX = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_IBX
        {
            get { return myRegister_IBX; }
            set
            {
                myRegister_IBX = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_ICX
        {
            get { return myRegister_ICX; }
            set
            {
                myRegister_ICX = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_IDX
        {
            get { return myRegister_IDX; }
            set
            {
                myRegister_IDX = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_IEX
        {
            get { return myRegister_IEX; }
            set
            {
                myRegister_IEX = value;
                UpdateRegisterStatus();
            }
        }
        public uint Register_IFX
        {
            get { return myRegister_IFX; }
            set
            {
                myRegister_IFX = value;
                UpdateRegisterStatus();
            }
        }

        public float Register_FAX
        {
            get { return myRegister_FAX; }
            set
            {
                myRegister_FAX = value;
                UpdateRegisterStatus();
            }
        }
        public float Register_FBX
        {
            get { return myRegister_FBX; }
            set
            {
                myRegister_FBX = value;
                UpdateRegisterStatus();
            }
        }
        public float Register_FCX
        {
            get { return myRegister_FCX; }
            set
            {
                myRegister_FCX = value;
                UpdateRegisterStatus();
            }
        }

        public uint Register_RIA
        {
            get { return myRegister_RIA; }
            set
            {
                myRegister_RIA = value;
                UpdateRegisterStatus();
            }
        }
        public float Register_RFA
        {
            get { return myRegister_RFA; }
            set
            {
                myRegister_RFA = value;
                UpdateRegisterStatus();
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
            myRegister_IP = 0;
            myRegister_SP = 0;
            myRegister_BP = 0;
            myRegister_DI = 0;
            myRegister_SI = 0;

            myRegister_IAX = 0;
            myRegister_IBX = 0;
            myRegister_ICX = 0;
            myRegister_IDX = 0;
            myRegister_IEX = 0;
            myRegister_IFX = 0;

            myRegister_FAX = 0;
            myRegister_FBX = 0;
            myRegister_FCX = 0;

            myRegister_RIA = 0;
            myRegister_RFA = 0;

            UpdateRegisterStatus();
        }

        /// <summary>
        /// Sets the given register to the value. This will only effect floating point buffers and non-restricted registers
        /// </summary>
        /// <param name="register">The register to set</param>
        /// <param name="value">The value to set</param>
        public void SetRegisterSafe(byte register, float value)
        {
            switch (register)
            {
                case (byte)RegisterAddress.FAX:
                    Register_FAX = value;
                    break;
                case (byte)RegisterAddress.FBX:
                    Register_FBX = value;
                    break;
                case (byte)RegisterAddress.FCX:
                    Register_FCX = value;
                    break;
            }
        }

        /// <summary>
        /// Sets the given register to the value. This will only effect integer buffers and non-restricted registers
        /// </summary>
        /// <param name="register">The register to set</param>
        /// <param name="value">The value to set</param>
        public void SetRegisterSafe(byte register, uint value)
        {
            switch (register)
            {
                case (byte)RegisterAddress.IP:
                    Register_IP = value;
                    break;
                case (byte)RegisterAddress.SP:
                    Register_SP = value;
                    break;
                case (byte)RegisterAddress.BP:
                    Register_BP = value;
                    break;

                case (byte)RegisterAddress.IAX:
                    Register_IAX = value;
                    break;
                case (byte)RegisterAddress.IBX:
                    Register_IBX = value;
                    break;
                case (byte)RegisterAddress.ICX:
                    Register_ICX = value;
                    break;
                case (byte)RegisterAddress.IDX:
                    Register_IDX = value;
                    break;
                case (byte)RegisterAddress.IEX:
                    Register_IEX = value;
                    break;
                case (byte)RegisterAddress.IFX:
                    Register_IFX = value;
                    break;
            }
        }

        public uint GetValueWord(byte register)
        {
            switch (register)
            {
                case (byte)RegisterAddress.IP:
                    return Register_IP;
                case (byte)RegisterAddress.SP:
                    return Register_SP ;                    
                case (byte)RegisterAddress.BP:
                    return Register_BP ;                    

                case (byte)RegisterAddress.IAX:
                    return Register_IAX ;                    
                case (byte)RegisterAddress.IBX:
                    return Register_IBX ;                    
                case (byte)RegisterAddress.ICX:
                    return Register_ICX ;                    
                case (byte)RegisterAddress.IDX:
                    return Register_IDX ;                    
                case (byte)RegisterAddress.IEX:
                    return Register_IEX ;                    
                case (byte)RegisterAddress.IFX:
                    return Register_IFX ;

                case (byte)RegisterAddress.RIA:
                    return Register_RIA;

                default:
                    return 0;                    
            }
        }
    }
}
