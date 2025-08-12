using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMsgGuardShield;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public struct MsgLoginClient
    {
        public ushort Length;
        public ushort PacketID;
        public uint AccountHash;
        public uint Key, HDSerial;
        public string MachineName, MacAddress, HWID, Username;
        public static ConcurrentDictionary<string, List<string>> PlayersIP = new ConcurrentDictionary<string, List<string>>();

        [PacketAttribute((ushort)MsgGuardShield.PacketIDs.MsgLoginGame)]
        public unsafe static void LoginGameEx(Client.GameClient client, ServerSockets.Packet packet)
        {
            if (client.Guard == null)
                return;
            packet.Seek(0);
            byte[] bytes = packet.ReadBytes(packet.Size);
            var msg = new MsgGuardShield.MsgLoginGame(bytes, client.Guard);
            if (!msg.AllowLogin)
                return;

            var transferCipher = new Cryptography.TransferCipher(ServerKernel.LoginServerAddress);
            uint[] Decrypt = transferCipher.Decrypt(new uint[] { msg.Key, msg.AccountHash });
            client.EncryptTokenSpell = msg.EncryptAttack;
            client.OnLogin = new MsgLoginClient()
            {
                Key = Decrypt[0],
                AccountHash = Decrypt[1],
                HDSerial = msg.HDSerial,
                MachineName = msg.MachineName,
                MacAddress = msg.MacAddress,
                HWID = msg.HWID,
                Username = msg.Username,
            };
            client.ClientFlag |= Client.ServerFlag.OnLoggion;
            Database.ServerDatabase.LoginQueue.TryEnqueue(client);
        }

        [PacketAttribute((ushort)GamePackets.LoginGame)]
        public unsafe static void LoginGame(Client.GameClient client, ServerSockets.Packet packet)
        {
            uint[] Decrypt = Program.transferCipher.Decrypt(new uint[] { packet.ReadUInt32(), packet.ReadUInt32() });
            client.Guard = new CMsgGuardShield.GuardShield(client);
        }
        public static void LoginHandler(GameClient client, MsgLoginClient OnLogin)
        {
            client.ClientFlag &= ~ServerFlag.OnLoggion;
            if (client.Socket != null && client.Socket.RemoteIp == "NONE")
            {
                ServerKernel.Log.SaveLog("Break login client.");
                return;
            }
            try
            {
                //if (OnLogin.Key > 100000000 || OnLogin.Key < 1000000)
                //{
                //    using (RecycledPacket rec = new RecycledPacket())
                //    {
                //        Packet stream;
                //        stream = rec.GetStream();
                //        string Messaj;
                //        Messaj = "The account server is illegal or hasn`t been connected.";
                //        client.Send(new MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                //        return;
                //    }
                //}
                if (SystemBannedPC.IsBanned(client, out var BanMessaje))
                {
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream14;
                        stream14 = recycledPacket.GetStream();
                        string Messaj7;
                        Messaj7 = BanMessaje ?? "";
                        client.Send(new MsgMessage(Messaj7, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream14));
                        return;
                    }
                }
                //if (Server.GamePoll.Values.Where((GameClient p) => p.Player.OfflineTraining != MsgOfflineTraining.Mode.Hunting && p.Player.OfflineTraining != MsgOfflineTraining.Mode.TrainingGroup && p.Player.OfflineTraining != MsgOfflineTraining.Mode.Shopping && p.OnLogin.MacAddress == client.OnLogin.MacAddress).ToArray().Length >= 10)//bahaa
                //{
                //    using (RecycledPacket recycledPacket2 = new RecycledPacket())
                //    {
                //        Packet stream5;
                //        stream5 = recycledPacket2.GetStream();
                //        string Messaj4;
                //        Messaj4 = $"You can`t open more {ServerKernel.MAX_USER_LOGIN_ON_PC} accounts on this pc.";
                //        client.Send(new MsgMessage(Messaj4, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream5));
                //        return;
                //    }
                //}
                if (Program.OnMainternance)
                {
                    using (RecycledPacket recycledPacket2 = new RecycledPacket())
                    {
                        Packet stream13;
                        stream13 = recycledPacket2.GetStream();
                        string Messaj6;
                        Messaj6 = "The server will be brought down for maintenance in (5 Minutes).";
                        client.Send(new MsgMessage(Messaj6, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream13));
                        return;
                    }
                }
                if (!ServerKernel.ServerManager.OnLogged)
                {
                    using (RecycledPacket recycledPacket3 = new RecycledPacket())
                    {
                        Packet stream12;
                        stream12 = recycledPacket3.GetStream();
                        string Messaj5;
                        Messaj5 = "This account cannot enter the server right now.";
                        client.Send(new MsgMessage(Messaj5, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream12));
                        return;
                    }
                }

                if (ServerKernel.ServerManager.TestUser)
                {
                    using (RecycledPacket recycledPacket4 = new RecycledPacket())
                    {
                        Packet stream11;
                        stream11 = recycledPacket4.GetStream();
                        string Messaj4;
                        Messaj4 = "Game Server can't accept any more connection. Please wait Project Manager instruction";
                        client.Send(new MsgMessage(Messaj4, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream11));
                        return;
                    }
                }
                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) == Client.ServerFlag.CreateCharacterSucces)
                {
                    if (Database.ServerDatabase.AllowCreate(client.ConnectionUID))
                    {
                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacterSucces;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (client.Player.SubClass == null)
                                client.Player.SubClass = new Role.Instance.SubClass();
                            if (client.Player.Flowers == null)
                            {
                                client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
                                client.Player.Flowers.FreeFlowers = 1;
                            }
                            if (client.Player.Nobility == null)
                                client.Player.Nobility = new Role.Instance.Nobility(client);
                            if (client.Player.Associate == null)
                            {
                                client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                                client.Player.Associate.MyClient = client;
                                client.Player.Associate.Online = true;
                            }
                            Database.ServerDatabase.CreateCharacte(client);
                            Database.ServerDatabase.SaveClient(client);
                            client.CharReadyCreate(stream);
                        }
                        return;
                    }
                }
                if ((client.ClientFlag & ServerFlag.AcceptLogin) == ServerFlag.AcceptLogin)
                    return;
                MsgLoginClient login;
                login = client.OnLogin;
                client.ConnectionUID = login.Key;
                if (SystemBannedAccount.IsUsingCheat(client.ConnectionUID, out BanMessaje))
                {
                    using (RecycledPacket recycledPacket7 = new RecycledPacket())
                    {
                        Packet stream8;
                        stream8 = recycledPacket7.GetStream();
                        string aMessaj;
                        aMessaj = BanMessaje ?? "";
                        client.Send(new MsgMessage(aMessaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream8));
                        return;
                    }
                }
                string Messaj2;
                Messaj2 = "NEW_ROLE";
                if (!ServerDatabase.AllowCreate(login.Key))
                {
                    if (Server.GamePoll.TryGetValue(login.Key, out var InGame))
                    {
                        if (InGame.Player != null)
                        {
                            ServerKernel.Log.SaveLog($"{InGame.Player.Name} try to logged once account 2 times.", true, "Login_Server");
                            if (InGame.Player.UID == 0)
                            {
                                Server.GamePoll.TryRemove(login.Key, out InGame);
                                if (InGame != null && InGame.Player != null)
                                    InGame.Map?.Denquer(InGame);
                            }
                        }
                        if (InGame.Player.OfflineTraining == MsgOfflineTraining.Mode.Hunting)
                        {
                            InGame.Player.OfflineTraining = MsgOfflineTraining.Mode.Completed;
                            InGame.Socket.Disconnect();
                            Messaj2 = "Offline Mode: \n You were stuck and Auto-Hunting was interrupted.\n Try again later.";
                            using (RecycledPacket recycledPacket8 = new RecycledPacket())
                            {
                                Packet stream7;
                                stream7 = recycledPacket8.GetStream();
                                client.Send(new MsgMessage(Messaj2, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream7));
                            }
                        }
                        else if (InGame.Player.OfflineTraining == MsgOfflineTraining.Mode.Shopping)
                        {
                            InGame.Player.OfflineTraining = MsgOfflineTraining.Mode.Completed;
                            InGame.Socket.Disconnect();
                            Messaj2 = "Offline Mode: \n You were stuck and Auto-Vending was interrupted.\n Try again later.";
                            using (RecycledPacket recycledPacket9 = new RecycledPacket())
                            {
                                Packet stream6;
                                stream6 = recycledPacket9.GetStream();
                                client.Send(new MsgMessage(Messaj2, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream6));
                            }
                        }
                        else if (InGame.Player.OfflineTraining == MsgOfflineTraining.Mode.TrainingGroup)
                        {
                            InGame.Player.OfflineTraining = MsgOfflineTraining.Mode.Completed;
                            InGame.Socket.Disconnect();
                            Messaj2 = "Offline Mode: \n You were stuck and Auto Training-Group was interrupted.\n Try again later.";
                            using (RecycledPacket recycledPacket10 = new RecycledPacket())
                            {
                                Packet stream5;
                                stream5 = recycledPacket10.GetStream();
                                client.Send(new MsgMessage(Messaj2, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream5));
                            }
                        }
                        else
                        {
                            InGame.Socket.Disconnect();
                            Messaj2 = "you are already logged in from another computer.\n Try again later.";
                            using (RecycledPacket recycledPacket11 = new RecycledPacket())
                            {
                                Packet stream4;
                                stream4 = recycledPacket11.GetStream();
                                client.Send(new MsgMessage(Messaj2, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream4));
                            }
                        }
                        if (InGame.TRyDisconnect-- == 0)
                        {
                            if (InGame.Player != null && InGame.FullLoading)
                            {
                                InGame.ClientFlag |= ServerFlag.Disconnect;
                                ServerDatabase.SaveClient(InGame);
                            }
                            Server.GamePoll.TryRemove(login.Key, out InGame);
                            if (InGame != null && InGame.Player != null)
                                InGame.Map?.Denquer(InGame);
                        }
                        return;
                    }
                    Server.GamePoll.TryAdd(login.Key, client);
                    Messaj2 = "ANSWER_OK";
                    using (RecycledPacket recycledPacket12 = new RecycledPacket())
                    {
                        Packet stream3;
                        stream3 = recycledPacket12.GetStream();
                        if ((client.ClientFlag & ServerFlag.CreateCharacterSucces) != ServerFlag.CreateCharacterSucces)
                            ServerDatabase.LoadCharacter(client, login.Key);
                        client.ClientFlag |= ServerFlag.AcceptLogin;
                        client.IP = client.Socket.RemoteIp;
                        ServerKernel.Log.SaveLog($"User [{client.Player.Name}] has logged in [{client.IP}]", true, LogType.MESSAGE);
                        try
                        {
                            if (PlayersIP.TryGetValue(client.IP, out var ListP))
                            {
                                if (!ListP.Contains(client.Player.Name))
                                    PlayersIP[client.IP].Add(client.Player.Name);
                            }
                            else
                                PlayersIP.TryAdd(client.IP, new List<string> { client.Player.Name });
                        }
                        catch
                        {
                            ServerKernel.Log.SaveLog(string.Format("faild to get Players IPs", client.Player.Name, client.IP), true, LogType.MESSAGE);
                        }
                        client.Send(new MsgMessage(Messaj2, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream3));
                        client.Send(stream3.LoginHandlerCreate(1, client.Player.Map));
                        MsgLoginHandler.LoadMap(client, stream3);
                        return;
                    }
                }
                client.ClientFlag |= ServerFlag.CreateCharacter;
                using (RecycledPacket recycledPacket13 = new RecycledPacket())
                {
                    Packet stream2;
                    stream2 = recycledPacket13.GetStream();
                    client.Send(new MsgMessage(Messaj2, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream2));
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
