using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using System.IO;
using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Role.Instance
{
    public class Clan
    {
        public enum Ranks : ushort
        {
            Leader = 100,
            Spouse = 11,
            Member = 10
        }

        public class Member
        {
            public Ranks Rank = Ranks.Member;

            public uint UID;

            public string Name = "";

            public uint Donation;

            public byte Level;

            public byte Class;

            public bool Online;
        }

        public static ConcurrentDictionary<uint, string> ChangeNameRecords = new ConcurrentDictionary<uint, string>();

        public static Counter CounterClansID = new Counter(10u);

        public static ConcurrentDictionary<uint, Clan> Clans = new ConcurrentDictionary<uint, Clan>();

        public ConcurrentDictionary<uint, Clan> Ally = new ConcurrentDictionary<uint, Clan>();

        public ConcurrentDictionary<uint, Clan> Enemy = new ConcurrentDictionary<uint, Clan>();

        public ConcurrentDictionary<uint, Member> Members = new ConcurrentDictionary<uint, Member>();

        public uint RequestAlly;

        public string Name = "";

        public string LeaderName = "";

        public uint ID;

        public byte Level = 1;

        public int Donation;

        public string ClanBuletin = "None";

        public byte BP;

        public static int MaxPlayersInClan(byte level)
        {
            return 11;
        }

        public static void RegisterChangeName(GameClient client,uint clanid, string name)
        {
            if (client.Player.ConquerPoints < ServerKernel.CREATE_CLAN)
            {
                client.CreateBoxDialog($"You don't have enough {ServerKernel.CREATE_CLAN} Conquer Points (CPS) to create Clan.");
                return;
            }
                
            if (!ChangeNameRecords.ContainsKey(clanid))
                ChangeNameRecords.TryAdd(clanid, name);
            else
                ChangeNameRecords[clanid] = name;
        }

        public static void ProcessChangeNames()
        {
            foreach (KeyValuePair<uint, string> record in ChangeNameRecords)
            {
                if (Clans.TryGetValue(record.Key, out var clan))
                    clan.Name = record.Value;
            }
        }

        public bool TryGetClan(string Name, out Clan cln)
        {
            Clan[] dicionary;
            dicionary = Clans.Values.ToArray();
            Clan[] array;
            array = dicionary;
            foreach (Clan cl in array)
            {
                if (cl.Name == Name)
                {
                    cln = cl;
                    return true;
                }
            }
            cln = null;
            return false;
        }

        public override string ToString()
        {
            WriteLine writer;
            writer = new WriteLine('/');
            writer.Add(ID).Add(Name).Add(LeaderName)
                .Add(Level)
                .Add(Donation)
                .Add(ClanBuletin)
                .Add(BP)
                .Add(0)
                .Add(0)
                .Add(0);
            return writer.Close();
        }

        public void Load(string line)
        {
            if (!(line == "") && line != null)
            {
                ReadLine reader;
                reader = new ReadLine(line, '/');
                ID = reader.Read(0u);
                Name = reader.Read("None");
                LeaderName = reader.Read("None");
                Level = reader.Read((byte)0);
                Donation = reader.Read(0);
                ClanBuletin = reader.Read("None");
                BP = reader.Read((byte)0);
            }
        }

        public string SaveAlly()
        {
            WriteLine writer;
            writer = new WriteLine('/');
            writer.Add(Ally.Count);
            foreach (Clan aly in Ally.Values)
            {
                writer.Add(aly.ID);
            }
            return writer.Close();
        }

        public string SaveEnemy()
        {
            WriteLine writer;
            writer = new WriteLine('/');
            writer.Add(Enemy.Count);
            foreach (Clan enemy in Enemy.Values)
            {
                writer.Add(enemy.ID);
            }
            return writer.Close();
        }

        public static bool AllowCreateClan(string Name)
        {
            if (!Program.NameStrCheck(Name))
                return false;
            foreach (string nname in ChangeNameRecords.Values)
            {
                if (nname == Name)
                    return false;
            }
            foreach (Clan clan in Clans.Values)
            {
                if (clan.Name == Name)
                    return false;
            }
            return true;
        }

        public void Create(GameClient client, string ClanName, Packet stream)
        {
            Level = 0;
            BP = 0;
            Name = ClanName;
            ID = CounterClansID.Next;
            LeaderName = client.Player.Name;
            Donation = 1000;
            Clans.TryAdd(ID, this);
            if (AddMember(client, Ranks.Leader, stream))
                Members[client.Player.UID].Donation = 1000;
            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " has created new clan " + ClanName + " .", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
        }

        public void ShareBattlePower(uint leaderUID, uint BpShare, GameClient client)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgUpdate upd;
                upd = new MsgUpdate(stream, client.Player.UID, 2u);
                stream = upd.Append(stream, MsgUpdate.DataType.ClanShareBp, leaderUID);
                stream = upd.Append(stream, MsgUpdate.DataType.ClanShareBp, BpShare);
                client.Send(upd.GetArray(stream));
            }
        }

        public uint ProcentClanBp(uint Bp)
        {
            switch (Bp)
            {
                case 1u:
                    return 40u;
                case 2u:
                    return 50u;
                case 3u:
                    return 60u;
                case 4u:
                    return 70u;
                default:
                    return 30u;
            }
        }

        public bool AddMember(GameClient client, Ranks rnk, Packet stream)
        {
            if (!Members.ContainsKey(client.Player.UID))
            {
                Member memb;
                memb = new Member
                {
                    Class = client.Player.Class,
                    UID = client.Player.UID,
                    Level = (byte)client.Player.Level,
                    Rank = rnk,
                    Online = true,
                    Name = client.Player.Name
                };
                Members.TryAdd(memb.UID, memb);
                client.Player.MyClanMember = memb;
                client.Player.MyClan = this;
                client.Player.ClanName = Name;
                client.Player.ClanRank = (ushort)rnk;
                client.Player.ClanUID = ID;
                SendThat(stream, client);
                SendBuletin(stream, client);
                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                Send(new MsgMessage(client.Player.Name + " has join in clan !", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.Talk).GetArray(stream));
                return true;
            }
            return false;
        }

        public void SendThat(Packet stream, GameClient client)
        {
            client.Send(stream.ClanCreate(client, this));
        }

        public void Send(Packet data)
        {
            GameClient[] dicionary;
            dicionary = Server.GamePoll.Values.ToArray();
            GameClient[] array;
            array = dicionary;
            foreach (GameClient client in array)
            {
                if (client.Player.ClanUID == ID)
                    client.Send(data);
            }
        }

        public void RemoveMember(string name, Packet stream)
        {
            if (!TryGetMember(name, out var member))
                return;
            if (Server.GamePoll.TryGetValue(member.UID, out var pClient))
            {
                if (pClient.Player.MyClan != null && pClient.Player.MyClanMember != null)
                {
                    RemoveMember(pClient);
                    pClient.Player.View.ReSendView(stream);
                }
            }
            else if (Members.TryRemove(member.UID, out member))
            {
                ServerDatabase.LoginQueue.Enqueue(member);
            }
        }

        public bool RemoveMember(GameClient client)
        {
            if (Members.TryRemove(client.Player.UID, out var _))
            {
                client.Player.MyClanMember = null;
                client.Player.MyClan = null;
                client.Player.ClanName = "";
                client.Player.ClanRank = 0;
                client.Player.ClanUID = 0u;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    client.Send(stream.ClanCallBackCreate(MsgClan.Info.Quit, 0u, 0u, null));
                }
                ShareBattlePower(0u, 0u, client);
                client.Player.ClanBp = 0u;
                return true;
            }
            return false;
        }

        public void DisbandClan(Packet stream)
        {
            if (!Clans.TryRemove(ID, out var _))
                return;
            Program.SendGlobalPackets.Enqueue(new MsgMessage(LeaderName + " has disbanded the clan " + Name + " !", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
            foreach (Member memeber in Members.Values)
            {
                if (Server.GamePoll.TryGetValue(memeber.UID, out var client))
                {
                    RemoveMember(client);
                    client.Player.View.ReSendView(stream);
                }
                else
                    ServerDatabase.LoginQueue.TryEnqueue(memeber);
            }
            File.Delete($"{ServerKernel.CO2FOLDER}\\Clans\\{ID}.txt");
        }

        public bool TryGetMember(string MemberName, out Member membe)
        {
            foreach (Member obj in Members.Values)
            {
                if (obj.Name == MemberName)
                {
                    membe = obj;
                    return true;
                }
            }
            membe = null;
            return false;
        }

        public void SendBuletin(Packet stream, GameClient client)
        {
            if (ClanBuletin != "None")
                client.Send(stream.ClanBulletinCreate(client, this));
        }

        public bool IsEnemy(string name)
        {
            foreach (Clan cln in Enemy.Values)
            {
                if (cln.Name == name)
                    return true;
            }
            return false;
        }

        public bool IsAlly(string name)
        {
            foreach (Clan cln in Ally.Values)
            {
                if (cln.Name == name)
                    return true;
            }
            return false;
        }

        public uint GetClanAlly(string a_name)
        {
            foreach (Clan cln in Ally.Values)
            {
                if (a_name == cln.Name)
                    return cln.ID;
            }
            return 0u;
        }

        public uint GetClanEnemy(string a_name)
        {
            foreach (Clan cln in Enemy.Values)
            {
                if (a_name == cln.Name)
                    return cln.ID;
            }
            return 0u;
        }
    }
}