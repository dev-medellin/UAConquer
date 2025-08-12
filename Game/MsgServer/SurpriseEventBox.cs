using TheChosenProject.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Game.MsgTournaments;

namespace TheChosenProject.Game.MsgServer
{
    public class SurpriseEventBox
    {
        static List<uint> High = new List<uint>()
        {
            1088000, 1200001, 2100045, 720027, 721080, 723017, 723715,
            730002, 730002
        };
        static List<uint> Mid = new List<uint>()
        {
            730002, 730002, 730004, 723744, 723717,
            780010
        };
        #region SuperGems
        public static List<uint> SuperGems = new List<uint>()
        {
            700013,//DragonGem
            700003,//PhoenixGem
            700023,//FuryGem
            700033,//RainbowGem
            700043,//KylinGem
            700053,//VioletGem
            700063,//MoonGem

        };
        #endregion
        public static void GetReward(GameClient client, ServerSockets.Packet stream)
        {
            return;
            if (Role.MyMath.Success(0.5))
            {
                var reward = SuperGems[Role.Core.Random.Next(0, Mid.Count)];
                client.Inventory.Add(stream, reward);
                client.SendSysMesage("You got a nice reward check your inventory");
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening Surprise EventBox!", MsgMessage.ChatMode.Talk);
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening Surprise EventBox!", MsgMessage.ChatMode.Center);

            }
            else if (Role.MyMath.Success(10))
            {
                var reward = Mid[Role.Core.Random.Next(0, Mid.Count)];
                client.Inventory.Add(stream, reward, 1);
                client.SendSysMesage("You got a nice reward check your inventory");
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening Surprise EventBox!", MsgMessage.ChatMode.Talk);
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening Surprise EventBox!", MsgMessage.ChatMode.Center);

            }
            else
            {
                var reward = High[Role.Core.Random.Next(0, High.Count)];
                client.Inventory.Add(stream, reward, 1);
                client.SendSysMesage("You got a nice reward check your inventory");
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening Surprise EventBox!", MsgMessage.ChatMode.Talk);
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening Surprise EventBox!", MsgMessage.ChatMode.Center);

            }
        }
    }
}
