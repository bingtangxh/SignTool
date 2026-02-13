using System;
using System.IO;
using System.Windows.Forms;

namespace SignTool1
{
    public partial class SignForm : Form
    {
        public SignForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            open.Filter = "Executables|*.exe;*.dll;*.sys";
            open.Multiselect = true;

            if (open.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in open.FileNames)
                {
                    logtxt.Text += fileName + "...";

                    Application.DoEvents();

                    SignTool.SignWithCert(fileName, "http://timestamp.verisign.com/scripts/timstamp.dll");

                    logtxt.Text += "OK!" + Environment.NewLine;
                }

                MessageBox.Show("Done!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog;

            folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string[] fileNames = Directory.GetFiles(folderDialog.SelectedPath, "*", SearchOption.AllDirectories);
                
                foreach (string fileName in fileNames)
                {
                    if (fileName.Contains(".exe")
                        || fileName.Contains(".dll")
                        || fileName.Contains(".sys"))
                    {
                        logtxt.Text += fileName + "...";

                        Application.DoEvents();

                        SignTool.SignWithCert(fileName, "http://timestamp.verisign.com/scripts/timstamp.dll");

                        logtxt.Text += "OK!" + Environment.NewLine;
                    }
                }

                MessageBox.Show("Done!");
            }
        }
    }
}
