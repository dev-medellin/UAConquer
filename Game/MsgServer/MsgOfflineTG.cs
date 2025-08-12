using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetOfflineTG(this ServerSockets.Packet stream, out MsgOfflineTG.Mode Mode)
        {
            Mode = (MsgOfflineTG.Mode)stream.ReadUInt32();
        }

        public static Packet OfflineTGCreate(this Packet stream, MsgOfflineTG.Mode Mode)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Write(0);
            stream.Finalize(2044);
            return stream;
        }


    }
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgOfflineTG
    {
        public enum Mode : uint
        {
            OnConfirmation,
            Disconnect,
            UnKnow,
            ReplyToConfirmation,
            ClaimExperience
        }

        public static void OfflineMode(GameClient client)
        {
            bool can_join;
            can_join = false;
            if (client.Player.AutoHunting == AutoStructures.Mode.Enable)
            {
                client.Player.OfflineTraining = MsgOfflineTraining.Mode.Hunting;
                can_join = true;
            }
            else if (client.IsVendor)
            {
                if (client.MyVendor.Items.Count > 0)
                {
                    client.Player.OfflineTraining = MsgOfflineTraining.Mode.Shopping;
                    can_join = true;
                }
            }
            else if (client.Player.Map == 1039)
            {
                if (client.OnAutoAttack)
                {
                    client.Player.OfflineTraining = MsgOfflineTraining.Mode.TrainingGroup;
                    can_join = true;
                }
                else
                    client.CreateBoxDialog("It`ll be available when you are on Auto-attack");
            }
            if (can_join)
            {
                client.Player.JoinOnflineTG = DateTime.Now;
                client.Player.AddFlag(MsgUpdate.Flags.OfflineMode, 2592000, false);
                ServerKernel.Log.SaveLog(string.Format("{0} --> actived offline mode({2}). left: {1}!", client.Player.Name, client.Player.ExpireVip.ToString("d/M/yyyy (H:mm)"), client.Player.OfflineTraining.ToString()), true, LogType.EXCEPTION);
            }
        }

        [Packet(2044)]
        public static void OfflineTG(GameClient client, Packet packet)
        {
            packet.GetOfflineTG(out var Action);
            switch (Action)
            {
                case Mode.OnConfirmation:
                    client.CreateBoxDialog("Sorry but OfflineTG is not available on this Server");
                        //if (!client.Player.Alive)
                        //{
                        //    client.SendSysMesage("You are not alive.");
                        //    break;
                        //}
                        //if (client.Team != null)
                        //{
                        //    client.SendSysMesage("You need to leave team before confirmation offline-mode");
                        //    break;
                        //}
                        //if (client.Player.VipLevel != 6)
                        //{
                        //    client.SendSysMesage("You need to be vip level 6 before confirmation offline-mode");
                        //    break;
                        //}
                        //OfflineMode(client);
                        //client.Send(packet.OfflineTGCreate(Mode.Disconnect));
                    break;
                case Mode.ClaimExperience:
                    {
                        client.CreateBoxDialog("Sorry but Clain Experience is not available on this Server");
                        //TimeSpan T1;
                        //T1 = new TimeSpan(DateTime.Now.Ticks);
                        //TimeSpan T2;
                        //T2 = new TimeSpan(client.Player.JoinOnflineTG.Ticks);
                        //ushort minutes;
                        //minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                        //Math.Min((ushort)900, minutes);
                        //client.ActiveNpc = 4294967247u;
                        //NpcHandler.OfflineCollector(client, packet, 0, "", 0);
                        break;
                    }
            }
        }
    }
}
