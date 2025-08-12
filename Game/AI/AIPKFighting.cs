using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;
using System.Collections.Generic;
using System.Linq;

 
namespace TheChosenProject.Game.ConquerStructures.AI
{
    public class AIPKFighting
    {
        private static GameClient IPlayers(GameClient client)
        {
            return client.Map.Values.Where(p => Validated(p)).OrderByDescending(a => client.Player.Position.Distance(a.Player.Position)).LastOrDefault();
        }
        private static bool Validated(GameClient target)
        {
            try
            {
                if (target == null) return false;
                if (target.Fake) return false;
                if (!target.Player.Alive) return false;
                if (target.Player.OfflineTraining == MsgOfflineTraining.Mode.Hunting) return false;
                if (target.Player.OfflineTraining == MsgOfflineTraining.Mode.Shopping) return false;
                if (target.Player.OfflineTraining == MsgOfflineTraining.Mode.TrainingGroup) return false;
                if (!Program.FreePkMap.Contains(target.Player.Map)) return false;
                if (target.Player.AutoHunting == Game.MsgAutoHunting.AutoStructures.Mode.Enable) return false;
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
                case AI.AIEnum.AIStatus.Idle:
                    {
                        var obj = IPlayers(client);
                        if (obj != null)
                        {
                            client.Target = obj.Player;
                            client.Player.SetPkMode(TheChosenProject.Role.Flags.PKMode.PK);
                            client.AIStatus = AIEnum.AIStatus.Jumping;
                        }
                        break;
                    }
                case AIEnum.AIStatus.Jumping:
                    {
                        if (client.Player.Position.Distance(client.Target.Position) > AI.AIStructures.GetAttackDistance(client))
                        {
                            if (client.pathfinder != null)
                            {
                                int point_x = client.pathfinder[client._pathfinder_length].X;
                                int point_y = client.pathfinder[client._pathfinder_length].Y;
                                InterActionWalk inter = new InterActionWalk()
                                {
                                    Mode = MsgInterAction.Action.Jump,
                                    X = (ushort)point_x,
                                    Y = (ushort)point_y,
                                    UID = client.Player.UID,
                                    OponentUID = 0
                                };
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.View.SendView(stream.InterActionWalk(&inter), true);
                                }
                                client.Player.Angle = Core.GetAngle(client.Player.X, client.Player.Y, (ushort)point_x, (ushort)point_y);
                                client.Player.Action = TheChosenProject.Role.Flags.ConquerAction.Jump;
                                client.Map.View.MoveTo<IMapObj>(client.Player, (ushort)point_x, (ushort)point_y);
                                client.Player.X = (ushort)point_x;
                                client.Player.Y = (ushort)point_y;
                                client.Player.View.Role(false, null);
                                if (++client._pathfinder_length == client.pathfinder.Length)
                                    client.pathfinder = null;
                            }
                            else
                            {
                                if (!client.Map.Pathfinder.Search(client.Player.Position, client.Target.Position, AI.AIStructures.GetJumpDistance(client.AIType), client.Map, out client.pathfinder))
                                    client.AIStatus = AIEnum.AIStatus.Idle;
                                else client._pathfinder_length = 0;
                            }
                        }
                        else if (client.Player.Position.Distance(client.Target.Position) <= AI.AIStructures.GetAttackDistance(client))
                            client.AIStatus = AIEnum.AIStatus.Attacking;
                        break;
                    }
                case AIEnum.AIStatus.Attacking:
                    {
                        TheChosenProject.Role.Flags.SpellID SpellID = AI.AIStructures.WeaponTypeValid(client, client.Target.Position);
                        if (Core.Rate(10) && !AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            if (AtributesStatus.IsWarrior(client.Player.Class) || AtributesStatus.IsTaoist(client.Player.Class))
                                client.Player.ActiveDefensePotion(1);
                            else
                            {
                                client.Player.ActiveAttackPotion(1);
                                if (Core.Rate(5))
                                    client.Player.ActiveDefensePotion(1);
                            }
                        }
                        switch (SpellID)
                        {
                            case TheChosenProject.Role.Flags.SpellID.Physical:
                                {
                                    MsgSpellAnimation.SpellObj DmgObj;
                                    MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, client.Target, null, out DmgObj);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        InteractQuery action = new InteractQuery()
                                        {
                                            AtkType = TheChosenProject.Game.MsgServer.MsgAttackPacket.AttackID.Physical,
                                            X = client.Target.X,
                                            Y = client.Target.Y,
                                            UID = client.Player.UID,
                                            OpponentUID = client.Target.UID,
                                            Damage = (int)DmgObj.Damage
                                        };
                                        client.Player.View.SendView(stream.InteractionCreate(&action), true);
                                        if (client.Target.HitPoints <= (int)DmgObj.Damage)
                                        {
                                            client.Map.SetMonsterOnTile(client.Target.X, client.Target.Y, false);
                                            client.Target.Dead(client.Player, client.Target.X, client.Target.Y, 0);
                                            client.AIStatus = AI.AIEnum.AIStatus.Idle;
                                        }
                                        else
                                        {
                                            client.Target.HitPoints -= (int)DmgObj.Damage;
                                            client.AIStatus = AI.AIEnum.AIStatus.Jumping;
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    TheChosenProject.Game.MsgServer.MsgAttackPacket.AttackID _type = TheChosenProject.Game.MsgServer.MsgAttackPacket.AttackID.Magic;
                                    if (SpellID == Flags.SpellID.ScatterFire)
                                        _type = TheChosenProject.Game.MsgServer.MsgAttackPacket.AttackID.Archer;
                                    if (SpellID == Flags.SpellID.DragonWhirl)
                                        _type = TheChosenProject.Game.MsgServer.MsgAttackPacket.AttackID.InMoveSpell;
                                    InteractQuery AttackPaket = new InteractQuery
                                    {
                                        OpponentUID = client.Target.UID,
                                        UID = client.Player.UID,
                                        X = client.Target.X,
                                        Y = client.Target.Y,
                                        SpellID = (ushort)SpellID
                                    };
                                    if (MagicType.RandomSpells.Contains(SpellID))
                                    {
                                        client.Player.RandomSpell = AttackPaket.SpellID;
                                        AttackPaket.OpponentUID = client.Target.UID;
                                    }
                                    AttackPaket.AtkType = _type;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                                    }
                                    if (!client.Target.Alive)
                                        client.AIStatus = AIEnum.AIStatus.Idle;
                                    else client.AIStatus = AIEnum.AIStatus.Jumping;
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
