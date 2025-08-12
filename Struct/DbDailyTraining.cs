using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Struct
{
    public struct DbDailyTraining
    {
        public byte WeekOne;
        public byte WeekOneC;
        public byte WeekOneCP;
        public uint TotalCompleteWeekOne;
        public byte WeekTwo;
        public byte WeekTwoC;
        public byte WeekTwoCP;
        public uint TotalCompleteWeekTwo;
        public byte WeekThree;
        public byte WeekThreeC;
        public byte WeekThreeCP;
        public uint TotalCompleteWeekThree;
        public byte WeekFour;
        public byte WeekFourC;
        public byte WeekFourCP;
        public uint TotalCompleteWeekFour;
        public byte OneGreatBoss;
        public byte SwordMaster;
        public byte NemesisTyrant;
        public byte CityBosses;
        public byte ArenaWin;
        public byte ArenaLose;
        public byte PremiumPass;

        public void GetDB(DbDailyTraining _db)
        {
            this = _db;
        }

        public void Reset()
        {
            WeekOne = 0;
            WeekTwo = 0;
            WeekThree = 0;
            WeekFour = 0;
            OneGreatBoss = 0;
            SwordMaster = 0;
            NemesisTyrant = 0;
            CityBosses = 0;
            ArenaWin = 0;
            ArenaLose = 0;
            PremiumPass = 0;

        }

        public static unsafe void Save(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\DailyTraining\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\DailyTraining\\");
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\DailyTraining\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                DbDailyTraining DB = new DbDailyTraining();
                DB.GetDB(client.DbDailyTraining);
                binary.Write(&DB, sizeof(DbDailyTraining));
                binary.Close();
            }
        }

        public static unsafe void Load(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\DailyTraining\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\DailyTraining\\");
            client.DbDailyTraining = new DbDailyTraining();
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\DailyTraining\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                DbDailyTraining DB;
                binary.Read(&DB, sizeof(DbDailyTraining));
                client.DbDailyTraining.GetDB(DB);
                binary.Close();
            }
        }
    }
}