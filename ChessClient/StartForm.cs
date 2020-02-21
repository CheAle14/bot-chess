using ChessClient.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient
{
    public partial class StartForm : Form
    {
        public static MLAPI API;
        public GameForm GameForm;
        public StartForm()
        {
            InitializeComponent();
        }

        static Dictionary<int, ChessPlayer> Players = new Dictionary<int, ChessPlayer>();

        static AutoResetEvent getPlayerEvent = new AutoResetEvent(false);
        public static ChessPlayer GetPlayer(int id)
        {
            if (Players.TryGetValue(id, out var p))
                return p;
            var jobj = new JObject();
            jobj["id"] = id;
            var ping = new Packet(PacketId.IdentRequest, jobj);
            Send(ping);
            if (!getPlayerEvent.WaitOne(1000 * 10))
                return null; // since we timed out, it didnt work
            return GetPlayer(id); // buuuut, since it *did* work, we can recurse.
        }


        public static WebSocketSharp.WebSocket Client;
        public QueryParser Parser;
        public OnlineGame Game;
        public ChessPlayer Self;

        void appendChat(string message, FontStyle style = FontStyle.Regular, Color? clr = null) =>
            appendText(message + "\r\n", style, clr);

        void appendText(string message, FontStyle style = FontStyle.Regular, Color? clr = null)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    appendChat(message, style, clr);
                }));
                return;
            }
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionFont = new Font(rtbChat.Font, style);
            rtbChat.SelectionColor = clr.GetValueOrDefault(Color.FromKnownColor(KnownColor.ControlText));
            rtbChat.AppendText(message);
            rtbChat.SelectionColor = rtbChat.ForeColor;
        }

        void displayLoadError(APIException ex)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    displayLoadError(ex);
                }));
                return;
            }
            txtInput.Enabled = false;
            btnSend.Enabled = false;
            appendText("Error: ", FontStyle.Bold, Color.Red);
            appendText(ex.ToString() + "\r\n");
        }

        void initialConnect()
        {
            try
            {
                appendChat($"Fetching your identity...", clr: Color.Blue);
                Self = API.GetIdentity();
                Players[Self.Id] = Self;
                appendChat($"Found: {Self.Name}", clr: Color.Blue);
            }
            catch (APIException ex)
            {
                displayLoadError(ex);
                return;
            }
            if (Self == null)
            {
                MessageBox.Show("Failed to get your identity. Please try again");
                return;
            }
            StartMode mode = Parser.Mode;
            if (Parser.Mode == StartMode.Create)
            {
                try
                {
                    appendChat($"Creating new game...", clr: Color.Blue);
                    API.CreateNewGame();
                    appendChat($"Created.", clr: Color.Blue);
                    mode = StartMode.Join;
                }
                catch (APIException ex)
                {
                    displayLoadError(ex);
                    return;
                }
            }

            appendChat($"Joining game...", clr: Color.Blue);
            Client = new WebSocketSharp.WebSocket("ws://localhost:4649/chess");
            Client.OnMessage += Client_OnMessage;
            Client.OnClose += Client_OnClose;
            Client.OnError += Client_OnError;
            Client.Connect();
            var jobj = new JObject();
            jobj["token"] = Parser.Token;
            jobj["mode"] = mode == StartMode.Join ? "join" : "spectate";
            Send(new Packet(PacketId.ConnRequest, jobj));
        }

        public static void Send(Packet p)
        {
            Client.Send(p.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var args = Environment.GetCommandLineArgs();
            string cli = "";
            if(args.Length != 2)
            {
                try
                {
                    cli = File.ReadAllText("commandline.txt");
                } catch { }
                if(string.IsNullOrWhiteSpace(cli))
                {
                    MessageBox.Show("Program must be started from website.");
                    try
                    {
                        System.Diagnostics.Process.Start(MLAPI.URL + "chess/online"); 
                    } catch { }
                    this.Close();
                    return;
                }
            }
            else
            {
                cli = args[1];
            }
            Parser = new QueryParser(cli);
            API = new MLAPI(Parser.Token);
            appendChat($"Connecting to central server...", clr: Color.Blue);
            var th = new Thread(initialConnect);
            th.Start();
        }

        private void Client_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            appendChat($"Errored: {e.Message}; {e.Exception}", FontStyle.Regular, Color.Red);
        }

        private void Client_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            appendChat($"Closed: {e.Code} -- {e.Reason}\r\n", FontStyle.Bold, Color.DarkOrange);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception)e.ExceptionObject).ToString());
        }

        private void Client_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            var jobj = JObject.Parse(e.Data);
            var packet = new Packet(jobj["id"].ToObject<PacketId>(), JObject.Parse(jobj["content"].ToString()));
            this.Invoke(new Action(() =>
            {
                appendText("<< ", FontStyle.Regular, Color.Purple);
                appendText(e.Data);
                handleMessage(packet);
            }));
        }

        void handleMessage(Packet ping)
        {
            this.Game = new OnlineGame();
            if(ping.Id == PacketId.GameStatus)
            {
                Game.FromJson(ping.Content);
                GameForm?.UpdateUI();
            } else if (ping.Id == PacketId.MoveMade)
            {
                GameForm.AuthorativeMove(ping);
            } else if (ping.Id == PacketId.PlayerIdent)
            {
                var player = new ChessPlayer();
                var id = ping.Content["id"].ToObject<int>();
                var content = ping.Content["player"].ToString();
                if (content != "null")
                {
                    player.FromJson(JObject.Parse(content));
                    Players[id] = player;
                } else
                {
                    Players[id] = null;
                }
                getPlayerEvent.Set();
            } else if (ping.Id == PacketId.ConnectionMade)
            {
                var player = new ChessPlayer();
                player.FromJson(JObject.Parse(ping.Content["player"].ToString()));
                if (player.Id == Self.Id)
                    Self = player;
                else
                    Players[player.Id] = player;
                string msg = "";
                if(player.Side == PlayerSide.None)
                {
                    msg = "spectating";
                }
                else
                {
                    msg = player.Side.ToString();
                    if(player.Side == PlayerSide.White)
                    {
                        this.Game.White = player;
                    } else
                    {
                        this.Game.Black = player;
                    }
                }
                appendChat($"{player.Name} has joined! They are {msg}!");
            }
            if(GameForm == null)
            {
                GameForm = new GameForm(this);
                GameForm.Show();
            }
            GameForm.UpdateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.Send(txtInput.Text);
            appendChat($">> " + txtInput.Text);
        }
    }
}
