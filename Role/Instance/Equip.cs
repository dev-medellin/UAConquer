using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;
using Extensions;

namespace TheChosenProject.Role.Instance
{
    public class Equip
    {
        public ConcurrentDictionary<uint, MsgGameItem> ClientItems = new ConcurrentDictionary<uint, MsgGameItem>();
        public uint SoulsPotency;
        public int WeaponsMinAttack;
        public uint ArmorID;
        public bool CreateSpawn = true;
        public bool SuperArmor;
        public TheChosenProject.Role.Flags.ItemEffect RightWeaponEffect;
        public TheChosenProject.Role.Flags.ItemEffect LeftWeaponEffect;
        public TheChosenProject.Role.Flags.ItemEffect RingEffect;
        public TheChosenProject.Role.Flags.ItemEffect NecklaceEffect;
        public bool UseMonkWeapon;
        public uint ShieldID;
        public uint RidingCrop;
        public uint HeadID;
        public uint RightWeapon;
        public uint LeftWeapon;
        public uint SteedPlusPorgres;
        public int rangeR;
        public int rangeL;
        public int SizeAdd;
        public int SpeedR;
        public int SpeedL;
        public int SpeedRing;
        public bool SuperDragonGem;
        public bool SuperPheonixGem;
        public bool SuperVioletGem;
        public bool SuperRaibowGem;
        public bool SuperMoonGem;
        public bool SuprtTortoiseGem;
        public bool HaveBless;
        public bool Alternante;
        private GameClient Owner;
        public MsgGameItem[] CurentEquip = new MsgGameItem[0];
        public MsgGameItem TempGarment;
        public MsgGameItem TempLeftWeapon;
        public MsgGameItem TempRightWeapon;
        public bool FullSuper
        {
            get
            {
                if (!this.SuperArmor)
                    return false;
                foreach (MsgGameItem msgGameItem in this.CurentEquip)
                {
                    if (msgGameItem.Position != (ushort)12 && msgGameItem.Position != (ushort)9 && msgGameItem.Position != (ushort)7 && msgGameItem.Position != (ushort)27 && msgGameItem.Position != (ushort)29 && msgGameItem.Position != (ushort)17 && msgGameItem.Position != (ushort)15 && msgGameItem.Position != (ushort)16 && msgGameItem.ITEM_ID % 10U != 9U)
                        return false;
                }
                return true;
            }
        }

        public byte SteedPlus => (byte)this.Owner.Player.SteedPlus;

        //public int AttackSpeed(int MS_Delay)
        //{
        //    MS_Delay = Math.Max(300, MS_Delay - 100);
        //    MS_Delay = Math.Max(300, MS_Delay - this.SpeedR);
        //    MS_Delay = Math.Max(300, MS_Delay - this.SpeedL);
        //    MS_Delay = Math.Max(300, MS_Delay - this.SpeedRing);
        //    MS_Delay = Math.Max(300, MS_Delay - (int)this.Owner.Player.Agility / 2);
        //    if (this.Owner.Player.ContainFlag(MsgUpdate.Flags.Cyclone))
        //        MS_Delay = Math.Max(300, MS_Delay - 150);
        //    return MS_Delay;
        //}

        public int AttackSpeed(bool physical)
        {
            int speed = 800;
            speed = Math.Max(300, speed - SpeedR);
            speed = Math.Max(300, speed - SpeedL);
            speed = Math.Max(300, speed - SpeedRing);
            speed = Math.Max(300, speed - Owner.Player.Agility / 2);

            if (Owner.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45 && Owner.Player.Agility >= 420)
            {
                speed = Math.Max(150, speed - 300);
            }
            if (Owner.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                speed = Math.Max(150, speed - 300);

            return speed;
        }
        //public int AttackSpeed(bool physical)
        //{
        //    int num = Math.Max(300, Math.Max(300, Math.Max(300, Math.Max(300, 800 - this.SpeedR) - this.SpeedL) - this.SpeedRing) - (int)this.Owner.Player.Agility / 2);
        //    if (this.Owner.Player.ContainFlag(MsgUpdate.Flags.Cyclone))
        //        num = Math.Max(300, num - 150);
        //    return num;
        //}

        public int GetAttackRange(int targetSizeAdd)
        {
            int num = 1;
            if (this.rangeR != 0 && this.rangeL != 0)
                num = (this.rangeR + this.rangeL) / 2;
            else if (this.rangeR != 0)
                num = this.rangeR;
            else if (this.rangeL != 0)
                num = this.rangeL;
            return num + (this.SizeAdd + targetSizeAdd + 1) / 2;
        }

        public Equip(GameClient client) => this.Owner = client;

        public bool Add(
          Packet stream,
          uint ID,
          TheChosenProject.Role.Flags.ConquerItem position,
          byte plus = 0,
          byte bless = 0,
          byte Enchant = 0,
          TheChosenProject.Role.Flags.Gem sockone = TheChosenProject.Role.Flags.Gem.NoSocket,
          TheChosenProject.Role.Flags.Gem socktwo = TheChosenProject.Role.Flags.Gem.NoSocket,
          bool bound = false,
          TheChosenProject.Role.Flags.ItemEffect Effect = TheChosenProject.Role.Flags.ItemEffect.None)
        {
            ItemType.DBItem dbItem;
            if (!this.FreeEquip(position) || !Server.ItemsBase.TryGetValue(ID, out dbItem))
                return false;
            MsgGameItem ItemDat = new MsgGameItem()
            {
                UID = Server.ITEM_Counter.Next,
                ITEM_ID = ID,
                Effect = Effect
            };
            ItemDat.Durability = ItemDat.MaximDurability = dbItem.Durability;
            ItemDat.Plus = plus;
            ItemDat.Bless = bless;
            ItemDat.Enchant = Enchant;
            ItemDat.SocketOne = sockone;
            ItemDat.SocketTwo = socktwo;
            ItemDat.Color = (TheChosenProject.Role.Flags.Color)ServerKernel.NextAsync(3, 9);
            ItemDat.Bound = bound ? (byte)1 : (byte)0;
            this.CheakUp(ItemDat);
            ItemDat.Position = (ushort)position;
            ItemDat.Mode = TheChosenProject.Role.Flags.ItemMode.AddItem;
            ItemDat.Send(this.Owner, stream);
            this.Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, ItemDat.UID, (ulong)ItemDat.Position, 0U, 0U, 0U, 0U));
            return true;
        }

        private void CheakUp(MsgGameItem ItemDat)
        {
            if (ItemDat.UID == 0U)
                ItemDat.UID = Server.ITEM_Counter.Next;
            if (this.ClientItems.TryAdd(ItemDat.UID, ItemDat))
                return;
            do
            {
                ItemDat.UID = Server.ITEM_Counter.Next;
            }
            while (!this.ClientItems.TryAdd(ItemDat.UID, ItemDat));
        }

