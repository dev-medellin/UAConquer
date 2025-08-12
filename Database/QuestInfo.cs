using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.WindowsAPI;

namespace TheChosenProject.Database
{
    public class QuestInfo
    {
        public class DBQuest
        {
            public class NpcInfo
            {
                public uint ID;

                public ushort Map;

                public ushort X;

                public ushort Y;

                public string Name;

                public string MapName;

                public NpcInfo(string line)
                {
                    if (line == "")
                        return;
                    try
                    {
                        string[] data;
                        data = line.Split(',');
                        ID = uint.Parse(data[0]);
                        Map = ushort.Parse(data[1]);
                        X = ushort.Parse(data[2]);
                        Y = ushort.Parse(data[3]);
                        Name = data[4];
                        MapName = data[5];
                    }
                    catch
                    {
                    }
                }
            }

            public byte TypeId = 0;

            public byte CompleteFlag = 0;

            public uint ActivityType = 0;

            public ushort MissionId = 0;

            public string Name = "";

            public ushort Lv_min;

            public ushort Lv_max;

            public ushort First;

            public ushort Map;

            public byte[] Profession;

            public ulong Exp = 0;

            public ulong Gold = 0;

            public byte Sex;

            public uint FinishTime;

            public uint ActivityBeginTime;

            public uint ActivityEndTime;

            public NpcInfo BeginNpcId;

            public NpcInfo FinishNpcId;

            public uint Intentions;

            public uint[] Prize;

            public ushort[] Prequest;

            public bool CanAcceptQuest(byte Class)
            {
                if (Profession.Length == 0 || Profession.Length == 1)
                    return true;
                for (int x = 0; x < Profession.Length; x++)
                {
                    if (Class == Profession[x])
                        return true;
                }
                return false;
            }
        }

        public class ListNpcQuests
        {
            public List<DBQuest> Quests = new List<DBQuest>();
        }

        public static Dictionary<uint, DBQuest> AllQuests = new Dictionary<uint, DBQuest>();

        public static Dictionary<uint, ListNpcQuests> NpcQuests = new Dictionary<uint, ListNpcQuests>();

        public static DBQuest GetFinishQuest(uint NPCID, byte _class, uint QuestID = 0u, Func<DBQuest, bool> P = null)
        {
            ListNpcQuests array;
            array = NpcQuests[NPCID];
            for (int x = 0; x < array.Quests.Count; x++)
            {
                DBQuest quest;
                quest = array.Quests[x];
                if (QuestID == 0)
                {
                    if (quest.CanAcceptQuest(_class))
                    {
                        if (P == null)
                            return quest;
                        if (P(quest))
                            return quest;
                    }
                }
                else if (quest.MissionId == QuestID)
                {
                    return quest;
                }
            }
            return null;
        }

        public static void Init()
        {
            IniFile reader;
            reader = new IniFile("\\Questinfo.ini");
            int TotalMission;
            TotalMission = reader.ReadUInt16("TotalMission", "TotalMission", 0);
            for (ushort count = 1; count < TotalMission; count = (ushort)(count + 1))
            {
                DBQuest quest;
                quest = new DBQuest();
                quest.TypeId = reader.ReadByte(count.ToString(), "TypeId", 0);
               
                quest.CompleteFlag = reader.ReadByte(count.ToString(), "CompleteFlag", 0);
                quest.ActivityType = reader.ReadByte(count.ToString(), "ActivityType", 0);
                quest.MissionId = reader.ReadUInt16(count.ToString(), "MissionId", 0);
                quest.Intentions = reader.ReadUInt16(count.ToString(), "IntentAmount", 0);
                if (quest.MissionId == 20200)
                {

                }
                string Prequest;
                Prequest = reader.ReadString(count.ToString(), "Prequest", "");
                quest.Name = reader.ReadString(count.ToString(), "Name", "");
                if (Prequest != "")
                {
                    if (Prequest.Contains('|'))
                    {
                        string[] preq3;
                        preq3 = Prequest.Split('|');
                        quest.Prequest = new ushort[preq3.Length];
                        for (int x4 = 0; x4 < preq3.Length; x4++)
                        {
                            quest.Prequest[x4] = ushort.Parse(preq3[x4]);
                        }
                    }
                    else if (Prequest.Contains(','))
                    {
                        string[] preq2;
                        preq2 = Prequest.Split(',');
                        quest.Prequest = new ushort[preq2.Length];
                        for (int x3 = 0; x3 < preq2.Length; x3++)
                        {
                            quest.Prequest[x3] = ushort.Parse(preq2[x3]);
                        }
                    }
                    else
                    {
                        quest.Prequest = new ushort[1] { ushort.Parse(Prequest) };
                    }
                }
                else
                    quest.Prequest = new ushort[1];
                string profesion;
                profesion = reader.ReadString(count.ToString(), "Profession", "");
                if (profesion != "")
                {
                    if (profesion.Contains(','))
                    {
                        string[] profesion2;
                        profesion2 = profesion.Split(',');
                        quest.Profession = new byte[profesion2.Length];
                        for (int x2 = 0; x2 < profesion2.Length; x2++)
                        {
                            quest.Profession[x2] = byte.Parse(profesion2[x2]);
                        }
                    }
                    else
                        quest.Profession = new byte[1] { byte.Parse(profesion) };
                }
                else
                    quest.Profession = new byte[1];
                string Prize;
                Prize = reader.ReadString(count.ToString(), "Prize", "");
                if (Prize.Contains('['))
                {
                    string[] items;
                    items = Prize.Split('[');
                    quest.Prize = new uint[items.Length - 1];
                    try
                    {
                        for (int x = 1; x < items.Length; x++)
                        {
                            string element;
                            element = items[x];
                            string[] sp1;
                            sp1 = element.Split(' ');
                            string[] sp2;
                            sp2 = sp1[1].Split(',');
                            quest.Prize[x - 1] = uint.Parse(sp2[0]);
                        }
                    }
                    catch
                    {
                    }
                }
                quest.BeginNpcId = new DBQuest.NpcInfo(reader.ReadString(count.ToString(), "BeginNpcId", ""));
                quest.FinishNpcId = new DBQuest.NpcInfo(reader.ReadString(count.ToString(), "FinishNpcId", ""));
                if (!AllQuests.ContainsKey(quest.MissionId))
                    AllQuests.Add(quest.MissionId, quest);
                if (NpcQuests.ContainsKey(quest.BeginNpcId.ID))
                    NpcQuests[quest.BeginNpcId.ID].Quests.Add(quest);
                else
                {
                    NpcQuests.Add(quest.BeginNpcId.ID, new ListNpcQuests());
                    NpcQuests[quest.BeginNpcId.ID].Quests.Add(quest);
                }
            }
        }
    }
}
