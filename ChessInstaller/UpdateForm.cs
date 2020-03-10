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
        public bool isEnteringManualVersion = false;
        public ClientVersion manualVersion;

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
            installPath = fullPath.Replace("ChessInstaller.exe", "").Replace("ChessClient.exe", "").Replace("ChessInstall.exe", "");
            var version = new ClientVersion((string)reg.GetValue("Version"));
            var installer = new InstallProcess(installPath, update, percentage);
            var latest = new ClientVersion(installer.getLatestVersion());
            string delta = "";
            int compare = version.CompareTo(latest);
            if (compare == 0)
                delta = "Up to date";
            else if (compare < 0)
                delta = "Outdated";
            else
                delta = "Newer";
            update($"Current: {version}\r\nLatest: {latest}\r\n{delta}");
            Thread.Sleep(1500);
            if(compare < 0)
            { // remove everything
                update("Updating...");
                Thread.Sleep(250);
                installer.unInstall();
                Thread.Sleep(250);
                if (!Directory.Exists(installPath))
                    Directory.CreateDirectory(installPath);
                Thread.Sleep(250);
                installer.install(latest);
                installer.Complete += Installer_Complete;
            } else
            {
                update("Hacking NASA and the NSA...");
                Thread.Sleep(2500);
                while (isEnteringManualVersion && manualVersion == null)
                {
                    update("Waiting manual version...");
                    Thread.Sleep(500);
                }
                if(manualVersion != null)
                {
                    update("Getting manual version: " + manualVersion.ToString());
                    Thread.Sleep(250);
                    installer.unInstall();
                    Thread.Sleep(250);
                    if (!Directory.Exists(installPath))
                        Directory.CreateDirectory(installPath);
                    Thread.Sleep(250);
                    installer.install(manualVersion);
                    installer.Complete += Installer_Complete;
                } else
                {
                    update($"Running latest ({latest}) no update needed");
                    Installer_Complete(null, null);
                }
            }
        }

        private void Installer_Complete(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                this.Close();
            }));
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1) // if via Chrome, will have additional args.
            {
                // we want to start client, but also to be safe we'll write the commandline
                var txt = Path.Combine(installPath, "commandline.txt");
                try
                {
                    File.WriteAllText(txt, args[1]);
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Non-critical exception.");
                }
                Process.Start(Path.Combine(installPath, "ChessClient.exe"), args[1]);
            }
        }

        private void lblUpdate_Click(object sender, EventArgs e)
        {
            isEnteringManualVersion = true;
            var form = new VersionSelectForm();
            form.ShowDialog();
            if(!string.IsNullOrWhiteSpace(form.TagName))
            {
                manualVersion = new ClientVersion(form.TagName);
            }
        }
    }
}
