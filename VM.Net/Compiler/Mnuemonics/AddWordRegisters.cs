using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class AddWordRegisters : Mneumonic
    {
        public AddWordRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                sourceCrawler.CurrentNdx++;

                sourceCrawler.AssemblyLength += 3;
                if (isLabelScan) return;

                ushort value = sourceCrawler.ReadWordValue();

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(value);
                }
            }
        }
    }

    public class ADDX : AddWordRegisters
    {
        public ADDX() : base("ADDX", 0x2B) { }
    }

    public class ADDY : AddWordRegisters
    {
        public ADDY() : base("ADDY", 0x2C) { }
    }
}
