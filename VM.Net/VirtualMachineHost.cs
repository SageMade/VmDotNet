using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VM.Net.Common;
using VM.Net.Compiler;
using VM.Net.VirtualMachine;

namespace VM.Net
{
    public partial class VirtualMachineHost : Form
    {
        public VirtualScreen Screen
        {
            get { return myVirtualScreen; }
        }

        Processor myProcessor;
        Assembler myAssembler;

        VirtualKeyboard myVirtualKeyboard;

        delegate void UpdateCacheDelegate(object sender, EventArgs e);
        
        public VirtualMachineHost()
        {
            InitializeComponent();

            myAssembler = new Assembler();

            myVirtualKeyboard = new VirtualKeyboard(this);

            myProcessor = new Processor();
            myProcessor.Cache.OnRegistersUpdated += ThreadedCacheUpdate;
            myProcessor.AttachPeripheral(Screen, 1);
            myProcessor.AttachPeripheral(myVirtualKeyboard, 2);
            opt5Hz.Checked = true;
        }

        private void RegisterCacheUpdates(object sender, EventArgs e)
        {
            ProcessorCache cache = sender as ProcessorCache;

            string strRegisters = "";
            strRegisters += "Stack Size: " + cache.Register_SP;
            //strRegisters = "Register A = $" + cache.RegisterA.ToString("X").PadLeft(2, '0');
            //strRegisters += "     Register B = $" + cache.RegisterB.ToString("X").PadLeft(2,'0'); 
            //strRegisters += "     Register D = $" + cache.RegisterD.ToString("X").PadLeft(4, '0'); 
            //strRegisters += "\nRegister X = $" + cache.RegisterX.ToString("X").PadLeft(4, '0'); 
            //strRegisters += "   Register Y = $" + cache.RegisterY.ToString("X").PadLeft(4, '0'); 
            //strRegisters += "   Instruction Pointer = $" + myProcessor.InstructionPointer.ToString("X").PadLeft(4, '0');

            this.lblRegisters.Text = strRegisters; 
        }

        private void ThreadedCacheUpdate(object sender, EventArgs e)
        {
            if (Screen.InvokeRequired && !IsDisposed && !Disposing)
            {
                this.Invoke(new UpdateCacheDelegate(RegisterCacheUpdates), new object[] { sender, e });
            }
            else
            {
                RegisterCacheUpdates(sender, e);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile.ShowDialog();

            BinaryReader reader;
            FileStream input = (FileStream)dlgOpenFile.OpenFile();
            reader = new BinaryReader(input);

            myProcessor.LoadProgram(reader, 4096);
            myProcessor.ExecuteProgramFromMemory(4096);
        }

        private void UncheckAllClocks()
        {
            foreach(ToolStripMenuItem item in mnuClockSpeed.DropDownItems)
            {
                item.Checked = false;
            }
        }

        private void ClockSpeedClicked(object sender, EventArgs e)
        {
            string tag = (sender as ToolStripMenuItem).Tag as string;
            long clockSpeed;

            if (long.TryParse(tag, out clockSpeed))
            {
                myProcessor.ClockSpeed = clockSpeed;
                UncheckAllClocks();
                (sender as ToolStripMenuItem).Checked = true;
            }
        }

        private void optPauseClicked(object sender, EventArgs e)
        {
            optPaused.Checked = !optPaused.Checked;

            myProcessor.IsPasued = optPaused.Checked;
        }

        private void optRamDump_Click(object sender, EventArgs e)
        {
            myProcessor.Memory.DumpText("ram_dump.txt", 4096, 5120);
            MessageBox.Show("Dumped RAM 4096 - 5120 in ram_dump.txt");
        }

        private void optCompile_Click(object sender, EventArgs e)
        {
            dlgOpenVmFile.ShowDialog();

            if (File.Exists(dlgOpenVmFile.FileName))
            {
                myAssembler.Compile(dlgOpenVmFile.FileName);
            }
        }

        private void optCompileAndLoad_Click(object sender, EventArgs e)
        {
            uint loadLoc = frmNumericInput.Show("Enter Location", "Enter the location to load program in hexidecimal");

            if (loadLoc != 0)
            {
                dlgOpenVmFile.ShowDialog();

                if (File.Exists(dlgOpenVmFile.FileName))
                {
                    byte[] data = myAssembler.CompileRaw(dlgOpenVmFile.FileName);
                    
                    myProcessor.LoadProgram(data, loadLoc);
                }
            }
        }

        private void optExcecute_Click(object sender, EventArgs e)
        {
            uint loadLoc = frmNumericInput.Show("Enter Location", "Enter the location to load program from in hexidecimal");

            if (loadLoc != 0)
            {
                myProcessor.ExecuteProgramFromMemory(loadLoc);
            }
        }

        private void optMemoryView_Click(object sender, EventArgs e)
        {
            MemoryDiag diag = new MemoryDiag();
            diag.LoadMemory(myProcessor.Memory);
            diag.Show();
        }

        private void optRegisterView_Click(object sender, EventArgs e)
        {
            RegisterView diag = new RegisterView();
            diag.Attach(myProcessor);
            diag.Show();
        }
    }
}
