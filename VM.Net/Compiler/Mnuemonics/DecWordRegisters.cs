using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class DecWordRegisters : Mneumonic
    {
        public DecWordRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.AssemblyLength += 1;

            if (!isLabelScan)
            {
                output.Write(ByteCode);
            }
        }
    }

    public class DECX : DecWordRegisters
    {
        public DECX() : base("DECX", 0x19) { }
    }

    public class DECY : DecWordRegisters
    {
        public DECY() : base("DECY", 0x1A) { }
    }

    public class DECD : DecWordRegisters
    {
        public DECD() : base("DECD", 0x1B) { }
    }
}
