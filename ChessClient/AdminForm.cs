using ChessClient.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient
{
    public partial class AdminForm : Form
    {
        public StartForm Main;
        public AdminForm(StartForm m)
        {
            Main = m;
            InitializeComponent();
        }

        public void UpdateUI()
        {
            lblWhite.Text = Main.Game.White?.Name ?? "Waiting for White";
            lblBlack.Text = Main.Game.Black?.Name ?? "Waiting for Black";
            var wait = Main.Game?.Waiting ?? PlayerSide.None;
            lblWhite.ForeColor = wait == PlayerSide.White ? Color.Red : Color.FromKnownColor(KnownColor.ControlText);
            lblBlack.ForeColor = wait == PlayerSide.Black ? Color.Red : Color.FromKnownColor(KnownColor.ControlText);
        }

        void demandScreen(ChessPlayer player)
        {
            if (player == null)
                return;
            var jobj = new JObject();
            jobj["id"] = player.Id;
            StartForm.Send(new Packet(PacketId.RequestScreen, jobj));
        }

        private void btnScreenshot_Click(object sender, EventArgs e)
        {
            btnScreenshotW.Enabled = false;
            btnScreenB.Enabled = false;
            demandScreen(Main.Game.White);
            resetTimer.Start();
        }

        private void resetTimer_Tick(object sender, EventArgs e)
        {
            btnScreenB.Enabled = true;
            btnScreenshotW.Enabled = true;
            resetTimer.Stop();
        }

        private void btnScreenB_Click(object sender, EventArgs e)
        {
            btnScreenshotW.Enabled = false;
            btnScreenB.Enabled = false;
            demandScreen(Main.Game.Black);
            resetTimer.Start();
        }
    }
}
