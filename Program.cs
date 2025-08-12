using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game;
using TheChosenProject.Cryptography;
using TheChosenProject.Multithreading;
using TheChosenProject.Database;
using System.Net;
using System.Net.Sockets;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Game.MsgNpc;
using Extensions;
using System.Windows.Forms;
using TheChosenProject;
using TheChosenProject.Role;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using System.Text.RegularExpressions;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.ServerSockets;
using TheChosenProject.Managers;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgEvents;
using System.Net.Mail;
using TheChosenProject.Role.Bot;
using DotNetEnv;
using System.Diagnostics;

namespace TheChosenProject
{
    public class Discord
    {
        string API;
        Queue<string> Msgs;
        Uri webhook;
        private static readonly int delayMilliseconds = 2000; // Adjust delay to avoid rate limit

        public Discord(string API)
        {
            this.API = API;
            Msgs = new Queue<string>();
            webhook = new Uri(API);
            //Console.WriteLine("Discord Server Ready.");
            var thread = new Thread(Dequeue);
            thread.Start();
        }

        private void Dequeue()
        {
            while (true)
            {
                try
                {
                    while (Msgs.Count != 0)
                    {
                        var msg = Msgs.Dequeue();
                        //Console.WriteLine($"Dequeuing message: {msg}");
                        postToDiscord(msg);
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error in Dequeue: {e}");
                }
            }
        }

        public void Enqueue(string str)
        {
            Msgs.Enqueue($"``{str} - {DateTime.Now}``");
            //Console.WriteLine($"Enqueued message: {str}");
            //postToDiscord(str);
        }

        private void postToDiscord(string Text)
        {
            try
            {
                Text = Text.Replace("@everyone", "");
                HttpClient client = new HttpClient();

                Dictionary<string, string> discordToPost = new Dictionary<string, string>
        {
            { "content", Text }
        };

                var content = new FormUrlEncodedContent(discordToPost);
                var res = client.PostAsync(webhook, content).Result;

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Message sent to Discord!");
                }
                else
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Failed to send message. Webhook URL not found (404). Please check the webhook URL.");
                    }
                    else if (res.StatusCode == (System.Net.HttpStatusCode)429)
                    {
                        Console.WriteLine("Failed to send message. Rate limit exceeded (429). Retrying after delay...");
                        Thread.Sleep(delayMilliseconds); // Wait before retrying
                        Msgs.Enqueue(Text); // Re-enqueue the message
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send message. Status code: {res.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting to Discord: {ex.Message}");
            }
        }

}
    internal class Program
    {
        public static DropRule DropRuleBase;
        public static VoteSystem.VoteRank VoteRank;
        public static Dictionary<string, string> MacJoin = new Dictionary<string, string>();

        #region Random
        public static WindowsAPI.CryptoRandom DropRandom = new WindowsAPI.CryptoRandom();
        public static Dictionary<DBLevExp.Sort, Dictionary<byte, DBLevExp>> LevelInfo = new Dictionary<DBLevExp.Sort, Dictionary<byte, DBLevExp>>();
        public static Random Rand = new Random();
        #endregion Random

