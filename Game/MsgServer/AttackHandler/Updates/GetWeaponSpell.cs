using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;

using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler.Updates
{
    public class GetWeaponSpell
    {
        public static void CheckExtraEffects(GameClient client, Packet stream)
        {
            //if (client.Equipment.RingEffect != 0 && Base.Success(20.0) && !client.Player.ContainFlag(MsgUpdate.Flags.Stigma))
            //{
            //    MsgSpellAnimation MsgSpell2;
            //    MsgSpell2 = new MsgSpellAnimation(client.Player.UID, 0, client.Player.X, client.Player.Y, 1095, 4, 0);
            //    client.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, 20, true);
            //    MsgSpell2.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
            //    MsgSpell2.SetStream(stream);
            //    MsgSpell2.Send(client);
            //}//bahaa

            if ((client.Equipment.NecklaceEffect != 0 || client.Equipment.RightWeaponEffect == Flags.ItemEffect.Stigma) && Base.Success(20.0) && !client.Player.ContainFlag(MsgUpdate.Flags.Stigma))
            {
                MsgSpellAnimation MsgSpell2;
                MsgSpell2 = new MsgSpellAnimation(client.Player.UID, 0, client.Player.X, client.Player.Y, 1095, 3, 0);
                client.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, 20, true);
                MsgSpell2.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                MsgSpell2.SetStream(stream);
                MsgSpell2.Send(client);
            }
            //if ((client.Equipment.NecklaceEffect != 0 || client.Equipment.RightWeaponEffect == Flags.ItemEffect.Shield) && Base.Success(20.0) && !client.Player.ContainFlag(MsgUpdate.Flags.MagicShield))
            //{
            //    MsgSpellAnimation MsgSpell;
            //    MsgSpell = new MsgSpellAnimation(client.Player.UID, 0, client.Player.X, client.Player.Y, 1020, 4, 0);
            //    client.Player.AddSpellFlag(MsgUpdate.Flags.MagicShield, 15, true);
            //    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
            //    MsgSpell.SetStream(stream);
            //    MsgSpell.Send(client);
            //}
        }

        public static bool Check(InteractQuery Attack, Packet stream, GameClient client, IMapObj Target)
        {
            if (!client.Player.ActivePassive)
                return false;
            if (Base.Success(50) && client.Equipment.RightWeapon != 0) //default 35.0
            {
                if ((client.Equipment.RightWeaponEffect != 0 || client.Equipment.LeftWeaponEffect != 0) && Target.ObjType != MapObjectType.SobNpc /*&& Base.Success(50)*/ && !client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                {
                    //client.Player.AttackStamp = default(Time32);
                    InteractQuery interactQuery;
                    interactQuery = default(InteractQuery);
                    interactQuery.OpponentUID = Attack.OpponentUID;
                    interactQuery.UID = Attack.UID;
                    interactQuery.X = Target.X;
                    interactQuery.Y = Target.Y;
                    InteractQuery AttackPaket3;
                    AttackPaket3 = interactQuery;
                    if (client.Equipment.RightWeaponEffect == Flags.ItemEffect.Poison || client.Equipment.LeftWeaponEffect == Flags.ItemEffect.Poison)
                        AttackPaket3.SpellID = 3306;
                    else if (client.Equipment.RightWeaponEffect == Flags.ItemEffect.MP)
                    {
                        AttackPaket3.SpellID = 1175;
                    }
                    //else if (client.Equipment.LeftWeaponEffect == Flags.ItemEffect.Stigma)
                    //{
                    //    AttackPaket3.SpellID = 1095;
                    //}
                    //else if (client.Equipment.LeftWeaponEffect == Flags.ItemEffect.HP)
                    //{
                    //    AttackPaket3.SpellID = 1190;
                    //}
                    client.Player.RandomSpell = AttackPaket3.SpellID;
                    AttackPaket3.AtkType = MsgAttackPacket.AttackID.Magic;
                    MsgAttackPacket.ProcescMagic(client, stream, AttackPaket3);
                    return false; //return true;
                }
                uint rightWeapon;
                rightWeapon = client.Equipment.RightWeapon;
                ushort wep1subyte;
                wep1subyte = (ushort)(rightWeapon / 1000);
                ushort wep2subyte;
                wep2subyte = 0;
                bool doWep1Spell;
                doWep1Spell = false;
                bool doWep2Spell;
                doWep2Spell = false;
                ushort wep1spellid;
                wep1spellid = 0;
                ushort wep2spellid;
                wep2spellid = 0;
                if (wep1subyte == 421)
                    wep1subyte = (ushort)(wep1subyte - 1);
                if (Server.WeaponSpells.ContainsKey(wep1subyte))
                    wep1spellid = Server.WeaponSpells[wep1subyte];
                if (client.MySpells.ClientSpells.ContainsKey(wep1spellid))
                    doWep1Spell = Base.Success(50.0);
                if (!doWep1Spell)
                {
                    if (client.Equipment.LeftWeapon != 0)
                    {
                        uint leftWeapon;
                        leftWeapon = client.Equipment.LeftWeapon;
                        wep2subyte = (ushort)(leftWeapon / 1000);
                        if (Server.WeaponSpells.ContainsKey(wep2subyte))
                            wep2spellid = Server.WeaponSpells[wep2subyte];
                        if (client.MySpells.ClientSpells.ContainsKey(wep2spellid))
                            doWep2Spell = true;
                        if (doWep2Spell)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey(wep2spellid))
                                return false;
                            if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike) && Target.ObjType == MapObjectType.Monster)
                                client.Shift(Target.X, Target.Y, stream);
                            if (Target.ObjType != MapObjectType.Monster)
                                MsgAttackPacket.CreateAutoAtack(Attack, client);
                            else
                            {
                                MonsterRole attacked2;
                                attacked2 = Target as MonsterRole;
                                //if (attacked2.Family.ID != 4145)
                                    MsgAttackPacket.CreateAutoAtack(Attack, client);
                            }
                            InteractQuery interactQuery;
                            interactQuery = default(InteractQuery);
                            interactQuery.OpponentUID = Attack.OpponentUID;
                            interactQuery.UID = Attack.UID;
                            interactQuery.X = Target.X;
                            interactQuery.Y = Target.Y;
                            interactQuery.SpellID = wep2spellid;
                            interactQuery.AtkType = MsgAttackPacket.AttackID.Magic;
                            InteractQuery AttackPaket2;
                            AttackPaket2 = interactQuery;
                            client.Player.RandomSpell = wep2spellid;
                            MsgAttackPacket.ProcescMagic(client, stream, AttackPaket2, true);
                            return false; //return true;
                        }
                        doWep1Spell = true;
                    }
                    else
                        doWep1Spell = true;
                }
                if (doWep1Spell)
                {
                    if (!client.MySpells.ClientSpells.ContainsKey(wep1spellid))
                        return false;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                        client.Shift(Target.X, Target.Y, stream);
                    if (Target.ObjType != MapObjectType.Monster)
                        MsgAttackPacket.CreateAutoAtack(Attack, client);
                    else
                    {
                        MonsterRole attacked;
                        attacked = Target as MonsterRole;
                        //if (attacked.Family.ID != 4145)
                            MsgAttackPacket.CreateAutoAtack(Attack, client);
                    }
                    InteractQuery interactQuery;
                    interactQuery = default(InteractQuery);
                    interactQuery.OpponentUID = Attack.OpponentUID;
                    interactQuery.UID = Attack.UID;
                    interactQuery.X = Target.X;
                    interactQuery.Y = Target.Y;
                    interactQuery.SpellID = wep1spellid;
                    InteractQuery AttackPaket;
                    AttackPaket = interactQuery;
                    client.Player.RandomSpell = wep1spellid;
                    AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                    MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                    return false; //return true;
                }
            }
            return false;
        }
    }
}
