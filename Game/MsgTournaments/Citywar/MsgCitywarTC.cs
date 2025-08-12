using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using DevExpress.XtraPrinting.Native;
using System.Diagnostics;
using TheChosenProject.Client;
using TheChosenProject.ServerSockets;
using TheChosenProject.Role;
using Google.Protobuf.Reflection;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using static TheChosenProject.Game.MsgTournaments.MsgClanWar;

namespace TheChosenProject.Game.MsgTournaments
{
    public class CoordinateTC
    {
        public ushort X;
        public ushort Y;
        public uint Map;
        public CoordinateTC(uint Map, ushort X, ushort Y)
        {
            this.Map = Map;
            this.X = X;
            this.Y = Y;
        }
    }

    public class MsgCitywarTC
    {

        public bool SendInvitation = false;
        public class CitywarTCScrore
        {
            public const int ConquerPointsReward = 1000000;

            public uint GuildID;
            public string Name;
            public uint Score;

            //for reward
            public int LeaderReward = 1;
            public int DeputiLeaderReward = 7;
        }

        public List<uint> RewardLeader = new List<uint>();
        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound = new DateTime();
        public DateTime StampShuffleScore = new DateTime();
        //public GameMap Map;
        private Role.GameMap GuildWarMap;

        public ProcesType Proces { get; set; }

        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures { get; set; }
        public ConcurrentDictionary<uint, CitywarTCScrore> ScoreList;
        public ConcurrentDictionary<uint, Scoreboard> Scoreboards = new ConcurrentDictionary<uint, Scoreboard>();
        public CitywarTCScrore Winner;
        public MsgCitywarTC()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
            ScoreList = new ConcurrentDictionary<uint, CitywarTCScrore>();
            Winner = new CitywarTCScrore() { Name = "None", Score = 100, GuildID = 0 };
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
        public class coords
        {
            public ushort x;
            public ushort y;
        }
        public Dictionary<uint, coords> CoordsPole = new Dictionary<uint, coords>();
       

