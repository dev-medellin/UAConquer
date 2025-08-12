using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Database
{
    public class SystemBannedPC
    {
        public static Dictionary<string, Client> BannedPoll = new Dictionary<string, Client>();
        public class Client
        {
            public uint HDSerial;
            public string HWID;
            public string IP;
            public string MACAdress;
            public string PlayerName;

            public override string ToString()
            {

                Database.DBActions.WriteLine writer = new DBActions.WriteLine('/');
                writer.Add(HDSerial).Add(HWID).Add(IP).Add(MACAdress).Add(PlayerName);
                return writer.Close();
            }
        }

        public static bool AddBan(TheChosenProject.Client.GameClient client, string Reson = "")
        {
            if (!BannedPoll.ContainsKey(client.OnLogin.HWID))
            {
                Client msg = new Client();
                msg.HDSerial = client.OnLogin.HDSerial;
                msg.HWID = client.OnLogin.HWID;
                msg.IP = client.Socket.RemoteIp;
                msg.MACAdress = client.OnLogin.MacAddress;
                msg.PlayerName = client.Player.Name;
                BannedPoll.Add(msg.HWID, msg);
                return true;
            }
            return false;
        }

        public static bool RemoveBan(string playername)
        {
            var CheakHDSerial = BannedPoll.Values.Where(p => p.PlayerName == playername).FirstOrDefault();

            if (BannedPoll.ContainsKey(CheakHDSerial.HWID))
            {
                var msg = BannedPoll[CheakHDSerial.HWID];
                BannedPoll.Remove(msg.HWID);
                Save();
                return true;
            }
            return false;
        }
        public static bool IsBanned(TheChosenProject.Client.GameClient client, out string Messaj)
        {
            var CheakHWID = BannedPoll.Values.Where(p => p.HWID == client.OnLogin.HWID);
            var CheakMAC = BannedPoll.Values.Where(p => p.MACAdress == client.OnLogin.MacAddress);

            if (CheakHWID.Count() > 0 || CheakMAC.Count() > 0)
            {
                AddBan(client);
                Messaj = "You Are Banned For (25) Years";
                return true;
            }
            Messaj = "";
            return false;
        }

        public static void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("BannedPC.txt"))
            {
                foreach (var ban in BannedPoll.Values)
                {
                    if (ban.HDSerial < 1)
                        continue;
                    writer.Add(ban.ToString());
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }

        public static void Load()
        {
            using (Database.DBActions.Read Reader = new DBActions.Read("BannedPC.txt"))
            {

                if (Reader.Reader())
                {
                    uint count = (uint)Reader.Count;
                    for (uint x = 0; x < count; x++)
                    {

                        DBActions.ReadLine readline = new DBActions.ReadLine(Reader.ReadString(""), '/');
                        Client msg = new Client();
                        msg.HDSerial = readline.Read((uint)0);
                        if (msg.HDSerial < 1)
                            continue;
                        msg.HWID = readline.Read("");
                        msg.IP = readline.Read("");
                        msg.MACAdress = readline.Read("");
                        msg.PlayerName = readline.Read("");
                        BannedPoll.Add(msg.HWID, msg);
                    }
                }
            }
        }

    }
}




