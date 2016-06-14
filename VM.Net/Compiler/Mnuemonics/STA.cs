using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class STA : Mneumonic
    {
        public STA() : base("STA", 0x03) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
            {
                RegisterAddress register;
                byte opcode = 0x00;

                sourceCrawler.CurrentNdx++;
                sourceCrawler.EatWhitespace();
                register = sourceCrawler.ReadRegister();

                switch (register)
                {
                    case RegisterAddress.X:
                        opcode = 0x01;
                        break;
                    case RegisterAddress.Y:
                        opcode = 0x02;
                        break;
                }

                sourceCrawler.AssemblyLength += 2;

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(opcode);
                }
            }
        }
    }
}
