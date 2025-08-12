using TheChosenProject.Database;
using System.Collections.Generic;

 
namespace TheChosenProject.Role
{
  public struct SkillLearn
  {
    public Flags.SpellID ID;
    public byte Lvl;
    public SkillLearn.type Type;

    public static void AddLearnSkills()
    {
      //List<SkillLearn> skillLearnList1 = new List<SkillLearn>()
      //{
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Superman,
      //    Lvl = (byte) 0,
      //    Type = SkillLearn.type.XP
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.FastBlader,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.ScrenSword,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.ViperFang,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.DragonTail,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.StrandedMonster,
      //    Lvl = (byte) 9,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Snow,
      //    Lvl = (byte) 9,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.SpeedGun,
      //    Lvl = (byte) 9,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.MagicShield,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Stigma,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  }
      //};
      //Server.SkillForLearning.Add(Flags.BaseClassType.Warrior, skillLearnList1);
      //List<SkillLearn> skillLearnList2 = new List<SkillLearn>()
      //{
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Cyclone,
      //    Lvl = (byte) 0,
      //    Type = SkillLearn.type.XP
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.FastBlader,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.ScrenSword,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Hercules,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Phoenix,
      //    Lvl = (byte) 9,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Rage,
      //    Lvl = (byte) 9,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.DragonWhirl,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.MagicShield,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Stigma,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  }
      //};
      //Server.SkillForLearning.Add(Flags.BaseClassType.Trojan, skillLearnList2);
      //List<SkillLearn> skillLearnList3 = new List<SkillLearn>()
      //{
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.ScatterFire,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.XpFly,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.XP
      //  }
      //};
      //Server.SkillForLearning.Add(Flags.BaseClassType.Archer, skillLearnList3);
      //List<SkillLearn> skillLearnList4 = new List<SkillLearn>()
      //{
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Thunder,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Fire,
      //    Lvl = (byte) 3,
      //    Type = SkillLearn.type.Normal
      //  }
      //};
      //Server.SkillForLearning.Add(Flags.BaseClassType.Water, skillLearnList4);
      //List<SkillLearn> skillLearnList5 = new List<SkillLearn>()
      //{
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Thunder,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Fire,
      //    Lvl = (byte) 3,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Tornado,
      //    Lvl = (byte) 3,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.FireCircle,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.FireRing,
      //    Lvl = (byte) 3,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Bomb,
      //    Lvl = (byte) 3,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.FireofHell,
      //    Lvl = (byte) 3,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.FireMeteor,
      //    Lvl = (byte) 7,
      //    Type = SkillLearn.type.Normal
      //  }
      //};
      //Server.SkillForLearning.Add(Flags.BaseClassType.Fire, skillLearnList5);
      //List<SkillLearn> skillLearnList6 = new List<SkillLearn>()
      //{
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.TwofoldBlades,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.PoisonStar,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.ToxicFog,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.MagicShield,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Stigma,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  }
      //};
      //Server.SkillForLearning.Add(Flags.BaseClassType.Ninja, skillLearnList6);
      //List<SkillLearn> skillLearnList7 = new List<SkillLearn>()
      //{
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.WhirlwindKick,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.TripleAttack,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.RadiantPalm,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.MagicShield,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  },
      //  new SkillLearn()
      //  {
      //    ID = Flags.SpellID.Stigma,
      //    Lvl = (byte) 4,
      //    Type = SkillLearn.type.Normal
      //  }
      //};
      //Server.SkillForLearning.Add(Flags.BaseClassType.Monk, skillLearnList7);
    }

    public enum type
    {
      Normal,
      Pure,
      XP,
    }
  }
}
