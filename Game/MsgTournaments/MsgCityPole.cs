using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgTournaments
{
    using static TheChosenProject.Game.MsgServer.MsgMessage;
    using Coordinate = Tuple<uint, ushort, ushort>;

    public class MsgCityPole
    {
        public class StagesMap
        {
            public StageType StageType;
            public Role.GameMap Map;
            public uint DynamicID = 0;
            public Coordinate Locations;
        }

        public class CityPoleScore
        {
            public Role.Instance.Guild Guild;
            public string Name;
            public uint GuildID;
            public uint Score;
        }

        private bool MoveToNext = false;
        public Extensions.Counter NpcIDCounter = new Extensions.Counter(1000);
        public DateTime StampRound = new DateTime();
        public DateTime StampStage = new DateTime();
        public DateTime StampShuffleScore = new DateTime();
        public CityPoleScore Winner;
        public ConcurrentDictionary<uint, CityPoleScore> ScoreList;
        public List<uint> NpcIDs;
        public ConcurrentDictionary<StageType, StagesMap> Stage;
        public StageType StageType { get; set; }
        public ProcesType Process { get; set; }

        public Role.SobNpc Pole = new Role.SobNpc();

        public bool InWar(uint id)
        {
            var st = Stage.Values.OrderByDescending(e => e.Map.ID == id).ToArray();
            if (st != null)
                if (st.Length > 0)
                    return true;
            return false;
        }

        public uint GetWinnerPrize(Client.GameClient client)
        {
            uint value = client.Player.MyGuild.CityPolePoint * 10;
            return value;
        }

        public MsgCityPole()
        {
            StageType = StageType.None;
            Process = ProcesType.Dead;
            Stage = new ConcurrentDictionary<StageType, StagesMap>();
            Winner = new CityPoleScore() { Name = "None", Score = 100, GuildID = 0 };
            ScoreList = new ConcurrentDictionary<uint, CityPoleScore>();
            NpcIDs = new List<uint>();
        }

        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                NpcIDs.Clear();

                #region BuildMap

                //stage1
                Role.GameMap.EnterMap(1505);
                StagesMap st1 = new StagesMap()
                {
                    StageType = StageType.One,
                    Map = Server.ServerMaps[1505]
                };
                st1.DynamicID = st1.Map.GenerateDynamicID();
                st1.Locations = new Coordinate(st1.Map.ID, 162, 217);
                if (!Stage.TryAdd(st1.StageType, st1))
                    Console.WriteLine("Error Add::MsgCityPole::Stage >> " + st1.StageType.ToString());
                AddNpc(st1.StageType);
                Program.FreePkMap.Add(st1.Map.ID);
                //stage2
                Role.GameMap.EnterMap(1509);
                StagesMap st2 = new StagesMap()
                {
                    StageType = StageType.Tow,
                    Map = Server.ServerMaps[1509],
                };
                st2.DynamicID = st2.Map.GenerateDynamicID();
                st2.Locations = new Coordinate(st2.Map.ID, 081, 119);
                if (!Stage.TryAdd(st2.StageType, st2))
                    Console.WriteLine("Error Add::MsgCityPole::Stage >> " + st2.StageType.ToString());
                AddNpc(st2.StageType);
                Program.FreePkMap.Add(st2.Map.ID);
                //stage3
                Role.GameMap.EnterMap(1506);
                StagesMap st3 = new StagesMap()
                {
                    StageType = StageType.Three,
                    Map = Server.ServerMaps[1506],
                };
                st3.DynamicID = st3.Map.GenerateDynamicID();
                st3.Locations = new Coordinate(st3.Map.ID, 109, 124);
                if (!Stage.TryAdd(st3.StageType, st3))
                    Console.WriteLine("Error Add::MsgCityPole::Stage >> " + st3.StageType.ToString());
                AddNpc(st3.StageType);
                Program.FreePkMap.Add(st3.Map.ID);
                //stage4
                Role.GameMap.EnterMap(1508);
                StagesMap st4 = new StagesMap()
                {
                    StageType = StageType.Four,
                    Map = Server.ServerMaps[1508],
                };
                st4.DynamicID = st4.Map.GenerateDynamicID();
                st4.Locations = new Coordinate(st4.Map.ID, 117, 135);
                if (!Stage.TryAdd(st4.StageType, st4))
                    Console.WriteLine("Error Add::MsgCityPole::Stage >> " + st4.StageType.ToString());
                AddNpc(st4.StageType);
                Program.FreePkMap.Add(st4.Map.ID);
                //stage5
                Role.GameMap.EnterMap(1507);
                StagesMap st5 = new StagesMap()
                {
                    StageType = StageType.Five,
                    Map = Server.ServerMaps[1507],
                };
                st5.DynamicID = st5.Map.GenerateDynamicID();
                st5.Locations = new Coordinate(st5.Map.ID, 096, 113);
                if (!Stage.TryAdd(st5.StageType, st5))
                    Console.WriteLine("Error Add::MsgCityPole::Stage >> " + st5.StageType.ToString());
                AddNpc(st5.StageType);
                Program.FreePkMap.Add(st5.Map.ID);

                #endregion BuildMap

                Process = ProcesType.Alive;
                StageType = StageType.One;
                AddPole();
                StampRound = DateTime.Now.AddSeconds(3);
                foreach (var client in Server.GamePoll.Values)
                {
                    client.Player.MessageBox("CityPole began in [Stage (" + (uint)StageType + "/ 5)], Would you like to join?", new Action<Client.GameClient>(user => user.Teleport(315, 265, 1002)), null, 60);
                }
            }
        }

        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process != ProcesType.Dead)
            {
                if (user.Player.MyGuild != null)
                {
                    StagesMap stagesMap;
                    if (Stage.TryGetValue(StageType, out stagesMap))
                    {
                        ushort x = 0;
                        ushort y = 0;
                        stagesMap.Map.GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, stagesMap.Map.ID, stagesMap.DynamicID);
                        return true;
                    }
                }
                else user.CreateBoxDialog("You not have any guild.");
            }
            return false;
        }

        public void EndPole()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ShuffleGuildScores(true);
                if (ScoreList.Count > 0)
                {
                    if (ScoreList[Winner.GuildID].Guild != null)
                        ScoreList[Winner.GuildID].Guild.CityPolePoint++;
                }
                if (Winner.Name != "None")
                {
                    string msg = "#53 Congratulations to Guild " + Winner.Name + " have taken the Pole [Stage (" + (uint)StageType + "/ 5)] in CityPole and got +1Pts, Are they going to win?";
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                }
                RestartPole();
            }
        }

        internal unsafe void FinishStage()
        {
            if (Process == ProcesType.Alive)
            {
                Process = ProcesType.Idle;
                ShuffleGuildScores(true);
                if (ScoreList.Count > 0)
                {
                    if (ScoreList[Winner.GuildID].Guild != null)
                        ScoreList[Winner.GuildID].Guild.CityPolePoint += 3;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (Winner.Name != "None")
                    {
                        string msg = "#53 Congratulations to Guild " + Winner.Name + " won the CityPole in [Stage (" + (uint)StageType + "/ 5)] and got +3Pts, The CityPole moved to stage[" + (StageType + 1) + "].";
                        if (StageType == StageType.Five)
                            msg = "#53 Congratulations to Guild " + Winner.Name + " won the CityPole in [Stage (" + (uint)StageType + "/ 5)] and got +3Pts.";
                        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    }
                }
                ScoreList.Clear();
                StagesMap stagesMap;
                if (Stage.TryGetValue(StageType, out stagesMap))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        stagesMap.Map.View.LeaveMap<Role.IMapObj>(Pole);
                        stagesMap.Map.RemoveFlagNpc(Pole.X, Pole.Y);
                        ActionQuery action;
                        action = new ActionQuery()
                        {
                            ObjId = Pole.UID,
                            Type = ActionType.RemoveEntity
                        };
                        SendMapPacket(stream.ActionCreate(&action));
                    }
                }
                StampRound = DateTime.Now.AddSeconds(3);
                MoveToNext = true;
            }
        }

        public void ZeroScore()
        {
            ScoreList.Clear();
        }

        public void NextStage()
        {
            MoveToNext = false;
            if (StageType != StageType.Five)
            {
                StageType++;
                AddPole();
                Process = ProcesType.Alive;
                foreach (var client in Server.GamePoll.Values)
                {
                    client.Player.MessageBox($"CityPole began in [Stage ({(uint)StageType}/ 5)], Would you like to join ?", new Action<Client.GameClient>(user => Join(user, null)), null, 60);
                }
            }
            else
            {
                Process = ProcesType.Dead;
                StagesMap stages;
                if (Stage.TryGetValue(StageType, out stages))
                {
                    foreach (var client in stages.Map.View.GetAllMapRoles(MapObjectType.Player).ToArray())
                    {
                        var obj = client as Player;
                        obj.Owner.ExitToTwin();
                    }
                    StageType = StageType.None;
                }
            }
        }

        public void CheckUp()
        {
            if (Process != ProcesType.Dead)
            {
                DateTime now32 = DateTime.Now;
                if (now32 > StampRound && MoveToNext)
                {
                    NextStage();
                }
                if (now32 > StampStage)
                {
                    FinishStage();
                }
                if (now32 > StampShuffleScore)
                {
                    ShuffleGuildScores();
                }
            }
        }

        public bool InTournament(Client.GameClient user)
        {
            return false;
        }

        public void AddPole()
        {
            StagesMap stagesMap;
            if (Stage.TryGetValue(StageType, out stagesMap))
            {
                if (stagesMap.Map.View.Contain(123456, stagesMap.Locations.Item2, stagesMap.Locations.Item3))
                    return;
                Pole = new Role.SobNpc();
                Pole.Map = stagesMap.Map.ID;
                Pole.X = stagesMap.Locations.Item2;
                Pole.Y = stagesMap.Locations.Item3;
                Pole.ObjType = Role.MapObjectType.SobNpc;
                Pole.UID = 123456;
                Pole.DynamicID = stagesMap.DynamicID;
                Pole.Type = Role.Flags.NpcType.Stake;
                Pole.Mesh = (Role.SobNpc.StaticMesh)1137;
                Winner = new CityPoleScore() { Name = "None", Score = 100, GuildID = 0 };
                Pole.Name = Winner.Name;
                Pole.HitPoints = Pole.MaxHitPoints = 5000000;
                Pole.Sort = 17;
                stagesMap.Map.View.EnterMap<Role.IMapObj>(Pole);
                stagesMap.Map.SetFlagNpc(Pole.X, Pole.Y);
                StampStage = DateTime.Now.AddMinutes(1);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var user in stagesMap.Map.View.Roles(Role.MapObjectType.Player, Pole.X, Pole.Y))
                    {
                        user.Send(Pole.GetArray(stream, false));
                    }
                }
            }
        }

        public bool IsNpc(uint ID)
        {
            return NpcIDs.Contains(ID);
        }

        public void AddNpc(StageType stageType)
        {
            StagesMap stagesMap;
            if (Stage.TryGetValue(stageType, out stagesMap))
            {
                ushort x = 0, y = 0;
                switch (stageType)
                {
                    case StageType.One: { x = 153; y = 207; break; }
                    case StageType.Tow: { x = 83; y = 102; break; }
                    case StageType.Three: { x = 92; y = 124; break; }
                    case StageType.Four: { x = 99; y = 140; break; }
                    case StageType.Five: { x = 80; y = 111; break; }
                }
                SobNpc npc = new Role.SobNpc();
                npc.Map = stagesMap.Map.ID;
                npc.X = x;
                npc.Y = y;
                npc.ObjType = Role.MapObjectType.SobNpc;
                npc.UID = NpcIDCounter.Next;
                npc.DynamicID = stagesMap.DynamicID;
                npc.Type = Role.Flags.NpcType.Talker;
                npc.Mesh = (Role.SobNpc.StaticMesh)7217;
                npc.Name = "Exit";
                npc.HitPoints = npc.MaxHitPoints = 0;
                npc.Sort = 17;
                stagesMap.Map.View.EnterMap<Role.IMapObj>(npc);
                stagesMap.Map.SetFlagNpc(npc.X, npc.Y);
                NpcIDs.Add(npc.UID);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var user in stagesMap.Map.View.Roles(Role.MapObjectType.Player, npc.X, npc.Y))
                    {
                        user.Send(npc.GetArray(stream, false));
                    }
                }
            }
        }

        public void RestartPole()
        {
            if (Pole != null)
            {
                StagesMap stagesMap;
                if (Stage.TryGetValue(StageType, out stagesMap))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Pole.Name = Winner.Name;
                        Pole.HitPoints = Pole.MaxHitPoints = 5000000;
                        ScoreList.Clear();
                        SendMapPacket(Pole.GetArray(stream, false));
                    }
                }
            }
        }

        internal void UpdateScore(Role.Player client, uint Damage)
        {
            if (client.MyGuild == null)
                return;
            if (Process == ProcesType.Alive)
            {
                if (!ScoreList.ContainsKey(client.GuildID))
                {
                    ScoreList.TryAdd(client.GuildID, new CityPoleScore() { Guild = client.MyGuild, GuildID = client.MyGuild.Info.GuildID, Name = client.MyGuild.GuildName, Score = Damage });
                }
                else
                {
                    ScoreList[client.MyGuild.Info.GuildID].Score += Damage;
                }
                if (Pole.HitPoints == 0)
                    EndPole();
            }
        }

        internal unsafe void SendMapPacket(ServerSockets.Packet packet)
        {
            StagesMap stages;
            if (Stage.TryGetValue(StageType, out stages))
            {
                foreach (var client in stages.Map.View.GetAllMapRoles(MapObjectType.Player).ToArray())
                {
                    client.Send(packet);
                }
            }
        }

        internal unsafe void ShuffleGuildScores(bool createWinned = false)
        {
            if (Process != ProcesType.Dead)
            {
                DateTime now = DateTime.Now;
                StampShuffleScore = now.AddSeconds(1);
                StagesMap stages;
                if (Stage.TryGetValue(StageType, out stages))
                {
                    foreach (var client in stages.Map.View.GetAllMapRoles(MapObjectType.Player).ToArray())
                    {
                        var obj = client as Player;
                        obj.Owner.SendSysMesage($"*ScoreBoard-Stage {StageType}({(uint)StageType}/5).", ChatMode.FirstRightCorner);
                    }
                    uint score = 0;
                    foreach (var kvp in ScoreList.Values.OrderByDescending((s => s.Score)).ToArray())
                    {
                        if (kvp.Guild != null)
                        {
                            if (score < 4)
                            {
                                score++;
                                string msg = $"No {score}. {kvp.Name} ({kvp.Score}) Pts:({kvp.Guild.CityPolePoint})";
                                foreach (var client in stages.Map.View.GetAllMapRoles(MapObjectType.Player).ToArray())
                                {
                                    if (score == 1 && createWinned)
                                        Winner = kvp;
                                    var obj = client as Player;
                                    obj.Owner.SendSysMesage(msg, ChatMode.ContinueRightCorner);
                                }
                            }
                        }
                    }
                    if (StampStage > now)
                    {
                        TimeSpan T = TimeSpan.FromSeconds((StampStage - now).TotalSeconds);
                        string time = $"Time left {T.ToString(@"mm\:ss")}";
                        foreach (var client in stages.Map.View.GetAllMapRoles(MapObjectType.Player).ToArray())
                        {
                            var obj = client as Player;
                            obj.Owner.SendSysMesage($"------------------------------------", ChatMode.ContinueRightCorner);
                            obj.Owner.SendSysMesage(time, ChatMode.ContinueRightCorner);
                        }
                    }
                }
            }
        }
    }
}