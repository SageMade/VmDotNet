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
    /// Represents the CMP mnemonic, which is used to compare a value in a register with another value. <br/>
    /// This mnemonic wraps around instruction codes 0x08, 0x09, 0x0A and 0x0B
    /// </summary>
    public class CMP : Mneumonic
    {
        public CMP() : base("CMP", new byte[] { 0x08, 0x09, 0x0A, 0x0B }) { }

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
                    uint compareValue = sourceCrawler.ReadWordValue();

                    // Add the correct size to the assembly length
                    sourceCrawler.AssemblyLength += (uint)(1 + 1 + CompilerSettings.WORD_LENGTH);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[0]); // 0x08
                        output.Write((byte)targetRegister);
                        output.Write(compareValue);
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
                    sourceCrawler.AssemblyLength += (uint)(1 + 1 + 1);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[1]); // 0x09
                        output.Write((byte)targetRegister);
                        output.Write((byte)sourceRegister);
                    }
                }
            }
            // Well it didn't start with a register, so we must be comparing a value
            else if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                // Pass over the register delimiter
                sourceCrawler.CurrentNdx++;
                // Read the value
                uint value = sourceCrawler.ReadWordValue();
                // Eat the whitespace leading to next parameter
                sourceCrawler.EatWhitespace();

                // Peek at the next character, if it is a literal delimiter, we parse a literal, otherwise we parse a register
                if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read hard-coded location
                    uint compareValue = sourceCrawler.ReadWordValue();

                    // Add the correct size to the assembly length
                    sourceCrawler.AssemblyLength += (uint)(1 + CompilerSettings.WORD_LENGTH + CompilerSettings.WORD_LENGTH);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[2]); // 0x0A
                        output.Write(value);
                        output.Write(compareValue);
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
                    sourceCrawler.AssemblyLength += (uint)(1 + CompilerSettings.WORD_LENGTH + 1);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[3]); // 0x0B
                        output.Write(value);
                        output.Write((byte)sourceRegister);
                    }
                }
            }
        }
    }
}
