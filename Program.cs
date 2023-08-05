using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Wrapper
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Determine path+name of wrapped executable
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string exe = AppDomain.CurrentDomain.FriendlyName;
            exe = exe.Replace("_wrapper", "");
            exe = Path.Combine(dir, exe);

            // If the file doesn't exist we're done
            if (!File.Exists(exe))
            {
                MessageBox.Show($"Bestand {exe} niet gevonden", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Create the form, and pass it the path to the wrapped executable
            MainForm form = new MainForm(exe);

            Application.Run(form);
        }
    }
}
