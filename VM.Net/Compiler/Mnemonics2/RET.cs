using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnemonics2
{
    /// <summary>
    /// Represents the RET mnemonic, which handles sending the instruction pointer back to the calling section. <br/>
    /// This mnemonic wraps around instruction codes 0x38
    /// </summary>
    public class RET : Mneumonic
    {
        public RET() : base("RET", new byte[] { 0x38 }) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            // Eat whitespace to right of mnemonic
            sourceCrawler.EatWhitespace();

            uint localSize = 0;

            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                // Pass over delimiter
                sourceCrawler.CurrentNdx++;
                // Read register
                localSize = sourceCrawler.ReadWordValue();
            }

            // This instruction is 3 bytes wide
            sourceCrawler.AssemblyLength += (1 + 1);

            // If this is not a label scanning pass, write to output
            if (!isLabelScan)
            {
                output.Write(ByteCodes[0]); // 0x37
                output.Write(localSize);
            }
        }
    }
}
