using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMsgGuardShield;

namespace TheChosenProject.GuardShield
{

    public struct MsgOpenedProcesses
    {
        [PacketAttribute((ushort)MsgGuardShield.PacketIDs.MsgOpenedProcesses)]
        public unsafe static void Process(Client.GameClient client, ServerSockets.Packet packet)
        {
            packet.Seek(0);
            byte[] bytes = packet.ReadBytes(packet.Size);
            var msg = new MsgGuardShield.MsgOpenedProcesses(bytes, client.Guard);

            if (msg.ActionType == MsgGuardShield.MsgOpenedProcesses.Type.Start)//Clean
            {
                client.OpenedProcesses.Clear();
                Console.WriteLine("Someone requested " + client.Player.Name + "'s opened processes, Processing...");
            }
            else if (msg.ActionType == MsgGuardShield.MsgOpenedProcesses.Type.Insert)//Insert
            {
                foreach (var proc in msg.Processes)
                    client.OpenedProcesses.Add(proc);
            }
            else if (msg.ActionType == MsgGuardShield.MsgOpenedProcesses.Type.Finish)//Finish
            {
                Console.WriteLine(client.Player.Name + "'s " + client.OpenedProcesses.Count + " opened processes have been received successfully, Check log files.");
                string text = "==========================================================" + Environment.NewLine;
                text += "Request Time: " + DateTime.Now.ToString("dddd, dd MMMM yyyy [HH:mm:ss]") + Environment.NewLine;
                var bg = client.OpenedProcesses.Where(i => i.StartsWith("B"));
                var window = client.OpenedProcesses.Where(i => i.StartsWith("W"));
                text += "Visible Processes Instances Count: " + window.Count() + Environment.NewLine + Environment.NewLine;

                foreach (var line in window)
                {
                    var split = line.Split('|');
                    try
                    {
                        text += "---------------" + Environment.NewLine;
                        text += "Name: '" + split[1] + "'" + Environment.NewLine;
                    }
                    catch { }
                }
               
                text += Environment.NewLine;
                text += Environment.NewLine;
                text += "Background Processes Instances Count: " + bg.Count() + Environment.NewLine + Environment.NewLine;
                foreach (var line in bg)
                {
                    var split = line.Split('|');
                    try
                    {
                        text += "---------------" + Environment.NewLine;
                        text += "Name: '" + split[1] + "'" + Environment.NewLine;
                    }
                    catch { }
                }
                text += "==========================================================" + Environment.NewLine + Environment.NewLine;
                if (!System.IO.Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\LoaderLogg\\"))
                    System.IO.Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\LoaderLogg\\");
                string path = System.Windows.Forms.Application.StartupPath + "\\LoaderLogg\\[Processes]\\";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                path += client.Player.Name.RemoveIllegalCharacters(false, true) + ".txt";
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.WriteAllLines(path, new string[0]);
                }


                using (var SW = System.IO.File.AppendText(path))
                {
                    SW.WriteLine(text);
                    SW.Close();
                }
            }
        }
    }
}