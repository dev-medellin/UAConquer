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



namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgScoresWar
    {
        public const uint MapID = 8250;
        public class GuildWarScrore
        {
            public uint GuildID;

            public string Name;

            public uint Score;

            public byte Rank;

            public int LeaderReward = 1;

            public int DeputiLeaderReward = 7;
        }

        public bool SendInvitation;


        public int ConquerPointsPrize = ServerKernel.ELITE_GUILD_WAR_Reward;

        public List<uint> RewardLeader = new List<uint>();

        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound;

        public DateTime StampShuffleScore;

        public ConcurrentDictionary<uint, GuildWarScrore> ScoreList;
        public ConcurrentDictionary<uint, GuildWarScrore> WinnerScores;

        public GuildWarScrore Winner;

        public GameMap Map;
        public ProcesType Proces { get; set; }

        public Dictionary<SobNpc.StaticMesh, SobNpc> Furnitures { get; set; }

        public MsgScoresWar()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<SobNpc.StaticMesh, SobNpc>();
            ScoreList = new ConcurrentDictionary<uint, GuildWarScrore>();
            WinnerScores = new ConcurrentDictionary<uint, GuildWarScrore>();
            Winner = new GuildWarScrore
            {
                Name = "None",
                Score = 100u,
                GuildID = 0u
            };
        }

        public const int MaxHitPoints = 30000000;
        public const int PoleUID = 82020;

        public static SobNpc Pole = new SobNpc
        {
            HitPoints = 30000000,
            MaxHitPoints = 30000000,
            Mesh = (SobNpc.StaticMesh)1137,
            Name = "ScoresWar",
            Type = Flags.NpcType.Pole,
            ObjType = MapObjectType.SobNpc,
            Sort = 21,
            UID = PoleUID
        };

        public void CreateFurnitures()
        {
            Map = Database.Server.ServerMaps[2072];
            //Furnitures.Add(SobNpc.StaticMesh.RightGate, Map.View.GetMapObject<SobNpc>(MapObjectType.SobNpc, 516877));
            //Furnitures.Add(SobNpc.StaticMesh.Pole, Server.ServerMaps[8250u].View.GetMapObject<SobNpc>(MapObjectType.SobNpc, 820u));
        }

        internal void ResetFurnitures(Packet stream)
        {
            //foreach (SobNpc npc2 in Furnitures.Values)
            //{
            //    npc2.HitPoints = npc2.MaxHitPoints;
            //}
            //Furnitures[SobNpc.StaticMesh.RightGate].Mesh = SobNpc.StaticMesh.RightGate;
            //foreach (SobNpc npc2 in Furnitures.Values)
            //{
            //    npc2.HitPoints = npc2.MaxHitPoints;
            //}
            //SobNpc rgate = Map.View.GetMapObject<SobNpc>(MapObjectType.SobNpc, 516977);
            //rgate.HitPoints = rgate.MaxHitPoints;
            //rgate.Mesh = SobNpc.StaticMesh.RightGate;
            Pole.HitPoints = Pole.MaxHitPoints;
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map != 2072)
                    continue;
                //foreach (SobNpc npc2 in Furnitures.Values)
                //{
                //    if (Core.GetDistance(client.Player.X, client.Player.Y, npc2.X, npc2.Y) <= 19)
                //    {
                //        var upd = new MsgUpdate(stream, rgate.UID, 2u);
                //        stream = upd.Append(stream, MsgUpdate.DataType.Mesh, (long)npc2.Mesh);
                //        stream = upd.Append(stream, MsgUpdate.DataType.Hitpoints, npc2.HitPoints);
                //        stream = upd.GetArray(stream);
                //        client.Send(stream);
                //    }
                //}
                //if (Core.GetDistance(client.Player.X, client.Player.Y, rgate.X, rgate.Y) <= 19)
                //{
                //    var upd = new MsgUpdate(stream, rgate.UID, 2u);
                //    stream = upd.Append(stream, MsgUpdate.DataType.Mesh, (long)rgate.Mesh);
                //    stream = upd.Append(stream, MsgUpdate.DataType.Hitpoints, rgate.HitPoints);
                //    stream = upd.GetArray(stream);
                //    client.Send(stream);
                //}
                if (Core.GetDistance(client.Player.X, client.Player.Y, Pole.X, Pole.Y) <= 19)
                {
                    var upd = new MsgUpdate(stream, Pole.UID, 2u);
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
                if (client.Player.Map == 2072)
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
            Winner = WinnerScores.Values.Where(x => x.Score > 0).OrderByDescending(x => x.Score).ThenBy(x => x.Rank).FirstOrDefault();
            WinnerScores.Clear();
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == 2072)
                {
                    client.Player.ReviveC = 0;
                    client.Player.Kills = 0;
                    client.Player.Death = 0;
                }
            }
            foreach (var client in Map.Values)
            {
                client.Teleport(50, 50, 1005);
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                string msg;
                msg = "";
                msg = ((!(Winner.Name != "None") || Winner.Score == 100) ? "Scores War has ended with no winner." : ("Congratulations to " + Winner.Name + ", they've won the Scores War with a score of " + Winner.Score));
                Packet stream;
                stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage(msg, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgMessage(msg, MsgMessage.MsgColor.white, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
            }
            removePole();
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
                SobNpc pole;
                int hitPoints;
                pole = Pole;
                Pole.Map = 2072;
                Pole.X = 126;
                Pole.Y = 125;
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
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Scores war has started!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                try
                {
                    ITournamentsAlive.Tournments.Add(10, ": started at(" + DateTime.Now.ToString("H:mm)"));
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not start Scores War", true, LogType.WARNING);
                }
            }
        }

        internal void FinishRound()
        {
            ShuffleGuildScores(true);
            if (!WinnerScores.ContainsKey(Winner.GuildID))
                WinnerScores.TryAdd(Winner.GuildID, new GuildWarScrore
                {
                    GuildID = Winner.GuildID,
                    Name = Winner.Name,
                    Score = 1,
                    Rank = (byte)(WinnerScores.Count + 1)
                });
            else
                WinnerScores[Winner.GuildID].Score++;
            Proces = ProcesType.Idle;
            ScoreList.Clear();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the Scores War round with a score of " + Winner.Score, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Congratulations to " + Winner.Name + ", they've won the Scores War round with a score of " + Winner.Score, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                ResetFurnitures(stream);
                foreach (var client in Map.Values)
                {
                    var rnd = Program.GetRandom.Next(100);
                    if (rnd < 25)
                        client.Teleport(44, 130, 2072);
                    else if (rnd < 50)
                        client.Teleport(44, 142, 2072);
                    else if (rnd < 75)
                        client.Teleport(212, 109, 2072);
                    else
                        client.Teleport(212, 125, 2072);
                }
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
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Scores War has began!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
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

                //if (Furnitures[SobNpc.StaticMesh.Pole].HitPoints == 0)
                //    FinishRound();

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
                    if (Now64.Hour == 22)
                        rawGold *= 1.5;

                    //// Split if too many attackers (optional)
                    //rawGold = rawGold / Math.Max(numberOfAttackers, 1, 10);

                    int goldEarned = (int)Math.Min(rawGold, 150);

                    client.Owner.Player.MyGuildMember.MoneyDonate += goldEarned;
                    client.Owner.Player.MyGuild.Info.SilverFund += goldEarned;
                    //client.Owner.Player.Money += goldEarned;
                    client.Owner.SendSysMesage($"You earn {goldEarned} Silver while hitting", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.yellow);
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        client.Owner.Player.MyGuild.SendThat(client.Owner.Player);
                        client.View.SendView(client.GetArray(stream, false), false);
                    }
                }
                else
                {
                    // Don't reward gold
                    client.Owner.SendSysMesage($"Too weak!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.yellow);
                }
            }
        }

        public void CheckUp()
        {
            if (Proces == ProcesType.Alive)
            {
                ShuffleGuildScores();
            }
            else
            {
                foreach (var client in Database.Server.ServerMaps[2072].Values)
                {
                    client.Teleport(50, 50, 1005);
                }
            }
        }

        internal void ShuffleGuildScores(bool createWinned = false)
        {
            if (Proces != ProcesType.Alive)
                return;
            StampShuffleScore = DateTime.Now.AddSeconds(3.0);
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
                        client.SendSysMesage("No " + (x + 1) + ". " + element.Name + " (" + element.Score + ")", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        if (x == 4) break;
                    }
                }
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage("Scores Boards", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                foreach (var killer in WinnerScores.Values.Where(x=> x.Score > 0).OrderByDescending(x=> x.Score).Take(5))
                {
                    SendMapPacket(new MsgMessage(killer.Name + " : " + killer.Score, MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"Players Kill Scores (Kill/Death)", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                var top = Map.Values.OrderByDescending(x => x.Player.Kills).ToArray();
                foreach (var killer in top.Take(5))
                {
                    SendMapPacket(new MsgMessage(killer.Player.Name + " : [ Kill : " + killer.Player.Kills + " ] / [ Death : " + killer.Player.Death + " ] ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"Players Revive Scores (Revive/Death)", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                top = Map.Values.Where(x => x.Player.ReviveC > 0).OrderByDescending(x => x.Player.ReviveC).ToArray();
                foreach (var killer in top.Take(5))
                {
                    SendMapPacket(new MsgMessage(killer.Player.Name + " [ Revived : " + killer.Player.ReviveC + " ] / [ Death : " + killer.Player.Death + " ]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
                SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                foreach (var user in Map.Values)
                {
                    if (AtributesStatus.IsWater(user.Player.Class))
                    {
                        user.SendSysMesage("Your Revive Score (Revive/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        user.SendSysMesage($"{user.Player.Name}- {user.Player.ReviveC}/{user.Player.Death}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                    }
                    else
                    {
                        user.SendSysMesage("Your Kills Score (Kills/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        user.SendSysMesage($"{user.Player.Name} - [Kill : {user.Player.Kills} ] / [Death : {user.Player.Death} ]", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
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
            if (MsgSchedules.ScoresWar.Proces != ProcesType.Dead)
            {
                var rnd = Program.GetRandom.Next(100);
                if (rnd < 25)
                    client.Teleport(44, 130, 2072);
                else if (rnd < 50)
                    client.Teleport(44, 142, 2072);
                else if (rnd < 75)
                    client.Teleport(212, 109, 2072);
                else
                    client.Teleport(212, 125, 2072);

               // client.Teleport(158, 052, 2072);
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
    }
}
