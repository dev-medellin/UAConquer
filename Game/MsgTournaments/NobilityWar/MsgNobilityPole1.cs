using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgNobilityPole1
    {
        public static SafeDictionary<uint, Info> NobilityPoleList = new SafeDictionary<uint, Info>();

        public static Info RoundOwner;

        public static ProcesType Proces = ProcesType.Dead;

        public static Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
        /// <summary>
        /// SobNpc 
        /// 2515,WarOfPlayers,10,1137,6521,95,113,20000000,20000000,17,1
        /// Npc 1002, 283, 290
        /// 6525,2,9696,1002,283,290
        /// </summary>
        public static ushort MapID = 22341;

        public class Info
        {
            public uint UID;
            public uint Score;
            public string Name;
        }

        public static void Load()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 22341));

        }

        public static void Start()
        {
            RoundOwner = null;
            NobilityPoleList = new SafeDictionary<uint, Info>();
            Proces = ProcesType.Alive;    
            
        }

        public static void End()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;
                NobilityPoleList.Clear();
            }
        }

        public static void CheckUP()
        {
            DateTime Now = DateTime.Now;

            if (Proces == ProcesType.Dead && Now.Hour == 22 && Now.Minute == 30 && Now.Second <= 1)
                Start();
            //using (var rec = new ServerSockets.RecycledPacket())
            //{
            //    var stream = rec.GetStream();
            //    Reset(stream);
            //}
            if (Proces == ProcesType.Alive)
            {
                if (Now.Hour == 22 && Now.Minute == 45 && Now.Second <= 2)
                    MsgSchedules.SendSysMesage("Nobility Tournaments will ended after 15 Minutes.", MsgServer.MsgMessage.ChatMode.Center);

                if (Now.Hour == 22 && Now.Minute == 50 && Now.Second <= 2)
                    MsgSchedules.SendSysMesage("Nobility Tournaments will ended after 10 Minutes.", MsgServer.MsgMessage.ChatMode.Center);
                if (Now.Hour == 23 && Now.Minute == 00 && Now.Second > 1)
                    End();
            }
        }

        public static void Reset(ServerSockets.Packet stream)
        {
            NobilityPoleList = new SafeDictionary<uint, Info>();

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
            var users = Database.Server.GamePoll.Values.Where(u => Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
            if (users != null)
            {
                foreach (var user in users)
                {
                    MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Pole.UID, 2);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Pole.Mesh);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Pole.HitPoints);
                    stream = upd.GetArray(stream);
                    user.Send(stream);
                    if ((Role.SobNpc.StaticMesh)Pole.Mesh == Role.SobNpc.StaticMesh.Pole)
                        user.Send(Pole.GetArray(stream, false));
                }
            }
        }
        public static void FinishRound(ServerSockets.Packet stream)
        {
            SortScores(true);
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.Name;
            MsgSchedules.SendSysMesage("The [Prince] Round Ownered by " + RoundOwner.Name + ".", MsgServer.MsgMessage.ChatMode.Center);
            Reset(stream);
        }
        public static void UpdateScore(ServerSockets.Packet stream, uint Score, Role.Player Player)
        {
            if (!NobilityPoleList.ContainsKey(Player.UID))
                NobilityPoleList.Add(Player.UID, new Info() { Name = Player.Name, UID = Player.UID, Score = Score });
            else
                NobilityPoleList[Player.UID].Score += Score;

            SortScores();

            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 1)
            {
                FinishRound(stream);
                return;
            }
        }
        private static void SortScores(bool getwinner = false)
        {
            if (Proces != ProcesType.Dead)
            {
                var Array = NobilityPoleList.Values.ToArray();
                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];
                    if (x == 0 && getwinner)
                    {
                        RoundOwner = element;
                    }
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        MsgMessage msg;
                        msg = new MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score + ")", MsgMessage.MsgColor.yellow, (x == 0) ? MsgMessage.ChatMode.FirstRightCorner : MsgMessage.ChatMode.ContinueRightCorner);
                        SendMapPacket(msg.GetArray(stream));
                    }
                    if (x == 4)
                        break;
                }
            }
        }
        public static void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.ServerMaps[MapID].Values)
                client.Send(packet);
        }
    }
}
