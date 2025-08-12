using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.WindowsAPI;

namespace TheChosenProject.Database
{
    public class GuildTable
    {
        public static Dictionary<uint, List<uint>> ally = new Dictionary<uint, List<uint>>();

        public static Dictionary<uint, List<uint>> enemy = new Dictionary<uint, List<uint>>();
        public string ConnectionString = DatabaseConfig.ConnectionString;

        internal static void Save()
        {
            foreach (KeyValuePair<uint, Guild> obj in Guild.GuildPoll)
            {
                if (obj.Value.CanSave)
                {
                    Guild guild;
                    guild = obj.Value;
                    using (Write writer = new Write("Guilds\\" + obj.Key + ".txt"))
                    {
                        writer.Add(guild.ToString()).Add(guild.Recruit.ToString()).Add(guild.AdvertiseRecruit.ToString())
                            .Add(ToStringAlly(guild))
                            .Add(ToStringEnemy(guild))
                            .Add(guild.MyArsenal.ToString())
                            .Add(guild.CTF_Exploits.ToString())
                            .Add(guild.CTF_Next_ConquerPoints.ToString())
                            .Add(guild.CTF_Next_Money.ToString())
                            .Add(guild.CTF_Rank.ToString())
                            .Add(guild.ClaimCtfReward.ToString());
                        writer.Execute(Mode.Open);
                    }
                    //using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
                    //{

                    //    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("insert into cq_syndicate (id,name,announce,leader_name,money) " +
                    //        "values (@id,@name,@announce,@leader_name,@money)"
                    //        , conn))
                    //    {
                    //        conn.Open();
                    //        #region ClearCommand
                    //        try
                    //        {
                    //            using (var cmdd = new MySql.Data.MySqlClient.MySqlCommand("Delete from cq_syndicate where id=" + guild.Info.GuildID + "", conn))
                    //                cmdd.ExecuteNonQuery();

                    //        }
                    //        catch (Exception e)
                    //        {
                    //            Console.WriteLine(e.ToString());
                    //        }
                    //        #endregion

                    //        cmd.Parameters.AddWithValue("@id", guild.Info.GuildID);
                    //        cmd.Parameters.AddWithValue("@name", guild.GuildName);
                    //        cmd.Parameters.AddWithValue("@announce", guild.Bulletin);
                    //        cmd.Parameters.AddWithValue("@leader_name", guild.Info.LeaderName);
                    //        cmd.Parameters.AddWithValue("@money", guild.Info.SilverFund);
                    //        cmd.ExecuteNonQuery();

                    //    }
                    //}
                }
            }
        }

        public static string ToStringAlly(Guild guild)
        {
            WriteLine writer;
            writer = new WriteLine('/');
            writer.Add(guild.Ally.Count);
            foreach (Guild ally in guild.Ally.Values)
            {
                writer.Add(ally.Info.GuildID);
            }
            return writer.Close();
        }

        public static string ToStringEnemy(Guild guild)
        {
            WriteLine writer;
            writer = new WriteLine('/');
            writer.Add(guild.Enemy.Count);
            foreach (Guild enemy in guild.Enemy.Values)
            {
                writer.Add(enemy.Info.GuildID);
            }
            return writer.Close();
        }

