using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.WindowsAPI;

namespace TheChosenProject.Database
{
    public class RoleQuests
    {
        public static unsafe void Save(GameClient user)
        {
            BinaryFile binaryFile = new BinaryFile();
            if (!binaryFile.Open(ServerKernel.CO2FOLDER + "\\Quests\\" + user.Player.UID.ToString() + ".bin", FileMode.Create))
                return;
            int count = user.Player.QuestGUI.src.Count;
            binaryFile.Write((void*)&count, 4);
            foreach (MsgQuestList.QuestListItem questListItem in user.Player.QuestGUI.src.Values)
            {
                uint uid = questListItem.UID;
                uint status = (uint)questListItem.Status;
                uint time = questListItem.Time;
                int length = questListItem.Intentions.Length;
                binaryFile.Write((void*)&uid, 4);
                binaryFile.Write((void*)&status, 4);
                binaryFile.Write((void*)&time, 4);
                binaryFile.Write((void*)&length, 4);
                for (int index = 0; index < length; ++index)
                {
                    uint intention = questListItem.Intentions[index];
                    binaryFile.Write((void*)&intention, 4);
                }
            }
            binaryFile.Close();
        }

        public static unsafe void Load(GameClient user)
        {
            BinaryFile binaryFile = new BinaryFile();
            if (!binaryFile.Open(ServerKernel.CO2FOLDER + "\\Quests\\" + user.Player.UID.ToString() + ".bin", FileMode.Open))
                return;
            int count = user.Player.QuestGUI.src.Count;
            binaryFile.Read((void*)&count, 4);
            for (int index1 = 0; index1 < count; ++index1)
            {
                uint num1;
                binaryFile.Read((void*)&num1, 4);
                uint num2;
                binaryFile.Read((void*)&num2, 4);
                uint num3;
                binaryFile.Read((void*)&num3, 4);
                int length;
                binaryFile.Read((void*)&length, 4);
                uint[] numArray = new uint[length];
                for (int index2 = 0; index2 < length; ++index2)
                {
                    uint num4;
                    binaryFile.Read((void*)&num4, 4);
                    numArray[index2] = num4;
                }
                MsgQuestList.QuestListItem questListItem = new MsgQuestList.QuestListItem()
                {
                    UID = num1,
                    Status = (MsgQuestList.QuestListItem.QuestStatus)num2,
                    Time = num3,
                    Intentions = numArray
                };
                if (!user.Player.QuestGUI.src.ContainsKey(questListItem.UID))
                    user.Player.QuestGUI.src.Add(questListItem.UID, questListItem);
                if (questListItem.Status == MsgQuestList.QuestListItem.QuestStatus.Accepted)
                    user.Player.QuestGUI.AcceptedQuests.Add(questListItem.UID, questListItem);
            }
        }
    }
}