        public bool Exist(Func<MsgGameItem, bool> predicate)
        {
            bool flag = false;
            foreach (MsgGameItem msgGameItem in this.CurentEquip)
            {
                if (predicate(msgGameItem))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public void Have(Func<MsgGameItem, bool> predicate, out int count)
        {
            count = 0;
            foreach (MsgGameItem msgGameItem in this.CurentEquip)
            {
                if (predicate(msgGameItem))
                    ++count;
            }
        }

        public bool Exist(Func<MsgGameItem, bool> predicate, int count)
        {
            int num = 0;
            foreach (MsgGameItem msgGameItem in this.CurentEquip)
            {
                if (predicate(msgGameItem))
                    ++num;
            }
            return num >= count;
        }

        public ICollection<MsgGameItem> AllItems => this.ClientItems.Values;

        public bool TryGetValue(uint UID, out MsgGameItem itemdata)
        {
            return this.ClientItems.TryGetValue(UID, out itemdata);
        }

        public bool FreeEquip(TheChosenProject.Role.Flags.ConquerItem position)
        {
            return this.ClientItems.Values.Where<MsgGameItem>((Func<MsgGameItem, bool>)(p => (TheChosenProject.Role.Flags.ConquerItem)p.Position == position)).FirstOrDefault<MsgGameItem>() == null;
        }

        public bool TryGetEquip(TheChosenProject.Role.Flags.ConquerItem position, out MsgGameItem itemdata)
        {
            itemdata = this.ClientItems.Values.Where<MsgGameItem>((Func<MsgGameItem, bool>)(p => (TheChosenProject.Role.Flags.ConquerItem)p.Position == position)).FirstOrDefault<MsgGameItem>();
            return itemdata != null;
        }

        public MsgGameItem TryGetEquip(TheChosenProject.Role.Flags.ConquerItem position)
        {
            return this.ClientItems.Values.Where<MsgGameItem>((Func<MsgGameItem, bool>)(p => (TheChosenProject.Role.Flags.ConquerItem)p.Position == position)).FirstOrDefault<MsgGameItem>();
        }

        public bool Remove(TheChosenProject.Role.Flags.ConquerItem position, Packet stream)
        {
            //if (position == TheChosenProject.Role.Flags.ConquerItem.Steed)
            //    this.Owner.Player.RemoveFlag(MsgUpdate.Flags.Ride);
            if (this.Owner.Player.ContainFlag(MsgUpdate.Flags.Fly))
                this.Owner.Player.RemoveFlag(MsgUpdate.Flags.Fly);
            if (this.FreeEquip(position))
                return false;
            if ((byte)position > (byte)20)
                return this.RemoveAlternante(position, stream);
            bool flag = this.Owner.Inventory.HaveSpace((byte)1);
            if (flag)
            {
                MsgGameItem itemdata;
                if (this.TryGetEquip(position, out itemdata) && this.ClientItems.TryRemove(itemdata.UID, out itemdata))
                {
                    this.Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, (ulong)itemdata.Position, 0U, 0U, 0U, 0U));
                    itemdata.Position = (ushort)0;
                    itemdata.Mode = TheChosenProject.Role.Flags.ItemMode.AddItem;
                    if (itemdata.Fake)
                    {
                        this.Owner.Inventory.Update(itemdata, AddMode.REMOVE, stream);
                    }
                    else this.Owner.Inventory.Update(itemdata, AddMode.MOVE, stream);
                }
            }
            else
                this.Owner.SendSysMesage("Your inventory is full.");
            return flag;
        }

