using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Game.MsgMonster
{
    public class MonsterFamily
    {
        public static Random Random;

        public int rest_secs;

        public string Name;

        public int MaxAttack;

        public int MinAttack;

        public int MaxHealth;

        public ushort Defense;

        public ushort Mesh;

        public ushort Level;

        public byte ViewRange;

        public sbyte AttackRange;

        public uint LeftWeaponId;

        public uint RightWeaponId;

        public uint HeadId;

        public uint ArmorId;

        public byte Dodge;

        public uint ID;

        public byte Boss;

        public int AttackSpeed;

        public int MoveSpeed;

        public uint SpellId;

        public int RespawnTime;

        public int Defense2;

        public byte DropBoots;

        public byte DropArmor;

        public byte DropShield;

        public byte DropWeapon;

        public byte DropArmet;

        public byte DropRing;

        public byte DropNecklace;

        public ushort DropMoney;

        public uint DropHPItem;

        public uint DropMPItem;

        public ushort SpawnX;

        public ushort SpawnY;

        public ushort MaxSpawnX;

        public ushort MaxSpawnY;

        public uint MapID;

        public int SpawnCount;

        public int maxnpc;

        public int extra_battlelev;

        public int extra_exp;

        public int extra_damage;

        public uint ExtraCritical;

        public uint ExtraBreack;

        public Extensions.Time32 LastMove;
        public Extensions.Time32 Lastpop;

        //public SpecialItemWatcher[] DropSpecials;

        public MonsterSettings Settings;

        public MobItemGenerator ItemGenerator;

        private static Dictionary<uint, MobItemGenerator> ItemGeneratorLinker;

        static MonsterFamily()
        {
            Random = new Random();
            ItemGeneratorLinker = new Dictionary<uint, MobItemGenerator>();
        }

        public void CreateMonsterSettings()
        {
            Settings = MonsterSettings.Standard;
            if ((Name.Contains("Guard") &&  !Name.Contains("Chaos"))  || Name.Contains("CitySoldier"))
                Settings = MonsterSettings.Guard;
            else if (Name.Contains("Reviver"))
            {
                Settings = MonsterSettings.Reviver;
            }
            else if (Name.Contains("King"))
            {
                Settings = MonsterSettings.King;
            }
            else if (Name.Contains("Messenger"))
            {
                Settings = MonsterSettings.Messenger;
            }
            //else if (Name.Contains("Satan"))
            //{
            //    Settings = MonsterSettings.HasPlayerOwner;
            //}
        }

        public void CreateItemGenerator()
        {
            if (!ItemGeneratorLinker.TryGetValue(ID, out ItemGenerator))
            {
                ItemGenerator = new MobItemGenerator(this);
                ItemGeneratorLinker.Add(ID, ItemGenerator);
            }
        }

        public MonsterFamily Copy()
        {
            MonsterFamily Mob;
            Mob = new MonsterFamily
            {
                Name = Name,
                MaxAttack = MaxAttack,
                MinAttack = MinAttack,
                MaxHealth = MaxHealth,
                Defense = Defense,
                Mesh = Mesh,
                Level = Level,
                ViewRange = ViewRange,
                AttackRange = AttackRange,
                Dodge = Dodge,
                ID = ID,
                DropBoots = DropBoots,
                DropArmor = DropArmor,
                DropShield = DropShield,
                DropWeapon = DropWeapon,
                DropRing = DropRing,
                DropNecklace = DropNecklace,
                DropMoney = DropMoney,
                DropHPItem = DropHPItem,
                DropMPItem = DropMPItem,
                Boss = Boss,
                Defense2 = Defense2,
                AttackSpeed = AttackSpeed,
                MoveSpeed = MoveSpeed,
                SpellId = SpellId,
                RespawnTime = Random.Next(10, 20),
                ExtraCritical = ExtraCritical,
                ExtraBreack = ExtraBreack,
                extra_battlelev = extra_battlelev,
                extra_damage = extra_damage,
                extra_exp = extra_exp
            };
            if (Mob.MaxHealth > 300000 && Mob.MaxHealth < 7000000)
            {
                Mob.AttackRange = 12;
                Mob.RespawnTime = 3600;
            }
            if (Mob.ID == 61190 || Mob.ID == 3143 || Mob.ID == 3146 || Mob.ID == 3149 || Mob.ID == 31569 || Mob.ID == 3102
                || Mob.ID == 8500)
                Mob.RespawnTime = rest_secs;
            //if (Mob.ID == 20055)
            //    Mob.RespawnTime = 60 * 60;
            if (Mob.MapID == 1700)
                Mob.RespawnTime = rest_secs;
            //Mob.DropSpecials = new SpecialItemWatcher[DropSpecials.Length];
            //for (int x = 0; x < DropSpecials.Length; x++)
            //{
            //    Mob.DropSpecials[x] = DropSpecials[x];
            //}
            Mob.CreateItemGenerator();
            Mob.CreateMonsterSettings();
            return Mob;
        }
    }
}
