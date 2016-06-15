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
    /// Represents the POP mnemonic, which handles popping data off the stack into a register. <br/>
    /// This mnemonic wraps around instruction codes 0x36
    /// </summary>
    public class POP : Mneumonic
    {
        public POP() : base("POP", new byte[] { 0x36 }) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            // Eat whitespace to right of mnemonic
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
            {
                // Pass over delimiter
                sourceCrawler.CurrentNdx++;
                // Read register
                RegisterAddress destinationRegister = sourceCrawler.ReadRegister();


                // This instruction is 3 bytes wide
                sourceCrawler.AssemblyLength += (1 + 1);

                // If this is not a label scanning pass, write to output
                if (!isLabelScan)
                {
                    output.Write(ByteCodes[0]); // 0x35
                    output.Write((byte)destinationRegister);
                }
            }
        }
    }
}
