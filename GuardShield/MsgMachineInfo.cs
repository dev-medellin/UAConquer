using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMsgGuardShield;

namespace TheChosenProject.GuardShield
{
    public struct MsgMachineInfo
    {
        [PacketAttribute((ushort)MsgGuardShield.PacketIDs.MsgMachineInfo)]
        public unsafe static void Process(Client.GameClient client, ServerSockets.Packet packet)
        {
            packet.Seek(0);
            byte[] bytes = packet.ReadBytes(packet.Size);
            var msg = new MsgGuardShield.MsgMachineInfo(bytes, client.Guard);
            Console.WriteLine(client.Player.Name + "'s [PC]: '" + msg.MachineName + "' [MAC]: '" + msg.MacAddress + "' [HD Serial]: " + msg.HDSerial.ToString());
        }
    }
}