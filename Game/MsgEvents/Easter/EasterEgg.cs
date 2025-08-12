using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Game.MsgNpc.Scripts.Quests.Easter
{
    internal class EasterEgg
    {
        public static void RemoveEgg(ServerSockets.Packet stream, TheChosenProject.Client.GameClient client, uint ID)
        {
            Role.IMapObj obj;
            if (client.Player.View.TryGetValue(ID, out obj, MapObjectType.Npc))
            {
                var NPC = obj as Game.MsgNpc.Npc;
                #region RemoveEgg

                client.Map.RemoveNpc(NPC, stream);

                #endregion RemoveEgg
                #region SpawnEgg
                Random Rndom = new Random();
                int Location = Program.Rand.Next(1, 9);
                if (ID == 4001)
                    #region Locations

                    switch (Location)
                    {
                        case 1:
                            {
                                NPC.X = 377 - 128;
                                NPC.Y = 393 - 100;
                                break;
                            }
                        case 2:
                            {
                                NPC.X = 369 - 128;
                                NPC.Y = 312 - 100;
                                break;
                            }
                        case 3:
                            {
                                NPC.X = 399 - 128;
                                NPC.Y = 314 - 100;
                                break;
                            }
                        case 4:
                            {
                                NPC.X = 395 - 128;
                                NPC.Y = 294 - 100;
                                break;
                            }
                        case 5:
                            {
                                NPC.X = 465 - 128;
                                NPC.Y = 240 - 100;
                                break;
                            }
                        case 6:
                            {
                                NPC.X = 413 - 128;
                                NPC.Y = 219 - 100;
                                break;
                            }
                        case 7:
                            {
                                NPC.X = 385 - 128;
                                NPC.Y = 240 - 100;
                                break;
                            }
                        case 8:
                            {
                                NPC.X = 428 - 128;
                                NPC.Y = 389 - 100;
                                break;
                            }
                    }

                #endregion Locations
                else if (ID == 4002)
                    #region Locations

                    switch (Location)
                    {
                        case 1:
                            {
                                NPC.X = 210;
                                NPC.Y = 260;
                                break;
                            }
                        case 2:
                            {
                                NPC.X = 180;
                                NPC.Y = 225;
                                break;
                            }
                        case 3:
                            {
                                NPC.X = 220;
                                NPC.Y = 230;
                                break;
                            }
                        case 4:
                            {
                                NPC.X = 212;
                                NPC.Y = 197;
                                break;
                            }
                        case 5:
                            {
                                NPC.X = 247;
                                NPC.Y = 230;
                                break;
                            }
                        case 6:
                            {
                                NPC.X = 190;
                                NPC.Y = 271;
                                break;
                            }
                        case 7:
                            {
                                NPC.X = 236;
                                NPC.Y = 283;
                                break;
                            }
                        case 8:
                            {
                                NPC.X = 246;
                                NPC.Y = 296;
                                break;
                            }
                    }

                #endregion Locations
                else if (ID == 4003)
                    #region Locations

                    switch (Location)
                    {
                        case 1:
                            {
                                NPC.X = 542;
                                NPC.Y = 545;
                                break;
                            }
                        case 2:
                            {
                                NPC.X = 525;
                                NPC.Y = 501;
                                break;
                            }
                        case 3:
                            {
                                NPC.X = 559;
                                NPC.Y = 495;
                                break;
                            }
                        case 4:
                            {
                                NPC.X = 570;
                                NPC.Y = 531;
                                break;
                            }
                        case 5:
                            {
                                NPC.X = 583;
                                NPC.Y = 576;
                                break;
                            }
                        case 6:
                            {
                                NPC.X = 578;
                                NPC.Y = 601;
                                break;
                            }
                        case 7:
                            {
                                NPC.X = 541;
                                NPC.Y = 607;
                                break;
                            }
                        case 8:
                            {
                                NPC.X = 577;
                                NPC.Y = 558;
                                break;
                            }
                    }

                #endregion Locations
                else if (ID == 4004)
                    #region Locations

                    switch (Location)
                    {
                        case 1:
                            {
                                NPC.X = 462;
                                NPC.Y = 668;
                                break;
                            }
                        case 2:
                            {
                                NPC.X = 533;
                                NPC.Y = 684;
                                break;
                            }
                        case 3:
                            {
                                NPC.X = 507;
                                NPC.Y = 604;
                                break;
                            }
                        case 4:
                            {
                                NPC.X = 523;
                                NPC.Y = 593;
                                break;
                            }
                        case 5:
                            {
                                NPC.X = 496;
                                NPC.Y = 579;
                                break;
                            }
                        case 6:
                            {
                                NPC.X = 463;
                                NPC.Y = 531;
                                break;
                            }
                        case 7:
                            {
                                NPC.X = 463;
                                NPC.Y = 618;
                                break;
                            }
                        case 8:
                            {
                                NPC.X = 492;
                                NPC.Y = 649;
                                break;
                            }
                    }

                #endregion Locations
                else if (ID == 4005)
                    #region Locations

                    switch (Location)
                    {
                        case 1:
                            {
                                NPC.X = 687;
                                NPC.Y = 548;
                                break;
                            }
                        case 2:
                            {
                                NPC.X = 706;
                                NPC.Y = 594;
                                break;
                            }
                        case 3:
                            {
                                NPC.X = 766;
                                NPC.Y = 601;
                                break;
                            }
                        case 4:
                            {
                                NPC.X = 762;
                                NPC.Y = 588;
                                break;
                            }
                        case 5:
                            {
                                NPC.X = 729;
                                NPC.Y = 499;
                                break;
                            }
                        case 6:
                            {
                                NPC.X = 694;
                                NPC.Y = 520;
                                break;
                            }
                        case 7:
                            {
                                NPC.X = 731;
                                NPC.Y = 533;
                                break;
                            }
                        case 8:
                            {
                                NPC.X = 709;
                                NPC.Y = 572;
                                break;
                            }
                    }

                #endregion Locations
                client.Map.AddNpc(NPC);
                #endregion SpawnEgg
            }
        }

        [NpcAttribute(NpcID.EasterEgg1)]
        public static void Handle(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(7);
            switch (Option)
            {
                case 0:
                    {
                        var _increment = id - 4001;
                        if (client.Inventory.Contain(729921, 1))//EggBag
                        {
                            if (!client.Inventory.HaveSpace(2))
                            {
                                dialog.AddText("You need at least 2 free spaces in your inventory!");
                                dialog.AddOption("I see", 255);
                                dialog.FinalizeDialog();
                            }
                            else if (client.Inventory.Contain((729922 + _increment), 1))
                            {
                                dialog.AddText("It seems like you already have an Egg Fragment from this city! Go find the other ones!");
                                dialog.AddOption("I see", 255);
                                dialog.FinalizeDialog();
                            }
                            else
                            {
                                client.Inventory.RemoveStackItem(729921, 1, stream);
                                if (MyMath.ChanceSuccess(70))//EggFragment
                                {
                                    client.Inventory.Add(stream, 729922 + _increment);

                                    Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                    packet.ID = MsgStringPacket.StringID.Effect;
                                    packet.UID = client.Player.UID;
                                    packet.Strings = new string[1] { "lottery" };
                                    client.Player.View.SendView(stream.StringPacketCreate(packet), true);

                                    string _city = "Twin  City";
                                    if (id == 4002)
                                        _city = "Phoenix Castle";
                                    else if (id == 4003)
                                        _city = "Ape City";
                                    else if (id == 4004)
                                        _city = "Desert City";
                                    else if (id == 4005)
                                        _city = "Bird Island";
                                    dialog.AddText("Congratulations you received the " + _city + " Egg Fragment!");
                                    dialog.AddOption("Thanks.", 255);
                                    dialog.FinalizeDialog();
                                }
                                else
                                {
                                    dialog.AddText("Ouch! The egg fragment was broken! Find me again for a new one!");
                                    dialog.AddOption("Damn", 255);
                                    dialog.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            dialog.AddText("Get a Feed from Commander in Twin City (303, 335) before talking to me!");
                            dialog.AddOption("I see", 255);
                            dialog.FinalizeDialog();
                        }
                        RemoveEgg(stream, client, id);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.EasterEgg2)]
        public static void EasterEgg2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Handle(client, stream, Option, Input, id);
        }

        [NpcAttribute(NpcID.EasterEgg3)]
        public static void EasterEgg3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Handle(client, stream, Option, Input, id);
        }

        [NpcAttribute(NpcID.EasterEgg4)]
        public static void EasterEgg4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Handle(client, stream, Option, Input, id);
        }

        [NpcAttribute(NpcID.EasterEgg5)]
        public static void EasterEgg5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Handle(client, stream, Option, Input, id);
        }
    }
}