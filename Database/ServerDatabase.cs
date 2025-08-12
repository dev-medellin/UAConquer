using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TheChosenProject.Game.MsgServer;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.WindowsAPI;
using Extensions;
using TheChosenProject.Struct;
using DevExpress.Utils.MVVM.Internal;

namespace TheChosenProject.Database
{
    public class ServerDatabase
    {
        public static ServerDatabase.ExecuteLogin LoginQueue = new ServerDatabase.ExecuteLogin();

        public class ExecuteLogin : ConcurrentSmartThreadQueue<object>
        {
            public object SynRoot = new object();

            public ExecuteLogin()
                : base(5)
            {
                Start(10);
            }

            public void TryEnqueue(object obj)
            {
                base.Enqueue(obj);
            }
            protected override void OnDequeue(object obj, int time)
            {
                try
                {
                    if (obj is string)
                    {
                        string text = obj as string;
                        if (text.StartsWith("[Chat]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Chat" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Trade]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Trade" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Item]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Item" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[CallStack]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "CallStack" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[CallStack2]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "CallStack2" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        
                        if (text.StartsWith("[GMLogs]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "GMLogs";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        if (text.StartsWith("[PlayersLogs]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "PlayersLogs";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        if (text.StartsWith("[OnlinePoints]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            //string date = "OnlinePoints";
                            string date = "OnlinePoints" + dt.Month + "-" + dt.Day + "";


                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        //GMLogs
                    }

                    if (obj is GameMap)
                    {
                        UpdateMapRace(obj as GameMap);
                        return;
                    }
                    if (obj is Guild.Member)
                    {
                        UpdateGuildMember(obj as Guild.Member);
                        return;
                    }
                    if (obj is Guild.UpdateDB)
                    {
                        UpdateGuildMember(obj as Guild.UpdateDB);
                        return;
                    }
                    if (obj is Clan.Member)
                    {
                        UpdateClanMember(obj as Clan.Member);
                        return;
                    }
                    GameClient client;
                    client = obj as GameClient;
                    if (client != null && client.Player != null)
                    {

                        if (client.Player.Delete)
                        {
                            client.Map?.View.LeaveMap(client.Player);

                            ServerKernel.Log.SaveLog($"Client {client.Player.Name} deleted their account.", true, LogType.DEBUG);

                            string timestamp = GenerateDate(); // Example: "20250719_0430"
                            string deletedFolder = Path.Combine(ServerKernel.CO2FOLDER, "DeletedAccount", $"{client.Player.UID}__{timestamp}");

                            // Ensure the deleted folder exists
                            Directory.CreateDirectory(deletedFolder);

                            void BackupFile(string relativePath)
                            {
                                string fullPath = Path.Combine(ServerKernel.CO2FOLDER, relativePath);
                                if (File.Exists(fullPath))
                                {
                                    string fileName = Path.GetFileName(fullPath);
                                    string destPath = Path.Combine(deletedFolder, fileName);
                                    File.Copy(fullPath, destPath, true);
                                    File.Delete(fullPath);
                                }
                            }

                            // Move and delete related player files
                            BackupFile($"Users\\{client.Player.UID}.ini");
                            BackupFile($"PlayersSpells\\{client.Player.UID}.bin");
                            BackupFile($"PlayersProfs\\{client.Player.UID}.bin");
                            BackupFile($"PlayersItems\\{client.Player.UID}.bin");
                            BackupFile($"Quests\\{client.Player.UID}.bin");
                            BackupFile($"Houses\\{client.Player.UID}.bin");

                            // Clean up memory associations
                            if (client.MyHouse != null && House.HousePoll.ContainsKey(client.Player.UID))
                                House.HousePoll.TryRemove(client.Player.UID, out _);

                            if (Flowers.ClientPoll.ContainsKey(client.Player.UID))
                                Flowers.ClientPoll.TryRemove(client.Player.UID, out _);

                            if (TheChosenProject.Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out var associate))
                                TheChosenProject.Role.Instance.Associate.Associates.TryRemove(client.Player.UID, out _);

                            if (Server.GamePoll.TryRemove(client.Player.UID, out var user))
                            {
                                if (Server.NameUsed.Contains(user.Player.Name.GetHashCode()))
                                {
                                    lock (Server.NameUsed)
                                        Server.NameUsed.Remove(user.Player.Name.GetHashCode());
                                }
                            }
                        }

                        if ((client.ClientFlag & ServerFlag.RemoveSpouse) == ServerFlag.RemoveSpouse)
                        {
                            DestroySpouse(client);
                            client.ClientFlag &= ~ServerFlag.RemoveSpouse;
                        }
                        else if ((client.ClientFlag & ServerFlag.UpdateSpouse) == ServerFlag.UpdateSpouse)
                        {
                            UpdateSpouse(client);
                            client.ClientFlag &= ~ServerFlag.UpdateSpouse;
                        }
                        else if ((client.ClientFlag & ServerFlag.SetLocation) != ServerFlag.SetLocation && (client.ClientFlag & ServerFlag.OnLoggion) == ServerFlag.OnLoggion)
                        {
                            MsgLoginClient.LoginHandler(client, client.OnLogin);
                        }
                        else if ((client.ClientFlag & ServerFlag.QueuesSave) == ServerFlag.QueuesSave && !client.Player.Delete)
                        {
                            if (client.Player.OnTransform)
                                client.Player.HitPoints = Math.Min(client.Player.HitPoints, (int)client.Status.MaxHitpoints);
                            SaveClient(client);
                        }
                    }
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
            }

            //protected override void OnDequeue(object obj, int time)
            //{
            //    try
            //    {
            //        if (obj is GameMap)
            //        {
            //            UpdateMapRace(obj as GameMap);
            //            return;
            //        }
            //        if (obj is Guild.Member)
            //        {
            //            UpdateGuildMember(obj as Guild.Member);
            //            return;
            //        }
            //        if (obj is Guild.UpdateDB)
            //        {
            //            UpdateGuildMember(obj as Guild.UpdateDB);
            //            return;
            //        }
            //        if (obj is Clan.Member)
            //        {
            //            UpdateClanMember(obj as Clan.Member);
            //            return;
            //        }
            //        GameClient client;
            //        client = obj as GameClient;
            //        if (client != null && client.Player != null && client.Player.Delete)
            //        {
            //            client.Map?.View.LeaveMap(client.Player);
            //            DateTime Now64;
            //            Now64 = DateTime.Now;
            //            ServerKernel.Log.SaveLog("Client " + client.Player.Name + " delete he account.", true, LogType.DEBUG);
            //            if (File.Exists(ServerKernel.CO2FOLDER + "\\Users\\" + client.Player.UID + ".ini"))
            //                File.Copy(ServerKernel.CO2FOLDER + "\\Users\\" + client.Player.UID + ".ini", ServerKernel.CO2FOLDER + "\\BackUp\\Users\\" + client.Player.UID + "__" + GenerateDate() + ".ini", true);
            //            if (File.Exists(ServerKernel.CO2FOLDER + "\\PlayersSpells\\" + client.Player.UID + ".bin"))
            //                File.Copy(ServerKernel.CO2FOLDER + "\\PlayersSpells\\" + client.Player.UID + ".bin", ServerKernel.CO2FOLDER + "\\BackUp\\PlayersSpells\\" + client.Player.UID + "__" + GenerateDate() + ".bin", true);
            //            if (File.Exists(ServerKernel.CO2FOLDER + "\\PlayersProfs\\" + client.Player.UID + ".bin"))
            //                File.Copy(ServerKernel.CO2FOLDER + "\\PlayersProfs\\" + client.Player.UID + ".bin", ServerKernel.CO2FOLDER + "\\BackUp\\PlayersProfs\\" + client.Player.UID + "__" + GenerateDate() + ".bin", true);
            //            if (File.Exists(ServerKernel.CO2FOLDER + "\\PlayersItems\\" + client.Player.UID + ".bin"))
            //                File.Copy(ServerKernel.CO2FOLDER + "\\PlayersItems\\" + client.Player.UID + ".bin", ServerKernel.CO2FOLDER + "\\BackUp\\PlayersItems\\" + client.Player.UID + "__" + GenerateDate() + ".bin", true);
            //            if (File.Exists(ServerKernel.CO2FOLDER + "\\Users\\" + client.Player.UID + ".ini"))
            //                File.Delete(ServerKernel.CO2FOLDER + "\\Users\\" + client.Player.UID + ".ini");
            //            if (File.Exists(ServerKernel.CO2FOLDER + "\\PlayersSpells\\" + client.Player.UID + ".bin"))
            //                File.Delete(ServerKernel.CO2FOLDER + "\\PlayersSpells\\" + client.Player.UID + ".bin");
            //            if (File.Exists(ServerKernel.CO2FOLDER + "\\PlayersProfs\\" + client.Player.UID + ".bin"))
            //                File.Delete(ServerKernel.CO2FOLDER + "\\PlayersProfs\\" + client.Player.UID + ".bin");
            //            if (File.Exists(ServerKernel.CO2FOLDER + "\\PlayersItems\\" + client.Player.UID + ".bin"))
            //                File.Delete(ServerKernel.CO2FOLDER + "\\PlayersItems\\" + client.Player.UID + ".bin");
            //            try
            //            {
            //                if (File.Exists(ServerKernel.CO2FOLDER + "\\Quests\\" + client.Player.UID + ".bin"))
            //                    File.Delete(ServerKernel.CO2FOLDER + "\\Quests\\" + client.Player.UID + ".bin");
            //            }
            //            catch
            //            {
            //            }
            //            if (client != null)
            //            {
            //                if (client.MyHouse != null && House.HousePoll.ContainsKey(client.Player.UID))
            //                    House.HousePoll.TryRemove(client.Player.UID, out var _);
            //                if (File.Exists(ServerKernel.CO2FOLDER + "\\Houses\\" + client.Player.UID + ".bin"))
            //                    File.Delete(ServerKernel.CO2FOLDER + "\\Houses\\" + client.Player.UID + ".bin");
            //                if (Flowers.ClientPoll.ContainsKey(client.Player.UID))
            //                    Flowers.ClientPoll.TryRemove(client.Player.UID, out var _);
            //                if (TheChosenProject.Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out var Associate))
            //                    TheChosenProject.Role.Instance.Associate.Associates.TryRemove(client.Player.UID, out Associate);
            //                Server.GamePoll.TryRemove(client.Player.UID, out var user);
            //                if (Server.NameUsed.Contains(user.Player.Name.GetHashCode()))
            //                {
            //                    lock (Server.NameUsed)
            //                    {
            //                        Server.NameUsed.Remove(user.Player.Name.GetHashCode());
            //                        return;
            //                    }
            //                }
            //            }
            //        }

            //            if ((client.ClientFlag & Client.ServerFlag.RemoveSpouse) == Client.ServerFlag.RemoveSpouse)
            //            {
            //                DestroySpouse(client);
            //                client.ClientFlag &= ~Client.ServerFlag.RemoveSpouse;
            //                return;
            //            }
            //            if ((client.ClientFlag & Client.ServerFlag.UpdateSpouse) == Client.ServerFlag.UpdateSpouse)
            //            {
            //                UpdateSpouse(client);
            //                client.ClientFlag &= ~Client.ServerFlag.UpdateSpouse;
            //                return;
            //            }
            //            if ((client.ClientFlag & Client.ServerFlag.SetLocation) != Client.ServerFlag.SetLocation && (client.ClientFlag & Client.ServerFlag.OnLoggion) == Client.ServerFlag.OnLoggion)
            //            {
            //                Game.MsgServer.MsgLoginClient.LoginHandler(client, client.OnLogin);
            //            }
            //            else if ((client.ClientFlag & Client.ServerFlag.QueuesSave) == Client.ServerFlag.QueuesSave)
            //            {
            //                if (client.Player.OnTransform)
            //                {
            //                    client.Player.HitPoints = Math.Min(client.Player.HitPoints, (int)client.Status.MaxHitpoints);

            //                }
            //                SaveClient(client);

            //            }

            //    }
            //    catch (Exception e)
            //    {
            //        ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            //    }
            //}
        }

        public static void DeleteCharacter(GameClient client)
        {
            client.Map?.View.LeaveMap(client.Player);
            ServerKernel.Log.SaveLog($"Client {client.Player.Name} deleted their account.", true, LogType.DEBUG);

            // Folder naming: DeletedAccountDS/{UID}__{Name}__MM-dd-yyyy
            string today = DateTime.Now.ToString("MM-dd-yyyy");
            string baseDir = Path.Combine(ServerKernel.CO2FOLDER, "DeletedAccountDS", $"{client.Player.UID}__{client.Player.Name}__{today}");

            string userDir = Path.Combine(baseDir, "Users");
            string spellsDir = Path.Combine(baseDir, "PlayersSpells");
            string profsDir = Path.Combine(baseDir, "PlayersProfs");
            string itemsDir = Path.Combine(baseDir, "PlayersItems");
            string housesDir = Path.Combine(baseDir, "Houses");
            string clansDir = Path.Combine(baseDir, "Clans");
            string guildsDir = Path.Combine(baseDir, "Guilds");
            string createquests = Path.Combine(baseDir, "Quests");

            try
            {
                // Create directories
                Directory.CreateDirectory(userDir);
                Directory.CreateDirectory(spellsDir);
                Directory.CreateDirectory(profsDir);
                Directory.CreateDirectory(itemsDir);
                Directory.CreateDirectory(housesDir);
                Directory.CreateDirectory(clansDir);
                Directory.CreateDirectory(guildsDir);
                Directory.CreateDirectory(createquests);

                // Helper: Backup one file if it exists
                void BackupFile(string srcFolder, string dstFolder, string extension = "bin", bool isIni = false)
                {
                    string ext = isIni ? ".ini" : "." + extension;
                    string fileName = client.Player.UID + ext;
                    string srcPath = Path.Combine(ServerKernel.CO2FOLDER, srcFolder, fileName);
                    string dstPath = Path.Combine(dstFolder, fileName);
                    if (File.Exists(srcPath))
                    {
                        File.Copy(srcPath, dstPath, true);
                        File.Delete(srcPath);
                    }
                }

                // Copy only this player's files
                BackupFile("Users", userDir, "ini", true);
                BackupFile("PlayersSpells", spellsDir);
                BackupFile("PlayersProfs", profsDir);
                BackupFile("PlayersItems", itemsDir);
                BackupFile("Houses", housesDir);
                BackupFile("Clans", clansDir);
                BackupFile("Guilds", guildsDir);
                BackupFile("Quests", createquests);

                //Program.NobilityRanking.RemoveToRank(client.Player);
                client.UnlinkAccountFromEntity(client.Player.UID);
                Console.WriteLine($"Backup delete complete for {client.Player.Name} on {today}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during delete backup: " + ex.Message);
            }

            // Clean memory associations
            if (client.MyHouse != null && House.HousePoll.ContainsKey(client.Player.UID))
                House.HousePoll.TryRemove(client.Player.UID, out _);

            if (Flowers.ClientPoll.ContainsKey(client.Player.UID))
                Flowers.ClientPoll.TryRemove(client.Player.UID, out _);

            if (TheChosenProject.Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out var associate))
                TheChosenProject.Role.Instance.Associate.Associates.TryRemove(client.Player.UID, out _);

            Server.GamePoll.TryRemove(client.Player.UID, out var user);

            if (Server.NameUsed.Contains(client.Player.Name.GetHashCode()))
            {
                lock (Server.NameUsed)
                {
                    Server.NameUsed.Remove(client.Player.Name.GetHashCode());
                }
            }
        }


        public static void CopyDirectory(string sourceDir, string destDir, bool copySubDirs = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                Console.WriteLine($"Source directory does not exist: {sourceDir}");
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetPath = Path.Combine(destDir, file.Name);
                file.CopyTo(targetPath, true);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string targetSubDir = Path.Combine(destDir, subdir.Name);
                    CopyDirectory(subdir.FullName, targetSubDir);
                }
            }
        }

        public static void ResetingEveryDay(GameClient client)
        {
            try
            {
                if (DateTime.Now.DayOfYear != client.Player.Day)
                {
                    //client.Player.MiningAttempts = 200;
                    client.Player.KeyBoxTRY = 3;
                    client.Player.lettersTRY = 3;
                    client.Player.LavaTRY = 3;
                    client.Player.MisShoot = client.Player.HitShoot = 0;
                    client.Player.ArenaDeads = client.Player.ArenaKills = 0;
                    client.Player.LotteryEntries = 0;
                    client.Player.ConquerLetter = 0;
                    client.Player.LavaQuest = 0;
                    client.Player.BDExp = 0;
                    client.Player.TCCaptainTimes = 0;
                    client.DemonExterminator.FinishToday = 0;
                    client.Player.ExpBallUsed = 0;
                    client.Player.Flowers.FreeFlowers = 1;
                    foreach (var flower in client.Player.Flowers)
                        flower.Amount2day = 0;
                    if (client.Player.QuestGUI.CheckQuest(20195, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20195);
                    if (client.Player.QuestGUI.CheckQuest(20199, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20199);
                    if (client.Player.QuestGUI.CheckQuest(20198, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20198);
                    if (client.Player.QuestGUI.CheckQuest(20197, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20197);
                    if (client.Player.QuestGUI.CheckQuest(20193, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20193);
                    if (client.Player.QuestGUI.CheckQuest(20191, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20191);
                    if (client.Player.QuestGUI.CheckQuest(20192, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20192);
                    if (client.Player.QuestGUI.CheckQuest(20196, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20196);
                    if (client.Player.QuestGUI.CheckQuest(20194, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20194);
                    if (client.Player.QuestGUI.CheckQuest(20200, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.RemoveQuest(20200);
                    client.OnlinePointsManager.Reset();
                    client.TournamentsManager.Reset();
                    client.RoyalPassManager.Reset();
                    client.LimitedDailyTimes.Reset();
                    //client.DbDailyTraining.Reset();
                    client.SendSysMesage("A new day has begun and all missions have been reset.");

                    client.Player.Day = DateTime.Now.DayOfYear;
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static ushort CalculateEnlighten(Player player)
        {
            if (player.Level < 90)
                return 0;
            ushort val;
            val = 100;
            if (player.NobilityRank == Nobility.NobilityRank.Knight || player.NobilityRank == Nobility.NobilityRank.Baron)
                val = (ushort)(val + 100);
            if (player.NobilityRank == Nobility.NobilityRank.Earl || player.NobilityRank == Nobility.NobilityRank.Duke)
                val = (ushort)(val + 200);
            if (player.NobilityRank == Nobility.NobilityRank.Prince)
                val = (ushort)(val + 300);
            if (player.NobilityRank == Nobility.NobilityRank.King)
                val = (ushort)(val + 400);
            if (player.VipLevel <= 3)
                val = (ushort)(val + 100);
            if (player.VipLevel > 3 && player.VipLevel <= 5)
                val = (ushort)(val + 200);
            if (player.VipLevel > 5)
                val = (ushort)(val + 300);
            return val;
        }

        public static ushort CalculateRewardDaily(Player player)
        {
            ushort val;
            val = 100;
            if (player.NobilityRank == Nobility.NobilityRank.Knight || player.NobilityRank == Nobility.NobilityRank.Baron)
                val = (ushort)(val + 1000);
            if (player.NobilityRank == Nobility.NobilityRank.Earl || player.NobilityRank == Nobility.NobilityRank.Duke)
                val = (ushort)(val + 2000);
            if (player.NobilityRank == Nobility.NobilityRank.Prince)
                val = (ushort)(val + 3000);
            if (player.NobilityRank == Nobility.NobilityRank.King)
                val = (ushort)(val + 4000);
            if (player.VipLevel <= 3)
                val = (ushort)(val + 1000);
            if (player.VipLevel > 3 && player.VipLevel <= 5)
                val = (ushort)(val + 2000);
            if (player.VipLevel > 5)
                val = (ushort)(val + 3000);
            return val;
        }

        public static string GetSpecialTitles(GameClient user)
        {
            WriteLine writer;
            writer = new WriteLine(',');
            writer.Add((uint)user.Player.SpecialTitles.Count);
            foreach (KeyValuePair<MsgTitle.TitleType, DateTime> title in user.Player.SpecialTitles)
            {
                writer.Add((uint)title.Key);
                writer.Add(title.Value.Ticks);
            }
            return writer.Close();
        }

        public static void LoadSpecialTitles(GameClient user, string line)
        {
            ReadLine reader;
            reader = new ReadLine(line, ',');
            uint count;
            count = reader.Read((uint)0);
            for (int x = 0; x < count; x++)
            {
                uint Title;
                Title = reader.Read((uint)0);
                DateTime Time;
                Time = DateTime.FromBinary(reader.Read(0L));
                user.Player.SpecialTitles.Add((MsgTitle.TitleType)Title, Time);
                SpecialTitles dbtitle;
                if (DateTime.Now > Time)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        user.Player.RemoveSpecialTitle((MsgTitle.TitleType)Title, stream);
                        user.CreateBoxDialog("Your Title " + SpecialTitles.Titles[Title].Name + " expired.");
                    }
                }
                else if (SpecialTitles.Titles.TryGetValue(Title, out dbtitle))
                {
                    user.Player.SpecialTitleID = dbtitle.ID;
                    user.Player.SwitchTitle((byte)dbtitle.ID);
                    user.SendSysMesage("Your Title " + dbtitle.Name + " will removed on " + Time.ToString("d/M/yyyy (H:mm)") + ".");
                }
            }
        }
        //public const string ConnectionString = "Server=localhost;username=root;password=P@ssw0rd;database=bd;";

        public static void SaveClient(GameClient client)
        {
            try
            {
                IniFile write;
                write = new IniFile("\\Users\\" + client.Player.UID + ".ini");
                if ((client.ClientFlag & ServerFlag.LoginFull) != ServerFlag.LoginFull)
                    client.Map?.Denquer(client);
                if (((HouseTable.InHouse(client.Player.Map) && client.Player.DynamicID != 0) || client.Player.DynamicID != 0) && client.Socket != null && !client.Socket.Alive)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 428;
                    client.Player.Y = 378;
                }
                if ((client.ClientFlag & ServerFlag.Disconnect) == ServerFlag.Disconnect)
                {
                    if (client.Player.Map == 1036 || client.Player.Map == 1038 || client.Player.Map == 1076 || client.Player.Map == 1212 || client.Player.Map == 1211 || client.Player.Map == 1210 || client.Player.Map == 700 || client.Player.Map == 1121 || client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 1080 || client.Player.Map == 1508 || client.Player.Map == 1768 || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 1509 || client.Player.Map == 1508 || client.Player.Map == 1507 || client.Player.Map == 1780 || client.Player.Map == 1068)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 428;
                        client.Player.Y = 378;
                    }


                    if (client.Player.Map == 1038 && MsgSchedules.GuildWar.Proces == ProcesType.Alive && client.Player.HitPoints == 0)
                    {
                        client.Player.Map = 6001;
                        client.Player.X = 28;
                        client.Player.Y = 72;
                    }
                    else if (client.Player.Map == 1038 && MsgSchedules.GuildWar.Proces == ProcesType.Alive)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 428;
                        client.Player.Y = 378;
                    }
                    //if (client.Player.Map == 9991)
                    //{
                    //    client.Player.Map = 1002;
                    //    client.Player.X = 428;
                    //    client.Player.Y = 378;
                    //    if (Program.MacJoin.ContainsKey(client.OnLogin.MacAddress))
                    //    {
                    //        Program.MacJoin.Remove(client.OnLogin.MacAddress);
                    //    }
                    //}

                }
                if (!client.FullLoading)
                    return;
                //write.Write("Character", "MacAddress", client.OnLogin.MacAddress);
                write.Write("Character", "UID", client.Player.UID);
                write.Write("Character", "Body", client.Player.Body);
                write.Write("Character", "Face", client.Player.Face);
                write.WriteString("Character", "Name", client.Player.Name);
                write.WriteString("Character", "Spouse", client.Player.Spouse);
                write.Write("Character", "Class", client.Player.Class);
                write.Write("Character", "FirstClass", client.Player.FirstClass);
                write.Write("Character", "SecoundeClass", client.Player.SecondClass);
                write.Write("Character", "Avatar", client.Player.Avatar);
                write.Write("Character", "Map", client.Player.Map);
                write.Write("Character", "X", client.Player.X);
                write.Write("Character", "Y", client.Player.Y);
                write.Write("Character", "PMap", client.Player.PMap);
                write.Write("Character", "PMapX", client.Player.PMapX);
                write.Write("Character", "PMapY", client.Player.PMapY);
                write.Write("Character", "Agility", client.Player.Agility);
                write.Write("Character", "Strength", client.Player.Strength);
                write.Write("Character", "Vitaliti", client.Player.Vitality);
                write.Write("Character", "Spirit", client.Player.Spirit);
                write.Write("Character", "Atributes", client.Player.Atributes);
                write.Write("Character", "Reborn", client.Player.Reborn);
                write.Write("Character", "Level", client.Player.Level);
                write.Write("Character", "Haire", client.Player.Hair);
                write.Write("Character", "Experience", client.Player.Experience);
                write.Write("Character", "MinHitPoints", client.Player.HitPoints);
                write.Write("Character", "MinMana", client.Player.Mana);
                write.Write("Character", "ConquerPoints", client.Player.ConquerPoints);
                write.Write("Character", "Money", (long)client.Player.Money);
                write.Write<uint>("Character", "KeyBoxTRY", client.Player.KeyBoxTRY);
                write.Write<uint>("Character", "lettersTRY", client.Player.lettersTRY);
                write.Write<uint>("Character", "LavaTRY", client.Player.LavaTRY);
                write.Write<uint>("Character", "MiningAttempts", client.Player.MiningAttempts);
                write.Write("Character", "VirtutePoints", client.Player.VirtutePoints);
                write.Write("Character", "PkPoints", client.Player.PKPoints);
                write.Write("Character", "JailerUID", client.Player.JailerUID);
                write.Write("Character", "QuizPoints", client.Player.QuizPoints);
                write.Write("Character", "Enilghten", client.Player.Enilghten);
                write.Write("Character", "EnlightenReceive", client.Player.EnlightenReceive);
                write.Write("Character", "VipLevel", client.Player.VipLevel);
                write.Write("Character", "OldVIPLevel", client.Player.OldVIPLevel);
                write.Write("Character", "VipTime", client.Player.ExpireVip.Ticks);
                write.Write("Character", "VendorTime", client.Player.VendorTime.Ticks);
                write.Write("Character", "MeteorSocket", client.Player.MeteorSocket);
                write.Write("Character", "DragonBallSocket", client.Player.DragonBallSocket);
                write.Write("Character", "LastDragonPill", client.Player.LastDragonPill.Ticks);
                write.Write("Character", "LastSwordSoul", client.Player.LastSwordSoul.Ticks);
                write.Write("Character", "WHMoney", client.Player.WHMoney);
                write.Write("Character", "BlessTime", client.Player.BlessTime);
                write.Write("Character", "SpouseUID", client.Player.SpouseUID);
                write.Write("Character", "HeavenBlessing", client.Player.HeavenBlessing);
                write.Write("Character", "LostTimeBlessing", client.Player.HeavenBlessTime.Value);
                write.Write("Character", "HuntingBlessing", client.Player.HuntingBlessing);
                write.Write("Character", "JoinOnflineTG", client.Player.JoinOnflineTG.Ticks);
                write.WriteString("Character", "DemonEx", client.DemonExterminator.ToString());
                write.Write("Character", "Day", client.Player.Day);
                write.Write("Character", "BDExp", client.Player.BDExp);
                write.Write("Character", "RateExp", client.Player.RateExp);
                write.Write<uint>("Character", "Quest2rbS2Point", client.Player.Quest2rbS2Point);
                write.Write<byte>("Character", "Quest2rbStage", client.Player.Quest2rbStage);
                write.Write<byte>("Character", "Quest2rbBossesOrderby", client.Player.Quest2rbBossesOrderby);
                write.Write("Character", "DExpTime", client.Player.DExpTime);
                write.Write("Character", "ExpBallUsed", client.Player.ExpBallUsed);
                write.WriteString("Character", "SubProfInfo", client.Player.SubClass.ToString());
                write.WriteString("Character", "Flowers", client.Player.Flowers.ToString());
                write.Write("Character", "DonationNobility", client.Player.Nobility.Donation);
                write.Write<byte>("Character", "TCT", (byte)client.Player.TCCaptainTimes);
                write.Write("Character", "GuildID", client.Player.GuildID);
                write.Write("Character", "GuildRank", (ushort)client.Player.GuildRank);
                if (client.Player.MyGuildMember != null)
                {
                    client.Player.MyGuildMember.LastLogin = DateTime.Now.Ticks;
                    write.Write("Character", "CpsDonate", client.Player.MyGuildMember.CpsDonate);
                    write.Write("Character", "MoneyDonate", client.Player.MyGuildMember.MoneyDonate);
                    write.Write("Character", "PkDonation", client.Player.MyGuildMember.PkDonation);
                    write.Write("Character", "LastLogin", client.Player.MyGuildMember.LastLogin);
                    write.Write("Character", "CTF_Exploits", client.Player.MyGuildMember.CTF_Exploits);
                    write.Write("Character", "CTF_RCPS", client.Player.MyGuildMember.RewardConquerPoints);
                    write.Write("Character", "CTF_RM", client.Player.MyGuildMember.RewardMoney);
                    write.Write("Character", "CTF_R", client.Player.MyGuildMember.CTF_Claimed);
                }
                if (client.Player.MyClan != null)
                {
                    write.Write("Character", "ClanID", client.Player.MyClan.ID);
                    write.Write("Character", "ClanRank", client.Player.ClanRank);
                    if (client.Player.MyClanMember != null)
                        write.Write("Character", "ClanDonation", client.Player.MyClanMember.Donation);
                }
                write.Write("Character", "FRL", client.Player.FirstRebornLevel);
                write.Write("Character", "SRL", client.Player.SecoundeRebornLevel);
                write.Write("Character", "Reincanation", client.Player.Reincarnation);
                write.Write("Character", "LotteryEntries", client.Player.LotteryEntries);
                write.Write("Character", "Cursed", client.Player.CursedTimer);
                write.Write("Character", "AparenceType", client.Player.AparenceType);
                write.WriteString("Character", "SecurityPass", GetSecurityPassword(client));
                write.Write("Character", "ChampionPoints", client.Player.ChampionPoints);
                write.Write("Character", "NameEditCount", client.Player.NameEditCount);
                write.Write("Character", "InventorySashCount", client.Player.InventorySashCount);
                write.Write("Character", "CountryID", client.Player.CountryID);
                write.Write("Character", "BanCount", (uint)client.BanCount);
                write.Write("Character", "ExtraAtributes", client.Player.ExtraAtributes);
                write.Write("Character", "GiveFlowersToPerformer", client.Player.GiveFlowersToPerformer);
                write.Write("Character", "OnlinePoints", client.Player.OnlinePoints);
                write.Write("Character", "Agates", client.Player.AgatesString());
                write.Write("Character", "EmoneyPoints", client.Player.EmoneyPoints);
                write.Write("Character", "TournamentsPoints", client.Player.TournamentsPoints);
                write.Write("Character", "RoyalPassPoints", client.Player.RoyalPassPoints);
                //write.Write("Character", "NewbieProtection", (byte)client.Player.NewbieProtection);
                write.Write("Character", "ConquerLetter", client.Player.ConquerLetter);
                write.Write("Character", "LavaQuest", client.Player.LavaQuest);
                write.Write("Character", "LastLoginClient", client.Player.LastLoginClient.Ticks);
                write.Write("Character", "BossPoints", client.Player.BossPoints);
                write.WriteString("Character", "SpecialTitles", GetSpecialTitles(client));
                write.Write("Character", "OfflineTraining", (byte)client.Player.OfflineTraining);
                write.Write("Character", "ArenaRankingCP", client.Player.ArenaRankingCP);
                write.Write<bool>("Character", "GuildBeastClaimd", client.Player.GuildBeastClaimd);
                write.Write<bool>("Character", "SpawnGuildBeast", client.Player.SpawnGuildBeast);
                #region Temp VIPbank
                write.Write<uint>("Character", "VIPMetScrolls", client.Player.VIPMetScrolls);
                write.Write<uint>("Character", "VIPDBscrolls", client.Player.VIPDBscrolls);
                write.Write<uint>("Character", "VIPStone", client.Player.VIPStone);

                write.Write<uint>("Character", "VIPNPhoenixGem", client.Player.VIPNPhoenixGem);
                write.Write<uint>("Character", "VIPRPhoenixGem", client.Player.VIPRPhoenixGem);
                write.Write<uint>("Character", "VIPSPhoenixGem", client.Player.VIPSPhoenixGem);

                write.Write<uint>("Character", "VIPNDragonGem", client.Player.VIPNDragonGem);
                write.Write<uint>("Character", "VIPRDragonGem", client.Player.VIPRDragonGem);
                write.Write<uint>("Character", "VIPSDragonGem", client.Player.VIPSDragonGem);

                write.Write<uint>("Character", "VIPNFuryGem", client.Player.VIPNFuryGem);
                write.Write<uint>("Character", "VIPRFuryGem", client.Player.VIPRFuryGem);
                write.Write<uint>("Character", "VIPSFuryGem", client.Player.VIPSFuryGem);

                write.Write<uint>("Character", "VIPNRainbowGem", client.Player.VIPNRainbowGem);
                write.Write<uint>("Character", "VIPRRainbowGem", client.Player.VIPRRainbowGem);
                write.Write<uint>("Character", "VIPSRainbowGem", client.Player.VIPSRainbowGem);

                write.Write<uint>("Character", "VIPNKylinGem", client.Player.VIPNKylinGem);
                write.Write<uint>("Character", "VIPRKylinGem", client.Player.VIPRKylinGem);
                write.Write<uint>("Character", "VIPSKylinGem", client.Player.VIPSKylinGem);

                write.Write<uint>("Character", "VIPNVioletGem", client.Player.VIPNVioletGem);
                write.Write<uint>("Character", "VIPRVioletGem", client.Player.VIPRVioletGem);
                write.Write<uint>("Character", "VIPSVioletGem", client.Player.VIPSVioletGem);

                write.Write<uint>("Character", "VIPNMoonGem", client.Player.VIPNMoonGem);
                write.Write<uint>("Character", "VIPRMoonGem", client.Player.VIPRMoonGem);
                write.Write<uint>("Character", "VIPSMoonGem", client.Player.VIPSMoonGem);

                write.Write<uint>("Character", "VIPNTortoiseGem", client.Player.VIPNTortoiseGem);
                write.Write<uint>("Character", "VIPRTortoiseGem", client.Player.VIPRTortoiseGem);
                write.Write<uint>("Character", "VIPSTortoiseGem", client.Player.VIPSTortoiseGem);

                #endregion
                #region Temp bank
                write.Write<uint>("Character", "MetScrolls", client.Player.MetScrolls);
                write.Write<uint>("Character", "DBscrolls", client.Player.DBscrolls);

                write.Write<uint>("Character", "NPhoenixGem", client.Player.NPhoenixGem);
                write.Write<uint>("Character", "RPhoenixGem", client.Player.RPhoenixGem);
                write.Write<uint>("Character", "SPhoenixGem", client.Player.SPhoenixGem);

                write.Write<uint>("Character", "NDragonGem", client.Player.NDragonGem);
                write.Write<uint>("Character", "RDragonGem", client.Player.RDragonGem);
                write.Write<uint>("Character", "SDragonGem", client.Player.SDragonGem);

                write.Write<uint>("Character", "NFuryGem", client.Player.NFuryGem);
                write.Write<uint>("Character", "RFuryGem", client.Player.RFuryGem);
                write.Write<uint>("Character", "SFuryGem", client.Player.SFuryGem);

                write.Write<uint>("Character", "NRainbowGem", client.Player.NRainbowGem);
                write.Write<uint>("Character", "RRainbowGem", client.Player.RRainbowGem);
                write.Write<uint>("Character", "SRainbowGem", client.Player.SRainbowGem);

                write.Write<uint>("Character", "NKylinGem", client.Player.NKylinGem);
                write.Write<uint>("Character", "RKylinGem", client.Player.RKylinGem);
                write.Write<uint>("Character", "SKylinGem", client.Player.SKylinGem);

                write.Write<uint>("Character", "NVioletGem", client.Player.NVioletGem);
                write.Write<uint>("Character", "RVioletGem", client.Player.RVioletGem);
                write.Write<uint>("Character", "SVioletGem", client.Player.SVioletGem);

                write.Write<uint>("Character", "NMoonGem", client.Player.NMoonGem);
                write.Write<uint>("Character", "RMoonGem", client.Player.RMoonGem);
                write.Write<uint>("Character", "SMoonGem", client.Player.SMoonGem);

                write.Write<uint>("Character", "NTortoiseGem", client.Player.NTortoiseGem);
                write.Write<uint>("Character", "RTortoiseGem", client.Player.RTortoiseGem);
                write.Write<uint>("Character", "STortoiseGem", client.Player.STortoiseGem);

                #endregion
                write.Write<bool>("Character", "IsBannedChat", client.Player.IsBannedChat);
                write.Write<bool>("Character", "PermenantBannedChat", client.Player.PermenantBannedChat);
                write.Write<long>("Character", "BannedChatStamp", client.Player.BannedChatStamp.ToBinary());
                write.Write<uint>("Character", "HitShoot", client.Player.HitShoot);
                write.Write<uint>("Character", "MisShoot", client.Player.MisShoot);
                write.Write<uint>("Character", "ArenaDeads", client.Player.ArenaDeads);
                write.Write<uint>("Character", "ArenaKills", client.Player.ArenaKills);
                write.Write<ushort>("Character", "PumpkinPoints", client.Player.PumpkinPoints);
                write.Write<ushort>("Character", "CountVote", (ushort)client.Player.CountVote);
                write.Write<int>("Character", "VotePoints", client.Player.VotePoints);
                write.Write<int>("Character", "VipClaimChance", client.Player.VipClaimChance);
                #region ranks
                write.Write<int>("Character", "KingRank", client.Player.KingRank);

                #endregion
                SaveClientItems(client);
                SaveClientSpells(client);
                SaveClientProfs(client);
                DbKillMobsExterminator.Save(client);
                OnlinePointsManager.Save(client);
                RoyalPassManager.Save(client);
                TournamentsManager.Save(client);
                LimitedDailyTimes.Save(client);
                DbDailyTraining.Save(client);
                RoleQuests.Save(client);
                House.Save(client);
                //using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
                //{
                //    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("insert into characters (UID,Name,Level,Experience,Spouse,Body,Face,Hair,Silvers,WHSilvers,CPs,GuildID,GuildName,Map,X,Y,Job,PreviousJob1,PreviousJob2,Strength,Agility,Vitality,Spirit,ExtraStats,Life,Mana,VirtuePoints,WHPassword,VIPLevel,ExpireVip,BossPoints,OnlineTime,Nobility,PKPoints) " +
                //        "values (@UID,@name,@Level,@Experience,@Spouse,@Body,@Face,@Hair,@Silvers,@WHSilvers,@CPs,@GuildID,@GuildName,@Map,@X,@Y,@Job,@PreviousJob1,@PreviousJob2,@Strength,@Agility,@Vitality,@Spirit,@ExtraStats,@Life,@Mana,@VirtuePoints,@WHPassword,@VIPLevel,@ExpireVip,@BossPoints,@OnlineTime,@Nobility,@PKPoints)"
                //        , conn))
                //    {
                //        conn.Open();
                //        #region ClearCommand
                //        try
                //        {
                //            using (var cmdd = new MySql.Data.MySqlClient.MySqlCommand("Delete from characters where UID=" + client.Player.UID + "", conn))
                //                cmdd.ExecuteNonQuery();

                //        }
                //        catch (Exception e)
                //        {
                //            Console.WriteLine(e.ToString());
                //        }
                //        #endregion

                //        cmd.Parameters.AddWithValue("@UID", client.Player.UID);
                //        cmd.Parameters.AddWithValue("@Name", client.Player.Name);
                //        cmd.Parameters.AddWithValue("@Level", client.Player.Level);
                //        cmd.Parameters.AddWithValue("@Experience", client.Player.Experience);
                //        cmd.Parameters.AddWithValue("@Spouse", client.Player.Spouse);
                //        cmd.Parameters.AddWithValue("@Body", client.Player.Body);//
                //        cmd.Parameters.AddWithValue("@Face", client.Player.Face);
                //        cmd.Parameters.AddWithValue("@Hair", client.Player.Hair);
                //        cmd.Parameters.AddWithValue("@Silvers", client.Player.Money);
                //        cmd.Parameters.AddWithValue("@WHSilvers", client.Player.WHMoney);
                //        cmd.Parameters.AddWithValue("@CPs", client.Player.ConquerPoints);
                //        cmd.Parameters.AddWithValue("@GuildID", client.Player.GuildID);
                //        if (client.Player.MyGuild != null)
                //            cmd.Parameters.AddWithValue("@GuildName", client.Player.MyGuild.GuildName);
                //        else
                //            cmd.Parameters.AddWithValue("@GuildName", "None");
                //        cmd.Parameters.AddWithValue("@Map", client.Player.Map);
                //        cmd.Parameters.AddWithValue("@X", client.Player.X);
                //        cmd.Parameters.AddWithValue("@Y", client.Player.Y);
                //        cmd.Parameters.AddWithValue("@Job", client.Player.Class);
                //        cmd.Parameters.AddWithValue("@PreviousJob1", client.Player.FirstClass);
                //        cmd.Parameters.AddWithValue("@PreviousJob2", client.Player.SecondClass);
                //        cmd.Parameters.AddWithValue("@Strength", client.Player.Strength);
                //        cmd.Parameters.AddWithValue("@Agility", client.Player.Agility);
                //        cmd.Parameters.AddWithValue("@Vitality", client.Player.Vitality);
                //        cmd.Parameters.AddWithValue("@Spirit", client.Player.Spirit);
                //        cmd.Parameters.AddWithValue("@ExtraStats", client.Player.Atributes);
                //        cmd.Parameters.AddWithValue("@Life", client.Player.HitPoints);
                //        cmd.Parameters.AddWithValue("@Mana", client.Player.Mana);
                //        cmd.Parameters.AddWithValue("@VirtuePoints", client.Player.VirtutePoints);
                //        cmd.Parameters.AddWithValue("@WHPassword", client.Player.SecurityPassword);
                //        cmd.Parameters.AddWithValue("@VIPLevel", client.Player.VipLevel);
                //        cmd.Parameters.AddWithValue("@ExpireVip", client.Player.ExpireVip);
                //        cmd.Parameters.AddWithValue("@BossPoints", client.Player.BossPoints);
                //        cmd.Parameters.AddWithValue("@OnlineTime", client.Player.OnlinePoints);
                //        cmd.Parameters.AddWithValue("@Nobility", client.Player.Nobility);
                //        cmd.Parameters.AddWithValue("@PKPoints", client.Player.PKPoints);//
                //        cmd.Parameters.AddWithValue("@CurrentHonor", client.HonorPoints);
                //        cmd.Parameters.AddWithValue("@TotalHonor", client.ArenaPoints);
                //        cmd.ExecuteNonQuery();

                //    }
                //}
                if ((client.ClientFlag & ServerFlag.Disconnect) == ServerFlag.Disconnect)
                    Server.GamePoll.TryRemove(client.Player.UID, out var _);
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static string GetSecurityPassword(GameClient user)
        {
            WriteLine writer;
            writer = new WriteLine(',');
            writer.Add(user.Player.SecurityPassword);
            writer.Add(user.Player.OnReset);
            writer.Add(user.Player.ResetSecurityPassowrd.Ticks);
            return writer.Close();
        }

        public static void LoadSecurityPassword(string line, GameClient user)
        {
            ReadLine reader;
            reader = new ReadLine(line, ',');
            user.Player.SecurityPassword = reader.Read((uint)0);
            user.Player.OnReset = reader.Read((uint)0);
            if (user.Player.OnReset == 1)
            {
                user.Player.ResetSecurityPassowrd = DateTime.FromBinary(reader.Read(0L));
                if (DateTime.Now > user.Player.ResetSecurityPassowrd)
                {
                    user.Player.OnReset = 0;
                    user.Player.SecurityPassword = 0;
                }
            }
        }

        public static void LoadCharacter(GameClient client, uint UID)
        {
            client.Player.UID = UID;
            IniFile reader;
            reader = new IniFile("\\Users\\" + UID + ".ini");
            client.Player.Body = reader.ReadUInt16("Character", "Body", 1002);
            client.Player.Face = reader.ReadUInt16("Character", "Face", 0);
            client.Player.Name = reader.ReadString("Character", "Name", "None");
            client.Player.Spouse = reader.ReadString("Character", "Spouse", "None");
            client.Player.Class = reader.ReadByte("Character", "Class", 0);
            client.Player.FirstClass = reader.ReadByte("Character", "FirstClass", 0);
            client.Player.SecondClass = reader.ReadByte("Character", "SecoundeClass", 0);
            client.Player.Avatar = reader.ReadUInt16("Character", "Avatar", 0);
            client.Player.Map = reader.ReadUInt32("Character", "Map", 1002);
            client.Player.X = reader.ReadUInt16("Character", "X", 248);
            client.Player.Y = reader.ReadUInt16("Character", "Y", 238);
            client.Player.MiningAttempts = reader.ReadUInt32("Character", "MiningAttempts", 0);
            client.Player.PMap = reader.ReadUInt32("Character", "PMap", 1002);
            client.Player.PMapX = reader.ReadUInt16("Character", "PMapX", 300);
            client.Player.PMapY = reader.ReadUInt16("Character", "PMapY", 300);
            if (client.EventBase != null)
            {
                client.EventBase.RemovePlayer(client, true);
                client.Player.Map = 1002;
                client.Player.X = 428;//428,378
                client.Player.Y = 378;
            }
            client.Player.Agility = reader.ReadUInt16("Character", "Agility", 0);
            client.Player.Strength = reader.ReadUInt16("Character", "Strength", 0);
            client.Player.Spirit = reader.ReadUInt16("Character", "Spirit", 0);
            client.Player.Vitality = reader.ReadUInt16("Character", "Vitaliti", 0);
            client.Player.Atributes = reader.ReadUInt16("Character", "Atributes", 0);
            client.Player.Reborn = reader.ReadByte("Character", "Reborn", 0);
            client.Player.Level = reader.ReadUInt16("Character", "Level", 0);
            client.Player.Hair = reader.ReadUInt16("Character", "Haire", 0);
            client.Player.Experience = (ulong)reader.ReadInt64("Character", "Experience", 0L);
            client.Player.HitPoints = reader.ReadInt32("Character", "MinHitPoints", 0);
            client.Player.Mana = reader.ReadUInt16("Character", "MinMana", 0);
            client.Player.ConquerPoints = (int)reader.ReadUInt32("Character", "ConquerPoints", 0);
            client.Player.Money = (int)reader.ReadUInt32("Character", "Money", 0);
            client.Player.VirtutePoints = reader.ReadUInt32("Character", "VirtutePoints", 0);
            client.Player.KeyBoxTRY = reader.ReadUInt32("Character", "KeyBoxTRY", 0);
            client.Player.lettersTRY = reader.ReadUInt32("Character", "lettersTRY", 0);
            client.Player.LavaTRY = reader.ReadUInt32("Character", "LavaTRY", 0);
            client.Player.MiningAttempts = reader.ReadUInt32("Character", "MiningAttempts", 0);
            client.Player.HitShoot = reader.ReadUInt32("Character", "HitShoot", 0);
            client.Player.MisShoot = reader.ReadUInt32("Character", "MisShoot", 0);
            client.Player.ArenaKills = reader.ReadUInt32("Character", "ArenaKills", 0);
            client.Player.ArenaDeads = reader.ReadUInt32("Character", "ArenaDeads", 0);
            client.Player.PKPoints = reader.ReadUInt16("Character", "PkPoints", 0);
            client.Player.JailerUID = reader.ReadUInt32("Character", "JailerUID", 0);
            client.Player.QuizPoints = reader.ReadUInt32("Character", "QuizPoints", 0);
            client.Player.Enilghten = reader.ReadUInt16("Character", "Enilghten", 0);
            client.Player.EnlightenReceive = reader.ReadUInt16("Character", "EnlightenReceive", 0);
            client.Player.VipLevel = reader.ReadByte("Character", "VipLevel", 1);
            client.Player.OldVIPLevel = reader.ReadByte("Character", "OldVIPLevel", 1);
            client.Player.Quest2rbS2Point = reader.ReadUInt32("Character", "Quest2rbS2Point", 0);
            client.Player.Quest2rbStage = reader.ReadByte("Character", "Quest2rbStage", 0);
            client.Player.Quest2rbBossesOrderby = reader.ReadByte("Character", "Quest2rbBossesOrderby", 0);
            client.Player.ExpireVip = DateTime.FromBinary(reader.ReadInt64("Character", "VipTime", 0L));
            client.Player.VendorTime = DateTime.FromBinary(reader.ReadInt64("Character", "VendorTime", 0L));
            client.Player.MeteorSocket = reader.ReadUInt32("Character", "MeteorSocket", 0);
            client.Player.DragonBallSocket = reader.ReadUInt32("Character", "DragonBallSocket", 0);
            client.Player.LastDragonPill = DateTime.FromBinary(reader.ReadInt64("Character", "LastDragonPill", 0L));
            client.Player.LastSwordSoul = DateTime.FromBinary(reader.ReadInt64("Character", "LastSwordSoul", 0L));
            List<VIPExperience.Client> VIPPoll;
            VIPPoll = (from p in VIPExperience.VIPExperiencePoll.GetValues()
                       where p.StudentUID == UID
                       select p).ToList();
            if (VIPPoll.Count > 0)
            {
                VIPExperience.Client Student;
                Student = VIPPoll.FirstOrDefault();
                if (Student != null)
                {
                    client.Player.VipLevel = Student.ExperienceLevShare;
                    client.Player.ExpireVip = Student.ShareEnds;
                }
            }
            if (DateTime.Now > client.Player.ExpireVip && client.Player.VipLevel > 6)
            {
                client.Player.VipLevel = client.Player.OldVIPLevel;
                client.Player.OldVIPLevel = client.Player.VipLevel;
                if (client.Player.ContainFlag(MsgUpdate.Flags.VIP))
                    client.Player.RemoveFlag(MsgUpdate.Flags.VIP);
            }
            client.Player.TCCaptainTimes = reader.ReadByte("Character", "TCT", 0);
            client.Player.WHMoney = reader.ReadInt64("Character", "WHMoney", 0L);
            client.Player.BlessTime = reader.ReadUInt32("Character", "BlessTime", 0);
            client.Player.SpouseUID = reader.ReadUInt32("Character", "SpouseUID", 0);
            client.Player.HeavenBlessing = reader.ReadInt32("Character", "HeavenBlessing", 0);
            client.Player.HeavenBlessTime = new Time32(reader.ReadUInt32("Character", "LostTimeBlessing", 0));
            client.Player.HuntingBlessing = reader.ReadUInt32("Character", "HuntingBlessing", 0);
            client.Player.JoinOnflineTG = DateTime.FromBinary(reader.ReadInt64("Character", "JoinOnflineTG", 0L));
            client.Player.RateExp = reader.ReadUInt32("Character", "RateExp", 0);
            client.Player.DExpTime = reader.ReadUInt32("Character", "DExpTime", 0);
            client.Player.Day = reader.ReadInt32("Character", "Day", 0);
            client.Player.BDExp = reader.ReadByte("Character", "BDExp", 0);
            client.DemonExterminator.ReadLine(reader.ReadString("Character", "DemonEx", "0/0/"));
            client.Player.ExpBallUsed = reader.ReadByte("Character", "ExpBallUsed", 0);
            client.Player.PermenantBannedChat = reader.ReadBool("Character", "PermenantBannedChat", false);
            client.Player.IsBannedChat = reader.ReadBool("Character", "IsBannedChat", false);
            client.Player.BannedChatStamp = DateTime.FromBinary(reader.ReadInt64("Character", "BannedChatStamp", 0));
            DataCore.LoadClient(client.Player);
            client.Player.GuildID = reader.ReadUInt32("Character", "GuildID", 0);
            client.Player.GuildRank = (Flags.GuildMemberRank)reader.ReadUInt32("Character", "GuildRank", 200);
            if (client.Player.GuildID != 0)
            {
                if (Guild.GuildPoll.TryGetValue(client.Player.GuildID, out var myguild))
                {
                    client.Player.MyGuild = myguild;
                    if (myguild.Members.TryGetValue(client.Player.UID, out var member3))
                    {
                        member3.IsOnline = true;
                        client.Player.GuildID = (ushort)myguild.Info.GuildID;
                        client.Player.MyGuildMember = member3;
                        client.Player.GuildRank = member3.Rank;
                        client.Player.GuildBattlePower = myguild.ShareMemberPotency(member3.Rank);
                    }
                    else
                    {
                        client.Player.MyGuild = null;
                        client.Player.GuildID = 0;
                        client.Player.GuildRank = Flags.GuildMemberRank.None;
                    }
                }
                else
                {
                    client.Player.MyGuild = null;
                    client.Player.GuildID = 0;
                    client.Player.GuildRank = Flags.GuildMemberRank.None;
                }
            }
            client.Player.SubClass = new SubClass();
            client.Player.SubClass.Load(reader.ReadString("Character", "SubProfInfo", ""));
            client.Player.SubClass.CreateSpawn(client);
            if (Flowers.ClientPoll.ContainsKey(UID))
                client.Player.Flowers = Flowers.ClientPoll[UID];
            else
                client.Player.Flowers = new Flowers(UID, client.Player.Name);
            string flowerStr;
            flowerStr = reader.ReadString("Character", "Flowers", "");
            ReadLine Linereader;
            Linereader = new ReadLine(flowerStr, '/');
            client.Player.Flowers.FreeFlowers = Linereader.Read((uint)0);
            if (Program.NobilityRanking.TryGetValue(UID, out var nobility))
            {
                client.Player.Nobility = nobility;
                client.Player.NobilityRank = client.Player.Nobility.Rank;
            }
            else
            {
                client.Player.Nobility = new Nobility(client)
                {
                    Donation = reader.ReadUInt64("Character", "DonationNobility", 0uL)
                };
                client.Player.NobilityRank = client.Player.Nobility.Rank;
            }
            if (TheChosenProject.Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out var Associate))
            {
                client.Player.Associate = Associate;
                client.Player.Associate.MyClient = client;
                client.Player.Associate.Online = true;
                if (client.Player.Associate.Associat.ContainsKey(4))
                {
                    foreach (Associate.Member member2 in client.Player.Associate.Associat[4].Values)
                    {
                        if (member2.UID != 0 && TheChosenProject.Role.Instance.Associate.Associates.TryGetValue(member2.UID, out var mentor))
                        {
                            client.Player.MyMentor = mentor;
                            break;
                        }
                    }
                }
            }
            else
                client.Player.Associate = new Associate.MyAsociats(client.Player.UID)
                {
                    MyClient = client,
                    Online = true
                };
            client.Player.ClanUID = reader.ReadUInt32("Character", "ClanID", 0);
            if (client.Player.ClanUID != 0)
            {
                if (Clan.Clans.TryGetValue(client.Player.ClanUID, out var myclan))
                {
                    client.Player.MyClan = myclan;
                    if (myclan.Members.TryGetValue(client.Player.UID, out var member))
                    {
                        member.Online = true;
                        client.Player.ClanName = myclan.Name;
                        client.Player.MyClanMember = member;
                        client.Player.ClanRank = (ushort)member.Rank;
                    }
                    else
                    {
                        client.Player.MyClan = null;
                        client.Player.ClanUID = 0;
                        client.Player.ClanRank = 0;
                    }
                }
                else
                    client.Player.ClanUID = 0;
            }
            client.Player.FirstRebornLevel = reader.ReadByte("Character", "FRL", 0);
            client.Player.SecoundeRebornLevel = reader.ReadByte("Character", "SRL", 0);
            client.Player.Reincarnation = reader.ReadBool("Character", "Reincanation", false);
            client.Player.LotteryEntries = reader.ReadByte("Character", "LotteryEntries", 0);
            client.Player.CursedTimer = reader.ReadInt32("Character", "Cursed", 0);
            client.Player.AparenceType = reader.ReadUInt32("Character", "AparenceType", 0);
            LoadSecurityPassword(reader.ReadString("Character", "SecurityPass", "0,0,0"), client);
            LoadSpecialTitles(client, reader.ReadString("Character", "SpecialTitles", "0,0,"));
            client.Player.ChampionPoints = (int)reader.ReadUInt32("Character", "ChampionPoints", 0);
            client.Player.NameEditCount = reader.ReadUInt16("Character", "NameEditCount", 0);
            client.Player.CountryID = reader.ReadUInt16("Character", "CountryID", 0);
            client.Player.InventorySashCount = reader.ReadUInt16("Character", "InventorySashCount", 0);
            client.BanCount = reader.ReadByte("Character", "BanCount", 0);
            client.Player.ExtraAtributes = reader.ReadUInt16("Character", "ExtraAtributes", 0);
            client.Player.GiveFlowersToPerformer = reader.ReadInt32("Character", "GiveFlowersToPerformer", 0);
            client.Player.OnlinePoints = reader.ReadInt32("Character", "OnlinePoints", 0);
            client.Player.EmoneyPoints = reader.ReadInt32("Character", "EmoneyPoints", 0);
            client.Player.TournamentsPoints = reader.ReadInt32("Character", "TournamentsPoints", 0);
            client.Player.RoyalPassPoints = reader.ReadInt32("Character", "RoyalPassPoints", 0);
            client.Player.CountVote = (uint)reader.ReadInt32("Character", "CountVote", 0);
            client.Player.VotePoints = (int)reader.ReadUInt32("Character", "VotePoints", 0);
            client.Player.VipClaimChance = (int)reader.ReadUInt32("Character", "VipClaimChance", 0);
            //client.Player.NewbieProtection = (Flags.NewbieExperience)reader.ReadByte("Character", "NewbieProtection", 0);
            client.Player.ConquerLetter = reader.ReadByte("Character", "ConquerLetter", 0);
            client.Player.LavaQuest = reader.ReadByte("Character", "LavaQuest", 0);
            client.Player.LastLoginClient = DateTime.FromBinary(reader.ReadInt64("Character", "LastLoginClient", 0L));
            client.Player.BossPoints = reader.ReadInt32("Character", "BossPoints", 0);
            client.Player.OfflineTraining = (MsgOfflineTraining.Mode)reader.ReadByte("Character", "OfflineTraining", 0);
            LoadClientItems(client);
            client.Player.LoadAgates(reader.ReadString("Character", "Agates", ""));
            client.Player.ArenaRankingCP = reader.ReadUInt32("Character", "ArenaRankingCP", 0);
            client.Player.GuildBeastClaimd = reader.ReadBool("Character", "GuildBeastClaimd", false);
            client.Player.SpawnGuildBeast = reader.ReadBool("Character", "SpawnGuildBeast", false);
            #region Temp bank
            client.Player.MetScrolls = reader.ReadUInt32("Character", "MetScrolls", 0);
            client.Player.DBscrolls = reader.ReadUInt32("Character", "DBscrolls", 0);

            client.Player.NPhoenixGem = reader.ReadUInt32("Character", "NPhoenixGem", 0);
            client.Player.RPhoenixGem = reader.ReadUInt32("Character", "RPhoenixGem", 0);
            client.Player.SPhoenixGem = reader.ReadUInt32("Character", "SPhoenixGem", 0);

            client.Player.NDragonGem = reader.ReadUInt32("Character", "NDragonGem", 0);
            client.Player.RDragonGem = reader.ReadUInt32("Character", "RDragonGem", 0);
            client.Player.SDragonGem = reader.ReadUInt32("Character", "SDragonGem", 0);

            client.Player.NFuryGem = reader.ReadUInt32("Character", "NFuryGem", 0);
            client.Player.RFuryGem = reader.ReadUInt32("Character", "RFuryGem", 0);
            client.Player.SFuryGem = reader.ReadUInt32("Character", "SFuryGem", 0);

            client.Player.NRainbowGem = reader.ReadUInt32("Character", "NRainbowGem", 0);
            client.Player.RRainbowGem = reader.ReadUInt32("Character", "RRainbowGem", 0);
            client.Player.SRainbowGem = reader.ReadUInt32("Character", "SRainbowGem", 0);

            client.Player.NKylinGem = reader.ReadUInt32("Character", "NKylinGem", 0);
            client.Player.RKylinGem = reader.ReadUInt32("Character", "RKylinGem", 0);
            client.Player.SKylinGem = reader.ReadUInt32("Character", "SKylinGem", 0);

            client.Player.NVioletGem = reader.ReadUInt32("Character", "NVioletGem", 0);
            client.Player.RVioletGem = reader.ReadUInt32("Character", "RVioletGem", 0);
            client.Player.SVioletGem = reader.ReadUInt32("Character", "SVioletGem", 0);

            client.Player.NMoonGem = reader.ReadUInt32("Character", "NMoonGem", 0);
            client.Player.RMoonGem = reader.ReadUInt32("Character", "RMoonGem", 0);
            client.Player.SMoonGem = reader.ReadUInt32("Character", "SMoonGem", 0);

            client.Player.NTortoiseGem = reader.ReadUInt32("Character", "NTortoiseGem", 0);
            client.Player.RTortoiseGem = reader.ReadUInt32("Character", "RTortoiseGem", 0);
            client.Player.STortoiseGem = reader.ReadUInt32("Character", "STortoiseGem", 0);
            #endregion
            #region Temp VIPbank
            client.Player.VIPMetScrolls = reader.ReadUInt32("Character", "VIPMetScrolls", 0);
            client.Player.VIPDBscrolls = reader.ReadUInt32("Character", "VIPDBscrolls", 0);
            client.Player.VIPStone = reader.ReadUInt32("Character", "VIPStone", 0);

            client.Player.VIPNPhoenixGem = reader.ReadUInt32("Character", "VIPNPhoenixGem", 0);
            client.Player.VIPRPhoenixGem = reader.ReadUInt32("Character", "VIPRPhoenixGem", 0);
            client.Player.VIPSPhoenixGem = reader.ReadUInt32("Character", "VIPSPhoenixGem", 0);

            client.Player.VIPNDragonGem = reader.ReadUInt32("Character", "VIPNDragonGem", 0);
            client.Player.VIPRDragonGem = reader.ReadUInt32("Character", "VIPRDragonGem", 0);
            client.Player.VIPSDragonGem = reader.ReadUInt32("Character", "VIPSDragonGem", 0);

            client.Player.VIPNFuryGem = reader.ReadUInt32("Character", "VIPNFuryGem", 0);
            client.Player.VIPRFuryGem = reader.ReadUInt32("Character", "VIPRFuryGem", 0);
            client.Player.VIPSFuryGem = reader.ReadUInt32("Character", "VIPSFuryGem", 0);

            client.Player.VIPNRainbowGem = reader.ReadUInt32("Character", "VIPNRainbowGem", 0);
            client.Player.VIPRRainbowGem = reader.ReadUInt32("Character", "VIPRRainbowGem", 0);
            client.Player.VIPSRainbowGem = reader.ReadUInt32("Character", "VIPSRainbowGem", 0);

            client.Player.VIPNKylinGem = reader.ReadUInt32("Character", "VIPNKylinGem", 0);
            client.Player.VIPRKylinGem = reader.ReadUInt32("Character", "VIPRKylinGem", 0);
            client.Player.VIPSKylinGem = reader.ReadUInt32("Character", "VIPSKylinGem", 0);

            client.Player.VIPNVioletGem = reader.ReadUInt32("Character", "VIPNVioletGem", 0);
            client.Player.VIPRVioletGem = reader.ReadUInt32("Character", "VIPRVioletGem", 0);
            client.Player.VIPSVioletGem = reader.ReadUInt32("Character", "VIPSVioletGem", 0);

            client.Player.VIPNMoonGem = reader.ReadUInt32("Character", "VIPNMoonGem", 0);
            client.Player.VIPRMoonGem = reader.ReadUInt32("Character", "VIPRMoonGem", 0);
            client.Player.VIPSMoonGem = reader.ReadUInt32("Character", "VIPSMoonGem", 0);

            client.Player.VIPNTortoiseGem = reader.ReadUInt32("Character", "VIPNTortoiseGem", 0);
            client.Player.VIPRTortoiseGem = reader.ReadUInt32("Character", "VIPRTortoiseGem", 0);
            client.Player.VIPSTortoiseGem = reader.ReadUInt32("Character", "VIPSTortoiseGem", 0);
            #endregion
            client.Player.PumpkinPoints = reader.ReadUInt16("Character", "PumpkinPoints", 0);
            #region ranks
            client.Player.KingRank = reader.ReadUInt16("Character", "KingRank", 0);

            #endregion
            LoadClientSpells(client);
            LoadClientProfs(client);
            LimitedDailyTimes.Load(client);
            OnlinePointsManager.Load(client);
            RoyalPassManager.Load(client);
            TournamentsManager.Load(client);
            DbKillMobsExterminator.Load(client);
            DbDailyTraining.Load(client);
            RoleQuests.Load(client);
            House.Load(client);
            //client.MaxDBMobs = Core.Random.Next(400, 700);
            ResetingEveryDay(client);
            if (Server.QueueContainer.PollContainers.TryGetValue(client.Player.UID, out var Container))
                client.Confiscator = Container;
            try
            {
                client.Player.Associate.OnLoading(client);
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
            if (MsgArena.ArenaPoll.TryGetValue(client.Player.UID, out client.ArenaStatistic))
                client.ArenaStatistic.ApplayInfo(client.Player);
            else
            {
                client.ArenaStatistic = new MsgArena.User();
                client.ArenaStatistic.ApplayInfo(client.Player);
                client.ArenaStatistic.Info.ArenaPoints = 4000;
                MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);
            }
            if (MsgTeamArena.ArenaPoll.TryGetValue(client.Player.UID, out client.TeamArenaStatistic))
                client.TeamArenaStatistic.ApplayInfo(client.Player);
            else
            {
                client.TeamArenaStatistic = new MsgTeamArena.User();
                client.TeamArenaStatistic.ApplayInfo(client.Player);
                client.TeamArenaStatistic.Info.ArenaPoints = 4000;
                MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);
            }
            client.FullLoading = true;
        }

        public unsafe static void LoadClientItems(GameClient client)
        {
            BinaryFile binary;
            binary = new BinaryFile();
            if (!binary.Open(ServerKernel.CO2FOLDER + "\\PlayersItems\\" + client.Player.UID + ".bin", FileMode.Open))
                return;
            int ItemCount = default(int);
            binary.Read(&ItemCount, 4);
            ClientItems.DBItem Item = default(ClientItems.DBItem);
            for (int x = 0; x < ItemCount; x++)
            {
                binary.Read(&Item, sizeof(ClientItems.DBItem));
                if (Item.ITEM_ID == 750000)//demonExterminator jar
                    client.DemonExterminator.ItemUID = Item.UID;
                MsgGameItem ClienItem;
                ClienItem = Item.GetDataItem();
                if (!ClienItem.Fake)
                {
                    if (ItemType.ItemPosition(Item.ITEM_ID) != 18 && Item.Bless > 1)
                        Item.Bless = 1;
                    //if (Item.ITEM_ID == 720598 || (Item.ITEM_ID >= 2100065 && Item.ITEM_ID <= 2100105))
                    //    ClienItem.Bound = 1;
                    
                    if (Item.WH_ID != 0)
                    {
                        if (Item.WH_ID == 100)
                        {
                            if (Item.Position > 0 && Item.Position <= 29)
                                client.Equipment.ClientItems.TryAdd(Item.UID, ClienItem);
                            continue;
                        }
                        if (!client.Warehouse.ClientItems.ContainsKey(Item.WH_ID))
                            client.Warehouse.ClientItems.TryAdd(Item.WH_ID, new ConcurrentDictionary<uint, MsgGameItem>());
                        if (client.Player.GuildID == 0)
                            ClienItem.Inscribed = 0;
                        client.Warehouse.ClientItems[Item.WH_ID].TryAdd(Item.UID, ClienItem);
                    }
                    else if (Item.Position > 0 && Item.Position <= 29)
                    {
                        if (client.Player.GuildID == 0)
                            ClienItem.Inscribed = 0;
                        client.Equipment.ClientItems.TryAdd(Item.UID, ClienItem);
                    }
                    else if (Item.Position == 0)
                    {
                        if (client.Player.GuildID == 0)
                            ClienItem.Inscribed = 0;
                        client.Inventory.AddDBItem(ClienItem);
                    }
                }
            }
            binary.Read(&ItemCount, 4);
            binary.Close();
        }

        public unsafe static void SaveClientItems(GameClient client)
        {
            BinaryFile binary;
            binary = new BinaryFile();
            if (!binary.Open(ServerKernel.CO2FOLDER + "\\PlayersItems\\" + client.Player.UID + ".bin", FileMode.Create))
                return;
            ClientItems.DBItem DBItem;
            DBItem = default(ClientItems.DBItem);
            int ItemCount;
            ItemCount = client.GetItemsCount();
            binary.Write(&ItemCount, 4);
            foreach (MsgGameItem item in client.AllMyItems())
            {
                if (!item.Fake)
                {
                    if (item.Position == 0 && item.WH_ID == 0)
                    {
                        ushort pos;
                        pos = ItemType.ItemPosition(item.ITEM_ID);
                        _ = 9;
                    }
                    DBItem.GetDBItem(item);
                    if (!binary.Write(&DBItem, sizeof(ClientItems.DBItem)))
                        ServerKernel.Log.SaveLog("Could not load DBItem  ", true, LogType.WARNING);
                }
            }
            binary.Write(&ItemCount, 4);
            binary.Close();
        }

        public unsafe static void LoadClientProfs(GameClient client)
        {
            BinaryFile binary;
            binary = new BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\PlayersProfs\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                int CountProf = default(int);
                binary.Read(&CountProf, 4);
                ClientProficiency.DBProf DBProf = default(ClientProficiency.DBProf);
                for (int x = 0; x < CountProf; x++)
                {
                    binary.Read(&DBProf, sizeof(ClientProficiency.DBProf));
                    MsgProficiency ClientProf;
                    ClientProf = DBProf.GetClientProf();
                    client.MyProfs.ClientProf.TryAdd(ClientProf.ID, ClientProf);
                }
                binary.Close();
            }
        }

        public unsafe static void SaveClientProfs(GameClient client)
        {
            BinaryFile binary;
            binary = new BinaryFile();
            if (!binary.Open(ServerKernel.CO2FOLDER + "\\PlayersProfs\\" + client.Player.UID + ".bin", FileMode.Create))
                return;
            ClientProficiency.DBProf DBProf;
            DBProf = default(ClientProficiency.DBProf);
            int CountProf;
            CountProf = client.MyProfs.ClientProf.Count;
            binary.Write(&CountProf, 4);
            foreach (MsgProficiency prof in client.MyProfs.ClientProf.Values)
            {
                DBProf.GetDBSpell(prof);
                binary.Write(&DBProf, sizeof(ClientProficiency.DBProf));
            }
            binary.Close();
        }

        public unsafe static void LoadClientSpells(GameClient client)
        {
            BinaryFile binary;
            binary = new BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\PlayersSpells\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                int CountSpell = default(int);
                binary.Read(&CountSpell, 4);
                ClientSpells.DBSpell DBSpell = default(ClientSpells.DBSpell);
                for (int x = 0; x < CountSpell; x++)
                {
                    binary.Read(&DBSpell, sizeof(ClientSpells.DBSpell));
                    MsgSpell clientSpell;
                    clientSpell = DBSpell.GetClientSpell();
                    client.MySpells.ClientSpells.TryAdd(clientSpell.ID, clientSpell);
                }
                binary.Close();
            }
        }

        public unsafe static void SaveClientSpells(GameClient client)
        {
            BinaryFile binary;
            binary = new BinaryFile();
            if (!binary.Open(ServerKernel.CO2FOLDER + "\\PlayersSpells\\" + client.Player.UID + ".bin", FileMode.Create))
                return;
            ClientSpells.DBSpell DBSpell;
            DBSpell = default(ClientSpells.DBSpell);
            int SpellCount;
            SpellCount = client.MySpells.ClientSpells.Count;
            binary.Write(&SpellCount, 4);
            foreach (MsgSpell spell in client.MySpells.ClientSpells.Values)
            {
                DBSpell.GetDBSpell(spell);
                binary.Write(&DBSpell, sizeof(ClientSpells.DBSpell));
            }
            binary.Close();
        }

        public static void CreateCharacte(GameClient client)
        {
            IniFile write;
            write = new IniFile("\\Users\\" + client.Player.UID + ".ini");
            write.Write("Character", "UID", client.Player.UID);
            write.Write("Character", "Body", client.Player.Body);
            write.Write("Character", "Face", client.Player.Face);
            write.WriteString("Character", "Name", client.Player.Name);
            write.Write("Character", "Class", client.Player.Class);
            write.Write("Character", "Map", client.Player.Map);
            write.Write("Character", "X", client.Player.X);
            write.Write("Character", "Y", client.Player.Y);
            client.ArenaStatistic = new MsgArena.User();
            client.ArenaStatistic.ApplayInfo(client.Player);
            client.ArenaStatistic.Info.ArenaPoints = 4000;
            MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);
            client.Player.Nobility = new Nobility(client);
            client.TeamArenaStatistic = new MsgTeamArena.User();
            client.TeamArenaStatistic.ApplayInfo(client.Player);
            client.TeamArenaStatistic.Info.ArenaPoints = 4000;
            MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);
            client.Player.Associate = new Associate.MyAsociats(client.Player.UID)
            {
                MyClient = client,
                Online = true
            };
            client.Player.Flowers = new Flowers(client.Player.UID, client.Player.Name);
            client.Player.SubClass = new SubClass();
            client.FullLoading = true;
        }

        public static bool AllowCreate(uint UID)
        {
            return !File.Exists(ServerKernel.CO2FOLDER + "\\Users\\" + UID + ".ini");
        }

        public static void UpdateGuildMember(Guild.Member Member)
        {
            IniFile write;
            write = new IniFile("\\Users\\" + Member.UID + ".ini");
            write.Write("Character", "GuildRank", (ushort)0);
        }

        public static void UpdateGuildMember(Guild.UpdateDB Member)
        {
            IniFile write;
            write = new IniFile("\\Users\\" + Member.UID + ".ini");
            write.Write("Character", "GuildRank", (ushort)0);
            write.Write("Character", "GuildID", (ushort)0);
        }

        public static void UpdateMapRace(GameMap map)
        {
            IniFile write;
            write = new IniFile("\\maps\\" + map.ID + ".ini");
            write.Write("info", "race_record", map.RecordSteedRace);
        }

        public static void UpdateClanMember(Clan.Member Member)
        {
            IniFile write;
            write = new IniFile("\\Users\\" + Member.UID + ".ini");
            write.Write("Character", "ClanID", 0);
            write.Write("Character", "ClanRank", (ushort)0);
            write.Write("Character", "ClanDonation", 0);
        }

        public static void DestroySpouse(GameClient client)
        {
            if (client.Player.SpouseUID != 0)
            {
                IniFile write;
                write = new IniFile("\\Users\\" + client.Player.SpouseUID + ".ini");
                write.Write("Character", "SpouseUID", 0);
                write.WriteString("Character", "Spouse", "None");
                client.Player.SpouseUID = 0;
            }
            client.Player.Spouse = "None";
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Player.SendString(stream, MsgStringPacket.StringID.Spouse, false, "None");
            }
        }

        public static string GenerateDate()
        {
            return DateTime.Now.Second.ToString();
        }

        public static void UpdateSpouse(GameClient client)
        {
            if (client.Player.SpouseUID != 0)
            {
                IniFile write;
                write = new IniFile("\\Users\\" + client.Player.SpouseUID + ".ini");
                write.WriteString("Character", "Spouse", client.Player.Name);
            }
        }
    }
}
