using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgMentorApprentice
    {
        public enum Action : uint
        {
            RequestApprentice = 1u,
            RequestMentor = 2u,
            LeaveMentor = 3u,
            ExpellApprentice = 4u,
            AcceptRequestApprentice = 8u,
            AcceptRequestMentor = 9u,
            DumpApprentice = 18u,
            DumpMentor = 19u
        }

        public static void GetMentorApprentice(this Packet stream, out Action Mode, out uint UID, out uint dwParam, out uint ShareBattlePower, out bool Online)
        {
            Mode = (Action)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            dwParam = stream.ReadUInt32();
            ShareBattlePower = stream.ReadUInt32();
            Online = stream.ReadInt8() == 1;
        }

        public static Packet MentorApprenticeCreate(this Packet stream, Action Mode, uint UID, uint dwParam, uint ShareBattlePower, bool Online, string Name)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Write(UID);
            stream.Write(dwParam);
            stream.Write(ShareBattlePower);
            stream.Write((byte)(Online ? 1u : 0));
            stream.Write((byte)Name.Length);
            stream.Write(Name, 16);
            stream.ZeroFill(1);
            stream.Finalize(2065);
            return stream;
        }

        [Packet(2065)]
        public static void HandlerMentorAndApprentice(GameClient user, Packet stream)
        {
            stream.GetMentorApprentice(out var Mode, out var UID, out var dwParam, out var ShareBattlePower, out var _);
            switch (Mode)
            {
                case Action.LeaveMentor:
                    if (user.Player.MyMentor == null)
                        break;
                    if (Server.GamePoll.ContainsKey(user.Player.MyMentor.MyUID))
                    {
                        if (user.Player.MyMentor.OnlineApprentice.TryRemove(user.Player.UID, out var _))
                        {
                            user.Send(stream.MentorApprenticeCreate(Action.DumpMentor, UID, dwParam, 0, false, ""));
                            user.Player.MyMentor.MyClient?.Send(stream.MentorApprenticeCreate(Action.DumpApprentice, UID, dwParam, ShareBattlePower, false, user.Player.Name));
                            user.Player.Associate.Remove(4, user.Player.MyMentor.MyUID);
                            user.Player.MyMentor.Remove(5, user.Player.UID);
                            user.Player.SetBattlePowers(0, 0);
                            user.Player.MyMentor = null;
                        }
                    }
                    else
                    {
                        user.Send(stream.MentorApprenticeCreate(Action.DumpMentor, UID, dwParam, 0, false, ""));
                        Associate.RemoveOffline(5, user.Player.MyMentor.MyUID, user.Player.UID);
                        user.Player.Associate.Remove(4, user.Player.MyMentor.MyUID);
                        user.Player.MyMentor = null;
                    }
                    break;
                case Action.ExpellApprentice:
                    {
                        if (Server.GamePoll.TryGetValue(dwParam, out var pClient))
                        {
                            pClient.Send(stream.MentorApprenticeCreate(Action.DumpMentor, UID, dwParam, ShareBattlePower, false, ""));
                            if (pClient.Player.MyMentor != null)
                            {
                                pClient.Player.MyMentor.OnlineApprentice.TryRemove(pClient.Player.UID, out var _);
                                user.Player.Associate.Remove(5, pClient.Player.UID);
                                pClient.Player.Associate.Remove(4, user.Player.UID);
                            }
                            user.Player.Associate.Remove(5, dwParam);
                            pClient.Player.MyMentor = null;
                        }
                        else
                        {
                            Associate.RemoveOffline(4, dwParam, user.Player.UID);
                            user.Player.Associate.Remove(5, dwParam);
                        }
                        break;
                    }
                case Action.RequestMentor:
                    {
                        if (user.Player.View.TryGetValue(dwParam, out var obj2, MapObjectType.Player))
                        {
                            Player player2;
                            player2 = obj2 as Player;
                            player2.Owner.Send(stream.MentorApprenticeCreate(Action.AcceptRequestMentor, user.Player.UID, player2.UID, (uint)user.Player.BattlePower, true, user.Player.Name));
                            player2.Owner.Send(stream.PopupInfoCreate(user.Player.UID, player2.UID, user.Player.Level, user.Player.BattlePower));
                        }
                        break;
                    }
                case Action.RequestApprentice:
                    {
                        if (user.Player.View.TryGetValue(dwParam, out var obj, MapObjectType.Player))
                        {
                            Player player;
                            player = obj as Player;
                            player.Owner.Send(stream.MentorApprenticeCreate(Action.AcceptRequestApprentice, user.Player.UID, player.UID, (uint)user.Player.BattlePower, true, user.Player.Name));
                            player.Owner.Send(stream.PopupInfoCreate(user.Player.UID, player.UID, user.Player.Level, user.Player.BattlePower));
                        }
                        break;
                    }
                case Action.AcceptRequestApprentice:
                    {
                        if (!user.Player.View.TryGetValue(UID, out var obj3, MapObjectType.Player))
                            break;
                        Player target;
                        target = obj3 as Player;
                        if (ShareBattlePower == 1)
                        {
                            if (target.Associate.AllowAdd(5, user.Player.UID, 10) && user.Player.Associate.AllowAdd(4, target.UID, 1))
                            {
                                user.Player.Associate.AddMentor(user, target);
                                target.Associate.AddAprrentice(target.Owner, user.Player);
                                uint EnroleDate;
                                EnroleDate = (uint)(DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day);
                                user.Player.MyMentor = target.Associate;
                                MsgApprenticeInformation Information;
                                Information = MsgApprenticeInformation.Create();
                                Information.Mode = MsgApprenticeInformation.Action.Mentor;
                                Information.Mentor_ID = target.UID;
                                Information.Apprentice_ID = user.Player.UID;
                                Information.Enrole_date = EnroleDate;
                                Information.Level = (byte)target.Level;
                                Information.Class = target.Class;
                                Information.PkPoints = target.PKPoints;
                                Information.Mesh = target.Mesh;
                                Information.Online = 1;
                                Information.Shared_Battle_Power = 0;
                                Information.WriteString(target.Name, user.Player.Spouse, user.Player.Name);
                                user.Send(Information.GetArray(stream));
                                Information.Mode = MsgApprenticeInformation.Action.Apprentice;
                                Information.Mentor_ID = target.UID;
                                Information.Apprentice_ID = user.Player.UID;
                                Information.Enrole_date = EnroleDate;
                                Information.Level = (byte)user.Player.Level;
                                Information.Class = user.Player.Class;
                                Information.PkPoints = user.Player.PKPoints;
                                Information.Mesh = user.Player.Mesh;
                                Information.Online = 1;
                                Information.WriteString(target.Name, user.Player.Spouse, user.Player.Name);
                                target.Owner.Send(Information.GetArray(stream));
                            }
                        }
                        else
                            target.Owner.SendSysMesage(user.Player.Name + " declined your request.");
                        break;
                    }
                case Action.AcceptRequestMentor:
                    {
                        if (!user.Player.View.TryGetValue(UID, out var obj4, MapObjectType.Player))
                            break;
                        Player target2;
                        target2 = obj4 as Player;
                        if (ShareBattlePower == 1)
                        {
                            if (target2.Associate.AllowAdd(4, user.Player.UID, 1) && user.Player.Associate.AllowAdd(5, target2.UID, 10))
                            {
                                user.Player.Associate.AddAprrentice(user, target2);
                                target2.Associate.AddMentor(target2.Owner, user.Player);
                                uint EnroleDate2;
                                EnroleDate2 = (uint)(DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day);
                                target2.MyMentor = user.Player.Associate;
                                MsgApprenticeInformation Information2;
                                Information2 = MsgApprenticeInformation.Create();
                                Information2.Mode = MsgApprenticeInformation.Action.Mentor;
                                Information2.Mentor_ID = user.Player.UID;
                                Information2.Apprentice_ID = target2.UID;
                                Information2.Enrole_date = EnroleDate2;
                                Information2.Level = (byte)user.Player.Level;
                                Information2.Class = user.Player.Class;
                                Information2.PkPoints = user.Player.PKPoints;
                                Information2.Mesh = user.Player.Mesh;
                                Information2.Online = 1;
                                Information2.Shared_Battle_Power = 0;
                                Information2.WriteString(user.Player.Name, target2.Spouse, target2.Name);
                                target2.Owner.Send(Information2.GetArray(stream));
                                Information2.Mode = MsgApprenticeInformation.Action.Apprentice;
                                Information2.Mentor_ID = user.Player.UID;
                                Information2.Apprentice_ID = target2.UID;
                                Information2.Enrole_date = EnroleDate2;
                                Information2.Level = (byte)target2.Level;
                                Information2.Class = target2.Class;
                                Information2.PkPoints = target2.PKPoints;
                                Information2.Mesh = target2.Mesh;
                                Information2.Online = 1;
                                Information2.WriteString(user.Player.Name, target2.Spouse, target2.Name);
                                user.Send(Information2.GetArray(stream));
                            }
                        }
                        else
                            target2.Owner.SendSysMesage(user.Player.Name + " declined your request.");
                        break;
                    }
                case (Action)5u:
                case (Action)6u:
                case (Action)7u:
                    break;
            }
        }
    }
}
