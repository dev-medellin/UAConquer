using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Game.MsgNpc.Scripts.Quests.Easter
{
    internal class Tracy
    {
        [NpcAttribute(NpcID.Tracy)]
        public static void Handler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(1);
            return;
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("I'm here to favour those who are helping us retrieve all the eggs that were stolen by monsters!");
                        dialog.AddText(" If you have already helped you may get the reward you deserve!");
                        dialog.AddOption("Tell me more about it", 1);
                        dialog.AddOption("Claim Reward", 2);
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.AddText("If you managed to help EasterBuddy by hunting some Eggs for him I'm sure he has given you either a ColoredStone");
                        dialog.AddText(" Or a WhiteEgg depending on which type of Eggs you have found during your hunt! I'm giving an EasterFlamePack for 5 ColoredStones and ");
                        dialog.AddText("Bronze/Silver/Gold EasterPacks for WhiteEggs depending on how many you're willing to trade!");
                        dialog.AddOption("Claim Reward", 2);
                        dialog.AddOption("I see", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.AddText("It's always great to have some help in this world. Fight is all we see everywhere.");
                        dialog.AddText(" What would you like to exchange?");
                        dialog.AddOption("5 ColoredStones", 15);
                        dialog.AddOption("3 WhiteEggs", 3);
                        dialog.AddOption("5 WhiteEggs", 5);
                        dialog.AddOption("10 WhiteEggs", 10);
                        dialog.AddOption("I see", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Contain(710072, 5))
                        {
                            client.Inventory.RemoveStackItem(710072, 5, stream);

                            client.Inventory.Add(stream, 720124);

                            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                            packet.ID = MsgStringPacket.StringID.Effect;
                            packet.UID = client.Player.UID;
                            packet.Strings = new string[1] { "lottery" };
                            client.Player.View.SendView(stream.StringPacketCreate(packet), true);

                            dialog.AddText("Here you go! You may open the EasterFlamePack for rewards!");
                            dialog.AddOption("Thanks", 255);
                            dialog.FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("You don't have five ColoredStones!");
                            dialog.AddOption("I see", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                case 5:
                case 10:
                    {
                        if (client.Inventory.Contain(710064, Option))
                        {
                            client.Inventory.RemoveStackItem(710064, Option, stream);

                            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                            packet.ID = MsgStringPacket.StringID.Effect;
                            packet.UID = client.Player.UID;
                            packet.Strings = new string[1] { "lottery" };
                            client.Player.View.SendView(stream.StringPacketCreate(packet), true);

                            if (Option == 3)
                            {
                                client.Inventory.Add(stream, 720123);
                                dialog.AddText("Take this EasterBronzePack! You may open it to obtain rewards!");
                                dialog.AddOption("Thanks", 255);
                                dialog.FinalizeDialog();
                            }
                            else if (Option == 5)
                            {
                                client.Inventory.Add(stream, 720122);
                                dialog.AddText("Take this EasterSilverPack! You may open it to obtain rewards!");
                                dialog.AddOption("Thanks", 255);
                                dialog.FinalizeDialog();
                            }
                            else if (Option == 10)
                            {
                                client.Inventory.Add(stream, 720121);
                                dialog.AddText("Take this EasterGoldPack! You may open it to obtain rewards!");
                                dialog.AddOption("Thanks", 255);
                                dialog.FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.AddText("You don't have enough WhiteEggs! Please come back later!");
                            dialog.AddOption("I see", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}
