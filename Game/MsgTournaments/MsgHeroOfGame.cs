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
  public class MsgHeroOfGame : ITournament
  {
    public string EventName = "Hero Of Game";
    public uint DinamicMap;
    public uint MaxCount;
    public static uint MapID;
    public static uint DinimycID;
    public DateTime IdelTimer;
    public DateTime UpdateTimer;
    public GameMap Map;
    public KillerSystem KillSystem;

    public TournamentType Type { get; set; }

    public ProcesType Process { get; set; }

    public MsgHeroOfGame(TournamentType _type)
    {
      this.Type = _type;
      this.Process = ProcesType.Dead;
    }

    public void Close()
    {
      foreach (GameClient gameClient in this.Map.Values)
        gameClient.Teleport((ushort) 430, (ushort) 388, 1002);
      this.Process = ProcesType.Dead;
      try
      {
        ITournamentsAlive.Tournments.Remove((ushort) 19);
      }
      catch
      {
        ServerKernel.Log.SaveLog("Could not finish HeroOfGame", true, LogType.WARNING);
      }
    }

    public void Open()
    {
      if (this.Process != ProcesType.Dead && this.Process != ProcesType.None)
        return;
      this.KillSystem = new KillerSystem();
      foreach (GameClient gameClient in (IEnumerable<GameClient>) Server.GamePoll.Values)
        gameClient.Player.MessageBox(this.EventName + " is about to begin! Will you join it?", (Action<GameClient>) (p =>
        {
          using (RecycledPacket recycledPacket = new RecycledPacket())
          {
            Packet stream = recycledPacket.GetStream();
            this.Join(p, stream);
          }
        }), (Action<GameClient>) null, 120);
      MsgSchedules.SendSysMesage("[" + this.EventName + "]: Gather up heroes! You have 2 minutes to join!", MsgMessage.ChatMode.Center);
      if (this.Map == null)
      {
        this.Map = Server.ServerMaps[1507U];
        this.DinamicMap = this.Map.GenerateDynamicID();
        MsgHeroOfGame.MapID = this.Map.ID;
        MsgHeroOfGame.DinimycID = this.DinamicMap;
      }
      DateTime dateTime = DateTime.Now;
      this.IdelTimer = dateTime.AddMinutes(2.0);
      this.Process = ProcesType.Idle;
      try
      {
        Dictionary<ushort, string> tournments = ITournamentsAlive.Tournments;
        dateTime = DateTime.Now;
        string str = ": started at(" + dateTime.ToString("H:mm)");
        tournments.Add((ushort) 19, str);
      }
      catch
      {
        ServerKernel.Log.SaveLog("Could not start Hero Of Game", true, LogType.WARNING);
      }
    }

    public void CheckUp()
    {
      if (this.Process == ProcesType.None)
        return;
      switch (this.Process)
      {
        case ProcesType.Idle:
          if (DateTime.Now > this.IdelTimer)
          {
            this.IdelTimer = DateTime.Now;
            this.Process = ProcesType.Alive;
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
              Packet stream = recycledPacket.GetStream();
              foreach (GameClient mapPlayer in this.MapPlayers())
              {
                mapPlayer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "gamebegin");
                mapPlayer.Send(new MsgMessage("[" + this.EventName + "]: FIGHT", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
              }
            }
            MsgSchedules.SendSysMesage("[" + this.EventName + "]: Tournament has started! Battle on heroes!", MsgMessage.ChatMode.Qualifier);
            break;
          }
          this.Send(MsgHeroOfGame.Statue.CountDown);
          break;
        case ProcesType.Alive:
          this.Send(MsgHeroOfGame.Statue.RightCorner);
          if (this.MapPlayers().Length == 1)
          {
            GameClient gameClient = ((IEnumerable<GameClient>) this.MapPlayers()).First<GameClient>();
            MsgSchedules.SendSysMesage(gameClient.Player.Name + " has Won  " + this.EventName + " , he received " + 107500 + " ConquerMoney", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.white);
            gameClient.Player.Money += 107500;
            this.Process = ProcesType.Dead;
            try
            {
              IEventRewards.Add(this.Type.ToString(), 1075, 0, "", "[" + gameClient.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
              break;
            }
            catch
            {
              break;
            }
          }
          else
          {
            if (this.MapPlayers().Length != 0)
              break;
            MsgSchedules.SendSysMesage(this.EventName + " Tournament has ended as time has ran out with no winners.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.white);
            this.Process = ProcesType.Dead;
            break;
          }
        case ProcesType.Dead:
          foreach (GameClient mapPlayer in this.MapPlayers(true))
          {
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
              Packet stream = recycledPacket.GetStream();
              if (!mapPlayer.Player.Alive)
                mapPlayer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "sports_failure");
              else
                mapPlayer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "sports_victory");
            }
            mapPlayer.Teleport((ushort) 428, (ushort) 378, 1002);
            mapPlayer.Player.SetPkMode(TheChosenProject.Role.Flags.PKMode.Capture);
          }
          try
          {
            ITournamentsAlive.Tournments.Remove((ushort) 19);
          }
          catch
          {
            ServerKernel.Log.SaveLog("Could not finish HeroOfGame", true, LogType.WARNING);
          }
          MsgSchedules.SendSysMesage(this.EventName + " has ended. All Players has teleported to TwinCity.", MsgMessage.ChatMode.Center);
          this.Process = ProcesType.None;
          break;
      }
    }

    public bool Join(GameClient user, Packet stream)
    {
      if (this.Process != ProcesType.Idle)
        return false;
      user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "moveback");
      ushort x = 0;
      ushort y = 0;
      this.Map.GetRandCoord(ref x, ref y);
      user.Teleport(x, y, this.Map.ID, this.DinamicMap);
      this.MaxCount = (uint) this.MapPlayers().Length;
      user.Player.ProtectJumpAttack((int) (this.IdelTimer - DateTime.Now).TotalSeconds);
      user.Player.SetPkMode(TheChosenProject.Role.Flags.PKMode.PK);
            user.Player.FairbattlePower = TheChosenProject.Role.Flags.FairbattlePower.UpdateToSerf;
            user.SendSysMesage(string.Format("You've been sent to the PK Area and will need to wait until the start. just {0} Seconds", (object) (this.IdelTimer - DateTime.Now).TotalSeconds));
      return true;
    }

    public GameClient[] MapPlayers(bool kickout = false)
    {
      return ((IEnumerable<GameClient>) this.Map.Values).Where<GameClient>((Func<GameClient, bool>) (p => (((int) p.Player.DynamicID != (int) this.DinamicMap ? 0 : ((int) p.Player.Map == (int) this.Map.ID ? 1 : 0)) & (kickout ? 1 : 0)) == 0 ? p.Player.Alive : p.Player.Level > (ushort) 1)).ToArray<GameClient>();
    }

    public unsafe void Send(MsgHeroOfGame.Statue status)
    {
      foreach (GameClient mapPlayer in this.MapPlayers())
      {
        switch (status)
        {
          case MsgHeroOfGame.Statue.CountDown:
            if (DateTime.Now < this.UpdateTimer.AddSeconds(2.0))
              return;
            this.UpdateTimer = DateTime.Now;
            ActionQuery actionQuery = new ActionQuery();
            actionQuery.ObjId = mapPlayer.Player.UID;
            actionQuery.Type = ActionType.CountDown;
            actionQuery.dwParam = (uint) (this.IdelTimer - DateTime.Now).TotalSeconds;
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
              Packet stream = recycledPacket.GetStream();
              mapPlayer.Send(stream.ActionCreate(&actionQuery));
              break;
            }
          case MsgHeroOfGame.Statue.RightCorner:
            if (DateTime.Now < this.UpdateTimer.AddSeconds(5.0))
              return;
            this.UpdateTimer = DateTime.Now;
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
              Packet stream = recycledPacket.GetStream();
              mapPlayer.Send(new MsgMessage("<------- " + this.EventName + " -------->", MsgMessage.MsgColor.red, MsgMessage.ChatMode.FirstRightCorner).GetArray(stream));
              mapPlayer.Send(new MsgMessage("Player Count: " + this.MapPlayers().Length.ToString() + " / " + this.MaxCount.ToString() + " Players", MsgMessage.MsgColor.red, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
              break;
            }
        }
      }
    }

    public bool InTournament(GameClient user)
    {
      return this.Map != null && (int) user.Player.Map == (int) this.Map.ID && (int) user.Player.DynamicID == (int) this.DinamicMap;
    }

    public enum Statue
    {
      CountDown,
      RightCorner,
    }
  }
}
