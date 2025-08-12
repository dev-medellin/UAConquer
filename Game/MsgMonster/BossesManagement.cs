using Extensions;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using TheChosenProject.Game.MsgFloorItem;

namespace TheChosenProject
{
	public class BossesManagement
	{
		public static Time32 Stamp = Time32.Now.AddMilliseconds(1000);

		public static void work(Time32 clock)
		{
            bool ReleaseStat = false;

            if (!(clock > Stamp))
				return;
			foreach (Boss boss in BossDatabase.Bosses.Values)
			{
				if (boss != null && boss.CanSpawn() && ReleaseStat)
				{

					Tuple<ushort, ushort> Coordinate;
					Coordinate = boss.SelectRandomCoordinate();
					GameMap map;
					map = Server.ServerMaps[boss.MapID];
					using (RecycledPacket rec = new RecycledPacket())
					{
						Packet stream;
						stream = rec.GetStream();
						boss.MapX = Coordinate.Item1;
						boss.MapY = Coordinate.Item2;
						Server.AddMapMonster(stream, map, boss.MonsterID, boss.MapX, boss.MapY, 1, 1, 1);
                        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                        effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                        effect.DropType = MsgDropID.Earth;
                        Program.SendGlobalPackets.Enqueue(stream.ItemPacketCreate(effect));
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(boss.Name + " has appeared around (" + boss.MapX + "," + boss.MapY + ") on the " + map.Name + " Hurry and go defeat the beast", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
						ServerKernel.Log.SaveLog(boss.Name + " has appeared around(" + boss.MapX + ", " + boss.MapY + ") on the " + map.Name, true, LogType.DEBUG);
						//Program.DiscordCBosses.Enqueue($"`{boss.Name} has appeared around ({boss.MapX}, {boss.MapY}) on the {map.Name} Hurry and go defeat the beast`");

					}
                }
			}
			Stamp.Value = clock.Value + 1000;
		}

		public static List<Boss> WhoAlive(ushort mapid)
		{
			return BossDatabase.Bosses.Values.Where((Boss p) => p.MapID == mapid && p.Alive).ToList();
		}
	}
}