        public unsafe void CreateFurnitures()
        {

            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[19891].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 831));

        }
        internal unsafe void ResetFurnitures(ServerSockets.Packet stream)
        {

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == 19891)
                {
                    foreach (var npc in Furnitures.Values)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, npc.X, npc.Y) <= Role.SobNpc.SeedDistrance)
                        {
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, npc.UID, 1);
                            //upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (uint)npc.Mesh);
                            upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, npc.HitPoints);
                            stream = upd.GetArray(stream);
                            client.Send(stream);
                            if ((Role.SobNpc.StaticMesh)npc.Mesh == Role.SobNpc.StaticMesh.Pole)
                                client.Send(npc.GetArray(stream, false));
                        }
                    }
                }
            }
        }
        internal unsafe void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == 19891 || client.Player.Map == 6001)
                {
                    client.Send(packet);
                }
            }
        }
        internal unsafe void CompleteEndCitywarTC()
        {
            SendInvitation = false;
            ShuffleScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            foreach (var client in Database.Server.ServerMaps[19891].Values)
            {
                client.Teleport(50, 50, 1005);
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                string msg = "";
                if (Winner.Name != "None" && Winner.Score != 100)
                    msg = "Congratulations to " + Winner.Name + ", they've won the Citywar TC with a score of " + Winner.Score.ToString();
                else msg = "Citywar TC has ended with no winner.";

                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
            }

            RewardLeader.Clear();
            Winner.LeaderReward = 1;
            //HidePole();
        }
        
        
        internal unsafe void Start()
        {
            //ShowPole();
            Proces = ProcesType.Alive;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ResetFurnitures(stream);
                ScoreList.Clear();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Citywar TC war in Twin City has started!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Citywar TC war in Twin City has started!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
            }
        }

        public uint DinamicMap;
        public bool Join(GameClient user, Packet stream)
        {
            if (Proces == ProcesType.Alive)
            {
                //ushort x;
                //x = 0;
                //ushort y;
                //y = 0;
                //Database.Server.ServerMaps[19891].GetRandCoord(ref x, ref y);
                //user.Teleport(x, y, 19891, 0);
                var rnd = Program.GetRandom.Next(100);
                if (rnd < 25)
                    user.Teleport(441, 464, 19891);
                else if (rnd < 50)
                    user.Teleport(551, 464, 19891);
                else if (rnd < 75)
                    user.Teleport(551, 413, 19891);
                else
                    user.Teleport(537, 351, 19891);
                return true;
            }
            return false;
        }
        internal unsafe void FinishRound()
        {
            ShuffleScores(true);
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = Winner.Name;
            Proces = ProcesType.Idle;
            ScoreList.Clear();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the Citywar TC round with a score of " + Winner.Score.ToString() + ""
                   , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the Citywar TC round with a score of " + Winner.Score.ToString() + ""
                    , MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                ResetFurnitures(stream);
            }
            StampRound = DateTime.Now.AddSeconds(3);
        }
        internal unsafe void Began()
        {
            if (Proces == ProcesType.Idle)
            {
                Proces = ProcesType.Alive;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("TwinCityWar TC has began!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                    //Program.DiscordEventsAPI.Enqueue("TwinCityWar Started");
                }
            }
        }
        internal void UpdateScore(Role.Player client, uint Damage)
        {
            if (client.MyGuild == null)
                return;
            if (Proces == ProcesType.Alive)
            {
                if (!ScoreList.ContainsKey(client.GuildID))
                {
                    ScoreList.TryAdd(client.GuildID, new CitywarTCScrore() { GuildID = client.MyGuild.Info.GuildID, Name = client.MyGuild.GuildName, Score = Damage });
                }
                else
                {
                    ScoreList[client.MyGuild.Info.GuildID].Score += Damage;
                }

                if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    FinishRound();
            }
        }
        internal unsafe void CompleteEndPoleDominationTC()
        {
            SendInvitation = false;
            ShuffleScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            foreach (var client in Database.Server.ServerMaps[19891].Values)
            {
                client.Teleport(50, 50, 1005);
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                string msg = "";
                if (Winner.Name != "None" && Winner.Score != 100)
                    msg = "Congratulations to " + Winner.Name + ", they've won the TwinCity Pole with a score of " + Winner.Score.ToString();
                else msg = "TwinCity Pole has ended with no winner.";

                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
            }

            RewardLeader.Clear();
            Winner.LeaderReward = 1;
        }
        internal void ShuffleScores(bool createWinned = false)
        {
            if (Proces != ProcesType.Alive)
                return;
            StampShuffleScore = DateTime.Now.AddSeconds(8.0);
            CitywarTCScrore[] Array;
            Array = ScoreList.Values.ToArray();
            CitywarTCScrore[] DescendingList;
            DescendingList = Array.OrderByDescending((CitywarTCScrore p) => p.Score).ToArray();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                bool first = false;
                SendMapPacket(new MsgMessage($"", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.FirstRightCorner).GetArray(stream));
                foreach (var client in Database.Server.ServerMaps[19891].Values)
                {
                    for (int x = 0; x < DescendingList.Length; x++)
                    {
                        CitywarTCScrore element;
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
                foreach (var user in Database.Server.ServerMaps[19891].Values)
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
                            user.SendSysMesage($"{score.Name}- {score.MainPoints} / {score.DeathPoints}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        }
                    }
                    else
                    {
                        user.SendSysMesage("Your Kills Score (Kills/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        user.SendSysMesage($"{user.Name} - 0 / 0", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                    }
                }
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"Clan - [Total Players in Map]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                var Clans = new List<Role.Instance.Clan>();
                foreach (var client in Database.Server.ServerMaps[19891].Values)
                {
                    if (client.Player.MyClan != null)
                    {
                        if (!Clans.Contains(client.Player.MyClan))
                            Clans.Add(client.Player.MyClan);
                    }
                }
                foreach (var clan in Clans.OrderByDescending(x => x.Members.Count))
                {
                    SendMapPacket(new MsgMessage($"[ {clan.Name} ] - [ {Database.Server.ServerMaps[19891].Values.Count(x => x.Player.ClanUID == clan.ID)} ]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    //SendMapPacket(new MsgMessage($"[ {guild.GuildName} ] - [ {GuildWarMap.Values.Count(x => x.Player.GuildID == guild.Info.GuildID)} ] - [ {GuildWarMap.Values.Count(x => x.Player.GuildID == guild.Info.GuildID && x.Player.Alive)} ]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
            }
            //for (int x = 0; x < DescendingList.Length; x++)
            //{
            //    ClanWarScrore element;
            //    element = DescendingList[x];
            //    if (x == 0 && createWinned)
            //        Winner = element;
            //    using (RecycledPacket rec = new RecycledPacket())
            //    {
            //        Packet stream;
            //        stream = rec.GetStream();
            //        MsgMessage msg;
            //        msg = new MsgMessage("No " + (x + 1) + ". " + element.Name + " (" + element.Score + ")", MsgMessage.MsgColor.yellow, (x == 0) ? MsgMessage.ChatMode.FirstRightCorner : MsgMessage.ChatMode.ContinueRightCorner);
            //        SendMapPacket(msg.GetArray(stream));
            //    }
            //    if (x == 4)
            //        break;
            //}
        }
        public void CheckUp()
        {
            if (Proces == ProcesType.Alive)
            {
                ShuffleScores();
            }
            else
            {
                foreach (var client in Database.Server.ServerMaps[19891].Values)
                {
                    client.Teleport(50, 50, 1005);
                }
            }
        }
    }
}
