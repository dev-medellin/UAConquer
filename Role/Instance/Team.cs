using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.ServerCore;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;
using System.Windows.Forms;
using TheChosenProject.WindowsAPI;
using System.Drawing;
using static MongoDB.Driver.WriteConcern;
using static Mysqlx.Notice.Warning.Types;

namespace TheChosenProject.Role.Instance
{
    public class Team
    {
        public static Counter TeamCounter = new Counter(1U);
        public bool ForbidJoin;
        public bool PickupMoney = true;
        public bool PickupItems;
        public bool AutoInvite;
        public byte LowestLevel;
        public MsgTeamEliteGroup.FighterStats PKStats;
        public MsgTeamEliteGroup.Match PkMatch;
        public Counter CounterMembers = new Counter(10U);
        public MsgTeamArena.Match TeamArenaMatch;
        public uint Damage;
        public Team.TournamentProces Status;
        public uint Cheers;
        public Team.StateType ArenaState;
        public DateTime AcceptBoxShow;
        public bool AcceptBox;
        public Time32 InviteTimer;
        public Time32 CreateTimer;
        public Time32 UpdateLeaderLocationStamp;
        public List<uint> SendInvitation = new List<uint>();
        public GameClient Leader;
        public uint UID;
        public ConcurrentDictionary<uint, Team.MemberInfo> Members;
        public string TeamName;
        public const sbyte pScreenDistance = 19;

        public Team.MemberInfo GetMember(uint UID)
        {
            Team.MemberInfo member;
            this.Members.TryGetValue(UID, out member);
            return member;
        }

        public void SendTeamInfo(Team.MemberInfo Member)
        {
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
                Packet stream = recycledPacket.GetStream();
                stream.TeamMemberInfoCreate(MsgTeamMemberInfo.TeamMemberAction.AddMember, new Team.MemberInfo[1]
                {
          Member
                });
                foreach (Team.MemberInfo temate in this.Temates)
                    temate.client?.Send(stream);
            }
        }
        public bool CanGetNoobExperience(Team.MemberInfo Teammate)
        {
            return Teammate.client.Player.Level > LowestLevel && LowestLevel < 70;

        }
        public void ResetTeamArena()
        {
            this.ArenaState = Team.StateType.None;
            this.AcceptBox = false;
            this.Cheers = 0U;
        }

        public Team.MemberInfo[] Temates => this.Members.Values.ToArray<Team.MemberInfo>();

        public Team.MemberInfo[] GetOrdonateMembers()
        {
            return this.Members.Values.OrderBy<Team.MemberInfo, uint>((Func<Team.MemberInfo, uint>)(p => p.Index)).ToArray<Team.MemberInfo>();
        }

        public IEnumerable<GameClient> GetMembers()
        {
            Team.MemberInfo[] memberInfoArray = this.Temates;
            for (int index = 0; index < memberInfoArray.Length; ++index)
                yield return memberInfoArray[index].client;
            memberInfoArray = (Team.MemberInfo[])null;
        }

        public bool IsDead(ushort Map)
        {
            foreach (Team.MemberInfo temate in this.Temates)
            {
                if (temate.client.Player.Alive && (int)temate.client.Player.Map == (int)Map)
                    return false;
            }
            return true;
        }

