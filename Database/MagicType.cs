using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TheChosenProject.Role;

namespace TheChosenProject.Database
{
    public class MagicType : Dictionary<ushort, Dictionary<ushort, MagicType.Magic>>
    {
        public enum WeaponsType : ushort
        {
            Boxing = 0,
            Blade = 410,
            Sword = 420,
            Backsword = 421,
            Hook = 430,
            Whip = 440,
            Mace = 441,
            Axe = 450,
            Hammer = 460,
            Crutch = 470,
            Club = 480,
            Scepter = 481,
            Dagger = 490,
            Prod = 491,
            Fan = 492,
            Flute = 493,
            Glaive = 510,
            Scythe = 511,
            Epee = 520,
            Zither = 521,
            Lute = 522,
            Poleaxe = 530,
            LongHammer = 540,
            Spear = 560,
            Pickaxe = 562,
            Spade = 570,
            Halbert = 580,
            Wand = 561,
            Bow = 500,
            NinjaSword = 601,
            ThrowingKnife = 613,
            MagicSword = 721,
            Shield = 900,
            Other = 422,
            PrayerBeads = 610,
            Rapier = 611,
            Pistol = 612,
            CrossSaber = 614,
            TwistedClaw = 616,
            Nunchaku = 617,
            Fist = 624,
            WindFan = 626
        }

        public enum MagicSort
        {
            Attack = 1,
            Recruit = 2,
            Cross = 3,
            Sector = 4,
            Bomb = 5,
            AttachStatus = 6,
            DetachStatus = 7,
            Square = 8,
            JumpAttack = 9,
            RandomTransport = 10,
            DispatchXp = 11,
            Collide = 12,
            SerialCut = 13,
            Line = 14,
            AtkRange = 15,
            AttackStatus = 16,
            CallTeamMember = 17,
            RecordTransportSpell = 18,
            Transform = 19,
            AddMana = 20,
            LayTrap = 21,
            Dance = 22,
            CallPet = 23,
            Vampire = 24,
            Instead = 25,
            DecLife = 26,
            Toxic = 27,
            ShurikenVortex = 28,
            CounterKill = 29,
            Spook = 30,
            WarCry = 31,
            Riding = 32,
            ChainBolt = 37,
            StarArrow = 38,
            DragonWhirl = 40,
            RemoveBuffers = 46,
            Tranquility = 47,
            DirectAttack = 48,
            Compasion = 50,
            Auras = 51,
            ShieldBlock = 52,
            Oblivion = 53,
            WhirlwindKick = 54,
            PhysicalSpells = 55,
            ScurvyBomb = 56,
            CannonBarrage = 57,
            BlackSpot = 58,
            Summon = 72,
            BombLine = 60,
            MoveLine = 61,
            AddBlackSpot = 62,
            PirateXpSkill = 63,
            ChargingVortex = 64,
            MortalDrag = 65,
            KineticSpark = 67,
            BladeFlurry = 68,
            SectorBack = 69,
            BreathFocus = 70,
            FatalCross = 71,
            Summons = 72,
            FatalSpin = 73,
            DragonCyclone = 75,
            StraightFist = 76,
            Perimeter = 78,
            AirKick = 79,
            Strike = 81,
            SectorPasive = 82,
            ManiacDance = 83,
            Omnipotence = 85,
            Pounce = 84,
            BurntFrost = 86,
            Rectangle = 87,
            RemoveStamin = 88,
            PetAttachStatus = 89,
            SwirlingStorm = 91
        }

        public class Magic
        {
            public ushort ID;

            public string Name;

            public MagicSort Type;

            public byte Level;

            public ushort UseMana;

            public uint UseArrows;

            public float Damage;

            public int Rate;

            public uint Experience;

            public byte Range;

            public ushort Sector;

            public uint Duration;

            public uint WeaponType;

            public byte UseStamina;

            public uint GiveHitPoints;

            public bool AttackInFly;

            public int Status;

            public ushort NeedLevel;

            public uint CpsCost;

            public byte MaxTargets;

            public byte IncreaseStamin;

            public ushort CoolDown = 500;

            //public byte AutoLearn;

            public int ColdTime;

            public int Damage2;

            public int Damage3;

            public int DamageOnMonster;

            public uint CPUpgradeRatio;

            public int CustomCoolDown = 200;

            public bool IsSpellWithColdTime => ColdTime > 0;
        }

