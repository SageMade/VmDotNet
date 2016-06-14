using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Common
{
    public static class CompilerSettings
    {
        public static char LabelEndDelimiter = ':';
        public static char LiteralDelimiter = '#';
        public static char RegisterDelimiter = ',';
        public static char NewlineChar = '\n';
        public static int MagicLength
        {
            get { return MagicCharacters.Length; }
        }
        public static char[] MagicCharacters = { 'B', '3', '2' };

        public static bool VerifyMagicNumbers(char[] input)
        {
            if (input.Length != MagicCharacters.Length)
                return false;

            for (int index = 0; index < input.Length; index++)
                if (input[index] != MagicCharacters[index])
                    return false;

            return true;
        }
    }
}
