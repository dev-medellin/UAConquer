using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgEvents;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.ServerSockets;
using static DevExpress.XtraEditors.RoundedSkinPanel;
using static TheChosenProject.Game.MsgServer.MsgAttackPacket;

namespace TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Player
    {
        public unsafe static void ExecutePet(int obj, Client.GameClient client, Role.Player attacked)
        {
            if (Calculate.Base.Success(10))
            {
                CheckAttack.CheckItems.RespouseDurability(client);
            }
            ushort X = attacked.X;
            ushort Y = attacked.Y;
            if (attacked.HitPoints <= obj)
            {
                attacked.DeadState = true;
                attacked.Dead(client.Player, X, Y, 0);
                client.Pet.Target = null;
            }
            else
            {
                CheckAttack.CheckGemEffects.CheckRespouseDamage(attacked.Owner);

                attacked.HitPoints -= (int)obj;
            }


        }

        public unsafe static void Execute(MsgSpellAnimation.SpellObj obj, GameClient client, TheChosenProject.Role.Player attacked)
        {
            if (client.Pet != null) client.Pet.Target = attacked;

            if (MsgSchedules.CurrentTournament.Type == TournamentType.SkillTournament && MsgSchedules.CurrentTournament.Process == ProcesType.Alive && MsgSchedules.CurrentTournament.InTournament(client))
            {
                MsgSkillTournament tournament;
                tournament = MsgSchedules.CurrentTournament as MsgSkillTournament;
                tournament.KillSystem.Update(client);
                tournament.KillSystem.CheckDead(attacked.UID);
                if (attacked.SkillTournamentLifes != 0)
                {
                    attacked.SkillTournamentLifes--;
                    attacked.Owner.SendSysMesage("You have " + attacked.SkillTournamentLifes + " more life's left.", MsgMessage.ChatMode.Center);
                    return;
                }
                if (attacked.SkillTournamentLifes == 0)
                {
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        attacked.SendString(stream2, MsgStringPacket.StringID.Effect, true, "accession1");
                        attacked.HitPoints = 0;
                        attacked.Dead(client.Player, attacked.X, attacked.Y, client.Player.UID);
                    }
                }
            }

            if (client.InTDM)
                TeamDeathMatch.OnTakeAttack(attacked.Owner, ref obj.Damage);
            if (client.InFIveOut)
                Get5HitOut.OnTakeAttack(attacked.Owner, ref obj.Damage);
            if (client.InPassTheBomb)
                PassTheBomb.OnTakeAttack(client, attacked.Owner, ref obj.Damage);
            
            if (client.EventBase != null)
                if (client.EventBase.NoDamage && client.EventBase.Stage == MsgEvents.EventStage.Fighting)
                    obj.Damage = client.EventBase.GetDamage(client, attacked.Owner);
            if (client.EventBase != null)
                if (client.EventBase.Stage != MsgEvents.EventStage.Fighting)
                    obj.Damage = 0;
            if (Base.Success(10.0))
                CheckItems.RespouseDurability(client);
            ushort X;
            X = attacked.X;
            ushort Y;
            Y = attacked.Y;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.Type = (ActionType)158;
                actionQuery.ObjId = client.Player.UID;
                actionQuery.wParam1 = client.Player.X;
                actionQuery.wParam2 = client.Player.Y;
                ActionQuery Gui;
                Gui = actionQuery;
                client.Send(stream.ActionCreate(&Gui));
                actionQuery = default(ActionQuery);
                actionQuery.Type = (ActionType)158;
                actionQuery.ObjId = attacked.UID;
                actionQuery.wParam1 = attacked.X;
                actionQuery.wParam2 = attacked.Y;
                ActionQuery action;
                action = actionQuery;
                attacked.Owner.Send(stream.ActionCreate(&action));
            }
            if (client.InST)
            {
                SkillsTournament.OnTakeAttack(client, attacked.Owner, ref obj.Damage);
                //attacked.Dead(client.Player, attacked.X, attacked.Y, client.Player.UID);             
                //Role.Player.KillSystem.Send(client);
                //Role.Player.KillSystem.Update(client);
                //Role.Player.KillSystem.CheckDead(attacked.UID);
                return;
            }
            if (attacked.HitPoints <= obj.Damage)
            {

                attacked.DeadState = true;
                if (client.Player.OnTransform)
                    client.Player.TransformInfo?.FinishTransform();
                attacked.Dead(client.Player, X, Y, 0u);
                if (client.Pet != null) if (client.Pet.Target != null && client.Pet.Target.UID == attacked.UID) client.Pet.Target = null;
                if (attacked.Owner.InLastManStanding)
                    LastManStanding.OnDead(attacked.Owner, ref obj.Damage);
            }
            else
            {
                CheckGemEffects.CheckRespouseDamage(attacked.Owner);
                client.UpdateQualifier(client, attacked.Owner, obj.Damage);
                bool pass = false;
                if (client.EventBase != null)
                {
                    if (client.EventBase.Stage == MsgEvents.EventStage.Fighting)
                        client.EventBase.Hit(client, attacked.Owner);
                }
                if (!pass)
                    attacked.HitPoints -= (int)obj.Damage;
               
            }
        }
    }
}
