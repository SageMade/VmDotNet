using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class CMPY : WordCompare
    {
        public CMPY() : base("CMPY", 0x08) { }
    }
}
