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
    /// Represents the PUSH mnemonic, which handles pushing data to the stack. <br/>
    /// This mnemonic wraps around instruction codes 0x34 and 0x35
    /// </summary>
    public class PUSH : Mneumonic
    {
        public PUSH() : base("PUSH", new byte[] { 0x34, 0x35 }) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            // Eat whitespace to right of mnemonic
            sourceCrawler.EatWhitespace();

            // Peek at the next character, if it is a literal delimiter, we parse a literal, otherwise we parse a register
            if (sourceCrawler.Peek() == CompilerSettings.LiteralDelimiter)
            {
                // Pass over delimiter
                sourceCrawler.CurrentNdx++;
                // Read hard-coded location
                uint value = sourceCrawler.ReadWordValue();
                
                // Add the correct size to the assembly length
                sourceCrawler.AssemblyLength += (uint)(1 + CompilerSettings.WORD_LENGTH);

                // If this is not a label scanning pass, write to output
                if (!isLabelScan)
                {
                    output.Write(ByteCodes[0]); // 0x34
                    output.Write(value);
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
                sourceCrawler.AssemblyLength += (1 + 1);

                // If this is not a label scanning pass, write to output
                if (!isLabelScan)
                {
                    output.Write(ByteCodes[1]); // 0x35
                    output.Write((byte)sourceRegister);
                }
            }
            else if (sourceCrawler.Peek() == CompilerSettings.ConstantDelimiter)
            {
                string constant = "";

                while (!char.IsWhiteSpace(sourceCrawler.Peek()))
                    constant += sourceCrawler.Get();

                if (constant.Equals("_ret_addr_"))
                {
                    sourceCrawler.AssemblyLength += 1 + 1;

                    if (!isLabelScan)
                    {
                        output.Write(ByteCodes[1]); // 0x35
                        output.Write((byte)0xF0);
                    }
                }
            }
        }
    }
}
