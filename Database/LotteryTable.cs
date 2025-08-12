using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Game.MsgServer;
using TheChosenProject;
using TheChosenProject.Role;

namespace TheChosenProject.Database
{
    public class LotteryTable
    {
        public class LotteryItem
        {
            public int Rank;

            public int Chance;

            public string Name;

            public uint ID;

            public byte Color;

            public byte Sockets;

            public byte Plus;

            public override string ToString()
            {
                return Rank + " " + Chance + " " + Name + " " + ID + " " + Color + " " + Sockets + " " + Plus;
            }
        }

        public class LotteryRandom
        {
            public class RankRateWatcher
            {
                private int tick;

                private int count;

                public static implicit operator bool(RankRateWatcher q)
                {
                    bool result;
                    result = false;
                    q.count++;
                    if (q.count == q.tick)
                    {
                        q.count = 0;
                        result = true;
                    }
                    return result;
                }

                public RankRateWatcher(int Tick)
                {
                    tick = Tick;
                    count = 0;
                }
            }

            public RankRateWatcher[] Ranks;

            public LotteryRandom()
            {
                Ranks = new RankRateWatcher[5];
                Ranks[0] = new RankRateWatcher(600);
                Ranks[1] = new RankRateWatcher(500);
                Ranks[2] = new RankRateWatcher(300);
                Ranks[3] = new RankRateWatcher(100);
                Ranks[4] = new RankRateWatcher(40);
            }

            public int GenerateRank()
            {
                for (int x = 0; x < Ranks.Length; x++)
                {
                    if ((bool)Ranks[x])
                        return x + 1;
                }
                return 6;
            }
        }

        public LotteryRandom RandomGenerator;

        private Dictionary<int, List<LotteryItem>> LotteryItems;

        private Random Rand;

        public void LoadLotteryItems()
        {
            Rand = new Random();
            RandomGenerator = new LotteryRandom();
            LotteryItems = new Dictionary<int, List<LotteryItem>>();
            using (Read reader = new Read("lottery.ini"))
            {
                if (!reader.Reader())
                    return;
                for (int x = 0; x < reader.Count; x++)
                {
                    ReadLine line;
                    line = new ReadLine(reader.ReadString(""), ' ');
                    LotteryItem item;
                    item = new LotteryItem
                    {
                        Rank = line.Read(0),
                        Chance = line.Read(0),
                        Name = line.Read(""),
                        ID = (uint)line.Read(0),
                        Color = line.Read((byte)0),
                        Sockets = line.Read((byte)0),
                        Plus = line.Read((byte)0)
                    };
                    if (!LotteryItems.ContainsKey(item.Rank))
                        LotteryItems.Add(item.Rank, new List<LotteryItem>());
                    LotteryItems[item.Rank].Add(item);
                }
            }
        }

        public int LotteryEntry(byte vipLevel)
        {
            return int.MaxValue;
        }

        public LotteryItem GenerateLotteryItem(GameClient user)
        {
            int Rank;
            Rank = RandomGenerator.GenerateRank();
            List<LotteryItem> items;
            items = LotteryItems[Rank];
            if (items.Count == 1)
                return items[0];
            return items[(byte)Extensions.BaseFunc.RandGet(items.Count, true)];
        }

        public MsgGameItem CreateGameItem(LotteryItem Item)
        {
            if (Server.ItemsBase.ContainsKey(Item.ID))
            {
                MsgGameItem GameItem;
                GameItem = new MsgGameItem
                {
                    ITEM_ID = Item.ID,
                    Color = (Flags.Color)(byte)ServerKernel.NextAsync(4, 8),
                    Plus = Item.Plus
                };
                if (Item.Sockets > 0)
                    GameItem.SocketOne = Flags.Gem.EmptySocket;
                if (Item.Sockets > 1)
                    GameItem.SocketTwo = Flags.Gem.EmptySocket;
                GameItem.UID = Server.ITEM_Counter.Next;
                ItemType.DBItem DBItem;
                DBItem = Server.ItemsBase[Item.ID];
                GameItem.Durability = (GameItem.MaximDurability = DBItem.Durability);
                return GameItem;
            }
            return null;
        }
    }
}