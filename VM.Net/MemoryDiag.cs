using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VM.Net.VirtualMachine;

namespace VM.Net
{
    public partial class MemoryDiag : Form
    {
        private Memory myMemory;

        public MemoryDiag()
        {
            InitializeComponent();
        }

        public void LoadMemory(Memory memory)
        {
            myMemory = memory;
            string line = "";

            for (uint index = 1; index < memory.Size; index++)
            {
                line += " " + memory[index].ToString("X2") + " ";

                if (index != 0 && index % 8 == 0)
                {
                    rtbMemory.AppendText(string.Format("0x{0:X4} {1}\n", index - 8, line));
                    line = "";
                }
            }
        }

        private void MemoryDiag_Load(object sender, EventArgs e)
        {
            rtbMemory.Clear();

            string line = "";

            for (uint index = 1; index < myMemory.Size; index++)
            {
                line += " " + myMemory[index].ToString("X2") + " ";

                if (index != 0 && index % 8 == 0)
                {
                    rtbMemory.AppendText(string.Format("0x{0:X4} {1}\n", index - 8, line));
                    line = "";
                }
            }
        }
    }
}
