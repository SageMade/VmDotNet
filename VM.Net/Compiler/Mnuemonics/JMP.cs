using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class JMP : Mneumonic
    {
        public JMP() : base("JMP", 0x0A) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                sourceCrawler.CurrentNdx++;
                sourceCrawler.AssemblyLength += 3;
                if (isLabelScan) return;

                ushort jumpTo = sourceCrawler.ReadWordValue();


                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(jumpTo);
                }
            }
        }
    }
}
