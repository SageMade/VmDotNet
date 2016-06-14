using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public class JLT : JumpMnuemonic
    {
        public JLT() : base("JLT", 0x0E) { }
    }
}
