using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanUseSpell
    {
        public static bool Verified(InteractQuery Attack, GameClient client, Dictionary<ushort, MagicType.Magic> DBSpells, out MsgSpell ClientSpell, out MagicType.Magic Spell)
        {
            try
            {
                if (MagicType.RandomSpells.Contains((Flags.SpellID)Attack.SpellID))
                {
                    if (client.Player.RandomSpell != Attack.SpellID)
                    {
                        ClientSpell = null;
                        Spell = null;
                        return false;
                    }
                    client.Player.RandomSpell = 0;
                }
                if (client.MySpells.ClientSpells.TryGetValue(Attack.SpellID, out ClientSpell) && DBSpells.TryGetValue(ClientSpell.Level, out Spell))
                {
                    if ((Spell.Type == MagicType.MagicSort.DirectAttack || Spell.Type == MagicType.MagicSort.Attack) && !client.IsInSpellRange(Attack.OpponentUID, Spell.Range))
                    {
                        ClientSpell = null;
                        Spell = null;
                        return false;
                    }
                    uint IncreaseSpellStamina;
                    IncreaseSpellStamina = 0u;
                    if (client.Player.ContainFlag(MsgUpdate.Flags.ScurvyBomb))
                        IncreaseSpellStamina = (uint)(client.Player.UseStamina + 5);
                    if (client.Player.AutoHunting == AutoStructures.Mode.NotActive && client.AIType == AIEnum.AIType.NotActive)
                    {
                        if (client.Player.Map != 1039)
                        {
                            if (Spell.UseStamina + IncreaseSpellStamina > client.Player.Stamina)
                                return false;
                            if ((ushort)(Spell.UseStamina + IncreaseSpellStamina) > 0)
                            {
                                client.Player.Stamina -= (ushort)(Spell.UseStamina + IncreaseSpellStamina);
                                using (RecycledPacket rec = new RecycledPacket())
                                {
                                    Packet stream;
                                    stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                                }
                            }
                            if (Spell.UseMana > client.Player.Mana)
                                return false;
                            if (Spell.UseMana > 0)
                                client.Player.Mana -= Spell.UseMana;
                        }
                        if (Spell.UseArrows != 0 && Spell.ID >= 8000 && Spell.ID <= 9875)
                        {
                            if (!client.Equipment.FreeEquip(Flags.ConquerItem.LeftWeapon))
                            {
                                client.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeapon, out var arrow);
                                if (arrow.Durability <= 0)
                                    return false;
                                if (client.Player.Map != 1039)
                                {
                                    using (RecycledPacket recycledPacket = new RecycledPacket())
                                    {
                                        Packet stream2;
                                        stream2 = recycledPacket.GetStream();
                                        arrow.Durability -= (ushort)Math.Min(arrow.Durability, Spell.UseArrows);
                                        client.Send(stream2.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, arrow.UID, arrow.Durability, 0u, 0u, 0u, 0u));
                                        if (arrow.Durability <= 0 || arrow.Durability > 10000)
                                            ReloadArrows(client.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeapon), client, stream2);
                                    }
                                }
                            }
                            else
                            {
                                if (client.Equipment.FreeEquip(Flags.ConquerItem.AleternanteLeftWeapon))
                                    return false;
                                client.Equipment.TryGetEquip(Flags.ConquerItem.AleternanteLeftWeapon, out var arrow2);
                                if (arrow2.Durability <= 0)
                                    return false;
                                if (client.Player.Map != 1039)
                                {
                                    using (RecycledPacket recycledPacket2 = new RecycledPacket())
                                    {
                                        Packet stream3;
                                        stream3 = recycledPacket2.GetStream();
                                        arrow2.Durability -= (ushort)Math.Min(arrow2.Durability, Spell.UseArrows);
                                        client.Send(stream3.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, arrow2.UID, arrow2.Durability, 0u, 0u, 0u, 0u));
                                        if (arrow2.Durability <= 0 || arrow2.Durability > 10000)
                                            ReloadArrows(client.Equipment.TryGetEquip(Flags.ConquerItem.AleternanteLeftWeapon), client, stream3);
                                    }
                                }
                            }
                        }
                        if (Spell.IsSpellWithColdTime)
                        {
                            Extensions.Time32 nows = Extensions.Time32.Now;
                            if (ClientSpell.ColdTime > nows)
                                return false;
                            else
                            {
                                ClientSpell.IsSpellWithColdTime = true;
                                ClientSpell.ColdTime = nows.AddMilliseconds(Spell.ColdTime);
                            }

                        }
                        else
                        {
                            if (ClientSpell.ID == 10381)
                            {
                                if (client.Player.WhirlWind || DateTime.Now > ClientSpell.LastUse.AddMilliseconds(600.0))
                                {
                                    ClientSpell.LastUse = DateTime.Now;
                                    client.Player.WhirlWind = false;
                                    return true;
                                }
                            }
                            else if (DateTime.Now > ClientSpell.LastUse.AddMilliseconds(Spell.CustomCoolDown) || Server.RebornInfo.StaticSpells.Contains(Spell.ID))
                            {
                                ClientSpell.LastUse = DateTime.Now;
                                return true;
                            }
                            return false;
                        }
                        Time32 now;
                        now = Time32.Now;
                        if (ClientSpell.ColdTime > now)
                            return false;
                        ClientSpell.IsSpellWithColdTime = true;
                        ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);
                    }
                    return true;
                }
                ClientSpell = null;
                Spell = null;
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                ClientSpell = null;
                Spell = null;
                return false;
            }
            return false;
        }

        public static void ReloadArrows(MsgGameItem arrow, GameClient client, Packet stream)
        {
            if (client.Player.Class >= 40 && client.Player.Class <= 45 && !client.Equipment.FreeEquip(Flags.ConquerItem.LeftWeapon) && client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon).ITEM_ID / 1000u == 500)
            {
                client.Equipment.DestoyArrow(Flags.ConquerItem.LeftWeapon, stream);
                uint id;
                id = 1050002u;
                if (arrow != null)
                    id = arrow.ITEM_ID;
                if (client.Inventory.Contain(id, 1u, 0))
                {
                    client.Inventory.SearchItemByID(id, out var newArrow);
                    newArrow.Position = 5;
                    client.Inventory.Update(newArrow, AddMode.REMOVE, stream);
                    client.Equipment.Add(newArrow, stream);
                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                    client.SendSysMesage("Arrows Reloaded.", MsgMessage.ChatMode.TopLeft);
                }
                else if (!client.Inventory.Contain(id, 1u, 0))
                {
                    client.SendSysMesage("Can't reload arrows, you are out of " + Server.ItemsBase[arrow.ITEM_ID].Name + "s!", MsgMessage.ChatMode.TopLeft);
                    client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, 0u, 0uL, 0u, 0u, 0u, 0u));
                }
            }
        }
    }
}
