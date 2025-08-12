using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgSecondaryPassword
    {
        public enum ActionID : uint
        {
            VerifiedPassword = 1u,
            ForgetPassword,
            SetNewPass,
            SendInformation,
            PasswordCorrect,
            PasswordWrong
        }

        public static Packet SecondaryPasswordCreate(this Packet stream, ActionID Type, uint OldPassowrd, uint NewPassword)
        {
            stream.InitWriter();
            stream.Write((uint)Type);
            stream.Write(OldPassowrd);
            stream.Write(NewPassword);
            stream.Finalize(2261);
            return stream;
        }

        public static void GetSecondaryPassword(this Packet stream, out ActionID Type, out uint OldPassowrd, out uint NewPassword)
        {
            Type = (ActionID)stream.ReadUInt32();
            OldPassowrd = stream.ReadUInt32();
            NewPassword = stream.ReadUInt32();
        }

        [Packet(2261)]
        private static void Process(GameClient user, Packet stream)
        {
            stream.GetSecondaryPassword(out var Type, out var OldPassoword, out var NewPassoword);
            switch (Type)
            {
                case ActionID.SetNewPass:
                    if (user.Player.SecurityPassword == 0)
                    {
                        user.Player.SecurityPassword = NewPassoword;
                        user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 257, 0));
                        user.SendSysMesage("Successfully set! Please remember your secondary password");
                    }
                    break;
                case ActionID.SendInformation:
                    if (user.Player.SecurityPassword == 0)
                    {
                        user.Player.SecurityPassword = NewPassoword;
                        user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 257, 0));
                        user.SendSysMesage("Successfully set! Please remember your secondary password");
                    }
                    else if (OldPassoword == user.Player.SecurityPassword)
                    {
                        user.Player.OnReset = 0;
                        user.Player.SecurityPassword = NewPassoword;
                        user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 257, 0));
                        user.SendSysMesage("Successfully modified! Please remember your secondary password");
                    }
                    else
                    {
                        user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordWrong, 1, 0));
                    }
                    break;
                case ActionID.VerifiedPassword:
                    if (user.Player.SecurityPassword != 0)
                    {
                        if (user.Player.SecurityPassword == NewPassoword)
                        {
                            user.Player.VerifiedPassword = true;
                            user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 257, 0));
                            user.SendSysMesage("Secondary verified!");
                        }
                        else
                            user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordWrong, 0, 0));
                    }
                    break;
                case ActionID.ForgetPassword:
                    if (user.Player.SecurityPassword != 0)
                    {
                        user.Player.OnReset = 1;
                        user.Player.ResetSecurityPassowrd = DateTime.Now.AddDays(7.0);
                        user.Send(stream.SecondaryPasswordCreate(ActionID.PasswordCorrect, 257, 0));
                        user.SendSysMesage("Your secondary password will be removed on " + user.Player.ResetSecurityPassowrd.ToString("d/M/yyyy (H:mm)") + ".");
                    }
                    break;
            }
        }
    }
}
