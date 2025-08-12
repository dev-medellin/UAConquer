using System.Collections.Concurrent;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerCore;
using TheChosenProject.ServerSockets;


namespace TheChosenProject.Game.ConquerStructures.AI
{
    public class AIStructures
    {
        public static ConcurrentDictionary<int, int[]> BuffersLocation = new ConcurrentDictionary<int, int[]>();

        public static void GetMsgKills(GameClient client, string killedname)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                string Msg;
                Msg = Translator.GetTranslatedString(AIEnum.GetKillerMessage(), Translator.Language.EN, client.Language);
                client.Player.View.SendView(new MsgMessage(Msg, killedname, client.Player.Name, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream), false);
                //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "jsb_1eddy_zan");
                //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "newmonbro");
                //client.Player.Action = Flags.ConquerAction.Cool;
            }
        }

        public static byte GetJumpDistance(AIEnum.AIType type)
        {
            switch (type)
            {
                case AIEnum.AIType.Hunting:
                case AIEnum.AIType.PKFighting:
                    return 14;
                default:
                    return 14;
            }
        }

        public static byte GetAttackDistance(GameClient client)
        {
            if (AtributesStatus.IsTrojan(client.Player.Class))
                return 3;
            if (AtributesStatus.IsWarrior(client.Player.Class))
                return 5;
            return 10;
        }

        public static AIEnum.WeaponType GetWeaponType(GameClient client)
        {
            if (ItemType.IsClub(client.Equipment.RightWeapon) || ItemType.IsClub(client.Equipment.LeftWeapon))
                return AIEnum.WeaponType.Club;
            if (ItemType.IsSword(client.Equipment.RightWeapon) || ItemType.IsSword(client.Equipment.LeftWeapon))
                return AIEnum.WeaponType.Sword;
            if (ItemType.IsBlade(client.Equipment.RightWeapon) || ItemType.IsBlade(client.Equipment.LeftWeapon))
                return AIEnum.WeaponType.Blade;
            if (ItemType.IsSpear(client.Equipment.RightWeapon))
                return AIEnum.WeaponType.Spear;
            if (ItemType.IsWand(client.Equipment.RightWeapon))
                return AIEnum.WeaponType.Wand;
            if (ItemType.IsGlaive(client.Equipment.RightWeapon))
                return AIEnum.WeaponType.Glaive;
            if (ItemType.IsPoleaxe(client.Equipment.RightWeapon))
                return AIEnum.WeaponType.Poleaxe;
            if (ItemType.IsHalbert(client.Equipment.RightWeapon))
                return AIEnum.WeaponType.Halbert;
            if (ItemType.IsBackSword(client.Equipment.RightWeapon))
                return AIEnum.WeaponType.BackSword;
            if (ItemType.IsBow(client.Equipment.RightWeapon))
                return AIEnum.WeaponType.BowMan;
            return AIEnum.WeaponType.Hand;
        }

        public static Flags.SpellID WeaponTypeValid(GameClient client, Position poition)
        {
            Flags.SpellID get_spell;
            get_spell = Flags.SpellID.Physical;
            switch (client.Player.Position.Distance(poition))
            {
                case 0:
                case 1:
                    switch (GetWeaponType(client))
                    {
                        case AIEnum.WeaponType.BowMan:
                            {
                                if (client.AIType != AIEnum.AIType.Training)
                                    if (client.MySpells.ClientSpells.ContainsKey((ushort)8001))
                                    {
                                        get_spell = TheChosenProject.Role.Flags.SpellID.ScatterFire;
                                    }
                                    else
                                        get_spell = Flags.SpellID.Physical;
                                break;
                            }
                        case AIEnum.WeaponType.Sword:
                           
                            if (client.MySpells.ClientSpells.ContainsKey(5030))
                                get_spell = Flags.SpellID.Phoenix;
                            
                            break;
                        case AIEnum.WeaponType.BackSword:
                           
                            if (AtributesStatus.IsWater(client.Player.Class) && client.MySpells.ClientSpells.ContainsKey(1001))
                                get_spell = Flags.SpellID.Fire;
                            if (AtributesStatus.IsFire(client.Player.Class))
                            {
                                if (client.MySpells.ClientSpells.ContainsKey(1000))
                                    get_spell = Flags.SpellID.Thunder;

                                if (client.MySpells.ClientSpells.ContainsKey(1001))
                                    get_spell = Flags.SpellID.Fire;

                                if (client.MySpells.ClientSpells.ContainsKey(1002))
                                    get_spell = Flags.SpellID.Tornado;

                            }
                            break;
                        
                    }
                    break;
                case 2:
                    switch (GetWeaponType(client))
                    {
                        case AIEnum.WeaponType.BowMan:
                            {
                                if (client.AIType != AIEnum.AIType.Training)
                                    if (client.MySpells.ClientSpells.ContainsKey((ushort)8001))
                                    {
                                        get_spell = TheChosenProject.Role.Flags.SpellID.ScatterFire;
                                    }
                                else
                                get_spell = Flags.SpellID.Physical;
                                break;
                            }
                        case AIEnum.WeaponType.BackSword:
                            if (AtributesStatus.IsWater(client.Player.Class) && client.MySpells.ClientSpells.ContainsKey(1001))
                                get_spell = Flags.SpellID.Fire;
                            if (AtributesStatus.IsFire(client.Player.Class))
                            {
                                if (client.MySpells.ClientSpells.ContainsKey(1000))
                                    get_spell = Flags.SpellID.Thunder;

                                if (client.MySpells.ClientSpells.ContainsKey(1001))
                                    get_spell = Flags.SpellID.Fire;

                                if (client.MySpells.ClientSpells.ContainsKey(1002))
                                    get_spell = Flags.SpellID.Tornado;

                            }
                            break;
                       
                        case AIEnum.WeaponType.Club:
                            if (client.MySpells.ClientSpells.ContainsKey(7020))
                                get_spell = Flags.SpellID.Rage;
                           
                            break;
                        case AIEnum.WeaponType.Sword:
                            if (client.MySpells.ClientSpells.ContainsKey(5030))
                                get_spell = Flags.SpellID.Phoenix;
                            
                            break;
                        
                        case AIEnum.WeaponType.Wand:
                            if (client.MySpells.ClientSpells.ContainsKey(5010))
                                get_spell = Flags.SpellID.Snow;
                            break;
                        case AIEnum.WeaponType.Spear:
                            if (client.MySpells.ClientSpells.ContainsKey(1260))
                                get_spell = Flags.SpellID.SpeedGun;
                            break;
                        case AIEnum.WeaponType.Glaive:
                            if (client.MySpells.ClientSpells.ContainsKey(1250))
                                get_spell = Flags.SpellID.WideStrike;
                            break;
                        case AIEnum.WeaponType.Poleaxe:
                            if (client.MySpells.ClientSpells.ContainsKey(5050))
                                get_spell = Flags.SpellID.Boreas;
                            break;
                        case AIEnum.WeaponType.Halbert:
                            if (client.MySpells.ClientSpells.ContainsKey(5020))
                                get_spell = Flags.SpellID.StrandedMonster;
                            break;
                    }
                    break;
                default:
                    switch (GetWeaponType(client))
                    {
                        case AIEnum.WeaponType.BowMan:
                            if (client.AIType != AIEnum.AIType.Training)
                                if (client.MySpells.ClientSpells.ContainsKey((ushort)8001))
                                {
                                    get_spell = TheChosenProject.Role.Flags.SpellID.ScatterFire;
                                }
                                else
                                    get_spell = Flags.SpellID.Physical;
                            break;
                        case AIEnum.WeaponType.BackSword:
                            if (AtributesStatus.IsWater(client.Player.Class) && client.MySpells.ClientSpells.ContainsKey(1000))
                                get_spell = Flags.SpellID.Thunder;
                            if (AtributesStatus.IsFire(client.Player.Class))
                            {
                                if (client.MySpells.ClientSpells.ContainsKey(1000))
                                    get_spell = Flags.SpellID.Thunder;
                                
                                if (client.MySpells.ClientSpells.ContainsKey(1001))
                                    get_spell = Flags.SpellID.Fire;

                                if (client.MySpells.ClientSpells.ContainsKey(1002))
                                    get_spell = Flags.SpellID.Tornado;
                                
                            }
                            break;
                        
                        case AIEnum.WeaponType.Blade:
                            if (client.MySpells.ClientSpells.ContainsKey(1045))
                                get_spell = Flags.SpellID.FastBlader;
                            
                            break;
                        case AIEnum.WeaponType.Club:
                            if (client.MySpells.ClientSpells.ContainsKey(7020))
                                get_spell = Flags.SpellID.Rage;
                            break;
                        
                        case AIEnum.WeaponType.Sword:
                            if (client.MySpells.ClientSpells.ContainsKey(1046))
                                get_spell = Flags.SpellID.ScrenSword;
                           
                            break;
                        
                        case AIEnum.WeaponType.Wand:
                            if (client.MySpells.ClientSpells.ContainsKey(11000))
                                get_spell = Flags.SpellID.DragonTail;
                            break;
                        case AIEnum.WeaponType.Spear:
                            if (client.MySpells.ClientSpells.ContainsKey(1260))
                                get_spell = Flags.SpellID.SpeedGun;
                            break;
                    }
                    break;
            }
            return get_spell;
        }
    }
}
