using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgKnowPersons
    {
        public enum Action : byte
        {
            RequestFriendship = 10,
            AcceptRequest = 11,
            AddOnline = 12,
            AddOffline = 13,
            RemovePerson = 14,
            AddFriend = 15,
            RemoveEnemy = 18,
            AddEnemy = 19
        }

        public static void GetKnowPersons(this Packet stream, out uint UID, out Action mode, out bool online)
        {
            UID = stream.ReadUInt32();
            mode = (Action)stream.ReadInt8();
            online = stream.ReadInt8() == 1;
        }

        public static Packet KnowPersonsCreate(this Packet stream, Action Typ, uint UID, bool online, string Name, uint NobilityRank, uint body)
        {
            stream.InitWriter();
            stream.Write(UID);
            stream.Write((byte)Typ);
            stream.Write((byte)(online ? 1u : 0));
            stream.Write((ushort)0);
            stream.Write(NobilityRank);
            if (body % 10u < 3)
                stream.Write(2);
            else
                stream.Write(1);
            stream.Write(Name, 16);
            stream.Finalize(1019);
            return stream;
        }

        [Packet(1019)]
        public static void HandlerKnowPersons(GameClient user, Packet stream)
        {
            stream.GetKnowPersons(out var UID, out var Mode, out var Online);
            switch (Mode)
            {
                case Action.RequestFriendship:
                    {
                        if (!user.Player.Associate.AllowAdd(1, UID, 50) || !Server.GamePoll.TryGetValue(UID, out var target3))
                            break;
                        user.Player.TargetFriend = target3.Player.UID;
                        target3.Player.TargetFriend = user.Player.UID;
                        target3.Player.MessageBox(user.Player.Name + " wants to be your friend.", delegate (GameClient p)
                        {
                            if (p.Player.Associate.AllowAdd(1, p.Player.TargetFriend, 50) && Server.GamePoll.TryGetValue(p.Player.TargetFriend, out var value) && p.Player.UID == value.Player.TargetFriend)
                            {
                                value.Send(stream.KnowPersonsCreate(Action.AddFriend, p.Player.UID, true, p.Player.Name, (uint)p.Player.NobilityRank, p.Player.Body));
                                p.Send(stream.KnowPersonsCreate(Action.AddFriend, value.Player.UID, true, value.Player.Name, (uint)target3.Player.NobilityRank, value.Player.Body));
                                p.Player.Associate.AddFriends(value, value.Player);
                                value.Player.Associate.AddFriends(value, p.Player);
                                p.SendSysMesage($"{p.Player.Name} and {value.Player.Name} are friends from now on!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red, true);
                            }
                        }, null);
                        break;
                    }
                case Action.AcceptRequest:
                    {
                        if (user.Player.Associate.AllowAdd(1, UID, 50) && Server.GamePoll.TryGetValue(UID, out var target) && user.Player.UID == target.Player.TargetFriend)
                        {
                            target.Send(stream.KnowPersonsCreate(Action.AddFriend, user.Player.UID, true, user.Player.Name, (uint)user.Player.NobilityRank, user.Player.Body));
                            user.Send(stream.KnowPersonsCreate(Action.AddFriend, target.Player.UID, true, target.Player.Name, (uint)target.Player.NobilityRank, target.Player.Body));
                            user.Player.Associate.AddFriends(target, target.Player);
                            target.Player.Associate.AddFriends(target, user.Player);
                            user.SendSysMesage($"{user.Player.Name} and {target.Player.Name} are friends from now on!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red, true);
                        }
                        break;
                    }
                case Action.RemovePerson:
                    {
                        if (!user.Player.Associate.Remove(1, UID))
                            break;
                        user.Send(stream.KnowPersonsCreate(Action.RemovePerson, UID, Online, "", 0, 0));
                        if (Server.GamePoll.TryGetValue(UID, out var target2))
                        {
                            if (target2.Player.Associate.Remove(1, user.Player.UID))
                                target2.Send(stream.KnowPersonsCreate(Action.RemovePerson, user.Player.UID, Online, "", (uint)target2.Player.NobilityRank, target2.Player.Body));
                        }
                        else
                            Associate.RemoveOffline(1, UID, user.Player.UID);
                        break;
                    }
                case Action.RemoveEnemy:
                    if (user.Player.Associate.Remove(2, UID))
                        user.Send(stream.KnowPersonsCreate(Action.RemovePerson, UID, Online, "", 0, 0));
                    break;
            }
        }
    }
}
