using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Role.Instance
{
   public class Trade
    {
       public Client.GameClient Owner;
       public Client.GameClient Target;
       public uint ConquerPoints;
       public uint Money;
       public bool WindowOpen;
       public bool Confirmed;

       public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> Items;

       public Trade(Client.GameClient _owner)
       {
           Owner = _owner;
           Items = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();
       }

       public bool ItemInTrade(Game.MsgServer.MsgGameItem Dataitem)
       {
           return Items.ContainsKey(Dataitem.ITEM_ID);
       }

       public unsafe void AddConquerPoints(uint dwParam, ServerSockets.Packet stream)
       {
            string logss = "[Trade]" + Owner.Player.Name + " to " + Target.Player.Name + " trade CPS = " + dwParam + "";
            Database.ServerDatabase.LoginQueue.Enqueue(logss);
            if (Target.InTrade)
            {
                string logs = "[Trade]" + Owner.Player.Name + " ";

                if (Owner.Player.ConquerPoints >= dwParam)
                {
                    Owner.Player.ConquerPoints -= (int)dwParam;
                    ConquerPoints += dwParam;

                    Target.Send(stream.TradeCreate((uint)ConquerPoints, MsgTrade.TradeID.DisplayConquerPoints));
                    ServerKernel.Log.GmLog("trade_cps", $"{Owner.Player.Name}({Owner.Player.UID}) trade :[id={dwParam} conquer points to {Target.Player.Name} ");

                    //if (DatabaseConfig.discord_stat)
                        //if (Owner.Player.Name.Contains("PM") || Owner.Player.Name.Contains("GM"))
                            //Program.DiscordGMTradeAPI.Enqueue($"{Owner.Player.Name}({Owner.Player.UID}) trade :[id={dwParam} conquer points to {Target.Player.Name} ");

                }
            }

        }
        public unsafe void AddMoney(uint dwParam, ServerSockets.Packet stream)
       {
            string logss = "[Trade]" + Owner.Player.Name + " to " + Target.Player.Name + " trade money = " + dwParam + "";
            Database.ServerDatabase.LoginQueue.Enqueue(logss);
            if (Target.InTrade)
           {
                string logs = "[Trade]" + Owner.Player.Name + " ";

                if (Owner.Player.Money >= dwParam)
               {
                   Owner.Player.Money -= (int)dwParam;
                   Money += dwParam;
                   Target.Send(stream.TradeCreate((ulong)Money, MsgTrade.TradeID.DisplayMoney));
                    ServerKernel.Log.GmLog("trade_money", $"{Owner.Player.Name}({Owner.Player.UID}) trade :[id={dwParam} money to {Target.Player.Name} ");
                    //if (DatabaseConfig.discord_stat)
                        //if (Owner.Player.Name.Contains("PM") || Owner.Player.Name.Contains("GM"))
                            //Program.DiscordGMTradeAPI.Enqueue($"{Owner.Player.Name}({Owner.Player.UID}) trade :[id={dwParam} money to {Target.Player.Name}");

                }
            }
       }
       public bool ValidItems()
       {
           foreach (var item in Items.Values)
               if (!Owner.Inventory.ClientItems.ContainsKey(item.UID))
                   return false;
           return true;
       }
      
       public unsafe void AddItem(ServerSockets.Packet stream, uint dwparam, Game.MsgServer.MsgGameItem DataItem)
       {
            string logss = "[Trade]" + Owner.Player.Name + " to " + Target.Player.Name + " trade item = " + Database.Server.ItemsBase[DataItem.ITEM_ID].Name + " - plus [" + DataItem.Plus + "] - Bless [" + DataItem.Bless + "] - Enchant [" + DataItem.Enchant + "] - s1[" + DataItem.SocketOne + "] - s2[" + DataItem.SocketTwo + "]";
            Database.ServerDatabase.LoginQueue.Enqueue(logss);
            if (Target.InTrade)
            {
                string logs = "[trade]" + Owner.Player.Name + " ";

                if (DataItem.Locked != 0)
               {
                   ConcurrentDictionary<uint, Role.Instance.Associate.Member> src;
                   if (!Owner.Player.Associate.Associat.TryGetValue(Role.Instance.Associate.Partener, out src))
                   {
                       Owner.Send(stream.TradeCreate(dwparam, MsgTrade.TradeID.RemoveItem));
                       Owner.SendSysMesage("unable to trade this item.");
                       return;

                   }
                   else if (!src.ContainsKey(Target.Player.UID))
                   {
                       Owner.Send(stream.TradeCreate(dwparam, MsgTrade.TradeID.RemoveItem));
                       Owner.SendSysMesage("unable to trade this item.");
                       return;
                   }
                }
               if (DataItem.Bound >= 1 || DataItem.Inscribed == 1 || Database.ItemType.undropeitem.Contains(DataItem.ITEM_ID)|| Database.ItemType.unabletradeitem.Contains(DataItem.ITEM_ID))
               {

                   Owner.Send(stream.TradeCreate(dwparam, MsgTrade.TradeID.RemoveItem));

                   Owner.SendSysMesage("unable to trade this item.");


                   return;
               }
               if (Target.Inventory.HaveSpace((byte)(Items.Count + 1)))
               {
                   DataItem.Mode = Flags.ItemMode.Trade;
                   DataItem.Send(Target, stream);
                   DataItem.Mode = Flags.ItemMode.AddItem;
                   Items.TryAdd(DataItem.UID, DataItem);
                    ServerKernel.Log.GmLog("trade_item", $"{Owner.Player.Name}({Owner.Player.UID}) trade :[{Database.Server.ItemsBase[DataItem.ITEM_ID].Name} - plus {DataItem.Plus} - Bless {DataItem.Bless} - Enchant {DataItem.Enchant} soc1 {DataItem.SocketOne} - Soc2 {DataItem.SocketTwo} to {Target.Player.Name} ");

                }
                else
               {
                   Owner.Send(stream.TradeCreate(dwparam, MsgTrade.TradeID.RemoveItem));
                   Owner.SendSysMesage("There is not enough room in your partner inventory.");
               }
            }

            //if (DatabaseConfig.discord_stat)
            //    if (Owner.Player.Name.Contains("PM") || Owner.Player.Name.Contains("GM"))
                    //Program.DiscordGMTradeAPI.Enqueue(logss);
            //if (Owner.Player.Name.Contains("PM") || Owner.Player.Name.Contains("GM"))
            //{
            //    Program.DiscordAdminAPI.Enqueue(logss);
            //}
        }

       public unsafe void CloseTrade()
       {
           using (var rec = new ServerSockets.RecycledPacket())
           {
               var msg = rec.GetStream();

               if (Target.InTrade)
               {
                   Owner.Send(msg.TradeCreate(Owner.Player.UID, MsgTrade.TradeID.CloseTradeWindow));
                   Target.Send(msg.TradeCreate(Owner.Player.UID, MsgTrade.TradeID.CloseTradeWindow));

                   Owner.Player.targetTrade = 0;
                   Target.Player.targetTrade = 0;

                   Target.MyTrade.DestroyItems(msg);
                   Target.MyTrade = null;

                   Owner.MyTrade.DestroyItems(msg);
                   Owner.MyTrade = null;
               }
           }
       }

       public void DestroyItems(ServerSockets.Packet stream)
       {
            Owner.Player.ConquerPoints += (int)ConquerPoints;
            Owner.Player.Money += (int)Money;
          
           Owner.Player.SendUpdate(stream,Owner.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);

           foreach (var item in Items.Values)
           {
               item.Mode = Flags.ItemMode.AddItem;
               item.Send(Owner,stream);
           }
       }
      
    }
}
