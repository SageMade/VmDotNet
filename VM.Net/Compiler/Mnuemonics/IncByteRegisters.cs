using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class IncByteRegisters : Mneumonic
    {
        public IncByteRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.AssemblyLength += 1;

            if (!isLabelScan)
            {
                output.Write(ByteCode);
            }
        }
    }

    public class INCA : IncByteRegisters
    {
        public INCA() : base("INCA", 0x12) { }
    }

    public class INCB : IncByteRegisters
    {
        public INCB() : base("INCB", 0x13) { }
    }
}
