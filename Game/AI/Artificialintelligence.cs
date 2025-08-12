using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;
using System;
using System.Collections.Generic;
using System.IO;

 
namespace TheChosenProject.Game.Ai
{
    public class Artificialintelligence
    {
        public class ArtificialintelligenceInfo
        {
            public uint UID;
            //public TheChosenProject.Game.AI.AIEnum.AIType AI_type;
            public AIEnum.AIType AI_type;

            public byte AI_plus;
            public string AI_name;
            public byte AI_class;
            public ushort AI_body;
            public byte AI_rank;
            public byte AI_damg;
            public byte AI_HP;
            public byte AI_socone;
            public byte AI_soctwo;
            public List<ushort> AI_skills;
            public List<uint> AI_equipments;
            public unsafe void AI_Initialize(ServerSockets.Packet stream, GameClient target, uint count = 1)
            {
                for (int x = 0; x < count; x++)
                {
                    if (!Database.Server.GamePoll.ContainsKey(UIDCounter.Next))
                    {
                        GameClient AI_user = new GameClient(null)
                        {
                            Fake = true
                        };
                        AI_user.Player = new Player(AI_user);
                        AI_user.Inventory = new Role.Instance.Inventory(AI_user);
                        AI_user.Equipment = new Role.Instance.Equip(AI_user);
                        AI_user.Warehouse = new Role.Instance.Warehouse(AI_user);
                        AI_user.MyProfs = new Role.Instance.Proficiency(AI_user);
                        AI_user.MySpells = new Role.Instance.Spell(AI_user);
                        AI_user.Player.SubClass = new Role.Instance.SubClass();
                        // AI_user.Player.Nobility = new Role.Instance.Nobility(AI_user);
                        AI_user.Status = new MsgStatus();
                        ushort face = 296;
                        if (target != null)
                        {
                            if (AI_type == AIEnum.AIType.Leveling)
                                //AI_user.Player.Name = "" + AI.AIEnum.GetName() + "(" + target.Player.Name + ")";
                                AI_user.Player.Name = AIEnum.GetName() + "(" + target.Player.Name + ")";

                            else
                                AI_user.Player.Name = "" + AIEnum.GetName() + "";
                            AI_user.Player.X = (ushort)(target.Player.X + 2);
                            AI_user.Player.Y = (ushort)(target.Player.Y + 2);
                            AI_user.Player.Map = target.Player.Map;
                            AI_user.Player.DynamicID = target.Player.DynamicID;
                            AI_user.Target = target.Player;
                            face = target.Player.Face;
                            AI_user.Player.Body = target.Player.Body;
                        }
                        AI_user.Player.Spouse = "None";
                        do
                        {
                            AI_user.Player.UID = UIDCounter.Next;
                        }
                        while (Database.Server.GamePoll.ContainsKey(AI_user.Player.UID));
                        AI_user.Player.Associate = new Role.Instance.Associate.MyAsociats(AI_user.Player.UID);
                        AI_user.Player.Level = 140;

                        byte Color = (byte)Program.GetRandom.Next(4, 8);
                        AI_user.Player.Hair = (ushort)(Color * 100 + 10 + (byte)Program.GetRandom.Next(4, 9));
                        //AI_user.Player.QuizPoints = ushort.MaxValue;
                        AI_user.Player.Reborn = 2;
                        AI_user.Player.Class = AI_user.Player.FirstClass = AI_user.Player.SecondClass = (byte)AI_class;
                        AI_user.Player.Vitality += 0;
                        AI_user.Player.Strength += 0;
                        AI_user.Player.Spirit += 0;
                        AI_user.Player.Agility += 0;
                        AI_user.Player.SendUpdate(stream, AI_user.Player.Strength, MsgUpdate.DataType.Strength);
                        AI_user.Player.SendUpdate(stream, AI_user.Player.Agility, MsgUpdate.DataType.Agility);
                        AI_user.Player.SendUpdate(stream, AI_user.Player.Spirit, MsgUpdate.DataType.Spirit);
                        AI_user.Player.SendUpdate(stream, AI_user.Player.Vitality, MsgUpdate.DataType.Vitality);
                        AI_user.Player.GuildBattlePower = 0;
                        AI_user.Player.Face = face;
                        AI_user.Player.NobilityRank = (Role.Instance.Nobility.NobilityRank)AI_rank;
                        AI_user.Map = Database.Server.ServerMaps[AI_user.Player.Map];
                        AI_user.Map.Enquer(AI_user);
                        Database.Server.GamePoll.TryAdd(AI_user.Player.UID, AI_user);
                        AI_user.Player.View.SendView(AI_user.Player.GetArray(stream, false), false);
                        AI_Creating(AI_user, stream);
                        AI_user.Player.SetPkMode(Flags.PKMode.Peace);
                        AI_user.Player.HitPoints = ushort.MaxValue;
                        AI_user.Status.MaxHitpoints = ushort.MaxValue;
                        AI_user.AIType = AI_type;
                        AI_user.AIStatus = AIEnum.AIStatus.Idle;
                        switch (AI_user.AIType)
                        {
                            case AIEnum.AIType.BufferBot:
                                {
                                    
                                    AI_user.Player.AddFlag(MsgUpdate.Flags.Stigma, StatusFlagsBigVector32.PermanentFlag, false);
                                    AI_user.Player.AddFlag(MsgUpdate.Flags.Shield, StatusFlagsBigVector32.PermanentFlag, false);
                                    AI_user.Player.AIBotExpire = DateTime.Now.AddHours(1);
                                    break;
                                }
                            case AIEnum.AIType.Hunting:
                                {
                                    AI_user.Player.Action = Flags.ConquerAction.Angry;
                                    AI_user.Player.Angle = Flags.ConquerAngle.North;
                                    AI_user.Player.AddFlag(MsgUpdate.Flags.Hunting, StatusFlagsBigVector32.PermanentFlag, false);
                                    AI_user.Player.AIBotExpire = DateTime.Now.AddHours(1); break;
                                }
                            case AIEnum.AIType.Training:
                                {
                                    //AI_user.Player.Action = Flags.ConquerAction.Angry;
                                    //AI_user.Player.Angle = Flags.ConquerAngle.North;
                                    AI_user.Player.AIBotExpire = DateTime.Now.AddHours(1); break;
                                }
                            case AIEnum.AIType.Leveling:
                                {
                                    AI_user.Player.Action = Flags.ConquerAction.Angry;
                                    AI_user.Player.Angle = Flags.ConquerAngle.North;
                                    AI_user.Team = new Role.Instance.Team(AI_user);
                                    AI_user.Team.PickupItems = AI_user.Team.PickupMoney = AI_user.Team.AutoInvite = true;
                                    AI_user.Player.AddFlag(MsgUpdate.Flags.Hunting, StatusFlagsBigVector32.PermanentFlag, false);
                                    AI_user.Player.AIBotExpire = DateTime.Now.AddHours(1);
                                    break;
                                }
                        }
                    }
                }
            }
            public unsafe void AI_Creating(GameClient client, ServerSockets.Packet stream)
            {
                //skills loading
                for (int x = 0; x < AI_skills.Count; x++)
                {
                    ushort spell = AI_skills[x];
                    if (spell != 0)
                    {
                        client.MySpells.Add(stream, (ushort)spell, 4);
                    }
                }
                //equipments loading
                for (int x = 0; x < AI_equipments.Count; x++)
                {
                    uint equip_items = AI_equipments[x];
                    if (equip_items != 0)
                    {
                        Flags.ConquerItem position = (Flags.ConquerItem)Database.ItemType.ItemPosition(equip_items);
                        switch (position)
                        {
                            case Flags.ConquerItem.RightWeapon:
                            case Flags.ConquerItem.LeftWeapon:
                                {
                                    if (!Database.AtributesStatus.IsWarrior(AI_class) && !Database.AtributesStatus.IsTaoist(AI_class))
                                        client.Equipment.Add(stream, AI_equipments[AI_equipments.Count - 1], Flags.ConquerItem.LeftWeapon, (byte)AI_plus, (byte)AI_damg, (byte)AI_HP, Flags.Gem.SuperDragonGem, Flags.Gem.SuperDragonGem);
                                    client.Equipment.Add(stream, equip_items, position, (byte)AI_plus, (byte)AI_damg, (byte)AI_HP, Flags.Gem.SuperDragonGem, Flags.Gem.SuperDragonGem);
                                    break;
                                }
                            //case Flags.ConquerItem.Tower:
                            //    client.Equipment.Add(stream, equip_items, position, (byte)AI_plus);
                            //    break;
                            //case Flags.ConquerItem.Fan:
                            //    client.Equipment.Add(stream, equip_items, position, (byte)AI_plus);
                            //    break;
                            case Flags.ConquerItem.Bottle:
                                client.Equipment.Add(stream, equip_items, position);
                                break;
                            //case Flags.ConquerItem.Steed:
                            //    client.Equipment.Add(stream, equip_items, position, (byte)AI_plus);
                            //    break;
                            //case Flags.ConquerItem.RidingCrop:
                            //    client.Equipment.Add(stream, equip_items, position, (byte)AI_plus);
                            //    break;
                            case Flags.ConquerItem.RightWeaponAccessory:
                            case Flags.ConquerItem.Garment:
                            //case Flags.ConquerItem.SteedMount:
                                client.Equipment.Add(stream, equip_items, position, 0, 0, 0);
                                break;
                            default:
                                {
                                    if (AI_class == (byte)(Flags.ProfessionType.WATER_SAINT))
                                        client.Equipment.Add(stream, equip_items, position, (byte)AI_plus, (byte)AI_damg, (byte)AI_HP, Flags.Gem.SuperRainbowGem, Flags.Gem.SuperRainbowGem);
                                    else if (AI_class == (byte)(Flags.ProfessionType.FIRE_SAINT))
                                        client.Equipment.Add(stream, equip_items, position, (byte)AI_plus, (byte)AI_damg, (byte)AI_HP, Flags.Gem.SuperPhoenixGem, Flags.Gem.SuperPhoenixGem);
                                    else client.Equipment.Add(stream, equip_items, position, (byte)AI_plus, (byte)AI_damg, (byte)AI_HP, Flags.Gem.SuperDragonGem, Flags.Gem.SuperDragonGem);
                                    if (AI_class == (byte)(Flags.ProfessionType.ARCHER_MASTER))
                                        client.Equipment.Add(stream, 1050002, Flags.ConquerItem.LeftWeapon);
                                    break;
                                }
                        }
                    }
                }
                client.Send(stream.HeroInfo(client.Player));
                client.Equipment.Show(stream);
            }
        }
        public static Extensions.Counter UIDCounter = new Extensions.Counter(60000000);

