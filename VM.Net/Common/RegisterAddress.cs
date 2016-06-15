using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Common
{
    public enum RegisterAddress
    {
        Unknown = 0,
        IP = 0x01,
        SP = 0x02,
        BP = 0x03,
        DI = 0x04,
        SI = 0x05,
        IAX = 0x06,
        IBX = 0x07,
        ICX = 0x08,
        IDX = 0x09,
        IEX = 0x0A,
        IFX = 0x0B,
        FAX = 0x0C,
        FBX = 0x0D,
        FCX = 0x0E,
        RIA = 0x0F,
        RFA = 0x10,
    }
}
