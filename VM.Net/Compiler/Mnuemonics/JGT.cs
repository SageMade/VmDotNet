using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public class JGT : JumpMnuemonic
    {
        public JGT() : base("JGT", 0x0D) { }
    }
}
