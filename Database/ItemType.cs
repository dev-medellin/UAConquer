using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using System.CodeDom;

namespace TheChosenProject.Database
{
    public class RefinaryBoxes : Dictionary<uint, RefinaryBoxes.Boxe>
    {
        public class Boxe
        {
            public UInt32 Identifier, Position;
            public Boolean Untradable;
            public Rifinery.RefineryType Type;
        }

        public RefinaryBoxes()
        {
            using (Database.DBActions.Read reader = new DBActions.Read("databaserefineryboxes.txt"))
            {
                if (reader.Reader())
                {
                    for (int x = 0; x < reader.Count; x++)
                    {
                        DBActions.ReadLine line = new DBActions.ReadLine(reader.ReadString("0,0"), ',');
                        Boxe box = new Boxe();
                        box.Identifier = line.Read((uint)0);
                        box.Position = line.Read((uint)0);
                        box.Type = (Rifinery.RefineryType)line.Read((byte)0);
                        box.Untradable = line.Read((byte)0) == 1;

                        Add(box.Identifier, box);
                    }
                }

            }

        }

        public uint GainRefineryItem(uint ID)
        {
            Boxe RefineryB = null;
            if (TryGetValue(ID, out RefineryB))
            {
                List<Rifinery.Item> Possible = new List<Rifinery.Item>();
                foreach (Rifinery.Item RefineryI in Database.Server.RifineryItems.Values)
                {
                    if (RefineryI.Type == RefineryB.Type)
                    {
                        if (RefineryI.ForItemPosition == RefineryB.Position)
                        {
                            if (RefineryB.Identifier >= 3004197 && RefineryB.Identifier <= 3004226
                                || RefineryB.Identifier >= 3004266 && RefineryB.Identifier <= 3004280)
                            {
                                if (RefineryI.Level > 3)
                                    Possible.Add(RefineryI);
                            }
                            else
                                if (RefineryI.Level < 6)
                                Possible.Add(RefineryI);
                        }
                    }
                }
                if (Possible.Count > 0)
                {
                    Random Rand = new Random();
                    Int32 x = Rand.Next(1, Possible.Count);
                    Rifinery.Item Refinery = Possible[x];

                    if (Refinery != null)
                    {
                        return Refinery.ItemID;
                    }
                }
            }
            return 0;

        }
    }
    public class Rifinery : Dictionary<uint, Rifinery.Item>
    {
        public enum RefineryType
        {
            None = 0,
            MDefence = 1,
            CriticalStrike = 2,
            SkillCriticalStrike = 3,
            Immunity = 4,
            Break = 5,
            Counteraction = 6,
            Detoxication = 7,
            Block = 8,
            Penetration = 9,
            Intensification = 10,

            FinalMDamage = 11,
            FinalMAttack = 12
        }
        public Rifinery()
        {
            string[] baseText = File.ReadAllLines(ServerKernel.CO2FOLDER + "Rifinery.txt");
            foreach (string aline in baseText)
            {
                string[] line = aline.Split(' ');
                Item ite = new Item();
                ite.ItemID = uint.Parse(line[0]);
                string ItemName = line[1];
                ite.ItemName = ItemName;
                ite.Level = CalculateLevel(line[1]);
                ite.Procent = uint.Parse(line[2]);
                if (ite.Procent == 0)
                {

                }
                ite.Type = (RefineryType)byte.Parse(line[3]);
                if (ite.Type == 0)
                {

                }

                ite.Type2 = (RefineryType)byte.Parse(line[4]);
                ite.Procent2 = uint.Parse(line[5]);

                string UseItemName = line[53];
                if (ite.ItemID >= 3006165 && ite.ItemID <= 3006170)
                {
                    UseItemName = UseItemName.Replace("(", "");
                    UseItemName = UseItemName.Split(')')[0];
                }
                else if (ite.ItemID >= 3004136)
                {
                    UseItemName = UseItemName.Replace("[", "");
                    UseItemName = UseItemName.Split(']')[0];
                }

                ite.Name = UseItemName;
                ite.ForItemPosition = ForItemPosition(UseItemName);
                //  if(ite.ItemID >= 3004136)
                //      MyConsole.WriteLine(ite.ItemID + " level " + ite.Level + " foritem= " + ite.ForItemPosition + " ite.Type= " + ite.Type + " procent" + ite.Procent + " " + UseItemName);
                if (!ContainsKey(ite.ItemID))
                    Add(ite.ItemID, ite);

                if (!ItemType.Refinary.ContainsKey(ite.Level))
                    ItemType.Refinary.Add(ite.Level, new Dictionary<uint, Item>());
                if (!ItemType.Refinary[ite.Level].ContainsKey(ite.ItemID))
                    ItemType.Refinary[ite.Level].Add(ite.ItemID, ite);

            }
            Console.WriteLine("loaded : [" + baseText.Length + "] Refinery items");
            GC.Collect();
        }

        public uint ForItemPosition(string name)
        {
            uint pos = 0;
            if (name == "Bow" || name == "2-Handed" || name == "1-Handed" || name == "Backsword" || name == "2-handed" || name == "1-handed")
                pos = (ushort)Role.Flags.ConquerItem.RightWeapon;
            if (name == "Shield" || name == "Hossu")
                pos = (ushort)Role.Flags.ConquerItem.LeftWeapon;
            if (name == "Ring" || name == "Bracelet")
                pos = (ushort)Role.Flags.ConquerItem.Ring;
            if (name == "Armor")
                pos = (ushort)Role.Flags.ConquerItem.Armor;
            if (name == "Boots")
                pos = (ushort)Role.Flags.ConquerItem.Boots;
            if (name == "Headgear")
                pos = (ushort)Role.Flags.ConquerItem.Head;
            if (name == "Necklace" || name == "Bag")
                pos = (ushort)Role.Flags.ConquerItem.Necklace;
            return pos;
        }

        public static uint CalculateLevel(string name)
        {
            byte level = 0;
            if (name.Contains("Normal")) level = 1;
            if (name.Contains("Refined")) level = 2;
            if (name.Contains("Unique")) level = 3;
            if (name.Contains("Elite")) level = 4;
            if (name.Contains("Super")) level = 5;
            if (name.Contains("Sacred")) level = 6;
            return level;
        }
        public class Item
        {
            public string ItemName = "";
            public string Name = "";
            public uint ItemID = 0;
            public uint Level = 0;
            public uint ForItemPosition = 0;
            public uint Procent = 0;
            public uint Procent2 = 0;
            public RefineryType Type = 0;
            public RefineryType Type2 = 0;

        }
    }
    public class ItemType : Dictionary<uint, ItemType.DBItem>
    {
        public class ITPlus
        {
            public uint ID;

            public byte Plus;

            public ushort ItemHP;

            public uint MinAttack;

            public uint MaxAttack;

            public ushort PhysicalDefence;

            public ushort MagicAttack;

            public ushort MagicDefence;

            public ushort Agility;

            public byte Dodge;

            public ushort Vigor => Agility;

            public ushort SpeedPlus => Dodge;

            public void Parse(string Line)
            {
                string[] Info;
                Info = Line.Split(' ');
                ID = uint.Parse(Info[0]);
                Plus = byte.Parse(Info[1]);
                ItemHP = ushort.Parse(Info[2]);
                MinAttack = uint.Parse(Info[3]);
                MaxAttack = uint.Parse(Info[4]);
                PhysicalDefence = ushort.Parse(Info[5]);
                MagicAttack = ushort.Parse(Info[6]);
                MagicDefence = ushort.Parse(Info[7]);
                Agility = ushort.Parse(Info[8]);
                Dodge = byte.Parse(Info[9]);
            }
        }

        public class DBItem
        {
            public enum ItemType : byte
            {
                Dropable,
                Others
            }

            public uint ID;

            public ITPlus[] Plus;

            public bool AllowUpgradePlus;

            public string Name;

            public byte Class;

            public byte Proficiency;

            public byte Level;

            public byte Gender;

            public ushort Strength;

            public ushort Agility;

            public int GoldWorth;

            public ushort MinAttack;

            public ushort MaxAttack;

            public ushort PhysicalDefence;

            public ushort MagicDefence;

            public ushort MagicAttack;

            public byte Dodge;

            public ushort Frequency;

            public int ConquerPointsWorth;

            public ushort Durability;

            public ushort StackSize;

            public ushort ItemHP;

            public ushort ItemMP;

            public ushort AttackRange;

            public ItemType Type;

            public string Description;

            public int GradeKey;

            public uint Crytical;

            public uint SCrytical;

            public uint Imunity;

            public uint Penetration;

            public uint Block;

            public uint BreackTrough;

            public uint ConterAction;

            public uint Detoxication;

            public uint MetalResistance;

            public uint WoodResistance;

            public uint WaterResistance;

            public uint FireResistance;

            public uint EarthResistance;

            public ushort Auction_Class;

            public ushort PurificationLevel;

            public ushort PurificationMeteorNeed;

