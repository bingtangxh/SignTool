using System;
using System.IO;
using System.Reflection;
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

            OpenFileDialog open = new OpenFileDialog {
                Filter="Executables|*.exe;*.dll;*.sys",
                Multiselect=true
            };

            if(open.ShowDialog()==DialogResult.OK) {
                foreach(string fileName in open.FileNames) {
                    isCurrentFailed=Program.SignSingleFile(fileName, message => logtxt.Text+=message, message => logtxt.Text+=message+Environment.NewLine);  
                    if(isCurrentFailed) {
                        isAnyErrorHappened=true;
                    }
                }
                if(isAnyErrorHappened) {
                    // MessageBox.Show(this,"Done with some errors! Please check the log for details.","Oh no...",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                } else { 
                    // MessageBox.Show(this,"All Done!","Congratulations!",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            bool isAnyErrorHappened=false;

            // Here used to be FolderBrowserDialog, but it looks ugly and not good for use
            // Then I copied an alternative named FolderSelectDialog from http://www.lyquidity.com/devblog/?p=136 & https://www.cnblogs.com/xyz0835/p/5056464.html
            FolderSelectDialog folderDialog;
            
            folderDialog = new FolderSelectDialog();

            
            if(
                // (folderDialog.ShowDialog(this) == DialogResult.OK)
                folderDialog.ShowDialog(this.Handle)
            ) 
            {
                isAnyErrorHappened=Program.SignFolder(folderDialog.FileName, message => logtxt.Text+=message, message => logtxt.Text+=message+Environment.NewLine);
                
                if(isAnyErrorHappened) {
                    MessageBox.Show(this,"Done with some errors! Please check the log for details.","Oh no...",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                } else {
                    MessageBox.Show(this,"All Done!","Congratulations!",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
        }
    }
}
