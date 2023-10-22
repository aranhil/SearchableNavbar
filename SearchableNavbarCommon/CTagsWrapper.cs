using Microsoft.VisualStudio.Shell;
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
        public static string Parse(string path, SearchableNavbarPackage package)
        {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string exePath = Path.Combine(assemblyDirectory, "ctags.exe");

            string[] args = new string[] {
                "-x",
                "--fields=S",
                GetCppKindsFromPackage(package),
                GetCKindsFromPackage(package),
                "--kinds-C#=m",
                "--kinds-Java=m",
                "--kinds-JavaScript=mf",
                "--kinds-Pascal=fp",
                "--kinds-PHP=f",
                "--kinds-Python=f",
                "--kinds-Ruby=f",
                "--kinds-Rust=fP",
                "--kinds-TypeScript=fm",
                GetExtraOptionsFromPackage(package),
                "--_xformat=\"%N\t%n\t%S\t%k\t%l\"",
                GetSortOptionFromPackage(package),
                GetIgnoredCppMacrosFromPackage(package),
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

        private static string GetCKindsFromPackage(SearchableNavbarPackage package)
        {
            if (package == null)
            {
                return "--kinds-C=fp";
            }

            string returnString = "--kinds-C=";
            if (package.CShowMacroDefinitions) returnString += "d";
            if (package.CShowEnumerators) returnString += "e";
            if (package.CShowFunctionDefinitions) returnString += "f";
            if (package.CShowEnumerationNames) returnString += "g";
            if (package.CShowLocalVariables) returnString += "l";
            if (package.CShowFunctionPrototypes) returnString += "p";
            if (package.CShowStructureNames) returnString += "s";
            if (package.CShowTypedefs) returnString += "t";
            if (package.CShowUnionNames) returnString += "u";
            if (package.CShowVariableDefinitions) returnString += "v";
            if (package.CShowFunctionParameters) returnString += "z";
            if (package.CShowGotoLabels) returnString += "L";
            if (package.CShowMacroParameters) returnString += "D";
            return returnString;
        }

        private static string GetIgnoredCppMacrosFromPackage(SearchableNavbarPackage package)
        {
            if(package == null || package.IgnoredCppMacros.Length == 0)
            {
                return "";
            }

            return "-I " + package.IgnoredCppMacros;
        }

        private static string GetExtraOptionsFromPackage(SearchableNavbarPackage package)
        {
            if(package == null)
            {
                return "--extras=+q";
            }

            string returnString = "--extras=";
            if(package.ShowFullyQualifiedTags) returnString += "+q";
            if(!package.ShowAnonymousTags) returnString += "-{anonymous}";
            return returnString;
        }

        private static string GetSortOptionFromPackage(SearchableNavbarPackage package)
        {
            if(package == null)
            {
                return "--sort=yes";
            }

            return "--sort=" + (package.SortAlphabetically ? "yes" : "no");
        }

        private static string GetCppKindsFromPackage(SearchableNavbarPackage package)
        {
            if(package == null)
            {
                return "--kinds-C++=fp";
            }

            string returnString = "--kinds-C++=";
            if (package.CppShowMacroDefinitions) returnString += "d";
            if (package.CppShowEnumerators) returnString += "e";
            if (package.CppShowFunctionDefinitions) returnString += "f";
            if (package.CppShowEnumerationNames) returnString += "g";
            if (package.CppShowLocalVariables) returnString += "l";
            if (package.CppShowClassStructUnionMembers) returnString += "m";
            if (package.CppShowFunctionPrototypes) returnString += "p";
            if (package.CppShowStructureNames) returnString += "s";
            if (package.CppShowTypedefs) returnString += "t";
            if (package.CppShowUnionNames) returnString += "u";
            if (package.CppShowVariableDefinitions) returnString += "v";
            if (package.CppShowExternalAndForwardVariableDeclarations) returnString += "x";
            if (package.CppShowFunctionParameters) returnString += "z";
            if (package.CppShowGotoLabels) returnString += "L";
            if (package.CppShowMacroParameters) returnString += "D";
            if (package.CppShowClasses) returnString += "c";
            if (package.CppShowNamespaces) returnString += "n";
            if (package.CppShowUsingNamespaceStatements) returnString += "U";
            if (package.CppShowTemplateParameters) returnString += "Z";
            return returnString;
        }
    }
}
