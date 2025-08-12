using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.WindowsAPI;

namespace TheChosenProject.Database
{
    public class FlowersTable
    {
        public static void Load()
        {
            IniFile ini;
            ini = new IniFile("");
            string[] files;
            files = Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\");
            foreach (string fname in files)
            {
                ini.FileName = fname;
                ushort Body;
                Body = ini.ReadUInt16("Character", "Body", 1002);
                uint UID;
                UID = ini.ReadUInt32("Character", "UID", 0u);
                string Name;
                Name = ini.ReadString("Character", "Name", "None");
                Flowers flower;
                flower = new Flowers(UID, Name);
                string FlowerArray;
                FlowerArray = ini.ReadString("Character", "Flowers", "None");
                ReadLine reader;
                reader = new ReadLine(FlowerArray, '/');
                flower.FreeFlowers = reader.Read(0u);
                foreach (Flowers.Flower flow3 in flower)
                {
                    flow3.Amount = reader.Read(0u);
                    flow3.Amount2day = reader.Read(0u);
                }
                if (flower.Sum((Flowers.Flower x) => x.Amount) == 0L)
                    continue;
                if (!Flowers.ClientPoll.ContainsKey(UID))
                    Flowers.ClientPoll.TryAdd(UID, flower);
                if (Core.IsBoy(Body))
                {
                    foreach (Flowers.Flower flow2 in flower)
                    {
                        Program.BoysFlowersRanking.UpdateRank(flow2, flow2.Type);
                    }
                }
                else
                {
                    if (!Core.IsGirl(Body))
                        continue;
                    foreach (Flowers.Flower flow in flower)
                    {
                        Program.GirlsFlowersRanking.UpdateRank(flow, flow.Type);
                    }
                }
            }
            GC.Collect();
        }
    }
}
