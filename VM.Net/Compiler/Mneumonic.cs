using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Net.Compiler
{
    
    public abstract class Mneumonic
    {
        private static Dictionary<string, Mneumonic> MnuemonicList;

        static Mneumonic()
        {
            if (MnuemonicList == null)
            {
                MnuemonicList = new Dictionary<string, Mneumonic>();

                Type[] types = (
                    from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()                   // Get the referenced assemblies
                from assemblyType in domainAssembly.GetTypes()                  // Get all types in assembly
                where typeof(Mneumonic).IsAssignableFrom(assemblyType)                  // Check to see if the type is a game rule
                where assemblyType.GetConstructor(Type.EmptyTypes) != null      // Make sure there is an empty constructor
                select assemblyType).ToArray();                                 // Convert IEnumerable to array

                // Iterate over them
                for (int index = 0; index < types.Length; index++)
                {
                    try
                    {
                        Mneumonic instance = (Mneumonic)Activator.CreateInstance(types[index]);

                        if (!MnuemonicList.ContainsKey(instance.Name))
                            MnuemonicList.Add(instance.Name, instance);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to init Mneumonic:");
                        System.Diagnostics.Debug.Write(e);
                    }
                }
            }
        }

        public static Mneumonic GetFromName(string name)
        {
            return MnuemonicList[name.ToUpper()];
        }

        public string Name
        {
            get;
            private set;
        }
        public byte[] ByteCodes
        {
            get;
            private set;
        }

        public byte ByteCode
        {
            get { return ByteCodes[0]; }
        }

        protected Mneumonic(string name, byte byteCode)
        {
            name = name.ToUpper();
            
            if (MnuemonicList.ContainsKey(name))
                throw new InvalidOperationException("Cannot add Mnuemonic with that name, already exists");

            foreach (KeyValuePair<string, Mneumonic> pair in MnuemonicList)
                if (pair.Value.ByteCodes.Contains(byteCode))
                    throw new InvalidOperationException("Cannot add Mnuemonic with that byteCode, code already exists");

            Name = name;
            ByteCodes = new byte[] { byteCode };
            MnuemonicList.Add(Name, this);
        }

        protected Mneumonic(string name, byte[] byteCodes)
        {
            name = name.ToUpper();

            if (MnuemonicList.ContainsKey(name))
                throw new InvalidOperationException("Cannot add Mnuemonic with that name, already exists");

            foreach (KeyValuePair<string, Mneumonic> pair in MnuemonicList)
                for(int index = 0; index < byteCodes.Length; index ++)
                    if (pair.Value.ByteCodes.Contains(byteCodes[index]))
                        throw new InvalidOperationException("Cannot add Mnuemonic with that byteCode, code already exists");

            Name = name;
            ByteCodes = byteCodes;
            MnuemonicList.Add(Name, this);
        }

        public abstract void Interpret(SourceCrawler sourceCrawler, BinaryWriter output, bool isLabelScan);

        public override string ToString()
        {
            string result = Name + " | [";
            for (int index = 0; index < ByteCodes.Length; index++)
                result += String.Format("{0:X2},", ByteCodes[index]);
            result = result.Trim(',');
            result += "]";  
            return result;
        }
    }
}
