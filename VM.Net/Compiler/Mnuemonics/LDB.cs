using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler
{
    public class LDB : Mneumonic
    {
        public LDB() : base("LDB", 0x0F)
        {
        }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                sourceCrawler.CurrentNdx++;

                byte value = sourceCrawler.ReadByteValue();
                sourceCrawler.AssemblyLength += 2;

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(value);
                }
            }
        }
    }
}
