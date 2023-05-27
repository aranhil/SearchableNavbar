using System;
using System.Collections.Generic;
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
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr MyFunctionPrototype(int size, IntPtr[] array);

        public static string Parse(string path)
        {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dllPath = Path.Combine(assemblyDirectory, "ctags.dll");

            IntPtr pDll = LoadLibrary(dllPath);
            if (pDll == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load DLL");
                return "";
            }

            IntPtr pAddressOfFunctionToCall = GetProcAddress(pDll, "ctags_cli_lib");
            if (pAddressOfFunctionToCall == IntPtr.Zero)
            {
                Console.WriteLine("Failed to find function");
                return "";
            }

            MyFunctionPrototype myFunction = (MyFunctionPrototype)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(MyFunctionPrototype));

            string[] args = new string[] { 
                "ctags", 
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
                "--kinds-Rust=fp",
                "--kinds-TypeScript=fm",
                "--extras=+q",
                "--_xformat=%N\t%n", 
                path 
            };

            string output = "";

            try
            {
                IntPtr[] argvPointers = new IntPtr[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    argvPointers[i] = Marshal.StringToHGlobalAnsi(args[i]);
                }

                IntPtr outputPtr = myFunction(argvPointers.Length, argvPointers);
                output = Marshal.PtrToStringAnsi(outputPtr);
            }
            catch { }

            bool resultFree = FreeLibrary(pDll);
            if (!resultFree)
            {
                Console.WriteLine("Failed to free DLL");
            }

            return output;
        }
    }
}
