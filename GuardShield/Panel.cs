using TheChosenProject.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheChosenProject.Database;
using CMsgGuardShield;

namespace TheChosenProject
{
    public partial class Panel : Form
    {
        public Panel()
        {
            InitializeComponent();
            LoadingOnline();
            Loadingbanned();
            LoadingbannedPC();
        }
        public TheChosenProject.Client.GameClient GetPlayer(string Name)
        {
            Client.GameClient player =Server.GamePoll.Values.Where(s => s.Player.Name == Name).FirstOrDefault();
            if (player == null)
                return null;
            return player;
        }

        public SystemBannedAccount.Client GetPlayerBanned(string Name)
        {
            var player = SystemBannedAccount.BannedPoll.Values.Where(s => s.Name == Name).FirstOrDefault();
            if (player == null)
                return null;
            return player;
        }


        public void LoadingOnline()
        {
            //load player
            comboBox1.Items.Clear();
            comboBox1.Text = "";
            foreach (var C in Server.GamePoll.Values)
            {
                if(!C.Fake)
                   comboBox1.Items.Add(C.Player.Name);
            }
        }
        public void Loadingbanned()
        {
            //banned list
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            textBox4.Text = "";//UID
            textBox5.Text = "";//PlayerName
            textBox7.Text = "";//reson
            foreach (var p in SystemBannedAccount.BannedPoll.Values)
            {
                comboBox2.Items.Add(p.Name);
            }
        }
        public void LoadingbannedPC()
        {
            //banned list
            comboBox3.Items.Clear();
            comboBox3.Text = "";
            foreach (var p in SystemBannedPC.BannedPoll.Values)
            {
                comboBox3.Items.Add(p.PlayerName);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            LoadingOnline();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Loadingbanned();
        }

        private void button2_Click(object sender, EventArgs e)//scan
        {
            var player = GetPlayer(comboBox1.Text);
            if (player != null)
            {
                player.Guard.RequestOpenedProcesses();
            }
            else
                Console.WriteLine("sorry cant find player " + comboBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)//MachineInfo
        {
            var player = GetPlayer(comboBox1.Text);
            if (player != null)
            {
                player.Guard.RequestMachineInfo();
            }
            else
                Console.WriteLine("sorry cant find player " + comboBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var player = GetPlayer(comboBox1.Text);
            if (player != null)
            {
                player.Guard.TerminateLoader("TheChosenProject","Your Client Closed by GM");             
                LoadingOnline();
            }
            else
                Console.WriteLine("sorry cant find player" + comboBox1.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string Statue = textBox6.Text;
            var player = GetPlayer(comboBox1.Text);
            if (player != null)
            {
                player.Guard.PingStatuesLoader(Statue);
                Console.WriteLine("Done Update Message for " +player.Player.Name);
            }
            else
                Console.WriteLine("sorry cant find player " + comboBox1.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {


            uint Hours = uint.Parse(textBox1.Text);
            if (Hours > 0)
            {
                var player = GetPlayer(comboBox1.Text);
                if (player != null)
                {
                    SystemBannedAccount.AddBan(player, Hours, "By~GM");
                    player.Socket.Disconnect();
                    LoadingOnline();
                }
                else
                Console.WriteLine("sorry cant find player " + comboBox1.Text);
            }
            else
                Console.WriteLine("sorry please select hours banned from 1 to 999");
        }

        private void button7_Click(object sender, EventArgs e)//close client for all player
        {
            string Caption = textBox2.Text;
            string Message = richTextBox1.Text;
            foreach (var C in Database.Server.GamePoll.Values)
            {
                C.Guard.TerminateLoader(Caption, Message);
            }
            LoadingOnline();
            Console.WriteLine("Done Closed All Client's");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string Message = textBox3.Text;
            foreach (var C in Database.Server.GamePoll.Values)
            {
                C.Guard.PingStatuesLoader(Message);
            }
            Console.WriteLine("Done Update Message for All Client's");
        }

        //banned comb_box
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SystemBannedAccount.Client player = GetPlayerBanned(comboBox2.Text);
            if (player != null)
            {
                textBox4.Text = player.UID.ToString();//UID
                textBox5.Text = player.Name;//PlayerName
                textBox7.Text = player.Reason;//reson
            }
            else
                Console.WriteLine("sorry cant find player " + comboBox1.Text);
        }
        private void button10_Click(object sender, EventArgs e)//remove banned
        {
            uint UID = uint.Parse(textBox4.Text);
            if (SystemBannedAccount.RemoveBan(UID))
            {
                Console.WriteLine("Done remove player " +  comboBox2.Text +" from banned ");
                textBox4.Text = "";//UID
                textBox5.Text = "";//PlayerName
                textBox7.Text = "";//reson
                Loadingbanned();
            }
            else
                Console.WriteLine("sorry cant remove player"+  comboBox2.Text+ " from banned");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var player = Client.GameClient.CharacterFromName(comboBox1.Text);
            if (player == null)
                return;
            if (Database.SystemBannedPC.AddBan(player))
            {
                comboBox1.Text = "";
                LoadingOnline();
                player.Socket.Disconnect();
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            LoadingbannedPC();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string Name = comboBox3.Text;
            if (Name.Length <= 0)
                return;
            if (Database.SystemBannedPC.RemoveBan(Name))
            {
                LoadingbannedPC();
            }
        }
    }
}
