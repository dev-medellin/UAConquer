using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetNobility(this ServerSockets.Packet stream, out MsgNobility.NobilityAction mode, out ulong UID, out MsgNobility.DonationTyp donationtyp)
        {
            mode = (MsgNobility.NobilityAction)stream.ReadInt32();//4
            UID = stream.ReadUInt64();//8
            //   stream.SeekForward(4);
            donationtyp = (MsgNobility.DonationTyp)stream.ReadUInt32();

        }
        public static unsafe ServerSockets.Packet NobilityIconCreate(this ServerSockets.Packet stream, Role.Instance.Nobility nobility)
        {
            stream.InitWriter();

            stream.Write((uint)MsgNobility.NobilityAction.Icon);//4
            stream.Write(nobility.UID);//8

            string StrList = "" + nobility.UID + " " + nobility.Donation / 50000 + " " + (byte)nobility.Rank + " " + nobility.Position + "";

            stream.ZeroFill(20);

            stream.Write(StrList);

            stream.Finalize(GamePackets.Nobility);

            return stream;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgNobility
    {
        public enum NobilityAction : uint
        {
            Donate = 1u,
            RankListen,
            Icon,
            NobilityInformarion
        }

        public enum DonationTyp : byte
        {
            Money,
            BoundConquerPoint,
            ConquerPoints
        }

        [Packet(2064)]
        public static void HandlerNobility(GameClient user, Packet stream)
        {
            if (user.InTrade || !user.Player.VerifiedPassword || user.InQualifier() || user.Player.FairbattlePower != 0)
                return;
            bool ByConquerPoints;
            ByConquerPoints = false;
            stream.GetNobility(out var Action, out var UID, out var donationtyp);
            switch (Action)
            {
                case NobilityAction.Donate:
                    {
                        if (user.Player.Map == 22340 || user.Player.Map == 22341 || user.Player.Map == 22342 || user.Player.Map == 22343)
                        {
                            user.CreateBoxDialog("You can't donate in nobility war map");
                            break;
                        }
                        //if (user.Player.Map == 22341)
                        //{
                        //    user.CreateBoxDialog("You can't donate in nobility war map");
                        //    break;
                        //}
                        //if (user.Player.Map == 22342)
                        //{
                        //    user.CreateBoxDialog("You can't donate in nobility war map");
                        //    break;
                        //}
                        //if (user.Player.Map == 22343)
                        //{
                        //    user.CreateBoxDialog("You can't donate in nobility war map");
                        //    break;
                        //}
                        if (UID < 50000)
                            break;
                        uint CP;
                        CP = (uint)(UID / 50000);
                        if (user.Player.Money > (int)UID)
                        {
                            ByConquerPoints = false;
                            donationtyp = DonationTyp.Money;
                        }
                        else if (user.Player.ConquerPoints > CP)
                        {
                            ByConquerPoints = true;
                            donationtyp = DonationTyp.ConquerPoints;
                        }
                        switch (donationtyp)
                        {
                            case DonationTyp.Money:
                                if (!ByConquerPoints && user.Player.Money >= (uint)UID && (uint)UID >= 10000000)
                                {
                                    user.Player.Nobility.Donation += UID;
                                    user.Player.Money -= (int)UID;
                                    user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                    user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                                    Program.NobilityRanking.UpdateRank(user.Player.Nobility);
                                }
                                else
                                {
                                    user.CreateBoxDialog("Donate more than 10,000,000 Conquer Money!");
                                }
                                break;
                            case DonationTyp.ConquerPoints:
                                if (ByConquerPoints && user.Player.ConquerPoints >= CP)
                                {
                                    user.Player.Nobility.Donation += UID;
                                    user.Player.ConquerPoints -= (int)CP;
                                    user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                                    Program.NobilityRanking.UpdateRank(user.Player.Nobility);
                                }
                                break;
                            case DonationTyp.BoundConquerPoint:
                                user.CreateBoxDialog("try donate by conquer points or silver");
                                break;
                        }

                        user.Equipment.QueryEquipment(user.Equipment.Alternante, false);

                        break;
                    }
                //case NobilityAction.Donate:
                //    {
                //        if (UID < 50000)
                //            break;
                //        uint CP;
                //        CP = (uint)(UID / 50000uL);
                //        if (user.Player.Money > (int)UID)
                //        {
                //            ByConquerPoints = false;
                //            donationtyp = DonationTyp.Money;
                //        }
                //        else if (user.Player.ConquerPoints > CP)
                //        {
                //            ByConquerPoints = true;
                //            donationtyp = DonationTyp.ConquerPoints;
                //        }
                //        switch (donationtyp)
                //        {
                //            case DonationTyp.Money:
                //                if (!ByConquerPoints)
                //                {
                //                    user.Player.Money -= (int)(UID);
                //                    user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                //                    user.Player.Nobility.Donation += UID;
                //                    user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                //                    Program.NobilityRanking.UpdateRank(user.Player.Nobility);
                //                }
                //                break;
                //            case DonationTyp.ConquerPoints:
                //                if (ByConquerPoints)
                //                {
                //                    user.Player.ConquerPoints -= (int)(CP);
                //                    user.Player.Nobility.Donation += UID;
                //                    user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                //                    Program.NobilityRanking.UpdateRank(user.Player.Nobility);
                //                }
                //                break;
                //            case DonationTyp.BoundConquerPoint:
                //                user.CreateBoxDialog("try donate by conquer points or silver");
                //                break;
                //        }
                //        break;
                //    }
                case NobilityAction.RankListen:
                    {
                        int displyPage;
                        displyPage = (int)UID;
                        Nobility[] info;
                        info = Program.NobilityRanking.GetArray();
                        try
                        {
                            int offset;
                            offset = displyPage * 10;
                            int count;
                            count = Math.Min(10, Math.Max(0, info.Length - offset));
                            stream.InitWriter();
                            stream.Write(2);
                            stream.Write((ushort)displyPage);
                            int max_show;
                            max_show = (int)Math.Ceiling((double)info.Length * 1.0 / 10.0);
                            stream.Write((ushort)max_show);
                            int count_show;
                            count_show = 50;
                            if (info.Length < 50)
                            {
                                int current;
                                current = info.Length / 10;
                                if (current == displyPage)
                                    count_show = current;
                            }
                            count_show = ((info.Length >= 10) ? (info.Length - offset) : info.Length);
                            stream.Write((ushort)count_show);
                            stream.ZeroFill(18);
                            for (int x = 0; x < count; x++)
                            {
                                if (info.Length > offset + x)
                                {
                                    Nobility element;
                                    element = info[offset + x];
                                    if (element.Position < 50)
                                    {
                                        stream.Write(element.UID);
                                        stream.Write((uint)element.Gender);
                                        stream.Write(element.Mesh);
                                        stream.Write(element.Name, 16);
                                        stream.Write(0);
                                        stream.Write(element.Donation / 50000);
                                        stream.Write((uint)element.Rank);
                                        stream.Write(element.Position);
                                    }
                                }
                            }
                            stream.Finalize(2064);
                            user.Send(stream);
                            break;
                        }
                        catch (Exception e)
                        {
                            ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                            break;
                        }
                    }
                case NobilityAction.NobilityInformarion:
                    stream.InitWriter();
                    stream.Write(4);
                    stream.Write(Program.NobilityRanking.KnightDonation);
                    stream.Write(uint.MaxValue);
                    stream.Write(1);
                    stream.Write(Program.NobilityRanking.KnightDonation);
                    stream.Write(uint.MaxValue);
                    stream.Write(3);
                    stream.Write(Program.NobilityRanking.EarlDonation);
                    stream.Write(uint.MaxValue);
                    stream.Write(5);
                    stream.Write(Program.NobilityRanking.DukeDonation);
                    stream.Write(uint.MaxValue);
                    stream.Write(7);
                    stream.Write(Program.NobilityRanking.PrinceDonation);
                    stream.Write(uint.MaxValue);
                    stream.Write(9);
                    stream.Write(Program.NobilityRanking.KingDonation);
                    stream.Write(uint.MaxValue);
                    stream.Write(12);
                    stream.Finalize(2064);
                    user.Send(stream);
                    break;
                case NobilityAction.Icon:
                    break;
            }
        }
    }
}
