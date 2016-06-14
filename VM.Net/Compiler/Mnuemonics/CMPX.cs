using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class CMPX : WordCompare
    {
        public CMPX() : base("CMPX", 0x07) { }
    }
}
