using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgGuildDeathMatch
    {
        public bool SendInvitation = false;
        internal ushort[][] StatueCoords =
        {
          new ushort[] {140 ,134 }
         ,new ushort[] {144 ,124 }
         ,new ushort[] {130 ,138 }
         ,new ushort[] {153 ,124 }
         ,new ushort[] {161 ,124 }
         ,new ushort[] {130 ,147 }
         ,new ushort[] {130 ,155 }
        };


        public class GuildDeathMatchScrore
        {
            public const int ConquerPointsReward = 100000;

            public uint GuildID;
            public string Name;
            public uint Score;

            //for reward
            public int LeaderReward = 1;
            //public int DeputiLeaderReward = 7;
        }

        public List<uint> RewardLeader = new List<uint>();
        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound = new DateTime();
        public DateTime StampShuffleScore = new DateTime();
        //private Role.GameMap Guild DeathMatchMap;

        public ProcesType Proces { get; set; }

        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures { get; set; }
        public ConcurrentDictionary<uint, GuildDeathMatchScrore> ScoreList;
        public GuildDeathMatchScrore Winner;
        public MsgGuildDeathMatch()
        {
            Proces = ProcesType.Dead;
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
            ScoreList = new ConcurrentDictionary<uint, GuildDeathMatchScrore>();
            Winner = new GuildDeathMatchScrore() { Name = "None", Score = 100, GuildID = 0 };
        }

        public unsafe void CreateFurnitures()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[2071].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 820));
        }
        internal unsafe void ResetFurnitures(ServerSockets.Packet stream)
        {

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            foreach (var client in Database.Server.GamePoll.Values)
            {
                if (client.Player.Map == 2071)
                {
                    foreach (var npc in Furnitures.Values)
                    {
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, npc.X, npc.Y) <= Role.SobNpc.SeedDistrance)
                        {
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, npc.UID, 2);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)npc.Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, npc.HitPoints);
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
                if (client.Player.Map == 2071 || client.Player.Map == 6001)
                {
                    client.Send(packet);
                }
            }
        }
        public const string ConnectionString = "Server=localhost;username=root;password=P@ssw0rd;database=bd;";

        internal unsafe void CompleteEndGuildDeathMatch()
        {
            SendInvitation = false;
            ShuffleGuildScores();
            Proces = ProcesType.Dead;
            ScoreList.Clear();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                string msg = "";
                if (Winner.Name != "None" && Winner.Score != 100)
                    msg = "Congratulations to " + Winner.Name + ", they've won the Guild DeathMatch with a score of " + Winner.Score.ToString();
                else msg = "Guild DeathMatch has ended with no winner.";
                //Program.DiscordEventsAPI.Enqueue($"``{msg}``");

                var stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
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
            RewardLeader.Clear();
            Winner.LeaderReward = 1;
            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (user.Player.Map == 2071)
                {
                    user.Teleport(428, 378, 1002);
                    MsgSchedules.SendSysMesage("Guild DeathMatch has ended. All Players of Guild DeathMatch has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.TopLeft, MsgServer.MsgMessage.MsgColor.red);
                }
            }
        }

        internal unsafe void Start()
        {
            Proces = ProcesType.Alive;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ResetFurnitures(stream);
                ScoreList.Clear();
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    client.Player.MessageBox("", new Action<Client.GameClient>(p => p.Teleport(423, 253, 1002, 0)), null, 60, MsgServer.MsgStaticMessage.Messages.GuildDeathMatch);

                }
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Guild DeathMatch war has started!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
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
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the Guild DeathMatch round with a score of " + Winner.Score.ToString() + ""
                   , MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Congratulations to " + Winner.Name + ", they've won the Guild DeathMatch round with a score of " + Winner.Score.ToString() + ""
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
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Guild DeathMatch has began!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
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
                    ScoreList.TryAdd(client.GuildID, new GuildDeathMatchScrore() { GuildID = client.MyGuild.Info.GuildID, Name = client.MyGuild.GuildName, Score = Damage });
                }
                else
                {
                    ScoreList[client.MyGuild.Info.GuildID].Score += Damage;
                }

                if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    FinishRound();
            }
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
