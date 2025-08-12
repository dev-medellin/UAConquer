using System;
using System.Collections.Generic;

 
namespace TheChosenProject.Game.MsgTournaments
{
  internal class ITournamentsAlive
  {
    public static Dictionary<ushort, string> Tournments = new Dictionary<ushort, string>();

    [Flags]
    public enum ID : ushort
    {
      None = 0,
      LastmanStand = 6,
      TreasureThief = 7,
      FindTheBox = 23,
      FB_SS_Tournament = 8,
      GuildWar = 9,
      EliteGuildWar = 10, // 0x000A
      ClanWar = 11, // 0x000B
      ClassPkWar = 12, // 0x000C
      ElitePkTournament = 13, // 0x000D
      CaptureTheFlag = 14, // 0x000E
      WeeklyPK = 15, // 0x000F
      TeamPkTournament = 16, // 0x0010
      SkillTeamPkTournament = 17, // 0x0011
      MonthlyPK = 18, // 0x0012
      HeroOfGame = 19, // 0x0013
      BossDomination = 25, // 0x0013            
        }
  }
}
