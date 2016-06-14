using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public class JNE : JumpMnuemonic
    {
        public JNE() : base("JNE", 0x0C) { }        
    }
}
