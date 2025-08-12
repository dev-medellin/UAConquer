using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanAttackMonster
    {
        public static bool Verified(Client.GameClient client, MsgMonster.MonsterRole attacked
            , Database.MagicType.Magic DBSpell)
        {
            foreach (var item in client.Equipment.CurentEquip)
            {
                if (item.Position == (ushort)Role.Flags.ConquerItem.RightWeapon
                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
                    || item.Position == (ushort)Role.Flags.ConquerItem.Ring
                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRing
                    || item.Position == (ushort)Role.Flags.ConquerItem.Fan
                    || item.Position == (ushort)Role.Flags.ConquerItem.RidingCrop)
                {
                    if (item.Durability <= 0)
                        return false;
                }
            }

            if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Reviver) == MsgMonster.MonsterSettings.Reviver)
                return false;
            if (client.Player.OnTransform)
                return false;
            if (!attacked.Alive)
                return false;
            if (client.Pet != null && client.Pet.monster.UID == attacked.UID) return false;

            if(attacked.Family.ID == 21060 && client.Player.Reborn >= 1)
            {
                client.SendSysMesage("You can't attack Divine[EXP] because you're already Rebirth. Go outside and explore the world!");
                return false;
            }

            if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard || attacked.Family.Name.Contains("Guard"))
            {
                if (client.Player.PkMode != Role.Flags.PKMode.PK)
                    return false;
                else
                {
                    client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 30, true);
                }
            }
            //if (DBSpell != null && attacked.Family.ID == 4145 && !Server.RebornInfo.StaticSpells.Contains(DBSpell.ID) && DBSpell.ID != 1045 && DBSpell.ID != 1046 && DBSpell.ID != 11000 && DBSpell.ID != 11005)
            //{
            //    client.SendSysMesage("You can`t use any magic spells on the TwinCity Boss bitch!");
            //    return false;
            //}
            return true;

        }
    }
}
