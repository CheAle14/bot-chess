using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessInstaller
{
    public partial class VersionSelectForm : Form
    {
        public VersionSelectForm()
        {
            InitializeComponent();
        }
        HttpClient client;
        public string TagName;

        private void VersionSelectForm_Load(object sender, EventArgs e)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "chessInstallCheAle14");
            var tth = new Thread(getVersions);
            tth.Start();
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        const string rgPattern = "\\\"name\\\":\\\"(v.+)\\\"";
        List<string> getVersionsList()
        {
            var ls = new List<string>();
            var r = client.GetAsync("https://api.github.com/repos/CheAle14/bot-chess/tags").Result;
            if (r.IsSuccessStatusCode)
            {
                var RGX = new Regex(rgPattern);
                var str = r.Content.ReadAsStringAsync().Result;
                foreach (var line in str.Split(','))
                { // "tag_name":"v1.6"
                    var match = RGX.Match(line);
                    if(match.Success)
                    {
                        ls.Add(match.Groups[1].Value);
                    }
                }
            } else
            {
                string resetIn = "unknown";
                if(r.Headers.TryGetValues("X-RateLimit-Reset", out var vals))
                {
                    var val = string.Join("", vals);
                    if(long.TryParse(val, out long stamp))
                    {
                        var dt = FromUnixTime(stamp);
                        var diff = dt - DateTime.Now;
                        resetIn = $"{diff.Minutes}m {diff.Seconds}s";
                    }

                }
                this.Invoke(new Action(() =>
                {
                    label1.Text = r.Content.ReadAsStringAsync().Result + "\r\nReset in: " + resetIn;
                }));
            }
            return ls;
        }

        void getVersions()
        {
            var tags = getVersionsList();
            this.Invoke(new Action(() =>
            {
                foreach (var tag in tags)
                {
                    var page = new TabPage(tag);
                    page.Name = tag;
                    tabControl1.TabPages.Add(page);
                }
                if(tags.Count > 0)
                    label1.Text = "Select a tab above to view version changes";
            }));
        }

        string getDisplay(string type)
        {
            string clr = "default";
            if (type.StartsWith("Add"))
                clr = "success";
            else if (type.StartsWith("Fix"))
                clr = "info";
            else if (type.StartsWith("Remove"))
                clr = "error";
            return $"<span class='label label-{clr}'>{type}</span>";
        }

        string parseDescription(string body)
        {
            body = body.Replace("\\r\\n", "\n");
            string display = "";
            string TEXT = "";
            foreach (var line in body.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (line.StartsWith("##"))
                {
                    display = getDisplay(line.Replace("##", "").Trim());
                } else
                {
                    TEXT += "<div class='change'>";
                    TEXT += display + " " + line.Replace("* ", "");
                    TEXT += "</div>";
                }
            }
            return TEXT;
        }

        const string bodyRegex = "\"body\":\"(.*)\"";
        const string titleRegex = "\"name\":\"(.+?(?=\\\"\\,))\",";
        string getTextOf(string version)
        {
            var r = client.GetAsync("https://api.github.com/repos/CheAle14/bot-chess/releases/tags/" + version).Result;
            if(r.IsSuccessStatusCode)
            {
                var content = r.Content.ReadAsStringAsync().Result;
                var bodyReg = new Regex(bodyRegex);
                var bodyM = bodyReg.Match(content);
                string bodyText = "## Failed\n* To get description of release.";
                if(bodyM.Success)
                {
                    bodyText = bodyM.Groups[1].Value;
                }
                var titleReg = new Regex(titleRegex);
                var titleM = titleReg.Match(content);
                string title = "Failed to get title";
                if(titleM.Success)
                {
                    title = titleM.Groups[1].Value;
                }
                return $"<h2>{title}</h2><div style='update'>{parseDescription(bodyText)}</div>";
            }
            string resetIn = "the end of the universe";
            if (r.Headers.TryGetValues("X-RateLimit-Reset", out var vals))
            {
                var val = string.Join("", vals);
                if (long.TryParse(val, out long stamp))
                {
                    var dt = FromUnixTime(stamp);
                    var diff = dt - DateTime.Now;
                    resetIn = $"{diff.Minutes}m {diff.Seconds}s";
                }

            }
            return "<p style='background-color: red; color:white'>An error occured meaning I can't get this version's details.</p>" +
                $"<p>{r.Content.ReadAsStringAsync().Result}</p><p>If rate limited, retry in {resetIn}</p>";
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = $"Sel: {tabControl1.SelectedTab.Name}";
            var tab = tabControl1.SelectedTab;
            if(tab.Controls.Count == 0)
            {
                var btn = new Button();
                btn.Name = "sel:" + tab.Name;
                btn.Dock = DockStyle.Bottom;
                btn.Size = new Size(tab.Width, 30);
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.Text = "Install Version";
                btn.Click += Btn_Click;
                btn.Tag = tab.Name;
                tab.Controls.Add(btn);
                var browser = new WebBrowser();
                browser.Location = new Point(0, 0);
                browser.Size = new Size(tab.Width, tab.Height - btn.Size.Height - 10);
                browser.Dock = DockStyle.Fill;
                tab.Controls.Add(browser);
                btn.BringToFront();

                browser.Navigate("about:blank");
                string style = "<style>";
                style += ".update .label {width: 40px; text-align: center;}";
                style += ".label {display: inline;padding: .2em .6em .3em;" +
                    "font-size: 75%;font-weight: 700;line-height: 1;color: #fff;" +
                    "text-align: center;white-space: nowrap;vertical-align: baseline;border-radius: .25em;}";
                style += ".label-info { background-color: #35c5f4;}";
                style += ".label-success { background-color: #5cb85c;}";
                style += ".label-error { background-color: #ff3232;}";
                style += ".update .change {margin-bottom: 2px;font-size: 13px;}";
                style += "body { font-family: \"open sans\",\"Helvetica Neue\",Helvetica,Arial,sans-serif; " +
                    "font-size: 14px; line-height: 1.42857143; color: #333; background-color: #fff; }";
                style += "html {font-size: 10px;}";
                style += "* {box-sizing: border-box;}";
                style += "fieldset { padding: 0;  margin: 0; border: 0; min-width: 0;}";
                style += "</style>";
                string full = "<html><head>" + style + "</head>" +
                    "<body><fieldset>" + getTextOf(tab.Name) + "</fieldset></body>";
                browser.Document.Write(full);
                System.IO.File.WriteAllText("output.html", full);
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            TagName = (string)((sender as Button).Tag);
            client.Dispose();
            this.Close();
        }
    }
}
