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
                GetCSharpKindsFromPackage(package),
                "--kinds-Java=m",
                "--kinds-JavaScript=mf",
                "--kinds-Pascal=fp",
                "--kinds-PHP=f",
                "--kinds-Python=f",
                "--kinds-Ruby=f",
                "--kinds-Rust=fP",
                "--kinds-TypeScript=fm",
                GetLanguageMapFromPackage(package),
                GetExtraOptionsFromPackage(package),
                "--_xformat=\"%N\t%n\t%S\t%k\t%l\t%Z\"",
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

        private static string GetCSharpKindsFromPackage(SearchableNavbarPackage package)
        {
            if (package == null)
            {
                return "--kinds-C#=m";
            }

            string returnString = "--kinds-C#=";
            if (package.CSharpShowClasses) returnString += "c";
            if (package.CSharpShowMacroDefinitions) returnString += "d";
            if (package.CSharpShowEnumerators) returnString += "e";
            if (package.CSharpShowEvents) returnString += "E";
            if (package.CSharpShowFields) returnString += "f";
            if (package.CSharpShowEnumerationNames) returnString += "g";
            if (package.CSharpShowInterfaces) returnString += "i";
            if (package.CSharpShowLocalVariables) returnString += "l";
            if (package.CSharpShowMethods) returnString += "m";
            if (package.CSharpShowNamespaces) returnString += "n";
            if (package.CSharpShowProperties) returnString += "p";
            if (package.CSharpShowStructureNames) returnString += "s";
            if (package.CSharpShowTypedefs) returnString += "t";
            return returnString;
        }

        private static string GetCKindsFromPackage(SearchableNavbarPackage package)
        {
            if (package == null)
            {
                return "--kinds-C=fp";
            }

            string returnString = "--kinds-C=";
            if (package.CppShowMacroDefinitions) returnString += "d";
            if (package.CppShowEnumerators) returnString += "e";
            if (package.CppShowFunctionDefinitions) returnString += "f";
            if (package.CppShowEnumerationNames) returnString += "g";
            if (package.CppShowLocalVariables) returnString += "l";
            if (package.CppShowFunctionPrototypes) returnString += "p";
            if (package.CppShowStructureNames) returnString += "s";
            if (package.CppShowTypedefs) returnString += "t";
            if (package.CppShowUnionNames) returnString += "u";
            if (package.CppShowVariableDefinitions) returnString += "v";
            if (package.CppShowFunctionParameters) returnString += "z";
            if (package.CppShowGotoLabels) returnString += "L";
            if (package.CppShowMacroParameters) returnString += "D";
            return returnString;
        }

        private static string GetCppKindsFromPackage(SearchableNavbarPackage package)
        {
            if (package == null)
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

        private static string GetIgnoredCppMacrosFromPackage(SearchableNavbarPackage package)
        {
            if(package == null || package.CppIgnoredMacros.Length == 0)
            {
                return "";
            }

            return "-I " + package.CppIgnoredMacros;
        }

        private static string GetExtraOptionsFromPackage(SearchableNavbarPackage package)
        {
            if (package == null)
            {
                return "--extras=+q";
            }

            string returnString = "--extras=";
            if (package.ShowFullyQualifiedTags) returnString += "+q";
            if (!package.ShowAnonymousTags) returnString += "-{anonymous}";
            return returnString;
        }

        private static string GetLanguageMapFromPackage(SearchableNavbarPackage package)
        {
            if (package == null || package.LanguageMap.Length == 0)
            {
                return "";
            }

            return "--langmap=" + package.LanguageMap;
        }

        private static string GetSortOptionFromPackage(SearchableNavbarPackage package)
        {
            if(package == null)
            {
                return "--sort=yes";
            }

            return "--sort=" + (package.SortAlphabetically ? "yes" : "no");
        }
    }
}
