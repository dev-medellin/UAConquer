using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static TheChosenProject.Game.MsgServer.MsgAttackPacket;

namespace TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Monster
    {
        public static uint ExecutePet(ServerSockets.Packet stream, uint obj, Client.GameClient client, MsgMonster.MonsterRole monster, bool CounterKill = false)
        {
            if (monster.HitPoints <= obj)
            {
                client.Map.SetMonsterOnTile(monster.X, monster.Y, false);
                monster.Dead(stream, client, client.Player.UID, client.Map);
                if (monster.UID >= 700000)
                {
                    var array = Database.Server.GamePoll.Values.Where(c => c.Pet != null && c.Pet?.monster.UID == monster.UID).SingleOrDefault();
                    if (array != null)
                        array.Pet.DeAtach(stream);
                   
                }
            }
            else
            {
                monster.HitPoints -= obj;
                
                if (monster.Boss == 1)
                {
                    
                    Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, monster.UID, 2);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, monster.Family.MaxHealth);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, monster.HitPoints);
                    stream = Upd.GetArray(stream);
                    client.Player.View.SendView(stream, true);

                }
            }

            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) != MsgMonster.MonsterSettings.Guard)
            {
                if (obj > monster.Family.MaxHealth)
                    return (uint)AdjustExp(monster.Family.MaxHealth, client.Player.Level, monster.Level);
                else
                    return (uint)AdjustExp((int)obj, client.Player.Level, monster.Level);
            }
            else
                return 0;
        }

        public static uint Execute(ServerSockets.Packet stream, MsgSpellAnimation.SpellObj obj, Client.GameClient client, MsgMonster.MonsterRole monster, bool CounterKill = false)
        {
            if (monster.Map == 3845)
            {
                if (!monster.isTargetToSolomon)
                {
                    client.OnAutoAttack = false;
                    client.Map.SetMonsterOnTile(monster.X, monster.Y, false);
                    monster.Dead(stream, client, (uint)client?.Player.UID, client?.Map, CounterKill);
                    if (client?.Team != null)
                        client?.Team.ShareExperience(stream, client, monster);
                    return 0;
                }
            }

            if (client.Pet != null) client.Pet.Target = monster;

            if (monster.HitPoints <= obj.Damage)
            {

                client.Map.SetMonsterOnTile(monster.X, monster.Y, false);
                monster.Dead(stream, client, client.Player.UID, client.Map, CounterKill);
                var Pet = Database.Server.GamePoll.Values.Where(p => p != null && p.Pet != null && p.Pet.monster.UID == monster.UID).FirstOrDefault();
                if (Pet != null)
                {
                    Pet.Pet.DeAtach(stream);
                    if (!Program.FreePkMap.Contains(client.Player.Map))
                    client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 15, true);
                }
                if (client.Team != null)
                    client.Team.ShareExperience(stream, client, monster);
               
            }
            else
            {
                
                    monster.HitPoints -= obj.Damage;
                if (monster.UID >= 700000)
                {
                    if (!Program.FreePkMap.Contains(client.Player.Map))
                        client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 15, true);
                }
                if (monster.Boss == 1)
                {
                    Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, monster.UID, 2);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, monster.Family.MaxHealth);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, monster.HitPoints);
                    stream = Upd.GetArray(stream);
                    client.Player.View.SendView(stream, true);
                    monster.SendScores(stream);
                    monster.UpdateScores(client.Player, obj.Damage);
                }
            }

            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) != MsgMonster.MonsterSettings.Guard)
            {
                if (obj.Damage > monster.Family.MaxHealth)
                    return (uint)AdjustExp(monster.Family.MaxHealth, client.Player.Level, monster.Level);
                else
                    return (uint)AdjustExp((int)obj.Damage, client.Player.Level, monster.Level);
            }
            else
                return 0;
        }
        public static int AdjustExp(int nDamage, int nAtkLev, int nDefLev)
        {

            //   return nDamage;
            //if (nAtkLev > 120)
            //    nAtkLev = 120;
            int nExp = nDamage;
            int nNameType = Calculate.Base.GetNameType(nAtkLev, nDefLev);
            int Level = 120;
            if (nDefLev < 120)
                Level = nDefLev;


            if (nNameType == Calculate.Base.StatusConstants.NAME_GREEN)
            {
                Int32 DeltaLvl = nAtkLev - nDefLev;
                if (DeltaLvl >= 3 && DeltaLvl <= 5)
                    nExp = nExp * 70 / 100;
                else if (DeltaLvl > 5 && DeltaLvl <= 10)
                    nExp = nExp * 20 / 100;
                else if (DeltaLvl > 10 && DeltaLvl <= 20)
                    nExp = nExp * 10 / 100;
                else if (DeltaLvl > 20)
                    nExp = nExp * 5 / 100;
            }
            else if (nNameType == Calculate.Base.StatusConstants.NAME_RED)
            { nExp *= (int)1.5; }
            else if(nNameType == Calculate.Base.StatusConstants.NAME_BLACK)
            {
                Int32 DeltaLvl = nDefLev - Level;
                if (DeltaLvl >= -10 && DeltaLvl <= -5)
                    nExp *= (int)2.0;
                else if (DeltaLvl >= -20 && DeltaLvl < -10)
                    nExp *= (int)3.5;
                else if (DeltaLvl < -20)
                    nExp *= (int)5.0;
            }

            return Math.Max(0, (Int32)nExp);


            //int nExp = nDamage;

            //int nNameType = Calculate.Base.GetNameType(nAtkLev, nDefLev);
            //int nDeltaLev = nAtkLev - nDefLev;
            //if (nNameType == Calculate.Base.StatusConstants.NAME_GREEN)
            //{
            //    if (nDeltaLev >= 3 && nDeltaLev <= 5)
            //        nExp = nExp * 50 / 100;
            //    else if (nDeltaLev > 5 && nDeltaLev <= 10)
            //        nExp = nExp * 20 / 100;
            //    else if (nDeltaLev > 10 && nDeltaLev <= 20)
            //        nExp = nExp * 10 / 100;
            //    else if (nDeltaLev > 20)
            //        nExp = nExp * 5 / 100;
            //}
            //else if (nNameType == Calculate.Base.StatusConstants.NAME_RED)
            //{
            //    nExp = (int)(nExp * 1.3);
            //}
            //else if (nNameType == Calculate.Base.StatusConstants.NAME_BLACK)
            //{
            //    if (nDeltaLev >= -10 && nDeltaLev < -5)
            //        nExp = (int)(nExp * 1.5);
            //    else if (nDeltaLev >= -20 && nDeltaLev < -10)
            //        nExp = (int)(nExp * 1.8);
            //    else if (nDeltaLev < -20)
            //        nExp = (int)(nExp * 2.3);
            //}

            //return Math.Max(10, nExp);
        }
    }
}
