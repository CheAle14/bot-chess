using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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

        void extractFiles()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    extractFiles();
                }));
                return;
            }
            setUpdate("Extracting files...");
            ZipFile.ExtractToDirectory(downloadPath, txtLocation.Text);
            setUpdate("Extracted");
            continueRegistry();
        }

        void continueRegistry()
        {
            var path = Path.Combine(txtLocation.Text, "ChessClient.exe");
            setUpdate("Registering web protocol");
            var main = Registry.CurrentUser.CreateSubKey("Software");
            var cls = main.CreateSubKey("Classes");
            var key = cls.CreateSubKey("chess", true);
            key.SetValue("", "URL:chess Protocol");
            key.SetValue("URL Protocol", "");
            var keyShell = key.CreateSubKey("shell");
            var keyOpen = keyShell.CreateSubKey("open");
            var keyCom = keyOpen.CreateSubKey("command");
            keyCom.SetValue("", $"\"{path}\" \"%1\" \"%2\" \"%3\" \"%4\" \"%5\" \"%6\" \"%7\" \"%8\" \"%9\"");
            setUpdate("Creating empty registry");
            var empty = Registry.CurrentUser.CreateSubKey("CheAle14");
            var chess = empty.CreateSubKey("ChessClient");
            chess.SetValue("", path);
            chess.SetValue("InstalledOn", DateTime.Now.ToString());
            setUpdate("Complete!");
            if(MessageBox.Show("In order to use this client, you must use the hyperlinks at the Chess Online webpage to open it.\r\n" +
                "Click OK to navigate to that webpage.", "Installation Complete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                System.Diagnostics.Process.Start("https://ml-api.uk.ms/chess/online");
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            lblFolderFeedback.Text = originalF;
            lblFolderFeedback.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            var browser = new FolderBrowserDialog();
            browser.Description = "Select install folder";
            browser.ShowNewFolderButton = true;
            //browser.SelectedPath = @"C:\Program Files (x86)\";
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
            cbTerms.Enabled = !string.IsNullOrWhiteSpace(txtLocation.Text);
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
                txtLocation.Text = installPath.Replace(Path.GetFileName(installPath), "");
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
        string url = "https://github.com/CheAle14/bot-chess/releases/latest/download/Chess.zip";
        string downloadPath = "";
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
                Directory.Delete(txtLocation.Text, true);
            }
            catch { }
            var k = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Classes", true);
            try
            {
                k.DeleteSubKeyTree("chess");
            }
            catch { }
            var che = Registry.CurrentUser.CreateSubKey("CheAle14");
            che.DeleteSubKey("ChessClient");
            setUpdate("Program uninstalled");
        }

        void install()
        {
            Downloader = new WebClient();
            Downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            Downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
            downloadPath = Path.Combine(Path.GetTempPath(), "Chess.zip");
            Downloader.DownloadFileAsync(new Uri(url),
                downloadPath);
        }

        private void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Cancelled || e.Error != null)
            {
                string err = null;
                err = e.Error?.InnerException?.Message ?? err;
                err = e.Error?.Message ?? "Cancelled";
                setUpdate("Failed: " + err);
            } else
            {
                setPercentage(100, formatBytes(lastKnown));
                setUpdate("Download complee");
                extractFiles();
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

        private void cbTerms_CheckedChanged(object sender, EventArgs e)
        {
            if(cbTerms.Checked && !isValidLocation(txtLocation.Text))
            {
                MessageBox.Show("Set install location first.");
                cbTerms.Checked = false;
                return;
            }
            if (cbTerms.Checked)
            {
                var result = MessageBox.Show("By agreeing to the Terms and Conditions, you agree to the use of this Chess Client.\r\n" +
                        "The Client uses a number of anti-cheating mechanisms, such as scanning active programs.\r\n" +
                        "These mechanisms may infringe on your privacy.\r\n" +
                        "If you have any concerns, close any private information before running the Client, or do not use it at all.\r\n" +
                        "Any collected information is viewable only by the Chief Justice unless released in accordance to the Terms.", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result != DialogResult.OK)
                    cbTerms.Checked = false;
            }
            btnInstall.Enabled = cbTerms.Checked;
        }
    }
}
