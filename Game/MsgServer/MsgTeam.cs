using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgTeam
    {
        public enum TeamTypes : uint
        {
            Create,
            JoinRequest,
            ExitTeam,
            AcceptInvitation,
            InviteRequest,
            AcceptJoinRequest,
            Dismiss,
            Kick,
            ForbidJoining,
            UnforbidJoining,
            LootMoneyOff,
            LootMoneyOn,
            LootItemsOff,
            LootItemsOn,
            RejectInvitation,
            Send
        }

        public static void GetTeamPacket(this Packet stream, out TeamTypes Typ, out uint UID)
        {
            Typ = (TeamTypes)stream.ReadUInt32();
            UID = stream.ReadUInt32();
        }

        public static Packet TeamCreate(this Packet stream, TeamTypes Typ, uint UID)
        {
            stream.InitWriter();
            stream.Write((uint)Typ);
            stream.Write(UID);
            stream.Finalize(1023);
            return stream;
        }

        [Packet(1023)]
        private static void Process(GameClient user, Packet stream)
        {
            stream.GetTeamPacket(out var Typ, out var UID);
            if (user.Player.Map == 700)
                return;
            switch (Typ)
            {
                case TeamTypes.Create:
                    user.Team = new Team(user);
                    Typ = TeamTypes.Send;
                    user.Send(stream.TeamCreate(Typ, UID));
                    Typ = TeamTypes.Create;
                    user.Send(stream.TeamCreate(Typ, UID));
                    break;
                case TeamTypes.Dismiss:
                    if (user.Team != null)
                        user.Team.Remove(user, false);
                    else
                        user.SendSysMesage("[Team]You do not have a team yet.");
                    break;
                case TeamTypes.Kick:
                    {
                        GameClient Kicker;
                        if (user.Team == null)
                            user.SendSysMesage("[Team]You do not have a team yet.");
                        else if (user.Team.TryGetMember(UID, out Kicker))
                        {
                            user.Team.Remove(Kicker, false);
                        }
                        break;
                    }
                case TeamTypes.ExitTeam:
                    user.Team?.Remove(user, true);
                    break;
                case TeamTypes.InviteRequest:
                    {
                        if (user.Team != null && user.Team.CkeckToAdd() && user.Team.TeamLider(user) && user.Player.View.TryGetValue(UID, out var Invitee, MapObjectType.Player))
                        {
                            Player Inv;
                            Inv = Invitee as Player;
                            if (Inv.Owner.Team == null)
                            {
                                Inv.Send(stream.PopupInfoCreate(user.Player.UID, Inv.UID, user.Player.Level, user.Player.BattlePower));
                                UID = user.Player.UID;
                                Inv.Send(stream.TeamCreate(Typ, UID));
                            }
                            else
                                user.SendSysMesage("[Team]You are already in a team and cannot join another team.");
                        }
                        break;
                    }
                case TeamTypes.AcceptJoinRequest:
                    {
                        if (user.Team != null && user.Player.Alive && user.Team.CkeckToAdd() && user.Team.TeamLider(user) && !user.Team.ForbidJoin && user.Player.View.TryGetValue(UID, out var obj3, MapObjectType.Player))
                        {
                            GameClient NewTeammate;
                            NewTeammate = (obj3 as Player).Owner;
                            if (NewTeammate.Team != null)
                            {
                                NewTeammate.SendSysMesage("[Team]You are already in a team and cannot join another team.");
                                break;
                            }
                            NewTeammate.Team = user.Team;
                            user.Team.Add(stream, NewTeammate);
                        }
                        break;
                    }
                case TeamTypes.AcceptInvitation:
                    {
                        if (user.Team != null || !user.Player.Alive || !user.Player.View.TryGetValue(UID, out var obj2, MapObjectType.Player))
                            break;
                        GameClient Leader2;
                        Leader2 = (obj2 as Player).Owner;
                        if (Leader2.Team != null)
                        {
                            if (Leader2.Team.CkeckToAdd() && !Leader2.Team.ForbidJoin)
                            {
                                user.Team = Leader2.Team;
                                Leader2.Team.Add(stream, user);
                            }
                        }
                        else
                            user.SendSysMesage($"[Team]The {Leader2.Player.Name} has not created a team.");
                        break;
                    }
                case TeamTypes.JoinRequest:
                    {
                        if (user.Team != null || !user.Player.Alive || !user.Player.View.TryGetValue(UID, out var obj, MapObjectType.Player))
                            break;
                        GameClient Leader;
                        Leader = (obj as Player).Owner;
                        if (Leader.Team != null)
                        {
                            if (Leader.Team.TeamLider(Leader) && Leader.Team.CkeckToAdd())
                            {
                                Leader.Send(stream.PopupInfoCreate(user.Player.UID, Leader.Player.UID, user.Player.Level, user.Player.BattlePower));
                                UID = user.Player.UID;
                                Leader.Send(stream.TeamCreate(Typ, UID));
                            }
                            else
                                user.SendSysMesage($"[Team]The {Leader.Player.Name}'s team is full.");
                        }
                        else
                            user.SendSysMesage($"[Team]The {Leader.Player.Name} has not created a team.");
                        break;
                    }
                case TeamTypes.ForbidJoining:
                    if (user.Team != null)
                    {
                        user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                        user.Team.ForbidJoin = true;
                    }
                    break;
                case TeamTypes.UnforbidJoining:
                    if (user.Team != null)
                    {
                        user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                        user.Team.ForbidJoin = false;
                    }
                    break;
                case TeamTypes.LootMoneyOn:
                    if (user.Team != null)
                    {
                        user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                        user.Team.PickupMoney = true;
                    }
                    break;
                case TeamTypes.LootMoneyOff:
                    if (user.Team != null)
                    {
                        user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                        user.Team.PickupMoney = false;
                    }
                    break;
                case TeamTypes.LootItemsOn:
                    if (user.Team != null)
                    {
                        user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                        user.Team.PickupItems = true;
                    }
                    break;
                case TeamTypes.LootItemsOff:
                    if (user.Team != null)
                    {
                        user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                        user.Team.PickupItems = false;
                    }
                    break;
            }
        }
    }
}
