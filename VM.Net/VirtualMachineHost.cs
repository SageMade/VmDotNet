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

        delegate void UpdateCacheDelegate(object sender, EventArgs e);

        public VirtualMachineHost()
        {
            InitializeComponent();

            myProcessor = new Processor();
            myProcessor.Cache.OnRegistersUpdated += ThreadedCacheUpdate;
            myProcessor.Screen = Screen;
            opt5Hz.Checked = true;
        }

        private void RegisterCacheUpdates(object sender, EventArgs e)
        {
            ProcessorCache cache = sender as ProcessorCache;

            string strRegisters = ""; 
            strRegisters = "Register A = $" + cache.RegisterA.ToString("X").PadLeft(2, '0');
            strRegisters += "     Register B = $" + cache.RegisterB.ToString("X").PadLeft(2,'0'); 
            strRegisters += "     Register D = $" + cache.RegisterD.ToString("X").PadLeft(4, '0'); 
            strRegisters += "\nRegister X = $" + cache.RegisterX.ToString("X").PadLeft(4, '0'); 
            strRegisters += "   Register Y = $" + cache.RegisterY.ToString("X").PadLeft(4, '0'); 
            strRegisters += "   Instruction Pointer = $" + myProcessor.InstructionPointer.ToString("X").PadLeft(4, '0');

            this.lblRegisters.Text = strRegisters; 
        }

        private void ThreadedCacheUpdate(object sender, EventArgs e)
        {
            if (Screen.InvokeRequired)
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
    }
}
