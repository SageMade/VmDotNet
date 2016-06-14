using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler
{
    public class Assembler
    {
        private Hashtable myLabelLookup;
        private bool isEnd;
        private ushort myExecutionAddress;

        private SourceCrawler mySourceCrawler;

        public Assembler()
        {
            myLabelLookup = new Hashtable();
            myExecutionAddress = 0;
            isEnd = false;
        }

        public void Assemble(string program, BinaryWriter output)
        {
            myLabelLookup = new Hashtable();
            myExecutionAddress = 0;
            isEnd = false;

            mySourceCrawler = new SourceCrawler(program, 0);
            mySourceCrawler.SetLookupTable(myLabelLookup);
            
            // Write the majic number
            output.Write(CompilerSettings.MagicCharacters);

            output.Write(mySourceCrawler.AssemblyLength);
            output.Write((ushort)0); // Leave space for execution address

            long start = output.BaseStream.Position;

            Parse(output); // Actually generate bytecode

            long end = output.BaseStream.Position;

            ushort progLength = (ushort)(end - start);

            output.Seek(3, SeekOrigin.Begin); // Seek to execution address
            output.Write(progLength); // write execution address
            output.Seek(3 + 2, SeekOrigin.Begin); // Seek to execution address
            output.Write(myExecutionAddress); // write execution address
        }

        private void Parse(BinaryWriter output)
        {
            mySourceCrawler.CurrentNdx = 0;

            while (isEnd == false)
                PerformLabelScan(output, true);

            isEnd = false;
            mySourceCrawler.CurrentNdx = 0;

            while(isEnd == false)
                PerformLabelScan(output, false);
        }

        private void PerformLabelScan(BinaryWriter output, bool isLabelScan)
        {
            if (char.IsLetter(mySourceCrawler.Peek()))
            {
                if (isLabelScan)
                    myLabelLookup.Add(mySourceCrawler.GetLabelName(), mySourceCrawler.AssemblyLength);

                mySourceCrawler.ReadToLineEnd();
                return;
            }
            mySourceCrawler.EatWhitespace();
            ReadMneumonic(output, isLabelScan);
        }

        private void ReadMneumonic(BinaryWriter output, bool isLabelScan)
        {
            string mneumonic = "";

            while (!char.IsWhiteSpace(mySourceCrawler.Peek()))
            {
                mneumonic = mneumonic + mySourceCrawler.Get();
            }

            mneumonic = mneumonic.ToUpper();

            // Special case for the END mneumonic
            if (mneumonic == "END")
            {
                isEnd = true;
                DoEnd(output, isLabelScan);
                mySourceCrawler.EatWhitespace();
                myExecutionAddress = (ushort)myLabelLookup[mySourceCrawler.GetLabelName()];
                return;
            }
            else
            {
                Mneumonic mneumonicInstance = Mneumonic.GetFromName(mneumonic);
                mneumonicInstance.Interpret(mySourceCrawler, output, isLabelScan);
            }

            mySourceCrawler.ReadToLineEnd();
        }

        private void DoEnd(BinaryWriter output, bool isLabelScan)
        {
            mySourceCrawler.AssemblyLength++;

            if (!isLabelScan)
            {
                output.Write((byte)0x04);
            }
        }
    }
}
