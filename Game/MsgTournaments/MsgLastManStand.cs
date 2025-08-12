using TheChosenProject.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static TheChosenProject.Role.Flags;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgLastManStand : ITournament
    {
        public DateTime StartTimer;

        public DateTime InfoTimer;

        public uint Seconds = 60u;

        public static uint MapID;

        public static uint DinimycID;

        public GameMap Map;

        public uint DinamicMap;

        public KillerSystem KillSystem;

        public ProcesType Process { get; set; }

        public TournamentType Type { get; set; }

        public MsgLastManStand(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        public void Close()
        {
            GameClient[] values;
            values = Map.Values;
            foreach (GameClient user in values)
            {
                user.Teleport(430, 388, 1002u);
            }
            Process = ProcesType.Dead;
            try
            {
                ITournamentsAlive.Tournments.Remove(6);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not finish LastMan Pk War", true, LogType.WARNING);
            }
        }

        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                KillSystem = new KillerSystem();
                StartTimer = DateTime.Now;
                MsgSchedules.SendInvitation("Lastmanstanding", "ConquerPoints", 439, 392, 1002, 0, 60);
                if (Map == null)
                {
                    Map = Server.ServerMaps[700u];
                    DinamicMap = Map.GenerateDynamicID();
                }
                InfoTimer = DateTime.Now;
                Seconds = 60u;
                Process = ProcesType.Idle;
                MapID = Map.ID;
                DinimycID = DinamicMap;
                Program.FreePkMap.Add(Map.ID);
                try
                {
                    ITournamentsAlive.Tournments.Add(6, ": started at(" + DateTime.Now.ToString("H:mm)"));
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not start Last Man Standing", true, LogType.WARNING);
                }
            }
        }

        public bool Join(GameClient user, Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                ushort x;
                x = 0;
                ushort y;
                y = 0;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, Map.ID, DinamicMap);
                return true;
            }
            return false;
        }

        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddMinutes(1.0))
                {
                    MsgSchedules.SendSysMesage("ThroneSiege has started! signup are now closed.", MsgMessage.ChatMode.Center);
                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10.0))
                {
                    Seconds -= 10u;
                    MsgSchedules.SendSysMesage("[ThroneSiege] Fight starts in " + Seconds + " Seconds.", MsgMessage.ChatMode.Center);
                    InfoTimer = DateTime.Now;
                }
            }
            if (Process != ProcesType.Alive)
                return;
            if (DateTime.Now > StartTimer.AddMinutes(15.0) || MapPlayers().Length == 0)
            {
                GameClient[] array;
                array = MapPlayers();
                foreach (GameClient user in array)
                {
                    user.Teleport(430, 388, 1002u);
                }
                MsgSchedules.SendSysMesage("ThroneSiege has ended. All Players of LastManStand has teleported to TwinCity.", MsgMessage.ChatMode.Center);
                Process = ProcesType.Dead;
                try
                {
                    ITournamentsAlive.Tournments.Remove(6);
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not finish Last Man War", true, LogType.WARNING);
                }
            }
            if (MapCount() == 1)
            {
                GameClient winner;
                winner = Map.Values.Where((GameClient p) => p.Player.DynamicID == DinamicMap && p.Player.Map == Map.ID).FirstOrDefault();
                if (winner == null)
                    return;
                MsgSchedules.SendSysMesage(winner.Player.Name + " has Won  ThroneSiege. ", MsgMessage.ChatMode.BroadcastMessage, MsgMessage.MsgColor.white);
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    if (winner.Inventory.HaveSpace(1))
                        winner.Inventory.Add(stream, 720027u, 1, 0, 0, 0);
                    else
                        winner.Inventory.AddReturnedItem(stream, 720027u, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false, Flags.ItemEffect.None, 0);
                    IEventRewards.Add(Type.ToString(), 0, 0u, Server.ItemsBase[720027u].Name, "[" + winner.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
                    winner.SendSysMesage("You received +3Stone. ");
                    winner.Teleport(427, 388, 1002u);
                    Process = ProcesType.Dead;
                    try
                    {
                        ITournamentsAlive.Tournments.Remove(6);
                        return;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog("Could not finish Last Man", true, LogType.WARNING);
                        return;
                    }
                }
            }
            if (MapCount() == 0)
            {
                Process = ProcesType.Dead;
                try
                {
                    ITournamentsAlive.Tournments.Remove(6);
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not finish Last Man", true, LogType.WARNING);
                }
            }
        }

        public int MapCount()
        {
            return MapPlayers().Length;
        }

        public GameClient[] MapPlayers()
        {
            return Map.Values.Where((GameClient p) => p.Player.DynamicID == DinamicMap && p.Player.Map == Map.ID).ToArray();
        }

        public bool InTournament(GameClient user)
        {
            if (Map == null)
                return false;
            if (user.Player.Map == Map.ID)
                return user.Player.DynamicID == DinamicMap;
            return false;
        }
    }
}
