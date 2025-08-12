using TheChosenProject.Database.DBActions;
using System.Collections.Generic;

 
namespace TheChosenProject
{
    public class BossDatabase
    {
        public static Dictionary<uint, Boss> Bosses = new Dictionary<uint, Boss>();

        public static void Load()
        {
            using (Read read = new Read("\\BossesManagement.ini"))
            {
                if (!read.Reader())
                    return;
                uint count;
                count = (uint)read.Count;
                for (uint i = 0; i < count; i++)
                {
                    ReadLine line;
                    line = new ReadLine(read.ReadString(""), ' ');
                    if (line == null)
                        break;
                    Boss boss;
                    boss = new Boss
                    {
                        Name = line.Read(""),
                        Type = (Boss.SpawnType)line.Read((byte)0),
                        MapID = line.Read((ushort)0)
                    };
                    boss.X.AddRange(line.Read("").Split(','));
                    boss.Y.AddRange(line.Read("").Split(','));
                    boss.SpawnHours.AddRange(line.Read("").Split(','));
                    boss.SpawnMinutes.AddRange(line.Read("").Split(','));
                    boss.MonsterID = (uint)line.Read(0);
                    boss.StudyPoints = line.Read((ushort)0);
                    boss.ConquerPointDropped = (uint)line.Read(0);
                    boss.SoulDropped = line.Read((byte)0);
                    boss.MaxSoulDropped = line.Read((byte)0);
                    boss.RefinaryDropped = line.Read((byte)0);
                    boss.MaxRefienryDropped = line.Read((byte)0);
                    boss.Items.AddRange(line.Read("").Split(','));
                    boss.ConquerPointScores = line.Read(0u);
                    boss.BossPointScores = line.Read(0u);
                    boss.ItemDropScores = line.Read(0u);
                    Bosses.Add(boss.MonsterID, boss);
                }
            }
        }

        public static void Save()
        {
            Write writer;
            writer = new Write("\\BossesManagement.ini");
            foreach (Boss boss in Bosses.Values)
            {
                WriteLine writerline;
                writerline = new WriteLine(' ');
                writerline.Add(boss.Name).Add((byte)boss.Type).Add(boss.MapID)
                    .Add(string.Join(",", boss.X))
                    .Add(string.Join(",", boss.Y))
                    .Add(string.Join(",", boss.SpawnHours))
                    .Add(string.Join(",", boss.SpawnMinutes))
                    .Add(boss.MonsterID)
                    .Add(boss.StudyPoints)
                    .Add(boss.ConquerPointDropped)
                    .Add(boss.SoulDropped)
                    .Add(boss.MaxSoulDropped)
                    .Add(boss.RefinaryDropped)
                    .Add(boss.MaxRefienryDropped)
                    .Add(string.Join(",", boss.Items))
                    .Add(boss.ConquerPointScores)
                    .Add(boss.BossPointScores)
                    .Add(boss.ItemDropScores);
                writer.Add(writerline.Close());
            }
            writer.Execute(Mode.Open);
        }
    }
}
