using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class AddByteRegisters : Mneumonic
    {
        public AddByteRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                sourceCrawler.CurrentNdx++;

                sourceCrawler.AssemblyLength += 2;
                if (isLabelScan) return;

                byte value = sourceCrawler.ReadByteValue();

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(value);
                }
            }
        }
    }

    public class ADDA : AddByteRegisters
    {
        public ADDA() : base("ADDA", 0x20) { }
    }

    public class ADDB : AddByteRegisters
    {
        public ADDB() : base("ADDB", 0x21) { }
    }
}
