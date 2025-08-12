using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetTitle(this ServerSockets.Packet stream, out byte Title
            , out MsgTitle.QueueTitle Action)
        {
            uint UID = stream.ReadUInt32();
            Title = stream.ReadUInt8();
            Action = (MsgTitle.QueueTitle)stream.ReadUInt8();
        }

        public static unsafe ServerSockets.Packet TitleCreate(this ServerSockets.Packet stream, uint UID, byte Title
            , MsgTitle.QueueTitle Action)
        {
            stream.InitWriter();

            stream.Write(UID);
            stream.Write(Title);
            stream.Write((byte)Action);

            stream.Finalize(GamePackets.Title);

            return stream;
        }


    }

    public unsafe struct MsgTitle
    {
        public enum TitleType : uint
        {
            Victor = 20, // 0x00000014
            HeartlessEmperor = 21, // 0x00000015
            RisingStar = 22, // 0x00000016
            Scholar = 23, // 0x00000017
            SuperIdol = 24, // 0x00000018
            ImparialRuler = 25, // 0x00000019
            Incredible = 26, // 0x0000001A
            GrandMaster = 27, // 0x0000001B
            OverLord = 28, // 0x0000001C
            SuperMan = 29, // 0x0000001D
            Conqueror = 30, // 0x0000001E
            SwiftChaser = 31, // 0x0000001F
            VIP4Player = 32, // 0x00000020
            VIP6Player = 33, // 0x00000021
        }
        public enum QueueTitle : uint
        {
            Enqueue = 1,
            Dequeue = 2,
            Change = 3,
            List = 4
        }
        [PacketAttribute(GamePackets.Title)]
        public static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            byte Title;
            MsgTitle.QueueTitle Action;

            stream.GetTitle(out Title, out Action);

            switch (Action)
            {
                case QueueTitle.List:
                    {
                        foreach (var title in user.Player.Titles)
                            user.Send(stream.TitleCreate(user.Player.UID, title, QueueTitle.Enqueue));
                        break;
                    }
                case QueueTitle.Change:
                    {
                        user.Player.SwitchTitle(Title);
                        break;
                    }
            }
        }
    }
}
