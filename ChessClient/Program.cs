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
        static StartForm form;

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

        public static void SetVisibilityAll(bool setting)
        {
            foreach(Form frm in new Form[] { form, form.GameForm, form.AdminForm})
            {
                if (frm == null)
                    continue;
                frm.Visible = setting;
            }
        }

        public static object GetResource(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name);
        }

        static bool closing = false;
        public static void CloseAll()
        {
            if (closing)
                return;
            closing = true;
            foreach(Form frm in new Form[] { form?.GameForm, form?.AdminForm, form})
            { // form last to close.
                frm?.Close();
            }
            Environment.Exit(0);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new StartForm();
            Application.Run(form);
        }
    }
}
