using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack
{
   public class CheckItems
    {

        public static void AttackDurability(Client.GameClient client, ServerSockets.Packet stream)
        {
            if(client.Player.Map == 1002)
                return;
            if (client.Player.Rate(1))
            {
                bool dura_zero = false;
                foreach (var item in client.Equipment.CurentEquip)
                {
                    if (item != null)
                    {
                        if (client.Player.Class > 39 && client.Player.Class < 46)
                        {
                            if (item.Position == (ushort)Role.Flags.ConquerItem.RightWeapon
                               || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
                               || item.Position == (ushort)Role.Flags.ConquerItem.Ring
                               || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRing
                               || item.Position == (ushort)Role.Flags.ConquerItem.Fan
                               || item.Position == (ushort)Role.Flags.ConquerItem.RidingCrop)
                            {
                                byte durability = (byte)Program.GetRandom.Next(1, Math.Max(2, (int)(item.MaximDurability / 1000)));

                                if (item.Durability < 100)
                                {
                                    if ((item.Durability % 10) == 0)
                                    {
                                        client.SendSysMesage($"{Database.Server.ItemsBase.GetItemName(item.ITEM_ID)} has been severely damaged. Please repair it soon, otherwise, it will be gone.", MsgMessage.ChatMode.TopLeft);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "EquipBroken");
                                    }
                                }
                                else if (item.Durability < 400)
                                {
                                    if (item.Durability % 10 == 0)
                                    {
                                        client.SendSysMesage($"Durability of {Database.Server.ItemsBase.GetItemName(item.ITEM_ID)}  is too low. Please repair it soon to prevent further damaging.", MsgMessage.ChatMode.TopLeft);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "EquipBroken");
                                    }
                                }

                                if (item.Durability > durability)
                                    item.Durability -= durability;
                                else
                                {
                                    item.Durability = 0;
                                    dura_zero = true;
                                }
                                item.Mode = Role.Flags.ItemMode.Update;

                                item.Send(client, stream);
                            }
                        }
                        else
                        {
                            if (item.Position == (ushort)Role.Flags.ConquerItem.RightWeapon
                                   || item.Position == (ushort)Role.Flags.ConquerItem.LeftWeapon
                                   || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
                                   || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteLeftWeapon
                                   || item.Position == (ushort)Role.Flags.ConquerItem.Ring
                                   || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRing
                                     || item.Position == (ushort)Role.Flags.ConquerItem.Fan
                                     || item.Position == (ushort)Role.Flags.ConquerItem.RidingCrop)
                            {
                                byte durability = (byte)Program.GetRandom.Next(1, Math.Max(2, (int)(item.MaximDurability / 1000)));
                                if (item.Durability < 100)
                                {
                                    if ((item.Durability % 10) == 0)
                                    {
                                        client.SendSysMesage($"{Database.Server.ItemsBase.GetItemName(item.ITEM_ID)} has been severely damaged. Please repair it soon, otherwise, it will be gone.", MsgMessage.ChatMode.TopLeft);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "EquipBroken");
                                    }
                                }
                                else if (item.Durability < 400)
                                {
                                    if (item.Durability % 10 == 0)
                                    {
                                        client.SendSysMesage($"Durability of {Database.Server.ItemsBase.GetItemName(item.ITEM_ID)}  is too low. Please repair it soon to prevent further damaging.", MsgMessage.ChatMode.TopLeft);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "EquipBroken");
                                    }
                                }

                                if (item.Durability > durability)
                                    item.Durability -= durability;
                                else
                                {
                                    item.Durability = 0;
                                    dura_zero = true;
                                }
                                item.Mode = Role.Flags.ItemMode.Update;

                                item.Send(client, stream);
                            }
                        }


                    }
                }
                if (dura_zero)
                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
            }
        }
        public static void RespouseDurability(Client.GameClient client)
        {
            //return;
            if (client.Player.Map == 1002)
                return;

            if (client.Player.Rate(5))
            {

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    bool dura_zero = false;
                    foreach (var item in client.Equipment.CurentEquip)
                    {
                        if (item != null)
                        {

                            if (item.Position == (ushort)Role.Flags.ConquerItem.Armor
                               || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteArmor
                               || item.Position == (ushort)Role.Flags.ConquerItem.Necklace
                               || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteNecklace
                               || item.Position == (ushort)Role.Flags.ConquerItem.Boots
                               || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteBoots
                                || item.Position == (ushort)Role.Flags.ConquerItem.Head
                                || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteHead)
                            {
                                byte durability = (byte)Program.GetRandom.Next(1, Math.Max(2, (int)(item.MaximDurability / 1000)));
                                if (item.Durability < 100)
                                {
                                    if ((item.Durability % 10) == 0)
                                    {
                                        client.SendSysMesage($"{Database.Server.ItemsBase.GetItemName(item.ITEM_ID)} has been severely damaged. Please repair it soon, otherwise, it will be gone.", MsgMessage.ChatMode.TopLeft);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "EquipBroken");
                                    }
                                }
                                else if (item.Durability < 400)
                                {
                                    if (item.Durability % 10 == 0)
                                    {
                                        client.SendSysMesage($"Durability of {Database.Server.ItemsBase.GetItemName(item.ITEM_ID)}  is too low. Please repair it soon to prevent further damaging.", MsgMessage.ChatMode.TopLeft);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "EquipBroken");
                                    }
                                }
                                if (item.Durability > durability)
                                    item.Durability -= durability;
                                else
                                {
                                    item.Durability = 0;
                                    dura_zero = true;
                                }
                                item.Mode = Role.Flags.ItemMode.Update;


                                item.Send(client, stream);
                            }

                        }
                    }
                    if (dura_zero)
                        client.Equipment.QueryEquipment(client.Equipment.Alternante);
                }
            }
        }
        //public static void AttackDurability(Client.GameClient client, ServerSockets.Packet stream)
        //{
        //     //return;
        //     if (Role.MyMath.Success(5))
        //     {
        //         bool dura_zero = false;
        //        foreach (var item in client.Equipment.CurentEquip)
        //        {
        //            if (item != null)
        //            {
        //                if (item.Position == (ushort)Role.Flags.ConquerItem.RightWeapon
        //                    || item.Position == (ushort)Role.Flags.ConquerItem.LeftWeapon
        //                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
        //                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteLeftWeapon
        //                    || item.Position == (ushort)Role.Flags.ConquerItem.Ring
        //                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRing
        //                      //|| item.Position == (ushort)Role.Flags.ConquerItem.Fan
        //                      /*|| item.Position == (ushort)Role.Flags.ConquerItem.RidingCrop*/)
        //                {
        //                     if (ItemType.IsArrow(item.ITEM_ID))
        //                         continue;
        //                    byte durability = (byte)Program.GetRandom.Next(1, Math.Max(2, (int)(item.MaximDurability / 1000)));
        //                    if (item.Durability > durability)
        //                        item.Durability -= durability;
        //                    else
        //                    {
        //                        item.Durability = 0;
        //                        dura_zero = true;
        //                    }
        //                    item.Mode = Role.Flags.ItemMode.Update;

        //                    item.Send(client, stream);
        //                }

        //            }
        //        }
        //        if (dura_zero)
        //            client.Equipment.QueryEquipment(client.Equipment.Alternante);
        //    }
        //}
        //public static void RespouseDurability(Client.GameClient client)
        //{
        //     //return;
        //     if (Role.MyMath.Success(5))
        //     {
        //         using (var rec = new ServerSockets.RecycledPacket())
        //        {
        //            var stream = rec.GetStream();

        //            bool dura_zero = false;
        //            foreach (var item in client.Equipment.CurentEquip)
        //            {
        //                if (item != null)
        //                {
        //                    if (item.Position == (ushort)Role.Flags.ConquerItem.Armor
        //                        || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteArmor
        //                        || item.Position == (ushort)Role.Flags.ConquerItem.Necklace
        //                        || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteNecklace
        //                        || item.Position == (ushort)Role.Flags.ConquerItem.Boots
        //                        || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteBoots
        //                         || item.Position == (ushort)Role.Flags.ConquerItem.Head
        //                         || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteHead
        //                         /*|| item.Position == (ushort)Role.Flags.ConquerItem.Tower*/)
        //                    {
        //                         if (ItemType.IsArrow(item.ITEM_ID))
        //                             continue;
        //                         byte durability = (byte)Program.GetRandom.Next(1, Math.Max(2, (int)(item.MaximDurability / 1000)));
        //                        if (item.Durability > durability)
        //                            item.Durability -= durability;
        //                        else
        //                        {
        //                            item.Durability = 0;
        //                            dura_zero = true;
        //                        }
        //                        item.Mode = Role.Flags.ItemMode.Update;


        //                        item.Send(client, stream);
        //                    }

        //                }
        //            }
        //            if (dura_zero)
        //                client.Equipment.QueryEquipment(client.Equipment.Alternante);
        //        }
        //    }
        //}
    }
}
