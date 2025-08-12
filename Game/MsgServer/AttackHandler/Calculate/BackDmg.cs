using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler.Calculate
{
    public class BackDmg
    {
        public unsafe static bool Calculate(TheChosenProject.Role.Player player, TheChosenProject.Role.Player target, MagicType.Magic DBSpell, uint Damage, out MsgSpellAnimation.SpellObj SpellObj)
        {
            if (!player.Alive)
            {
                SpellObj = null;
                return false;
            }
            if (Base.Success(10.0))
            {
                if (target.ActivateCounterKill)
                {
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        if (target.Owner.MySpells.ClientSpells.TryGetValue(6003, out var ClientSpell) && Server.Magic.TryGetValue(6003, out var DBSpells) && DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                        {
                            SpellObj = new MsgSpellAnimation.SpellObj
                            {
                                Damage = 0u
                            };
                            MsgSpellAnimation.SpellObj DmgObj2;
                            DmgObj2 = new MsgSpellAnimation.SpellObj();
                            Physical.OnPlayer(target, player, DBSpell, out DmgObj2, true);
                            DmgObj2.Damage = (uint)((double)DmgObj2.Damage * 0.75);
                            if (ClientSpell.Level < DBSpells.Count - 1)
                            {
                                ClientSpell.Experience += (int)(DmgObj2.Damage / ServerKernel.SPELL_RATE);
                                if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                {
                                    ClientSpell.Level++;
                                    ClientSpell.Experience = 0;
                                }
                                target.Send(stream2.SpellCreate(ClientSpell));
                                target.Owner.MySpells.ClientSpells[ClientSpell.ID] = ClientSpell;
                            }
                            InteractQuery interactQuery;
                            interactQuery = default(InteractQuery);
                            interactQuery.ResponseDamage = DmgObj2.Damage;
                            interactQuery.X = player.X;
                            interactQuery.Y = player.Y;
                            interactQuery.OpponentUID = player.UID;
                            interactQuery.UID = target.UID;
                            interactQuery.Effect = DmgObj2.Effect;
                            interactQuery.AtkType = MsgAttackPacket.AttackID.Scapegoat;
                            InteractQuery action;
                            action = interactQuery;
                            target.View.SendView(stream2.InteractionCreate(&action), true);
                            TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(DmgObj2, target.Owner, player);
                            return true;
                        }
                    }
                }
                if (target.ContainReflect)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        SpellObj = new MsgSpellAnimation.SpellObj
                        {
                            Damage = 0u,
                            UID = target.UID
                        };
                        MsgSpellAnimation.SpellObj DmgObj;
                        DmgObj = new MsgSpellAnimation.SpellObj
                        {
                            Damage = Damage / 2u,
                            UID = player.UID
                        };
                        InteractQuery interactQuery;
                        interactQuery = default(InteractQuery);
                        interactQuery.ResponseDamage = DmgObj.Damage;
                        interactQuery.Damage = (int)DmgObj.Damage;
                        interactQuery.AtkType = MsgAttackPacket.AttackID.Reflect;
                        interactQuery.X = player.X;
                        interactQuery.Y = player.Y;
                        interactQuery.OpponentUID = player.UID;
                        interactQuery.UID = target.UID;
                        interactQuery.Effect = DmgObj.Effect;
                        InteractQuery action2;
                        action2 = interactQuery;
                        target.View.SendView(stream.InteractionCreate(&action2), true);
                        TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(DmgObj, target.Owner, player);
                    }
                    return true;
                }
            }
            SpellObj = null;
            return false;
        }
    }
}
