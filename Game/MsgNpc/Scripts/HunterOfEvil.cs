namespace TheChosenProject.Game.MsgNpc.Scripts
{
    internal unsafe class HunterOfEvil
    {
        [NpcAttribute(NpcID.HunterOfEvil)]
        public static void Handle(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(17);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("I am a hunter of evil spirits, I can help you to become a better fighter by giving me the [EvilFeather] and i will make an legandary effect in your gear and I can do a lot.\n If you have some [EvilFeather] you can choose one of these Option:")
                          .AddOption("Damage -1.", 1)
                          .AddOption("Not~interested.", 255)
                          .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.AddText("Hello, you wana make Damage -1   \nfor your items [Accessorys, Garments] for 7 EvilFeather? [You Can Find EvilFeathers From Bosses Or DonationShop!]")
                        //.AddOption("Tower.", 11)
                        //.AddOption("Fan.", 10)
                        .AddOption("Bottle.", 7)
                        //.AddOption("Crop.", 18)
                        .AddOption("Garment.", 9)
                        .AddOption("RightWeaponAccessory.", 15)
                        .AddOption("LeftWeaponAccessory.", 16)
                        //.AddOption("SteedMount.", 17)
                        .AddOption("Not~interested.", 255)
                        .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        MsgServer.MsgGameItem item;
                        if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)Option, out item))
                        {
                            if (item.Durability != item.MaximDurability)
                            {
                                dialog.AddText("Sorry, this item has been damaged. Please get it repaired first.")
                                .AddOption("Oh,~I~see.~Farewell.")
                                .FinalizeDialog();
                                break;
                            }
                            if (item.Bless > 0)
                            {
                                dialog.AddText("Sorry, this item is a Damage -1. There is no bless higher than this. It`s perfect!")
                                 .AddOption("Oh,~I~see.~Farewell.")
                                 .FinalizeDialog();
                                break;
                            }
                            if (client.Inventory.Contain(723410, 7) || client.ProjectManager)
                            {
                                client.Inventory.RemoveStackItem(723410, 7, stream);
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);
                                client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                if (item.Position != 0)
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                dialog.AddText($"Your item {((Role.Flags.ConquerItem)Option).ToString()} has been upgraded bless -1.")
                               .AddOption("Thanks~a~lot!")
                               .FinalizeDialog();
                            }
                            else
                            {
                                dialog.AddText("You don't have 7 EvilFeather.")
                                   .AddOption("Sorry", 255)
                                   .FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.AddText("Sorry, please wear the item you want to upgrade first.")
                            .AddOption("Oh,~I~see.~Farewell.")
                            .FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}