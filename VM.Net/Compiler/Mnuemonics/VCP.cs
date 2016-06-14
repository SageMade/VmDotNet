using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler
{
    public class VCP : Mneumonic
    {
        public VCP() : base("VCP", 0x25) { }

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
                    case RegisterAddress.X:
                        subopcode = 0x01;
                        break;
                    case RegisterAddress.Y:
                        subopcode = 0x02;
                        break;
                }

                sourceCrawler.AssemblyLength += 2;

                if (!isLabelScan)
                {

                    //output.Write("VCP UNIMPLEMENTED".ToCharArray());
                    output.Write(ByteCode);
                    output.Write(subopcode);
                    //output.Write(value);
                }
            }
        }
    }
}
