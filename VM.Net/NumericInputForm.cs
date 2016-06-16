using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VM.Net
{
    public partial class frmNumericInput : Form
    {
        private frmNumericInput()
        {
            InitializeComponent();
        }

        public static uint Show(string title, string message)
        {
            frmNumericInput diag = new frmNumericInput();
            diag.Text = title;
            diag.lblMessage.Text = message;
            diag.ShowDialog();

            uint value = 0;

            if (diag.DialogResult == DialogResult.OK)
            {
                var hex = diag.txtInput.Text;
                if (hex.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) || hex.StartsWith("&H", StringComparison.CurrentCultureIgnoreCase))
                {
                    hex = hex.Substring(2);
                }

                uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out value);
            }

            return value;
        }
    }
}
