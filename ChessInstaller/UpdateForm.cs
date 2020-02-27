using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessInstaller
{
    public partial class UpdateForm : Form
    {
        public UpdateForm()
        {
            InitializeComponent();
        }
        public bool Existing;

        string installPath = "";
        RegistryKey reg;
        private void UpdateForm_Load(object sender, EventArgs e)
        {
            reg = Registry.CurrentUser.CreateSubKey("CheAle14").CreateSubKey("ChessClient");
            if(reg.ValueCount == 0)
            {
                Existing = false;
                this.Close();
                return; // causes proper install form to appear.
            }
            Existing = true;
            update("Checking for updates");
            var th = new Thread(threadDoStuff);
            th.Start();
        }

        void update(string text)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() => update(text)));
                return;
            }
            lblUpdate.Text = text;
        }

        void percentage(int val, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => percentage(val, text)));
                return;
            }
            label1.Text = text;
            progressBar1.Value = val;
        }

        void threadDoStuff()
        {
            var fullPath = (string)reg.GetValue("");
            installPath = fullPath.Replace("ChessInstaller.exe", "").Replace("ChessClient.exe", "");
            string version = (string)reg.GetValue("Version");
            var installer = new InstallProcess(installPath, update, percentage);
            var latest = installer.getLatestVersion();
            update($"Current: {version}\r\nLatest: {latest}");
            Thread.Sleep(1500);
            if(latest != version && latest != "v0.0")
            { // remove everything
                update("Updating...");
                Thread.Sleep(250);
                installer.unInstall();
                Thread.Sleep(250);
                if (!Directory.Exists(installPath))
                    Directory.CreateDirectory(installPath);
                Thread.Sleep(250);
                installer.install();
                installer.Complete += Installer_Complete;
            } else
            {
                update($"Running latest ({latest}) no update needed");
            }
        }

        private void Installer_Complete(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                this.Close();
            }));
            var args = Environment.GetCommandLineArgs();
            if(args.Length > 1) // if via Chrome, will have additional args.
                Process.Start(Path.Combine(installPath, "ChessClient.exe"), Environment.CommandLine);
        }
    }
}
