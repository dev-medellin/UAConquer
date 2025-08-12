using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgPkWar
    {
        //public int RewardConquerPoints = (ServerKernel.TWO_SOC_RATE;
        public int FinishMinutes = 15;
        public bool Claimed;
        public ProcesType Mode;
        private DateTime FinishTimer;
        public bool Wasstarted;
        public uint WinnerUID;

        public MsgPkWar() => this.Mode = ProcesType.Dead;

        public void Open()
        {
            if (this.Mode != ProcesType.Dead)
                return;
            this.Mode = ProcesType.Idle;
            this.Claimed = false;
            this.Wasstarted = true;
            this.WinnerUID = 0;
            MsgSchedules.SendInvitation("WeeklyPK", 0 + " CPs", (ushort)452, (ushort)294, (ushort)1002, (ushort)0, 60);
            DateTime now = DateTime.Now;
            this.FinishTimer = now.AddMinutes((double)this.FinishMinutes);
            try
            {
                Dictionary<ushort, string> tournments = ITournamentsAlive.Tournments;
                now = DateTime.Now;
                string str = ": started at(" + now.ToString("H:mm)");
                tournments.Add((ushort)15, str);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not start WeeklyPk", true, LogType.WARNING);
            }
        }

        public bool AllowJoin() => this.Mode == ProcesType.Idle;

        public void CheckUp(bool finish = false)
        {
            if (!(this.Mode == ProcesType.Idle | finish))
                return;
            if (!(DateTime.Now > this.FinishTimer | finish))
                return;
            try
            {
                ITournamentsAlive.Tournments.Remove((ushort)15);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not finish Weekly Pk War", true, LogType.WARNING);
            }
            this.Mode = ProcesType.Dead;
            MsgSchedules.SendSysMesage("Weekly PkWar has finished!", MsgMessage.ChatMode.Center);
        }

        public bool IsFinished() => this.Mode == ProcesType.Dead;

        public bool TheLastPlayer()
        {
            return Server.GamePoll.Values.Where<GameClient>((Func<GameClient, bool>)(p => p.Player.Map == 1508U && p.Player.Alive)).Count<GameClient>() == 1;
        }

        public void GiveReward(GameClient client, Packet stream)
        {
            this.WinnerUID = client.Player.UID;
            IEventRewards.Add("WeeklyPK", 0u, 0u, Server.ItemsBase[723467u].Name + " + " + Server.ItemsBase[730006u].Name, "[" + client.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
            string str = "[EVENT]" + client.Player.Name + " was rewarded MegaMeteorScroll | DBScroll from WeeklyPK.";
            ServerDatabase.LoginQueue.Enqueue((object)str);
            if (client.Inventory.HaveSpace((byte)11))
            {
                //client.Inventory.Add(stream, 721169, 2, 0, 0, 0);//5xUltimatePack
                //client.Inventory.Add(stream, 722057, 5, 0, 0, 0);//4xPowerExpball
                //client.Inventory.Add(stream, 730003, 3, 0, 0, 0);//+3 stone
                client.Inventory.Add(stream, 720028, 1, 0, 0, 0);//MegaMetsPack
                client.Inventory.Add(stream, 720546, 1, 0, 0, 0);//MegaDBPack
            }
            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " , was rewarded MegaMeteorScroll | DBScroll from WeeklyPK.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));

            this.Claimed = true;
            this.AddTop(client);
            
            client.Teleport((ushort)430, (ushort)269, 1002);
        }

        public void AddTop(GameClient client)
        {
            if ((int)this.WinnerUID != (int)client.Player.UID)
                return;
            client.Player.AddFlag(MsgUpdate.Flags.WeeklyPKChampion, 2592000, false);
        }
    }
}