            public void Parse(string Line)
            {
                Plus = new ITPlus[13];
                string[] data;
                data = Line.Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                if (data[2] == "")
                {
                    for (int x = 2; x < data.Length - 1; x++)
                    {
                        data[x] = data[x + 1];
                    }
                }
                try
                {
                    if (data.Length > 52 && data[0] != "\0")
                    {
                        ID = Convert.ToUInt32(data[0]);
                        Name = data[1].Trim();
                        Class = Convert.ToByte(data[2]);
                        Proficiency = Convert.ToByte(data[3]);
                        Level = Convert.ToByte(data[4]);
                        Gender = Convert.ToByte(data[5]);
                        Strength = Convert.ToUInt16(data[6]);
                        Agility = Convert.ToUInt16(data[7]);
                        Type = ((Convert.ToUInt32(data[10]) != 0) ? ItemType.Others : ItemType.Dropable);
                        GoldWorth = (int)Convert.ToUInt32(data[12]);
                        MaxAttack = Convert.ToUInt16(data[14]);
                        MinAttack = Convert.ToUInt16(data[15]);
                        PhysicalDefence = Convert.ToUInt16(data[16]);
                        Frequency = Convert.ToUInt16(data[17]);
                        Dodge = Convert.ToByte(data[18]);
                        ItemHP = Convert.ToUInt16(data[19]);
                        ItemMP = Convert.ToUInt16(data[20]);
                        Durability = Convert.ToUInt16(data[22]);
                        MagicAttack = Convert.ToUInt16(data[30]);
                        MagicDefence = Convert.ToUInt16(data[31]);
                        AttackRange = Convert.ToUInt16(data[32]);
                        ConquerPointsWorth = (int)Convert.ToUInt32(data[37]);
                        Crytical = Convert.ToUInt32(data[40]);
                        SCrytical = Convert.ToUInt32(data[41]);
                        Imunity = Convert.ToUInt32(data[42]);
                        Penetration = Convert.ToUInt32(data[43]);
                        Block = Convert.ToUInt32(data[44]);
                        BreackTrough = Convert.ToUInt32(data[45]);
                        ConterAction = Convert.ToUInt32(data[46]);
                        Detoxication = Convert.ToUInt32(data[47]);
                        MetalResistance = Convert.ToByte(data[48]);
                        WoodResistance = Convert.ToByte(data[49]);
                        WaterResistance = Convert.ToByte(data[50]);
                        FireResistance = Convert.ToByte(data[51]);
                        EarthResistance = Convert.ToByte(data[52]);
                        StackSize = Convert.ToUInt16(data[47]);
                        Description = data[53].Replace("`s", "");
                        if (Description == "NinjaKatana")
                            Description = "NinjaWeapon";
                        if (Description == "Earrings")
                            Description = "Earring";
                        if (Description == "Bow")
                            Description = "ArcherBow";
                        if (Description == "Backsword")
                            Description = "TaoistBackSword";
                        Description = Description.ToLower();
                        if (ID >= 730001 && ID <= 730009)
                            Name = "(+" + ID % 10u + ")Stone";
                        if (data.Length >= 58)
                        {
                            ushort.TryParse(data[56].ToString(), out PurificationLevel);
                            ushort.TryParse(data[57].ToString(), out PurificationMeteorNeed);
                            if (PurificationLevel != 0 && ID != 729305 && ID != 727465)
                            {
                                if (!PurificationItems.ContainsKey(PurificationLevel))
                                    PurificationItems.Add(PurificationLevel, new Dictionary<uint, DBItem>());
                                if (!PurificationItems[PurificationLevel].ContainsKey(ID))
                                    PurificationItems[PurificationLevel].Add(ID, this);
                                if (ID == 800415 && !PurificationItems[6].ContainsKey(ID))
                                    PurificationItems[6].Add(ID, this);
                            }
                        }
                        if (data.Length >= 60)
                            ushort.TryParse(data[59].ToString(), out Auction_Class);
                        if (ItemPosition(ID) == 16 || ItemPosition(ID) == 15)
                            StackSize = 1;
                        if (ItemPosition(ID) == 17 && !SteedMounts.ContainsKey(ID))
                            SteedMounts.Add(ID, this);
                        if (ID == 754099)
                            ConquerPointsWorth = 299;
                        if (ID == 754999 || ID == 753999 || ID == 751999)
                            ConquerPointsWorth = 1699;
                        if (ID == 619028)
                            ConquerPointsWorth = 489;
                        if (ID == 723723)
                            ConquerPointsWorth = 7100;
                        if (ID == 3005945)
                            ConquerPointsWorth = 3100;
                    }
                    else
                        ServerKernel.Log.GmLog("itemtype_error", $"ERROr loading item {data[0]}");
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
            }
        }

        public static Dictionary<ushort, Dictionary<uint, DBItem>> PurificationItems = new Dictionary<ushort, Dictionary<uint, DBItem>>();
        public static Dictionary<ushort, Dictionary<uint, DBItem>> Stones = new Dictionary<ushort, Dictionary<uint, DBItem>>();

        public static Dictionary<uint, Dictionary<uint, Rifinery.Item>> Refinary = new Dictionary<uint, Dictionary<uint, Rifinery.Item>>();

        public static List<uint> RareAccessories = new List<uint>();

        public static List<uint> RareGarments = new List<uint>();

        public static Dictionary<uint, DBItem> SteedMounts = new Dictionary<uint, DBItem>();
        public static List<uint> ElitePlus1AllItems = new List<uint>()
        {
                /*CopperBacksword+8*/ 421105,
                /*AmethystBlade+8*/ 410085,
                /*FangSword+8*/ 420095,
                /*UnitClub+8*/ 480085,
                /*PeaceAxe+8*/ 450095,
                /*MelonHammer+8*/ 460095,
                /*PhoenixHook+8*/ 430085,
                /*TwinWhip+8*/ 440095,
                /*WishScepter+8*/ 481095,
                /*CaoDagger+8*/ 490095,
                /*QinBow+8*/ 500085,
                /*TwinPoleaxe+8*/ 530095,
                /*ShaolinWand+8*/ 561095,
                /*LuckSpear+8*/ 560095,
                /*UnionGlaive+8*/ 510095,
                /*GreatLongHammer+8*/ 540095,
                /*GoldHalbert+8*/ 580095,
                /*WavyHeavyRing+8*/ 151155,
                /*SilverEarring+8*/ 117025,
                /*GoldRing+8*/ 150055,
                /*PureBracelet+8*/ 152045,
                /*TaoRobe+8*/ 134005,
                /*EagleBoots+8*/ 160075,
                /*JadeNecklace+8*/ 120065,
                /*JadeNecklace+8*/ 120065,
                /*SteelCoronet+8*/ 118035,
                /*ShiningHelmet+8*/ 111035,
                /*CatHat+8*/ 113015,
                /*StoneCap+8*/ 114035,
                /*SoftShield+8*/ 900005,
                /*BreastPlate+8*/ 130025,
                /*BronzeArmor+8*/ 131025,
                /*FoxCoat+8*/ 133015,
                /*RoseHeadband+8*/ 141035,
                /*EmeraldPlume+8*/ 142015,
                /*MaskBag+8*/ 121045,
        };
        #region items OnlinePoints ElitePlus1HItems
        public static List<uint> ElitePlus1HItems = new List<uint>()
        {
   
                                                                /*CopperBacksword+8*/ 421108,
                                                                /*AmethystBlade+8*/ 410088,
                                                                /*FangSword+8*/ 420098,
                                                                /*UnitClub+8*/ 480088,
                                                                /*PeaceAxe+8*/ 450098,
                                                                /*MelonHammer+8*/ 460098,
                                                                /*PhoenixHook+8*/ 430088,
                                                                /*TwinWhip+8*/ 440098,
                                                                /*WishScepter+8*/ 481098,
                                                                /*CaoDagger+8*/ 490098,
           
                            
        };

        #endregion
        #region items OnlinePoints ElitePlus2HItems
        public static List<uint> ElitePlus2HItems = new List<uint>()
        {
                
                /*QinBow+8*/ 500088,
                /*TwinPoleaxe+8*/ 530098,
                /*ShaolinWand+8*/ 561098,
                /*LuckSpear+8*/ 560098,
                /*UnionGlaive+8*/ 510098,
                /*GreatLongHammer+8*/ 540098,
                /*GoldHalbert+8*/ 580098,

        };

        #endregion
        #region items OnlinePoints ElitePlusEquipment
        public static List<uint> ElitePlusEquipment = new List<uint>()
        {
                /*WavyHeavyRing+8*/ 151158,
                /*SilverEarring+8*/ 117028,
                /*GoldRing+8*/ 150058,
                /*PureBracelet+8*/ 152048,
                /*TaoRobe+8*/ 134008,
                /*EagleBoots+8*/ 160078,
                /*JadeNecklace+8*/ 120068,
                /*JadeNecklace+8*/ 120068,
                /*SteelCoronet+8*/ 118038,
                /*ShiningHelmet+8*/ 111038,
                /*CatHat+8*/ 113018,
                /*StoneCap+8*/ 114038,
                /*SoftShield+8*/ 900008,
                /*BreastPlate+8*/ 130028,
                /*BronzeArmor+8*/ 131028,
                /*FoxCoat+8*/ 133018,
                /*RoseHeadband+8*/ 141038,
                /*EmeraldPlume+8*/ 142018,
                /*MaskBag+8*/ 121048,
                                            
                                                                

        };

        #endregion
        #region items OnlinePoints random AccessoriesOP
        public static List<uint> AccessoriesOP = new List<uint>()
        {
                360479,//SteelBlade 
                360480,//LeafBlade 
                360481,//DemonBlade 
                360482,//LightBlade 
                360483,//Falchion 
                360484,//BroadSword 
                360485,//StrangeBlade 
                360486,//Cutlass 
                360487,//AmethystBlade 
                360488,//MoonBlade 
                360489,//ColdBlade 
                360490,//Ataghan 
                360491,//DearBlade 
                360493,//DragonBlade 
                360494,//HeavyBlade 
                360495,//Tulwar 
                360496,//GodBlade 
                360497,//SharpBlade 
                360498,//RainbowBlade 
                360499,//SunBlade 
                360500,//MythicBlade 
                360501,//ConquestBlade 
                360502,//VanSword 
                360503,//SpringSword 
                360504,//DarkSword 
                360505,//BrightSword 
                360506,//CrystalSword 
                360507,//FireSword 
                360508,//BasaltSword 
                360509,//MountSword 
                360510,//PeaceSword 
                360511,//FangSword 
                360512,//MirroredSword 
                360513,//Claymore 
                360514,//WandererSword 
                360515,//SharkSword 
                360516,//LightSword 
                360517,//TwinSword 
                360518,//RainbowSword 
                360519,//LongSword 
                360520,//DragonSword 
                360521,//LoyalSword 
                360522,//FlyingSword 
                360523,//PureSword 
                360524,//DeepSword 
                360525,//OnimaKatana 
                360526,//StealthKatana 
                360527,//PhantomKatana 
                360528,//DivineKnife 
                360529,//AbyssalKnife 
                360530,//GhostKnife 
                360531,//ShortScepter 
                360532,//AnvilScepter 
                360533,//IronScepter 
                360534,//BronzeScepter 
                360535,//SteelScepter 
                360536,//GhostScepter 
                360537,//DullScepter 
                360538,//LightningRod 
                360539,//LotusScepter 
                360540,//WishScepter 
                360541,//GoldScepter 
                360542,//VioletScepter 
                360543,//MagicScepter 
                360544,//MonkScepter 
                360545,//WarScepter 
                360546,//LongDagger 
                360547,//SnakeDagger 
                360548,//FlowerDagger 
                360549,//GentleDagger 
                360550,//KnightDagger 
                360551,//StarDagger 
                360552,//DragonDagger 
                360553,//CaoDagger 
                360554,//SteelDagger 
                360555,//ArtDagger 
                360556,//HornDagger 
                360557,//HexDagger 
                360558,//SharpDagger 
                360559,//Spike 
                360560,//OddDagger 
                360561,//WeepDagger 
                360562,//BoningKnife 
                360563,//RainbowDagger 
                360564,//DevilDagger 
                360565,//GoldDagger 
                360566,//BloodDagger 

    };

