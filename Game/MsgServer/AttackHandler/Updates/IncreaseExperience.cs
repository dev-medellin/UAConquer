using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler.Updates
{
   public class IncreaseExperience
    {
        public static void Up(Packet stream, GameClient user, uint Damage)
        {
            if (Damage == 0)
                return;
            if (user.Player.ContainFlag(MsgUpdate.Flags.Oblivion))
                user.ExpOblivion += (ulong)(Damage * 4);
            else
                user.IncreaseExperience(stream, (double)Damage);
            if (user.Player.HeavenBlessing <= 0)
                return;
            user.Player.HuntingBlessing += Damage / 10;
        }


    }
}
