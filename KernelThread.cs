using Extensions.ThreadGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerCore.Website;
using Extensions;

namespace TheChosenProject
{
    public class KernelThread
    {
        public const int TournamentsStamp = 1000;

        public const int ChatItemsStamp = 180000;

        public const int TeamArena_CreateMatches = 900;

        public const int TeamArena_VerifyMatches = 980;

        public const int TeamArena_CheckGroups = 960;

        public const int Arena_CreateMatches = 1100;

        public const int Arena_VerifyMatches = 1200;

        public const int Arena_CheckGroups = 1150;

        public const int TeamPkStamp = 1000;

        public const int ElitePkStamp = 1000;

        public const int BroadCastStamp = 1000;

        public const int ResetDayStamp = 6000;

        public const int SaveDatabaseStamp = 180000;

        public Time32 UpdateDBStatus = Time32.Now;

        public Time32 UpdateServerStatus = Time32.Now;

        public ThreadItem Thread;

        public ThreadItem SaveThread;

        private static int _last;

        public static DateTime LastServerPulse;

        public static DateTime LastSavePulse;

        public static DateTime LastGuildPulse;

        public static int Online
        {
            get
            {
                int current;
                current = Server.GamePoll.Count;
                if (current > _last)
                    _last = current;
                return current;
            }
        }

        public static int MaxOnline => _last;

        public KernelThread(int interval, string name)
        {
            Thread = new ThreadItem(interval, name, OnProcess);
            SaveThread = new ThreadItem(interval, "SaveThread", OnSaveThread);
        }

        public void Start()
        {
            Thread.Open();
            SaveThread.Open();
        }

        public void OnProcess()
        {
            Time32 clock;
            clock = Time32.Now;
            try
            {
                if (DateTime.Now > LastGuildPulse.AddHours(24.0))
                {
                    foreach (Guild guilds in Guild.GuildPoll.Values)
                    {
                        guilds.CreateMembersRank();
                        guilds.UpdateGuildInfo();
                    }
                    LastGuildPulse = DateTime.Now;
                }
                if (clock > UpdateServerStatus)
                {
                    DateTime now;
                    now = DateTime.Now;
                    ServerKernel.ServerManager.WindowTitle(string.Format("[{0}] Conquer Online Server - Server Time: {1:0000}/{2:00}/{3:00} {4:00}:{5:00} - Online: {6}/{7} - CPS[{8}] Silver[{13}] Hunting[{10}] Events[{9}] Bots[{11}] - {12}", ServerKernel.ServerName, now.Year, now.Month, now.Day, now.Hour, now.Minute, Online, MaxOnline, ServerKernel.TOTAL_CPS_HUNTING.ToString("0,0"), ITournamentsAlive.Tournments.Count, ServerKernel.ServerManager.Hunters(), ServerKernel.ServerManager.Bots(), Program.NetworkMonitor.UpdateStats(1000), ServerKernel.TOTAL_Silver_HUNTING.ToString("0,0")));
                    using (var conn = new MySql.Data.MySqlClient.MySqlConnection(TopRankings.ConnectionString))
                    {
                        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                            "UPDATE Servers SET Online = @Online, MaxOnline = @MaxOnline WHERE Name = @Name", conn))
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("@Online", Online);
                            cmd.Parameters.AddWithValue("@MaxOnline", MaxOnline);
                            cmd.Parameters.AddWithValue("@Name", "UAConquer"); // Match the server name in your DB

                            cmd.ExecuteNonQuery();
                        }
                    }
                    UpdateServerStatus = Time32.Now.AddSeconds(5);
                    LastServerPulse = DateTime.Now;
                }
                if (clock > UpdateDBStatus)
                {
                    TopRankings.LoadTopRankings();
                    UpdateDBStatus = Time32.Now.AddMinutes(15);
                }
                if (clock > Program.ResetRandom)
                {
                    Program.GetRandom.SetSeed(Environment.TickCount);
                    Program.ResetRandom = Time32.Now.AddMinutes(30);
                }
                MsgSchedules.CheckUp(clock);
                Program.GlobalItems.Work(clock);
                BossesManagement.work(clock);
                MsgSchedules.TeamArena.CheckGroups(clock);
                MsgSchedules.TeamArena.CreateMatches(clock);
                MsgSchedules.TeamArena.VerifyMatches(clock);
                MsgSchedules.Arena.CheckGroups(clock);
                MsgSchedules.Arena.CreateMatches(clock);
                MsgSchedules.Arena.VerifyMatches(clock);
                MsgTeamEliteGroup[] eliteGroups;
                eliteGroups = MsgTeamPkTournament.EliteGroups;
                foreach (MsgTeamEliteGroup elitegroup in eliteGroups)
                {
                    elitegroup.timerCallback(clock);
                }
                MsgTeamEliteGroup[] eliteGroups2;
                eliteGroups2 = MsgSkillTeamPkTournament.EliteGroups;
                foreach (MsgTeamEliteGroup elitegroup2 in eliteGroups2)
                {
                    elitegroup2.timerCallback(clock);
                }
                MsgEliteGroup[] eliteGroups3;
                eliteGroups3 = MsgEliteTournament.EliteGroups;
                foreach (MsgEliteGroup elitegroup3 in eliteGroups3)
                {
                    elitegroup3.timerCallback(clock);
                }
                MsgBroadcast.Work(clock);
                DateTime DateNow;
                DateNow = DateTime.Now;
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public void OnSaveThread()
        {
            Time32 clock;
            clock = Time32.Now;
            try
            {
                Server.Reset(clock);
                Program.SaveDBPayers(clock);
                LastSavePulse = DateTime.Now;
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
