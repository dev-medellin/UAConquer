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
    public class AIBuffer
    {
        public static unsafe void StartAsync(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!client.MySpells.ClientSpells.ContainsKey(1095))
                    client.MySpells.Add(stream, 1095, 4);
                if (!client.MySpells.ClientSpells.ContainsKey(1090))
                    client.MySpells.Add(stream, 1090, 4);
            }
            var arrays = client.Map.View.GetAllMapRoles(MapObjectType.Player).ToArray();
            foreach (Role.Player P in arrays)
            {
                //if (!P.Owner.Fake && P.Owner.Fake)
                {
                    if (!P.ContainFlag(MsgUpdate.Flags.Stigma))
                    {
                        if (Core.GetDistance(P.X, P.Y, client.Player.X, client.Player.Y) <= 10)
                        {
                            InteractQuery AttackPaket = new InteractQuery
                            {
                                OpponentUID = P.UID,
                                UID = client.Player.UID,
                                X = P.X,
                                Y = P.Y,
                                SpellID = (ushort)1095
                            };
                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                            }
                        }
                    }
                    //if (!P.ContainFlag(MsgUpdate.Flags.Shield))
                    //{
                    //    if (Core.GetDistance(P.X, P.Y, client.Player.X, client.Player.Y) <= 10)
                    //    {
                    //        InteractQuery AttackPaket = new InteractQuery
                    //        {
                    //            OpponentUID = P.UID,
                    //            UID = client.Player.UID,
                    //            X = P.X,
                    //            Y = P.Y,
                    //            SpellID = (ushort)1090
                    //        };
                    //        AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                    //        using (var rec = new ServerSockets.RecycledPacket())
                    //        {
                    //            var stream = rec.GetStream();
                    //            MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                    //        }
                    //    }
                    //}
                }
            }
            if (DateTime.Now > client.BotBuffersStampJump.AddMilliseconds(700))
            {
                if (AIStructures.BuffersLocation.Count == 0)
                {
                    AIStructures.BuffersLocation.TryAdd(0, new int[] { 432, 367 });
                    AIStructures.BuffersLocation.TryAdd(1, new int[] { 447, 367 });
                    AIStructures.BuffersLocation.TryAdd(2, new int[] { 447, 378 });
                    AIStructures.BuffersLocation.TryAdd(3, new int[] { 447, 386 });
                    AIStructures.BuffersLocation.TryAdd(4, new int[] { 439, 387 });
                    AIStructures.BuffersLocation.TryAdd(5, new int[] { 431, 384 });
                    AIStructures.BuffersLocation.TryAdd(6, new int[] { 430, 376 });
                }
                switch (client.AIStatus)
                {
                    case AIEnum.AIStatus.Idle:
                        {
                            client.PointBufferLocation++;
                            if (client.PointBufferLocation >= AIStructures.BuffersLocation.Count)
                                client.PointBufferLocation = 0;
                            client.AIStatus = AIEnum.AIStatus.Jumping;
                            break;
                        }
                    case AIEnum.AIStatus.Jumping:
                        {
                            if (AIStructures.BuffersLocation.TryGetValue(client.PointBufferLocation, out int[] Loc))
                            {
                                var loc = new Position((int)client.Player.Map, Loc[0], Loc[1], false);
                                if (client.Player.Position.Distance(loc) > 1)
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
                                        if (!client.Map.Pathfinder.Search(client.Player.Position, loc, 12, client.Map, out client.pathfinder))
                                            client.AIStatus = AIEnum.AIStatus.Idle;
                                        else client._pathfinder_length = 0;
                                    }
                                }
                                else if (client.Player.Position.Distance(loc) <= 1)
                                {
                                    client.AIStatus = AIEnum.AIStatus.Attacking;
                                }

                            }
                            break;
                        }
                    case AIEnum.AIStatus.Attacking:
                        {
                            client.AIStatus = AIEnum.AIStatus.Idle;
                            break;
                        }
                }
                client.BotBuffersStampJump = DateTime.Now;
            }
        }
    }
}
