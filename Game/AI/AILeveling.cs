using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.Ai;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;
using System.Linq;


namespace TheChosenProject.Game.ConquerStructures.AI
{
    public class AILeveling
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
                return target != null && !target.InLine && (int)target.Map == (int)client.Player.Map && target != client.MobTarget && target.Alive && (target.Family.Settings & MonsterSettings.Guard) != MonsterSettings.Guard;
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
                if (target.Taken_Selected == true) return false;
                if (target == null) return false;
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
                        if (obj != null)
                        {
                            client.MobTarget = obj;
                            if (client.Team != null)
                            {
                                if (client.Team.IsAIWithNewbie())
                                {
                                    client.AIStatus = AIEnum.AIStatus.Jumping;
                                    //if (!client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                        //client.Player.AddFlag(MsgUpdate.Flags.Ride, StatusFlagsBigVector32.PermanentFlag, false);
                                }
                                else
                                {
                                    if (Extensions.Time32.Now > client.Team.CreateTimer.AddSeconds(60))
                                    {
                                        client.Team.CreateTimer = Extensions.Time32.Now;
                                        AIThread.Kicked(client.Player.UID);
                                    }
                                    //if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                    //    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                                    if (client.Player.Action != Flags.ConquerAction.Wave)
                                        client.Player.Action = Flags.ConquerAction.Wave;
                                }
                            }
                        }
                        break;
                    }
                case AIEnum.AIStatus.Jumping:
                    {
                        if (!client.Team.IsAIWithNewbie())
                        {
                            client.AIStatus = AIEnum.AIStatus.Idle;
                            return;
                        }
                        if (client.Player.Position.Distance(client.MobTarget.Position) > AI.AIStructures.GetAttackDistance(client))
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
                                client.Player.Action = Flags.ConquerAction.Jump;
                                client.Map.View.MoveTo<IMapObj>(client.Player, (ushort)point_x, (ushort)point_y);
                                client.Player.X = (ushort)point_x;
                                client.Player.Y = (ushort)point_y;
                                client.Player.View.Role(false, null);
                                if (++client._pathfinder_length == client.pathfinder.Length)
                                    client.pathfinder = null;
                            }
                            else
                            {
                                if (!client.Map.Pathfinder.Search(client.Player.Position, client.MobTarget.Position, AI.AIStructures.GetJumpDistance(client.AIType), client.Map, out client.pathfinder))
                                    client.AIStatus = AIEnum.AIStatus.Idle;
                                else client._pathfinder_length = 0;
                            }
                        }
                        else if (client.Player.Position.Distance(client.MobTarget.Position) <= AI.AIStructures.GetAttackDistance(client))
                            client.AIStatus = AIEnum.AIStatus.Attacking;
                        break;
                    }
                case AIEnum.AIStatus.Attacking:
                    {
                        if (Database.ItemType.IsBow(client.Equipment.RightWeapon) && client.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ScatterFire))
                        {
                            Flags.SpellID SpellID = Flags.SpellID.ScatterFire;
                            TheChosenProject.Game.MsgServer.MsgAttackPacket.AttackID _type = TheChosenProject.Game.MsgServer.MsgAttackPacket.AttackID.Archer;
                            InteractQuery AttackPaket = new InteractQuery
                            {
                                OpponentUID = client.MobTarget.UID,
                                UID = client.Player.UID,
                                X = client.MobTarget.X,
                                Y = client.MobTarget.Y,
                                SpellID = (ushort)SpellID,
                                AtkType = _type
                            };
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);
                            }
                            if (!client.MobTarget.Alive)
                                client.AIStatus = AIEnum.AIStatus.Idle;
                            else client.AIStatus = AIEnum.AIStatus.Jumping;
                        }
                        break;
                    }
            }
        }

    }
}
