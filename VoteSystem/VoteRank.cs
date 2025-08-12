using Extensions.ThreadGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.VoteSystem
{
    public class VoteRank
    {
        public class Users
        {
            public string Name;
            public uint UID;
            public int VoteCount;
        }
        public static Dictionary<uint, Users> VoteRanksPoll = new Dictionary<uint, Users>();
        private ThreadItem _thread;
        public VoteRank()
        {
            Load();
            _thread = new ThreadItem(1000, "", WorkThread);
            _thread.Open();
        }
        public void Load()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\"))
            {
                ini.FileName = fname;
                string name = ini.ReadString("Character", "Name", "");
                uint uid = ini.ReadUInt32("Character", "UID", 0);
                int votepoint = (int)ini.ReadUInt32("Character", "CountVote", 0);
                Users users = new Users() { Name = name, UID = uid, VoteCount = votepoint };
                if (users.VoteCount > 0)
                    VoteRanksPoll.Add(users.UID, users);
            }
        }
        public Users[] GetRanks
        {
            get
            {
                if (VoteRanksPoll.Count > 0)
                    return VoteRanksPoll.Values.OrderByDescending(e => e.VoteCount).Take(5).ToArray();
                else return null;
            }
        }
        public static uint Reward(uint rank)
        {
            uint valueReward = 0;
            switch (rank)
            {
                case 1: valueReward = 6250; break;
                case 2: valueReward = 5000; break;
                case 3: valueReward = 3750; break;
                case 4: valueReward = 2500; break;
                case 5: valueReward = 1250; break;
            }
            return valueReward;
        }
        public void WorkThread()
        {
            try
            {
                DateTime Now64 = DateTime.Now;
                if (Now64.DayOfWeek == DayOfWeek.Friday)
                {
                    if (Now64.Hour == 23 && Now64.Minute == 59 && Now64.Second <= 1)
                    {
                        if (GetRanks != null)
                        {
                            if (GetRanks.Length > 0)
                            {
                                for (int i = 0; i < GetRanks.Length; i++)
                                {
                                    uint Rank = (uint)(i + 1);
                                    uint cps = Reward(Rank);
                                    Client.GameClient gameClient;
                                    if (Database.Server.GamePoll.TryGetValue(GetRanks[i].UID, out gameClient))
                                    {
                                        gameClient.Player.Money += (int)cps * 100;
                                        if (Rank == 1)
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                Program.SendGlobalPackets.Enqueue(
                                             new Game.MsgServer.MsgMessage(gameClient.Player.Name + " won the best weekly voter and win " + cps + " ConquerMoney.", "ALLUSERS", "BestWeeklyVoter", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                                            }
                                        }
                                        gameClient.SendSysMesage("You won rank " + Rank + " in weekly voter and win " + cps + " ConquerMoney.");
                                    }
                                    else
                                    {
                                        if (Rank == 1)
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                Program.SendGlobalPackets.Enqueue(
                                             new Game.MsgServer.MsgMessage(GetRanks[i].Name + " won the best weekly voter and win " + cps + " ConquerPoints.", "ALLUSERS", "BestWeeklyVoter", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                                            }
                                        }
                                        WindowsAPI.IniFile data = new WindowsAPI.IniFile("\\Users\\" + GetRanks[i].UID + ".ini");
                                        uint Cps = data.ReadUInt32("Character", "Money", 0);
                                        Cps += cps;
                                        data.Write<uint>("Character", "Money", Cps);
                                    }
                                }
                                foreach (var c in Database.Server.GamePoll.Values)
                                {
                                    c.Player.CountVote = 0;
                                }
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                foreach (string fname in System.IO.Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\"))
                                {
                                    try
                                    {
                                        ini.FileName = fname;
                                        ini.Write<uint>("Character", "CountVote", 0);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                VoteRanksPoll.Clear();
                                VoteRanksPoll = new Dictionary<uint, Users>();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.SaveException(e);
            }
        }
    }
}
