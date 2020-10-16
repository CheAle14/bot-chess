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
        public InstallProcess INSTALL;

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
                if(InstallProcess.isValidLocation(browser.SelectedPath))
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
            loadOptions();
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
            File.AppendAllText("log.txt", "mf: " + text + "\r\n");
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

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if(btnInstall.Text.StartsWith("Un"))
            {
                btnInstall.Enabled = false;
                uninstall();
            } else
            {
                if (!InstallProcess.isValidLocation(txtLocation.Text))
                    return;
                btnInstall.Enabled = false;
                INSTALL = new InstallProcess(txtLocation.Text, setUpdate, (y, z) =>
                {
                    this.Invoke(new Action(() =>
                    {
                        lblBytes.Text = z;
                        pbBar.Value = y;
                    }));
                });
                var latest = INSTALL.getLatestVersion();
                INSTALL.install(new ClientVersion(latest));
            }
        }

        void uninstall()
        {
            INSTALL = new InstallProcess(txtLocation.Text, setUpdate, null);
            INSTALL.unInstall();
        }
        private void cbTerms_CheckedChanged(object sender, EventArgs e)
        {
            if (!promptWarnings)
                return;
            if (cbTerms.Checked && !InstallProcess.isValidLocation(txtLocation.Text))
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

        bool promptWarnings = false;
        void loadOptions()
        {
            var empty = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("CheAle14");
            var chess = empty.CreateSubKey("ChessClient");
            foreach (var control in Controls)
            {
                if(control is CheckBox cb)
                {
                    if(cb.Name.StartsWith("cb"))
                    {
                        var value = chess.GetValue(cb.Name.Substring("cb".Length), cb.Checked);
                        if(!(value is bool bVal))
                        {
                            bVal = bool.Parse((string)value);
                        }
                        cb.Checked = bVal;
                    }
                }
            }
            promptWarnings = true;
        }

        void setOption(string name, object value)
        {
            var empty = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("CheAle14");
            var chess = empty.CreateSubKey("ChessClient");
            chess.SetValue(name, value);
        }

        private void cbUseDiscord_CheckedChanged(object sender, EventArgs e)
        {
            if (!promptWarnings)
                return;
            setOption("UseDiscord", cbUseDiscord.Checked);
        }

        private void cbEnableAntiCheat_CheckedChanged(object sender, EventArgs e)
        {
            if (!promptWarnings)
                return;
            setOption("UseAntiCheat", cbUseAntiCheat.Checked);
            if(cbUseAntiCheat.Checked == false)
            {
                var r = MessageBox.Show("Disabling the anti-cheating mechanisms means any games you play in will not count towards or be added on the Leaderboard!", "Game Status: Invalid", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (r == DialogResult.Cancel)
                    cbUseAntiCheat.Checked = true;
            }
        }
    }
}