        internal static void Load()
        {
            string[] files;
            files = Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Guilds\\");
            foreach (string fname in files)
            {
                using (Read reader = new Read(fname, true))
                {
                    if (!reader.Reader())
                        continue;
                    ReadLine GuildReader;
                    GuildReader = new ReadLine(reader.ReadString("0/"), '/');
                    uint ID;
                    ID = GuildReader.Read(0u);
                    if (ID <= 100000)
                    {
                        if (ID > Guild.Counter.Count)
                            Guild.Counter.Set(ID);
                        Guild guild;
                        guild = new Guild(null, GuildReader.Read("None"), null);
                        guild.Info.GuildID = ID;
                        guild.Info.LeaderName = GuildReader.Read("None");
                        guild.Info.SilverFund = GuildReader.Read(0L);
                        guild.Info.ConquerPointFund = GuildReader.Read(0u);
                        guild.Info.CreateTime = GuildReader.Read(0u);
                        guild.Bulletin = GuildReader.Read("None");
                        guild.UseAdvertise = GuildReader.Read((byte)0) == 1;
                        guild.BuletinEnrole = GuildReader.Read(0);
                        guild.Recruit.Load(reader.ReadString("0/"));
                        guild.AdvertiseRecruit.Load(reader.ReadString("0/"));
                        LoadGuildAlly(ID, reader.ReadString("0/"));
                        LoadGuildEnemy(ID, reader.ReadString("0/"));
                        guild.MyArsenal.Load(reader.ReadString("0/"));
                        try
                        {
                            guild.CTF_Exploits = reader.ReadUInt32(0u);
                        }
                        catch
                        {
                            guild.CTF_Exploits = 0u;
                        }
                        try
                        {
                            guild.CTF_Next_ConquerPoints = reader.ReadUInt32(0u);
                            guild.CTF_Next_Money = reader.ReadUInt32(0u);
                            guild.CTF_Rank = reader.ReadUInt32(0u);
                            guild.ClaimCtfReward = reader.ReadUInt32(0u);
                        }
                        catch
                        {
                        }
                        if (guild.UseAdvertise)
                            Guild.Advertise.Add(guild);
                        if (!Guild.GuildPoll.ContainsKey(guild.Info.GuildID))
                            Guild.GuildPoll.TryAdd(guild.Info.GuildID, guild);
                        guild.MyArsenal.CheckLoad();
                    }
                }
            }
            ExecuteAllyAndEnemy();
            LoadMembers();
            LoadArsenals();
            foreach (Guild guilds in Guild.GuildPoll.Values)
            {
                guilds.CreateMembersRank();
                guilds.UpdateGuildInfo();
            }
            KernelThread.LastGuildPulse = DateTime.Now;
            enemy.Clear();
            ally.Clear();
            ServerKernel.Log.SaveLog("Loaded " + Guild.GuildPoll.Count + " Guilds ! ", true);
            GC.Collect();
        }

        private unsafe static void LoadArsenals()
        {
            int ItemCount = default(int);
            ClientItems.DBItem Item = default(ClientItems.DBItem);
            foreach (Guild guild in Guild.GuildPoll.Values)
            {
                foreach (Guild.Member member in guild.Members.Values)
                {
                    BinaryFile binary;
                    binary = new BinaryFile();
                    if (!binary.Open(ServerKernel.CO2FOLDER + "\\PlayersItems\\" + member.UID + ".bin", FileMode.Open))
                        continue;
                    binary.Read(&ItemCount, 4);
                    for (int x = 0; x < ItemCount; x++)
                    {
                        binary.Read(&Item, sizeof(ClientItems.DBItem));
                        MsgGameItem ClienItem;
                        ClienItem = Item.GetDataItem();
                        if (ClienItem.Inscribed == 1)
                        {
                            guild.MyArsenal.Add(Guild.Arsenal.GetArsenalPosition(ClienItem.ITEM_ID), new Guild.Arsenal.InscribeItem
                            {
                                BaseItem = ClienItem,
                                Name = member.Name,
                                UID = member.UID
                            });
                            member.ArsenalDonation += GetItemDonation(ClienItem);
                        }
                    }
                    binary.Close();
                }
            }
        }

        private static uint GetItemDonation(MsgGameItem Item)
        {
            uint Return;
            Return = 0u;
            switch ((int)(Item.ITEM_ID % 10u))
            {
                case 8:
                    Return = 1000u;
                    break;
                case 9:
                    Return = 16660u;
                    break;
            }
            if ((int)Item.SocketOne > 0 && Item.SocketTwo == Flags.Gem.NoSocket)
                Return += 33330;
            if ((int)Item.SocketOne > 0 && (int)Item.SocketTwo > 0)
                Return += 133330;
            switch (Item.Plus)
            {
                case 1:
                    Return += 90;
                    break;
                case 2:
                    Return += 490;
                    break;
                case 3:
                    Return += 1350;
                    break;
                case 4:
                    Return += 4070;
                    break;
                case 5:
                    Return += 12340;
                    break;
                case 6:
                    Return += 37030;
                    break;
                case 7:
                    Return += 111110;
                    break;
                case 8:
                    Return += 333330;
                    break;
                case 9:
                    Return += 1000000;
                    break;
                case 10:
                    Return += 1033330;
                    break;
                case 11:
                    Return += 1101230;
                    break;
                case 12:
                    Return += 1212340;
                    break;
            }
            return Return;
        }

