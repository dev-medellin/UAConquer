using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;

 
namespace TheChosenProject.Game.Ai
{
    public class AIThread
    {
        public static void TeamInvitationAsync(GameClient client)
        {
            if ((client.AIType != AIEnum.AIType.Leveling && client.AIType != AIEnum.AIType.Hunting) || client.Team == null || !client.Team.AutoInvite || client.Player.Map == 1036 || !client.Team.CkeckToAdd() || !(Time32.Now > client.Team.InviteTimer.AddSeconds(15)))
            {
                client.SendSysMesage("Please try later.");
                return;
            }
            client.Team.InviteTimer = Time32.Now;
            foreach (IMapObj obj in client.Player.View.Roles(MapObjectType.Player))
            {
                if ((obj as Player).Owner.Team == null)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        obj.Send(stream.PopupInfoCreate(client.Player.UID, obj.UID, client.Player.Level, client.Player.BattlePower));
                        stream.TeamCreate(MsgTeam.TeamTypes.InviteRequest, client.Player.UID);
                        obj.Send(stream);
                    }
                }
            }
        }

        public unsafe static void Kicked(uint UID)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                if (Server.GamePoll.TryRemove(UID, out var mero))
                {
                    GameMap Map;
                    Map = Server.ServerMaps[mero.Player.Map];
                    mero.Team?.Remove(mero, true);
                    Map.Denquer(mero);
                    ActionQuery actionQuery;
                    actionQuery = default(ActionQuery);
                    actionQuery.ObjId = mero.Player.UID;
                    actionQuery.Type = ActionType.RemoveEntity;
                    ActionQuery action;
                    action = actionQuery;
                    mero.Player.View.SendView(stream.ActionCreate(&action), false);
                    ServerKernel.Log.SaveLog($"AI[{mero.Player.Name}] has logout!", true, LogType.WARNING);
                }
            }
        }

        public unsafe static void ExpiredAsync(GameClient client)
        {
            if (DateTime.Now > client.Player.AIBotExpire)
                Kicked(client.Player.UID);
            if (client.Player.OnDefensePotion && Time32.Now > client.Player.OnDefensePotionStamp)
                client.Player.OnDefensePotion = false;
            if (client.Player.OnAttackPotion && Time32.Now > client.Player.OnAttackPotionStamp)
                client.Player.OnAttackPotion = false;
            int Medicine;
            Medicine = (int)client.Status.MaxHitpoints - client.Player.HitPoints;
            if (Medicine > 1000 && client.Player.Alive)
            {
                int Plushealth;
                Plushealth = Medicine / 1000 * 100;
                if (DateTime.Now > client.Player.MedicineStamp.AddMilliseconds(Plushealth))
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar) || client.Player.HitPoints == client.Status.MaxHitpoints)
                        return;
                    client.Player.HitPoints = Math.Min(client.Player.HitPoints + Plushealth, (int)client.Status.MaxHitpoints);
                    client.Player.MedicineStamp = DateTime.Now;
                }
            }
            StatusFlagsBigVector32.Flag[] flags;
            flags = client.Player.BitVector.GetFlags();
            foreach (StatusFlagsBigVector32.Flag flag in flags)
            {
                if (flag.Expire(Time32.Now))
                {
                    if (flag.Key >= 98 && flag.Key <= 110)
                        client.Player.AddAura(client.Player.UseAura, null, 0);
                    else if (flag.Key == 32)
                    {
                        client.Player.CursedTimer = 0;
                        client.Player.RemoveFlag(MsgUpdate.Flags.Cursed);
                    }
                    else
                    {
                        client.Player.RemoveFlag((MsgUpdate.Flags)flag.Key);
                    }
                }
                else if (flag.Key == 111)
                {
                    client.Player.RemovedShackle = DateTime.Now;
                }
                else if (flag.Key == 1)
                {
                    if (flag.CheckInvoke(Time32.Now))
                    {
                        int damage;
                        damage = (int)Base.CalculatePoisonDamageFog((uint)client.Player.HitPoints, 80, 4);
                        if (client.Player.HitPoints == 1)
                            damage = 0;
                        else
                            client.Player.HitPoints = Math.Max(1, client.Player.HitPoints - damage);
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket.GetStream();
                            InteractQuery interactQuery;
                            interactQuery = default(InteractQuery);
                            interactQuery.Damage = damage;
                            interactQuery.AtkType = MsgAttackPacket.AttackID.Physical;
                            interactQuery.X = client.Player.X;
                            interactQuery.Y = client.Player.Y;
                            interactQuery.OpponentUID = client.Player.UID;
                            InteractQuery action;
                            action = interactQuery;
                            client.Player.View.SendView(stream2.InteractionCreate(&action), true);
                        }
                    }
                }
                else if (flag.Key == 46)
                {
                    if (flag.CheckInvoke(Time32.Now))
                    {
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;
                            stream = rec.GetStream();
                            InteractQuery interactQuery;
                            interactQuery = default(InteractQuery);
                            interactQuery.UID = client.Player.UID;
                            interactQuery.X = client.Player.X;
                            interactQuery.Y = client.Player.Y;
                            interactQuery.SpellID = 6009;
                            interactQuery.AtkType = MsgAttackPacket.AttackID.Magic;
                            InteractQuery action2;
                            action2 = interactQuery;
                            MsgAttackPacket.ProcescMagic(client, stream.InteractionCreate(&action2), action2);
                        }
                    }
                }
                else if (flag.Key == 14 || (flag.Key == 15 && client.Player.Map != 6000))
                {
                    if (flag.CheckInvoke(Time32.Now))
                    {
                        if (client.Player.PKPoints > 0)
                            client.Player.PKPoints--;
                        client.Player.PkPointsStamp = Time32.Now;
                    }
                }
                else if (flag.Key == 32 && flag.CheckInvoke(Time32.Now))
                {
                    if (client.Player.CursedTimer > 0)
                    {
                        client.Player.CursedTimer--;
                        continue;
                    }
                    client.Player.CursedTimer = 0;
                    client.Player.RemoveFlag(MsgUpdate.Flags.Cursed);
                }
            }
        }

        public static void RevivedAsync(GameClient client)
        {
            if (client.Player.Alive)
                return;
            if (DateTime.Now > client.Player.GhostStamp && !client.Player.ContainFlag(MsgUpdate.Flags.Ghost))
            {
                client.Player.AddFlag(MsgUpdate.Flags.Ghost, 2592000, true);
                if ((int)client.Player.Body % 10 < 3)
                    client.Player.TransformationID = 99;
                else
                    client.Player.TransformationID = 98;
                GameClient[] values;
                values = client.Map.Values;
                foreach (GameClient get_users in values)
                {
                    if (get_users != null)
                    {
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket.GetStream();
                            get_users.Send(stream2.MapStatusCreate(client.Player.Map, client.Map.ID, client.Map.TypeStatus));
                            get_users.Player.View.ReSendView(stream2);
                        }
                    }
                }
            }
            if (Time32.Now > client.Player.DeadStamp.AddSeconds(20) && client.Player.ContainFlag(MsgUpdate.Flags.Ghost))
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    client.Player.Revive(stream);
                }
            }
        }

        public static bool Validated(GameClient client)
        {
            bool Valid;
            Valid = true;
            if (client == null)
                Valid = false;
            if (client.Map == null)
                Valid = false;
            if (!client.Fake)
                Valid = false;
            if (client.AIType == AIEnum.AIType.NotActive)
                Valid = false;
            return Valid;
        }

        public static void AIStartAsync(GameClient client)
        {
            try
            {
                if (!Validated(client))
                    return;
                ExpiredAsync(client);
                TeamInvitationAsync(client);
                RevivedAsync(client);
                if (client.Player.Alive)
                {
                    switch (client.AIType)
                    {
                        case AIEnum.AIType.Leveling:
                            AILeveling.StartAsync(client);
                            break;
                        case AIEnum.AIType.Training:
                            AITrainingGroup.StartAsync(client);
                            break;
                        case AIEnum.AIType.Hunting:
                            AIHunting.StartAsync(client);
                            break;
                        case AIEnum.AIType.PKFighting:
                            AIPKFighting.StartAsync(client);
                            break;
                        case AIEnum.AIType.BufferBot:
                            AIBuffer.StartAsync(client);
                            break;
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
