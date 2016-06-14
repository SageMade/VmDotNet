using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class RorByteRegisters : Mneumonic
    {
        public RorByteRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.AssemblyLength += 1;

            if (!isLabelScan)
            {
                output.Write(ByteCode);
            }
        }
    }

    public class RORA : RorByteRegisters
    {
        public RORA() : base("RORA", 0x1E) { }
    }

    public class RORB : RorByteRegisters
    {
        public RORB() : base("RORB", 0x1F) { }
    }
}
