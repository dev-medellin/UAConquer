using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using static MongoDB.Driver.WriteConcern;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgNewRole
    {
        public static class CharacterCreation
        {
            public static string AccessDenied = "You have not been granted access to Character Creation by the server.";

            public static string AccountHasCharacter = "This account already have an character registered.";

            public static string NameTaken = "The name you entered has already been taken.";

            public static string RegisterTryAgain = "Error, please try later";
        }

        public static object SynName = new object();

        private const uint _START_MAP = 1002;

        private static readonly ushort[] m_startX = new ushort[7] { 430, 423, 439, 428, 452, 464, 439 };

        private static readonly ushort[] m_startY = new ushort[7] { 378, 394, 384, 365, 365, 378, 396 };

        private static readonly byte[] Hairstyles = new byte[12]
        {
            10, 11, 13, 14, 15, 24, 30, 35, 37, 38, 39, 40
        };

        public static void GetNewRoleInfo(this Packet msg, out string name, out ushort Body, out byte Class, out string Mac)
        {
            msg.ReadBytes(20);
            name = msg.ReadCString(16);
            msg.ReadBytes(32);
            Body = msg.ReadUInt16();
            Class = msg.ReadUInt8();
            msg.ReadBytes(5);
            Mac = msg.ReadCString(12);
        }

        [Packet(1001)]
        public static void CreateCharacter(GameClient client, Packet stream)
        {
            if ((client.ClientFlag & ServerFlag.CreateCharacter) != ServerFlag.CreateCharacter)
                return;
            if (!ServerDatabase.AllowCreate(client.ConnectionUID))
            {
                ServerKernel.Log.AppendGameLog($"{client.ConnectionUID} try to create two chracter in same account");
                client.Send(new MsgMessage(CharacterCreation.AccountHasCharacter, MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
                return;
            }
            client.ClientFlag &= ~ServerFlag.AcceptLogin;
            stream.GetNewRoleInfo(out var CharacterName, out var Body, out var Class, out var mac);
            Flags.ProfessionType profession;
            profession = ((Class > 100) ? Flags.ProfessionType.INTERN_TAOIST : ((Flags.ProfessionType)((int)Class / 10 * 10)));
            if (!Enum.IsDefined(typeof(Flags.BodyType), Body) || !Enum.IsDefined(typeof(Flags.ProfessionType), profession))
            {
                ServerKernel.Log.AppendGameLog($"{client.ConnectionUID} try to create a unknown Body or Class");
                client.Send(new MsgMessage(CharacterCreation.AccessDenied, MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
                return;
            }
            CharacterName = CharacterName.Replace("\0", "");
            int idx;
            idx = new Random().Next(m_startX.Length - 1);
            ushort startX;
            startX = m_startX[idx];
            ushort startY;
            startY = m_startY[idx];
            if (Program.NameStrCheck(CharacterName))
            {
                if (!Server.NameUsed.Contains(CharacterName.GetHashCode()))
                {
                    client.ClientFlag &= ~ServerFlag.CreateCharacter;
                    lock (Server.NameUsed)
                    {
                        Server.NameUsed.Add(CharacterName.GetHashCode());
                    }
                    client.Player.Name = CharacterName;
                    client.Player.Class = Class;
                    client.Player.Body = Body;
                    client.Player.Level = 1;
                    client.Player.Map = 1002;
                    client.Player.AddVIPLevel(6, DateTime.Now.AddDays(7.0), stream);
                    client.Player.AddHeavenBlessing(stream, 604800);
                    client.Player.X = startX;
                    client.Player.Y = startY;
                    DataCore.LoadClient(client.Player);
                    client.Player.UID = client.ConnectionUID;
                    DataCore.AtributeStatus.GetStatus(client.Player);
                    ushort lookface;
                    lookface = ((Body == 1003 || Body == 1004) ? (((int)Class / 10 == 5) ? ((ushort)ServerKernel.NextAsync(103, 107)) : (((int)Class / 10 != 6) ? ((ushort)ServerKernel.NextAsync(1, 102)) : ((ushort)ServerKernel.NextAsync(109, 113)))) : (((int)Class / 10 == 5) ? ((ushort)ServerKernel.NextAsync(291, 295)) : (((int)Class / 10 != 6) ? ((ushort)ServerKernel.NextAsync(201, 290)) : ((ushort)ServerKernel.NextAsync(300, 304)))));
                    client.OnLogin.MacAddress = mac.ToString();
                    int value;
                    value = 10;
                    client.Player.Face = lookface;
                    client.Player.Hair = (ushort)(ServerKernel.NextAsync(3, 9) * 100 + Hairstyles[ServerKernel.NextAsync(0, Hairstyles.Length)]);
                    client.Player.Money += value * 1000;
                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                    try
                    {
                        client.Inventory.Add(stream, 723753, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                        //GenerateInitialStatus(client, profession, stream);
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog("Could not create initial status for character " + client.Player.UID, true, LogType.WARNING);
                    }
                    if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                    {
                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);

                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure))
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);

                        client.Status.MaxMana = client.CalculateMana();
                        client.Player.Mana = (ushort)client.Status.MaxMana;
                    }
                    client.Status.MaxHitpoints = client.CalculateHitPoint();
                    client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                    MsgSchedules.SendSysMesage("Lets Welcome The New Player: [ " + client.Player.Name + " ] , Has Joined Our Empire.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                    //client.Player.NewbieProtection = Flags.NewbieExperience.NotActive;
                    client.ClientFlag |= ServerFlag.CreateCharacterSucces;
                    Database.ServerDatabase.CreateCharacte(client);
                    Database.ServerDatabase.SaveClient(client);
                    client.CharReadyCreate(stream);
                }
                else
                    client.Send(new MsgMessage(CharacterCreation.NameTaken, MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
            }
            else
                client.Send(new MsgMessage(CharacterCreation.AccountHasCharacter, MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
        }

        private static void GenerateInitialStatus(GameClient client, Flags.ProfessionType profession, Packet stream)
        {
            for (int i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                    case 2:
                        {
                            uint ItemID;
                            ItemID = 1000000;
                            client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                            break;
                        }
                    case 3:
                    case 4:
                    case 5:
                        {
                            uint ItemID;
                            ItemID = 1001000;
                            client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                            break;
                        }
                    case 6:
                        {
                            uint ItemID;
                            ItemID = 150003;
                            client.Equipment.Add(stream, ItemID, Flags.ConquerItem.Ring, 0, 0, 0);
                            break;
                        }
                    case 7:
                        {
                            if(client.Player.Class == 40)
                            {
                                client.Inventory.Add(stream, 1050000, 5, 0, 0, 0);
                            }
                            //client.Inventory.Add(stream, 723017, 3, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                            uint ItemID;
                            ItemID = 723753;
                            client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                            //client.Inventory.AddItemWitchStack(722136, 0, 5, stream, true);//10xExpBall(Event)
                            break;
                        }
                    case 8:
                        {
                            uint ItemID;
                            switch (profession)
                            {
                                //case Flags.ProfessionType.INTERN_MONK:
                                //    ItemID = 610301;
                                //    break;
                                //case Flags.ProfessionType.INTERN_NINJA:
                                //    ItemID = 601301;
                                //    break;
                                case Flags.ProfessionType.INTERN_TAOIST:
                                    ItemID = 421301;
                                    break;
                                case Flags.ProfessionType.INTERN_ARCHER:
                                    ItemID = 500301;
                                    break;
                                default:
                                    ItemID = 410301;
                                    break;
                            }
                            client.Equipment.Add(stream, ItemID, Flags.ConquerItem.RightWeapon, 0, 0, 0);
                            break;
                        }
                    case 9:
                        {
                            uint ItemID;
                            ItemID = 132005;
                            client.Equipment.Add(stream, ItemID, Flags.ConquerItem.Armor, 0, 0, 0);
                            break;
                        }
                }
            }
            switch (profession)
            {
                //case Flags.ProfessionType.INTERN_MONK:
                //    if (!client.MySpells.ClientSpells.ContainsKey(10415))
                //        client.MySpells.Add(stream, 10415, 0, 0, 0);
                //    if (!client.MySpells.ClientSpells.ContainsKey(10490))
                //        client.MySpells.Add(stream, 10490, 0, 0, 0);
                //    if (!client.MySpells.ClientSpells.ContainsKey(10390))
                //        client.MySpells.Add(stream, 10390, 0, 0, 0);
                //    break;
                //case Flags.ProfessionType.INTERN_NINJA:
                //    if (!client.MySpells.ClientSpells.ContainsKey(6011))
                //        client.MySpells.Add(stream, 6011, 0, 0, 0);
                //    if (!client.MySpells.ClientSpells.ContainsKey(6001))
                //        client.MySpells.Add(stream, 6001, 0, 0, 0);
                //    if (!client.MySpells.ClientSpells.ContainsKey(6000))
                //        client.MySpells.Add(stream, 6000, 0, 0, 0);
                //    break;
                //case Flags.ProfessionType.INTERN_ARCHER:
                //    if (!client.MySpells.ClientSpells.ContainsKey(1045))
                //        client.MySpells.Add(stream, 1045, 0, 0, 0);
                //    if (!client.MySpells.ClientSpells.ContainsKey(1046))
                //        client.MySpells.Add(stream, 1046, 0, 0, 0);
                    //client.MySpells.Add(stream, 8002, 0, 0, 0);
                    //client.MySpells.Add(stream, 8001, 0, 0, 0);
                //    break;
                //case Flags.ProfessionType.INTERN_TROJAN:
                //    if (!client.MySpells.ClientSpells.ContainsKey(1045))
                //        client.MySpells.Add(stream, 1045, 0, 0, 0);
                //    if (!client.MySpells.ClientSpells.ContainsKey(1046))
                //        client.MySpells.Add(stream, 1046, 0, 0, 0);
                //    if (!client.MySpells.ClientSpells.ContainsKey(1110))
                //        client.MySpells.Add(stream, 1110, 0, 0, 0);
                //    break;
                //case Flags.ProfessionType.INTERN_WARRIOR:
                //    if (!client.MySpells.ClientSpells.ContainsKey(1045))
                //        client.MySpells.Add(stream, 1045, 0, 0, 0);
                //    if (!client.MySpells.ClientSpells.ContainsKey(1046))
                //        client.MySpells.Add(stream, 1046, 0, 0, 0);
                    //if (!client.MySpells.ClientSpells.ContainsKey(1025))
                    //    client.MySpells.Add(stream, 1025, 0, 0, 0);
                    //if (!client.MySpells.ClientSpells.ContainsKey(1045))
                    //    client.MySpells.Add(stream, 1045, 0, 0, 0);
                    //if (!client.MySpells.ClientSpells.ContainsKey(1046))
                    //    client.MySpells.Add(stream, 1046, 0, 0, 0);
                    //if (!client.MySpells.ClientSpells.ContainsKey(1020))
                    //    client.MySpells.Add(stream, 1020, 0, 0, 0);
                    //if (!client.MySpells.ClientSpells.ContainsKey(1015))
                    //    client.MySpells.Add(stream, 1015, 0, 0, 0);
                    //if (!client.MySpells.ClientSpells.ContainsKey(1040))
                    //    client.MySpells.Add(stream, 1040, 0, 0, 0);
                    //break;
                case Flags.ProfessionType.INTERN_TAOIST:
                    //if (!client.MySpells.ClientSpells.ContainsKey(10309))
                    //    client.MySpells.Add(stream, 10309, 0, 0, 0);
                    if (!client.MySpells.ClientSpells.ContainsKey(1010))
                        client.MySpells.Add(stream, 1010, 0, 0, 0);
                    if (!client.MySpells.ClientSpells.ContainsKey(1000))
                        client.MySpells.Add(stream, 1000, 0, 0, 0);
                    if (!client.MySpells.ClientSpells.ContainsKey(1005))
                        client.MySpells.Add(stream, 1005, 0, 0, 0);
                    break;
            }
        }
    }
}
