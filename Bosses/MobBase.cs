using TheChosenProject.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Structures;
using TheChosenProject.Client;

namespace TheChosenProject.Bosses
{
    public class MobBase
    {
        public uint ID;
        Mob _boss;

        public MobBase(Mob _mob)
        {
            _boss = _mob;
        }
        public virtual void Run(GameClient C, Mob _mob, uint _damage, List<GameClient> PlayerTargets)
        {
            Dictionary<GameClient, uint> ToDisplay = new Dictionary<GameClient, uint>();
            foreach (uint UID in World.BossesDamage[_mob.MobID].Keys)
                if (World.H_Chars.ContainsKey(UID))
                    ToDisplay.Add(World.H_Chars[UID], World.BossesDamage[_mob.MobID][UID]);

            foreach (GameClient player in PlayerTargets)
            {
                byte Score = 2;
                player.MyClient.AddSend(Packets.ChatMessage(0, "SYSTEM", "ALLUSERS", _mob.Name + $" HP: {_mob.CurrentHP } / { _mob.MaxHP }", 0x83c, 0));
                player.MyClient.AddSend(Packets.ChatMessage(1, "SYSTEM", "ALLUSERS", $"--- Highest Damage ---", 0x83d, 0));
                foreach (var kvp in ToDisplay.OrderByDescending((s => s.Value)))
                {
                    if (Score == 5)
                        break;
                    if (Score == World.BossesDamage[_mob.MobID].Count + 2)
                        break;
                    player.MyClient.AddSend(Packets.ChatMessage(Score, "SYSTEM", "ALLUSERS", $"Nº {Score - 1}: { kvp.Key.Name } - {kvp.Value}", 0x83d, 0));
                    Score++;
                }
            }
        }

