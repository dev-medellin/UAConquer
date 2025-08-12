using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{

    public class MsgGuildMinDonations
    {
        public MsgGuildMinDonations(Packet stream, ushort counts = 0)
        {
            stream.InitWriter();
            stream.Write((ushort)0);
            stream.Write(counts);
        }

        public void Append(Packet stream, Flags.GuildMemberRank Rank, uint amount)
        {
            stream.Write((uint)Rank);
            stream.Write(amount);
        }

        public Packet ToArray(Packet stream)
        {
            stream.Finalize(1061);
            return stream;
        }

        public void AprendGuild(Packet stream, Guild guild)
        {
            if (guild.RankArsenalDonations.Length >= 5)
            {
                Guild.Member obj31;
                obj31 = guild.RankArsenalDonations[4];
                Append(stream, Flags.GuildMemberRank.Manager, obj31.ArsenalDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.Manager, 0u);
            if (guild.RankArsenalDonations.Length >= 7)
            {
                Guild.Member obj30;
                obj30 = guild.RankArsenalDonations[6];
                Append(stream, Flags.GuildMemberRank.HonoraryManager, obj30.ArsenalDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.HonoraryManager, 0u);
            if (guild.RankArsenalDonations.Length >= 8)
            {
                Guild.Member obj29;
                obj29 = guild.RankArsenalDonations[7];
                Append(stream, Flags.GuildMemberRank.Supervisor, obj29.ArsenalDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.Supervisor, 0u);
            if (guild.RankArsenalDonations.Length >= 13)
            {
                Guild.Member obj28;
                obj28 = guild.RankArsenalDonations[12];
                Append(stream, Flags.GuildMemberRank.Steward, obj28.ArsenalDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.Steward, 0u);
            if (guild.RankArsenalDonations.Length >= 15)
            {
                Guild.Member obj27;
                obj27 = guild.RankArsenalDonations[14];
                Append(stream, Flags.GuildMemberRank.ArsFollower, obj27.ArsenalDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.ArsFollower, 0u);
            if (guild.RankCPDonations.Length >= 3)
            {
                Guild.Member obj26;
                obj26 = guild.RankCPDonations[2];
                Append(stream, Flags.GuildMemberRank.CPSupervisor, obj26.CpsDonate);
            }
            else
                Append(stream, Flags.GuildMemberRank.CPSupervisor, 0u);
            if (guild.RankCPDonations.Length >= 5)
            {
                Guild.Member obj25;
                obj25 = guild.RankCPDonations[4];
                Append(stream, Flags.GuildMemberRank.CPAgent, obj25.CpsDonate);
            }
            else
                Append(stream, Flags.GuildMemberRank.CPAgent, 0u);
            if (guild.RankCPDonations.Length >= 7)
            {
                Guild.Member obj24;
                obj24 = guild.RankCPDonations[6];
                Append(stream, Flags.GuildMemberRank.CPFollower, obj24.CpsDonate);
            }
            else
                Append(stream, Flags.GuildMemberRank.CPFollower, 0u);
            if (guild.RankPkDonations.Length >= 3)
            {
                Guild.Member obj23;
                obj23 = guild.RankPkDonations[2];
                Append(stream, Flags.GuildMemberRank.PKSupervisor, obj23.PkDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.PKSupervisor, 0u);
            if (guild.RankPkDonations.Length >= 5)
            {
                Guild.Member obj22;
                obj22 = guild.RankPkDonations[4];
                Append(stream, Flags.GuildMemberRank.PKAgent, obj22.PkDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.PKAgent, 0u);
            if (guild.RankPkDonations.Length >= 7)
            {
                Guild.Member obj21;
                obj21 = guild.RankPkDonations[6];
                Append(stream, Flags.GuildMemberRank.PKFollower, obj21.PkDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.PKFollower, 0u);
            if (guild.RankRosseDonations.Length >= 3)
            {
                Guild.Member obj20;
                obj20 = guild.RankRosseDonations[2];
                Append(stream, Flags.GuildMemberRank.RoseSupervisor, obj20.Rouses);
            }
            else
                Append(stream, Flags.GuildMemberRank.RoseSupervisor, 0u);
            if (guild.RankRosseDonations.Length >= 5)
            {
                Guild.Member obj19;
                obj19 = guild.RankRosseDonations[4];
                Append(stream, Flags.GuildMemberRank.RoseAgent, obj19.Rouses);
            }
            else
                Append(stream, Flags.GuildMemberRank.RoseAgent, 0u);
            if (guild.RankRosseDonations.Length >= 7)
            {
                Guild.Member obj18;
                obj18 = guild.RankRosseDonations[6];
                Append(stream, Flags.GuildMemberRank.RoseFollower, obj18.Rouses);
            }
            else
                Append(stream, Flags.GuildMemberRank.RoseFollower, 0u);
            if (guild.RankLiliesDonations.Length >= 3)
            {
                Guild.Member obj17;
                obj17 = guild.RankLiliesDonations[2];
                Append(stream, Flags.GuildMemberRank.LilySupervisor, obj17.Lilies);
            }
            else
                Append(stream, Flags.GuildMemberRank.LilySupervisor, 0u);
            if (guild.RankLiliesDonations.Length >= 5)
            {
                Guild.Member obj16;
                obj16 = guild.RankLiliesDonations[4];
                Append(stream, Flags.GuildMemberRank.LilyAgent, obj16.Lilies);
            }
            else
                Append(stream, Flags.GuildMemberRank.LilyAgent, 0u);
            if (guild.RankLiliesDonations.Length >= 7)
            {
                Guild.Member obj15;
                obj15 = guild.RankLiliesDonations[6];
                Append(stream, Flags.GuildMemberRank.LilyFollower, obj15.Lilies);
            }
            else
                Append(stream, Flags.GuildMemberRank.LilyFollower, 0u);
            if (guild.RankTulipsDonations.Length >= 3)
            {
                Guild.Member obj14;
                obj14 = guild.RankTulipsDonations[2];
                Append(stream, Flags.GuildMemberRank.TSupervisor, obj14.Tulips);
            }
            else
                Append(stream, Flags.GuildMemberRank.TSupervisor, 0u);
            if (guild.RankTulipsDonations.Length >= 5)
            {
                Guild.Member obj13;
                obj13 = guild.RankTulipsDonations[4];
                Append(stream, Flags.GuildMemberRank.TulipAgent, obj13.Tulips);
            }
            else
                Append(stream, Flags.GuildMemberRank.TulipAgent, 0u);
            if (guild.RankTulipsDonations.Length >= 7)
            {
                Guild.Member obj12;
                obj12 = guild.RankTulipsDonations[6];
                Append(stream, Flags.GuildMemberRank.TulipFollower, obj12.Tulips);
            }
            else
                Append(stream, Flags.GuildMemberRank.TulipFollower, 0u);
            if (guild.RankOrchidsDonations.Length >= 3)
            {
                Guild.Member obj11;
                obj11 = guild.RankOrchidsDonations[2];
                Append(stream, Flags.GuildMemberRank.OSupervisor, obj11.Orchids);
            }
            else
                Append(stream, Flags.GuildMemberRank.OSupervisor, 0u);
            if (guild.RankOrchidsDonations.Length >= 5)
            {
                Guild.Member obj10;
                obj10 = guild.RankOrchidsDonations[4];
                Append(stream, Flags.GuildMemberRank.OrchidAgent, obj10.Orchids);
            }
            else
                Append(stream, Flags.GuildMemberRank.OrchidAgent, 0u);
            if (guild.RankOrchidsDonations.Length >= 7)
            {
                Guild.Member obj9;
                obj9 = guild.RankOrchidsDonations[6];
                Append(stream, Flags.GuildMemberRank.OrchidFollower, obj9.Orchids);
            }
            else
                Append(stream, Flags.GuildMemberRank.OrchidFollower, 0u);
            if (guild.RankTotalDonations.Length >= 2)
            {
                Guild.Member obj8;
                obj8 = guild.RankTotalDonations[1];
                Append(stream, Flags.GuildMemberRank.HDeputyLeader, obj8.TotalDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.HDeputyLeader, 0u);
            if (guild.RankTotalDonations.Length >= 4)
            {
                Guild.Member obj7;
                obj7 = guild.RankTotalDonations[3];
                Append(stream, Flags.GuildMemberRank.HonorarySteward, obj7.TotalDonation);
            }
            else
                Append(stream, Flags.GuildMemberRank.HonorarySteward, 0u);
            if (guild.RankSilversDonations.Length >= 4)
            {
                Guild.Member obj6;
                obj6 = guild.RankSilversDonations[3];
                Append(stream, Flags.GuildMemberRank.SSupervisor, (uint)obj6.MoneyDonate);
            }
            else
                Append(stream, Flags.GuildMemberRank.SSupervisor, 0u);
            if (guild.RankSilversDonations.Length >= 6)
            {
                Guild.Member obj5;
                obj5 = guild.RankSilversDonations[5];
                Append(stream, Flags.GuildMemberRank.SilverAgent, (uint)obj5.MoneyDonate);
            }
            else
                Append(stream, Flags.GuildMemberRank.SilverAgent, 0u);
            if (guild.RankSilversDonations.Length >= 8)
            {
                Guild.Member obj4;
                obj4 = guild.RankSilversDonations[7];
                Append(stream, Flags.GuildMemberRank.SilverFollower, (uint)obj4.MoneyDonate);
            }
            else
                Append(stream, Flags.GuildMemberRank.SilverFollower, 0u);
            if (guild.RankGuideDonations.Length >= 3)
            {
                Guild.Member obj3;
                obj3 = guild.RankGuideDonations[2];
                Append(stream, Flags.GuildMemberRank.GSupervisor, obj3.VirtutePointes);
            }
            else
                Append(stream, Flags.GuildMemberRank.GSupervisor, 0u);
            if (guild.RankGuideDonations.Length >= 5)
            {
                Guild.Member obj2;
                obj2 = guild.RankGuideDonations[4];
                Append(stream, Flags.GuildMemberRank.GuideAgent, obj2.VirtutePointes);
            }
            else
                Append(stream, Flags.GuildMemberRank.GuideAgent, 0u);
            if (guild.RankGuideDonations.Length >= 7)
            {
                Guild.Member obj;
                obj = guild.RankGuideDonations[6];
                Append(stream, Flags.GuildMemberRank.GuideFollower, obj.VirtutePointes);
            }
            else
                Append(stream, Flags.GuildMemberRank.GuideFollower, 0u);
        }
    }
}
