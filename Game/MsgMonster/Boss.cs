using System;
using System.Collections.Generic;
using System.Linq;
using TheChosenProject.Game.MsgMonster;

namespace TheChosenProject
{
    public class Boss
    {
        public enum SpawnType : byte
        {
            Hourly,
            Daily
        }

        public ushort StudyPoints;

        public uint ConquerPointDropped;

        public byte SoulDropped;

        public byte MaxSoulDropped;

        public byte RefinaryDropped;

        public byte MaxRefienryDropped;

        public uint ConquerPointScores;

        public uint BossPointScores;

        public uint ItemDropScores;

        public string Name;

        public bool Alive;

        public ushort MapX;

        public ushort MapY;

        public List<string> X;

        public List<string> Items;

        public List<string> Y;

        public List<string> SpawnHours;

        public List<string> SpawnMinutes;

        public ushort MapID;

        public SpawnType Type;

        public uint MonsterID;

        public Boss()
        {
            X = new List<string>();
            Y = new List<string>();
            Items = new List<string>();
            SpawnHours = new List<string>();
            SpawnMinutes = new List<string>();
        }

        public bool CanSpawn()
        {
            int minuteTolerance = 1;
            bool isSpawnHour = SpawnHours.Contains(DateTime.Now.Hour.ToString());
            bool isSpawnMinute = SpawnMinutes.Any(m =>
                int.TryParse(m, out int spawnMinute) &&
                Math.Abs(DateTime.Now.Minute - spawnMinute) <= minuteTolerance);

            if (DateTime.Now.Second > 15)
                return false;
            if(MonsterID == 19890)
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
                {
                    return false;
                }
            }
            switch (Type)
            {
                case SpawnType.Hourly:
                    if (SpawnHours.Contains(DateTime.Now.Hour.ToString()) && SpawnMinutes.Contains(DateTime.Now.Minute.ToString()) && !Alive)
                    {
                        Alive = true;
                        return Alive;
                    }
                    break;
                case SpawnType.Daily:
                    if (SpawnHours.Contains(DateTime.Now.Hour.ToString()) && SpawnMinutes.Contains(DateTime.Now.Minute.ToString()) && !Alive)
                    {
                        Alive = true;
                        return Alive;
                    }
                    break;
            }
            return false;
        }

        public Tuple<ushort, ushort> SelectRandomCoordinate()
        {
            Random r;
            r = new Random();
            int i;
            i = r.Next(0, X.Count);
            return new Tuple<ushort, ushort>(ushort.Parse(X[i]), ushort.Parse(Y[i]));
        }
    }
}