        public static Dictionary<uint, ArtificialintelligenceInfo> AIBots = new Dictionary<uint, ArtificialintelligenceInfo>();
        public static void Load()
        {
            string[] baseplusText = System.IO.File.ReadAllLines(ServerKernel.CO2FOLDER + "Artificialintelligence.txt");
            foreach (string line in baseplusText)
            {
                string[] data = line.Split(',');
                var ArtificialintelligenceInfo = new ArtificialintelligenceInfo
                {
                    UID = uint.Parse(data[0]),
                    AI_type = (AIEnum.AIType)byte.Parse(data[1]),
                    AI_plus = byte.Parse(data[2]),
                    AI_class = byte.Parse(data[3]),
                    AI_rank = byte.Parse(data[4]),
                    AI_body = ushort.Parse(data[5]),
                    AI_skills = new List<ushort>(),
                    AI_damg = byte.Parse(data[7])

                };
                for (int x = 5; x < 10; x++)
                {
                    ushort SkillID = ushort.Parse(data[x]);
                    if (SkillID != 0)
                        ArtificialintelligenceInfo.AI_skills.Add(SkillID);
                }
                ArtificialintelligenceInfo.AI_equipments = new List<uint>();
                for (int x = 10; x < data.Length; x++)
                {
                    uint ItemID = uint.Parse(data[x]);
                    if (ItemID != 0)
                        ArtificialintelligenceInfo.AI_equipments.Add(ItemID);
                }
                AIBots.Add(ArtificialintelligenceInfo.UID, ArtificialintelligenceInfo);
            }
        }
    }
}
