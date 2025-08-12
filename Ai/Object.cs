using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Ai
{
    public class Object
    {
        public static uint[] Class = new uint[]
        {
            15,
            //55,
            25,
            45,
            //55,
            //65,
            //75,
            //85,
            135,
            145
            //165
        };
        public static string[] Names = new string[]
        {
            "Mace", "Falchion", "Montante", "Battleaxe", "Zweihander", "Hatchet",
            "Billhook", "Club", "Hammer", "Caltrop", "Maul", "Sledgehammer", "Longbow",
            "Bludgeon", "Harpoon", "Crossbow", "Lance", "Angon", "Pike", "Tiger Claw", "FireLance",
            "Poleaxe", "BrassKnuckle", "Matchlock", "Quarterstaff", "Gauntlet", "Bullwhip", "WarHammer", "Katar",
            "FlyingClaw", "Spear", "Dagger", "Slungshot", "Katana", "Gladius", "Aspis", "Saber", "Cutlass",
            "Blade", "Broadsword", "Scimitar", "Lockback", "Claymore", "Espada", "Machete", "Grizzly", "Wolverine",
            "Deathstalker", "Snake", "Wolf", "Scorpion", "Vulture", "Claw", "Boomslang", "Falcon", "Fang", "Viper",
            "Ram", "Grip", "Sting", "Boar", "BlackMamba", "Lash", "Tusk", "Goshawk", "Gnaw", "Amazon", "Majesty",
            "Anomoly", "Malice", "Banshee", "Mannequin", "Belladonna", "Minx", "Beretta", "Mirage", "BlackBeauty",
            "Nightmare", "Calypso", "Nova", "Carbon", "Pumps", "Cascade", "Raven", "Colada", "Resin", "Cosma", "Riveter",
            "Cougar", "Rogue", "Countess", "Roulette", "Enchantress", "Shadow", "Enigma", "Siren", "FemmeFatale", "Stiletto",
            "Firecracker", "Tattoo", "Geisha", "T-Back", "Goddess", "Temperance", "HalfPint", "Tequila", "Harlem", "Terror", "Heroin",
            "Thunderbird", "Infinity", "Ultra", "Insomnia", "Vanity", "Ivy", "Velvet", "Legacy", "Vixen", "Lithium", "Voodoo", "Lolita",
            "Wicked", "Lotus", "Widow", "Mademoiselle", "Xenon", "Kahina", "Teuta", "Isis", "Dihya", "Artemis", "Nefertiti", "RunningEagle",
            "Atalanta", "Sekhmet", "Colestah", "Athena", "Ishtar", "Calamity Jane", "Enyo", "Ashtart", "PearlHeart", "Bellona", "Juno", "BelleStarr",
            "WhiteTights", "Tanit", "HuaMulan", "Shieldmaiden", "Devi", "Boudica", "Valkyrie", "Selkie", "Medb", "Cleo", "Venus", "Fate", "Beguile", "Deviant",
            "Illusion", "Crafty", "Variance", "Delusion", "Deceit", "Caprice", "Deception", "Waylay", "Aberr", "Myth", "Ambush", "Variant", "Daydream", "Feint", "Hero",
            "Night Terror", "Catch-22", "Villain", "Figment", "Puzzler", "Daredevil", "Virtual", "Curio", "Mercenary", "Chicanery", "Prodigy", "Voyager", "Trick",
            "Breach", "Wanderer", "Vile", "MissFortune", "Audacity", "Horror", "Vex", "Swagger", "Dismay", "Grudge", "Nerve", "Phobia", "Enmity", "Egomania", "Fright",
            "Animus", "Scheme", "Panic", "Hostility", "Paramour", "Agony", "Rancor", "X-hibit", "Inferno", "Malevolence", "Charade", "Blaze", "Poison", "Hauteur", "Crucible",
            "Spite", "Vainglory", "Haunter", "Spitefulness", "Narcissus", "Bane", "Venom", "Brass","Volcano","Vampire","Hulk"
        };

        public Client.GameClient BEntity;
        public Equipment Equipment;
        public Role.GameMap Map = null;

        public bool Exit = false;
        public DateTime TimeToExit = DateTime.Now;
        public uint OwnerUID = 0;
        public bool HaveOwner;
        public bool isVendor = false;
        public uint UID;
        public uint MapID;
        public ushort X;
        public ushort Y;
        public int HP;

        public Object(bool have_owner = false)
        {
            BEntity = new Client.GameClient(null);
            BEntity.MyVendor = new Role.Instance.Vendor(BEntity);
            HaveOwner = have_owner;
        }
        public bool Add(bool auto = false, bool vendor = false, ushort v_x = 0, ushort v_y = 0, string name = "", uint uid = 0, Client.GameClient gameClient = null)
        {
            ushort x = (ushort)Program.GetRandom.Next((int)X - 10, X + 11);
            ushort y = (ushort)Program.GetRandom.Next((int)Y - 10, Y + 11);
            if (Map.IsValidFlagNpc(x, y) || vendor)
            {
                Equipment = new Equipment(this);
                BEntity.Fake = true;
                BEntity.Player = new Role.Player(BEntity);
                BEntity.Inventory = new Role.Instance.Inventory(BEntity);
                BEntity.Equipment = new Role.Instance.Equip(BEntity);
                BEntity.Warehouse = new Role.Instance.Warehouse(BEntity);
                BEntity.MyProfs = new Role.Instance.Proficiency(BEntity);
                BEntity.MySpells = new Role.Instance.Spell(BEntity);
                BEntity.Status = new MsgStatus();
                string NamesID = Names[Program.GetRandom.Next(0, Names.Length)];
                BEntity.Player.Name = name != "" ? name : NamesID;
                BEntity.Player.Body = gameClient != null ? gameClient.Player.Body : (ushort)Program.GetRandom.Next(1001, 1005);
                BEntity.Player.UID = UID = (uint)(uid != 0 ? uid : Database.Server.ClientCounter.Next);
                BEntity.Player.HitPoints = this.HP + 5000;
                BEntity.Status.MaxHitpoints = (uint)BEntity.Player.HitPoints;
                if (vendor)
                {
                    BEntity.Map = Server.ServerMaps[1036];
                    if (v_x == 0)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var vend = BEntity.Map.View.GetAllMapRoles(Role.MapObjectType.Npc).Where(e => (e.UID >= 174 && e.UID <= 185 || e.UID >= 198 && e.UID <= 209) && BEntity.Map.ValidLocation((ushort)(e.X + 2), e.Y)).ToArray();
                            if (vend.Length <= 0)
                            {
                                BEntity = null;
                                return false;
                            }
                            BEntity.Player.X = (ushort)(vend[0].X + 2);
                            BEntity.Player.Y = vend[0].Y;
                        }
                    }
                    else
                    {
                        BEntity.Player.X = v_x;
                        BEntity.Player.Y = v_y;
                    }
                    if (gameClient == null)
                    {
                        var obj = BEntity.Map.View.GetAllMapRoles(MapObjectType.Player).Where(p => p.X == BEntity.Player.X && p.Y == BEntity.Player.Y).FirstOrDefault();
                        if (obj != null)
                            return false;
                    }
                    BEntity.Map.cells[BEntity.Player.X, BEntity.Player.Y] = MapFlagType.Player;
                    BEntity.Player.Map = 1036;
                }
                else
                {
                    BEntity.Player.X = x;
                    BEntity.Player.Y = y;
                    BEntity.Player.Map = this.MapID;
                }
                BEntity.Player.Level = (byte)(gameClient != null ? gameClient.Player.Level : (byte)Program.GetRandom.Next(60, 120));
                BEntity.Player.Reborn = gameClient != null ? gameClient.Player.Reborn : (byte)Program.GetRandom.Next(0, 3);
                BEntity.Player.Face = (ushort)(gameClient != null ? gameClient.Player.Face : 153);
                BEntity.Player.CountryID = gameClient != null ? gameClient.Player.CountryID : (ushort)Program.GetRandom.Next(1, 50);
                BEntity.Player.Action = gameClient != null ? gameClient.Player.Action : Role.Flags.ConquerAction.Dance;
                uint ClassID = Class[Program.GetRandom.Next(0, Class.Length)];
                BEntity.Player.Angle = gameClient != null ? gameClient.Player.Angle : (Role.Flags.ConquerAngle)Program.GetRandom.Next(0, 7);
                BEntity.Player.Class = gameClient != null ? gameClient.Player.Class : (byte)ClassID;
                BEntity.Player.FirstClass = gameClient != null ? gameClient.Player.FirstClass : (byte)ClassID;
                BEntity.Player.SecondClass = gameClient != null ? gameClient.Player.SecondClass : (byte)ClassID;
                BEntity.Map = this.Map;
                BEntity.Player.Vitality = (ushort)((BEntity.Player.Level + BEntity.Player.BattlePower) * (BEntity.Player.Reborn + 1));
                DataCore.AtributeStatus.GetStatus(BEntity.Player);

                DataCore.LoadClient(BEntity.Player);
                if (gameClient != null)
                {
                    BEntity.Player.Hair = gameClient.Player.Hair;
                    BEntity.Player.HairColor = gameClient.Player.HairColor;
                    Equipment.CopyEquipmentOwner(gameClient).Send();
                }
                else
                {
                    DataCore.SetCharacterSides(BEntity.Player);
                    DataCore.CreateHairStyle(BEntity);
                    Equipment.GetRandomEquipment((byte)ClassID).Send();
                }
                BEntity.Map.Enquer(BEntity);
                if (!auto && BEntity.Player.Reborn > 1)
                    BEntity.Player.AddFlag(MsgUpdate.Flags.CastPray, Role.StatusFlagsBigVector32.PermanentFlag, true);
                BEntity.Bot = this;
                Server.GamePoll.TryAdd(BEntity.Player.UID, BEntity);
                if (vendor)
                    StartVendor();
                return true;
            }
            return false;
        }

        public void Thread()
        {
            try
            {
                if (BEntity != null)
                {
                    DateTime now = DateTime.Now;
                    if (isVendor)
                    {
                        if (BEntity.MyVendor != null && BEntity.MyVendor.Items.Count == 0 && !Exit)
                        {
                            TimeToExit = now;
                            Exit = true;
                        }
                    }
                    if (Exit)
                    {
                        if (now > TimeToExit.AddSeconds(3))
                        {
                            Leave();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.SaveException(e);
            }
        }

        public void Leave(bool force = false)
        {
            try
            {
                if (HaveOwner && OwnerUID > 0 && isVendor)
                {
                    int CPs = BEntity.Player.ConquerPoints;
                    int Money = BEntity.Player.Money;
                    Client.GameClient owner;
                    if (Server.GamePoll.TryGetValue(OwnerUID, out owner))
                    {
                        if (CPs > 0)
                            owner.Player.ConquerPoints += CPs;
                        BEntity.Player.ConquerPoints = 0;
                        if (Money > 0)
                            owner.Player.Money += Money;
                        BEntity.Player.Money = 0;
                        owner.SendSysMesage($"Your vendor bot in market is empty and you collect CPs: {CPs} and Money: {Money}, Check your invetory.");
                    }
                    else
                    {
                        WindowsAPI.IniFile ini = new WindowsAPI.IniFile("\\Users\\" + OwnerUID + ".ini");
                        int cps = (int)ini.ReadUInt32("Character", "ConquerPoints", 0);
                        long money = ini.ReadUInt32("Character", "Money", 0);
                        cps += CPs;
                        money += Money;
                        ini.Write<uint>("Character", "ConquerPoints", (uint)cps);
                        ini.Write<long>("Character", "Money", money);
                    }
                }
                Client.GameClient client;
                if (Server.GamePoll.TryRemove(UID, out client))
                {
                    DataVendor.AIPoll.TryRemove(UID, out _);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (client.MyVendor != null)
                            client.MyVendor.StopVending(stream);
                        client.Map.View.LeaveMap<Role.IMapObj>(client.Player);
                        client.Player.View.Clear(stream);
                        BEntity.Map.cells[BEntity.Player.X, BEntity.Player.Y] = MapFlagType.Valid;
                        DataVendor.Remove(UID);
                    }
                }
            }
            catch (Exception e)
            {
                Console.SaveException(e);
            }
        }

        public void StartVendor()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!BEntity.IsVendor)
                {
                    BEntity.MyVendor = new Role.Instance.Vendor(BEntity);
                    BEntity.MyVendor.CreateVendor(stream);
                    Game.MsgServer.ActionQuery data = new Game.MsgServer.ActionQuery()
                    {
                        dwParam = BEntity.MyVendor.VendorUID,
                        wParam1 = BEntity.MyVendor.VendorNpc.X,
                        wParam2 = BEntity.MyVendor.VendorNpc.Y
                    };
                    unsafe
                    {
                        BEntity.Player.View.SendView(stream.ActionCreate(&data), true);
                    }
                }
            }
        }
    }
}