        public bool ReadyForTeamPK()
        {
            bool flag = this.Members.Count > 0 && this.Leader != null;
            if (!flag)
                return false;
            foreach (Team.MemberInfo temate in this.Temates)
            {
                if (temate.client.Player.InTeamPk && temate.client.Team != null)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public void GetClanShareBp(GameClient Target)
        {
            if (this.Leader == null)
                return;
            Clan clan1 = this.Leader.Player.MyClan;
            Clan clan2 = Target.Player.MyClan;
            if (clan1 == null || clan2 == null)
                return;
            if (Target.Team == null)
            {
                Target.Player.ClanBp = 0U;
                clan2.ShareBattlePower(0U, 0U, Target);
            }
            else if ((int)this.Leader.Player.Map != (int)Target.Player.Map || (int)this.Leader.Player.DynamicID != (int)Target.Player.DynamicID)
            {
                Target.Player.ClanBp = 0U;
                clan2.ShareBattlePower(0U, 0U, Target);
            }
            else if ((int)clan1.ID == (int)clan2.ID)
            {
                if (this.Leader.Player.ClanRank != (ushort)100)
                {
                    Target.Player.ClanBp = 0U;
                    clan2.ShareBattlePower(0U, 0U, Target);
                }
                else if (this.Leader.Player.RealBattlePower > Target.Player.RealBattlePower)
                {
                    int BpShare = (int)((long)(this.Leader.Player.RealBattlePower - Target.Player.RealBattlePower) * (long)this.ProcentClanBp((uint)clan1.BP) / 100L);
                    Target.Player.ClanBp = (uint)BpShare;
                    clan2.ShareBattlePower(this.Leader.Player.UID, (uint)BpShare, Target);
                }
                else
                {
                    Target.Player.ClanBp = 0U;
                    clan2.ShareBattlePower(0U, 0U, Target);
                }
            }
            else
            {
                Target.Player.ClanBp = 0U;
                clan2.ShareBattlePower(0U, 0U, Target);
            }
        }

        public uint ProcentClanBp(uint Bp)
        {
            switch (Bp)
            {
                case 1:
                    return 40;
                case 2:
                    return 50;
                case 3:
                    return 60;
                case 4:
                    return 70;
                default:
                    return 30;
            }
        }

        public void ShareExperience(ServerSockets.Packet stream, Client.GameClient Killer, Game.MsgMonster.MonsterRole Target)
        {
            int Experience = Target.Family.MaxHealth / 50;

            if (IsTeamWithNewbie(Target))
                Experience *= 2;

            AwardMembersExp(stream, Killer, Experience);
        }

        private void AwardMembersExp(ServerSockets.Packet stream, Client.GameClient Killer, int nExp)
        {
            GameClient leader = this.Leader;
            foreach (var user in Temates)
            {
                if (user.client.Player.UID == Killer.Player.UID)
                    continue;

                if (!user.client.Player.Alive)
                    continue;
                if (user.client.Player.Map != Killer.Player.Map)
                    continue;

                if (Core.GetDistance(user.client.Player.X, user.client.Player.Y, Killer.Player.X, Killer.Player.Y) > RoleView.ViewThreshold)
                    continue;

                byte TLevelN = (byte)user.client.Player.Level;
                var teammatePlayer = user.client.Player;
                double expRate;
                ulong nextLevelExp = Server.LevelInfo[DBLevExp.Sort.User][(byte)teammatePlayer.Level].Experience;

                if (user.client.Player.UID == leader.Player.SpouseUID)
                {
                    user.client.IncreaseExperience(stream, (double)(nExp * 2));
                     expRate = (double)(nExp * 2) / nextLevelExp * 10000000;
                    string expSpouse = expRate.ToString("0");
                    user.client.Player.Owner.SendSysMesage(
                        $"You gained extra {expSpouse} EXP. because you are married with {leader.Player.Name} ",
                        MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                }
                else
                {
                    expRate = (float)nExp / nextLevelExp * 10000000;
                    user.client.IncreaseExperience(stream, (double)(nExp));
                }


                teammatePlayer.Owner.SendSysMesage("You gained double exp because there a newbie on your team.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);

                if (teammatePlayer.Owner.Player.DExpTime != 0)
                    expRate *= (double)teammatePlayer.Owner.Player.RateExp;

                string expRateText = expRate.ToString("0"); // Keep 2 decimal places
                                                               // Notify
                teammatePlayer.Owner.SendSysMesage(
                    $"One of your teammates killed a monster. You gained {expRateText} EXP.",
                    MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);

                byte TLevelNn = (byte)user.client.Player.Level;
                byte newLevel = (byte)(TLevelNn - TLevelN);
                if (newLevel != 0)
                {
                    if (TLevelN < 70)
                    {
                        for (int i = TLevelN; i < TLevelNn; i++)
                        {
                            leader.Player.VirtutePoints += (uint)(i * 3.83F);
                            leader.Player.Owner.SendSysMesage("You gained " + (uint)(i * 7.7F) + " virtue points for power leveling the rookies.");
                            user.client.Player.Owner.SendSysMesage("The leader, " + leader.Player.Name + ", has gained " + (uint)(i * 7.7F) + " virtue points for power leveling the rookies.");
                        }
                       
                    }
                }


            }
        }

        //public void ShareExperience(Packet stream, GameClient Killer, MonsterRole Target)
        //{
        //    int maxHealth = Target.Family.MaxHealth;
        //    int killerLevelGap = Killer.Player.Level - Target.Family.Level;
        //    uint adjustedExp = (uint)maxHealth;

        //    if (killerLevelGap >= 30)
        //    {
        //        adjustedExp = (uint)(adjustedExp * 0.01); // 1%
        //    }
        //    else if (killerLevelGap >= 20)
        //    {
        //        adjustedExp = (uint)(adjustedExp * 0.02); // 2%
        //    }

        //    // Optional: Apply extra bonus if killing bot and team has newbie
        //    // if (this.IsTeamWithNewbie(Target) && Killer.AIType == AIEnum.AIType.Leveling)
        //    //     adjustedExp *= (uint)ServerKernel.AWARED_EXPERINCE_FROM_BOT;

        //    this.AwardMembersExp(stream, Killer, (int)adjustedExp, Target);
        //}
        //public void ShareExperience(Packet stream, GameClient Killer, MonsterRole Target)
        //{
        //    int maxHealth = Target.Family.MaxHealth;
        //    int killerLevelGap = Killer.Player.Level - Target.Family.Level;
        //    uint adjustedExp = (uint)maxHealth;

        //    if (killerLevelGap >= 30)
        //    {
        //        adjustedExp = (uint)(adjustedExp * 0.01); // 1%
        //    }
        //    else if (killerLevelGap >= 20)
        //    {
        //        adjustedExp = (uint)(adjustedExp * 0.02); // 2%
        //    }
        //    //if (this.IsTeamWithNewbie(Target) && Killer.AIType == AIEnum.AIType.Leveling)
        //    //    maxHealth *= (int)ServerKernel.AWARED_EXPERINCE_FROM_BOT;
        //    this.AwardMembersExp(stream, Killer, maxHealth, Target);
        //}
        //private uint GetReducedExp(uint exp, int playerLevel, int monsterLevel)
        //{
        //    int gap = playerLevel - monsterLevel;
        //    if (gap >= 30)
        //        return (uint)(exp * 0.01);
        //    else if (gap >= 20)
        //        return (uint)(exp * 0.02);
        //    else
        //        return (uint)(exp * 0.10);
        //}
        //private void AwardMembersExp(Packet stream, GameClient Killer, int baseExp, MonsterRole TargetMob)
        //{
        //    if (Killer.Player.Owner.Team == null)
        //        return;

        //    GameClient leader = this.Leader; //5517 - 5165 co2.0
        //    var killerPlayer = Killer.Player;

        //    // Killer gets EXP
        //    //uint killerFinalExp = GetReducedExp((uint)baseExp, killerPlayer.Level, TargetMob.Family.Level);
        //    killerPlayer.Owner.IncreaseExperience(stream, baseExp, Flags.ExperienceEffect.None);

        //    // Filter active teammates
        //    var validMembers = this.Temates
        //        .Where(teammate =>
        //            teammate.client.Player.UID != killerPlayer.UID &&
        //            teammate.client.Player.Alive &&
        //            Core.GetDistance(killerPlayer.X, killerPlayer.Y, teammate.client.Player.X, teammate.client.Player.Y) <= pScreenDistance)
        //        .ToList();

        //    int activeCount = validMembers.Count;
        //    if (activeCount == 0) return;

        //    foreach (var teammate in validMembers)
        //    {
        //        var teammateClient = teammate.client;
        //        var teammatePlayer = teammateClient.Player;

        //        uint sharedExp = (uint)(baseExp);
        //        byte TLevelN = (byte)teammate.client.Player.Level;
        //        // Spouse Bonus
        //        if (killerPlayer.Spouse == teammatePlayer.Name)
        //            sharedExp = (uint)(baseExp * 2);

        //        bool isNoob = teammatePlayer.Level < 137;

        //        // If team has a noob and this member is allowed to leech
        //        if (Killer.Player.Owner.Team.CanGetNoobExperience(teammate) && isNoob)
        //        {
        //            sharedExp = (uint)(sharedExp * activeCount);
        //        }

        //        // Reduce EXP further if teammate is too high-level for this monster
        //        //sharedExp = GetReducedExp(sharedExp, teammatePlayer.Level, TargetMob.Family.Level);

        //        // Apply EXP
        //        teammatePlayer.Owner.IncreaseExperience(stream, sharedExp, Flags.ExperienceEffect.None);

        //        ulong nextLevelExp = Server.LevelInfo[DBLevExp.Sort.User][(byte)teammatePlayer.Level].Experience;
        //        float expRate = (float)sharedExp / nextLevelExp * 100;
        //        string expRateText = expRate.ToString("0.00"); // Keep 2 decimal places
        //        // Notify
        //        teammatePlayer.Owner.SendSysMesage(
        //            $"One of your teammates killed a monster. You gained {expRateText} EXP.",
        //            MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);

        //        // Virtue Point reward if teammate leveled up and under 70
//        byte TLevelNn = (byte)teammate.client.Player.Level;
//        byte newLevel = (byte)(TLevelNn - TLevelN);
//                if (newLevel != 0)
//                {
//                    if (TLevelN< 70)
//                    {
//                        for (int i = TLevelN; i<TLevelNn; i++)
//                        {
//                            leader.Player.VirtutePoints += (uint) (i* 3.83F);
//                            teammate.client.Player.Owner.SendSysMesage("The leader, " + leader.Player.Name + ", has gained " + (uint) (i* 7.7F) + " virtue points for power leveling the rookies.");
//                        }
//}
//                }
        //    }
        //}

        private void AwardMembersExp(Packet stream, GameClient Killer, int nExp, MonsterRole TargetMob)
        {
            if (Killer.Player.Owner.Team != null)
            {
                Killer.Player.Owner.IncreaseExperience(stream, nExp, Flags.ExperienceEffect.None);
                foreach (Team.MemberInfo teammate in this.Temates)
                {
                    if (Core.GetDistance(Killer.Player.X, Killer.Player.Y, teammate.client.Player.Owner.Player.X, teammate.client.Player.Owner.Player.Y) <= pScreenDistance)
                    {
                        if (Killer.Player.UID != teammate.client.Player.UID)
                        {
                            uint extraExperience;

                            extraExperience = (uint)(nExp / 2);

                            if (Killer.Player.Spouse == teammate.client.Player.Name)
                                extraExperience = (uint)(nExp * 2);
                            byte TLevelN = (byte)teammate.client.Player.Level;
                            GameClient leader = this.Leader;
                            if (Killer.Player.Owner.Team.CanGetNoobExperience(teammate))
                            {
                                if (teammate.client.Player.Level < 137)
                                {
                                    extraExperience *= 2;
                                    teammate.client.Player.Owner.IncreaseExperience(stream, nExp, Flags.ExperienceEffect.None);
                                    teammate.client.Player.Owner.SendSysMesage("One of your teammates killed a monster and because you have a noob inside your team, you gained " + extraExperience + " experience.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                                }
                            }
                            else
                            {
                                if (teammate.client.Player.Level < 137)
                                {
                                    teammate.client.Player.Owner.IncreaseExperience(stream, nExp, Flags.ExperienceEffect.None);
                                    teammate.client.Player.Owner.SendSysMesage("One of your teammates killed a monster so you gained " + extraExperience + " experience.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                                }
                            }
                            byte TLevelNn = (byte)teammate.client.Player.Level;
                            byte newLevel = (byte)(TLevelNn - TLevelN);
                            if (newLevel != 0)
                            {
                                if (TLevelN < 70)
                                {
                                    for (int i = TLevelN; i < TLevelNn; i++)
                                    {
                                        leader.Player.VirtutePoints += (uint)(i * 3.83F);
                                        teammate.client.Player.Owner.SendSysMesage("The leader, " + leader.Player.Name + ", has gained " + (uint)(i * 7.7F) + " virtue points for power leveling the rookies.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //foreach (Team.MemberInfo temate in this.Temates)
            //{
            //    //if (Killer.AIType == AIEnum.AIType.Leveling)
            //    //{
            //    //    if (AtributesStatus.IsWater(temate.client.Player.Class) && temate.client.Player.Level <= 100
            //    //        || AtributesStatus.IsArcher(temate.client.Player.Class) && temate.client.Player.Level <= 110
            //    //        || AtributesStatus.IsWarrior(temate.client.Player.Class) && temate.client.Player.Level <= 110
            //    //        || AtributesStatus.IsTrojan(temate.client.Player.Class) && temate.client.Player.Level <= 110)
            //    //        break;
            //    //}
            //    if ((int)temate.client.Player.UID != (int)Killer.Player.UID && temate.client.Player.Alive && (int)temate.client.Player.Map == (int)Killer.Player.Map && (int)Core.GetDistance(temate.client.Player.X, temate.client.Player.Y, Killer.Player.X, Killer.Player.Y) <= (int)RoleView.ViewThreshold)
            //        temate.client.IncreaseExperience(stream, (double)nExp);
            //}
        }

        public bool IsTeamWithNewbie(MonsterRole Target)
        {
            foreach (Team.MemberInfo temate in this.Temates)
            {
  
                if (temate.client.Player.Alive && (int)temate.client.Player.Map == (int)Target.Map && (int)Core.GetDistance(temate.client.Player.X, temate.client.Player.Y, Target.X, Target.Y) <= (int)RoleView.ViewThreshold /*&& temate.client.Player.NewbieProtection == TheChosenProject.Role.Flags.NewbieExperience.Enable*/)
                    return true;
            }
            return false;
        }

        public bool IsAIWithNewbie()
        {
            bool flag = this.Members.Count > 1 && this.Leader != null;
            if (!flag)
                return false;
            foreach (Team.MemberInfo temate in this.Temates)
            {
                if (temate.client.Player.Position.Distance(this.Leader.Player.Position) <= (int)RoleView.ViewThreshold && temate.client.Team != null)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public Team(GameClient owner)
        {
            this.Status = Team.TournamentProces.None;
            this.Members = new ConcurrentDictionary<uint, Team.MemberInfo>();
            this.UID = Team.TeamCounter.Next;
            this.Leader = owner;
            this.TeamName = this.Leader.Player.Name;
            this.Members.TryAdd(owner.Player.UID, new Team.MemberInfo(owner, this));
            this.AddLider();
            this.CreateTimer = Time32.Now;
        }

        public unsafe void AddLider()
        {
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
                Packet stream = recycledPacket.GetStream();
                if (this.Temates.Length < 1)
                    return;
                Team.MemberInfo ordonateMember = this.GetOrdonateMembers()[0];
                if (ordonateMember.client == null)
                    return;
                this.Leader = ordonateMember.client;
                this.TeamName = this.Leader.Player.Name;
                ordonateMember.Lider = true;
                ordonateMember.client.Player.AddFlag(MsgUpdate.Flags.TeamLeader, 2592000, false);
                TeamLeadership teamLeadership = new TeamLeadership()
                {
                    UID = ordonateMember.client.Player.UID,
                    LeaderUID = ordonateMember.client.Player.UID,
                    Count = this.Members.Count,
                    Typ = MsgTeamLeadership.Mode.Leader
                };
                Team.MemberInfo[] ordonateMembers = this.GetOrdonateMembers();
                foreach (Team.MemberInfo memberInfo in ordonateMembers)
                {
                    if (memberInfo.client != null)
                    {
                        memberInfo.client.Send(stream.TeamMemberInfoCreate(MsgTeamMemberInfo.TeamMemberAction.AddMember, ordonateMembers));
                        memberInfo.client.Send(stream.TeamLeadershipCreate(&teamLeadership));
                    }
                }
                foreach (Team.MemberInfo memberInfo in ordonateMembers)
                {
                    if ((int)memberInfo.client.Player.UID != (int)ordonateMember.client.Player.UID)
                        this.Remove(memberInfo.client, true);
                }
                foreach (Team.MemberInfo memberInfo in ordonateMembers)
                    this.Add(stream, memberInfo.client);
            }
        }

        public unsafe void Add(Packet stream, GameClient client)
        {
            if (this.CkeckToAdd())
            {
                Team.MemberInfo memberInfo1 = new Team.MemberInfo(client, this)
                {
                    Index = this.CounterMembers.Next
                };
                this.Members.TryAdd(client.Player.UID, memberInfo1);
                TeamLeadership teamLeadership1 = new TeamLeadership();
                teamLeadership1.Typ = MsgTeamLeadership.Mode.Teammate;
                TeamLeadership teamLeadership2 = teamLeadership1;
                client.Send(stream.TeamLeadershipCreate(&teamLeadership2));
                teamLeadership1 = new TeamLeadership();
                teamLeadership1.Typ = MsgTeamLeadership.Mode.Leader;
                teamLeadership1.UID = client.Player.UID;
                teamLeadership1.LeaderUID = this.Leader.Player.UID;
                teamLeadership1.Count = this.Members.Count;
                teamLeadership2 = teamLeadership1;
                client.Send(stream.TeamLeadershipCreate(&teamLeadership2));
                teamLeadership2.UID = this.Leader.Player.UID;
                client.Send(stream.TeamLeadershipCreate(&teamLeadership2));
                MsgUpdate msgUpdate = new MsgUpdate(stream, client.Player.UID);
                stream = msgUpdate.Append(stream, MsgUpdate.DataType.Team, (long)this.UID);
                stream = msgUpdate.GetArray(stream);
                client.Send(stream);
                Team.MemberInfo[] ordonateMembers = this.GetOrdonateMembers();
                stream = stream.TeamMemberInfoCreate(MsgTeamMemberInfo.TeamMemberAction.AddMember, ordonateMembers);
                foreach (Team.MemberInfo memberInfo2 in ordonateMembers)
                    memberInfo2.client.Send(stream);
                this.GetClanShareBp(client);
                if(Leader.AIType == AIEnum.AIType.Leveling)

                {
                    string TextMessage;
                    TextMessage = "Welcome " + client.Player.Name + ", follow me to gain more experience!";
                    TextMessage = Translator.GetTranslatedString(TextMessage, Translator.Language.EN, client.Language);
                    Leader.Player.View.SendView(new MsgMessage(TextMessage, Leader.Player.Name, Leader.Player.Name, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream), false);
                }
            }
            else
                client.SendSysMesage("[Team]The team is full.");
        }

        public unsafe void Remove(GameClient client, bool mode)
        {
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
                Packet stream1 = recycledPacket.GetStream();
                Team.MemberInfo memberInfo;
                if (!this.Members.TryGetValue(client.Player.UID, out memberInfo))
                    return;
                foreach (Team.MemberInfo ordonateMember in this.GetOrdonateMembers())
                    ordonateMember.client.Send(stream1.TeamCreate(mode ? MsgTeam.TeamTypes.ExitTeam : MsgTeam.TeamTypes.Kick, memberInfo.client.Player.UID));
                this.Members.TryRemove(client.Player.UID, out memberInfo);
                TeamLeadership teamLeadership = new TeamLeadership()
                {
                    UID = memberInfo.client.Player.UID,
                    Count = this.Members.Count,
                    Typ = MsgTeamLeadership.Mode.Leader
                };
                memberInfo.client.Send(stream1.TeamLeadershipCreate(&teamLeadership));
                MsgUpdate msgUpdate = new MsgUpdate(stream1, client.Player.UID);
                Packet stream2 = msgUpdate.Append(stream1, MsgUpdate.DataType.Team, 0L);
                Packet array = msgUpdate.GetArray(stream2);
                client.Send(array);
                if (memberInfo.Lider)
                {
                    memberInfo.client.Player.RemoveFlag(MsgUpdate.Flags.TeamLeader);
                    this.AddLider();
                }
                memberInfo.client.Team = (Team)null;
                this.GetClanShareBp(client);
            }
        }

        public void SendFunc(Func<GameClient, bool> pr, Packet stream)
        {
            foreach (Team.MemberInfo memberInfo in (IEnumerable<Team.MemberInfo>)this.Members.Values)
            {
                if (pr(memberInfo.client))
                    memberInfo.client.Send(stream);
            }
        }

        public bool TryGetMember(uint UID, out GameClient client)
        {
            Team.MemberInfo memberInfo;
            if (!this.Members.TryGetValue(UID, out memberInfo))
            {
                client = (GameClient)null;
                return false;
            }
            client = memberInfo.client;
            return true;
        }

        public void TeleportTeam(
          ushort map,
          ushort x,
          ushort y,
          uint dinamic = 0,
          Func<GameClient, bool> pr = null)
        {
            foreach (Team.MemberInfo temate in this.Temates)
            {
                if (pr != null)
                {
                    if (pr(temate.client))
                        temate.client.Teleport(x, y, (uint)map, dinamic);
                }
                else
                    temate.client.Teleport(x, y, (uint)map, dinamic);
            }
        }

        public bool IsTeamMember(uint UID) => this.Members.ContainsKey(UID);

        public bool TeamLider(GameClient client)
        {
            return (int)client.Player.UID == (int)this.Leader.Player.UID;
        }

        public void SendTeam(Packet packet, uint UID)
        {
            foreach (Team.MemberInfo temate in this.Temates)
            {
                if ((int)temate.client.Player.UID != (int)UID)
                    temate.client.Send(packet);
            }
        }

        public void UpdatePlayers(Func<GameClient, bool> pr, Action<GameClient> handler)
        {
            foreach (Team.MemberInfo memberInfo in (IEnumerable<Team.MemberInfo>)this.Members.Values)
            {
                if (pr == null)
                    handler(memberInfo.client);
                else if (pr(memberInfo.client))
                    handler(memberInfo.client);
            }
        }

        public void SendTeam(Packet packet, uint UID, uint Map)
        {
            foreach (Team.MemberInfo temate in this.Temates)
            {
                if ((int)temate.client.Player.UID != (int)UID && (int)temate.client.Player.Map == (int)Map)
                    temate.client.Send(packet);
            }
        }

        public bool CkeckToAdd() => this.Members.Count < 5;

        public enum TournamentProces : byte
        {
            None,
            Winner,
            Loser,
        }

        public enum StateType : byte
        {
            None,
            FindMatch,
            WaitForBox,
            WaitForOther,
            Fight,
        }

        public class MemberInfo
        {
            public uint Index;
            public bool Lider;
            public GameClient client;
            public TeamMemberInfo Info;

            public MemberInfo(GameClient _client, Team _team)
            {
                _client.Team = _team;
                this.client = _client;
                this.Info = new TeamMemberInfo()
                {
                    Name = this.client.Player.Name,
                    MaxHitpoints = (ushort)Math.Min((uint)ushort.MaxValue, this.client.Status.MaxHitpoints),
                    Mesh = this.client.Player.Mesh,
                    UID = this.client.Player.UID,
                    MinMHitpoints = (ushort)Math.Min((int)ushort.MaxValue, this.client.Player.HitPoints)
                };
            }
        }
    }
}