        /// <summary>
        /// Add Discord APIs
        /// </summary>
        //public static Discord DiscordAdminLogHandler = new Discord("https://discord.com/api/webhooks/1381435258452578384/6VfkOA8sulQb3_fCcaUSIsgdNs1H-jgFzVoJOpYLVJAPjEvE50ZfGieYzbUewi1w8pez");
        //public static Discord DiscordWorldAPI = new Discord("https://discord.com/api/webhooks/1371383612590657536/8agvV3QSnbRk4pHtlhJ3nVh4eUNFGMmJtm74pvUkB5ZbEsHvjPuWApfvQGnPX9I3ZgMR");
        //public static Discord DiscordEventsAPI = new Discord("https://discord.com/api/webhooks/1381011020499390474/7-DYPS59Inz5UxuLGqEz0ZVDtbCN3uaM9PEPJpxatkgF-cWJ8UN-7Z929qTOGccmgWrv");
        //public static Discord DiscordGMTradeAPI = new Discord("https://discord.com/api/webhooks/1381006696071168131/PRT2mpWwuywJXhiDYTzkzk2f1uMICoGldwtIzDrBb6CuCS6Aozt2bb0zh-_yIVfnvqPe");
        //public static Discord DiscordFirstRB = new Discord("https://discord.com/api/webhooks/1381008183027302492/EVcG3C4xQK7kJZnlWhQvDJimM9pBjefkAGuDQmd4jdTtRqCYa8GXVUe7r1HAOxHzVFWK");
        //public static Discord DiscordSecondRB = new Discord("https://discord.com/api/webhooks/1381010065850372128/AMH3ylOAqmi8J0B3lEMEJjGNHSMX3tBkgEhZsOUnWkLKCI2WtijJhNB7SWKU0YY8h2W5");
        //public static Discord DiscordLevelAPI = new Discord("https://discord.com/api/webhooks/1371383582236606544/xhWXAnCFlSWzL-vFu_CfdUmr23sY_f6WXGgOSh5ZBH0eMJyaT8IlZKBCGkEVaDC1OYgy");
        //public static Discord DiscordCBosses = new Discord("https://discord.com/api/webhooks/1371395552490688562/0ich0kk_TouD_zBR11STUQJbABfgrGyHaYzoppCzHnUTFfgjMroR0dM7tUxHf6V0CJd-");
        //public static Discord DiscordSocketMaster1 = new Discord("https://discord.com/api/webhooks/1371458549632335892/kgCdw_skw0wwjMovQbl_DzL2lduy0vI9gQyVtDr2LyDOnKoetKnR4KCfhs6s0irsuz08");
        //public static Discord DiscordSocketMaster2 = new Discord("https://discord.com/api/webhooks/1381004822693806160/E57QzJ9s4rEWiBwIGB5uIs2fx_gIeEMi7fQCQbiHscizvphzmiQqhw2eGk1AGuWJfUGg");
        //public static Discord DiscordPlayerOnAPI = new Discord("https://discord.com/api/webhooks/1371458539842699345/gos-UyVl1Bm7nNnCFVhUhivIIGSTWSnIjEXB3hvIYCYS6pVgzB4WSaiCUK3jWvqjpA5e");
        //public static Discord DiscordOnlineAPI = new Discord("https://discord.com/api/webhooks/1371383980829577236/PfXn4lXhtxyOmtJHQdgG7dhBQ_MMZOxSu95mnfO00o5-md1R7Ye_V2jl_sRaOARhhfUW");
        //public static Discord DiscordWaterLord = new Discord("https://discord.com/api/webhooks/1393581977759645728/ss2s4Z2i9LleecqaT9yWiAtm6FoXbGVXAOv-5aK1xnSlRKBORs8gOtFrfR8seixCpqrr");
        //public static Discord DiscordAdminAPI = new Discord("https://discord.com/api/webhooks/1371387018688335872/ZO-_9COtRIu2FzxTFjkGM6EaHbR2xG-lDhDO1zRwue56Kdg5GkN8EzAHN1LDwuYfoGdr");
        //public static Discord DiscordDropPick = new Discord("https://discord.com/api/webhooks/1383982180904210595/FOxY27-er9uYZS1Ms6MRLB8qxr3f44nGAwpz1C1_axZyg629_bQZQaEfgxWDRr61zVR6");
        //public static Discord DiscordQuestRebirth = new Discord("https://discord.com/api/webhooks/1383809099313053888/qRcva3xMYE4aGQHYmuyRWrbLyZMRZ3W-_2elnAEUIZjwjfck_1RzwOGIdFT8mrJ3L63j");
        //public static Discord DiscordSpecialDrop = new Discord("https://discord.com/api/webhooks/1371457815901638736/S8vWlosPKank9oOhClr1kjC_N7mxZAAiJaT7oqyn1F5OjVRjwA1SUXimOPW2Eqc7huXR");
        //public static Discord DiscordBanned = new Discord("https://discord.com/api/webhooks/1382335797155594250/ADzb6sQISKhLIzaShpy_JVcBhcEA1jJGMTMZzixPK-9EEQba-TFJ2vj4FEW_7NVO76E7");
        //public static Discord DiscordOnlinePoints = new Discord("https://discord.com/api/webhooks/1384005049914757171/lEwlK8Fw6jmMYb9D-I1dRsaFxBHc_2rELW-YpEbOj_b8rDlmXjl7GCRsRgBIWJJ3gj3Y");
        //public static Discord DiscordBlueMouse = new Discord("https://discord.com/api/webhooks/1384530871628861514/A83pnJhL6C8c2PhB_Itpe78wsr8XSwvcgPTwJSgNQJIbv300K2JNlue-clqO1qalNtWP");
        //public static Discord DiscordStakeTC = new Discord("https://discord.com/api/webhooks/1390701024943149126/EPcOeJNP8ypxAts-PpkJslreljR5fpv0DKHYzyzCkiN9wPosVmcEHhyrJr7tuDjhOrn5");


