using TheChosenProject.Mobs;
using System;
using System.Collections.Concurrent;
using TheChosenProject.Database;
using System.Collections.Generic;

namespace TheChosenProject
{
    public static class MobsHandler
    {
        public static ConcurrentDictionary<byte, Mobs.Base> Pool = new ConcurrentDictionary<byte, Mobs.Base>();
        public static Dictionary<byte, Mobs.Base> Pool2 = new Dictionary<byte, Mobs.Base>();

        public static Mobs.Base CallUp(Game.MsgMonster.MonsterFamily famil, IDMonster iDMonster)
        {
            try
            {
                var type = Type.GetType("TheChosenProject.Mobs.MobID_" + famil.ID);
                return Activator.CreateInstance(type, famil) as Mobs.Base;
            }
            catch
            {
                return null;
            }
        }

        public static void Generate(IDMonster iDMonster)
        {
            try
            {
                Game.MsgMonster.MonsterFamily famil;
                if (Server.MonsterFamilies.TryGetValue((uint)iDMonster, out famil))
                {
                    var type = Type.GetType("TheChosenProject.Mobs.MobID_" + famil.ID);
                    var mob = (Activator.CreateInstance(type, famil) as Mobs.Base);
                    if (mob != null)
                        mob.Run();
                    else Console.WriteLine("Could not load combat script for Boss " + iDMonster);
                }
                else Console.WriteLine("Could not load combat script for Boss " + iDMonster);
            }
            catch (Exception e)
            {
                Console.SaveException(e);
            }
        }
    }
}