        public static List<Flags.SpellID> RandomSpells = new List<Flags.SpellID>
        {
            Flags.SpellID.Phoenix,
            Flags.SpellID.Rage,
            Flags.SpellID.Boreas,
            Flags.SpellID.Earthquake,
            Flags.SpellID.Halt,
            Flags.SpellID.Phoenix,
            Flags.SpellID.Rage,
            Flags.SpellID.Roamer,
            Flags.SpellID.Seizer,
            Flags.SpellID.Snow,
            Flags.SpellID.TripleAttack,
            Flags.SpellID.WideStrike,
            Flags.SpellID.Windstorm,
            //Flags.SpellID.FastBlader,
            //Flags.SpellID.ScrenSword,
            Flags.SpellID.Poison
        };

        public void Load()
        {
            ServerKernel.Log.SaveLog("Loading Magictype information...", true);
            if (File.Exists(ServerKernel.CO2FOLDER + "magictype.txt"))
            {
                using (StreamReader read = File.OpenText(ServerKernel.CO2FOLDER + "magictype.txt"))
                {
                    string Linebas;
                    while ((Linebas = read.ReadLine()) != null)
                    {
                        string[] line;
                        line = Linebas.Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                        Magic spell;
                        spell = new Magic
                        {
                            ID = ushort.Parse(line[1])
                        };
                        spell.AttackInFly = AttackInFlay(spell);
                        spell.Type = (MagicSort)byte.Parse(line[2]);
                        spell.Name = line[3];
                        spell.Level = byte.Parse(line[8]);
                        spell.UseMana = ushort.Parse(line[9]);
                        if (spell.ID == 1190 || spell.ID == 1195 || spell.ID == 1005 || spell.ID == 1175 || spell.ID == 1055 || spell.ID == 1170 || spell.ID == 4000)
                            spell.Damage = uint.Parse(line[10]);
                        else if (spell.ID == 6001)
                        {
                            spell.Damage = (int)(byte)(double.Parse(line[10]) % 100.0);
                        }
                        else
                        {
                            spell.Damage = (int)(byte)(double.Parse(line[10]) % 1000.0);
                        }
                        spell.CoolDown = ushort.Parse(line[33]);
                        if (spell.ID == 10310)
                        {
                            switch (spell.Level)
                            {
                                case 0:
                                    spell.UseMana = 500;
                                    break;
                                case 1:
                                    spell.UseMana = 1500;
                                    break;
                                case 2:
                                    spell.UseMana = 2500;
                                    break;
                                case 3:
                                    spell.UseMana = 3500;
                                    break;
                                case 4:
                                    spell.UseMana = 5000;
                                    break;
                            }
                        }
                        spell.Rate = int.Parse(line[12]);
                        spell.MaxTargets = byte.Parse(line[14]);
                        spell.Experience = uint.Parse(line[18]);
                        spell.NeedLevel = ushort.Parse(line[20]);
                        spell.WeaponType = uint.Parse(line[22]);
                        spell.UseStamina = byte.Parse(line[29]);
                        if (spell.ID == 6001)
                            spell.Duration = 20u;
                        else
                            spell.Duration = uint.Parse(line[13]);
                        //spell.AutoLearn = byte.Parse(line[30]);
                        spell.UseArrows = uint.Parse(line[34]);
                        spell.Damage2 = int.Parse(line[36]);
                        spell.Damage3 = int.Parse(line[37]);
                        spell.DamageOnMonster = int.Parse(line[44]);
                        spell.ColdTime = int.Parse(line[47]);
                        spell.Range = byte.Parse(line[15]);
                        if (spell.ID == 7020 || spell.ID == 1115 || spell.ID == 5010)
                            spell.Range /= 2;
                        spell.Status = int.Parse(line[16]);
                        spell.Sector = (ushort)(spell.Range * 20);
                        if (spell.ID == 10415)
                            spell.UseStamina = 30;
                        if (spell.ID == 10381 || spell.ID == 1115)
                            spell.CustomCoolDown = 800;
                        if (spell.ID == 1260)
                            spell.CustomCoolDown = 1000;
                        if (spell.ID == 6000)
                            spell.CustomCoolDown = 600;
                        if (spell.ID == 6002)
                            spell.CustomCoolDown = 1000;
                        if (spell.ID == 30000)
                            spell.CustomCoolDown = 1000;
                        if (spell.ID == 1045 || spell.ID == 1046 || spell.ID == 11005)
                        {
                            //if (spell.CoolDown > 300 && spell.CoolDown < 500)
                            spell.CustomCoolDown = 500;
                        }
                        if (spell.WeaponType != 0)
                        {
                            if (spell.WeaponType > 100000)
                            {
                                ushort proftwo;
                                proftwo = 0;
                                ushort profone;
                                profone = ushort.Parse(spell.WeaponType.ToString()[0].ToString() + spell.WeaponType.ToString()[1] + spell.WeaponType.ToString()[2]);
                                proftwo = ushort.Parse(spell.WeaponType.ToString()[3].ToString() + spell.WeaponType.ToString()[4] + spell.WeaponType.ToString()[5]);
                                if (!Server.WeaponSpells.ContainsKey(profone))
                                    Server.WeaponSpells.Add(profone, spell.ID);
                                if (!Server.WeaponSpells.ContainsKey(proftwo))
                                    Server.WeaponSpells.Add(proftwo, spell.ID);
                            }
                            else if (!Server.WeaponSpells.ContainsKey((ushort)spell.WeaponType))
                            {
                                Server.WeaponSpells.Add((ushort)spell.WeaponType, spell.ID);
                            }
                        }
                        if (spell.ID == 12070)
                            spell.Damage = 104f;
                        if (ContainsKey(spell.ID))
                        {
                            if (!base[spell.ID].ContainsKey(spell.Level))
                                base[spell.ID].Add(spell.Level, spell);
                        }
                        else
                        {
                            Add(spell.ID, new Dictionary<ushort, Magic>());
                            base[spell.ID].Add(spell.Level, spell);
                        }
                    }
                }
            }
            if (Server.WeaponSpells.ContainsKey(420))
                Server.WeaponSpells[420] = 5030;
            if (Server.WeaponSpells.ContainsKey(410))
                Server.WeaponSpells.Remove(410);
            if (Server.WeaponSpells.ContainsKey(900))
                Server.WeaponSpells.Remove(900);
            if (!Server.WeaponSpells.ContainsKey(460))
                Server.WeaponSpells.Add(460, 5040);
            if (Server.WeaponSpells.ContainsKey(601))
                Server.WeaponSpells.Remove(601);
            if (Server.WeaponSpells.ContainsKey(610))
                Server.WeaponSpells[610] = 10490;
            if (Server.WeaponSpells.ContainsKey(611))
                Server.WeaponSpells[611] = 11140;
            if (Server.WeaponSpells.ContainsKey(612))
                Server.WeaponSpells[612] = 11120;
            else
                Server.WeaponSpells.Add(612, 11120);
            if (Server.WeaponSpells.ContainsKey(616))
                Server.WeaponSpells[616] = 12110;
            if (Server.WeaponSpells.ContainsKey(617))
                Server.WeaponSpells[617] = 12240;
            if (Server.WeaponSpells.ContainsKey(511))
                Server.WeaponSpells[511] = 0;
            if (!Server.WeaponSpells.ContainsKey(624))
                Server.WeaponSpells.Add(624, 12670);
            //SkillLearn.AddLearnSkills();
            Add(6009, base[6010]);
        }

