using TheChosenProject.Database.DBActions;
using System;
using System.Collections.Generic;

 
namespace TheChosenProject.Game.MsgTournaments
{
    public class ISchedule
    {
        public ISchedule.EventID ID;
        public string Name;
        public string StartDay;
        public uint StartHour;
        public uint StartMinute;
        public DayOfWeek DayOfWeek;
        public uint EndHour;
        public uint EndMinute;
        public bool EveryDay;
        public uint ItemOne;
        public uint ItemTwo;
        public uint ItemThree;
        public static Dictionary<ISchedule.EventID, ISchedule> Schedules = new Dictionary<ISchedule.EventID, ISchedule>();

        public static void Load()
        {
            using (Read read = new Read("\\ISchedule.ini"))
            {
                if (!read.Reader())
                    return;
                uint count = (uint)read.Count;
                for (uint index = 0; index < count; ++index)
                {
                    ReadLine readLine = new ReadLine(read.ReadString(""), ' ');
                    if (readLine == null)
                        break;
                    ISchedule ischedule = new ISchedule()
                    {
                        ID = (ISchedule.EventID)readLine.Read(0),
                        Name = readLine.Read(""),
                        StartDay = readLine.Read("").ToLower(),
                        StartHour = readLine.Read((uint)0),
                        StartMinute = readLine.Read((uint)0),
                        EndHour = readLine.Read((uint)0),
                        EndMinute = readLine.Read((uint)0),
                        ItemOne = readLine.Read((uint)0),
                        ItemTwo = readLine.Read((uint)0),
                        ItemThree = readLine.Read((uint)0)
                    };
                    string startDay = ischedule.StartDay;
                    if (startDay != null)
                    {
                        switch (startDay.Length)
                        {
                            case 6:
                                switch (startDay[0])
                                {
                                    case 'f':
                                        if (startDay == "friday")
                                        {
                                            ischedule.EveryDay = false;
                                            ischedule.DayOfWeek = DayOfWeek.Friday;
                                            break;
                                        }
                                        break;
                                    case 'm':
                                        if (startDay == "monday")
                                        {
                                            ischedule.EveryDay = false;
                                            ischedule.DayOfWeek = DayOfWeek.Monday;
                                            break;
                                        }
                                        break;
                                    case 's':
                                        if (startDay == "sunday")
                                        {
                                            ischedule.EveryDay = false;
                                            ischedule.DayOfWeek = DayOfWeek.Sunday;
                                            break;
                                        }
                                        break;
                                }
                                break;
                            case 7:
                                if (startDay == "tuesday")
                                {
                                    ischedule.EveryDay = false;
                                    ischedule.DayOfWeek = DayOfWeek.Tuesday;
                                    break;
                                }
                                break;
                            case 8:
                                switch (startDay[0])
                                {
                                    case 'e':
                                        if (startDay == "everyday")
                                        {
                                            ischedule.EveryDay = true;
                                            break;
                                        }
                                        break;
                                    case 's':
                                        if (startDay == "saturday")
                                        {
                                            ischedule.EveryDay = false;
                                            ischedule.DayOfWeek = DayOfWeek.Saturday;
                                            break;
                                        }
                                        break;
                                    case 't':
                                        if (startDay == "thursday")
                                        {
                                            ischedule.EveryDay = false;
                                            ischedule.DayOfWeek = DayOfWeek.Thursday;
                                            break;
                                        }
                                        break;
                                }
                                break;
                            case 9:
                                if (startDay == "wednesday")
                                {
                                    ischedule.EveryDay = false;
                                    ischedule.DayOfWeek = DayOfWeek.Wednesday;
                                    break;
                                }
                                break;
                        }
                    }
                    ISchedule.Schedules.Add(ischedule.ID, ischedule);
                }
            }
        }

        public static void Save()
        {
            Write write = new Write("\\ISchedule.ini");
            foreach (ISchedule ischedule in ISchedule.Schedules.Values)
            {
                WriteLine writeLine = new WriteLine(' ');
                writeLine.Add((uint)ischedule.ID).Add(ischedule.Name).Add(ischedule.StartDay).Add(ischedule.StartHour).Add(ischedule.StartMinute).Add(ischedule.EndHour).Add(ischedule.EndMinute).Add(ischedule.ItemOne).Add(ischedule.ItemTwo).Add(ischedule.ItemThree);
                write.Add(writeLine.Close());
            }
            write.Execute(Mode.Open);
        }

        [Flags]
        public enum EventID : uint
        {
            None = 0,
            //LastmanStand = 6,
            //TreasureThief = 7,
            ////FindTheBox = 23,
            //FB_SS_Tournament = 8,
            GuildWar = 9,
            EliteGuildWar = 10, // 0x0000000A
            ClanWar = 11, // 0x0000000B
            ClassPkWar = 12, // 0x0000000C
            ElitePkTournament = 13, // 0x0000000D
            CaptureTheFlag = 14, // 0x0000000E
            WeeklyPK = 15, // 0x0000000F
            TeamPkTournament = 16, // 0x00000010
            SkillTeamPkTournament = 17, // 0x00000011
            MonthlyPK = 18, // 0x00000012
            HeroOfGame = 19, // 0x00000013
            PoleDomination = 20, // 0x00000014
            SmallCityGuilWar = 27, // 0x00000014
            GuildDeathMatch = 21, // 0x00000014
            CitywarTC = 22,
            CitywarPC = 23,
            CitywarAC = 24,
            CitywarDC = 25,
            CitywarBI = 26,
            CouplesPK = 28,
        }

        public static ISchedule GetReward(ISchedule.EventID id)
        {
            if (Schedules.ContainsKey(id))
                return Schedules[id];
            return null;
        }
    }
}
