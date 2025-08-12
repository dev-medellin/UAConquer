using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Game.MsgNpc.Scripts.Quests.Halloween
{
    internal class KingJack
    {
        [NpcAttribute(NpcID.KingJack)]
        public static void Handler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(250);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Trick or treat? Halloween is here ! Have you noticed the pumpkins dropping from monsters? ");
                        dialog.AddText("I'm looking for brave players to collect them for me and paying 1 Pumpkin Point for 2 Pumpkins! ");
                        dialog.AddText("Alternatively, you can also give me 1 Pumpkin Seeds in exchange for 5 Pumpkin Point!");
                        dialog.AddOption("Check my Pumpkin Points", 1);
                        dialog.AddOption("Exchange Pumpkins", 2);
                        dialog.AddOption("I have Pumpkin Seeds!", 8);
                        dialog.AddOption("I want to get my prize!", 3);
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.AddText("You currently have " + client.Player.PumpkinPoints + " pumpkin points.");
                        dialog.AddOption("Exchange Pumpkins", 2);
                        dialog.AddOption("I want to get my prize!", 3);
                        dialog.AddOption("Just passing by.", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(722176, 2))
                        {
                            client.Inventory.RemoveStackItem(722176, 2, stream);
                            client.Player.PumpkinPoints++;
                            dialog.AddText("Congratulations! You have " + client.Player.PumpkinPoints + " pumpkin points now!");
                            dialog.AddOption("Thanks.", 255);
                            dialog.FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("I'm sorry but you don't have 2 Pumpkins!");
                            dialog.AddOption("I see", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        dialog.AddText("You currently have " + client.Player.PumpkinPoints + " pumpkin points! What prize would you like to get?");
                        dialog.AddOption("Halloween Random Reward (10 Pts)", 7);
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.PumpkinPoints >= 10)
                        {
                            if (client.Inventory.HaveSpace(2))
                            {
                                client.Player.PumpkinPoints -= 10;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                int num = Program.GetRandom.Next(0, 8);
                                switch (num)
                                {
                                    case 1:
                                        {
                                            if (Core.ChanceSuccess(40))
                                            {
                                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                                client.SendSysMesage( "You have received a Dragonball!");
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a Dragonball in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a Dragonball in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                            }
                                            else
                                            {
                                                client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1);
                                                client.SendSysMesage( "You have received a MeteorScroll!");
                                            }
                                            break;
                                        }
                                    case 2:
                                        {
                                            if (MyMath.ChanceSuccess(90))
                                            {
                                                client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 1, stream);
                                                client.SendSysMesage( "You have received a ExpBall!");

                                            }
                                            else
                                            {
                                                client.Inventory.AddItemWitchStack(Database.ItemType.ExpBall, 0, 2, stream);
                                                client.SendSysMesage( "You have received a 2 ExpBall!");
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            if (Core.ChanceSuccess(3))
                                            {
                                                client.Inventory.Add(stream, 729022, 1);
                                                client.SendSysMesage( "You have received a +2StonePack!");
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a +2StonePack in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a +2StonePack in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                            }
                                            else
                                            {
                                                client.Inventory.Add(stream, 723712, 1);
                                                client.SendSysMesage( "You have received a +1StonePack!");
                                            }
                                            break;
                                        }
                                    case 4:
                                        {
                                            if (MyMath.ChanceSuccess(90))
                                            {
                                                goto case 1;
                                            }
                                            if (Core.ChanceSuccess(10))
                                            {
                                                client.Inventory.Add(stream, 187475, 1, 0, 1, 0, 0, 0, true);
                                                client.SendSysMesage( "You have received a BloodThirst(Bound) -1 Damage!");
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a BloodThirst(Bound) -1 Damage in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a BloodThirst(Bound) -1 Damage in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                            }
                                            else
                                            {
                                                client.Inventory.Add(stream, 187475, 1, 0, 0, 0, 0, 0, true);
                                                client.SendSysMesage( "You have received a BloodThirst(Bound)!");
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a BloodThirst(Bound) in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                            }
                                            break;
                                        }
                                    case 5:
                                        {
                                            if (MyMath.ChanceSuccess(90))
                                            {
                                                goto case 2;
                                            }
                                            if (Core.ChanceSuccess(10))
                                            {
                                                client.Inventory.Add(stream, 184305, 1, 0, 1, 0, 0, 0, true);
                                                client.SendSysMesage( "You have received a FatalAllure(Bound) -1 Damage!");
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a FatalAllure(Bound) -1 Damage in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a FatalAllure(Bound) -1 Damage in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                            }
                                            else
                                            {
                                                client.Inventory.Add(stream, 184305, 1, 0, 0, 0, 0, 0, true);
                                                client.SendSysMesage( "You have received a FatalAllure(Bound)!");
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a FatalAllure(Bound) in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                            }
                                            break;
                                        }
                                    case 6:
                                        {
                                            if (MyMath.ChanceSuccess(90))
                                            {
                                                goto case 2;
                                            }
                                            if (Core.ChanceSuccess(5))
                                            {
                                                client.Inventory.Add(stream, 184325, 1, 0, 1, 0, 0, 0, true);
                                                client.SendSysMesage( "You have received a Evernight(Bound) -1 Damage!");
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a Evernight(Bound) -1 Damage in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a Evernight(Bound) -1 Damage in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                            }
                                            else
                                            {
                                                client.Inventory.Add(stream, 184325, 1, 0, 0, 0, 0, 0, true);
                                                client.SendSysMesage( "You have received a Evernight(Bound)!");
                                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " won a Evernight(Bound) in Halloween Random Reward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            if (MyMath.ChanceSuccess(7))
                                            {
                                                client.Player.Money += 300000;
                                                client.SendSysMesage( "You have received a 500k Gold!");

                                            }
                                            else if (MyMath.ChanceSuccess(50))
                                            {
                                                client.Player.Money += 15000;
                                                client.SendSysMesage( "You have received a 300k Gold!");

                                            }
                                            else
                                            {
                                                client.Player.Money += 100000;
                                                client.SendSysMesage( "You have received a 100k Gold!");
                                            }
                                            break;
                                        }
                                }
                                dialog.AddText("Here's your reward!");
                                dialog.AddOption("Thanks", 255);
                                dialog.FinalizeDialog();
                            }
                            else
                            {
                                dialog.AddText("Please make some room in your inventory first!");
                                dialog.AddOption("I see", 255);
                                dialog.FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.AddText("You don't have enough points!");
                            dialog.AddOption("I see", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Contain(710587, 1))
                        {
                            client.Inventory.RemoveStackItem(710587, 1, stream);
                            client.Player.PumpkinPoints += 5;
                            dialog.AddText("Congratulations! You have " + client.Player.PumpkinPoints + " pumpkin points now!");
                            dialog.AddOption("Thanks.", 255);
                            dialog.FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("I'm sorry but you don't have Pumpkin Seeds!");
                            dialog.AddOption("I see", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}