        /// <summary>
        /// Skills for Fire Type Boss
        /// </summary>
        /// <param name="Boss"></param>
        /// <param name="Damage"></param>
        /// <param name="PlayerTargets"></param>
        public void FireOfHell(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            //World.Action(Boss, (Packets.StringPacket(Boss.EntityID, 10, "zf2-e267")).Get);
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 1165;
            S.Info.Level = 3;
            S.Info.Damage = Damage;
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    if (!_char.Flying)
                        _char.TakeAttack(Boss, S.Info.Damage, AttackType.Magic, true);

                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Fire Of Hell dealing some amount of Magic Damage to everyone!");
                }
            }
        }
        public void FireBreath(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    _char.TakeAttack(Boss, Damage, AttackType.Magic, true);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "change").Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used FireBreath dealing some amount of Magic Damage to everyone!");
                }
            }
            World.Action(Boss, Packets.ShakeScreen(Boss.EntityID).Get);
        }
        public void FireMeteor(Mob Boss, double Percentage, List<GameClient> PlayerTargets)
        {
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 1180;
            S.Info.Level = 7;
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    double _pct = Percentage / 100;
                    S.Info.Damage = Convert.ToUInt32(_char.CurHP * _pct);
                    _char.TakeAttack(S.Info.Damage, S);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used FireMeteor reducing everyone's HP by " + Percentage + "% !");
                }
            }
        }
        public void Inferno(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            World.Action(Boss, (Packets.StringPacket(Boss.EntityID, StringType.Effect, "glebesword")).Get);
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 1120;
            S.Info.Level = 3;
            S.Info.Damage = Damage;
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    if (!_char.Flying)
                    {
                        _char.TakeAttack(Boss, S.Info.Damage, AttackType.Magic);
                        World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "thunder2").Get);
                    }

                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Inferno dealing some amount of Magic Damage to everyone!");
                }
            }
        }
        public void Eruption(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 10183;
            S.Info.Level = 0;
            foreach (GameClient _char in PlayerTargets)
            {
                S.Info.Damage = Damage;
                _char.TakeAttack(S.Info.Damage, S);
                _char.MyClient.LocalMessage(2000, Boss.Name + " used Eruption dealing some amount of Physical Damage to everyone!");
                //Game.World.Action(Boss, Packets.SkillUse(Boss.EntityID, _char.EntityID, 1, 10183, 0, 0, 0).Get);
            }
            World.Action(Boss, (Packets.StringPacket(Boss.EntityID, StringType.Effect, "firemagic")).Get);
            //Game.World.Action(Boss, Packets.SkillUse(S).Get);
        }

        /// <summary>
        /// Skills for Water Type Boss
        /// </summary>
        /// <param name="Boss"></param>
        /// <param name="Damage"></param>
        /// <param name="PlayerTargets"></param>
        public void Pervade(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            World.Action(Boss, (Packets.StringPacket(Boss.EntityID, StringType.Effect, "zf2-e267")).Get);
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 3090;
            S.Info.Level = 5;
            S.Info.Damage = Damage;
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    if (!_char.Flying)
                        _char.TakeAttack(Boss, S.Info.Damage, AttackType.Magic);

                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Pervade dealing some amount of Magic Damage to everyone!");
                }
            }
        }
        public void RainDance(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "colorstar").Get);
                    _char.TakeAttack(Boss, Damage);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Rain Dance dealing some Physical Damage to everyone!");
                }
            }
        }
        public void Whirlpool(Mob Boss, double Percentage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    double _pct = Percentage / 100;
                    uint Damage = Convert.ToUInt32(_char.CurHP * _pct);
                    _char.TakeAttack(Boss, Damage);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "zf2-e121").Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Whirlpool reducing everyone's HP by " + Percentage + "%!");
                }
            }
        }
        public void AquaRing(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            foreach (Character _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    _char.TakeAttack(Boss, Damage, AttackType.Magic, true);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "fam_exp_special").Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used AquaRing dealing some amount of Magic Damage to everyone!");
                }
            }
        }
        public void Blizzard(Mob Boss, ushort Time, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                Buff B = new Buff();
                B.Eff = Features.SkillsClass.ExtraEffect.IceBlock;
                B.Lasts = Time;
                B.Value = Time;
                B.Started = DateTime.UtcNow;
                B.StEff = Game.StatusEffectEn.IceBlock;
                _char.TimeBuff = B.Lasts;
                _char.AddBuff(B);
                _char.MyClient.LocalMessage(2000, Boss.Name + " used Blizzard freezing everyone for " + Time + " seconds!");
            }
        }

        /// <summary>
        /// Skills for Leaf Type Boss
        /// </summary>
        /// <param name="Boss"></param>
        /// <param name="Damage"></param>
        /// <param name="PlayerTargets"></param>
        public void Aromatherapy(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    _char.TakeAttack(Boss, Damage, AttackType.Magic, true);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "zf2-e215").Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Aromatherapy dealing some amount of Magic Damage to everyone!");
                }
            }
        }
        public void Grasscutter(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    _char.TakeAttack(Boss, Damage);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "swordcaromstart02").Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Grass Cutter dealing some Physical Damage to everyone!");
                }
            }
        }
        public void Cataclysm(Mob Boss, double Percentage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    double _pct = Percentage / 100;
                    uint Damage = Convert.ToUInt32(_char.CurHP * _pct);
                    _char.TakeAttack(Boss, Damage);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "ScapegoatSwitchOpen").Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Cataclysm reducing everyone's HP by 5%!");
                }
            }
        }
        public void PoisonousFog(Mob Boss, uint Damage, List<GameClient> PlayerTargets)  
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    _char.TakeAttack(Boss, Damage, AttackType.Magic, true);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "poisonparoxysm05").Get);
                    Features.Poison.PoisonCharacter(_char.EntityID, _char.EntityID);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Poisonous Fog dealing a considerable amount of Magic Damage and poisoning everyone!");
                }
            }
        }
        public void SolarBeam(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    if (_char.Job != 135 && !_char.Flying)
                    {
                        _char.TakeAttack(Boss, Damage, AttackType.Magic, true);
                        World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "accession1").Get);
                        _char.MyClient.LocalMessage(2000, Boss.Name + " used Solar Beam dealing a considerable amount of Magic Damage!");
                    }
                }
            }
            World.Action(Boss, Packets.ShakeScreen(Boss.EntityID).Get);
        }

        /// <summary>
        /// Skills for Electric Type Boss
        /// </summary>
        /// <param name="Boss"></param>
        /// <param name="Damage"></param>
        /// <param name="PlayerTargets"></param>
        public void SpeedLightning(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            World.Action(Boss, (Packets.StringPacket(Boss.EntityID, StringType.Effect, "light04")).Get);
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 5001;
            S.Info.Level = 0;
            S.Info.Damage = Damage;
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    if (_char.Job != 135 && !_char.Flying)
                    {
                        _char.TakeAttack(Damage, S);
                        _char.MyClient.LocalMessage(2000, Boss.Name + " used SpeedLightning dealing some amount of Magic Damage to everyone!");
                    }
                }
            }
        }
        public void Thunderbolt(Mob Boss, uint Damage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    _char.TakeAttack(Boss, Damage, AttackType.Magic, true);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "break_accept").Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Thunderbolt dealing some amount of Magic Damage to everyone!");
                }
            }
        }
        public void Discharge(Mob Boss, double Percentage, List<GameClient> PlayerTargets)
        {
            foreach (GameClient _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    double _pct = Percentage / 100;
                    uint Damage = Convert.ToUInt32(_char.CurHP * _pct);
                    _char.TakeAttack(Boss, Damage);
                    World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "break_start").Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used Discharge reducing everyone's HP by " + Percentage + "% !");
                }
            }
        }
        public void BoltStrike(Mob Boss, uint Damage, List<Character> PlayerTargets)
        {
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 6004;
            S.Info.Level = 0;
            S.Info.Damage = Damage;
            foreach (Character _char in PlayerTargets)
            {
                if (_char != null && _char.MyClient != null)
                {
                    if (!(_char.Job == 135 && _char.CanBeMeeledByMobs) && !_char.Flying)
                    {
                        _char.TakeAttack(Boss, S.Info.Damage, AttackType.Melee, true);
                        World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "attach_discharge05").Get);
                        World.Action(_char, Packets.StringPacket(_char.EntityID, StringType.Effect, "attach_accept05").Get);
                        _char.MyClient.LocalMessage(2000, Boss.Name + " used BoltStrike dealing some amount of Physical Damage to everyone!");
                    }
                }
            }
        }
        public void ThunderWave(Mob Boss, List<Character> PlayerTargets)
        {
            World.Action(Boss, (Packets.StringPacket(Boss.EntityID, StringType.Effect, "ride_screen")).Get);
            foreach (Character _char in PlayerTargets)
            {
                if (!_char.Flying)
                {
                    byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(Boss.Loc.X, Boss.Loc.Y, _char.Loc.X, _char.Loc.Y) / 45 % 8)) - 1 % 8);
                    byte Direction = (byte)((int)ToDir % 8);
                    if (Direction == 0)//sw
                        _char.Loc.Y += 5;
                    else if (Direction == 2)//nw
                        _char.Loc.X -= 5;
                    else if (Direction == 4)//ne
                        _char.Loc.Y -= 5;
                    else if (Direction == 6)//se
                        _char.Loc.X += 5;
                    else if (Direction == 1)//w
                    {
                        _char.Loc.X -= 5;
                        _char.Loc.Y += 5;
                    }
                    else if (Direction == 3)//n
                    {
                        _char.Loc.X -= 5;
                        _char.Loc.Y -= 5;
                    }
                    else if (Direction == 5)//e
                    {
                        _char.Loc.X += 5;
                        _char.Loc.Y -= 5;
                    }
                    else if (Direction == 7)//s
                    {
                        _char.Loc.X += 5;
                        _char.Loc.Y += 5;
                    }
                    World.Action(_char, Packets.GeneralData(_char.EntityID, 0, _char.Loc.X, _char.Loc.Y, 0x9c).Get);
                    _char.MyClient.LocalMessage(2000, Boss.Name + " used ThunderWave knocking everyone back!");
                }
            }
        }

        /// <summary>
        /// Base skills for all bosses
        /// </summary>
        /// <param name="Boss"></param>
        /// <param name="Damage"></param>
        /// <param name="PlayerTargets"></param>
        public void Penetration(double Percentage, List<Character> PlayerTargets)
        {
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 1290;
            S.Info.Level = 9;
            foreach (Character _char in PlayerTargets)
            {
                if (!_char.Flying)
                {
                    double _pct = Percentage / 100;
                    S.Info.Damage = Convert.ToUInt32(_char.CurHP * _pct);
                    _char.TakeAttack(S.Info.Damage, S);
                }
            }
        }
        public void Penetration(double Percentage, Character C)
        {
            Features.SkillsClass.SkillUse S = new Features.SkillsClass.SkillUse();
            S.Info.ID = 1290;
            S.Info.Level = 9;
            double _pct = Percentage / 100;
            S.Info.Damage = Convert.ToUInt32(C.CurHP * _pct);
            C.TakeAttack(S.Info.Damage, S);
        }
    }
}