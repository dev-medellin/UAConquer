using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Struct
{
    public struct DbKillMobsExterminator
    {
        public static uint MaxKills = 500000;
        public static uint MaxKillsTwin = 500000;
        public static uint MaxKillsPhoenix = 500000;
        public static uint MaxKillsApe = 500000;
        public static uint MaxKillsDesert = 500000;
        public static uint MaxKillsBird = 500000;
        public static uint MaxKillsMystic = 500000;

        public uint TwinTimes;
        public uint TCMobs;
        //public uint Turtledove;
        //public uint Robin;
        //public uint Apparition;
        //public uint Poltergeist;
        public uint PhoenixTimes;
        public uint PCMobs;
        //public uint Bandit;
        //public uint Ratling;
        //public uint FireSpirit;
        public uint ApeTimes;
        public uint ACMobs;
        //public uint GiantApe;
        //public uint ThunderApe;
        //public uint Snakeman;
        public uint DesertTimes;
        public uint DCMobs;
        //public uint HillMonster;
        //public uint RockMonster;
        //public uint BladeGhost;
        public uint BirdTimes;
        public uint BIMobs;
        //public uint HawKing;
        //public uint BanditL97;
        //public uint SeniorBandit;
        //public uint MysticTimes;
        //public uint TombBat;
        //public uint BloodyBat;
        //public uint VampireBat;
        //public uint BullMaster;
        //public uint RedDevilL117;
        //public uint BloodyDevil;

        public void GetDB(DbKillMobsExterminator _db)
        {
            this = _db;
        }

        public void EnqueueKills(uint MapID)
        {
            if (MapID == 1002 && MaxKillsTwin <= 500000)
            {
                TCMobs++;
                //Turtledove++;
                //Robin++;
                //Apparition++;
                //Poltergeist++;
            }
            else if (MapID == 1011 && MaxKillsPhoenix <= 500000)

            {
                PCMobs++;
                //Turtledove++;
                //Robin++;
                //Apparition++;
                //Poltergeist++;
            }
            else if (MapID == 1020 && MaxKillsApe <= 500000)

            {
                ACMobs++;
                //Turtledove++;
                //Robin++;
                //Apparition++;
                //Poltergeist++;
            }
            else if (MapID == 1000 && MaxKillsDesert <= 500000)

            {
                DCMobs++;
                //Turtledove++;
                //Robin++;
                //Apparition++;
                //Poltergeist++;
            }
            else if (MapID == 1015 && MaxKillsBird <= 500000)

            {
                BIMobs++;
                //Turtledove++;
                //Robin++;
                //Apparition++;
                //Poltergeist++;
            }
            //if (MobID == 1 && Pheasant < MaxKillsTwin) Pheasant++;
            //else if (MobID == 2 && Turtledove < MaxKillsTwin) Turtledove++;
            //else if (MobID == 3 && Robin < MaxKillsTwin) Robin++;
            //else if (MobID == 4 && Apparition < MaxKillsTwin) Apparition++;
            //else if (MobID == 5 && Poltergeist < MaxKillsTwin) Poltergeist++;

            //else if (MobID == 6 && WingedSnake < MaxKillsPhoenix) WingedSnake++;
            //else if (MobID == 7 && Bandit < MaxKillsPhoenix) Bandit++;
            //else if (MobID == 8 && Ratling < MaxKillsPhoenix) Ratling++;
            //else if (MobID == 113 && FireSpirit < MaxKillsPhoenix) FireSpirit++;

            //else if (MobID == 10 && Macaque < MaxKillsApe) Macaque++;
            //else if (MobID == 11 && GiantApe < MaxKillsApe) GiantApe++;
            //else if (MobID == 12 && ThunderApe < MaxKillsApe) ThunderApe++;
            //else if (MobID == 13 && Snakeman < MaxKillsApe) Snakeman++;

            //else if (MobID == 14 && SandMonster < MaxKillsDesert) SandMonster++;
            //else if (MobID == 15 && HillMonster < MaxKillsDesert) HillMonster++;
            //else if (MobID == 51 && RockMonster < MaxKillsDesert) RockMonster++;
            //else if (MobID == 17 && BladeGhost < MaxKillsDesert) BladeGhost++;

            //else if (MobID == 18 && Birdman < MaxKillsBird) Birdman++;
            //else if (MobID == 19 && HawKing < MaxKillsBird) HawKing++;
            //else if (MobID == 55 && BanditL97 < MaxKillsBird) BanditL97++;
            //else if (MobID == 84 && SeniorBandit < MaxKillsBird) SeniorBandit++;

            //else if (MobID == 20 && TombBat < MaxKillsMystic) TombBat++;
            //else if (MobID == 56 && BloodyBat < MaxKillsMystic) BloodyBat++;
            //else if (MobID == 86 && VampireBat < MaxKillsMystic) VampireBat++;
            //else if (MobID == 2232 && BullMaster < MaxKillsMystic) BullMaster++;
            //else if (MobID == 58 && RedDevilL117 < MaxKillsMystic) RedDevilL117++;
            //else if (MobID == 88 && BloodyDevil < MaxKillsMystic) BloodyDevil++;
        }

        public void Reset()
        {
            TwinTimes = 0;
            PhoenixTimes = 0;
            ApeTimes = 0;
            DesertTimes = 0;
            BirdTimes = 0;
            //MysticTimes = 0;

        }

        public static void Reward(Client.GameClient client, uint id)
        {
            return;
            switch (id)
            {
                case 1002:
                    {
                        client.DbKillMobsExterminator.TwinTimes += 1;
                        client.DbKillMobsExterminator.TCMobs = 0;
                        //client.DbKillMobsExterminator.Turtledove = 0;
                        //client.DbKillMobsExterminator.Robin = 0;
                        //client.DbKillMobsExterminator.Apparition = 0;
                        //client.DbKillMobsExterminator.Poltergeist = 0;
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;//10
                            stream = rec.GetStream();
                            //client.Inventory.Add(stream, 720135, 1, 0, 0, 0);//1xSuperGem
                            //client.Inventory.Add(stream, 720279, 1, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730003, 1, 0, 0, 0);//1xStone+3
                            client.Inventory.Add(stream, 720027, 3, 0, 0, 0);//3xMeteorScrool
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 3, 0, 0, 0);//1xDBScrool
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Extermination] You just gained Spade (Quest Items) ! from [TwinCity Quest]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                        }
                        client.SendSysMesage($"[Extermination] You just gained Spade (Quest Items) ! from [TwinCity Quest]!");

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                        }
                        break;
                    }
                case 1011:
                    {
                        client.DbKillMobsExterminator.PhoenixTimes += 1;
                        client.DbKillMobsExterminator.PCMobs = 0;
                        //client.DbKillMobsExterminator.Bandit = 0;
                        //client.DbKillMobsExterminator.Ratling = 0;
                        //client.DbKillMobsExterminator.FireSpirit = 0;
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;//13
                            stream = rec.GetStream();
                            //client.Inventory.Add(stream, 720135, 1, 0, 0, 0);//1xSuperGem
                            client.Inventory.Add(stream, 720280, 1, 0, 0, 0);//3xPowerExpball
                      
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Extermination] You just gained AncientBead (Quest Items) ! from [PhoenixCity Quest]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                        }
                        client.SendSysMesage($"[Extermination] You just gained AncientBead (Quest Items) ! from [PhoenixCity Quest]!");

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                        }
                        break;
                    }
                case 1020:
                    {
                        client.DbKillMobsExterminator.ApeTimes += 1;
                        client.DbKillMobsExterminator.ACMobs = 0;
                        //client.DbKillMobsExterminator.GiantApe = 0;
                        //client.DbKillMobsExterminator.ThunderApe = 0;
                        //client.DbKillMobsExterminator.Snakeman = 0;
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;//16
                            stream = rec.GetStream();
                            //client.Inventory.Add(stream, 720282, 1, 0, 0, 0);//1xSuperGem
                            //client.Inventory.Add(stream, 722057, 3, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730003, 2, 0, 0, 0);//1xStone+6
                            client.Inventory.Add(stream, 720027, 3, 0, 0, 0);//3xMeteorScrool
                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 7, 0, 0, 0);//1xDBScrool
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Extermination] You just gained AncientBead (Quest Items) ! from [ApeCity Quest]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                        }
                        client.SendSysMesage($"[Extermination] You just gained AncientBead (Quest Items) ! from [ApeCity Quest]!!");

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                        }
                        break;
                    }
                case 1000:
                    {
                        client.DbKillMobsExterminator.DesertTimes += 1;
                        client.DbKillMobsExterminator.DCMobs = 0;
                        //client.DbKillMobsExterminator.HillMonster = 0;
                        //client.DbKillMobsExterminator.RockMonster = 0;
                        //client.DbKillMobsExterminator.BladeGhost = 0;
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;//12
                            stream = rec.GetStream();
                            //client.Inventory.Add(stream, 720282, 1, 0, 0, 0);//1xSuperGem
                            //client.Inventory.Add(stream, 722057, 3, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730003, 3, 0, 0, 0);//1xStone+6
                            client.Inventory.Add(stream, 720027, 3, 0, 0, 0);//3xMeteorScrool
                            client.Inventory.Add(stream, 720028, 1, 0, 0, 0);//1xDBScrool
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Extermination] You just gained HellFire (Quest Items) ! from [DesertCity Quest]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                        }
                        client.SendSysMesage($"[Extermination] You just gained HellFire (Quest Items) ! from [DesertCity Quest]!");
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                        }
                        break;
                    }
                case 1015:
                    {
                        client.DbKillMobsExterminator.BirdTimes += 1;
                        client.DbKillMobsExterminator.BIMobs = 0;
                        //client.DbKillMobsExterminator.HawKing = 0;
                        //client.DbKillMobsExterminator.BanditL97 = 0;
                        //client.DbKillMobsExterminator.SeniorBandit = 0;
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;//13
                            stream = rec.GetStream();
                            //client.Inventory.Add(stream, 720283, 1, 0, 0, 0);//1xSuperGem
                            //client.Inventory.Add(stream, 722057, 3, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730003, 3, 0, 0, 0);//1xStone+6
                            client.Inventory.Add(stream, 720027, 3, 0, 0, 0);//3xMeteorScrool
                            client.Inventory.Add(stream, 720028, 1, 0, 0, 0);//1xDBScrool
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Extermination] You just gained FrostCrystal (Quest Items) ! from [BirdCity Quest]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                        }
                        client.SendSysMesage($"[Extermination] You just gained FrostCrystal (Quest Items) ! from [BirdCity Quest]!");

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                        }
                        break;
                    }
                case 1001:
                    {
                        //client.DbKillMobsExterminator.MysticTimes += 1;
                        //client.DbKillMobsExterminator.TombBat = 0;
                        //client.DbKillMobsExterminator.BloodyBat = 0;
                        //client.DbKillMobsExterminator.VampireBat = 0;
                        //client.DbKillMobsExterminator.BullMaster = 0;
                        //client.DbKillMobsExterminator.RedDevilL117 = 0;
                        //client.DbKillMobsExterminator.BloodyDevil = 0;
                        /*
                          data.AddText("\n# ------------------------------------------------------- #.");
                        data.AddText("\n    (+5)Stone, 2x(Super)TortoiseGem, 5xClass5MoneyBag,");
                        data.AddText("\n        2xMegaDBPack, 3xMegaMetsPack, 3xUltimatePack,");
                        data.AddText("\n            5xPowerExpBall, 1x(1-Day)VIP6.");
                        data.AddText("\n# ------------------------------------------------------- #."); 
                         
                         */


                        using (RecycledPacket rec = new RecycledPacket())
                        {//+5 3
                            Packet stream;//22
                            stream = rec.GetStream();
                            client.DbKillMobsExterminator.Reset();
                            client.Inventory.Add(stream, 730005, 1, 0, 0, 0);//1x+5Stone
                            client.Inventory.Add(stream, 700073, 2, 0, 0, 0);//2xTortoiseGem
                            client.Inventory.Add(stream, 723717, 5, 0, 0, 0);//5xClass5MoneyBag
                            client.Inventory.Add(stream, 720546, 2, 0, 0, 0);//3xMegaDBPack
                            client.Inventory.Add(stream, 720547, 3, 0, 0, 0);//3xMegaMetsPack
                            client.Inventory.Add(stream, 721169, 3, 0, 0, 0);//3xUltimatePack
                            client.Inventory.Add(stream, 722057, 5, 0, 0, 0);//5xPowerExpBall
                            client.Inventory.Add(stream, 780002, 1, 0, 0, 0);//1x7D-VIP
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Extermination] You just gained (+5)Stone, 2x(Super)TortoiseGem, 5xClass5MoneyBag, 2xMegaDBPack, 3xMegaMetsPack, 3xUltimatePack, 5xPowerExpBall, 1x(7-Day)VIP6.! from [A Thousand Years Quest]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                        }
                        client.SendSysMesage($"[Extermination] You just gained (+5)Stone, 2x(Super)TortoiseGem, 5xClass5MoneyBag, 2xMegaDBPack, 3xMegaMetsPack, 3xUltimatePack, 5xPowerExpBall, 1x(7-Day)VIP6.!");

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");
                        }
                        break;
                    }
            }
        }

        public static unsafe void Save(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\KillMobsExterminator\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\KillMobsExterminator\\");
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\KillMobsExterminator\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                DbKillMobsExterminator DB = new DbKillMobsExterminator();
                DB.GetDB(client.DbKillMobsExterminator);
                binary.Write(&DB, sizeof(DbKillMobsExterminator));
                binary.Close();
            }
        }

        public static unsafe void Load(Client.GameClient client)
        {
            if (!Directory.Exists(ServerKernel.CO2FOLDER + "\\Tasks\\KillMobsExterminator\\"))
                Directory.CreateDirectory(ServerKernel.CO2FOLDER + "\\Tasks\\KillMobsExterminator\\");
            client.DbKillMobsExterminator = new DbKillMobsExterminator();
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(ServerKernel.CO2FOLDER + "\\Tasks\\KillMobsExterminator\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                DbKillMobsExterminator DB;
                binary.Read(&DB, sizeof(DbKillMobsExterminator));
                client.DbKillMobsExterminator.GetDB(DB);
                binary.Close();
            }
        }
    }
}