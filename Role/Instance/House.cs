using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using TheChosenProject.Client;
using TheChosenProject.WindowsAPI;
using TheChosenProject.Game.MsgNpc;

namespace TheChosenProject.Role.Instance
{
    public class House
    {
        public struct DBNpc
        {
            public uint UID;

            public uint UnKnow;

            public ushort X;

            public ushort Y;

            public ushort Mesh;

            public Flags.NpcType NpcType;

            public MapObjectType ObjType;

            public ushort Sort;

            public uint DynamicID;

            public uint Map;

            public static DBNpc Create(Npc npc)
            {
                DBNpc result;
                result = default(DBNpc);
                result.UID = npc.UID;
                result.UnKnow = npc.UnKnow;
                result.X = npc.X;
                result.Y = npc.Y;
                result.Mesh = npc.Mesh;
                result.NpcType = npc.NpcType;
                result.ObjType = npc.ObjType;
                result.Sort = npc.Sort;
                result.DynamicID = npc.DynamicID;
                result.Map = npc.Map;
                return result;
            }

            public static Npc GetServerNpc(DBNpc Dbnpc)
            {
                return new Npc
                {
                    UID = Dbnpc.UID,
                    UnKnow = Dbnpc.UnKnow,
                    X = Dbnpc.X,
                    Y = Dbnpc.Y,
                    Mesh = Dbnpc.Mesh,
                    NpcType = Dbnpc.NpcType,
                    ObjType = Dbnpc.ObjType,
                    Sort = Dbnpc.Sort,
                    DynamicID = Dbnpc.DynamicID,
                    Map = Dbnpc.Map
                };
            }
        }

        public static ConcurrentDictionary<uint, House> HousePoll = new ConcurrentDictionary<uint, House>();

        public byte Level = 1;

        public ConcurrentDictionary<uint, Npc> Furnitures = new ConcurrentDictionary<uint, Npc>();

        public House(uint UID)
        {
            if (!HousePoll.ContainsKey(UID))
                HousePoll.TryAdd(UID, this);
            if (!Program.BlockAttackMap.Contains(UID))
                Program.BlockAttackMap.Add(UID);
        }

        internal unsafe static void Load(GameClient client)
        {
            if (HousePoll.ContainsKey(client.Player.UID))
            {
                client.MyHouse = HousePoll[client.Player.UID];
                return;
            }
            BinaryFile binary;
            binary = new BinaryFile();
            if (!binary.Open(ServerKernel.CO2FOLDER + "\\Houses\\" + client.Player.UID + ".bin", FileMode.Open))
                return;
            int ItemCount;
            ItemCount = 0;
            byte Level;
            Level = 0;
            binary.Read(&Level, 1);
            client.MyHouse = new House(client.Player.UID)
            {
                Level = Level
            };
            binary.Read(&ItemCount, 4);
            for (int x = 0; x < ItemCount; x++)
            {
                DBNpc DBNpcc;
                DBNpcc = default(DBNpc);
                binary.Read(&DBNpcc, sizeof(DBNpc));
                Npc Furnitures;
                Furnitures = DBNpc.GetServerNpc(DBNpcc);
                if (!client.MyHouse.Furnitures.ContainsKey(Furnitures.UID))
                    client.MyHouse.Furnitures.TryAdd(Furnitures.UID, Furnitures);
            }
            binary.Close();
        }

        internal unsafe static void Save(GameClient client)
        {
            if (client.MyHouse == null)
                return;
            BinaryFile binary;
            binary = new BinaryFile();
            if (!binary.Open(ServerKernel.CO2FOLDER + "\\Houses\\" + client.Player.UID + ".bin", FileMode.Create))
                return;
            byte level;
            level = client.MyHouse.Level;
            int ItemCount;
            ItemCount = client.MyHouse.Furnitures.Count;
            binary.Write(&level, 1);
            binary.Write(&ItemCount, 4);
            foreach (Npc furniture in client.MyHouse.Furnitures.Values)
            {
                Npc Furnitures;
                Furnitures = furniture;
                DBNpc Db_npc;
                Db_npc = DBNpc.Create(Furnitures);
                binary.Write(&Db_npc, sizeof(DBNpc));
            }
            binary.Close();
        }
    }
}
