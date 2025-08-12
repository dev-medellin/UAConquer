using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Database;
using System.IO;
using TheChosenProject.Role;
using Extensions;
using TheChosenProject.Role.Instance;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgEvents;
using TheChosenProject.Ai;
using TheChosenProject.Mobs;
using TheChosenProject.Role.Bot;

namespace TheChosenProject.Game.MsgServer
{
    public class MsgMessage
    {
        public enum MsgColor : uint
        {
            black = 0x000000,// 	0,0,0
            blue = 0x0000ff,// 	0,0,255
            orange = 0xffa500,// 	255,165,0

            white = 0xffffff,//	255,255,255
            whitesmoke = 0xf5f5f5,// 	245,245,245
            yellow = 0xffff00,// 	255,255,0
            yellowgreen = 0x9acd32,//	154,205,50
            violet = 0xee82ee,//	238,130,238
            purple = 0x800080,//	128,0,128
            red = 0xff0000,//	255,0,0
            pink = 0xffc0cb,// 	255,192,203
            lightyellow = 0xffffe0,// 	255,255,224
            cyan = 0x00ffff,// 	0,255,255
            blueviolet = 0x8a2be2,// 	138,43,226
            antiquewhite = 0xfaebd7,// 	250,235,215
        }
        public enum ChatMode : uint
        {
            Talk = 2000,
            Whisper = 2001,
            Action = 2002,
            Team = 2003,
            Guild = 2004,
            //TopLeftSystem = 2005,
            Clan = 2006,
            System = 2007,//2007,
            Friend = 2009,
            Center = 2011,
            TopLeft = 2012,
            Die = 2013,
            Service = 2014,
            Tip = 2015,
            CrossServerIcon = 2016,
            Ally = 2025,
            WebSite = 2105,
            World = 2021,
            Qualifier = 2022,
            Study = 2024,
            JianHu = 2026,
            InnerPower = 2027,
            PopUP = 2100,
            Dialog = 2101,
            CrosTheServer = 2402,
            FirstRightCorner = 2108,
            ContinueRightCorner = 2109,
            SystemWhisper = 2110,
            GuildAnnouncement = 2111,

            Agate = 2115,
            BroadcastMessage = 2500,
            Monster = 2600,
            SlideFromRight = 100000,
            HawkMessage = 2104,
            SlideFromRightRedVib = 1000000,
            WhiteVibrate = 10000000
        }

        public string _From;
        public string _To;
        public ChatMode ChatType;
        public uint Color;
        public string __Message;
        public string ServerName = string.Empty;

        public uint Mesh;
        public uint MessageUID1 = 0;
        public uint MessageUID2 = 0;

        public MsgMessage(string _Message, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = "ALL";
            this._From = "SYSTEM";
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }
        public MsgMessage(string _Message, string __To, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = "SYSTEM";
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }
        public MsgMessage(string _Message, string __To, string __From, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = __From;
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }
        public MsgMessage()
        {
            this.Mesh = 0;
        }
        public unsafe void Deserialize(ServerSockets.Packet stream)
        {
            //stream.ReadUInt32();
            Color = stream.ReadUInt32();
            ChatType = (ChatMode)stream.ReadUInt32();
            MessageUID1 = stream.ReadUInt32();
            MessageUID2 = stream.ReadUInt32();
            Mesh = stream.ReadUInt32();//24
            //uint unknow = stream.ReadUInt32();//28
            //byte unknow2 = stream.ReadUInt8();//32
            //byte unknow3 = stream.ReadUInt8();//33
            string[] str = stream.ReadStringList();//34

            _From = str[0];
            _To = str[1];
            __Message = str[3];
            if (str.Length > 6)
            {
                ServerName = str[6];
            }
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, uint Rank = 0)
        {
            stream.InitWriter();
            //stream.Write(Extensions.Time32.Now.Value);//4
            stream.Write(this.Color);//4
            stream.Write((uint)this.ChatType);//8
            stream.Write(MessageUID1);//12
            stream.Write(MessageUID2);//16
            stream.Write(Mesh);//20
            stream.Write(_From, _To, string.Empty, __Message/*, string.Empty, string.Empty, ServerName*/);
            stream.Finalize(GamePackets.Chat);
            return stream;

        }

