using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler
{
    public class NOT : Mneumonic
    {
        public NOT() : base("NOT", 0x27) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.EatWhitespace();

            if (sourceCrawler.Peek() == CompilerSettings.RegisterDelimiter)
            {
                RegisterAddress register;
                byte subopcode = 0x00;

                sourceCrawler.CurrentNdx++;
                sourceCrawler.EatWhitespace();
                register = sourceCrawler.ReadRegister();

                switch (register)
                {
                    case RegisterAddress.A:
                        subopcode = 0x01;
                        break;
                    case RegisterAddress.B:
                        subopcode = 0x02;
                        break;
                    case RegisterAddress.X:
                        subopcode = 0x03;
                        break;
                    case RegisterAddress.Y:
                        subopcode = 0x04;
                        break;
                    case RegisterAddress.D:
                        subopcode = 0x05;
                        break;
                }

                sourceCrawler.AssemblyLength += 2;

                if (!isLabelScan)
                {
                    output.Write(ByteCode);
                    output.Write(subopcode);
                }
            }
        }
    }
}