        #endregion
        #region items OnlinePoints random RareGarmentsOP
        public static List<uint> RareGarmentsOP = new List<uint>()
        {
                191005,//LeatherArmor 
                191015,//HardArmor 
                191025,//BreastPlate 
                191035,//GothicPlate 
                191045,//DipperArmor 
                191055,//DemonArmor 
                191065,//RageArmor 
                191075,//SacredArmor 
                191085,//WarArmor 
                191095,//DragonArmor 
                191105,//ConquestArmor 
                191205,//OxhideArmor 
                191115,//IronArmor 
                191125,//BronzeArmor 
                191135,//SteelArmor 
                191145,//BrightArmor 
                191155,//ChainedArmor 
                191165,//LightArmor 
                191175,//LionArmor 
                191185,//BasaltArmor 
                191195,//AncientArmor 
                191215,//PhoenixArmor 
                191315,//DeerskinCoat 
                191335,//FoxCoat 
                191225,//WolfCoat 
                191235,//LeopardCoat 
                191245,//ApeCoat 
                191255,//Gambeson 
                191265,//SharkCoat 
                191275,//RhinoCoat 
                191285,//EagleCoat 
                191295,//DragonCoat 
                191325,//PhoenixJerkin 
                191415,//TaoRobe 
                191435,//MaskRobe 
                191445,//TaijiRobe 
                191345,//PureGown 
                191355,//PowerGown 
                191365,//CraneVestment 
                191375,//FullFrock 
                191385,//RoyalGown 
                191395,//ConquestGown 
                191425,//PineRobe 
                191455,//LowerNinjaVest 
                191465,//MiddleNinjaVest 
                191475,//MasterNinjaVest 
                191485,//ChainVest 
                191495,//MythVest 
                191515,//WolfVest 
                191525,//TigerVest 
                192005,//BearVest 
                191545,//MoonVest 
                191555,//CondorVest 
                191565,//OwlVest 
                191575,//BurlapFrock 
                191585,//CottonFrock 
                191595,//SpunGoldFrock 
                191615,//SilkFrock 
                191625,//RadiantFrock 
                191635,//Friar`sFrock 
                191645,//CloudFrock 
                191655,//ToughFrock 
                191665,//FrockOfAges 
                191675,//WrapOfDivineJustice 
                191685,//RobeOfTheBuddha 
                191695,//RecruitCoat 
                191715,//RippleCoat 
                191725,//WaveCoat 
                191735,//BillowCoat 
                191745,//AbyssCoat 
                191755,//HuntingCoat 
                191765,//TideCoat 
                191775,//OceanCoat 
                191785,//TunaCoat 
                191795,//SharkPirateCoat
                191815,//WhaleCoat 
                191825,//MistArmor 
                191835,//WolfArmor 
                191845,//WonderArmor 
                191855,//ObsidianArmor 
                191865,//SeekerArmor 
                191875,//BlazeArmor 
                191885,//KylinArmor 
                191895,//ImperiousArmor 
                191915,//BlizzardCoat 
                191925,//AureateCoat 
                191935,//ComfyCoat 
                191945,//WelkinCoat 
                191955,//RobeOfIllusion 
                191965,//HonorRobe 
                191975,//ChaosRobe 
                191985,//EternalRobe 
                191995,//StellarVest 
                193835,//FuryVest 
                193845,//DuskVest 
                193855,//NightmareVest 
                193865,//Asura`sFrock 
                193875,//FrockOfHeavenlyBliss 
                193885,//WhiteLotusFrock 
                193895,//BlackDragonCoat 
                193905,//ShadowDragonCoat 
                193915,//DarkDragonCoat 
                183475,//FlameRobe
                183425,//FancyAzure 
                192185,//AssassinSuit 
                188365,//PurePunk 
                183405,//NobodySuit 
                187805,//RightStarSuit 
                187455,//AncientBeauty 
                187465,//AncientGeneral 
                187505,//SwordShadow 
                193085,//DragonSuit 
                193075,//KungfuGown 
                193055,//Hero`sNature 
                193105,//ArtistCasualSuit 
                193095,//ClassicFashion 
                193065,//JingwuUniform 
                193045,//KungfuPants 
                188665,//SoberDark 
                188155,//ErrantryRobe 
                188165,//MatadorSuit 
                188175,//IvoryRobe 
                188255,//PoliceUniform 
                188295,//ClassySuit 
                188545,//IndianLegend 
                193015,//FlameDragon 
                193605,//FieryRedUniform 
                193235,//KungFuSuit 
                192555,//XmasReindeer 
                189095,//CloudRobe 
                187475,//BloodThirst 
                188355,//FamilyBusiness 
                183485,//CharmingSuit 
                188925,//EndlessDance 
                192685,//BrazilJersey 
                192675,//ArgentinaJersey 
                192665,//SpainJersey 
                192655,//GermanyJersey 
                192300,//FairyGarment 
                189105,//GoldenDream 
                188285,//DancingDress 
                187965,//DelightofSpeed 
                188225,//ChristmasGarment 
                183465,//ChristmasSuit 
                192310,//CollegeSuit 
                187605,//GracefulBeauty 
                192575,//TwinBliss 
                192565,//CogsoftheHeart 
                193635,//FlushofHearts 
                192635,//Winner~take~all
                192695,//EvilPumpkin 
                187325,//GotTalent 
                192785,//FlameDance 
                192625,//ChivalrousDream 
                188675,//DreamofYouth 
                188625,//WizardRobe 
                188495,//TenderFlame 
                193295,//SongofDespair 
                192605,//WindWalk 
                188265,//DreamGarment 
                193205,//PrideofTriumph 
                194065,//WindSuit 
                187175,//BlackAssasin 
                187185,//DarkRobe 
                187225,//SilverWarSuit 
                187235,//RedWarSuit 
                187245,//EpicWarSuit 
                187255,//BlueWarSuit 
                193325,//AspirationJacket 
                187775,//FairyTale 
                192615,//ImperialRobe 
                192425,//ColorOfWind 
                192435,//SpringShirt 
                194075,//PinkGoodLuck 
                194085,//GreenGoodLuck 
                194095,//PurpleGoodLuck 
                191305,//GoodLuck 
                191405,//DivineRobe 
                181355,//DarkWizard 
        };

        #endregion

        public static List<uint> NormalGems = new List<uint> { 700001u, 700011u, 700021u, 700031u, 700041u, 700051u, 700061u, 700071u, 700101u, 700121u };
        public static List<uint> RareGarments2 = new List<uint> { 183225,//ValentineCoat
            183275,//RobeofDarkness(Legend)
            183415,//SandArabian
            183635,//CowBoy
            183755,//WhiteScary
            183765,//MarijuanaCoat
            183775,//RedBugs
            183785,//DragonAssassin
            183795,//SoulDemon
            183805,//Infamous(Death)
            183815,//Infamous(W&R)
            183825,//RedCOP
            183915,//BlueWarrior
            187825,//EssenseOfWater(Legend)
            187835,//EssenseOfFlame(Legend)
            188204,//SantaCoat(Snow)
            188965,//HarbingersSuit(Legend)
            189010,//ZombeCoat
            189025,//RagnarokSuit(Legend)
            189785,//MonkeyKingArmor(Legend)
            191200,//NinjaCoat
            192215,//AscendedGarment(Legend)
            193245,//CupidSuit(Legend)
            193250,//MagicDance(Legend)
            193265,//NarutoVest(Legend)
            193290,//FlameGeneral(Legend)
            193315,//FrozenFantasy(Legend)
            193335,//PurplePirateCoat
            193345,//BlackPirateCoat
            193355,//GreenPirateCoat
            193365,//PolandJersey(Legend)
            193375,//EnglandJersey(Legend)
            193385,//FranceJersey(Legend)
            193395,//RomaniaJersey(Legend)
            193425,//DeathMaster
            193665,//193665
            193705,//RedSnow
            193725,//ImmortalRobe(Legend)
            193865,//JokerCoat
            193885,//LightDragon
            193895,//ShadowDragon
            193945,//KoFuMaster
            193955,//RedCoat
            193965,//SOSCoat
            193975,//GreenDragon
            193985,//BlackDragon
            193995,//DevilMayCry(Lover)
            194005,//DeadPool
            194015,//BlackSuit
            194025,//RedSuit
            194075,//FighterSuit
            194085,//PurpleDragon
            194095,//ThunderSuit
            194210,//KrakenSuit(Legend)
            194310,//SpringBlossoms(Legend)
            194320,//DivineRobe(Legend)
            194330,//SwordMaster(Legend)
            194350,//BlackCatSuit(Legend)
            194360,//WhiteCatSuit(Legend)
            194370,//HellSuit(Legend)
            194380,//HeavenSuit(Legend)
            195045,//DemonPumpkin
            195200,//LeatherArmor
            195210,//HardArmor
            195220,//BreastPlate
            195230,//GothicPlate
            195240,//DipperArmor
            195250,//DemonArmor
            195260,//RageArmor
            195270,//SacredArmor
            195280,//WarArmor
            195290,//DragonArmor
            195480,//PirateSuitGarment
            195490,//StormCloth(Legend)

            //classic garment
            196200,//OxhideArmor
            196210,//IronArmor
            196220,//BronzeArmor
            196230,//SteelArmor
            196240,//BrightArmor
            196250,//ChainedArmor
            196260,//LightArmor
            196270,//LionArmor
            196280,//BasaltArmor
            196290,//AncientArmor
            197200,//DeerskinCoat
            197210,//FoxCoat
            197220,//WolfCoat
            197230,//LeopardCoat
            197240,//ApeCoat
            197250,//Gambeson
            197260,//SharkCoat
            197270,//RhinoCoat
            197280,//EagleCoat
            197290,//DragonCoat
            198200,//TaoRobe
            198210,//MaskRobe
            198220,//TaijiRobe
            198230,//StarRobe
            198240,//PureGown
            198250,//PowerGown
            198260,//CraneVestment
            198270,//FullFrock
            198280,//RoyalGown
            198290,//ConquestGown
            199105,//RedWolfCoat
            199250,//ConquestArmor
            199260,//PhoenixArmor
            199280,//PhoenixJerkin
            199290,//PineRobe
            199300//CoatGarment
        };
        #region SuperGems
        public static List<uint> SuperGems = new List<uint>()
        {
            700013,//DragonGem
            700003,//PhoenixGem
            700023,//FuryGem
            700033,//RainbowGem
            700043,//KylinGem
            700053,//VioletGem
            700063,//MoonGem

        };
        #endregion
        public static List<uint> RefiendGems = new List<uint> { 710834, 1088000, 720027, 720145, 723727, 780010, 700062, 700052, 700042, 700032, 700022, 700012, 700002, };

        public static List<uint> unabletradeitem = new List<uint>
        {
            750000,//CloudSaint`sJar
            722700,//ExpPotion
            729549,
            711214,//LetterC
            711215,//LetterO
            711216,//LetterN
            711217,//LetterQ
            711218,//LetterU
            711219,//LetterE
            711220,//LetterR
            711301,//DaintyFruit(Tomato)
            711302,//DaintyFruit(Guava)
            711303,//DaintyFruit(Watermelon)
            711304,//DaintyFruit(Pear)
            711305,//DaintyFruit(Grape)
            720364,//BeanStalk
            720362,//FatPumpkin
            720365,//Shampoo
            710968,//Chocolate
            720157,//XmasCandy
            723249,//MiracleFlower
            //
            721876, // key box
            720300, 720301, 720302, 720303, 720304, 720305, 720306 //lava quest

        };
        public static List<uint> undropeitem = new List<uint>
        {

            721876, // key box
            720300, 720301, 720302, 720303, 720304, 720305, 720306, //lava quest
            2100245

        };
        
