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
    /// Represents the MOV mnemonic, which is used to move data from one register to another. <br/>
    /// This mnemonic wraps around instruction code 0x37
    /// </summary>
    public class MOV : Mneumonic
    {
        public MOV() : base("MOV", new byte[] { 0x37 }) { }

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

                if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
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
                        output.Write(ByteCodes[0]); // 0x37
                        output.Write((byte)targetRegister);
                        output.Write((byte)sourceRegister);
                    }
                }
            }
        }
    }
}
