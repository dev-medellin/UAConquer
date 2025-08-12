using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgNameChange
    {
        public enum ActionID : ushort
        {
            Request,
            Success,
            NameTaken,
            DialogInfo,
            FreeChange
        }

        public class ExecuteLogin : ConcurrentSmartThreadQueue<ChangeObject>
        {
            public ExecuteLogin()
                : base(5)
            {
                Start(10);
            }

            public void TryEnqueue(ChangeObject obj)
            {
                base.Enqueue(obj);
            }
            
        protected unsafe override void OnDequeue(ChangeObject obj, int time)
            {
                try
                {
                    Client.GameClient user = obj.user;
                    string Name = obj.Name;

                    switch (obj.mode)
                    {
                        case ActionID.Request:
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (user.Player.Money < ServerKernel.NAME_CHANGE)
                                        break;
                                    if (Name != null && Name != "" && Name.Length >= 4 && Program.NameStrCheck(Name) && user.Player.NameEditCount < 5 && !Database.Server.NameUsed.Contains<int>(Name.GetHashCode()))
                                    {
                                        user.Player.NameEditCount += 1;
                                        user.Send(stream.NameChangeCreate(ActionID.Success, user.Player.NameEditCount, 5, Name));

                                        MsgMessage messaj = new MsgMessage(user.Player.Name + " changed the name to be " + Name + " .The new name will be registered, immediately.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);

                                        Console.WriteLine(user.Player.Name + ", Changed He's/Hers Name to " + Name + ".");
                                        Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));

                                        Role.Instance.Nobility Nobility;
                                        if (Program.NobilityRanking.TryGetValue(user.Player.UID, out Nobility))
                                        {
                                            Nobility.Name = Name;
                                            user.Send(stream.NobilityIconCreate(Nobility));
                                        }

                                       
                                        if (user.Player.MyGuild != null && user.Player.MyGuildMember != null)
                                        {
                                            user.Player.MyGuildMember.Name = Name;
                                            if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                                            {
                                                user.Player.MyGuild.Info.LeaderName = Name;
                                            }
                                        }
                                        if (user.Player.MyClan != null && user.Player.MyClanMember != null)
                                        {
                                            user.Player.MyClanMember.Name = Name;
                                            if (user.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                                                user.Player.MyClan.LeaderName = Name;
                                        }
                                        if (user.ArenaStatistic != null)
                                            user.ArenaStatistic.Name = Name;
                                        if (user.TeamArenaStatistic != null)
                                            user.TeamArenaStatistic.Name = Name;
                                        if (user.Player.Flowers != null)
                                        {
                                            foreach (var flower in user.Player.Flowers)
                                                if (flower != null)
                                                    flower.Name = Name;
                                        }

                                        user.Player.Name = Name;
                                        user.Player.View.ReSendView(stream);

                                        if (user.Player.Spouse != null)
                                        {
                                            Client.GameClient spouse;
                                            if (Database.Server.GamePoll.TryGetValue(user.Player.SpouseUID, out spouse))
                                            {
                                                spouse.Player.Spouse = Name;
                                                spouse.Player.SendString(stream, MsgStringPacket.StringID.Spouse, false, Name);
                                            }
                                            else
                                            {
                                                user.ClientFlag |= Client.ServerFlag.UpdateSpouse;
                                                Database.ServerDatabase.LoginQueue.Enqueue(user);
                                            }
                                        }
                                        if (!user.Player.Name.Contains("[KN]"))
                                        {
                                            user.Player.Money -= ServerKernel.NAME_CHANGE;
                                        }
                                        lock (Database.Server.NameUsed)
                                            Database.Server.NameUsed.Add(Name.GetHashCode());

                                        try
                                        {
                                            Role.Instance.Associate.MyAsociats MyAsociation;
                                            if (Role.Instance.Associate.Associates.TryGetValue(user.Player.UID, out MyAsociation))
                                            {
                                                foreach (var table in MyAsociation.Associat.Values)
                                                {
                                                    if (table == null)
                                                        continue;
                                                    if (table.Values == null)
                                                        continue;
                                                    foreach (var _ass in table.Values)
                                                    {
                                                        if (_ass == null)
                                                            continue;
                                                        Role.Instance.Associate.MyAsociats associat;
                                                        if (Role.Instance.Associate.Associates.TryGetValue(_ass.UID, out associat))
                                                        {
                                                            foreach (var _table in associat.Associat)
                                                            {

                                                                if (_table.Value == null)
                                                                    continue;
                                                                Role.Instance.Associate.Member my_relation;
                                                                if (_table.Value.TryGetValue(user.Player.UID, out my_relation))
                                                                    my_relation.Name = Name;
                                                                if (associat.MyClient != null)//online.
                                                                {
                                                                    if (_table.Key == (byte)Role.Instance.Associate.Friends)
                                                                    {
                                                                        associat.MyClient.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.RemovePerson, user.Player.UID, true, "", (uint)user.Player.NobilityRank, user.Player.Body));
                                                                        associat.MyClient.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddFriend, user.Player.UID, true, Name, (uint)user.Player.NobilityRank, user.Player.Body));
                                                                    }
                                                                    else if (_table.Key == (byte)Role.Instance.Associate.Partener)
                                                                    {
                                                                        associat.MyClient.Send(stream.TradePartnerCreate(user.Player.UID, MsgTradePartner.Action.BreakPartnership, true, 0, ""));
                                                                        associat.MyClient.Send(stream.TradePartnerCreate(user.Player.UID, MsgTradePartner.Action.AddPartner, true, _ass.GetTimerLeft(), Name));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            Console.SaveException(e);
                                        }
                                    }
                                    else
                                        user.Send(stream.NameChangeCreate(ActionID.NameTaken, 0, 0, Name));
                                }
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Console.SaveException(e);
                }
            }
        }

        public class ChangeObject
        {
            public GameClient user;

            public bool Bypass;

            public ActionID mode;

            public ushort EditsCount;

            public ushort EditsAllowed;

            public string Name = "";
        }

        public static ExecuteLogin NameChangeQueue = new ExecuteLogin();

        public static Packet NameChangeCreate(this Packet stream, ActionID mode, ushort EditsCount, ushort EditsAllowed, string Name)
        {
            stream.InitWriter();
            stream.Write((ushort)mode);
            stream.Write(EditsCount);
            stream.Write(EditsAllowed);
            stream.Write(Name, 16);
            stream.Finalize(2080);
            return stream;
        }

        public static void GetNameChange(this Packet stream, out ActionID mode, out ushort EditsCount, out ushort EditsAllowed, out string Name)
        {
            mode = (ActionID)stream.ReadUInt16();
            EditsCount = stream.ReadUInt16();
            EditsAllowed = stream.ReadUInt16();
            Name = stream.ReadCString(16);
        }

        [Packet(2080)]
        public static void Proces(GameClient user, Packet stream)
        {
            ChangeObject obj;
            obj = new ChangeObject
            {
                user = user
            };
            stream.GetNameChange(out obj.mode, out obj.EditsCount, out obj.EditsAllowed, out obj.Name);
            NameChangeQueue.TryEnqueue(obj);
        }

        public static void ChangeName(GameClient user, string name, bool bypass = false)
        {
            ChangeObject obj;
            obj = new ChangeObject
            {
                user = user,
                Bypass = bypass,
                Name = name,
                mode = ActionID.Request
            };
            NameChangeQueue.TryEnqueue(obj);
        }
    }
}