        public bool RemoveAlternante(TheChosenProject.Role.Flags.ConquerItem position, Packet stream)
        {
            bool flag = this.Owner.Inventory.HaveSpace((byte)1);
            if (flag)
            {
                MsgGameItem itemdata;
                if (this.TryGetEquip(position, out itemdata) && this.ClientItems.TryRemove(itemdata.UID, out itemdata))
                {
                    this.Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, (ulong)itemdata.Position, 0U, 0U, 0U, 0U));
                    itemdata.Position = (ushort)0;
                    itemdata.Mode = TheChosenProject.Role.Flags.ItemMode.AddItem;
                    this.Owner.Inventory.Update(itemdata, AddMode.MOVE, stream);
                }
            }
            else
                this.Owner.SendSysMesage("Your inventory is full.");
            return flag;
        }

        public void Add(MsgGameItem item, Packet stream)
        {
            this.CheakUp(item);
            if (item.Position > (ushort)20)
            {
                this.AddAlternante(item, stream);
            }
            else
            {
                this.ClientItems.TryAdd(item.UID, item);
                item.Mode = TheChosenProject.Role.Flags.ItemMode.AddItem;
                item.Send(this.Owner, stream);
            }
        }

        public void AddAlternante(MsgGameItem itemdata, Packet stream)
        {
            this.ClientItems.TryAdd(itemdata.UID, itemdata);
            itemdata.Mode = TheChosenProject.Role.Flags.ItemMode.AddItem;
            this.Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, (ulong)itemdata.Position, 0U, 0U, 0U, 0U));
            itemdata.Send(this.Owner, stream);
        }

        public void Show(Packet stream)
        {
            foreach (MsgGameItem msgGameItem in (IEnumerable<MsgGameItem>)this.ClientItems.Values)
            {
                msgGameItem.Mode = TheChosenProject.Role.Flags.ItemMode.AddItem;
                msgGameItem.Send(this.Owner, stream);
            }
            this.QueryEquipment(this.Alternante);
        }

        public void ClearItemSpawn() => this.Owner.Player.ClearItemsSpawn();

        public void AddSpawn(MsgGameItem DataItem)
        {
            switch ((TheChosenProject.Role.Flags.ConquerItem)DataItem.Position)
            {
                case TheChosenProject.Role.Flags.ConquerItem.Head:
                case TheChosenProject.Role.Flags.ConquerItem.AleternanteHead:
                    this.Owner.Player.HeadId = DataItem.ITEM_ID;
                    this.Owner.Player.ColorHelment = (ushort)DataItem.Color;
                    this.Owner.Player.HeadSoul = DataItem.Purification.PurificationItemID;
                    break;
                case TheChosenProject.Role.Flags.ConquerItem.Armor:
                case TheChosenProject.Role.Flags.ConquerItem.AleternanteArmor:
                    this.Owner.Player.ArmorId = DataItem.ITEM_ID;
                    this.Owner.Player.ColorArmor = (ushort)DataItem.Color;
                    this.Owner.Player.ArmorSoul = DataItem.Purification.PurificationItemID;
                    break;
                case TheChosenProject.Role.Flags.ConquerItem.RightWeapon:
                case TheChosenProject.Role.Flags.ConquerItem.AleternanteRightWeapon:
                    this.Owner.Player.RightWeaponId = DataItem.ITEM_ID;
                    this.Owner.Player.ColorShield = (ushort)DataItem.Color;
                    this.Owner.Player.RightWeapsonSoul = DataItem.Purification.PurificationItemID;
                    break;
                case TheChosenProject.Role.Flags.ConquerItem.LeftWeapon:
                case TheChosenProject.Role.Flags.ConquerItem.AleternanteLeftWeapon:
                    this.Owner.Player.LeftWeaponId = DataItem.ITEM_ID;
                    this.Owner.Player.LeftWeapsonSoul = DataItem.Purification.PurificationItemID;
                    break;
                case TheChosenProject.Role.Flags.ConquerItem.Garment:
                case TheChosenProject.Role.Flags.ConquerItem.AleternanteGarment:
                    this.Owner.Player.GarmentId = DataItem.ITEM_ID;
                    break;
                //case TheChosenProject.Role.Flags.ConquerItem.Steed:
                //    this.Owner.Player.SteedId = DataItem.ITEM_ID;
                //    this.Owner.Player.SteedColor = DataItem.SocketProgress;
                //    this.Owner.Player.SteedPlus = (uint)DataItem.Plus;
                //    this.SteedPlusPorgres = DataItem.PlusProgress;
                //    break;
                case TheChosenProject.Role.Flags.ConquerItem.RightWeaponAccessory:
                    this.Owner.Player.RightWeaponAccessoryId = DataItem.ITEM_ID;
                    break;
                case TheChosenProject.Role.Flags.ConquerItem.LeftWeaponAccessory:
                    this.Owner.Player.LeftWeaponAccessoryId = DataItem.ITEM_ID;
                    break;
                //case TheChosenProject.Role.Flags.ConquerItem.SteedMount:
                //    this.Owner.Player.MountArmorId = DataItem.ITEM_ID;
                //    break;
            }
            if (this.Owner.Player.SpecialGarment != 0U)
                this.Owner.Player.GarmentId = this.Owner.Player.SpecialGarment;
            if (this.Owner.Player.RightSpecialAccessory != 0U)
                this.Owner.Player.RightWeaponAccessoryId = this.Owner.Player.RightSpecialAccessory;
            if (this.Owner.Player.LeftSpecialAccessory == 0U)
                return;
            this.Owner.Player.LeftWeaponAccessoryId = this.Owner.Player.LeftSpecialAccessory;
        }

        public void UpdateStats(MsgGameItem[] MyGear, Packet stream)
        {
            try
            {
                this.SoulsPotency = 0U;
                this.rangeR = this.rangeL = this.SizeAdd = 0;
                this.SpeedR = this.SpeedL = this.SpeedRing = 0;
                this.RightWeapon = 0U;
                this.LeftWeapon = 0U;
                this.UseMonkWeapon = false;
                this.SuperArmor = false;
                this.HeadID = 0U;
                this.WeaponsMinAttack = 0;
                this.HaveBless = false;
                this.RingEffect = TheChosenProject.Role.Flags.ItemEffect.None;
                this.RightWeaponEffect = TheChosenProject.Role.Flags.ItemEffect.None;
                this.LeftWeaponEffect = TheChosenProject.Role.Flags.ItemEffect.None;
                this.SteedPlusPorgres = 0U;
                this.Owner.Status.MaxVigor = (ushort)0;
                this.RidingCrop = 0U;
                if (this.CreateSpawn)
                {
                    lock (this.CurentEquip)
                        this.CurentEquip = MyGear;
                    this.ClearItemSpawn();
                }
                this.Owner.Status = new MsgStatus()
                {
                    UID = this.Owner.Player.UID,
                    MaxAttack = (uint)(ushort)((uint)this.Owner.Player.Strength + 1U),
                    MinAttack = (uint)this.Owner.Player.Strength,
                    MagicAttack = (uint)this.Owner.Player.Spirit
                };
                this.Owner.Gems = new ushort[13];
                foreach (MsgGameItem DataItem in MyGear)
                {
                    if (ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)1 || ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)21)
                        this.HeadID = DataItem.ITEM_ID;
                    try
                    {
                        if (this.CreateSpawn)
                            this.AddSpawn(DataItem);
                        if (DataItem.Durability != (ushort)0)
                        {
                            ushort num = (ushort)((uint)DataItem.Position % 20U);
                            if (DataItem.Bless >= (byte)1)
                                this.HaveBless = true;
                            if (num == (ushort)3)
                            {
                                this.SuperArmor = DataItem.ITEM_ID % 10U == 9U;
                                this.ArmorID = DataItem.ITEM_ID;
                            }
                            if (DataItem.SocketOne != TheChosenProject.Role.Flags.Gem.NoSocket && DataItem.SocketOne != TheChosenProject.Role.Flags.Gem.EmptySocket)
                            {
                                if (DataItem.SocketOne == TheChosenProject.Role.Flags.Gem.SuperTortoiseGem)
                                    this.SuprtTortoiseGem = true;
                                if (DataItem.SocketOne == TheChosenProject.Role.Flags.Gem.SuperDragonGem)
                                    this.SuperDragonGem = true;
                                if (DataItem.SocketOne == TheChosenProject.Role.Flags.Gem.SuperPhoenixGem)
                                    this.SuperPheonixGem = true;
                                if (DataItem.SocketOne == TheChosenProject.Role.Flags.Gem.SuperVioletGem)
                                    this.SuperVioletGem = true;
                                if (DataItem.SocketOne == TheChosenProject.Role.Flags.Gem.SuperRainbowGem)
                                    this.SuperRaibowGem = true;
                                if (DataItem.SocketOne == TheChosenProject.Role.Flags.Gem.SuperMoonGem)
                                    this.SuperMoonGem = true;
                            }
                            if (num == (ushort)18)
                            {
                                this.RidingCrop = DataItem.ITEM_ID;
                                this.Owner.Status.MaxVigor += (ushort)1000;
                            }
                            if (num == (ushort)5)
                            {
                                this.LeftWeapon = DataItem.ITEM_ID;
                                this.LeftWeaponEffect = DataItem.Effect;
                                if (ItemType.IsShield(DataItem.ITEM_ID))
                                    this.ShieldID = DataItem.ITEM_ID;
                                if (ItemType.IsPrayedBead(DataItem.ITEM_ID))
                                    this.UseMonkWeapon = true;
                            }
                            if (num == (ushort)4)
                            {
                                if (ItemType.IsPrayedBead(DataItem.ITEM_ID))
                                    this.UseMonkWeapon = true;
                                this.RightWeaponEffect = DataItem.Effect;
                                this.RightWeapon = DataItem.ITEM_ID;
                            }
                            if (num == (ushort)6)
                                this.RingEffect = DataItem.Effect;
                            if (num == (ushort)2)
                                this.NecklaceEffect = DataItem.Effect;
                            this.AddGem(DataItem.SocketOne);
                            this.AddGem(DataItem.SocketTwo);
                            if (Server.ItemsBase.ContainsKey(DataItem.ITEM_ID))
                            {
                                ItemType.DBItem dbItem1 = Server.ItemsBase[DataItem.ITEM_ID];
                                if (num == (ushort)10)
                                {
                                    this.Owner.Status.PhysicalDamageIncrease += (uint)dbItem1.MaxAttack;
                                    this.Owner.Status.MagicDamageIncrease += (uint)dbItem1.MagicAttack;
                                }
                                else
                                {
                                    if (num == (ushort)6)
                                        this.SpeedRing = (int)dbItem1.Frequency;
                                    if (num == (ushort)5)
                                    {
                                        this.rangeL = (int)dbItem1.AttackRange;
                                        this.SpeedL = (int)dbItem1.Frequency;
                                        this.WeaponsMinAttack += (int)dbItem1.MaxAttack / 2;
                                        this.Owner.Status.MaxAttack += (uint)dbItem1.MaxAttack / 2U;
                                        this.Owner.Status.MinAttack += (uint)dbItem1.MinAttack / 2U;
                                        this.Owner.Status.MagicAttack += (uint)dbItem1.MagicAttack / 2U;
                                    }
                                    else
                                    {
                                        if (num == (ushort)4)
                                        {
                                            this.WeaponsMinAttack += (int)dbItem1.MinAttack;
                                            this.rangeR = (int)dbItem1.AttackRange;
                                            this.SpeedR = (int)dbItem1.Frequency;
                                        }
                                        this.Owner.Status.MaxAttack += (uint)dbItem1.MaxAttack;
                                        this.Owner.Status.MinAttack += (uint)dbItem1.MinAttack;
                                        this.Owner.Status.MagicAttack += (uint)dbItem1.MagicAttack;
                                    }
                                }
                                if (num == (ushort)11)
                                {
                                    this.Owner.Status.MagicDamageDecrease += (uint)dbItem1.MagicDefence;
                                    this.Owner.Status.PhysicalDamageDecrease += (uint)dbItem1.PhysicalDefence;
                                }
                                else
                                {
                                    this.Owner.Status.Immunity += dbItem1.Imunity;
                                    this.Owner.Status.CriticalStrike += dbItem1.Crytical;
                                    this.Owner.Status.SkillCStrike += dbItem1.SCrytical;
                                    this.Owner.Status.Breakthrough += dbItem1.BreackTrough;
                                    this.Owner.Status.Counteraction += dbItem1.ConterAction;
                                    this.Owner.Status.MDefence += (uint)(byte)dbItem1.MagicDefence;
                                    this.Owner.Status.Defence += (uint)dbItem1.PhysicalDefence;
                                }
                                if (num != (ushort)12)
                                {
                                    this.Owner.Status.Dodge += (uint)dbItem1.Dodge;
                                    this.Owner.Status.AgilityAtack += (uint)dbItem1.Frequency;
                                    this.Owner.Status.ItemBless += (uint)DataItem.Bless;
                                    this.Owner.Status.MaxHitpoints += (uint)DataItem.Enchant;
                                }
                                this.Owner.Status.MaxHitpoints += (uint)dbItem1.ItemHP;
                                this.Owner.Status.MaxMana += (uint)dbItem1.ItemMP;
                                if (DataItem.Purification.InLife)
                                {
                                    ItemType.DBItem dbItem2 = Server.ItemsBase[DataItem.Purification.PurificationItemID];
                                    this.Owner.Status.MaxAttack += (uint)dbItem2.MaxAttack;
                                    this.Owner.Status.MinAttack += (uint)dbItem2.MinAttack;
                                    this.Owner.Status.MagicAttack += (uint)dbItem2.MagicAttack;
                                    this.Owner.Status.MDefence += (uint)(byte)dbItem2.MagicDefence;
                                    this.Owner.Status.Defence += (uint)dbItem2.PhysicalDefence;
                                    this.Owner.Status.CriticalStrike += dbItem2.Crytical;
                                    this.Owner.Status.SkillCStrike += dbItem2.SCrytical;
                                    this.Owner.Status.Immunity += dbItem2.Imunity;
                                    this.Owner.Status.Penetration += dbItem2.Penetration;
                                    this.Owner.Status.Block += dbItem2.Block;
                                    this.Owner.Status.Breakthrough += dbItem2.BreackTrough;
                                    this.Owner.Status.Counteraction += dbItem2.ConterAction;
                                    this.Owner.Status.Detoxication += dbItem2.Detoxication;
                                    this.Owner.Status.MetalResistance += dbItem2.MetalResistance;
                                    this.Owner.Status.WoodResistance += dbItem2.WoodResistance;
                                    this.Owner.Status.FireResistance += dbItem2.FireResistance;
                                    this.Owner.Status.EarthResistance += dbItem2.EarthResistance;
                                    this.Owner.Status.WaterResistance += dbItem2.WaterResistance;
                                    this.Owner.Status.MaxHitpoints += (uint)dbItem2.ItemHP;
                                    this.Owner.Status.MaxMana += (uint)dbItem2.ItemMP;
                                    this.SoulsPotency += DataItem.Purification.PurificationLevel;
                                }
                                if (DataItem.Refinary.InLife)
                                    this.IncreaseRifainaryStatus(DataItem.Refinary.EffectID);
                                if (DataItem.Plus > (byte)0)
                                {
                                    ItemType.ITPlus plu = dbItem1.Plus[(int)DataItem.Plus];
                                    if (plu != null)
                                    {
                                        if (num == (ushort)5 || num == (ushort)4)
                                            this.Owner.Status.Accuracy += (uint)plu.Agility;
                                        if (num == (ushort)12)
                                            this.Owner.Status.MaxVigor = plu.Agility;
                                        if (num == (ushort)10)
                                        {
                                            this.Owner.Status.PhysicalDamageIncrease += plu.MaxAttack;
                                            this.Owner.Status.MagicDamageIncrease += (uint)plu.MagicAttack;
                                        }
                                        else
                                        {
                                            this.Owner.Status.MaxAttack += plu.MaxAttack;
                                            this.Owner.Status.MinAttack += plu.MinAttack;
                                            this.Owner.Status.MagicAttack += (uint)plu.MagicAttack;
                                        }
                                        if (num == (ushort)11)
                                        {
                                            this.Owner.Status.MagicDamageDecrease += (uint)plu.MagicDefence;
                                            this.Owner.Status.PhysicalDamageDecrease += (uint)plu.PhysicalDefence;
                                        }
                                        else
                                        {
                                            this.Owner.Status.MagicDefence += (uint)plu.MagicDefence;
                                            this.Owner.Status.Defence += (uint)plu.PhysicalDefence;
                                        }
                                        if (num != (ushort)12)
                                            this.Owner.Status.Dodge += (uint)plu.Dodge;
                                        this.Owner.Status.MaxHitpoints += (uint)plu.ItemHP;
                                    }
                                    else
                                        ServerKernel.Log.SaveLog("Invalid Plus -> item " + DataItem.ITEM_ID.ToString() + " ->  plus " + DataItem.Plus.ToString(), true, LogType.WARNING);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
                    }
                }
                ++this.Owner.Status.MagicAttack;
                this.Owner.Status.MagicDefence += this.Owner.GemValues(TheChosenProject.Role.Flags.Gem.NormalGloryGem);
                this.Owner.Status.PhysicalDamageDecrease += this.Owner.GemValues(TheChosenProject.Role.Flags.Gem.NormalGloryGem);
                this.Owner.Status.PhysicalDamageIncrease += this.Owner.GemValues(TheChosenProject.Role.Flags.Gem.NormalThunderGem);
                this.Owner.Status.MagicDamageIncrease += this.Owner.GemValues(TheChosenProject.Role.Flags.Gem.NormalThunderGem);
                this.Owner.Status.MaxHitpoints += (uint)this.Owner.CalculateHitPoint();
                this.Owner.Status.MaxMana += (uint)this.Owner.CalculateMana();
                this.Owner.Vigor = (uint)(ushort)Math.Min((int)this.Owner.Vigor, (int)this.Owner.Status.MaxVigor);
                if (this.CreateSpawn)
                    this.Owner.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, this.Owner.Vigor));
                this.CalculateBattlePower();
                if (this.CreateSpawn)
                    this.Owner.Player.View.SendView(this.Owner.Player.GetArray(stream, false), false);
                this.Owner.Player.CheckAura();
                //this.Owner.Player.SubClass.UpdateStatus(this.Owner);
                this.Owner.Send(stream.StatusCreate(this.Owner.Status));
                this.Owner.Status.Damage = this.Owner.GemValues(TheChosenProject.Role.Flags.Gem.SuperTortoiseGem);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
            }
        }
        //public unsafe void UpdateStats(Game.MsgServer.MsgGameItem[] MyGear, ServerSockets.Packet stream)
        //{
        //    try
        //    {
        //        SoulsPotency = 0;
        //        rangeR = rangeL = SizeAdd = 0;
        //        SpeedR = SpeedL = SpeedRing = 0;
        //        RightWeapon = 0;
        //        LeftWeapon = 0;
        //        SuperArmor = false;
        //        HeadID = 0;
        //        WeaponsMinAttack = 0;
        //        HaveBless = false;
        //        RingEffect = Flags.ItemEffect.None;
        //        RightWeaponEffect = Flags.ItemEffect.None;
        //        LeftWeaponEffect = Flags.ItemEffect.None;
        //        SteedPlusPorgres = 0;
        //        Owner.Status.MaxVigor = 0;
        //        SuprtTortoiseGem = false;
        //        SuperDragonGem = false;
        //        SuperPheonixGem = false;
        //        SuperVioletGem = false;
        //        SuperRaibowGem = false;
        //        SuperMoonGem = false;
        //        RidingCrop = 0;
        //        if (CreateSpawn)
        //        {
        //            lock (CurentEquip)
        //                CurentEquip = MyGear;
        //            ClearItemSpawn();
        //        }
        //        Owner.Status = new MsgStatus();
        //        Owner.Status.UID = Owner.Player.UID;

        //        Owner.Status.MaxAttack = (ushort)(Owner.Player.Strength + 1);
        //        Owner.Status.MinAttack = (ushort)(Owner.Player.Strength);
        //        Owner.Status.MagicAttack = Owner.Player.Spirit;// (ushort)(Owner.Player.Spirit * 10);

        //        Owner.Gems = new ushort[13];

        //        foreach (var item in MyGear)
        //        {
        //            if (Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Head
        //                || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.AleternanteHead)
        //                HeadID = item.ITEM_ID;


        //            //aici sa fac schimbare de position !!!!!!!!!!!
        //            try
        //            {
        //                if (CreateSpawn)
        //                    AddSpawn(item);

        //                if (item.Durability == 0)
        //                    continue;

        //                ushort ItemPostion = (ushort)(item.Position % 20);

        //                if (item.Bless >= 1)
        //                    HaveBless = true;
        //                if (ItemPostion == (ushort)Role.Flags.ConquerItem.Armor)
        //                {
        //                    SuperArmor = (item.ITEM_ID % 10) == 9;

        //                    ArmorID = item.ITEM_ID;
        //                }
        //                if (item.SocketOne != Role.Flags.Gem.NoSocket && item.SocketOne != Role.Flags.Gem.EmptySocket)
        //                {
        //                    if (item.SocketOne == Role.Flags.Gem.SuperTortoiseGem)
        //                        SuprtTortoiseGem = true;
        //                    if (item.SocketOne == Role.Flags.Gem.SuperDragonGem)
        //                        SuperDragonGem = true;
        //                    if (item.SocketOne == Role.Flags.Gem.SuperPhoenixGem)
        //                        SuperPheonixGem = true;
        //                    if (item.SocketOne == Role.Flags.Gem.SuperVioletGem)
        //                        SuperVioletGem = true;
        //                    if (item.SocketOne == Role.Flags.Gem.SuperRainbowGem)
        //                        SuperRaibowGem = true;
        //                    if (item.SocketOne == Role.Flags.Gem.SuperMoonGem)
        //                        SuperMoonGem = true;
        //                }

        //                if (ItemPostion == (ushort)Role.Flags.ConquerItem.RidingCrop)
        //                {
        //                    RidingCrop = item.ITEM_ID;
        //                    Owner.Status.MaxVigor += 1000;
        //                }
        //                if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon)
        //                {
        //                    LeftWeapon = item.ITEM_ID;
        //                    LeftWeaponEffect = item.Effect;
        //                    if (Database.ItemType.IsShield(item.ITEM_ID))
        //                        ShieldID = item.ITEM_ID;
        //                }
        //                if (ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
        //                {
        //                    RightWeaponEffect = item.Effect;
        //                    RightWeapon = item.ITEM_ID;
        //                }

        //                if (ItemPostion == (ushort)Role.Flags.ConquerItem.Ring)
        //                    RingEffect = item.Effect;

        //                if (ItemPostion == (ushort)Role.Flags.ConquerItem.Necklace)
        //                    NecklaceEffect = item.Effect;

        //                AddGem(item.SocketOne);
        //                AddGem(item.SocketTwo);

        //                if (!Server.ItemsBase.ContainsKey(item.ITEM_ID))
        //                    continue;

        //                var DBItem = Server.ItemsBase[item.ITEM_ID];

        //                if (ItemPostion == (byte)Role.Flags.ConquerItem.Fan)
        //                {
        //                    Owner.Status.PhysicalDamageIncrease += DBItem.MaxAttack;
        //                    Owner.Status.MagicDamageIncrease += DBItem.MagicAttack;
        //                }
        //                else
        //                {
        //                    if (ItemPostion == (ushort)Role.Flags.ConquerItem.Ring)
        //                    {
        //                        SpeedRing = DBItem.Frequency;
        //                    }
        //                    if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon)
        //                    {
        //                        rangeL = DBItem.AttackRange;
        //                        SpeedL = DBItem.Frequency;
        //                        WeaponsMinAttack += (int)(DBItem.MaxAttack / 2);
        //                        Owner.Status.MaxAttack += (uint)(DBItem.MaxAttack / 2);
        //                        Owner.Status.MinAttack += (uint)(DBItem.MinAttack / 2);
        //                        Owner.Status.MagicAttack += (uint)(DBItem.MagicAttack / 2);
        //                    }
        //                    else
        //                    {
        //                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
        //                        {
        //                            WeaponsMinAttack += DBItem.MinAttack;
        //                            rangeR = DBItem.AttackRange;
        //                            SpeedR = DBItem.Frequency;
        //                        }
        //                        Owner.Status.MaxAttack += DBItem.MaxAttack;
        //                        Owner.Status.MinAttack += DBItem.MinAttack;
        //                        Owner.Status.MagicAttack += DBItem.MagicAttack;
        //                    }
        //                }
        //                if (ItemPostion == (byte)Role.Flags.ConquerItem.Tower)
        //                {
        //                    Owner.Status.MagicDamageDecrease += DBItem.MagicDefence;
        //                    Owner.Status.PhysicalDamageDecrease += DBItem.PhysicalDefence;
        //                }
        //                else
        //                {
        //                    Owner.Status.Immunity += DBItem.Imunity;
        //                    Owner.Status.CriticalStrike += DBItem.Crytical;
        //                    Owner.Status.SkillCStrike += DBItem.SCrytical;
        //                    Owner.Status.Breakthrough += DBItem.BreackTrough;
        //                    Owner.Status.Counteraction += DBItem.ConterAction;
        //                    Owner.Status.MDefence += (byte)DBItem.MagicDefence;
        //                    Owner.Status.Defence += DBItem.PhysicalDefence;
        //                }

        //                if (ItemPostion != (byte)Role.Flags.ConquerItem.Steed)
        //                {
        //                    Owner.Status.Dodge += DBItem.Dodge;
        //                    Owner.Status.AgilityAtack += DBItem.Frequency;
        //                    Owner.Status.ItemBless += item.Bless;
        //                    Owner.Status.MaxHitpoints += item.Enchant;
        //                }
        //                Owner.Status.MaxHitpoints += DBItem.ItemHP;
        //                Owner.Status.MaxMana += DBItem.ItemMP;




        //                //if (item.Purification.InLife)
        //                //{
        //                //    var purificare = Server.ItemsBase[item.Purification.PurificationItemID];
        //                //    Owner.Status.MaxAttack += purificare.MaxAttack;
        //                //    Owner.Status.MinAttack += purificare.MinAttack;
        //                //    Owner.Status.MagicAttack += purificare.MagicAttack;
        //                //    Owner.Status.MDefence += (byte)purificare.MagicDefence;
        //                //    Owner.Status.Defence += purificare.PhysicalDefence;

        //                //    Owner.Status.CriticalStrike += purificare.Crytical;
        //                //    Owner.Status.SkillCStrike += purificare.SCrytical;
        //                //    Owner.Status.Immunity += purificare.Imunity;
        //                //    Owner.Status.Penetration += purificare.Penetration;
        //                //    Owner.Status.Block += purificare.Block;
        //                //    Owner.Status.Breakthrough += purificare.BreackTrough;
        //                //    Owner.Status.Counteraction += purificare.ConterAction;
        //                //    Owner.Status.Detoxication += purificare.Detoxication;

        //                //    Owner.Status.MetalResistance += purificare.MetalResistance;
        //                //    Owner.Status.WoodResistance += purificare.WoodResistance;
        //                //    Owner.Status.FireResistance += purificare.FireResistance;
        //                //    Owner.Status.EarthResistance += purificare.EarthResistance;
        //                //    Owner.Status.WaterResistance += purificare.WaterResistance;

        //                //    Owner.Status.MaxHitpoints += purificare.ItemHP;
        //                //    Owner.Status.MaxMana += purificare.ItemMP;
        //                //    SoulsPotency += item.Purification.PurificationLevel;
        //                //}

        //                if (item.Refinary.InLife)
        //                {
        //                    IncreaseRifainaryStatus(item.Refinary.EffectID);
        //                }
        //                if (item.Plus > 0)
        //                {
        //                    var extraitematributes = DBItem.Plus[item.Plus];
        //                    if (extraitematributes != null)
        //                    {
        //                        if (ItemPostion == (ushort)Role.Flags.ConquerItem.LeftWeapon || ItemPostion == (ushort)Role.Flags.ConquerItem.RightWeapon)
        //                        {
        //                            Owner.Status.Accuracy += extraitematributes.Agility;
        //                        }
        //                        if (ItemPostion == (byte)Role.Flags.ConquerItem.Steed)
        //                            Owner.Status.MaxVigor = extraitematributes.Agility;
        //                        if (ItemPostion == (byte)Role.Flags.ConquerItem.Fan)
        //                        {
        //                            Owner.Status.PhysicalDamageIncrease += extraitematributes.MaxAttack;
        //                            Owner.Status.MagicDamageIncrease += extraitematributes.MagicAttack;
        //                        }
        //                        else
        //                        {
        //                            Owner.Status.MaxAttack += extraitematributes.MaxAttack;
        //                            Owner.Status.MinAttack += extraitematributes.MinAttack;
        //                            Owner.Status.MagicAttack += extraitematributes.MagicAttack;
        //                        }
        //                        if (ItemPostion == (byte)Role.Flags.ConquerItem.Tower)
        //                        {
        //                            Owner.Status.MagicDamageDecrease += extraitematributes.MagicDefence;
        //                            Owner.Status.PhysicalDamageDecrease += extraitematributes.PhysicalDefence;
        //                        }
        //                        else
        //                        {
        //                            Owner.Status.MagicDefence += extraitematributes.MagicDefence;
        //                            Owner.Status.Defence += extraitematributes.PhysicalDefence;
        //                        }
        //                        if (ItemPostion != (byte)Role.Flags.ConquerItem.Steed)
        //                        {
        //                            Owner.Status.Dodge += extraitematributes.Dodge;
        //                        }
        //                        Owner.Status.MaxHitpoints += extraitematributes.ItemHP;
        //                    }
        //                    else
        //                        Console.WriteLine("Invalid Plus -> item " + item.ITEM_ID.ToString() + " ->  plus " + item.Plus.ToString() + "");
        //                }
        //            }
        //            catch (Exception e) { Console.SaveException(e); }
        //        }

        //        if (Owner.Player.NobilityRank == Nobility.NobilityRank.King)
        //        {
        //            Owner.Status.MaxHitpoints += 3500;
        //            if ((Database.AtributesStatus.IsFire(Owner.Player.SecondClass) || Database.AtributesStatus.IsWater(Owner.Player.SecondClass)))
        //            {
        //                Owner.Status.MagicAttack += 800;
        //            }
        //            else
        //            {
        //                Owner.Status.MinAttack += 800;
        //                Owner.Status.MaxAttack += 900;
        //            }
        //        }
        //        else if (Owner.Player.NobilityRank == Nobility.NobilityRank.Prince)
        //        {
        //            Owner.Status.MaxHitpoints += 2500;
        //            if ((Database.AtributesStatus.IsFire(Owner.Player.SecondClass) || Database.AtributesStatus.IsWater(Owner.Player.SecondClass)))
        //            {
        //                Owner.Status.MagicAttack += 500;
        //            }
        //            else
        //            {
        //                Owner.Status.MinAttack += 600;
        //                Owner.Status.MaxAttack += 700;
        //            }
        //        }
        //        else if (Owner.Player.NobilityRank == Nobility.NobilityRank.Duke)
        //        {
        //            Owner.Status.MaxHitpoints += 1500;
        //            if ((Database.AtributesStatus.IsFire(Owner.Player.SecondClass) || Database.AtributesStatus.IsWater(Owner.Player.SecondClass)))
        //            {
        //                Owner.Status.MagicAttack += 300;
        //            }
        //            else
        //            {
        //                Owner.Status.MinAttack += 300;
        //                Owner.Status.MaxAttack += 400;
        //            }
        //        }


        //        if (Owner.Player.GuildBattlePower >= 15)
        //        {
        //            Owner.Status.MinAttack += 1000;
        //            Owner.Status.MaxAttack += 1200;
        //        }
        //        else if (Owner.Player.GuildBattlePower >= 10)
        //        {
        //            Owner.Status.MinAttack += 600;
        //            Owner.Status.MaxAttack += 800;
        //        }
        //        else if (Owner.Player.GuildBattlePower >= 7)
        //        {
        //            Owner.Status.MinAttack += 300;
        //            Owner.Status.MaxAttack += 400;
        //        }

        //        //add gem stats
        //        Owner.Status.PhysicalDamageDecrease += Owner.GemValues(Role.Flags.Gem.NormalGloryGem);
        //        Owner.Status.MagicDamageDecrease += Owner.GemValues(Role.Flags.Gem.NormalGloryGem);
        //        Owner.Status.PhysicalDamageIncrease += Owner.GemValues(Role.Flags.Gem.NormalThunderGem);
        //        Owner.Status.MagicDamageIncrease += Owner.GemValues(Role.Flags.Gem.NormalThunderGem);


        //        Owner.Status.MaxHitpoints += Owner.CalculateHitPoint();
        //        Owner.Status.MaxMana += Owner.CalculateMana();

        //        Owner.Vigor = (ushort)Math.Min((int)Owner.Vigor, (int)Owner.Status.MaxVigor);
        //        if (CreateSpawn)
        //            Owner.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, Owner.Vigor));

        //        CalculateBattlePower();

        //        if (CreateSpawn)
        //            Owner.Player.View.SendView(Owner.Player.GetArray(stream, false), false);

        //        Owner.Player.CheckAura();
        //        //if (Owner.Player.SubClass != null)
        //        //    Owner.Player.SubClass.UpdateStatus(Owner);

        //        Owner.Send(stream.StatusCreate(Owner.Status));

        //        Owner.Status.Damage = Owner.GemValues(Flags.Gem.SuperTortoiseGem);
        //    }
        //    catch (Exception e) { Console.SaveException(e); }
        //}

        public void IncreaseRifainaryStatus(uint ID)
        {
            Rifinery.Item obj;
            if (Server.RifineryItems.TryGetValue(ID, out obj))
            {
                switch (obj.Type)
                {
                    case Rifinery.RefineryType.MDefence:
                        this.Owner.Status.PhysicalDamageDecrease += obj.Procent;
                        this.Owner.Status.MDefence += obj.Procent;
                        break;
                    case Rifinery.RefineryType.CriticalStrike:
                        this.Owner.Status.CriticalStrike += obj.Procent * 100U;
                        break;
                    case Rifinery.RefineryType.SkillCriticalStrike:
                        this.Owner.Status.SkillCStrike += obj.Procent * 100U;
                        break;
                    case Rifinery.RefineryType.Immunity:
                        this.Owner.Status.Immunity += obj.Procent * 100U;
                        break;
                    case Rifinery.RefineryType.Break:
                        this.Owner.Status.Breakthrough += obj.Procent * 10U;
                        break;
                    case Rifinery.RefineryType.Counteraction:
                        this.Owner.Status.Counteraction += obj.Procent * 10U;
                        break;
                    case Rifinery.RefineryType.Detoxication:
                        this.Owner.Status.Detoxication += obj.Procent;
                        break;
                    case Rifinery.RefineryType.Block:
                        this.Owner.Status.Block += obj.Procent * 100U;
                        break;
                    case Rifinery.RefineryType.Penetration:
                        this.Owner.Status.Penetration += obj.Procent * 100U;
                        break;
                    case Rifinery.RefineryType.Intensification:
                        this.Owner.Status.MaxHitpoints += obj.Procent;
                        break;
                    case Rifinery.RefineryType.FinalMDamage:
                        this.Owner.Status.PhysicalDamageIncrease += obj.Procent;
                        break;
                    case Rifinery.RefineryType.FinalMAttack:
                        this.Owner.Status.MagicDamageIncrease += obj.Procent;
                        break;
                }
                switch (obj.Type2)
                {
                    case Rifinery.RefineryType.MDefence:
                        this.Owner.Status.PhysicalDamageDecrease += obj.Procent2;
                        break;
                    case Rifinery.RefineryType.CriticalStrike:
                        this.Owner.Status.CriticalStrike += obj.Procent2 * 100U;
                        break;
                    case Rifinery.RefineryType.SkillCriticalStrike:
                        this.Owner.Status.SkillCStrike += obj.Procent2 * 100U;
                        break;
                    case Rifinery.RefineryType.Immunity:
                        this.Owner.Status.Immunity += obj.Procent2 * 100U;
                        break;
                    case Rifinery.RefineryType.Break:
                        this.Owner.Status.Breakthrough += obj.Procent2 * 10U;
                        break;
                    case Rifinery.RefineryType.Counteraction:
                        this.Owner.Status.Counteraction += obj.Procent2;
                        break;
                    case Rifinery.RefineryType.Detoxication:
                        this.Owner.Status.Detoxication += obj.Procent2;
                        break;
                    case Rifinery.RefineryType.Block:
                        this.Owner.Status.Block += obj.Procent2;
                        break;
                    case Rifinery.RefineryType.Penetration:
                        this.Owner.Status.Penetration += obj.Procent2 * 100U;
                        break;
                    case Rifinery.RefineryType.Intensification:
                        this.Owner.Status.MaxHitpoints += obj.Procent2;
                        break;
                    case Rifinery.RefineryType.FinalMDamage:
                        this.Owner.Status.PhysicalDamageIncrease += obj.Procent2;
                        break;
                    case Rifinery.RefineryType.FinalMAttack:
                        this.Owner.Status.MagicDamageIncrease += obj.Procent2;
                        break;
                }
            }
            else
                ServerKernel.Log.SaveLog("Error refinery id " + ID.ToString(), true, LogType.WARNING);
        }

        public void AddGem(Flags.Gem gem)
        {
            switch (gem)
            {
                case Role.Flags.Gem.SuperThunderGem:
                case Role.Flags.Gem.SuperGloryGem: Owner.AddGem(gem, 500); break;

                case Role.Flags.Gem.RefinedGloryGem:
                case Role.Flags.Gem.RefinedThunderGem: Owner.AddGem(gem, 300); break;

                case Role.Flags.Gem.NormalGloryGem:
                case Role.Flags.Gem.NormalThunderGem: Owner.AddGem(gem, 100); break;

                case Role.Flags.Gem.NormalPhoenixGem:
                case Role.Flags.Gem.NormalDragonGem: Owner.AddGem(gem, 5); break;

                case Role.Flags.Gem.RefinedPhoenixGem:
                case Role.Flags.Gem.RefinedDragonGem: Owner.AddGem(gem, 10); break;

                case Role.Flags.Gem.SuperPhoenixGem:
                case Role.Flags.Gem.SuperDragonGem: Owner.AddGem(gem, 15); break;

                case Role.Flags.Gem.NormalTortoiseGem: Owner.AddGem(gem, 2); break;//1
                case Role.Flags.Gem.RefinedTortoiseGem: Owner.AddGem(gem, 4); break;//2
                case Role.Flags.Gem.SuperTortoiseGem: Owner.AddGem(gem, 6); break;//3

                case Flags.Gem.NormalRainbowGem: Owner.AddGem(gem, 10); break;
                case Flags.Gem.RefinedRainbowGem: Owner.AddGem(gem, 15); break;
                case Role.Flags.Gem.SuperRainbowGem: Owner.AddGem(gem, 25); break;

                case Flags.Gem.NormalVioletGem: Owner.AddGem(gem, 30); break;
                case Flags.Gem.RefinedVioletGem: Owner.AddGem(gem, 50); break;
                case Flags.Gem.SuperVioletGem: Owner.AddGem(gem, 100); break;

                case Flags.Gem.NormalMoonGem: Owner.AddGem(gem, 15); break;
                case Flags.Gem.RefinedMoonGem: Owner.AddGem(gem, 30); break;
                case Flags.Gem.SuperMoonGem: Owner.AddGem(gem, 50); break;

                case Flags.Gem.NormalFuryGem: Owner.AddGem(gem, 5); break;
                case Flags.Gem.RefinedFuryGem: Owner.AddGem(gem, 10); break;
                case Flags.Gem.SuperFuryGem: Owner.AddGem(gem, 15); break;


            }
            
        }

        //public void CalculateBattlePower()
        //{
        //    this.BattlePower = 0;
        //    int num1 = 0;
        //    foreach (MsgGameItem msgGameItem in this.CurentEquip)
        //    {
        //        if (ItemType.ItemPosition(msgGameItem.ITEM_ID) != (ushort)7 && ItemType.ItemPosition(msgGameItem.ITEM_ID) != (ushort)9 && ItemType.ItemPosition(msgGameItem.ITEM_ID) != (ushort)16 && ItemType.ItemPosition(msgGameItem.ITEM_ID) != (ushort)15 && ItemType.ItemPosition(msgGameItem.ITEM_ID) != (ushort)17)
        //        {
        //            int num2 = 0;
        //            switch ((byte)(msgGameItem.ITEM_ID % 10U))
        //            {
        //                case 6:
        //                    ++num2;
        //                    break;
        //                case 7:
        //                    num2 += 2;
        //                    break;
        //                case 8:
        //                    num2 += 3;
        //                    break;
        //                case 9:
        //                    num2 += 4;
        //                    break;
        //            }
        //            int num3 = num2 + (int)msgGameItem.Plus;
        //            if (msgGameItem.SocketOne != TheChosenProject.Role.Flags.Gem.NoSocket)
        //                ++num3;
        //            if ((byte)((int)msgGameItem.SocketOne % 10 - 3) == (byte)0)
        //                ++num3;
        //            if (msgGameItem.SocketTwo != TheChosenProject.Role.Flags.Gem.NoSocket)
        //                ++num3;
        //            if ((byte)((int)msgGameItem.SocketTwo % 10 - 3) == (byte)0)
        //                ++num3;
        //            if (ItemType.IsBackSword(msgGameItem.ITEM_ID))
        //                num3 *= 2;
        //            else if (ItemType.IsTwoHand(msgGameItem.ITEM_ID) && this.FreeEquip(TheChosenProject.Role.Flags.ConquerItem.LeftWeapon) && this.FreeEquip(TheChosenProject.Role.Flags.ConquerItem.AleternanteLeftWeapon))
        //                num3 += num3;
        //            num1 += num3;
        //        }
        //    }
        //    this.BattlePower = num1;
        //}
        public int BattlePower = 0;

        public void CalculateBattlePower()
        {
            BattlePower = 0;
            int val = 0;
            int val_item = 0;

            foreach (var item in CurentEquip)
            {
                if (Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Bottle
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.Garment
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory
                    || Database.ItemType.ItemPosition(item.ITEM_ID) == (ushort)Role.Flags.ConquerItem.SteedMount)
                    continue;
                
                val_item = 0;
                byte Quality = (byte)(item.ITEM_ID % 10);
                switch (Quality)
                {
                    case 9: val_item += 4; break;
                    case 8: val_item += 3; break;
                    case 7: val_item += 2; break;
                    case 6: val_item += 1; break;
                }
                val_item += item.Plus;

                if (item.SocketOne != Role.Flags.Gem.NoSocket)
                    val_item += 1;
                if ((byte)(((byte)item.SocketOne % 10) - 3) == 0)
                    val_item += 1;
                if (item.SocketTwo != Role.Flags.Gem.NoSocket)
                    val_item += 1;
                if ((byte)(((byte)item.SocketTwo % 10) - 3) == 0)
                    val_item += 1;

                if (Database.ItemType.IsBackSword(item.ITEM_ID))
                {
                    val_item *= 2;
                }
                else if (Database.ItemType.IsTwoHand(item.ITEM_ID) && FreeEquip(Flags.ConquerItem.LeftWeapon) && FreeEquip(Flags.ConquerItem.AleternanteLeftWeapon))
                {
                    val_item += val_item;
                }

                val += val_item;
            }
            BattlePower = val;
        }
        public void OnDequeue()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                try
                {
                    Dictionary<uint, Game.MsgServer.MsgGameItem> statusitens = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
                    foreach (var it in AllItems)
                        if (it.Position < 20)
                            if (!statusitens.ContainsKey(it.Position))
                                statusitens.Add(it.Position, it);
                    if (Alternante)
                    {
                        if (!FreeEquip(Flags.ConquerItem.RightWeapon) && !FreeEquip(Flags.ConquerItem.LeftWeapon))
                        {
                            MsgGameItem _left;
                            TryGetEquip(Flags.ConquerItem.LeftWeapon, out _left);
                            MsgGameItem _right;
                            TryGetEquip(Flags.ConquerItem.RightWeapon, out _right);
                            if (Database.ItemType.IsArrow(_left.ITEM_ID) && Database.ItemType.IsBow(_right.ITEM_ID))
                            {
                                MsgGameItem _bow;
                                if (TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon, out _bow) && FreeEquip(Flags.ConquerItem.AleternanteLeftWeapon))
                                {
                                    foreach (var it in AllItems)
                                        if (it.Position > 20)
                                        {
                                            if (statusitens.ContainsKey((ushort)(it.Position - 20)))
                                                statusitens.Remove((ushort)(it.Position - 20));
                                            statusitens.Add((ushort)(it.Position - 20), it);
                                        }
                                    if (statusitens.ContainsKey((ushort)Flags.ConquerItem.LeftWeapon))
                                    {
                                        statusitens.Remove((ushort)Flags.ConquerItem.LeftWeapon);
                                    }
                                    goto jmp;
                                }
                            }
                        }
                        foreach (var it in AllItems)
                            if (it.Position > 20)
                            {
                                if (it.Position == (byte)Role.Flags.ConquerItem.AleternanteRightWeapon)
                                {
                                    if (Database.ItemType.IsTwoHand(it.ITEM_ID))
                                    {
                                        if (Database.ItemType.IsBow(it.ITEM_ID) == false)
                                        {
                                            if (statusitens.ContainsKey((ushort)(it.Position - 19)))
                                            {
                                                statusitens.Remove((ushort)(it.Position - 19));
                                                Remove((Role.Flags.ConquerItem)((it.Position - 19)), stream);
                                            }
                                        }
                                    }
                                }
                                if (statusitens.ContainsKey((ushort)(it.Position - 20)))
                                    statusitens.Remove((ushort)(it.Position - 20));
                                statusitens.Add((ushort)(it.Position - 20), it);
                            }
                    }
                jmp:
                    AppendItems(CreateSpawn, statusitens.Values.ToArray(), stream);
                    UpdateStats(statusitens.Values.ToArray(), stream);

                    Owner.Player.HitPoints = Math.Min((int)Owner.Player.HitPoints, (int)Owner.Status.MaxHitpoints);
                    if (Owner.Player.OnTransform && Owner.Player.TransformInfo != null)
                        Owner.Player.TransformInfo.UpdateStatus();
                    else
                        Owner.Player.SendUpdateHP();

                    Owner.ClanShareBP();
                }
                catch (Exception e)
                {
                    Console.SaveException(e);
                }
            }
        }

        //public void OnDequeue()
        //{
        //    using (RecycledPacket recycledPacket = new RecycledPacket())
        //    {
        //        Packet stream = recycledPacket.GetStream();
        //        try
        //        {
        //            Dictionary<uint, MsgGameItem> dictionary = new Dictionary<uint, MsgGameItem>();
        //            foreach (MsgGameItem allItem in (IEnumerable<MsgGameItem>)this.AllItems)
        //            {
        //                if (allItem.Position < (ushort)20 && !dictionary.ContainsKey((uint)allItem.Position))
        //                    dictionary.Add((uint)allItem.Position, allItem);
        //            }
        //            if (this.Alternante)
        //            {
        //                if (!this.FreeEquip(TheChosenProject.Role.Flags.ConquerItem.RightWeapon) && !this.FreeEquip(TheChosenProject.Role.Flags.ConquerItem.LeftWeapon))
        //                {
        //                    MsgGameItem itemdata1;
        //                    this.TryGetEquip(TheChosenProject.Role.Flags.ConquerItem.LeftWeapon, out itemdata1);
        //                    MsgGameItem itemdata2;
        //                    this.TryGetEquip(TheChosenProject.Role.Flags.ConquerItem.RightWeapon, out itemdata2);
        //                    if (ItemType.IsArrow(itemdata1.ITEM_ID) && ItemType.IsBow(itemdata2.ITEM_ID) && this.TryGetEquip(TheChosenProject.Role.Flags.ConquerItem.AleternanteRightWeapon, out MsgGameItem _) && this.FreeEquip(TheChosenProject.Role.Flags.ConquerItem.AleternanteLeftWeapon))
        //                    {
        //                        foreach (MsgGameItem allItem in (IEnumerable<MsgGameItem>)this.AllItems)
        //                        {
        //                            if (allItem.Position > (ushort)20)
        //                            {
        //                                if (dictionary.ContainsKey((uint)(ushort)((uint)allItem.Position - 20U)))
        //                                    dictionary.Remove((uint)(ushort)((uint)allItem.Position - 20U));
        //                                dictionary.Add((uint)(ushort)((uint)allItem.Position - 20U), allItem);
        //                            }
        //                        }
        //                        if (dictionary.ContainsKey(5U))
        //                        {
        //                            dictionary.Remove(5U);
        //                            goto label_37;
        //                        }
        //                        else
        //                            goto label_37;
        //                    }
        //                }
        //                foreach (MsgGameItem allItem in (IEnumerable<MsgGameItem>)this.AllItems)
        //                {
        //                    if (allItem.Position > (ushort)20)
        //                    {
        //                        if (allItem.Position == (ushort)24 && ItemType.IsTwoHand(allItem.ITEM_ID) && !ItemType.IsBow(allItem.ITEM_ID) && dictionary.ContainsKey((uint)(ushort)((uint)allItem.Position - 19U)))
        //                        {
        //                            dictionary.Remove((uint)(ushort)((uint)allItem.Position - 19U));
        //                            this.Remove((TheChosenProject.Role.Flags.ConquerItem)((uint)allItem.Position - 19U), stream);
        //                        }
        //                        if (dictionary.ContainsKey((uint)(ushort)((uint)allItem.Position - 20U)))
        //                            dictionary.Remove((uint)(ushort)((uint)allItem.Position - 20U));
        //                        dictionary.Add((uint)(ushort)((uint)allItem.Position - 20U), allItem);
        //                    }
        //                }
        //            }
        //        label_37:
        //            this.AppendItems(this.CreateSpawn, dictionary.Values.ToArray<MsgGameItem>(), stream);
        //            this.UpdateStats(dictionary.Values.ToArray<MsgGameItem>(), stream);
        //            this.Owner.Player.HitPoints = Math.Min(this.Owner.Player.HitPoints, (int)this.Owner.Status.MaxHitpoints);
        //            if (this.Owner.Player.OnTransform && this.Owner.Player.TransformInfo != null)
        //                this.Owner.Player.TransformInfo.UpdateStatus();
        //            else
        //                this.Owner.Player.SendUpdateHP();
        //            this.Owner.ClanShareBP();
        //        }
        //        catch (Exception ex)
        //        {
        //            ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
        //        }
        //    }
        //}

        public void AppendItems(bool CreateSpawn, MsgGameItem[] Items, Packet stream)
        {
            MsgShowEquipment ShowEquip;
            ShowEquip = new MsgShowEquipment();
            ShowEquip.wParam = 46;
            ShowEquip.Alternante = (byte)(Alternante ? 1 : 0);
            if (!CreateSpawn)
                return;
            foreach (MsgGameItem item in Items)
            {
                if (item != null)
                {
                    switch ((Flags.ConquerItem)item.Position)
                    {
                        case Flags.ConquerItem.Ring:
                        case Flags.ConquerItem.AleternanteRing:
                            ShowEquip.Ring = item.UID;
                            break;
                        case Flags.ConquerItem.Head:
                        case Flags.ConquerItem.AleternanteHead:
                            ShowEquip.Head = item.UID;
                            break;
                        case Flags.ConquerItem.Necklace:
                        case Flags.ConquerItem.AleternanteNecklace:
                            ShowEquip.Necklace = item.UID;
                            break;
                        case Flags.ConquerItem.RightWeapon:
                        case Flags.ConquerItem.AleternanteRightWeapon:
                            ShowEquip.RightWeapon = item.UID;
                            break;
                        case Flags.ConquerItem.LeftWeapon:
                        case Flags.ConquerItem.AleternanteLeftWeapon:
                            ShowEquip.LeftWeapon = item.UID;
                            break;
                        case Flags.ConquerItem.Armor:
                        case Flags.ConquerItem.AleternanteArmor:
                            ShowEquip.Armor = item.UID;
                            break;
                        case Flags.ConquerItem.Boots:
                        case Flags.ConquerItem.AleternanteBoots:
                            ShowEquip.Boots = item.UID;
                            break;
                        case Flags.ConquerItem.Bottle:
                        case Flags.ConquerItem.AleternanteBottle:
                            ShowEquip.Bottle = item.UID;
                            break;
                        case Flags.ConquerItem.SteedMount:
                            ShowEquip.SteedMount = item.UID;
                            break;
                        case Flags.ConquerItem.Garment:
                        case Flags.ConquerItem.AleternanteGarment:
                            ShowEquip.Garment = item.UID;
                            break;
                        case Flags.ConquerItem.RidingCrop:
                            ShowEquip.RidingCrop = item.UID;
                            break;
                        case Flags.ConquerItem.LeftWeaponAccessory:
                            ShowEquip.LeftWeaponAccessory = item.UID;
                            break;
                        case Flags.ConquerItem.RightWeaponAccessory:
                            ShowEquip.RightWeaponAccessory = item.UID;
                            break;
                    }
                }
            }
            if (Owner.Player.SpecialGarment != 0)
                ShowEquip.Garment = 4294967294;
            if (Owner.Player.RightSpecialAccessory != 0)
                ShowEquip.RightWeaponAccessory = 4294967293;
            if (Owner.Player.LeftSpecialAccessory != 0)
                ShowEquip.LeftWeaponAccessory = 4294967292;
            if (TempGarment != null)
                ShowEquip.Garment = TempGarment.UID;
            if (TempLeftWeapon != null)
                ShowEquip.LeftWeapon = TempLeftWeapon.UID;
            if (TempRightWeapon != null)
                ShowEquip.RightWeapon = TempRightWeapon.UID;
            Owner.Send(stream.ShowEquipmentCreate(ShowEquip));
        }

        public unsafe void SendAlowAlternante(ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgShowEquipment ShowEquip = new MsgShowEquipment();
            ShowEquip.wParam = Game.MsgServer.MsgShowEquipment.AlternanteAllow;
            ShowEquip.Alternante = (byte)(Alternante ? 1 : 0);
            Owner.Send(stream.ShowEquipmentCreate(ShowEquip));
        }

        public unsafe void QueryEquipment(bool Alternantes, bool CallItems = true)
        {
            this.Alternante = Alternantes;
            CreateSpawn = CallItems;
            OnDequeue();
        }

        public bool DestoyArrow(Role.Flags.ConquerItem position, ServerSockets.Packet stream)
        {
            if (!FreeEquip(position))
            {
                Game.MsgServer.MsgGameItem itemdata;
                if (TryGetEquip(position, out itemdata))
                {
                    if (!(itemdata.ITEM_ID >= 1050000 && itemdata.ITEM_ID <= 1051000))
                        return false;
                    if (ClientItems.TryRemove(itemdata.UID, out itemdata))
                    {
                        Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, itemdata.UID, itemdata.Position, 0, 0, 0, 0));
                        itemdata.Position = 0;
                        itemdata.Mode = Flags.ItemMode.AddItem;
                        Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveInventory, itemdata.UID, 0, 0, 0, 0, 0));
                    }
                }
            }
            return false;
        }
    }
}
