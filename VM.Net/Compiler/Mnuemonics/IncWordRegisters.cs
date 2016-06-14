using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler.Mnuemonics
{
    public abstract class IncWordRegisters : Mneumonic
    {
        public IncWordRegisters(string name, byte bytecode) : base(name, bytecode) { }

        public override void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan)
        {
            sourceCrawler.AssemblyLength += 1;

            if (!isLabelScan)
            {
                output.Write(ByteCode);
            }
        }
    }

    public class INCX : IncWordRegisters
    {
        public INCX() : base("INCX", 0x14) { }
    }

    public class INCY : IncWordRegisters
    {
        public INCY() : base("INCY", 0x15) { }
    }

    public class INCD : IncWordRegisters
    {
        public INCD() : base("INCD", 0x16) { }
    }
}
