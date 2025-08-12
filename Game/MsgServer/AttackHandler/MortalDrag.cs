using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack;
using TheChosenProject.Game.MsgServer.AttackHandler.Updates;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public class MortalDrag
    {
        public static void Execute(GameClient user, InteractQuery Attack, Packet stream, Dictionary<ushort, MagicType.Magic> DBSpells)
        {
            if (!CanUseSpell.Verified(Attack, user, DBSpells, out var ClientSpell, out var DBSpell))
                return;
            ushort iD;
            iD = ClientSpell.ID;
            if (iD != 11180)
                return;
            MsgSpellAnimation MsgSpell;
            MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
            uint Experience;
            Experience = 0;
            if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target, MapObjectType.Monster))
            {
                MonsterRole attacked2;
                attacked2 = target as MonsterRole;
                if (Core.GetDistance(user.Player.X, user.Player.Y, attacked2.X, attacked2.Y) < DBSpell.Range && CanAttackMonster.Verified(user, attacked2, DBSpell) && attacked2.UpdateMapCoords(user.Player.X, user.Player.Y, user.Map))
                {
                    user.Map.View.MoveTo((IMapObj)attacked2, (int)user.Player.X, (int)user.Player.Y);
                    attacked2.X = user.Player.X;
                    attacked2.Y = user.Player.Y;
                    attacked2.UpdateMonsterView(null, stream);
                    Physical.OnMonster(user.Player, attacked2, DBSpell, out var AnimationObj2, 0);
                    AnimationObj2.Damage = Base.CalculateSoul(AnimationObj2.Damage, ClientSpell.UseSpellSoul);
                    Experience += Monster.Execute(stream, AnimationObj2, user, attacked2);
                    MsgSpell.Targets.Enqueue(AnimationObj2);
                }
            }
            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, MapObjectType.Player))
            {
                TheChosenProject.Role.Player attacked;
                attacked = target as TheChosenProject.Role.Player;
                if (Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range && CanAttackPlayer.Verified(user, attacked, DBSpell))
                {
                    attacked.Owner.Map.View.MoveTo((IMapObj)attacked, (int)user.Player.X, (int)user.Player.Y);
                    attacked.X = user.Player.X;
                    attacked.Y = user.Player.Y;
                    attacked.View.Role();
                    Physical.OnPlayer(user.Player, attacked, DBSpell, out var AnimationObj);
                    AnimationObj.Damage = Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                    TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                    MsgSpell.Targets.Enqueue(AnimationObj);
                }
            }
            IncreaseExperience.Up(stream, user, Experience);
            UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
            MsgSpell.SetStream(stream);
            MsgSpell.Send(user);
        }
    }
}
