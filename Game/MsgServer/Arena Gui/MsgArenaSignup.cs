using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.ServerSockets;
using TheChosenProject.Role;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaSignupCreate(this ServerSockets.Packet stream, MsgArenaSignup.DialogType DialogID
            , MsgArenaSignup.DialogButton OptionID, Client.GameClient user)
        {
            stream.InitWriter();

            stream.Write((uint)DialogID);
            stream.Write((uint)OptionID);
            stream.Write(user.Player.UID);
            stream.Write(user.Player.Name, 16);
            stream.Write(user.ArenaStatistic.Info.TodayRank);
            stream.Write((uint)user.Player.Class);
            stream.Write((uint)0);
            stream.Write(user.ArenaStatistic.Info.ArenaPoints);
            stream.Write((uint)user.Player.Level);

            stream.Finalize(GamePackets.MsgArenaSignup);
            return stream;
        }
        public static unsafe void GetArenaSignup(this ServerSockets.Packet stream, out MsgArenaSignup.DialogType DialogID, out MsgArenaSignup.DialogButton OptionID)
        {
            DialogID = (MsgArenaSignup.DialogType)stream.ReadUInt32();
            OptionID = (MsgArenaSignup.DialogButton)stream.ReadUInt32();
        }
    }


    [StructLayout(LayoutKind.Explicit, Size = 60)]
    public struct MsgArenaSignup
    {
        public enum DialogType : uint
        {
            ArenaIconOn = 0u,
            ArenaIconOff = 1u,
            ArenaGui = 3u,
            StartCountDown = 2u,
            OpponentGaveUp = 4u,
            BuyPoints = 5u,
            Match = 6u,
            YouAreKicked = 7u,
            StartTheFight = 8u,
            Dialog = 9u,
            Dialog2 = 10u,
            Continue = 11u
        }

        public enum DialogButton : uint
        {
            Lose = 3u,
            Win = 1u,
            DoGiveUp = 2u,
            Accept = 1u,
            MatchOff = 3u,
            MatchOn = 5u,
            SignUp = 0u
        }

        [Packet(2205)]
        private static void Handler(GameClient user, Packet stream)
        {
            stream.GetArenaSignup(out var DialogID, out var OptionID);
            switch (DialogID)
            {
                case DialogType.OpponentGaveUp:
                    if (OptionID == DialogButton.SignUp)
                        MsgSchedules.Arena.DoQuit(stream, user);
                    break;
                case DialogType.ArenaIconOn:
                    user.Send(stream.ArenaSignupCreate(DialogID, OptionID, user));
                    MsgSchedules.Arena.DoSignup(stream, user);
                    break;
                case DialogType.ArenaIconOff:
                    user.Send(stream.ArenaSignupCreate(DialogID, OptionID, user));
                    MsgSchedules.Arena.DoQuit(stream, user);
                    break;
                case DialogType.BuyPoints:
                    if (user.ArenaPoints <= 1500)
                    {
                        if (user.Player.Money >= 9000000)
                        {
                            user.Player.Money -= 9000000;
                            user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                            user.Send(stream.ArenaInfoCreate(user.ArenaStatistic.Info));
                        }
                    }
                    break;
                case DialogType.ArenaGui:
                    switch (OptionID)
                    {
                        case DialogButton.DoGiveUp:
                            MsgSchedules.Arena.DoGiveUp(stream, user);
                            break;
                        case DialogButton.Win:
                            if (user.ArenaStatistic.ArenaState == MsgArena.User.StateType.WaitForBox)
                            {
                                user.ArenaStatistic.AcceptBox = true;
                                user.ArenaStatistic.ArenaState = MsgArena.User.StateType.WaitForOther;
                            }
                            break;
                    }
                    break;
                case DialogType.Continue:
                    if (OptionID == DialogButton.SignUp && user.ArenaStatistic.CanCountine)
                        MsgSchedules.Arena.DoSignup(stream, user);
                    break;
            }
        }
    }
}
