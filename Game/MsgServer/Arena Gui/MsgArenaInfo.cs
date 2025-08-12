using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TheChosenProject.Client;
using TheChosenProject.ServerSockets;
namespace TheChosenProject.Game.MsgServer
{

    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaInfoCreate(this ServerSockets.Packet stream, MsgArenaInfo user)
        {
            stream.InitWriter();

            stream.Write(user.TodayRank);
            stream.Write((uint)0);
            stream.Write((uint)user.Status);
            stream.Write(user.TotalWin);
            stream.Write(user.TotalLose);
            stream.Write(user.TodayWin);
            stream.Write(user.TodayBattles);
            stream.Write(user.HistoryHonor);
            stream.Write(user.CurrentHonor);
            stream.Write(user.ArenaPoints);
            stream.ZeroFill(8);

            stream.Finalize(GamePackets.MsgArenaInfo);
            return stream;
        }
    }

    public class MsgArenaInfo
    {
        public enum Action : uint
        {
            NotSignedUp,
            WaitingForOpponent,
            WaitingInactive
        }

        public ushort Length;

        public ushort PacketID;

        public uint TodayRank;

        public Action Status;

        public uint TotalWin;

        public uint TotalLose;

        public uint TodayWin;

        public uint TodayBattles;

        public uint HistoryHonor;

        public uint CurrentHonor;

        public uint ArenaPoints;

        public uint ArenaWinnerMatches;

        [Packet(2209)]
        private static void Handler(GameClient user, Packet stream)
        {
            user.Send(stream.ArenaInfoCreate(user.ArenaStatistic.Info));
        }
    }
}
