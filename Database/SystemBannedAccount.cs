using DevExpress.Utils.Drawing.Helpers;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database.DBActions;

namespace TheChosenProject.Database
{
    public class SystemBannedAccount
    {
        public static SafeDictionary<uint, SystemBannedAccount.Client> BannedPoll = new SafeDictionary<uint, SystemBannedAccount.Client>();

        public static void AddBan(uint UID, string name, uint Hours, SystemBannedAccount._Type type)
        {
            SystemBannedAccount.Client client = new SystemBannedAccount.Client()
            {
                UID = UID,
                Type = type,
                Hours = Hours,
                Name = name,
                StartBan = DateTime.Now.Ticks,
                Reason = type.ToString()
            };
            SystemBannedAccount.BannedPoll.Add(client.UID, client);
        }
        public static void AddBan(TheChosenProject.Client.GameClient owner, uint Hours, string Reason = "")
        {
            if (owner.Player.UID <= 0 || BannedPoll.ContainsKey(owner.Player.UID))
                return;

            SystemBannedAccount.Client client = new SystemBannedAccount.Client()
            {
                UID = owner.Player.UID,
                Type = _Type.UsingCheat,
                Hours = Hours,
                Name = owner.Player.Name,
                StartBan = DateTime.Now.Ticks,
                Reason = Reason.ToString()
            };
            SystemBannedAccount.BannedPoll.Add(client.UID, client);
        }

        public static bool RemoveBan(uint UID)
        {
            if (!SystemBannedAccount.BannedPoll.ContainsKey(UID))
                return false;
            SystemBannedAccount.BannedPoll.Remove(UID);
            return true;
        }

        public static bool IsUsingCheat(uint UID, out string Messaj)
        {
            if (SystemBannedAccount.BannedPoll.ContainsKey(UID))
            {
                SystemBannedAccount.Client client = SystemBannedAccount.BannedPoll[UID];
                if (client.Type == SystemBannedAccount._Type.UsingCheat)
                {
                    if (DateTime.FromBinary(client.StartBan).AddHours((double)client.Hours) < DateTime.Now)
                    {
                        SystemBannedAccount.BannedPoll.Remove(client.UID);
                    }
                    else
                    {
                        TimeSpan timeSpan = TimeSpan.FromTicks(DateTime.FromBinary(client.StartBan).AddHours((double)client.Hours).Ticks) - TimeSpan.FromTicks(DateTime.Now.Ticks);
                        Messaj = "This account has been banned(" + client.Reason + ") for " + timeSpan.Days.ToString() + " Days " + timeSpan.Hours.ToString() + " Hours " + timeSpan.Minutes.ToString() + " Minutes";
                        return true;
                    }
                }
            }
            Messaj = "";
            return false;
        }

        public static bool IsSilence(uint UID, out string Messaj)
        {
            if (SystemBannedAccount.BannedPoll.ContainsKey(UID))
            {
                SystemBannedAccount.Client client = SystemBannedAccount.BannedPoll[UID];
                if (client.Type == SystemBannedAccount._Type.Silence)
                {
                    if (DateTime.FromBinary(client.StartBan).AddHours((double)client.Hours) < DateTime.Now)
                    {
                        SystemBannedAccount.BannedPoll.Remove(client.UID);
                    }
                    else
                    {
                        TimeSpan timeSpan = TimeSpan.FromTicks(DateTime.FromBinary(client.StartBan).AddHours((double)client.Hours).Ticks) - TimeSpan.FromTicks(DateTime.Now.Ticks);
                        Messaj = string.Format("You have been silenced! You need to wait +{0} minutes to speak again.", (object)timeSpan.Hours);
                        return true;
                    }
                }
            }
            Messaj = "";
            return false;
        }

        public static bool IsArena(uint UID, out string Messaj)
        {
            if (SystemBannedAccount.BannedPoll.ContainsKey(UID))
            {
                SystemBannedAccount.Client client = SystemBannedAccount.BannedPoll[UID];
                if (client.Type == SystemBannedAccount._Type.Arena)
                {
                    if (DateTime.FromBinary(client.StartBan).AddHours((double)client.Hours) < DateTime.Now)
                    {
                        SystemBannedAccount.BannedPoll.Remove(client.UID);
                    }
                    else
                    {
                        TimeSpan timeSpan = TimeSpan.FromTicks(DateTime.FromBinary(client.StartBan).AddHours((double)client.Hours).Ticks) - TimeSpan.FromTicks(DateTime.Now.Ticks);
                        Messaj = string.Format("You have been banned from the Arena Qualifer for {0} minutes!", (object)timeSpan.Minutes);
                        return true;
                    }
                }
            }
            Messaj = "";
            return false;
        }

        public static void Save()
        {
            using (Write write = new Write("BannedAccounts.txt"))
            {
                foreach (SystemBannedAccount.Client client in SystemBannedAccount.BannedPoll.Values)
                    write.Add(client.ToString());
                write.Execute(Mode.Open);
            }
        }

        public static void Load()
        {
            using (Read read = new Read("BannedAccounts.txt"))
            {
                if (!read.Reader())
                    return;
                uint count = (uint)read.Count;
                for (uint index = 0; index < count; ++index)
                {
                    ReadLine readLine = new ReadLine(read.ReadString(""), '/');
                    SystemBannedAccount.Client client = new SystemBannedAccount.Client()
                    {
                        UID = readLine.Read((uint)0),
                        Hours = readLine.Read((uint)0),
                        StartBan = readLine.Read(0L),
                        Name = readLine.Read(""),
                        Reason = readLine.Read("")
                    };
                    SystemBannedAccount.BannedPoll.Add(client.UID, client);
                }
            }
        }

        public enum _Type : byte
        {
            None,
            UsingCheat,
            Arena,
            Silence,
        }

        public class Client
        {
            public uint UID;
            public SystemBannedAccount._Type Type;
            public string Name;
            public uint Hours;
            public long StartBan;
            public string Reason = "";

            public override string ToString()
            {
                WriteLine writeLine = new WriteLine('/');
                writeLine.Add(this.UID).Add((byte)this.Type).Add(this.Hours).Add(this.StartBan).Add(this.Name).Add(this.Reason);
                return writeLine.Close();
            }
        }
    }
}
