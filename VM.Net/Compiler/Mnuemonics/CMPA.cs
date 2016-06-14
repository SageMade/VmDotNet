using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class CMPA : Mneumonic
    {
        public CMPA() : base("CMPA", 0x05) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                sourceCrawler.CurrentNdx++;
                byte val = sourceCrawler.ReadByteValue();
                sourceCrawler.AssemblyLength += 2;

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(val);
                }
            }
        }
    }
}
