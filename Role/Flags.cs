using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Role
{
    public class Flags
    {
        public enum MissionsFlag : uint
        {
            NONE = 0u,
            DEFEAT_PHEASANT = 20191u,
            DEFEAT_RED_DEVILES = 20192u,
            DEFEAT_FIVE_BOSS = 20193u,
            DEFEAT_TITAN_BOSS = 20194u,
            COMPLETE_5_MISSION = 20195u,
            DEFEAT_GANDORMA_BOSS = 20196u,
            DEFEAT_BIRD_MAN = 20197u,
            DEFEAT_300K_MOBS = 20198u,
            DEFEAT_100K_MOBS = 20199u,
            OPEN_3_BOXS = 20200u
        }

        public enum FairbattlePower : byte
        {
            NotActive,
            UpdateToSerf,
            UpdateToKnight,
            UpdateToBaron,
            UpdateToEarl,
            UpdateToDuke,
            UpdateToPrince,
            UpdateToKing
        }

        [Flags]
        public enum CurrentPoint : int
        {
            Silver = 2,
            CPS = 4,
            OTP = 8,
            GiftCPS = 0x10
        }

        [Flags]
        public enum CurrentPointAction : uint
        {
            Add = 1u,
            Remove = 2u
        }

        public enum NewbieExperience : byte
        {
            NotActive,
            Disable,
            Enable,
            Remove
        }

        public enum ExploitsRank : uint
        {
            Corporal = 1u,
            Decurion,
            Centurion,
            Sergeant,
            StaffSergeant,
            MasterSergeant,
            DeputyGeneral,
            AssistantGeneral,
            General,
            ChiefofStaff,
            ChariotsandCavalryGeneral,
            FlyingCavalryGeneral,
            GeneralinChief
        }

        public enum GuildMemberRank : ushort
        {
            GuildLeader = 1000,
            DeputyLeader = 990,
            HDeputyLeader = 980,
            LeaderSpouse = 920,
            Manager = 890,
            HonoraryManager = 880,
            TSupervisor = 859,
            OSupervisor = 858,
            CPSupervisor = 857,
            ASupervisor = 856,
            SSupervisor = 855,
            GSupervisor = 854,
            PKSupervisor = 853,
            RoseSupervisor = 852,
            LilySupervisor = 851,
            Supervisor = 850,
            HonorarySuperv = 840,
            Steward = 690,
            HonorarySteward = 680,
            DeputySteward = 650,
            DLeaderSpouse = 620,
            DLeaderAide = 611,
            LSpouseAide = 610,
            Aide = 602,
            TulipAgent = 599,
            OrchidAgent = 598,
            CPAgent = 597,
            ArsenalAgent = 596,
            SilverAgent = 595,
            GuideAgent = 594,
            PKAgent = 593,
            RoseAgent = 592,
            LilyAgent = 591,
            Agent = 590,
            SupervSpouse = 521,
            ManagerSpouse = 520,
            SupervisorAide = 511,
            ManagerAide = 510,
            TulipFollower = 499,
            OrchidFollower = 498,
            CPFollower = 497,
            ArsFollower = 496,
            SilverFollower = 495,
            GuideFollower = 494,
            PKFollower = 493,
            RoseFollower = 492,
            LilyFollower = 491,
            Follower = 490,
            StewardSpouse = 420,
            SeniorMember = 210,
            Member = 200,
            None = 0
        }

        public enum NpcType : ushort
        {
            Stun = 0,
            Shop = 1,
            Talker = 2,
            Beautician = 5,
            Upgrader = 6,
            Socketer = 7,
            Pole = 10,
            Booth = 14,
            Gambling = 19,
            Stake = 21,
            Scarecrow = 22,
            Furniture = 25,
            Gate = 26,
            ClanInfo = 31,
            DialogAndGui = 32
        }

        public enum ConquerAngle : byte
        {
            SouthWest,
            West,
            NorthWest,
            North,
            NorthEast,
            East,
            SouthEast,
            South
        }

        public enum ConquerAction : uint
        {
            None = 0,
            Cool = 230,
            Kneel = 210,
            Sad = 170,
            Happy = 150,
            Angry = 160,
            Lie = 14,
            Dance = 1,
            Wave = 190,
            Bow = 200,
            Sit = 250,
            Jump = 100,
            MagicDefender = 273344,
            InteractionKiss = 34466,
            InteractionHold = 34468,
            InteractionHug = 34469,
            CoupleDances = 34474u
        }

        public enum SoulTyp
        {
            None = 0,
            Headgear = 1,
            Necklace = 2,
            Armor = 3,
            OneHandWeapon = 4,
            TwoHandWeapon = 5,
            Ring = 6,
            Boots = 8
        }

        public enum ConquerItem : ushort
        {
            Inventory = 0,
            Head = 1,
            Necklace = 2,
            Armor = 3,
            RightWeapon = 4,
            LeftWeapon = 5,
            Ring = 6,
            Bottle = 7,
            Boots = 8,
            Garment = 9,
            Fan = 10,
            Tower = 11,
            Steed = 12,
            RightWeaponAccessory = 15,
            LeftWeaponAccessory = 16,
            SteedMount = 17,
            RidingCrop = 18,
            AleternanteHead = 21,
            AleternanteNecklace = 22,
            AleternanteArmor = 23,
            AleternanteRightWeapon = 24,
            AleternanteLeftWeapon = 25,
            AleternanteRing = 26,
            AleternanteBottle = 27,
            AleternanteBoots = 28,
            AleternanteGarment = 29
        }

        [Flags]
        public enum ItemMode : ushort
        {
            None = 0,
            AddItem = 1,
            Trade = 2,
            Update = 3,
            View = 4,
            Active = 5,
            AddItemReturned = 8,
            ChatItem = 9,
            Auction = 0xC
        }

        public enum PKMode : byte
        {
            PK,
            Peace,
            Team,
            Capture,
            Revange,
            Guild
        }

        public enum ItemEffect : uint
        {
            None = 0u,
            Poison = 200u,
            HP = 201u,
            MP = 202u,
            Stigma = 203u,
            Shield = 0xCB,
            Horse = 100u
        }

        public enum ItemQuality : byte
        {
            Fixed = 0,
            Normal = 2,
            NormalV1 = 3,
            NormalV2 = 4,
            NormalV3 = 5,
            Refined = 6,
            Unique = 7,
            Elite = 8,
            Super = 9,
            Other = 1
        }

        public enum Color : uint
        {
            Black = 2u,
            Orange,
            LightBlue,
            Red,
            Blue,
            Yellow,
            Purple,
            White
        }

        public enum Gem : byte
        {
            NormalPhoenixGem = 1,
            RefinedPhoenixGem = 2,
            SuperPhoenixGem = 3,
            NormalDragonGem = 11,
            RefinedDragonGem = 12,
            SuperDragonGem = 13,
            NormalFuryGem = 21,
            RefinedFuryGem = 22,
            SuperFuryGem = 23,
            NormalRainbowGem = 31,
            RefinedRainbowGem = 32,
            SuperRainbowGem = 33,
            NormalKylinGem = 41,
            RefinedKylinGem = 42,
            SuperKylinGem = 43,
            NormalVioletGem = 51,
            RefinedVioletGem = 52,
            SuperVioletGem = 53,
            NormalMoonGem = 61,
            RefinedMoonGem = 62,
            SuperMoonGem = 63,
            NormalTortoiseGem = 71,
            RefinedTortoiseGem = 72,
            SuperTortoiseGem = 73,
            NormalThunderGem = 101,
            RefinedThunderGem = 102,
            SuperThunderGem = 103,
            NormalGloryGem = 121,
            RefinedGloryGem = 122,
            SuperGloryGem = 123,
            NoSocket = 0,
            EmptySocket = byte.MaxValue
        }

        public enum ExperienceEffect : byte
        {
            None,
            angelwing,
            bless
        }

        public enum BodyType : ushort
        {
            AgileMale = 1003,
            MuscularMale = 1004,
            AgileFemale = 2001,
            MuscularFemale = 2002
        }

        public enum BaseClassType : ushort
        {
            Trojan = 15,
            Warrior = 25,
            Archer = 45,
            Ninja = 55,
            Monk = 65,
            Taoist = 100,
            Water = 130,
            Fire = 140
        }

        public enum ProfessionType : ushort
        {
            INTERN_TROJAN = 10,
            TROJAN = 11,
            VETERAN_TROJAN = 12,
            TIGER_TROJAN = 13,
            DRAGON_TROJAN = 14,
            TROJAN_MASTER = 15,
            INTERN_WARRIOR = 20,
            WARRIOR = 21,
            BRASS_WARRIOR = 22,
            SILVER_WARRIOR = 23,
            GOLD_WARRIOR = 24,
            WARRIOR_MASTER = 25,
            INTERN_ARCHER = 40,
            ARCHER = 41,
            EAGLE_ARCHER = 42,
            TIGER_ARCHER = 43,
            DRAGON_ARCHER = 44,
            ARCHER_MASTER = 45,
            //INTERN_NINJA = 50,
            //NINJA = 51,
            //MIDDLE_NINJA = 52,
            //DARK_NINJA = 53,
            //MYSTIC_NINJA = 54,
            //NINJA_MASTER = 55,
            //INTERN_MONK = 60,
            //MONK = 61,
            //DHYANA_MONK = 62,
            //DHARMA_MONK = 63,
            //PRAJNA_MONK = 64,
            //NIRVANA_MONK = 65,
            INTERN_TAOIST = 100,
            TAOIST = 101,
            WATER_TAOIST = 132,
            WATER_WIZARD = 133,
            WATER_MASTER = 134,
            WATER_SAINT = 135,
            FIRE_TAOIST = 142,
            FIRE_WIZARD = 143,
            FIRE_MASTER = 144,
            FIRE_SAINT = 145
        }

        public enum SpellID : ushort
        {
            Physical = 0,
            SummonGuard = 4000,
            FireEvil = 4060,
            BloodyBat = 4050,
            Skeleton = 4070,
            SummonBat = 4010,
            SummonBatBoss = 4020,
            SummonPanda = 4077,
            Poison = 3306,
            EffectMP = 1175,
            EffectHP = 1190,
            Thunder = 1000,
            Fire = 1001,
            Tornado = 1002,
            Cure = 1005,
            Lightning = 1010,
            Accuracy = 1015,
            Shield = 1020,
            Superman = 1025,
            FastBlader = 1045,
            ScrenSword = 1046,
            Roar = 1040,
            Revive = 1050,
            Dash = 1051,
            HealingRain = 1055,
            Invisibility = 1075,
            StarofAccuracy = 1085,
            MagicShield = 1090,
            Stigma = 1095,
            Pray = 1100,
            Cyclone = 1110,
            Hercules = 1115,
            FireCircle = 1120,
            Vulcano = 1125,
            FireRing = 1150,
            Bomb = 1160,
            FireofHell = 1165,
            Nectar = 1170,
            AdvancedCure = 1175,
            FireMeteor = 1180,
            SpiritHealing = 1190,
            Meditation = 1195,
            WideStrike = 1250,
            SpeedGun = 1260,
            Golem = 1270,
            WaterElf = 1280,
            Penetration = 1290,
            Halt = 1300,
            FlyingMoon = 1320,
            DivineHare = 1350,
            NightDevil = 1360,
            CruelShade = 3050,
            Dodge = 3080,
            FreezingArrow = 5000,
            SpeedLightning = 5001,
            PoisonousArrows = 5002,
            Snow = 5010,
            StrandedMonster = 5020,
            Phoenix = 5030,
            Boom = 5040,
            Boreas = 5050,
            TwofoldBlades = 6000,
            ToxicFog = 6001,
            PoisonStar = 6002,
            CounterKill = 6003,
            ArcherBane = 6004,
            ShurikenEffect = 6009,
            ShurikenVortex = 6010,
            FatalStrike = 6011,
            Seizer = 7000,
            Riding = 7001,
            Spook = 7002,
            WarCry = 7003,
            Earthquake = 7010,
            Rage = 7020,
            Celestial = 7030,
            Roamer = 7040,
            RapidFire = 8000,
            ScatterFire = 8001,
            XpFly = 8002,
            Fly = 8003,
            ArrowRain = 8030,
            Intensify = 9000,
            Bless = 9876,
            AzureShield = 30000,
            ChainBolt = 10309,
            HeavenBlade = 10310,
            StarArrow = 10313,
            DragonWhirl = 10315,
            Perseverance = 10311,
            RadiantPalm = 10381,
            Oblivion = 10390,
            TyrantAura = 10395,
            Serenity = 10400,
            SoulShackle = 10405,
            FendAura = 10410,
            WhirlwindKick = 10415,
            MetalAura = 10420,
            WoodAura = 10421,
            WatherAura = 10422,
            FireAura = 10423,
            EarthAura = 10424,
            Tranquility = 10425,
            Compassion = 10430,
            ShieldBlock = 10470,
            TripleAttack = 10490,
            DragonTail = 11000,
            ViperFang = 11005,
            EagleEye = 11030,
            ScurvyBomb = 11040,
            CannonBarrage = 11050,
            BlackbeardsRage = 11060,
            GaleBomb = 11070,
            KrakensRevenge = 11100,
            BladeTempest = 11110,
            AdrenalineRush = 11130,
            Windstorm = 11140,
            DefensiveStance = 11160,
            BloodyScythe = 11170,
            MortalDrag = 11180,
            ChargingVortex = 11190,
            MagicDefender = 11200,
            GapingWounds = 11230
        }
    }
}
