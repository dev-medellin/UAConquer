using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgClassPKWar
    {
        public enum TournamentType : byte
        {
            Trojan,
            Warrior,
            Archer,
            Ninja,
            Monk,
            Pirate,
            Fire,
            Water,
            LongLee,
            WindWalker,
            Count
        }

        public enum TournamentLevel : byte
        {
            Level_1_99,
            Level_100_119,
            Level_120_130,
            Level_130_140,
            Count
        }

        public class War
        {
            public uint Winner;

            public byte ReceiveReward;

            public MsgUpdate.Flags LastFlag;

            public TournamentLevel Level;

            public TournamentType Typ;

            public ProcesType Proces;

            public uint DinamicID;

            public DateTime FinishTimer;

            public War(TournamentType _typ, TournamentLevel _level, ProcesType proces)
            {
                Level = _level;
                Proces = proces;
                Typ = _typ;
            }

            public void Start(GameMap map)
            {
                if (Proces != ProcesType.Dead)
                    return;
                Proces = ProcesType.Alive;
                FinishTimer = DateTime.Now.AddMinutes(10.0);
                DinamicID = map.GenerateDynamicID();
                foreach (GameClient client in Server.GamePoll.Values)
                {
                    client.Player.MessageBox("", delegate (GameClient p)
                    {
                        p.Teleport(436, 244, 1002);
                    }, null, 60, MsgStaticMessage.Messages.ClassPk);
                }
            }

            internal void Stop()
            {
                Proces = ProcesType.Dead;
            }

            public bool CheckJoin()
            {
                if (Proces == ProcesType.Dead)
                    return false;
                if (DateTime.Now < FinishTimer)
                    return true;
                Proces = ProcesType.Alive;
                return false;
            }

            public bool IsFinish(GameClient client)
            {
                if (Proces == ProcesType.Alive)
                {
                    GameClient[] arrayMap;
                    arrayMap = client.Map.Values.Where((GameClient p) => p.Player.DynamicID == DinamicID && p.Player.Alive).ToArray();
                    if (arrayMap.Length == 1)
                        return true;
                }
                return false;
            }

            public void GetMyReward(GameClient client, Packet stream)
            {
                if (!IsFinish(client))
                    return;
                Proces = ProcesType.Dead;
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    if (user.Player != null && user.Player.UID == Winner)
                        user.Player.RemoveFlag(LastFlag);
                }
                MsgUpdate.Flags aura;
                aura = MsgSchedules.ClassPkWar.ObtainAura(Typ);
                if (aura != MsgUpdate.Flags.Normal)
                    client.Player.AddFlag(aura, 2592000, false);
                IEventRewards.Add("ClassPK War", 0u, 0u, Server.ItemsBase[723467u].Name + " + " + Server.ItemsBase[730006u].Name, "[" + client.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
                string str = "[EVENT]" + client.Player.Name + " was rewarded MegaMeteorScrool from ClassPK War.";
                ServerDatabase.LoginQueue.Enqueue((object)str);
                if (client.Inventory.HaveSpace((byte)11))
                {
                    //client.Inventory.Add(stream, 721169, 2, 0, 0, 0);//5xUltimatePack
                    //client.Inventory.Add(stream, 722057, 5, 0, 0, 0);//4xPowerExpball
                    //client.Inventory.Add(stream, 730003, 3, 0, 0, 0);//+3 stone
                    client.Inventory.Add(stream, 720547, 1, 0, 0, 0);//MegaMetsPack
                    //client.Inventory.Add(stream, 720546, 1, 0, 0, 0);//2xMegaDBPack
                }
                Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " , was rewarded MegaMeteorScrool from ClassPK War.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));

                LastFlag = aura;
                Winner = client.Player.UID;
                //MsgSchedules.SendSysMesage(client.Player.Name + " Won " + Typ.ToString() + " PK War (" + Level.ToString() + ") , he received Top " + Typ.ToString() + ", " + ServerKernel.CLASS_PK_WAR_REWARD.ToString("0,0") + " ConquerPoints and 4 PowerExpBalls !", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.white);
                client.Teleport(430, 269, 1002);
            }
        }

        public const ushort MapID = 1764;

        public const string FilleName = "\\ClassPkWar.ini";

        public War[][] PkWars;

        public ProcesType Proces;

        public MsgClassPKWar(ProcesType _Proces)
        {
            Proces = _Proces;
            PkWars = new War[10][];
            TournamentType i;
            i = TournamentType.Trojan;
            while ((int)i < 10)
            {
                PkWars[(uint)i] = new War[4];
                TournamentLevel x;
                x = TournamentLevel.Level_1_99;
                while ((int)x < 4)
                {
                    PkWars[(uint)i][(uint)x] = new War(i, x, ProcesType.Dead);
                    x++;
                }
                i++;
            }
        }

        internal void Start()
        {
            if (Proces != ProcesType.Dead)
                return;
            TournamentType i;
            i = TournamentType.Trojan;
            while ((int)i < 10)
            {
                PkWars[(uint)i] = new War[4];
                TournamentLevel x;
                x = TournamentLevel.Level_1_99;
                while ((int)x < 4)
                {
                    PkWars[(uint)i][(uint)x] = new War(i, x, ProcesType.Dead);
                    x++;
                }
                i++;
            }
            War[][] pkWars;
            pkWars = PkWars;
            foreach (War[] tournament in pkWars)
            {
                War[] array;
                array = tournament;
                foreach (War war in array)
                {
                    war.Start(Server.ServerMaps[1764u]);
                }
            }
            Proces = ProcesType.Idle;
            try
            {
                ITournamentsAlive.Tournments.Add(12, ": started at(" + DateTime.Now.ToString("H:mm)"));
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not start ClassPk War", true, LogType.WARNING);
            }
        }

        internal void Stop()
        {
            if (Proces == ProcesType.Dead)
                return;
            War[][] pkWars;
            pkWars = PkWars;
            foreach (War[] tournament in pkWars)
            {
                War[] array;
                array = tournament;
                foreach (War war in array)
                {
                    war.Stop();
                }
            }
            Proces = ProcesType.Dead;
            try
            {
                ITournamentsAlive.Tournments.Remove(12);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not finish Class Pk War", true, LogType.WARNING);
            }
        }

        internal MsgUpdate.Flags ObtainAura(TournamentType typ)
        {
            switch (typ)
            {
                case TournamentType.Trojan:
                    return MsgUpdate.Flags.TopTrojan;
                case TournamentType.Warrior:
                    return MsgUpdate.Flags.TopWarrior;
                case TournamentType.Archer:
                    return MsgUpdate.Flags.TopArcher;
                case TournamentType.Ninja:
                    return MsgUpdate.Flags.TopNinja;
                case TournamentType.Monk:
                    return MsgUpdate.Flags.TopMonk;
                case TournamentType.Fire:
                    return MsgUpdate.Flags.TopFireTaoist;
                case TournamentType.Water:
                    return MsgUpdate.Flags.TopWaterTaoist;
                default:
                    return MsgUpdate.Flags.Normal;
            }
        }

        public War GetMyWar(GameClient client)
        {
            War[][] pkWars;
            pkWars = PkWars;
            foreach (War[] tournament in pkWars)
            {
                War[] array;
                array = tournament;
                foreach (War war in array)
                {
                    if (war.DinamicID == client.Player.DynamicID)
                        return war;
                }
            }
            return null;
        }

        internal static TournamentType GetMyTournamentType(GameClient client)
        {
            if (AtributesStatus.IsTrojan(client.Player.Class))
                return TournamentType.Trojan;
            if (AtributesStatus.IsWarrior(client.Player.Class))
                return TournamentType.Warrior;
            if (AtributesStatus.IsArcher(client.Player.Class))
                return TournamentType.Archer;
            if (AtributesStatus.IsNinja(client.Player.Class))
                return TournamentType.Ninja;
            if (AtributesStatus.IsMonk(client.Player.Class))
                return TournamentType.Monk;
            if (AtributesStatus.IsWater(client.Player.Class))
                return TournamentType.Water;
            if (AtributesStatus.IsFire(client.Player.Class))
                return TournamentType.Fire;
            return TournamentType.Count;
        }

        internal static TournamentLevel GetMyTournamentLevel(GameClient client)
        {
            if (client.Player.Level <= 99)
                return TournamentLevel.Level_1_99;
            if (client.Player.Level > 99 && client.Player.Level < 120)
                return TournamentLevel.Level_100_119;
            if (client.Player.Level >= 120 && client.Player.Level <= 129)
                return TournamentLevel.Level_120_130;
            if (client.Player.Level >= 130)
                return TournamentLevel.Level_130_140;
            return TournamentLevel.Count;
        }

        internal ProcesType GetWar(GameClient client, out War mywar)
        {
            TournamentType typ;
            typ = GetMyTournamentType(client);
            TournamentLevel level;
            level = GetMyTournamentLevel(client);
            War[] tournament;
            tournament = PkWars[(uint)typ];
            War[] array;
            array = tournament;
            foreach (War war in array)
            {
                if (war.Typ == typ && war.Level == level)
                {
                    mywar = war;
                    return war.Proces;
                }
            }
            mywar = null;
            return ProcesType.Dead;
        }

        internal void LoginClient(GameClient client)
        {
            War[][] pkWars;
            pkWars = PkWars;
            foreach (War[] tournament in pkWars)
            {
                War[] array;
                array = tournament;
                foreach (War war in array)
                {
                    if (war.Winner == client.Player.UID)
                        client.Player.AddFlag(war.LastFlag, 2592000, false);
                }
            }
        }

        internal void Save()
        {
            Write writer;
            writer = new Write("\\ClassPkWar.ini");
            War[][] pkWars;
            pkWars = PkWars;
            foreach (War[] tournament in pkWars)
            {
                War[] array;
                array = tournament;
                foreach (War war in array)
                {
                    WriteLine line;
                    line = new WriteLine('/');
                    line.Add((byte)war.Typ).Add((byte)war.Level).Add(war.Winner)
                        .Add((int)war.LastFlag);
                    writer.Add(line.Close());
                }
            }
            writer.Execute(Mode.Open);
        }

        internal void Load()
        {
            Read reader;
            reader = new Read("\\ClassPkWar.ini");
            if (reader.Reader())
            {
                for (int x = 0; x < reader.Count; x++)
                {
                    ReadLine line;
                    line = new ReadLine(reader.ReadString(""), '/');
                    byte typ;
                    typ = line.Read((byte)0);
                    byte level;
                    level = line.Read((byte)0);
                    uint Winner;
                    Winner = line.Read((uint)0);
                    MsgUpdate.Flags LastFlag;
                    LastFlag = (MsgUpdate.Flags)line.Read(0);
                    PkWars[typ][level].Winner = Winner;
                    PkWars[typ][level].LastFlag = LastFlag;
                }
            }
        }
    }
}
