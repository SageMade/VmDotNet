using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class RolByteRegisters : Mneumonic
    {
        public RolByteRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.AssemblyLength += 1;

            if (!isLabelScan)
            {
                output.Write(ByteCode);
            }
        }
    }

    public class ROLA : RolByteRegisters
    {
        public ROLA() : base("ROLA", 0x1C) { }
    }

    public class ROLB : RolByteRegisters
    {
        public ROLB() : base("ROLB", 0x1D) { }
    }
}
