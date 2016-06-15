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

        private static readonly string[] REGISTER_NAMES = Enum.GetNames(typeof(RegisterAddress));

        public string Source
        {
            get { return mySource; }
        }
        public char this[int index]
        {
            get { return mySource[index]; }
        }

        public int CurrentNdx;
        public uint AssemblyLength;

        public char Current
        {
            get { return mySource[CurrentNdx]; }
        }
        public char Next
        {
            get { return mySource[CurrentNdx + 1]; }
        }

        public SourceCrawler(string source, ushort assemblyLength = 0)
        {
            mySource = source.Replace("\r", "");
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

        public uint ReadLabelLocation()
        {
            string label = GetLabelName();

            if (myLabelLookup.ContainsKey(label))
                return (uint)(myLabelLookup[label]);
            else
                return 0;
        }

        public uint ReadWordValue()
        {
            uint result = 0;
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
                        result = Convert.ToUInt32(sval, 1);
                        break;
                    case LiteralType.Hexidecimal:
                        result = Convert.ToUInt32(sval, 16);
                        break;
                    case LiteralType.Decimal:
                        result = uint.Parse(sval);
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
                result = (uint)(myLabelLookup[label]);
            }

            return result;
        }

        public RegisterAddress ReadRegister()
        {
            RegisterAddress result = RegisterAddress.Unknown;

            string str = "";

            while (!char.IsWhiteSpace(Peek()))
                str += Get();

            if (REGISTER_NAMES.Contains(str))
                result = (RegisterAddress)Enum.Parse(typeof(RegisterAddress), str);
            
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

            while(!char.IsWhiteSpace(Peek()))
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
