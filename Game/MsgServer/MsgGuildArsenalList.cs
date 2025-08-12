using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetArsenalList(this ServerSockets.Packet stream, out uint Type
            , out uint BeginAt, out uint EndAt, out byte ArsenalTyp)
        {
            Type = stream.ReadUInt32();
            BeginAt = stream.ReadUInt32();
            EndAt = stream.ReadUInt32();
            ArsenalTyp = stream.ReadUInt8();
        }
    }
    public class MsgGuildArsenalList
    {
        public static byte GetBattlePowerItem(MsgGameItem item)
        {
            byte potBase;
            potBase = 0;
            byte Quality;
            Quality = (byte)(item.ITEM_ID % 10u);
            if (Quality >= 5)
                potBase = (byte)(potBase + (byte)(Quality - 5));
            potBase = (byte)(potBase + item.Plus);
            if (item.SocketOne != 0)
                potBase = (byte)(potBase + 1);
            if (item.SocketTwo != 0)
                potBase = (byte)(potBase + 1);
            if ((int)item.SocketOne % 10 == 3)
                potBase = (byte)(potBase + 1);
            if ((int)item.SocketTwo % 10 == 3)
                potBase = (byte)(potBase + 1);
            return potBase;
        }

        [Packet(2202)]
        public static void Handler(GameClient user, Packet stream)
        {
            if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                return;
            try
            {
                DateTime Now;
                Now = default(DateTime).AddDays(30.0);
                int ExpireEnchant;
                ExpireEnchant = Now.Year * 10000 + Now.Month * 100 + Now.Day;
                stream.GetArsenalList(out var Type, out var BeginAt, out var _, out var ArsenalTyp);
                if (ArsenalTyp == 0)
                    ArsenalTyp = 8;
                Guild.Arsenal.Arsenals Arsenal;
                Arsenal = user.Player.MyGuild.MyArsenal.GetArsenal(ArsenalTyp);
                if (Arsenal == null)
                    return;
                Guild.Arsenal.InscribeItem[] items;
                items = Arsenal.DescreasedItems;
                if (items == null || items.Length == 0)
                    return;
                uint Start_index;
                Start_index = BeginAt - 1;
                uint count;
                count = (uint)Math.Min(8L, items.Length - Start_index);
                if (items.Length > Start_index)
                {
                    stream.InitWriter();
                    stream.Write(Type);
                    stream.Write(BeginAt);
                    stream.Write(BeginAt + count);
                    stream.Write((uint)ArsenalTyp);
                    stream.Write((uint)items.Length);
                    stream.Write((uint)Arsenal.GetPotency);
                    stream.Write((uint)Arsenal.Enchant);
                    stream.Write(ExpireEnchant);
                    stream.Write((uint)Arsenal.GetDonation);
                    stream.Write(count);
                    uint i;
                    i = BeginAt;
                    for (uint x = 0u; x < count && x < items.Length; x++)
                    {
                        Guild.Arsenal.InscribeItem client;
                        client = items[x + Start_index];
                        stream.Write(client.BaseItem.UID);
                        stream.Write(i);
                        stream.Write(client.Name, 16);
                        stream.Write(client.BaseItem.ITEM_ID);
                        stream.Write((byte)(client.BaseItem.ITEM_ID % 10u));
                        stream.Write(client.BaseItem.Plus);
                        stream.Write((byte)client.BaseItem.SocketOne);
                        stream.Write((byte)client.BaseItem.SocketTwo);
                        stream.Write((uint)GetBattlePowerItem(client.BaseItem));
                        stream.Write(Arsenal.GetItemDonation(client.BaseItem));
                        i++;
                    }
                    stream.Finalize(2202);
                    user.Send(stream);
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