        public bool AttackInFlay(Magic DBSpell)
        {
            if (DBSpell.ID == 1045 || DBSpell.ID == 1046 || DBSpell.ID == 1115 || DBSpell.ID == 1250 || DBSpell.ID == 1260 || DBSpell.ID == 1290 || (DBSpell.ID >= 5010 && DBSpell.ID <= 5050) || DBSpell.ID == 6000 || DBSpell.ID == 6001 || (DBSpell.ID >= 7000 && DBSpell.ID <= 7040) || DBSpell.ID == 10315 || DBSpell.ID == 10381 || DBSpell.ID == 10490 || DBSpell.ID == 11000 || DBSpell.ID == 11005 || DBSpell.ID == 11040 || DBSpell.ID == 11070 || DBSpell.ID == 11110 || DBSpell.ID == 11140 || DBSpell.ID == 11170 || DBSpell.ID == 11180 || DBSpell.ID == 11190 || DBSpell.ID == 11230)
                return false;
            return true;
        }

        public uint GetSpell(string name)
        {
            uint spellid;
            spellid = 0u;
            foreach (Dictionary<ushort, Magic> spells in base.Values)
            {
                foreach (Magic spell in spells.Values)
                {
                    if (spell.Name != null && spell.Name.Contains(name))
                        spellid = spell.ID;
                }
            }
            return spellid;
        }
    }

}