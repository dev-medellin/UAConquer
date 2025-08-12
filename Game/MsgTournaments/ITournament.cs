using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgTournaments
{
    public interface ITournament
    {
        ProcesType Process { get; set; }

        TournamentType Type { get; set; }

        void Close();

        void Open();

        bool Join(GameClient user, Packet stream);

        void CheckUp();

        bool InTournament(GameClient user);
    }
}
