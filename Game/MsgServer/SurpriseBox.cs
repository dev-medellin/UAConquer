using TheChosenProject.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Game.MsgTournaments;

namespace TheChosenProject.Game.MsgServer
{
    public class SurpriseBox
    {
        static List<uint> Easy = new List<uint>()
        {
            //753999,//999Orchids
            753099,//99Orchids
            754099,//999Tulips
            //780010,//1D-VIP
            720027,//MeteorScroll
            722057,//PowerEXPBall
            721080,
            721258,
            //1200000,
            //1200001,
            //1200002,
            //1100009,
            //1100006,
            191305,//goodluck
            751099,//999Tulips
            723727,//PenitenceAmulet
            720547,//MegaMetsPack
            //720135,//SuperGem
            //723410,
        };

        static List<uint> Mid = new List<uint>()
        {
            720028,//DBScroll
            720027,//MeteorScroll
            722057,//PowerEXPBall
            //1200000,
            //1200001,
            //1200002,
            //700103,//ThunderGemSuper
            //700123,//GloryGemSuper
            723727,//PenitenceAmulet
            //181355,//DarkWizard
            //181355,//DarkWizard
            191305,//GoodLuck
            754099,//999Tulips
            753099,//999Orchids
            //720135,//SuperGem
            751099,//999RedRoses
            721080,
            //780010,//1H-VIP
            721258,
            720547,//MegaMetsPack
            //730003//+3Stone

        };

        static List<uint> Hard = new List<uint>()
        {
            // Add some very rare items here
            //780010,//1H-VIP
            723727,//PenitenceAmulet
            181355,//DarkWizard
            1088000,
            721258,
            720547,//MegaMetsPack
            721080
            //721754,//5DonationPoints
            //700103,//ThunderGemSuper
            //700123,//GloryGemSuper
        };

        public static void GetReward(GameClient client, ServerSockets.Packet stream)
        {
            var randomValue = Role.Core.Random.NextDouble();

            if (randomValue < 0.7) // 70% chance for easy items
            {
                var reward = Easy[Role.Core.Random.Next(0, Easy.Count)];
                client.Inventory.Add(stream, reward, 1);
                client.SendSysMesage("You got a nice reward, check your inventory!");
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening SurpriseBox!", MsgMessage.ChatMode.Talk);
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening SurpriseBox!", MsgMessage.ChatMode.Center);
            }
            else if (randomValue < 0.95) // 25% chance for mid items
            {
                var reward = Mid[Role.Core.Random.Next(0, Mid.Count)];
                client.Inventory.Add(stream, reward, 1);
                client.SendSysMesage("You got a rare reward, check your inventory!");
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening SurpriseBox!", MsgMessage.ChatMode.Talk);
                MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening SurpriseBox!", MsgMessage.ChatMode.Center);
            }
            else // 5% chance for hard items
            {
                if (Hard.Count > 0)
                {
                    var reward = Hard[Role.Core.Random.Next(0, Hard.Count)];
                    client.Inventory.Add(stream, reward, 1);
                    client.SendSysMesage("You got a super rare reward, check your inventory!");
                    MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening SurpriseBox!", MsgMessage.ChatMode.Talk);
                    MsgSchedules.SendSysMesage(client.Player.Name + " got " + Database.Server.ItemsBase[reward].Name + " while opening SurpriseBox!", MsgMessage.ChatMode.Center);
                }
            }
        }
    }
}

