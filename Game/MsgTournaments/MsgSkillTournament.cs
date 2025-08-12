using Extensions;
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
  public class MsgSkillTournament : ITournament
  {
    public const uint MapID = 1505;
    public const uint RewardConquerPoints = 2000;
    public const uint MaxLifes = 3;
    public KillerSystem KillSystem;
    public DateTime StartTimer;
    public DateTime ScoreStamp;
    public DateTime InfoTimer;
    public GameMap Map;
    public uint DinamicID;
    public uint Secounds;

    public ProcesType Process { get; set; }

    public TournamentType Type { get; set; }

    public MsgSkillTournament(TournamentType _Type)
    {
      this.Process = ProcesType.Dead;
      this.Type = _Type;
    }

    public void Close()
    {
      foreach (GameClient gameClient in this.Map.Values)
        gameClient.Teleport((ushort) 430, (ushort) 388, 1002);
      this.Process = ProcesType.Dead;
      try
      {
        ITournamentsAlive.Tournments.Remove((ushort) 8);
      }
      catch
      {
        ServerKernel.Log.SaveLog("Could not finish Fb.SS Tournament", true, LogType.WARNING);
      }
    }

    public void Open()
    {
      if (this.Process != ProcesType.Dead)
        return;
      this.KillSystem = new KillerSystem();
      this.Map = Server.ServerMaps[1505U];
      this.DinamicID = this.Map.GenerateDynamicID();
      MsgSchedules.SendInvitation("SkillTournament", "ConquerPoints", (ushort) 439, (ushort) 394, (ushort) 1002, (ushort) 0, 60);
      this.StartTimer = DateTime.Now;
      this.InfoTimer = DateTime.Now;
      this.Secounds = 60;
      this.Process = ProcesType.Idle;
      Program.FreePkMap.Add(1505);
      try
      {
        ITournamentsAlive.Tournments.Add((ushort) 8, ": started at(" + DateTime.Now.ToString("H:mm)"));
      }
      catch
      {
        ServerKernel.Log.SaveLog("Could not start FB/SS Tournament", true, LogType.WARNING);
      }
    }

    public void Revive(Time32 Timer, GameClient user)
    {
      if (user.Player.Alive || this.Process == ProcesType.Dead || !this.InTournament(user) || !(user.Player.DeadStamp.AddSeconds(4) < Timer))
        return;
      user.Teleport((ushort) 427, (ushort) 388, 1002);
      user.CreateBoxDialog("You've been eliminated , good luck next time.");
    }

    public bool Join(GameClient user, Packet stream)
    {
      if (this.Process != ProcesType.Idle)
        return false;
      ushort x = 0;
      ushort y = 0;
      user.Player.SkillTournamentLifes = 3;
      this.Map.GetRandCoord(ref x, ref y);
      user.Teleport(x, y, this.Map.ID, this.DinamicID);
      user.Player.AddFlag(MsgUpdate.Flags.Freeze, 60, true);
      return true;
    }

    public void CheckUp()
    {
      if (this.Process == ProcesType.Idle)
      {
        if (DateTime.Now > this.StartTimer.AddMinutes(1.0))
        {
          MsgSchedules.SendSysMesage("SkillTournament has started! signup are now closed.", MsgMessage.ChatMode.Center);
          this.Process = ProcesType.Alive;
          this.StartTimer = DateTime.Now;
          foreach (GameClient mapPlayer in this.MapPlayers())
            mapPlayer.Player.RemoveFlag(MsgUpdate.Flags.Freeze);
        }
        else
        {
          if (!(DateTime.Now > this.InfoTimer.AddSeconds(10.0)))
            return;
          this.Secounds -= 10;
          MsgSchedules.SendSysMesage("[SkillTournament] Fight starts in " + this.Secounds.ToString() + " Secounds.", MsgMessage.ChatMode.Center);
          this.InfoTimer = DateTime.Now;
        }
      }
      else
      {
        if (this.Process != ProcesType.Alive || !(DateTime.Now > this.StartTimer.AddSeconds(5.0)))
          return;
        if (this.MapCount() == 1)
        {
          GameClient gameClient = ((IEnumerable<GameClient>) this.Map.Values).Where<GameClient>((Func<GameClient, bool>) (p => (int) p.Player.DynamicID == (int) this.DinamicID && (int) p.Player.Map == (int) this.Map.ID)).FirstOrDefault<GameClient>();
        
          MsgSchedules.SendSysMesage(gameClient.Player.Name + " has Won  SkillTournament. ", MsgMessage.ChatMode.BroadcastMessage, MsgMessage.MsgColor.white);
          using (RecycledPacket recycledPacket = new RecycledPacket())
          {
            Packet stream = recycledPacket.GetStream();
            if (gameClient.Inventory.HaveSpace((byte) 1))
              gameClient.Inventory.Add(stream, 720028);
            else
              gameClient.Inventory.AddReturnedItem(stream, 720028);
            //IEventRewards.Add(this.Type.ToString(), 0, 2000000, Server.ItemsBase[720028U].Name, "[" + gameClient.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
            gameClient.SendSysMesage("You received " + 2000000U.ToString() + " ConquerMoney and . ");
            gameClient.Teleport((ushort) 427, (ushort) 388, 1002);
            this.Process = ProcesType.Dead;
            try
            {
              ITournamentsAlive.Tournments.Remove((ushort) 8);
            }
            catch
            {
              ServerKernel.Log.SaveLog("Could not finish FB.SS Pk", true, LogType.WARNING);
            }
          }
        }
        else
        {
          if (this.MapCount() != 0)
            return;
          try
          {
            ITournamentsAlive.Tournments.Remove((ushort) 8);
          }
          catch
          {
            ServerKernel.Log.SaveLog("Could not finish FB.SS PK", true, LogType.WARNING);
          }
          this.Process = ProcesType.Dead;
        }
      }
    }

    public int MapCount() => this.MapPlayers().Length;

    public GameClient[] MapPlayers()
    {
      return ((IEnumerable<GameClient>) this.Map.Values).Where<GameClient>((Func<GameClient, bool>) (p => (int) p.Player.DynamicID == (int) this.DinamicID && (int) p.Player.Map == (int) this.Map.ID)).ToArray<GameClient>();
    }

    public bool InTournament(GameClient user)
    {
      return this.Map != null && (int) user.Player.Map == (int) this.Map.ID && (int) user.Player.DynamicID == (int) this.DinamicID;
    }
  }
}
