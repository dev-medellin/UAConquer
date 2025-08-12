using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;
using System.Linq;


namespace TheChosenProject.Game.ConquerStructures.AI
{
    public class AITrainingGroup
    {
        private static IMapObj ISob(GameClient client)
        {
            return (from a in client.Map.View.Roles(MapObjectType.SobNpc, client.Player.X, client.Player.Y, (IMapObj p) => Validated(p, client))
				orderby client.Player.Position.Distance(a.Position) descending
				orderby a.UID != 180 descending
				select a).LastOrDefault();
        }

        private static bool Validated(IMapObj obj, GameClient client)
        {
            if (obj.UID == 180)
                return false;
            SobNpc target;
            target = obj as SobNpc;
            if (obj.InLine)
            {
                if (target.BotCount > 3)
                    return false;
            }
            if (target == null)
                return false;
            if (target.Map != client.Player.Map)
                return false;
            if (target.DynamicID != client.Player.DynamicID)
                return false;
            if (target.UID == client.Player.UID)
                return false;
            return true;
        }

        public unsafe static void StartAsync(GameClient client)
        {
            switch (client.AIStatus)
            {
                case AIEnum.AIStatus.Idle:
                    {
                        IMapObj obj;
                        obj = ISob(client);
                        if (obj != null)
                        {
                            SobNpc target;
                            target = obj as SobNpc;
                            client.SobTarget = target;
                            client.SobTarget.InLine = true;
                            target.BotCount++;
                            client.AIStatus = AIEnum.AIStatus.Jumping;
                        }
                        break;
                    }
                case AIEnum.AIStatus.Jumping:
                    if (client.Player.Position.Distance(client.SobTarget.Position) > AIStructures.GetAttackDistance(client))
                    {
                        if (client.pathfinder != null)
                        {
                            int point_x;
                            point_x = client.pathfinder[client._pathfinder_length].X;
                            int point_y;
                            point_y = client.pathfinder[client._pathfinder_length].Y;
                            if (client.pathfinder.Length == client._pathfinder_length)
                            {
                                point_x = client.pathfinder[client._pathfinder_length].X + 1;
							point_y = client.pathfinder[client._pathfinder_length].Y + 1;
                            }
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
                        else if (!client.Map.Pathfinder.Search(client.Player.Position, client.SobTarget.Position, AIStructures.GetJumpDistance(client.AIType), client.Map, out client.pathfinder))
                        {
                            client.AIStatus = AIEnum.AIStatus.Idle;
                        }
                        else
                        {
                            client._pathfinder_length = 0;
                        }
                    }
                    else if (client.Player.Position.Distance(client.SobTarget.Position) <= AIStructures.GetAttackDistance(client))
                    {
                        client.AIStatus = AIEnum.AIStatus.Attacking;
                    }
                    break;
                case AIEnum.AIStatus.Attacking:
                    {
                        Flags.SpellID SpellID;
                        SpellID = AIStructures.WeaponTypeValid(client, client.SobTarget.Position);
                        InteractQuery interactQuery;
                        if (SpellID == Flags.SpellID.Physical)
                        {
                            new MsgSpellAnimation.SpellObj();
                            Physical.OnNpcs(client.Player, client.SobTarget, null, out var DmgObj);
                            using (RecycledPacket recycledPacket = new RecycledPacket())
                            {
                                Packet stream3;
                                stream3 = recycledPacket.GetStream();
                                interactQuery = default(InteractQuery);
                                interactQuery.AtkType = MsgAttackPacket.AttackID.Physical;
                                interactQuery.X = client.SobTarget.X;
                                interactQuery.Y = client.SobTarget.Y;
                                interactQuery.UID = client.Player.UID;
                                interactQuery.OpponentUID = client.SobTarget.UID;
                                interactQuery.Damage = (int)DmgObj.Damage;
                                InteractQuery action;
                                action = interactQuery;
                                client.Player.View.SendView(stream3.InteractionCreate(&action), true);
                                if (!client.SobTarget.Alive)
                                    client.AIStatus = AIEnum.AIStatus.Idle;
                                else
                                    client.AIStatus = AIEnum.AIStatus.Jumping;
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
                        interactQuery.OpponentUID = client.SobTarget.UID;
                        interactQuery.UID = client.Player.UID;
                        interactQuery.X = client.SobTarget.X;
                        interactQuery.Y = client.SobTarget.Y;
                        interactQuery.SpellID = (ushort)SpellID;
                        InteractQuery AttackPaket;
                        AttackPaket = interactQuery;
                        if (MagicType.RandomSpells.Contains(SpellID))
                        {
                            client.Player.RandomSpell = AttackPaket.SpellID;
                            AttackPaket.OpponentUID = client.SobTarget.UID;
                        }
                        AttackPaket.AtkType = _type;
                        using (RecycledPacket recycledPacket2 = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket2.GetStream();
                            MsgAttackPacket.ProcescMagic(client, stream2, AttackPaket, true);
                        }
                        if (!client.SobTarget.Alive)
                            client.AIStatus = AIEnum.AIStatus.Idle;
                        else
                            client.AIStatus = AIEnum.AIStatus.Jumping;
                        break;
                    }
            }
        }
    }
}
