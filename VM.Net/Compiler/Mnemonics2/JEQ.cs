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
    /// Represents the JEQ mnemonic, which is used to jump the instruction pointer to a given address <b>if</b> the equality flag is set. <br/>
    /// This mnemonic wraps around instruction code 0x0D
    /// </summary>
    public class JEQ : Mneumonic
    {
        public JEQ() : base("JEQ", new byte[] { 0x0D }) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            // Eat whitespace to right of mnemonic
            sourceCrawler.EatWhitespace();

            // Peek to make sure next character is a register delimiter, otherwise we return
            if (sourceCrawler.Peek() == CompilerSettings.LabelDelimiter)
            {
                // Pass over the register delimiter
                sourceCrawler.CurrentNdx++;
                // Read the register
                uint location = sourceCrawler.ReadLabelLocation();
                
                // This instruction is 1 byte for instruction, and 1 word for location
                sourceCrawler.AssemblyLength += (uint)(1 + CompilerSettings.WORD_LENGTH);

                // If this is not a label scanning pass, write to output
                if (!isLabelScan)
                {
                    output.Write(ByteCodes[0]); // 0x0D
                    output.Write(location);
                }
            }
        }
    }
}
