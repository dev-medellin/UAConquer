//* this project upgraded by walid mohamed fb/bn550 2020 - 2021
//* (Base Alex)
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using static TheChosenProject.Game.MsgServer.MsgSubClass;

namespace TheChosenProject
{
    public class RewardsFunctions
    {
        public static uint[] Garments = new uint[]
        {
            181395, 181345, 181455,
            181485, 181575, 183325,
            183315, 183375, 183305,
            191305, 181325, 181425,
            181525, 181625, 181725,
            181825, 182305, 181365,
            182305
        };
        public static bool SwitchID(Client.GameClient client, Game.MsgServer.MsgGameItem ItemDat)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                switch (ItemDat.ITEM_ID)
                {
                    case 724002://LotteryTickets
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Update(ItemDat, Role.Instance.AddMode.REMOVE, stream);
                                client.Inventory.AddItemWitchStack(711504, 0, 3, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "lottery");
                                client.SendSysMesage("You opened the Small Lottery Ticket Packet and received 3 Small Lottery Tickets!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            else
                            {
                                client.SendSysMesage("Please make 1 more spaces in your inventory.");
                            }
                            return true;
                        }
                }
                return false;
            }
        }

        public static uint StoneId(UInt16 Plus)
        {
            switch (Plus)
            {
                case 1: return 730001;
                case 2: return 730002;
                case 3: return 730003;
                case 4: return 730004;
                case 5: return 730005;
                case 6: return 730006;
                case 7: return 730007;
                case 8: return 730008;
            }
            return 0;
        }

        public static void RewardWars(Client.GameClient client, ServerSockets.Packet stream, string warname, ref string msg)
        {
            //if (warname == "GuildWar")
            //{
            //    if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
            //    {
            //        client.Player.Money += 10000000;
            //        msg = $"{Role.Core.WriteNumber(10000000)} Gold, ";
            //        client.Player.ConquerPoints += 2150;
            //        msg = $"{Role.Core.WriteNumber(2150)} Cps, ";
            //        client.Inventory.Add(stream, 2100095, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 7, 0, 0);
            //        msg = $"GoldPrize(7-Day).";
            //    }
            //    else if (client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader)
            //    {
            //        client.Player.Money += 5000000;
            //        msg = $"{Role.Core.WriteNumber(5000000)} Gold, ";
            //        client.Player.ConquerPoints += 1000;
            //        msg = $"{Role.Core.WriteNumber(1000)} Cps, ";
            //        client.Inventory.Add(stream, 2100065, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, false, "", 7, 0, 0);
            //        msg = $"SilverPrize(7-Day).";
            //    }
            //}
            //else if (warname == "EliteGuildWar")
            //{
            //    if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
            //    {
            //        client.Player.Money += 3000000;
            //        msg = $"{Role.Core.WriteNumber(3000000)} Gold, ";
            //        client.Player.ConquerPoints += 300;
            //        msg = $"{Role.Core.WriteNumber(300)} Cps.";
            //    }
            //    else if (client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader)
            //    {
            //        client.Player.Money += 1000000;
            //        msg = $"{Role.Core.WriteNumber(1000000)} Gold, ";
            //        client.Player.ConquerPoints += 100;
            //        msg = $"{Role.Core.WriteNumber(100)} Cps.";
            //    }
            //}
            //else if (warname == "ClanWar")
            //{
            //    if (client.Player.ClanRank == (ushort)Role.Instance.Clan.Ranks.Leader)
            //    {
            //        client.Player.ConquerPoints += 3000;
            //        client.SendSysMesage(2005, "Congratulations you received a 3,000 Cps!");
            //        msg = $"3,000 Cps.";
            //    }
            //}
            //else
            if (warname == "CouplesTournament"/* || warname == "WeeklyPK" || warname == "ClassPk"*/)
            {
                client.Player.Money += 5000000;
                client.SendSysMesage("Congratulations you received a 5,000,000 ConquerMoney!");
                msg = $"5,000,000 ConquerMoney.";
            }
            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "lottery");
        }
    }
}