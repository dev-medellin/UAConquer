using Extensions;
using System.Data;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgMonster
{
    public unsafe class MonsterPet
    {
        public void RemoveThat(Client.GameClient _Owner)
        {

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                ActionQuery action = new ActionQuery()
                {
                    ObjId = this.monster.UID,
                    Type = ActionType.RemoveEntity
                };
                Owner.Player.View.SendView(stream.ActionCreate(&action), true);
                _Owner.Pet = null;
            }
        }
        public Game.MsgMonster.MonsterFamily Family;
        public MonsterRole monster;
        public SobNpc npc;
        public Client.GameClient Owner;

        public Time32 AttackStamp = new Time32();

        public MonsterPet(Role.Player role, string Name, ServerSockets.Packet stream)
        {
            Owner = role.Owner;
            Owner.Pet = this;
            Family = new MonsterFamily();
            Family.SpellId = Server.Pets.ReadUInt16(Name, "SpellID", 0);
            Family.Level = Server.Pets.ReadUInt16(Name, "Level", 0);
            Family.MaxAttack = Server.Pets.ReadInt32(Name, "Attack", 0);
            Family.MinAttack = Family.MaxAttack;
            Family.Mesh = Server.Pets.ReadUInt16(Name, "Mesh", 0);
            Family.MaxHealth = Server.Pets.ReadInt32(Name, "Hitpoints", 0);
            Family.Defense = Server.Pets.ReadUInt16(Name, "Defence", 0);
            Family.Defense2 = Server.Pets.ReadUInt16(Name, "Defence2", 0);
            Family.AttackRange = Server.Pets.ReadSByte(Name, "AttackRange", 0);
            Family.Name = Server.Pets.ReadString(Name, "Name", "ERROR");
            Family.MapID = role.Map;
            monster = new MonsterRole(Family, Family.ID, string.Empty, Owner.Map);
            monster.ObjType = MapObjectType.Monster;
            monster.UID = 700000 + (role.UID - 1000000);
            monster.Name = Family.Name;
            monster.Level = (byte)Family.Level;
            monster.Map = role.Map;
            monster.Mesh = Family.Mesh;
            monster.HitPoints = (uint)Family.MaxHealth;
            monster.X = (ushort)(Owner.Player.X + 1);
            monster.Y = (ushort)(Owner.Player.Y + 1);
            ActionQuery action = new ActionQuery()
            {
                ObjId = monster.UID,
                Type = ActionType.ReviveMonster,
                wParam1 = monster.X,
                wParam2 = monster.Y
            };
            monster.Send(stream.ActionCreate(&action));
            monster.View.SendScreen(monster.GetArray(stream, false), Owner.Map);
            if (monster.HitPoints > 0)
            {
                Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, monster.UID, 2);
                stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Family.MaxHealth);
                stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, monster.HitPoints);
                stream = Upd.GetArray(stream);
                Owner.Send(stream);
            }
            SendView(stream);
        }

        public void SendView(ServerSockets.Packet stream)
        {
            Owner.Player.View.SendView(monster.GetArray(stream, false), true);
        }

        public void Attach(ServerSockets.Packet stream)
        {
            Owner.Pet.monster.GMap.View.EnterMap<MonsterRole>(monster);
        }

        public static sbyte[] XDir = new sbyte[] { 0, -1, -1, -1, 0, 1, 1, 1 };
        public static sbyte[] YDir = new sbyte[] { 1, 1, 0, -1, -1, -1, 0, 1 };
        public static sbyte[] XDir2 = new sbyte[] { 0, -2, -2, -2, 0, 2, 2, 2, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1 };
        public static sbyte[] YDir2 = new sbyte[] { 2, 2, 0, -2, -2, -2, 0, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2 };

        public IMapObj Target { get; set; }

        public bool Move(Flags.ConquerAngle Direction)
        {
            ushort _X = monster.X, _Y = monster.Y;
            monster.Facing = Direction;
            int dir = ((int)Direction) % XDir.Length;
            sbyte xi = XDir[dir], yi = YDir[dir];
            _X = (ushort)(monster.X + xi);
            _Y = (ushort)(monster.Y + yi);
            Core.IncXY((Flags.ConquerAngle)dir, ref _X, ref _Y);
            if (monster.GMap.ValidLocation(_X, _Y) && !monster.GMap.MonsterOnTile(_X, _Y))
            {
                monster.GMap.SetMonsterOnTile(monster.X, monster.Y, false);
                monster.GMap.SetMonsterOnTile(_X, _Y, true);

                monster.GMap.View.MoveTo<Role.IMapObj>(monster, _X, _Y);
                monster.X = _X;
                monster.Y = _Y;
                return true;
            }
            return false;
        }
        public void DeAtach(ServerSockets.Packet stream)
        {
            ActionQuery action = new ActionQuery()
            {
                ObjId = monster.UID,
                Type = ActionType.RemoveEntity,
            };
            Owner.Send(stream.ActionCreate(&action));
            monster.GMap.View.LeaveMap<MonsterRole>(monster);
            Owner.Pet = null;
            Owner.Player.View.SendView(stream.ActionCreate(&action), false);

        }

        public void Attack(uint TagertUID, GameClient client, ServerSockets.Packet stream)
        {
            InteractQuery action = new InteractQuery()
            {
                UID = monster.UID,
                AtkType = MsgAttackPacket.AttackID.Physical
            };
        }
    }
}
