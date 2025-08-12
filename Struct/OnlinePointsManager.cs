using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;
using TheChosenProject.Role;
using DevExpress.ClipboardSource.SpreadsheetML;
using System.Xml.Linq;

namespace TheChosenProject.Struct
{
    public struct OnlinePointsManager
    {
        public static uint MaxEuxeOres = 5;
        public static uint MaxStones = 3;
        public static uint MaxVIP = 3;
        public static uint MaxGarment = 1;
        public static uint MaxRandomItem = 3;
        public static uint MaxSuperTG = 1;
        public static uint MaxRandomGem = 3;
        public static uint MaxAccessories = 1;
        public static uint MaxClass5MoneyBag = 5;
        public static uint MaxMegaMetsPack = 10;
        public static uint MaxMegaDBPack = 3;
        public static uint MaxPowerExpball = 10;
        public static uint MaxExpPot = 2;
        public static uint MaxPromotionStones = 3;
        public static uint MaxGuildCertificate = 1;
        public static uint MaxClanCertificate = 1;
        public uint EuxeOreTimes;

        public uint ExpPotTimes;

        public uint StonesTimes;

        public uint VIPTimes;

        public uint GarmentTimes;

        public uint RandomItemTimes;

        public uint SuperTGTimes;

        public uint AccessoriesTimes;

        public uint Class5MoneyBagTimes;

        public uint MegaMetsPackTimes;

        public uint PowerExpballTimes;
        public uint PromotionStonesTimes;

        public uint GuildCertificate;

        public uint ClanCertificate;


        public void GetDB(OnlinePointsManager _db)
        {
            this = _db;
        }

        public void Reset()
        {
            PromotionStonesTimes = 0;
            EuxeOreTimes = 0;
            ExpPotTimes = 0;
            StonesTimes = 0;
            VIPTimes = 0;
            GarmentTimes = 0;
            RandomItemTimes = 0;
            SuperTGTimes = 0;
            AccessoriesTimes = 0;
            Class5MoneyBagTimes = 0;
            MegaMetsPackTimes = 0;
            PowerExpballTimes = 0;
            GuildCertificate = 0;
            ClanCertificate = 0;
        }

