using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TheChosenProject.Client;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TeamLeadership
    {
        public MsgTeamLeadership.Mode Typ;
        public uint UID;
        public uint LeaderUID;
        public int Count;
        public uint UnKnow;
    }

    public static class MsgTeamLeadership
    {
        public enum Mode : uint
        {
            Leader = 1u,
            Teammate
        }

        public unsafe static void GetTeamLeadership(this Packet stream, TeamLeadership* pQuery)
        {
            stream.ReadUnsafe(pQuery, sizeof(TeamLeadership));
        }

        public unsafe static Packet TeamLeadershipCreate(this Packet stream, TeamLeadership* pQuery)
        {
            stream.InitWriter();
            stream.WriteUnsafe(pQuery, sizeof(TeamLeadership));
            stream.Finalize(2045);
            return stream;
        }

        [Packet(2045)]
        private unsafe static void Process(GameClient user, Packet stream)
        {
            TeamLeadership action = default(TeamLeadership);
            stream.GetTeamLeadership(&action);
            user.Send(stream.TeamLeadershipCreate(&action));
            if (user.Team != null)
                user.Team.AutoInvite = action.UID == 1;
        }
    }
}
