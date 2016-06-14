using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler
{
    public class LDX : Mneumonic
    {
        public LDX() : base("LDX", 0x02)
        {
        }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                sourceCrawler.CurrentNdx++;

                ushort value = sourceCrawler.ReadWordValue();
                sourceCrawler.AssemblyLength += 3;

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(value);
                }
            }
        }
    }
}
