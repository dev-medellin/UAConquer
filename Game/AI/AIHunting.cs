using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;
using System.Linq;


namespace TheChosenProject.Game.ConquerStructures.AI
{
    public class AIHunting
    {
        private static MonsterRole IMonster(GameClient client)
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
        private static bool Validated(MonsterRole target, GameClient client)
        {
            try
            {
                if (target == null) return false;
                if (target.Taken_Selected == true) return false;
                if (target.Map != client.Player.Map) return false;
                if (target == client.MobTarget) return false;
                if (!target.Alive) return false;
                if ((target.Family.Settings & MonsterSettings.Guard) == MonsterSettings.Guard) return false;
                return true;
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);

                return false;
            }
        }
        public static unsafe void StartAsync(GameClient client)
        {
            switch (client.AIStatus)
            {
                case AIEnum.AIStatus.Idle:
                    {
                        var obj = IMonster(client);
                        if (obj != null && obj.Taken_Selected == false)
                        {
                            client.MobTarget = obj;
                            obj.Taken_Selected = true;
                            client.AIStatus = AIEnum.AIStatus.Jumping;
                        }
                        break;
                    }
                case AIEnum.AIStatus.Jumping:
                    if (client.Player.Position.Distance(client.MobTarget.Position) > AIStructures.GetAttackDistance(client))
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
                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;
                                stream = rec.GetStream();
                                client.Player.View.SendView(stream.InterActionWalk(&inter), true);
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
                                Packet stream3;
                                stream3 = recycledPacket.GetStream();
                                interactQuery = default(InteractQuery);
                                interactQuery.AtkType = MsgAttackPacket.AttackID.Physical;
                                interactQuery.X = client.MobTarget.X;
                                interactQuery.Y = client.MobTarget.Y;
                                interactQuery.UID = client.Player.UID;
                                interactQuery.OpponentUID = client.MobTarget.UID;
                                interactQuery.Damage = (int)DmgObj.Damage;
                                InteractQuery action;
                                action = interactQuery;
                                client.Player.View.SendView(stream3.InteractionCreate(&action), true);
                                if (client.MobTarget.HitPoints <= (int)DmgObj.Damage)
                                {
                                    client.Map.SetMonsterOnTile(client.MobTarget.X, client.MobTarget.Y, false);
                                    client.MobTarget.Dead(stream3, client, client.Player.UID, client.Map);
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
                            Packet stream2;
                            stream2 = recycledPacket2.GetStream();
                            MsgAttackPacket.ProcescMagic(client, stream2, AttackPaket, true);
                        }
                        if (!client.MobTarget.Alive)
                            client.AIStatus = AIEnum.AIStatus.Idle;
                        else
                            client.AIStatus = AIEnum.AIStatus.Jumping;
                        break;
                    }
            }
        }
    }
}
