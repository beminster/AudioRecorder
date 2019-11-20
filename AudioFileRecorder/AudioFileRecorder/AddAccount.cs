using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioFileRecorder
{
    public partial class AddAccount : Form
    {
        public AddAccount()
        {
            InitializeComponent();
        }

        string newAccount = null;
        public string accountNumber {get; set;}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            newAccount = textBox1.Text;
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            newAccount = textBox1.Text;

            if (newAccount == null) return;
            else
            {
                if (newAccount.Length != 4)
                {
                    MessageBox.Show("Please enter a 4 digit account number");
                    return;
                }
                else
                {
                    this.accountNumber = newAccount;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}