        public const int ExpBall_Pack = 720757;

        public const int ExpBall_Event = 722136;
        public const int Moss = 722723,
            DreamGrass = 722724,
            SoulAroma = 722725,
            EvilTooth = 722721,
             ImmortalStone = 722728,
 ImpureVigor = 722730;
        public const int ExpBall = 723700;

        public const int DragonBallScroll = 720028;
        public const int MegaDBPack = 720546;
        public const int MegaMetsPack = 720547;
        
        public const int DragonBall = 1088000;
        public const int DoubleExp = 723017;

        public const int MeteorScroll = 720027;

        public const int Meteor = 1088001;

        public const int MoonBox = 721080;

        public const int PowerExpBall = 722057;
        public const int CleanWater = 721258;
        public const int CelestialStone = 721259;
        public const int ExperiencePotion = 723017;
        public const int ExpAmrita = 720394;
        public const int LotteryTick = 710212;

        public const int LetterV = 711214;

        public const int LetterI = 711215;

        public const int LetterE = 711216;

        public const int LetterG = 711217;

        public const int LetterO = 711218;

        public const int MeteorTearPacket = 723711;

        public const int MeteorTear = 1088002;

        public const int LuckyAmulet = 723087;

        public const int Bomb = 721261;

        public const int DiligenceBook = 723340;

        public const int EnduranceBook = 723341;

        public const int MediumRefineryPack = 720549;

        public static int[] TalismanExtra = new int[13]
        {
            0, 6, 30, 70, 240, 740, 2240, 6670, 20000, 60000,
            62000, 67000, 73000
        };

        private static ushort[] purifyStabilizationPoints = new ushort[6] { 10, 30, 60, 100, 150, 200 };

        private static ushort[] refineryStabilizationPoints = new ushort[5] { 10, 30, 70, 150, 270 };

        private static ushort[] StonePoints = new ushort[9] { 1, 10, 40, 120, 360, 1080, 3240, 9720, 29160 };

        private static ushort[] ComposePoints = new ushort[13]
        {
            20, 20, 80, 240, 720, 2160, 6480, 19440, 58320, 2700,
            5500, 9000, 0
        };

        public static List<uint> IsAccessoryShield = new List<uint>
        {
            380080u, 380081u, 380082u, 380083u, 380075u, 380076u, 380077u, 380078u, 380079u, 380054u,
            380055u, 380068u, 380069u, 380070u, 380072u, 380073u, 380074u, 380001u, 380002u, 380003u,
            380004u, 380005u, 380006u, 380009u, 380045u, 380046u, 380071u
        };
        public static bool IsPickAxe(uint ID)
        {
            return ID == 562000;
        }
        public static uint[] Ores = new uint[]
        {
             1072010, 1072011, 1072012, 1072013,
             1072052, 1072049, 1072041, 1072041,
             1072016, 1072017, 1072018, 1072047,
             1072026, 1072026, 1072025, 1072014,
             1072015, 1072025, 1072019, 1072011,
        };

        public static uint[] SkipGoldMine = new uint[]
        {
            1072047,
            1072026, 1072026, 1072025, 1072014,
            1072015, 1072025, 1072019, 1072011,
        };

        public static uint[] promoteOre = new uint[]
        {
             810032, 810033, 810034,
        };

        public static uint[] Gems = new uint[]
        {
         700001, 700011, 700021, 700031,
         700041, 700051, 700061, 700061, 700011
        };
        public static uint AmountNormalGems(GameClient client, uint count = 15u)
        {
            uint Count;
            Count = 0u;
            client.Inventory.NormalGemsList.Clear();
            foreach (MsgGameItem Item in client.Inventory.ClientItems.Values)
            {
                if (Server.ItemsBase.TryGetValue(Item.ITEM_ID, out var item) && NormalGems.Contains(item.ID) && Count < 15)
                {
                    Count++;
                    client.Inventory.NormalGemsList.Add(item.ID);
                }
            }
            return Count;
        }

        public static uint AmountRefiendGems(GameClient client, uint count = 15u)
        {
            uint Count;
            Count = 0u;
            client.Inventory.RefiendGemsList.Clear();
            foreach (MsgGameItem Item in client.Inventory.ClientItems.Values)
            {
                if (Server.ItemsBase.TryGetValue(Item.ITEM_ID, out var item) && RefiendGems.Contains(item.ID) && Count < count)
                {
                    Count++;
                    client.Inventory.RefiendGemsList.Add(item.ID);
                }
            }
            return Count;
        }

        public static uint AmountItemPlus(GameClient client)
        {
            uint Count;
            Count = 0u;
            foreach (MsgGameItem Item in client.Inventory.ClientItems.Values)
            {
                if (Item != null && (Item.ITEM_ID < 730001 || Item.ITEM_ID > 730009) && (Item.Plus == 1 || Item.Plus == 2))
                    Count++;
            }
            return Count;
        }

        public static uint AmountStonePlus(GameClient client)
        {
            uint Count;
            Count = 0u;
            foreach (MsgGameItem Item in client.Inventory.ClientItems.Values)
            {
                if (Item != null && Item.ITEM_ID >= 730001 && Item.ITEM_ID <= 730009)
                    Count++;
            }
            return Count;
        }

        public static uint AmountLetters(GameClient client)
        {
            uint Count;
            Count = 0u;
            foreach (MsgGameItem Item in client.Inventory.ClientItems.Values)
            {
                if (Item != null && Item.ITEM_ID >= 711214 && Item.ITEM_ID <= 711218)
                    Count++;
            }
            return Count;
        }

        public static uint AmountPurification(GameClient client, byte level)
        {
            uint Count;
            Count = 0u;
            client.Inventory.SoulsList.Clear();
            foreach (MsgGameItem Item in client.Inventory.ClientItems.Values)
            {
                if (Server.ItemsBase.TryGetValue(Item.ITEM_ID, out var item) && item.PurificationLevel >= 3 && item.PurificationLevel <= level && Count < 15)
                {
                    Count++;
                    client.Inventory.SoulsList.Add(item.ID);
                }
            }
            return Count;
        }

        public static uint AmountMatrial(GameClient client, byte level)
        {
            uint Count;
            Count = 0u;
            client.Inventory.MatrialList.Clear();
            foreach (MsgGameItem Item in client.Inventory.ClientItems.Values)
            {
                if (Server.RifineryItems.TryGetValue(Item.ITEM_ID, out var item) && item.Level == level && Count < 15)
                {
                    Count++;
                    client.Inventory.MatrialList.Add(item.ItemID);
                }
            }
            return Count;
        }

        public static bool IsMoneyBag(uint ID)
        {
            if (ID >= 723717)
                return ID <= 723721;
            return false;
        }

        public static bool IsPistol(uint ID)
        {
            if (ID >= 612000)
                return ID <= 612439;
            return false;
        }

        public static bool IsRapier(uint ID)
        {
            if (ID >= 611000)
                return ID <= 611439;
            return false;
        }

        public static bool IsKnife(uint ID)
        {
            if (ID >= 613000)
                return ID <= 613429;
            return false;
        }

        public static bool CheckAddGemFan(Flags.Gem gem)
        {
            if (gem != Flags.Gem.NormalThunderGem && gem != Flags.Gem.RefinedThunderGem)
                return gem == Flags.Gem.SuperThunderGem;
            return true;
        }

        public static bool CheckAddGemTower(Flags.Gem gem)
        {
            if (gem != Flags.Gem.NormalGloryGem && gem != Flags.Gem.RefinedGloryGem)
                return gem == Flags.Gem.SuperGloryGem;
            return true;
        }

        public static bool CheckAddGemWing(Flags.Gem gem, byte slot)
        {
            switch (slot)
            {
                case 1:
                    return CheckAddGemFan(gem);
                case 2:
                    return CheckAddGemTower(gem);
                default:
                    return false;
            }
        }

        public static uint GetGemID(Flags.Gem Gem)
        {
            return 700000u + (uint)Gem;
        }

        public static ushort PurifyStabilizationPoints(byte plevel)
        {
            return purifyStabilizationPoints[Math.Min(plevel - 1, 5)];
        }

        public static ushort RefineryStabilizationPoints(byte elevel)
        {
            return refineryStabilizationPoints[Math.Min(elevel - 1, 4)];
        }

        public string GetItemName(uint ID)
        {
            if (Server.ItemsBase.TryGetValue(ID, out var item))
                return item.Name;
            return "";
        }

        public static uint ComposePlusPoints(byte plus)
        {
            return ComposePoints[Math.Min(plus, (byte)12)];
        }

        public static uint StonePlusPoints(byte plus)
        {
            return StonePoints[Math.Min((int)plus, 8)];
        }

        public static Flags.SoulTyp GetSoulPosition(uint ID)
        {
            if (ID >= 820001 && ID <= 820076)
                return Flags.SoulTyp.Headgear;
            if (ID >= 821002 && ID <= 821034)
                return Flags.SoulTyp.Necklace;
            if (ID >= 824002 && ID <= 824020)
                return Flags.SoulTyp.Boots;
            if (ID >= 823000 && ID <= 823062)
                return Flags.SoulTyp.Ring;
            if ((ID >= 800000 && ID <= 800142) || (ID >= 800701 && ID <= 800917) || (ID >= 801000 && ID <= 801104) || (ID >= 801200 && ID <= 801308))
                return Flags.SoulTyp.OneHandWeapon;
            if (ID < 800200 || ID > 800618)
            {
                switch (ID)
                {
                    case 801103u:
                        break;
                    case 822001u:
                    case 822002u:
                    case 822003u:
                    case 822004u:
                    case 822005u:
                    case 822006u:
                    case 822007u:
                    case 822008u:
                    case 822009u:
                    case 822010u:
                    case 822011u:
                    case 822012u:
                    case 822013u:
                    case 822014u:
                    case 822015u:
                    case 822016u:
                    case 822017u:
                    case 822018u:
                    case 822019u:
                    case 822020u:
                    case 822021u:
                    case 822022u:
                    case 822023u:
                    case 822024u:
                    case 822025u:
                    case 822026u:
                    case 822027u:
                    case 822028u:
                    case 822029u:
                    case 822030u:
                    case 822031u:
                    case 822032u:
                    case 822033u:
                    case 822034u:
                    case 822035u:
                    case 822036u:
                    case 822037u:
                    case 822038u:
                    case 822039u:
                    case 822040u:
                    case 822041u:
                    case 822042u:
                    case 822043u:
                    case 822044u:
                    case 822045u:
                    case 822046u:
                    case 822047u:
                    case 822048u:
                    case 822049u:
                    case 822050u:
                    case 822051u:
                    case 822052u:
                    case 822053u:
                    case 822054u:
                    case 822055u:
                    case 822056u:
                    case 822057u:
                    case 822058u:
                    case 822059u:
                    case 822060u:
                    case 822061u:
                    case 822062u:
                    case 822063u:
                    case 822064u:
                    case 822065u:
                    case 822066u:
                    case 822067u:
                    case 822068u:
                    case 822069u:
                    case 822070u:
                    case 822071u:
                    case 822072u:
                        return Flags.SoulTyp.Armor;
                    default:
                        return Flags.SoulTyp.None;
                }
            }
            return Flags.SoulTyp.TwoHandWeapon;
        }

