using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class JEQ : JumpMnuemonic
    {
        public JEQ() : base("JEQ", 0x0B) { }
    }
}
