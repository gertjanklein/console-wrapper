using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Wrapper
{
    public partial class MainForm : Form
    {
        private Process process;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Create the process instance
            process = new Process();
            
            // Use Python program to check stdout/stderr
            process.StartInfo.FileName = "py";
            string[] args = { "../../hello.py" };
            args = args.Concat(Environment.GetCommandLineArgs().Skip(1)).ToArray();
            process.StartInfo.Arguments = string.Join(" ", args);

            // Redirect the standard output and error streams
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Set up event handlers for capturing output
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;

            // Start the process
            process.Start();

            // Start asynchronous reading of the output and error streams
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Handle output data received from the console application
            if (!string.IsNullOrEmpty(e.Data))
            {
                // Update the GUI with the output
                UpdateOutputTextBox("[O] " + e.Data);
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Handle error data received from the console application
            if (!string.IsNullOrEmpty(e.Data))
            {
                // Update the GUI with the error
                UpdateOutputTextBox("[E] " + e.Data);
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
            process.ErrorDataReceived -= Process_ErrorDataReceived;
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
