using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet GuildMembersListCreate(this ServerSockets.Packet stream, MsgGuildMembers.Action Mode, int Page
            , Role.Instance.Guild.Member[] Members)
        {
            stream.InitWriter();

           // DateTime TimerNow = DateTime.Now;

            const int max = 12;
            int offset = Page / 12 * max;
            int count = Math.Min(max, Math.Max(0, Members.Length - offset));

            stream.Write((uint)Mode);
            stream.Write(Page);
            stream.Write(count);

            for (int x = 0; x < count; x++)
            {
                if (Members.Length > offset + x)
                {
                    var element = Members[offset + x];
                    //stream.Write((uint)0);
                    stream.Write(element.Name, 16);
                    stream.Write(element.NobilityRank);
                    stream.Write((uint)element.Graden);
                    stream.Write((uint)element.Level);
                    stream.Write((uint)element.Rank);
                    stream.Write((uint)0);
                    stream.Write((uint)element.MoneyDonate);
                    stream.Write((uint)(element.IsOnline ? 1 : 0));
                    stream.Write((uint)0);
                }
            }
            stream.Finalize(GamePackets.GuildMembers);
           
            return stream;
        }

        public static unsafe ServerSockets.Packet GuildRankListCreate(this ServerSockets.Packet stream, MsgGuildMembers.Action Mode, Role.Instance.Guild Guild, Role.Instance.Guild.Member[] Members)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write(0);
            stream.Write(Math.Min(28, Members.Length));
            for (int x = 0; x < Math.Min(28, Members.Length); x++)
            {
                try
                {
                    if (Members.Length > x)
                    {
                        var element = Members[x];

                        stream.Write((uint)element.Level);
                        stream.Write((uint)(element.IsOnline ? 1 : 0));
                        stream.Write((uint)Guild.ShareMemberPotency(element.Rank));
                        stream.Write(0);
                        stream.Write(element.Name, 16);
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }

            stream.Finalize(GamePackets.GuildMembers);

            return stream;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgGuildMembers
    {
        public enum Action : uint
        {
            MembersList,
            ListRanks
        }

        [Packet(2102)]
        private static void Process(GameClient user, Packet stream)
        {
            if (user.Player.MyGuild != null)
            {
                Guild.Member[] Members;
                Members = user.Player.MyGuild.Members.Values.ToArray();
                Array.Sort(Members, (Guild.Member f1, Guild.Member f2) => f2.IsOnline.CompareTo(f1.IsOnline));
                if (Members != null)
                {
                    Action Mode;
                    Mode = (Action)stream.ReadUInt32();
                    uint Page;
                    Page = stream.ReadUInt32();
                    user.Send(stream.GuildMembersListCreate(Mode, (int)Page, Members));
                }
            }
        }
    }
}
