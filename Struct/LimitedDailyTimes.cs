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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TheChosenProject.Struct
{
    public struct LimitedDailyTimes
    {
        

        public uint ExchangeTG;
       
       
      

        public void GetDB(LimitedDailyTimes _db)
        {
            this = _db;
        }

        public void Reset()
        {
            ExchangeTG = 0;
            

        }

        public static void Reward(Client.GameClient client, uint id)
        {
            switch (id)
            {
                #region Super TG
                case 100:
                    {
                        if (client.Inventory.Contain(700003, 1u, 0)
                        && client.Inventory.Contain(700013, 1u, 0)
                        && client.Inventory.Contain(700023, 1u, 0)
                        && client.Inventory.Contain(700033, 1u, 0)
                        && client.Inventory.Contain(700043, 1u, 0)
                        && client.Inventory.Contain(700053, 1u, 0)
                        && client.Inventory.Contain(700063, 1u, 0)
                        && client.Inventory.Contain(700102, 1u, 0)
                        && client.Inventory.Contain(700122, 1u, 0)
                        && client.Player.ConquerPoints >= 250000000)
                        {
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream;
                                stream = recycledPacket8.GetStream();
                                client.LimitedDailyTimes.ExchangeTG += 1;
                                client.Inventory.Remove(700003, 1u, stream);
                                client.Inventory.Remove(700013, 1u, stream);
                                client.Inventory.Remove(700023, 1u, stream);
                                client.Inventory.Remove(700033, 1u, stream);
                                client.Inventory.Remove(700043, 1u, stream);
                                client.Inventory.Remove(700053, 1u, stream);
                                client.Inventory.Remove(700063, 1u, stream);
                                client.Inventory.Remove(700102, 1u, stream);
                                client.Inventory.Remove(700122, 1u, stream);
                                client.Player.ConquerPoints -= 250000000;
                                client.Inventory.Add(stream, 700073, 1, 0, 0, 0);// suepr Tortoise Gem

                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Exchange Tortoise Gem] You just gained Super Tortoise Gem ! from [Exchange Gem Manager]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                            }
                            client.SendSysMesage($"[Exchange Tortoise Gem] You just gained Super Tortoise Gem ! from [Exchange Gem Manager]!");

                           
                        }
                        else
                            client.SendSysMesage("I~believe~you've~missed~some~gem~or~so,~please~come~again~with~7 different type of super gem~");
                        break;
                    }
                #endregion
                
                
            }
        }

        public static unsafe void Save(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\LimitedDailyTimes\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\LimitedDailyTimes\\");
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\LimitedDailyTimes\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                LimitedDailyTimes DB = new LimitedDailyTimes();
                DB.GetDB(client.LimitedDailyTimes);
                binary.Write(&DB, sizeof(LimitedDailyTimes));
                binary.Close();
            }
        }

        public static unsafe void Load(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\LimitedDailyTimes\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\LimitedDailyTimes\\");
            client.LimitedDailyTimes = new LimitedDailyTimes();
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\LimitedDailyTimes\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                LimitedDailyTimes DB;
                binary.Read(&DB, sizeof(LimitedDailyTimes));
                client.LimitedDailyTimes.GetDB(DB);
                binary.Close();
            }
        }
    }
}