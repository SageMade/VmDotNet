using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VM.Net.Compiler;
using VM.Net.VirtualMachine;

namespace VM.Net
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Assembler assembler = new Assembler();

            string[] files = Directory.GetFiles("Content");

            for(int index = 0; index < files.Length; index ++)
            {
                if (files[index].EndsWith(".vm"))
                    Compile(assembler, files[index]);
            }

            Console.WriteLine("Compiled...");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            VirtualMachineHost host = new VirtualMachineHost();
            VirtualScreen screen = host.Screen;
            screen.Poke(0xa000, 65);
            host.ShowDialog();
            Console.ReadKey();
        }

        private static void Compile(Assembler assembler, string path)
        {
            path = Directory.GetParent(path).FullName + "\\" + Path.GetFileNameWithoutExtension(path);

            BinaryWriter resultWriter;
            TextReader reader = File.OpenText(path + ".vm");

            File.Delete(path + ".vbc");
            FileStream outStream = File.OpenWrite(path + ".vbc");
            resultWriter = new BinaryWriter(outStream);
            string source = reader.ReadToEnd();

            assembler.Assemble(source, resultWriter);

            resultWriter.Close();
            outStream.Close();

            resultWriter.Dispose();
            outStream.Dispose();
        }
    }
}
