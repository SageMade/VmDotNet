using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.VirtualMachine
{
    [Flags]
    public enum CompareFlags
    {
        Equal       = 0x01,
        NotEqual    = 0x02,
        LessThan    = 0x04,
        GreaterThan = 0x08
    }

    [Flags]
    public enum ProcessorFlags
    {
        Overflow = 0x01,
        Carry    = 0x02
    }
}
