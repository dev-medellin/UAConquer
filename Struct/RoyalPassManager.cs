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
    public struct RoyalPassManager
    {
        public static uint MaxStonesTimesThree = 5;
        public static uint MaxStonesTimesFour = 4;
        public static uint MaxStonesTimesFive = 3;
        public static uint MaxStonesTimesSix = 2;
        public static uint MaxSuperTG = 1;
        public static uint MaxStonesTimesSeven = 1;
       

        public uint StonesTimesThree;
       
        public uint StonesTimesFour;
       
        public uint StonesTimesFive;
        
        public uint StonesTimesSix;
       
        public uint SuperTGTimes;
        
        public uint StonesTimesSeven;

        
      

        public void GetDB(RoyalPassManager _db)
        {
            this = _db;
        }

        public void Reset()
        {
            StonesTimesThree = 0;
            StonesTimesFour = 0;
            StonesTimesFive = 0;
            SuperTGTimes = 0;
            StonesTimesSix = 0;
            StonesTimesSeven = 0;         
        }

        public static void Reward(Client.GameClient client, uint id)
        {
            return;
            switch (id)
            {
                #region +3
                case 1002:
                    {
                        if (client.Player.RoyalPassPoints >= 10)
                        {
                            client.Player.RoyalPassPoints -= 10;

                            client.RoyalPassManager.StonesTimesThree += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//10
                                stream = rec.GetStream();


                                client.Inventory.Add(stream, 720028, 1, 0, 0, 0);//1xStone+3

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Royal Pass Points]" + client.Player.Name + " You just gained DBScroll ! from [Royal Pass Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.SendSysMesage($"[Royal Pass Points] You just gained DBScroll ! from [Royal Pass Points Manager]!");

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
                #region +4
                case 1011:
                    {
                        if (client.Player.RoyalPassPoints >= 30)
                        {
                            client.Player.RoyalPassPoints -= 30;
                            client.RoyalPassManager.StonesTimesFour += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//13
                                stream = rec.GetStream();
                                client.Inventory.Add(stream, 730004, 1, 0, 0, 0);//1xStone+3

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Royal Pass Points]" + client.Player.Name + " You just gained Stone +4 ! from [Royal Pass Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.SendSysMesage($"[Royal Pass Points] You just gained Stone +4 ! from [Royal Pass Points Manager]!");

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
                #region +5
                case 1020:
                    {
                        if (client.Player.RoyalPassPoints >= 90)
                        {
                            client.Player.RoyalPassPoints -= 90;
                            client.RoyalPassManager.StonesTimesFive += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//12
                                stream = rec.GetStream();
                                client.Inventory.Add(stream, 730005, 1, 0, 0, 0);//1xStone+3

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Royal Pass Points]" + client.Player.Name + " You just gained Stone +5 ! from [Royal Pass Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.SendSysMesage($"[Royal Pass Points] You just gained Stone +5 ! from [Royal Pass Points Manager]!");

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
                #region +6
                case 1000:
                    {
                        {
                            if (client.Player.RoyalPassPoints >= 260)
                            {
                                client.Player.RoyalPassPoints -= 260;
                                client.RoyalPassManager.StonesTimesSix += 1;

                                using (RecycledPacket rec = new RecycledPacket())
                                {
                                    Packet stream;//13
                                    stream = rec.GetStream();
                                    client.Inventory.Add(stream, 730006, 1, 0, 0, 0);//1xStone+3
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Royal Pass Points]" + client.Player.Name + " You just gained Stone +6 ! from [Royal Pass Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                    client.SendSysMesage($"[Royal Pass Points] You just gained Stone +6 ! from [Royal Pass Points Manager]!");

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
                    }
                #endregion
                #region Super TG
                //case 1015:
                //    {
                //        if (client.Player.RoyalPassPoints >= 90)
                //        {
                //            client.Player.RoyalPassPoints -= 90;
                //            client.RoyalPassManager.SuperTGTimes += 1;

                //            using (RecycledPacket rec = new RecycledPacket())
                //            {
                //                Packet stream;//13
                //                stream = rec.GetStream();
                //                client.Inventory.Add(stream, 700073, 1, 0, 0, 0);//1xSuperGem tg
                //                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Royal Pass Points]" + client.Player.Name +" You just gained Super TortoiseGem ! from [Royal Pass Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                //            }
                //            client.SendSysMesage($"[Royal Pass Points] You just gained Super TortoiseGem ! from [Royal Pass Points Manager]!");

                //            using (var rec = new ServerSockets.RecycledPacket())
                //            {
                //                var stream = rec.GetStream();
                //                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "perfect");

                //            }
                //        }
                //        else
                //        client.SendSysMesage("You don`t have enough points.");
                //    break;
                //    }
                #endregion
                #region +7
                case 1001:
                    {
                        if (client.Player.RoyalPassPoints >= 810)
                        {
                            client.Player.RoyalPassPoints -= 810;
                            client.RoyalPassManager.StonesTimesSeven += 1;

                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;//12
                                stream = rec.GetStream();
                                client.Inventory.Add(stream, 730007, 1, 0, 0, 0);//1xStone+3
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Royal Pass Points]" + client.Player.Name + " You just gained Stone +7 ! from [Royal Pass Points Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.SendSysMesage($"[Royal Pass Points] You just gained Stone +7 ! from [Royal Pass Points Manager]!");

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
             
                
            }
        }

        public static unsafe void Save(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\RoyalPassManager\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\RoyalPassManager\\");
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\RoyalPassManager\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                RoyalPassManager DB = new RoyalPassManager();
                DB.GetDB(client.RoyalPassManager);
                binary.Write(&DB, sizeof(RoyalPassManager));
                binary.Close();
            }
        }

        public static unsafe void Load(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\RoyalPassManager\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\RoyalPassManager\\");
            client.RoyalPassManager = new RoyalPassManager();
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\RoyalPassManager\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                RoyalPassManager DB;
                binary.Read(&DB, sizeof(RoyalPassManager));
                client.RoyalPassManager.GetDB(DB);
                binary.Close();
            }
        }
    }
}