using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler.Mnuemonics
{
    public class LOD : Mneumonic
    {
        public LOD() : base("LOD", 0x29) { }

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
                sourceCrawler.EatWhitespace();
                ushort value = sourceCrawler.ReadWordValue();

                switch (register)
                {
                    case RegisterAddress.A:
                        opcode = 0x01;
                        break;
                    case RegisterAddress.B:
                        opcode = 0x02;
                        break;
                    case RegisterAddress.X:
                        opcode = 0x03;
                        break;
                    case RegisterAddress.Y:
                        opcode = 0x04;
                        break;
                    case RegisterAddress.D:
                        opcode = 0x05;
                        break;
                }

                sourceCrawler.AssemblyLength += 4;

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(opcode);
                    output.Write(value);
                }
            }
        }
    }
}
