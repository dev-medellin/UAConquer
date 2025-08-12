using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Client;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgClanWar
    {
        public class ClanWarScrore
        {
            public uint ClanID;

            public string Name;

            public uint Score;

            public int LeaderReward = 1;

            public int DeputiLeaderReward = 7;
        }

        public uint DynamicId;

        public bool SendInvitation;

        public int ConquerPointsReward = ServerKernel.CLASSIC_CLAN_WAR_REWARD;

        public List<uint> RewardLeader = new List<uint>();

        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound;

        public DateTime StampShuffleScore;

        public ConcurrentDictionary<uint, ClanWarScrore> ScoreList;
        public ConcurrentDictionary<uint, Scoreboard> Scoreboards = new ConcurrentDictionary<uint, Scoreboard>();

        public ClanWarScrore Winner;

        public GameMap Map;

        public SobNpc Pole;

        public ProcesType Proces { get; set; }

        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures { get; set; }

        public MsgClanWar()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
            ScoreList = new ConcurrentDictionary<uint, ClanWarScrore>();
            Winner = new ClanWarScrore
            {
                Name = "None",
                Score = 100u,
                ClanID = 0u
            };
            if (!Program.FreePkMap.Contains(1125u))
                Program.FreePkMap.Add(1125u);
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

        //public void Create()
        //{
        //    Map = Server.ServerMaps[1125u];
        //    DynamicId = Map.GenerateDynamicID();
        //    AddNpc(148, 150);
        //}
        public unsafe void CreateFurnitures()
        {

            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[1125].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 811));

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

        public void AddNpc(ushort x, ushort y)
        {
            if (Map.View.Contain(807u, x, y))
                return;
            Pole = new SobNpc
            {
                X = x,
                Map = Map.ID,
                DynamicID = DynamicId,
                ObjType = MapObjectType.SobNpc,
                Y = y
            };
            Pole.DynamicID = DynamicId;
            Pole.UID = 807u;
            Pole.Type = Flags.NpcType.Pole;
            Pole.Mesh = (SobNpc.StaticMesh)1137;
            Pole.Name = Winner.Name;
            Pole.HitPoints = 30000000;
            Pole.MaxHitPoints = 30000000;
            Pole.Sort = 21;
            Map.View.EnterMap((IMapObj)Pole);
            Map.SetFlagNpc(Pole.X, Pole.Y);
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                foreach (IMapObj user in Map.View.Roles(MapObjectType.Player, Pole.X, Pole.Y))
                {
                    user.Send(Pole.GetArray(stream, false));
                }
            }
            Furnitures.Add(SobNpc.StaticMesh.Pole, Pole);
        }

        public void ResetPole()
        {
            Pole.Name = Winner.Name;
            Pole.HitPoints = 30000000;
            Pole.MaxHitPoints = 30000000;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                foreach (IMapObj user in Map.View.Roles(MapObjectType.Player, Pole.X, Pole.Y))
                {
                    user.Send(Pole.GetArray(stream, false));
                }
            }
        }

        internal void ResetFurnitures(Packet stream)
        {
            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == 1125)
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

        internal void SendMapPacket(Packet packet)
        {
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == 1125 || client.Player.Map == 6001)
                    client.Send(packet);
            }
        }

        internal void CompleteEndWar()
        {
            SendInvitation = false;
            ShuffleScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            foreach (var client in Database.Server.ServerMaps[1125].Values)
            {
                client.Teleport(50, 50, 1005);
            }
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == 1125)
                {
                    client.Player.ReviveC = 0;
                    client.Player.Kills = 0;
                    client.Player.Death = 0;
                }
            }
            foreach (var client in Database.Server.ServerMaps[1125].Values)
            {
                client.Teleport(50, 50, 1005);
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                try
                {
                    ITournamentsAlive.Tournments.Remove(11);
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not finish Clan War", true, LogType.WARNING);
                }
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the ClanWar.", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the ClanWar.", MsgMessage.MsgColor.white, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
            }
            RewardLeader.Clear();
            Winner.LeaderReward = 1;
        }

        internal void Start()
        {
            Proces = ProcesType.Alive;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                ResetFurnitures(stream);
                ScoreList.Clear();
                Program.SendGlobalPackets.Enqueue(new MsgMessage("ClanWar war has started!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                //Program.DiscordEventsAPI.Enqueue("ClanWar Has Starter!");
                try
                {
                    ITournamentsAlive.Tournments.Add(11, ": started at(" + DateTime.Now.ToString("H:mm)"));
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not start ClanWar", true, LogType.WARNING);
                }
            }
        }

        internal void FinishRound()
        {
            ShuffleScores(true);
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = Winner.Name;
            Proces = ProcesType.Idle;
            ScoreList.Clear();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the ClanWar round with a score of " + Winner.Score, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the ClanWar round with a score of " + Winner.Score, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
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
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("ClanWar has began!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                }
            }
        }

        internal void UpdateScore(Player client, uint Damage)
        {
            if (client.MyClan == null)
                return;
            if (Proces == ProcesType.Alive)
            {
                if (!ScoreList.ContainsKey(client.ClanUID))
                {
                    ScoreList.TryAdd(client.ClanUID, new ClanWarScrore() { ClanID = client.MyClan.ID, Name = client.MyClan.Name, Score = Damage });
                }
                else
                {
                    ScoreList[client.MyClan.ID].Score += Damage;
                }

                if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    FinishRound();
            }
        }

        internal void ShuffleScores(bool createWinned = false)
        {
            if (Proces != ProcesType.Alive)
                return;
            StampShuffleScore = DateTime.Now.AddSeconds(8.0);
            ClanWarScrore[] Array;
            Array = ScoreList.Values.ToArray();
            ClanWarScrore[] DescendingList;
            DescendingList = Array.OrderByDescending((ClanWarScrore p) => p.Score).ToArray();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                bool first = false;
                SendMapPacket(new MsgMessage($"", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.FirstRightCorner).GetArray(stream));
                foreach (var client in Database.Server.ServerMaps[1125].Values)
                {
                    for (int x = 0; x < DescendingList.Length; x++)
                    {
                        ClanWarScrore element;
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
                foreach (var user in Database.Server.ServerMaps[1125].Values)
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
                var Clans = new List<Role.Instance.Clan>();
                foreach (var client in Database.Server.ServerMaps[1125].Values)
                {
                    if (client.Player.MyGuild != null)
                    {
                        if (!Clans.Contains(client.Player.MyClan))
                            Clans.Add(client.Player.MyClan);
                    }
                }
                foreach (var clan in Clans.OrderByDescending(x => x.Members.Count))
                {
                    SendMapPacket(new MsgMessage($"[ {clan.Name} ] - [ {Database.Server.ServerMaps[1125].Values.Count(x => x.Player.ClanUID == clan.ID)} ]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
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
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    foreach (IMapObj user in Database.Server.ServerMaps[1125].View.Roles(MapObjectType.Player, Pole.X, Pole.Y))
                    {
                        user.Send(Pole.GetArray(stream, false));
                    }
                }
            }
        }

        public bool SignUp(GameClient client, Packet stream)
        {
            if (MsgSchedules.ClanWar.Proces != ProcesType.Dead)
            {
                ushort x;
                x = 0;
                ushort y;
                y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, Map.ID, DynamicId);
                return true;
            }
            return false;
        }
    }
}
