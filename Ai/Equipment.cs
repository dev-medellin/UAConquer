using TheChosenProject.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Ai
{
    public enum EquipmentType
    {
        Necklace,
        Ring,
        Armet,
        Armor,
        OneHander,
        TwoHander
    }
    public class Equipment
    {
        public Object Bot;
        public ServerSockets.Packet stream;
        public Equipment(Object _bot)
        {
            Bot = _bot;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                stream = rec.GetStream();
            }
        }
        public bool check_level(byte levelitem)
        {
            if (Bot.HaveOwner)
                return levelitem >= 120 && levelitem <= 137;
            else return levelitem >= 10 && levelitem < Bot.BEntity.Player.Level;
        }

        public Equipment CopyEquipmentOwner(Client.GameClient gameClient)
        {
            foreach (var item in gameClient.Equipment.ClientItems.Values)
            {
                Bot.BEntity.Equipment.Add(stream, item.ITEM_ID, (Role.Flags.ConquerItem)item.Position, item.Plus, item.Bless, item.Enchant, item.SocketOne, item.SocketTwo);
            }
            return this;
        }
        public Equipment GetRandomEquipment(byte c)
        {
            var all_items = Server.ItemsBase.Values.Where(e => Database.ItemType.EquipPassJobReq(e, Bot.BEntity) == true && (e.ID % 10) == 9).ToArray();
            var armor = all_items.Where(e => Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Armor && check_level(e.Level)).ToArray();
            Armor(armor[Role.Core.Random.Next(0, armor.Length)].ID);
            var head = all_items.Where(e => Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Head && check_level(e.Level)).ToArray();
            Head(head[Role.Core.Random.Next(0, head.Length)].ID);
            var ring = all_items.Where(e => Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Ring && check_level(e.Level)).ToArray();
            Ring(ring[Role.Core.Random.Next(0, ring.Length)].ID);
            var boots = all_items.Where(e => Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Boots && check_level(e.Level)).ToArray();
            Boots(boots[Role.Core.Random.Next(0, boots.Length)].ID);
            var necklace = all_items.Where(e => Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Necklace && check_level(e.Level)).ToArray();
            Necklace(necklace[Role.Core.Random.Next(0, necklace.Length)].ID);
            var r_wep = all_items.Where(e => Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.RightWeapon && check_level(e.Level)).ToArray();
            var l_wep = all_items.Where(e => Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.LeftWeapon && check_level(e.Level)).ToArray();
            if (c >= 40 && c <= 45)
            {
                var wep = r_wep.Where(e => Database.ItemType.IsBow(e.ID)).ToArray();
                RightWeapon(wep[Role.Core.Random.Next(0, wep.Length)].ID);
                wep = null;
            }
            if (c >= 50 && c <= 55)
            {
                var wep = r_wep.Where(e => Database.ItemType.IsKatana(e.ID)).ToArray();
                RightWeapon(wep[Role.Core.Random.Next(0, wep.Length)].ID);
                LeftWeapon(wep[Role.Core.Random.Next(0, wep.Length)].ID);
                wep = null;
            }
            if (c >= 10 && c <= 15)
            {
                var wep = r_wep.Where(e => (e.ID / 1000) == 480 || (e.ID / 1000) == 410 || (e.ID / 1000) == 420).ToArray();
                RightWeapon(wep[Role.Core.Random.Next(0, wep.Length)].ID);
                LeftWeapon(wep[Role.Core.Random.Next(0, wep.Length)].ID);
                wep = null;
            }
            if (c >= 100)
            {
                var wep = r_wep.Where(e => Database.ItemType.IsBackSword(e.ID)).ToArray();
                RightWeapon(wep[Role.Core.Random.Next(0, wep.Length)].ID);
                wep = null;
            }
            if (c >= 20 && c <= 25)
            {
                var wep = r_wep.Where(e => (e.ID / 1000) == 561 || (e.ID / 1000) == 560).ToArray();
                RightWeapon(wep[Role.Core.Random.Next(0, wep.Length)].ID);
                wep = null;
            }
            //if (Bot.BEntity.Player.Level >= 100)
            //{
            //    Tower(202009);
            //    Fan(201009);
            //}
            necklace = boots = ring = head = l_wep = r_wep = armor = all_items = null;
            GC.Collect();
            return this;
        }
        public byte check_plus()
        {
            if (Bot.HaveOwner)
                return 12;
            return (byte)Program.GetRandom.Next(3, 5);
        }

        public Equipment LeftWeapon(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.LeftWeapon, check_plus());
            return this;
        }
        public Equipment RightWeapon(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.RightWeapon, check_plus());
            return this;
        }
        public Equipment RightWeaponAccessory(uint ID)
        {

            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.RightWeaponAccessory);
            return this;
        }
        public Equipment LeftWeaponAccessory(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.LeftWeaponAccessory);
            return this;
        }
        public Equipment Garment(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.Garment);
            return this;
        }
        public Equipment Armor(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.Armor, check_plus());
            return this;
        }
        public Equipment Boots(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.Boots, check_plus());
            return this;
        }
        public Equipment Ring(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.Ring, check_plus());
            return this;
        }
        public Equipment Head(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.Head, check_plus());
            return this;
        }
        public Equipment Necklace(uint ID)
        {
            Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.Necklace, check_plus());
            return this;
        }
        //public Equipment Tower(uint ID)
        //{
        //    Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.Tower, check_plus());
        //    return this;
        //}
        //public Equipment Fan(uint ID)
        //{
        //    Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.Fan, check_plus());
        //    return this;
        //}
        //public Equipment Crop(uint ID)
        //{
        //    Bot.BEntity.Equipment.Add(stream, ID, Role.Flags.ConquerItem.RidingCrop, check_plus());
        //    return this;
        //}
        public Equipment Send()
        {
            Bot.BEntity.Equipment.QueryEquipment(false);
            Bot.BEntity.Player.View.SendView(Bot.BEntity.Player.GetArray(stream, false), false);
            return this;
        }
    }
}
