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
  public class MsgMonthlyPkWar
  {
    public int RewardConquerPoints = ServerKernel.MONTHLY_PK_REWARD;
    public int FinishMinutes = 15;
    public bool Claimed;
    public ProcesType Mode;
    private DateTime FinishTimer;
    public bool Wasstarted;
    public uint WinnerUID;

    public MsgMonthlyPkWar() => this.Mode = ProcesType.Dead;

    public void Open()
    {
      if (this.Mode != ProcesType.Dead)
        return;
      this.Mode = ProcesType.Idle;
      this.Claimed = false;
      this.Wasstarted = true;
      this.WinnerUID = 0;
      MsgSchedules.SendInvitation("MonthlyPK", this.RewardConquerPoints.ToString() + " CPs", (ushort) 452, (ushort) 294, (ushort) 1002, (ushort) 0, 60);
      DateTime now = DateTime.Now;
      this.FinishTimer = now.AddMinutes((double) this.FinishMinutes);
      try
      {
        Dictionary<ushort, string> tournments = ITournamentsAlive.Tournments;
        now = DateTime.Now;
        string str = ": started at(" + now.ToString("H:mm)");
        tournments.Add((ushort) 18, str);
      }
      catch
      {
        ServerKernel.Log.SaveLog("Could not start MonthlyPk", true, LogType.WARNING);
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
        ITournamentsAlive.Tournments.Remove((ushort) 18);
      }
      catch
      {
        ServerKernel.Log.SaveLog("Could not finish Monthly Pk War", true, LogType.WARNING);
      }
      this.Mode = ProcesType.Dead;
      MsgSchedules.SendSysMesage("Monthly PkWar has finished!", MsgMessage.ChatMode.Center);
    }

    public bool IsFinished() => this.Mode == ProcesType.Dead;

    public bool TheLastPlayer()
    {
      return Server.GamePoll.Values.Where<GameClient>((Func<GameClient, bool>) (p => p.Player.Map == 1505U && p.Player.Alive)).Count<GameClient>() == 1;
    }

    public void GiveReward(GameClient client, Packet stream)
    {
      this.WinnerUID = client.Player.UID;
            IEventRewards.Add("MonthlyPK", 0u, 0u, Server.ItemsBase[723467u].Name + " + " + Server.ItemsBase[730006u].Name, "[" + client.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
            string str = "[EVENT]" + client.Player.Name + " was rewarded 5xUltimatePack | 5xPowerExpball | 3xStone+5 | 5xMegaMeteorScrool | 3xMegaDBScrool from MonthlyPK.";
            ServerDatabase.LoginQueue.Enqueue((object)str);
            if (client.Inventory.HaveSpace((byte)21))
            {
                //client.Inventory.Add(stream, 721169, 5, 0, 0, 0);//5xUltimatePack
                //client.Inventory.Add(stream, 722057, 5, 0, 0, 0);//4xPowerExpball
                //client.Inventory.Add(stream, 730005, 3, 0, 0, 0);//+5 stone
                client.Inventory.Add(stream, 720547, 2, 0, 0, 0);//MegaMetsPack
                client.Inventory.Add(stream, 720028, 1, 0, 0, 0);//2xMegaDBPack
            }
            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " , was rewarded 2xMegaMeteorScrool | 1xDBScroll from MonthlyPK.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));

            this.Claimed = true;
      this.AddTop(client);
      client.Teleport((ushort) 430, (ushort) 269, 1002);
    }

    public void AddTop(GameClient client)
    {
      if ((int) this.WinnerUID != (int) client.Player.UID)
        return;
      client.Player.AddFlag(MsgUpdate.Flags.MonthlyPKChampion, 2592000, false);
    }
  }
}
