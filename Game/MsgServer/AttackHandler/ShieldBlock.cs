using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;

using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public class ShieldBlock
    {
        public static void Execute(GameClient user, InteractQuery Attack, Packet stream, Dictionary<ushort, MagicType.Magic> DBSpells)
        {
            if (CanUseSpell.Verified(Attack, user, DBSpells, out var ClientSpell, out var DBSpell))
            {
                MsgSpellAnimation MsgSpell;
                MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                user.Player.ShieldBlockDamage = (uint)DBSpell.Damage;
                user.Player.ShieldBlockEnd = DateTime.Now.AddSeconds(DBSpell.Duration);
                user.Player.AddSpellFlag(MsgUpdate.Flags.ShieldBlock, (int)DBSpell.Duration, true);
                user.Player.SendUpdate(stream, MsgUpdate.Flags.ShieldBlock, DBSpell.Duration, (uint)DBSpell.Damage, ClientSpell.Level, MsgUpdate.DataType.AzureShield, true);
                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);
                ClientSpell.Experience += (int)(10 * ServerKernel.EXP_RATE);
                if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience && ClientSpell.Level != 4)
                {
                    ClientSpell.Level++;
                    ClientSpell.Experience = 0;
                }
                user.Send(stream.SpellCreate(ClientSpell));
                user.MySpells.ClientSpells[ClientSpell.ID] = ClientSpell;
            }
        }
    }
}
