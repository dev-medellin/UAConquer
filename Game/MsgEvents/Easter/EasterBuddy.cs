using TheChosenProject.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Game.MsgNpc.Scripts.Quests.Easter
{
    internal unsafe class EasterBuddy
    {
        [NpcAttribute(NpcID.EasterBuddy)]
        public static void Handler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(7);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Hello, brave adventurer! Easter is approaching, and it's the perfect time to gather eggs for everyone in the community.");
                        dialog.AddText("Would you like to join the fun and help me collect these special eggs?");
                        dialog.AddOption("Tell me more about it", 1);
                        dialog.AddOption("Exchange Eggs", 2);
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.AddText("Monsters across the world will have a chance of dropping Eggs of different kinds! Players must ");
                        dialog.AddText("catch them and come back to me to exchange the eggs for rewards! Look out for Stripped Eggs, those give the best rewards!");
                        dialog.AddOption("Exchange Eggs", 2);
                        dialog.AddOption("I see", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.AddText("It's always great to have some help in this world. Fight is all we see everywhere.");
                        dialog.AddText(" What kind of eggs would you like to exchange?");
                        dialog.AddOption("Easter Eggs", 3);
                        dialog.AddOption("Stripped Eggs", 4);
                        dialog.AddOption("I see", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710060, 1) && client.Inventory.Contain(710061, 1) && client.Inventory.Contain(710062, 1) && client.Inventory.Contain(710063, 1))
                        {
                            client.Inventory.RemoveStackItem(710060, 1, stream);
                            client.Inventory.RemoveStackItem(710061, 1, stream);
                            client.Inventory.RemoveStackItem(710062, 1, stream);
                            client.Inventory.RemoveStackItem(710063, 1, stream);
                            client.Inventory.Add(stream, 710064);
                            dialog.AddText("Take this WhiteEgg! You may use it to exchange it for rewards!");
                            dialog.AddOption("Thanks", 255);
                            dialog.FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("You don't have enough Eggs. Please get one EasterEgg of each colour first!");
                            dialog.AddOption("I see", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710065, 1) ||
                            client.Inventory.Contain(710066, 1) ||
                            client.Inventory.Contain(710067, 1) ||
                            client.Inventory.Contain(710068, 1) ||
                            client.Inventory.Contain(710069, 1) ||
                            client.Inventory.Contain(710070, 1) ||
                            client.Inventory.Contain(710071, 1))
                        {
                            if (client.Inventory.Contain(710065, 1))
                                client.Inventory.RemoveStackItem(710065, 1, stream);
                            else if (client.Inventory.Contain(710066, 1))
                                client.Inventory.RemoveStackItem(710066, 1, stream);
                            else if (client.Inventory.Contain(710067, 1))
                                client.Inventory.RemoveStackItem(710067, 1, stream);
                            else if (client.Inventory.Contain(710068, 1))
                                client.Inventory.RemoveStackItem(710068, 1, stream);
                            else if (client.Inventory.Contain(710069, 1))
                                client.Inventory.RemoveStackItem(710069, 1, stream);
                            else if (client.Inventory.Contain(710070, 1))
                                client.Inventory.RemoveStackItem(710070, 1, stream);
                            else if (client.Inventory.Contain(710071, 1))
                                client.Inventory.RemoveStackItem(710071, 1, stream);

                            client.Inventory.Add(stream, 710072);

                            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                            packet.ID = MsgStringPacket.StringID.Effect;
                            packet.UID = client.Player.UID;
                            packet.Strings = new string[1] { "lottery" };
                            client.Player.View.SendView(stream.StringPacketCreate(packet), true);

                            dialog.AddText("Take this ColoredStone! Gather 5 of them and exchange them for an EasterFlamePack at Tracy in Market!");
                            dialog.AddOption("Thanks", 255);
                            dialog.FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("You don't have a Stripped Egg! Please go hunt for them!");
                            dialog.AddOption("I see", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}
