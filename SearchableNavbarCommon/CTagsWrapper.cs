using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SearchableNavbar
{
    class CTagsWrapper
    {
        public static string Parse(string path, string ignorableMacros)
        {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string exePath = Path.Combine(assemblyDirectory, "ctags.exe");

            string[] args = new string[] {
                "-x",
                "--fields=S",
                "--kinds-C++=fp",
                "--kinds-C#=m",
                "--kinds-Java=m",
                "--kinds-JavaScript=mf",
                "--kinds-Pascal=fp",
                "--kinds-PHP=f",
                "--kinds-Python=f",
                "--kinds-Ruby=f",
                "--kinds-Rust=fP",
                "--kinds-TypeScript=fm",
                "--extras=+q",
                "--_xformat=\"%N\t%n\t%S\"",
                ignorableMacros.Length > 0 ? "-I " + ignorableMacros : "",
                "\"" + path + "\""
            };

            string arguments = string.Join(" ", args);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;  // Redirect the output to retrieve it
            startInfo.FileName = exePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = arguments;

            string output = "";

            try
            {
                // Start the process with the info we specified.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    output = exeProcess.StandardOutput.ReadToEnd();  // Read the output from the process
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Handle any exceptions that occur when starting the process.
                Console.WriteLine("Failed to start process");
            }

            return output;
        }
    }
}