        public static bool CompareSoul(uint ITEMID, uint SoulID)
        {
            Flags.SoulTyp soul;
            soul = GetSoulPosition(SoulID);
            Flags.SoulTyp positionit;
            positionit = GetItemSoulTYPE(ITEMID);
            if (positionit == soul)
                return true;
            return false;
        }

        public static Flags.SoulTyp GetItemSoulTYPE(uint itemid)
        {
            uint iType;
            iType = itemid / 1000u;
            if (iType < 111 || iType > 118)
            {
                switch (iType)
                {
                    default:
                        if (itemid < 170000 || itemid > 170309)
                        {
                            if (iType >= 120 && iType <= 121)
                                return Flags.SoulTyp.Necklace;
                            if ((iType >= 130 && iType <= 139) || (itemid >= 101000 && itemid <= 101309))
                                return Flags.SoulTyp.Armor;
                            if (iType >= 150 && iType <= 152)
                                return Flags.SoulTyp.Ring;
                            if (iType == 160)
                                return Flags.SoulTyp.Boots;
                            if (IsTwoHand(itemid) || (itemid >= 421003 && itemid <= 421439))
                                return Flags.SoulTyp.TwoHandWeapon;
                            if ((iType >= 410 && iType <= 490) || (iType >= 500 && iType <= 580) || (iType >= 601 && iType <= 613) || iType == 616 || iType == 614 || iType == 617 || iType == 622 || iType == 624 || iType == 619)
                                return Flags.SoulTyp.OneHandWeapon;
                            return Flags.SoulTyp.TwoHandWeapon;
                        }
                        break;
                    case 123u:
                    case 141u:
                    case 142u:
                    case 143u:
                    case 144u:
                    case 145u:
                    case 146u:
                    case 147u:
                    case 148u:
                        break;
                }
            }
            return Flags.SoulTyp.Headgear;
        }

        public static uint MoneyItemID(uint value)
        {
            if (value < 100)
                return 1090000u;
            if (value < 399)
                return 1090010u;
            if (value < 5099)
                return 1090020u;
            if (value < 8099)
                return 1091000u;
            if (value < 12099)
                return 1091010u;
            return 1091020u;
        }

        public static ulong CalculateExpBall(byte Level)
        {
            if (Level < 130)
                return (ulong)(680462.7536 + 3479.5308 * (double)((int)Level / 2) * (double)(int)Level);
            if (Level < 135)
                return (ulong)((680462.7536 + 3479.5308 * (double)((int)Level / 2) * (double)(int)Level) * (double)((int)Level % 10 + 6));
            return (ulong)((680462.7536 + 3479.5308 * (double)((int)Level / 2) * (double)(int)Level) * (double)((int)Level % 10 + 8));
        }

        public void Loading()
        {
            ServerKernel.Log.SaveLog("Loading item information...", true, LogType.MESSAGE);
            Dictionary<uint, ITPlus[]> itemsplus;
            itemsplus = new Dictionary<uint, ITPlus[]>();
            string[] baseplusText;
            baseplusText = File.ReadAllLines(ServerKernel.CO2FOLDER + "ItemAdd.ini");
            string[] array;
            array = baseplusText;
            foreach (string line in array)
            {
                string _item_;
                _item_ = line.Trim();
                ITPlus pls;
                pls = new ITPlus();
                pls.Parse(_item_);
                if (itemsplus.ContainsKey(pls.ID))
                {
                    itemsplus[pls.ID][pls.Plus] = pls;
                    continue;
                }
                ITPlus[] a_pls;
                a_pls = new ITPlus[13];
                a_pls[pls.Plus] = pls;
                itemsplus.Add(pls.ID, a_pls);
            }
            string[] baseText;
            baseText = File.ReadAllLines(ServerKernel.CO2FOLDER + "itemtype.txt");
            string[] array2;
            array2 = baseText;
            foreach (string line2 in array2)
            {
                string _item_2;
                _item_2 = line2.Trim();
                if (_item_2.Length <= 11 || _item_2.IndexOf("//", 0, 2) == 0)
                    continue;
                DBItem item;
                item = new DBItem();
                item.Parse(line2);
                if (itemsplus.ContainsKey(GetBaseID(item.ID)) || itemsplus.ContainsKey(GetBaseID(item.ID) + 10) || itemsplus.ContainsKey(GetBaseID(item.ID) + 20))
                {
                    item.AllowUpgradePlus = true;
                    if (!itemsplus.TryGetValue(GetBaseID(item.ID), out item.Plus) && !itemsplus.TryGetValue(GetBaseID(item.ID) + 10, out item.Plus) && !itemsplus.TryGetValue(GetBaseID(item.ID) + 20, out item.Plus))
                    {
                        int pos;
                        pos = ItemPosition(item.ID);
                        if (pos < 6)
                            ServerKernel.Log.SaveLog($"ERROr loading item [{item.ID}][{item.Name}]", false, LogType.WARNING);
                    }
                }
                if (!ContainsKey(item.ID))
                    Add(item.ID, item);
            }
            itemsplus = null;
            GC.Collect();
        }

        public uint DowngradeItem(uint ID)
        {
            try
            {
                ushort Tryng;
                Tryng = 0;
                ushort firstposition;
                firstposition = ItemPosition(ID);
                uint rebornid;
                rebornid = ID;
                while (Tryng <= 1000)
                {
                    Tryng = (ushort)(Tryng + 1);
                    if ((ID >= 900000 && ID <= 900309 && base[rebornid].Level <= 40) || base[rebornid].Level <= ItemMinLevel((Flags.ConquerItem)ItemPosition(ID)))
                        break;
                    if (ContainsKey(rebornid - 10))
                        rebornid -= 10;
                    else if (ContainsKey(rebornid - 20))
                    {
                        rebornid -= 20;
                    }
                    else if (ContainsKey(rebornid - 30))
                    {
                        rebornid -= 30;
                    }
                    else if (ContainsKey(rebornid - 40))
                    {
                        rebornid -= 40;
                    }
                }
                if (firstposition == ItemPosition(rebornid) && ContainsKey(rebornid))
                    return rebornid;
                return ID;
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                return ID;
            }
        }

        public uint UpdateItem(uint id, out bool succesed)
        {
            ushort firstposition;
            firstposition = ItemPosition(id);
            uint nextId;
            nextId = 0u;
            if (base[id].Level < ItemMaxLevel((Flags.ConquerItem)ItemPosition(id)))
            {
                if (ContainsKey(id + 10))
                    nextId = id + 10;
                else if (ContainsKey(id + 20))
                {
                    nextId = id + 20;
                }
                else if (ContainsKey(id + 30))
                {
                    nextId = id + 30;
                }
                else if (ContainsKey(id + 40))
                {
                    nextId = id + 40;
                }
            }
            if (firstposition == ItemPosition(nextId) && ContainsKey(nextId))
            {
                succesed = true;
                return nextId;
            }
            succesed = false;
            return id;
        }

        public uint QuickUpdateItem(uint ItemID, ushort Level = 120)
        {
            uint UID;
            UID = 0;
            if (Server.ItemsBase.TryGetValue(ItemID, out var BaseInformation))
            {
                DBItem[] items;
                items = (from x in base.Values
                         where x.ID / 1000 == BaseInformation.ID / 1000 && x.ID % 10 == BaseInformation.ID % 10
                         orderby x.Level == Level
                         select x).ToArray();
                DBItem item;
                item = items.LastOrDefault();
                if (item == null)
                {
                    UID = BaseInformation.ID;
                    return BaseInformation.ID;
                }
                UID = item.ID;
                return item.ID;
            }
            return UID;
        }

        public uint GetReqLevel(uint itemtype, ushort Level)
        {
            return (from x in base.Values
                    where x.ID / 1000 == itemtype
                    orderby x.Level == Level descending
                    select x).FirstOrDefault()?.ID ?? itemtype;
        }

        public static byte ItemMinLevel(Flags.ConquerItem postion)
        {
            switch (postion)
            {
                case Flags.ConquerItem.Inventory:
                    return 0;
                case Flags.ConquerItem.Head:
                    return 15;
                case Flags.ConquerItem.Necklace:
                    return 7;
                case Flags.ConquerItem.Armor:
                    return 15;
                case Flags.ConquerItem.LeftWeapon:
                    return 15;
                case Flags.ConquerItem.RightWeapon:
                    return 15;
                case Flags.ConquerItem.Boots:
                    return 10;
                case Flags.ConquerItem.Ring:
                    return 10;
                //case Flags.ConquerItem.Tower:
                //    return 0;
                //case Flags.ConquerItem.Fan:
                //    return 0;
                //case Flags.ConquerItem.Steed:
                //    return 0;
                case Flags.ConquerItem.Garment:
                    return 0;
                //case Flags.ConquerItem.RidingCrop:
                //    return 0;
                default:
                    return 0;
            }
        }

        public static byte GetSashCounts(uint ID)
        {
            switch (ID)
            {
                case 1100009u:
                    return 12;
                case 1100006u:
                    return 6;
                case 1100003u:
                    return 3;
                default:
                    return 0;
            }
        }

        public static bool IsSash(uint ID)
        {
            if (ID == 1100009 || ID == 1100006 || ID == 1100003)
                return true;
            return false;
        }

        public static bool IsShield(uint ID)
        {
            if (ID >= 900000)
                return ID <= 900309;
            return false;
        }

        public static bool IsBackSword(uint ID)
        {
            if (ID >= 421003)
                return ID <= 421339;
            return false;
        }

        public static bool IsArmor(uint ID)
        {
            if (ID < 130003 || ID > 139309)
            {
                if (ID >= 101000)
                    return ID <= 101309;
                return false;
            }
            return true;
        }

        public static bool IsHelmet(uint ID)
        {
            if ((ID < 111003 || ID > 111109))
            {               
                return false;
            }
            return true;
        }
        public static bool IsHeadband(uint ID)
        {
            if ((ID < 141003 || ID > 141109))
            {               
                return false;
            }
            return true;
        }
        public static bool IsCoronet(uint ID)
        {
            if ((ID < 118003 || ID > 118109))
            {               
                return false;
            }
            return true;
        }
        
        public static bool IsEarring(uint ID)
        {
            if ((ID < 117003 || ID > 117109))
            {               
                return false;
            }
            return true;
        }
        public static bool IsTaoCap(uint ID)
        {
            if ((ID < 114003 || ID > 114109))
            {               
                return false;
            }
            return true;
        }
        public static bool IsArcherHat(uint ID)
        {
            if ((ID < 113003 || ID > 113109))
            {               
                return false;
            }
            return true;
        }
        public static bool IsBag(uint ID)
        {
            if ((ID < 121003 || ID > 121249))
            {               
                return false;
            }
            return true;
        }
        public static bool IsTroArmor(uint ID)
        {
            if ((ID < 130003 || ID > 130109))
            {               
                return false;
            }
            return true;
        }
        public static bool IsWarArmor(uint ID)
        {
            if ((ID < 130003 || ID > 130109))
            {               
                return false;
            }
            return true;
        }
        public static bool IsNeck(uint ID)
        {
            if ((ID < 120001 || ID > 120249))
            {               
                return false;
            }
            return true;
        }


