using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetGuildFastArsenal(this ServerSockets.Packet stream, out List<uint> Items)
        {
            Items = new List<uint>();
            int size = stream.ReadUInt8();
            for (int x = 0; x < size; x++)
            {
                Items.Add(stream.ReadUInt32());
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgGuildFastArsenal
    {
        [Packet(2204)]
        public static void FastArsenal(GameClient user, Packet stream)
        {
            stream.GetGuildFastArsenal(out var Items);
            stream.ReadUInt8();
            foreach (uint ItemUID in Items)
            {
                if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                    break;
                if (user.Inventory.TryGetItem(ItemUID, out var item))
                {
                    byte ArsenalTyp2;
                    ArsenalTyp2 = Guild.Arsenal.GetArsenalPosition(item.ITEM_ID);
                    if (ArsenalTyp2 == 0)
                        ArsenalTyp2 = 8;
                    if (item.Inscribed != 0)
                        break;
                    if (user.Player.MyGuild.MyArsenal.Add(ArsenalTyp2, new Guild.Arsenal.InscribeItem
                    {
                        BaseItem = item,
                        Name = user.Player.Name,
                        UID = user.Player.UID
                    }))
                    {
                        item.Inscribed = 1u;
                        item.Mode = Flags.ItemMode.Update;
                        item.Send(user, stream);
                        user.Player.MyGuildMember.ArsenalDonation += GetItemDonation(item);
                        user.Player.GuildBattlePower = user.Player.MyGuild.ShareMemberPotency(user.Player.MyGuildMember.Rank);
                    }
                }
                else if (user.Equipment.TryGetValue(ItemUID, out item))
                {
                    byte ArsenalTyp;
                    ArsenalTyp = Guild.Arsenal.GetArsenalPosition(item.ITEM_ID);
                    if (ArsenalTyp == 0)
                        ArsenalTyp = 8;
                    if (item.Inscribed != 0)
                        break;
                    if (user.Player.MyGuild.MyArsenal.Add(ArsenalTyp, new Guild.Arsenal.InscribeItem
                    {
                        BaseItem = item,
                        Name = user.Player.Name,
                        UID = user.Player.UID
                    }))
                    {
                        item.Inscribed = 1u;
                        item.Mode = Flags.ItemMode.Update;
                        item.Send(user, stream);
                        user.Player.MyGuildMember.ArsenalDonation += GetItemDonation(item);
                        user.Player.GuildBattlePower = user.Player.MyGuild.ShareMemberPotency(user.Player.MyGuildMember.Rank);
                    }
                }
            }
        }

        private static uint GetItemDonation(MsgGameItem GameItem)
        {
            uint Return;
            Return = 0u;
            switch ((int)(GameItem.ITEM_ID % 10u))
            {
                case 8:
                    Return = 1000u;
                    break;
                case 9:
                    Return = 16660u;
                    break;
            }
            if ((int)GameItem.SocketOne > 0 && GameItem.SocketTwo == Flags.Gem.NoSocket)
                Return += 33330;
            if ((int)GameItem.SocketOne > 0 && (int)GameItem.SocketTwo > 0)
                Return += 133330;
            switch (GameItem.Plus)
            {
                case 1:
                    Return += 90;
                    break;
                case 2:
                    Return += 490;
                    break;
                case 3:
                    Return += 1350;
                    break;
                case 4:
                    Return += 4070;
                    break;
                case 5:
                    Return += 12340;
                    break;
                case 6:
                    Return += 37030;
                    break;
                case 7:
                    Return += 111110;
                    break;
                case 8:
                    Return += 333330;
                    break;
                case 9:
                    Return += 1000000;
                    break;
                case 10:
                    Return += 1033330;
                    break;
                case 11:
                    Return += 1101230;
                    break;
                case 12:
                    Return += 1212340;
                    break;
            }
            return Return;
        }
    }
}
