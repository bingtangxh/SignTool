using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SignTool1 {
    static class Program {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd,int nCmdShow);

        private const int SW_HIDE = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(args.Length==0) {
                // No arguments, run GUI
                if(GetConsoleWindow()!=IntPtr.Zero) {
                    // Not NULL, Hide console window
                    ShowWindow(GetConsoleWindow(),SW_HIDE);
                } else {
                    // NULL, do nothing
                }
                Application.Run(new SignForm());
            } else {
                // Arguments provided, run in command-line mode
                if(
                    args[0].Equals("/FILE",StringComparison.OrdinalIgnoreCase)||
                    args[0].Equals("-FILE",StringComparison.OrdinalIgnoreCase)
                ) {
                    SignSingleFile(args[1],Console.Write,Console.WriteLine);
                } else if(
                      args[0].Equals("/FOLDER",StringComparison.OrdinalIgnoreCase)||
                      args[0].Equals("-FOLDER",StringComparison.OrdinalIgnoreCase)
                      ) {
                    bool isAnyErrorHappened = SignFolder(args[1],Console.Write,Console.WriteLine);
                    if(isAnyErrorHappened) {
                        Console.WriteLine("Some files failed to sign.");
                    } else {
                        Console.WriteLine("All done.");
                    }

                } else if(
                      args[0].Equals("/?",StringComparison.OrdinalIgnoreCase)||
                      args[0].Equals("-?",StringComparison.OrdinalIgnoreCase)||
                      args[0].Equals("-H",StringComparison.OrdinalIgnoreCase)||
                      args[0].Equals("/H",StringComparison.OrdinalIgnoreCase)||
                      args[0].Equals("--HELP",StringComparison.OrdinalIgnoreCase)
                    ) {
                    ShowUsage();
                } else {
                    Console.WriteLine("Could not specify arguments.");
                    ShowUsage();
                }
            }
        }

        public static bool SignSingleFile(string fileName,Action<string> logAction,Action<string> logNewLineAction) {
            logAction("Signing "+fileName+"...");
            Application.DoEvents();
            try {
                SignTool.SignWithCert(fileName,"http://timestamp.verisign.com/scripts/timstamp.dll");
                logNewLineAction("Done.");
            }
            catch(Exception ex) {
                logNewLineAction("Failed. "+ex.ToString());
                return true;
            }
            return false;
        }

        public static bool SignFolder(string folderPath,Action<string> logAction,Action<string> logNewLineAction) {
            bool isAnyErrorHappened = false, isCurrentFailed = false;
            string[] fileNames;
            try {
                fileNames=Directory.GetFiles(folderPath,"*",SearchOption.AllDirectories);
            }
            catch(Exception ex) {
                logNewLineAction("Failed to get files in folder.");
                logNewLineAction(ex.ToString());
                isAnyErrorHappened=true;
                return isAnyErrorHappened;
            }
            foreach(string fileName in fileNames) {
                if(
                    fileName.Contains(".exe")||
                    fileName.Contains(".dll")||
                    fileName.Contains(".sys")
                  ) {
                    isCurrentFailed=SignSingleFile(fileName,logAction,logNewLineAction);
                    if(isCurrentFailed) {
                        isAnyErrorHappened=true;
                    }
                }
            }
            return isAnyErrorHappened;
        }

        private static void ShowUsage() {
            Console.WriteLine("SignTool - A simple tool to sign executable files with a certificate.");
            Console.WriteLine("");
            Console.WriteLine("Mention that it is different from SignTool given in the Windows SDK.");
            Console.WriteLine("");
            Console.WriteLine("You can run me in GUI mode with no arguments given, and also CLI mode.");
            Console.WriteLine("");
            Console.WriteLine("The command line usage is below:");
            Console.WriteLine("  SignTool /FILE <filePath>     - Sign a single file");
            Console.WriteLine("  SignTool /FOLDER <folderPath> - Sign all files in a folder");
            Console.WriteLine("  SignTool /? or --HELP         - Show this help message");
        }
    }
}
