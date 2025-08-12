using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Database
{
    public class NpcServer
    {
        public class Furniture
        {
            public string Name;

            public uint ItemID;

            public int MoneyCost;

            public uint UID;

            public ushort Mesh;
        }

        public static Dictionary<uint, Furniture> FurnitureInformations = new Dictionary<uint, Furniture>();

        public static Furniture GetNpc(uint ItemID)
        {
            foreach (Furniture npc in FurnitureInformations.Values)
            {
                if (npc.ItemID == ItemID)
                    return npc;
            }
            return null;
        }

        public static Furniture GetNpcFromMesh(uint mesh)
        {
            foreach (Furniture npc in FurnitureInformations.Values)
            {
                if (npc.Mesh == mesh)
                    return npc;
            }
            return null;
        }

        internal static void LoadSobNpcs()
        {
            string[] baseText;
            baseText = File.ReadAllLines(ServerKernel.CO2FOLDER + "SobNpcs.txt");
            string[] array;
            array = baseText;
            foreach (string bas_line in array)
            {
                ReadLine line;
                line = new ReadLine(bas_line, ',');
                SobNpc npc;
                npc = new SobNpc
                {
                    ObjType = MapObjectType.SobNpc,
                    UID = line.Read(0u),
                    Name = line.Read(""),
                    Type = (Flags.NpcType)line.Read((ushort)0),
                    Mesh = (SobNpc.StaticMesh)line.Read((ushort)0),
                    Map = line.Read((ushort)0),
                    X = line.Read((ushort)0),
                    Y = line.Read((ushort)0),
                    HitPoints = line.Read(0),
                    MaxHitPoints = line.Read(0),
                    Sort = line.Read((ushort)0)
                };
                if (npc.Map == 1039)
                {
                    int maxHitPoints;
                    maxHitPoints = (npc.HitPoints = 10000);
                    npc.MaxHitPoints = maxHitPoints;
                }
                if (line.Read((byte)0) == 0)
                    npc.Name = null;
                if (npc.UID == 465221 && npc.Map == 1645)
                {
                    npc.AddFlag(Game.MsgServer.MsgUpdate.Flags.Praying, StatusFlagsBigVector32.PermanentFlag, false);
                }
                if (Server.ServerMaps.ContainsKey(npc.Map))
                {
                    Server.ServerMaps[npc.Map].View.EnterMap((IMapObj)npc);
                    if (GameMap.IsGate(npc.UID))
                        Server.ServerMaps[npc.Map].SetGateFlagNpc(npc.X, npc.Y);
                    else
                        Server.ServerMaps[npc.Map].SetFlagNpc(npc.X, npc.Y);
                }
            }
            MsgSchedules.GuildWar.CreateFurnitures();
            MsgSchedules.GuildWar.Load();
            MsgSchedules.EliteGuildWar.Load();
            //MsgSchedules.EliteGuildWar.CreateFurnitures();
            MsgSchedules.ClanWar.CreateFurnitures();

            //Game.MsgTournaments.MsgSchedules.CitywarAC.CreateFurnitures();
            //Game.MsgTournaments.MsgSchedules.CitywarBI.CreateFurnitures();
            Game.MsgTournaments.MsgSchedules.CitywarPC.CreateFurnitures();
            //Game.MsgTournaments.MsgSchedules.CitywarDC.CreateFurnitures();
            Game.MsgTournaments.MsgSchedules.CitywarTC.CreateFurnitures();
            //Game.MsgTournaments.MsgNobilityPole.Load();
            //Game.MsgTournaments.MsgNobilityPole1.Load();
            //Game.MsgTournaments.MsgNobilityPole2.Load();
            //Game.MsgTournaments.MsgNobilityPole3.Load();
            MsgSchedules.CityWar.CreateBases();
            MsgSchedules.CityWar.Load();
        }

        public static void LoadServerTraps()
        {
            if (!File.Exists(ServerKernel.CO2FOLDER + "Traps.txt"))
                return;
            using (StreamReader read = File.OpenText(ServerKernel.CO2FOLDER + "Traps.txt"))
            {
                while (true)
                {
                    string aline;
                    aline = read.ReadLine();
                    if (aline != null && aline != "")
                    {
                        string[] line;
                        line = aline.Split(',');
                        uint ID;
                        ID = uint.Parse(line[3]);
                        ushort map;
                        map = ushort.Parse(line[0]);
                        MsgItem Item;
                        Item = new MsgItem(null, ushort.Parse(line[1]), ushort.Parse(line[2]), MsgItem.ItemType.Effect, 0u, 0u, map, 0u, false, Server.ServerMaps[map], 3600000);
                        Item.MsgFloor.m_ID = ID;
                        Item.MsgFloor.m_Color = 2;
                        Item.MsgFloor.DropType = MsgDropID.Effect;
                        if (line.Length > 4)
                            Item.AllowDynamic = byte.Parse(line[4]) == 1;
                        Item.GMap.View.EnterMap<Role.IMapObj>(Item);
                        continue;
                    }
                    break;
                }
            }
        }

        public static void LoadNpcs(Packet stream = null)
        {
            List<uint> maps = new List<uint>();
            ServerKernel.Log.SaveLog("Loading NPCs and Dynamic NPCs...", true);
            uint Count;
            Count = 0u;
            if (File.Exists(ServerKernel.CO2FOLDER + "npcs.txt"))
            {
                using (StreamReader streamReader = File.OpenText(ServerKernel.CO2FOLDER + "npcs.txt"))
                {
                    while (true)
                    {
                        string aline2;
                        aline2 = streamReader.ReadLine();
                        if (aline2 == null || !(aline2 != ""))
                            break;
                        string[] line2;
                        line2 = aline2.Split(',');
                        Npc np2;
                        np2 = Npc.Create();
                        np2.UID = uint.Parse(line2[0]);
                        np2.NpcType = (Flags.NpcType)byte.Parse(line2[1]);
                        np2.Mesh = ushort.Parse(line2[2]);
                        np2.Map = ushort.Parse(line2[3]);
                        np2.X = ushort.Parse(line2[4]);
                        np2.Y = ushort.Parse(line2[5]);
                        if (np2.Mesh != 42580)
                        {
                            if (np2.UID == 16851 || np2.UID == 168052)
                                np2.AllowDynamic = true;
                            else if (line2.Length > 6)
                            {
                                np2.AllowDynamic = ushort.Parse(line2[6]) == 1;
                            }
                            if (Server.ServerMaps.ContainsKey(np2.Map))
                            {
                                if (stream != null)
                                {
                                    if (!maps.Contains(np2.Map))
                                    {

                                        var array = Server.ServerMaps[np2.Map].View.GetAllMapRoles(MapObjectType.Npc);
                                        foreach (var role in array)
                                        {
                                            if (role is Npc)
                                            {
                                                Server.ServerMaps[np2.Map].RemoveNpc(role as Npc, stream);
                                            }
                                        }
                                    }
                                    maps.Add(np2.Map);
                                }
                                Count++;
                                Server.ServerMaps[np2.Map].AddNpc(np2);
                            }
                        }
                    }
                }
            }
            if (!File.Exists(ServerKernel.CO2FOLDER + "furnitures.txt"))
                return;
            using (StreamReader read = File.OpenText(ServerKernel.CO2FOLDER + "furnitures.txt"))
            {
                while (true)
                {
                    string aline;
                    aline = read.ReadLine();
                    if (aline != null)
                    {
                        string[] line;
                        line = aline.Split(',');
                        Npc np;
                        np = Npc.Create();
                        np.UID = uint.Parse(line[0]);
                        np.NpcType = (Flags.NpcType)byte.Parse(line[1]);
                        np.Mesh = ushort.Parse(line[2]);
                        np.Map = ushort.Parse(line[3]);
                        np.X = ushort.Parse(line[4]);
                        np.Y = ushort.Parse(line[5]);
                        Furniture furnit;
                        furnit = new Furniture
                        {
                            Name = line[6],
                            ItemID = uint.Parse(line[7]),
                            UID = np.UID,
                            Mesh = np.Mesh,
                            MoneyCost = int.Parse(line[8])
                        };
                        if (!FurnitureInformations.ContainsKey(np.UID))
                            FurnitureInformations.Add(np.UID, furnit);
                        if (Server.ServerMaps.ContainsKey(np.Map))
                            Server.ServerMaps[np.Map].AddNpc(np);
                        continue;
                    }
                    break;
                }
            }
        }
    }
}
