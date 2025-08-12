using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Game
{
    public class Anniversary
    {
        public static Dictionary<string, uint> AnniversaryQuest = new Dictionary<string, uint>();

        public static List<string> GetRanking()
        {
            List<string> strings = new List<string>();
            if (AnniversaryQuest.Count > 0)
            {
                var myList = AnniversaryQuest.Take(10).ToList();
                myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                string Text = "";
                for (int a = 0; a < myList.Count; a++)
                    strings.Add($"Nº{a + 1} {myList[a].Key} {myList[a].Value.ToString()}\n");
            }
            return strings;
        }
    }
}
