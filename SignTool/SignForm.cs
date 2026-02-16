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

        private void Button1_Click(object sender, EventArgs e)
        {
            bool isAnyErrorHappened=false,isCurrentFailed=false;

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Executables|*.exe;*.dll;*.sys";
            open.Multiselect = true;

            if(open.ShowDialog()==DialogResult.OK) {
                foreach(string fileName in open.FileNames) {
                    isCurrentFailed=Program.SignSingleFile(fileName, message => logtxt.Text+=message, message => logtxt.Text+=message+Environment.NewLine);  
                    if(isCurrentFailed) {
                        isAnyErrorHappened=true;
                    }
                }
                if(isAnyErrorHappened) {
                    MessageBox.Show("Done with some errors! Please check the log for details.");
                } else { 
                    MessageBox.Show("All Done!");
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            bool isAnyErrorHappened=false;

            FolderBrowserDialog folderDialog;

            folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                isAnyErrorHappened=Program.SignFolder(folderDialog.SelectedPath, message => logtxt.Text+=message, message => logtxt.Text+=message+Environment.NewLine);
                
                if(isAnyErrorHappened) {
                    MessageBox.Show("Done with some errors! Please check the log for details.");
                } else {
                    MessageBox.Show("All Done!");
                }
            }
        }
    }
}
