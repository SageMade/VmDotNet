using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class WordCompare : Mneumonic
    {
        public WordCompare(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                sourceCrawler.CurrentNdx++;
                ushort val = sourceCrawler.ReadWordValue();
                sourceCrawler.AssemblyLength += 3;

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(val);
                }
            }
        }
    }
}
