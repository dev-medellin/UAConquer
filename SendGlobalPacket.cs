using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject
{
    public unsafe class SendGlobalPacket
    {
        public unsafe void Enqueue(ServerSockets.Packet data)
        {
            var array = Database.Server.GamePoll.Values.ToArray();
            foreach (var user in Database.Server.GamePoll.Values)
            {
                user.Send(data);
            }
        }
        public void SendMsgToAll(string msg, uint type)
        {
            foreach (var user in Server.GamePoll.Values)
            {
                user.SendSysMesage(msg, (Game.MsgServer.MsgMessage.ChatMode)type);
            }
        }

        public void SendMsgToAll(string msg, Game.MsgServer.MsgMessage.ChatMode type)
        {
            foreach (var user in Server.GamePoll.Values)
            {
                user.SendSysMesage(msg, type);
            }
        }

        public unsafe void EnqueueWithOutChannel(ServerSockets.Packet data)
        {
            var array = Server.GamePoll.Values.ToArray();
            foreach (var user in array)
            {
                user.Send(data);
            }
        }
    }
    public unsafe class SendGlobalWhisper
    {
        public unsafe void Enqueue(string msg, string from)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var array = Server.GamePoll.Values.ToArray();
                foreach (var user in Server.GamePoll.Values)
                {
                    var X = new Game.MsgServer.MsgMessage(msg, user.Player.Name, from, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Whisper);
                    X.Mesh = 21570000;
                    X.MessageUID1 = 2;
                    var x2 = X.GetArray(stream);
                    user.Send(x2);
                }
            }
        }
    }
}
