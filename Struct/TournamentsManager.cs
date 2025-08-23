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

namespace TheChosenProject.Struct
{
    public struct TournamentsManager
    {
        public static uint MaxStones = 5;
        public static uint MaxVIP = 1;
        public static uint MaxGarment = 5;
        public static uint MaxRandomItem = 5;
        public static uint MaxSuperTG = 1;
        public static uint MaxRandomGem = 5;
        public static uint MaxAccessories = 5;
        public static uint MaxClass5MoneyBag = 5;
        public static uint MaxMegaMetsPack = 5;
        public static uint MaxMegaDBPack = 5;
        public static uint MaxPowerExpball = 5;

        public uint StonesTimes;

        public uint VIPTimes;

        public uint GarmentTimes;

        public uint RandomItemTimes;

        public uint SuperTGTimes;

        public uint AccessoriesTimes;

        public uint Class5MoneyBagTimes;

        public uint MegaMetsPackTimes;

        public uint PowerExpballTimes;


        public void GetDB(TournamentsManager _db)
        {
            this = _db;
        }

        public void Reset()
        {
            StonesTimes = 0;
            VIPTimes = 0;
            GarmentTimes = 0;
            RandomItemTimes = 0;
            SuperTGTimes = 0;
            AccessoriesTimes = 0;
            Class5MoneyBagTimes = 0;
            MegaMetsPackTimes = 0;
            PowerExpballTimes = 0;

        }

        public static void Reward(Client.GameClient client, uint id)
        {
            switch (id)
            {
                #region Stone +3
                case 1002:
                    {
                        if (client.Player.TournamentsPoints >= 30)
                        {
                            client.Player.TournamentsPoints -= 30;

                            client.TournamentsManager.StonesTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 730003, 1, 0, 0, 0);//1xStone+3

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Stone +3 ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Stone +3 ! from [Events Points Manager]!");

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
                #region VIP-1H
                case 1011:
                    {
                        if (client.Player.TournamentsPoints >= 30)
                        {
                            client.Player.TournamentsPoints -= 30;
                            client.TournamentsManager.VIPTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//13
                                stream = rec.GetStream();
                                client.Inventory.Add(stream, 780010, 1, 0, 0, 0);//vip 1 hour

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained VIP-1H ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained VIP-1H ! from [Events Points Manager]!");

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
                #region garments
                case 1020:
                    {
                        if (client.Player.TournamentsPoints >= 30)
                        {
                            client.Player.TournamentsPoints -= 30;
                            client.TournamentsManager.GarmentTimes += 1;

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
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained  " + Server.ItemsBase[ReceiveItem].Name + " accessories! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.SendSysMesage($"[Events Points] You just gained " + Server.ItemsBase[ReceiveItem].Name + " accessories! from [Events Points Manager]!");

                            }
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
                #region plus items
                case 1000:
                    {
                        {
                            if (client.Player.TournamentsPoints >= 30)
                            {
                                client.Player.TournamentsPoints -= 30;
                                client.TournamentsManager.RandomItemTimes += 1;

                                using (RecycledPacket rec = new RecycledPacket())
                                {
                                    Packet stream;//13
                                    stream = rec.GetStream();
                                    client.Inventory.Add(stream, 720145, 1, 0, 0, 0);//opbox
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained [OP]GearBox ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                                }
                                client.SendSysMesage($"[Events Points] You just gained [OP]GearBox ! from [Events Points Manager]!");

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
                    }
                #endregion
                #region Super TG
                case 1015:
                    {
                        if (client.Player.TournamentsPoints >= 90)
                        {
                            client.Player.TournamentsPoints -= 90;
                            client.TournamentsManager.SuperTGTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//13
                                stream = rec.GetStream();
                                client.Inventory.Add(stream, 700073, 1, 0, 0, 0);//1xSuperGem tg
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Super TortoiseGem ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Super TortoiseGem ! from [Events Points Manager]!");

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
                #region accessories
                case 1001:
                    {
                        if (client.Player.TournamentsPoints >= 30)
                        {
                            client.Player.TournamentsPoints -= 30;
                            client.TournamentsManager.AccessoriesTimes += 1;

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
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained  " + Server.ItemsBase[ReceiveItem].Name + " accessories! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.SendSysMesage($"[Events Points] You just gained " + Server.ItemsBase[ReceiveItem].Name + " accessories! from [Events Points Manager]!");

                            }

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
                #region PowerExpball
                case 10021:
                    {
                        if (client.Player.TournamentsPoints >= 10)
                        {
                            client.Player.TournamentsPoints -= 10;
                            client.TournamentsManager.PowerExpballTimes += 1;


                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 722057, 1, 0, 0, 0);//PowerExpball

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained PowerExpball ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained PowerExpball ! from [Events Points Manager]!");

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
                #region MegaMetsPack
                case 10111:
                    {
                        if (client.Player.TournamentsPoints >= 10)
                        {
                            client.Player.TournamentsPoints -= 10;
                            client.TournamentsManager.MegaMetsPackTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 720547, 1, 0, 0, 0);//MegaMetsPack

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained MegaMetsPack ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained MegaMetsPack ! from [Events Points Manager]!");

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
                #region Class5MoneyBag
                case 10201:
                    {
                        if (client.Player.TournamentsPoints >= 15)
                        {
                            client.Player.TournamentsPoints -= 15;

                            client.TournamentsManager.Class5MoneyBagTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 723717, 1, 0, 0, 0);//MegaDBPack

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Class5MoneyBag ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Class5MoneyBag ! from [Events Points Manager]!");

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
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\TournamentsManager\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\TournamentsManager\\");
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\TournamentsManager\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                TournamentsManager DB = new TournamentsManager();
                DB.GetDB(client.TournamentsManager);
                binary.Write(&DB, sizeof(TournamentsManager));
                binary.Close();
            }
        }

        public static unsafe void Load(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\TournamentsManager\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\TournamentsManager\\");
            client.TournamentsManager = new TournamentsManager();
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\TournamentsManager\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                TournamentsManager DB;
                binary.Read(&DB, sizeof(TournamentsManager));
                client.TournamentsManager.GetDB(DB);
                binary.Close();
            }
        }
    }
}