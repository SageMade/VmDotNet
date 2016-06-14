using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM.Net.Common;

namespace VM.Net.Compiler
{
    public class SourceCrawler
    {
        private Hashtable myLabelLookup;
        private string mySource;

        public string Source
        {
            get { return mySource; }
        }
        public char this[int index]
        {
            get { return mySource[index]; }
        }

        public int CurrentNdx;
        public ushort AssemblyLength;

        public SourceCrawler(string source, ushort assemblyLength = 0)
        {
            mySource = source;
            CurrentNdx = 0;
            AssemblyLength = assemblyLength;
        }

        public void SetLookupTable(Hashtable table)
        {
            myLabelLookup = table;
        }

        public char Peek(int offset = 0)
        {
            return mySource[CurrentNdx + offset];
        }

        public char Get()
        {
            CurrentNdx++;
            return mySource[CurrentNdx - 1];
        }

        public byte ReadByteValue()
        {
            byte result = 0;
            LiteralType literalType = LiteralType.Decimal;
            string sval = "";

            if (Peek() == '0')
            {
                if (char.ToLower(Peek(1)) == 'x')
                {
                    CurrentNdx += 2;
                    literalType = LiteralType.Hexidecimal;
                }
                else if (char.ToLower(Peek(1)) == 'b')
                {
                    CurrentNdx += 2;
                    literalType = LiteralType.Binary;
                }
                else if (char.ToLower(Peek(1)) == 'd')
                {
                    CurrentNdx += 2;
                    literalType = LiteralType.Decimal;
                }
            }

            while (char.IsLetterOrDigit(Peek()))
            {
                sval += Get();
            }

            switch (literalType)
            {
                case LiteralType.Binary:
                    result = Convert.ToByte(sval, 2);
                    break;
                case LiteralType.Hexidecimal:
                    result = Convert.ToByte(sval, 16);
                    break;
                case LiteralType.Decimal:
                    result = byte.Parse(sval);
                    break;
                default:
                    result = 0;
                    break;
            }

            return result;
        }

        public ushort ReadWordValue()
        {
            ushort result = 0;
            LiteralType literalType = LiteralType.Decimal;
            string sval = "";

            char peek = Peek();

            if (peek == CompilerSettings.LiteralDelimiter)
                CurrentNdx++;

            peek = Peek();

            if (peek == '0' || char.IsDigit(peek))
            {
                if (char.ToLower(Peek(1)) == 'x')
                {
                    CurrentNdx += 2;
                    literalType = LiteralType.Hexidecimal;
                }
                else if (char.ToLower(Peek(1)) == 'b')
                {
                    CurrentNdx += 2;
                    literalType = LiteralType.Binary;
                }
                else if (char.ToLower(Peek(1)) == 'd')
                {
                    CurrentNdx += 2;
                    literalType = LiteralType.Decimal;
                }
                else if (char.IsWhiteSpace(Peek(1)))
                    literalType = LiteralType.Decimal;

                while(char.IsLetterOrDigit(Peek()))
                {
                    sval += Get();
                }

                switch (literalType)
                {
                    case LiteralType.Binary:
                        result = Convert.ToUInt16(sval, 1);
                        break;
                    case LiteralType.Hexidecimal:
                        result = Convert.ToUInt16(sval, 16);
                        break;
                    case LiteralType.Decimal:
                        result = ushort.Parse(sval);
                        break;
                    default:
                        result = 0;
                        break;
                }
            }
            else
            {
                literalType = LiteralType.Label;
                string label = GetLabelName();
                result = (ushort)(myLabelLookup[label]);
            }

            return result;
        }

        public RegisterAddress ReadRegister()
        {
            RegisterAddress result = RegisterAddress.Unknown;

            if (char.ToUpper(Peek()) == 'X')
                result = RegisterAddress.X;
            if (char.ToUpper(Peek()) == 'Y')
                result = RegisterAddress.Y;
            if (char.ToUpper(Peek()) == 'D')
                result = RegisterAddress.D;
            if (char.ToUpper(Peek()) == 'A')
                result = RegisterAddress.A;
            if (char.ToUpper(Peek()) == 'B')
                result = RegisterAddress.B;

            CurrentNdx++;
            return result;
        }

        public void EatWhitespace()
        {
            while (char.IsWhiteSpace(Peek()))
                CurrentNdx++;
        }

        public string GetLabelName()
        {
            string result = "";

            while(char.IsLetterOrDigit(Peek()))
            {
                if (Peek() == CompilerSettings.LabelEndDelimiter)
                {
                    CurrentNdx++;
                    break;
                }

                result = result + Get();
            }

            return result.ToUpper();
        }

        public void ReadToLineEnd()
        {
            while (mySource[CurrentNdx] != CompilerSettings.NewlineChar)
                CurrentNdx++;
            CurrentNdx++;
        }

        private enum LiteralType
        {
            Decimal = 'd',
            Hexidecimal = 'x',
            Binary = 'b',
            Label  = 'l'
        }
    }
}
