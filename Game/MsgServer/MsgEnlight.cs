using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{

    public static unsafe partial class MsgBuilder
    {

        public static unsafe void GetEnlight(this ServerSockets.Packet stream, MsgEnlight* enlight)
        {
            stream.ReadUnsafe(enlight, sizeof(MsgEnlight));

        }

        public static unsafe ServerSockets.Packet EnlightCreate(this ServerSockets.Packet stream, MsgEnlight* enlight)
        {
            stream.InitWriter();

            stream.WriteUnsafe(enlight, sizeof(MsgEnlight));

            stream.Finalize(GamePackets.Enlight);

            return stream;
        }
    }

    public struct MsgEnlight
    {
        public uint TimerStamp;

        public uint Enlighter;

        public uint Enlighted;

        public uint dwParam1;

        public uint dwParam2;

        public uint dwParam3;

        [Packet(1127)]
        private unsafe static void MsgEnlightHandler(GameClient user, Packet stream)
        {
            MsgEnlight EnlightInfo = default(MsgEnlight);
            stream.GetEnlight(&EnlightInfo);
            if (!user.Player.View.TryGetValue(EnlightInfo.Enlighted, out var obj, MapObjectType.Player))
                return;
            Player target;
            target = obj as Player;
            if (user.Player.Enilghten >= 100 && target.EnlightenReceive < 5 && target.Level + 20 < user.Player.Level)
            {
                if (target.EnlightenReceive == 0)
                    target.EnlightenTime = DateTime.Now;
                target.EnlightenReceive++;
                user.Player.Enilghten -= 100;
                user.Player.SendUpdate(stream, user.Player.Enilghten, MsgUpdate.DataType.EnlightPoints);
                target.Owner.GainExpBall(600.0, true, Flags.ExperienceEffect.angelwing);
                user.Player.View.SendView(stream.EnlightCreate(&EnlightInfo), true);
            }
        }
    }
}
