using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mysqlx.Expr;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler.Updates
{
    public class UpdateSpell
    {
        public static unsafe void CheckUpdate(ServerSockets.Packet stream, Client.GameClient client, InteractQuery Attack, uint Damage, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            if (Damage == 0)
                Damage = 1;
            uint safeDamage = Damage;
            uint rawDamage = safeDamage;

            // Now scale it to be within 500 - 785 range
            if (rawDamage > 785)
            {
                // Use a divisor that brings it within range
                safeDamage = (uint)Math.Max(500, rawDamage / (rawDamage / 700)); // dynamically scale
            }
            else if (rawDamage < 500)
            {
                safeDamage = 500;
            }
            else
            {
                safeDamage = rawDamage;
            }
            if (DBSpells != null)
            {
                MsgSpell ClientSpell;
                if (client.MySpells.ClientSpells.TryGetValue(Attack.SpellID, out ClientSpell))
                {
                    ushort firstlevel = ClientSpell.Level;
                    if (ClientSpell.Level < DBSpells.Count - 1)
                    {
                        if (client.GemValues(Role.Flags.Gem.NormalMoonGem) > 0)
                        {
                            Damage += (Damage * (1 + client.GemValues(Role.Flags.Gem.NormalMoonGem) / 100));
                        }

                        if (client.Player.Level >= DBSpells[ClientSpell.Level].NeedLevel)
                        {
                            safeDamage = Damage;
                            double baseExp = Damage;
                            if (ClientSpell.ID != 4000)
                            {
                                baseExp = Damage * 5 / 100.0;
                            }
                            int targetLevel = 0;
                            bool isMonster = false;

                            if (client.Player.View.TryGetValue(Attack.OpponentUID, out var target, Role.MapObjectType.Monster))
                            {
                                if (target is Game.MsgMonster.MonsterRole monster)
                                {
                                    isMonster = true;
                                    targetLevel = monster.Level;
                                    baseExp = Math.Min(monster.HitPoints, safeDamage);
                                    baseExp = monster.Family.MaxHealth * 5 / 100.0;
                                }
                            }
                            else if (client.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                baseExp = 1;
                            }

                            int finalExp = isMonster
                                ? AdjustExp((int)baseExp, client.Player.Level, targetLevel)
                                : (int)Math.Round(baseExp);

                            if (client.Player.Map == 1039)
                            {
                                finalExp = finalExp / 10;
                                finalExp = finalExp == 0 ? 1 : finalExp;
                            }

                            finalExp = (int)ServerKernel.SPELL_RATE * finalExp;

                            ClientSpell.Experience += finalExp;

                            if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                            {
                                ClientSpell.PreviousLevel = (byte)ClientSpell.Level;
                                ClientSpell.Level++;
                                ClientSpell.Experience = 0;
                            }

                            if (ClientSpell.PreviousLevel != 0 && ClientSpell.PreviousLevel >= ClientSpell.Level)
                            {
                                ClientSpell.Level = ClientSpell.PreviousLevel;
                            }

                            try
                            {
                                if (ClientSpell.Level > firstlevel)
                                    client.SendSysMesage("You have just leveled your skill " + DBSpells[0].Name + ".", MsgMessage.ChatMode.System);
                            }
                            catch (Exception e) { Console.SaveException(e); }

                            client.Send(stream.SpellCreate(ClientSpell));
                        }
                    }
                }
            }

            //if (Attack.AtkType == MsgAttackPacket.AttackID.Physical || Attack.AtkType == MsgAttackPacket.AttackID.Archer)
            //{
            //    uint ProfRightWeapon = client.Equipment.RightWeapon / 1000;
            //    uint PorfLeftWeapon = client.Equipment.LeftWeapon / 1000;
            //    if (ProfRightWeapon != 0)
            //        client.MyProfs.CheckUpdate(ProfRightWeapon, Damage, stream);
            //    if (PorfLeftWeapon != 0)
            //        client.MyProfs.CheckUpdate(PorfLeftWeapon, Damage, stream);
            //}
        }

        public static int AdjustExp(int nDamage, int nAtkLev, int nDefLev)
        {
            int nExp = nDamage;
            int nNameType = AttackHandler.Calculate.Base.GetNameType(nAtkLev, nDefLev);
            int Level = Math.Min(nDefLev, 120);

            if (nNameType == AttackHandler.Calculate.Base.StatusConstants.NAME_GREEN)
            {
                int DeltaLvl = nAtkLev - nDefLev;
                if (DeltaLvl >= 3 && DeltaLvl <= 5)
                    nExp = nExp * 8 / 100;
                else if (DeltaLvl > 5 && DeltaLvl <= 10)
                    nExp = nExp * 7 / 100;
                else if (DeltaLvl > 10 && DeltaLvl <= 20)
                    nExp = nExp * 6 / 100;
                else if (DeltaLvl > 20)
                    nExp = nExp * 5 / 100;
                else
                    nExp = nExp * 1 / 100;
            }
            else if (nNameType == AttackHandler.Calculate.Base.StatusConstants.NAME_RED)
            {
                nExp = (int)(nExp * 1.5);
            }
            else if (nNameType == AttackHandler.Calculate.Base.StatusConstants.NAME_BLACK)
            {
                int DeltaLvl = nDefLev - Level;
                if (DeltaLvl >= -10 && DeltaLvl <= -5)
                    nExp = (int)(nExp * 2.0);
                else if (DeltaLvl >= -20 && DeltaLvl < -10)
                    nExp = (int)(nExp * 3.5);
                else if (DeltaLvl < -20)
                    nExp = (int)(nExp * 5.0);
            }

            return Math.Max(0, nExp);
        }
    }
}
