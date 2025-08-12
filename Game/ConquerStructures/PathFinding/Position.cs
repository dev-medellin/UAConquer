using System;

 
namespace TheChosenProject.Game.ConquerStructures.PathFinding
{
    public class Position
    {
        private static int Counter = 10000;

        private int dyna;

        public bool Dynamic
        {
            get
            {
                return dyna != 0;
            }
            set
            {
                dyna = (value ? PostIncrement() : 0);
            }
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Id { get; set; }

        public int DynamicId => dyna;

        private static int PostIncrement()
        {
            Counter++;
            if (Counter > 500000)
                Counter = 10000;
            return Counter;
        }

        public Position(int[] values)
        {
            Id = values[0];
            X = values[1];
            Y = values[2];
            Dynamic = values.Length >= 4 && values[3] == 1;
        }

        public Position(int id, int x, int y, bool dynamic = false)
        {
            Dynamic = dynamic;
            Id = id;
            X = x;
            Y = y;
        }

        private Position()
        {
        }

        public Position Clone()
        {
            return new Position
            {
                Id = Id,
                X = X,
                Y = Y,
                dyna = dyna
            };
        }

        public override int GetHashCode()
        {
            return (dyna << 16) | Id;
        }

        public override string ToString()
        {
            if (!Dynamic)
                return Id + " - " + X + " " + Y;
            return Id + " (" + dyna + ") - " + X + " " + Y;
        }

        public override bool Equals(object obj)
        {
            Position other;
            other = obj as Position;
            if (other == null)
                return false;
            return this == other;
        }

        public static bool operator ==(Position pthis, Position other)
        {
            if ((object)pthis == null && (object)other == null)
                return true;
            if ((object)pthis == null && (object)other != null)
                return false;
            if ((object)pthis != null && (object)other == null)
                return false;
            if (pthis.dyna == other.dyna && pthis.X == other.X && pthis.Y == other.Y)
                return pthis.Id == other.Id;
            return false;
        }

        public static bool operator !=(Position pthis, Position other)
        {
            return !(pthis == other);
        }

        public static bool operator ==(Position pthis, int id)
        {
            return pthis.Id == id;
        }

        public static bool operator !=(Position pthis, int id)
        {
            return pthis.Id != id;
        }

        private int RoughDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        private int ConquerDirection(int x1, int y1, int x2, int y2)
        {
            double angle;
            angle = Math.Atan2(y2 - y1, x2 - x1);
            angle -= Math.PI / 2.0;
            if (angle < 0.0)
                angle += Math.PI * 2.0;
            angle *= 4.0 / Math.PI;
            return (int)angle;
        }

        public int Distance(int x, int y)
        {
            return RoughDistance(X, Y, x, y);
        }

        public int Distance(Position other)
        {
            if (Id != other.Id || DynamicId != other.DynamicId)
                return int.MaxValue;
            return RoughDistance(X, Y, other.X, other.Y);
        }

        public int Direction(int x, int y)
        {
            return ConquerDirection(X, Y, x, y);
        }

        public int Angle(int x, int y)
        {
            return (int)(Math.Atan2(Y - y, X - x) * 180.0 / Math.PI);
        }

        public static implicit operator PathFinder.Point(Position l)
        {
            PathFinder.Point result;
            result = default(PathFinder.Point);
            result.X = l.X;
            result.Y = l.Y;
            return result;
        }
    }
}
