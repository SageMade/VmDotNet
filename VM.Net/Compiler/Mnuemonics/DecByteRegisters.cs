using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class DecByteRegisters : Mneumonic
    {
        public DecByteRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.AssemblyLength += 1;

            if (!isLabelScan)
            {
                output.Write(ByteCode);
            }
        }
    }

    public class DECA : DecByteRegisters
    {
        public DECA() : base("DECA", 0x17) { }
    }

    public class DECB : DecByteRegisters
    {
        public DECB() : base("DECB", 0x18) { }
    }
}
