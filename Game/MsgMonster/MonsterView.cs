using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
namespace TheChosenProject.Game.MsgMonster
{
    public class MonsterView
    {
        public const int ViewThreshold = 30; //ServerKernel.ViewThreshold != 0 ? ServerKernel.ViewThreshold :
        private MonsterRole role;

        public MonsterView(MonsterRole _role) => this.role = _role;

        public IEnumerable<IMapObj> Roles(GameMap map, MapObjectType typ)
        {
            return map.View.Roles(typ, (int)this.role.X, (int)this.role.Y, (Predicate<IMapObj>)(p => this.CanSee(p)));
        }

        public IMapObj GetTarget(GameMap map, MapObjectType typ)
        {
            IEnumerable<IMapObj> source = map.View.Roles(typ, (int)this.role.X, (int)this.role.Y, (Predicate<IMapObj>)(p => this.CanSee(p) && p.ObjType == MapObjectType.Player));
            return source.Count<IMapObj>() > 0 ? source.OrderByDescending<IMapObj, uint>((Func<IMapObj, uint>)(p => p.IndexInScreen)).FirstOrDefault<IMapObj>() : (IMapObj)null;
        }

        public void SendScreen(Packet msg, GameMap map)
        {
            foreach (IMapObj role in this.Roles(map, MapObjectType.Player))
                role.Send(msg);
        }

        public bool CanSee(IMapObj obj)
        {
            return (int)obj.Map == (int)this.role.Map && (int)obj.DynamicID == (int)this.role.DynamicID && (int)obj.UID != (int)this.role.UID && MonsterView.GetDistance(obj.X, obj.Y, this.role.X, this.role.Y) <= (short)18;
        }

        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            short num1 = 0;
            short num2 = 0;
            if ((int)X >= (int)X2)
                num1 = (short)((int)X - (int)X2);
            else if ((int)X2 >= (int)X)
                num1 = (short)((int)X2 - (int)X);
            if ((int)Y >= (int)Y2)
                num2 = (short)((int)Y - (int)Y2);
            else if ((int)Y2 >= (int)Y)
                num2 = (short)((int)Y2 - (int)Y);
            return (int)num1 > (int)num2 ? num1 : num2;
        }
    }
}
