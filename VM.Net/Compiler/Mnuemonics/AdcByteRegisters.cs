using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class AdcByteRegisters : Mneumonic
    {
        public AdcByteRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.AssemblyLength += 1;

            if (!isLabelScan)
            {
                output.Write(ByteCode);
            }
        }
    }

    public class ADCA : AdcByteRegisters
    {
        public ADCA() : base("ADCA", 0x22) { }
    }

    public class ADCB : AdcByteRegisters
    {
        public ADCB() : base("ADCB", 0x23) { }
    }
}
