﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgBroadcast
    {
        public const int MaxBroadcasts = 50;

        public static Extensions.Time32 TimerStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.BroadCastStamp);


        public static Extensions.Counter BroadcastCounter = new Extensions.Counter(1);

        public struct BroadcastStr
        {
            public uint ID;
            public uint EntityID;
            public string EntityName;
            public uint SpentCPs;
            public string Message;
          
        }

        public static DateTime LastBroadcast = DateTime.Now;

        public static BroadcastStr CurrentBroadcast = new BroadcastStr() { EntityID = 1 };

        public static List<BroadcastStr> Broadcasts = new List<BroadcastStr>();

        public static void Create()
        {

        }
        public unsafe static void Work(Extensions.Time32 clock)
        {
            if (clock > TimerStamp)
            {
                DateTime Now = DateTime.Now;

                if (Now > LastBroadcast.AddMinutes(1))
                {
                    if (Broadcasts.Count > 0)
                    {
                        CurrentBroadcast = Broadcasts[0];
                        Broadcasts.Remove(CurrentBroadcast);

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                         
                            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(CurrentBroadcast.Message, "ALLUSERS", CurrentBroadcast.EntityName, MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                        }
                    }
                    else
                        CurrentBroadcast.EntityID = 1;

                    LastBroadcast = DateTime.Now;
                }
                TimerStamp.Value = clock.Value + KernelThread.BroadCastStamp;
            }

        }
   
    }
}
