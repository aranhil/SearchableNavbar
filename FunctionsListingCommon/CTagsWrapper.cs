using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsListing
{
    class CTagsWrapper
    {
        // Import the necessary Windows API functions
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        // Define a delegate that matches the prototype of the function you want to call
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr MyFunctionPrototype(int size, IntPtr[] array);

        public static string Test(string path)
        {
            // Load the DLL
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dllPath = Path.Combine(assemblyDirectory, "ctags.dll");

            IntPtr pDll = LoadLibrary(dllPath);
            if (pDll == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load DLL");
                return "";
            }

            // Get a pointer to the function
            IntPtr pAddressOfFunctionToCall = GetProcAddress(pDll, "ctags_cli_lib");
            if (pAddressOfFunctionToCall == IntPtr.Zero)
            {
                Console.WriteLine("Failed to find function");
                return "";
            }

            // Cast the function pointer to the delegate type
            MyFunctionPrototype myFunction = (MyFunctionPrototype)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(MyFunctionPrototype));

            // Create an array of command-line arguments
            string[] args = new string[] { "ctags", "-x", "--fields=S", "--language-force=c++", "--c++-kinds=fp", "--_xformat=%N\t%n\t%S\t%Z", path };

            string output = "";

            try
            {
                // Marshal the string arguments
                IntPtr[] argvPointers = new IntPtr[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    argvPointers[i] = Marshal.StringToHGlobalAnsi(args[i]);
                }

                // Now you can call the function
                IntPtr outputPtr = myFunction(argvPointers.Length, argvPointers);
                output = Marshal.PtrToStringAnsi(outputPtr);
            }
            catch { }

            // Unload the DLL
            bool resultFree = FreeLibrary(pDll);
            if (!resultFree)
            {
                Console.WriteLine("Failed to free DLL");
            }

            return output;
        }

        //[DllImport("ctags-x86.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void ctags_cli_lib_free(IntPtr str);

        //[DllImport("ctags-x86.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern IntPtr ctags_cli_lib(int size, IntPtr[] array);

        //public static string ParseFile(string path)
        //{
        //    // Create an array of command-line arguments
        //    string[] args = new string[] { "ctags", "-x", "--fields=S", "--language-force=c++", "--c++-kinds=fp", "--_xformat=%N\t%n\t%S\t%Z", path };

        //    string output = "";

        //    // Marshal the string arguments
        //    IntPtr[] argvPointers = new IntPtr[args.Length];
        //    try
        //    {
        //        for (int i = 0; i < args.Length; i++)
        //        {
        //            argvPointers[i] = Marshal.StringToHGlobalAnsi(args[i]);
        //        }

        //        IntPtr outputPtr = ctags_cli_lib(argvPointers.Length, argvPointers);
        //        output = Marshal.PtrToStringAnsi(outputPtr);
        //        ctags_cli_lib_free(outputPtr);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        // Release the allocated memory
        //        for (int i = 0; i < args.Length; i++)
        //        {
        //            Marshal.FreeHGlobal(argvPointers[i]);
        //        }
        //    }

        //    return output;
        //}
    }
}
