using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.VirtualMachine
{
    public interface IPeripheral
    {
        uint Poll();

        void Pass(uint value);
    }
}
