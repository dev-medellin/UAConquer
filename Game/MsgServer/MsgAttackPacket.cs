using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Database;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.Game.MsgServer.AttackHandler.Updates;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack;
using TheChosenProject.Game.MsgServer.AttackHandler;
using TheChosenProject.Game.ConquerStructures.MsgAutoHunting;
using System.Text.RegularExpressions;
using static Poker.Structures.PokerStructs;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgAttackPacket
    {
        [Flags]
        public enum AttackEffect : uint
        {
            None = 0u,
            Block = 1u,
            Penetration = 2u,
            CriticalStrike = 4u,
            Imunity = 8u,
            Break = 0xAu,
            ResistMetal = 0x10u,
            ResistWood = 0x20u,
            ResistWater = 0x40u,
            ResistFire = 0x80u,
            ResistEarth = 0x100u,
            AddStudyPoint = 0x200u,
            LuckyStrike = 0x400u
        }

        public enum AttackID : uint
        {
            None = 0u,
            Physical = 2u,
            Magic = 24u,
            Archer = 28u,
            RequestMarriage = 8u,
            AcceptMarriage = 9u,
            Death = 14u,
            Reflect = 26u,
            Dash = 27u,
            UpdateHunterJar = 36u,
            CounterKillSwitch = 44u,
            Scapegoat = 43u,
            ACT_ITR_PRESENT_EMONEY = 39u,
            FatalStrike = 45u,
            InteractionRequest = 46u,
            InteractionAccept = 47u,
            InteractionRefuse = 48u,
            InteractionEffect = 49u,
            InteractionStopEffect = 50u,
            InMoveSpell = 53u,
            BlueDamage = 55u,
            BackFire = 57u
        }

        public class AttackObj
        {
            public GameClient User;

            public InteractQuery Attack;
        }

        public class ProcessAttackQueue : ConcurrentSmartThreadQueue<AttackObj>
        {
            public ProcessAttackQueue()
                : base(10)
            {
                Start(5);
            }

            public void TryEnqueue(AttackObj action)
            {
                Enqueue(action);
            }

            protected override void OnDequeue(AttackObj action, int time)
            {
            }
        }

        public static ProcessAttackQueue ProcessAttack = new ProcessAttackQueue();
        public static unsafe void Interaction(this ServerSockets.Packet stream, InteractQuery* pQuery, TheChosenProject.Client.GameClient user)
        {
            stream.ReadUnsafe(pQuery, sizeof(InteractQuery));
            if (pQuery->AtkType == AttackID.Magic && user.OnAutoAttack == false)
            {
                DecodeMagicAttack(pQuery, user);
            }
        }

        public static unsafe ServerSockets.Packet InteractionCreate(this ServerSockets.Packet stream, InteractQuery* pQuery)
        {
            //pQuery->Timestamp = TimeStamp.GetTime();
            /*if (pQuery->AtkType == AttackID.Magic)
            {
                EncodeMagicAttack(pQuery);
            }*/

            stream.InitWriter();

            stream.WriteUnsafe(pQuery, sizeof(InteractQuery));
            stream.Finalize(GamePackets.Attack);

            return stream;
        }

        /// <summary>
        /// original  from cosv3
        /// </summary>
        /// <param name="pQuery"></param>
        public static unsafe void EncodeMagicAttack(InteractQuery* pQuery)
        {
            int magicType, magicLevel;
            BitUnfold32(pQuery->Damage, out magicType, out magicLevel);

            magicType = (ushort)(ExchangeShortBits((uint)magicType - 0x14be, 3) ^ pQuery->UID ^ 0x915d);
            magicLevel = (ushort)((magicLevel + 0x100 * (pQuery->TimeStamp % 0x100)) ^ 0x3721);

            pQuery->Damage = BitFold32(magicType, magicLevel);
            pQuery->OpponentUID = (uint)ExchangeLongBits((((uint)pQuery->OpponentUID - 0x8b90b51a) ^ (uint)pQuery->UID ^ 0x5f2d2463u), 32 - 13);
            pQuery->X = (ushort)(ExchangeShortBits((uint)pQuery->X - 0xdd12, 1) ^ pQuery->UID ^ 0x2ed6);
            pQuery->Y = (ushort)(ExchangeShortBits((uint)pQuery->Y - 0x76de, 5) ^ pQuery->UID ^ 0xb99b);
        }
        private static unsafe void DecodeMagicAttack(InteractQuery* pQuery, TheChosenProject.Client.GameClient user)
        {
            pQuery->UID ^= 0x44;
            pQuery->SpellID ^= 0x58;
            int magicType, magicLevel;
            BitUnfold32(pQuery->Damage, out magicType, out magicLevel);
            magicType = (ushort)(ExchangeShortBits(((ushort)magicType ^ (uint)pQuery->UID ^ user.EncryptTokenSpell), 16 - 3) + 0x14be);
            magicLevel = (ushort)(((byte)magicLevel) ^ 0x21);
            pQuery->Damage = BitFold32(magicType, magicLevel);
            pQuery->OpponentUID = (uint)((ExchangeLongBits((uint)pQuery->OpponentUID, 13) ^ (uint)pQuery->UID ^ 0x5f2d2463) + 0x8b90b51a);
            pQuery->X = (ushort)(ExchangeShortBits(((ushort)pQuery->X ^ (uint)pQuery->UID ^ 0x2ed6), 16 - 1) + 0xdd12);
            pQuery->Y = (ushort)(ExchangeShortBits(((ushort)pQuery->Y ^ (uint)pQuery->UID ^ 0xb99b), 16 - 5) + 0x76de);
            Dictionary<ushort, Database.MagicType.Magic> Spells;
            if (Database.Server.Magic.TryGetValue(pQuery->SpellID, out Spells))
            {
                Database.MagicType.Magic spell;
                if (Spells.TryGetValue(pQuery->SpellLevel, out spell))
                {
                    if (spell.Type == Database.MagicType.MagicSort.Collide)
                        return;
                }
            }
            pQuery->UID ^= 0X63;
            pQuery->X = (ushort)(pQuery->OpponentUID ^ pQuery->X);
            pQuery->Y = (ushort)(pQuery->OpponentUID ^ pQuery->X ^ pQuery->Y);
            pQuery->OpponentUID = pQuery->UID ^ 0X63 ^ pQuery->OpponentUID;
        }
        public static int BitFold32(int lower16, int higher16)
        {
            return (lower16) | (higher16 << 16);
        }
        public static void BitUnfold32(int bits32, out int lower16, out int upper16)
        {
            lower16 = (int)(bits32 & UInt16.MaxValue);
            upper16 = (int)(bits32 >> 16);
        }
        public static void BitUnfold64(ulong bits64, out int lower32, out int upper32)
        {
            lower32 = (int)(bits64 & UInt32.MaxValue);
            upper32 = (int)(bits64 >> 32);
        }
        private static uint ExchangeShortBits(uint data, int bits)
        {
            data &= 0xffff;
            return ((data >> bits) | (data << (16 - bits))) & 0xffff;
        }

        public static uint ExchangeLongBits(uint data, int bits)
        {
            return (data >> bits) | (data << (32 - bits));
        }
        [PacketAttribute(GamePackets.Attack)]
        public unsafe static void HandlerProcess(GameClient user, Packet stream)
        {
            if (user.Player.AutoHunting == AutoStructures.Mode.Enable)
                return;
            user.Player.Protect = Time32.Now;
            user.OnAutoAttack = false;
            user.Player.RemoveBuffersMovements(stream);
            user.Player.Action = Flags.ConquerAction.None;
            new AttackObj();
            InteractQuery Attack = default(InteractQuery);
            stream.Interaction(&Attack, user);
            if (user.Player.DelayedTask)
                user.Player.RemovePick(stream);
            user.Player.LastAttack = Time32.Now;
            if (user != null && ItemType.IsShield(user.Equipment.LeftWeapon) && (user.Player.Class < 21 || user.Player.Class > 25))
            {
                if (user.Inventory.HaveSpace(1))
                {
                    user.Equipment.Remove(Flags.ConquerItem.LeftWeapon, stream);
                    user.Equipment.QueryEquipment(user.Equipment.Alternante);
                }
                else
                    user.SendSysMesage("Remove the shield or free 1 inventory spot.");
            }
            else
                Process(user, Attack);
        }


        public unsafe static void Process(GameClient user, InteractQuery Attack)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                if (user.Player.Map == 2068 || (user.IsWatching() && user.Player.Map == 700 && !user.InQualifier()))
                    user.SendSysMesage("Spells are not allowed on this area.");
                else if (user.Player.AutoHunting == AutoStructures.Mode.Enable && Attack.AtkType == AttackID.ACT_ITR_PRESENT_EMONEY)
                {
                    user.Player.MessageBox("Are you sure you want to cancel auto-hunting?", delegate (GameClient p)
                    {
                        p.Player.AutoHunting = AutoStructures.Mode.Disable;
                    }, null);
                }
                else
                {
                    if (!user.Player.Alive)
                        return;
                    if (user.Player.Map != 1039)
                        CheckItems.AttackDurability(user, stream);
                    if (user.Player.ContainFlag(MsgUpdate.Flags.Freeze))
                        return;
                    switch (Attack.AtkType)
                    {
                        //case AttackID.ACT_ITR_PRESENT_EMONEY:
                        //    //if (!user.InTrade && !user.IsVendor)
                        //    //{
                        //    //    int value;
                        //    //    value = (int)user.Player.EmoneyPoints;
                        //    //    if (user.Player.EmoneyPoints >= value)
                        //    //    {
                        //    //        Attack.UID = (Attack.OpponentUID = user.Player.UID);
                        //    //        Attack.ResponseDamage = (uint)value;
                        //    //        user.Send(stream.InteractionCreate(&Attack));                                    
                        //    //        user.Player.ConquerPoints += value;
                        //    //    }
                        //    //}
                        //    break;
                        case AttackID.UpdateHunterJar:
                            {
                                if (user.Inventory.TryGetItem(user.DemonExterminator.ItemUID, out var Jar))
                                {
                                    Attack.UID = Attack.OpponentUID = user.Player.UID;
                                    Attack.X = Jar.MaximDurability;
                                    Attack.dwParam = (int)((user.DemonExterminator.HuntKills << 16) | Jar.MaximDurability);
                                    user.Send(stream.InteractionCreate(&Attack));
                                }
                                break;
                            }
                        case AttackID.InteractionRequest:
                            {
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out var Target, MapObjectType.Player))
                                {
                                    TheChosenProject.Role.Player Opponent;
                                    Opponent = Target as TheChosenProject.Role.Player;
                                    if (user.Player.ObjInteraction == null && Opponent.ObjInteraction == null)
                                    {
                                        Opponent.ActiveDance = (user.Player.ActiveDance = (ushort)Attack.ResponseDamage);
                                        Opponent.Owner.Send(stream.InteractionCreate(&Attack));
                                        Opponent.Owner.Send(stream.InteractionCreate(&Attack));
                                    }
                                }
                                break;
                            }
                        case AttackID.InteractionRefuse:
                            {
                                user.Player.ActiveDance = 0;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out var Target3, MapObjectType.Player))
                                {
                                    TheChosenProject.Role.Player Opponent3;
                                    Opponent3 = Target3 as TheChosenProject.Role.Player;
                                    Opponent3.ActiveDance = 0;
                                    Opponent3.Owner.Send(stream.InteractionCreate(&Attack));
                                }
                                break;
                            }
                        case AttackID.InteractionAccept:
                            {
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out var Target2, MapObjectType.Player))
                                {
                                    TheChosenProject.Role.Player Opponent2;
                                    Opponent2 = Target2 as TheChosenProject.Role.Player;
                                    if (user.Player.ObjInteraction == null && Opponent2.ObjInteraction == null)
                                    {
                                        Attack.ResponseDamage = user.Player.ActiveDance;
                                        Opponent2.Owner.Send(stream.InteractionCreate(&Attack));
                                        Attack.TimeStamp = 0;
                                        user.Send(stream.InteractionCreate(&Attack));
                                        user.Player.Action = (Flags.ConquerAction)Attack.Damage;
                                        Opponent2.Action = (Flags.ConquerAction)Attack.Damage;
                                        user.Player.ObjInteraction = Opponent2.Owner;
                                        Opponent2.ObjInteraction = user;
                                    }
                                }
                                break;
                            }
                        case AttackID.InteractionEffect:
                            if (user.Player.ObjInteraction != null && user.Player.ObjInteraction.Player.ObjInteraction != null)
                            {
                                Attack.ResponseDamage = user.Player.ActiveDance;
                                Attack.TimeStamp = 0;
                                CreateInteractionEffect(Attack, user);
                                InteractQuery user_effect;
                                user_effect = user.Player.InteractionEffect;
                                user.Player.View.SendView(stream.InteractionCreate(&user_effect), true);
                                Attack.UID = user.Player.ObjInteraction.Player.UID;
                                Attack.OpponentUID = user.Player.UID;
                                CreateInteractionEffect(Attack, user.Player.ObjInteraction);
                                user_effect = user.Player.ObjInteraction.Player.InteractionEffect;
                                user.Player.ObjInteraction.Player.View.SendView(stream.InteractionCreate(&user_effect), true);
                            }
                            break;
                        case AttackID.InteractionStopEffect:
                            Attack.ResponseDamage = user.Player.ActiveDance;
                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);
                            Attack.UID = Attack.OpponentUID;
                            Attack.OpponentUID = user.Player.UID;
                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);
                            if (user.Player.ObjInteraction != null)
                            {
                                user.Player.OnInteractionEffect = false;
                                user.Player.Action = Flags.ConquerAction.None;
                                user.Player.ObjInteraction.Player.OnInteractionEffect = false;
                                user.Player.ObjInteraction.Player.Action = Flags.ConquerAction.None;
                                user.Player.ObjInteraction.Player.ObjInteraction = null;
                                user.Player.ObjInteraction = null;
                            }
                            break;
                        case AttackID.RequestMarriage:
                            {
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out var Target4, MapObjectType.Player))
                                {
                                    TheChosenProject.Role.Player Opponent4;
                                    Opponent4 = Target4 as TheChosenProject.Role.Player;
                                    if (user.Player.Spouse != "None" || Opponent4.Spouse != "None")
                                    {
                                        user.SendSysMesage("The target is already married.");
                                        break;
                                    }
                                    if(user.Player.Body == Opponent4.Body)
                                    {
                                        user.SendSysMesage("We dont support same sex marriage.");
                                        break;
                                    }
                                    Opponent4.Send(stream.PopupInfoCreate(user.Player.UID, Opponent4.UID, user.Player.Level, user.Player.BattlePower));
                                    Attack.X = Opponent4.X;
                                    Attack.Y = Opponent4.Y;
                                    Opponent4.Send(stream.InteractionCreate(&Attack));
                                }
                                break;
                            }
                        case AttackID.AcceptMarriage:
                            {
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out var Target5, MapObjectType.Player))
                                {
                                    TheChosenProject.Role.Player Opponent5;
                                    Opponent5 = Target5 as TheChosenProject.Role.Player;
                                    if (user.Player.Spouse != "None" || Opponent5.Spouse != "None")
                                    {
                                        user.SendSysMesage("The target is already married.");
                                        break;
                                    }
                                    user.Player.Spouse = Opponent5.Name;
                                    user.Player.SpouseUID = Opponent5.UID;
                                    Opponent5.Spouse = user.Player.Name;
                                    Opponent5.SpouseUID = user.Player.UID;
                                    MsgMessage messaj;
                                    messaj = new MsgMessage("Joy and happiness! " + user.Player.Name + " and " + Opponent5.Name + " have joined together in the holy marriage. We wish them a stone house.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                                    Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                    user.Player.SendString(stream, MsgStringPacket.StringID.Spouse, false, user.Player.Spouse);
                                    Opponent5.SendString(stream, MsgStringPacket.StringID.Spouse, false, Opponent5.Spouse);
                                    user.Player.SendString(stream, MsgStringPacket.StringID.Fireworks, true, "1122");
                                    user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-2love");
                                }
                                break;
                            }
                        case AttackID.Physical:
                            {
                                if (!AttackHandler.CheckAttack.CheckLineSpells.CheckUp(user, Attack.SpellID))
                                {
                                    return;
                                }
                                Time32 Timer;
                                Timer = Time32.Now;
                                Role.IMapObj target;
                                bool TCBoss = false;
                                //if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                //{
                                //    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                //    if (attacked.Family.ID == 4145)
                                //        TCBoss = true;
                                //}
                                //else if (user.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45 && user.Player.Agility >= 420)
                                //{
                                //    if (Timer < user.Player.AttackStamp)
                                //        return;
                                //}
                                //if (TCBoss)
                                //{
                                //    if (Timer < user.Player.AttackStamp.AddMilliseconds(500))
                                //        return;
                                //}
                                //else
                                //{
                                //    if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(true)))
                                //    {
                                //        return;
                                //    }
                                //}
                                bool isTrojan = user.Player.Class >= 10 && user.Player.Class <= 15;
                                bool isWarrior = user.Player.Class >= 20 && user.Player.Class <= 25;
                                bool hasHighAgility = user.Player.Agility >= 420;
                                double attackSpeed = user.Equipment.AttackSpeed(false);

                                if (isTrojan && hasHighAgility)
                                {
                                    if (user.Player.Owner.Status.AgilityAtack > 280 && user.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45)
                                    {
                                        if (Timer < user.Player.AttackStamp)
                                            return;
                                    }
                                    else if (user.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45 && user.Player.Owner.Status.AgilityAtack < 290 || user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                                    {
                                        if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(false)))
                                            return;
                                    }
                                    else
                                    {
                                        if (Timer < user.Player.AttackStamp.AddMilliseconds(699))
                                            return;
                                    }
                                }
                                else if (isWarrior && hasHighAgility)
                                {
                                    if (user.Player.Owner.Status.AgilityAtack > 250 && user.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45)
                                    {
                                        if (Timer < user.Player.AttackStamp)
                                            return;
                                    }
                                    else
                                    {
                                        if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(false)))
                                            return;
                                    }
                                }
                                else
                                {
                                    if (Timer < user.Player.AttackStamp.AddMilliseconds(699))
                                        return;
                                }

                                AttackHandler.Updates.GetWeaponSpell.CheckExtraEffects(user, stream);
                                if (user.MySpells.ClientSpells.ContainsKey(Attack.SpellID))
                                {
                                    InteractQuery AttackPaket = new InteractQuery();
                                    Role.IMapObj _target;
                                    if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                        || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                        || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                                    {
                                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, _target.X, _target.Y) <= 1)
                                        {
                                            AttackPaket.X = _target.X;
                                            AttackPaket.Y = _target.Y;
                                            if (user.OnAutoAttack)
                                            {
                                                MsgSpell _clientspell;
                                                user.Player.RandomSpell = AttackPaket.SpellID;
                                                AttackPaket.OpponentUID = Attack.OpponentUID;
                                                AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;

                                                MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);
                                                if (_target.Alive)
                                                {
                                                    CreateAutoAtack(Attack, user);
                                                }
                                                else
                                                    user.OnAutoAttack = false;
                                                break;
                                            }

                                            if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, _target))
                                            {
                                                AttackPaket.UID = Attack.UID;
                                                AttackPaket.SpellID = Attack.SpellID;
                                                user.Player.RandomSpell = Attack.SpellID;
                                                AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                                MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);
                                                MsgSpell _clientspell;
                                                if (_target.Alive)
                                                {
                                                    CreateAutoAtack(Attack, user);
                                                }
                                                else
                                                    user.OnAutoAttack = false;
                                            }
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                        break;
                                    }
                                }
                                if (/*(Attack.SpellID == (ushort)Role.Flags.SpellID.AirStrike || Attack.SpellID == (ushort)Role.Flags.SpellID.EarthSweep) && */user.MySpells.ClientSpells.ContainsKey(Attack.SpellID))
                                {
                                    InteractQuery AttackPaket = new InteractQuery();
                                    Role.IMapObj _target;
                                    if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                        || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                        || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                                    {
                                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, _target.X, _target.Y) <= 1)
                                        {
                                            AttackPaket.X = _target.X;
                                            AttackPaket.Y = _target.Y;
                                            if (user.OnAutoAttack)
                                            {
                                                List<ushort> CanUse = new List<ushort>();
                                                //if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirStrike))
                                                //    CanUse.Add((ushort)Role.Flags.SpellID.AirStrike);
                                                //if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthSweep))
                                                //    CanUse.Add((ushort)Role.Flags.SpellID.EarthSweep);
                                                //if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Kick))
                                                //    CanUse.Add((ushort)Role.Flags.SpellID.Kick);
                                                AttackPaket.SpellID = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                                                user.Player.RandomSpell = AttackPaket.SpellID;
                                                AttackPaket.OpponentUID = Attack.OpponentUID;
                                                AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                                MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);
                                                if (_target.Alive)
                                                {
                                                    CreateAutoAtack(Attack, user);
                                                }
                                                else
                                                    user.OnAutoAttack = false;
                                                break;
                                            }

                                            if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, _target))
                                            {
                                                AttackPaket.X = _target.X;
                                                AttackPaket.Y = _target.Y;
                                                AttackPaket.OpponentUID = _target.UID;
                                                AttackPaket.UID = Attack.UID;
                                                AttackPaket.SpellID = Attack.SpellID;
                                                user.Player.RandomSpell = Attack.SpellID;

                                                AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                                MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);

                                                if (_target.Alive)
                                                    CreateAutoAtack(Attack, user);
                                                else
                                                    user.OnAutoAttack = false;
                                            }
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                        break;
                                    }
                                }
                                if (user.MySpells.ClientSpells.ContainsKey(Attack.SpellID))
                                {
                                    InteractQuery AttackPaket = new InteractQuery();
                                    Role.IMapObj _target;
                                    if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                        || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                        || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                                    {
                                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, _target.X, _target.Y) <= 3)
                                        {
                                            AttackPaket.X = _target.X;
                                            AttackPaket.Y = _target.Y;
                                            if (user.OnAutoAttack)
                                            {
                                                List<ushort> CanUse = new List<ushort>();
                                                AttackPaket.SpellID = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                                                user.Player.RandomSpell = AttackPaket.SpellID;
                                                AttackPaket.OpponentUID = Attack.OpponentUID;
                                                AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                                MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);
                                                if (_target.Alive)
                                                {
                                                    CreateAutoAtack(Attack, user);
                                                }
                                                else
                                                    user.OnAutoAttack = false;
                                                break;
                                            }
                                            AttackPaket.X = _target.X;
                                            AttackPaket.Y = _target.Y;
                                            AttackPaket.OpponentUID = _target.UID;
                                            AttackPaket.UID = Attack.UID;
                                            AttackPaket.SpellID = Attack.SpellID;
                                            user.Player.RandomSpell = Attack.SpellID;
                                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                            MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);
                                            if (_target.Alive)
                                                CreateAutoAtack(Attack, user);
                                            else
                                                user.OnAutoAttack = false;
                                            break;
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                        break;
                                    }
                                }
                                if (user.MySpells.ClientSpells.ContainsKey(Attack.SpellID))
                                {
                                    InteractQuery AttackPaket = new InteractQuery();
                                    Role.IMapObj _target;
                                    if (user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Player)
                                        || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.Monster)
                                        || user.Player.View.TryGetValue(Attack.OpponentUID, out _target, Role.MapObjectType.SobNpc))
                                    {
                                        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, _target.X, _target.Y) <= 3)
                                        {
                                            AttackPaket.X = _target.X;
                                            AttackPaket.Y = _target.Y;
                                            if (user.OnAutoAttack)
                                            {
                                                List<ushort> CanUse = new List<ushort>();
                                                if (user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                                                    CanUse.Add((ushort)Role.Flags.SpellID.TripleAttack);
                                                AttackPaket.SpellID = (ushort)CanUse[Program.GetRandom.Next(0, CanUse.Count)];
                                                user.Player.RandomSpell = AttackPaket.SpellID;
                                                AttackPaket.OpponentUID = Attack.OpponentUID;
                                                AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                                MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);
                                                if (_target.Alive)
                                                {
                                                    CreateAutoAtack(Attack, user);
                                                }
                                                else
                                                    user.OnAutoAttack = false;
                                                break;
                                            }
                                            AttackPaket.X = _target.X;
                                            AttackPaket.Y = _target.Y;
                                            AttackPaket.OpponentUID = _target.UID;

                                            AttackPaket.UID = Attack.UID;

                                            if (AttackHandler.Calculate.Base.Success(30) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                                            {
                                                AttackPaket.SpellID = (ushort)Role.Flags.SpellID.TripleAttack;
                                                user.Player.RandomSpell = AttackPaket.SpellID;
                                            }
                                            AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                            MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket);

                                            if (_target.Alive)
                                                CreateAutoAtack(Attack, user);
                                            else
                                                user.OnAutoAttack = false;
                                            break;
                                        }
                                        else
                                            user.OnAutoAttack = false;

                                        break;
                                    }
                                }
                                user.Player.AttackStamp = Timer;

                                MsgServer.AttackHandler.CheckAttack.CheckGemEffects.TryngEffect(user);
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, target))
                                    {
                                        if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 1)
                                        {
                                            Role.Player attacked = target as Role.Player;
                                            if (AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, null))
                                            {
                                                Attack.TimeStamp = 0;

                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                AttackHandler.Calculate.Physical.OnPlayer(user.Player, attacked, null, out AnimationObj);

                                                Attack.Damage = (int)AnimationObj.Damage;
                                                Attack.Effect = AnimationObj.Effect;

                                                user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                                                Attack.AtkType = AttackID.Physical;

                                                AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                                if (attacked.Alive)
                                                    CreateAutoAtack(Attack, user);
                                            }
                                            else
                                                user.OnAutoAttack = false;
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, target))
                                    {
                                        MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                        if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 3 || user.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                                        {
                                            if (AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, null))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                if (attacked.Family.ID == 4145)
                                                    Attack.SpellID = 0;
                                                Attack.TimeStamp = 0;

                                                if (user.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                                                {
                                                    Attack.AtkType = AttackID.FatalStrike;
                                                    user.Shift(target.X, target.Y, stream);
                                                    AttackHandler.Calculate.Physical.OnMonster(user.Player, attacked, Server.Magic[(ushort)Role.Flags.SpellID.FatalStrike][0], out AnimationObj);
                                                }
                                                else
                                                    AttackHandler.Calculate.Physical.OnMonster(user.Player, attacked, null, out AnimationObj);

                                                Attack.Damage = (int)AnimationObj.Damage;
                                                Attack.Effect = AnimationObj.Effect;
                                                if (attacked.Family.ID == 21060 && user.Player.Level >= 130)
                                                {
                                                    AnimationObj.Damage = AnimationObj.Damage * 100;
                                                }
                                                user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                                                Attack.AtkType = AttackID.Physical;
                                                AttackHandler.Updates.IncreaseExperience.Up(stream, user, AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked));
                                                AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                                if (attacked.Alive)
                                                    CreateAutoAtack(Attack, user);

                                                if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) != MsgMonster.MonsterSettings.Guard) 
                                                {
                                                    if (Attack.Damage > attacked.Family.MaxHealth)
                                                    {
                                                        uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                        uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                        if (ProfRightWeapon != 0)
                                                            user.MyProfs.CheckUpdate(ProfRightWeapon, (uint)AdjustExp(attacked.Family.MaxHealth, user.Player.Level, attacked.Level), stream);
                                                        if (PorfLeftWeapon != 0)
                                                            user.MyProfs.CheckUpdate(PorfLeftWeapon, (uint)AdjustExp(attacked.Family.MaxHealth, user.Player.Level, attacked.Level), stream);
                                                    }
                                                    else
                                                    {
                                                        uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                        uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                        if (ProfRightWeapon != 0)
                                                            user.MyProfs.CheckUpdate(ProfRightWeapon, (uint)AdjustExp((int)Attack.Damage, user.Player.Level, attacked.Level), stream);
                                                        if (PorfLeftWeapon != 0)
                                                            user.MyProfs.CheckUpdate(PorfLeftWeapon, (uint)AdjustExp((int)Attack.Damage, user.Player.Level, attacked.Level), stream);
                                                    }
                                                }
                                            else
                                                {
                                                    uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                    uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                    if (ProfRightWeapon != 0)
                                                        user.MyProfs.CheckUpdate(ProfRightWeapon, 0, stream);
                                                    if (PorfLeftWeapon != 0)
                                                        user.MyProfs.CheckUpdate(PorfLeftWeapon, 0, stream);
                                                }
                                            }
                                            else
                                                user.OnAutoAttack = false;
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                                {
                                    if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, target))
                                    {
                                        if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 3)
                                        {
                                            var attacked = target as Role.SobNpc;
                                            if (AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, null))
                                            {
                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                AttackHandler.Calculate.Physical.OnNpcs(user.Player, attacked, null, out AnimationObj);

                                                Attack.Damage = (int)AnimationObj.Damage;
                                                Attack.Effect = AnimationObj.Effect;
                                                user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                                                Attack.AtkType = AttackID.Physical;

                                                AttackHandler.Updates.IncreaseExperience.Up(stream, user, AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked));
                                                AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                                if (attacked.Alive)
                                                    CreateAutoAtack(Attack, user);

                                                if (attacked.Map == 1039 || (attacked.Map == 1002 && (attacked.UID == 6003 || attacked.UID == 6004 || attacked.UID == 6005 || attacked.UID == 6006)))
                                                {
                                                    if (Attack.Damage > attacked.MaxHitPoints)
                                                    {
                                                        uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                        uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                        if (ProfRightWeapon != 0)
                                                            user.MyProfs.CheckUpdate(ProfRightWeapon, (uint)AdjustExp(attacked.MaxHitPoints, user.Player.Level, 70), stream);
                                                        if (PorfLeftWeapon != 0)
                                                            user.MyProfs.CheckUpdate(PorfLeftWeapon, (uint)AdjustExp(attacked.MaxHitPoints, user.Player.Level, 70), stream);
                                                    }
                                                    else
                                                    {
                                                        uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                        uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                        if (ProfRightWeapon != 0)
                                                            user.MyProfs.CheckUpdate(ProfRightWeapon, (uint)AdjustExp((int)Attack.Damage, user.Player.Level, 70), stream);
                                                        if (PorfLeftWeapon != 0)
                                                            user.MyProfs.CheckUpdate(PorfLeftWeapon, (uint)AdjustExp((int)Attack.Damage, user.Player.Level, 70), stream);
                                                    }
                                                }
                                                else
                                                {
                                                    uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                    uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                    if (ProfRightWeapon != 0)
                                                        user.MyProfs.CheckUpdate(ProfRightWeapon, 0, stream);
                                                    if (PorfLeftWeapon != 0)
                                                        user.MyProfs.CheckUpdate(PorfLeftWeapon, 0, stream);
                                                }
                                            }
                                            else
                                                user.OnAutoAttack = false;
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                }
                                else
                                    user.OnAutoAttack = false;
                                break;
                            }
                        case AttackID.Archer:
                            {
                                if (!AttackHandler.CheckAttack.CheckLineSpells.CheckUp(user, Attack.SpellID))
                                    break;
                                AttackHandler.Updates.GetWeaponSpell.CheckExtraEffects(user, stream);

                                Time32 Timer;
                                Timer = Time32.Now;
                                //if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(false)))
                                //    return;
                                //foreach (var item in user.Equipment.ClientItems.Values)
                                //{
                                //    if (item != null)
                                //    {
                                //        try
                                //        {
                                //            if ( item.Position != (ushort)Role.Flags.ConquerItem.Ring)
                                //            {
                                //                item.ITEM_ID = Database.Server.ItemsBase.DowngradeItem(item.ITEM_ID);
                                //                item.Mode = Role.Flags.ItemMode.Update;
                                //                item.Send(user, stream);
                                //            }
                                //        }
                                //        catch (Exception e) { Console.WriteLine(e.ToString()); }
                                //    }
                                //}
                                //if (user.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45 && user.Player.Agility >= 420 )
                                //{
                                //    if (Timer < user.Player.AttackStamp.AddMilliseconds(150))
                                //        return;
                                //}
                                //else
                                //{
                                //    if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(false)))
                                //        return;
                                //}
                                if (user.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45 && user.Player.Agility >= 420)
                                {
                                    if (Timer < user.Player.AttackStamp.AddMilliseconds(200))
                                        return;
                                }
                                else
                                {
                                    if (Timer < user.Player.AttackStamp.AddMilliseconds(user.Equipment.AttackSpeed(false)))
                                        return;
                                }
                                user.Player.AttackStamp = Timer;
                                MsgGameItem arrow;
                                arrow = null;
                                if (user.Equipment.FreeEquip(Flags.ConquerItem.LeftWeapon))
                                    break;
                                user.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeapon, out arrow);
                                if (arrow.Durability <= 0)
                                    break;

                                MsgServer.AttackHandler.CheckAttack.CheckGemEffects.TryngEffect(user);

                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    if (!AttackHandler.Updates.GetWeaponSpell.Check(Attack, stream, user, target))
                                    {
                                        if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 18)
                                        {
                                            Role.Player attacked = target as Role.Player;
                                            if (AttackHandler.CheckAttack.CanAttackPlayer.Verified(user, attacked, null,true))
                                            {
                                                //if (user.Player.ContainFlag(MsgUpdate.Flags.KineticSpark))
                                                //{
                                                //    if (AttackHandler.Calculate.Base.Rate(30))
                                                //    {
                                                //        Dictionary<ushort, Database.MagicType.Magic> Spells;
                                                //        if (Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.KineticSpark, out Spells))
                                                //        {
                                                //            Attack.SpellID = (ushort)Role.Flags.SpellID.KineticSpark;
                                                //            Database.MagicType.Magic spell;
                                                //            if (Spells.TryGetValue(Attack.SpellLevel, out spell))
                                                //            {
                                                //                AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                //                break;
                                                //            }
                                                //        }
                                                //    }
                                                //}

                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                AttackHandler.Calculate.Range.OnPlayer(user.Player, attacked, null, out AnimationObj);

                                                Attack.Damage = (int)AnimationObj.Damage;
                                                Attack.Effect = AnimationObj.Effect;
                                                user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                                                Attack.AtkType = AttackID.Archer;
                                                CreateAutoAtack(Attack, user);

                                                AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                                if (user.Player.Map != 1039)
                                                    arrow.Durability -= Math.Min(arrow.Durability, (ushort)1);
                                                user.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, arrow.UID, arrow.Durability, 0u, 0u, 0u, 0u));
                                                if (arrow.Durability <= 0 || arrow.Durability > 10000)
                                                    CanUseSpell.ReloadArrows(user.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeapon), user, stream);
                                            }
                                            else
                                                user.OnAutoAttack = false;
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 18)
                                    {
                                        MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                        if (AttackHandler.CheckAttack.CanAttackMonster.Verified(user, attacked, null))
                                        {
                                            //if (user.Player.ContainFlag(MsgUpdate.Flags.KineticSpark))
                                            //{
                                            //    if (AttackHandler.Calculate.Base.Rate(30))
                                            //    {
                                            //        Dictionary<ushort, Database.MagicType.Magic> Spells;
                                            //        if (Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.KineticSpark, out Spells))
                                            //        {
                                            //            Attack.SpellID = (ushort)Role.Flags.SpellID.KineticSpark;
                                            //            Database.MagicType.Magic spell;
                                            //            if (Spells.TryGetValue(Attack.SpellLevel, out spell))
                                            //            {
                                            //                AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                            //                break;
                                            //            }
                                            //        }
                                            //    }
                                            //}

                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            AttackHandler.Calculate.Range.OnMonster(user.Player, attacked, null, out AnimationObj);

                                            Attack.Damage = (int)AnimationObj.Damage;
                                            Attack.Effect = AnimationObj.Effect;
                                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                                            Attack.AtkType = AttackID.Archer;
                                            AttackHandler.Updates.IncreaseExperience.Up(stream, user, AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked));
                                            AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                            CreateAutoAtack(Attack, user);
                                            AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                            if (user.Player.Map != 1039)
                                                arrow.Durability -= Math.Min(arrow.Durability, (ushort)1);
                                            user.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, arrow.UID, arrow.Durability, 0u, 0u, 0u, 0u));
                                            if (arrow.Durability <= 0 || arrow.Durability > 10000)
                                                CanUseSpell.ReloadArrows(user.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeapon), user, stream);
                                            if (attacked.Alive)
                                                CreateAutoAtack(Attack, user);

                                            if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) != MsgMonster.MonsterSettings.Guard) 
                                                {
                                                if (Attack.Damage > attacked.Family.MaxHealth)
                                                {
                                                    uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                    uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                    if (ProfRightWeapon != 0)
                                                        user.MyProfs.CheckUpdate(ProfRightWeapon, (uint)AdjustExp(attacked.Family.MaxHealth, user.Player.Level, attacked.Level), stream); 
                                                        if (PorfLeftWeapon != 0)
                                                        user.MyProfs.CheckUpdate(PorfLeftWeapon, (uint)AdjustExp(attacked.Family.MaxHealth, user.Player.Level, attacked.Level), stream);
                                                }
                                                else
                                                {
                                                    uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                    uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                    if (ProfRightWeapon != 0)
                                                        user.MyProfs.CheckUpdate(ProfRightWeapon, (uint)AdjustExp((int)Attack.Damage, user.Player.Level, attacked.Level), stream);
                                                    if (PorfLeftWeapon != 0)
                                                        user.MyProfs.CheckUpdate(PorfLeftWeapon, (uint)AdjustExp((int)Attack.Damage, user.Player.Level, attacked.Level), stream);
                                                }
                                            }
                                                else
                                            {
                                                uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                if (ProfRightWeapon != 0)
                                                    user.MyProfs.CheckUpdate(ProfRightWeapon, 0, stream);
                                                if (PorfLeftWeapon != 0)
                                                    user.MyProfs.CheckUpdate(PorfLeftWeapon, 0, stream);
                                            }
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                    else
                                        user.OnAutoAttack = false;
                                }
                                else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                                {
                                    if (AttackHandler.Calculate.Base.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) <= 18)
                                    {
                                        var attacked = target as Role.SobNpc;
                                        if (AttackHandler.CheckAttack.CanAttackNpc.Verified(user, attacked, null))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            AttackHandler.Calculate.Range.OnNpcs(user.Player, attacked, null, out AnimationObj);

                                            Attack.Damage = (int)AnimationObj.Damage;
                                            Attack.Effect = AnimationObj.Effect;

                                            user.Player.View.SendView(stream.InteractionCreate(&Attack), true);

                                            Attack.AtkType = AttackID.Archer;
                                            CreateAutoAtack(Attack, user);
                                            AttackHandler.Updates.IncreaseExperience.Up(stream, user, AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked));

                                            AttackHandler.Updates.UpdateSpell.CheckUpdate(stream, user, Attack, AnimationObj.Damage, null);
                                            if (user.Player.Map != 1039 && (Attack.OpponentUID < 6003 || Attack.OpponentUID > 6007))
                                                arrow.Durability -= Math.Min(arrow.Durability, (ushort)1);
                                            user.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, arrow.UID, arrow.Durability, 0u, 0u, 0u, 0u));
                                            if (arrow.Durability <= 0 || arrow.Durability > 10000)
                                                CanUseSpell.ReloadArrows(user.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeapon), user, stream);
                                            if (attacked.Alive)
                                                CreateAutoAtack(Attack, user);

                                            if (attacked.Map == 1039)
                                            {
                                                if (Attack.Damage > attacked.MaxHitPoints)
                                                {
                                                    uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                    uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                    if (ProfRightWeapon != 0)
                                                        user.MyProfs.CheckUpdate(ProfRightWeapon, (uint)AdjustExp(attacked.MaxHitPoints, user.Player.Level, 70), stream);
                                                    if (PorfLeftWeapon != 0)
                                                        user.MyProfs.CheckUpdate(PorfLeftWeapon, (uint)AdjustExp(attacked.MaxHitPoints, user.Player.Level, 70), stream);
                                                }
                                                else
                                                {
                                                    uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                    uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                    if (ProfRightWeapon != 0)
                                                        user.MyProfs.CheckUpdate(ProfRightWeapon, (uint)AdjustExp((int)Attack.Damage, user.Player.Level, 70), stream);
                                                    if (PorfLeftWeapon != 0)
                                                        user.MyProfs.CheckUpdate(PorfLeftWeapon, (uint)AdjustExp((int)Attack.Damage, user.Player.Level, 70), stream);
                                                }
                                            }
                                                else
                                            {
                                                uint ProfRightWeapon = user.Equipment.RightWeapon / 1000;
                                                uint PorfLeftWeapon = user.Equipment.LeftWeapon / 1000;
                                                if (ProfRightWeapon != 0)
                                                    user.MyProfs.CheckUpdate(ProfRightWeapon, 0, stream);
                                                if (PorfLeftWeapon != 0)
                                                    user.MyProfs.CheckUpdate(PorfLeftWeapon, 0, stream);
                                            }
                                        }
                                        else
                                            user.OnAutoAttack = false;
                                    }
                                    else
                                        user.OnAutoAttack = false;
                                }
                                else
                                    user.OnAutoAttack = false;

                                break;
                            }
                        case AttackID.CounterKillSwitch:
                            {
                                if (!CheckLineSpells.CheckUp(user, Attack.SpellID) || !Server.Magic.TryGetValue(6003, out var Spells) || !user.MySpells.ClientSpells.TryGetValue(6003, out var ClientSpell) || !Spells.TryGetValue(ClientSpell.Level, out var spell))
                                    break;
                                MagicType.MagicSort type;
                                type = spell.Type;
                                if (type == MagicType.MagicSort.CounterKill)
                                {
                                    Attack.SpellID = 6003;
                                    Attack.SpellLevel = ClientSpell.Level;
                                    if (CanUseSpell.Verified(Attack, user, Spells, out ClientSpell, out var _))
                                    {
                                        user.Player.ActivateCounterKill = !user.Player.ActivateCounterKill;
                                        Attack.OnCounterKill = user.Player.ActivateCounterKill;
                                        user.Send(stream.InteractionCreate(&Attack));
                                    }
                                }
                                break;
                            }
                        case AttackID.Magic:
                            ProcescMagic(user, stream, Attack);
                            break;
                    }
                    //user.Equipment.AppendItems(true, user.Equipment.CurentEquip, stream);

                }
            }
        }

        public static int AdjustExp(int nDamage, int nAtkLev, int nDefLev)
        {

            //   return nDamage;
            //if (nAtkLev > 120)
            //    nAtkLev = 120;
            int nExp = nDamage;
            int nNameType = AttackHandler.Calculate.Base.GetNameType(nAtkLev, nDefLev);
            int Level = 120;
            if (nDefLev < 120)
                Level = nDefLev;


            if (nNameType == AttackHandler.Calculate.Base.StatusConstants.NAME_GREEN)
            {
                Int32 DeltaLvl = nAtkLev - nDefLev;
                if (DeltaLvl >= 3 && DeltaLvl <= 5)
                    nExp = nExp * 4 / 100;
                else if (DeltaLvl > 5 && DeltaLvl <= 10)
                    nExp = nExp * 3 / 100;
                else if (DeltaLvl > 10 && DeltaLvl <= 20)
                    nExp = nExp * 2 / 100;
                else if (DeltaLvl > 20)
                    nExp = nExp * 1 / 100;
                else
                    nExp = nExp * 1 / 100;
            }
            else if (nNameType == AttackHandler.Calculate.Base.StatusConstants.NAME_RED)
            { nExp *= (int)1.5; }
            else if (nNameType == AttackHandler.Calculate.Base.StatusConstants.NAME_BLACK)
            {
                Int32 DeltaLvl = nDefLev - Level;
                if (DeltaLvl >= -10 && DeltaLvl <= -5)
                    nExp *= (int)2.0;
                else if (DeltaLvl >= -20 && DeltaLvl < -10)
                    nExp *= (int)3.5;
                else if (DeltaLvl < -20)
                    nExp *= (int)5.0;
            }

            return Math.Max(0, (Int32)nExp);
        }

        public static void ProcescMagic(GameClient user, Packet stream, InteractQuery Attack, bool ignoreStamp = false)
        {
            if (!CheckLineSpells.CheckUp(user, Attack.SpellID))
                return;
            CheckGemEffects.TryngEffect(user);
            if (!user.AllowUseSpellOnSteed(Attack.SpellID))
            {
                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                return;
            }
            bool OnTGAutoAttack;
            OnTGAutoAttack = true;
            if (Server.Magic.TryGetValue(Attack.SpellID, out var Spells) && Spells.TryGetValue(Attack.SpellLevel, out var spell))
            {
                Time32 Timer;
                Timer = Time32.Now;
                if (spell.CoolDown == 0)
                    spell.CoolDown = 500;

                if (!ignoreStamp)
                {
                    if (spell.CoolDown > 1000 && spell.CoolDown < 3000)
                        spell.CoolDown = 800;
                    else if (Timer < user.Player.AttackStamp.AddMilliseconds(spell.CoolDown)/* && spell.ID != 1045 && spell.ID != 1046 && spell.ID != 11005*/)
                    {
                        return;
                    }
                    user.Player.AttackStamp = Timer;
                    if (user.Player.Map == 1039)
                    {
                        if (spell.CoolDown > 300 && spell.CoolDown < 500)
                            spell.CoolDown = 800;
                    }
                    if (AtributesStatus.IsFire(user.Player.Class) && (spell.CoolDown > 300 && spell.CoolDown < 500))
                    {
                        spell.CoolDown = 600;
                    }
                }
                if (AtributesStatus.IsFire(user.Player.Class))
                {
                    spell.CoolDown = 500;
                }
                if (!user.OnAutoAttack)
                {
                    if (user.Equipment.RightWeaponEffect == Flags.ItemEffect.MP && spell != null && spell.UseMana > 0 && (Attack.SpellID == 1175 || Base.Success(30.0)))
                    {
                        user.Player.RandomSpell = 1175;
                        EffectMP.Execute(Attack, user, stream, Spells);
                    }
                    GetWeaponSpell.CheckExtraEffects(user, stream);
                }else if (Attack.OpponentUID >= 6003 && Attack.OpponentUID <= 6006)
                {
                    if (user.Equipment.RightWeaponEffect == Flags.ItemEffect.MP && spell != null && spell.UseMana > 0 && (Attack.SpellID == 1175 || Base.Success(100.0)))
                    {
                        user.Player.RandomSpell = 1175;
                        EffectMP.Execute(Attack, user, stream, Spells);
                    }
                    GetWeaponSpell.CheckExtraEffects(user, stream);
                }

                if (user.Map.ID == 1616 || user.Map.ID == 1036 && spell.Type != MagicType.MagicSort.AttachStatus)
                {
                    user.SendSysMesage("You can't use skill on this map!");
                    return;
                }

                switch (spell.Type)
                {
                    case MagicType.MagicSort.AddMana:
                        AddMana.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.MortalDrag:
                        MortalDrag.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.ChargingVortex:
                        ChargingVortex.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.PirateXpSkill:
                        PirateXpSkill.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.AddBlackSpot:
                        AddBlackSpot.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.MoveLine:
                        MoveLine.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.BombLine:
                        BombLine.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.BlackSpot:
                        BlackSpot.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.CannonBarrage:
                        CannonBarrage.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.ScurvyBomb:
                        ScurvyBomb.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.PhysicalSpells:
                        PhysicalSpells.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.WhirlwindKick:
                        WhirlwindKick.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Oblivion:
                        Oblivion.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.DispatchXp:
                        DispatchXp.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.ShieldBlock:
                        ShieldBlock.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.RemoveBuffers:
                    case MagicType.MagicSort.Tranquility:
                    case MagicType.MagicSort.Compasion:
                        RemoveBuffers.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Perimeter:
                        Perimeter.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Auras:
                        Auras.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.DirectAttack:
                        DirectAttack.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.DragonWhirl:
                        DragonWhirl.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.StarArrow:
                        StarArrow.ExecuteExecute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.ChainBolt:
                        ChainBolt.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Spook:
                        Spook.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.WarCry:
                        WarCry.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Riding:
                        Riding.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.ShurikenVortex:
                        ShurikenVortex.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Toxic:
                        Toxic.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.DecLife:
                        DecLife.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Transform:
                        Transform.Execute(user, Attack, stream, Spells);
                        break;
                    case Database.MagicType.MagicSort.AttackStatus:
                        {
                            if (Game.MsgServer.AttackHandler.Calculate.Base.Rate(20))
                            {
                                AttackHandler.AttackStatus.Execute(user, Attack, stream, Spells);
                            }
                        }
                    break;
                    case MagicType.MagicSort.Collide:
                        Collide.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Sector:
                        Sector.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Line:
                        Line.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Attack:
                        TheChosenProject.Game.MsgServer.AttackHandler.Attack.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.AttachStatus:
                        {
                            OnTGAutoAttack = true;
                            AttachStatus.Execute(user, Attack, stream, Spells);
                        }
                        break;
                    case MagicType.MagicSort.DetachStatus:
                        DetachStatus.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Recruit:
                        Recruit.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Bomb:
                        Bomb.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.Pounce:
                        Pounce.Execute(user, Attack, stream, Spells);
                        break;
                    case MagicType.MagicSort.CallPet:
                        if (user.Map.ID == 1616 || user.Map.ID == 1036 || user.Map.ID == 1700)
                        {
                            user.SendSysMesage("You can't summon guard on this map!");
                            return;
                        }
                        SummonGuard.Execute(user, Attack, stream, Spells);
                        break;
                }
            }
            if (user.Player.Map == 1039 && OnTGAutoAttack)
                CreateAutoAtack(Attack, user);
        }

        public static void CreateAutoAtack(InteractQuery pQuery, Client.GameClient client)
        {
            client.OnAutoAttack = true;

            client.AutoAttack = new InteractQuery();
            client.AutoAttack.AtkType = pQuery.AtkType;
            client.AutoAttack.Damage = pQuery.Damage;
            client.AutoAttack.Data = pQuery.Data;
            client.AutoAttack.dwParam = pQuery.dwParam;
            client.AutoAttack.Effect = pQuery.Effect;
            client.AutoAttack.OpponentUID = pQuery.OpponentUID;
            client.AutoAttack.ResponseDamage = pQuery.ResponseDamage;
            client.AutoAttack.SpellID = pQuery.SpellID;
            client.AutoAttack.SpellLevel = pQuery.SpellLevel;
            client.AutoAttack.UID = pQuery.UID;
            client.AutoAttack.X = pQuery.X;
            client.AutoAttack.Y = pQuery.Y;
        }

        public static void CreateInteractionEffect(InteractQuery pQuery, GameClient client)
        {
            client.Player.OnInteractionEffect = true;
            client.Player.InteractionEffect = default(InteractQuery);
            client.Player.InteractionEffect.AtkType = pQuery.AtkType;
            client.Player.InteractionEffect.Damage = pQuery.Damage;
            client.Player.InteractionEffect.Data = pQuery.Data;
            client.Player.InteractionEffect.dwParam = pQuery.dwParam;
            client.Player.InteractionEffect.Effect = pQuery.Effect;
            client.Player.InteractionEffect.OpponentUID = pQuery.OpponentUID;
            client.Player.InteractionEffect.ResponseDamage = pQuery.ResponseDamage;
            client.Player.InteractionEffect.SpellID = pQuery.SpellID;
            client.Player.InteractionEffect.SpellLevel = pQuery.SpellLevel;
            client.Player.InteractionEffect.UID = pQuery.UID;
            client.Player.InteractionEffect.X = pQuery.X;
            client.Player.InteractionEffect.Y = pQuery.Y;
        }
    }
}