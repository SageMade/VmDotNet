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
    /// Represents the PASS mnemonic, which is used to pass values to other devices. <br/>
    /// This mnemonic wraps around instruction codes 0x31, 0x31, 0x32 and 0x33
    /// </summary>
    public class PASS : Mneumonic
    {
        public PASS() : base("PASS", new byte[] { 0x32, 0x33, 0x30, 0x31 }) { }

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
                RegisterAddress sourceRegister = sourceCrawler.ReadRegister();
                // Eat the whitespace leading to next parameter
                sourceCrawler.EatWhitespace();

                // Peek at the next character, if it is a literal delimiter, we parse a literal, otherwise we parse a register
                if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read hard-coded location
                    byte deviceId = sourceCrawler.ReadByteValue();

                    // Add the correct size to the assembly length
                    sourceCrawler.AssemblyLength += (uint)(1 + 1 + 1);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[0]); // 0x32
                        output.Write((byte)sourceRegister);
                        output.Write(deviceId);
                    }
                }
                // This is not a literal location, check for register
                else if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read register
                    RegisterAddress destinationRegister = sourceCrawler.ReadRegister();

                    // This instruction is 3 bytes wide
                    sourceCrawler.AssemblyLength += (uint)(1 + 1 + 1);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[1]); // 0x33
                        output.Write((byte)sourceRegister);
                        output.Write((byte)destinationRegister);
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
                    byte deviceId = sourceCrawler.ReadByteValue();

                    // Add the correct size to the assembly length
                    sourceCrawler.AssemblyLength += (uint)(1 + CompilerSettings.WORD_LENGTH + 1);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[2]); // 0x30
                        output.Write(value);
                        output.Write(deviceId);
                    }
                }
                // This is not a literal location, check for register
                else if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read register
                    RegisterAddress destinationRegister = sourceCrawler.ReadRegister();

                    // This instruction is 3 bytes wide
                    sourceCrawler.AssemblyLength += (uint)(1 + CompilerSettings.WORD_LENGTH + 1);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[3]); // 0x31
                        output.Write(value);
                        output.Write((byte)destinationRegister);
                    }
                }
            }
        }
    }
}
