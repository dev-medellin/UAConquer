using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using System;
using TheChosenProject.Database;

namespace TheChosenProject.Mobs
{
    public enum BossType : byte
    {
        General,
        City,
    }

    public enum IDMonster : uint
    {
        None = 0,
        NemesisTyrant = 20300,
        SnowBanshee = 20070,
        TeratoDragon = 20060,
        LavaBeast = 20055,
        SowrdMaster = 6643,
        ThrillingSpook = 20160,
        DarkmoonDemon = 4145,
        CornDevil = 3737,
        DarkSpearman = 3739,
        MummySkeleton = 3738,
        Ganoderma = 3130,
        GuildBeast = 3120
    }

    public class Base
    {
        public byte LinkID;
        public IDMonster ID;
        public string MapName;
        public ushort MapID;
        public ushort X;
        public ushort Y;
        public BossType Type;
        public MonsterFamily Mob;

        public Base(MonsterFamily _mob, BossType type)
        {
            Mob = _mob;
            Type = type;
        }

        public virtual void SendAlret(Client.GameClient client)
        {
            if (client.Team != null)
                return;
            if (Server.ServerMaps.ContainsKey(client.Player.Map))
            {
                var _mob = Server.ServerMaps[client.Player.Map].GetMob((uint)ID);
                if (_mob == null)
                    return;
                Game.MsgServer.ActionQuery action = new Game.MsgServer.ActionQuery()
                {
                    Type = ActionType.TeamSearchForMember,
                    wParam1 = _mob.X,
                    wParam2 = _mob.Y
                };
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    unsafe { client.Send(stream.ActionCreate(&action)); }
                }
            }
        }

        public virtual void Run()
        {
            if (!SendInvitationStillAlive())
            {
                SendInvitationFirstLife();
            }
            if (Type == BossType.General)
            {
                if (!MobsHandler.Pool.TryAdd(LinkID, this))
                    MobsHandler.Pool[LinkID] = this;
            }
            //MyLogger.Events.WriteInfo($"{Mob.Name} has spawned in {MapID} at {X},{Y}.");
            ServerKernel.Log.SaveLog("["+Mob.Name+"] has spawned in {MapID} at ["+X+"],["+Y+"].", true, LogType.WARNING);

        }