        private static void LoadMembers()
        {
            IniFile ini;
            ini = new IniFile("");
            string[] files;
            files = Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\");
            foreach (string fname in files)
            {
                ini.FileName = fname;
                uint UID;
                UID = ini.ReadUInt32("Character", "UID", 0u);
                string Name;
                Name = ini.ReadString("Character", "Name", "None");
                uint GuildID;
                GuildID = ini.ReadUInt32("Character", "GuildID", 0u);
                if (GuildID != 0 && Role.Instance.Guild.GuildPoll.TryGetValue(GuildID, out var Guild))
                {
                    ushort Body;
                    Body = ini.ReadUInt16("Character", "Body", 1002);
                    ushort Face;
                    Face = ini.ReadUInt16("Character", "Face", 0);
                    Guild.Member member;
                    member = new Guild.Member
                    {
                        UID = UID,
                        Mesh = (uint)(Face * 10000 + Body),
                        Name = Name,
                        Rank = (Flags.GuildMemberRank)ini.ReadUInt32("Character", "GuildRank", 200u),
                        Class = ini.ReadByte("Character", "Class", 0),
                        CpsDonate = ini.ReadUInt32("Character", "CpsDonate", 0u),
                        MoneyDonate = ini.ReadInt64("Character", "MoneyDonate", 0L),
                        PkDonation = ini.ReadUInt32("Character", "PkDonation", 0u),
                        LastLogin = ini.ReadInt64("Character", "LastLogin", 0L),
                        Level = ini.ReadUInt16("Character", "Level", 0),
                        PrestigePoints = ini.ReadUInt16("Character", "PrestigePoints", 0),
                        CTF_Exploits = ini.ReadUInt32("Character", "CTF_Exploits", 0u),
                        RewardConquerPoints = ini.ReadUInt32("Character", "CTF_RCPS", 0u),
                        RewardMoney = ini.ReadUInt32("Character", "CTF_RM", 0u),
                        CTF_Claimed = ini.ReadByte("Character", "CTF_R", 0)
                    };
                    if (Flowers.ClientPoll.TryGetValue(UID, out var flower))
                    {
                        member.Lilies = flower.Lilies;
                        member.Orchids = flower.Orchids;
                        member.Rouses = flower.RedRoses;
                        member.Tulips = flower.Tulips;
                    }
                    ulong nobilitydonation;
                    nobilitydonation = ini.ReadUInt64("Character", "DonationNobility", 0uL);
                    if (Program.NobilityRanking.TryGetValue(UID, out var nobility))
                        member.NobilityRank = (uint)nobility.Rank;
                    else if (nobilitydonation >= 200000000)
                    {
                        member.NobilityRank = 5u;
                    }
                    else if (nobilitydonation >= 100000000)
                    {
                        member.NobilityRank = 3u;
                    }
                    else if (nobilitydonation >= 30000000)
                    {
                        member.NobilityRank = 1u;
                    }
                    if (!Guild.Members.ContainsKey(member.UID))
                        Guild.Members.TryAdd(member.UID, member);
                }
            }
        }

        public static void ExecuteAllyAndEnemy()
        {
            foreach (KeyValuePair<uint, List<uint>> obj2 in ally)
            {
                foreach (uint guild2 in obj2.Value)
                {
                    if (Guild.GuildPoll.TryGetValue(guild2, out var alyguild) && Guild.GuildPoll.ContainsKey(obj2.Key))
                        Guild.GuildPoll[obj2.Key].Ally.TryAdd(alyguild.Info.GuildID, alyguild);
                }
            }
            foreach (KeyValuePair<uint, List<uint>> obj in enemy)
            {
                foreach (uint guild in obj.Value)
                {
                    if (Guild.GuildPoll.TryGetValue(guild, out var alyenemy) && Guild.GuildPoll.ContainsKey(obj.Key))
                        Guild.GuildPoll[obj.Key].Enemy.TryAdd(alyenemy.Info.GuildID, alyenemy);
                }
            }
        }

        public static void LoadGuildAlly(uint id, string line)
        {
            ReadLine reader;
            reader = new ReadLine(line, '/');
            int count;
            count = reader.Read(0);
            for (int x = 0; x < count; x++)
            {
                if (ally.ContainsKey(id))
                {
                    ally[id].Add(reader.Read(0u));
                    continue;
                }
                ally.Add(id, new List<uint> { reader.Read(0u) });
            }
        }

        public static void LoadGuildEnemy(uint id, string line)
        {
            ReadLine reader;
            reader = new ReadLine(line, '/');
            int count;
            count = reader.Read(0);
            for (int x = 0; x < count; x++)
            {
                if (enemy.ContainsKey(id))
                {
                    enemy[id].Add(reader.Read(0u));
                    continue;
                }
                enemy.Add(id, new List<uint> { reader.Read(0u) });
            }
        }
    }
}
