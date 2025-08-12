using System;
using System.Collections.Generic;
using System.IO;

using TheChosenProject.Database.DBActions;
using TheChosenProject.Role.Instance;
using TheChosenProject.WindowsAPI;

namespace TheChosenProject.Database
{
    public class ClanTable
    {
        public static Dictionary<uint, List<uint>> clanally = new Dictionary<uint, List<uint>>();

        public static Dictionary<uint, List<uint>> clanenemy = new Dictionary<uint, List<uint>>();

        internal static void Save()
        {
            foreach (KeyValuePair<uint, Clan> obj in Clan.Clans)
            {
                Clan clan;
                clan = obj.Value;
                using (Write writer = new Write("Clans\\" + obj.Key + ".txt"))
                    writer.Add(clan.ToString()).Add(clan.SaveAlly()).Add(clan.SaveEnemy())
                        .Execute(Mode.Open);
            }
        }

        internal static void Load()
        {
            string[] files;
            files = Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Clans\\");
            foreach (string fname in files)
            {
                using (Read reader = new Read(fname, true))
                {
                    if (reader.Reader())
                    {
                        Clan clan;
                        clan = new Clan();
                        clan.Load(reader.ReadString(""));
                        if (clan.ID > Clan.CounterClansID.Count)
                            Clan.CounterClansID.Set(clan.ID + 1);
                        LoadclanAlly(clan.ID, reader.ReadString(""));
                        LoadclanEnemy(clan.ID, reader.ReadString(""));
                        if (!Clan.Clans.ContainsKey(clan.ID))
                            Clan.Clans.TryAdd(clan.ID, clan);
                    }
                }
            }
            LoadMemebers();
            ClanExecuteAllyAndEnemy();
            GC.Collect();
        }

        public static void ClanExecuteAllyAndEnemy()
        {
            foreach (KeyValuePair<uint, List<uint>> obj2 in clanally)
            {
                foreach (uint clan2 in obj2.Value)
                {
                    if (Clan.Clans.TryGetValue(clan2, out var alyclan) && Clan.Clans.ContainsKey(obj2.Key))
                        Clan.Clans[obj2.Key].Ally.TryAdd(alyclan.ID, alyclan);
                }
            }
            foreach (KeyValuePair<uint, List<uint>> obj in clanenemy)
            {
                foreach (uint clan in obj.Value)
                {
                    if (Clan.Clans.TryGetValue(clan, out var enemyclan) && Clan.Clans.ContainsKey(obj.Key))
                        Clan.Clans[obj.Key].Enemy.TryAdd(enemyclan.ID, enemyclan);
                }
            }
        }

        private static void LoadMemebers()
        {
            IniFile ini;
            ini = new IniFile("");
            string[] files;
            files = Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\");
            foreach (string fname in files)
            {
                ini.FileName = fname;
                uint UID;
                UID = ini.ReadUInt32("Character", "UID", 0u);
                string Name;
                Name = ini.ReadString("Character", "Name", "None");
                uint ClanID;
                ClanID = ini.ReadUInt32("Character", "ClanID", 0u);
                if (ClanID != 0 && Role.Instance.Clan.Clans.TryGetValue(ClanID, out var Clan))
                {
                    Clan.Member member;
                    member = new Clan.Member
                    {
                        UID = UID,
                        Name = Name,
                        Rank = (Clan.Ranks)ini.ReadUInt16("Character", "ClanRank", 200),
                        Class = ini.ReadByte("Character", "Class", 0),
                        Level = (byte)ini.ReadUInt16("Character", "Level", 0),
                        Donation = ini.ReadUInt32("Character", "ClanDonation", 0u)
                    };
                    if (!Clan.Members.ContainsKey(member.UID))
                        Clan.Members.TryAdd(member.UID, member);
                }
            }
        }

        private static void LoadclanAlly(uint id, string line)
        {
            ReadLine reader;
            reader = new ReadLine(line, '/');
            int count;
            count = reader.Read(0);
            for (int x = 0; x < count; x++)
            {
                uint obj;
                obj = reader.Read(0u);
                if (clanally.ContainsKey(id))
                {
                    clanally[id].Add(obj);
                    continue;
                }
                clanally.Add(id, new List<uint> { obj });
            }
        }

        private static void LoadclanEnemy(uint id, string line)
        {
            ReadLine reader;
            reader = new ReadLine(line, '/');
            int count;
            count = reader.Read(0);
            for (int x = 0; x < count; x++)
            {
                uint obj;
                obj = reader.Read(0u);
                if (clanenemy.ContainsKey(id))
                {
                    clanenemy[id].Add(obj);
                    continue;
                }
                clanenemy.Add(id, new List<uint> { obj });
            }
        }
    }
}
