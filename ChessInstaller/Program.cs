using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessInstaller
{
    static class Program
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AllocConsole();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            log(string.Join(" ", args));
            if (!args.Contains("--run"))
            {
                var temp = System.IO.Path.GetTempPath();
                var path = System.IO.Path.Combine(temp, "ChessInstaller.exe");
                var from = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChessInstaller.exe");
                log($"Moving from {from} to {path}");
                System.IO.File.Copy(from, path, true);
                log("Finished moving; executing...");
                var a = args.ToList();
                a.Add("--run");
                Process.Start(path, string.Join(" ", a.ToArray()));
                log("Started. Closing.");
                return;
            } else
            {
                Thread.Sleep(500);
                log("Continuing from other program...");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var thing = new UpdateForm();
            Application.Run(thing);
            if(thing.Existing == false)
                Application.Run(new MainForm());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
            log(e.ExceptionObject.ToString());
        }

        static void log(string t)
        {
            Console.WriteLine(t);
            File.AppendAllText("log.txt", "pg: " + t + "\r\n");
        }
    }
}
