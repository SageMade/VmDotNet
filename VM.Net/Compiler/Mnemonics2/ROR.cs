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
    /// Represents the ROL mnemonic, which is used to bitwise roll a value to the right. <br/>
    /// This mnemonic wraps around instruction code 0x14
    /// </summary>
    public class ROR : Mneumonic
    {
        public ROR() : base("ROR", new byte[] { 0x14 }) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            // Eat whitespace to right of mnemonic
            sourceCrawler.EatWhitespace();

            // Peek to make sure next character is a register delimiter, otherwise we return
            if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
            {
                // Pass over the register delimiter
                sourceCrawler.CurrentNdx++;
                // Read the register
                RegisterAddress register = sourceCrawler.ReadRegister();
                
                // This instruction is 1 byte for instruction, and 1 word for location
                sourceCrawler.AssemblyLength += (uint)(1 + 1);

                // If this is not a label scanning pass, write to output
                if (!isLabelScan)
                {
                    output.Write(ByteCodes[0]); // 0x13
                    output.Write((byte)register);
                }
            }
        }
    }
}
