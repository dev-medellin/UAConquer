using TheChosenProject.Game;
using TheChosenProject.PacketHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Bosses
{
    public static class BossHandler
    {
        public static void Handle(Character C, Mob _mob, uint _damage, List<Character> PlayerTargets)
        {
            try
            {
                var type = Type.GetType("TheChosenProject.Bosses.MobID_" + _mob.MobID);
                var mob = Activator.CreateInstance(type, _mob) as MobBase;
                
                mob.Run(C, _mob, _damage, PlayerTargets);
            }
            catch (Exception)
            {
                if (_mob.MobID != 0)
                    Console.WriteLine("Could not load combat script for Boss ID: " + _mob.MobID);
            }
        }

        private static byte _boss = 0;

        public static void SpawnBoss()
        {
            switch (_boss)
            {
                case 0:
                    break;
                case 1:
                    World.Raikou = true;
                    break;
                case 2:
                    World.Capricorn = true;
                    break;
                case 4:
                    World.Tash = true;
                    break;
                case 5:
                    World.ThrillingSpook = true;
                    break;
            }
            _boss = 0;
        }

        public static void ChooseBoss(DayOfWeek Day)
        {
            int Hour = DateTime.UtcNow.Hour;
            switch (Day)
            {
                case DayOfWeek.Monday:
                    if (Hour == 1) _boss = 1;
                    else if (Hour == 5) _boss = 0;
                    else if (Hour == 9) _boss = 4;
                    else if (Hour == 13) _boss = 0;
                    else if (Hour == 17) _boss = 2;
                    else if (Hour == 21) _boss = 5;
                    break;
                case DayOfWeek.Tuesday:
                    if (Hour == 1) _boss = 0;
                    else if (Hour == 5) _boss = 2;
                    else if (Hour == 9) _boss = 5;
                    else if (Hour == 13) _boss = 1;
                    else if (Hour == 17) _boss = 0;
                    else if (Hour == 21) _boss = 4;
                    break;
                case DayOfWeek.Wednesday:
                    if (Hour == 1) _boss = 2;
                    else if (Hour == 5) _boss = 0;
                    else if (Hour == 9) _boss = 1;
                    else if (Hour == 13) _boss = 5;
                    else if (Hour == 17) _boss = 4;
                    else if (Hour == 21) _boss = 0;
                    break;
                case DayOfWeek.Thursday:
                    if (Hour == 1) _boss = 5;
                    else if (Hour == 5) _boss = 4;
                    else if (Hour == 9) _boss = 0;
                    else if (Hour == 13) _boss = 2;
                    else if (Hour == 17) _boss = 0;
                    else if (Hour == 21) _boss = 1;
                    break;
                case DayOfWeek.Saturday:
                    if (Hour == 1) _boss = 0;
                    else if (Hour == 5) _boss = 1;
                    else if (Hour == 9) _boss = 0;
                    else if (Hour == 13) _boss = 4;
                    else if (Hour == 17) _boss = 5;
                    else if (Hour == 21) _boss = 2;
                    break;
            }
            switch (_boss)
            {
                case 0:
                    World.CurrentBoss = "";
                    break;
                case 1:
                    World.CurrentBoss = "Raikou";
                    break;
                case 2:
                    World.CurrentBoss = "Capricorn";
                    break;
                case 4:
                    World.CurrentBoss = "Tash";
                    break;
                case 5:
                    World.CurrentBoss = "ThrillingSpook";
                    break;
            }
        }
        public static void WindowInformation(Character C)
        {
            MSG_DLG_Text T = new MSG_DLG_Text()
            {
                DlgId = 34,
                TextCount = 1,
                Text = new List<MSG_DLG_Text.DlgTxtData>()
            };

            MSG_DLG_Text.DlgTxtData Text = new MSG_DLG_Text.DlgTxtData()
            {
                Id = 1,
                Fontsize = 14,
                Text = $"{World.CurrentBoss} is about to spawn and terrify the world!\nWould you like to join the fight against it?",
                Color = 0xfff09e,
                xpos = 30,
                ypos = 110
            };
            Text.TextLength = (byte)Text.Text.Length;
            T.Text.Add(Text);


            C.MyClient.AddSend(Packets.MsgDlgText(T));
        }
    }
}
