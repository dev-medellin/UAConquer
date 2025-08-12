using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;
using System.Linq;

 
namespace TheChosenProject.Game.ConquerStructures.MsgAutoHunting
{
    public class AutoHunting
    {
        private static MonsterRole IMonster(Client.GameClient client)
        {
            return (MonsterRole)(from p in client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster)
                                 where Monster(p as MonsterRole, client)
                                 select p into a
                                 orderby client.Player.Position.Distance((a as MonsterRole).Position)
                                 descending
                                 select a).LastOrDefault();
        }

        private static bool Monster(MonsterRole target, Client.GameClient client)
        {
            try
            {
                if (target == null)
                    return false;
                if (target.Map != client.Player.Map)
                    return false;
                if (target == client.MobTarget)
                    return false;
                if (!target.Alive)
                    return false;
                if ((target.Family.Settings & MonsterSettings.Guard) == MonsterSettings.Guard)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public unsafe static void HuntingAsync(GameClient client, Time32 Timer)
        {
            switch (client.AIStatus)
            {
                case AIEnum.AIStatus.Idle:
                    if (client.Player.Alive)
                    {
                        MonsterRole obj;
                        obj = IMonster(client);
                        if (obj != null)
                        {
                            MonsterRole target;
                            target = obj;
                            client.MobTarget = target;
                            client.AIStatus = AIEnum.AIStatus.Jumping;
                        }
                    }
                    else
                    {
                        if (!(Timer > client.Player.DeadStamp.AddSeconds(20)))
                            break;
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;
                            stream = rec.GetStream();
                            client.Player.Revive(stream);
                            if (client.Player.VipLevel == 6)
                                client.SendWhisper($"You were killed by {client.Player.KillerHunting}. Since you have VIP6, you got auto-revived and continued auto-hunting.", "Auto-hunting", client.Player.Name, 2991003u);
                            else
                                client.SendWhisper($"Auto-hunting was interrupted, since you were killed by {client.Player.KillerHunting}. (You`ll get auto-revived if you have VIP6.)", "Auto-hunting", client.Player.Name, 2991003u);
                            client.AIStatus = AIEnum.AIStatus.Idle;
                            break;
                        }
                    }
                    break;
                case AIEnum.AIStatus.Jumping:
                    if (!client.Player.Alive)
                    {
                        client.AIStatus = AIEnum.AIStatus.Idle;
                        break;
                    }
                    int GetAttackDistance = 3;

                    if (client.Player.Class >= 40 && client.Player.Class <= 45)
                    {

                        if (client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out MsgGameItem msgGameItem))
                        {
                            if (ItemType.IsBow(msgGameItem.ITEM_ID))
                            {
                                GetAttackDistance = 5;
                            }
                        }
                    }
                    if (client.Player.Class >= 100)
                    {
                        GetAttackDistance = 5;
                    }
                    else if (client.Player.Position.Distance(client.MobTarget.Position) > AIStructures.GetAttackDistance(client))
                    {
                        if (client.pathfinder != null)
                        {
                            int point_x;
                            point_x = client.pathfinder[client._pathfinder_length].X;
                            int point_y;
                            point_y = client.pathfinder[client._pathfinder_length].Y;
                            InterActionWalk interActionWalk;
                            interActionWalk = default(InterActionWalk);
                            interActionWalk.Mode = MsgInterAction.Action.Jump;
                            interActionWalk.X = (ushort)point_x;
                            interActionWalk.Y = (ushort)point_y;
                            interActionWalk.UID = client.Player.UID;
                            interActionWalk.OponentUID = 0u;
                            InterActionWalk inter;
                            inter = interActionWalk;
                            using (RecycledPacket recycledPacket3 = new RecycledPacket())
                            {
                                Packet stream2;
                                stream2 = recycledPacket3.GetStream();
                                client.Player.View.SendView(stream2.InterActionWalk(&inter), true);
                            }
                            client.Player.Angle = Core.GetAngle(client.Player.X, client.Player.Y, (ushort)point_x, (ushort)point_y);
                            client.Player.Action = Flags.ConquerAction.Jump;
                            client.Map.View.MoveTo((IMapObj)client.Player, (int)(ushort)point_x, (int)(ushort)point_y);
                            client.Player.X = (ushort)point_x;
                            client.Player.Y = (ushort)point_y;
                            client.Player.View.Role();
                            if (++client._pathfinder_length == client.pathfinder.Length)
                                client.pathfinder = null;
                        }
                        else if (!client.Map.Pathfinder.Search(client.Player.Position, client.MobTarget.Position, AIStructures.GetJumpDistance(client.AIType), client.Map, out client.pathfinder))
                        {
                            client.AIStatus = AIEnum.AIStatus.Idle;
                        }
                        else
                        {
                            client._pathfinder_length = 0;
                        }
                    }
                    else if (client.Player.Position.Distance(client.MobTarget.Position) <= AIStructures.GetAttackDistance(client))
                    {
                        client.AIStatus = AIEnum.AIStatus.Attacking;
                    }
                    break;
                case AIEnum.AIStatus.Attacking:
                    {
                        Flags.SpellID SpellID;
                        SpellID = AIStructures.WeaponTypeValid(client, client.MobTarget.Position);
                        InteractQuery interactQuery;
                        if (SpellID == Flags.SpellID.Physical)
                        {
                            new MsgSpellAnimation.SpellObj();
                            Physical.OnMonster(client.Player, client.MobTarget, null, out var DmgObj, 0);
                            using (RecycledPacket recycledPacket = new RecycledPacket())
                            {
                                Packet stream4;
                                stream4 = recycledPacket.GetStream();
                                interactQuery = default(InteractQuery);
                                interactQuery.AtkType = MsgAttackPacket.AttackID.Physical;
                                interactQuery.X = client.MobTarget.X;
                                interactQuery.Y = client.MobTarget.Y;
                                interactQuery.UID = client.Player.UID;
                                interactQuery.OpponentUID = client.MobTarget.UID;
                                interactQuery.Damage = (int)DmgObj.Damage;
                                InteractQuery action;
                                action = interactQuery;
                                client.Player.View.SendView(stream4.InteractionCreate(&action), true);
                                if (client.MobTarget.HitPoints <= (int)DmgObj.Damage)
                                {
                                    client.Map.SetMonsterOnTile(client.MobTarget.X, client.MobTarget.Y, false);
                                    client.MobTarget.Dead(stream4, client, client.Player.UID, client.Map);
                                    client.AIStatus = AIEnum.AIStatus.Idle;
                                }
                                else
                                {
                                    client.MobTarget.HitPoints -= DmgObj.Damage;
                                    client.AIStatus = AIEnum.AIStatus.Jumping;
                                }
                                break;
                            }
                        }
                        MsgAttackPacket.AttackID _type;
                        _type = MsgAttackPacket.AttackID.Magic;
                        if (SpellID == Flags.SpellID.ScatterFire)
                            _type = MsgAttackPacket.AttackID.Archer;
                        if (SpellID == Flags.SpellID.DragonWhirl)
                            _type = MsgAttackPacket.AttackID.InMoveSpell;
                        interactQuery = default(InteractQuery);
                        interactQuery.OpponentUID = client.MobTarget.UID;
                        interactQuery.UID = client.Player.UID;
                        interactQuery.X = client.MobTarget.X;
                        interactQuery.Y = client.MobTarget.Y;
                        interactQuery.SpellID = (ushort)SpellID;
                        InteractQuery AttackPaket;
                        AttackPaket = interactQuery;
                        if (MagicType.RandomSpells.Contains(SpellID))
                        {
                            client.Player.RandomSpell = AttackPaket.SpellID;
                            AttackPaket.OpponentUID = client.MobTarget.UID;
                        }
                        AttackPaket.AtkType = _type;
                        using (RecycledPacket recycledPacket2 = new RecycledPacket())
                        {
                            Packet stream3;
                            stream3 = recycledPacket2.GetStream();
                            MsgAttackPacket.ProcescMagic(client, stream3, AttackPaket);
                        }
                        if (!client.MobTarget.Alive)
                        {
                            client.AIStatus = AIEnum.AIStatus.Idle;
                            if (client.OnAutoAttack)
                                client.OnAutoAttack = false;
                        }
                        else
                        {
                            client.AIStatus = AIEnum.AIStatus.Jumping;
                            if (client.OnAutoAttack)
                                client.OnAutoAttack = false;
                        }
                        break;
                    }
            }
        }
    }
}
