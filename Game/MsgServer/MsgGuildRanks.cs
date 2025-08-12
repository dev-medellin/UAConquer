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
        public static unsafe ServerSockets.Packet GuildRanksCreate(this ServerSockets.Packet stream, MsgGuildRanks.RankTyp rank, ushort count, ushort Page)
        {
            stream.InitWriter();


            stream.Write((ushort)rank);
            stream.Write(count);
            stream.Write((ushort)20);//register count;
            stream.Write(Page);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemGuildRanks(this ServerSockets.Packet stream, MsgGuildRanks.RankTyp rank, Role.Instance.Guild.Member member, long Donation, int RealRank)
        {
            stream.Write(member.UID);
            stream.Write((uint)member.Rank);
            stream.Write((uint)RealRank);
            stream.ZeroFill((ushort)(4 * (ushort)rank));
            stream.Write((uint)Donation);
            stream.ZeroFill(36 - (ushort)(4 * (ushort)rank));
            stream.Write(member.Name, 16);

            return stream;
        }
        public static unsafe ServerSockets.Packet GuildRanksFinalize(this ServerSockets.Packet stream)
        {

            stream.Finalize(GamePackets.GuildRanks);
            return stream;
        }
    }
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgGuildRanks
    {
        public enum RankTyp : ushort
        {
            SilverRank,
            CpRank,
            GuideDonation,
            PkRank,
            ArsenalRank,
            RosesRank,
            OrchidRank,
            LilyRank,
            TulipRank,
            TotalDonaion,
            MaxCounts
        }

        [Packet(2101)]
        private static void Process(GameClient user, Packet stream)
        {
            if (user.Player.MyGuild == null)
                return;
            RankTyp Rank;
            Rank = (RankTyp)stream.ReadUInt8();
            byte Page;
            Page = stream.ReadUInt8();
            Page = (byte)Math.Min(2, (int)Page);
            switch (Rank)
            {
                case RankTyp.SilverRank:
                    {
                        Guild.Member[] array;
                        array = user.Player.MyGuild.RankSilversDonations.OrderByDescending((Guild.Member e) => e.MoneyDonate).ToArray();
                        int offset;
                        offset = Page * 10;
                        int count;
                        count = Math.Min(10, Math.Max(0, array.Length - offset));
                        stream.GuildRanksCreate(Rank, (ushort)count, Page);
                        int RealRank;
                        RealRank = Page * 10;
                        for (int x = 0; x < count; x++)
                        {
                            if (array.Length > x + offset)
                            {
                                Guild.Member element;
                                element = array[x + offset];
                                stream.AddItemGuildRanks(Rank, element, element.MoneyDonate, RealRank++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.CpRank:
                    {
                        Guild.Member[] array2;
                        array2 = user.Player.MyGuild.RankCPDonations.OrderByDescending((Guild.Member e) => e.CpsDonate).ToArray();
                        int offset2;
                        offset2 = Page * 10;
                        int count2;
                        count2 = Math.Min(10, Math.Max(0, array2.Length - offset2));
                        stream.GuildRanksCreate(Rank, (ushort)count2, Page);
                        int RealRank2;
                        RealRank2 = Page * 10;
                        for (int x2 = 0; x2 < count2; x2++)
                        {
                            if (array2.Length > x2 + offset2)
                            {
                                Guild.Member element2;
                                element2 = array2[x2 + offset2];
                                stream.AddItemGuildRanks(Rank, element2, element2.CpsDonate, RealRank2++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.GuideDonation:
                    {
                        Guild.Member[] array3;
                        array3 = user.Player.MyGuild.RankGuideDonations.OrderByDescending((Guild.Member e) => e.VirtutePointes).ToArray();
                        int offset3;
                        offset3 = Page * 10;
                        int count3;
                        count3 = Math.Min(10, Math.Max(0, array3.Length - offset3));
                        stream.GuildRanksCreate(Rank, (ushort)count3, Page);
                        int RealRank3;
                        RealRank3 = Page * 10;
                        for (int x3 = 0; x3 < count3; x3++)
                        {
                            if (array3.Length > x3 + offset3)
                            {
                                Guild.Member element3;
                                element3 = array3[x3 + offset3];
                                stream.AddItemGuildRanks(Rank, element3, element3.VirtutePointes, RealRank3++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.PkRank:
                    {
                        Guild.Member[] array4;
                        array4 = user.Player.MyGuild.RankPkDonations.OrderByDescending((Guild.Member e) => e.PkDonation).ToArray();
                        int offset4;
                        offset4 = Page * 10;
                        int count4;
                        count4 = Math.Min(10, Math.Max(0, array4.Length - offset4));
                        stream.GuildRanksCreate(Rank, (ushort)count4, Page);
                        int RealRank4;
                        RealRank4 = Page * 10;
                        for (int x4 = 0; x4 < count4; x4++)
                        {
                            if (array4.Length > x4 + offset4)
                            {
                                Guild.Member element4;
                                element4 = array4[x4 + offset4];
                                stream.AddItemGuildRanks(Rank, element4, element4.PkDonation, RealRank4++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.ArsenalRank:
                    {
                        Guild.Member[] array5;
                        array5 = user.Player.MyGuild.RankArsenalDonations.OrderByDescending((Guild.Member e) => e.ArsenalDonation).ToArray();
                        int offset5;
                        offset5 = Page * 10;
                        int count5;
                        count5 = Math.Min(10, Math.Max(0, array5.Length - offset5));
                        stream.GuildRanksCreate(Rank, (ushort)count5, Page);
                        int RealRank5;
                        RealRank5 = Page * 10;
                        for (int x5 = 0; x5 < count5; x5++)
                        {
                            if (array5.Length > x5 + offset5)
                            {
                                Guild.Member element5;
                                element5 = array5[x5 + offset5];
                                stream.AddItemGuildRanks(Rank, element5, element5.ArsenalDonation, RealRank5++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.RosesRank:
                    {
                        Guild.Member[] array6;
                        array6 = user.Player.MyGuild.RankRosseDonations.OrderByDescending((Guild.Member e) => e.Rouses).ToArray();
                        int offset6;
                        offset6 = Page * 10;
                        int count6;
                        count6 = Math.Min(10, Math.Max(0, array6.Length - offset6));
                        stream.GuildRanksCreate(Rank, (ushort)count6, Page);
                        int RealRank6;
                        RealRank6 = Page * 10;
                        for (int x6 = 0; x6 < count6; x6++)
                        {
                            if (array6.Length > x6 + offset6)
                            {
                                Guild.Member element6;
                                element6 = array6[x6 + offset6];
                                stream.AddItemGuildRanks(Rank, element6, element6.Rouses, RealRank6++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.LilyRank:
                    {
                        Guild.Member[] array8;
                        array8 = user.Player.MyGuild.RankLiliesDonations.OrderByDescending((Guild.Member e) => e.Lilies).ToArray();
                        int offset8;
                        offset8 = Page * 10;
                        int count8;
                        count8 = Math.Min(10, Math.Max(0, array8.Length - offset8));
                        stream.GuildRanksCreate(Rank, (ushort)count8, Page);
                        int RealRank8;
                        RealRank8 = Page * 10;
                        for (int x8 = 0; x8 < count8; x8++)
                        {
                            if (array8.Length > x8 + offset8)
                            {
                                Guild.Member element8;
                                element8 = array8[x8 + offset8];
                                stream.AddItemGuildRanks(Rank, element8, element8.Lilies, RealRank8++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.OrchidRank:
                    {
                        Guild.Member[] array7;
                        array7 = user.Player.MyGuild.RankOrchidsDonations.OrderByDescending((Guild.Member e) => e.Orchids).ToArray();
                        int offset7;
                        offset7 = Page * 10;
                        int count7;
                        count7 = Math.Min(10, Math.Max(0, array7.Length - offset7));
                        stream.GuildRanksCreate(Rank, (ushort)count7, Page);
                        int RealRank7;
                        RealRank7 = Page * 10;
                        for (int x7 = 0; x7 < count7; x7++)
                        {
                            if (array7.Length > x7 + offset7)
                            {
                                Guild.Member element7;
                                element7 = array7[x7 + offset7];
                                stream.AddItemGuildRanks(Rank, element7, element7.Orchids, RealRank7++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.TulipRank:
                    {
                        Guild.Member[] array9;
                        array9 = user.Player.MyGuild.RankTulipsDonations.OrderByDescending((Guild.Member e) => e.Tulips).ToArray();
                        int offset9;
                        offset9 = Page * 10;
                        int count9;
                        count9 = Math.Min(10, Math.Max(0, array9.Length - offset9));
                        int RealRank9;
                        RealRank9 = Page * 10;
                        stream.GuildRanksCreate(Rank, (ushort)count9, Page);
                        for (int x9 = 0; x9 < count9; x9++)
                        {
                            if (array9.Length > x9 + offset9)
                            {
                                Guild.Member element9;
                                element9 = array9[x9 + offset9];
                                stream.AddItemGuildRanks(Rank, element9, element9.Tulips, RealRank9++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
                case RankTyp.TotalDonaion:
                    {
                        Guild.Member[] array10;
                        array10 = user.Player.MyGuild.RankTotalDonations.OrderByDescending((Guild.Member e) => e.TotalDonation).ToArray();
                        int offset10;
                        offset10 = Page * 10;
                        int count10;
                        count10 = Math.Min(10, Math.Max(0, array10.Length - offset10));
                        int RealRank10;
                        RealRank10 = Page * 10;
                        stream.GuildRanksCreate(Rank, (ushort)count10, Page);
                        for (int x10 = 0; x10 < count10; x10++)
                        {
                            if (array10.Length > x10 + offset10)
                            {
                                Guild.Member element10;
                                element10 = array10[x10 + offset10];
                                stream.AddItemGuildRanks(Rank, element10, element10.TotalDonation, RealRank10++);
                            }
                        }
                        user.Send(stream.GuildRanksFinalize());
                        break;
                    }
            }
        }
    }
}
