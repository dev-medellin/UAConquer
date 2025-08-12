using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Role.Instance
{
    public class Associate
    {
        public class Member
        {
            public uint UID;

            public ulong Timer;

            public uint ExpBalls;

            public uint Stone;

            public uint Blessing;

            public string Map = "";

            public string Name = "";

            public ushort KillsCount;

            public ushort BattlePower;

            public bool IsOnline => Server.GamePoll.ContainsKey(UID);

            public int GetTimerLeft()
            {
                if (Timer == 0L)
                    return 0;
                int timer;
                timer = (int)(new TimeSpan((long)Timer).TotalMinutes - new TimeSpan(DateTime.Now.Ticks).TotalMinutes);
                if (timer <= 0)
                {
                    Timer = 0uL;
                    return 0;
                }
                return timer;
            }

            public override string ToString()
            {
                WriteLine writer;
                writer = new WriteLine('/');
                writer.Add(UID).Add(Timer).Add(ExpBalls)
                    .Add(Stone)
                    .Add(Blessing)
                    .Add(Name)
                    .Add(KillsCount)
                    .Add(BattlePower)
                    .Add(Map);
                return writer.Close();
            }
        }

        public class MyAsociats
        {
            public ConcurrentDictionary<byte, ConcurrentDictionary<uint, Member>> Associat = new ConcurrentDictionary<byte, ConcurrentDictionary<uint, Member>>();

            public ConcurrentDictionary<uint, GameClient> OnlineApprentice = new ConcurrentDictionary<uint, GameClient>();

            public bool Online;

            public GameClient MyClient;

            public uint MyUID;

            public uint Mentor_Stones;

            public uint Mentor_ExpBalls;

            public uint Mentor_Blessing;

            public bool HaveAsociats()
            {
                foreach (KeyValuePair<byte, ConcurrentDictionary<uint, Member>> items in Associat)
                {
                    if (items.Key != 6 && items.Key != 2 && items.Value.Count > 0)
                        return true;
                }
                return false;
            }

            public MyAsociats(uint uid)
            {
                MyUID = uid;
            }

            public bool OnUse()
            {
                if (MyUID != 0)
                {
                    if (!Associates.ContainsKey(MyUID))
                        Associates.TryAdd(MyUID, this);
                    return true;
                }
                return false;
            }

            public bool Contain(byte Mode, uint UID)
            {
                if (Associat.ContainsKey(Mode) && Associat[Mode].ContainsKey(UID))
                    return true;
                return false;
            }

            public bool Remove(byte Mode, uint UID)
            {
                if (!Associat.ContainsKey(Mode))
                    return false;
                if (!Associat[Mode].ContainsKey(UID))
                    return false;
                Member value;
                return Associat[Mode].TryRemove(UID, out value);
            }

            public bool AllowAdd(byte Mode, uint UID, byte amout)
            {
                if (!Associat.ContainsKey(Mode))
                    return true;
                if (Associat[Mode].ContainsKey(UID))
                    return false;
                if (Associat[Mode].Count < amout)
                    return true;
                return false;
            }

            public void Add(byte mode, Member member)
            {
                if (OnUse())
                {
                    if (Associat.ContainsKey(mode))
                    {
                        Associat[mode].TryAdd(member.UID, member);
                        return;
                    }
                    Associat.TryAdd(mode, new ConcurrentDictionary<uint, Member>());
                    Associat[mode].TryAdd(member.UID, member);
                }
            }

            public void AddPartener(GameClient Owner, Player client)
            {
                if (AllowAdd(3, client.UID, 10))
                {
                    Member member;
                    member = new Member
                    {
                        UID = client.UID,
                        Name = client.Name,
                        Timer = (ulong)DateTime.Now.AddDays(3.0).Ticks
                    };
                    Add(3, member);
                }
                else
                    Owner.SendSysMesage("You have achieved the max number of apprentices.");
            }

            public void AddAprrentice(GameClient Owner, Player client)
            {
                if (AllowAdd(5, client.UID, (byte)TutorInfo.AddAppCount(Owner)))
                {
                    if (OnlineApprentice.TryAdd(client.UID, client.Owner))
                    {
                        Member member;
                        member = new Member
                        {
                            UID = client.UID,
                            Name = client.Name
                        };
                        Add(5, member);
                    }
                }
                else
                    Owner.SendSysMesage("You have achieved the max number of apprentices.");
            }

            public void AddMentor(GameClient Owner, Player client)
            {
                if (AllowAdd(4, client.UID, 1))
                {
                    Member member;
                    member = new Member
                    {
                        UID = client.UID,
                        Name = client.Name
                    };
                    Add(4, member);
                }
            }

            public void AddFriends(GameClient Owner, Player client)
            {
                if (AllowAdd(1, client.UID, 50))
                {
                    Member member;
                    member = new Member
                    {
                        UID = client.UID,
                        Name = client.Name
                    };
                    Add(1, member);
                }
                else
                    Owner.SendSysMesage("You can't have more friends.");
            }

            public uint GetTimePkExplorer()
            {
                uint valu = 0;
                DateTime timer = DateTime.Now;
                valu = (uint)((timer.Month * 1000000) + (timer.Day * 10000) + (timer.Hour * 100) + timer.Minute);
                return valu;
            }

            public void AddPKExplorer(GameClient Owner, Player client)
            {
                if (!OnUse())
                    return;
                Member member;
                member = new Member
                {
                    UID = client.UID,
                    Timer = GetTimePkExplorer(),
                    BattlePower = (ushort)client.BattlePower,
                    Name = client.Name,
                    Map = client.Owner.Map.Name
                };
                member.Timer = GetTimePkExplorer();
                member.KillsCount++;
                if (Associat.ContainsKey(6))
                {
                    if (Associat[6].TryGetValue(member.UID, out var Gmem))
                    {
                        Gmem.Timer = GetTimePkExplorer();
                        Gmem.KillsCount++;
                        Gmem.BattlePower = (ushort)client.BattlePower;
                        return;
                    }
                    if (AllowAdd(6, member.UID, 50))
                    {
                        Associat[6].TryAdd(member.UID, member);
                        return;
                    }
                    Member remover;
                    remover = Associat[6].Values.ToArray()[0];
                    Remove(6, remover.UID);
                    if (AllowAdd(6, member.UID, 50))
                        Add(6, member);
                }
                else
                {
                    Associat.TryAdd(6, new ConcurrentDictionary<uint, Member>());
                    Associat[6].TryAdd(member.UID, member);
                }
            }

            public Member[] GetPkExplorerRank()
            {
                if (Associat.ContainsKey(6))
                    return Associat[6].Values.OrderByDescending((Member kill) => kill.KillsCount).ToArray();
                return new Member[0];
            }

            public void AddEnemy(GameClient Owner, Player Killer)
            {
                Member member;
                member = new Member
                {
                    UID = Killer.UID,
                    Name = Killer.Name
                };
                if (AllowAdd(2, Killer.UID, 20))
                    Add(2, member);
                else
                {
                    Member remover;
                    remover = Associat[2].Values.ToArray()[0];
                    Remove(2, remover.UID);
                    if (AllowAdd(2, Killer.UID, 20))
                        Add(2, member);
                }
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Owner.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddEnemy, Killer.UID, true, Killer.Name, (uint)Killer.NobilityRank, Killer.Body));
                }
            }

            public void OnLoading(GameClient client)
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    MsgApprenticeInformation mentorandapprentice;
                    mentorandapprentice = MsgApprenticeInformation.Create();
                    foreach (KeyValuePair<byte, ConcurrentDictionary<uint, Member>> typ in Associat)
                    {
                        foreach (Member mem in typ.Value.Values)
                        {
                            if (typ.Key == 5)
                            {
                                if (Server.GamePoll.TryGetValue(mem.UID, out var clients2))
                                {
                                    if (client.Player.Associate.OnlineApprentice.TryAdd(clients2.Player.UID, clients2))
                                    {
                                        Member my_apprentice;
                                        my_apprentice = Associat[5][clients2.Player.UID];
                                        mentorandapprentice.Mode = MsgApprenticeInformation.Action.Apprentice;
                                        mentorandapprentice.Mentor_ID = client.Player.UID;
                                        mentorandapprentice.Apprentice_ID = clients2.Player.UID;
                                        mentorandapprentice.Apprentice_Blessing = (ushort)my_apprentice.Blessing;
                                        mentorandapprentice.Apprentice_Composing = (ushort)my_apprentice.Stone;
                                        mentorandapprentice.Apprentice_Experience = (ushort)my_apprentice.ExpBalls;
                                        mentorandapprentice.Class = clients2.Player.Class;
                                        mentorandapprentice.Enrole_date = (uint)my_apprentice.Timer;
                                        mentorandapprentice.Mesh = clients2.Player.Mesh;
                                        mentorandapprentice.Level = (byte)clients2.Player.Level;
                                        mentorandapprentice.Online = 1;
                                        mentorandapprentice.PkPoints = clients2.Player.PKPoints;
                                        mentorandapprentice.WriteString(client.Player.Name, clients2.Player.Spouse, clients2.Player.Name);
                                        client.Send(mentorandapprentice.GetArray(stream));
                                        mentorandapprentice.Mode = MsgApprenticeInformation.Action.Mentor;
                                        mentorandapprentice.Class = client.Player.Class;
                                        mentorandapprentice.Enrole_date = (uint)my_apprentice.Timer;
                                        mentorandapprentice.Mesh = client.Player.Mesh;
                                        mentorandapprentice.Level = (byte)client.Player.Level;
                                        mentorandapprentice.Online = 1;
                                        mentorandapprentice.PkPoints = client.Player.PKPoints;
                                        mentorandapprentice.Shared_Battle_Power = 0;
                                        mentorandapprentice.WriteString(client.Player.Name, clients2.Player.Spouse, clients2.Player.Name);
                                        clients2.Send(mentorandapprentice.GetArray(stream));
                                    }
                                }
                                else
                                {
                                    mentorandapprentice.Class = 0;
                                    mentorandapprentice.Mesh = 0;
                                    mentorandapprentice.Level = 0;
                                    mentorandapprentice.Online = 0;
                                    mentorandapprentice.PkPoints = 0;
                                    mentorandapprentice.Shared_Battle_Power = 0;
                                    mentorandapprentice.Mode = MsgApprenticeInformation.Action.Apprentice;
                                    mentorandapprentice.Mentor_ID = MyUID;
                                    mentorandapprentice.Apprentice_ID = mem.UID;
                                    mentorandapprentice.Enrole_date = (uint)mem.Timer;
                                    mentorandapprentice.WriteString("NULL", "NULL", mem.Name);
                                    client.Send(mentorandapprentice.GetArray(stream));
                                }
                            }
                            if (typ.Key == 4)
                            {
                                if (Server.GamePoll.TryGetValue(mem.UID, out var clients))
                                {
                                    if (clients.Player.Associate.Associat[5].TryGetValue(client.Player.UID, out var apprentice) && clients.Player.Associate.OnlineApprentice.TryAdd(client.Player.UID, client))
                                    {
                                        mentorandapprentice.Mode = MsgApprenticeInformation.Action.Apprentice;
                                        mentorandapprentice.Mentor_ID = clients.Player.UID;
                                        mentorandapprentice.Apprentice_ID = client.Player.UID;
                                        mentorandapprentice.Apprentice_Blessing = (ushort)apprentice.Blessing;
                                        mentorandapprentice.Apprentice_Composing = (ushort)apprentice.Stone;
                                        mentorandapprentice.Apprentice_Experience = (ushort)apprentice.ExpBalls;
                                        mentorandapprentice.Class = client.Player.Class;
                                        mentorandapprentice.Enrole_date = (uint)apprentice.Timer;
                                        mentorandapprentice.Mesh = client.Player.Mesh;
                                        mentorandapprentice.Level = (byte)client.Player.Level;
                                        mentorandapprentice.Online = 1;
                                        mentorandapprentice.PkPoints = client.Player.PKPoints;
                                        mentorandapprentice.WriteString(clients.Player.Name, client.Player.Spouse, client.Player.Name);
                                        clients.Send(mentorandapprentice.GetArray(stream));
                                        mentorandapprentice.Mode = MsgApprenticeInformation.Action.Mentor;
                                        mentorandapprentice.Class = clients.Player.Class;
                                        mentorandapprentice.Enrole_date = (uint)apprentice.Timer;
                                        mentorandapprentice.Mesh = clients.Player.Mesh;
                                        mentorandapprentice.Level = (byte)clients.Player.Level;
                                        mentorandapprentice.Online = 1;
                                        mentorandapprentice.PkPoints = clients.Player.PKPoints;
                                        mentorandapprentice.Shared_Battle_Power = 0;
                                        mentorandapprentice.WriteString(clients.Player.Name, client.Player.Spouse, client.Player.Name);
                                        client.Send(mentorandapprentice.GetArray(stream));
                                    }
                                }
                                else
                                {
                                    mentorandapprentice.Class = 0;
                                    mentorandapprentice.Mesh = 0;
                                    mentorandapprentice.Level = 0;
                                    mentorandapprentice.Online = 0;
                                    mentorandapprentice.PkPoints = 0;
                                    mentorandapprentice.Shared_Battle_Power = 0;
                                    mentorandapprentice.Mode = MsgApprenticeInformation.Action.Mentor;
                                    mentorandapprentice.Mentor_ID = mem.UID;
                                    mentorandapprentice.Apprentice_ID = MyUID;
                                    mentorandapprentice.Enrole_date = (uint)mem.Timer;
                                    mentorandapprentice.WriteString(mem.Name, "", "");
                                    client.Send(mentorandapprentice.GetArray(stream));
                                }
                            }
                            if (typ.Key == 1)
                            {
                                if (Server.GamePoll.TryGetValue(mem.UID, out var Targer3))
                                {
                                    client.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddFriend, mem.UID, true, mem.Name, (uint)Targer3.Player.NobilityRank, Targer3.Player.Body));
                                    Targer3.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddOnline, client.Player.UID, true, client.Player.Name, (uint)client.Player.NobilityRank, client.Player.Body));
                                }
                                else
                                    client.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddFriend, mem.UID, false, mem.Name, 0, 0));
                            }
                            if (typ.Key == 2)
                            {
                                if (Server.GamePoll.TryGetValue(mem.UID, out var Targer2))
                                    client.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddEnemy, mem.UID, true, mem.Name, (uint)Targer2.Player.NobilityRank, Targer2.Player.Body));
                                else
                                    client.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddEnemy, mem.UID, false, mem.Name, 0, 0));
                            }
                            if (typ.Key == 3)
                            {
                                if (Server.GamePoll.TryGetValue(mem.UID, out var Targer))
                                {
                                    client.Send(stream.TradePartnerCreate(mem.UID, MsgTradePartner.Action.AddPartner, true, mem.GetTimerLeft(), mem.Name));
                                    Targer.Send(stream.TradePartnerCreate(client.Player.UID, MsgTradePartner.Action.AddOnline, true, mem.GetTimerLeft(), client.Player.Name));
                                }
                                else
                                    client.Send(stream.TradePartnerCreate(mem.UID, MsgTradePartner.Action.AddPartner, false, mem.GetTimerLeft(), mem.Name));
                            }
                        }
                    }
                }
            }

            public void OnDisconnect(Packet stream, GameClient client)
            {
                MsgApprenticeInformation mentorandapprentice;
                mentorandapprentice = MsgApprenticeInformation.Create();
                foreach (GameClient clients in Server.GamePoll.Values)
                {
                    foreach (KeyValuePair<byte, ConcurrentDictionary<uint, Member>> typ in Associat)
                    {
                        if (typ.Value.ContainsKey(clients.Player.UID))
                        {
                            if (typ.Key == 5)
                            {
                                mentorandapprentice.Mentor_ID = client.Player.UID;
                                mentorandapprentice.Apprentice_ID = clients.Player.UID;
                                clients.Player.SetBattlePowers(0, 0);
                                mentorandapprentice.Mode = MsgApprenticeInformation.Action.Mentor;
                                mentorandapprentice.Online = 0;
                                mentorandapprentice.WriteString(client.Player.Name, clients.Player.Spouse, clients.Player.Name);
                                clients.Send(mentorandapprentice.GetArray(stream));
                            }
                            if (typ.Key == 4)
                            {
                                mentorandapprentice.Mode = MsgApprenticeInformation.Action.Apprentice;
                                mentorandapprentice.Mentor_ID = clients.Player.UID;
                                mentorandapprentice.Apprentice_ID = client.Player.UID;
                                mentorandapprentice.Online = 0;
                                mentorandapprentice.WriteString(clients.Player.Name, client.Player.Spouse, client.Player.Name);
                                clients.Send(mentorandapprentice.GetArray(stream));
                            }
                            if (typ.Key == 1)
                                clients.Send(stream.KnowPersonsCreate(MsgKnowPersons.Action.AddOffline, client.Player.UID, false, client.Player.Name, 0, 0));
                            if (typ.Key == 3)
                                clients.Send(stream.TradePartnerCreate(client.Player.UID, MsgTradePartner.Action.AddOffline, false, 0, client.Player.Name));
                        }
                    }
                }
            }

            public IEnumerable<string> ToStringMember()
            {
                foreach (KeyValuePair<byte, ConcurrentDictionary<uint, Member>> typ in Associat)
                {
                    foreach (Member member in typ.Value.Values)
                    {
                        yield return MyUID + "/" + typ.Key + "/" + Mentor_ExpBalls + "/" + Mentor_Blessing + "/" + Mentor_Stones + "/" + 0 + "/" + 0 + "/" + 0 + "/" + member.ToString();
                    }
                }
            }
        }

        public const byte Friends = 1;

        public const byte Enemy = 2;

        public const byte Partener = 3;

        public const byte Mentor = 4;

        public const byte Apprentice = 5;

        public const byte PKExplorer = 6;

        public static ConcurrentDictionary<uint, MyAsociats> Associates = new ConcurrentDictionary<uint, MyAsociats>();

        public static void RemoveOffline(byte Mode, uint UID, uint OnRemove)
        {
            if (Associates.TryGetValue(UID, out var associate))
                associate.Remove(Mode, OnRemove);
        }

        public static void Save()
        {
            try
            {
                using (Write _wr = new Write("Associate.txt"))
                {
                    foreach (KeyValuePair<uint, MyAsociats> associate in Associates)
                    {
                        foreach (string member in associate.Value.ToStringMember())
                        {
                            _wr.Add(member);
                        }
                    }
                    _wr.Execute(Mode.Open);
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static void Load()
        {
            try
            {
                using (Read r = new Read("Associate.txt"))
                {
                    if (r.Reader())
                    {
                        int count;
                        count = r.Count;
                        for (uint x = 0; x < count; x++)
                        {
                            string[] data;
                            data = r.ReadString("").Split('/');
                            uint UID;
                            UID = uint.Parse(data[0]);
                            byte Mod;
                            Mod = byte.Parse(data[1]);
                            uint MentorExpBalls;
                            MentorExpBalls = uint.Parse(data[2]);
                            uint MentorBless;
                            MentorBless = uint.Parse(data[3]);
                            uint MentorStone;
                            MentorStone = uint.Parse(data[4]);
                            Member membru;
                            membru = new Member
                            {
                                UID = uint.Parse(data[8]),
                                Timer = ulong.Parse(data[9]),
                                ExpBalls = uint.Parse(data[10]),
                                Stone = uint.Parse(data[11]),
                                Blessing = uint.Parse(data[12]),
                                Name = data[13],
                                KillsCount = ushort.Parse(data[14]),
                                BattlePower = ushort.Parse(data[15]),
                                Map = data[16]
                            };
                            if (Associates.ContainsKey(UID))
                            {
                                if (Associates[UID].Associat.ContainsKey(Mod))
                                {
                                    Associates[UID].Associat[Mod].TryAdd(membru.UID, membru);
                                    continue;
                                }
                                Associates[UID].Associat.TryAdd(Mod, new ConcurrentDictionary<uint, Member>());
                                Associates[UID].Associat[Mod].TryAdd(membru.UID, membru);
                                continue;
                            }
                            MyAsociats assoc;
                            assoc = new MyAsociats(UID)
                            {
                                MyUID = UID,
                                Mentor_ExpBalls = MentorExpBalls,
                                Mentor_Blessing = MentorBless,
                                Mentor_Stones = MentorStone
                            };
                            assoc.Associat.TryAdd(Mod, new ConcurrentDictionary<uint, Member>());
                            assoc.Associat[Mod].TryAdd(membru.UID, membru);
                            Associates.TryAdd(UID, assoc);
                        }
                    }
                }
                GC.Collect();
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
