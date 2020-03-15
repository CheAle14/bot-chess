using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient
{
    internal static class Program
    {
#if DEBUG
        public static bool DEBUG = true;
#else
        public static bool DEBUG = false;
#endif
        #region OPTIONS

        public static class Options
        {
            public static bool UseDiscord = true;
            public static bool UseAntiCheat = true;
        }
        static void loadOptions()
        {
            var empty = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("CheAle14");
            var chess = empty.CreateSubKey("ChessClient");
            var fields = typeof(Options).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach(var option in fields)
            {
                var value = (string)chess.GetValue(option.Name, null);
                if (value != null)
                {
                    if(option.FieldType == typeof(string))
                    {
                        option.SetValue(null, value);
                    } else if (option.FieldType == typeof(bool))
                    {
                        option.SetValue(null, bool.Parse(value));
                    }
                }
            }
        }
        #endregion
        public static Discord.Discord DiscordClient = null;
        static StartForm form;

        public static string[] BannedProcesses = new string[]
        { // TODO: find things to ban.
            "cheatengine"
        };
        // These phrases cannot appear in a chrome tab.
        public static string[] BannedPhrases = new string[]
        {
            "calculator",
            "chess",
            "next move"
        };

        public static void SetVisibilityAll(bool setting)
        {
            foreach(Form frm in new Form[] { form, form.GameForm, form.AdminForm})
            {
                if (frm == null)
                    continue;
                frm.Visible = setting;
            }
        }

        public static object GetResource(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name);
        }

        static bool closing = false;
        public static void CloseAll()
        {
            if (closing)
                return;
            closing = true;
            foreach(Form frm in new Form[] { form?.GameForm, form?.AdminForm, form})
            { // form last to close.
                frm?.Close();
                frm?.Dispose();
            }
            form = null;
            try
            {
                dsThread.Abort();
            }
            catch { }
            if(Options.UseDiscord)
            {
                try
                {
                    DiscordClient.Dispose();
                } catch { }
            }
            Environment.Exit(0);
        }

#if DEBUG
        static int PROGRAM_COUNTER = 0;
#else
        static int PROGRAM_COUNTER = 0;
# endif
        static string logFileLoc = "";
        public static void DSLog(Discord.LogLevel level, string message)
        {
            Console.WriteLine("Discord:{0} - {1}", level, message);
            System.IO.File.AppendAllText(logFileLoc, $"{level}: {message}\r\n");
        }

        static string getState()
        {
            if (form == null)
                return "Connecting";
            if (form.GameForm == null)
                return "Loading";
            if (form.Self != null)
                return form.Self.Name;
            return "Chessing...";
        }


        static string getPlayers()
        {
            if (form == null || form.GameForm == null || form.Game == null)
                return "";
            return $"{(form.Game.White?.Name ?? "None")} v {(form.Game.Black?.Name ?? "None")}";
        }

        static string getDescription()
        {
            if (form == null || form.GameForm == null || form.Game == null)
                return null;
            if (form.Self.Side == Classes.PlayerSide.None)
                return "Spectating " + getPlayers();
            return "Playing " + getPlayers();
        }

        static Discord.ActivityParty? getParty()
        {
            if (form == null || form.GameForm == null || form.Game == null)
                return null;
            var party = new Discord.ActivityParty();
            party.Id = "aoasnuhbasnuan";
            party.Size = new Discord.PartySize();
            party.Size.MaxSize = 2;
            party.Size.CurrentSize = form.Game.White == null ? 0 : form.Game.Black == null ? 1 : 2;
            return party;
        }

        static Discord.ActivitySecrets? getSecrets()
        {
            if (form == null || form.GameForm == null || form.Game == null)
                return null;
            return new Discord.ActivitySecrets()
            {
                Join = "joinGammon",
                Spectate = "someSpectate",
                Match = "someid"
            };
        }

        static Discord.ActivityAssets? getAssets()
        {
            if (form == null || form.GameForm == null || form.Game == null)
                return null;
            var ass = new Discord.ActivityAssets();
            var wait = form.Game.Waiting;
            ass.LargeImage = $"{wait.ToString().ToLower()[0]}_king";
            var p = wait == Classes.PlayerSide.White ? form.Game.White : form.Game.Black;
            ass.LargeText = $"{p?.Name}'s go";
            if(form.Self != null && form.Self.Side != Classes.PlayerSide.None)
            {
                ass.SmallImage = $"{form.Self.Side.ToString().ToLower()[0]}_pawn";
                ass.SmallText = $"Playing as {form.Self.Side}";
            }
            return ass;
        }

        static Discord.ActivityTimestamps? getTimestamps()
        {
#pragma warning disable CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
            if (form == null || form.GameForm == null || form.Game == null)
                return null;
            if (form.JoinedAt.HasValue == false)
                return null;
            var time = new Discord.ActivityTimestamps();
            time.Start = form.GetTimestamp(form.JoinedAt.Value);
#pragma warning restore CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
            return time;
        }

        public static void setActivity()
        {
            var activityManager = DiscordClient.GetActivityManager();

            var activity = new Discord.Activity()
            {
                ApplicationId = CLIENT_ID,
                State = getState(),
                Name = "Chess",
                Details = getDescription(),
                Instance = true,
            };
            var party = getParty();
            if (party.HasValue)
                activity.Party = party.Value;
            var secrets = getSecrets();
            if (secrets.HasValue)
                activity.Secrets = secrets.Value;
            var ass = getAssets();
            if (ass.HasValue)
                activity.Assets = ass.Value;
            var time = getTimestamps();
            if (time.HasValue)
                activity.Timestamps = time.Value;
#if DEBUG
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(activity));
#endif
            activityManager.UpdateActivity(activity, x =>
            {
                DSLog(Discord.LogLevel.Info, $"Activity: {x}");
            });
        }

        const long CLIENT_ID = 553650964276576318;
        static void performDiscordStuffs()
        {
            System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", PROGRAM_COUNTER.ToString());
            DiscordClient = new Discord.Discord(CLIENT_ID, (ulong)Discord.CreateFlags.Default);
            var empty = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("CheAle14");
            var chess = empty.CreateSubKey("ChessClient");
            DiscordClient.SetLogHook(Discord.LogLevel.Debug, DSLog);
            /*dsThread = new Thread(dsUpdate);
            dsThread.SetApartmentState(ApartmentState.MTA);
            dsThread.Priority = ThreadPriority.Highest;
            dsThread.Start();*/
            var appManager = DiscordClient.GetApplicationManager();
            var activityManager = DiscordClient.GetActivityManager();
            var usrMan = DiscordClient.GetUserManager();

            try
            {
                var cur = usrMan.GetCurrentUser();
                DSLog(Discord.LogLevel.Info, "User: " + cur.Username);
            }
            catch(Exception ex)
            {
                DSLog(Discord.LogLevel.Warn, "Maybe no user: " + ex.ToString());
            }
            activityManager.RegisterCommand(chess.GetValue("") + " --discord");
            activityManager.OnActivitySpectate += ActivityManager_OnActivitySpectate;
            activityManager.OnActivityJoinRequest += ActivityManager_OnActivityJoinRequest;
            activityManager.OnActivityJoin += ActivityManager_OnActivityJoin;
            activityManager.OnActivityInvite += ActivityManager_OnActivityInvite;
        }

        private static void ActivityManager_OnActivityInvite(Discord.ActivityActionType type, ref Discord.User user, ref Discord.Activity activity)
        {
            DSLog(Discord.LogLevel.Info, $"Invited: {type} {user.Username}");
        }

        private static void ActivityManager_OnActivityJoin(string secret)
        {
            DSLog(Discord.LogLevel.Info, $"Join w/ {secret}");
            if(form.Self == null)
            { // something errored
                DSLog(Discord.LogLevel.Warn, "Not joining: Self null");
                return;
            }
            form.joinWithMethod("join");
        }

        private static void ActivityManager_OnActivityJoinRequest(ref Discord.User user)
        {
            var USER = user;
            var mang = DiscordClient.GetActivityManager();
            if(form == null || form.Game == null || form.GameForm == null)
            {
                mang.SendRequestReply(user.Id, Discord.ActivityJoinRequestReply.No, x =>
                {
                    DSLog(Discord.LogLevel.Info, $"Refuse {USER.Id}: {x}");
                });
            }
            DSLog(Discord.LogLevel.Info, $"Req Join: {user.Username}");
            form.GameForm.ShowJoinRequest(user);
        }

        private static void ActivityManager_OnActivitySpectate(string secret)
        {
            DSLog(Discord.LogLevel.Info, $"Spec w/: {secret}");
            if (form.Self == null)
            { // something errored
                DSLog(Discord.LogLevel.Warn, "Not joining: Self null");
                return;
            }
            form.joinWithMethod("spectate");
        }

        static Thread dsThread;
        static void dsUpdate()
        {
            Thread.Sleep(10 * 1000);
            DSLog(Discord.LogLevel.Debug, "Starting callback loop");
            while(form != null && form.Disposing == false && !closing)
            {
                /*try
                {
                    */DiscordClient.RunCallbacks();/*
                } catch (NullReferenceException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    DSLog(Discord.LogLevel.Error, ex.ToString());
                }*/
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            loadOptions();
            if(Options.UseDiscord)
            {
                var empty = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("CheAle14");
                var chess = empty.CreateSubKey("ChessClient");
                string path = (string)chess.GetValue("", System.IO.Directory.GetCurrentDirectory() + "/file.txt");
                string file = System.IO.Path.GetFileName(path);
                logFileLoc = System.IO.Path.Combine(path.Replace(file, ""),
                    $"ds_{PROGRAM_COUNTER}.txt");
                DSLog(Discord.LogLevel.Debug, "Starting new Discord-enabled program");
                performDiscordStuffs();
            }
            form = new StartForm();
            Application.Run(form);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.ExceptionObject.ToString());
            MessageBox.Show(e.ExceptionObject.ToString(), "Unhandled Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