        public virtual void Reward(MonsterRole MobRole, Client.GameClient killer)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                //if (Type == BossType.City && ID == IDMonster.GuildBeast)
                {
                    //MobRole.DropItemID2(killer, Database.ItemType.DragonBall, stream, 3, true);

                    string msg = "The " + MobRole.Name + " has been destroyed by " + killer.Player.Name.ToString() + "! in " + MapName + " at (" + MobRole.X + "," + MobRole.Y + ")! just dropped Dragonball.";
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.World).GetArray(stream));
                }
                if (ID == IDMonster.SowrdMaster)
                    killer.DbDailyTraining.SwordMaster++;
                
                if (ID == IDMonster.TeratoDragon && ID == IDMonster.ThrillingSpook && ID == IDMonster.NemesisTyrant && ID == IDMonster.SnowBanshee)
                    killer.DbDailyTraining.CityBosses ++;

                if (MobsHandler.Pool.ContainsKey(LinkID))
                    MobsHandler.Pool2.Remove(LinkID);
                
            }
        }

        public void SendInvitationFirstLife()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Role.GameMap.EnterMap((int)MapID);
                string msg = "";
                Database.Server.AddMapMonster2(stream, Database.Server.ServerMaps[MapID], (uint)ID, X, Y, 1, 1, 1, 0, true, Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None, "", this);
                Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                effect.DropType = MsgDropID.Earth;
                if (ID == IDMonster.ThrillingSpook)
                {
                     msg = Mob.Name + " has spawned! in Market at (" + X + "," + Y + ")! Hurry and kill it.";
                }
                else if (ID == IDMonster.TeratoDragon)
                {
                    msg = Mob.Name + " has spawned! in Twin City (Altar) at (" + X + "," + Y + ")! Hurry and kill it.";

                }
                else if (ID == IDMonster.SnowBanshee)
                {
                    msg = Mob.Name + " has spawned! in Grotto 2 at (" + X + "," + Y + ")! Hurry and kill it.";

                }
                else if (ID == IDMonster.SowrdMaster)
                {
                    msg = Mob.Name + " has spawned! in Upgraded Area at (" + X + "," + Y + ")! Hurry and kill it.";

                }

                if (Type == BossType.City)
                {
                    Program.MonsterCity.TryAdd(ID, this);
                }

                //Pool.WorldGameChannel.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                foreach (var client in Server.GamePoll.Values)
                {
                    if (Type == BossType.General)
                        client.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    client.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.World).GetArray(stream));
                    if (Type == BossType.City)
                    {
                        client.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.orange, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                        client.Send(stream.ItemPacketCreate(effect));
                    }
                    else
                    {
                        //client.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.orange, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                        //client.Send(stream.ItemPacketCreate(effect));
                        //if (ID == IDMonster.ThrillingSpook)
                        //{
                        //    msg = "Thrilling Spook has spawned and terrify the world! (Join from Market) at (245,153).";
                        //    client.Player.MessageBox("", new Action<Client.GameClient>(user => user.Teleport(245, 153, 1036, 0)), null, 180, (Game.MsgServer.MsgStaticMessage.Messages)ID);
                        //}
                        //if (ID == IDMonster.TeratoDragon)
                        //{
                        //    msg = "Terato Dragon has spawned and terrify the world! (Join from Altar) at (555,597).";
                        //    client.Player.MessageBox("", new Action<Client.GameClient>(user => user.Teleport(555, 597, 1002, 0)), null, 180, (Game.MsgServer.MsgStaticMessage.Messages)ID);
                        //}
                        //if (ID == IDMonster.SnowBanshee)
                        //{
                        //    msg = "Terato Dragon has spawned and terrify the world! (Join from Altar) at (555,597).";
                        //    client.Player.MessageBox("", new Action<Client.GameClient>(user => user.Teleport(555, 597, 1002, 0)), null, 180, (Game.MsgServer.MsgStaticMessage.Messages)ID);
                        //}
                        //if (ID == IDMonster.SowrdMaster)
                        //{
                        //    msg = "Terato Dragon has spawned and terrify the world! (Join from Altar) at (555,597).";
                        //    client.Player.MessageBox("", new Action<Client.GameClient>(user => user.Teleport(555, 597, 1002, 0)), null, 180, (Game.MsgServer.MsgStaticMessage.Messages)ID);
                        //}
                        //else
                        //client.Player.MessageBox("", new Action<Client.GameClient>(user => user.Teleport(424, 378, 1002, 0)), null, 180, (Game.MsgServer.MsgStaticMessage.Messages)ID);
                        client.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.orange, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                        client.Send(stream.ItemPacketCreate(effect));
                    }
                }
            }
        }

        public bool SendInvitationStillAlive()
        {
            Role.GameMap.EnterMap((int)MapID);
            var map = Database.Server.ServerMaps[MapID];
            var loc = map.GetMobLoc((uint)ID);
            MapName = map.Name;
            if (loc != "")
            {
                string msg = "";

                if (ID == IDMonster.ThrillingSpook)
                {
                    msg = Mob.Name + " is still alive in at (" + X + "," + Y + ")! Hurry and kill it.";
                }
                else if (ID == IDMonster.TeratoDragon)
                {
                    msg = Mob.Name + " is still alive in Twin City (Altar) at (" + X + "," + Y + ")! Hurry and kill it.";

                }
                else if (ID == IDMonster.SnowBanshee)
                {
                    msg = Mob.Name + " is still alive in Grotto 2 at (" + X + "," + Y + ")! Hurry and kill it.";

                }
                else if (ID == IDMonster.SowrdMaster)
                {
                    msg = Mob.Name + " is still alive in Upgraded Area at (" + X + "," + Y + ")! Hurry and kill it.";

                }

                Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                effect.DropType = MsgDropID.Earth;
                if (Type == BossType.City)
                {
                    Program.MonsterCity.TryAdd(ID, this);
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var client in Server.GamePoll.Values)
                    {
                        if (Type == BossType.General)
                            client.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.yellow, Game.MsgServer.MsgMessage.ChatMode.World).GetArray(stream));
                        client.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.yellow, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        if (Type == BossType.City)
                        {
                            client.Send(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.orange, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                            client.Send(stream.ItemPacketCreate(effect));
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}