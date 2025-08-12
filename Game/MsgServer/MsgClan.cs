using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static Packet ClanMembersCreate(this Packet stream, Clan.Member[] Elements)
        {
            stream.InitWriter();
            stream.Write(4);
            stream.Write(0);
            stream.Write(0);
            stream.Write((uint)Elements.Length);
            foreach (Clan.Member element in Elements)
            {
                stream.Write(element.Name, 16);
                stream.Write((uint)element.Level);
                stream.Write((ushort)element.Rank);
                stream.Write(element.Online ? (ushort)1 : (ushort)0);
                stream.Write((uint)element.Class);
                stream.Write(1);
                stream.Write(element.Donation);
            }
            stream.Finalize((ushort)1312);
            return stream;
        }

        public static Packet ClanBulletinCreate(this Packet stream, GameClient client, Clan clan)
        {
            stream.InitWriter();
            stream.Write(25);
            stream.Write(client.Player.ClanUID);
            stream.Write(0);
            stream.Write(clan.ClanBuletin);
            stream.Finalize((ushort)1312);
            return stream;
        }

        public static Packet ClanRelationCreate(
          this Packet stream,
          uint ClanID,
          string ClanName,
          string ClanLeader,
          MsgClan.Info aTyp)
        {
            stream.InitWriter();
            stream.Write((uint)aTyp);
            stream.Write(ClanID);
            stream.Write(0);
            stream.Write(ClanName, ClanLeader);
            stream.Finalize((ushort)1312);
            return stream;
        }

        public static Packet ClanAppendSingleClientCreate(
          this Packet stream,
          uint ClanID,
          string Name,
          MsgClan.Info aTyp)
        {
            stream.InitWriter();
            stream.Write((uint)aTyp);
            stream.Write(ClanID);
            stream.Write(0);
            stream.Write(Name);
            stream.Finalize((ushort)1312);
            return stream;
        }

        public static Packet ClanCreate(this Packet stream, GameClient client, Clan clan)
        {
            stream.InitWriter();
            string str1 = client.Player.MyClan.ID.ToString() + " " + client.Player.MyClan.Members.Count.ToString() + " 0 " + client.Player.MyClan.Donation.ToString() + " " + client.Player.MyClan.Level.ToString() + " " + ((byte)client.Player.MyClan.Members[client.Player.UID].Rank).ToString() + " 0 " + client.Player.MyClan.BP.ToString() + " 0 0 0 " + client.Player.MyClan.Members[client.Player.UID].Donation.ToString();
            string str2 = "0 0 0 0 0 0 0";
            string str3 = "";
            string str4 = "";
            stream.Write(1);
            stream.Write(client.Player.ClanUID);
            stream.Write(0);
            stream.Write(str1, clan.Name, clan.LeaderName, str2, str3, str4);
            stream.Finalize((ushort)1312);
            return stream;
        }

        public static void GetClan(
          this Packet stream,
          out MsgClan.Info Mode,
          out uint UID,
          out uint dwparam,
          out string[] list)
        {
            Mode = (MsgClan.Info)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            dwparam = stream.ReadUInt32();
            list = stream.ReadStringList();
        }

        public static Packet ClanCallBackCreate(
          this Packet stream,
          MsgClan.Info Mode,
          uint UID,
          uint dwparam,
          string[] list)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Write(UID);
            stream.Write(dwparam);
            if (list != null)
                stream.Write(list);
            stream.Finalize((ushort)1312);
            return stream;
        }

    }
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgClan
    {
        public enum Info : uint
        {
            Info = 1u,
            Members = 4u,
            Recruit = 9u,
            AcceptRecruit = 10u,
            Join = 11u,
            AcceptJoinRequest = 12u,
            AddEnemy = 14u,
            DeleteEnemy = 15u,
            AddAlly = 19u,
            RequestAlly = 17u,
            AcceptAlly = 18u,
            DeleteAlly = 20u,
            TransferLeader = 21u,
            KickMember = 22u,
            Quit = 23u,
            Announce = 24u,
            SetAnnouncement = 25u,
            Dedicate = 26u,
            MyClan = 29u
        }

        [Packet(1312)]
        private static void Process(GameClient client, Packet stream)
        {
            stream.GetClan(out var Mode, out var UID, out var dwparam, out var list);
            switch (Mode)
            {
                case Info.AcceptAlly:
                    if (client.Player.MyClan == null || client.Player.MyClanMember == null || client.Player.MyClanMember.Rank != Clan.Ranks.Leader)
                        break;
                    if (UID == 1)
                    {
                        Clan clan3;
                        if (client.Player.MyClan.Ally.Count >= 5)
                            client.SendSysMesage("The amount of Allies have exceeded the maximum amount.");
                        else if (Clan.Clans.TryGetValue(client.Player.MyClan.RequestAlly, out clan3))
                        {
                            if (client.Player.MyClan.Ally.ContainsKey(clan3.ID))
                                client.SendSysMesage("This clan already is it on your Ally list .");
                            else if (client.Player.MyClan.Enemy.ContainsKey(clan3.ID))
                            {
                                client.SendSysMesage("This clan already is on your Enemy list.");
                            }
                            else if (client.Player.MyClan.Ally.TryAdd(clan3.ID, clan3) && clan3.Ally.TryAdd(client.Player.MyClan.ID, client.Player.MyClan))
                            {
                                client.Player.MyClan.Send(stream.ClanRelationCreate(clan3.ID, clan3.Name, clan3.LeaderName, Info.AddAlly));
                                clan3.Send(stream.ClanRelationCreate(client.Player.MyClan.ID, client.Player.MyClan.Name, client.Player.MyClan.LeaderName, Info.AddAlly));
                            }
                        }
                    }
                    else
                        client.Player.MyClan.RequestAlly = 0u;
                    break;
                case Info.RequestAlly:
                    if (client.Player.MyClan == null || client.Player.MyClanMember == null || client.Player.MyClanMember.Rank != Clan.Ranks.Leader)
                        break;
                    if (client.Player.MyClan.Ally.Count >= 5)
                        client.SendSysMesage("The amount of Allys have exceeded the maximum amount.");
                    else
                    {
                        if (!client.Player.View.TryGetValue(UID, out var obj5, MapObjectType.Player))
                            break;
                        Player player;
                        player = obj5 as Player;
                        if (player.MyClan != null && player.MyClanMember != null && player.MyClanMember.Rank == Clan.Ranks.Leader)
                        {
                            if (client.Player.MyClan.Ally.ContainsKey(player.MyClan.ID))
                            {
                                client.SendSysMesage("This clan already is it on your Ally list .");
                                break;
                            }
                            if (client.Player.MyClan.Enemy.ContainsKey(player.MyClan.ID))
                            {
                                client.SendSysMesage("This clan already is on your Enemy list.");
                                break;
                            }
                            player.MyClan.RequestAlly = client.Player.MyClan.ID;
                            player.Owner.Send(stream.ClanRelationCreate(client.Player.UID, client.Player.MyClan.Name, client.Player.MyClan.LeaderName, Info.RequestAlly));
                        }
                    }
                    break;
                case Info.DeleteAlly:
                    {
                        if (client.Player.MyClan == null || client.Player.MyClanMember == null || client.Player.MyClanMember.Rank != Clan.Ranks.Leader || list.Length == 0)
                            break;
                        string Name3;
                        Name3 = list[0];
                        if (client.Player.MyClan.IsAlly(Name3))
                        {
                            if (client.Player.MyClan.Ally.TryRemove(client.Player.MyClan.GetClanAlly(Name3), out var clan4))
                            {
                                client.Player.MyClan.Send(stream.ClanRelationCreate(clan4.ID, clan4.Name, "", Info.DeleteAlly));
                                if (clan4.Ally.TryRemove(client.Player.MyClan.ID, out var _))
                                    clan4.Send(stream.ClanRelationCreate(client.Player.MyClan.ID, client.Player.MyClan.Name, "", Info.DeleteAlly));
                            }
                        }
                        else
                            client.SendSysMesage("This clan is not on your allies list .");
                        break;
                    }
                case Info.DeleteEnemy:
                    {
                        if (client.Player.MyClan == null || client.Player.MyClanMember == null || client.Player.MyClanMember.Rank != Clan.Ranks.Leader || list.Length == 0)
                            break;
                        string Name2;
                        Name2 = list[0];
                        if (client.Player.MyClan.IsEnemy(Name2))
                        {
                            if (client.Player.MyClan.Enemy.TryRemove(client.Player.MyClan.GetClanEnemy(Name2), out var clan2))
                                client.Player.MyClan.Send(stream.ClanRelationCreate(clan2.ID, clan2.Name, clan2.LeaderName, Info.DeleteEnemy));
                        }
                        else
                            client.SendSysMesage("This clan is not on your enemy list.");
                        break;
                    }
                case Info.AddEnemy:
                    if (client.Player.MyClan != null && client.Player.MyClanMember != null && client.Player.MyClanMember.Rank == Clan.Ranks.Leader && list.Length != 0)
                    {
                        string Name;
                        Name = list[0];
                        Clan clan;
                        if (Name == client.Player.MyClan.Name)
                            client.SendSysMesage("You can't use your own clan name at enemy list.");
                        else if (client.Player.MyClan.Enemy.Count >= 5)
                        {
                            client.SendSysMesage("You can't have more than 5 Enemy's .");
                        }
                        else if (client.Player.MyClan.IsAlly(Name))
                        {
                            client.SendSysMesage("This clan belongs already to Ally's you can't add them to Enemys.");
                        }
                        else if (client.Player.MyClan.IsEnemy(Name))
                        {
                            client.SendSysMesage("This clan is already on your Enemy list.");
                        }
                        else if (client.Player.MyClan.TryGetClan(Name, out clan) && client.Player.MyClan.Enemy.TryAdd(clan.ID, clan))
                        {
                            client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, clan.LeaderName, Info.AddEnemy));
                        }
                    }
                    break;
                case Info.KickMember:
                    if (client.Player.MyClan != null && client.Player.MyClanMember != null && client.Player.MyClanMember.Rank == Clan.Ranks.Leader && list.Length != 0)
                    {
                        string Name5;
                        Name5 = list[0];
                        client.Player.MyClan.RemoveMember(Name5, stream);
                        goto case Info.Members;
                    }
                    break;
                case Info.AcceptRecruit:
                    {
                        if (list.Length != 0 && client.Player.MyClan == null && client.Player.MyClanMember == null && client.Player.View.TryGetValue(UID, out var obj2, MapObjectType.Player) && obj2 != null && obj2 is Player Leader && Leader.MyClan != null && Leader.MyClanMember != null && Leader.MyClanMember.Rank == Clan.Ranks.Leader)
                        {
                            if (Leader.MyClan.Members.Count >= Clan.MaxPlayersInClan(Leader.MyClan.Level))
                                Leader.Owner.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Clan.MaxPlayersInClan(Leader.MyClan.Level) + ".");
                            else
                                Leader.MyClan.AddMember(client, Clan.Ranks.Member, stream);
                        }
                        break;
                    }
                case Info.Recruit:
                    if (client.Player.MyClan != null && client.Player.MyClanMember != null && client.Player.MyClanMember.Rank == Clan.Ranks.Leader)
                    {
                        IMapObj obj;
                        if (client.Player.MyClan.Members.Count >= Clan.MaxPlayersInClan(client.Player.MyClan.Level))
                            client.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Clan.MaxPlayersInClan(client.Player.MyClan.Level) + ".");
                        else if (client.Player.View.TryGetValue(UID, out obj, MapObjectType.Player) && obj != null && obj is Player recuiter && recuiter.MyClan == null && recuiter.MyClanMember == null)
                        {
                            recuiter.Owner.Send(stream.ClanRelationCreate(client.Player.UID, client.Player.MyClan.Name, client.Player.Name, Info.Recruit));
                        }
                    }
                    break;
                case Info.AcceptJoinRequest:
                    if (list.Length != 0)
                    {
                        if (client.Player.MyClan != null && client.Player.MyClanMember != null && client.Player.MyClanMember.Rank == Clan.Ranks.Leader)
                        {
                            IMapObj obj4;
                            if (client.Player.MyClan.Members.Count >= Clan.MaxPlayersInClan(client.Player.MyClan.Level))
                                client.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Clan.MaxPlayersInClan(client.Player.MyClan.Level) + ".");
                            else if (client.Player.View.TryGetValue(UID, out obj4, MapObjectType.Player) && obj4 != null && obj4 is Player member && member.MyClan == null && member.MyClanMember == null)
                            {
                                client.Player.MyClan.AddMember(member.Owner, Clan.Ranks.Member, stream);
                            }
                        }
                    }
                    else
                        client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                    break;
                case Info.Join:
                    {
                        if (client.Player.MyClan == null && client.Player.MyClanMember == null && client.Player.View.TryGetValue(UID, out var obj3, MapObjectType.Player) && obj3 != null && obj3 is Player Leader2 && Leader2.MyClan != null && Leader2.MyClanMember != null && Leader2.MyClanMember.Rank == Clan.Ranks.Leader)
                            Leader2.Owner.Send(stream.ClanAppendSingleClientCreate(client.Player.UID, client.Player.Name, Info.Join));
                        break;
                    }
                case Info.TransferLeader:
                    if (client.Player.MyClan != null && client.Player.MyClanMember != null && client.Player.MyClanMember.Rank == Clan.Ranks.Leader && list.Length != 0)
                    {
                        string Name4;
                        Name4 = list[0];
                        if (!(Name4 == client.Player.Name) && client.Player.MyClan.TryGetMember(Name4, out var member2) && Server.GamePoll.TryGetValue(member2.UID, out var pClient) && pClient.Player.MyClan != null && pClient.Player.MyClanMember != null && pClient.Player.ClanUID == client.Player.ClanUID)
                        {
                            pClient.Player.MyClan.LeaderName = pClient.Player.Name;
                            pClient.Player.ClanRank = 100;
                            pClient.Player.MyClanMember.Rank = Clan.Ranks.Leader;
                            pClient.Player.MyClan.SendThat(stream, pClient);
                            client.Player.MyClanMember.Rank = Clan.Ranks.Member;
                            client.Player.ClanRank = 10;
                            client.Player.MyClan.SendThat(stream, client);
                            client.Player.View.ReSendView(stream);
                            pClient.Player.View.ReSendView(stream);
                        }
                    }
                    break;
                case Info.Quit:
                    if (client.Player.MyClan != null && client.Player.MyClanMember != null)
                    {
                        client.Player.MyClan.RemoveMember(client);
                        client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                    }
                    break;
                case Info.Dedicate:
                    {
                        if (client.Player.MyClan == null || client.Player.MyClanMember == null)
                            break;
                        uint Amount;
                        Amount = UID;
                        client.Player.MessageBox("[Silver] Are you sure you want to spend  " + Amount + " cps to your clan funds?", delegate (GameClient p)
                        {
                            int value;
                            value = (int)Amount;
                            if (client.Player.Money >= value)
                            {
                                client.Player.Money -= (int)Amount;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                p.Player.MyClan.Donation += (int)Amount;
                                p.Player.MyClanMember.Donation += Amount;
                                p.Send(stream.ClanCallBackCreate(Mode, Amount, dwparam, list));
                                p.Player.MyClan.SendThat(stream, p);
                            }
                            else
                                client.CreateBoxDialog($"You don't have {Amount} Silver!");
                        }, null);
                        break;
                    }
                case Info.SetAnnouncement:
                    if (client.Player.MyClan != null && client.Player.MyClanMember != null && client.Player.MyClanMember.Rank == Clan.Ranks.Leader && list.Length != 0)
                    {
                        string Name6;
                        Name6 = list[0];
                        if (Program.NameStrCheck(Name6))
                        {
                            client.Player.MyClan.ClanBuletin = Name6;
                            client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        }
                        else
                            client.SendSysMesage("Your buletin requires alpha-numeric characters (a-z/0-9).");
                    }
                    break;
                case Info.Members:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        Clan.Member[] members;
                        members = client.Player.MyClan.Members.Values.ToArray();
                        Array.Sort(members, delegate (Clan.Member c1, Clan.Member c2)
                        {
                            int result;
                            result = c2.Online.CompareTo(c1.Online);
                            if (c1.Online == c2.Online)
                            {
                                result = c1.Rank.CompareTo(c2.Rank);
                                if (c1.Rank == c2.Rank)
                                    result = c1.Level.CompareTo(c2.Level);
                            }
                            return result;
                        });
                        client.Send(stream.ClanMembersCreate(members));
                        break;
                    }
                case Info.Announce:
                    client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                    break;
                case Info.MyClan:
                    if (client.Player.MyClan != null)
                        try
                        {
                            client.Send(stream.ClanCreate(client, client.Player.MyClan));
                            string ExtraInfo;
                            ExtraInfo = "0 0 0 0 0 0 0";
                            client.Send(stream.ClanAppendSingleClientCreate(client.Player.MyClan.ID, ExtraInfo, Info.MyClan));
                            client.Player.MyClan.SendBuletin(stream, client);
                            break;
                        }
                        catch (Exception e)
                        {
                            ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                            break;
                        }
                    client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                    break;
                case (Info)5u:
                case (Info)6u:
                case (Info)7u:
                case (Info)8u:
                case (Info)13u:
                case (Info)16u:
                case Info.AddAlly:
                case (Info)27u:
                case (Info)28u:
                    break;
            }
        }
    }

}
