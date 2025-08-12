using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.ServerSockets;
using System.Collections.Generic;

 
namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public class CallPet
    {
        public static void Execute(GameClient user, InteractQuery Attack, Packet stream, Dictionary<ushort, MagicType.Magic> DBSpells)
        {
            if (!CanUseSpell.Verified(Attack, user, DBSpells, out var ClientSpell, out var _))
                return;
            switch (ClientSpell.ID)
            {
                case 4000:
                case 4010:
                case 4020:
                case 4050:
                case 4060:
                case 4070:
                    if (user.ProjectManager)
                    {
                        new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        Server.AddMapMonster(stream, user.Map, 3644, Attack.X, Attack.Y, 3, 3, 1, user.Player.DynamicID, true, MsgItemPacket.EffectMonsters.EarthquakeLeftRight);
                    }
                    break;
            }
        }
    }
}
