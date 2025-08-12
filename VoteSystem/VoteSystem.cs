using System;
using System.Collections.Generic;
using TheChosenProject.Game.MsgServer;
using System.Linq;
using System.Text;
using TheChosenProject.VoteSystem;

namespace TheChosenProject.Database
{
    public class VoteSystem
    {

        public class User
        {
            public uint UID;
            public string MAC;
            public DateTime Timer = new DateTime();
            public override string ToString()
            {
                var writer = new DBActions.WriteLine('/');
                writer.Add(UID).Add(MAC).Add(Timer.Ticks);
                return writer.Close();
            }
        }
        public static List<User> UsersPoll = new List<User>();
        public static bool TryGetObject(uint UID, string IP, out User obj)
        {
            foreach (var _obj in UsersPoll)
            {
                if (_obj.UID == UID || _obj.MAC == IP)
                {
                    obj = _obj;
                    return true;
                }
            }
            obj = null;
            return false;
        }
        public static bool CanClaim(Client.GameClient client)
        {
            if (!client.Player.StartVote)
                return false;
            if (client.Player.StartVote)
                if (Extensions.Time32.Now > client.Player.StartVoteStamp)
                    return true;
            return false;
        }
        //public static bool CanVote(Client.GameClient client)
        //{
        //    User _user;
        //    DateTime? lastVoteTimeNullable = client.Player.Owner.VoteSystemDb();
        //    if (lastVoteTimeNullable.HasValue)
        //    {
        //        DateTime lastVoteTime = lastVoteTimeNullable.Value; // Explicitly convert nullable DateTime to non-nullable
        //        if (lastVoteTime.AddHours(12) < DateTime.Now)
        //            return true;
        //        else
        //            return false;
        //    } else
        //    {

        //        client.Player.Owner.InsertVoteTime(client.Player.UID, DateTime.Now, client.OnLogin.MacAddress);
        //        return true;
        //    }
        //}
        public static bool CanVote(Client.GameClient client)
        {
            User _user;
            DateTime now = DateTime.Now;

            // 1. Check if same MAC has voted on another UID within 12 hours
            var macVoteTime = client.Player.Owner.GetLastVoteTimeByMac(client.OnLogin.MacAddress, client.Player.UID);
            if (macVoteTime.HasValue && macVoteTime.Value.AddHours(12) > now)
            {
                return false;
            }

            // 2. Check if current UID has voted within 12 hours
            DateTime? lastVoteTimeNullable = client.Player.Owner.VoteSystemDb();
            if (lastVoteTimeNullable.HasValue)
            {
                DateTime lastVoteTime = lastVoteTimeNullable.Value;
                if (lastVoteTime.AddHours(12) < now)
                    return true;
                else
                    return false;
            }
            else
            {
                // 3. First time vote - allow and insert
                client.Player.Owner.InsertVoteTime(client.Player.UID, now, client.OnLogin.MacAddress);
                return true;
            }
        }
        public static void Vote(Client.GameClient client)
        {
            client.SendSysMesage(ServerKernel.XtremeTopLink, MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.white);
            client.Player.StartVote = true;
            client.LastVote = DateTime.Now;
            client.Player.vote = DateTime.Now;
            client.Player.StartVoteStamp = Extensions.Time32.Now.AddSeconds(60);

            //client.Player.SendMsgBox(" Enter the code and please wait for your vote to be validated by the system", 60, MsgStaticMessage.Messages.Vote);
        }
        public static void CheckUp(Client.GameClient client)
        {
            if (client.Player.StartVote)
            {
                if (Extensions.Time32.Now > client.Player.StartVoteStamp)
                {
                    User _user;
                    DateTime? lastVoteTimeNullable = client.Player.Owner.VoteSystemDb();
                    if (lastVoteTimeNullable.HasValue)
                    {
                        //_user.Timer = DateTime.Now;
                        client.Player.Owner.UpdateVoteTime(client.Player.UID, DateTime.Now);
                    }
                    else
                    {
                        _user = new User();
                        _user.UID = client.Player.UID;
                        _user.MAC = client.OnLogin.MacAddress;
                        _user.Timer = DateTime.Now;
                        client.Player.Owner.InsertVoteTime(client.Player.UID, DateTime.Now, client.OnLogin.MacAddress);
                        UsersPoll.Add(_user);
                    }

                    client.Player.VotePoints += 1;
                    client.Player.CountVote += 1;
                    VoteRank.Users hero;
                    if (VoteRank.VoteRanksPoll.TryGetValue(client.Player.UID, out hero))
                    {
                        hero.VoteCount = (int)client.Player.CountVote;
                    }
                    else
                    {
                        VoteRank.Users newhero = new VoteRank.Users() { Name = client.Player.Name, UID = client.Player.UID, VoteCount = (int)client.Player.CountVote };
                        VoteRank.VoteRanksPoll.Add(newhero.UID, newhero);
                    }

                    client.SendSysMesage("Thank you for taking the time to vote for the server! You have been rewarded with one Vote Point! Exchange it for treasures by speaking with Mrs.Vote!", TheChosenProject.Game.MsgServer.MsgMessage.ChatMode.System);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[Xtremetop100] " + client.Player.Name + " - has just cast a vote in favour of the server! #11#12", "ALLUSERS", "Server", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    }
                    client.Player.StartVote = false;
                }
            }
        }
        public static void Save()
        {
            using (Database.DBActions.Write _wr = new Database.DBActions.Write("Votes.txt"))
            {
                foreach (var _obj in UsersPoll)
                    _wr.Add(_obj.ToString());
                _wr.Execute(DBActions.Mode.Open);
            }
        }
        public static void Load()
        {
            using (Database.DBActions.Read r = new Database.DBActions.Read("Votes.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine reader = new DBActions.ReadLine(r.ReadString(""), '/');
                        User user = new User();
                        user.UID = reader.Read((uint)0);
                        user.MAC = reader.Read("");
                        user.Timer = DateTime.FromBinary(reader.Read((long)0));
                        UsersPoll.Add(user);
                    }
                }
            }

        }

    }
}
