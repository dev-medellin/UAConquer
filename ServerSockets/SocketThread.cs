using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Extensions.ThreadGroup;
using System.Diagnostics;

namespace TheChosenProject.ServerSockets
{
    public class SocketPoll
    {
        private const int SOCKET_PROCESS_INTERVAL = 20;

        private const int FD_SETSIZE = 2048;

        public static MyList<SecuritySocket> ConnectionPoll = new MyList<SecuritySocket>();

        private static ServerSocket[] Sockets;

        public SocketPoll(string GroupName, params ServerSocket[] _Sockets)
        {
            Sockets = _Sockets;
            ThreadItem ThreadItem;
            ThreadItem = new ThreadItem(20, GroupName, CheckUp);
            ThreadItem.Open();
        }

        public static void CheckUp()
        {
            try
            {
                List<Socket> RecSockets;
                RecSockets = new List<Socket>();
                if (ConnectionPoll.Count <= 0 && Sockets.Length == 0)
                    return;
                Stopwatch timer;
                timer = new Stopwatch();
                timer.Start();
                ServerSocket[] sockets;
                sockets = Sockets;
                foreach (ServerSocket socket in sockets)
                {
                    if (socket != null && socket.IsAlive)
                        RecSockets.Add(socket.GetConnection);
                }
                SecuritySocket[] values;
                values = ConnectionPoll.GetValues();
                foreach (SecuritySocket socket2 in values)
                {
                    if (socket2 != null)
                    {
                        if (socket2.Alive && socket2.Connection.Connected)
                            RecSockets.Add(socket2.Connection);
                        else
                            socket2.Disconnect();
                    }
                }
                SecuritySocket[] values2;
                values2 = ConnectionPoll.GetValues();
                foreach (SecuritySocket socket3 in values2)
                {
                    if (socket3 == null)
                        continue;
                    try
                    {
                        socket3.ReceiveBuffer();
                        socket3.HandlerBuffer();
                    }
                    catch (Exception e3)
                    {
                        ServerKernel.Log.SaveLog(e3.ToString(), false, LogType.EXCEPTION);
                        continue;
                    }
                    try
                    {
                        while (SecuritySocket.TrySend(socket3))
                        {
                        }
                    }
                    catch (Exception e2)
                    {
                        ServerKernel.Log.SaveLog(e2.ToString(), false, LogType.EXCEPTION);
                    }
                }
                ServerSocket[] sockets2;
                sockets2 = Sockets;
                for (int l = 0; l < sockets2.Length; l++)
                {
                    sockets2[l]?.Accept();
                }
                timer.Stop();
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