        public static bool IsPrayedBead(uint ID)
        {
            if (ID >= 610000)
                return ID <= 610439;
            return false;
        }

        public static bool IsTalisman(uint ID)
        {
            if (ItemPosition(ID) != 11)
                return ItemPosition(ID) == 10;
            return true;
        }

        public static bool AllowToUpdate(Flags.ConquerItem Position)
        {
            if (/*Position == Flags.ConquerItem.RidingCrop || Position == Flags.ConquerItem.Fan || Position == Flags.ConquerItem.Tower ||*/ Position == Flags.ConquerItem.Garment || Position == Flags.ConquerItem.AleternanteGarment || Position == Flags.ConquerItem.AleternanteBottle || Position == Flags.ConquerItem.Bottle || Position == Flags.ConquerItem.LeftWeaponAccessory || Position == Flags.ConquerItem.RightWeaponAccessory /*|| Position == Flags.ConquerItem.SteedMount*//* || Position == Flags.ConquerItem.Steed*/)
                return false;
            return true;
        }

        public static ushort ItemPosition(uint ID)
        {
            uint iType;
            iType = ID / 1000u;
            switch (iType)
            {
                case 622u:
                case 624u:
                case 626u:
                    return 4;
                case 620u:
                    return 4;
                case 619u:
                    return 5;
                case 617u:
                    return 4;
                case 148u:
                    return 1;
                case 614u:
                    return 4;
                case 615u:
                case 616u:
                    return 4;
                default:
                    switch (iType)
                    {
                        default:
                            switch (iType)
                            {
                                case 145u:
                                case 170u:
                                    break;
                                case 120u://neck
                                case 121u://Bag
                                    return 2;
                                default:
                                    if (iType < 130 || iType > 139)
                                    {
                                        switch (iType)
                                        {
                                            case 101u:
                                                break;
                                            case 150u://ring
                                            case 151u://HeavyRing
                                            case 152u://Bracelet
                                                return 6;
                                            default:
                                                switch (iType)
                                                {
                                                    case 160u://Boots
                                                        return 8;
                                                    case 181u://garment
                                                    case 182u:
                                                    case 183u:
                                                    case 184u:
                                                    case 185u:
                                                    case 186u:
                                                    case 187u:
                                                    case 188u:
                                                    case 189u:
                                                    case 190u:
                                                    case 191u:
                                                    case 192u:
                                                    case 193u:
                                                    case 194u:
                                                    case 195u:
                                                    case 197u:
                                                    case 196u:
                                                    case 198u:
                                                    case 199u:
                                                        return 9;//garmet
                                                    default:
                                                        switch (iType)
                                                        {
                                                            case 201u:
                                                                return 10;
                                                            case 202u:
                                                                return 11;
                                                            case 203u:
                                                                return 18;
                                                            case 200u:
                                                                return 17;
                                                            case 300u:
                                                                return 12;
                                                            case 2100u://MiraculousGourd
                                                                return 7;
                                                            case 900u:
                                                            case 1050u:
                                                                return 5;
                                                            default:
                                                                if ((iType < 500 || iType > 580) && (iType < 601 || iType > 613))
                                                                    break;
                                                                goto case 410u;
                                                            case 410u:
                                                            case 411u:
                                                            case 412u:
                                                            case 413u:
                                                            case 414u:
                                                            case 415u:
                                                            case 416u:
                                                            case 417u:
                                                            case 418u:
                                                            case 419u:
                                                            case 420u:
                                                            case 421u:
                                                            case 422u:
                                                            case 423u:
                                                            case 424u:
                                                            case 425u:
                                                            case 426u:
                                                            case 427u:
                                                            case 428u:
                                                            case 429u:
                                                            case 430u:
                                                            case 431u:
                                                            case 432u:
                                                            case 433u:
                                                            case 434u:
                                                            case 435u:
                                                            case 436u:
                                                            case 437u:
                                                            case 438u:
                                                            case 439u:
                                                            case 440u:
                                                            case 441u:
                                                            case 442u:
                                                            case 443u:
                                                            case 444u:
                                                            case 445u:
                                                            case 446u:
                                                            case 447u:
                                                            case 448u:
                                                            case 449u:
                                                            case 450u:
                                                            case 451u:
                                                            case 452u:
                                                            case 453u:
                                                            case 454u:
                                                            case 455u:
                                                            case 456u:
                                                            case 457u:
                                                            case 458u:
                                                            case 459u:
                                                            case 460u:
                                                            case 461u:
                                                            case 462u:
                                                            case 463u:
                                                            case 464u:
                                                            case 465u:
                                                            case 466u:
                                                            case 467u:
                                                            case 468u:
                                                            case 469u:
                                                            case 470u:
                                                            case 471u:
                                                            case 472u:
                                                            case 473u:
                                                            case 474u:
                                                            case 475u:
                                                            case 476u:
                                                            case 477u:
                                                            case 478u:
                                                            case 479u:
                                                            case 480u:
                                                            case 481u:
                                                            case 482u:
                                                            case 483u:
                                                            case 484u:
                                                            case 485u:
                                                            case 486u:
                                                            case 487u:
                                                            case 488u:
                                                            case 489u:
                                                            case 490u:
                                                                return 4;
                                                        }
                                                        if (iType >= 350 && iType <= 370)
                                                            return 15;
                                                        if (iType == 380)
                                                            return 16;
                                                        return 0;
                                                }
                                        }
                                    }
                                    return 3;
                            }
                            break;
                        case 123u:
                        case 141u:
                        case 142u:
                        case 143u:
                        case 144u:
                            break;
                    }
                    break;
                case 111u:
                case 112u:
                case 113u:
                case 114u:
                case 115u:
                case 116u:
                case 117u:
                case 118u:
                    break;
            }
            return 1;
        }

        public static bool IsArrow(uint ID)
        {
            if (ID >= 1050000 && ID <= 1051000)
                return true;
            return false;
        }

        public static bool IsTwoHand(uint ID)
        {
            if (ID.ToString()[0] != '5')
            {
                if (ID >= 510003)
                    return ID <= 580339;
                return false;
            }
            return true;
        }
        public static bool IsOneHand(uint ID)
        {
            if (ID.ToString()[0] != '4')
            {
                if (ID >= 410003)
                    return ID <= 490339;
                return false;
            }
            return true;
        }
        public static bool IsBow(uint ID)
        {
            return ID >= 500003 && ID <= 500429;
        }
        public static bool IsClub(uint ID)
        {
            if (ID >= 480003)
                return ID <= 480439;
            return false;
        }

        public static bool IsSpear(uint ID)
        {
            if (ID >= 560003)
                return ID <= 560439;
            return false;
        }

        public static bool IsWand(uint ID)
        {
            if (ID >= 561003)
                return ID <= 561439;
            return false;
        }

        public static bool IsPoleaxe(uint ID)
        {
            if (ID >= 530003)
                return ID <= 530439;
            return false;
        }

        public static bool IsHalbert(uint ID)
        {
            if (ID >= 580003)
                return ID <= 580439;
            return false;
        }

        public static bool IsGlaive(uint ID)
        {
            if (ID >= 510003)
                return ID <= 510439;
            return false;
        }

        public static bool IsSword(uint ID)
        {
            if (ID >= 420003)
                return ID <= 420439;
            return false;
        }

        public static bool IsBlade(uint ID)
        {
            if (ID >= 410003)
                return ID <= 410439;
            return false;
        }

        public static bool IsBraclet(uint ID)
        {
            if (ID >= 152013)
                return ID <= 152279;
            return false;
        }

        public static bool IsRing(uint ID)
        {
            if (ID >= 150013)
                return ID <= 150269;
            return false;
        }

        public static bool IsHeavyRing(uint ID)
        {
            if (ID >= 151013)
                return ID <= 151269;
            return false;
        }

        public static bool IsAccessory(uint ID)
        {
            if (ID >= 350001)
                return ID <= 380015;
            return false;
        }

        public static byte ItemMaxLevel(Flags.ConquerItem Position)
        {
            switch (Position)
            {
                case Flags.ConquerItem.Inventory:
                    return 0;
                case Flags.ConquerItem.Head:
                    return 140;
                case Flags.ConquerItem.Necklace:
                    return 139;
                case Flags.ConquerItem.Armor:
                    return 140;
                case Flags.ConquerItem.LeftWeapon:
                    return 140;
                case Flags.ConquerItem.RightWeapon:
                    return 140;
                case Flags.ConquerItem.Boots:
                    return 129;
                case Flags.ConquerItem.Ring:
                    return 136;
                //case Flags.ConquerItem.Tower:
                //    return 100;
                //case Flags.ConquerItem.Fan:
                //    return 100;
                //case Flags.ConquerItem.Steed:
                //    return 0;
                //case Flags.ConquerItem.SteedMount:
                //    return 30;
                default:
                    return 0;
            }
        }
       

        public static bool IsKatana(uint ID)
        {
            if (ID >= 601000)
                return ID <= 601439;
            return false;
        }

        public static bool IsMonkWeapon(uint ID)
        {
            if (ID >= 610000)
                return ID <= 610439;
            return false;
        }

        public static bool IsWeapon(uint ID)
        {
            if (ID >= 601000)
                return ID <= 601439;
            return false;
        }

        public static bool EquipPassStatsReq(DBItem baseInformation, GameClient client)
        {
            if (client.Player.Strength >= baseInformation.Strength && client.Player.Agility >= baseInformation.Agility)
                return true;
            return false;
        }