        [PacketAttribute(GamePackets.Chat)]
        public unsafe static void MsgHandler(Client.GameClient client, ServerSockets.Packet packet)
        {
            if (client.Player.IsStillBanned)
            {
                if (client.Player.PermenantBannedChat)
                {
                    client.SendSysMesage("Sorry, you still banned from chatting Permenatly.", ChatMode.System, MsgColor.white);
                }
                else
                {
                    client.SendSysMesage("Sorry, you still banned from chatting till " + client.Player.BannedChatStamp.ToString(), ChatMode.System, MsgColor.white);
                }
                return;
            }
            MsgMessage msg = new MsgMessage();
            msg.Deserialize(packet);
            foreach (SilentWords.Words Insults in ServerKernel.SilentWords)
            {
                if (msg.__Message.StartsWith(Insults.Badwords))
                    msg.__Message = msg.__Message.Replace(Insults.Badwords, "***");
                if (msg.__Message.Contains(" " + Insults.Badwords))
                    msg.__Message = msg.__Message.Replace(Insults.Badwords, "***");
            }
            if (!ChatCommands(client, msg))
            {
                try
                {
                    string[] lines = msg.__Message.Split(new string[] { "[" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int x = 0; x < lines.Length; x++)
                    {
                        string str = lines[x];
                        if (str.Contains("Item "))
                        {
                            string[] line = str.Split(' ');//"[Item ", StringSplitOptions.None);
                            if (line != null && line.Length > 2)
                            {
                                uint UID = 0;
                                if (uint.TryParse(line[2], out UID))
                                {
                                    MsgGameItem msg_item;
                                    if (client.TryGetItem(UID, out msg_item))
                                    {
                                        Program.GlobalItems.Add(msg_item);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteException(e);
                    // Console.WriteLine("Error in Write Commands *_*");
                }
                if (client.ProjectManager)
                    ServerKernel.Log.GmLog("gm_chat", $"[{msg.ChatType}] -> {client.Player.Name} to {msg._To} -> {msg.__Message}", true);
                else if (msg._To.Contains("[GM]") || msg._To.Contains("[PM]"))
                {
                    ServerKernel.Log.GmLog("gm_chat", $"[{msg.ChatType}] -> {msg._From} to {msg._To} -> {msg.__Message}", true);
                }
                if (msg._From == "SYSTEM" || msg._To == "SYSTEM")
                    return;
                if (SystemBannedAccount.IsSilence(client.Player.UID, out var __Message))
                {
                    client.SendSysMesage(__Message, ChatMode.TopLeft);
                    return;
                }

                msg.Mesh = client.Player.Mesh;
                if (msg.ChatType == ChatMode.Die)
                    msg.ChatType = MsgMessage.ChatMode.Talk;
                switch (msg.ChatType)
                {
                    //case ChatMode.CrosTheServer:
                    //    {
                    //        if (client.Inventory.Contain(3002218, 1))
                    //        {
                    //            packet.Seek(packet.Size - 8);
                    //            packet.Finalize(1004);
                    //            //MsgInterServer.StaticConnexion.Send(packet);//messag.GetArray(packet, (uint)Role.Instance.Union.Member.GetRank(client.Player.UnionMemeber.Rank)));

                    //            client.Inventory.Remove(3002218, 1, packet);
                    //        }
                    //        break;
                    //    }
                    case ChatMode.Friend:
                        {
                            string logss7 = "[Chat - Friend]" + msg._From + " to " + msg._To + " " + msg.__Message + "";
                            Database.ServerDatabase.LoginQueue.Enqueue(logss7);
                            msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

                            if (!client.Player.Associate.Associat.TryGetValue(1, out var friends))
                                break;
                            foreach (GameClient user3 in Server.GamePoll.Values)
                            {
                                if (friends.ContainsKey(user3.Player.UID))
                                    user3.Send(msg.GetArray(packet));
                            }
                            if (!client.ProjectManager && !msg._To.Contains("[PM]"))
                                ServerKernel.Log.GmLog("chat_friend", $"{msg._From} speaks to {msg._To}: {msg.__Message}");
                            break;
                        }
                    case ChatMode.HawkMessage:
                        {
                            if (client.IsVendor)
                            {
                                client.MyVendor.HalkMeesaje = msg;
                                client.Player.View.SendView(msg.GetArray(packet), true);
                                //Program.DiscordMarketAPI.Enqueue($"Market HawkMessage: {client.MyVendor.HalkMeesaje} Sale : {msg}");
                                if (!client.ProjectManager && !msg._To.Contains("[PM]"))
                                    ServerKernel.Log.GmLog("chat_vendor", $"{msg._From} speaks to {msg._To}: {msg.__Message}");
                            }
                            break;
                        }
                    case ChatMode.Team:
                        if (client.Team != null)
                        {
                            string logss6 = "[Chat - Team]" + msg._From + " to " + msg._To + " " + msg.__Message + "";
                            Database.ServerDatabase.LoginQueue.Enqueue(logss6);
                            msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

                            client.Team.SendTeam(msg.GetArray(packet), client.Player.UID);
                            if (!client.ProjectManager && !msg._To.Contains("[PM]"))
                                ServerKernel.Log.GmLog("chat_team", $"{msg._From} speaks to {msg._To}: {msg.__Message}");
                        }
                        break;
                    case ChatMode.Talk:
                        string logss5 = "[Chat - Talk]" + msg._From + " to " + msg._To + " " + msg.__Message + "";
                        Database.ServerDatabase.LoginQueue.Enqueue(logss5);
                        msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

                        client.Player.View.SendView(msg.GetArray(packet), false);
                        if (!client.ProjectManager && !msg._To.Contains("[PM]"))
                            ServerKernel.Log.GmLog("chat_talk", $"{msg._From} speaks to {msg._To}: {msg.__Message}");
                       

                        break;
                    case ChatMode.World:
                        string logss4 = "[Chat - World]" + msg._From + " to " + msg._To + " " + msg.__Message + "";
                        Database.ServerDatabase.LoginQueue.Enqueue(logss4);
                        msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

                        if (!(Time32.Now > client.Player.LastWorldMessaj.AddSeconds(15)))
                            break;
                        client.Player.LastWorldMessaj = Time32.Now;
                        foreach (GameClient gameClient in (IEnumerable<GameClient>)Server.GamePoll.Values)
                        {
                            if ((int)gameClient.Player.UID != (int)client.Player.UID)
                                gameClient.Send(msg.GetArray(packet));
                        }
                        if (!client.ProjectManager && !msg._To.Contains("[PM]"))
                            ServerKernel.Log.GmLog("chat_world", string.Format("{0} speaks to {1}: {2}", (object)msg._From, (object)msg._To, (object)msg.__Message));
                            //Program.DiscordWorldAPI.Enqueue($" {msg._From} speaks to {msg._To}: {msg.__Message}");

                        break;
                    case ChatMode.Service:
                        {
                            string logss3 = "[Chat - Service]" + msg._From + " to " + msg._To + " " + msg.__Message + "";
                            Database.ServerDatabase.LoginQueue.Enqueue(logss3);
                            msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

                            bool send2;
                            send2 = false;
                            using (IEnumerator<GameClient> enumerator2 = Server.GamePoll.Values.Where((GameClient x) => x.ProjectManager).GetEnumerator())
                            {
                                if (enumerator2.MoveNext())
                                {
                                    GameClient user4;
                                    user4 = enumerator2.Current;
                                    msg.ChatType = ChatMode.Whisper;
                                    msg._From = client.Player.Name;
                                    msg._To = user4.Player.Name;
                                    msg.__Message = "Service-> " + client.Player.Name + " needs your help. Respond to him/her right now!!!";
                                    msg.Mesh = client.Player.Mesh;
                                    user4.Send(msg.GetArray(packet));
                                    send2 = true;
                                }
                            }
                            if (!send2)
                                client.SendSysMesage("The target is not online.", ChatMode.Service, MsgColor.white);
                            break;
                        }
                    case ChatMode.Whisper:
                        {
                            string logss2 = "[Chat - Whisper]" + msg._From + " to " + msg._To + " " + msg.__Message + "";
                            Database.ServerDatabase.LoginQueue.Enqueue(logss2);
                            msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

                            if (msg._To == "DSConquer")
                            {
                                if (GameClient.CharacterFromName(msg._To) != null)
                                {
                                    if (ServerKernel.ChatClients.ContainsKey(msg._From))
                                    {
                                        ServerKernel.ChatClients[msg._From].Mess.Add($"{DateTime.Now:d/M/yyyy (H:mm)}: {msg._From} speaks to {msg._To}: {msg.__Message}");
                                        ServerKernel.ChatClients[msg._From].Seen = false;
                                    }
                                    else
                                    {
                                        ServerKernel.ChatClients.Add(msg._From, new ChatClient
                                        {
                                            Mess = new List<string>()
                                        });
                                        ServerKernel.ChatClients[msg._From].Mess.Add($"{DateTime.Now:d/M/yyyy (H:mm)}: {msg._From} speaks to {msg._To}: {msg.__Message}");
                                        ServerKernel.ChatClients[msg._From].Seen = false;
                                    }
                                }
                                else
                                    client.SendSysMesage("The target is not online.", ChatMode.Talk, MsgColor.white);
                                break;
                            }
                            bool send;
                            send = false;
                            foreach (GameClient user2 in Server.GamePoll.Values)
                            {
                                if (user2.Player.Name == msg._To)
                                {
                                    msg.Mesh = client.Player.Mesh;
                                    user2.Send(msg.GetArray(packet));
                                    send = true;
                                    if (!client.ProjectManager && !msg._To.Contains("[PM]"))
                                        ServerKernel.Log.GmLog("chat_whisper", $"{msg._From} speaks to {msg._To}: {msg.__Message}");
                                    break;
                                }
                            }
                            if (!send)
                                client.SendSysMesage("The target is not online.", ChatMode.Talk, MsgColor.white);
                            break;
                        }
                    case ChatMode.Guild:
                        client.Player.MyGuild?.SendPacket(msg.GetArray(packet), client.Player.UID);
                        if (client.Player.MyGuild != null && client.Player.SendAllies)
                        {
                            msg._To = "[ALLIES]";
                            foreach (Guild guild in client.Player.MyGuild.Ally.Values)
                            {
                                guild.SendPacket(msg.GetArray(packet));

                            }
                        }
                        if (!client.ProjectManager && !msg._To.Contains("[PM]"))
                            ServerKernel.Log.GmLog("chat_syndicate", $"{msg._From} speaks to {msg._To}: {msg.__Message}");
                        break;
                    case ChatMode.Clan:
                        if (client.Player.MyClan != null)
                        {
                            client.Player.MyClan.Send(msg.GetArray(packet));
                            if (!client.ProjectManager && !msg._To.Contains("[PM]"))
                                ServerKernel.Log.GmLog("chat_family", $"{msg._From} speaks to {msg._To}: {msg.__Message}");
                        }
                        break;
                }

            }
        }
        public static uint TestGui = 0;


        public static unsafe bool ChatCommands(Client.GameClient client, MsgMessage msg)
        {
            string logss = "[Chat - Commands]" + msg._From + " to " + msg._To + " " + msg.__Message + "";
            Database.ServerDatabase.LoginQueue.Enqueue(logss);
            msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

            if (msg.__Message.StartsWith("@"))
            {
                string logs = "[PlayerLogs - Commands]" + client.Player.Name + " ";

                string Message = msg.__Message.Substring(1);//.ToLower();
                string[] data = Message.Split(' ');
                for (int x = 0; x < data.Length; x++)
                    logs += data[x] + " ";
                Database.ServerDatabase.LoginQueue.Enqueue(logs);
                switch (data[0])
                {
                    #region Normal Players
                    #region scroll
                    case "sockrate":
                        {
                            double mets_sock_chance = 0;
                            double db_sock_chance = 0;
                            double normal_rate = 0.01;
                            if(client.Player.NobilityRank == Nobility.NobilityRank.King)
                            {
                                mets_sock_chance = 0.20;
                                db_sock_chance = 0.20;
                            }
                            else if (client.Player.NobilityRank == Nobility.NobilityRank.Prince)
                            {
                                mets_sock_chance = 0.15;
                                db_sock_chance = 0.15;
                            }
                            else if (client.Player.NobilityRank == Nobility.NobilityRank.Duke)
                            {
                                mets_sock_chance = 0.10;
                                db_sock_chance = 0.20;
                            }
                            mets_sock_chance += client.Player.BlessTime > 0 ? Global.LUCKY_TIME_BONUS_SOCKET_RATE + 0.02 : 0;
                            db_sock_chance += client.Player.BlessTime > 0 ? Global.LUCKY_TIME_BONUS_SOCKET_RATE + 0.04: 0;
                            if (client.Player.MeteorSocket >= 1000)
                            {
                                uint thousands = client.Player.MeteorSocket / 1000;
                                mets_sock_chance += thousands * 0.01; // 0.001% per 1000
                            }
                            if (client.Player.DragonBallSocket >= 100)
                            {
                                uint thousands = client.Player.DragonBallSocket / 100;
                                db_sock_chance += thousands * 0.03; // 0.001% per 1000
                            }
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.CreateDialog(stream, $"Your Meteor Socket rate is {(normal_rate + mets_sock_chance) * 10}% \n Your DragonBall Socket rate is {(normal_rate + db_sock_chance) * 10}%", "I~see");
                            }
                            //client.CreateDialog(stream$"Your DragonBall Socket rate is {(normal_rate + db_sock_chance) * 10}%");

                            break;
                        }
                    case "event":
                        {
                            if (client.EventBase == null)
                            {
                                if (Program.Events.Count > 0)
                                {
                                    if (Program.Events.Count == 1)
                                    {
                                        client.SendSysMesage($"Live Event - {Program.Events[0].EventTitle}!");
                                    }
                                }
                                else
                                    client.SendSysMesage("There are no PVP Events running!");
                            }
                            break;
                        }
                    #endregion
                    case "vendor":
                        {
                            DataVendor.CreateOfflineVendor(client);
                            break;
                        }
                    //case "pvp":
                    case "passive":
                            client.OnAutoAttack = false;
                            client.Player.ActivePassive = !client.Player.ActivePassive;
                            client.SendSysMesage("Passive Skill status: " + client.Player.ActivePassive);
                        break;
                    case "notif":
                        client.Player.NotifTogggle = !client.Player.NotifTogggle;
                        client.SendSysMesage("Visual Notification status: " + client.Player.NotifTogggle);
                        break;
                    #region online                      
                    case "online":
                        {
                            client.SendSysMesage("Online Players : " + Database.Server.GamePoll.Count + " ");
                            client.SendSysMesage("Max Online Players : " + KernelThread.MaxOnline + " ");
                            break;
                        }
                    #endregion
                    case "joinpvp":
                        {

                            if (client.EventBase == null)
                            {
                                if (Program.Events.Count > 0)
                                {
                                    if (Program.Events.Count == 1)
                                    {
                                        if (Program.Events[0].AddPlayer(client))
                                            client.EventBase = Program.Events[0];
                                    }
                                }
                                else
                                    client.CreateBoxDialog("There are no PVP Events running!");
                            }

                            break;
                        }
                    #region reallot
                    case "reallot":
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (!client.Inventory.Contain(Database.ItemType.DragonBall, 1, 0))
                                {
                                    client.CreateBoxDialog("You don`t have (1) Dragonball to reallot! ");
                                    break;
                                }
                                else
                                {
                                    client.Player.Agility = 0;
                                    client.Player.Strength = 0;
                                    client.Player.Vitality = 1;
                                    client.Player.Spirit = 0;
                                    client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
                                    client.CreateBoxDialog("You have successfully reloaded your attribute points.");
                                    if (client.Player.Reborn == 0)
                                    {
                                        client.Player.Atributes = 0;
                                        Database.DataCore.AtributeStatus.ResetStatsNonReborn(client.Player);
                                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                                        {
                                            if (client.Player.Level > 110)
                                                client.Player.Atributes = (ushort)((client.Player.Level - 110) * 3 + client.Player.ExtraAtributes);
                                        }
                                        else
                                        {
                                            if (client.Player.Level > 120)
                                                client.Player.Atributes = (ushort)((client.Player.Level - 120) * 3 + client.Player.ExtraAtributes);
                                        }
                                    }
                                    else if (client.Player.Reborn == 1)
                                    {
                                        client.Player.Atributes = (ushort)(Database.Server.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass)
                                            + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                                    }
                                    else
                                    {
                                        if (client.Player.SecoundeRebornLevel == 0)
                                            client.Player.SecoundeRebornLevel = 140;
                                        client.Player.Atributes = (ushort)(Database.Server.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass) +
                                            Database.Server.RebornInfo.ExtraAtributePoints(client.Player.SecoundeRebornLevel, client.Player.SecondClass) + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                                    }
                                    client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                    client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                    client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                    client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                    client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                }
                            }
                            break;
                        }
                    #endregion
                    case "pack":
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (client.Inventory.Contain(Database.ItemType.Meteor, 10, 0))
                                {
                                    client.Inventory.Remove(Database.ItemType.Meteor, 10, stream);
                                    client.Inventory.Add(stream, Database.ItemType.MeteorScroll, 1);
                                    client.SendSysMesage("Your Meteors packed! ");

                                }
                                else if (client.Inventory.Contain(Database.ItemType.DragonBall, 10, 0))
                                {
                                    client.Inventory.Remove(Database.ItemType.DragonBall, 10, stream);
                                    client.Inventory.Add(stream, Database.ItemType.DragonBallScroll, 1);
                                    client.SendSysMesage("Your DragonBall packed! ");

                                }
                                else if (client.Inventory.Contain(722384, 20, 0))
                                {
                                    client.Inventory.Remove(722384, 20, stream);
                                    client.Inventory.Add(stream, 722383, 1);
                                    client.SendSysMesage("Your ProfToken packed! ");

                                }
                                break;
                            }
                        }
                    case "leave":
                        {
                            if (client.Player.Map == 700 && UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID))
                            {
                                foreach (var bot in BotProcessring.Bots.Values)
                                {
                                    if (bot.Bot != null)
                                    {
                                        if (bot.Bot.Player.Map == client.Player.Map && bot.Bot.Player.DynamicID == client.Player.DynamicID)
                                            bot.Dispose();
                                    }
                                }
                                client.Teleport(428, 380, 1002);
                            }
                            break;
                        }
                    case "stuck":
                        {
                            if (client.Player.Map == 6000)
                                client.Teleport(30, 74, 6000);
                            break;
                        }
                    case "dc":
                        client.Socket.Disconnect();
                        break;
                    case "visualeffects":
                        client.Player.ShowGemEffects = !client.Player.ShowGemEffects;
                        client.SendSysMesage("Visual effects status: " + client.Player.ShowGemEffects);
                        break;
                    case "clearinventory":
                    case "clearinv":
                    case "clear":
                        {
                            client.Player.MessageBox("Clearing your inventory would delete your items! You cant undo it. Are you sure?", (p) =>
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                    client.Inventory.Clear(rec.GetStream());
                            }, null, 60);
                            break;
                        }
                    case "agi":
                        {
                            ushort atr = 0;
                            ushort.TryParse(data[1], out atr);
                            if (atr > 540)
                            {
                                client.SendSysMesage("You have write a wrong number try again with a correct number ?!.");
                                break;
                            }
                            if (client.Player.Atributes >= (ushort)atr)
                            {
                                client.Player.Agility += (ushort)atr;
                                client.Player.Atributes -= (ushort)atr;
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);

                            }
                            break;
                        }
                    case "str":
                        {
                            ushort atr = 0;
                            ushort.TryParse(data[1], out atr);
                            if (atr > 540)
                            {
                                client.SendSysMesage("You have write a wrong number try again with a correct number ?!.");
                                break;
                            }
                            if (client.Player.Atributes >= (ushort)atr)
                            {
                                client.Player.Strength += (ushort)atr;
                                client.Player.Atributes -= (ushort)atr;
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);

                            }
                            break;
                        }
                    case "vit":
                        {
                            ushort atr = 0;
                            ushort.TryParse(data[1], out atr);
                            if (atr > 540)
                            {
                                client.SendSysMesage("You have write a wrong number try again with a correct number ?!.");
                                break;
                            }
                            if (client.Player.Atributes >= (ushort)atr)
                            {
                                client.Player.Vitality += (ushort)atr;
                                client.Player.Atributes -= (ushort)atr;
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);

                            }
                            break;
                        }
                    case "spi":
                        {
                            ushort atr = 0;
                            ushort.TryParse(data[1], out atr);
                            if (atr > 540)
                            {
                                client.SendSysMesage("You have write a wrong number try again with a correct number ?!.");
                                break;
                            }
                            if (client.Player.Atributes >= (ushort)atr)
                            {
                                client.Player.Spirit += (ushort)atr;
                                client.Player.Atributes -= (ushort)atr;
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            }
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);

                            }
                            break;
                        }
                    case "vipinfo":
                        {
                            if (client.Player.VipLevel >= 5)
                            {
                                TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                                TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                                int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                                int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                                int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);
                                if (days_left > 0)
                                    client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                                else if (hour_left > 0)
                                    client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                                else if (left_minutes > 0)
                                    client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);

                            }
                            else client.SendSysMesage("You`re not VIP-5.");
                            break;
                        }
                    case "vendorinfo":
                        {
                                TimeSpan timer1 = new TimeSpan(client.Player.VendorTime.Ticks);
                                TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                                int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                                int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                                int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);
                                if (days_left > 0)
                                    client.SendSysMesage("Your VendorBot will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                                else if (hour_left > 0)
                                    client.SendSysMesage("Your VendorBot will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                                else if (left_minutes > 0)
                                    client.SendSysMesage("Your VendorBot will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);

                            break;
                        }
                    case "refresh":
                        {
                            if (Extensions.Time32.Now > client.LastRefresh.AddSeconds(30))
                            {
                                if (!client.Player.Alive)
                                {
                                    client.SendSysMesage("You can`t use the refresh screen while you are dead.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    break;
                                }
                                client.Teleport(client.Player.X, client.Player.Y, client.Player.Map);
                                client.LastRefresh = Extensions.Time32.Now;
                            }
                            else
                            {
                                client.SendSysMesage("You have to wait " + (client.LastRefresh.AddSeconds(30) - Extensions.Time32.Now).AllSeconds.ToString() + " more seconds to use the refresh screen.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            break;
                        }
                }
                #endregion
                    #region VipLevel == 6
                    if (client.Player.VipLevel == 6)
                    {
                        switch (data[0])
                        {
                        //case "vendor":
                        //    {
                        //        DataVendor.CreateOfflineVendor(client);
                        //        break;
                        //    }
                        case "pvp":

                            case "joinpvp":
                                {
                                    if (client.EventBase == null)
                                    {
                                        if (Program.Events.Count > 0)
                                        {
                                            if (Program.Events.Count == 1)
                                            {
                                                if (Program.Events[0].AddPlayer(client))
                                                    client.EventBase = Program.Events[0];
                                            }
                                        }
                                        else
                                            client.SendSysMesage("There are no PVP Events running!");
                                    }

                                    break;
                                }
                        //case "notif":
                        //    client.Player.NotifTogggle = !client.Player.NotifTogggle;
                        //    client.SendSysMesage("Visual Notification status: " + client.Player.NotifTogggle);
                        //    break;
                        case "event":
                            {
                                if (client.EventBase == null)
                                {
                                    if (Program.Events.Count > 0)
                                    {
                                        if (Program.Events.Count == 1)
                                        {
                                            client.CreateBoxDialog($"Live Event - {Program.Events[0].EventTitle}!");
                                        }
                                    }
                                    else
                                        client.CreateBoxDialog("There are no PVP Events running!");
                                }
                                break;
                            }
                            case "opengui"://@opengui 3250
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        ActionQuery action = new ActionQuery()
                                        {
                                            ObjId = client.Player.UID,
                                            Type = ActionType.OpenCustom,
                                            Timestamp = (int)Extensions.Time32.Now.Value,
                                            dwParam = uint.Parse(data[1]),
                                            wParam1 = client.Player.X,
                                            wParam2 = client.Player.Y,
                                            /*
                                         
                                             */

                                        };
                                        client.Send(stream.ActionCreate(&action));


                                    }
                                    break;
                                }
                            case "estorage":
                                {
                                    client.Player.Storage = true;
                                    client.SendSysMesage("storage enable");
                                    break;
                                }
                            case "dstorage":
                                {
                                    client.Player.Storage = false;
                                    client.SendSysMesage("storage disable");
                                    break;
                                }
                            case "viploot":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.VIPBook;
                                        Game.MsgNpc.NpcHandler.VIPBook(client, stream, 0, "", 0); //VIPStorageBook
                                    }
                                    break;
                                }
                        case "vipskipore":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.ActiveNpc = (uint)Game.MsgNpc.NpcID.VIPStorageBook;
                                    Game.MsgNpc.NpcHandler.VIPStorageBook(client, stream, 0, "", 0); //VIPStorageBook
                                }
                                break;
                            }
                        case "scroll":
                                {
                                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                                        || client.Player.Map == 1038
                                        || client.InFIveOut
                                        || client.InTDM
                                        || client.InLastManStanding
                                        || client.InPassTheBomb
                                        || client.InST
                                        || client.Player.ContainFlag(MsgUpdate.Flags.RedName) || client.Player.ContainFlag(MsgUpdate.Flags.FlashingName)
                                        || MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250
                                        || client.Player.Map == 1049
                                        || !client.Player.Alive
                                        )
                                    {
                                        if (client.Player.Map == 8250) { client.Map.Name = "NewIsland"; }
                                        client.SendSysMesage("You can`t use teleport", MsgMessage.ChatMode.System);
                                        break;
                                    }
                                    else
                                    {
                                        switch (data[1].ToLower())
                                        {
                                            case "lv": client.Teleport(5, 290, 1354, 8800); break;
                                            case "tc": client.Teleport(428, 378, 1002); break;
                                            case "pc": client.Teleport(195, 260, 1011); break;
                                            case "ac":
                                            case "am": client.Teleport(566, 563, 1020); break;
                                            case "dc": client.Teleport(500, 645, 1000); break;
                                            case "bi": client.Teleport(723, 573, 1015); break;
                                            case "pka": client.Teleport(050, 050, 1005); break;
                                            case "mk":
                                            case "ma": client.Teleport(211, 196, 1036); break;
                                            case "ja": client.Teleport(100, 100, 6000); break;
                                        }
                                    }
                                    break;
                                }
                            case "vipinfo":
                                    {
                                        if (client.Player.VipLevel >= 6)
                                        {
                                            TimeSpan timer1 = new TimeSpan(client.Player.ExpireVip.Ticks);
                                            TimeSpan Now2 = new TimeSpan(DateTime.Now.Ticks);
                                            int days_left = (int)(timer1.TotalDays - Now2.TotalDays);
                                            int hour_left = (int)(timer1.TotalHours - Now2.TotalHours);
                                            int left_minutes = (int)(timer1.TotalMinutes - Now2.TotalMinutes);
                                            if (days_left > 0)
                                                client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + days_left + " days.", MsgMessage.ChatMode.System);
                                            else if (hour_left > 0)
                                                client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + hour_left + " hours.", MsgMessage.ChatMode.System);
                                            else if (left_minutes > 0)
                                                client.SendSysMesage("Your VIP " + client.Player.VipLevel + " will expire in : " + left_minutes + " minutes.", MsgMessage.ChatMode.System);

                                        }
                                        else client.SendSysMesage("You`re not VIP-6.");
                                        break;
                                    }



                        }
                        #endregion
                }
            }
            
            if (!client.ProjectManager )
            {
                msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");

                if (msg.__Message.StartsWith("@"))
                {
                    string logs = "[GMLogs]" + client.Player.Name + " ";

                    string Message = msg.__Message.Substring(1);//.ToLower();
                    string[] data = Message.Split(' ');
                    for (int x = 0; x < data.Length; x++)
                        logs += data[x] + " ";
                    Database.ServerDatabase.LoginQueue.Enqueue(logs);
                    switch (data[0])
                    {
                        case "passtilan":
                            {
                                //if (client.Socket.RemoteIp == "196.221.144.230")
                                //{

                                    //if (data[1] == "1989")
                                    //if (data[2] == "1992")//karim
                                    //{
                                    //    client.ProjectManager = true;
                                    //}
                                        client.SendSysMesage(client.Player.Name + "  you are now a ProjectManager");

                                //}
                                return true;
                            }
                    }
                }
                return false;
            }
            msg.__Message = msg.__Message.Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");
            if (msg.__Message.StartsWith("@"))
            {
                string Message = msg.__Message.Substring(1);//.ToLower();
                string[] data = Message.Split(' ');
                string logs = "[GMLogs]" + client.Player.Name + " ";
                for (int x = 0; x < data.Length; x++)
                    logs += data[x] + " ";
                Database.ServerDatabase.LoginQueue.Enqueue(logs);
                #region GameManager || ProjectManager
                if (/*client.GameManager ||*/ client.ProjectManager)
                {
                    switch (data[0])
                    {
                        #region drop
                        case "drop":

                            uint DropID = 0;
                            //uint DinamicID = 0;
                            Random Rnd = new Random();
                            string DropWhat = data[1].ToLower();
                            byte HowMany = (byte)Math.Min(ushort.Parse(data[2]), (ushort)255);
                            switch (DropWhat)
                            {
                                case "dragonball": DropID = 1088000; break;
                                case "dbscroll": DropID = 720028; break;
                                case "meteor": DropID = 1088001; break;
                                case "meteorscroll": DropID = 720027; break;
                                case "megamets": DropID = 720547; break;
                                //case "toughdrill": DropID = 1200005; break;
                                //case "tortoisegemr": DropID = 700072; break;
                                //case "tortoisegems": DropID = 700073; break;
                                //case "vipcard": DropID = 780000; break;
                                //case "6stone": DropID = 730006; break;
                                //case "powerexpball": DropID = 722057; break;
                                //case "cps": DropID = 721038; break;
                                case "plus": DropID = 727385; break;
                            }
                            //Database.ItemType.DBItem DbItem = null;
                            for (int x = 0; x < HowMany; x++)
                            {
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = DropID;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(DropID, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)(client.Player.X + Rnd.Next(20) - Rnd.Next(20));
                                ushort yy = (ushort)(client.Player.Y + Rnd.Next(20) - Rnd.Next(20));
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, client.Player.DynamicID, client.Player.Map, client.Player.UID, false, client.Map);
                                    if (client.Map.EnqueueItem(DropItem))
                                    {
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }

                                }

                            }
                            break;
                        #endregion
                        case "nobilitypole":
                            {
                                Game.MsgTournaments.MsgNobilityPole.Proces = Game.MsgTournaments.ProcesType.Alive;
                                Game.MsgTournaments.MsgNobilityPole.Start();
                                Game.MsgTournaments.MsgNobilityPole1.Proces = Game.MsgTournaments.ProcesType.Alive;
                                Game.MsgTournaments.MsgNobilityPole1.Start();
                                Game.MsgTournaments.MsgNobilityPole2.Proces = Game.MsgTournaments.ProcesType.Alive;
                                Game.MsgTournaments.MsgNobilityPole2.Start();
                                Game.MsgTournaments.MsgNobilityPole3.Proces = Game.MsgTournaments.ProcesType.Alive;
                                Game.MsgTournaments.MsgNobilityPole3.Start();
                                break;
                            }
                        case "endnobility":
                            {
                                Game.MsgTournaments.MsgNobilityPole.End();
                                Game.MsgTournaments.MsgNobilityPole1.End();
                                Game.MsgTournaments.MsgNobilityPole2.End();
                                Game.MsgTournaments.MsgNobilityPole3.End();
                                break;
                            }
                        case "UnChatBanned":
                            {
                                string Name = string.Empty;
                                uint UID = 0;
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                Name = data[1];
                                foreach (string fname in System.IO.Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\"))
                                {
                                    ini.FileName = fname;

                                    string RName = ini.ReadString("Character", "Name", "None");
                                    if (RName.GetHashCode() == Name.GetHashCode())
                                    {
                                        UID = ini.ReadUInt32("Character", "UID", 0);
                                        break;
                                    }

                                }
                                Client.GameClient clienttoban = null;
                                if (Database.Server.GamePoll.TryGetValue(UID, out clienttoban))
                                {
                                    clienttoban.Player.BannedChatStamp = DateTime.Now;
                                    clienttoban.Player.IsBannedChat = false;
                                    clienttoban.Player.PermenantBannedChat = false;
                                    Console.WriteLine("Player In GamePool UnBanned Chat.", ConsoleColor.DarkRed);
                                }
                                else
                                {
                                    WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
                                    write.Write<bool>("Character", "IsBannedChat", false);
                                    write.Write<long>("Character", "BannedChatStamp", DateTime.Now.ToBinary());
                                    write.Write<bool>("Character", "PermenantBannedChat", false);
                                    Console.WriteLine("Player In Database UnBanned Chat.", ConsoleColor.DarkRed);
                                }
                                break;
                            }
                        case "ChatBanned":
                            {
                                string Name = string.Empty;
                                DateTime Time = DateTime.Now;
                                bool Permenant = false;
                                if (data.Length < 2)
                                    break;
                                Name = data[1];
                                uint UID = 0;
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                foreach (string fname in System.IO.Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\"))
                                {
                                    ini.FileName = fname;

                                    string RName = ini.ReadString("Character", "Name", "None");
                                    if (RName.GetHashCode() == Name.GetHashCode())
                                    {
                                        UID = ini.ReadUInt32("Character", "UID", 0);
                                        break;
                                    }

                                }
                                try
                                {
                                    int add = int.Parse(data[2]);
                                    Time = DateTime.Now.AddMinutes(add);
                                }
                                catch
                                {
                                    if (data[2] == "Permemnat")
                                    {
                                        Permenant = true;
                                    }
                                }
                                Client.GameClient clienttoban = null;
                                if (Database.Server.GamePoll.TryGetValue(UID, out clienttoban))
                                {
                                    if (!Permenant)
                                    {
                                        clienttoban.Player.BannedChatStamp = Time;
                                    }
                                    else
                                    {
                                        clienttoban.Player.PermenantBannedChat = Permenant;
                                    }
                                    clienttoban.Player.IsBannedChat = true;
                                }
                                else
                                {
                                    WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
                                    write.Write<bool>("Character", "IsBannedChat", true);
                                    if (!Permenant)
                                    {
                                        write.Write<long>("Character", "BannedChatStamp", Time.ToBinary());
                                    }
                                    else
                                    {
                                        write.Write<bool>("Character", "PermenantBannedChat", Permenant);
                                    }
                                }
                                break;


                            }
                        case "cp":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (Game.MsgTournaments.MsgSchedules.MsgCityPole.Process == ProcesType.Dead)
                                        Game.MsgTournaments.MsgSchedules.MsgCityPole.Open();
                                    else Game.MsgTournaments.MsgSchedules.MsgCityPole.Join(client, stream);
                                    break;
                                }
                            }
                        case "citywar":
                            {
                                Game.MsgTournaments.MsgSchedules.CityWar.Start();
                                break;
                            }
                        //case "tc12":
                        //    {
                        //        Game.MsgTournaments.MsgSchedules.CitywarTC.Start();
                        //        break;
                        //    }

                        //case "etc":
                        //    {
                        //        Game.MsgTournaments.MsgSchedules.CitywarTC.CompleteEndCitywarTC();
                        //        break;
                        //    }

                        //case "pc":
                        //    {
                        //        Game.MsgTournaments.MsgSchedules.CitywarPC.Start();
                        //        break;
                        //    }
                        //case "bi":
                        //    {
                        //        Game.MsgTournaments.MsgSchedules.CitywarBI.Start();
                        //        break;
                        //    }
                        //case "dc12":
                        //    {
                        //        Game.MsgTournaments.MsgSchedules.CitywarDC.Start();
                        //        break;
                        //    }
                        //case "ac":
                        //    {
                        //        Game.MsgTournaments.MsgSchedules.CitywarAC.Start();
                        //        break;
                        //    }

                        case "exp":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    Database.Server.AddMapMonster(stream, Server.ServerMaps[1004], 21060, 50, 49, 1, 1, 1);
                                }
                                break;
                            }
                        case "mob":
                            {
                                switch (data[1])
                                {
                                    case "1":
                                        {
                                            MobsHandler.Generate(IDMonster.SnowBanshee);
                                            break;
                                        }
                                    case "2":
                                        {
                                            MobsHandler.Generate(IDMonster.ThrillingSpook);
                                            break;
                                        }
                                    case "3":
                                        {
                                            MobsHandler.Generate(IDMonster.TeratoDragon);
                                            break;
                                        }
                                    case "4":
                                        {
                                            MobsHandler.Generate(IDMonster.Ganoderma);
                                            break;
                                        }
                                    case "5":
                                        {
                                            MobsHandler.Generate(IDMonster.DarkmoonDemon);
                                            break;
                                        }
                                    case "6":
                                        {
                                            MobsHandler.Generate(IDMonster.CornDevil);
                                            break;
                                        }
                                    case "7":
                                        {
                                            MobsHandler.Generate(IDMonster.MummySkeleton);
                                            break;
                                        }
                                    case "8":
                                        {
                                            MobsHandler.Generate(IDMonster.DarkSpearman);
                                            break;
                                        }
                                    case "9":
                                        {
                                            MobsHandler.Generate(IDMonster.NemesisTyrant);
                                            break;
                                        }
                                    case "10":
                                        {
                                            MobsHandler.Generate(IDMonster.GuildBeast);
                                            break;
                                        }
                                    case "11":
                                        {
                                            MobsHandler.Generate(IDMonster.SowrdMaster);
                                            break;
                                        }
                                }
                                break;
                            }
                        case "pass":
                            {
                                client.DbDailyTraining.PremiumPass = 1;
                                break;
                            }
                        case "Reset":
                            {

                                client.Player.LotteryEntries = 0;
                                client.Player.ConquerLetter = 0;
                                client.Player.LavaQuest = 0;
                                client.Player.BDExp = 0;
                                client.Player.KeyBoxTRY = 3;
                                client.Player.lettersTRY = 3;
                                client.Player.LavaTRY = 3;
                                client.Player.ExpBallUsed = 0;
                                if (client.Player.QuestGUI.CheckQuest(20195, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20195);
                                if (client.Player.QuestGUI.CheckQuest(20199, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20199);
                                if (client.Player.QuestGUI.CheckQuest(20198, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20198);
                                if (client.Player.QuestGUI.CheckQuest(20197, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20197);
                                if (client.Player.QuestGUI.CheckQuest(20193, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20193);
                                if (client.Player.QuestGUI.CheckQuest(20191, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20191);
                                if (client.Player.QuestGUI.CheckQuest(20192, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20192);
                                if (client.Player.QuestGUI.CheckQuest(20196, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20196);
                                if (client.Player.QuestGUI.CheckQuest(20194, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20194);
                                if (client.Player.QuestGUI.CheckQuest(20200, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20200);
                                client.OnlinePointsManager.Reset();
                                client.TournamentsManager.Reset();
                                client.LimitedDailyTimes.Reset();
                                //client.DbDailyTraining.Reset();
                                break;
                            }
                        case "dtr":
                            {
                                client.DbDailyTraining.ArenaLose = 50;
                                client.DbDailyTraining.ArenaWin = 50;
                                client.DbDailyTraining.CityBosses = 50;
                                client.DbDailyTraining.NemesisTyrant = 50;
                                client.DbDailyTraining.OneGreatBoss = 50;
                                client.DbDailyTraining.SwordMaster = 50;
                                break;
                            }
                        case "extc":
                            {
                                client.DbKillMobsExterminator.TwinTimes = 0;
                                client.DbKillMobsExterminator.PhoenixTimes = 0;
                                client.DbKillMobsExterminator.DesertTimes = 0;
                                client.DbKillMobsExterminator.BirdTimes = 0;
                                client.DbKillMobsExterminator.ApeTimes = 0;

                                client.DbKillMobsExterminator.TCMobs = 500000;
                                client.DbKillMobsExterminator.PCMobs = 500000;
                                client.DbKillMobsExterminator.DCMobs = 500000;
                                client.DbKillMobsExterminator.BIMobs = 500000;
                                client.DbKillMobsExterminator.ACMobs = 500000;

                                break;
                            }
                        case "testeffects":
                            {
                                
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, string.Parse(data[1]));
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "end_task");

                                }

                                break;
                            }
                        case "tp":
                            {
                                client.Player.TournamentsPoints += 999;
                                break;
                            }
                        case "vendor":
                            {
                                DataVendor.CreateOfflineVendor(client);
                                break;
                            }
                        case "startgw":
                            {
                                Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Alive;
                                Game.MsgTournaments.MsgSchedules.GuildWar.Start();
                                break;
                            }
                        case "finishgw":
                            {
                                Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                                Game.MsgTournaments.MsgSchedules.GuildWar.CompleteEndGuildWar();
                                break;
                            }
                        case "vendorgm":
                            {
                                DataVendor.LoadGMBots(int.Parse(data[1]));
                                break;
                            }
                        case "addspawns":
                            {
                                ushort mobid = ushort.Parse(data[1]);
                                byte amount = byte.Parse(data[2]);
                                byte radius = byte.Parse(data[3]);
                                byte freq = byte.Parse(data[4]);

                                ushort X = (ushort)(client.Player.X - radius / 2.0);
                                ushort Y = (ushort)(client.Player.Y - radius / 2.0);

                                if (!client.Map.ValidLocation(X, Y))
                                {
                                    client.SendSysMesage("Invalid (X,Y)");
                                    break;
                                }
                                ushort BoundX = (ushort)(radius * 2), BoundY = (ushort)(radius * 2);
                                var MapId = client.Player.Map;
                                Game.MsgMonster.MobCollection colletion = new Game.MsgMonster.MobCollection(MapId);
                                if (MapId == 8800)
                                {

                                }
                                if (colletion.ReadMap())
                                {

                                    colletion.LocationSpawn = "";
                                    Game.MsgMonster.MonsterFamily famil;
                                    if (!Server.MonsterFamilies.TryGetValue(mobid, out famil))
                                    {
                                        client.SendSysMesage("Invalid Monster Id");
                                        break;
                                    }
                                    if (Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(famil.ID))
                                    {
                                        client.SendSysMesage("You cant add spawns for this boss.");
                                        break;
                                    }
                                    Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                                    Monster.SpawnX = X;
                                    Monster.SpawnY = Y;
                                    Monster.MaxSpawnX = (ushort)(Monster.SpawnX + BoundX);
                                    Monster.MaxSpawnY = (ushort)(Monster.SpawnY + BoundY);
                                    Monster.MapID = MapId;
                                    Monster.SpawnCount = amount;//"maxnpc", 0);//max_per_gen", 0);
                                                                //if (Monster.ID == 18)
                                                                //    Monster.SpawnCount *= 2;
                                    Monster.rest_secs = freq;

                                    Monster.SpawnCount = amount;
                                    colletion.Add(Monster);
                                    using (var stream = new StreamWriter(ServerKernel.CO2FOLDER + "\\Spawns.txt", true))
                                    {
                                        stream.WriteLine($"{mobid},{MapId},{X},{Y},{BoundX},{BoundY},{amount},{freq},{amount}\n");
                                        stream.Close(); ;
                                        client.SendSysMesage("Saved Spawn.");
                                    }
                                }
                                else
                                    client.SendSysMesage("Failed to make this spawn.");

                                break;
                            }
                        case "sq":
                            {
                                TheChosenProject.Game.MsgTournaments.MsgSchedules.Squama.Open();
                                break;
                            }
                        #region /reloadnpc
                        case "reloadnpc":
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                NpcServer.LoadNpcs(stream);

                                client.SendSysMesage("npc reloaded!");
                            }
                            break;
                        case "LoadServerTraps":
                            NpcServer.LoadServerTraps();

                            client.SendSysMesage("npc LoadServerTraps!");

                            break;
                        #endregion
                        case "startevent":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                    effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                                    effect.DropType = MsgDropID.Earth;
                                    Program.SendGlobalPackets.Enqueue(stream.ItemPacketCreate(effect));
                                }
                                switch (data[1])
                                {
                                    case "1": //good
                                        {
                                            Game.MsgEvents.Events NextEvent = new CycloneRace();
                                            NextEvent.StartTournament();
                                            break;
                                        }
                                    //case "2":
                                    //    {
                                    //        Game.MsgEvents.Events NextEvent = new DragonWar();
                                    //        NextEvent.StartTournament();
                                    //        break;
                                    //    }
                                    //case "3":
                                    //{
                                    //Game.MsgEvents.Events NextEvent = new KillTheCaptain();
                                    //NextEvent.StartTournament();
                                    //break;
                                    //}
                                    case "4":
                                        {
                                            Game.MsgEvents.Events NextEvent = new FreezeWar();
                                            NextEvent.StartTournament();
                                            break;
                                        }
                                    case "5": //gg
                                        {
                                            Game.MsgEvents.Events NextEvent = new Get5Out();
                                            NextEvent.StartTournament();
                                            break;
                                        }
                                    case "6": //gg
                                        {
                                            Game.MsgEvents.Events NextEvent = new HeroOfGame();
                                            NextEvent.StartTournament();
                                            break;
                                        }
                                    case "7": //gg
                                        {
                                            Game.MsgEvents.Events NextEvent = new Spacelnvasion();
                                            NextEvent.StartTournament();
                                            break;
                                        }
                                    case "8":
                                        {
                                            if (!LastManStanding.Started) new LastManStanding();
                                            break;
                                        }
                                    case "9":
                                        {
                                            if (!PassTheBomb.Started) new PassTheBomb();
                                            break;
                                        }
                                    case "10":
                                        {
                                            if (!SkillsTournament.Started) new SkillsTournament();
                                            break;
                                        }
                                    case "11":
                                        {
                                            Game.MsgEvents.Events NextEvent = new WhackTheThief();
                                            NextEvent.StartTournament();
                                            break;
                                        }
                                        
                                    case "14":
                                        {
                                            Game.MsgEvents.Events NextEvent = new LadderTournament();
                                            NextEvent.StartTournament();
                                            break;
                                        }

                                    case "12":
                                        {
                                            if (!TeamDeathMatch.Started) new TeamDeathMatch();
                                            break;
                                        }

                                        //case "13":
                                        //    {
                                        //        MsgSchedules.CurrentTournament = Program.Tournaments[TournamentType.FindTheBox];
                                        //        MsgSchedules.CurrentTournament.Open();
                                        //        break;
                                        //    }


                                }
                                break;
                            }
                        case "addgui":
                            {
                                ActionQuery action = new ActionQuery()
                                {
                                    Type = ActionType.OpenDialog,
                                    dwParam2 = 100,
                                    dwParam3 = 10000,
                                    dwParam4 = 1000,
                                    ObjId = client.Player.UID,
                                    dwParam = uint.Parse(data[1]),//MsgServer.DialogCommands.JiangHuSetName,
                                    wParam1 = client.Player.X,
                                    wParam2 = client.Player.Y
                                };
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var apacket = rec.GetStream();
                                    client.Send(apacket.ActionCreate(&action));
                                }
                                break;
                            }
                        case "gui":
                            {
                                TestGui = ushort.Parse(data[1]);
                                break;
                            }
                        case "tgui":
                            {
                                /*Data datapacket = new Data(true);
                                        datapacket.UID = client.Entity.UID;
                                        datapacket.ID = 162;
                                        datapacket.dwParam = 4020;
                                        datapacket.Facing = (Game.Enums.ConquerAngle)client.Entity.Facing;
                                        datapacket.wParam1 = 73;
                                        datapacket.wParam2 = 98;
                                        client.Send(datapacket);*/
                                var action = new ActionQuery()
                                {
                                    ObjId = client.Player.UID,
                                    Type = (ActionType)443,
                                    dwParam = uint.Parse(data[1])

                                };
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var packet = rec.GetStream();
                                    client.Send(packet.ActionCreate(&action));

                                }
                                break;
                            }

                        #region SystemMessage (sm)            
                        case "sm":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    string sms = msg.__Message.Substring(4);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(client.Player.Name + ": " + sms, "ALLUSERS", MsgColor.white, ChatMode.Center).GetArray(stream));
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(client.Player.Name + ": " + sms, "ALLUSERS", MsgColor.white, ChatMode.System).GetArray(stream));
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(client.Player.Name + ": " + sms, "ALLUSERS", MsgColor.white, ChatMode.BroadcastMessage).GetArray(stream));

                                }
                                break;
                            }
                        #endregion
                        #region invisible
                        case "inv":
                            {
                                client.Player.Invisible = true;
                                break;
                            }
                        case "uinv":
                            {
                                client.Player.Invisible = false;
                                break;
                            }
                        #endregion
                        case "boss":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var map = Database.Server.ServerMaps[ushort.Parse(data[1])];
                                    var stream = rec.GetStream();
                                    Database.Server.AddMapMonster(stream, map, ushort.Parse(data[2]), client.Player.X, client.Player.Y, 0, 0, 1);
                                    //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The SnakeKing have spawned in the BirdIslan(SnakeIsland NPC)! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                }
                                break;
                            }
                        case "checkboss":
                            {
                                var map = Database.Server.ServerMaps[ushort.Parse(data[1])];
                                if (map.ContainMobID(ushort.Parse(data[2])))
                                {
                                    string loc = map.GetMobLoc(ushort.Parse(data[2]));
                                    client.SendSysMesage("The Boss location" + loc + ", Kill him!.", MsgMessage.ChatMode.System);
                                }
                                break;
                            }
                        #region tour
                        case "tour":
                            {
                                Game.MsgTournaments.MsgSchedules.CurrentTournament = Game.MsgTournaments.MsgSchedules.Tournaments[(MsgTournaments.TournamentType)ushort.Parse(data[1])];
                                Game.MsgTournaments.MsgSchedules.CurrentTournament.Open();
                                break;
                            }
                        #endregion
                        #region clearspells

                        case "clearspells":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    foreach (var spell in client.MySpells.ClientSpells.Values)
                                        client.MySpells.Remove(spell.ID, stream);
                                }
                                break;
                            }
                        #endregion
                        #region addnpc
                        case "addnpc":
                            {
                                Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                                np.UID = (uint)Program.GetRandom.Next(10000, 100000);
                                np.NpcType = (Role.Flags.NpcType)byte.Parse(data[1]);
                                np.Mesh = ushort.Parse(data[2]);
                                np.Map = client.Player.Map;//ushort.Parse(data[3]);
                                np.X = client.Player.X;//ushort.Parse(data[4]);
                                np.Y = client.Player.Y;//ushort.Parse(data[5]);
                                client.Map.AddNpc(np);
                                break;
                            }
                        #endregion
                        #region itemeffect
                        case "itemeffect":
                            {
                                MsgGameItem item;
                                if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out item))
                                {
                                    item.Effect = (Role.Flags.ItemEffect)ushort.Parse(data[1]);
                                    item.Mode = Role.Flags.ItemMode.Update;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        item.Send(client, stream);
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region superman
                        case "superman":
                            {
                                client.Player.Vitality += 500;
                                client.Player.Strength += 5000;
                                client.Player.Spirit += 500;
                                client.Player.Agility += 5000;

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                    client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                    client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                    client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);

                                }
                                break;
                            }
                        #endregion
                        #region resetstats
                        case "resetstats":
                            {
                                client.Player.Vitality = 0;
                                client.Player.Strength = 0;
                                client.Player.Spirit = 0;
                                client.Player.Agility = 0;

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                    client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                    client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                    client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);

                                }
                                break;
                            }
                        #endregion
                        #region classicpoints
                        //case "classicpoints":
                        //case "ClassicPoints":
                        //case "CP":
                        //case "cp":
                        //    client.SendSysMesage("Your ClassicPoints is : [" + client.Player.ClassicPoints + "] ", MsgMessage.ChatMode.System);
                        //    break;
                        #endregion
                        case "h1pts":
                            {
                                client.Player.PumpkinPoints = ushort.Parse(data[1]);
                                break;
                            }
                        case "help":
                            {
                                foreach (var user in Database.Server.GamePoll.Values)
                                {
                                    if (user.Player.Name.ToLower() == data[1].ToLower() || data[1].ToLower() == "me")
                                    {
                                        switch (data[2])
                                        {
                                            case "sockrate":
                                                {
                                                    user.Player.MeteorSocket += uint.Parse(data[3]);
                                                    client.SendSysMesage($"{user.Player.Name} total meteor socketed : {client.Player.MeteorSocket}");
                                                    break;
                                                }
                                            case "cp":
                                                {
                                                    //user.Player.ClassicPoints = ushort.Parse(data[2]);
                                                    break;
                                                }
                                            case "viptime":
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();
                                                        if (DateTime.Now > user.Player.ExpireVip)
                                                        {
                                                            user.Player.ExpireVip = DateTime.Now;
                                                            user.Player.ExpireVip = user.Player.ExpireVip.AddDays(ushort.Parse(data[3]));
                                                        }
                                                        else
                                                            user.Player.ExpireVip = user.Player.ExpireVip.AddDays(ushort.Parse(data[3]));

                                                        user.Player.VipLevel = 6;

                                                        user.Player.SendUpdate(stream, user.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                                                        user.Player.UpdateVip(stream);
                                                    }
                                                    break;
                                                }
                                            case "vendortime":
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();
                                                        if (DateTime.Now > user.Player.VendorTime)
                                                        {
                                                            user.Player.VendorTime = DateTime.Now;
                                                            user.Player.VendorTime = user.Player.VendorTime.AddDays(ushort.Parse(data[3]));
                                                        }
                                                        else
                                                            user.Player.VendorTime = user.Player.VendorTime.AddDays(ushort.Parse(data[3]));
                                                    }
                                                    break;
                                                }
                                            case "spell":
                                                {
                                                    ushort ID = 0;
                                                    if (!ushort.TryParse(data[3], out ID))
                                                    {
                                                        client.SendSysMesage("Invlid spell ID !");
                                                        break;
                                                    }
                                                    byte level = 0;
                                                    if (!byte.TryParse(data[4], out level))
                                                    {
                                                        client.SendSysMesage("Invlid spell Level ! ");
                                                        break;
                                                    }

                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                        user.MySpells.Add(rec.GetStream(), ID, level, 0, 0, 0);
                                                    break;
                                                }
                                            case "level":
                                                {
                                                    byte amount = 0;
                                                    if (byte.TryParse(data[3], out amount))
                                                    {
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                        {
                                                            var stream = rec.GetStream();
                                                            user.UpdateLevel(stream, amount, true);
                                                        }
                                                    }
                                                    break;
                                                }

                                            case "money":
                                                {
                                                    user.Player.Money += int.Parse(data[3]);
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();
                                                        user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                                    }
                                                    break;
                                                }

                                            case "reborns":
                                                {
                                                    user.Player.Reborn = byte.Parse(data[3]);

                                                    break;
                                                }

                                            case "item":
                                                {
                                                    uint ID = 0;
                                                    if (!uint.TryParse(data[3], out ID))
                                                    {
                                                        client.SendSysMesage("Invlid item ID !");
                                                        break;
                                                    }
                                                    byte plus = 0;
                                                    if (!byte.TryParse(data[4], out plus))
                                                    {
                                                        client.SendSysMesage("Invlid item plus !");
                                                        break;
                                                    }
                                                    byte bless = 0;
                                                    if (!byte.TryParse(data[5], out bless))
                                                    {
                                                        client.SendSysMesage("Invlid item Enchant !");
                                                        break;
                                                    }
                                                    byte enchant = 0;
                                                    if (!byte.TryParse(data[6], out enchant))
                                                    {
                                                        client.SendSysMesage("Invlid item Enchant !");
                                                        break;
                                                    }
                                                    byte sockone = 0;
                                                    if (!byte.TryParse(data[7], out sockone))
                                                    {
                                                        client.SendSysMesage("Invlid item Socket One !");
                                                        break;
                                                    }
                                                    byte socktwo = 0;
                                                    if (!byte.TryParse(data[8], out socktwo))
                                                    {
                                                        client.SendSysMesage("Invlid item Socket Two !");
                                                        break;
                                                    }
                                                    byte count = 1;
                                                    if (data.Length > 9)
                                                    {
                                                        if (!byte.TryParse(data[9], out count))
                                                        {
                                                            client.SendSysMesage("Invlid item count !");
                                                            break;
                                                        }
                                                    }
                                                    byte Effect = 0;
                                                    if (data.Length > 10)
                                                    {
                                                        if (!byte.TryParse(data[10], out Effect))
                                                        {
                                                            client.SendSysMesage("Invlid Effect Type !");
                                                            break;
                                                        }
                                                    }
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                        user.Inventory.Add(rec.GetStream(), ID, count, plus, bless, enchant, (Role.Flags.Gem)sockone, (Role.Flags.Gem)socktwo, false, (Role.Flags.ItemEffect)Effect, false, "", 3);

                                                    break;
                                                }
                                        }
                                    }
                                }
                            break;
                            }
                        #region give
                        case "give":
                            {
                                foreach (var user in Database.Server.GamePoll.Values)
                                {
                                    if (user.Player.Name.ToLower() == data[1].ToLower() || data[1].ToLower() == "me")
                                    {

                                        switch (data[2])
                                        {
                                            case "cp":
                                                {
                                                    //user.Player.ClassicPoints = ushort.Parse(data[2]);
                                                    break;
                                                }
                                            case "spell":
                                                {
                                                    ushort ID = 0;
                                                    if (!ushort.TryParse(data[3], out ID))
                                                    {
                                                        client.SendSysMesage("Invlid spell ID !");
                                                        break;
                                                    }
                                                    byte level = 0;
                                                    if (!byte.TryParse(data[4], out level))
                                                    {
                                                        client.SendSysMesage("Invlid spell Level ! ");
                                                        break;
                                                    }

                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                        user.MySpells.Add(rec.GetStream(), ID, level, 0, 0, 0);
                                                    break;
                                                }
                                            case "level":
                                                {
                                                    byte amount = 0;
                                                    if (byte.TryParse(data[3], out amount))
                                                    {
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                        {
                                                            var stream = rec.GetStream();
                                                            user.UpdateLevel(stream, amount, true);
                                                        }
                                                    }
                                                    break;
                                                }

                                            case "money":
                                                {
                                                    user.Player.Money += int.Parse(data[3]);
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();
                                                        user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                                    }
                                                    break;
                                                }

                                            case "reborns":
                                                {
                                                    user.Player.Reborn = byte.Parse(data[3]);

                                                    break;
                                                }

                                            case "item":
                                                {
                                                    uint ID = 0;
                                                    if (!uint.TryParse(data[3], out ID))
                                                    {
                                                        client.SendSysMesage("Invlid item ID !");
                                                        break;
                                                    }
                                                    byte plus = 0;
                                                    if (!byte.TryParse(data[4], out plus))
                                                    {
                                                        client.SendSysMesage("Invlid item plus !");
                                                        break;
                                                    }
                                                    byte bless = 0;
                                                    if (!byte.TryParse(data[5], out bless))
                                                    {
                                                        client.SendSysMesage("Invlid item Enchant !");
                                                        break;
                                                    }
                                                    byte enchant = 0;
                                                    if (!byte.TryParse(data[6], out enchant))
                                                    {
                                                        client.SendSysMesage("Invlid item Enchant !");
                                                        break;
                                                    }
                                                    byte sockone = 0;
                                                    if (!byte.TryParse(data[7], out sockone))
                                                    {
                                                        client.SendSysMesage("Invlid item Socket One !");
                                                        break;
                                                    }
                                                    byte socktwo = 0;
                                                    if (!byte.TryParse(data[8], out socktwo))
                                                    {
                                                        client.SendSysMesage("Invlid item Socket Two !");
                                                        break;
                                                    }
                                                    byte count = 1;
                                                    if (data.Length > 9)
                                                    {
                                                        if (!byte.TryParse(data[9], out count))
                                                        {
                                                            client.SendSysMesage("Invlid item count !");
                                                            break;
                                                        }
                                                    }
                                                    byte Effect = 0;
                                                    if (data.Length > 10)
                                                    {
                                                        if (!byte.TryParse(data[10], out Effect))
                                                        {
                                                            client.SendSysMesage("Invlid Effect Type !");
                                                            break;
                                                        }
                                                    }
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                        user.Inventory.Add(rec.GetStream(), ID, count, plus, bless, enchant, (Role.Flags.Gem)sockone, (Role.Flags.Gem)socktwo, false, (Role.Flags.ItemEffect)Effect, false, "", 3);

                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                }
                                //if (DatabaseConfig.discord_stat)
                                    //Program.DiscordAdminAPI.Enqueue(logss);
                                break;
                            }
                        #endregion

                        #region kick
                        case "kick":
                            {
                                foreach (var user in Database.Server.GamePoll.Values)
                                {
                                    if (user.Player.Name.ToLower() == data[1].ToLower())
                                    {
                                        user.Socket.Disconnect();
                                        break;
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region rev
                        case "rev":
                        case "revive":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                    client.Player.Revive(rec.GetStream());

                                break;
                            }
                        #endregion
                        #region online                      
                        case "online":
                            {
                                client.SendSysMesage("Online Players : " + Database.Server.GamePoll.Count + " ");
                                client.SendSysMesage("Max Online Players : " + KernelThread.MaxOnline + " ");
                                break;
                            }
                        #endregion

                        #region info
                        case "info":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    foreach (var user in Database.Server.GamePoll.Values)
                                    {
                                        if (user.Player.Name.ToLower() == data[1].ToLower())
                                        {

                                            client.Send(new MsgMessage("[Info" + user.Player.Name + "]", MsgColor.yellow, ChatMode.FirstRightCorner).GetArray(stream));
                                            client.Send(new MsgMessage("UID = " + user.Player.UID + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            client.Send(new MsgMessage("IP = " + user.Socket.RemoteIp + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            client.Send(new MsgMessage("ConquerPoints = " + user.Player.ConquerPoints + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            client.Send(new MsgMessage("Money = " + user.Player.Money + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            //client.Send(new MsgMessage("DonationPoints = " + user.Player.DonationPoints + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            //client.Send(new MsgMessage("VotePoints = " + user.Player.VotePoints + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            //client.Send(new MsgMessage("VotePoints = " + user.Player.ClassicPoints + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            var list = MsgLoginClient.PlayersIP.Where(e => e.Key == user.IP).FirstOrDefault();
                                            client.Send(new MsgMessage("----- \n", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            foreach (var pl in list.Value)
                                                client.Send(new MsgMessage(pl, MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region scroll
                        case "scroll":
                            {
                                //if (Program.BlockTeleportMap.Contains(client.Player.Map)
                                //    || client.Player.Map == 1038
                                //    || client.InFIveOut
                                //     || client.InTDM
                                //    || client.InLastManStanding
                                //    || client.InPassTheBomb
                                //    || client.InST
                                //    || MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250
                                //    || client.Player.Map == 1049
                                //    )
                                //{
                                //    if(client.Player.Map == 8250) { client.Map.Name = "NewIsland"; }
                                //    client.SendSysMesage("You can`t use teleport in " + client.Map.Name + " ", MsgMessage.ChatMode.System);
                                //    break;
                                //}
                                //else
                                //{
                                    switch (data[1].ToLower())
                                    {
                                        case "lv": client.Teleport(5, 290, 1354, 8800); break;
                                        case "tc": client.Teleport(428, 378, 1002); break;
                                        case "pc": client.Teleport(195, 260, 1011); break;
                                        case "ac":
                                        case "am": client.Teleport(566, 563, 1020); break;
                                        case "dc": client.Teleport(500, 645, 1000); break;
                                        case "bi": client.Teleport(723, 573, 1015); break;
                                        case "pka": client.Teleport(050, 050, 1005); break;
                                        case "mk":
                                        case "ma": client.Teleport(211, 196, 1036); break;
                                        case "ja": client.Teleport(100, 100, 6000); break;
                                    }
                                //}
                                    break;
                            }
                        #endregion
                        #region trace
                        case "find":
                        case "trace":
                            {
                                foreach (var user in Database.Server.GamePoll.Values)
                                {
                                    if (user.Player.Name.ToLower().Contains(data[1].ToLower()))
                                    {
                                        client.Teleport(user.Player.X, user.Player.Y, user.Player.Map, user.Player.DynamicID);
                                        break;
                                    }
                                }

                                break;
                            }
                        #endregion
                        #region bring
                        case "bring":
                            {
                                foreach (var user in Database.Server.GamePoll.Values)
                                {
                                    if (user.Player.Name.ToLower() == data[1].ToLower())
                                    {
                                        user.Teleport(client.Player.X, client.Player.Y, client.Player.Map);
                                        break;
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region life
                        case "life":
                            {
                                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                                client.Player.Mana = (ushort)client.Status.MaxMana;
                                client.Player.SendUpdateHP();
                                break;
                            }
                        #endregion
                        #region donationpoints
                        case "dp":
                            {
                                client.Player.ChampionPoints = (int)uint.Parse(data[1]);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                                    client.SendSysMesage($"Donation Points Added {client.Player.ChampionPoints}");
                                }
                                break;
                            }
                        #endregion
                        #region ClassicPoints
                        case "classic":
                            {
                                //client.Player.ClassicPoints = uint.Parse(data[1]);

                                break;
                            }
                        #endregion

                        #region staticrole
                        case "staticrole":
                            {
                                var staticrole = new Role.StaticRole(client.Player.X, client.Player.Y);
                                staticrole.Map = client.Player.Map;

                                client.Map.AddStaticRole(staticrole);
                                break;
                            }
                        #endregion
                        #region facke
                        case "facke":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    string[] names = new string[] { "Gool", "Hooy", "Thor", "Mido", "3tra", "omyy", "anaa", "a7aa", "Fuck", "MArk", "Lucy", "Tomy", "Dark", "Noop", "Hway", "Bosy", "Nosa", "Whits", "Rober", "Kota", "Contesa", "Kory", "Moko", "Hayato", "Adams"
                                                            , "Jask", "hasky", "mask", "masr", "balas", "moza", "a7mos", "bardis", "therock", "herok", "bsbopsa", "bate5a", "manga", "moza", "logy", "rosy", "hohoz", "troz", "bika", "kora", "kola", "afion", "7chicha", "bango", "shata"};
                                    byte[] levels = new byte[] { 15, 37, 50, 55, 101, 111, 112, 19, 78, 99, 107, 103, 48, 46, 88, 95, 53, 83, 119, 121, 110, 80, 60, 69, 62, 105,
                                                                33, 44, 104, 37, 50, 55, 101, 111, 112, 19, 78, 99, 107, 103, 48, 46, 88, 95, 53, 83, 119, 121, 110, 80, 60, 69, 62, 105, 33, 44, 104 };
                                    uint[] garment = new uint[] { 192695, 188335, 192615, 194210, 193075, 193625, 188705, 187975, 192345, 188945, 193355, 194320, 192575, 187575, 193115, 192785, 193195, 192635, 187315, 188175, 188165, 188265, 187965, 192435, 193015, 187775 };
                                    for (int i = 0; i < ushort.Parse(data[1]); i++)
                                    {
                                        Client.GameClient pclient = new Client.GameClient(null);
                                        pclient.Fake = true;

                                        pclient.Player = new Role.Player(pclient);
                                        pclient.Inventory = new Role.Instance.Inventory(pclient);
                                        pclient.Equipment = new Role.Instance.Equip(pclient);
                                        pclient.Warehouse = new Role.Instance.Warehouse(pclient);
                                        pclient.MyProfs = new Role.Instance.Proficiency(pclient);
                                        pclient.MySpells = new Role.Instance.Spell(pclient);
                                        //pclient.Achievement = new Database.AchievementCollection();
                                        pclient.Status = new MsgStatus();

                                        pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;

                                        pclient.Player.Name = "" + names[i] + "";
                                        pclient.Player.Body = client.Player.Body;
                                        pclient.Player.Face = client.Player.Face;
                                        pclient.Player.Hair = client.Player.Hair;
                                        pclient.Player.UID = Database.Server.ClientCounter.Next;
                                        pclient.Player.HitPoints = client.Player.HitPoints;
                                        pclient.Status.MaxHitpoints = client.Status.MaxHitpoints;
                                        //pclient.Status.MaxAttack = client.Status.MaxAttack;

                                        ushort x = client.Player.X;
                                        ushort y = client.Player.Y;
                                        pclient.Player.X = (ushort)Program.GetRandom.Next((int)x - 15, x + 15);
                                        pclient.Player.Y = (ushort)Program.GetRandom.Next((int)y - 15, y + 15);
                                        pclient.Player.Map = client.Player.Map;
                                        pclient.Player.Level = 140; //levels[i];
                                        pclient.Player.Class = 45;
                                        pclient.Player.Action = Role.Flags.ConquerAction.Jump;
                                        pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                                        pclient.Player.GarmentId = garment[i];
                                        pclient.Player.RightWeaponId = 420339;
                                        pclient.Player.LeftWeaponId = 420339;
                                        //pclient.MySpells.ClientSpells.ContainsKey((ushort)8001);
                                        pclient.Player.Vitality = 300;
                                        pclient.Player.Strength = 100;
                                        pclient.Player.Agility = 256;
                                        pclient.Status.MaxAttack = uint.MaxValue;
                                        pclient.Status.MinAttack = uint.MaxValue;

                                        #region Revive
                                        if (pclient.Player.ContainFlag(MsgUpdate.Flags.Ghost) && Time32.Now > client.Player.DeadStamp.AddSeconds(20))
                                        {
                                            pclient.Player.Action = Flags.ConquerAction.None;
                                            pclient.Player.TransformationID = 0;
                                            pclient.Player.RemoveFlag(MsgUpdate.Flags.Dead);
                                            pclient.Player.RemoveFlag(MsgUpdate.Flags.Ghost);
                                            pclient.Player.HitPoints = (int)client.Status.MaxHitpoints;
                                        }
                                        #endregion
                                        client.Send(pclient.Player.GetArray(stream, false));

                                        pclient.Map = client.Map;
                                        pclient.Map.Enquer(pclient);
                                        Database.Server.GamePoll.TryAdd(pclient.Player.UID, pclient);
                                        ////pclient.Player.Robot = true;
                                        //Catching.Start(pclient);
                                        pclient.Player.View.SendView(pclient.Player.GetArray(stream, false), false);

                                    }
                                }
                                break;
                            }
                        #endregion
                        #region op
                        case "op":
                            {
                                client.Player.OnlinePoints = int.Parse(data[1]);
                                break;
                            }
                        #endregion
                        #region max_attack
                        case "max_attack":
                            {
                                client.Status.Defence = uint.MaxValue;
                                client.Status.MaxAttack = uint.Parse(data[1]);
                                client.Status.MinAttack = uint.Parse(data[1]) - 1;
                                break;
                            }
                        #endregion
                        #region pkp
                        case "pkp":
                            {
                                client.Player.PKPoints = ushort.Parse(data[1]);
                                break;
                            }
                        #endregion
                        #region map
                        case "map":
                            {
                                client.SendSysMesage("MapID = " + client.Player.Map, ChatMode.System);
                                break;
                            }
                        #endregion
                        #region studypoints
                        case "studypoints":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                    client.Player.SubClass.AddStudyPoints(client, ushort.Parse(data[1]), rec.GetStream());
                                break;
                            }
                        #endregion
                        #region xp
                        case "xp":
                            {
                                client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                                break;
                            }
                        case "cycloneall":
                            {
                                foreach (GameClient clienst in Server.GamePoll.Values)
                                {
                                    clienst.Player.AddFlag(MsgUpdate.Flags.Cyclone, 20, true);
                                }
                                break;
                            }
                        #endregion
                        #region leve
                        case "level":
                            {
                                byte amount = 0;
                                if (byte.TryParse(data[1], out amount))
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.UpdateLevel(stream, amount, true);
                                    }
                                }

                                break;
                            }
                        #endregion
                        #region money
                        case "money":
                            {
                                int amount = 0;
                                if (int.TryParse(data[1], out amount))
                                {
                                    client.Player.Money = amount;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region cps
                        case "cps":
                            {
                                int amount = 0;
                                if (int.TryParse(data[1], out amount))
                                {
                                    client.Player.ConquerPoints = amount;

                                }
                                break;
                            }
                        #endregion
                        #region RoyalPassPoints
                        case "RoyalPassPoints":
                            {
                                int amount = 0;
                                if (int.TryParse(data[1], out amount))
                                {
                                    client.Player.RoyalPassPoints = amount;

                                }
                                break;
                            }
                        #endregion
                        #region remspell
                        case "remspell":
                            {
                                ushort ID = 0;
                                if (!ushort.TryParse(data[1], out ID))
                                {
                                    client.SendSysMesage("Invlid spell ID !");
                                    break;
                                }
                                using (var rec = new ServerSockets.RecycledPacket())
                                    client.MySpells.Remove(ID, rec.GetStream());
                                break;
                            }
                        #endregion
                        #region spell
                        case "spell":
                            {
                                ushort ID = 0;
                                if (!ushort.TryParse(data[1], out ID))
                                {
                                    client.SendSysMesage("Invlid spell ID !");
                                    break;
                                }
                                byte level = 0;
                                if (data.Length >= 2)
                                {
                                    try
                                    {
                                        if (!byte.TryParse(data[2], out level))
                                        {
                                            client.SendSysMesage("Invlid spell Level ! ");
                                            break;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                        Console.WriteLine("Error in spell command from arrays.");
                                        return false;
                                    }
                                }
                                byte levelHu = 0;
                                if (data.Length >= 3)
                                {
                                    if (!byte.TryParse(data[3], out levelHu))
                                    {
                                        client.SendSysMesage("Invlid spell Level Souls ! ");
                                        break;
                                    }
                                }
                                int Experience = 0;
                                if (!int.TryParse(data[4], out Experience))
                                {
                                    client.SendSysMesage("Invlid spell Experience ! ");
                                    break;
                                }

                                using (var rec = new ServerSockets.RecycledPacket())
                                    client.MySpells.Add(rec.GetStream(), ID, level, levelHu, 0, Experience);
                                break;
                            }
                        #endregion
                        #region prof
                        case "prof":
                            {
                                ushort ID = 0;
                                if (!ushort.TryParse(data[1], out ID))
                                {
                                    client.SendSysMesage("Invlid prof ID !");
                                    break;
                                }
                                byte level = 0;
                                if (!byte.TryParse(data[2], out level))
                                {
                                    client.SendSysMesage("Invlid prof Level ! ");
                                    break;
                                }
                                uint Experience = 0;
                                if (!uint.TryParse(data[3], out Experience))
                                {
                                    client.SendSysMesage("Invlid prof Experience ! ");
                                    break;
                                }
                                using (var rec = new ServerSockets.RecycledPacket())
                                    client.MyProfs.Add(rec.GetStream(), ID, level, Experience);
                                break;
                            }
                        #endregion
                        #region clear
                        case "clear":
                        case "clearinventory":
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                    client.Inventory.Clear(rec.GetStream());
                                break;
                            }
                        #endregion
                        #region tele
                        case "tele":
                            {

                                client.TerainMask = 0;
                                uint mapid = 0;
                                if (!uint.TryParse(data[1], out mapid))
                                {
                                    client.SendSysMesage("Invlid Map ID !");
                                    break;
                                }
                                ushort X = 0;
                                if (!ushort.TryParse(data[2], out X))
                                {
                                    client.SendSysMesage("Invlid X !");
                                    break;
                                }
                                ushort Y = 0;
                                if (!ushort.TryParse(data[3], out Y))
                                {
                                    client.SendSysMesage("Invlid Y !");
                                    break;
                                }
                                //if (mapid == 1601)
                                //{
                                //    client.SendSysMesage("You can`t go there, its nova`s office.");
                                //    break;
                                //}

                                client.Teleport(X, Y, mapid);
                                break;
                            }
                        #endregion
                        #region itemm
                        case "itemm":
                            {
                                uint ID = 0;
                                if (!uint.TryParse(data[1], out ID))
                                {
                                    client.SendSysMesage("Invlid item ID !");
                                    break;
                                }
                                using (var rec = new ServerSockets.RecycledPacket())
                                    client.Inventory.AddItemWitchStack(ID, 0, 1, rec.GetStream(), false);

                                break;
                            }
                        #endregion
                        #region item
                        case "item":
                            {
                                uint ID = 0;
                                if (!uint.TryParse(data[1], out ID))
                                {
                                    client.SendSysMesage("Invlid item ID !");
                                    break;
                                }
                                byte plus = 0;
                                if (!byte.TryParse(data[2], out plus))
                                {
                                    client.SendSysMesage("Invlid item plus !");
                                    break;
                                }
                                byte bless = 0;
                                if (!byte.TryParse(data[3], out bless))
                                {
                                    client.SendSysMesage("Invlid item Enchant !");
                                    break;
                                }
                                byte enchant = 0;
                                if (!byte.TryParse(data[4], out enchant))
                                {
                                    client.SendSysMesage("Invlid item Enchant !");
                                    break;
                                }
                                byte sockone = 0;
                                if (!byte.TryParse(data[5], out sockone))
                                {
                                    client.SendSysMesage("Invlid item Socket One !");
                                    break;
                                }
                                byte socktwo = 0;
                                if (!byte.TryParse(data[6], out socktwo))
                                {
                                    client.SendSysMesage("Invlid item Socket Two !");
                                    break;
                                }
                                byte count = 1;
                                if (data.Length > 7)
                                {
                                    if (!byte.TryParse(data[7], out count))
                                    {
                                        client.SendSysMesage("Invlid item count !");
                                        break;
                                    }
                                }
                                byte Effect = 0;
                                if (data.Length > 8)
                                {
                                    if (!byte.TryParse(data[8], out Effect))
                                    {
                                        client.SendSysMesage("Invlid Effect Type !");
                                        break;
                                    }
                                }
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    client.Inventory.Add(rec.GetStream(), ID, count, plus, bless, enchant, (Role.Flags.Gem)sockone, (Role.Flags.Gem)socktwo, false, (Role.Flags.ItemEffect)Effect);
                                }
                                break;
                            }
                        #endregion
                        #region Top
                        case "top":
                            {
                                client.Player.AddFlag((MsgServer.MsgUpdate.Flags)int.Parse(data[1]), Role.StatusFlagsBigVector32.PermanentFlag, false);
                                break;
                            }
                        #endregion
                        #region reborn
                        case "reborn":
                            {
                                client.Player.Reborn = byte.Parse(data[1]);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.Reborn, MsgUpdate.DataType.Reborn);
                                }
                                break;
                            }
                        #endregion
                        #region class
                        case "class":
                            {
                                client.Player.Class = byte.Parse(data[1]);
                                break;
                            }
                        #endregion
                        #region reallot
                        //case "reallot":
                        //    {
                        //        using (var rec = new ServerSockets.RecycledPacket())
                        //        {
                        //            var stream = rec.GetStream();
                        //            if (!client.Inventory.Contain(Database.ItemType.DragonBall, 1, 0))
                        //            {
                        //                client.CreateBoxDialog("You don`t have (1) Dragonball to reallot! ");
                        //                break;
                        //            }
                        //            else
                        //            {
                        //                client.Player.Agility = 0;
                        //                client.Player.Strength = 0;
                        //                client.Player.Vitality = 1;
                        //                client.Player.Spirit = 0;
                        //                client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
                        //                client.CreateBoxDialog("You have successfully reloaded your attribute points.");
                        //                if (client.Player.Reborn == 0)
                        //                {
                        //                    client.Player.Atributes = 0;
                        //                    Database.DataCore.AtributeStatus.ResetStatsNonReborn(client.Player);
                        //                    if (Database.AtributesStatus.IsWater(client.Player.Class))
                        //                    {
                        //                        if (client.Player.Level > 110)
                        //                            client.Player.Atributes = (ushort)((client.Player.Level - 110) * 3 + client.Player.ExtraAtributes);
                        //                    }
                        //                    else
                        //                    {
                        //                        if (client.Player.Level > 120)
                        //                            client.Player.Atributes = (ushort)((client.Player.Level - 120) * 3 + client.Player.ExtraAtributes);
                        //                    }
                        //                }
                        //                else if (client.Player.Reborn == 1)
                        //                {
                        //                    client.Player.Atributes = (ushort)(Database.Server.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass)
                        //                        + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                        //                }
                        //                else
                        //                {
                        //                    if (client.Player.SecoundeRebornLevel == 0)
                        //                        client.Player.SecoundeRebornLevel = 140;
                        //                    client.Player.Atributes = (ushort)(Database.Server.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass) +
                        //                        Database.Server.RebornInfo.ExtraAtributePoints(client.Player.SecoundeRebornLevel, client.Player.SecondClass) + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                        //                }
                        //                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        //                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        //                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        //                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        //                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        //            }
                        //        }
                        //        break;
                        //    }
                        #endregion
                        case "reset2":
                            {
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                foreach (string fname in System.IO.Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\"))
                                {
                                    ini.FileName = fname;
                                    ini.Write<uint>("Character", "KeyBoxTRY", 3);
                                    ini.Write<uint>("Character", "lettersTRY", 3);
                                    ini.Write<uint>("Character", "LavaTRY", 3);
                                    Console.WriteLine("reset");
                                    

                                }
                                break;
                            }
                        case "DonationNobility":
                            {
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                foreach (string fname in System.IO.Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\"))
                                {
                                    ini.FileName = fname;
                                    ini.Write<uint>("Character", "DonationNobility", 0);
                                   
                                    Console.WriteLine("DonationNobility reset");
                                    

                                }
                                break;
                            }
                        #region reset op/tp/stg/quests
                        case "reset1989":
                            {
                                client.Player.LotteryEntries = 0;
                                client.Player.ConquerLetter = 0;
                                client.Player.LavaQuest = 0;
                                client.Player.BDExp = 0;
                                client.Player.KeyBoxTRY = 3;
                                client.Player.lettersTRY = 3;
                                client.Player.LavaTRY = 3;
                                client.Player.ExpBallUsed = 0;
                                if (client.Player.QuestGUI.CheckQuest(20195, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20195);
                                if (client.Player.QuestGUI.CheckQuest(20199, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20199);
                                if (client.Player.QuestGUI.CheckQuest(20198, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20198);
                                if (client.Player.QuestGUI.CheckQuest(20197, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20197);
                                if (client.Player.QuestGUI.CheckQuest(20193, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20193);
                                if (client.Player.QuestGUI.CheckQuest(20191, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20191);
                                if (client.Player.QuestGUI.CheckQuest(20192, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20192);
                                if (client.Player.QuestGUI.CheckQuest(20196, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20196);
                                if (client.Player.QuestGUI.CheckQuest(20194, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20194);
                                if (client.Player.QuestGUI.CheckQuest(20200, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    client.Player.QuestGUI.RemoveQuest(20200);                                
                                client.OnlinePointsManager.Reset();
                                client.TournamentsManager.Reset();
                                client.LimitedDailyTimes.Reset();
                                break;
                            }
                            #endregion
                    }
                }
                return true;
                #endregion
            }
            return false;

        }
    }
}
