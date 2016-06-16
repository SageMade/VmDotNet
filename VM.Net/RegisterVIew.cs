using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VM.Net.VirtualMachine;

namespace VM.Net
{
    public partial class RegisterView : Form
    {
        Processor myProcessor;

        public RegisterView()
        {
            InitializeComponent();
        }

        public void Attach(Processor processor)
        {
            myProcessor = processor;
            myProcessor.Cache.OnRegistersUpdated += CacheUpdated;

            Type t = typeof(ProcessorCache);

            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_IP")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_SP")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_BP")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_DI")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_SI")));

            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_IAX")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_IBX")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_ICX")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_IDX")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_IEX")));

            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_FAX"), "#.#"));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_FBX"), "#.#"));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_FCX"), "#.#"));

            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_RIA")));
            dgvRegisters.Rows.Add(BuildRow(t.GetProperty("Register_RFA"), "#.#"));
        }

        private DataGridViewRow BuildRow(PropertyInfo register, string format = null)
        {
            DataGridViewRow result = new DataGridViewRow();
            result.CreateCells(dgvRegisters);
            result.Cells[0].Value = register.Name;
            result.Cells[1].Value = register.GetValue(myProcessor.Cache);
            if (format != null)
                result.Cells[1].Style.Format = format;
            result.Tag = register;
            return result;
        }

        private void UpdateRows()
        {
            for(int index = 0; index < dgvRegisters.Rows.Count; index ++)
            {
                if (dgvRegisters.Rows[index].Tag is PropertyInfo)
                {
                    dgvRegisters.Rows[index].Cells[1].Value = (dgvRegisters.Rows[index].Tag as PropertyInfo).GetValue(myProcessor.Cache);
                }
            }
        }

        private void CacheUpdated(object sender, EventArgs e)
        {
            UpdateRows();
        }

        private void RegisterVIew_Load(object sender, EventArgs e)
        {

        }
    }
}
