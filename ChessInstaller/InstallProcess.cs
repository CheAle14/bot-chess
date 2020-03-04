using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessInstaller
{
    public class InstallProcess
    {
        Action<string> setUpdate;
        Action<int, string> setPercentage;
        string installLocation;
        WebClient Downloader;
        string url = "https://github.com/CheAle14/bot-chess/releases/latest/download/Chess.zip";
        string downloadPath = "";
        long lastKnown;

        public event EventHandler Complete;

        public InstallProcess(string location, Action<string> update, Action<int, string> perc)
        {
            setUpdate = update;
            setPercentage = perc;
            installLocation = location;
        }

        public void install()
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
            if (e.Cancelled || e.Error != null)
            {
                string err = null;
                err = e.Error?.InnerException?.Message ?? err;
                err = e.Error?.Message ?? "Cancelled";
                setUpdate("Failed: " + err);
                Complete?.Invoke(this, null);
            }
            else
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


        public void extractFiles()
        {
            setUpdate("Extracting files...");
            ZipFile.ExtractToDirectory(downloadPath, installLocation);
            setUpdate("Extracted");
            setUpdate("Copying installer...");
            var to = Path.Combine(installLocation, "ChessInstaller.exe");
            File.Copy(Environment.GetCommandLineArgs()[0], to, true);
            setUpdate("Copied");
            continueRegistry();
        }

        public string getLatestVersion()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "chessInstallCheAle14");
                var r = client.GetAsync("https://api.github.com/repos/CheAle14/bot-chess/releases/latest").Result;
                if (r.IsSuccessStatusCode)
                {
                    var str = r.Content.ReadAsStringAsync().Result;
                    foreach(var line in str.Split(','))
                    { // "tag_name":"v1.6"
                        if(line.StartsWith("\"tag_name\""))
                        {
                            var version = line.Substring("'tag_name':".Length);
                            return version.Replace("\"", "");
                        }
                    }
                    return "v0.1";
                }
                else
                {
                    return "v0.0";
                }
            }
        }

        public void continueRegistry()
        {
            var path = Path.Combine(installLocation, "ChessInstaller.exe");
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
            chess.SetValue("Version", getLatestVersion());
            setUpdate("Complete!");
            if (MessageBox.Show("In order to use this client, you must use the hyperlinks at the Chess Online webpage to open it.\r\n" +
                "Click OK to navigate to that webpage.", "Installation Complete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                System.Diagnostics.Process.Start("https://ml-api.uk.ms/chess/online");
            }
            Complete?.Invoke(this, null);
        }

        public void unInstall()
        {
            try
            {
                Directory.Delete(installLocation, true);
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
            Complete?.Invoke(this, null);
        }

        public static bool isValidLocation(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            if (Directory.Exists(path))
            {
                var cont = Directory.GetFiles(path);
                if (cont.Length > 0)
                    return false;
                var fold = Directory.GetDirectories(path);
                if (fold.Length > 0)
                    return false;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                    return false;
                }
            }
            return true;
        }
    }
}
