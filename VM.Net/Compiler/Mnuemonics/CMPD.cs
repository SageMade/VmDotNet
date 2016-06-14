using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class CMPD : WordCompare
    {
        public CMPD() : base("CMPD", 0x09) { }
    }
}
