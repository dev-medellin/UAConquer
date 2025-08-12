using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Core.Configuration;
using TheChosenProject.WindowsAPI;



namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgEliteGuildWar
    {
        public const uint MapID = 8250;
        public class GuildWarScrore
        {
            public uint GuildID;

            public string Name;

            public uint Score;

            public int LeaderReward = 1;

            public int DeputiLeaderReward = 7;
        }

        public bool SendInvitation;

        internal ushort[][] StatueCoords = new ushort[7][]
        {
            new ushort[2] { 140, 134 },
            new ushort[2] { 144, 124 },
            new ushort[2] { 130, 138 },
            new ushort[2] { 153, 124 },
            new ushort[2] { 161, 124 },
            new ushort[2] { 130, 147 },
            new ushort[2] { 130, 155 }
        };

        public int ConquerPointsPrize = ServerKernel.ELITE_GUILD_WAR_Reward;

        public List<uint> RewardLeader = new List<uint>();

        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound;

        public DateTime StampShuffleScore;

        public ConcurrentDictionary<uint, GuildWarScrore> ScoreList;

        public GuildWarScrore Winner;

        public GameMap Map;

        public ConcurrentDictionary<uint, Scoreboard> Scoreboards = new ConcurrentDictionary<uint, Scoreboard>();

        public ProcesType Proces { get; set; }

        public Dictionary<SobNpc.StaticMesh, SobNpc> Furnitures { get; set; }

        public MsgEliteGuildWar()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<SobNpc.StaticMesh, SobNpc>();
            ScoreList = new ConcurrentDictionary<uint, GuildWarScrore>();
            Winner = new GuildWarScrore
            {
                Name = "None",
                Score = 100u,
                GuildID = 0u
            };
        }

        public class Scoreboard
        {
            public uint UID;
            public string Name;
            public uint MainPoints;
            public uint DeathPoints;
            public byte Class;
            public bool Claimed = false;
        }

        public const int MaxHitPoints = 30000000;
        public const int PoleUID = 820820;

        public static SobNpc Pole = new SobNpc
        {
            HitPoints = 30000000,
            MaxHitPoints = 30000000,
            Mesh = (SobNpc.StaticMesh)1137,
            Name = "EliteGuildWar",
            Type = Flags.NpcType.Pole,
            ObjType = MapObjectType.SobNpc,
            Sort = 21,
            UID = PoleUID
        };

        public void AddPoints(Role.Player client, uint points, uint death)
        {
            Scoreboards.AddOrUpdate(client.UID, new Scoreboard()
            {
                UID = client.UID,
                Name = client.Name,
                MainPoints = points,
                DeathPoints = death,
                Class = client.Class
            }, (key, oldValue) =>
            {
                oldValue.MainPoints += points;
                oldValue.DeathPoints += death;
                return oldValue;
            });
        }

        public void CreateFurnitures()
        {
            Map = Database.Server.ServerMaps[8250u];
            //Furnitures.Add(SobNpc.StaticMesh.Pole, Server.ServerMaps[8250u].View.GetMapObject<SobNpc>(MapObjectType.SobNpc, 820u));
        }

        internal void ResetFurnitures(Packet stream)
        {
            //foreach (SobNpc npc2 in Furnitures.Values)
            //{
            //    npc2.HitPoints = npc2.MaxHitPoints;
            //}
            Pole.HitPoints = Pole.MaxHitPoints;
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map != 8250)
                    continue;
                if (Core.GetDistance(client.Player.X, client.Player.Y, Pole.X, Pole.Y) <= 19)
                {
                    MsgUpdate upd;
                    upd = new MsgUpdate(stream, Pole.UID, 2u);
                    stream = upd.Append(stream, MsgUpdate.DataType.Mesh, (long)Pole.Mesh);
                    stream = upd.Append(stream, MsgUpdate.DataType.Hitpoints, Pole.HitPoints);
                    stream = upd.GetArray(stream);
                    client.Send(stream);
                    if (Pole.Mesh == SobNpc.StaticMesh.Pole)
                        client.Send(Pole.GetArray(stream, false));
                }
                //foreach (SobNpc npc in Furnitures.Values)
                //{
                //    if (Core.GetDistance(client.Player.X, client.Player.Y, npc.X, npc.Y) <= 19)
                //    {
                //        MsgUpdate upd;
                //        upd = new MsgUpdate(stream, npc.UID, 2u);
                //        stream = upd.Append(stream, MsgUpdate.DataType.Mesh, (long)npc.Mesh);
                //        stream = upd.Append(stream, MsgUpdate.DataType.Hitpoints, npc.HitPoints);
                //        stream = upd.GetArray(stream);
                //        client.Send(stream);
                //        if (npc.Mesh == SobNpc.StaticMesh.Pole)
                //            client.Send(npc.GetArray(stream, false));
                //    }
                //}
            }
        }

        internal void SendMapPacket(Packet packet)
        {
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == 8250 || client.Player.Map == 6001)
                    client.Send(packet);
            }
        }
        public string ConnectionString = DatabaseConfig.ConnectionString;

        internal void CompleteEndGuildWar()
        {
            try
            {
                ITournamentsAlive.Tournments.Remove(10);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not finish Elite GuildWar", true, LogType.WARNING);
            }
            SendInvitation = false;
            ShuffleGuildScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == 8250 || client.Player.Map == 6001)
                {
                    client.Player.ReviveC = 0;
                    client.Player.Kills = 0;
                    client.Player.Death = 0;
                }
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                string msg;
                msg = "";
                msg = ((!(Winner.Name != "None") || Winner.Score == 100) ? "EliteGuildWar has ended with no winner." : ("Congratulations to " + Winner.Name + ", they've won the EliteGW with a score of " + Winner.Score));
                Packet stream;
                stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage(msg, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgMessage(msg, MsgMessage.MsgColor.white, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
            }
            removePole();
            RewardLeader.Clear();
            Winner.LeaderReward = 1;
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("insert into cq_synattr (guildid,name,type) " +
                    "values (@guildid,@name,@type)"
                    , conn))
                {
                    conn.Open();
                    #region ClearCommand
                    try
                    {
                        using (var cmdd = new MySql.Data.MySqlClient.MySqlCommand("Delete from cq_synattr where type=2", conn))
                            cmdd.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    #endregion

                    cmd.Parameters.AddWithValue("@guildid", Winner.GuildID);
                    cmd.Parameters.AddWithValue("@name", Winner.Name);
                    cmd.Parameters.AddWithValue("@type", "2");

                    cmd.ExecuteNonQuery();

                }
            }
        }

        internal void Start()
        {
            Proces = ProcesType.Alive;
            Scoreboards.Clear();
            MsgSchedules.EliteGuildWar.CreateFurnitures();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                ResetFurnitures(stream);
                ScoreList.Clear();
                SobNpc pole;
                int hitPoints;
                pole = Pole;
                Pole.Map = 8250;
                Pole.X = 196;
                Pole.Y = 178;
                pole = Pole;
                hitPoints = (Pole.MaxHitPoints = 30000000);
                pole.HitPoints = hitPoints;
                Map.View.EnterMap((IMapObj)Pole);
                Map.SetFlagNpc(Pole.X, Pole.Y);
                Map.WasPKFree = Program.FreePkMap.Contains(Map.ID);
                if (!Map.WasPKFree)
                    Program.FreePkMap.Add(Map.ID);
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream4;
                    stream4 = recycledPacket.GetStream();
                    foreach (IMapObj user in Map.View.Roles(MapObjectType.Player, Pole.X, Pole.Y))
                    {
                        user.Send(Pole.GetArray(stream4, false));
                    }
                }
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Elite Guild war has started!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                try
                {
                    ITournamentsAlive.Tournments.Add(10, ": started at(" + DateTime.Now.ToString("H:mm)"));
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not start Elite GuildWar", true, LogType.WARNING);
                }
            }
        }

        internal void FinishRound()
        {
            ShuffleGuildScores(true);
            //Furnitures[SobNpc.StaticMesh.Pole].Name = Winner.Name
            Pole.Name = Winner.Name;
            Proces = ProcesType.Idle;
            ScoreList.Clear();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the EliteGW round with a score of " + Winner.Score, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the EliteGW round with a score of " + Winner.Score, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                ResetFurnitures(stream);
            }
            StampRound = DateTime.Now.AddSeconds(3.0);
        }

        internal void Began()
        {
            if (Proces == ProcesType.Idle)
            {
                SobNpc pole;
                int hitPoints;
                Proces = ProcesType.Alive;
                pole = Pole;
                hitPoints = (Pole.MaxHitPoints = 30000000);
                pole.HitPoints = hitPoints;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("EliteGuildWar has began!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                }
            }
        }

        internal void UpdateScore(Player client, uint Damage)
        {
            if (client.MyGuild != null && Proces == ProcesType.Alive)
            {
                if (!ScoreList.ContainsKey(client.GuildID))
                    ScoreList.TryAdd(client.GuildID, new GuildWarScrore
                    {
                        GuildID = client.MyGuild.Info.GuildID,
                        Name = client.MyGuild.GuildName,
                        Score = Damage
                    });
                else
                    ScoreList[client.MyGuild.Info.GuildID].Score += Damage;
                if (Pole.HitPoints == 0)
                    FinishRound();

                DateTime Now64;
                Now64 = DateTime.Now;
                double damageMultiplier = 10.0;
                double timeBonus = 1.0;
                double baseMultiplier = 10.0;

                if (Damage >= 10000)
                {
                    Random rand = new Random(); // Ideally declared once globally if in a loop
                    double rawGold = rand.Next(50, 151); // 151 is exclusive, so this gives 50–150

                    // Time bonus for late-game effort
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.EliteGuildWar].EndHour - 1)
                        rawGold *= 1.5;
                    //else if (Now64.Minute >= 30)
                    //    rawGold *= 1.2;

                    //// Split if too many attackers (optional)
                    //rawGold = rawGold / Math.Max(numberOfAttackers, 1, 10);

                    int goldEarned = (int)Math.Min(rawGold, 500);

                    client.Owner.Player.MyGuildMember.MoneyDonate += goldEarned;
                    client.Owner.Player.MyGuild.Info.SilverFund += goldEarned;

                    if (Role.Instance.Guild.GuildPoll.TryGetValue(Winner.GuildID, out var Guild))
                    {
                        if (Guild.Info.SilverFund > 1000)
                            Guild.Info.SilverFund -= goldEarned;
                    }
                    //client.Owner.Player.Money += goldEarned;
                    //client.Owner.SendSysMesage($"You earn {goldEarned} Silver while hitting", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.yellow);
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        //client.Owner.Player.MyGuild.SendThat(client.Owner.Player);
                        var persons = Map.Values.ToArray();
                        foreach (var killer in persons)
                        {
                            killer.Player.MyGuild.SendThat(killer.Player);
                        }
                    }
                }
                //if (Furnitures[SobNpc.StaticMesh.Pole].HitPoints == 0)
                //    FinishRound();
            }
        }

        public void CheckUp()
        {
            if (Proces == ProcesType.Alive)
            {
                ShuffleGuildScores();
            }
        }

        internal void ShuffleGuildScores(bool createWinned = false)
        {
            if (Proces != ProcesType.Alive)
                return;
            StampShuffleScore = DateTime.Now.AddSeconds(8.0);
            GuildWarScrore[] Array;
            Array = ScoreList.Values.ToArray();
            GuildWarScrore[] DescendingList;
            DescendingList = Array.OrderByDescending((GuildWarScrore p) => p.Score).ToArray();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                bool first = false;
                SendMapPacket(new MsgMessage($"", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.FirstRightCorner).GetArray(stream));
                foreach (var client in Map.Values)
                {
                    for (int x = 0; x < DescendingList.Length; x++)
                    {
                        GuildWarScrore element;
                        element = DescendingList[x];
                        if (x == 0 && createWinned)
                            Winner = element;
                        first = true;
                        client.SendSysMesage("No " + (x + 1) + ". " + element.Name + " (" + element.Score + ")", (x == 0) ? MsgMessage.ChatMode.FirstRightCorner : MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        if (x == 4) break;
                    }
                }
                //for (int x = 0; x < DescendingList.Length; x++)
                //{
                //    GuildWarScrore element;
                //    element = DescendingList[x];
                //    if (x == 0 && createWinned)
                //        Winner = element;

                //    MsgMessage msg;
                //    msg = new MsgMessage("No " + (x + 1) + ". " + element.Name + " (" + element.Score + ")", MsgMessage.MsgColor.yellow, (x == 0) ? MsgMessage.ChatMode.FirstRightCorner : MsgMessage.ChatMode.ContinueRightCorner);
                //    SendMapPacket(msg.GetArray(stream));
                //    if (x == 4)
                //        break;
                //}
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, first ? MsgMessage.ChatMode.ContinueRightCorner : MsgMessage.ChatMode.FirstRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage("Winners Boards", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"Round Winner : [ {Winner.Name} ]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"Players Kill Scores (Kill/Death)", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                var top = Scoreboards.Values.Where(x => !AtributesStatus.IsWater(x.Class)).OrderByDescending(x => x.MainPoints).ToArray();
                foreach (var killer in top.Take(5))
                {
                    SendMapPacket(new MsgMessage(killer.Name + " " + killer.MainPoints + " / " + killer.DeathPoints + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"Players Revive Scores (Revive/Death)", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                top = Scoreboards.Values.Where(x => AtributesStatus.IsWater(x.Class)).OrderByDescending(x => x.MainPoints).ToArray();
                foreach (var killer in top.Take(5))
                {
                    SendMapPacket(new MsgMessage(killer.Name + " " + killer.MainPoints + " / " + killer.DeathPoints + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                foreach (var user in Map.Values)
                {
                    if (Scoreboards.TryGetValue(user.Player.UID, out var score))
                    {
                        if (AtributesStatus.IsWater(score.Class))
                        {
                            user.SendSysMesage("Your Revive Score (Revive/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                            user.SendSysMesage($"{score.Name}- {score.MainPoints}/{score.DeathPoints}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        }
                        else
                        {
                            user.SendSysMesage("Your Kills Score (Kills/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                            user.SendSysMesage($"{score.Name}- {score.MainPoints}/{score.DeathPoints}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        }
                    }
                }
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"Guild - [Total Players] - [Remaining]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                var Guilds = new List<Role.Instance.Guild>();
                foreach (var client in Map.Values)
                {
                    if (client.Player.MyGuild != null)
                    {
                        if (!Guilds.Contains(client.Player.MyGuild))
                            Guilds.Add(client.Player.MyGuild);
                    }
                }
                foreach (var guild in Guilds.OrderByDescending(x => x.Members.Count))
                {
                    SendMapPacket(new MsgMessage($"[ {guild.GuildName} ] - [ {Map.Values.Count(x => x.Player.GuildID == guild.Info.GuildID)} ] - [ {Map.Values.Count(x => x.Player.GuildID == guild.Info.GuildID && x.Player.Alive)} ]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
            }
        }

        public bool SignUp(GameClient client, Packet stream)
        {
            if (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead)
            {
                client.Teleport(158, 052, 8250);
                return true;
            }
            return false;
        }

        public void removePole()
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Map.RemoveSobNpc(Pole, stream);
            }
            if (!Map.WasPKFree)
                Program.FreePkMap.Remove(Map.ID);
        }

        internal void Save()
        {
            IniFile write;
            write = new IniFile("\\EliteGuildWar.ini");
            if (Proces == ProcesType.Dead)
            {
                write.Write("Info", "ID", Winner.GuildID);
                write.WriteString("Info", "Name", Winner.Name);
                write.Write("Info", "LeaderReward", Winner.LeaderReward);
                write.Write("Info", "DeputiLeaderReward", Winner.DeputiLeaderReward);
                for (int x2 = 0; x2 < RewardLeader.Count; x2++)
                {
                    write.Write("Info", "LeaderTop" + x2, RewardLeader[x2]);
                }
                for (int x = 0; x < 8 && x < RewardDeputiLeader.Count; x++)
                {
                    write.Write("Info", "DeputiTop" + x, RewardDeputiLeader[x]);
                }
                write.WriteString("Pole", "Name", Winner.Name);
            }
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("insert into cq_synattr (guildid,name,type) " +
                    "values (@guildid,@name,@type)"
                    , conn))
                {
                    conn.Open();
                    #region ClearCommand
                    try
                    {
                        using (var cmdd = new MySql.Data.MySqlClient.MySqlCommand("Delete from cq_synattr where type=2", conn))
                            cmdd.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    #endregion

                    cmd.Parameters.AddWithValue("@guildid", Winner.GuildID);
                    cmd.Parameters.AddWithValue("@name", Winner.Name);
                    cmd.Parameters.AddWithValue("@type", "2");

                    cmd.ExecuteNonQuery();

                }
            }
            write.Write<int>("Scoreboard", "Count", Scoreboards.Count);
            int count = 0;
            foreach (var xx in Scoreboards.Values)
            {
                write.Write<uint>("Scoreboard", "UID" + count, xx.UID);
                write.Write<string>("Scoreboard", "Name" + count, xx.Name);
                write.Write<byte>("Scoreboard", "Class" + count, xx.Class);
                write.Write<uint>("Scoreboard", "MainPoints" + count, xx.MainPoints);
                write.Write<uint>("Scoreboard", "DeathPoints" + count, xx.DeathPoints);
                write.Write<bool>("Scoreboard", "Claimed" + count, xx.Claimed);
                count++;
            }
        }

        internal void Load()
        {
            IniFile reader;
            reader = new IniFile("\\EliteGuildWar.ini");
            Winner.GuildID = reader.ReadUInt32("Info", "ID", 0u);
            Winner.Name = reader.ReadString("Info", "Name", "None");
            Winner.LeaderReward = reader.ReadInt32("Info", "LeaderReward", 0);
            Winner.DeputiLeaderReward = reader.ReadInt32("Info", "DeputiLeaderReward", 0);
            RewardLeader.Add(reader.ReadUInt32("Info", "LeaderTop0", 0u));
            for (int x2 = 0; x2 < 8; x2++)
            {
                RewardDeputiLeader.Add(reader.ReadUInt32("Info", "DeputiTop" + x2, 0u));
            }
            var count = reader.ReadInt32("Scoreboard", "Count", 0);
            for (var xx = 0; xx < count; xx++)
            {
                var UID = reader.ReadUInt32("Scoreboard", "UID" + xx, 0);
                var Name = reader.ReadString("Scoreboard", "Name" + xx, "");
                var Class = reader.ReadByte("Scoreboard", "Class" + xx, 0);
                var MainPoints = reader.ReadUInt32("Scoreboard", "MainPoints" + xx, 0);
                var DeathPoints = reader.ReadUInt32("Scoreboard", "DeathPoints" + xx, 0);
                var claimed = reader.ReadBool("Scoreboard", "Claimed" + xx, false);
                Scoreboards.TryAdd(UID, new Scoreboard()
                {
                    UID = UID,
                    Name = Name,
                    Class = Class,
                    MainPoints = MainPoints,
                    DeathPoints = DeathPoints,
                    Claimed = claimed,
                });
            }
            Map = Server.ServerMaps[8250];
        }
    }
}