        public static int WorldEvent = 0;
        public static MyRandom Rnd = new MyRandom();
        public static string ExcAdd = "";
        public static List<uint> EventsMaps = new List<uint>()
            {
                (uint)700, (uint)701, (uint)1763,
                (uint)1080, (uint)1126, (uint)1505,
                (uint)1506, (uint)1507, (uint)1508, 1090,1767
            };
        public static List<Game.MsgEvents.Events> Events = new List<Game.MsgEvents.Events>();
        public static List<byte[]> LoadPackets = new List<byte[]>();
        public static SocketPoll SocketsGroup;
        public static List<uint> ProtectMapSpells = new List<uint> { 1038 };
        public static List<uint> MapCounterHits = new List<uint> { 1005, 6000, 1550 };
        public static bool OnMainternance = false;
        public static TransferCipher transferCipher;
        public static Time32 SaveDBStamp = Time32.Now.AddMilliseconds(180000);
        public static List<uint> NotAllowAutoHunting = new List<uint> { 1121, 3845, 1039, 3040, 1005 };
        public static List<uint> NoDropItems = new List<uint> { 3060,3040, 19891, 19892,19893,19894,19895, 1767,  1764, 700, 1505 };
        public static List<uint> FreePkMap = new List<uint>
        {
            2058,3060,22340,22341,22342,22343,3040, 19891,19892,19893,19894,19895,1505, 2071, 6000, 6001, 1505, 1005, 1038, 700, 1508, 1509,
            2057,2071, 3998, 3071, 6000, 6001, 1126,1090,1767,
            1505, 1005, 1038, 700, 1508, Game.MsgTournaments.MsgCaptureTheFlag.MapID,2072
        };
        public static List<uint> BlockAttackMap = new List<uint>
        {
            1801, 1780, 1779, 1806, 1036, 1004, 1008, 601,
            1006, 1511, 1039,1009, 700, 2068,1002/*,1700*/,
        };
        public static List<uint> BlockTeleportMap = new List<uint> { 1121, 3040, 601, 6000, 6001, 2068, 1768, 1038, 700, 1049 };
        public static Nobility.NobilityRanking NobilityRanking = new Nobility.NobilityRanking();
        public static Flowers.FlowersRankingToday FlowersRankToday = new Flowers.FlowersRankingToday();
        public static Flowers.FlowerRanking GirlsFlowersRanking = new Flowers.FlowerRanking();
        public static Flowers.FlowerRanking BoysFlowersRanking = new Flowers.FlowerRanking(false);
        public static ShowChatItems GlobalItems;
        public static SendGlobalPacket SendGlobalPackets;
        public static ConcurrentDictionary<TheChosenProject.Mobs.IDMonster, Mobs.Base> MonsterCity = new ConcurrentDictionary<TheChosenProject.Mobs.IDMonster, Mobs.Base>();
        public static CachedAttributeInvocation<Action<GameClient, Packet>, PacketAttribute, ushort> MsgInvoker;
        public static ServerSocket TheChosenProject;
        public static Time32 ResetRandom = default(Time32);
        public static SafeRandom GetRandom = new SafeRandom();
        public static RandomLite LiteRandom = new RandomLite();
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            CopyAll(diSource, diTarget);
        }
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static NetworkMonitor NetworkMonitor { get; set; } = new NetworkMonitor();
        public static string MyIP => Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault((IPAddress ip) => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        public static bool ProcessConsoleEvent(int type)
        {
            try
            {
                try
                {
                    if (TheChosenProject != null)
                        TheChosenProject.Close();
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
                ServerKernel.Log.SaveLog("Saving Database...", true, LogType.DEBUG);
                foreach (GameClient client in Server.GamePoll.Values)
                {
                    try
                    {
                        if (client.Socket != null)
                            client.Socket.Disconnect();
                    }
                    catch (Exception e3)
                    {
                        ServerKernel.Log.SaveLog(e3.ToString(), false, LogType.EXCEPTION);
                    }
                }
                Clan.ProcessChangeNames();
                Server.SaveDatabase();
                if (ServerDatabase.LoginQueue.Finish())
                {
                    Thread.Sleep(1000);
                    ServerKernel.Log.SaveLog("Database Save Succefull.", false, LogType.DEBUG);
                }
            }
            catch (Exception e2)
            {
                ServerKernel.Log.SaveLog(e2.ToString(), false, LogType.EXCEPTION);
            }
            return true;
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
        }

        public static void Main(string[] args)
        {
            try
            {

                ///  MsgProtect.HelperFunctions.Init();
                // DiscordWorldAPI.Enqueue("Welcome to the World of North Conquer!");
                // DiscordMarketAPI.Enqueue("Market Update: New items available!");
                // DiscordEventsAPI.Enqueue("Event Announcement: Double XP Weekend!");
                // DiscordTOSAPI.Enqueue("Terms of Service have been updated.");
                // DiscordCBosses.Enqueue("New Bosses have been added to the game!");
                // DiscordDIBAPI.Enqueue("Daily Item Bonus: Log in to claim yours!");
                // DiscordDonationAPI.Enqueue("Support the server by donating!");
                //if(DatabaseConfig.discord_stat)
                //    DiscordOnlineAPI.Enqueue("Server is now online!");
                // DiscordChangeNameAPI.Enqueue("Your name change has been processed.");
                Env.Load();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(ServerKernel.ServerManager = new ServerManager());
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static void SaveDBPayers(Time32 clock)
        {
            if (!(clock > SaveDBStamp))
                return;
            if (Server.FullLoading)
            {
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    if ((user.ClientFlag & ServerFlag.LoginFull) == ServerFlag.LoginFull)
                    {
                        user.ClientFlag |= ServerFlag.QueuesSave;
                        ServerDatabase.LoginQueue.TryEnqueue(user);
                    }
                }
                Server.SaveDatabase();
            }
            SaveDBStamp.Value = clock.Value + 180000;
        }

        public static void Maintenance()
        {
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
                Packet stream11;
                stream11 = recycledPacket.GetStream();
                OnMainternance = true;
                string message;
                message = "The server will be brought down for maintenance in (5 Minutes). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg11;
                msg11 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg11.GetArray(stream11));
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket2 = new RecycledPacket())
            {
                Packet stream10;
                stream10 = recycledPacket2.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (4 Minutes & 30 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg10;
                msg10 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg10.GetArray(stream10));
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket3 = new RecycledPacket())
            {
                Packet stream9;
                stream9 = recycledPacket3.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (4 Minutes & 00 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg9;
                msg9 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg9.GetArray(stream9));
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket4 = new RecycledPacket())
            {
                Packet stream8;
                stream8 = recycledPacket4.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (3 Minutes & 30 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg8;
                msg8 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg8.GetArray(stream8));
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket5 = new RecycledPacket())
            {
                Packet stream7;
                stream7 = recycledPacket5.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (3 Minutes & 00 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg7;
                msg7 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg7.GetArray(stream7));
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket6 = new RecycledPacket())
            {
                Packet stream6;
                stream6 = recycledPacket6.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (2 Minutes & 30 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg6;
                msg6 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg6.GetArray(stream6));
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket7 = new RecycledPacket())
            {
                Packet stream5;
                stream5 = recycledPacket7.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (2 Minutes & 00 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg5;
                msg5 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket8 = new RecycledPacket())
            {
                Packet stream4;
                stream4 = recycledPacket8.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (1 Minutes & 30 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg4;
                msg4 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg4.GetArray(stream4));
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket9 = new RecycledPacket())
            {
                Packet stream3;
                stream3 = recycledPacket9.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (1 Minutes & 00 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg3;
                msg3 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg3.GetArray(stream3));
            }
            Thread.Sleep(30000);
            using (RecycledPacket recycledPacket10 = new RecycledPacket())
            {
                Packet stream2;
                stream2 = recycledPacket10.GetStream();
                string message;
                message = "The server will be brought down for maintenance in (0 Minutes & 30 Seconds). Please log off immediately to avoid data loss.";
                ServerKernel.Log.SaveLog(message, true, LogType.DEBUG);
                MsgMessage msg2;
                msg2 = new MsgMessage(message, "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg2.GetArray(stream2));
            }
            Thread.Sleep(20000);
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgMessage msg;
                msg = new MsgMessage("Server maintenance(few minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            //string exePath = Process.GetCurrentProcess().MainModule.FileName;

            //if (File.Exists(exePath))
            //{
            //    Process.Start(new ProcessStartInfo
            //    {
            //        FileName = exePath,
            //        UseShellExecute = true
            //    });
            //}

            Thread.Sleep(10000);
            ProcessConsoleEvent(0);
            Environment.Exit(0);
        }

        public static void Game_Receive(SecuritySocket obj, Packet stream)
        {
            if (!obj.SetDHKey)
            {
                CreateDHKey(obj, stream);
                return;
            }
            try
            {
                if (obj.Game != null)
                {
                    ushort PacketID;
                    PacketID = stream.ReadUInt16();
                    if (MsgInvoker.TryGetInvoker(PacketID, out Action<GameClient, Packet> hinvoker))
                    {
                        hinvoker(obj.Game, stream);
                    }
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
            finally
            {
                PacketRecycle.Reuse(stream);
            }
        }

        public unsafe static void CreateDHKey(SecuritySocket obj, Packet Stream)
        {
            try
            {
                byte[] buffer;
                buffer = new byte[36];
                bool extra;
                extra = false;
                string text;
                text = Encoding.ASCII.GetString(obj.DHKeyBuffer.buffer, 0, obj.DHKeyBuffer.Length());
                if (!text.EndsWith("TQClient"))
                {
                    Buffer.BlockCopy(obj.EncryptedDHKeyBuffer.buffer, obj.EncryptedDHKeyBuffer.Length() - 36, buffer, 0, 36);
                    extra = true;
                }
                if (Stream.GetHandshakeReplyKey(out var key))
                {
                    obj.SetDHKey = true;
                    obj.Game.Cryptography = obj.Game.DHKeyExchance.HandleClientKeyPacket(key, obj.Game.Cryptography);
                    if (!extra)
                        return;
                    Stream.Seek(0);
                    obj.Game.Cryptography.Decrypt(buffer);
                    fixed (byte* ptr = buffer)
                    {
                        Stream.memcpy(Stream.Memory, ptr, 36);
                    }
                    Stream.Size = buffer.Length;
                    Stream.Size = buffer.Length;
                    Stream.Seek(2);
                    ushort PacketID;
                    PacketID = Stream.ReadUInt16();
                    if (MsgInvoker.TryGetInvoker(PacketID, out Action<GameClient, Packet> hinvoker))
                    {
                        hinvoker(obj.Game, Stream);
                        return;
                    }
                    obj.Disconnect();
                    ServerKernel.Log.SaveLog("DH KEY Not found the packet ----> " + PacketID, true, LogType.WARNING);
                }
                else
                    obj.Disconnect();
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public unsafe static void Game_Disconnect(SecuritySocket obj)
        {
            if (obj.Game != null && obj.Game.Player != null)
            {
                try
                {
                    if (Game.MsgEvents.TeamDeathMatch.AwaitingPlayers.Contains(obj.Game))
                        Game.MsgEvents.TeamDeathMatch.AwaitingPlayers.Remove(obj.Game);
                    if (obj.Game.InPassTheBomb)
                        Game.MsgEvents.PassTheBomb.OnDisconnect(obj.Game);
                    if (obj.Game.InTDM)
                        Game.MsgEvents.TeamDeathMatch.OnDisconnect(obj.Game);
                    if (obj.Game.InFIveOut)
                        Game.MsgEvents.Get5HitOut.OnDisconnect(obj.Game);
                    if (obj.Game.InLastManStanding)
                        Game.MsgEvents.LastManStanding.OnDisconnect(obj.Game);
                    if (obj.Game.InSSFB)
                        Game.MsgEvents.SkillsTournament.OnDisconnect(obj.Game);
                    if (Server.GamePoll.TryRemove(obj.Game.Player.UID, out var client))
                    {
                        ServerKernel.Log.SaveLog($"User [{client.Player.Name}] has disconnected.", true, LogType.MESSAGE);
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;
                            stream = rec.GetStream();
                            try
                            {
                                if (UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID))
                                {
                                    foreach (var bot in BotProcessring.Bots.Values)
                                    {
                                        if (bot.Bot != null)
                                        {
                                            if (bot.Bot.Player.Map == client.Player.Map &&
                                                bot.Bot.Player.DynamicID == client.Player.DynamicID)
                                                bot.Dispose();
                                        }
                                    }
                                    client.Teleport(428, 380, 1002);
                                }
                                client.EndQualifier();
                                if (client.Player.AutoHunting == AutoStructures.Mode.Enable)
                                    client.Player.AutoHunting = AutoStructures.Mode.Disable;
                                client.Team?.Remove(client, true);
                                if (client.Player.MyClanMember != null)
                                    client.Player.MyClanMember.Online = false;
                                if (client.IsVendor)
                                    client.MyVendor.StopVending(stream);
                                if (client.Mining)
                                    client.StopMining();
                                if (client.InTrade)
                                    client.MyTrade.CloseTrade();
                                if (client.Pet != null)
                                    client.Pet.DeAtach(stream);
                                if (client.Player.MyGuildMember != null)
                                    client.Player.MyGuildMember.IsOnline = false;
                                if (client.Player.ObjInteraction != null)
                                {
                                    client.Player.InteractionEffect.AtkType = MsgAttackPacket.AttackID.InteractionStopEffect;
                                    InteractQuery action;
                                    action = InteractQuery.ShallowCopy(client.Player.InteractionEffect);
                                    client.Send(stream.InteractionCreate(&action));
                                    client.Player.ObjInteraction.Player.OnInteractionEffect = false;
                                    client.Player.ObjInteraction.Player.ObjInteraction = null;
                                }
                                client.Player.LastLoginClient = DateTime.Now;
                                client.Player.UpdateSurroundings(stream, clear: false, true);
                                client.Player.View.Clear(stream);

                            }
                            catch (Exception e3)
                            {
                                ServerKernel.Log.SaveLog(e3.ToString(), false, LogType.EXCEPTION);
                                client.Player.View.Clear(stream);
                            }
                            finally
                            {
                                ServerDatabase.SaveClient(client);
                                if (client.Player.Delete)
                                {
                                    ServerDatabase.DeleteCharacter(client);
                                }
                            }
                            try
                            {
                                client.Player.Associate.OnDisconnect(stream, client);
                                if (client.Player.MyMentor != null)
                                {
                                    client.Player.MyMentor.OnlineApprentice.TryRemove(client.Player.UID, out var _);
                                    client.Player.MyMentor = null;
                                }
                                client.Player.Associate.Online = false;
                                lock (client.Player.Associate.MyClient)
                                {
                                    client.Player.Associate.MyClient = null;
                                }
                                foreach (GameClient clien in client.Player.Associate.OnlineApprentice.Values)
                                {
                                    clien.Player.SetBattlePowers(0, 0);
                                }
                                client.Player.Associate.OnlineApprentice.Clear();
                                if (client.Map != null)
                                    client.Map.Denquer(client);
                                return;
                            }
                            catch (Exception e2)
                            {
                                ServerKernel.Log.SaveLog(e2.ToString(), false, LogType.EXCEPTION);
                                return;
                            }
                        }
                    }
                    return;
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                    return;
                }
            }
            if (obj.Game != null && obj.Game.ConnectionUID != 0)
                Server.GamePoll.TryRemove(obj.Game.ConnectionUID, out var _);
        }

        public static bool CheckNumberPassword(string password)
        {
            if (password == "")
                return false;
            if (password.StartsWith("0"))
                return false;
            if (password.Length > 8)
                return false;
            foreach (char c in password)
            {
                if ((byte)c < 48 || (byte)c > 57)
                    return false;
            }
            return true;
        }

        public static bool NameStrCheck(string name, bool ExceptedSize = true)
        {
            if (name.Contains("~"))
                return false;
            if (name == null)
                return false;
            if (name == "")
                return false;
            string ValidChars;
            ValidChars = "[^A-Za-z0-9ء-ي*~.&.$]$";
            Regex r;
            r = new Regex(ValidChars);
            if (r.IsMatch(name))
                return false;
            if (name.ToLower().Contains("none"))
                return false;
            if (name.ToLower().Contains("Vs"))
                return false;
            if (name.ToLower().Contains("gm"))
                return false;
            if (name.ToLower().Contains("pm"))
                return false;
            if (name.ToLower().Contains("p~m"))
                return false;
            if (name.ToLower().Contains("p!m"))
                return false;
            if (name.ToLower().Contains("g~m"))
                return false;
            if (name.ToLower().Contains("g!m"))
                return false;
            if (name.ToLower().Contains("help"))
                return false;
            if (name.ToLower().Contains("desk"))
                return false;
            if (name.ToLower().Contains("admin"))
                return false;
            if (name.ToLower().Contains("prvixy"))
                return false;
            if (name.Contains('/'))
                return false;
            if (name.Contains("\\"))
                return false;
            if (name.Contains("'"))
                return false;
            if (name.Contains("GM") || name.Contains("PM") || name.Contains("SYSTEM") || name.Contains("{") || name.Contains("}") || name.Contains("[") || name.Contains("]"))
                return false;
            if (name.Length > 16 && ExceptedSize)
                return false;
            for (int x = 0; x < name.Length; x++)
            {
                if (name[x] == '\u0019')
                    return false;
            }
            return true;
        }

        public static bool StringCheck(string pszString)
        {
            for (int x = 0; x < pszString.Length; x++)
            {
                if (pszString[x] > ' ' && pszString[x] <= '~')
                    return false;
            }
            return true;
        }
    }
}
