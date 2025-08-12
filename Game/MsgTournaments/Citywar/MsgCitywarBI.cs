using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Client;

namespace TheChosenProject.Game.MsgTournaments
{
    public class CoordinateBI
    {
        public ushort X;
        public ushort Y;
        public uint Map;
        public CoordinateBI(uint Map, ushort X, ushort Y)
        {
            this.Map = Map;
            this.X = X;
            this.Y = Y;
        }
    }

    public class MsgCitywarBI
    {

        public bool SendInvitation = false;
        public class CitywarBIScrore
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
        //private Role.GameMap GuildWarMap;

        public ProcesType _p;
        public ProcesType Proces 
        {
            get { return _p; }
            set
            {
                _p = value;
            }
        }

        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures { get; set; }
        public ConcurrentDictionary<uint, CitywarBIScrore> ScoreList;
        public CitywarBIScrore Winner;
        public MsgCitywarBI()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
            ScoreList = new ConcurrentDictionary<uint, CitywarBIScrore>();
            Winner = new CitywarBIScrore() { Name = "None", Score = 100, GuildID = 0 };
        }
        public class coords
        {
            public ushort x;
            public ushort y;
        }
        public Dictionary<uint, coords> CoordsPole = new Dictionary<uint, coords>();
        public void ShowPole()
        {
            foreach (var pole in Furnitures.Values)
            {
                if (CoordsPole.TryGetValue(pole.UID, out var coord))
                {
                    pole.X = coord.x;
                    pole.Y = coord.y;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        pole.SendScrennPacket(pole.GetArray(stream, false));
                    }
                }
            }
        }

        public void HidePole()
        {
            foreach (var pole in Furnitures.Values)
            {
                if (!CoordsPole.ContainsKey(pole.UID))
                    CoordsPole.Add(pole.UID, new coords() { x = pole.X, y = pole.Y });
                pole.X = 0;
                pole.Y = 0;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    pole.SendScrennPacket(pole.GetArray(stream, false));
                }
            }
        }
        public unsafe void CreateFurnitures()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[19895].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 835));
        }
        internal unsafe void ResetFurnitures(ServerSockets.Packet stream)
        {

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == 19895)
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

        public bool Join(GameClient user, ServerSockets.Packet stream)
        {
            if (Proces == ProcesType.Alive)
            {
                ushort x;
                x = 0;
                ushort y;
                y = 0;
                Database.Server.ServerMaps[19895].GetRandCoord(ref x, ref y);
                user.Teleport(x, y, 19895, 0);
                return true;
            }
            return false;
        }

        internal unsafe void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == 19895 || client.Player.Map == 6001)
                {
                    client.Send(packet);
                }
            }
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
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("BirdIsland Pole war in CityWars NPC has started!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("BirdIsland Pole war in CityWars NPC has started!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
            }
        }

        internal unsafe void FinishRound()
        {
            ShuffleGuildScores(true);
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = Winner.Name;
            Proces = ProcesType.Idle;
            ScoreList.Clear();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the BirdIsland Pole round with a score of " + Winner.Score.ToString() + ""
                   , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the BirdIsland Pole round with a score of " + Winner.Score.ToString() + ""
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
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("BirdIsland Pole has began!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                    //Program.DiscordEventsAPI.Enqueue("BirdIsland CityWar Started");
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
                    ScoreList.TryAdd(client.GuildID, new CitywarBIScrore() { GuildID = client.MyGuild.Info.GuildID, Name = client.MyGuild.GuildName, Score = Damage });
                }
                else
                {
                    ScoreList[client.MyGuild.Info.GuildID].Score += Damage;
                }

                if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    FinishRound();
            }
        }
        internal unsafe void CompleteEndPoleDominationBI()
        {
            SendInvitation = false;
            ShuffleGuildScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                string msg = "";
                if (Winner.Name != "None" && Winner.Score != 100)
                    msg = "Congratulations to " + Winner.Name + ", they've won the BirdIsland Pole with a score of " + Winner.Score.ToString();
                else msg = "BirdIsland Pole has ended with no winner.";

                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
            }

            RewardLeader.Clear();
            Winner.LeaderReward = 1;
            //HidePole();
        }
        internal unsafe void ShuffleGuildScores(bool createWinned = false)
        {
            if (Proces != ProcesType.Dead)
            {
                StampShuffleScore = DateTime.Now.AddSeconds(8);
                var Array = ScoreList.Values.ToArray();
                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];
                    if (x == 0 && createWinned)
                        Winner = element;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score.ToString() + ")"
                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

                        SendMapPacket(msg.GetArray(stream));

                    }
                    if (x == 4)
                        break;
                }
            }
        }
    }
}
