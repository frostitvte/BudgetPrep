using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace OBSecurity
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (tbPlain.Text.Trim() == string.Empty)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "Please Enter a Plain Text";
            }
            else
            {
                try
                {
                    tbEncrypted.Text = Security.Encrypt(tbPlain.Text.Trim());
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Encryption Successful";
                }
                catch(Exception ex)
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Encryption Failed";
                }
            }
        }

        private void btnDecript_Click(object sender, EventArgs e)
        {
            if (tbEncrypted.Text.Trim() == string.Empty)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "Please Enter an Encrypted Text";
            }
            else
            {
                try
                {
                    tbPlain.Text = Security.Decrypt(tbEncrypted.Text.Trim());
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Decryption Successful";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Decryption Failed";
                }
            }
        }
    }
}
