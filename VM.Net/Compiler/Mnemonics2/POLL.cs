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
    /// Represents the POLL mnemonic, which will poll the device with the given ID and store the result in the register. <br/>
    /// This mnemonic wraps around instruction codes 0x2E and 0x2F
    /// </summary>
    public class POLL : Mneumonic
    {
        public POLL() : base("POLL", new byte[] { 0x2E, 0x2F }) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            // Eat whitespace to right of mnemonic
            sourceCrawler.EatWhitespace();
            
            // Peek to make sure next character is a register delimiter, otherwise we return
            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                // Pass over the register delimiter
                sourceCrawler.CurrentNdx++;
                // Read the register
                byte deviceID = sourceCrawler.ReadByteValue();
                // Eat the whitespace leading to next parameter
                sourceCrawler.EatWhitespace();

                // Peek at the next character, if it is a literal delimiter, we parse a literal, otherwise we parse a register
                if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read hard-coded location
                    RegisterAddress targetRegister = sourceCrawler.ReadRegister();

                    // Add the correct size to the assembly length
                    sourceCrawler.AssemblyLength += (uint)(1 + 1 + 1);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[0]); // 0x2E
                        output.Write(deviceID);
                        output.Write((byte)targetRegister);
                    }
                }
            }
            else if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
            {
                sourceCrawler.CurrentNdx++;
                RegisterAddress sourceRegister = sourceCrawler.ReadRegister();
                sourceCrawler.EatWhitespace();

                // Peek at the next character, if it is a literal delimiter, we parse a literal, otherwise we parse a register
                if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
                {
                    // Pass over delimiter
                    sourceCrawler.CurrentNdx++;
                    // Read hard-coded location
                    RegisterAddress targetRegister = sourceCrawler.ReadRegister();

                    // Add the correct size to the assembly length
                    sourceCrawler.AssemblyLength += (uint)(1 + 1 + 1);

                    // If this is not a label scanning pass, write to output
                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[1]); // 0x2E
                        output.Write((byte)sourceRegister);
                        output.Write((byte)targetRegister);
                    }
                }
            }
        }
    }
}