        public static bool EquipPassJobReq(DBItem baseInformation, Client.GameClient client)
        {
            switch (baseInformation.Class)
            {
                #region WindWalker
                case 160: if (client.Player.Class <= 165 && client.Player.Class >= 160) return true; break;
                case 161: if (client.Player.Class <= 165 && client.Player.Class >= 161) return true; break;
                case 162: if (client.Player.Class <= 165 && client.Player.Class >= 162) return true; break;
                case 163: if (client.Player.Class <= 165 && client.Player.Class >= 163) return true; break;
                case 164: if (client.Player.Class <= 165 && client.Player.Class >= 164) return true; break;
                case 165: if (client.Player.Class == 165) return true; break;
                #endregion
                #region Trojan
                case 10: if (client.Player.Class <= 15 && client.Player.Class >= 10) return true; break;
                case 11: if (client.Player.Class <= 15 && client.Player.Class >= 11) return true; break;
                case 12: if (client.Player.Class <= 15 && client.Player.Class >= 12) return true; break;
                case 13: if (client.Player.Class <= 15 && client.Player.Class >= 13) return true; break;
                case 14: if (client.Player.Class <= 15 && client.Player.Class >= 14) return true; break;
                case 15: if (client.Player.Class == 15) return true; break;
                #endregion
                #region Warrior
                case 20: if (client.Player.Class <= 25 && client.Player.Class >= 20) return true; break;
                case 21: if (client.Player.Class <= 25 && client.Player.Class >= 21) return true; break;
                case 22: if (client.Player.Class <= 25 && client.Player.Class >= 22) return true; break;
                case 23: if (client.Player.Class <= 25 && client.Player.Class >= 23) return true; break;
                case 24: if (client.Player.Class <= 25 && client.Player.Class >= 24) return true; break;
                case 25: if (client.Player.Class == 25) return true; break;
                #endregion
                #region Archer
                case 40: if (client.Player.Class <= 45 && client.Player.Class >= 40) return true; break;
                case 41: if (client.Player.Class <= 45 && client.Player.Class >= 41) return true; break;
                case 42: if (client.Player.Class <= 45 && client.Player.Class >= 42) return true; break;
                case 43: if (client.Player.Class <= 45 && client.Player.Class >= 43) return true; break;
                case 44: if (client.Player.Class <= 45 && client.Player.Class >= 44) return true; break;
                case 45: if (client.Player.Class == 45) return true; break;
                #endregion
                #region Ninja
                case 50: if (client.Player.Class <= 55 && client.Player.Class >= 50) return true; break;
                case 51: if (client.Player.Class <= 55 && client.Player.Class >= 51) return true; break;
                case 52: if (client.Player.Class <= 55 && client.Player.Class >= 52) return true; break;
                case 53: if (client.Player.Class <= 55 && client.Player.Class >= 53) return true; break;
                case 54: if (client.Player.Class <= 55 && client.Player.Class >= 54) return true; break;
                case 55: if (client.Player.Class == 55) return true; break;
                #endregion
                #region Monk
                case 60: if (client.Player.Class <= 65 && client.Player.Class >= 60) return true; break;
                case 61: if (client.Player.Class <= 65 && client.Player.Class >= 61) return true; break;
                case 62: if (client.Player.Class <= 65 && client.Player.Class >= 62) return true; break;
                case 63: if (client.Player.Class <= 65 && client.Player.Class >= 63) return true; break;
                case 64: if (client.Player.Class <= 65 && client.Player.Class >= 64) return true; break;
                case 65: if (client.Player.Class == 65) return true; break;
                #endregion
                #region Taoist
                case 190: if (client.Player.Class >= 100) return true; break;
                #endregion
                #region Pirate
                case 70: if (client.Player.Class <= 75 && client.Player.Class >= 70) return true; break;
                case 71: if (client.Player.Class <= 75 && client.Player.Class >= 71) return true; break;
                case 72: if (client.Player.Class <= 75 && client.Player.Class >= 72) return true; break;
                case 73: if (client.Player.Class <= 75 && client.Player.Class >= 73) return true; break;
                case 74: if (client.Player.Class <= 75 && client.Player.Class >= 74) return true; break;
                case 75: if (client.Player.Class == 75) return true; break;
                #endregion
                #region LongLee
                case 80: if (client.Player.Class <= 85 && client.Player.Class >= 80) return true; break;
                case 81: if (client.Player.Class <= 85 && client.Player.Class >= 81) return true; break;
                case 82: if (client.Player.Class <= 85 && client.Player.Class >= 82) return true; break;
                case 83: if (client.Player.Class <= 85 && client.Player.Class >= 83) return true; break;
                case 84: if (client.Player.Class <= 85 && client.Player.Class >= 84) return true; break;
                case 85: if (client.Player.Class == 85) return true; break;
                #endregion
                case 0: return true;
                default: return false;
            }
            return false;
        }
        public static bool EquipPassJobReq(DBItem baseInformation, byte cls)
        {
            switch (baseInformation.Class)
            {
                #region WindWalker
                case 160: if (cls <= 165 && cls >= 160) return true; break;
                case 161: if (cls <= 165 && cls >= 161) return true; break;
                case 162: if (cls <= 165 && cls >= 162) return true; break;
                case 163: if (cls <= 165 && cls >= 163) return true; break;
                case 164: if (cls <= 165 && cls >= 164) return true; break;
                case 165: if (cls == 165) return true; break;
                #endregion
                #region Trojan
                case 10: if (cls <= 15 && cls >= 10) return true; break;
                case 11: if (cls <= 15 && cls >= 11) return true; break;
                case 12: if (cls <= 15 && cls >= 12) return true; break;
                case 13: if (cls <= 15 && cls >= 13) return true; break;
                case 14: if (cls <= 15 && cls >= 14) return true; break;
                case 15: if (cls == 15) return true; break;
                #endregion
                #region Warrior
                case 20: if (cls <= 25 && cls >= 20) return true; break;
                case 21: if (cls <= 25 && cls >= 21) return true; break;
                case 22: if (cls <= 25 && cls >= 22) return true; break;
                case 23: if (cls <= 25 && cls >= 23) return true; break;
                case 24: if (cls <= 25 && cls >= 24) return true; break;
                case 25: if (cls == 25) return true; break;
                #endregion
                #region Archer
                case 40: if (cls <= 45 && cls >= 40) return true; break;
                case 41: if (cls <= 45 && cls >= 41) return true; break;
                case 42: if (cls <= 45 && cls >= 42) return true; break;
                case 43: if (cls <= 45 && cls >= 43) return true; break;
                case 44: if (cls <= 45 && cls >= 44) return true; break;
                case 45: if (cls == 45) return true; break;
                #endregion
                #region Ninja
                case 50: if (cls <= 55 && cls >= 50) return true; break;
                case 51: if (cls <= 55 && cls >= 51) return true; break;
                case 52: if (cls <= 55 && cls >= 52) return true; break;
                case 53: if (cls <= 55 && cls >= 53) return true; break;
                case 54: if (cls <= 55 && cls >= 54) return true; break;
                case 55: if (cls == 55) return true; break;
                #endregion
                #region Monk
                case 60: if (cls <= 65 && cls >= 60) return true; break;
                case 61: if (cls <= 65 && cls >= 61) return true; break;
                case 62: if (cls <= 65 && cls >= 62) return true; break;
                case 63: if (cls <= 65 && cls >= 63) return true; break;
                case 64: if (cls <= 65 && cls >= 64) return true; break;
                case 65: if (cls == 65) return true; break;
                #endregion
                #region Taoist
                case 190: if (cls >= 100) return true; break;
                #endregion
                #region Pirate
                case 70: if (cls <= 75 && cls >= 70) return true; break;
                case 71: if (cls <= 75 && cls >= 71) return true; break;
                case 72: if (cls <= 75 && cls >= 72) return true; break;
                case 73: if (cls <= 75 && cls >= 73) return true; break;
                case 74: if (cls <= 75 && cls >= 74) return true; break;
                case 75: if (cls == 75) return true; break;
                #endregion
                #region LongLee
                case 80: if (cls <= 85 && cls >= 80) return true; break;
                case 81: if (cls <= 85 && cls >= 81) return true; break;
                case 82: if (cls <= 85 && cls >= 82) return true; break;
                case 83: if (cls <= 85 && cls >= 83) return true; break;
                case 84: if (cls <= 85 && cls >= 84) return true; break;
                case 85: if (cls == 85) return true; break;
                #endregion
                case 0: return true;
                default: return false;
            }
            return false;
        }
        public static bool UpdatePlusPassJobReq(DBItem baseInformation, DBItem olditem)
        {
            switch (baseInformation.Class)
            {
                case 10:
                    if (olditem.Class <= 15 && olditem.Class >= 10)
                        return true;
                    break;
                case 11:
                    if (olditem.Class <= 15 && olditem.Class >= 11)
                        return true;
                    break;
                case 12:
                    if (olditem.Class <= 15 && olditem.Class >= 12)
                        return true;
                    break;
                case 13:
                    if (olditem.Class <= 15 && olditem.Class >= 13)
                        return true;
                    break;
                case 14:
                    if (olditem.Class <= 15 && olditem.Class >= 14)
                        return true;
                    break;
                case 15:
                    if (olditem.Class == 15)
                        return true;
                    break;
                case 20:
                    if (olditem.Class <= 25 && olditem.Class >= 20)
                        return true;
                    break;
                case 21:
                    if (olditem.Class <= 25 && olditem.Class >= 21)
                        return true;
                    break;
                case 22:
                    if (olditem.Class <= 25 && olditem.Class >= 22)
                        return true;
                    break;
                case 23:
                    if (olditem.Class <= 25 && olditem.Class >= 23)
                        return true;
                    break;
                case 24:
                    if (olditem.Class <= 25 && olditem.Class >= 24)
                        return true;
                    break;
                case 25:
                    if (olditem.Class == 25)
                        return true;
                    break;
                case 40:
                    if (olditem.Class <= 45 && olditem.Class >= 40)
                        return true;
                    break;
                case 41:
                    if (olditem.Class <= 45 && olditem.Class >= 41)
                        return true;
                    break;
                case 42:
                    if (olditem.Class <= 45 && olditem.Class >= 42)
                        return true;
                    break;
                case 43:
                    if (olditem.Class <= 45 && olditem.Class >= 43)
                        return true;
                    break;
                case 44:
                    if (olditem.Class <= 45 && olditem.Class >= 44)
                        return true;
                    break;
                case 45:
                    if (olditem.Class == 45)
                        return true;
                    break;
                case 50:
                    if (olditem.Class <= 55 && olditem.Class >= 50)
                        return true;
                    break;
                case 51:
                    if (olditem.Class <= 55 && olditem.Class >= 51)
                        return true;
                    break;
                case 52:
                    if (olditem.Class <= 55 && olditem.Class >= 52)
                        return true;
                    break;
                case 53:
                    if (olditem.Class <= 55 && olditem.Class >= 53)
                        return true;
                    break;
                case 54:
                    if (olditem.Class <= 55 && olditem.Class >= 54)
                        return true;
                    break;
                case 55:
                    if (olditem.Class == 55)
                        return true;
                    break;
                case 60:
                    if (olditem.Class <= 65 && olditem.Class >= 60)
                        return true;
                    break;
                case 61:
                    if (olditem.Class <= 65 && olditem.Class >= 61)
                        return true;
                    break;
                case 62:
                    if (olditem.Class <= 65 && olditem.Class >= 62)
                        return true;
                    break;
                case 63:
                    if (olditem.Class <= 65 && olditem.Class >= 63)
                        return true;
                    break;
                case 64:
                    if (olditem.Class <= 65 && olditem.Class >= 64)
                        return true;
                    break;
                case 65:
                    if (olditem.Class == 65)
                        return true;
                    break;
                case 190:
                    if (olditem.Class >= 100)
                        return true;
                    break;
                case 70:
                    if (olditem.Class <= 75 && olditem.Class >= 70)
                        return true;
                    break;
                case 71:
                    if (olditem.Class <= 75 && olditem.Class >= 71)
                        return true;
                    break;
                case 72:
                    if (olditem.Class <= 75 && olditem.Class >= 72)
                        return true;
                    break;
                case 73:
                    if (olditem.Class <= 75 && olditem.Class >= 73)
                        return true;
                    break;
                case 74:
                    if (olditem.Class <= 75 && olditem.Class >= 74)
                        return true;
                    break;
                case 75:
                    if (olditem.Class == 75)
                        return true;
                    break;
                case 0:
                    return true;
                default:
                    return false;
            }
            return false;
        }

