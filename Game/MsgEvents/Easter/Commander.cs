using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game.MsgServer;
using static DevExpress.Xpo.DB.DataStoreLongrunnersWatch;
using System.Drawing;


namespace TheChosenProject.Game.MsgNpc.Scripts.Quests.Easter
{
    public class Commander
    {
        [NpcAttribute(NpcID.Commander)]
        public static void Handler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(7);
            return;
            switch(Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(729922, 1) &&
                            client.Inventory.Contain(729923, 1) &&
                            client.Inventory.Contain(729924, 1) &&
                            client.Inventory.Contain(729925, 1) &&
                            client.Inventory.Contain(729926, 1))
                        {
                            if (client.Inventory.HaveSpace(3))
                            {
                                #region Rewards
                                client.Inventory.RemoveStackItem(729922, 1, stream);
                                client.Inventory.RemoveStackItem(729923, 1, stream);
                                client.Inventory.RemoveStackItem(729924, 1, stream);
                                client.Inventory.RemoveStackItem(729925, 1, stream);
                                client.Inventory.RemoveStackItem(729926, 1, stream);

                                if (client.Inventory.Contain(729921, 1))
                                    client.Inventory.RemoveStackItem(729921, 1, stream);

                                Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                packet.ID = MsgStringPacket.StringID.Effect;
                                packet.UID = client.Player.UID;
                                packet.Strings = new string[1] { "lottery" };
                                client.Player.View.SendView(stream.StringPacketCreate(packet), true);
                                {
                                    if (MyMath.ChanceSuccess(1))
                                    {
                                        client.Inventory.Add(stream, 720547);
                                        client.SendSysMesage("You have received a MegaMetsPack!");
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has found all the egg fragments and received a MegaMetsPack from Commander!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                    }
                                    else if (MyMath.ChanceSuccess(30))
                                    {
                                        client.Inventory.Add(stream, 730002, 1);
                                        client.SendSysMesage( "You have received +2 Stone!");
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has found all the egg fragments and received a  +2 Stone from Commander!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                    }
                                    else if (MyMath.ChanceSuccess(40))
                                    {
                                        client.Inventory.Add(stream, 730002, 1);
                                        client.SendSysMesage("You have received +2 Stone!");
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has found all the egg fragments and received a  +2 Stone from Commander!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                    }
                                    else if (MyMath.ChanceSuccess(20))
                                    {
                                        client.Inventory.Add(stream, 730003, 1);
                                        client.SendSysMesage("You have received the +3 Stone!");
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has found all the egg fragments and received a  +3 Stone from Commander!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                    }
                                    //else if (MyMath.ChanceSuccess(20))
                                    //{
                                    //    client.Inventory.Add(stream, 700072);
                                    //    client.SendSysMesage("You have received a Ref TortoiseGem!");
                                    //    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has found all the egg fragments and received a  Ref TortoiseGem from Commander!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                    //}
                                    else if (MyMath.ChanceSuccess(5))
                                    {
                                        client.Inventory.Add(stream, 721170);
                                        client.SendSysMesage( "You have received a HousePermit!");
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has found all the egg fragments and received a HousePermit from Commander!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                    }
                                    else if (MyMath.ChanceSuccess(20))
                                    {
                                        client.Inventory.Add(stream, 720135, 1);
                                        client.SendSysMesage( "You have received SuperGemPack!");
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has found all the egg fragments and received a  SuperGem Pack from Commander!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 730002, 1);
                                        client.SendSysMesage("You have received +2 Stone!");
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + client.Player.Name + " has found all the egg fragments and received a  +2 Stone from Commander!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                                    }
                                }
                                #endregion

                                dialog.AddText("Seems like you've found all the egg fragments! Here's your reward!");
                                dialog.AddOption("Thanks", 255);
                                dialog.FinalizeDialog();
                            }
                            else
                            {
                                dialog.AddText("Please make 4 or 5 room in your inventory first!");
                                dialog.AddOption("Alright", 255);
                                dialog.FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.AddText("Help! Hey, if you have some free time, I'll reward you? My precious EpicEgg was broken into fragments and scattered across the land upon my recent trip to Twin City. ");
                            dialog.AddText(" A Demon with incredible power shot with me with a bow, but I was protected by the Egg. It split into five fragments and one landed in each city. Just don't tell the Easter Bunny...");
                            dialog.AddOption("Tell me more about it", 1);
                            dialog.AddOption("Yeah right...", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        dialog.AddText("Oh, Well lets just say, I'll give you whatever is in my bag! Meteors, DragonBalls, +n Items, my precious garments... What ever it takes.");
                        dialog.AddText(" All of it just to make sure that we get those Eggs safe without Easter Bunny knowing about it!");
                        dialog.AddOption("Alright I'll help you", 2);
                        dialog.AddOption("I don't have time for that", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        dialog.AddText("Think about it, I'd go but my leg got shot with an arrow... looking foward to seeing you again!");
                        dialog.AddText(" You just need to go to each city and look for the Easter Eggs! They contain fragments to make the ultimate EpicEgg!");
                        dialog.AddOption("Let's save Easter then", 3);
                        dialog.AddOption("Maybe later", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(729921, 1))
                        {
                            dialog.AddText("It seems like I already gave you a bag were you can keep the eggs fragments! What ");
                            dialog.AddText("are you still doing here? The damn rabbit is going to know I lost the eggs!");
                            dialog.AddOption("I'm on my way!", 255);
                            dialog.FinalizeDialog();
                        }
                        else if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 729921);
                            dialog.AddText("Off you go! Make sure you come back in time with those Eggs so that the Bunny won't notice! Take this Feed with you ");
                            dialog.AddText("It will help you grabbing the Eggs! Some crazy guys told me they moved!");
                            dialog.AddOption("Let's go then!", 255);
                            dialog.FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("Please make up some room in your inventory first! I need to give you something!");
                            dialog.AddOption("Alright", 255);
                            dialog.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}
