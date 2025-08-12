using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgNone : ITournament
    {
        public ProcesType Process { get; set; }

        public TournamentType Type { get; set; }

        public MsgNone(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        public bool Join(GameClient user, Packet stream)
        {
            return false;
        }

        public void CheckUp()
        {
        }

        public bool InTournament(GameClient user)
        {
            return false;
        }
    }
}

