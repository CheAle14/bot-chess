using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient
{
    internal static class Program
    {
#if DEBUG
        public static bool DEBUG = true;
#else
        public static bool DEBUG = false;
#endif

        public static string[] BannedProcesses = new string[]
        { // TODO: find things to ban.
            "cheatengine"
        };
        // These phrases cannot appear in a chrome tab.
        public static string[] BannedPhrases = new string[]
        {
            "calculator",
            "chess",
            "next move"
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartForm());
        }
    }
}
