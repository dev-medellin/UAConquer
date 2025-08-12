using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Role
{
    public class KOBoard
    {
        public class Entry
        {
            public uint UID = 0;
            public string Name = "";
            public uint Points = 0;
            public uint Rank = 0;
        }
        public static class KOBoardRanking
        {
            public const int MaxItems = 100;
            public static Extensions.SafeDictionary<uint, Entry> Items = new Extensions.SafeDictionary<uint, Entry>();
            public static object SynRoot = new object();
            private static Entry[] Top50 = new Entry[0];

            public static bool GetMyRank(uint UID, out Entry item)
            {
                
                if (Items.TryGetValue(UID, out item))
                {
                    return true;
                }

                return false;
            }

            public static void OnLoggin(Client.GameClient user)
            {
                Entry Item;
                if (GetMyRank(user.Player.UID, out Item))
                {
                    user.SendSysMesage("You have set a new personal! KO record with " + Item.Points + " KOs! You currently rank# " + Item.Rank+ "", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.white);
                }
            }
            public static void SendMessage(string Message)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(Message, Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                }               
            }
            public static void AddItem(Entry item, bool SendMessages = false)
            {
                //if (Program.ServerConfig.IsInterServer)
                //    return;
                if (Items.Count < MaxItems)
                {
                    if (!Items.ContainsKey(item.UID))
                    {
                        Items.Add(item.UID, item);
                        lock (SynRoot)
                        {
                            Top50 = Items.Values.OrderByDescending(p => p.Points).ToArray();
                            for (int x = 0; x < Top50.Length; x++)
                                Top50[x].Rank = (ushort)(x + 1);
                        }
                        if (SendMessages)
                            SendMessage("" + item.Name + " has killed " + item.Points + " monsters with XP skills and ranks " + item.Rank + " on the KO board.");
                    }
                    else if (Items.ContainsKey(item.UID))
                    {
                        uint points = Items[item.UID].Points;
                        uint rank = Items[item.UID].Rank;
                        Items[item.UID] = item;
                        item.Rank = rank;
                        if (points < item.Points)
                        {
                            lock (SynRoot)
                            {
                                Top50 = Items.Values.OrderByDescending(p => p.Points).ToArray();
                                for (int x = 0; x < Top50.Length; x++)
                                    Top50[x].Rank = (ushort)(x + 1);
                            }
                            if (SendMessages)
                                SendMessage("" + item.Name + " has killed " + item.Points + " monsters with XP skills and ranks " + item.Rank + " on the KO board.");
                        }
                    }
                }
                else
                {
                    if (Items.ContainsKey(item.UID))
                    {
                        uint points = Items[item.UID].Points;
                        uint rank = Items[item.UID].Rank;
                        Items[item.UID] = item;
                        item.Rank = rank;

                        if (points < item.Points)
                        {
                            lock (SynRoot)
                            {
                                Top50 = Items.Values.OrderByDescending(p => p.Points).ToArray();
                                for (int x = 0; x < Top50.Length; x++)
                                    Top50[x].Rank = (ushort)(x + 1);
                            }
                            if (SendMessages)
                                SendMessage("" + item.Name + " has killed " + item.Points + " monsters with XP skills and ranks " + item.Rank + " on the KO board.");
                        }
                    }
                    else
                    {
                        var last = Top50[Top50.Length - 1];
                        if (item.Points > last.Points)
                        {
                            Items.Remove(last.UID);
                            Items.Add(item.UID, item);

                            lock (SynRoot)
                            {
                                Top50 = Items.Values.OrderByDescending(p => p.Points).ToArray();
                                for (int x = 0; x < Top50.Length; x++)
                                    Top50[x].Rank = (ushort)(x + 1);
                            }
                            if (SendMessages)
                                SendMessage("" + item.Name + " has killed " + item.Points + " monsters with XP skills and ranks " + item.Rank + " on the KO board.");
                        }
                    }
                }
            }
            public static void Load()
            {
                using (Database.DBActions.Read reader = new Database.DBActions.Read("KOBoardRanks.txt", false))
                {
                    if (reader.Reader())
                    {
                        int count = reader.Count;
                        for (int x = 0; x < count; x++)
                        {
                            Database.DBActions.ReadLine line = new Database.DBActions.ReadLine(reader.ReadString("/"), '/');
                            Entry item = new Entry();
                            item.UID = line.Read((uint)0);
                            item.Name = line.Read("");
                            item.Points = line.Read((uint)0);
                            AddItem(item, false);
                        }
                    }
                }
            }
            // Removed the instance member declaration from the static class and made it static instead.
            public static string ConnectionString = DatabaseConfig.ConnectionString;

            public static void Save()
            {
                List<Role.Player> Players = new List<Role.Player>();

                using (Database.DBActions.Write writer = new Database.DBActions.Write("KOBoardRanks.txt"))
                {
                    foreach (var obj in Items.GetValues())
                    {
                        Database.DBActions.WriteLine line = new Database.DBActions.WriteLine('/');
                        line.Add(obj.UID).Add(obj.Name).Add(obj.Points);
                        
                        writer.Add(line.Close());
                        foreach (var p in Players.OrderByDescending(e => obj.Points).Take(10))
                        {
                            try
                            {
                                using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
                                {
                                    //Entry item = new Entry();

                                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("insert into cq_superman VALUES(@id,@number,@name)", conn))
                                    {
                                        conn.Open();


                                        cmd.Parameters.AddWithValue("@id", obj.UID);
                                        cmd.Parameters.AddWithValue("@number", obj.Points);
                                        cmd.Parameters.AddWithValue("@Name", obj.Name);
                                        cmd.ExecuteNonQuery();

                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                    writer.Execute(Database.DBActions.Mode.Open);
                }
            }
        }
    }
}
