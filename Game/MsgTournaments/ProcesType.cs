using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Game.MsgTournaments
{
    public enum ProcesType : byte
    {
        None,
        Idle,
        Alive,
        Dead,
    }
    public enum StageType : byte
    {
        None = 0,
        One = 1,
        Tow = 2,
        Three = 3,
        Four = 4,
        Five = 5
    }
}
