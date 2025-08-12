using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetQuestList(this ServerSockets.Packet stream, out  MsgQuestList.QuestMode Mode, out ushort Count, out uint QuestID, out MsgQuestList.QuestListItem.QuestStatus QuestMode
            , out uint QuestTimer)
        {
            Mode = (MsgQuestList.QuestMode)stream.ReadUInt16();
            Count = stream.ReadUInt16();
            QuestID = stream.ReadUInt32();
            QuestMode = (MsgQuestList.QuestListItem.QuestStatus)stream.ReadUInt32();
            QuestTimer = stream.ReadUInt32();

        }
        public static unsafe ServerSockets.Packet QuestListCreate(this ServerSockets.Packet stream, MsgQuestList.QuestMode Mode, ushort Count)
        {
            stream.InitWriter();

            stream.Write((ushort)Mode);
            stream.Write(Count);


            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemQuestList(this ServerSockets.Packet stream, MsgQuestList.QuestListItem item)
        {

            stream.Write(item.UID);
            stream.Write((uint)item.Status);
            stream.Write(item.Time);


            return stream;
        }
        public static unsafe ServerSockets.Packet QuestListFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.QuestList);

            return stream;
        }
    }


    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgQuestList
    {
        [Packet(1134)]
        private static void Process(GameClient user, Packet stream)
        {
            MsgQuestList.QuestMode Mode;
            ushort Count;
            uint QuestID;
            uint QuestTimer;
            stream.GetQuestList(out Mode, out Count, out QuestID, out MsgQuestList.QuestListItem.QuestStatus _, out QuestTimer);
            switch (Mode)
            {
                case MsgQuestList.QuestMode.AcceptQuest:
                    QuestInfo.DBQuest quest;
                    if (!user.Player.QuestGUI.AllowAccept() || !QuestInfo.AllQuests.TryGetValue(QuestID, out quest))
                        break;
                    user.Player.QuestGUI.Accept(quest, QuestTimer);
                    break;
                case MsgQuestList.QuestMode.QuitQuest:
                    MsgQuestList.QuestListItem data;
                    if (!user.Player.QuestGUI.src.TryGetValue(QuestID, out data))
                        break;
                    user.Player.QuestGUI.SendSinglePacket(data, MsgQuestList.QuestMode.QuitQuest);
                    user.Player.QuestGUI.RemoveQuest(data.UID);
                    break;
                case MsgQuestList.QuestMode.Review:
                    if (Count >= (ushort)80)
                        break;
                    user.Player.QuestGUI.SendFullGUI(stream);
                    break;
            }
        }

        public class QuestListItem
        {
            public uint UID;
            public MsgQuestList.QuestListItem.QuestStatus Status;
            public uint Time;
            public uint[] Intentions = new uint[1];

            public enum QuestStatus : uint
            {
                Accepted,
                Finished,
                Available,
            }
        }

        public enum QuestMode : ushort
        {
            AcceptQuest = 1,
            QuitQuest = 2,
            Review = 3,
            FinishQuest = 4,
        }
    }
}
