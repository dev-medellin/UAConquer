using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

 
namespace TheChosenProject.Game
{
    public class MsgPoleDomination
    {
        public const int MaxHitPoints = 30000000;
        public const int PoleUID = 85976;

        public static SobNpc Pole = new SobNpc
        {
            HitPoints = 30000000,
            MaxHitPoints = 30000000,
            Mesh = (SobNpc.StaticMesh)8686,
            Name = "Domination Pole",
            Type = Flags.NpcType.Stake,
            ObjType = MapObjectType.SobNpc,
            Sort = 21,
            UID = PoleUID
        };

        private Tuple<ushort, ushort, ushort>[] Locations = new Tuple<ushort, ushort, ushort>[5]
        {
            new Tuple<ushort, ushort, ushort>(1002, 534, 357),
            new Tuple<ushort, ushort, ushort>(1011, 216, 328),
            new Tuple<ushort, ushort, ushort>(1020, 573, 633),
            new Tuple<ushort, ushort, ushort>(1000, 500, 707),
            new Tuple<ushort, ushort, ushort>(1015, 713, 631)
        };

        public static string Files = "\\PoleDomination.txt";

        private int currentLocation;

        private bool poleKilled;

        private GameMap currentMap;

        private Tuple<ushort, ushort, ushort> currentCoordinate;

        private uint prize;

        private Client.GameClient lastKiller;

        private List<string> winners;

        public uint KillerGuildID
        {
            get
            {
                if (lastKiller == null)
                    return 0;
                return lastKiller.Player.UID;
            }
        }

        public MsgPoleDomination(uint prize)
        {
            currentLocation = -1;
            this.prize = prize;
            winners = new List<string>();
            File.Open(ServerKernel.CO2FOLDER + Files, FileMode.OpenOrCreate).Close();
            string[] array;
            array = File.ReadAllLines(ServerKernel.CO2FOLDER + Files);
            foreach (string name in array)
            {
                if (name.Length != 0)
                    winners.Add(name);
            }
        }

        public bool InFight = false;
        public void GetRound()
        {
            if (!InFight)
            {
                InFight = true;
                SobNpc pole;
                pole = Pole;
                int hitPoints;
                ushort ID;
                ID = 0;
                if (currentLocation != -1)
                {
                    ID = (ushort)currentMap.ID;
                    removePole();
                }
                setWinner();
                bool killedPole;
                killedPole = poleKilled;
                poleKilled = false;
                lastKiller = null;
                currentLocation++;
                currentCoordinate = Locations[currentLocation];
                currentMap = Server.ServerMaps[currentCoordinate.Item1];
                Pole.Map = currentMap.ID;
                Pole.X = currentCoordinate.Item2;
                Pole.Y = currentCoordinate.Item3;
                pole = Pole;
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
                    client.Player.MessageBox("Pole Domination began in " + currentMap.Name + ". Would you like to join?", delegate (GameClient p)
                    {
                        joinClient(p);
                    }, null);
                }
                //ServerKernel.Log.SaveLog("PLayer " + lastKiller.Player.Name + " won the pole in " + currentMap.Name + "!The Pole Domination moved to " + currentMap.Name + "!", true, LogType.DEBUG);
                currentMap.WasPKFree = Program.FreePkMap.Contains(currentMap.ID);
                if (!currentMap.WasPKFree)
                    Program.FreePkMap.Add(currentMap.ID);
                using (RecycledPacket recycledPacket2 = new RecycledPacket())
                {
                    Packet stream3;
                    stream3 = recycledPacket2.GetStream();
                    if (currentLocation != -1 && killedPole && lastKiller != null && currentMap != null)
                        Program.SendGlobalPackets.Enqueue(new MsgMessage("PLayer " + lastKiller.Player.Name + " won the pole in " + currentMap.Name + "!The Pole Domination moved to " + currentMap.Name + "!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Guild).GetArray(stream3));
                    else Program.SendGlobalPackets.Enqueue(new MsgMessage("The Pole Domination moved to " + currentMap.Name + "!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Guild).GetArray(stream3));
                }
            }
        }

        public void work(DateTime time64)
        {
            if (time64.DayOfWeek != ISchedule.Schedules[ISchedule.EventID.PoleDomination].DayOfWeek && !ISchedule.Schedules[ISchedule.EventID.PoleDomination].EveryDay)
                return;
            if (time64.Minute == ISchedule.Schedules[ISchedule.EventID.PoleDomination].StartMinute)//xx:15
            {
                GetRound();
            }
            if (time64.Minute == ISchedule.Schedules[ISchedule.EventID.PoleDomination].StartMinute * 2)//xx:30
            {
                GetRound();
            }

        }

        private void setWinner()
        {
            if (poleKilled)
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Player " + lastKiller.Player.Name + " won the pole in " + currentMap.Name + "!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Center).GetArray(stream));
                }
                winners.Add(lastKiller.Player.Name);
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
            Files = "\\poledomination.txt";
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
            return (uint)(prize * winners.Count((string p) => p == name));
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

        internal void KillPole(GameClient killer)
        {
            lastKiller = killer;
            SobNpc pole;
            pole = Pole;
            int hitPoints;
            hitPoints = (Pole.MaxHitPoints = 30000000);
            pole.HitPoints = hitPoints;
            Pole.Name = lastKiller.Player.Name;
            lastKiller.Player.Money += (int)ServerKernel.POLE_DOMINATION_REWARD;
            IEventRewards.Add("PoleDomination", ServerKernel.POLE_DOMINATION_REWARD , 0, "", "[" + lastKiller.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
            RemoveWinner(lastKiller.Player.Name);
            lastKiller.Player.MessageBox("You have claimed " + prize.ToString("0,0") + " CPs Congratulations!", null, null);
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                foreach (IMapObj user in currentMap.View.Roles(MapObjectType.Player, Pole.X, Pole.Y))
                {
                    user.Send(Pole.GetArray(stream, false));
                }
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Player " + lastKiller.Player.Name + " have taken the pole in " + currentMap.Name + "! Are they going to win?", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Center).GetArray(stream));
            }
            poleKilled = true;
            InFight = false;
            if (currentCoordinate.Item1 != 1015)
            {
                GetRound();
            }
            else
            {
                currentLocation = -1;
                //setWinner();
                removePole();
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("The Pole Domination finish and Player " + lastKiller.Player.Name + " have taken the pole in " + currentMap.Name + ".", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
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
        }
    }
}
