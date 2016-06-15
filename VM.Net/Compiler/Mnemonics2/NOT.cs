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
    /// Represents the NOT mnemonic, which is used to perform a bitwise inversion on a register. <br/>
    /// This mnemonic wraps around instruction code 0x17
    /// </summary>
    public class NOT : Mneumonic
    {
        public NOT() : base("NOT", new byte[] { 0x17 }) { }

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
                RegisterAddress targetRegister = sourceCrawler.ReadRegister();

                // Add the correct size to the assembly length
                sourceCrawler.AssemblyLength += (uint)(1 + 1);

                // If this is not a label scanning pass, write to output
                if (!isLabelScan)
                {
                    output.Write(ByteCodes[0]); // 0x17
                    output.Write((byte)targetRegister);
                }
            }
        }
    }
}