        public static void Reward(Client.GameClient client, uint id)
        {
            switch (id)
            {
                #region GuildCertificates
                case 102095:
                    {
                        if (client.Player.OnlinePoints >= 300)
                        {
                            client.Player.OnlinePoints -= 300;
                            client.OnlinePointsManager.GuildCertificate += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 102095, 1, 0, 0, 0);//ExpPotTimes

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained GuildCertification ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[OnlinePoints] You just gained GuildCertification ! from [OnlinePoints Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                            string logs = "[OnlinePoints]" + client.Player.Name + " You just gained GuildCertification ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                            //  logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                case 810031:
                    {
                        if (client.Player.OnlinePoints >= 300)
                        {
                            client.Player.OnlinePoints -= 300;
                            client.OnlinePointsManager.ClanCertificate += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 810031, 1, 0, 0, 0);//ClanCertificate

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained ClanCertificate ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[OnlinePoints] You just gained ClanCertificate ! from [OnlinePoints Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                            string logs = "[OnlinePoints]" + client.Player.Name + " You just gained ClanCertificate ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                            //  logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region ExpPotTimes
                case 1002:
                    {
                        if (client.Player.OnlinePoints >= 2)
                        {
                            client.Player.OnlinePoints -= 2;
                            client.OnlinePointsManager.ExpPotTimes += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 723017, 1, 0, 0, 0);//ExpPotTimes

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained ExpPotion ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[OnlinePoints] You just gained ExpPotion ! from [OnlinePoints Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                            string logs = "[OnlinePoints]" + client.Player.Name + " You just gained ExpPotion ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                            //  logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region VIP-1H
                case 1011:
                    {
                        if (client.Player.OnlinePoints >= 480)
                        {
                            client.Player.OnlinePoints -= 480;
                            client.OnlinePointsManager.VIPTimes += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }
                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//13
                                stream = rec.GetStream();
                                client.Player.AddVIPLevel(6, DateTime.Now.AddMinutes(60), stream);
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("#07#42 It was a pleasure to negotiate with [" + client.Player.Name + "], you he actived VIP 6 for 60 minutes from OnlinePoints.", "ALLUSERS", "[System]", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just actived VIP 6 for 1 Hour ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[OnlinePoints] You just gained VIP-1H ! from [OnlinePoints Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                            string logs = "[OnlinePoints]" + client.Player.Name + " You just gained VIP-1H ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                            //  logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region garments
                case 1020:
                    {
                        if (client.Player.OnlinePoints >= 300)
                        {
                            client.Player.OnlinePoints -= 300;
                            client.OnlinePointsManager.GarmentTimes += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }
                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//12
                                stream = rec.GetStream();
                                byte plus = 0;

                                var RndAccessory = Database.ItemType.RareGarmentsOP;
                                var Position = Program.GetRandom.Next(0, RndAccessory.Count);
                                var ReceiveItem = RndAccessory.ToArray()[Position];
                                client.Inventory.Add(stream, ReceiveItem, 1, (byte)plus);
                                Game.MsgServer.MsgGameItem DataItem = new Game.MsgServer.MsgGameItem();
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained  " + Server.ItemsBase[ReceiveItem].Name + " accessories! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.SendSysMesage($"[OnlinePoints] You just gained " + Server.ItemsBase[ReceiveItem].Name + " accessories! from [OnlinePoints Manager]!");
                                string logs = "[OnlinePoints]" + client.Player.Name + " You just gained " + Server.ItemsBase[ReceiveItem].Name + " accessories ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                                //Program.DiscordOnlinePoints.Enqueue(logs);
                                //  logs += Environment.StackTrace;
                                Database.ServerDatabase.LoginQueue.Enqueue(logs);
                            }
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }

                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region plus items
                case 1000:
                    {
                        {
                            if (client.Player.OnlinePoints >= 360)
                            {
                                client.Player.OnlinePoints -= 360;
                                client.OnlinePointsManager.RandomItemTimes += 1;
                                using (RecycledPacket recycledPacket8 = new RecycledPacket())
                                {
                                    Packet stream;
                                    stream = recycledPacket8.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                                }
                                using (RecycledPacket rec = new RecycledPacket())
                                {
                                    Packet stream;//13
                                    stream = rec.GetStream();
                                    client.Inventory.Add(stream, 720145, 1, 0, 0, 0);//opbox
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained [OP]GearBox ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                                }
                                client.SendSysMesage($"[OnlinePoints] You just gained [OP]GearBox ! from [OnlinePoints Manager]!");

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                                }
                                string logs = "[OnlinePoints]" + client.Player.Name + " You just gained [OP]GearBox ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                                //  logs += Environment.StackTrace;
                                Database.ServerDatabase.LoginQueue.Enqueue(logs);
                            }
                            else
                                client.SendSysMesage("You don`t have enough points.");
                            break;
                        }
                    }
                #endregion
                #region Super TG
                //case 1015:
                //    {
                //        if (client.Player.OnlinePoints >= 720)
                //        {
                //            client.Player.OnlinePoints -= 720;
                //            client.OnlinePointsManager.SuperTGTimes += 1;
                //            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                //            {
                //                Packet stream;
                //                stream = recycledPacket8.GetStream();
                //                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                //            }
                //            using (RecycledPacket rec = new RecycledPacket())
                //            {
                //                Packet stream;//13
                //                stream = rec.GetStream();
                //                client.Inventory.Add(stream, 700073, 1, 0, 0, 0);//1xSuperGem tg
                //                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name +" You just gained Super TortoiseGem ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                //            }
                //            client.SendSysMesage($"[OnlinePoints] You just gained Super TortoiseGem ! from [OnlinePoints Manager]!");

                //            using (var rec = new ServerSockets.RecycledPacket())
                //            {
                //                var stream = rec.GetStream();
                //                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                //            }
                //            string logs = "[OnlinePoints]" + client.Player.Name + " You just gained TortoiseGem ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                //            //  logs += Environment.StackTrace;
                //            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        }
                //        else
                //        client.SendSysMesage("You don`t have enough points.");
                //    break;
                //    }
                #endregion
                #region accessories
                case 1001:
                    {
                        if (client.Player.OnlinePoints >= 300)
                        {
                            client.Player.OnlinePoints -= 300;
                            client.OnlinePointsManager.AccessoriesTimes += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }
                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//12
                                stream = rec.GetStream();
                                byte plus = 0;

                                var RndAccessory = Database.ItemType.AccessoriesOP;
                                var Position = Program.GetRandom.Next(0, RndAccessory.Count);
                                var ReceiveItem = RndAccessory.ToArray()[Position];
                                client.Inventory.Add(stream, ReceiveItem, 1, (byte)plus);
                                Game.MsgServer.MsgGameItem DataItem = new Game.MsgServer.MsgGameItem();
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained  " + Server.ItemsBase[ReceiveItem].Name + " accessories! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.SendSysMesage($"[OnlinePoints] You just gained " + Server.ItemsBase[ReceiveItem].Name + " accessories! from [OnlinePoints Manager]!");
                                string logs = "[OnlinePoints]" + client.Player.Name + " You just gained " + Server.ItemsBase[ReceiveItem].Name + " accessories ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                                //Program.DiscordOnlinePoints.Enqueue(logs);
                                //  logs += Environment.StackTrace;
                                Database.ServerDatabase.LoginQueue.Enqueue(logs);
                            }

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region PowerExpball
                case 10021:
                    {
                        if (client.Player.OnlinePoints >= 144)
                        {
                            client.Player.OnlinePoints -= 144;
                            client.OnlinePointsManager.PowerExpballTimes += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 722057, 1, 0, 0, 0);//PowerExpball

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained PowerExpball ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[OnlinePoints] You just gained PowerExpball ! from [OnlinePoints Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                            string logs = "[OnlinePoints]" + client.Player.Name + " You just gained PowerExpball  ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                            //  logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region MegaMetsPack
                case 10111:
                    {
                        if (client.Player.OnlinePoints >= 144)
                        {
                            client.Player.OnlinePoints -= 144;
                            client.OnlinePointsManager.MegaMetsPackTimes += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 720547, 1, 0, 0, 0);//MegaMetsPack

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained MegaMetsPack ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[OnlinePoints] You just gained MegaMetsPack ! from [OnlinePoints Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                            string logs = "[OnlinePoints]" + client.Player.Name + " You just gained MegaMetsPack  ! from [OnlinePoints Manager]" + DateTime.Now + "!!";
                            //  logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region Class5MoneyBag
                case 10201:
                    {
                        if (client.Player.OnlinePoints >= 288)
                        {
                            client.Player.OnlinePoints -= 288;
                            client.OnlinePointsManager.Class5MoneyBagTimes += 1;
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.Player.SendUpdate(stream, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                            }

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 723717, 1, 0, 0, 0);//MegaDBPack

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[OnlinePoints]" + client.Player.Name + " You just gained Class5MoneyBag ! from [OnlinePoints Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[OnlinePoints] You just gained Class5MoneyBag ! from [OnlinePoints Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                            string logs = "[OnlinePoints]" + client.Player.Name + " You just gained Class5MoneyBag  ! from [OnlinePoints Manager]" + DateTime.Now + "!";
                            //  logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                #endregion
                #region PromotionStone
                case 810032:
                    {
                        if (client.Player.OnlinePoints >= 20)
                        {
                            client.Player.OnlinePoints -= 20;

                            client.OnlinePointsManager.PromotionStonesTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 810032, 1, 0, 0, 0);//PromotionStones(Valor)

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Valorstone ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Valorstone ! from [Events Points Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "perfect");

                            }
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                case 810033:
                    {
                        if (client.Player.OnlinePoints >= 20)
                        {
                            client.Player.OnlinePoints -= 20;

                            client.OnlinePointsManager.PromotionStonesTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 810033, 1, 0, 0, 0);//PromotionStones(Valor)

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Windshard ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Windshard ! from [Events Points Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "perfect");

                            }
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                case 810034:
                    {
                        if (client.Player.OnlinePoints >= 20)
                        {
                            client.Player.OnlinePoints -= 20;

                            client.OnlinePointsManager.PromotionStonesTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 810034, 1, 0, 0, 0);//PromotionStones(Valor)

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Mystara ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Mystara ! from [Events Points Manager]!");

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "perfect");

                            }
                        }
                        else
                            client.SendSysMesage("You don`t have enough points.");
                        break;
                    }
                    #endregion

            }
        }

        public static unsafe void Save(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\OnlineManager\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\OnlineManager\\");
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\OnlineManager\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                OnlinePointsManager DB = new OnlinePointsManager();
                DB.GetDB(client.OnlinePointsManager);
                binary.Write(&DB, sizeof(OnlinePointsManager));
                binary.Close();
            }
        }

        public static unsafe void Load(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\OnlineManager\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\OnlineManager\\");
            client.OnlinePointsManager = new OnlinePointsManager();
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\OnlineManager\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                OnlinePointsManager DB;
                binary.Read(&DB, sizeof(OnlinePointsManager));
                client.OnlinePointsManager.GetDB(DB);
                binary.Close();
            }
        }
    }
}