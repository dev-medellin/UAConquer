using System;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgFloorItem;
using System.Collections.Generic;
using TheChosenProject.Client;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using Extensions;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using System.Xml.Linq;

namespace TheChosenProject.Game.MsgMonster
{
    public class PoolProcesses
    {
        public unsafe static void BuffersCallback(GameClient client, Time32 timer)
        {
            try
            {
                IEnumerable<IMapObj> Array;
                Array = client.Player.View.Roles(MapObjectType.Monster);
                foreach (IMapObj map_mob in Array)
                {
                    MonsterRole Mob;
                    Mob = map_mob as MonsterRole;
                    if (Mob.BlackSpot && timer > Mob.Stamp_BlackSpot)
                    {
                        Mob.BlackSpot = false;
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket.GetStream();
                            Mob.Send(stream2.BlackspotCreate(false, Mob.UID));
                        }
                    }
                    StatusFlagsBigVector32.Flag[] flags;
                    flags = Mob.BitVector.GetFlags();
                    foreach (StatusFlagsBigVector32.Flag flag in flags)
                    {
                        if (flag.Expire(timer))
                            Mob.RemoveFlag((MsgUpdate.Flags)flag.Key);
                        else
                        {
                            if (flag.Key != 1 || !flag.CheckInvoke(timer))
                                continue;
                            uint damage;
                            damage = Base.CalculatePoisonDamage(Mob.HitPoints, Mob.PoisonLevel);
                            if (Mob.Boss != 1)
                            {
                                if (Mob.HitPoints == 1)
                                    damage = 0;
                                else
                                    Mob.HitPoints = (uint)Math.Max(1, (int)(Mob.HitPoints - damage));
                                using (RecycledPacket rec = new RecycledPacket())
                                {
                                    Packet stream;
                                    stream = rec.GetStream();
                                    InteractQuery interactQuery;
                                    interactQuery = default(InteractQuery);
                                    interactQuery.Damage = (int)damage;
                                    interactQuery.AtkType = MsgAttackPacket.AttackID.Physical;
                                    interactQuery.X = Mob.X;
                                    interactQuery.Y = Mob.Y;
                                    interactQuery.OpponentUID = Mob.UID;
                                    InteractQuery action;
                                    action = interactQuery;
                                    Mob.Send(stream.InteractionCreate(&action));
                                }
                            }
                            else
                                Mob.RemoveFlag((MsgUpdate.Flags)flag.Key);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static void GuardsCallback(GameClient client, Time32 timer)
        {
            try
            {
                if (client.Map == null)
                    return;
                IEnumerable<IMapObj> Array;
                Array = client.Player.View.Roles(MapObjectType.Monster);
                foreach (IMapObj map_mob in Array)
                {
                    MonsterRole Guard;
                    Guard = map_mob as MonsterRole;
                    if ((Guard.Family.Settings & MonsterSettings.Guard) != MonsterSettings.Guard || !(timer > Guard.AttackSpeed.AddMilliseconds(Guard.Family.AttackSpeed + 1000)))
                        continue;
                    client.Player.View.MobActions.CheckGuardPosition(client.Player.View.GetPlayer(), Guard);
                    if (client.Player.View.MobActions.GuardAttackPlayer(client.Player.View.GetPlayer(), Guard))
                        Guard.AttackSpeed = timer;
                    if (!Guard.Alive)
                    {
                        Guard.AddFadeAway(timer.AllMilliseconds);
                        Guard.RemoveView(timer.AllMilliseconds, client.Map);
                    }
                    foreach (IMapObj mob in Array)
                    {
                        MonsterRole monseter;
                        monseter = mob as MonsterRole;
                        if ((monseter.Family.Settings & MonsterSettings.Guard) != MonsterSettings.Guard && (monseter.Family.Settings & MonsterSettings.Reviver) != MonsterSettings.Reviver && !monseter.IsFloor && client.Player.View.MobActions.GuardAttackMonster(client.Map, monseter, Guard))
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static void AliveMonstersCallback(GameClient client, Time32 timer)
        {
            try
            {
                if (client.Map == null)
                    return;
                IEnumerable<IMapObj> Array;
                Array = client.Player.View.Roles(MapObjectType.Monster);
                foreach (IMapObj map_mob in Array)
                {
                    MonsterRole monster;
                    monster = map_mob as MonsterRole;
                    if (!monster.Alive && monster.State == MobStatus.Respawning)
                    {
                        if (BossDatabase.Bosses.ContainsKey(monster.Family.ID))
                            continue;

                        if (timer > monster.RespawnStamp && !client.Map.MonsterOnTile(monster.RespawnX, monster.RespawnY))
                        {
                            monster.Respawn();
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (monster.Family.ID == 8500)
                                {
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("WaterLord has spawned in Adventure Islands at (520,735), Kill him and get The CleanWater.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                    //Program.DiscordWaterLord.Enqueue("WaterLord has spawned in Adventure Islands at (520,735), Kill him and get The CleanWater.");
                                }
                            }
                            client.Map.SetMonsterOnTile(monster.X, monster.Y, true);
                        }
                        continue;
                    }
                    if ((monster.Family.Settings & MonsterSettings.Guard) == MonsterSettings.Guard || (monster.Family.Settings & MonsterSettings.Reviver) == MonsterSettings.Reviver || (monster.Family.Settings & MonsterSettings.Lottus) == MonsterSettings.Lottus)
                        continue;
                    MonsterRole Mob;
                    Mob = map_mob as MonsterRole;
                    if (Mob.Family.ID != 20211)
                    {
                        // Check if the current target is dead or null
                        if (monster.Target == null || !monster.Target.Alive)
                        {
                            foreach (var otherTarget in monster.View.Roles(client.Map, MapObjectType.Player))
                            {
                                var player = otherTarget as Role.Player;
                                if (player != null && player.Alive)
                                {
                                    bool isFlying = player.ContainFlag(MsgUpdate.Flags.Fly);
                                    bool monsterCanAttackFly = monster.Family.SpellId != 0;

                                    if (!isFlying || (isFlying && monsterCanAttackFly))
                                    {
                                        short distance = MonsterView.GetDistance(otherTarget.X, otherTarget.Y, monster.X, monster.Y);
                                        if (distance <= monster.Family.ViewRange)
                                        {
                                            monster.Target = player;
                                            monster.State = MobStatus.SearchTarget;
                                            break; // Stop after finding the first valid target
                                        }
                                    }
                                }
                            }
                        }

                        // Execute attack only if the target is still valid
                        if (monster.Target != null && monster.Target.Alive && !monster.Target.Owner.ProjectManager)
                        {
                            client.Player.View.MobActions.ExecuteAction(client.Player.View.GetPlayer(), monster);
                        }

                        if (!Mob.Alive)
                        {
                            Time32 now;
                            now = Time32.Now;
                            Mob.AddFadeAway(now.AllMilliseconds);
                            Mob.RemoveView(now.AllMilliseconds, client.Map);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static void ReviversCallback(GameClient client, Time32 timer)
        {
            try
            {
                if (client.Map == null)
                    return;
                IEnumerable<IMapObj> Array;
                Array = client.Player.View.Roles(MapObjectType.Monster);
                foreach (IMapObj map_mob in Array)
                {
                    MonsterRole monseter;
                    monseter = map_mob as MonsterRole;
                    if ((monseter.Family.Settings & MonsterSettings.Reviver) != MonsterSettings.Reviver)
                        continue;
                    if (!monseter.Alive)
                    {
                        monseter.AddFadeAway(timer.AllMilliseconds);
                        monseter.RemoveView(timer.AllMilliseconds, client.Map);
                    }
                    if (Core.GetDistance(map_mob.X, map_mob.Y, client.Player.View.GetPlayer().X, client.Player.View.GetPlayer().Y) < 13 && !client.Player.View.GetPlayer().Alive)
                    {
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket.GetStream();
                            client.Player.View.GetPlayer().Revive(stream2);
                        }
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;
                            stream = rec.GetStream();
                            MsgSpellAnimation SpellPacket;
                            SpellPacket = new MsgSpellAnimation(map_mob.UID, 0, map_mob.X, map_mob.Y, 1100, 0, 0);
                            SpellPacket.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.View.GetPlayer().UID, 0, MsgAttackPacket.AttackEffect.None));
                            SpellPacket.SetStream(stream);
                            SpellPacket.Send(map_mob as MonsterRole);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
