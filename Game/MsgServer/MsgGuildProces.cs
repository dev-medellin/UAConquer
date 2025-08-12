using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;
using static MongoDB.Driver.WriteConcern;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgGuildProces
    {
        public enum GuildAction : uint
        {
            JoinRequest = 1u,
            AcceptRequest = 2u,
            Quit = 3u,
            InfoName = 6u,
            Allied = 7u,
            RemoveAlly = 8u,
            Enemy = 9u,
            RemoveEnemy = 10u,
            SilverDonate = 11u,
            Show = 12u,
            Disband = 19u,
            CpDonate = 20u,
            RequestAllied = 23u,
            Requirements = 24u,
            Bulletin = 27u,
            Promote = 28u,
            ConfirmPromote = 29u,
            Discharge = 30u,
            Resign = 32u,
            RequestPromote = 37u,
            UpdatePromote = 38u
        }

        public static void GuildRequest(this Packet stream, out GuildAction requesttype, out uint UID, out int[] args, out string[] strlist)
        {
            requesttype = (GuildAction)stream.ReadInt32();
            UID = stream.ReadUInt32();
            args = new int[3];
            for (int i = 0; i < 3; i++)
            {
                args[i] = stream.ReadInt32();
            }
            strlist = stream.ReadStringList();
            _ = new int[2];
            stream.ReadBytes(3);
        }

        public static Packet GuildRequestCreate(this Packet stream, GuildAction requesttype, uint UID, int[] args, params string[] strlist)
        {
            stream.InitWriter();
            stream.Write((uint)requesttype);
            stream.Write(UID);
            stream.Write(args[0]);
            stream.Write(args[1]);
            stream.Write(args[2]);
            stream.Write(strlist);
            stream.ZeroFill(3);
            stream.Finalize(1107);
            return stream;
        }

        [Packet(1107)]
        public static void Process(GameClient user, Packet stream)
        {
            stream.GuildRequest(out var Action, out var UID, out var args, out var strlist);
            switch (Action)
            {
                case GuildAction.Resign:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;

                        user.Player.MyGuild.Promote((uint)Role.Flags.GuildMemberRank.Member, user.Player, user.Player.Name, stream);
                        break;
                    }
                case GuildAction.InfoName:
                    {
                        if (Role.Instance.Guild.GuildPoll.TryGetValue(UID, out var Guild))
                            user.Player.SendString(stream, MsgStringPacket.StringID.GuildName, Guild.Info.GuildID, true, Guild.GuildName + " " + Guild.Info.LeaderName + " " + Guild.Info.Level + " " + Guild.Info.MembersCount);
                        break;
                    }
                case GuildAction.AcceptRequest:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || !user.Player.View.TryGetValue(UID, out var obj2, MapObjectType.Player))
                            break;
                        GameClient Target2;
                        Target2 = null;
                        Target2 = (obj2 as Player).Owner;
                        if (Target2 != null && Target2.Player.MyGuild == null && Target2.Player.MyGuildMember == null)
                        {
                            if (Target2.Player.TargetGuild == user.Player.UID)
                            {
                                Target2.Player.TargetGuild = 0u;
                                user.Player.MyGuild.SendMessajGuild(user.Player.MyGuildMember.Rank.ToString() + " " + user.Player.Name + " of " + user.Player.MyGuild.GuildName + " agress " + Target2.Player.Name + " to join in.");
                                user.Player.MyGuild.AddPlayer(Target2.Player, stream);
                            }
                            else if (!user.Player.MyGuild.Recruit.Compare(Target2.Player, Guild.Recruitment.Mode.Requirements))
                            {
                                Target2.SendSysMesage("Sorry, Guild Recruitment wana block you join request");
                            }
                            else
                            {
                                Target2.AcceptedGuildID = user.Player.GuildID;
                                Target2.Send(stream.PopupInfoCreate(user.Player.UID, Target2.Player.UID, user.Player.Level, user.Player.BattlePower));
                                Target2.Send(stream.GuildRequestCreate(GuildAction.AcceptRequest, user.Player.UID, args, strlist));
                            }
                        }
                        break;
                    }
                case GuildAction.JoinRequest:
                    {
                        if (user.Player.MyGuild != null || !user.Player.View.TryGetValue(UID, out var obj, MapObjectType.Player))
                            break;
                        GameClient Target;
                        Target = null;
                        Target = (obj as Player).Owner;
                        if (Target == null || Target.Player.MyGuild == null || Target.Player.MyGuildMember == null)
                            break;
                        if (!Target.Player.MyGuild.Recruit.Compare(user.Player, Guild.Recruitment.Mode.Requirements))
                            user.SendSysMesage("Sorry, Guild Recruitment wana block you join request");
                        else if (Target.Player.MyGuildMember.Accepter)
                        {
                            if (user.AcceptedGuildID == Target.Player.GuildID)
                            {
                                user.AcceptedGuildID = 0;
                                Target.Player.MyGuild.SendMessajGuild(Target.Player.MyGuildMember.Rank.ToString() + " " + Target.Player.Name + " of " + Target.Player.MyGuild.GuildName + " agress " + user.Player.Name + " to join in.");
                                Target.Player.MyGuild.AddPlayer(user.Player, stream);
                            }
                            else
                            {
                                user.Player.TargetGuild = Target.Player.UID;
                                Target.Send(stream.PopupInfoCreate(user.Player.UID, Target.Player.UID, user.Player.Level, user.Player.BattlePower));
                                Target.Send(stream.GuildRequestCreate(GuildAction.JoinRequest, user.Player.UID, args, strlist));
                            }
                        }
                        break;
                    }
                case GuildAction.SilverDonate:
                    {
                        if (UID >= 10000)
                        {
                            if (user.InTrade)
                                return;                           
                            if (user.Player.Money < UID)
                            {
                                user.CreateBoxDialog($"You don't have {UID} Money!");
                                break;
                            }
                            if (user.Player.MyGuild != null && user.Player.MyGuildMember != null)
                            {
                                user.Player.Money -= (int)UID;
                                user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                user.Player.MyGuildMember.MoneyDonate += UID;
                                user.Player.MyGuild.Info.SilverFund += UID;
                                user.Player.MyGuild.SendThat(user.Player);
                            }
                        }
                        break;
                    }
                case GuildAction.CpDonate:
                    {
                        if (UID >= 1)
                        {
                            if (user.InTrade)
                                return;
                            if (user.Player.ConquerPoints < UID)
                            {
                                user.CreateBoxDialog($"You don't have {UID} ConquerPoints!");
                                break;
                            }
                            if (user.Player.MyGuild != null && user.Player.MyGuildMember != null)
                            {
                                user.Player.ConquerPoints -= (int)UID;

                                user.Player.MyGuildMember.CpsDonate += UID;
                                user.Player.MyGuild.Info.ConquerPointFund += UID;
                                user.Player.MyGuild.SendThat(user.Player);
                            }
                        }
                        break;
                    }
                case GuildAction.Show:
                    {
                        if (user.Player.MyGuild != null)
                        {
                            user.Player.MyGuild.SendThat(user.Player);
                        }
                        break;
                    }
                case GuildAction.Bulletin:
                    {
                        if (user.Player.MyGuild != null)
                        {
                            if (user.Player.Name != user.Player.MyGuild.Info.LeaderName)
                                break;
                            if (strlist.Length > 0 && strlist[0] != null)
                            {
                                if (Program.NameStrCheck(strlist[0], false))
                                {
                                    user.Player.MyGuild.CreateBuletinTime();
                                    user.Player.MyGuild.Bulletin = strlist[0];
                                    user.Player.MyGuild.SendThat(user.Player);
                                }
                                else
                                {
                                    user.SendSysMesage("Invalid Charasters in Bulletin.");
                                }
                            }
                        }
                        break;
                    }
                case GuildAction.Quit:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            user.Player.MyGuild.Quit(user.Player.Name, false, stream);
                        }
                        break;
                    }
                case GuildAction.RequestPromote:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        SendPromote(stream, user, Action);
                        break;
                    }
                case GuildAction.Promote:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        if (strlist.Length > 0 && strlist[0] != null)
                        {
                            user.Player.MyGuild.Promote(UID, user.Player, strlist[0], stream);
                        }
                        break;
                    }
                case GuildAction.RemoveAlly:
                    if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Flags.GuildMemberRank.GuildLeader && strlist.Length != 0 && strlist[0] != null)
                        user.Player.MyGuild.RemoveAlly(strlist[0], stream);
                    break;
                case GuildAction.RequestAllied:
                    if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Flags.GuildMemberRank.GuildLeader && strlist.Length != 0 && strlist[0] != null)
                    {
                        string name3;
                        name3 = strlist[0];
                        if (!(name3 == user.Player.MyGuild.GuildName) && !user.Player.MyGuild.IsEnemy(name3))
                        {
                            user.Player.MyGuild.AddAlly(stream, name3);
                            user.Player.MyGuild.SendMessajGuild($"{user.Player.Name} Guild Leader of {name3} has added our guild to the allies list.");
                        }
                    }
                    break;
                case GuildAction.Allied:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Flags.GuildMemberRank.GuildLeader || strlist.Length == 0 || strlist[0] == null)
                            break;
                        string name;
                        name = strlist[0];
                        if (!(name == user.Player.MyGuild.GuildName) && !user.Player.MyGuild.IsEnemy(name))
                        {
                            Guild.Member leader;
                            leader = Guild.GetLeaderGuild(name);
                            if (leader != null && leader.IsOnline && Server.GamePoll.TryGetValue(leader.UID, out var LeaderClient))
                                LeaderClient.Send(stream.GuildRequestCreate(GuildAction.RequestAllied, 0u, new int[3], user.Player.MyGuild.GuildName));
                        }
                        break;
                    }
                case GuildAction.Enemy:
                    if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Flags.GuildMemberRank.GuildLeader && strlist.Length != 0 && strlist[0] != null)
                    {
                        string name2;
                        name2 = strlist[0];
                        if (!(name2 == user.Player.MyGuild.GuildName) && user.Player.MyGuild.AllowAddAlly(name2))
                        {
                            user.Player.MyGuild.AddEnemy(stream, name2);
                            user.Player.MyGuild.SendMessajGuild($"{user.Player.Name} Guild Leader of {name2} has added our guild to the enemies list.");
                        }
                    }
                    break;
                case GuildAction.RemoveEnemy:
                    if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Flags.GuildMemberRank.GuildLeader && strlist.Length != 0 && strlist[0] != null)
                        user.Player.MyGuild.RemoveEnemy(strlist[0], stream);
                    break;
                case GuildAction.Discharge:
                case (GuildAction)33u:
                    {
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null || user.Player.MyGuildMember.Rank != Flags.GuildMemberRank.GuildLeader || strlist.Length == 0)
                            break;
                        Guild.Member player;
                        player = user.Player.MyGuild.GetMember(strlist[0]);
                        if (player != null && player.Rank != Flags.GuildMemberRank.Member)
                        {
                            user.Player.MyGuild.RanksCounts[(uint)player.Rank]--;
                            user.Player.MyGuild.RanksCounts[200]++;
                            user.Player.MyGuild.Members[player.UID].Rank = Flags.GuildMemberRank.Member;
                            foreach (GameClient userc in Server.GamePoll.Values)
                            {
                                if (userc.Player.Name.ToLower() == player.Name.ToLower())
                                {
                                    userc.Player.MyGuild.Info.MyRank = 200u;
                                    userc.Player.GuildRank = Flags.GuildMemberRank.Member;
                                    userc.Player.View.SendView(userc.Player.GetArray(stream, false), false);
                                    userc.Player.MyGuild.SendThat(userc.Player);
                                    break;
                                }
                            }
                        }
                        user.Player.MyGuild.SendMessajGuild(user.Player.Name + " has discharged " + strlist[0] + " to a member.");
                        break;
                    }
                case GuildAction.Requirements:
                    if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Flags.GuildMemberRank.GuildLeader)
                    {
                        user.Player.MyGuild.Recruit.Level = (byte)args[0];
                        user.Player.MyGuild.Recruit.Reborn = (byte)args[1];
                        user.Player.MyGuild.Recruit.SetFlag(args[2], Guild.Recruitment.Mode.Requirements);
                    }
                    break;
                case GuildAction.UpdatePromote:
                    {
                        if (user.Player.MyGuild == null)
                            break;
                        if (user.Player.MyGuildMember == null)
                            break;

                        if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            Role.Instance.Guild.Member[] Members = user.Player.MyGuild.Members.Values.Where((mem) => mem.Rank == Role.Flags.GuildMemberRank.DeputyLeader
                        || mem.Rank == Role.Flags.GuildMemberRank.Aide || mem.Rank == Role.Flags.GuildMemberRank.Steward || mem.Rank == Role.Flags.GuildMemberRank.Follower).ToArray();

                            user.Send(stream.GuildRankListCreate(MsgGuildMembers.Action.ListRanks, user.Player.MyGuild, Members));
                        }
                        else if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.LeaderSpouse)
                        {
                            Role.Instance.Guild.Member[] Members = user.Player.MyGuild.Members.Values.Where((mem) => mem.Rank == Role.Flags.GuildMemberRank.DeputyLeader
                       || mem.Rank == Role.Flags.GuildMemberRank.Steward || mem.Rank == Role.Flags.GuildMemberRank.Follower).ToArray();
                            user.Send(stream.GuildRankListCreate(MsgGuildMembers.Action.ListRanks, user.Player.MyGuild, Members));
                        }
                        else if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.Manager)
                        {
                            Role.Instance.Guild.Member[] Members = user.Player.MyGuild.Members.Values.Where((mem) => mem.Rank == Role.Flags.GuildMemberRank.Aide).ToArray();
                            user.Send(stream.GuildRankListCreate(MsgGuildMembers.Action.ListRanks, user.Player.MyGuild, Members));
                        }
                        break;
                    }
                
            }
        }

        private static string CreatePromotionString(StringBuilder builder, Flags.GuildMemberRank rank, int occupants, int maxOccupants, int extraBattlePower, int conquerPoints)
        {
            builder.Remove(0, builder.Length);
            builder.Append((int)rank);
            builder.Append(" ");
            builder.Append(occupants);
            builder.Append(" ");
            builder.Append(maxOccupants);
            builder.Append(" ");
            builder.Append(extraBattlePower);
            builder.Append(" ");
            builder.Append(conquerPoints);
            builder.Append(" ");
            return builder.ToString();
        }

        public static unsafe void SendPromote(ServerSockets.Packet stream, Client.GameClient client, GuildAction typ)
        {
            if (client.Player.MyGuild == null) return;
            if (client.Player.MyGuildMember == null) return;
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();

            #region Guild Leader

            if (client.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
            {
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.GuildLeader, 1, 1, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.GuildLeader), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Aide, (int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.Aide), 6, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Aide), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.DeputyLeader, (int)(int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.DeputyLeader), 8, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.DeputyLeader), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Steward, (int)(int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.Steward), 3, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Steward), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Follower, (int)(int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.Follower), 10, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Follower), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Member, (int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.Member), (int)300, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Member), 0));
            }

            #endregion Guild Leader

            #region Leader's Spouse

            if (client.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.LeaderSpouse)
            {
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.DeputyLeader, (int)(int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.DeputyLeader), 4, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.DeputyLeader), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Steward, (int)(int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.Steward), 3, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Steward), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Follower, (int)(int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.Follower), 10, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Follower), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Member, (int)(int)client.Player.MyGuild.RankCount(Role.Flags.GuildMemberRank.Member), (int)300, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Member), 0));
            }

            #endregion Leader's Spouse

            int extraLength = 0;
            foreach (var str in list) extraLength += str.Length + 1;

            client.Send(stream.GuildRequestCreate(typ, 0, new int[3], list.ToArray()));
        }
    }
}
