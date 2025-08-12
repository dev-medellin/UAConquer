using System;
using System.Collections.Generic;


namespace TheChosenProject.Game.ConquerStructures.AI
{
    public class AIEnum
    {
        public static string GetName()
        {
            List<string> stringList = new List<string>()
      {
        "Hermione",
        "Flash",
        "Sweetie",
        "Mikie",
        "Amazing",
        "Silverto",
        "Bubble",
        "Percy",
        "Twinklerry",
        "Bruno",
        "Raymond",
        "Hydra",
        "Pendragon",
        "Lincoln",
        "Optimus",
        "Eagor",
        "Lucero",
        "Ravyn",
        "Marcus",
        "James",
        "Charles"
      };
            int index = new Random().Next(stringList.Count);
            return stringList[index];
        }

        public static string GetKillerMessage()
        {
            List<string> stringList = new List<string>()
      {
        "HAHAHAA NOP :P #10 #00",
        "You are too slow o_O #04",
        "don't play with me #01",
        "nice try #18 #39",
        "you can do it next time #19 #39",
        "There can only be one winner #28 #16",
        "bring me someone who can help you #54 #58"
      };
            int index = new Random().Next(stringList.Count);
            return stringList[index];
        }

        public enum AIType : uint
        {
            NotActive,
            Leveling,
            Training,
            Hunting,
            PKFighting,
            BufferBot
        }

        public enum AIStatus : uint
        {
            NotActive,
            Idle,
            Jumping,
            Attacking,
        }

        public enum WeaponType : uint
        {
            Hand,
            PrayedBead,
            Katana,
            Club,
            Sword,
            Blade,
            Spear,
            Wand,
            Glaive,
            Poleaxe,
            Halbert,
            BackSword,
            BowMan,
        }
    }
}
