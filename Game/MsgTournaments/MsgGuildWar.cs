using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer.AttackHandler.Algoritms;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.WindowsAPI;
using TheChosenProject.Database;
using MongoDB.Driver.Core.Configuration;
using DevExpress.XtraPrinting.Native;
using Org.BouncyCastle.Crypto;
using System.IO;
using static DevExpress.XtraEditors.Mask.Design.MaskSettingsForm.DesignInfo.MaskManagerInfo;
using TheChosenProject.Role.Instance;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgGuildWar
    {
        public DateTime ClosableLeftGateStamp, ClosableRightGateStamp;

        public class GuildConductor
        {
            public static List<uint> BlockMaps = new List<uint> { 1038 };

            public Npc Npc;

            public uint ToMap;

            public ushort ToX;

            public ushort ToY;

            public override string ToString()
            {
                WriteLine Line;
                Line = new WriteLine(',');
                Line.Add(Npc.UID).Add(Npc.X).Add(Npc.Y)
                    .Add(Npc.Map)
                    .Add(Npc.Mesh)
                    .Add((ushort)Npc.NpcType)
                    .Add(ToX)
                    .Add(ToY)
                    .Add(ToMap);
                return Line.Close();
            }

            internal void Load(string Line, NpcID UID)
            {
                Npc = Npc.Create();
                if (Line == "")
                {
                    Npc.UID = (uint)UID;
                    return;
                }
                ReadLine Reader;
                Reader = new ReadLine(Line, ',');
                Npc.UID = Reader.Read(0u);
                Npc.X = Reader.Read((ushort)0);
                Npc.Y = Reader.Read((ushort)0);
                Npc.Map = Reader.Read((ushort)0);
                Npc.Mesh = Reader.Read((ushort)0);
                Npc.NpcType = (Flags.NpcType)Reader.Read((ushort)0);
                ToX = Reader.Read((ushort)0);
                ToY = Reader.Read((ushort)0);
                ToMap = Reader.Read((ushort)0);
            }

            internal void GetCoords(out ushort x, out ushort y, out uint map)
            {
                if (ToMap != 0 && ToX != 0 && ToY != 0)
                {
                    x = ToX;
                    y = ToY;
                    map = ToMap;
                }
                else
                {
                    x = 429;
                    y = 378;
                    map = 1002;
                }
            }
        }

        public class DataFlameQuest
        {
            public List<uint> Registred;

            public bool ActiveFlame10;

            public DataFlameQuest()
            {
                Registred = new List<uint>();
                ActiveFlame10 = false;
            }
        }

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

        public List<uint> RewardLeader = new List<uint>();

        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound;

        public DateTime StampShuffleScore;

        private GameMap GuildWarMap;

        public DataFlameQuest FlamesQuest;

        public ConcurrentDictionary<uint, GuildWarScrore> ScoreList;

        public GuildWarScrore Winner;

        public Dictionary<NpcID, GuildConductor> GuildConductors;

        public ProcesType Proces { get; set; }

        public Dictionary<SobNpc.StaticMesh, SobNpc> Furnitures { get; set; }

        public bool LeftGateOpen => Furnitures[SobNpc.StaticMesh.LeftGate].Mesh == SobNpc.StaticMesh.OpenLeftGate;

        public bool RightGateOpen => Furnitures[SobNpc.StaticMesh.RightGate].Mesh == SobNpc.StaticMesh.OpenRightGate;

        public ConcurrentDictionary<uint, Scoreboard> Scoreboards = new ConcurrentDictionary<uint, Scoreboard>();

        public const uint MapID = 1038;

        public MsgGuildWar()
        {
            FlamesQuest = new DataFlameQuest();
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<SobNpc.StaticMesh, SobNpc>();
            GuildConductors = new Dictionary<NpcID, GuildConductor>();
            ScoreList = new ConcurrentDictionary<uint, GuildWarScrore>();
            Winner = new GuildWarScrore
            {
                Name = "None",
                Score = 100,
                GuildID = 0
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
            Furnitures.Add(SobNpc.StaticMesh.LeftGate, Server.ServerMaps[1038].View.GetMapObject<SobNpc>(MapObjectType.SobNpc, 516074));
            Furnitures.Add(SobNpc.StaticMesh.RightGate, Server.ServerMaps[1038].View.GetMapObject<SobNpc>(MapObjectType.SobNpc, 516075));
            Furnitures.Add(SobNpc.StaticMesh.Pole, Server.ServerMaps[1038].View.GetMapObject<SobNpc>(MapObjectType.SobNpc, 810));
            this.ClosableLeftGateStamp = DateTime.Now;
            this.ClosableRightGateStamp = DateTime.Now;
        }

        public bool Bomb(Packet stream, GameClient client, SobNpc.StaticMesh gate)
        {
            if (Furnitures[gate].HitPoints > 3000000)
            {
                Furnitures[gate].HitPoints -= 2000000;
                MsgUpdate upd;
                upd = new MsgUpdate(stream, Furnitures[gate].UID);
                stream = upd.Append(stream, MsgUpdate.DataType.Hitpoints, Furnitures[gate].HitPoints);
                Furnitures[gate].SendScrennPacket(upd.GetArray(stream));
                client.Player.Dead(null, client.Player.X, client.Player.Y, client.Player.UID);
                Furnitures[gate].SendString(stream, MsgStringPacket.StringID.Effect, "firemagic");
                Furnitures[gate].SendString(stream, MsgStringPacket.StringID.Effect, "bombarrow");
                Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " from " + ((client.Player.MyGuild != null) ? client.Player.MyGuild.GuildName.ToString() : "None".ToString()).ToString() + " detonated the Bomb and killed herself/himself. But the " + gate.ToString() + " was blown up!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                return true;
            }
            return false;
        }

        internal void ResetFurnitures(Packet stream)
        {
            Furnitures[SobNpc.StaticMesh.LeftGate].Mesh = SobNpc.StaticMesh.LeftGate;
            Furnitures[SobNpc.StaticMesh.RightGate].Mesh = SobNpc.StaticMesh.RightGate;
            foreach (SobNpc npc2 in Furnitures.Values)
            {
                npc2.HitPoints = npc2.MaxHitPoints;
            }
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map != 1038)
                    continue;
                foreach (SobNpc npc in Furnitures.Values)
                {
                    if (Core.GetDistance(client.Player.X, client.Player.Y, npc.X, npc.Y) <= 19)
                    {
                        MsgUpdate upd;
                        upd = new MsgUpdate(stream, npc.UID, 2);
                        stream = upd.Append(stream, MsgUpdate.DataType.Mesh, (long)npc.Mesh);
                        stream = upd.Append(stream, MsgUpdate.DataType.Hitpoints, npc.HitPoints);
                        stream = upd.GetArray(stream);
                        client.Send(stream);
                        if (npc.Mesh == SobNpc.StaticMesh.Pole)
                            client.Send(npc.GetArray(stream, false));
                    }
                }
            }
        }

        internal void SendMapPacket(Packet packet)
        {
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == 1038 || client.Player.Map == 6001)
                    client.Send(packet);
            }
        }

        internal void CompleteEndGuildWar()
        {
            try
            {
                ITournamentsAlive.Tournments.Remove(9);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not finish Guild War", true, LogType.WARNING);
            }
            SendInvitation = false;
            ShuffleGuildScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            foreach (var client in GuildWarMap.Values)
            {
                client.Teleport(50, 50, 1005);
            }
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == 1038 || client.Player.Map == 6001)
                {          
                    client.Player.ReviveC = 0;
                    client.Player.Kills = 0;
                    client.Player.Death = 0;
                }
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the guildwar with a score of " + Winner.Score, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the guildwar with a score of " + Winner.Score, MsgMessage.MsgColor.white, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
            }
            RewardDeputiLeader.Clear();
            RewardLeader.Clear();
            Winner.DeputiLeaderReward = 7;
            Winner.LeaderReward = 1;
        }

        internal void Start()
        {
            Scoreboards.Clear();
            FlamesQuest = new DataFlameQuest();
            Proces = ProcesType.Alive;
            foreach (GameClient clienst in Server.GamePoll.Values)
            {
                if (clienst.Player.ContainFlag(MsgUpdate.Flags.TopGuildLeader) || clienst.Player.ContainFlag(MsgUpdate.Flags.TopDeputyLeader))
                {
                    clienst.Player.RemoveFlag(MsgUpdate.Flags.TopGuildLeader);
                    clienst.Player.RemoveFlag(MsgUpdate.Flags.TopDeputyLeader);
                }
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                ResetFurnitures(stream);
                ScoreList.Clear();
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Guild war has started!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                //Program.DiscordEventsAPI.Enqueue("Guildwar Has Started");
                try
                {
                    ITournamentsAlive.Tournments.Add(9, ": started at(" + DateTime.Now.ToString("H:mm)"));
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not start Guild War", true, LogType.WARNING);
                }
            }
        }

        internal void FinishRound()
        {
            ShuffleGuildScores(true);
            Furnitures[SobNpc.StaticMesh.Pole].Name = Winner.Name;
            Proces = ProcesType.Idle;
            ScoreList.Clear();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the guildwar with a score of " + Winner.Score, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the guildwar with a score of " + Winner.Score, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                ResetFurnitures(stream);
            }
            StampRound = DateTime.Now.AddSeconds(3.0);
        }

        internal void Began()
        {
            if (Proces == ProcesType.Idle)
            {
                Proces = ProcesType.Alive;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Guild war has began!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
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
                if (Furnitures[SobNpc.StaticMesh.Pole].HitPoints == 0)
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
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.GuildWar].EndHour - 1)
                        rawGold *= 1.5;

                    //// Split if too many attackers (optional)
                    //rawGold = rawGold / Math.Max(numberOfAttackers, 1, 10);

                    int goldEarned = (int)Math.Min(rawGold, 500);

                    client.Owner.Player.MyGuildMember.MoneyDonate += goldEarned;
                    client.Owner.Player.MyGuild.Info.SilverFund += goldEarned;

                    if (Role.Instance.Guild.GuildPoll.TryGetValue(Winner.GuildID, out var Guild))
                    {
                        if(Guild.Info.SilverFund > 1000)
                            Guild.Info.SilverFund -= goldEarned;
                    }
                    //client.Owner.Player.Money += goldEarned;
                    //client.Owner.SendSysMesage($"You earn {goldEarned} Silver while hitting", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.yellow);
                    //using (RecycledPacket rec = new RecycledPacket())
                    //{
                    //    Packet stream;
                    //    stream = rec.GetStream();
                    //    //client.Owner.Player.MyGuild.SendThat(client.Owner.Player);
                    //    var persons = GuildWarMap.Values.ToArray();
                    //    foreach (var killer in persons)
                    //    {
                    //        killer.Player.MyGuild.SendThat(killer.Player);
                    //    }
                    //}
                }
            }
        }

        public void CheckUp()
        {
            if (Proces == ProcesType.Alive)
            {
                ShuffleGuildScores();
            }
        }

        public void SortScores()
        {
            if (Proces == ProcesType.Alive)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    ShuffleGuildScores();
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Players Kill Scores (Kill/Death)", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    var top = GuildWarMap.Values.OrderByDescending(x => x.Player.Kills).ToArray();
                    foreach (var killer in top.Take(5))
                    {
                        SendMapPacket(new MsgMessage(killer.Player.Name + " " + killer.Player.Kills + " / " + killer.Player.Death + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    }
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Players Revive Scores (Revive/Death)", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    top = GuildWarMap.Values.Where(x => x.Player.ReviveC > 0).OrderByDescending(x => x.Player.ReviveC).ToArray();
                    foreach (var killer in top.Take(5))
                    {
                        SendMapPacket(new MsgMessage(killer.Player.Name + " " + killer.Player.ReviveC + " / " + killer.Player.Death + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    }
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    foreach (var user in GuildWarMap.Values)
                    {
                        if (AtributesStatus.IsWater(user.Player.Class))
                        {
                            user.SendSysMesage("Your Revive Score (Revive/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                            user.SendSysMesage($"{user.Player.Name}- {user.Player.ReviveC}/{user.Player.Death}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        }
                        else
                        {
                            user.SendSysMesage("Your Kills Score (Kills/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                            user.SendSysMesage($"{user.Player.Name}- {user.Player.Kills}/{user.Player.Death}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        }
                    }
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Guild - [Total Players] - [Remaining]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    var Guilds = new List<Role.Instance.Guild>();
                    foreach (var client in GuildWarMap.Values)
                    {
                        if (client.Player.MyGuild != null)
                        {
                            if (!Guilds.Contains(client.Player.MyGuild))
                                Guilds.Add(client.Player.MyGuild);
                        }
                    }
                    foreach (var guild in Guilds.OrderByDescending(x => x.Members.Count))
                    {
                        SendMapPacket(new MsgMessage($"{guild.GuildName} - {guild.Members.Count} - {GuildWarMap.Values.Count(x => x.Player.GuildID == guild.Info.GuildID && x.Player.Alive)}", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    }
                }
            }
        }
        internal void ShuffleGuildScores(bool createWinned = false)
        {
            if (Proces == ProcesType.Dead)
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
                foreach (var client in GuildWarMap.Values)
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
                foreach (var user in GuildWarMap.Values)
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
                SendMapPacket(new MsgMessage($"Guild - [Total Players in Map]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                var Guilds = new List<Role.Instance.Guild>();
                foreach (var client in GuildWarMap.Values)
                {
                    if (client.Player.MyGuild != null)
                    {
                        if (!Guilds.Contains(client.Player.MyGuild))
                            Guilds.Add(client.Player.MyGuild);
                    }
                }
                foreach (var guild in Guilds.OrderByDescending(x => x.Members.Count))
                {
                    SendMapPacket(new MsgMessage($"[ {guild.GuildName} ] - [ {GuildWarMap.Values.Count(x => x.Player.GuildID == guild.Info.GuildID)} ]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    //SendMapPacket(new MsgMessage($"[ {guild.GuildName} ] - [ {GuildWarMap.Values.Count(x => x.Player.GuildID == guild.Info.GuildID)} ] - [ {GuildWarMap.Values.Count(x => x.Player.GuildID == guild.Info.GuildID && x.Player.Alive)} ]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
            }
        }

        internal bool ValidJump(int Current, out int New, ushort X, ushort Y)
        {
            if (Core.GetDistance(217, 177, X, Y) <= 3)
            {
                New = 0;
                return true;
            }
            New = Current;
            int new_FloorType;
            new_FloorType = GuildWarMap.FloorType[X, Y];
            if (Current == 3 && (new_FloorType == 0 || new_FloorType == 9 || new_FloorType == 13))
            {
                if (Core.GetDistance(X, Y, 164, 209) <= 20 && LeftGateOpen)
                {
                    New = new_FloorType;
                    return true;
                }
                if (Core.GetDistance(X, Y, 222, 177) <= 15 && RightGateOpen)
                {
                    New = new_FloorType;
                    return true;
                }
                return false;
            }
            New = new_FloorType;
            return true;
        }

        internal bool ValidWalk(int Current, out int New, ushort X, ushort Y)
        {
            if (Core.GetDistance(217, 177, X, Y) <= 3)
            {
                New = 0;
                return true;
            }
            New = Current;
            int new_mask;
            new_mask = GuildWarMap.FloorType[X, Y];
            if (Current == 3 && (new_mask == 0 || new_mask == 9 || new_mask == 13))
            {
                if (Y == 209 || Y == 208)
                {
                    if (Core.GetDistance(X, Y, 164, 209) <= 3 && LeftGateOpen)
                    {
                        New = new_mask;
                        return true;
                    }
                }
                else if (X == 216 && Core.GetDistance(X, Y, 216, 177) <= 4 && RightGateOpen)
                {
                    New = new_mask;
                    return true;
                }
                return false;
            }
            New = new_mask;
            return true;
        }
        public string ConnectionString = DatabaseConfig.ConnectionString;

        internal void Save()
        {
            IniFile write;
            write = new IniFile("\\GuildWarInfo.ini");
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
                write.Write("Pole", "HitPoints", Furnitures[SobNpc.StaticMesh.Pole].HitPoints);
            }
            write.WriteString("Condutors", "GuildConductor1", GuildConductors[NpcID.TeleGuild1].ToString());
            write.WriteString("Condutors", "GuildConductor2", GuildConductors[NpcID.TeleGuild2].ToString());
            write.WriteString("Condutors", "GuildConductor3", GuildConductors[NpcID.TeleGuild3].ToString());
            write.WriteString("Condutors", "GuildConductor4", GuildConductors[NpcID.TeleGuild4].ToString());
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
                        using (var cmdd = new MySql.Data.MySqlClient.MySqlCommand("Delete from cq_synattr where type=1", conn))
                            cmdd.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    #endregion

                    cmd.Parameters.AddWithValue("@guildid", Winner.GuildID);
                    cmd.Parameters.AddWithValue("@name", Winner.Name);
                    cmd.Parameters.AddWithValue("@type", "1");

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
            reader = new IniFile("\\GuildWarInfo.ini");
            Winner.GuildID = reader.ReadUInt32("Info", "ID", 0u);
            Winner.Name = reader.ReadString("Info", "Name", "None");
            Winner.LeaderReward = reader.ReadInt32("Info", "LeaderReward", 0);
            Winner.DeputiLeaderReward = reader.ReadInt32("Info", "DeputiLeaderReward", 0);
            RewardLeader.Add(reader.ReadUInt32("Info", "LeaderTop0", 0u));
            for (int x2 = 0; x2 < 8; x2++)
            {
                RewardDeputiLeader.Add(reader.ReadUInt32("Info", "DeputiTop" + x2, 0u));
            }
            Furnitures[SobNpc.StaticMesh.Pole].Name = reader.ReadString("Pole", "Name", "None");
            Furnitures[SobNpc.StaticMesh.Pole].HitPoints = reader.ReadInt32("Pole", "HitPoints", 0);
            for (int x = 0; x < 4; x++)
            {
                GuildConductor conductor;
                conductor = new GuildConductor();
                conductor.Load(reader.ReadString("Condutors", "GuildConductor" + (x + 1), ""), (NpcID)(101614 + x * 2));
                GuildConductors.Add((NpcID)(101614 + x * 2), conductor);
                if (conductor.Npc.Map != 0 && Server.ServerMaps.ContainsKey(conductor.Npc.Map))
                    Server.ServerMaps[conductor.Npc.Map].AddNpc(conductor.Npc);
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
            GuildWarMap = Server.ServerMaps[1038];
        }
    }
}