        public static byte GetNextRefineryItem()
        {
            if (Extensions.BaseFunc.RandGet(100, false) < 30)
                return 2;
            if (Extensions.BaseFunc.RandGet(100, false) < 30)
                return 1;
            return 0;
        }

        public static byte GetLevel(uint ID)
        {
            if (ItemPosition(ID) == 3 || ItemPosition(ID) == 1 || IsShield(ID))
                return (byte)(ID % 100u / 10u);
            return (byte)(ID % 1000u / 10u);
        }

        public uint GetNextItemQuality()
        {
            DBItem item;
            item = new DBItem();
            if (item.ID % 10u < 3 || item.ID % 10u == 9)
                return item.ID;
            uint tempID;
            tempID = item.ID;
            if (item.ID < 5)
                tempID = item.ID / 10u * 10 + 5;
            if (!Server.ItemsBase.ContainsKey(tempID + 1))
                return item.ID;
            return tempID + 1;
        }

        internal static int ChanceToUpgradeQuality(uint ID)
        {
            int chance;
            chance = 100;
            if (ID % 10u == 9 || ID % 10u < 3)
                return 0;
            Math.Max(5u, ID % 10u);
            switch (ID % 10u)
            {
                case 6u:
                    chance = 50;
                    break;
                case 7u:
                    chance = 33;
                    break;
                case 8u:
                    chance = 20;
                    break;
                default:
                    chance = 100;
                    break;
            }
            byte lvl;
            lvl = Server.ItemsBase[ID].Level;
            if (lvl > 70)
                chance = chance * (100 - (lvl - 70)) / 100;
            return Math.Max(1, chance);
        }

        public static int GetUpEpLevelInfo(uint ID)
        {
            int cost = 0;
            int nLev = GetLevel(ID);

            if (ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Armor || ItemPosition(ID) == (ushort)Role.Flags.ConquerItem.Head || IsShield(ID))
            {
                switch (nLev)
                {
                    case 5: cost = 50; break;
                    case 6: cost = 40; break;
                    case 7: cost = 30; break;
                    case 8:
                    case 9: cost = 20; break;
                    default: cost = 500; break;
                }

                int nQuality = (int)(ID % 10);
                switch (nQuality)
                {
                    case 6: cost = cost * 90 / 100; break;
                    case 7: cost = cost * 70 / 100; break;
                    case 8: cost = cost * 30 / 100; break;
                    case 9: cost = cost * 10 / 100; break;
                    default:
                        break;
                }
            }
            else
            {
                switch (nLev)
                {
                    case 11: cost = 95; break;
                    case 12: cost = 90; break;
                    case 13: cost = 85; break;
                    case 14: cost = 80; break;
                    case 15: cost = 75; break;
                    case 16: cost = 70; break;
                    case 17: cost = 65; break;
                    case 18: cost = 60; break;
                    case 19: cost = 55; break;
                    case 20: cost = 50; break;
                    case 21: cost = 45; break;
                    case 22:
                    case 23: cost = 40; break;
                    default: cost = 500; break;
                }

                int nQuality = (int)(ID % 10);
                switch (nQuality)
                {
                    case 6: cost = cost * 90 / 100; break;
                    case 7: cost = cost * 70 / 100; break;
                    case 8: cost = cost * 30 / 100; break;
                    case 9: cost = (cost * 10 / 100); break;
                    default: break;
                }
            }
            return (100 / cost + 1) * 12 / 10;
        }

        //public static int GetUpEpLevelInfo(uint ID)
        //{
        //    int cost;
        //    cost = 0;
        //    int nLev;
        //    nLev = GetLevel(ID);
        //    if (ItemPosition(ID) == 3 || ItemPosition(ID) == 1 || IsShield(ID))
        //    {
        //        switch (nLev)
        //        {
        //            case 5:
        //                cost = 50;
        //                break;
        //            case 6:
        //                cost = 40;
        //                break;
        //            case 7:
        //                cost = 30;
        //                break;
        //            case 8:
        //            case 9:
        //                cost = 20;
        //                break;
        //            default:
        //                cost = 500;
        //                break;
        //        }
        //        switch ((int)(ID % 10u))
        //        {
        //            case 6:
        //                cost = cost * 90 / 100;
        //                break;
        //            case 7:
        //                cost = cost * 70 / 100;
        //                break;
        //            case 8:
        //                cost = cost * 30 / 100;
        //                break;
        //            case 9:
        //                cost = cost * 10 / 100;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        switch (nLev)
        //        {
        //            case 11:
        //                cost = 95;
        //                break;
        //            case 12:
        //                cost = 90;
        //                break;
        //            case 13:
        //                cost = 85;
        //                break;
        //            case 14:
        //                cost = 80;
        //                break;
        //            case 15:
        //                cost = 75;
        //                break;
        //            case 16:
        //                cost = 70;
        //                break;
        //            case 17:
        //                cost = 65;
        //                break;
        //            case 18:
        //                cost = 60;
        //                break;
        //            case 19:
        //                cost = 55;
        //                break;
        //            case 20:
        //                cost = 50;
        //                break;
        //            case 21:
        //                cost = 45;
        //                break;
        //            case 22:
        //                cost = 40;
        //                break;
        //            default:
        //                cost = 500;
        //                break;
        //        }
        //        switch ((int)(ID % 10u))
        //        {
        //            case 6:
        //                cost = cost * 90 / 100;
        //                break;
        //            case 7:
        //                cost = cost * 70 / 100;
        //                break;
        //            case 8:
        //                cost = cost * 30 / 100;
        //                break;
        //            case 9:
        //                cost = cost * 10 / 100;
        //                break;
        //        }
        //    }
        //    return (100 / cost + 1) * 12 / 10;
        //}

        public static int GetUpEpQualityInfo(uint ID)
        {
            DBItem item;
            item = Server.ItemsBase[ID];
            int change;
            change = 100;
            switch (ID % 10u)
            {
                case 6u:
                    change = 50;
                    break;
                case 7u:
                    change = 33;
                    break;
                case 8u:
                    change = 20;
                    break;
                default:
                    change = 100;
                    break;
            }
            int level;
            level = item.Level;
            if (level > 70)
                change = (int)((double)change * (100.0 - (double)(level - 70) * 1.0) / 100.0);
            return Math.Max(1, 100 / change);
        }

        public static bool UpQualityDB(uint ID, uint DBs)
        {
            int cost;
            cost = GetUpEpQualityInfo(ID);
            if (DBs >= cost)
                return true;
            double percent;
            percent = 100 / cost;
            double MyCost;
            MyCost = (double)DBs * percent;
            return (double)Extensions.BaseFunc.RandGet(100, true) < MyCost;
        }

        public static bool UpItemMeteors(uint ID, uint Meteors)
        {
            int CompleteCost;
            CompleteCost = GetUpEpLevelInfo(ID);
            if (Meteors >= CompleteCost)
                return true;
            double percent;
            percent = 100 / CompleteCost;
            double MyCost;
            MyCost = (double)Meteors * percent;
            return (double)Extensions.BaseFunc.RandGet(100, true) < MyCost;
        }

        public static bool EquipPassSexReq(DBItem baseInformation, GameClient client)
        {
            int ClientGender;
            ClientGender = (((int)client.Player.Body % 10000 < 1005) ? 1 : 2);
            if (baseInformation.Gender == 2 && ClientGender == 2)
                return true;
            if (baseInformation.Gender == 1 && ClientGender == 1)
                return true;
            if (baseInformation.Gender == 0)
                return true;
            return false;
        }

        public static bool EquipPassRbReq(DBItem baseInformation, GameClient client)
        {
            if (baseInformation.Level < 71 && client.Player.Reborn > 0 && client.Player.Level >= 70)
                return true;
            return false;
        }

        public static bool EquipPassLvlReq(DBItem baseInformation, GameClient client)
        {
            if (client.Player.Level < baseInformation.Level)
                return false;
            return true;
        }

        public static bool Equipable(MsgGameItem item, GameClient client)
        {
            if (Server.ItemsBase.TryGetValue(item.ITEM_ID, out var BaseInformation))
            {
                bool pass;
                pass = false;
                if (!EquipPassSexReq(BaseInformation, client))
                    return false;
                if (EquipPassRbReq(BaseInformation, client))
                    pass = true;
                else if (EquipPassJobReq(BaseInformation, client) && EquipPassStatsReq(BaseInformation, client) && EquipPassLvlReq(BaseInformation, client))
                {
                    pass = true;
                }
                if (!pass)
                    return false;
                if (client.Player.Reborn > 0 && client.Player.Level >= 70)
                {
                    _ = BaseInformation.Level;
                    _ = 70;
                    return pass;
                }
                return pass;
            }
            return false;
        }

        public static bool Equipable(uint ItemID, GameClient client, bool UpgradeByPass = false)
        {
            if (Server.ItemsBase.TryGetValue(ItemID, out var BaseInformation))
            {
                bool pass;
                pass = false;
                if (EquipPassRbReq(BaseInformation, client))
                    pass = true;
                else if (UpgradeByPass && (EquipPassJobReq(BaseInformation, client) || UpgradeByPass) && EquipPassLvlReq(BaseInformation, client))
                {
                    pass = true;
                }
                if (!pass)
                    return false;
                if (client.Player.Reborn > 0 && client.Player.Level >= 70)
                {
                    _ = BaseInformation.Level;
                    _ = 70;
                    return pass;
                }
                return pass;
            }
            return false;
        }

        public static uint GetBaseID(uint ID)
        {
            int itemtype;
            itemtype = (int)(ID / 1000u);
            if (ID == 300000)
                return ID;
            byte b;
            b = (byte)(ID / 10000u);
            if ((uint)b <= 20u)
            {
                if ((uint)(b - 11) > 5u && b != 20)
                    goto IL_005b;
            }
            else if (b != 42)
            {
                if ((uint)(b - 60) <= 2u)
                {
                    ID -= ID % 10u;
                    goto IL_008d;
                }
                if (b != 90)
                    goto IL_005b;
            }
            if (itemtype == 420)
                goto IL_005b;
            ID -= ID % 10u;
            goto IL_008d;
        IL_008d:
            return ID;
        IL_005b:
            byte def_val;
            def_val = (byte)(ID / 100000u);
            ID = (uint)(def_val * 100000 + def_val * 10000 + def_val * 1000 + (ID % 1000u - ID % 10u));
            goto IL_008d;
        }
    }
}
