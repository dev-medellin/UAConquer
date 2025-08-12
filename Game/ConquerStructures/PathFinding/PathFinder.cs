using TheChosenProject.Role;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

 
namespace TheChosenProject.Game.ConquerStructures.PathFinding
{
    public class PathFinder
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct Point
        {
            [FieldOffset(0)]
            public int X;

            [FieldOffset(4)]
            public int Y;

            [FieldOffset(0)]
            public long Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct Node
        {
            [FieldOffset(0)]
            public int X;

            [FieldOffset(4)]
            public int Y;

            [FieldOffset(0)]
            public long Data;

            [FieldOffset(8)]
            public int Cost;

            [FieldOffset(12)]
            public bool Processed;
        }

        private static Node default_node = new Node
        {
            Data = -1L,
            Cost = int.MaxValue,
            Processed = false
        };

        private Size size;

        private object SendSyncRoot = new object();

        public PathFinder(Size map)
        {
            size = map;
        }

        public bool Search(Point src, Point dst, byte jumpdistance, GameMap Map, out Point[] path)
        {
            if (!Map.ValidLocation((ushort)dst.X, (ushort)dst.Y) || !Map.ValidLocation((ushort)src.X, (ushort)src.Y))
            {
                path = null;
                return false;
            }
            Node[,] nodes;
            nodes = new Node[size.Width, size.Height];
            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    nodes[x, y] = default_node;
                }
            }
            Queue<Node> queue;
            queue = new Queue<Node>();
            queue.Enqueue(new Node
            {
                X = src.X,
                Y = src.Y,
                Cost = 1,
                Processed = false
            });
            bool found;
            found = false;
            lock (SendSyncRoot)
            {
                while (queue.Count() > 0)
                {
                    Node p;
                    p = queue.Dequeue();
                    if (p.Data == dst.Data)
                    {
                        found = true;
                        break;
                    }
                    for (int i = 0; i < jumpdistance; i++)
                    {
                        int nx;
                        nx = (ushort)ServerKernel.NextAsync(p.X - i, p.X + i);
                        int ny;
                        ny = (ushort)ServerKernel.NextAsync(p.Y - i, p.Y + i);
                        if (Map.ValidLocation((ushort)nx, (ushort)ny))
                        {
                            int cost;
                            cost = p.Cost + 1;
                            if (cost < nodes[nx, ny].Cost)
                            {
                                nodes[nx, ny].Data = p.Data;
                                nodes[nx, ny].Cost = cost;
                            }
                            if (!nodes[nx, ny].Processed)
                            {
                                nodes[nx, ny].Processed = true;
                                queue.Enqueue(new Node
                                {
                                    X = nx,
                                    Y = ny,
                                    Cost = cost
                                });
                            }
                        }
                    }
                }
            }
            if (!found)
            {
                path = null;
                return false;
            }
            Stack<Point> pathStack;
            pathStack = new Stack<Point>();
            Point c;
            c = dst;
            while (c.Data != src.Data)
            {
                pathStack.Push(c);
                c.Data = nodes[c.X, c.Y].Data;
            }
            path = pathStack.ToArray();
            return true;
        }
    }
}
