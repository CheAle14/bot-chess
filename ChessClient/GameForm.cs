using ChessClient.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ChessClient
{
    public partial class GameForm : Form
    {
        public GameForm(StartForm main)
        {
            Main = main;
            InitializeComponent();
            UpdateUI();
        }

        public void UpdateUI()
        {
            lblWhite.Text = Main.Game.White?.Name ?? "Waiting for White";
            lblBlack.Text = Main.Game.Black?.Name ?? "Waiting for Black";
            if(Main.Self == null)
            {
                this.Text = "Your account is unknown";
            } else
            {
                this.Text = $"You are {(Main.Self.Side == PlayerSide.None ? "spectating" : Main.Self.Side.ToString())}";
            }
            var wait = Main.Game?.Waiting ?? PlayerSide.None;
            lblWhite.ForeColor = wait == PlayerSide.White ? Color.Red : Color.FromKnownColor(KnownColor.ControlText);
            lblBlack.ForeColor = wait == PlayerSide.Black ? Color.Red : Color.FromKnownColor(KnownColor.ControlText);
            Main.AdminForm?.UpdateUI();
        }

        public StartForm Main;
        public GameBoard Board;

        System.Windows.Forms.Timer chromeTimer;
        Panel blockPanel;
        Label blockLbl;

        void setBlock(string text)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    setBlock(text);
                }));
                return;
            }
            blockLbl.Text = $"Game has been halted.\r\n{text ?? ""}";
            hasTriggered = !string.IsNullOrWhiteSpace(text);
            blockPanel.Visible = hasTriggered; 
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            Board = new GameBoard(this);
            blockPanel = new Panel();
            blockPanel.Name = "block";
            blockPanel.Location = new Point(0, 0);
            blockPanel.Dock = DockStyle.Fill;
            Controls.Add(blockPanel);
            blockPanel.BringToFront();
            blockLbl = new Label();
            blockLbl.Name = "blockLbl";
            blockLbl.Text = "Game has been halted.\r\n";
            blockLbl.Location = new Point(0, 0);
            blockLbl.Dock = DockStyle.Fill;
            blockLbl.Font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
            blockLbl.ForeColor = Color.Red;
            blockLbl.TextAlign = ContentAlignment.MiddleCenter;
            blockPanel.Controls.Add(blockLbl);
            setBlock(null);
            chromeTimer = new System.Windows.Forms.Timer();
            chromeTimer.Interval = 5000;
            chromeTimer.Tick += ChromeTimer_Tick;
            chromeTimer.Start();

        }

        /// <summary>
        /// Determines, from the name of a tab, whether we should be concerned
        /// </summary>
        bool isTabCheating(string name)
        {
            var lower = name.ToLower();
            foreach(var phrase in Program.BannedPhrases)
            {
                if(lower.Contains(phrase))
                {
                    Console.WriteLine($"Tab '{name}' is prohibited!");
                    return true;
                }
            }
            return false;
        }

        string getReference(int number)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(number));
        }


        List<int> blockedProcesses = new List<int>();
        bool checkChrome(out string list, out List<int> processes)
        {
            list = "";
            processes = new List<int>();
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            if (procsChrome.Length <= 0)
            {
                Console.WriteLine("Chrome is not running");
            }
            else
            {
                bool issue = false;
                foreach (Process proc in procsChrome)
                {
                    if(blockedProcesses.Contains(proc.Id))
                    {
                        list += $">BLOCKED={proc.Id}<\r\n";
                        issue = true;
                    }
                    // the chrome process must have a window 
                    if (proc.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }
                    AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                    Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
                    var tabs = root.FindAll(TreeScope.Descendants, condition);
                    list += $">Process={proc.Id}<\r\n";
                    var thisIssue = false;
                    foreach (AutomationElement tab in tabs)
                    {
                        if (isTabCheating(tab.Current.Name))
                        {
                            issue = true;
                            thisIssue = true;
                            list += "BLOCKED: ";
                        }
                        list += $"{tab.Current.Name}\r\n";
                    }
                    if (thisIssue)
                    {
                        processes.Add(proc.Id);
                        blockedProcesses.Add(proc.Id);
                    }
                }
                return issue;
            }
            return false;
        }

        TimeQueue times = new TimeQueue(10);

        bool hasTriggered = false;
        void performAntiCheatChecks()
        {
            var watch = new Stopwatch();
            Console.WriteLine("Beginning checks...");
            watch.Start();
            try
            {
                if(checkChrome(out string ls, out var lsProcesses))
                {
                    if(!hasTriggered)
                        StartForm.API.UploadChromes(Encoding.UTF8.GetBytes(ls));
                    setBlock("Please close any internet browser sessions\r\n" +
#if DEBUG
                        "\r\n" +
                        string.Join("\r\n- ", lsProcesses.Select(x => getReference(x))) + "\r\n----\r\n" +
                        ls +
#endif
                        "\r\nGame will show once your browser is closed");
                    return;
                }
                // All is good, reset everything.
                setBlock(null);
                hasTriggered = false;
                blockedProcesses = new List<int>();
            } catch (System.Windows.Automation.ElementNotAvailableException)
            {
                Console.WriteLine("Element not available.");
            }
            catch
            {
                throw;
            } finally
            {
                watch.Stop();
                Console.WriteLine($"Checks took {watch.ElapsedMilliseconds}");
                times.Add(watch.ElapsedMilliseconds);
            }
        }

        private void ChromeTimer_Tick(object sender, EventArgs e)
        {
            var th = new Thread(performAntiCheatChecks);
            th.Start();
            if(times.Items % 2 == 0)
            {
                chromeTimer.Interval = Math.Max(1500, (int)(times.GetAverage() * 1.2));
                Console.WriteLine("Interval now: " + chromeTimer.Interval);
                if (times.Items > 30)
                    times.Items = 10; // prevent integer overflows. (technically.. shouldnt be massively likely)
            }
        }

        private void GameForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Program.DEBUG = !Program.DEBUG;
        }

        public ChessButton firstClick;
        public bool handlingMove = false;
        public void Btn_Click(object sender, EventArgs e)
        {
            if (sender is ChessButton btn)
            {
                if (Control.ModifierKeys.HasFlag(Keys.Shift))
                {
                    string move = "Can Move:";
                    foreach (var piece in btn.CanMoveHere)
                    {
                        move += $"\n{piece.Owner} - {piece.Type} @ {piece.Location.Name}";
                    }
                    MessageBox.Show(move);
                    return;
                }
                if(Main.Game.Waiting != Main.Self.Side)
                    return;
                if (handlingMove)
                    return;
                if (firstClick == null)
                {
                    if (btn.PieceHere == null)
                        return;
                    if (btn.PieceHere?.Owner != Main.Self.Side)
                        return; // cant move opponent's pieces
                    firstClick = btn;
                    firstClick.Evaluate(true);
                }
                else
                {
                    if (firstClick == btn)
                    {
                        firstClick.Evaluate(false);
                    }
                    else if (btn.BackColor == Color.FromKnownColor(KnownColor.Control) || btn.BackColor == Color.Gray || btn.BackColor == Color.Purple)
                    {
                        firstClick.Evaluate(false);
                    }
                    else
                    {
                        if(willKingStillCheck(firstClick, btn))
                        {
                            MessageBox.Show("Your King is in Check!\nYou must defend your king.");
                            return;
                        }
                        handlingMove = true;
                        var jobj = new JObject();
                        jobj["from"] = firstClick.Name;
                        jobj["to"] = btn.Name;
                        StartForm.Send(new Packet(PacketId.MoveRequest, jobj));
                    }
                    firstClick = null;
                }
            }
        }

        bool willKingStillCheck(ChessButton from, ChessButton to)
        {
            if (Board.CheckingKing.Count == 0)
                return false;
            Dictionary<int, ChessButton> original = new Dictionary<int, ChessButton>();
            original[from.PieceHere.Id] = from;
            if (to.PieceHere != null)
            {
                original[to.PieceHere.Id] = to;
                to.PieceHere.Location = null;
            }
            to.PieceHere = from.PieceHere;
            from.PieceHere.Location = to;
            var ourKing = Board.Pieces[Main.Self.Side].FirstOrDefault(x => x.Type == PieceType.King);
            ourKing.Location.Reset();
            foreach(var t in Board.CheckingKing)
            {
                t?.Location?.Evaluate(false);
            }
            foreach (var t in original.Values)
                t.Evaluate(false);
            var can = ourKing.Location.CanMoveHere.Count > 0;
            from.PieceHere = null;
            to.PieceHere = null;
            foreach(var t in original)
            {
                var piece = Board.GetPiece(t.Key);
                t.Value.PieceHere = piece;
                piece.Location = t.Value;
            }
            Board.Evaluate();
            return can;
        }
    
        public void AuthorativeMove(Packet ping)
        {
            var from = ping.Content["from"].ToObject<string>();
            var fromBtn = Board.GetButtonAt(from);
            var to = ping.Content["to"].ToObject<string>();
            var toBtn = Board.GetButtonAt(to);
            if (toBtn.PieceHere != null)
                toBtn.PieceHere.Location = null;
            toBtn.PieceHere = fromBtn.PieceHere;
            if (fromBtn.PieceHere != null)
                fromBtn.PieceHere.Location = toBtn;
            fromBtn.PieceHere = null;
            toBtn.PieceHere.HasMoved = true;
            Main.Game.Waiting = (PlayerSide)((int)Main.Game.Waiting ^ 0b11);
            Board.Evaluate();
        }
    
    }
}
