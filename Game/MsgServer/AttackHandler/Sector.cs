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
    public class Sector
    {
        public static void Execute(GameClient user, InteractQuery Attack, Packet stream, Dictionary<ushort, MagicType.Magic> DBSpells)
        {
            if (!CanUseSpell.Verified(Attack, user, DBSpells, out var ClientSpell, out var DBSpell))
                return;
            ushort iD;
            iD = ClientSpell.ID;
            if (iD == 8001)
            {
                ushort Sector;
                Sector = DBSpell.Sector;
                byte Range;
                Range = DBSpell.Range == 9 ? (byte)(DBSpell.Range + 6) : DBSpell.Range;
                MsgSpellAnimation MsgSpell2;
                MsgSpell2 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                TheChosenProject.Game.MsgServer.AttackHandler.Algoritms.Sector SpellSector2;
                SpellSector2 = new TheChosenProject.Game.MsgServer.AttackHandler.Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                SpellSector2.Arrange(Range, Sector);
                uint Experience2;
                Experience2 = 0u;
                foreach (IMapObj target2 in user.Player.View.Roles(MapObjectType.Monster))
                {
                    MonsterRole attacked6;
                    attacked6 = target2 as MonsterRole;
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target2.X, target2.Y) < Range && CanAttackMonster.Verified(user, attacked6, DBSpell))
                    {
                        
                        TheChosenProject.Game.MsgServer.AttackHandler.Calculate.Range.OnMonster(user.Player, attacked6, DBSpell, out var AnimationObj6, 0);
                        AnimationObj6.Damage = Base.CalculateSoul(AnimationObj6.Damage, ClientSpell.UseSpellSoul);
                        Experience2 += Monster.Execute(stream, AnimationObj6, user, attacked6);
                        MsgSpell2.Targets.Enqueue(AnimationObj6);
                    }
                }
                foreach (IMapObj targer4 in user.Player.View.Roles(MapObjectType.Player))
                {
                    TheChosenProject.Role.Player attacked5;
                    attacked5 = targer4 as TheChosenProject.Role.Player;
                    if (SpellSector2.Inside(attacked5.X, attacked5.Y) && CanAttackPlayer.Verified(user, attacked5, DBSpell))
                    {
                        TheChosenProject.Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(user.Player, attacked5, DBSpell, out var AnimationObj5);
                        AnimationObj5.Damage = Base.CalculateSoul(AnimationObj5.Damage, ClientSpell.UseSpellSoul);
                        TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj5, user, attacked5);
                        AnimationObj5.Hit = 0u;
                        MsgSpell2.Targets.Enqueue(AnimationObj5);
                    }
                }
                foreach (IMapObj targer3 in user.Player.View.Roles(MapObjectType.SobNpc))
                {
                    SobNpc attacked4;
                    attacked4 = targer3 as SobNpc;
                    if (SpellSector2.Inside(attacked4.X, attacked4.Y) && CanAttackNpc.Verified(user, attacked4, DBSpell))
                    {
                        TheChosenProject.Game.MsgServer.AttackHandler.Calculate.Range.OnNpcs(user.Player, attacked4, DBSpell, out var AnimationObj4);
                        AnimationObj4.Damage = Base.CalculateSoul(AnimationObj4.Damage, ClientSpell.UseSpellSoul);
                        Experience2 += Npc.Execute(stream, AnimationObj4, user, attacked4);
                        MsgSpell2.Targets.Enqueue(AnimationObj4);
                    }
                }
                IncreaseExperience.Up(stream, user, Experience2);
                UpdateSpell.CheckUpdate(stream, user, Attack, Experience2, DBSpells);
                MsgSpell2.SetStream(stream);
                MsgSpell2.Send(user);
                return;
            }
            MsgSpellAnimation MsgSpell;
            MsgSpell = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
            TheChosenProject.Game.MsgServer.AttackHandler.Algoritms.Sector SpellSector;
            SpellSector = new TheChosenProject.Game.MsgServer.AttackHandler.Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
            SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);
            uint Experience;
            Experience = 0u;
            foreach (IMapObj target in user.Player.View.Roles(MapObjectType.Monster))
            {
                MonsterRole attacked3;
                attacked3 = target as MonsterRole;
                if (SpellSector.Inside(attacked3.X, attacked3.Y) && Base.GetDistance(user.Player.X, user.Player.Y, attacked3.X, attacked3.Y) < 5 && CanAttackMonster.Verified(user, attacked3, DBSpell))
                {
                    Physical.OnMonster(user.Player, attacked3, DBSpell, out var AnimationObj3, 0);
                    AnimationObj3.Damage = Base.CalculateSoul(AnimationObj3.Damage, ClientSpell.UseSpellSoul);
                    Experience += Monster.Execute(stream, AnimationObj3, user, attacked3);
                    MsgSpell.Targets.Enqueue(AnimationObj3);
                }
            }
            foreach (IMapObj targer2 in user.Player.View.Roles(MapObjectType.Player))
            {
                TheChosenProject.Role.Player attacked2;
                attacked2 = targer2 as TheChosenProject.Role.Player;
                if (SpellSector.Inside(attacked2.X, attacked2.Y) && Base.GetDistance(user.Player.X, user.Player.Y, attacked2.X, attacked2.Y) < 5 && CanAttackPlayer.Verified(user, attacked2, DBSpell))
                {
                    Physical.OnPlayer(user.Player, attacked2, DBSpell, out var AnimationObj2);
                    AnimationObj2.Damage = Base.CalculateSoul(AnimationObj2.Damage, ClientSpell.UseSpellSoul);
                    TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj2, user, attacked2);
                    MsgSpell.Targets.Enqueue(AnimationObj2);
                }
            }
            foreach (IMapObj targer in user.Player.View.Roles(MapObjectType.SobNpc))
            {
                SobNpc attacked;
                attacked = targer as SobNpc;
                if (SpellSector.Inside(attacked.X, attacked.Y) && Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 5 && CanAttackNpc.Verified(user, attacked, DBSpell))
                {
                    Physical.OnNpcs(user.Player, attacked, DBSpell, out var AnimationObj);
                    AnimationObj.Damage = Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                    Experience += Npc.Execute(stream, AnimationObj, user, attacked);
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
