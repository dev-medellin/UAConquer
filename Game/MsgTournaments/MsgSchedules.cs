using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using TheChosenProject.Game.MsgServer.AttackHandler;
using TheChosenProject.Database;
//using TheChosenProject.Game.MsgEvents;
using System.Threading;
using TheChosenProject.Client;
using TheChosenProject.ServerCore;
using TheChosenProject.ServerSockets;
using Extensions;
using TheChosenProject.Game.MsgEvents;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Role;
using System.IO;
using TheChosenProject.Mobs;
using DevExpress.XtraEditors;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgSchedules
    {
        public static Time32 Stamp = Time32.Now.AddMilliseconds(1000);

        public static Dictionary<TournamentType, ITournament> Tournaments = new Dictionary<TournamentType, ITournament>();

        public static ITournament CurrentTournament;

        internal static MsgGuildWar GuildWar;

        internal static MsgEliteGuildWar EliteGuildWar;

        internal static MsgClanWar ClanWar;

        internal static MsgArena Arena;

        internal static MsgPoleDomination PoleDomination;
        internal static MsgSmallCityGuilWar SmallCityGuilWar;
        internal static MsgGuildDeathMatch GuildDeathMatch;
        internal static MsgCityPole MsgCityPole;
        internal static MsgMsConquer MsConquer;
        internal static MsgMrConquer MrConquer;
        internal static MsgTeamArena TeamArena;
        internal static MsgCouples CouplesPKWar;

        internal static MsgClassPKWar ClassPkWar;

        internal static MsgEliteTournament ElitePkTournament;

        internal static MsgTeamPkTournament TeamPkTournament;

        internal static MsgSkillTeamPkTournament SkillTeamPkTournament;

        internal static MsgCaptureTheFlag CaptureTheFlag;

        internal static MsgMonthlyPkWar MonthlyPKWar;
        internal static MsgScoresWar ScoresWar;

        //internal static MsgCitywarAC CitywarAC;
        //internal static MsgCitywarBI CitywarBI;
        //internal static MsgCitywarDC CitywarDC;
        internal static MsgCitywarPC CitywarPC;
        internal static MsgCitywarTC CitywarTC;

        internal static MsgCityWar CityWar;

        internal static MsgPkWar PkWar;
        internal static MsgSquama Squama;   
        static bool hideNSeek = false;
        internal static DateTime NextLavaCheck;

        //public static void SpawnLavaBeast()
        //{
        //    Random Rand = new Random();
        //    var Map = Database.Server.ServerMaps[2056];
        //    ushort x = 0;
        //    ushort y = 0;
        //    Map.GetRandCoord(ref x, ref y);
        //    using (var rec = new ServerSockets.RecycledPacket())
        //    {
        //        var stream = rec.GetStream();
        //        string msg = "LavaBeast has spawned in FrozenGrotto! Hurry find it and kill it.";
        //        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
        //        Database.Server.AddMapMonster(stream, Map, 20055, (ushort)x, (ushort)y, 1, 1, 1);
        //        Console.WriteLine($"LavaBeast location: {x}, {y}");
        //    }
        //}
        private static List<string> SystemMsgs = new List<string> {
"Selling/trading cps outside the game will lead to your accounts banned forever.",
"Join our discord group to be in touch with the community and suggest/report stuff.",
"Administrators have [GM/PM] in their names,do not trust anyone else claiming to be a [GM/PM].",
"Refer our server and gain rewards! (contact GM/PM).",
"Thanks for supporting us! we will keep on working to provide the best for you!",
"Check out Guide in TwinCity for information about the game.",
"Sharing accounts is done at your own risk. You alone are responsible for your own accounts, Support will not be given on cases for shared accounts.",
"Always treat the STAFF of UAConquer with the utmost respect. No insulting/cursing about them or the server.",
"It's forbidden to advertise any other servers. Your account will be permanently banned without prior notice/warning, Repeated offenses will result in your IP Address being permanently banned.",
"It's forbidden to abuse bugs or any kind of bug/glitch found in the game, If a player discovers a bug/glitch in the game, it must be reported in Facebook or to the first STAFF member you can find.",
"It's forbidden to use Bots/Hacks/Cheats in-game. If you find any working Bots/Hacks/Cheats please report them to our STAFF.",
"Mouse clickers are allowed as long as you're not away-from-keyboard. If you're found using any mouse clicker or macro while away you'll be botjailed.",
"Only English is allowed in the world chat.",
"Selling/Trading accounts/items/gold outside the game for real life currencies, for items in other servers or for any other exchange or just the attempt of doing so, will result in all your accounts being permanently banned."        };
        
        internal static void Create()
        {
            Tournaments.Add(TournamentType.None, new MsgNone(TournamentType.None));
            //Tournaments.Add(TournamentType.FindTheBox, new MsgFindTheBox(TournamentType.FindTheBox));
            CurrentTournament = Tournaments[TournamentType.None];
            GuildWar = new MsgGuildWar();
            EliteGuildWar = new MsgEliteGuildWar();
            ClanWar = new MsgClanWar();
            Arena = new MsgArena();
            TeamArena = new MsgTeamArena();
            ClassPkWar = new MsgClassPKWar(ProcesType.Dead);
            //PoleDomination = new MsgPoleDomination(ServerKernel.POLE_DOMINATION_REWARD);
            //SmallCityGuilWar = new MsgSmallCityGuilWar(ServerKernel.SmallCityGuilWar_REWARD);
            GuildDeathMatch = new MsgGuildDeathMatch(ServerKernel.Guild_DeathMatch_REWARD);
            //MsConquer = new MsgMsConquer(ProcesType.Dead);
            //MrConquer = new MsgMrConquer(ProcesType.Dead);
            ElitePkTournament = new MsgEliteTournament();
            CaptureTheFlag = new MsgCaptureTheFlag();
            PkWar = new MsgPkWar();
            MonthlyPKWar = new MsgMonthlyPkWar();
            TeamPkTournament = new MsgTeamPkTournament();
            SkillTeamPkTournament = new MsgSkillTeamPkTournament();
            //Squama = new MsgSquama();
            //MsgCityPole = new MsgCityPole();
            //CitywarAC = new MsgCitywarAC();
            //CitywarBI = new MsgCitywarBI();
            //CitywarDC = new MsgCitywarDC();
            CitywarPC = new MsgCitywarPC();
            CitywarTC = new MsgCitywarTC();
            CityWar = new MsgCityWar();
            ScoresWar = new MsgScoresWar();
            //CouplesPKWar = new MsgCouples();

            MsgBroadcast.Create();
        }

        internal static void SendInvitation(string Name, string Prize, ushort X, ushort Y, ushort map, ushort DinamicID, int Seconds, MsgStaticMessage.Messages messaj = MsgStaticMessage.Messages.None)
        {
            string Message;
            Message = " " + Name + " is about to begin! Will you join it? Prize[" + Prize + "]";
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Packet packet;
                packet = new MsgMessage(Message, MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.Center).GetArray(stream);
                foreach (GameClient client in Server.GamePoll.Values)
                {
                    client.Send(packet);
                    client.Player.MessageBox(Message, delegate (GameClient user)
                    {
                        user.Teleport(X, Y, map, DinamicID);
                    }, null, Seconds, messaj);
                }
            }
        }

        internal static void SendSysMesage(string Messaj, MsgMessage.ChatMode ChatType = MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor color = MsgMessage.MsgColor.red, bool SendScren = false)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Packet packet;
                packet = new MsgMessage(Messaj, color, ChatType).GetArray(stream);
                foreach (GameClient client in Server.GamePoll.Values)
                {
                    client.Send(packet);
                    Messaj = Translator.GetTranslatedString(Messaj, Translator.Language.EN, client.Language);
                }
            }
        }

        private static bool GetLastWeekdayOfMonth(DateTime date, DayOfWeek day)
        {
            DateTime lastDayOfMonth;
            lastDayOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1.0);
            int lastDay;
            lastDay = (int)lastDayOfMonth.DayOfWeek;
            lastDayOfMonth.AddDays((double)((lastDay >= (int)day) ? (day - lastDay) : (day - lastDay - 7)));
            return date.Day == lastDayOfMonth.Day;
        }

        public static string GetLastWeekdayOfMonthSTR(DateTime date, DayOfWeek day)
        {
            DateTime lastDayOfMonth;
            lastDayOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1.0);
            int lastDay;
            lastDay = (int)lastDayOfMonth.DayOfWeek;
            lastDayOfMonth.AddDays((double)((lastDay >= (int)day) ? (day - lastDay) : (day - lastDay - 7)));
            return $"{lastDayOfMonth.DayOfWeek} {lastDayOfMonth.Day}";
        }

        public static DateTime WaterLordStillTime = DateTime.Now;
        public static DateTime WaterLordTime = DateTime.Now;

        public static DateTime SnakeManStillTime = DateTime.Now;
        public static DateTime SnakeManLastDeathTime = DateTime.Now;
        public static DateTime SnakeManTime = DateTime.Now;

        public static DateTime Lab1BossStillTime = DateTime.Now;
        public static DateTime Lab1BossLastDeathTime = DateTime.Now;
        public static DateTime Lab1BossTime = DateTime.Now;

        public static DateTime GandormaStillTime = DateTime.Now;
        public static DateTime GandormaTime = DateTime.Now;

        public static DateTime TitanStillTime = DateTime.Now;
        public static DateTime TitanTime = DateTime.Now;
        public static bool SpawnDevil = false;


        internal static void CheckUp(Time32 clock)
        {
            if (!(clock > Stamp))
                return;
            DateTime Now64;
            Now64 = DateTime.Now;
            if (!Server.FullLoading)
                return;
            #region Events list
            if (Program.Events != null)
                foreach (Game.MsgEvents.Events E in Program.Events.ToList())
                    E.ActionHandler();
            #endregion
            #region Nobilitywars
            //MsgNobilityPole.CheckUP();
            //MsgNobilityPole1.CheckUP();
            //MsgNobilityPole2.CheckUP();
            //MsgNobilityPole3.CheckUP();
            #endregion
            #region Tournaments
            //if (Now64.Minute == 1 && Now64.Second < 1)
            //{
            //    if (!SkillsTournament.Started)
            //        new SkillsTournament();
            //    Console.WriteLine("Started Event SkillsTournament at " + Now64);

            //}
            //if (Now64.Minute == 16 && Now64.Second < 1)
            //{
            //    if (!TeamDeathMatch.Started)
            //        new TeamDeathMatch();
            //    Console.WriteLine("Started Event TeamDeathMatch at " + Now64);

            //}
            //if (Now64.Minute == 31 && Now64.Second < 1)
            //{
            //    if (!Get5HitOut.Started)
            //        new Get5HitOut();
            //    Console.WriteLine("Started Event Get5HitOut at " + Now64);

            //}
            //if (Now64.Minute == 46 && Now64.Second < 1)
            //{
            //    if (!PassTheBomb.Started)
            //        new PassTheBomb();
            //    Console.WriteLine("Started Event PassTheBomb at " + Now64);

            //}
            //if (Now64.Minute == 56 && Now64.Second < 1)
            //{
            //    if (!LastManStanding.Started)
            //        new LastManStanding();
            //    Console.WriteLine("Started Event LastManStanding at " + Now64);

            //}
            //if (Now64.Minute == 56 && Now64.Second < 1)
            //{
            //    if (!LastManStanding.Started)
            //        new LastManStanding();
            //    Console.WriteLine("Started Event LastManStanding at " + Now64);

            //}
            #endregion
            #region HourlyTournament BattleField DropParty SantaClaus FootBall
            HourlyTournament.Tick(Now64);
            #endregion
            #region Arena
            try
            {
                if (Arena.Proces == ProcesType.Dead)
                    Arena.Proces = ProcesType.Alive;
            }
            catch
            {
                ServerKernel.Log.SaveLog("Arena Theards undercontrol", false, LogType.WARNING);
            }
            #endregion
            #region Squama
                //try
                //{
                //    if (Now64.Minute % 30 == 30 && Now64.Second < 2)
                //    {
                //        Squama.Open();
                //    }
                //}
                //catch
                //{
                //    ServerKernel.Log.SaveLog("Squama Theards undercontrol", false, LogType.WARNING);
                //}
            #endregion
            #region TeamArena
                //try
                //{
                //    if (TeamArena.Proces == ProcesType.Dead)
                //        TeamArena.Proces = ProcesType.Alive;
                //}
                //catch
                //{
                //    ServerKernel.Log.SaveLog("TeamArena Theards undercontrol", false, LogType.WARNING);
                //}
            #endregion
            #region CaptureTheFlag
                //try
                //{
                //    if (CaptureTheFlag.Proces == ProcesType.Alive)
                //    {
                //        CaptureTheFlag.UpdateMapScore();
                //        CaptureTheFlag.CheckUpX2();
                //        CaptureTheFlag.SpawnFlags();
                //    }
                //}
                //catch
                //{
                //    ServerKernel.Log.SaveLog("CaptureTheFlag Theards undercontrol", false, LogType.WARNING);
                //}
            #endregion
            #region Rand Tournament 
            //if (DateTime.Now.Minute == Now64.TimeOfDay. && Now64.Second < 0.5 || DateTime.Now.Minute == TimePanel.HTournaments_Min_2 && Now64.Second < 0.5 || DateTime.Now.Minute == TimePanel.HTournaments_Min_3 && Now64.Second < 0.5 || DateTime.Now.Minute == TimePanel.HTournaments_Min_4 && Now64.Second < 0.5)
            //{
            //    byte _totalEvents = 11;
            //    int _nextEvent = Program.Rnd.Next(0, _totalEvents);
            //    while (Program.WorldEvent == _nextEvent)
            //        _nextEvent = Program.Rnd.Next(0, _totalEvents);

            //    Program.WorldEvent = _nextEvent;
            //    TheChosenProject.Game.MsgEvents.Events NextEvent = new TheChosenProject.Game.MsgEvents.Events();
            //    switch (_nextEvent)
            //    {
            //        case 0:
            //            NextEvent = new CycloneRace();
            //            break;
            //        case 1:
            //            NextEvent = new DragonWar();
            //            break;
            //        case 2:
            //            NextEvent = new KillTheCaptain();
            //            break;
            //        case 3:
            //            NextEvent = new Get5Out();
            //            break;
            //        case 4:
            //            NextEvent = new KillTheCaptain();
            //            break;
            //        case 5:
            //            NextEvent = new LastManStand();
            //            break;
            //        case 6:
            //            NextEvent = new CrazyWar();
            //            break;
            //        case 7:
            //            NextEvent = new Spacelnvasion();
            //            break;
            //        case 8:
            //            NextEvent = new KOTH();
            //            break;
            //        case 9:
            //            NextEvent = new Vampire_War();
            //            break;
            //        case 10:
            //            NextEvent = new TDM();
            //            break;

            //    }
            //    NextEvent.StartTournament();
            //    Console.WriteLine("Started Event " + NextEvent.EventTitle + " at " + Now64);


            //}
            #endregion
            #region WeeklyPk
                //try
                //{
                //    PkWar.CheckUp();
                //}
                //catch
                //{
                //    ServerKernel.Log.SaveLog("WeeklyPk Theards undercontrol", false, LogType.WARNING);
                //}
            #endregion
            #region MonthlyPKWar
                //try
                //{
                //    MonthlyPKWar.CheckUp();
                //}
                //catch
                //{
                //    ServerKernel.Log.SaveLog("MonthlyPKWar Theards undercontrol", false, LogType.WARNING);
                //}
            #endregion
            #region SmallCityGuilWar
            //try
            //{
            //    SmallCityGuilWar.work(Now64);
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("SmallCityGuilWar Theards undercontrol", false, LogType.WARNING);
            //}
            #endregion
            #region PoleDomination
            //try
            //{
            //    PoleDomination.work(Now64);
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("PoleDomination Theards undercontrol", false, LogType.WARNING);
            //}
            #endregion

            if (ScoresWar.Proces == ProcesType.Idle)
                ScoresWar.Began();
            ScoresWar.CheckUp();
            if (Now64.Hour == 22 && (DateTime.Now.DayOfWeek == DayOfWeek.Monday))
            {
                if (ScoresWar.Proces == ProcesType.Dead)
                    ScoresWar.Start();
                if (!ScoresWar.SendInvitation)
                {
                    MsgSchedules.SendInvitation("ScoreWar", " Start 21:00 and End 23:00 , x2 DBscroll , x5 MegeMetScroll and 10,000,000 GOLD", 388, 315, 1002, 0, 60);
                    ScoresWar.SendInvitation = true;
                }
            }
            else
            {
                if (ScoresWar.Proces != ProcesType.Dead)
                    ScoresWar.CompleteEndGuildWar();
            }
            //if (Now64.Hour == 15)
            //{
            //    CityWar.Start();
            //}
            //else CityWar.Finish();

            //CityWar.CheckUp();
            //#region MrConquer
            ////if (Now64.Hour == 2 && (Now64.Minute >= 0 && Now64.Minute <= 5))/*EU*/
            ////{
            ////    MrConquer.Start();
            ////    //Program.DiscordEventsAPI.Enqueue("``MrConquer Event is about to start!``");

            ////}
            ////if (Now64.Hour == 2 && (Now64.Minute >= 0 && Now64.Minute <= 5))/*EU*/
            ////{
            ////    MsConquer.Start();
            ////    //Program.DiscordEventsAPI.Enqueue("``MsConquer Event is about to start!``");

            ////}
            //#endregion
            #region CitywarTC
            if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.CitywarTC].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.CitywarTC].EveryDay)
            {
                if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarTC].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarTC].StartMinute && Now64.Second < 2)
                {
                    if (CitywarTC.Proces == ProcesType.Dead)
                        CitywarTC.Start();
                    if (CitywarTC.SendInvitation == false)
                    {
                        SendInvitation("Twin City WAR", "is about to begin! Will you join it?", 359, 314, 1002, 0, 60, MsgStaticMessage.Messages.Citywar);
                        CitywarTC.SendInvitation = true;
                    }
                }
                if (CitywarTC.Proces == ProcesType.Idle && Now64 > CitywarTC.StampRound)
                    CitywarTC.Began();
                if (CitywarTC.Proces != ProcesType.Dead && DateTime.Now > CitywarTC.StampShuffleScore)
                    CitywarTC.ShuffleScores();
                if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarTC].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarTC].EndMinute && (CitywarTC.Proces == ProcesType.Alive || CitywarTC.Proces == ProcesType.Idle))
                    CitywarTC.CompleteEndPoleDominationTC();
            }
            else
            {
                CitywarTC.CheckUp();
            }
            //if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.CitywarTC].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.CitywarTC].EveryDay)
            //{
                //if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarTC].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarTC].StartMinute && Now64.Second < 2)
                //{
                //    if (CityWar.Proces == ProcesType.Dead)
                //        CityWar.Start();
                //    if (CityWar.SendInvitation == false)
                //    {
                //        SendInvitation("City WAR", "is about to begin! Will you join it?", 354, 323, 1002, 0, 60, MsgStaticMessage.Messages.Citywar);
                //        CityWar.SendInvitation = true;
                //    }
                //    //if (CitywarTC.Proces == ProcesType.Dead)
                //    //    CitywarTC.Start();
                //    //if (CitywarTC.SendInvitation == false)
                //    //{
                //    //    SendInvitation("Twin City WAR", "is about to begin! Will you join it?", 354, 323, 1002, 0, 60, MsgStaticMessage.Messages.Citywar);
                //    //    CitywarTC.SendInvitation = true;
                //    //}
                //}
                //if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarTC].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarTC].EndMinute && Now64.Second < 2)
                //{
                //    CityWar.Finish();
                //}
                //else
                //{
                //    CityWar.CheckUp();
                //}
                //if (CitywarTC.Proces == ProcesType.Idle && Now64 > CitywarTC.StampRound)
                //    CitywarTC.Began();
                //if (CitywarTC.Proces != ProcesType.Dead && DateTime.Now > CitywarTC.StampShuffleScore)
                //    CitywarTC.ShuffleGuildScores();
                //if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarTC].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarTC].EndMinute && (CitywarTC.Proces == ProcesType.Alive || CitywarTC.Proces == ProcesType.Idle))
                //    CitywarTC.CompleteEndPoleDominationTC();

            //}



            #endregion

            #region CitywarPC

            if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.CitywarPC].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.CitywarPC].EveryDay)
            {
                if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarPC].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarPC].StartMinute && Now64.Second < 2)
                {
                    if (CitywarPC.Proces == ProcesType.Dead)
                         CitywarPC.Start();
                    if (CityWar.SendInvitation == false)
                        {
                            SendInvitation("Phoenix City WAR", "is about to begin! Will you join it?", 263, 264, 1011, 0, 60, MsgStaticMessage.Messages.Citywar);
                            CitywarPC.SendInvitation = true;
                        }
                }
            if (CitywarPC.Proces == ProcesType.Idle && Now64 > CitywarPC.StampRound)
                    CitywarPC.Began();
                if (CitywarPC.Proces != ProcesType.Dead && DateTime.Now > CitywarPC.StampShuffleScore)
                    CitywarPC.ShuffleScores();
                if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarPC].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarPC].EndMinute && (CitywarPC.Proces == ProcesType.Alive || CitywarPC.Proces == ProcesType.Idle))
                    CitywarPC.CompleteEndPoleDominationPC();
            }
            else
            {
                CitywarPC.CheckUp();
            }
            #endregion
            #region CitywarAC
            //if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.CitywarAC].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.CitywarAC].EveryDay)
            //{
            //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarAC].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarAC].StartMinute && Now64.Second < 2)
            //    {
            //        if (CitywarAC.Proces == ProcesType.Dead)
            //            CitywarAC.Start();
            //        if (CitywarAC.SendInvitation == false)
            //        {
            //            SendInvitation("Ape City WAR", "is about to begin! Will you join it?", 354, 323, 1002, 0, 60, MsgStaticMessage.Messages.Citywar);
            //            CitywarAC.SendInvitation = true;
            //        }
            //    }
            //    if (CitywarAC.Proces == ProcesType.Idle && Now64 > CitywarAC.StampRound)
            //        CitywarAC.Began();
            //    if (CitywarAC.Proces != ProcesType.Dead && DateTime.Now > CitywarAC.StampShuffleScore)
            //        CitywarAC.ShuffleGuildScores();
            //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarAC].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarAC].EndMinute && (CitywarAC.Proces == ProcesType.Alive || CitywarAC.Proces == ProcesType.Idle))
            //        CitywarAC.CompleteEndPoleDominationAC();
            //}
            #endregion
            #region CitywarDC
            //if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.CitywarDC].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.CitywarDC].EveryDay)
            //{
            //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarDC].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarDC].StartMinute && Now64.Second < 2)
            //    {
            //        if (CitywarDC.Proces == ProcesType.Dead)
            //            CitywarDC.Start();
            //        if (CitywarDC.SendInvitation == false)
            //        {
            //            SendInvitation("Desert City WAR", "is about to begin! Will you join it?", 354, 323, 1002, 0, 60, MsgStaticMessage.Messages.Citywar);
            //            CitywarDC.SendInvitation = true;
            //        }
            //    }
            //    if (CitywarDC.Proces == ProcesType.Idle && Now64 > CitywarDC.StampRound)
            //        CitywarDC.Began();
            //    if (CitywarDC.Proces != ProcesType.Dead && DateTime.Now > CitywarDC.StampShuffleScore)
            //        CitywarDC.ShuffleGuildScores();
            //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarDC].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarDC].EndMinute && (CitywarDC.Proces == ProcesType.Alive || CitywarDC.Proces == ProcesType.Idle))
            //        CitywarDC.CompleteEndPoleDominationDC();
            //}

            #endregion
            #region CitywarBI
            //if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.CitywarBI].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.CitywarBI].EveryDay)
            //{
            //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarBI].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarBI].StartMinute && Now64.Second < 2)
            //    {
            //        if (CitywarBI.Proces == ProcesType.Dead)
            //            CitywarBI.Start();
            //        if (CitywarBI.SendInvitation == false)
            //        {
            //            SendInvitation("Bird City WAR", "is about to begin! Will you join it?", 354, 323, 1002, 0, 60, MsgStaticMessage.Messages.Citywar);
            //            CitywarBI.SendInvitation = true;
            //        }
            //    }
            //    if (CitywarBI.Proces == ProcesType.Idle && Now64 > CitywarBI.StampRound)
            //        CitywarBI.Began();
            //    if (CitywarBI.Proces != ProcesType.Dead && DateTime.Now > CitywarBI.StampShuffleScore)
            //        CitywarBI.ShuffleGuildScores();
            //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CitywarBI].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CitywarBI].EndMinute && (CitywarBI.Proces == ProcesType.Alive || CitywarBI.Proces == ProcesType.Idle))
            //        CitywarBI.CompleteEndPoleDominationBI();
            //}

            #endregion
            #region GuildDeathMatch
            //try
            //{
            //    GuildDeathMatch.work(Now64);
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("GuildDeathMatch Theards undercontrol", false, LogType.WARNING);
            //}
            #endregion
            #region DiscordAPI
            try
            {
                if (Now64.Minute % 10 == 0 && Now64.Second > 58)
                {
                    string rndMsg;
                    rndMsg = SystemMsgs[ServerKernel.NextAsync(0, SystemMsgs.Count)];
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(rndMsg, "ALLUSERS", "[DSConquer System]", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream2));
                    }
                    //Program.DiscordOnlineAPI.Enqueue($"`[UAConquer] Total Online: {Server.GamePoll.Count} - Max Online: {KernelThread.MaxOnline}`");

                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not connected to DiscordAPI", false, LogType.WARNING);
            }
            #endregion
            #region DungeonDomination 6:32
            //try
            //{
            //    bool righthour = DateTime.Now.Hour == 6;
            //    bool rightmin = DateTime.Now.Minute == 32;
            //    bool rightsec = DateTime.Now.Second == 1;
            //    var GMap = Database.Server.ServerMaps[3080];
            //    var mobs = GMap.View.GetAllMapRoles(MapObjectType.Monster).Where(p => (p as MonsterRole).Family.ID == 19896 && p.Alive);

            //    if (righthour && rightmin && rightsec)
            //    {
            //        if (mobs.Count() == 0)
            //        {
            //            for (int i = 0; i < 1; i++)
            //            {
            //                using (RecycledPacket rec = new RecycledPacket())
            //                {
            //                    Packet stream;
            //                    stream = rec.GetStream();
            //                    Database.Server.AddMapMonster(stream, GMap, 19896, 62, 72, 1, 1, 1);
            //                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Dungeon Bosses have spawned! Hurry to kill them.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //                    SendSysMesage("The Dungeon Bosses have spawned! Hurry to kill them.", ChatMode.System);
            //                }
            //                foreach (var user in Database.Server.GamePoll.Values)
            //                    user.Player.MessageBox("The Dungeon Bosses have spawned! Hurry to kill them.", new Action<Client.GameClient>(p =>{p.Teleport(369, 328, 1002);}), null, 60);
            //            }
            //        }                    
            //    }
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("Could not do DungeonDomination", false, LogType.WARNING);
            //}
            #endregion
            #region DungeonDomination 16:30
            //try
            //{
            //    bool righthour = DateTime.Now.Hour == 16;
            //    bool rightmin = DateTime.Now.Minute == 32;
            //    bool rightsec = DateTime.Now.Second == 1;
            //    var GMap = Database.Server.ServerMaps[3080];
            //    var mobs = GMap.View.GetAllMapRoles(MapObjectType.Monster).Where(p => (p as MonsterRole).Family.ID == 19896 && p.Alive);

            //    if (righthour && rightmin && rightsec)
            //    {
            //        if (mobs.Count() == 0)
            //        {
            //            for (int i = 0; i < 1; i++)
            //            {
            //                using (RecycledPacket rec = new RecycledPacket())
            //                {
            //                    Packet stream;
            //                    stream = rec.GetStream();
            //                    Database.Server.AddMapMonster(stream, GMap, 19896, 62, 72, 1, 1, 1);
            //                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Dungeon Bosses have spawned! Hurry to kill them.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //                    SendSysMesage("The Dungeon Bosses have spawned! Hurry to kill them.", ChatMode.System);
            //                }
            //                foreach (var user in Database.Server.GamePoll.Values)
            //                    user.Player.MessageBox("The Dungeon Bosses have spawned! Hurry to kill them.", new Action<Client.GameClient>(p => { p.Teleport(386, 326, 1002); }), null, 60);
            //            }
            //        }
            //    }
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("Could not do DungeonDomination", false, LogType.WARNING);
            //}
            #endregion
            #region ExpMob
            //if (Now64.Hour == 12 && Now64.Minute == 11 && Now64.Second < 1 ||
            //    Now64.Hour == 15 && Now64.Minute == 11 && Now64.Second < 1 ||
            //    Now64.Hour == 18 && Now64.Minute == 11 && Now64.Second < 1 ||
            //    Now64.Hour == 02 && Now64.Minute == 11 && Now64.Second < 1 ||//pm
            //    Now64.Hour == 0 && Now64.Minute == 11 && Now64.Second < 1 ||
            //    Now64.Hour == 3 && Now64.Minute == 11 && Now64.Second < 1 ||
            //    Now64.Hour == 6 && Now64.Minute == 11 && Now64.Second < 1 ||
            //    Now64.Hour == 9 && Now64.Minute == 11 && Now64.Second < 1)
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        string msg = "In 50 minutes, the EXP Mob will emerge!";
            //        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "ExpMob", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //    }
            //if (Now64.Hour == 12 && Now64.Minute == 21 && Now64.Second < 1 ||
            //    Now64.Hour == 15 && Now64.Minute == 21 && Now64.Second < 1 ||
            //    Now64.Hour == 18 && Now64.Minute == 21 && Now64.Second < 1 ||
            //    Now64.Hour == 02 && Now64.Minute == 21 && Now64.Second < 1 ||//pm
            //    Now64.Hour == 0 && Now64.Minute == 21 && Now64.Second < 1 ||
            //    Now64.Hour == 3 && Now64.Minute == 21 && Now64.Second < 1 ||
            //    Now64.Hour == 6 && Now64.Minute == 21 && Now64.Second < 1 ||
            //    Now64.Hour == 9 && Now64.Minute == 21 && Now64.Second < 1)
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        string msg = "In 40 minutes, the EXP Mob will emerge!";
            //        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "ExpMob", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //    }
            //if (Now64.Hour == 12 && Now64.Minute == 31 && Now64.Second < 1 ||
            //    Now64.Hour == 15 && Now64.Minute == 31 && Now64.Second < 1 ||
            //    Now64.Hour == 18 && Now64.Minute == 31 && Now64.Second < 1 ||
            //    Now64.Hour == 02 && Now64.Minute == 31 && Now64.Second < 1 ||//pm
            //    Now64.Hour == 0 && Now64.Minute == 31 && Now64.Second < 1 ||
            //    Now64.Hour == 3 && Now64.Minute == 31 && Now64.Second < 1 ||
            //    Now64.Hour == 6 && Now64.Minute == 31 && Now64.Second < 1 ||
            //    Now64.Hour == 9 && Now64.Minute == 31 && Now64.Second < 1)
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        string msg = "In 30 minutes, the EXP Mob will emerge!";
            //        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "ExpMob", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //    }
            //if (Now64.Hour == 12 && Now64.Minute == 41 && Now64.Second < 1 ||
            //   Now64.Hour == 15 && Now64.Minute == 41 && Now64.Second < 1 ||
            //   Now64.Hour == 18 && Now64.Minute == 41 && Now64.Second < 1 ||
            //   Now64.Hour == 02 && Now64.Minute == 41 && Now64.Second < 1 ||//pm
            //   Now64.Hour == 0 && Now64.Minute == 41 && Now64.Second < 1 ||
            //   Now64.Hour == 3 && Now64.Minute == 41 && Now64.Second < 1 ||
            //   Now64.Hour == 6 && Now64.Minute == 41 && Now64.Second < 1 ||
            //   Now64.Hour == 9 && Now64.Minute == 41 && Now64.Second < 1)
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        string msg = "In 20 minutes, the EXP Mob will emerge!";
            //        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "ExpMob", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //    }
            //if (Now64.Hour == 12 && Now64.Minute == 51 && Now64.Second < 1 ||
            //   Now64.Hour == 15 && Now64.Minute == 51 && Now64.Second < 1 ||
            //   Now64.Hour == 18 && Now64.Minute == 51 && Now64.Second < 1 ||
            //   Now64.Hour == 02 && Now64.Minute == 51 && Now64.Second < 1 ||//pm
            //   Now64.Hour == 0 && Now64.Minute == 51 && Now64.Second < 1 ||
            //   Now64.Hour == 3 && Now64.Minute == 51 && Now64.Second < 1 ||
            //   Now64.Hour == 6 && Now64.Minute == 51 && Now64.Second < 1 ||
            //   Now64.Hour == 9 && Now64.Minute == 51 && Now64.Second < 1)
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        string msg = "In 10 minutes, the EXP Mob will emerge!";
            //        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "ExpMob", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //    }
            //if (Now64.Hour == 12 && Now64.Minute == 55 && Now64.Second < 1 ||
            //   Now64.Hour == 15 && Now64.Minute == 55 && Now64.Second < 1 ||
            //   Now64.Hour == 18 && Now64.Minute == 55 && Now64.Second < 1 ||
            //   Now64.Hour == 02 && Now64.Minute == 55 && Now64.Second < 1 ||//pm
            //   Now64.Hour == 0 && Now64.Minute == 55 && Now64.Second < 1 ||
            //   Now64.Hour == 3 && Now64.Minute == 55 && Now64.Second < 1 ||
            //   Now64.Hour == 6 && Now64.Minute == 55 && Now64.Second < 1 ||
            //   Now64.Hour == 9 && Now64.Minute == 55 && Now64.Second < 1)
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        string msg = "In 5 minutes, the EXP Mob will emerge!";
            //        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "ExpMob", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //    }
            //if (Now64.Hour == 13 && Now64.Minute == 01 && Now64.Second < 1 ||
            //    Now64.Hour == 16 && Now64.Minute == 01 && Now64.Second < 1 ||
            //    Now64.Hour == 19 && Now64.Minute == 01 && Now64.Second < 1 ||
            //    Now64.Hour == 02 && Now64.Minute == 01 && Now64.Second < 1 ||//pm
            //    Now64.Hour == 01 && Now64.Minute == 01 && Now64.Second < 1 ||
            //    Now64.Hour == 04 && Now64.Minute == 01 && Now64.Second < 1 ||
            //    Now64.Hour == 07 && Now64.Minute == 01 && Now64.Second < 1 ||
            //    Now64.Hour == 10 && Now64.Minute == 01) { }

            //if( Now64.Second > 30)//am
            //{
            //    Random RandExpMob = new Random();
            //    var X = RandExpMob.Next(0, 2);
            //    if (X == 1)
            //    {
            //        var map = Database.Server.ServerMaps[1004];
            //        if (!map.ContainMobID(21060))
            //        {

            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                Database.Server.AddMapMonster(stream, Server.ServerMaps[1004], 21060, 50, 49, 1, 1, 1);
            //            }

            //            SendSysMesage("EXP Mob has appeared inside the Promotion Area at 51,49!", MsgServer.MsgMessage.ChatMode.TopLeft, MsgServer.MsgMessage.MsgColor.red);
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                string msg = "EXP Mob has appeared inside the Promotion Area at 51,49! ";
            //                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "[ExpMob]", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

            //            }
            //            Console.WriteLine("EXP Mob has appeared at Promotion Area at 51,49 at " + Now64 + "");
            //        }
            //    }

            //}
            #endregion
            #region Boss

            //if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday) && Now64.Hour % 4 == 0 && Now64.Minute == 15 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.SnowBanshee);

            //if ((DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) && Now64.Hour % 4 == 0 && Now64.Minute == 25 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.TeratoDragon);

            //if ((DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) && Now64.Hour % 4 == 0 && Now64.Minute == 35 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.NemesisTyrant);

            //if ((DateTime.Now.DayOfWeek == DayOfWeek.Thursday) && Now64.Hour % 4 == 0 && Now64.Minute == 45 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.ThrillingSpook);

            //if ((DateTime.Now.DayOfWeek == DayOfWeek.Friday) && Now64.Hour % 4 == 0 && Now64.Minute == 55 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.SowrdMaster);

            //if (Now64.Minute == 15 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.SowrdMaster);

            //if (Now64.Minute == 25 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.TeratoDragon);

            //if (Now64.Minute == 35 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.ThrillingSpook);

            if (Now64.Hour == 20 && Now64.Minute == 35 && Now64.Second < 1)
            {
                MobsHandler.Generate(IDMonster.SnowBanshee);
            }

            //if (Now64.Hour == 8 && Now64.Minute == 5 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.GuildBeast);



            //if (!TimeServer.Timex2() && Now64.Minute == 10 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.CornDevil);

            //if (TimeServer.Timex2() && Now64.Minute == 50 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.Ganoderma);

            //if (!TimeServer.Timex2() && Now64.Minute == 40 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.MummySkeleton);

            //if (TimeServer.Timex2() && Now64.Minute == 5 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.DarkSpearman);

            //if (TimeServer.Timex2() && Now64.Minute == 27 && Now64.Second < 1)
            //    MobsHandler.Generate(IDMonster.DarkmoonDemon);

            #endregion Boss
            #region BossDomination 10:36
            //try
            //{
            //    bool righthour = DateTime.Now.Hour == 10;
            //    bool rightmin = DateTime.Now.Minute == 36;
            //    bool rightsec = DateTime.Now.Second == 1;
            //    var GMap = Database.Server.ServerMaps[3020];
            //    var mobs = GMap.View.GetAllMapRoles(MapObjectType.Monster).Where(p => (p as MonsterRole).Family.ID == 6699 && p.Alive);

            //    if (righthour && rightmin && rightsec)
            //    {
            //        if (mobs.Count() == 0)
            //        {
            //            for (int i = 0; i < 4; i++)
            //            {
            //                using (RecycledPacket rec = new RecycledPacket())
            //                {
            //                    Packet stream;
            //                    stream = rec.GetStream();
            //                    ushort X = 0, Y = 0;
            //                    GMap.GetRandCoord(ref X, ref Y);
            //                    Database.Server.AddMapMonster(stream, GMap, 6699, X, Y, 1, 1, 1);
            //                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Minotaurs at Boss Domination has spawned join the event and, kill them!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //                    SendSysMesage("The Bosses Domination start! Hurry to kill them all.", ChatMode.System);
            //                    string MSG;
            //                    MSG = "Minotaurs at BossDomination has spawned join the event and, kill them!";
            //                    Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
            //                }
            //                foreach (var user in Database.Server.GamePoll.Values)
            //                    user.Player.MessageBox("The Minotaurs Bosses have spawned! Hurry to kill them.", new Action<Client.GameClient>(p => { p.Teleport(377, 343, 1002); }), null, 60);

            //            }

            //        }
            //    }
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("Could not do Boss Domination", false, LogType.WARNING);
            //}
            #endregion
            #region BossDomination 23:36
            //try
            //{
            //    bool righthour = DateTime.Now.Hour == 23;
            //    bool rightmin = DateTime.Now.Minute == 36;
            //    bool rightsec = DateTime.Now.Second == 1;
            //    var GMap = Database.Server.ServerMaps[3020];
            //    var mobs = GMap.View.GetAllMapRoles(MapObjectType.Monster).Where(p => (p as MonsterRole).Family.ID == 6699 && p.Alive);

            //    if (righthour && rightmin && rightsec)
            //    {
            //        if (mobs.Count() == 0)
            //        {
            //            for (int i = 0; i < 4; i++)
            //            {
            //                using (RecycledPacket rec = new RecycledPacket())
            //                {
            //                    Packet stream;
            //                    stream = rec.GetStream();
            //                    ushort X = 0, Y = 0;
            //                    GMap.GetRandCoord(ref X, ref Y);
            //                    Database.Server.AddMapMonster(stream, GMap, 6699, X, Y, 1, 1, 1);
            //                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Minotaurs at Boss Domination has spawned join the event and, kill them!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
            //                    SendSysMesage("The Bosses Domination start! Hurry to kill them all.", ChatMode.System);
            //                    string MSG;
            //                    MSG = "Minotaurs at BossDomination has spawned join the event and, kill them!";
            //                    Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
            //                }
            //                foreach (var user in Database.Server.GamePoll.Values)
            //                    user.Player.MessageBox("The Minotaurs Bosses have spawned! Hurry to kill them.", new Action<Client.GameClient>(p => { p.Teleport(377, 343, 1002); }), null, 60);

            //            }

            //        }
            //    }
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("Could not do Boss Domination", false, LogType.WARNING);
            //}
            #endregion
            //#region AutoMaintenance
            //try
            //{
            //    ServerKernel.Maintenance = DateTime.Today.AddHours(15);
            //    if (ServerKernel.AutoMaintenance && Now64 == ServerKernel.Maintenance && !Program.OnMainternance)
            //        if (Now64.Hour == 15 && Now64.Minute < 1 && Now64.Second < 1)
            //            new Thread(Program.Maintenance).Start();
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("Could not do AutoMaintenance", false, LogType.WARNING);
            //}
            //#endregion
            #region WaterLord
            //try
            //{

            //    if (Now64.Minute == 27 && Now64.Second < 1)//re-spawn ganoderma
            //    {
            //        ushort MapID = 1212;
            //        Role.GameMap.EnterMap(MapID);
            //        var map = Server.ServerMaps[MapID];
            //        string loc = map.GetMobLoc(8500);
            //        if (loc == "")
            //        {
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                ushort X = 517;
            //                ushort Y = 732;
            //                WaterLordStillTime = Now64;
            //                MsgMonster.MobCollection.TryObtainSpawnXY(map, 517, 732, 60, 60, out X, out Y);
            //                Database.Server.AddMapMonster(stream, map, 8500, X, Y, 1, 1, 1);
            //                SendSysMesage("WaterLord has spawned in Adventure Islands at(" + X + ", " + Y + "), Kill him and get The CleanWater.", ChatMode.System);
            //            }
            //        }
            //        else
            //        {
            //            if (Now64 >= WaterLordStillTime.AddMilliseconds(420000))//7 min
            //            {
            //                SendSysMesage("WaterLord has spawned in Adventure Islands at" + loc + ", Kill him and get The CleanWater.", ChatMode.System);
            //                WaterLordStillTime = Now64;
            //            }
            //        }
            //    }

            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("Could not do WaterLord", false, LogType.WARNING);
            //}
            #endregion WaterLord
            #region SnakeMan - old time
            //try
            //{

            //    if (Now64.Minute == 1 && Now64.Second < 1)//re-spawn ganoderma
            //    {
            //        ushort MapID = 1063;
            //        Role.GameMap.EnterMap(MapID);
            //        var map = Server.ServerMaps[MapID];
            //        string loc = map.GetMobLoc(3102);
            //        if (loc == "")
            //        {
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                //ushort X = 517;
            //                //ushort Y = 732;
            //                SnakeManStillTime = Now64;
            //                //MsgMonster.MobCollection.TryObtainSpawnXY(map, 84, 64, 60, 60, out X, out Y);
            //                Database.Server.AddMapMonster(stream, map, 3102, 84, 64, 1, 1, 1);
            //                SendSysMesage("SnakeMan has spawned in Bird Islands at(" + 84 + ", " + 64 + "), kill them. Drop [Special Items 50% change.].", ChatMode.System);
            //            }
            //        }
            //        else
            //        {
            //            if (Now64 >= SnakeManStillTime.AddMilliseconds(480000))//7 min
            //            {
            //                SendSysMesage("SnakeMan has spawned in Bird Islands at" + loc + ", kill them. Drop [Special Items 50% change.].", ChatMode.System);
            //                SnakeManStillTime = Now64;
            //            }
            //        }
            //    }

            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("Could not do SnakeMan", false, LogType.WARNING);
            //}
            #endregion SnakeMan         
            #region SnakeMan
            try
            {
                ushort MapID = 1063;
                Role.GameMap.EnterMap(MapID);
                var map = Server.ServerMaps[MapID];

                // Check if 2 hours have passed since last death
                if (Now64 >= SnakeManLastDeathTime.AddHours(2))
                {
                    string loc = map.GetMobLoc(3102);

                    if (string.IsNullOrEmpty(loc)) // Not alive, time to spawn
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            SnakeManStillTime = Now64;

                            // spawn mob
                            Database.Server.AddMapMonster(stream, map, 3102, 84, 64, 1, 1, 1);

                            SendSysMesage("SnakeMan has spawned in Bird Islands at (84, 64), kill them. Drop [Special Items 50% chance.]", ChatMode.System);
                        }
                    }
                    else
                    {
                        // Alive too long, resend message every 7 min
                        //if (Now64 >= SnakeManLastDeathTime.AddHours(2))
                        //{
                        //    SendSysMesage("SnakeMan has spawned in Bird Islands at " + loc + ", kill them. Drop [Special Items 50% chance.]", ChatMode.System);
                        //    SnakeManStillTime = Now64;
                        //}
                    }
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not do SnakeMan", false, LogType.WARNING);
            }
            #endregion SnakeMan
            #region LavaBeasts
            //if (DateTime.Now > NextLavaCheck)
            //{// check for lava (every 5 minutes spawn 3)
            //    NextLavaCheck = DateTime.Now.AddMinutes(5);
            //    var MapFr = Database.Server.ServerMaps[2056];
            //    if (MapFr.MobsCount(20055) < 3)
            //        SpawnLavaBeast();
            //}
            #endregion
            #region Hide(n)Seek
            #region hideNSeek
            if (Now64.Minute == 0 && Now64.Second <= 5)
            {
                hideNSeek = false;
            }
            #endregion
                if (Now64.Minute == 1 && Now64.Second >= 2 && !hideNSeek)
                {
                    hideNSeek = true;
                    var map = Server.ServerMaps[1036];
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Role.Core.SendGlobalMessage(stream, $"Hide(n)Seek event has started! Find the [Hide(n)Seek] Npc in market to claim a prize.", MsgMessage.ChatMode.TopLeft);
                        Role.Core.SendGlobalMessage(stream, $"Hide(n)Seek event has started! Find the [Hide(n)Seek] Npc in market to claim a prize.", MsgMessage.ChatMode.Center);
                    }
                    ushort x = 0, y = 0;
                    map.GetRandCoord(ref x, ref y);
                    var npc = Game.MsgNpc.Npc.Create();
                    npc.UID = (uint)NpcID.HideNSeek;
                    map.GetRandCoord(ref x, ref y);
                    npc.X = (ushort)x;
                    npc.Y = (ushort)y;
                    npc.Mesh = 29681;
                    npc.NpcType = Role.Flags.NpcType.Talker;
                    npc.Map = 1036;
                    map.AddNpc(npc);
                    Console.WriteLine($"Hide(N)Seek location: {npc.X}, {npc.Y}");

                }
            #endregion
            #region Ganodema
            if (Now64.Minute == 15 && Now64.Second < 1)//re-spawn ganoderma
            {
                var map = Database.Server.ServerMaps[1011];
                if (!map.ContainMobID(3130))
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Database.Server.AddMapMonster(stream, map, 3130, 667, 753, 18, 18, 1);
#if Arabic
                                   Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Ganodema & Titan have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                     
#else
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Ganodema have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        SendSysMesage("The Ganodema have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", ChatMode.System);

#endif
                    }
                }
            }
            #endregion
            #region Titan
            else if (Now64.Minute == 17 && Now64.Second < 1)//re-spawn titan
            {
                var map = Database.Server.ServerMaps[1020];
                if (!map.ContainMobID(3134))
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Database.Server.AddMapMonster(stream, map, 3134, 419, 618, 18, 18, 1);
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Titan have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        SendSysMesage("The Titan have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", ChatMode.System);

                    }

                }
            }
            #endregion
            #region Tournaments
            try
            {
                Random Rand;
                Rand = new Random();
                #region ElitePkTournament
                if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.ElitePkTournament].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.ElitePkTournament].EveryDay)
                {
                    if (Now64.Hour != ISchedule.Schedules[ISchedule.EventID.ElitePkTournament].StartHour && Now64.Hour != ISchedule.Schedules[ISchedule.EventID.ElitePkTournament].StartHour + 1 && Now64.Hour != ISchedule.Schedules[ISchedule.EventID.ElitePkTournament].StartHour + 2 && ElitePkTournament.Proces != ProcesType.Dead)
                        ElitePkTournament.Proces = ProcesType.Dead;
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.ElitePkTournament].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.ElitePkTournament].StartMinute)
                        ElitePkTournament.Start();
                }
                #endregion
                #region Find The Box
                //Random Rand = new Random();
                if (CurrentTournament.Process == ProcesType.Dead)
                {
                    if (Now64.Hour == 6 && Now64.Minute == 11 && Now64.Second < 2)
                    {
                        var X = Rand.Next(0, 1);
                        if (X == 0)
                            CurrentTournament = Tournaments[TournamentType.FindTheBox];
                        if (X == 1)
                            CurrentTournament = Tournaments[TournamentType.FindTheBox];
                        CurrentTournament.Open();
                    }
                }
                #endregion
                #region ClassPkWar
                //if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.ClassPkWar].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.ClassPkWar].EveryDay)
                //{
                //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.ClassPkWar].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.ClassPkWar].StartMinute)
                //        ClassPkWar.Start();
                //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.ClassPkWar].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.ClassPkWar].EndMinute)
                //    {
                //        MsgClassPKWar.War[][] pkWars;
                //        pkWars = ClassPkWar.PkWars;
                //        foreach (MsgClassPKWar.War[] war in pkWars)
                //        {
                //            MsgClassPKWar.War[] array;
                //            array = war;
                //            foreach (MsgClassPKWar.War map in array)
                //            {
                //                IEnumerable<GameClient> players_in_map;
                //                players_in_map = Server.GamePoll.Values.Where((GameClient e) => e.Player.DynamicID == map.DinamicID && e.Player.Alive);
                //                if (players_in_map.Count() == 1)
                //                {
                //                    GameClient winner;
                //                    winner = players_in_map.SingleOrDefault();
                //                    using (RecycledPacket rec = new RecycledPacket())
                //                    {
                //                        Packet stream;
                //                        stream = rec.GetStream();
                //                        map.GetMyReward(winner, stream);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion
                #region TeamPkTournament
                if ((Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.TeamPkTournament].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.TeamPkTournament].EveryDay) && Now64.Hour == ISchedule.Schedules[ISchedule.EventID.TeamPkTournament].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.TeamPkTournament].StartMinute)
                    TeamPkTournament.Start();
                #endregion
                #region WeeklyPK
                //if ((Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.WeeklyPK].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.WeeklyPK].EveryDay) && Now64.Hour == ISchedule.Schedules[ISchedule.EventID.WeeklyPK].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.WeeklyPK].StartMinute)
                //    PkWar.Open();
                #endregion
                #region GuildWar
                if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.GuildWar].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.GuildWar].EveryDay)
                {
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.GuildWar].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.GuildWar].StartMinute)
                    {
                        if (GuildWar.Proces == ProcesType.Dead)
                            GuildWar.Start();
                        if (!GuildWar.SendInvitation)
                        {
                            SendInvitation("GuildWar", "is about to begin! Will you join it?", 200, 254, 1038, 0, 60, MsgStaticMessage.Messages.GuildWar);
                            GuildWar.SendInvitation = true;
                        }
                    }
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.GuildWar].EndHour - 1)
                    {
                        if (GuildWar.FlamesQuest.ActiveFlame10 == false)
                        {
                            SendSysMesage("The Flame Stone 9 is Active now. Light up the Flame Stone (62,59) near the Stone Pole in the Guild City.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            GuildWar.FlamesQuest.ActiveFlame10 = true;
                        }
                    }
                    if (GuildWar.Proces == ProcesType.Idle && Now64 > GuildWar.StampRound)
                        GuildWar.Began();
                    if (GuildWar.Proces != ProcesType.Dead && DateTime.Now > GuildWar.StampShuffleScore)
                        GuildWar.ShuffleGuildScores();
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.GuildWar].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.GuildWar].EndMinute && (GuildWar.Proces == ProcesType.Alive || GuildWar.Proces == ProcesType.Idle))
                        GuildWar.CompleteEndGuildWar();

                    GuildWar.CheckUp();
                }
                #endregion
                #region CaptureTheFlag
                //if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.CaptureTheFlag].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.CaptureTheFlag].EveryDay)
                //{
                //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CaptureTheFlag].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CaptureTheFlag].StartMinute)
                //        CaptureTheFlag.Start();
                //    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CaptureTheFlag].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CaptureTheFlag].EndMinute)
                //        CaptureTheFlag.CheckFinish();
                //}
                #endregion
                #region SkillTeamPkTournament
                if ((Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.SkillTeamPkTournament].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.SkillTeamPkTournament].EveryDay) && Now64.Hour == ISchedule.Schedules[ISchedule.EventID.SkillTeamPkTournament].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.SkillTeamPkTournament].StartMinute)
                    SkillTeamPkTournament.Start();
                #endregion
                #region EliteGuildWar
                if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.EliteGuildWar].DayOfWeek || (ISchedule.Schedules[ISchedule.EventID.EliteGuildWar].EveryDay &&
                    (Now64.DayOfWeek != DayOfWeek.Saturday || Now64.DayOfWeek != DayOfWeek.Sunday)))
                {
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.EliteGuildWar].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.EliteGuildWar].StartMinute)
                    {
                        if (EliteGuildWar.Proces == ProcesType.Dead)
                            EliteGuildWar.Start();
                        if (!EliteGuildWar.SendInvitation)
                        {
                            SendInvitation("EliteGuildWar", "is about to begin! Will you join it?", 159, 052, 8250, 0, 60);
                            EliteGuildWar.SendInvitation = true;
                        }
                    }
                    if (EliteGuildWar.Proces == ProcesType.Idle && Now64 > EliteGuildWar.StampRound)
                        EliteGuildWar.Began();
                    if (DateTime.Now > EliteGuildWar.StampShuffleScore)
                        EliteGuildWar.ShuffleGuildScores();
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.EliteGuildWar].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.EliteGuildWar].EndMinute && (EliteGuildWar.Proces == ProcesType.Alive || EliteGuildWar.Proces == ProcesType.Idle))
                        EliteGuildWar.CompleteEndGuildWar();

                    EliteGuildWar.CheckUp();
                }
                #endregion
                #region ClanWar
                if (Now64.DayOfWeek == ISchedule.Schedules[ISchedule.EventID.ClanWar].DayOfWeek || ISchedule.Schedules[ISchedule.EventID.ClanWar].EveryDay)
                {
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.ClanWar].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.ClanWar].StartMinute)
                    {
                        if (ClanWar.Proces == ProcesType.Dead)
                            ClanWar.Start();
                        if (!ClanWar.SendInvitation)
                        {
                            SendInvitation("ClanWar", "is about to begin! Will you join it?", 377, 317, 1002, 0, 60);
                            ClanWar.SendInvitation = true;
                        }
                    }
                    if (ClanWar.Proces == ProcesType.Idle && Now64 > ClanWar.StampRound)
                        ClanWar.Began();
                    if (ClanWar.Proces != ProcesType.Dead && DateTime.Now > ClanWar.StampShuffleScore)
                        ClanWar.ShuffleScores();
                    if (Now64.Hour == ISchedule.Schedules[ISchedule.EventID.ClanWar].EndHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.ClanWar].EndMinute && (ClanWar.Proces == ProcesType.Alive || ClanWar.Proces == ProcesType.Idle))
                        ClanWar.CompleteEndWar();

                    ClanWar.CheckUp();
                }
                #endregion
                #region MonthlyPK
                //if ((GetLastWeekdayOfMonth(Now64, ISchedule.Schedules[ISchedule.EventID.MonthlyPK].DayOfWeek) || ISchedule.Schedules[ISchedule.EventID.MonthlyPK].EveryDay) && Now64.Hour == ISchedule.Schedules[ISchedule.EventID.MonthlyPK].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.MonthlyPK].StartMinute)
                //    MonthlyPKWar.Open();
                #endregion

            }
            catch
            {
                ServerKernel.Log.SaveLog("Tournaments Theard missed", false, LogType.WARNING);
            }
            #endregion
            #region Labirth1-Bosses -
            //try
            //{
            //}
            //try
            //{
            //    ushort MapID = 1351;
            //    Role.GameMap.EnterMap(MapID);
            //    var map = Server.ServerMaps[MapID];

            //    // Check if 2 hours have passed since last death
            //    if (Now64 >= Lab1BossLastDeathTime.AddHours(2))
            //    {
            //        string loc = map.GetMobLoc(3102);

            //        if (!map.ContainMobID(3143))// Not alive, time to spawn
            //        {
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                Lab1BossStillTime = Now64;

            //                // spawn mob
            //                Database.Server.AddMapMonster(stream, map, 3143, 237, 154, 1, 1, 1);

            //                SendSysMesage("Gibbon has spawned in Labirth 1 at (237, 154), kill them. Drop [Special Items 50% chance.]", ChatMode.System);
            //            }
            //        }
            //        //else
            //        //{
            //        //    // Alive too long, resend message every 7 min
            //        //    if (Now64 >= Lab1BossStillTime.AddHours(2))
            //        //    {
            //        //        SendSysMesage("Gibbon has spawned in Labirth 1 at " + loc + ", kill them. Drop [Special Items 50% chance.]", ChatMode.System);
            //        //    }
            //        //}
            //    }
            //}
            //catch
            //{
            //    ServerKernel.Log.SaveLog("Could not do Gibbon", false, LogType.WARNING);
            //}
            #endregion
            Stamp.Value = clock.Value + 1000;
        }
        private static void CouplesPKTime(DateTime Now64)
        {
            //bool date1 = Now64.Hour == ISchedule.Schedules[ISchedule.EventID.CouplesPK].StartHour && Now64.Minute == ISchedule.Schedules[ISchedule.EventID.CouplesPK].StartMinute;
            //if (date1)
            //{
            //    CouplesPKWar.Open();
            //}
            //CouplesPKWar.CheckUp();
        }
    }
}