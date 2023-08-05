using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Wrapper
{
    public partial class MainForm : Form
    {
        // The full path to the program to call
        private readonly string wrapped;
        // The process instance for the wrapped program
        private Process process;

        public MainForm(string wrapped)
        {
            InitializeComponent();
            this.wrapped = wrapped;
            this.Text = wrapped;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Create the process instance
            process = new Process();

            // Run program in cmd shell to redirect stderr to stdout
            process.StartInfo.FileName = "cmd";
            string[] args = { "/c", wrapped };
            // Add any parameters we've been given
            args = args.Concat(Environment.GetCommandLineArgs().Skip(1)).ToArray();
            // Add redirect
            args = args.Append("2>&1").ToArray();
            process.StartInfo.Arguments = string.Join(" ", args);

            // Redirect the standard output and error streams
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.EnableRaisingEvents = true;

            // Set up event handler for capturing output
            process.OutputDataReceived += Process_OutputDataReceived;

            // Start the process
            process.Start();

            // Start asynchronous reading of the output stream
            process.BeginOutputReadLine();

            // Ask to be notified when wrapped program exits
            process.Exited += Process_Exited;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            UpdateOutputTextBox($"[Proces beëindigd met code {process.ExitCode}.]");
            this.BeginInvoke((MethodInvoker) delegate() { this.Text += " [Gereed]"; });
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Handle output data received from the console application
            if (!string.IsNullOrEmpty(e.Data))
            {
                // Update the GUI with the output
                UpdateOutputTextBox(e.Data);
            }
        }

        private void UpdateOutputTextBox(string text)
        {
            // Ensure thread-safe updating of the TextBox control
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke(new Action<string>(UpdateOutputTextBox), text);
            }
            else
            {
                txtOutput.AppendText(text + Environment.NewLine);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Clean up resources when the form is closing
            process.OutputDataReceived -= Process_OutputDataReceived;
            process.Exited -= Process_Exited;
            process.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
        }
    }
}
