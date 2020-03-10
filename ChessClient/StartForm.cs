using ChessClient.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        public AdminForm AdminForm;
        private GlobalKeyboardHook _globalKeyboardHook;
        public StartForm()
        {
            InitializeComponent();
            _globalKeyboardHook = new GlobalKeyboardHook(new Keys[] { Keys.P });
            _globalKeyboardHook.KeyboardPressed += _globalKeyboardHook_KeyboardPressed;
        }

        private void _globalKeyboardHook_KeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if(e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp) 
            { 
                if(ModifierKeys.HasFlag(Keys.Control))
                {
                    Program.SetVisibilityAll(!this.Visible);
                }
            }
        }

        public static StartForm INSTANCE;

        static Dictionary<int, ChessPlayer> Players = new Dictionary<int, ChessPlayer>();

        static AutoResetEvent getPlayerEvent = new AutoResetEvent(false);
        public static ChessPlayer GetPlayer(int id)
        {
            if (Players.TryGetValue(id, out var p))
                return p;
            return null;
            // since the below doesnt always like to work
            /*var jobj = new JObject();
            jobj["id"] = id;
            var ping = new Packet(PacketId.IdentRequest, jobj);
            Send(ping);
            if (!getPlayerEvent.WaitOne(1000 * 30))
                return null; // since we timed out, it didnt work
            return GetPlayer(id); // buuuut, since it *did* work, we can recurse.*/
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

        void displayLoadError(Exception ex)
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
            catch (Exception ex)
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
#if DEBUG
            Client = new WebSocketSharp.WebSocket($"ws://localhost:4650/chess");
#else
            Client = new WebSocketSharp.WebSocket($"ws://ml-api.uk.ms:4650/chess");
#endif
            Client.Log.Output = (x, y) =>
            {
                Console.WriteLine($"{x.Date} {x.Message} {y}");
            };
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

        #region Integrity Checks

        void checkDuplicateProcesses()
        { // we only want to run this once - on start.
            var cur = Process.GetCurrentProcess();
            var others = Process.GetProcessesByName(cur.ProcessName);
            others = others.Where(x => x.Id != cur.Id).ToArray();
            IllegalStateException inner = null;
#if DEBUG
            inner = new IllegalStateException($"Processes at: " + string.Join(", ", others.Select(x => x.Id.ToString())));
#endif
            if (others.Count() > 0)
                throw new IllegalStateException("Client is already running", inner);
        }

        void checkBannedProcesses()
        {
            var cur = Process.GetCurrentProcess();
            var p = Process.GetProcesses(cur.MachineName);
            foreach(var process in p)
            {
                try
                {
                    if (Program.BannedProcesses.Contains(process.ProcessName))
                        throw new IllegalStateException("Others programs must be closed before client may be ran.",
                            #if DEBUG
                            new IllegalStateException($"{process.ProcessName} @ {process.Id}")
#else
                            null
#endif
                            );
                } catch (IllegalStateException) 
                { 
                    throw; 
                } catch { }
            }
        }

        void runIntegrityChecks()
        {
            checkBannedProcesses();
        }
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            INSTANCE = this;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                checkDuplicateProcesses();
                runIntegrityChecks();
            } catch (Exception ex)
            {
                displayLoadError(ex);
                // debug will mark error, but continue
#if !DEBUG
                return;
#endif
            }
#if !DEBUG
            btnSend.Enabled = false;
            txtInput.Enabled = false;
#endif
            var args = Environment.GetCommandLineArgs();
#if DEBUG
            MessageBox.Show(string.Join("\r\n", args), "Cli");
#endif
            string cli = "";
            if(args.Length == 1)
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
#if DEBUG
                        System.Diagnostics.Process.Start("http://localhost:8887/chess/online");
#else
                        System.Diagnostics.Process.Start("https://ml-api.uk.ms/chess/online");
