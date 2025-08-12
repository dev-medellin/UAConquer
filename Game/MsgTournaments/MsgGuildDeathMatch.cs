using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game
{
    public class MsgGuildDeathMatch
    {
        public const int MaxHitPoints = 30000000;
        public const int PoleUID = 85977;
        public bool SendInvitation = false;

        private SobNpc Pole = null;

        private Tuple<ushort, ushort, ushort>[] Locations = new Tuple<ushort, ushort, ushort>[5]
        {
            new Tuple<ushort, ushort, ushort>(3060, 71,60),
            new Tuple<ushort, ushort, ushort>(3060, 47,83),
            new Tuple<ushort, ushort, ushort>(3060, 73,81),
            new Tuple<ushort, ushort, ushort>(3060, 46,59),
            new Tuple<ushort, ushort, ushort>(3060, 59,72)
        };

        public static string Files = "\\GuildDeathMatch.txt";

        private int currentLocation;

        private bool poleKilled;

        private GameMap currentMap;

        private Tuple<ushort, ushort, ushort> currentCoordinate;

        private uint prize;

        private Guild LastGuildWinner;
        public ProcesType Proces { get; set; }

        private ConcurrentDictionary<Guild, ulong> damages;

        private List<string> winners;

        public uint KillerGuildID
        {
            get
            {
                if (LastGuildWinner == null)
                    return 0u;
                return LastGuildWinner.Info.GuildID;
            }
        }
        //354,315
        public MsgGuildDeathMatch(uint prize)
        {
            currentLocation = -1;
            this.prize = prize;
            damages = new ConcurrentDictionary<Guild, ulong>();
            winners = new List<string>();
            File.Open(ServerKernel.CO2FOLDER + Files, FileMode.OpenOrCreate).Close();
            string[] array;
            array = File.ReadAllLines(ServerKernel.CO2FOLDER + Files);
            foreach (string name in array)
            {
                if (name.Length != 0)
                    winners.Add(name);
            }
            Pole = new SobNpc
            {
                HitPoints = 30000000,
                MaxHitPoints = 30000000,
                Mesh = (SobNpc.StaticMesh)8686,
                Name = "GuildDeathMatch Pole",
                Type = Flags.NpcType.Stake,
                ObjType = MapObjectType.SobNpc,
                Sort = 21,
                UID = PoleUID
            };
        }
        public bool Start = false;
        public void work(DateTime time64)
        {
            if (time64.DayOfWeek != ISchedule.Schedules[ISchedule.EventID.GuildDeathMatch].DayOfWeek && !ISchedule.Schedules[ISchedule.EventID.GuildDeathMatch].EveryDay)
                return;
            if (time64.Hour == ISchedule.Schedules[ISchedule.EventID.GuildDeathMatch].StartHour)
            {
                if ((long)time64.Minute % (long)ISchedule.Schedules[ISchedule.EventID.GuildDeathMatch].StartMinute == 0 && time64.Second < 3)
                {
                    if (Start)
                        return;
                    Start = true;
                    ushort ID;
                    ID = 0;
                    if (currentLocation != -1)
                    {
                        ID = (ushort)currentMap.ID;
                        removePole();
                    }
                    winners.Clear();
                    bool killedPole;
                    killedPole = poleKilled;
                    poleKilled = false;
                    LastGuildWinner = null;
                    currentLocation = 0;
                    currentCoordinate = Locations[currentLocation];
                    currentMap = Server.ServerMaps[currentCoordinate.Item1];
                    Pole.Map = currentMap.ID;
                    Pole.X = currentCoordinate.Item2;
                    Pole.Y = currentCoordinate.Item3;
                    SobNpc pole;
                    pole = Pole;
                    int hitPoints;
                    hitPoints = (Pole.MaxHitPoints = 30000000);
                    pole.HitPoints = hitPoints;
                    currentMap.View.EnterMap((IMapObj)Pole);
                    currentMap.SetFlagNpc(Pole.X, Pole.Y);
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream4;
                        stream4 = recycledPacket.GetStream();
                        foreach (IMapObj user in currentMap.View.Roles(MapObjectType.Player, Pole.X, Pole.Y))
                        {
                            user.Send(Pole.GetArray(stream4, false));
                        }
                    }
                    foreach (GameClient client in Server.GamePoll.Values)
                    {
                        client.Player.MessageBox("Guild DeathMatch began! Would you like to join?", delegate (GameClient p)
                        {
                            p.Teleport(354, 315, 1002); /*joinClient(p);*/
                        }, null);
                    }
                    if (LastGuildWinner != null)
                        ServerKernel.Log.SaveLog("Guild " + LastGuildWinner.GuildName + " won the City pole! The Guild DeathMatch pole moved!", true, LogType.DEBUG);
                    currentMap.WasPKFree = Program.FreePkMap.Contains(currentMap.ID);
                    if (!currentMap.WasPKFree)
                        Program.FreePkMap.Add(currentMap.ID);
                    using (RecycledPacket recycledPacket2 = new RecycledPacket())
                    {
                        Packet stream3;
                        stream3 = recycledPacket2.GetStream();
                        if (currentLocation != -1 && killedPole && LastGuildWinner != null && currentMap != null)
                            Program.SendGlobalPackets.Enqueue(new MsgMessage("Guild " + LastGuildWinner.GuildName + " won the City pole! The Guild DeathMatch pole moved!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Guild).GetArray(stream3));
                        else
                            Program.SendGlobalPackets.Enqueue(new MsgMessage("The City Pole moved!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Guild).GetArray(stream3));
                    }
                }
                if (time64.Second % 5 != 0)
                    return;
                IOrderedEnumerable<KeyValuePair<Guild, ulong>> array;
                array = damages.OrderByDescending((KeyValuePair<Guild, ulong> p) => p.Value);
                int Place;
                Place = 0;
                {
                    foreach (KeyValuePair<Guild, ulong> entry in array)
                    {
                        string str;
                        str = "No  " + (Place + 1) + ": " + entry.Key.GuildName + "(" + entry.Value + ")";
                        using (RecycledPacket recycledPacket3 = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket3.GetStream();
                            MsgMessage msg;
                            msg = new MsgMessage(str, MsgMessage.MsgColor.yellow, (Place == 0) ? MsgMessage.ChatMode.FirstRightCorner : MsgMessage.ChatMode.ContinueRightCorner);
                            SendMapPacket(msg.GetArray(stream2));
                        }
                        Place++;
                        if (Place == 4)
                            break;
                    }
                    return;
                }
            }
        }

        private void setWinner()
        {
            if (LastGuildWinner == null)
                return;
            if (poleKilled)
            {
                //using (RecycledPacket rec = new RecycledPacket())
                //{
                //    Packet stream;
                //    stream = rec.GetStream();
                //    Program.SendGlobalPackets.Enqueue(new MsgMessage("Guild " + LastGuildWinner.GuildName + " won the City pole!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Center).GetArray(stream));
                //}
                winners.Add(LastGuildWinner.GuildName);
                LastGuildWinner = null;
                SaveWinners();
            }
        }

        internal void SendMapPacket(Packet packet)
        {
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == currentMap.ID)
                    client.Send(packet);
            }
        }

        public void SaveWinners()
        {
            string Files;
            Files = "\\GuildDeathMatch.txt";
            StringBuilder builder;
            builder = new StringBuilder();
            foreach (string line in winners)
            {
                builder.AppendLine(line);
            }
            File.WriteAllText(ServerKernel.CO2FOLDER + Files, builder.ToString());
        }

        public uint GetWinnerPrize(string name)
        {
            uint count = (uint)winners.Count((string p) => p == name);
            return (uint)(prize * count);
        }

        public void RemoveWinner(string name)
        {
            winners.RemoveAll((string p) => p == name);
        }

        private void joinClient(GameClient client)
        {
            int x;
            x = Core.Random.Next(36) - 18;
            int y;
            y = Core.Random.Next(36) - 18;
            client.Teleport((ushort)(Pole.X + x), (ushort)(Pole.Y + y), currentMap.ID);
        }

        internal void KillPole()
        {
            KeyValuePair<Guild, ulong>[] array;
            array = damages.OrderByDescending((KeyValuePair<Guild, ulong> p) => p.Value).ToArray();
            if (array.Length != 0)
                LastGuildWinner = array[0].Key;
            string last_win = LastGuildWinner != null ? LastGuildWinner.GuildName : "";
            poleKilled = true;
            setWinner();
            damages.Clear();
            removePole();
            int curloc = currentLocation;
            curloc++;
            if (curloc >= Locations.Length)
            {
                Start = false;
                currentLocation = -1;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    if (poleKilled)
                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Guild " + last_win + " won the City pole!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Center).GetArray(stream));
                }
                foreach (var user in currentMap.Values)
                    user.Teleport(428, 378, 1002);
            }
            else
            {
                currentLocation++;
                currentCoordinate = Locations[currentLocation];
                currentMap = Server.ServerMaps[currentCoordinate.Item1];
                Pole.HitPoints = (Pole.MaxHitPoints = 30000000);
                Pole.Name = "ThePole";
                Pole.Map = currentMap.ID;
                Pole.X = currentCoordinate.Item2;
                Pole.Y = currentCoordinate.Item3;
                currentMap.View.EnterMap((IMapObj)Pole);
                currentMap.SetFlagNpc(Pole.X, Pole.Y);
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream4;
                    stream4 = recycledPacket.GetStream();
                    foreach (IMapObj user in currentMap.View.Roles(MapObjectType.Player, Pole.X, Pole.Y))
                    {
                        user.Send(Pole.GetArray(stream4, false));
                    }
                }
            }
        }

        public void removePole()
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                currentMap.RemoveSobNpc(Pole, stream);
            }
            if (!currentMap.WasPKFree)
                Program.FreePkMap.Remove(currentMap.ID);
            damages.Clear();
        }

        internal void AddScore(uint damage, Guild guild)
        {
            if (guild != null)
            {
                if (!damages.ContainsKey(guild))
                    damages[guild] = 0uL;
                damages[guild] += damage;
            }
        }
        public bool SignUp(GameClient client, Packet stream)
        {
            if (MsgSchedules.GuildDeathMatch.Start == true)
            {
                //client.Teleport(50, 50, 3060);
                int x;
                x = Core.Random.Next(36) - 18;
                int y;
                y = Core.Random.Next(36) - 18;
                client.Teleport((ushort)(Pole.X + x), (ushort)(Pole.Y + y), 3060);
                return true;
            }
            return false;
        }
    }
}
