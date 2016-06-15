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
    /// Represents the STOR mnemonic, which is used to store values from a register to memory. <br/>
    /// This mnemonic wraps around instruction codes 0x06 and 0x07
    /// </summary>
    public class STOR : Mneumonic
    {
        public STOR() : base("STOR", new byte[] { 0x06, 0x07 }) { }

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
                // Eat the whitespace leading to next parameter
                sourceCrawler.EatWhitespace();

                // Peek at the next character, if it is a literal delimiter, we parse a literal, otherwise we parse a register
                if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read hard-coded location
                    uint location = sourceCrawler.ReadWordValue();

                    // Add the correct size to the assembly length
                    sourceCrawler.AssemblyLength += (uint)(2 + CompilerSettings.WORD_LENGTH);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[0]); // 0x06
                        output.Write((byte)targetRegister);
                        output.Write(location);
                    }
                }
                // This is not a literal location, check for register
                else if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read register
                    RegisterAddress sourceRegister = sourceCrawler.ReadRegister();

                    // This instruction is 3 bytes wide
                    sourceCrawler.AssemblyLength += 3;

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[1]); // 0x07
                        output.Write((byte)targetRegister);
                        output.Write((byte)sourceRegister);
                    }
                }
            }
        }
    }
}
