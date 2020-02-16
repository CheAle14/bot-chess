using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessInstaller
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        bool isValidLocation(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            if(Directory.Exists(path))
            {
                var cont = Directory.GetFiles(path);
                if (cont.Length > 0)
                    return false;
                var fold = Directory.GetDirectories(path);
                if (fold.Length > 0)
                    return false;
            } else
            {
                try
                {
                    Directory.CreateDirectory(path);
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                    return false;
                }
            }
            return true;
        }

        void continueRegistry()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    continueRegistry();
                }));
                return;
            }
            setUpdate("Registering web protocol");
            var key = Registry.ClassesRoot.CreateSubKey("chess", true);
            key.SetValue("", "URL:chess Protocol");
            key.SetValue("URL Protocol", "");
            var keyShell = key.CreateSubKey("shell");
            var keyOpen = keyShell.CreateSubKey("open");
            var keyCom = keyOpen.CreateSubKey("command");
            keyCom.SetValue("", $"\"{localPath}\" \"%1\" \"%2\" \"%3\" \"%4\" \"%5\" \"%6\" \"%7\" \"%8\" \"%9\"");
            setUpdate("Creating empty registry");
            var empty = Registry.CurrentUser.CreateSubKey("CheAle14");
            var chess = empty.CreateSubKey("ChessClient");
            chess.SetValue("", localPath);
            chess.SetValue("InstalledOn", DateTime.Now.ToString());
            setUpdate("Complete!");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            lblFolderFeedback.Text = originalF;
            lblFolderFeedback.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            var browser = new FolderBrowserDialog();
            browser.Description = "Select install folder";
            browser.ShowNewFolderButton = true;
            browser.SelectedPath = @"C:\Program Files (x86)\";
            var r = browser.ShowDialog();
            if(r == DialogResult.OK)
            {
                if(isValidLocation(browser.SelectedPath))
                {
                    txtLocation.Text = browser.SelectedPath;
                } else
                {
                    lblFolderFeedback.Text = "Folder path is not valid (must be empty)";
                    lblFolderFeedback.ForeColor = Color.Red;
                }
            } else
            {
                txtLocation.Text = "";
            }
            btnInstall.Enabled = !string.IsNullOrWhiteSpace(txtLocation.Text);
        }

        string originalF;
        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if(args.Count() > 1)
            {
                MessageBox.Show(string.Join("\n", args), "Command Line");
            }
            var chess = Registry.CurrentUser.CreateSubKey("CheAle14").CreateSubKey("ChessClient");
            var installPath = (string)chess.GetValue("");
            if(!string.IsNullOrWhiteSpace(installPath))
            {
                localPath = installPath.Replace(Path.GetFileName(installPath), "");
                txtLocation.Text = localPath;
                btnBrowse.Enabled = false;
                btnInstall.Text = "Uninstall";
                btnInstall.Enabled = true;
                lblFolderFeedback.Text = "Location of previous install found";
            }

            originalF = lblFolderFeedback.Text;
            pbBar.Maximum = 100;
        }

        void setUpdate(string text)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    setUpdate(text);
                }));
                return;
            }
            lblUpdate.Text = text;
        }

        void setPercentage(int perc, string additional)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    setPercentage(perc, additional);
                }));
                return;
            }
            pbBar.Value = perc;
            lblBytes.Text = additional;
        }

        public WebClient Downloader;
        string url = "https://github.com/CheAle14/bot-chess/releases/download/v0.1/ChessInstaller.exe";
        string localPath = "";
        long lastKnown;

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if(btnInstall.Text.StartsWith("Un"))
            {
                btnInstall.Enabled = false;
                uninstall();
            } else
            {
                if (!isValidLocation(txtLocation.Text))
                    return;
                btnInstall.Enabled = false;
                install();
            }
        }

        void uninstall()
        {
            try
            {
                Directory.Delete(localPath, true);
            }
            catch { }
            Registry.ClassesRoot.DeleteSubKeyTree("chess");
            var che = Registry.CurrentUser.CreateSubKey("CheAle14");
            che.DeleteSubKey("ChessClient");
            setUpdate("Program uninstalled");
        }

        void install()
        {
            Downloader = new WebClient();
            Downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            Downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
            localPath = Path.Combine(txtLocation.Text, "ChessInstaller.exe");
            Downloader.DownloadFileAsync(new Uri(url),
                localPath);
        }

        private void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Cancelled || e.Error != null)
            {
                setUpdate("Failed: " + (e.Error?.Message ?? "Cancelled"));
            } else
            {
                setPercentage(100, formatBytes(lastKnown));
                setUpdate("Download complee");
                continueRegistry();
            }
        }

        string formatBytes(long bytes)
        {
            long gb = bytes / (1024 * 1024 * 1024);
            if (gb > 0)
                return $"{gb}GB";
            long mb = bytes / (1024 * 1024);
            if (mb > 0)
                return $"{mb}MB";
            long kb = bytes / (1024);
            if (kb > 0)
                return $"{kb}KB";
            return $"{bytes}B";
        }

        private void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lastKnown = e.BytesReceived;
            setUpdate($"Downloading {url}");
            setPercentage(e.ProgressPercentage, formatBytes(e.BytesReceived));
        }
    }
}
