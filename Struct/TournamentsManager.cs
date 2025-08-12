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
        public static uint MaxGarment = 5;
        public static uint MaxRandomItem = 5;
        public static uint MaxPenitence = 3;
        public static uint MaxAccessories = 5;
        public static uint MaxLevelExpTimes = 5;

        public static uint MaxPromotionStones = 3;
        public static uint MaxDBPieces = 5;
        public static uint MaxMagicBall = 5;
        public static uint MaxGuildClan = 5;


        //ArtoksVersion
        public uint PromotionStonesTimes;
        public uint DBPiecesTimes;
        public uint MagicBallTimes;

        public uint GarmentTimes;
        
        public uint RandomItemTimes;
       
        public uint PenitenceTimes;
        
        public uint AccessoriesTimes;

        public uint GuildClanTimes;

        public uint LevelExpTimes;

        public void GetDB(TournamentsManager _db)
        {
            this = _db;
        }

        public void Reset()
        {
            GuildClanTimes = 0;
            DBPiecesTimes = 0;
            LevelExpTimes = 0;
            PromotionStonesTimes = 0;
            GarmentTimes = 0;
            RandomItemTimes = 0;
            PenitenceTimes = 0;
            AccessoriesTimes = 0;

        }

        public static void Reward(Client.GameClient client, uint id)
        {
            switch (id)
            {
                case 9991:
                    if (client.Player.TournamentsPoints >= 20)
                    {
                        client.Player.TournamentsPoints -= 20;

                        client.TournamentsManager.LevelExpTimes += 1;

                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;//10
                            stream = rec.GetStream();


                            client.Inventory.Add(stream, 722057, 1, 0, 0, 0);//PowerBall

                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained PowerExpBall ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                        }
                        client.SendSysMesage($"[Events Points] You just gained PowerExpBall ! from [Events Points Manager]!");

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "perfect");

                        }
                    }
                    else
                        client.SendSysMesage("You don`t have enough points.");
                    break;
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
                #region Penitence
                case 1015:
                    {
                        if (client.Player.TournamentsPoints >= 10)
                        {
                            client.Player.TournamentsPoints -= 10;
                            client.TournamentsManager.PenitenceTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//13
                                stream = rec.GetStream();
                                client.Inventory.Add(stream, 720128, 1, 0, 0, 0);//PenitenceAmulet x1
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained PenitenceAmulet ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained PenitenceAmulet ! from [Events Points Manager]!");

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
                #region MagicBall
                //case 10021:
                //    {
                //        if (client.Player.TournamentsPoints >= 10)
                //        {
                //            client.Player.TournamentsPoints -= 10;
                //            client.TournamentsManager.MagicBallTimes += 1;


                //            using (RecycledPacket rec = new RecycledPacket())
                //            {
                //                Packet stream;//10
                //                stream = rec.GetStream();


                //                client.Inventory.Add(stream, 720668, 1, 0, 0, 0);//MagicBall

                //                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained MagicBall[EXP] ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                //            }
                //            client.SendSysMesage($"[Events Points] You just gained MagicBall[EXP] ! from [Events Points Manager]!");

                //            using (var rec = new ServerSockets.RecycledPacket())
                //            {
                //                var stream = rec.GetStream();
                //                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "perfect");

                //            }
                //        }
                //        else
                //            client.SendSysMesage("You don`t have enough points.");
                //        break;
                //    }
                #endregion
                case 810032:
                    {
                        if (client.Player.TournamentsPoints >= 20)
                        {
                            client.Player.TournamentsPoints -= 20;

                            client.TournamentsManager.PromotionStonesTimes += 1;

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
                        if (client.Player.TournamentsPoints >= 20)
                        {
                            client.Player.TournamentsPoints -= 20;

                            client.TournamentsManager.PromotionStonesTimes += 1;

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
                        if (client.Player.TournamentsPoints >= 20)
                        {
                            client.Player.TournamentsPoints -= 20;

                            client.TournamentsManager.PromotionStonesTimes += 1;

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
                case 710834:
                    {
                        if (client.Player.TournamentsPoints >= 10)
                        {
                            client.Player.TournamentsPoints -= 10;

                            client.TournamentsManager.DBPiecesTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 1088000, 1, 0, 0, 0);//DragonballPiece

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Dragonball ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Dragonball ! from [Events Points Manager]!");

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
                case 2095:
                    {
                        if (client.Player.TournamentsPoints >= 25)
                        {
                            client.Player.TournamentsPoints -= 25;

                            client.TournamentsManager.GuildClanTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 102095, 1, 0, 0, 0);//GuildCertificate

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Guild Certificate ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Guild Certificate ! from [Events Points Manager]!");

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
                case 0031:
                    {
                        if (client.Player.TournamentsPoints >= 25)
                        {
                            client.Player.TournamentsPoints -= 25;

                            client.TournamentsManager.GuildClanTimes += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 810031, 1, 0, 0, 0);//GuildCertificate

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Events Points]" + client.Player.Name + " You just gained Clan Certificate ! from [Events Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                            }
                            client.SendSysMesage($"[Events Points] You just gained Clan Certificate ! from [Events Points Manager]!");

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