#endif
                    }
                    catch { }
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
            this.Invoke(new Action(() =>
            {
                GameForm?.Close();
                GameForm = null;
            }));
        }

        private void Client_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            appendChat($"Closed: {e.Code} -- {e.Reason}\r\n", FontStyle.Bold, Color.DarkOrange);
            this.Invoke(new Action(() =>
            {
                GameForm?.Close();
                GameForm = null;
            }));
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception)e.ExceptionObject).ToString());
        }


        private void Client_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if(GameForm != null)
                GameForm.handlingMove = false;
            var jobj = JObject.Parse(e.Data);
            var packet = new Packet(jobj["id"].ToObject<PacketId>(), JObject.Parse(jobj["content"].ToString()));
            this.BeginInvoke(new Action(() =>
            {
                appendText("<< ", FontStyle.Regular, Color.Purple);
                appendText(e.Data);
            }));
            var th = new Thread(() =>
            {
                handleMessage(packet);
            });
            th.Start();
        }

        void handleMessage(Packet ping)
        {
            this.Game = this.Game ?? new OnlineGame();
            if (ping.Id == PacketId.MoveMade)
            {
                GameForm.AuthorativeMove(ping);
            }
            else if (ping.Id == PacketId.PlayerIdent)
            {
                var player = new ChessPlayer();
                var id = ping.Content["id"].ToObject<int>();
                var content = ping.Content["player"].ToString();
                if (content != "null")
                {
                    player.FromJson(JObject.Parse(content));
                    Players[id] = player;
                }
                else
                {
                    Players[id] = null;
                }
                getPlayerEvent.Set();
            }
            else if (ping.Id == PacketId.ConnectionMade)
            {
                var player = new ChessPlayer();
                player.FromJson(JObject.Parse(ping.Content["player"].ToString()));
                if (player.Id == Self.Id)
                    Self = player;
                else
                    Players[player.Id] = player;
                string msg = "";
                if (player.Side == PlayerSide.None)
                {
                    msg = "spectating";
                }
                else
                {
                    msg = player.Side.ToString();
                    if (player.Side == PlayerSide.White)
                    {
                        this.Game.White = player;
                    }
                    else
                    {
                        this.Game.Black = player;
                    }
                }
                appendChat($"{player.Name} has joined! They are {msg}!");
            } else if (ping.Id == PacketId.NotifyAdmin)
            {
                this.Invoke(new Action(() =>
                {
                    AdminForm = new AdminForm(this);
                    AdminForm.Show();
                    AdminForm.UpdateUI();
                }));
            } else if (ping.Id == PacketId.DemandScreen)
            {
                try
                {
                    performScreenShot();
                }
                catch (Exception ex)
                {
                    var jobj = new JObject();
                    jobj["level"] = "Warning";
                    jobj["about"] = "DemandScreen";
                    var jError = new JObject();
                    jError["message"] = ex.Message;
                    jError["stack"] = ex.StackTrace;
                    jError["source"] = ex.Source;
                    jobj["error"] = jError;
                    Send(new Packet(PacketId.Errored, jobj));
                }
            } else if (ping.Id == PacketId.DemandProcesses)
            {
                try
                {
                    performProcesses();
                }
                catch (Exception ex)
                {
                    var jobj = new JObject();
                    jobj["level"] = "Warning";
                    jobj["about"] = "DemandProcesses";
                    var jError = new JObject();
                    jError["message"] = ex.Message;
                    jError["stack"] = ex.StackTrace;
                    jError["source"] = ex.Source;
                    jobj["error"] = jError;
                    Send(new Packet(PacketId.Errored, jobj));
                }
            } else if (ping.Id == PacketId.UserDisconnected)
            {
                var player = GetPlayer(ping.Content["id"].ToObject<int>());
                if((player?.Side ?? PlayerSide.None) != PlayerSide.None)
                {
                    this.Invoke(new Action(() =>
                    {
                        GameForm.Hide();
                    }));
                }
            } else if (ping.Id == PacketId.GameEnd)
            {
                Game.Waiting = PlayerSide.None;
                Game.Ended = true;
                this.Invoke(new Action(() =>
                {
                    GameForm.UpdateUI();
                }));
                if (ping.Content == null)
                    return;
                var player = GetPlayer(ping.Content["id"].ToObject<int>());
                if(player == null)
                {
                    MessageBox.Show("Game is drawn!");
                } else
                {
                    MessageBox.Show($"{player.Name} has won!");
                }
                this?.Close();
                return;
            }
            if (Game.Ended)
                return;
            this?.BeginInvoke(new Action(() =>
            {
                if (GameForm == null)
                {
                    GameForm = new GameForm(this);
                    GameForm.Show();
                }
                GameForm?.UpdateUI();
                if (ping.Id == PacketId.GameStatus)
                {
                    Game.FromJson(ping.Content);
                    GameForm.Board.Evaluate();
                    GameForm.UpdateUI();
                }
                if (GameForm.Visible == false)
                    GameForm.Show();
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.Send(txtInput.Text);
            appendChat($">> " + txtInput.Text);
        }

        void performProcesses()
        {
            try
            {
                var processes = Process.GetProcesses();
                string TEXT = "";
                foreach(var proc in processes)
                {
                    try
                    {
                        TEXT += $"{proc.Id}: {proc.ProcessName}\r\n";
                    } catch
                    {
                        TEXT += $"-1: error\r\n";
                    }
                }
                var fName = Path.Combine(Path.GetTempPath(), "_chess.txt");
                File.WriteAllText(fName, TEXT);
                API.UploadProcesses(Encoding.UTF8.GetBytes(TEXT));
            } catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.ToString(), "Processes");
#endif
            }
        }

        const int ENUM_CURRENT_SETTINGS = -1;
        void performScreenShot()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                DEVMODE dm = new DEVMODE();
                dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
                EnumDisplaySettings(screen.DeviceName, ENUM_CURRENT_SETTINGS, ref dm);

                using (Bitmap bmp = new Bitmap(dm.dmPelsWidth, dm.dmPelsHeight))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(dm.dmPositionX, dm.dmPositionY, 0, 0, bmp.Size);
                    string name = screen.DeviceName.Split('\\').Last();
                    using (MemoryStream mStr = new MemoryStream())
                    {
                        bmp.Save(mStr, System.Drawing.Imaging.ImageFormat.Png);
                        API.UploadImage(mStr.ToArray(), name);
                        Thread.Sleep(1500);
                    }
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    }
}
