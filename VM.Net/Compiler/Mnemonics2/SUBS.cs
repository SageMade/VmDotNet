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
    /// Represents the SUBS mnemonic, which subtracts a value to a register, and stores the result in a third register. <br/>
    /// This mnemonic wraps around instruction codes 0x24 and 0x25
    /// </summary>
    public class SUBS : Mneumonic
    {
        public SUBS() : base("SUBS", new byte[] { 0x24, 0x25 }) { }

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
                    uint value = sourceCrawler.ReadWordValue();

                    // Eat whitepace to next delimiter
                    sourceCrawler.EatWhitespace();

                    // Make sure we have a target register
                    if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
                    {
                        sourceCrawler.CurrentNdx++;
                        RegisterAddress destinationRegister = sourceCrawler.ReadRegister();

                        // Add the correct size to the assembly length
                        sourceCrawler.AssemblyLength += (uint)(1 + 1 + CompilerSettings.WORD_LENGTH + 1);

                        // If this is not a label scanning pass, write to output
                        if (!isLabelScan)
                        {
                            output.Write(ByteCodes[0]); // 0x20
                            output.Write((byte)targetRegister);
                            output.Write(value);
                            output.Write((byte)destinationRegister);
                        }
                    }
                }
                // This is not a literal location, check for register
                else if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read register
                    RegisterAddress sourceRegister = sourceCrawler.ReadRegister();

                    // Make sure we have a target register
                    if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
                    {
                        sourceCrawler.CurrentNdx++;
                        RegisterAddress destinationRegister = sourceCrawler.ReadRegister();

                        // This instruction is 3 bytes wide
                        sourceCrawler.AssemblyLength += (1 + 1 + 1 + 1);

                        // If this is not a label scanning pass, write to output
                        if (!isLabelScan)
                        {
                            output.Write(ByteCodes[1]); // 0x21
                            output.Write((byte)targetRegister);
                            output.Write((byte)sourceRegister);
                            output.Write((byte)destinationRegister);
                        }
                    }
                }
            }
        }
    }
}
