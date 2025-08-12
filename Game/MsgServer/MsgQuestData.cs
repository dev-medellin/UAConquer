using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetQuestData(this ServerSockets.Packet stream, out uint UnKnow, out uint UID, out uint[] intents)
        {
            UnKnow = stream.ReadUInt32();
            UID = stream.ReadUInt32();
            intents = new uint[1];
            intents[0] = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet MsgQuestDataCreate(this ServerSockets.Packet stream, uint UnKnow, uint UID, params uint[] intents)
        {
            stream.InitWriter();
            stream.Write((ushort)0);
            stream.Write((ushort)intents.Length);
            stream.Write(UID);
            for (int x = 0; x < intents.Length; x++)
                stream.Write(intents[x]);
            if(UID== 35024)
                for (int x = 0; x < intents.Length; x++)
                    stream.Write(intents[x]);
            stream.Write(0);
            if (UID == 35007)
            {
                stream.ZeroFill(12);
                for (int x = 0; x < intents.Length; x++)
                    stream.Write(intents[x]);
            }

            stream.Finalize(GamePackets.QuestData);
            return stream;
        }
        public static unsafe ServerSockets.Packet MsgQuestDataCreate(this ServerSockets.Packet stream, uint UnKnow, uint UID , uint OwnerUID, params uint[] intents)
        {
            stream.InitWriter();

            stream.Write(UnKnow);
            stream.Write(UID);
            for (int x = 0; x < intents.Length; x++)
                stream.Write(intents[x]);
            stream.ZeroFill(sizeof(uint) * Math.Min(6, (6 - intents.Length)));
            stream.Write(OwnerUID);
            stream.ZeroFill(sizeof(uint));
            stream.Finalize(GamePackets.QuestData);
            return stream;
        }
    }
   public class MsgQuestData
    {
       [PacketAttribute(GamePackets.QuestData)]
        private static void Process(GameClient user, Packet stream)
        {
            stream.GetQuestData(out uint _, out uint _, out uint[] _);
        }
    }
}
