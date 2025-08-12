using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TheChosenProject.Game.MsgFloorItem;
using System.IO;
using Extensions;
using TheChosenProject.ServerSockets;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.Game.MsgAutoHunting;

namespace TheChosenProject.Game.MsgServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WalkQuery
    {
        public uint Direction;
        public uint UID;
        public uint Running;
        public uint TimeStamp;

        public ushort wParam5;
    }
    public static unsafe class MsgMovement
    {
        public const uint Walk = 0, Run = 1, Steed = 9;


        public static sbyte[] DeltaMountX = new sbyte[24] { 0, -2, -2, -2, 0, 2, 2, 2, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1 };
        public static sbyte[] DeltaMountY = new sbyte[24] { 2, 2, 0, -2, -2, -2, 0, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2 };


        public static unsafe void GetWalk(this ServerSockets.Packet stream, WalkQuery* pQuery)
        {
            stream.ReadUnsafe(pQuery, sizeof(WalkQuery));
        }

        public static unsafe ServerSockets.Packet MovementCreate(this ServerSockets.Packet stream, WalkQuery* pQuery)
        {
            stream.InitWriter();
            pQuery->TimeStamp = (uint)Extensions.Time32.Now.Value;
            stream.WriteUnsafe(pQuery, sizeof(WalkQuery));
            stream.Finalize(GamePackets.Movement);

            return stream;
        }

        public static uint Bodyyyy = 0;
        public static uint UIDDDD = 1000000;
        public static int eeffect = 1;
        public static int LastClientStamp = 0;
        [Packet(10006)]
        public unsafe static void CheckMovement(GameClient client, Packet packet)
        {
            WalkQuery walkPacket = default(WalkQuery);
            packet.GetWalk(&walkPacket);
            walkPacket.UID = client.Player.UID;
            if (client.CheckMove(walkPacket.TimeStamp))
                Movement(client, packet);
        }

        [PacketAttribute(GamePackets.Movement)]
        public unsafe static void Movement(Client.GameClient client, ServerSockets.Packet packet)
        {
            if (client.Player.AutoHunting == AutoStructures.Mode.Enable)
            {
                client.Pullback(true);
                client.Player.MessageBox("You can`t move while using AutoHunt, do you want disable it?", new Action<GameClient>(p => p.Player.AutoHunting = AutoStructures.Mode.Disable), null);
                return;
            }
            if (client.Player.Away == 1)
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet apacket;
                    apacket = rec.GetStream();
                    client.Player.Away = 0;
                    client.Player.View.SendView(client.Player.GetArray(apacket, false), false);
                }
            }
            if (client.Player.InUseIntensify)
            {
                if (client.Player.ContainFlag(MsgUpdate.Flags.Intensify))
                    client.Player.RemoveFlag(MsgUpdate.Flags.Intensify);
                client.Player.InUseIntensify = false;
            }
            bool MyPet = false;
            
            client.Player.LastMove = DateTime.Now;
            Role.Flags.ConquerAngle dir;
            WalkQuery walkPacket;
            packet.GetWalk(&walkPacket);
            if (client.Pet != null)
                MyPet = (walkPacket.UID == client.Pet.monster.UID);
            if (!MyPet && walkPacket.UID != client.Player.UID)
                return;
            ushort walkX = MyPet ? client.Pet.monster.X : client.Player.X, walkY = MyPet ? client.Pet.monster.Y : client.Player.Y;
            dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 8);

            if (!MyPet)
            {
                walkPacket.UID = client.Player.UID;

                client.Player.Action = Role.Flags.ConquerAction.None;
                client.OnAutoAttack = false;
                client.Player.Protect = Time32.Now;
                client.Player.RemoveBuffersMovements(packet);


                Role.Core.IncXY(dir, ref walkX, ref walkY);


                if (client.Map == null)
                {
                    client.Teleport(428, 378, 1002);
                    return;
                }

                if (client.Player.Map == 1038)
                {
                    if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, walkX, walkY))
                    {
                        client.SendSysMesage("Illegal jumping over the gates detected.");
                        client.Pullback();
                        return;
                    }
                }


                #region BlueMouse Teleporter
                if (client.Player.Map == 1025 && client.Player.X >= 166 && client.Player.X <= 169
                    && client.Player.Y >= 110 && client.Player.Y <= 114)
                {
                    client.Teleport(16, 80, 1502);
                }
                if (client.Player.Map == 1025 && client.Player.X >= 112 && client.Player.X <= 117
                  && client.Player.Y >= 159 && client.Player.Y <= 163)
                {
                    client.Teleport(74, 17, 1503);
                }
                if (client.Player.Map == 1502 && client.Player.X >= 10 && client.Player.X <= 14
                  && client.Player.Y >= 77 && client.Player.Y <= 79)
                {
                    client.Teleport(162, 111, 1025);
                }
                if (client.Player.Map == 1503 && client.Player.X >= 72 && client.Player.X <= 74
                 && client.Player.Y >= 11 && client.Player.Y <= 14)
                {
                    client.Teleport(114, 157, 1025);
                }
                #endregion
                #region AncientDevil
                if (client.Player.Map == 1082 && !Game.MsgTournaments.MsgSchedules.SpawnDevil)
                {
                    if (client.Player.X >= 217 && client.Player.X <= 218 && client.Player.Y >= 207 && client.Player.Y <= 208)
                    {
                        if (client.Inventory.Contain(710011, 1)
                        && client.Inventory.Contain(710012, 1)
                        && client.Inventory.Contain(710013, 1)
                            && client.Inventory.Contain(710014, 1)
                            && client.Inventory.Contain(710015, 1) /*&& client.Inventory.Contain(710022, 1)*/)
                        {
                            client.Inventory.Remove(710011, 1, packet);
                            client.Inventory.Remove(710012, 1, packet);
                            client.Inventory.Remove(710013, 1, packet);
                            client.Inventory.Remove(710014, 1, packet);
                            client.Inventory.Remove(710015, 1, packet);
                            //client.Inventory.Remove(710022, 1, packet);
                            Database.Server.AddMapMonster(packet, client.Map, 9111, 213, 205, 1, 1, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                            Game.MsgTournaments.MsgSchedules.SpawnDevil = true;
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The AncientDevil is being awaken! Prepare yourself to fight, it will appear in a matter of seconds!.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(packet));
                        }
                        else
                        {
                            client.SendSysMesage("You can only summon the AncientDevil inside its map near 218,208! also you need the five Amulets (Trojan, Fire, Archer, Warrior and Water)");
                        }
                    }
                }
                #endregion

                if (client.Player.ObjInteraction != null)
                {
                    if (client.Player.ObjInteraction.Player.X == client.Player.X && client.Player.ObjInteraction.Player.Y == client.Player.Y)
                    {

                        InterActionWalk query = new InterActionWalk()
                        {
                            Mode = MsgInterAction.Action.Walk,
                            UID = client.Player.UID,
                            OponentUID = client.Player.ObjInteraction.Player.UID,
                            DirectionOne = (byte)dir
                        };

                        client.Player.View.SendView(packet.InterActionWalk(&query), true);

                        client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                        client.Player.X = walkX;
                        client.Player.Y = walkY;
                        client.Player.Angle = dir;

                        client.Player.View.Role(false, packet.InterActionWalk(&query));

                        client.Map.View.MoveTo<Role.IMapObj>(client.Player.ObjInteraction.Player, walkX, walkY);
                        client.Player.ObjInteraction.Player.X = walkX;
                        client.Player.ObjInteraction.Player.Y = walkY;
                        client.Player.ObjInteraction.Player.Angle = dir;

                        client.Player.ObjInteraction.Player.View.Role();
                        return;
                    }
                }
                client.Player.View.SendView(packet.MovementCreate(&walkPacket), true);
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                client.Player.X = walkX;
                client.Player.Y = walkY;
                client.Player.Angle = dir;

                client.Player.View.Role(false, packet.MovementCreate(&walkPacket));

               
            }
            else if (client.Pet != null)
            {
                walkPacket.UID = client.Pet.monster.UID;
                walkPacket.Running = 1;
                walkPacket.Direction = (byte)dir;
                client.Pet.monster.Action = Role.Flags.ConquerAction.None;
                client.OnAutoAttack = false;

                Role.Core.IncXY(dir, ref walkX, ref walkY);
                client.Player.View.SendView(packet.MovementCreate(&walkPacket), true);
                client.Map.View.MoveTo<Role.IMapObj>(client.Pet.monster, walkX, walkY);
                client.Pet.monster.X = walkX;
                client.Pet.monster.Y = walkY;
                client.Pet.monster.Facing = dir;
                client.Player.View.Role(false, packet.MovementCreate(&walkPacket));
                client.Pet.monster.UpdateMonsterView2(client.Player.View, packet);
                //client.Player.View.SendView(client.Pet.monster.GetArray(packet, false), true);
            }
            MsgSchedules.CaptureTheFlag?.ChechMoveFlag(client);
            if (client.Player.DelayedTask)
                client.Player.RemovePick(packet);
            client.Player.UpdateSurroundings(packet);
        }
    }
}
