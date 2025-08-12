using TheChosenProject.Client;
using TheChosenProject.Cryptography;
using TheChosenProject.Database;
using TheChosenProject.Game.Ai;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Multithreading;
//using TheChosenProject.NetDragon;
using TheChosenProject.Properties;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerCore;
using TheChosenProject.ServerCore.Website;
using TheChosenProject.ServerSockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TheChosenProject.WindowsAPI;
//using MsgProtect;
using System.Runtime.InteropServices.ComTypes;
using TheChosenProject.Game.MsgServer.AttackHandler.Algoritms;
using TheChosenProject.VoteSystem;
using TheChosenProject.Role.Bot;

namespace TheChosenProject
{
    public class ServerManager : Form
    {
        public enum Enable
        {
            Show = 1,
            Hide
        }

        public enum MainFlag
        {
            Bannedlist = 1,
            BannedPClist = 2,
            ItemsBaselist = 4,
            Titleslist = 8,
            Tournamentlist = 16,
            Configrationlist = 32,
            Resetlist = 64,
            Bosseslist = 128,
            TournamentTimerlist = 256,
            Userslist = 512,
            Botslist = 1024,
            TournamentPrizes = 2048,
            Alllist = 2559
        }

        public enum Logger
        {
            Main = 1,
            Cheater
        }

        public bool RefrshBeforeUpdate;

        public bool OnLogged;

        public bool TestUser;

        public bool MonsterDBSpawn;

        private IContainer components;

        private TextBox TxtLogger;

        private GroupBox groupBox1;

        private TextBox TxtConnectIp;

        private Label label1;

        private NumericUpDown NumPlayerCount;

        private Label label3;

        private ListBox LbxCharacters;

        private GroupBox groupBox2;

        private TabControl tabControl1;

        private TabPage tabPage1;

        private TabPage tabPage2;

        private TabPage tabPage3;

        private TabPage tabPage4;

        private Label label7;

        private Label label6;

        private Label label5;

        private Label label10;

        private Label label9;

        private Label label8;

        private Label label15;

        private Label label14;

        private Label label13;

        private Label label12;

        private Label label11;

        private Label LblUserAdditionalPoints;

        private Label LblUserSpirit;

        private Label LblUserVitality;

        private Label LblUserAgility;

        private Label LblUserStrength;

        private Label LblUserLocation;

        private Label LblUserExperience;

        private Label LblUserLevel;

        private Label LblUserMateName;

        private Label LblUserName;

        private Label LblUserId;

        private Label LblUserPkPoints;

        private Label label16;

        private Label LblUserManaPoints;

        private Label label18;

        private Label LblUserHealthPoints;

        private Label label20;

        private ListBox LbxOnScreen;

        private Label label17;

        private Button BtnRefreshPC;

        private Button BtnDisconnectUser;

        private CheckBox CheckBoxAutoHunt;

        private Label LblConquerPoints;

        private Label VIPLevel;

        private Label LblVIPLevel;

        private Label Money;

        private Label UserClamCP;

        private Label LblUserClamCP;

        private Label ConquerPoints;

        private Label LblChampionPoints;

        private Label NobilityRank;

        private Label LblVIPTime;

        private Label OnlinePoints;

        private Label LblOnlinePoints;

        private Label ChampionPoints;

        private Label LblMoney;

        private Label SpecialTitles;

        private Label LblSpecialTitles;

        private Label NewbieProtection;

        private Label LblNewbieProtection;

        private Label SecurityPass;

        private Label LblSecurityPass;

        private Label VIPTime;

        private Label LblNobilityRank;

        private Label SecondClass;

        private Label LblSecondClass;

        private Label FirstClass;

        private Label LblClass;

        private Label ExtraAtributes;

        private Label LblExtraAtributes;

        private Label NameChange;

        private Label LblFirstClass;

        private Label WHMoney;

        private Label LblWHMoney;

        private Label Class;

        private Label GuildRank;

        private Label LblGuildRank;

        private Label GuildName;

        private Label LblGuildName;

        private Label ClanRank;

        private Label LblClanRank;

        private Label ClanName;

        private CheckBox CheckBoxOfflineTraining;

        private CheckBox CheckBoxOfflineVending;

        private CheckBox CheckBoxOfflineHunting;

        private Label label21;

        private ListBox LbxOnPCCount;

        private Button button1;

        private Button BtnStart;

        private Label LblNameChange;

        private Label LblClanName;

        private GroupBox groupBox4;

        private ComboBox comboBox3;

        private Button button6;

        private Label label22;

        private CheckBox checkBox1;

        private ComboBox comboBox1;

        public Button button3;

        private Label label26;

        private TextBox textBox1;

        public ListBox RecList;

        public Button button4;

        public Button Send;

        public TextBox SendText;

        public ListBox ClientList;

        public Button button2;

        public Button button5;

        private GroupBox groupBox3;

        private ComboBox comboBox2;

        private Button button7;

        private GroupBox groupBox5;

        private ComboBox comboBox4;

        private Button button8;

        private Button button17;

        private Button button14;

        private Button button13;

        private Button button12;

        private Button button11;

        private Button button10;

        private Button button9;

        private Button button15;

        private Button button16;

        private TabPage tabPage5;

        private Button button18;

        private Button button19;

        private Button button20;

        private Button button21;

        private Button button22;

        private Label label27;

        private TextBox textBox2;

        private GroupBox groupBox6;

        private ComboBox comboBox5;

        private GroupBox groupBox7;

        private ComboBox comboBox6;

        private TextBox textBox7;

        private Button button25;

        private TextBox textBox5;

        private TextBox textBox4;

        private Button button24;

        private TextBox textBox6;

        private Label label2;

        private TextBox textBox3;

        private Label label4;

        private Label label28;

        private Label label29;

        private Label label30;

        private Label label31;

        private CheckBox checkBox2;

        private Button button28;

        private Button button27;

        private Button button29;

        private Label label34;

        private Label label35;

        private ListBox listBox1;

        private Button button31;

        private PictureBox pictureBox1;

        public Button button32;

        public TextBox textBox8;

        private ComboBox comboBox7;

        private Button button44;

        private Button button43;

        private Button button42;

        private Button button41;

        private Button button40;

        private Button button39;
        private Button button309;

        private Button button38;

        private Button button37;

        private Button button36;

        private Button button34;

        private Button button33;

        private Button button48;

        private Button button35;

        private GroupBox groupBox8;

        private ComboBox comboBox8;

        private Button button45;

        private GroupBox groupBox9;

        private ComboBox comboBox9;

        private Button button46;

        private GroupBox groupBox10;

        private ComboBox comboBox10;

        private Button button49;

        private CheckBox checkBox3;

        private TabPage tabPage6;

        private GroupBox groupBox11;

        private GroupBox groupBox12;

        private Label label61;

        private Label label63;

        private PictureBox pictureBox2;

        private Label label70;

        private Label label71;

        private Label label72;

        private Label label67;

        private Label label68;

        private Label label69;

        private Label label66;

        private Label label65;

        private Label label64;

        private Label label75;

        private Label label74;

        private Label label76;

        private Label label80;

        private Label label79;

        private Label label86;

        private Label label85;

        private Label label84;

        private Label label83;

        private Label label82;

        private Label label81;

        private Label label87;

        private Label label89;

        private FontDialog fontDialog1;

        private Label label77;

        private NumericUpDown numericUpDown1;

        private NumericUpDown numericUpDown7;

        private NumericUpDown numericUpDown6;

        private NumericUpDown numericUpDown5;

        private NumericUpDown numericUpDown4;

        private NumericUpDown numericUpDown2;

        private Label label19;

        private NumericUpDown numericUpDown17;

        private NumericUpDown numericUpDown18;

        private NumericUpDown numericUpDown19;

        private NumericUpDown numericUpDown20;

        private NumericUpDown numericUpDown16;

        private NumericUpDown numericUpDown15;

        private NumericUpDown numericUpDown14;

        private NumericUpDown numericUpDown13;

        private NumericUpDown numericUpDown12;

        private NumericUpDown numericUpDown11;

        private NumericUpDown numericUpDown10;

        private NumericUpDown numericUpDown9;

        private NumericUpDown numericUpDown8;

        private Label label23;

        private Label label25;

        private TextBox textBox9;

        private TextBox textBox11;

        private TextBox textBox10;

        private TextBox TxtBotRidingCrop;

        private TextBox TxtBotStarTower;

        private TextBox TxtBotHeavenFan;

        private TextBox TxtBotBoots;

        private TextBox TxtBotRightWeapon;

        private TextBox TxtBotLeftWeapon;

        private TextBox TxtBotArmors;

        private GroupBox groupBox13;

        private TextBox textBox20;

        private TextBox textBox21;

        private TextBox textBox24;

        private TextBox textBox19;

        private TextBox textBox22;

        private Label label24;

        private Label label33;

        private Label label43;

        private TextBox textBox28;

        private Label label42;

        private Label label41;

        private TextBox textBox26;

        private Label label39;

        private Label label38;

        private Label label37;

        private Label label45;

        private Label label44;

        private Button button47;

        private Button button50;

        private Label label32;

        private Label label46;

        private Label label62;

        private ListBox listBox2;

        private ComboBox comboBox11;

        private CheckBox checkBox4;

        private NumericUpDown numericUpDown3;

        private ComboBox comboBox12;

        private TabPage tabPage7;

        private GroupBox groupBox14;

        private CheckBox checkBox5;

        private Button button52;

        private Label label40;

        private TextBox textBox25;

        private Label label52;

        private TextBox textBox35;

        private Label label51;

        private TextBox textBox34;

        private Label label50;
        private Label label505;

        private TextBox textBox33;
        private TextBox textBox330;

        private Label label48;

        private TextBox textBox31;

        private Label label49;

        private TextBox textBox32;

        private Label label47;

        private TextBox textBox27;

        private TextBox textBox40;

        private Label label57;

        private Label label56;

        private TextBox textBox37;

        private Label label55;

        private Label label54;

        private TextBox textBox38;

        private TextBox textBox39;

        private Label label53;

        private TextBox textBox36;

        private GroupBox groupBox15;

        private TextBox textBox42;

        private Label label59;

        private TextBox textBox43;

        private Label label60;

        private Label label73;

        private TextBox textBox44;

        private Label label78;

        private Label label88;

        private TextBox textBox45;

        private TextBox textBox46;

        private TextBox textBox47;

        private Label label91;

        private TextBox textBox48;

        private Label label92;

        private TextBox textBox49;

        private Label label93;

        private TextBox textBox50;

        private Label label94;

        private TextBox textBox51;

        private Label label95;

        private TextBox textBox52;

        private TextBox textBox53;

        private Label label97;

        private TextBox textBox54;

        private Label label98;

        private TextBox textBox55;

        private Label label99;

        private TextBox textBox56;

        private Label label108;

        private TextBox textBox65;

        private Label label109;

        private TextBox textBox66;

        private Label label110;

        private TextBox textBox67;

        private Label label111;

        private TextBox textBox68;

        private Label label106;

        private TextBox textBox63;

        private Label label107;

        private TextBox textBox64;

        private Label label104;

        private TextBox textBox61;

        private Label label105;

        private TextBox textBox62;

        private Label label101;

        private TextBox textBox58;

        private Label label102;

        private TextBox textBox59;

        private Label label100;

        private TextBox textBox57;

        private TextBox textBox75;

        private Label label119;

        private TextBox textBox76;

        private Label label112;

        private TextBox textBox69;

        private Label label113;

        private TextBox textBox70;

        private Label label114;

        private TextBox textBox71;

        private Label label115;

        private TextBox textBox72;

        private Label label58;

        private TextBox textBox41;

        private Label label103;

        private TextBox textBox60;

        private TextBox textBox74;

        private Label label116;

        private TextBox textBox73;

        private Label label120;

        private TextBox textBox77;

        private TextBox textBox78;

        private Button button54;

        private TabPage tabPage9;

        private TextBox textBox84;

        private TextBox textBox83;

        private TextBox textBox82;

        private TextBox textBox81;

        private TextBox textBox80;

        private CheckBox checkBox6;

        private Button button55;

        private Label label123;

        private Label label124;

        private Label label125;

        private Label label127;

        private Label label128;

        private TextBox textBox85;

        private TextBox textBox86;

        private Label label129;

        private CheckBox checkBox7;

        private TextBox textBox87;

        private Label label131;

        private CheckBox checkBox8;

        private TextBox textBox89;

        private Label label132;

        private TextBox textBox90;

        private Label label133;

        private TextBox textBox91;

        private Label label134;

        private TextBox textBox92;

        private Label label135;

        private TextBox textBox93;

        private TextBox textBox94;

        private TextBox textBox95;

        private Label label138;

        private DateTimePicker dateTimePicker1;

        private TextBox textBox96;

        private Label label139;

        private TabPage tabPage10;

        private DataGridView dataGridView1;

        private Button button58;

        private GroupBox groupBox16;

        private Button button59;

        private TextBox textBox98;

        private Label label142;

        private TextBox textBox97;

        private Label label141;

        private GroupBox groupBox17;

        private TextBox textBox104;

        private Label label148;

        private TextBox textBox103;

        private Label label147;

        private TextBox textBox101;

        private Label label145;

        private TextBox textBox102;

        private Label label146;

        private TextBox textBox99;

        private Label label143;

        private TextBox textBox100;

        private Label label144;

        private Button button62;

        private TextBox textBox107;

        private TextBox textBox105;

        private TextBox textBox106;

        private Label label150;

        private PictureBox pictureBox3;

        private Button button63;

        private Label label96;

        private Label label149;

        private TextBox textBox108;

        private Label label151;

        private TextBox textBox109;

        private TextBox textBox110;

        private Label label152;

        private TabPage tabPage11;

        private Label label156;

        private Button button69;

        private Label label170;

        private Button button68;

        private ComboBox comboBox14;

        private DataGridView dataGridView2;

        private PictureBox pictureBox4;

        private DataGridView dataGridView4;

        private Label label171;

        private Button button65;

        private TextBox textBox88;

        private Label label130;

        private Label label153;

        private StatusStrip statusStrip1;

        private ToolStripStatusLabel toolStripStatusLabel1;

        private ToolStripStatusLabel toolStripStatusLabel2;

        private ToolStripStatusLabel toolStripStatusLabel3;

        private ToolStripStatusLabel toolStripStatusLabel4;

        private ToolStripProgressBar toolStripProgressBar1;

        private CheckBox checkBox9;

        private Label label154;

        private Button button60;

        private PictureBox pictureBox5;

        private CheckBox checkBox10;

        private TrackBar trackBar1;

        private Button button26;

        private Button button23;

        private Label label157;

        private Label label155;

        private DataGridView dataGridView3;

        private Label label36;

        private ListBox listBox3;

        private Button button61;

        private Button button30;

        private DataGridView dataGridView5;

        private Label label140;

        private Label label159;

        private Label label158;

        private Label label160;

        private NumericUpDown numericUpDown21;

        private Button button51;

        private Button button53;

        private Button button56;

        private PictureBox pictureBox7;

        private PictureBox pictureBox8;

        private ComboBox comboBox13;

        private RadioButton ReqLev120;

        private RadioButton ReqLev140;

        private RadioButton ReqLev100;

        private RadioButton ReqLev70;

        private ComboBox comboBox15;

        private PictureBox pictureBox6;

        private Label label121;

        private Button button57;

        private ComboBox RebornBox;

        private CheckBox GemsChecked;

        private CheckBox checkBox11;

        private CheckBox checkBox12;

        private Label label117;

        private Label label126;

        private CheckBox checkBox13;

        private Label label90;

        private Button button67;

        private TextBox textBox12;
        private Button button64;
        private Button button66;
        private CheckBox checkBox14;
        private TabPage tabPage8;

        public ServerManager()
        {
            InitializeComponent();
            GetController(Enable.Hide);
            base.Load += Main;
        }

        public void GetController(Enable typ)
        {
            if (typ == Enable.Show)
            {
                button29.Enabled = true;
                button27.Enabled = true;
                tabControl1.Enabled = true;
                LbxCharacters.Enabled = true;
                button28.Enabled = true;
                button35.Enabled = true;
                TextBox textBox;
                textBox = textBox6;
                TextBox textBox2;
                textBox2 = this.textBox3;
                string text2;
                text2 = (TxtConnectIp.Text = "*******");
                string text4;
                text4 = (textBox2.Text = text2);
                textBox.Text = text4;
                NumPlayerCount.Visible = true;
                NumPlayerCount.Enabled = false;
                NumericUpDown numPlayerCount;
                numPlayerCount = NumPlayerCount;
                TextBox textBox3;
                textBox3 = textBox6;
                Button btnStart;
                btnStart = BtnStart;
                TextBox textBox4;
                textBox4 = this.textBox3;
                bool flag2;
                flag2 = (TxtConnectIp.Enabled = false);
                bool flag4;
                flag4 = (textBox4.Enabled = flag2);
                bool flag6;
                flag6 = (btnStart.Enabled = flag4);
                bool enabled;
                enabled = (textBox3.Enabled = flag6);
                numPlayerCount.Enabled = enabled;
            }
            else
            {
                button29.Enabled = false;
                button27.Enabled = false;
                tabControl1.Enabled = false;
                LbxCharacters.Enabled = false;
                button28.Enabled = false;
                button35.Enabled = false;
                trackBar1.Enabled = false;
            }
        }

        public void GetDefault(MainFlag ID, GameClient client = null)
        {
            try
            {
                switch (ID)
                {
                    case MainFlag.Botslist:
                        listBox2.Items.Clear();
                        listBox2.Text = "";
                        {
                            foreach (GameClient item in Server.GamePoll.Values)
                            {
                                if (item.Fake)
                                    listBox2.Items.Add(item.Player.Name ?? "");
                            }
                            break;
                        }
                    case MainFlag.Userslist:
                        {
                            if (client == null)
                                break;
                            comboBox10.Items.Clear();
                            comboBox10.Text = "";
                            MsgGameItem[] items;
                            items = client.Inventory.ClientItems.Values.ToArray();
                            MsgGameItem[] array;
                            array = items;
                            foreach (MsgGameItem item2 in array)
                            {
                                if (item2 != null)
                                    comboBox10.Items.Add($"{Server.ItemsBase[item2.ITEM_ID].Name} {item2.ITEM_ID}");
                            }
                            comboBox9.Items.Clear();
                            comboBox9.Text = "";
                            MsgSpell[] spells;
                            spells = client.MySpells.ClientSpells.Values.ToArray();
                            MsgSpell[] array2;
                            array2 = spells;
                            foreach (MsgSpell spell in array2)
                            {
                                comboBox9.Items.Add($"{(Flags.SpellID)spell.ID} {spell.ID} {spell.Level}");
                            }
                            break;
                        }
                    case MainFlag.Bannedlist:
                        {
                            comboBox5.Items.Clear();
                            ComboBox comboBox3;
                            comboBox3 = comboBox5;
                            TextBox textBox;
                            textBox = textBox4;
                            TextBox textBox2;
                            textBox2 = textBox5;
                            string text6;
                            text6 = (textBox7.Text = "");
                            string text2;
                            text2 = (textBox2.Text = text6);
                            string text4;
                            text4 = (textBox.Text = text2);
                            comboBox3.Text = text4;
                            {
                                foreach (SystemBannedAccount.Client p in SystemBannedAccount.BannedPoll.Values)
                                {
                                    comboBox5.Items.Add(p.Name);
                                }
                                break;
                            }
                        }
                    case MainFlag.BannedPClist:
                        comboBox6.Items.Clear();
                        comboBox6.Text = "";
                        {
                            foreach (SystemBannedPC.Client p2 in SystemBannedPC.BannedPoll.Values)
                            {
                                comboBox6.Items.Add(p2.MACAdress);
                            }
                            break;
                        }
                    case MainFlag.ItemsBaselist:
                        {
                            this.comboBox2.Items.Clear();
                            this.comboBox3.Items.Clear();
                            comboBox4.Items.Clear();
                            ComboBox comboBox;
                            comboBox = this.comboBox2;
                            ComboBox comboBox2;
                            comboBox2 = this.comboBox3;
                            string text2;
                            text2 = (comboBox4.Text = "");
                            string text4;
                            text4 = (comboBox2.Text = text2);
                            comboBox.Text = text4;
                            {
                                foreach (ItemType.DBItem p3 in Server.ItemsBase.Values)
                                {
                                    if (p3 != null)
                                    {
                                        this.comboBox3.Items.Add(p3.Name + " " + p3.ID);
                                        if (ItemType.RareAccessories.Contains(p3.ID))
                                            comboBox4.Items.Add(p3.Name + " " + p3.ID);
                                        if (ItemType.RareGarments.Contains(p3.ID))
                                            this.comboBox2.Items.Add(p3.Name + " " + p3.ID);
                                    }
                                }
                                break;
                            }
                        }
                    case MainFlag.Titleslist:
                        comboBox8.Items.Clear();
                        comboBox8.Text = "";
                        {
                            foreach (SpecialTitles item4 in TheChosenProject.Database.SpecialTitles.Titles.Values)
                            {
                                comboBox8.Items.Add($"[{item4.Time}Minutes] {item4.Name} {item4.ID}");
                            }
                            break;
                        }
                    case MainFlag.Tournamentlist:
                        listBox1.Items.Clear();
                        listBox1.Text = "";
                        {
                            foreach (KeyValuePair<ushort, string> item3 in ITournamentsAlive.Tournments)
                            {
                                listBox1.Items.Add($"{item3.Key} {(ITournamentsAlive.ID)item3.Key} {item3.Value}");
                            }
                            break;
                        }
                    case MainFlag.Configrationlist:
                        checkBox5.Checked = ServerKernel.Test_Center;
                        checkBox12.Checked = ServerKernel.MonsterFromText;
                        checkBox14.Checked = ServerKernel.MonsterDBSpawn;
                        checkBox9.Checked = TestUser;
                        checkBox13.Checked = ServerKernel.Allow_Server_Translate;
                        textBox25.Text = ServerKernel.CHANCE_LETTERS.ToString();
                        textBox27.Text = ServerKernel.CHANCE_PLUS_ONE.ToString();
                        textBox32.Text = ServerKernel.CHANCE_METEOR.ToString();
                        textBox31.Text = ServerKernel.CHANCE_GEMS.ToString();
                        textBox33.Text = ServerKernel.CHANCE_EXP.ToString();
                        textBox330.Text = ServerKernel.CHANCE_Key.ToString();
                        textBox39.Text = ServerKernel.CHANCE_STONE_ONE_ITEM.ToString();
                        textBox37.Text = ServerKernel.CHANCE_STONE_TWO_ITEM.ToString();
                        textBox40.Text = ServerKernel.CHANCE_DRAGONBALL_ITEM.ToString();
                        textBox34.Text = ServerKernel.SPELL_RATE.ToString();
                        textBox35.Text = ServerKernel.PROF_RATE.ToString();
                        textBox36.Text = ServerKernel.EXP_RATE.ToString();
                        textBox38.Text = ServerKernel.MAX_USER_LOGIN_ON_PC.ToString();
                        textBox54.Text = ServerKernel.CP_MONSTER_DROP_RATE[0].ToString();
                        textBox53.Text = ServerKernel.CP_MONSTER_DROP_RATE[1].ToString();
                        textBox52.Text = ServerKernel.NAME_CHANGE.ToString();
                        textBox51.Text = ServerKernel.NAME_CHANGE_RESET_LIMIT.ToString();
                        textBox50.Text = ServerKernel.GENDAR_CHANGE.ToString();
                        textBox46.Text = ServerKernel.CREATE_CLAN.ToString();
                        textBox44.Text = ServerKernel.STAY_ONLINE.ToString();
                        textBox43.Text = ServerKernel.CONQUER_LETTER_REWARD.ToString();
                        textBox42.Text = ServerKernel.TWO_SOC_RATE.ToString();
                        textBox49.Text = ServerKernel.MONTHLY_PK_REWARD.ToString();
                        textBox48.Text = ServerKernel.QUALIFIER_PK_REWARD.ToString();
                        textBox47.Text = ServerKernel.TWO_SOC_RATE.ToString();
                        textBox45.Text = ServerKernel.ELITE_GUILD_WAR_Reward.ToString();
                        textBox56.Text = ServerKernel.GUILD_WAR_REWARD.ToString();
                        textBox55.Text = ServerKernel.CLASS_PK_WAR_REWARD.ToString();
                        textBox57.Text = ServerKernel.CLASSIC_CLAN_WAR_REWARD.ToString();
                        textBox59.Text = ServerKernel.CAPTURE_THE_FLAG_WAR_REWARD_MONEY.ToString();
                        textBox58.Text = ServerKernel.CAPTURE_THE_FLAG_WAR_REWARD_CPS.ToString();
                        textBox41.Text = ServerKernel.ONE_SOC_RATE.ToString();
                        textBox68.Text = ServerKernel.TEAM_PK_TOURNAMENT_REWARD[0].ToString();
                        textBox67.Text = ServerKernel.TEAM_PK_TOURNAMENT_REWARD[1].ToString();
                        textBox66.Text = ServerKernel.TEAM_PK_TOURNAMENT_REWARD[2].ToString();
                        textBox65.Text = ServerKernel.TEAM_PK_TOURNAMENT_REWARD[3].ToString();
                        textBox62.Text = ServerKernel.ELITE_PK_TOURNAMENT_REWARD[0].ToString();
                        textBox61.Text = ServerKernel.ELITE_PK_TOURNAMENT_REWARD[1].ToString();
                        textBox64.Text = ServerKernel.ELITE_PK_TOURNAMENT_REWARD[2].ToString();
                        textBox63.Text = ServerKernel.ELITE_PK_TOURNAMENT_REWARD[3].ToString();
                        textBox72.Text = ServerKernel.SKILL_PK_TOURNAMENT_REWARD[0].ToString();
                        textBox71.Text = ServerKernel.SKILL_PK_TOURNAMENT_REWARD[1].ToString();
                        textBox70.Text = ServerKernel.SKILL_PK_TOURNAMENT_REWARD[2].ToString();
                        textBox69.Text = ServerKernel.SKILL_PK_TOURNAMENT_REWARD[3].ToString();
                        textBox60.Text = ServerKernel.KILLER_SYSTEM_REWARD[1].ToString();
                        textBox76.Text = ServerKernel.KILLER_SYSTEM_REWARD[2].ToString();
                        textBox75.Text = ServerKernel.KILLER_SYSTEM_REWARD[3].ToString();
                        textBox74.Text = ServerKernel.KILLER_SYSTEM_REWARD[4].ToString();
                        textBox77.Text = ServerKernel.TREASURE_THIEF_MAX.ToString();
                        textBox85.Text = ServerKernel.TREASURE_THIEF_MIN.ToString();
                        textBox73.Text = ServerKernel.MAXIMUM_LETTER_DAILY_TIMES.ToString();
                        textBox86.Text = ServerKernel.AWARED_EXPERINCE_FROM_BOT.ToString();
                        textBox87.Text = ServerKernel.Max_PLUS.ToString();
                        textBox89.Text = ServerKernel.Max_Bless.ToString();
                        textBox90.Text = ServerKernel.Max_Enchant.ToString();
                        textBox91.Text = ServerKernel.MAX_UPLEVEL.ToString();
                        textBox92.Text = ServerKernel.Bound_Equipments_Plus.ToString();
                        textBox95.Text = ServerKernel.ARENA_DAILY_RANKING[0].ToString();
                        textBox94.Text = ServerKernel.ARENA_DAILY_RANKING[1].ToString();
                        textBox93.Text = ServerKernel.ARENA_DAILY_RANKING[2].ToString();
                        textBox108.Text = ServerKernel.MONEY_MONSTER_DROP_RATE[1].ToString();
                        textBox109.Text = ServerKernel.MONEY_MONSTER_DROP_RATE[0].ToString();
                        textBox110.Text = ServerKernel.MONSTER_SPWANS.ToString();
                        checkBox8.Checked = ServerKernel.AutoMaintenance;
                        textBox96.Text = ServerKernel.ViewThreshold.ToString();
                        dateTimePicker1.MinDate = DateTime.Now;
                        dateTimePicker1.MaxDate = new DateTime(2024, 1, 1);
                        RefrshBeforeUpdate = true;
                        break;
                    case MainFlag.Resetlist:
                        {
                            dataGridView4.Rows.Clear();
                            dataGridView4.Columns.Clear();
                            dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dataGridView4.ColumnCount = 6;
                            dataGridView4.Columns[0].Name = "DonationNobility%";
                            dataGridView4.Columns[1].Name = "ConquerPoints%";
                            dataGridView4.Columns[2].Name = "Money%";
                            dataGridView4.Columns[3].Name = "WHMoney%";
                            dataGridView4.Columns[4].Name = "OnlinePoints%";
                            dataGridView4.Columns[5].Name = "ChampionPoints%";
                            dataGridView4.Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Italic);
                            string[] nobility;
                            nobility = new string[6] { "0", "0", "0", "0", "0", "0" };
                            DataGridViewRowCollection rows4;
                            rows4 = dataGridView4.Rows;
                            object[] values;
                            values = nobility;
                            rows4.Add(values);
                            break;
                        }
                    case MainFlag.Bosseslist:
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dataGridView1.ColumnCount = 17;
                            dataGridView1.Columns[0].Name = "ID";
                            dataGridView1.Columns[1].Name = "Name";
                            dataGridView1.Columns[2].Name = "StudyPts";
                            dataGridView1.Columns[3].Name = "CPS";
                            dataGridView1.Columns[4].Name = "Soul(level)";
                            dataGridView1.Columns[5].Name = "Soul(count)";
                            dataGridView1.Columns[6].Name = "Matrial(Level)";
                            dataGridView1.Columns[7].Name = "Matrial(count)";
                            dataGridView1.Columns[8].Name = "Items(id)";
                            dataGridView1.Columns[9].Name = "Ranks(CPS)";
                            dataGridView1.Columns[10].Name = "Ranks(Point)";
                            dataGridView1.Columns[11].Name = "Ranks(Item)";
                            dataGridView1.Columns[12].Name = "MapID";
                            dataGridView1.Columns[13].Name = "X";
                            dataGridView1.Columns[14].Name = "Y";
                            dataGridView1.Columns[15].Name = "Hour";
                            dataGridView1.Columns[16].Name = "Minute";
                            dataGridView1.Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Italic);
                            string[] row2;
                            row2 = new string[17];
                            {
                                foreach (Boss bkh2 in BossDatabase.Bosses.Values)
                                {
                                    if (bkh2 != null)
                                    {
                                        row2[0] = bkh2.MonsterID.ToString();
                                        row2[1] = bkh2.Name.ToString();
                                        row2[2] = bkh2.StudyPoints.ToString();
                                        row2[3] = bkh2.ConquerPointDropped.ToString();
                                        row2[4] = bkh2.SoulDropped.ToString();
                                        row2[5] = bkh2.MaxSoulDropped.ToString();
                                        row2[6] = bkh2.RefinaryDropped.ToString();
                                        row2[7] = bkh2.MaxRefienryDropped.ToString();
                                        row2[8] = string.Join(",", bkh2.Items);
                                        row2[9] = bkh2.ConquerPointScores.ToString();
                                        row2[10] = bkh2.BossPointScores.ToString();
                                        row2[11] = bkh2.ItemDropScores.ToString();
                                        row2[12] = bkh2.MapID.ToString();
                                        row2[13] = string.Join(",", bkh2.X);
                                        row2[14] = string.Join(",", bkh2.Y);
                                        row2[15] = string.Join(",", bkh2.SpawnHours).ToString();
                                        row2[16] = string.Join(",", bkh2.SpawnMinutes).ToString();
                                        dataGridView1.Rows.Add(row2);
                                    }
                                }
                                break;
                            }
                        }
                    case MainFlag.TournamentTimerlist:
                        {
                            dataGridView2.Rows.Clear();
                            dataGridView2.Columns.Clear();
                            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dataGridView2.ColumnCount = 7;
                            dataGridView2.Columns[0].Name = "ID";
                            dataGridView2.Columns[1].Name = "Schedule";
                            dataGridView2.Columns[2].Name = "(Start)Day";
                            dataGridView2.Columns[3].Name = "(Start)Hour";
                            dataGridView2.Columns[4].Name = "(Start)Minute";
                            dataGridView2.Columns[5].Name = "(End)Hour";
                            dataGridView2.Columns[6].Name = "(End)Minute";
                            dataGridView2.Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Italic);
                            string[] row3;
                            row3 = new string[7];
                            {
                                foreach (ISchedule bkh3 in ISchedule.Schedules.Values)
                                {
                                    if (bkh3 != null)
                                    {
                                        uint id2;
                                        id2 = (uint)bkh3.ID;
                                        row3[0] = id2.ToString();
                                        row3[1] = bkh3.Name.ToString();
                                        row3[2] = bkh3.StartDay.ToString();
                                        row3[3] = bkh3.StartHour.ToString();
                                        row3[4] = bkh3.StartMinute.ToString();
                                        row3[5] = bkh3.EndHour.ToString();
                                        row3[6] = bkh3.EndMinute.ToString();
                                        DataGridViewRowCollection rows2;
                                        rows2 = dataGridView2.Rows;
                                        object[] values;
                                        values = row3;
                                        rows2.Add(values);
                                    }
                                }
                                break;
                            }
                        }
                    case MainFlag.TournamentPrizes:
                        {
                            dataGridView5.Rows.Clear();
                            dataGridView5.Columns.Clear();
                            dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dataGridView5.ColumnCount = 5;
                            dataGridView5.Columns[0].Name = "ID";
                            dataGridView5.Columns[1].Name = "Schedule";
                            dataGridView5.Columns[2].Name = "ItemReward(1)";
                            dataGridView5.Columns[3].Name = "ItemReward(2)";
                            dataGridView5.Columns[4].Name = "ItemReward(3)";
                            dataGridView5.Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Italic);
                            string[] row;
                            row = new string[5];
                            {
                                foreach (ISchedule bkh in ISchedule.Schedules.Values)
                                {
                                    if (bkh != null)
                                    {
                                        uint id;
                                        id = (uint)bkh.ID;
                                        row[0] = id.ToString();
                                        row[1] = bkh.Name.ToString();
                                        row[2] = bkh.ItemOne.ToString();
                                        row[3] = bkh.ItemTwo.ToString();
                                        row[4] = bkh.ItemThree.ToString();
                                        DataGridViewRowCollection rows;
                                        rows = dataGridView5.Rows;
                                        object[] values;
                                        values = row;
                                        rows.Add(values);
                                    }
                                }
                                break;
                            }
                        }
                    case MainFlag.Alllist:
                        GetDefault(MainFlag.Bannedlist);
                        GetDefault(MainFlag.BannedPClist);
                        GetDefault(MainFlag.ItemsBaselist);
                        GetDefault(MainFlag.Titleslist);
                        GetDefault(MainFlag.Tournamentlist);
                        GetDefault(MainFlag.Bosseslist);
                        GetDefault(MainFlag.Resetlist);
                        GetDefault(MainFlag.Configrationlist);
                        GetDefault(MainFlag.TournamentTimerlist);
                        GetDefault(MainFlag.TournamentPrizes);
                        break;
                }
            }
            catch
            {
            }
        }

        public void AppendLog(string message, Logger type)
        {
            try
            {
                switch (type)
                {
                    case Logger.Main:
                        TxtLogger.AppendText(message + Environment.NewLine);
                        break;
                    case Logger.Cheater:
                        textBox78.AppendText(message + Environment.NewLine);
                        break;
                }
            }
            catch
            {
            }
        }

        public static bool GetLoginStatus(int port)
        {
            bool inUse;
            inUse = false;
            IPGlobalProperties ipProperties;
            ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints;
            ipEndPoints = ipProperties.GetActiveTcpListeners();
            IPEndPoint[] array;
            array = ipEndPoints;
            foreach (IPEndPoint endPoint in array)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }

        public void WindowTitle(string title, bool once = true)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        this.Text = title;
                        if (!once)
                            return;
                        if (OnLogged)
                        {
                            toolStripProgressBar1.Maximum = KernelThread.MaxOnline;
                            toolStripProgressBar1.Value = KernelThread.Online;
                            toolStripStatusLabel2.ForeColor = Color.Green;
                            toolStripStatusLabel2.Text = "Online";
                        }
                        if (GetLoginStatus(ServerKernel.LoginServerPort))
                        {
                            if (toolStripStatusLabel4.Text == "Offline")
                                ServerKernel.Log.SaveLog($"Server [{ServerKernel.ServerName}] has connected with the following information>\r\n\tPort: {ServerKernel.LoginServerPort}, Online: {KernelThread.Online}, MaxOnline: {KernelThread.MaxOnline}", true, LogType.DEBUG);
                            toolStripStatusLabel4.ForeColor = Color.Green;
                            toolStripStatusLabel4.Text = "Online";
                            if (trackBar1.Value >= 50)
                                trackBar1.Value = 1;
                        }
                        else
                        {
                            toolStripStatusLabel4.ForeColor = Color.Red;
                            toolStripStatusLabel4.Text = "Offline";
                            ServerKernel.Log.SaveLog($"Waiting for server [{ServerKernel.LoginServerAddress}] to accept connection.", true, LogType.WARNING);
                        }
                    }));
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("faild to update window title", true, LogType.WARNING);
            }
        }
        public void LoadingCharacters()
        {
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client != null && !client.Fake)
                    AddUser(client.Player);
            }
        }
        delegate void _AddUser(Player user);
        public void AddUser(Player user)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new _AddUser(AddUser), new object[] { user });
                }
                else
                {
                    if (!LbxCharacters.Items.Contains(user.Name.ToString()))
                        LbxCharacters.Items.Add(user.Name.ToString());
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("faild to add user in character list!", true, LogType.WARNING);
            }
        }

        delegate void _RemoveUser(uint iduser);
        public void RemoveUser(uint idUser)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new _RemoveUser(RemoveUser), new object[] { idUser });
                }
                else
                {
                tryagain:
                    int removeIndex;
                    removeIndex = -1;
                    for (int i = 0; i < LbxCharacters.Items.Count; i++)
                    {
                        string name;
                        name = LbxCharacters.Items[i].ToString();
                        GameClient client;
                        client = GameClient.CharacterFromName(name);
                        if (client == null)
                        {
                            removeIndex = i;
                            if (removeIndex >= 0 && removeIndex < LbxCharacters.Items.Count)
                                LbxCharacters.Items.RemoveAt(removeIndex);
                            comboBox10.Items.Clear();
                            comboBox9.Items.Clear();
                            FillUserInformationEmpty();
                            goto tryagain;
                        }
                        if (client == null || client.Fake)
                            continue;
                        if (client.Player.UID == idUser)
                        {
                            removeIndex = i;
                            break;
                        }
                    }
                    if (removeIndex >= 0 && removeIndex < LbxCharacters.Items.Count)
                        LbxCharacters.Items.RemoveAt(removeIndex);
                    comboBox10.Items.Clear();
                    comboBox9.Items.Clear();
                    FillUserInformationEmpty();
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("faild to remove user from character list!", true, LogType.WARNING);
            }
        }
        public void Main(object sender, EventArgs e)
        {
            try
            {
                Console.DissableButton();
                Ai.DataVendor.AddPlaceMarket();

                AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
                Packet.SealString = "TQServer";
                ServerKernel.Log = new LogWriter(Environment.CurrentDirectory + "\\");
                WindowTitle("Starting server...", false);
                ServerKernel.Log.SaveLog("\tProject UAConquer Conquer: Conquer Online Private Server", true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("\t\tDeveloped by Siege)");
                ServerKernel.Log.SaveLog("\t\tMarch 2nd, 2024 - All Rights Reserved\n\n", true, LogType.DEBUG);
                AppendLog("", Logger.Main);
                ServerKernel.Log.SaveLog(Environment.CurrentDirectory, true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("Computer Name: " + Environment.MachineName, true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("User Name: " + Environment.UserName, true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("User Address: " + Program.MyIP, true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("System Directory: " + Environment.SystemDirectory, true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("Some environment variables:", true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("OS=" + Environment.OSVersion, true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("NUMBER_OF_PROCESSORS: " + Environment.ProcessorCount, true, LogType.DEBUG);
                ServerKernel.Log.SaveLog("PROCESSOR_ARCHITETURE: " + (Environment.Is64BitProcess ? "x64" : "x86"), true, LogType.DEBUG);
                AppendLog("", Logger.Main);
                ServerKernel.Log.SaveLog("Initializing game server...", true);
                ServerKernel.Log.SaveLog("Initializing Database", true);
                Program.MsgInvoker = new CachedAttributeInvocation<Action<GameClient, Packet>, PacketAttribute, ushort>(PacketAttribute.Translator);
                MsgSchedules.Create();
                Server.Initialize();
                Ai.DataVendor.Load();
                ServerKernel.Log.SaveLog("Initializing GlobalPackets", true);
                Program.SendGlobalPackets = new SendGlobalPacket();
                AuthCryptography.PrepareAuthCryptography();
                ServerKernel.Log.SaveLog("Initializing blowfish", true);
                TransferCipher.Key = Encoding.ASCII.GetBytes("EypKhLvYJ3zdLCTyz9Ak8RAgM78tY5F32b7CUXDuLDJDFBH8H67BWy9QThmaN5VS"); // EypKhLvYJ3zdLCTyz9Ak8RAgM78tY5F32b7CUXDuLDJDFBH8H67BWy9QThmaN5VS
                TransferCipher.Salt = Encoding.ASCII.GetBytes("MyqVgBf3ytALHWLXbJxSUX4uFEu3Xmz2UAY9sTTm8AScB7Kk2uwqDSnuNJske4BJ"); //MyqVgBf3ytALHWLXbJxSUX4uFEu3Xmz2UAY9sTTm8AScB7Kk2uwqDSnuNJske4BJ
                Program.transferCipher = new TransferCipher("26.23.75.141");


                CMsgGuardShield.MsgGuardShield.Load(ServerKernel.LoginServerAddress, true);

                ServerKernel.Log.SaveLog("Starting sockets", true);
                ServerKernel.Log.SaveLog("Starting server socket...");

                try
                {
                    Program.TheChosenProject = new ServerSocket(delegate (SecuritySocket p)
                    {
                        new GameClient(p);
                    }, Program.Game_Receive, Program.Game_Disconnect);
                    Program.TheChosenProject.Initilize(ServerKernel.Port_SendSize, ServerKernel.Port_ReceiveSize);
                    Program.TheChosenProject.Open(ServerKernel.GameServerPort);
                    Program.SocketsGroup = new SocketPoll("ConquerServer", Program.TheChosenProject);
                    new KernelThread(300, "ConquerServer2").Start();
                    new MapGroupThread(100, "ConquerServer3").Start();
                    ServerKernel.Log.SaveLog("Server is ready for connections...", true, LogType.MESSAGE);
                }
                catch (Exception ex)
                {
                    ServerKernel.Log.SaveLog("Could not open game socket on port " + ServerKernel.GameServerPort, true, LogType.WARNING);
                    ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
                    Environment.Exit(-1);
                }
                ServerKernel.Log.SaveLog("Starting threads");

                Program.GlobalItems = new ShowChatItems();
                Program.VoteRank = new VoteSystem.VoteRank();//bahaa
                foreach (GameClient client in Server.GamePoll.Values)
                {
                    if (client.Player.ChampionPoints != 0)
                        Console.WriteLine("" + client.Player.Name + " | " + client.Player.UID + " | " + client.Player.ChampionPoints + "");
                }
                //new KernelThread(1000, "ConquerServer2").Start();
                //new MapGroupThread(300, "ConquerServer3").Start();
                AppendLog("", Logger.Main);
                //Program.DiscordDIBAPI.Enqueue($"``The game server is now online, you can login!``");
                BtnStart_Click(sender, e);
              
                this.Hide();
                //for (int i = 0; i < 3; i++)
                //    MsgSchedules.SpawnLavaBeast();
            }
            catch (Exception x)
            {
                ServerKernel.Log.SaveLog(x.ToString(), false, LogType.EXCEPTION);
            }
        }

        public uint Hunters()
        {
            uint hunters;
            hunters = 0u;
            foreach (GameClient user in Server.GamePoll.Values)
            {
                if (user.Player.ContainFlag(MsgUpdate.Flags.AutoHunting))
                    hunters++;
            }
            return hunters;
        }

        public uint Bots()
        {
            uint bot;
            bot = 0u;
            foreach (GameClient user in Server.GamePoll.Values)
            {
                if (user.Fake)
                    bot++;
            }
            return bot;
        }

        public void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                //bool flag = true;
                //if (this.TxtConnectIp.Text != Program.MyIP)
                //    flag = false;
                //if (this.textBox3.Text != ServerKernel.Allow_User)
                //    flag = false;
                //if (this.textBox6.Text != ServerKernel.Allow_Password)
                //    flag = false;
                //if (this.NumPlayerCount.Value != (Decimal)ServerKernel.Allow_Code)
                //    flag = false;
                //if (!ServerKernel.EnableServer)
                //    flag = false;
                //if (flag)
                {
                    this.GetController(ServerManager.Enable.Show);
                    this.GetDefault(ServerManager.MainFlag.Alllist);
                    DateTime now = DateTime.Now;
                    TimeSpan timeSpan1 = new TimeSpan(ServerKernel.StartDate.ToBinary());
                    TimeSpan timeSpan2 = new TimeSpan(now.ToBinary());
                    this.OnLogged = true;
                    LogWriter log = ServerKernel.Log;
                    string[] strArray = new string[7];
                    strArray[0] = "The server was started ";
                    int num = (int)(timeSpan2.TotalHours - timeSpan1.TotalHours);
                    strArray[1] = num.ToString();
                    strArray[2] = " hours, ";
                    num = (int)((timeSpan2.TotalMinutes - timeSpan1.TotalMinutes) % 60.0);
                    strArray[3] = num.ToString();
                    strArray[4] = " minutes, ";
                    num = (int)((timeSpan2.TotalSeconds - timeSpan1.TotalSeconds) % 60.0);
                    strArray[5] = num.ToString();
                    strArray[6] = " seconds.";
                    string szMessage = string.Concat(strArray);
                    log.AppendServerLog(szMessage);
                }
                //else
                //    this.CloseServer((object)null, (EventArgs)null);
            }
            catch (Exception ex)
            {
                ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
            }
        }

        private void FillUserInformationEmpty()
        {
            try
            {
                LblUserId.Text = "0";
                LblUserName.Text = "None";
                LblUserMateName.Text = "0";
                LblUserLevel.Text = "0";
                LblUserExperience.Text = "0";
                LblUserStrength.Text = "0";
                LblUserAgility.Text = "0";
                LblUserVitality.Text = "0";
                LblUserSpirit.Text = "0";
                LblUserAdditionalPoints.Text = "0";
                LblUserHealthPoints.Text = "0/0";
                LblUserManaPoints.Text = "0/0";
                LblVIPLevel.Text = "0";
                LblUserClamCP.Text = "0";
                LblChampionPoints.Text = "0";
                LblVIPTime.Text = "0";
                LblOnlinePoints.Text = "0";
                LblMoney.Text = "0";
                LblSpecialTitles.Text = "0";
                LblNewbieProtection.Text = "0";
                LblSecurityPass.Text = "0";
                LblNobilityRank.Text = "0";
                try
                {
                    pictureBox1.Image = null;
                }
                catch
                {
                    ServerKernel.Log.SaveLog("faild to found players faces folder", true, LogType.WARNING);
                }
                try
                {
                    pictureBox5.BackgroundImage = null;
                }
                catch
                {
                    ServerKernel.Log.SaveLog("faild to found players ranks folder", true, LogType.WARNING);
                }
                LblClass.Text = "0";
                LblSecondClass.Text = "0";
                LblFirstClass.Text = "0";
                LblExtraAtributes.Text = "0";
                LblConquerPoints.Text = "0";
                LblWHMoney.Text = "0";
                LblNameChange.Text = "0";
                LblGuildRank.Text = "0";
                LblGuildName.Text = "0";
                LblClanRank.Text = "0";
                LblClanName.Text = "0";
                label30.Text = "unknown";
                label28.Text = "unknown";
                CheckBoxAutoHunt.Checked = false;
                checkBox2.Checked = false;
                label35.Text = "0";
                checkBox1.Checked = false;
                CheckBoxOfflineVending.Checked = false;
                CheckBoxOfflineTraining.Checked = false;
                CheckBoxOfflineHunting.Checked = false;
                try
                {
                    textBox3.Text = "unknown";
                    textBox6.Text = "unknown";
                }
                catch
                {
                    ServerKernel.Log.SaveLog("faild to fetch accounts db information", true, LogType.WARNING);
                }
                TxtConnectIp.Text = "unknown";
                checkBox3.Checked = false;
                LblUserLocation.Text = "None (0,0)";
                LbxOnPCCount.Items.Clear();
                LbxOnScreen.Items.Clear();
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild: Clear UserInformation", true, LogType.WARNING);
            }
        }

        private void FillUserInformation(Player user)
        {
            try
            {
                LblUserId.Text = user.UID.ToString();
                LblUserName.Text = user.Name;
                LblUserMateName.Text = user.Spouse;
                LblUserLevel.Text = user.Level.ToString();
                LblUserExperience.Text = user.Experience.ToString();
                LblUserStrength.Text = user.Strength.ToString();
                LblUserAgility.Text = user.Agility.ToString();
                LblUserVitality.Text = user.Vitality.ToString();
                LblUserSpirit.Text = user.Spirit.ToString();
                LblUserAdditionalPoints.Text = user.Atributes.ToString();
                LblUserHealthPoints.Text = $"{user.HitPoints}/{user.Owner.Status.MaxHitpoints}";
                LblUserManaPoints.Text = $"{user.Mana}/{user.Owner.Status.MaxMana}";
                LblVIPLevel.Text = user.VipLevel.ToString();
                LblUserClamCP.Text = user.EmoneyPoints.ToString("0,0");
                LblChampionPoints.Text = user.ChampionPoints.ToString();
                if (user.ExpireVip > DateTime.Now)
                    LblVIPTime.Text = user.ExpireVip.ToString("d/M/yyyy (H:mm)");
                else
                    LblVIPTime.Text = "Expired";
                LblOnlinePoints.Text = user.OnlinePoints.ToString();
                LblMoney.Text = user.Money.ToString("0,0");
                Label lblSpecialTitles;
                lblSpecialTitles = LblSpecialTitles;
                MsgTitle.TitleType myTitle;
                myTitle = (MsgTitle.TitleType)user.MyTitle;
                lblSpecialTitles.Text = myTitle.ToString();
                //LblNewbieProtection.Text = user.NewbieProtection.ToString();
                LblSecurityPass.Text = user.SecurityPassword.ToString();
                LblNobilityRank.Text = $"{user.NobilityRank}(Position:{user.Nobility.Position + 1})";
                try
                {
                    pictureBox1.Image = Image.FromFile("Resources/64/" + user.Face + ".jpg");
                }
                catch
                {
                    ServerKernel.Log.SaveLog("faild to found players faces folder", true, LogType.WARNING);
                }
                try
                {
                    pictureBox5.BackgroundImage = Image.FromFile("Resources/ranks/" + user.NobilityRank.ToString() + ".png");
                }
                catch
                {
                    ServerKernel.Log.SaveLog("faild to found players ranks folder", true, LogType.WARNING);
                }
                LblClass.Text = ((Flags.ProfessionType)user.Class).ToString();
                Label lblSecondClass;
                lblSecondClass = LblSecondClass;
                Flags.ProfessionType secondClass;
                secondClass = (Flags.ProfessionType)user.SecondClass;
                lblSecondClass.Text = secondClass.ToString();
                Label lblFirstClass;
                lblFirstClass = LblFirstClass;
                secondClass = (Flags.ProfessionType)user.FirstClass;
                lblFirstClass.Text = secondClass.ToString();
                LblExtraAtributes.Text = user.ExtraAtributes.ToString();
                LblConquerPoints.Text = user.ConquerPoints.ToString("0,0");
                LblWHMoney.Text = user.WHMoney.ToString("0,0");
                LblNameChange.Text = user.NameEditCount.ToString();
                LblGuildRank.Text = user.GuildRank.ToString();
                if (user.MyGuild != null)
                    LblGuildName.Text = user.MyGuild.GuildName.ToString();
                Label lblClanRank;
                lblClanRank = LblClanRank;
                Clan.Ranks clanRank;
                clanRank = (Clan.Ranks)user.ClanRank;
                lblClanRank.Text = clanRank.ToString();
                if (user.MyClan != null)
                    LblClanName.Text = user.MyClan.Name.ToString();
                if (string.IsNullOrEmpty(user.Owner.IP))
                    label30.Text = "unknown";
                else
                    label30.Text = user.Owner.IP.ToString();
                if (string.IsNullOrEmpty(user.Owner.OnLogin.MacAddress))
                    label28.Text = "unknown";
                else
                    label28.Text = user.Owner.OnLogin.MacAddress.ToString();
                CheckBoxAutoHunt.Checked = user.IsHunting;
                checkBox2.Checked = user.Owner.IsVendor;
                switch (user.Reborn)
                {
                    case 2:
                        label35.Text = "2nd Reborn";
                        break;
                    case 1:
                        label35.Text = "1st Reborn";
                        break;
                    default:
                        label35.Text = "Nono";
                        break;
                }
                checkBox1.Checked = user.Owner.ProjectManager;
                CheckBoxOfflineVending.Checked = user.GetOfflineMode(MsgOfflineTraining.Mode.Shopping);
                CheckBoxOfflineTraining.Checked = user.GetOfflineMode(MsgOfflineTraining.Mode.TrainingGroup);
                CheckBoxOfflineHunting.Checked = user.GetOfflineMode(MsgOfflineTraining.Mode.Hunting);
                try
                {
                    textBox3.Text = user.Owner.AccountName();
                    textBox6.Text = user.Owner.AccountPassword();
                }
                catch
                {
                    ServerKernel.Log.SaveLog("faild to fetch accounts db information", true, LogType.WARNING);
                }
                TxtConnectIp.Text = user.UID.ToString();
                if (user.Map == 6003)
                    checkBox3.Checked = true;
                else
                    checkBox3.Checked = false;
                if (user.Owner.Map != null)
                    LblUserLocation.Text = $"{user.Map} ({user.X},{user.Y})";
                else
                    LblUserLocation.Text = "None (0,0)";
                GetDefault(MainFlag.Userslist, user.Owner);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild: FillUserInformation SelectedUser: " + user.Name, true, LogType.WARNING);
            }
        }

        private void FillBotInformation(Player user)
        {
            try
            {
                label61.Text = user.Name;
                label75.Text = user.Owner.Map.Name;
                label74.Text = user.X.ToString();
                label66.Text = user.Y.ToString();
                label80.Text = ((Flags.ProfessionType)user.Class).ToString();
                Label label;
                label = label63;
                string text2;
                text2 = (label81.Text = user.Level.ToString());
                label.Text = text2;
                label82.Text = user.Body.ToString();
                label83.Text = user.NobilityRank.ToString();
                label84.Text = $"{user.HitPoints}/{user.Owner.Status.MaxHitpoints}";
                label85.Text = $"{user.Mana}/{user.Owner.Status.MaxMana}";
                label86.Text = user.Reborn.ToString();
                label32.Text = user.AIBotExpire.ToString("d/M/yyyy (H:mm)");
                label46.Text = user.Owner.MySpells.ClientSpells.Count().ToString();
                label62.Text = $"{user.Owner.AIType} ({user.Owner.AIStatus})";
                if (File.Exists("Resources/64/" + user.Face + ".jpg"))
                    pictureBox2.Image = Image.FromFile("Resources/64/" + user.Face + ".jpg");
                label79.Text = user.UID.ToString();
                label87.Text = user.BattlePower.ToString();
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild: FillBotInformation SelectedBot: " + user.Name, true, LogType.WARNING);
            }
        }

        private void FillBotInformationEmpty()
        {
            try
            {
                label61.Text = "None";
                label75.Text = "0";
                label74.Text = "0";
                label66.Text = "0";
                label80.Text = "None";
                Label label;
                label = label63;
                string text2;
                text2 = (label81.Text = "0");
                label.Text = text2;
                label82.Text = "0";
                label83.Text = "0";
                label84.Text = "0/0";
                label85.Text = "0/0";
                label86.Text = "0";
                label32.Text = "0";
                label46.Text = "0";
                label62.Text = "0";
                pictureBox2.Image = Image.FromFile("Resources/64/296.jpg");
                label79.Text = "0";
                label87.Text = "0";
            }
            catch
            {
            }
        }

        private void FillBossInformationEmpty()
        {
            textBox100.Text = "0";
            textBox99.Text = "0";
            textBox102.Text = "0";
            textBox101.Text = "0";
            textBox103.Text = "0";
            textBox104.Text = "0";
            textBox105.Text = "0";
            textBox106.Text = "0";
            textBox107.Text = "0";
            try
            {
                pictureBox3.Image = null;
            }
            catch
            {
                ServerKernel.Log.SaveLog("Boss PictureID not found in Resources/boss/", true, LogType.WARNING);
            }
        }

        private void LbxCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            try
            {
                FillUserInformation(client.Player);
                BtnRefreshScreen_Click(sender, e);
                BtnRefreshPC_Click(sender, e);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild: FillUserInformation SelectedUser: " + client.Player.Name, true, LogType.WARNING);
            }
        }

        private void BtnRefreshScreen_Click(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            try
            {
                LbxOnScreen.Items.Clear();
                if (client.Player.View != null)
                {
                    foreach (IMapObj onScreen in client.Player.View.Roles(MapObjectType.Player))
                    {
                        Player user;
                        user = onScreen as Player;
                        if (!user.Owner.Fake)
                            LbxOnScreen.Items.Add(user.Name);
                    }
                }
                if (trackBar1.Value < 50 && trackBar1.Value < 50)
                    trackBar1.Value++;
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild: RefreshPC SelectedUser: " + client.Player.Name, true, LogType.WARNING);
            }
        }

        private GameClient CurrentlySelectedUser()
        {
            int selectedIndex;
            selectedIndex = LbxCharacters.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= LbxCharacters.Items.Count)
                return null;
            string Name;
            Name = LbxCharacters.Items[selectedIndex].ToString();
            GameClient client;
            client = GameClient.CharacterFromName(Name);
            if (client == null)
                return null;
            if (!client.FullLoading)
                return null;
            return client;
        }

        private GameClient CurrentlySelectedBot()
        {
            int selectedIndex;
            selectedIndex = listBox2.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= listBox2.Items.Count)
                return null;
            string Name;
            Name = listBox2.Items[selectedIndex].ToString();
            GameClient client;
            client = GameClient.CharacterFromName(Name);
            if (client == null)
                return null;
            return client;
        }

        private MonsterRole CurrentlySelectedBoss()
        {
            int selectedIndex = this.listBox3.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= this.listBox3.Items.Count)
                return (MonsterRole)null;
            string Name = this.listBox3.Items[selectedIndex].ToString();
            return Server.MonsterRole.Values.Where<MonsterRole>((Func<MonsterRole, bool>)(p => p.Name == Name && p.Alive)).FirstOrDefault<MonsterRole>() ?? (MonsterRole)null;
        }

        private void BtnDisconnectUser_Click(object sender, EventArgs e)
        {
            try
            {
                GameClient User;
                User = CurrentlySelectedUser();
                if (User != null)
                {
                    if (User.Player.OfflineTraining == MsgOfflineTraining.Mode.NotActive)
                    {
                        User.Socket.Disconnect();
                        return;
                    }
                    User.Player.OfflineTraining = MsgOfflineTraining.Mode.Completed;
                    User.Socket.Disconnect();
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to disconnect user!", true, LogType.WARNING);
            }
        }

        private void BtnRefreshPC_Click(object sender, EventArgs e)
        {
            try
            {
                GameClient client;
                client = CurrentlySelectedUser();
                if (client == null)
                    return;
                LbxOnPCCount.Items.Clear();
                foreach (string pl in MsgLoginClient.PlayersIP.Where((KeyValuePair<string, List<string>> IP) => IP.Key == client.IP).FirstOrDefault().Value)
                {
                    if (pl != null)
                        LbxOnPCCount.Items.Add(pl);
                }

                if (trackBar1.Value < 50)
                    trackBar1.Value++;
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to refresh players on pc list!", true, LogType.WARNING);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(1))
            {
                MessageBox.Show("Character must have 1 free slots into inventory");
                return;
            }
            string[] id;
            id = comboBox3.Text.Split(' ').ToArray();
            if (!Server.ItemsBase.TryGetValue(uint.Parse(id[1]), out var DBItem))
                return;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.Add(uint.Parse(id[1]), (byte)numericUpDown21.Value, DBItem, stream);
                client.SendSysMesage("The PM has added the item " + DBItem.Name + " to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ChatClient.SelectedClient = ClientList.Text;
                if (ChatClient.SelectedClient.Contains("(UnSeen)"))
                {
                    ChatClient.SelectedClient = ChatClient.SelectedClient.Remove(ChatClient.SelectedClient.Length - 8, 8);
                    ClientList.Text = ChatClient.SelectedClient;
                    ClientList.Items[ClientList.SelectedIndex] = ChatClient.SelectedClient;
                }
                GameClient getclient;
                getclient = GameClient.CharacterFromName(ChatClient.SelectedClient);
                if (getclient == null || !ServerKernel.ChatClients.TryGetValue(ChatClient.SelectedClient, out var client))
                    return;
                client.Seen = true;
                RecList.Items.Clear();
                foreach (string item in client.Mess)
                {
                    RecList.Items.Add(item);
                }
                try
                {
                    pictureBox4.Image = Image.FromFile("Resources/64/" + getclient.Player.Face + ".jpg");
                }
                catch
                {
                    ServerKernel.Log.SaveLog("faild to found players faces folder", true, LogType.WARNING);
                }
            }
            catch
            {
            }
        }

        private void RemoveFromChat(object sender, EventArgs e)
        {
            try
            {
                if (ServerKernel.ChatClients.ContainsKey(ClientList.Items[ClientList.SelectedIndex].ToString()))
                    ServerKernel.ChatClients.Remove(ClientList.Items[ClientList.SelectedIndex].ToString());
                ClientList.Items.RemoveAt(ClientList.SelectedIndex);
                RecList.Items.Clear();
                try
                {
                    pictureBox4.Image = Image.FromFile("Resources/64/0.jpg");
                }
                catch
                {
                    ServerKernel.Log.SaveLog("faild to found players faces folder", true, LogType.WARNING);
                }
            }
            catch
            {
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    if (user.Player.Name == ChatClient.SelectedClient)
                    {
                        user.SendWhisper(comboBox1.Text, "UAConquer", ChatClient.SelectedClient, 2961003u);
                        ServerKernel.ChatClients[ChatClient.SelectedClient].Mess.Add($"{DateTime.Now:d/M/yyyy (H:mm)}: UAConquer speaks to {ChatClient.SelectedClient}: {comboBox1.Text}");
                        string x;
                        x = ServerKernel.ChatClients[ChatClient.SelectedClient].Mess.Last();
                        RecList.Items.Add(x);
                        comboBox1.Text = "";
                    }
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to view last message chat box!", true, LogType.WARNING);
            }
        }

        private void MakeUserPMChecked(object sender, EventArgs e)
        {
            GameClient User;
            User = CurrentlySelectedUser();
            if (User == null)
                return;
            if (!User.Player.Name.Contains("[PM]") && checkBox1.Checked)
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream2;
                    stream2 = recycledPacket.GetStream();
                    MsgNameChange.ChangeName(User, User.Player.Name + "[PM]", true);
                    checkBox1.Checked = true;
                }
            }
            else if (User.Player.Name.Contains("[PM]") && !checkBox1.Checked)
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    checkBox1.Checked = false;
                    string PM;
                    PM = User.Player.Name.Replace("[PM]", "");
                    MsgNameChange.ChangeName(User, PM ?? "", true);
                }
            }
            if (trackBar1.Value < 50)
                trackBar1.Value++;
        }

        private void ViewMessageslist(object sender, EventArgs e)
        {
            try
            {
                ClientList.Items.Clear();
                foreach (KeyValuePair<string, ChatClient> x in ServerKernel.ChatClients)
                {
                    if (x.Value.Seen)
                        ClientList.Items.Add(x.Key);
                    else if (ChatClient.SelectedClient != x.Key)
                    {
                        ClientList.Items.Add(x.Key + "(UnSeen)");
                    }
                    else
                    {
                        ClientList.Items.Add(x.Key);
                    }
                }
                RecList.Items.Clear();
                if (ChatClient.SelectedClient != "" && ServerKernel.ChatClients.TryGetValue(ChatClient.SelectedClient, out var client))
                {
                    client.Seen = true;
                    foreach (string item in client.Mess)
                    {
                        RecList.Items.Add(item);
                    }
                }
                if (trackBar1.Value < 50)
                    trackBar1.Value++;
            }
            catch
            {
                ServerKernel.Log.SaveLog("Chat Box faild to view message", true, LogType.WARNING);
            }
        }

        private void StartChatPanel(object sender, EventArgs e)
        {
            try
            {
                if (!Server.GamePoll.ContainsKey(uint.MaxValue))
                {
                    GameClient pclient;
                    pclient = new GameClient(null)
                    {
                        Fake = true
                    };
                    pclient.Player = new Player(pclient);
                    pclient.Inventory = new Inventory(pclient);
                    pclient.Equipment = new Equip(pclient);
                    pclient.Warehouse = new Warehouse(pclient);
                    pclient.MyProfs = new Proficiency(pclient);
                    pclient.MySpells = new Spell(pclient);
                    pclient.Status = new MsgStatus();
                    pclient.Player.Name = "UAConquer";
                    pclient.Player.Body = 223;
                    pclient.Player.UID = uint.MaxValue;
                    pclient.Player.HitPoints = 65535;
                    pclient.Status.MaxHitpoints = 65535u;
                    pclient.Player.X = 439;
                    pclient.Player.Y = 385;
                    pclient.Player.Map = 1002u;
                    pclient.Player.Level = 255;
                    pclient.Player.Face = 296;
                    pclient.Player.Action = Flags.ConquerAction.Sit;
                    pclient.Player.Angle = Flags.ConquerAngle.SouthWest;
                    pclient.Player.Hair = 774;
                    pclient.Player.GarmentId = 0u;
                    pclient.Player.LeftWeaponAccessoryId = 0u;
                    pclient.Player.RightWeaponAccessoryId = 0u;
                    pclient.Map = Server.ServerMaps[1002u];
                    pclient.Map.Enquer(pclient);
                    Server.GamePoll.TryAdd(pclient.Player.UID, pclient);
                    using (RecycledPacket p = new RecycledPacket())
                    {
                        Packet stream;
                        stream = p.GetStream();
                        pclient.Player.AddMapEffect(stream, pclient.Player.X, pclient.Player.Y, "eddy");
                        pclient.Player.View.SendView(pclient.Player.GetArray(stream, false), false);
                    }
                    ServerKernel.Log.AppendServerLog($"{pclient.Player.Name} has loaded at {pclient.Player.Map}({pclient.Player.X},{pclient.Player.Y})");
                    if (trackBar1.Value < 50)
                        trackBar1.Value++;
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to Add UAConquer!", true, LogType.WARNING);
            }
        }

        private void RecList_MouseClick(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = RecList.SelectedItem.ToString();
            }
            catch
            {
                textBox1.Text = "";
            }
        }

        private void ClientList_MouseClick(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = ClientList.SelectedItem.ToString();
            }
            catch
            {
                textBox1.Text = "";
            }
        }

        private void SendMessageToUser(object sender, EventArgs e)
        {
            try
            {
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    if (user.Player.Name == ChatClient.SelectedClient)
                    {
                        user.SendWhisper(SendText.Text, "UAConquer", ChatClient.SelectedClient, 2961003u);
                        ServerKernel.ChatClients[ChatClient.SelectedClient].Mess.Add($"{DateTime.Now:d/M/yyyy (H:mm)}: UAConquer speaks to {ChatClient.SelectedClient}: {SendText.Text}");
                        string x;
                        x = ServerKernel.ChatClients[ChatClient.SelectedClient].Mess.Last();
                        RecList.Items.Add(x);
                        SendText.Text = "";
                    }
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild Send message!", true, LogType.WARNING);
            }
        }

        private unsafe void RemoveUserFromChat(object sender, EventArgs e)
        {
            try
            {
                if (Server.GamePoll.TryRemove(uint.MaxValue, out var client))
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        GameMap Map;
                        Map = Server.ServerMaps[client.Player.Map];
                        client.Team?.Remove(client, true);
                        Map.Denquer(client);
                        ActionQuery actionQuery;
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = client.Player.UID;
                        actionQuery.Type = ActionType.RemoveEntity;
                        ActionQuery action;
                        action = actionQuery;
                        client.Player.View.SendView(stream.ActionCreate(&action), false);
                        ServerKernel.Log.AppendServerLog(client.Player.Name + " has removed.");
                        if (trackBar1.Value < 50)
                            trackBar1.Value++;
                    }
                }
                ServerKernel.ChatClients.Clear();
                ChatClient.SelectedClient = "";
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to Remove user from chat box!", true, LogType.WARNING);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(1))
            {
                MessageBox.Show("Character must have 1 free slots into inventory");
                return;
            }
            string[] id;
            id = comboBox2.Text.Split(' ').ToArray();
            if (!Server.ItemsBase.TryGetValue(uint.Parse(id[1]), out var DBItem))
                return;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.Add(uint.Parse(id[1]), 0, DBItem, stream);
                ServerKernel.Log.AppendServerLog("Added the item " + DBItem.Name + " to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item " + DBItem.Name + " to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(1))
            {
                MessageBox.Show("Character must have 1 free slots into inventory");
                return;
            }
            string[] id;
            id = comboBox4.Text.Split(' ').ToArray();
            if (!Server.ItemsBase.TryGetValue(uint.Parse(id[1]), out var DBItem))
                return;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.Add(uint.Parse(id[1]), 0, DBItem, stream);
                ServerKernel.Log.AppendServerLog("Added the item " + DBItem.Name + " to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item " + DBItem.Name + " to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(1))
            {
                MessageBox.Show("Character must have 1 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.Add(stream, 780001u, 1, 0, 0, 0);
                ServerKernel.Log.AppendServerLog("Added the item VIPToken 1-day to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item VIPToken 1-day to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void VIPToken30(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(1))
            {
                MessageBox.Show("Character must have 1 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.Add(stream, 780000u, 1, 0, 0, 0);
                ServerKernel.Log.AppendServerLog("Added the item VIPToken 30-day to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item VIPToken 30-day to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void GiveEquipmentMonk(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(11))
            {
                MessageBox.Show("Character must have 11 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.AddSoul2(120269u, 821031u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(150269u, 823055u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(160249u, 824017u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                
                client.Inventory.AddSoul2(610439u, 800722u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 2, stream, false);
                client.Inventory.AddSoul2(610439u, 800722u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 2, stream, false);
                client.Inventory.AddSoul2(136309u, 822053u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(143309u, 820072u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                ServerKernel.Log.AppendServerLog("Added the item Full Equipment Monk to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item Full Equipment Monk to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void GiveEquipmentNinja(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(11))
            {
                MessageBox.Show("Character must have 11 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.AddSoul2(120269u, 821031u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(150269u, 823055u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(160249u, 824017u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                
                client.Inventory.AddSoul2(601439u, 800017u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 2, stream, false);
                client.Inventory.AddSoul2(601439u, 800017u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 2, stream, false);
                client.Inventory.AddSoul2(135309u, 822053u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(112309u, 820072u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                ServerKernel.Log.AppendServerLog("Added the item Full Equipment Ninja to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item Full Equipment Ninja to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void GiveEquipmentTaoist(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(11))
            {
                MessageBox.Show("Character must have 11 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                
                client.Inventory.AddSoul2(421439u, 800513u, 6u, 12u, 12, 3, 3, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(134309u, 822055u, 6u, 12u, 12, 3, 3, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(114309u, 820071u, 6u, 12u, 12, 3, 3, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(152279u, 823056u, 6u, 12u, 12, 3, 3, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(121269u, 821032u, 6u, 12u, 12, 3, 3, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(160249u, 824017u, 6u, 12u, 12, 3, 3, byte.MaxValue, 7, 1, stream, false);
                ServerKernel.Log.AppendServerLog("Added the item Full Equipment Taoist to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item Full Equipment Taoist to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void GiveEquipmentTrojan(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(13))
            {
                MessageBox.Show("Character must have 13 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.Add(stream, 120249, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); //Necklace                                    
                client.Inventory.Add(stream, 150249, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); //Ring                                    
                client.Inventory.Add(stream, 160249, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); //Boot   
                client.Inventory.Add(stream, 410339, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); //SkyBlade                                    
                client.Inventory.Add(stream, 420339, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); //SquallSword                                    
                client.Inventory.Add(stream, 480339, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); //NirvanaClub                                    
                client.Inventory.Add(stream, 130109, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); //ObsidianArmor                                    
                client.Inventory.Add(stream, 118109, 1, 12, 7, 255, Role.Flags.Gem.SuperDragonGem, Role.Flags.Gem.SuperDragonGem, false); //PeerlessCoronet                                   
                ServerKernel.Log.AppendServerLog("Added the item Full Equipment Trojan to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item Full Equipment Trojan to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void GiveEquipmentWarrior(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(16))
            {
                MessageBox.Show("Character must have 16 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.AddSoul2(120269u, 821031u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(150269u, 823055u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(160249u, 824017u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
               
                client.Inventory.AddSoul2(560439u, 800320u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(561439u, 800320u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(510439u, 800320u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(530439u, 800320u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(580439u, 800320u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(131309u, 822053u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(900309u, 724362u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(111309u, 820072u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                ServerKernel.Log.AppendServerLog("Added the item Full Equipment Warrior to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item Full Equipment Warrior to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void GiveEquipmentArchera(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(10))
            {
                MessageBox.Show("Character must have 10 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.AddSoul2(120269u, 821031u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(150269u, 823055u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(160249u, 824017u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
               
                client.Inventory.AddSoul2(500429u, 800616u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(133309u, 822053u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                client.Inventory.AddSoul2(113309u, 820072u, 6u, 12u, 12, 13, 13, byte.MaxValue, 7, 1, stream, false);
                ServerKernel.Log.AppendServerLog("Added the item Full Equipment Archer to " + client.Player.Name);
                client.SendSysMesage("The PM has added the item Full Equipment Archer to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
            }
        }

        private void GiveSurpriseBox(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client == null)
                return;
            if (!client.Inventory.HaveSpace(1))
            {
                MessageBox.Show("Character must have 1 free slots into inventory");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                client.Inventory.Add(stream, 722178u, 1, 0, 0, 0);
                client.SendSysMesage("The PM has added the item Surprise Box to your inventory", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
                ServerKernel.Log.AppendServerLog("Added the item Surprise Box to " + client.Player.Name);
            }
        }

        private void AddBannedAccount(object sender, EventArgs e)
        {
            uint Hours;
            Hours = uint.Parse(textBox2.Text);
            if (Hours != 0)
            {
                GameClient client;
                client = CurrentlySelectedUser();
                if (client != null)
                {
                    SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, Hours, SystemBannedAccount._Type.UsingCheat);
                    client.Socket.Disconnect();
                    ServerKernel.Log.AppendServerLog($"{client.Player.Name} has bannded for {Hours} hours!");
                    GetDefault(MainFlag.Bannedlist);
                }
            }
        }

        private void RemoveBannedAccount(object sender, EventArgs e)
        {
            uint UID;
            UID = uint.Parse(textBox4.Text);
            if (SystemBannedAccount.RemoveBan(UID))
                GetDefault(MainFlag.Bannedlist);
        }

        private void RemoveBannedPC(object sender, EventArgs e)
        {
            string banned;
            banned = comboBox6.Text;
            if (banned.Length > 0 && SystemBannedPC.RemoveBan(banned))
                GetDefault(MainFlag.BannedPClist);
        }

        private void AddBannedPC(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client != null && SystemBannedPC.AddBan(client))
            {
                client.Socket.Disconnect();
                GetDefault(MainFlag.BannedPClist);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client != null)
            {
                client.PrintProcesses = true;
                //client.Send(ProGuardHandler.CreatePacket(ProtectType.GetProcesses));
                if (trackBar1.Value < 50)
                    trackBar1.Value++;
            }
        }

        private void SendScanFiles(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client != null)
            {
                //ProGuardHandler.SendScanFiles(client);
                client.PrintProcesses = true;
                if (trackBar1.Value < 50)
                    trackBar1.Value++;
            }
        }

        private void SendCloseClient(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client != null)
            {
                //ProGuardHandler.SendCloseClient(client);
                client.PrintProcesses = true;
                if (trackBar1.Value < 50)
                    trackBar1.Value++;
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value < 50)
                trackBar1.Value++;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value < 50)
                trackBar1.Value++;
        }

        public SystemBannedAccount.Client GetPlayerBanned(string Name)
        {
            SystemBannedAccount.Client player;
            player = SystemBannedAccount.BannedPoll.Values.Where((SystemBannedAccount.Client s) => s.Name == Name).FirstOrDefault();
            if (player == null)
                return null;
            return player;
        }

        private void GetPlayerBanned(object sender, EventArgs e)
        {
            SystemBannedAccount.Client player;
            player = GetPlayerBanned(comboBox5.Text);
            if (player != null)
            {
                textBox4.Text = player.UID.ToString();
                textBox5.Text = player.Name;
                textBox7.Text = player.Reason;
            }
            else
                ServerKernel.Log.AppendServerLog("sorry cant find player " + comboBox5.Text);
        }

        private unsafe void VendorChecked(object sender, EventArgs e)
        {
            GameClient User;
            User = CurrentlySelectedUser();
            if (User == null)
                return;
            if (checkBox2.Checked)
            {
                if (!User.IsVendor && User.Player.AutoHunting != AutoStructures.Mode.Enable && User.Player.Map != 1039)
                {
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        User.MyVendor = new Vendor(User);
                        User.MyVendor.CreateVendor(stream2);
                        ActionQuery actionQuery;
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = User.Player.UID;
                        actionQuery.Type = ActionType.StartVendor;
                        actionQuery.dwParam = User.MyVendor.VendorUID;
                        actionQuery.wParam1 = User.MyVendor.VendorNpc.X;
                        actionQuery.wParam2 = User.MyVendor.VendorNpc.Y;
                        ActionQuery action;
                        action = actionQuery;
                        User.Player.View.SendView(stream2.ActionCreate(&action), true);
                        User.Player.View.SendView(stream2.MapStatusCreate(User.Map.ID, User.Map.BaseID, Server.ServerMaps[1036u].TypeStatus), true);
                        ServerKernel.Log.AppendServerLog(User.Player.Name + ", IsVendor: On");
                    }
                }
            }
            else if (User.IsVendor)
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    User.Pullback();
                    checkBox2.Checked = User.IsVendor;
                    ServerKernel.Log.AppendServerLog(User.Player.Name + ", IsVendor: Off");
                }
            }
            if (trackBar1.Value < 50)
                trackBar1.Value++;
        }

        private void MaintenanceServer(object sender, EventArgs e)
        {
            new Thread(Program.Maintenance).Start();
            ServerKernel.Log.AppendServerLog("The server will be brought down for maintenance in (5 Minutes). ");
        }

        public void CloseServer(object sender, EventArgs e)
        {
            Program.ProcessConsoleEvent(0);
            Environment.Exit(0);
            ServerKernel.Log.AppendServerLog("The server down");
        }

        private void SaveServer(object sender, EventArgs e)
        {
            ServerKernel.Log.AppendServerLog($"Last Server Saved [{KernelThread.LastSavePulse}] to accept connection.");
            Server.SaveDatabase();
            if (Server.FullLoading)
            {
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    if ((user.ClientFlag & ServerFlag.LoginFull) == ServerFlag.LoginFull)
                    {
                        user.ClientFlag |= ServerFlag.QueuesSave;
                        ServerDatabase.LoginQueue.TryEnqueue(user);
                    }
                }
                ServerKernel.Log.AppendServerLog("Saving Database...");
                ServerKernel.Log.SaveLog("Saving Database...", true, LogType.DEBUG);
            }
            if (ServerDatabase.LoginQueue.Finish())
            {
                Thread.Sleep(1000);
                ServerKernel.Log.SaveLog("Database saved successfully.", true, LogType.DEBUG);
                ServerKernel.Log.AppendServerLog("Database saved successfully.");
            }
        }

        private void ChatSystemSendMessage(object sender, EventArgs e)
        {
            string _type;
            _type = comboBox7.Text;
            string _message;
            _message = textBox8.Text;
            switch (_type)
            {
                case "Broadcast":
                    {
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;
                            stream = rec.GetStream();
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(_message, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(_message, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(_message, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
                            break;
                        }
                    }
                case "World":
                    {
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket.GetStream();
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(_message, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.WhiteVibrate).GetArray(stream2));
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(_message, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.World).GetArray(stream2));
                            break;
                        }
                    }
                case "Discord":
                    
                    //Program.DiscordEventsAPI.Enqueue($"``{_message}``");

                    break;
                default:
                    ServerKernel.Log.AppendServerLog("Message selected not found");
                    break;
            }
        }

        private void StartElitePkTournament(object sender, EventArgs e)
        {
            MsgSchedules.ElitePkTournament.Start();
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartSkillTeamPkTournament(object sender, EventArgs e)
        {
            MsgSchedules.SkillTeamPkTournament.Start();
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartTeamPkTournament(object sender, EventArgs e)
        {
            MsgSchedules.TeamPkTournament.Start();
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartClassPkWar(object sender, EventArgs e)
        {
            MsgSchedules.ClassPkWar.Start();
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartWeeklyPK(object sender, EventArgs e)
        {
            MsgSchedules.PkWar.Open();
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartClanWar(object sender, EventArgs e)
        {
            if (MsgSchedules.ClanWar.Proces == ProcesType.Dead)
                MsgSchedules.ClanWar.Start();
            if (MsgSchedules.ClanWar.Proces == ProcesType.Idle)
                MsgSchedules.ClanWar.Began();
            if (!MsgSchedules.ClanWar.SendInvitation)
            {
                MsgSchedules.SendInvitation("ClanWar", "ConquerPoints", 424, 251, 1002, 0, 60);
                MsgSchedules.ClanWar.SendInvitation = true;
            }
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartEliteGuildWar(object sender, EventArgs e)
        {
            if (MsgSchedules.EliteGuildWar.Proces == ProcesType.Dead)
                MsgSchedules.EliteGuildWar.Start();
            if (MsgSchedules.EliteGuildWar.Proces == ProcesType.Idle)
                MsgSchedules.EliteGuildWar.Began();
            if (!MsgSchedules.EliteGuildWar.SendInvitation)
            {
                MsgSchedules.SendInvitation("EliteGuildWar", "ConquerPoints", 383, 316, 1002, 0, 60);
                MsgSchedules.EliteGuildWar.SendInvitation = true;
            }
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartGuildWar(object sender, EventArgs e)
        {
            if (MsgSchedules.GuildWar.Proces == ProcesType.Dead)
                MsgSchedules.GuildWar.Start();
            if (MsgSchedules.GuildWar.Proces == ProcesType.Idle)
                MsgSchedules.GuildWar.Began();
            if (MsgSchedules.GuildWar.Proces != ProcesType.Dead)
                MsgSchedules.GuildWar.ShuffleGuildScores();
            if (!MsgSchedules.GuildWar.SendInvitation)
            {
                MsgSchedules.SendInvitation("GuildWar", "ConquerPoints", 200, 254, 1038, 0, 60, MsgStaticMessage.Messages.GuildWar);
                MsgSchedules.GuildWar.SendInvitation = true;
            }
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartCaptureTheFlag(object sender, EventArgs e)
        {
            MsgSchedules.CaptureTheFlag.Start();
            GetDefault(MainFlag.Tournamentlist);
        }

        private void StartTreasureThief(object sender, EventArgs e)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.TreasureThief];
                MsgSchedules.CurrentTournament.Open();
                GetDefault(MainFlag.Tournamentlist);
            }
        }

        private void StartFindTheBox(object sender, EventArgs e)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.FindTheBox];
                MsgSchedules.CurrentTournament.Open();
                GetDefault(MainFlag.Tournamentlist);
            }
        }
        private void StartFootBall(object sender, EventArgs e)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.FootBall];
                MsgSchedules.CurrentTournament.Open();
                GetDefault(MainFlag.Tournamentlist);
            }
        }

        private void StartSkillTournament(object sender, EventArgs e)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.SkillTournament];
                MsgSchedules.CurrentTournament.Open();
                GetDefault(MainFlag.Tournamentlist);
            }
        }

        private void UpdateWebsite(object sender, EventArgs e)
        {
            LbxOnScreen.Items.Clear();
            LbxOnPCCount.Items.Clear();
            comboBox10.Items.Clear();
                comboBox9.Items.Clear();
            LbxCharacters.Items.Clear();
                FillUserInformationEmpty();
                        LoadingCharacters();
            //bahaa
            TopRankings.LoadTopRankings();
            ServerKernel.Log.AppendServerLog("Updated Website Tables --> Ranking[Tops]");
        }

        private void AttackPanel(object sender, EventArgs e)
        {
            Rayzo_Panle cp = new Rayzo_Panle();
            cp.ShowDialog();
        }
        private void LOADERPanel(object sender, EventArgs e)
        {
            Panel cp = new Panel();
            cp.ShowDialog();
        }
        private void GiveSpecialTitles(object sender, EventArgs e)
        {
            try
            {
                string[] id;
                id = comboBox8.Text.Split(' ').ToArray();
                uint TitleID;
                TitleID = uint.Parse(id[2]);
                if (TheChosenProject.Database.SpecialTitles.Titles.TryGetValue(TitleID, out var dbtitle))
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        GameClient client;
                        client = CurrentlySelectedUser();
                        if (client == null)
                            return;
                        client.Player.AddSpecialTitle(stream, (MsgTitle.TitleType)dbtitle.ID, true);
                        Label lblSpecialTitles;
                        lblSpecialTitles = LblSpecialTitles;
                        MsgTitle.TitleType myTitle;
                        myTitle = (MsgTitle.TitleType)client.Player.MyTitle;
                        lblSpecialTitles.Text = myTitle.ToString();
                        ServerKernel.Log.AppendServerLog(client.Player.Name + " received a special title (" + dbtitle.Name + ")");
                    }
                }
                if (trackBar1.Value < 50)
                    trackBar1.Value++;
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to give player a title!", true, LogType.WARNING);
            }
        }

        private void RemoveSpell(object sender, EventArgs e)
        {
            try
            {
                GameClient client;
                client = CurrentlySelectedUser();
                if (client != null)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        string[] id;
                        id = comboBox9.Text.Split(' ').ToArray();
                        uint SpellID;
                        SpellID = uint.Parse(id[1]);
                        client.MySpells.Remove((ushort)SpellID, stream);
                        ServerKernel.Log.AppendServerLog($"{client.Player.Name} removed a Spell: ({(Flags.SpellID)SpellID})");
                        GetDefault(MainFlag.Userslist, client);
                    }
                    if (trackBar1.Value < 50)
                        trackBar1.Value++;
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to remove spell from player!", true, LogType.WARNING);
            }
        }

        private void RemoveItem(object sender, EventArgs e)
        {
            try
            {
                GameClient client;
                client = CurrentlySelectedUser();
                if (client == null)
                    return;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    string[] id;
                    id = comboBox10.Text.Split(' ').ToArray();
                    uint ItemID;
                    ItemID = uint.Parse(id[1]);
                    if (client.Inventory.Contain(ItemID, 1u, 0) || client.Inventory.Contain(ItemID, 1u, 1))
                    {
                        client.Inventory.Remove(ItemID, 1u, stream);
                        GetDefault(MainFlag.Userslist, client);
                        ServerKernel.Log.AppendServerLog(client.Player.Name + " removed a Item: (" + Server.ItemsBase[ItemID].Name + ")");
                    }
                }
                if (trackBar1.Value < 50)
                    trackBar1.Value++;
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to remove item from player!", true, LogType.WARNING);
            }
        }

        private void FinishTournament(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex;
                selectedIndex = listBox1.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < listBox1.Items.Count)
                {
                    string _tournament_selected;
                    _tournament_selected = listBox1.Items[selectedIndex].ToString();
                    string[] _tournament;
                    _tournament = _tournament_selected.Split(' ').ToArray();
                    switch ((ITournamentsAlive.ID)byte.Parse(_tournament[0]))
                    {
                        case ITournamentsAlive.ID.CaptureTheFlag:
                            MsgSchedules.CaptureTheFlag.CheckFinish(true);
                            break;
                        case ITournamentsAlive.ID.ClanWar:
                            MsgSchedules.ClanWar.CompleteEndWar();
                            break;
                        case ITournamentsAlive.ID.ElitePkTournament:
                            {
                                MsgEliteGroup EPK_Lvl130Plus;
                                EPK_Lvl130Plus = MsgEliteTournament.EliteGroups[3];
                                EPK_Lvl130Plus.Finish();
                                break;
                            }
                        case ITournamentsAlive.ID.LastmanStand:
                            MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.LastmanStand];
                            MsgSchedules.CurrentTournament.Close();
                            break;
                        case ITournamentsAlive.ID.TeamPkTournament:
                            {
                                MsgTeamEliteGroup EPK_Lvl130Plus2;
                                EPK_Lvl130Plus2 = MsgTeamPkTournament.EliteGroups[3];
                                EPK_Lvl130Plus2.Finish();
                                break;
                            }
                        case ITournamentsAlive.ID.FB_SS_Tournament:
                            MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.SkillTournament];
                            MsgSchedules.CurrentTournament.Close();
                            break;
                        case ITournamentsAlive.ID.SkillTeamPkTournament:
                            {
                                MsgTeamEliteGroup EPK_Lvl130Plus3;
                                EPK_Lvl130Plus3 = MsgSkillTeamPkTournament.EliteGroups[3];
                                EPK_Lvl130Plus3.Finish();
                                break;
                            }
                        case ITournamentsAlive.ID.ClassPkWar:
                            MsgSchedules.ClassPkWar.Stop();
                            break;
                        case ITournamentsAlive.ID.GuildWar:
                            MsgSchedules.GuildWar.CompleteEndGuildWar();
                            break;
                        case ITournamentsAlive.ID.EliteGuildWar:
                            MsgSchedules.EliteGuildWar.CompleteEndGuildWar();
                            break;
                        case ITournamentsAlive.ID.TreasureThief:
                            MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.TreasureThief];
                            MsgSchedules.CurrentTournament.Close();
                            break;
                        case ITournamentsAlive.ID.FindTheBox:
                            MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.FindTheBox];
                            MsgSchedules.CurrentTournament.Close();
                            break;
                        case ITournamentsAlive.ID.WeeklyPK:
                            MsgSchedules.PkWar.CheckUp(true);
                            break;
                        case ITournamentsAlive.ID.MonthlyPK:
                            MsgSchedules.MonthlyPKWar.CheckUp(true);
                            break;
                    }
                    GetDefault(MainFlag.Tournamentlist);
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Cannot finish event!", true, LogType.WARNING);
            }
        }

        private void CheckBoxAutoHunt_CheckedChanged(object sender, EventArgs e)
        {
            GameClient User;
            User = CurrentlySelectedUser();
            if (User != null)
            {
                if (User.Player.AutoHunting == AutoStructures.Mode.NotActive)
                {
                    CheckBoxAutoHunt.Checked = true;
                    User.Player.AutoHunting = AutoStructures.Mode.Enable;
                }
                else
                {
                    CheckBoxAutoHunt.Checked = false;
                    User.Player.AutoHunting = AutoStructures.Mode.Disable;
                }
                if (trackBar1.Value < 50)
                    trackBar1.Value++;
            }
        }

        private void BotJailChecked(object sender, EventArgs e)
        {
            GameClient User;
            User = CurrentlySelectedUser();
            if (User == null)
                return;
            if (User.Player.Map != 6003 && checkBox3.Checked)
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream2;
                    stream2 = recycledPacket.GetStream();
                    User.Teleport(100, 100, 6003u);
                    string _message;
                    _message = "Sod.." + User.Player.Name + "..override the law, now is gone to the prison of hell Bot Jail, the signing of the Administration";
                    ServerKernel.Log.AppendServerLog(_message);
                    checkBox3.Checked = true;
                    Program.SendGlobalPackets.Enqueue(new MsgMessage(_message, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.WhiteVibrate).GetArray(stream2));
                }
            }
            else if (User.Player.Map == 6003 && !checkBox3.Checked)
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    User.Teleport(428, 378, 1002u, 0u, true, true);
                    checkBox3.Checked = false;
                    ServerKernel.Log.AppendServerLog(User.Player.Name + " has left the Botjail.");
                }
            }
            if (trackBar1.Value < 50)
                trackBar1.Value++;
        }

        private unsafe void KickoutBot(object sender, EventArgs e)
        {
            GameClient User;
            User = CurrentlySelectedBot();
            if (User != null && Server.GamePoll.TryRemove(User.Player.UID, out var client))
            {
                GameMap Map;
                Map = Server.ServerMaps[client.Player.Map];
                client.Team?.Remove(client, true);
                Map.Denquer(client);
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.ObjId = client.Player.UID;
                actionQuery.Type = ActionType.RemoveEntity;
                ActionQuery action;
                action = actionQuery;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    client.Player.View.SendView(stream.ActionCreate(&action), false);
                }
                FillBotInformationEmpty();
                GetDefault(MainFlag.Botslist);
                ServerKernel.Log.SaveLog($"AI[{client.Player.Name}] has logout!", true, LogType.WARNING);
            }
        }

        private void SelectedBotIndex(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedBot();
            if (client != null)
                FillBotInformation(client.Player);
        }

        public bool CheckBotFailed()
        {
            bool can;
            can = true;
            string Name;
            Name = textBox20.Text;
            string Rank;
            Rank = comboBox12.Text;
            string Type;
            Type = comboBox11.Text;
            string Body;
            Body = comboBox13.Text;
            string Class;
            Class = comboBox15.Text;
            decimal.TryParse(textBox21.Text, out var Level);
            decimal.TryParse(textBox19.Text, out var Map);
            decimal.TryParse(textBox22.Text, out var X);
            decimal.TryParse(textBox24.Text, out var Y);
            decimal.TryParse(textBox26.Text, out var LeftOn);
            decimal.TryParse(textBox28.Text, out var Face);
            if (Name == null || Name == "" || Enumerable.Contains(Server.NameUsed, Name.GetHashCode()) || !Program.NameStrCheck(Name))//bahaa
            {
                MessageBox.Show("Name have problem");
                can = false;
            }
            if (Level <= 0m || Level > 140m)
            {
                MessageBox.Show("Level have problem");
                can = false;
            }
            if (Map <= 0m || X <= 0m || Y <= 0m)
            {
                MessageBox.Show("Map(X,Y) have problem");
                can = false;
            }
            if (!Enum.IsDefined(typeof(AIEnum.AIType), Type) && Type != "Buffers" && Type != "PK-Fighting")
            {
                MessageBox.Show("Type(Hunting,Training) have problem");
                can = false;
            }
            if (!Enum.IsDefined(typeof(Flags.BaseClassType), Class))
            {
                MessageBox.Show("Class have problem");
                can = false;
            }
            if (LeftOn <= 0m)
            {
                MessageBox.Show("Timer have problem");
                can = false;
            }
            if (!Enum.IsDefined(typeof(Nobility.NobilityRank), Rank))
            {
                MessageBox.Show("Rank have problem");
                can = false;
            }
            if (!Enum.IsDefined(typeof(Flags.BodyType), Body))
            {
                MessageBox.Show("Body have problem");
                can = false;
            }
            if (!File.Exists("Resources/64/" + Face + ".jpg"))
            {
                MessageBox.Show("Face have problem");
                can = false;
            }
            return can;
        }

        public bool CheckEquipmentFailed()
        {
            bool can;
            can = true;
            decimal.TryParse(textBox9.Text, out var HeadId);
            decimal.TryParse(textBox10.Text, out var RingID);
            decimal.TryParse(TxtBotArmors.Text, out var ArmorID);
            decimal.TryParse(TxtBotRightWeapon.Text, out var WeaponID);
            decimal.TryParse(TxtBotLeftWeapon.Text, out var LeftWeaponID);
            decimal.TryParse(TxtBotBoots.Text, out var BootID);
            decimal.TryParse(TxtBotStarTower.Text, out var FanID);
            decimal.TryParse(TxtBotHeavenFan.Text, out var TowerID);
            decimal.TryParse(TxtBotRidingCrop.Text, out var CropID);
            decimal.TryParse(textBox11.Text, out var NecID);
            if (ItemType.ItemPosition((uint)HeadId) != 1)
            {
                MessageBox.Show("HeadID have problem");
                can = false;
            }
            if (ItemType.ItemPosition((uint)NecID) != 2)
            {
                MessageBox.Show("NecklaceID have problem");
                can = false;
            }
            if (ItemType.ItemPosition((uint)RingID) != 6)
            {
                MessageBox.Show("RingID have problem");
                can = false;
            }
            if (ItemType.ItemPosition((uint)ArmorID) != 3)
            {
                MessageBox.Show("ArmorID have problem");
                can = false;
            }
            if (ItemType.ItemPosition((uint)WeaponID) != 4)
            {
                MessageBox.Show("WeaponID have problem");
                can = false;
            }
            if (LeftWeaponID > 0m && ItemType.ItemPosition((uint)LeftWeaponID) != 4 && ItemType.ItemPosition((uint)LeftWeaponID) != 5)
            {
                MessageBox.Show("LeftWeaponID have problem");
                can = false;
            }
            if (ItemType.ItemPosition((uint)BootID) != 8)
            {
                MessageBox.Show("BootID have problem");
                can = false;
            }
            //if (ItemType.ItemPosition((uint)FanID) != 10)
            //{
            //    MessageBox.Show("FanID have problem");
            //    can = false;
            //}
            //if (ItemType.ItemPosition((uint)TowerID) != 11)
            //{
            //    MessageBox.Show("TowerID have problem");
            //    can = false;
            //}
            //if (ItemType.ItemPosition((uint)CropID) != 9)
            //{
            //    MessageBox.Show("CropID have problem");
            //    can = false;
            //}
            return can;
        }

        private void AddingBot(object sender, EventArgs e)
        {
            try
            {
                if (!CheckBotFailed() || !CheckEquipmentFailed())
                    return;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                   
                    string Name;
                    Name = textBox20.Text;
                    string Rank;
                    Rank = comboBox12.Text;
                    string Type;
                    Type = comboBox11.Text;
                    string Body;
                    Body = comboBox13.Text;
                    string Class;
                    Class = comboBox15.Text;
                    string Reborn;
                    Reborn = RebornBox.Text;
                    decimal.TryParse(textBox21.Text, out var Level);
                    decimal.TryParse(textBox19.Text, out var Map);
                    decimal.TryParse(textBox22.Text, out var X);
                    decimal.TryParse(textBox24.Text, out var Y);
                    decimal.TryParse(textBox26.Text, out var LeftOn);
                    decimal.TryParse(textBox28.Text, out var Face);
                    if (Server.GamePoll.ContainsKey(Artificialintelligence.UIDCounter.Next))
                        return;
                    GameClient AI_player;
                    AI_player = new GameClient(null)
                    {
                        Fake = true
                    };
                    AI_player.Player = new Player(AI_player);
                    AI_player.Inventory = new Inventory(AI_player);
                    AI_player.Equipment = new Equip(AI_player);
                    AI_player.Warehouse = new Warehouse(AI_player);
                    AI_player.MyProfs = new Proficiency(AI_player);
                    AI_player.MySpells = new Spell(AI_player);
                    AI_player.Player.Nobility = new Nobility(AI_player);
                    AI_player.Status = new MsgStatus();
                    if (checkBox11.Checked)
                    {
                        AI_player.Player.Name = Name + "[Bot]";//bahaa
                    }
                    else
                        AI_player.Player.Name = Name;
                    lock (Server.NameUsed)
                    {
                        Server.NameUsed.Add(Name.GetHashCode());
                    }
                    //if (!Enumerable.Contains(Server.NameUsed, Name.GetHashCode()))//bahaa
                    //{
                    //    if (checkBox11.Checked)
                    //    {
                    //        AI_player.Player.Name = Name + "[Bot]";//bahaa
                    //    }
                    //    else
                    //        AI_player.Player.Name = Name;
                    //}
                    //else
                    //{

                    //    AI_player.Player.Name = "[X|]" + Name;

                    //}

                    AI_player.Player.X = (ushort)X;
                    AI_player.Player.Y = (ushort)Y;
                    AI_player.Player.Map = (ushort)Map;
                    AI_player.Player.DynamicID = 0;
                    AI_player.Player.Spouse = "None";
                    switch (Body)
                    {
                        case "AgileMale":
                            AI_player.Player.Body = 1003;
                            break;
                        case "MuscularMale":
                            AI_player.Player.Body = 1004;
                            break;
                        case "AgileFemale":
                            AI_player.Player.Body = 2001;
                            break;
                        case "MuscularFemale":
                            AI_player.Player.Body = 2002;
                            break;
                    }
                    do
                    {
                        AI_player.Player.UID = Artificialintelligence.UIDCounter.Next;
                    }
                    while (Server.GamePoll.ContainsKey(AI_player.Player.UID));
                    AI_player.Player.Associate = new Associate.MyAsociats(AI_player.Player.UID);
                    AI_player.Player.Level = (ushort)Level;
                    //AI_player.Player.Action = Flags.ConquerAction.Angry;
                    AI_player.Player.Angle = Flags.ConquerAngle.North;
                    byte Color;
                    Color = (byte)ServerKernel.NextAsync(4, 8);
                    AI_player.Player.Hair = (ushort)(Color * 100 + 10 + (byte)ServerKernel.NextAsync(4, 9));
                    //AI_player.Player.QuizPoints = 65535u;
                    byte _Class;
                    _Class = 0;
                    if (Class != null)
                    {
                        switch (Class.Length)
                        {
                            case 6:
                                switch (Class[0])
                                {
                                    case 'T':
                                        if (Class == "Trojan")
                                            if (AI_player.Player.Level >= 110)
                                                _Class = 15;
                                            else if (AI_player.Player.Level >= 100 && AI_player.Player.Level < 110)
                                                _Class = 14;
                                            else if (AI_player.Player.Level >= 70 && AI_player.Player.Level < 100)
                                                _Class = 13;
                                            else if (AI_player.Player.Level >= 40 && AI_player.Player.Level < 70)
                                                _Class = 12;
                                            else if (AI_player.Player.Level >= 15 && AI_player.Player.Level < 40)
                                                _Class = 11;
                                            else
                                                _Class = 10;
                                        break;
                                    case 'A':
                                        if (Class == "Archer")
                                            if(AI_player.Player.Level >= 110)
                                                _Class = 45;
                                            else if(AI_player.Player.Level >= 100 && AI_player.Player.Level < 110)
                                                _Class = 44;
                                            else if (AI_player.Player.Level >= 70 && AI_player.Player.Level < 100)
                                                _Class = 43;
                                            else if (AI_player.Player.Level >= 40 && AI_player.Player.Level < 70)
                                                _Class = 42;
                                            else if (AI_player.Player.Level >= 15 && AI_player.Player.Level < 40)
                                                _Class = 41;
                                            else
                                                _Class = 40;
                                        break;
                                }
                                break;
                            case 5:
                                switch (Class[0])
                                {
                                    case 'N':
                                        if (Class == "Ninja")
                                            _Class = 55;
                                        break;
                                    case 'W':
                                        if (Class == "Water")
                                            if (AI_player.Player.Level >= 110)
                                                _Class = 135;
                                            else if (AI_player.Player.Level >= 100 && AI_player.Player.Level < 110)
                                                _Class = 134;
                                            else if (AI_player.Player.Level >= 70 && AI_player.Player.Level < 100)
                                                _Class = 133;
                                            else if (AI_player.Player.Level >= 40 && AI_player.Player.Level < 70)
                                                _Class = 132;
                                            else if (AI_player.Player.Level >= 15 && AI_player.Player.Level < 40)
                                                _Class = 131;
                                            else
                                                _Class = 100;
                                        break;
                                }
                                break;
                            case 4:
                                switch (Class[0])
                                {
                                    case 'M':
                                        if (Class == "Monk")
                                            _Class = 65;
                                        break;
                                    case 'F':
                                        if (Class == "Fire")
                                            if (AI_player.Player.Level >= 110)
                                                _Class = 145;
                                            else if (AI_player.Player.Level >= 100 && AI_player.Player.Level < 110)
                                                _Class = 144;
                                            else if (AI_player.Player.Level >= 70 && AI_player.Player.Level < 100)
                                                _Class = 143;
                                            else if (AI_player.Player.Level >= 40 && AI_player.Player.Level < 70)
                                                _Class = 142;
                                            else if (AI_player.Player.Level >= 15 && AI_player.Player.Level < 40)
                                                _Class = 141;
                                            else
                                                _Class = 100;
                                        break;
                                }
                                break;
                            case 7:
                                if (Class == "Warrior")
                                    if (AI_player.Player.Level >= 110)
                                        _Class = 25;
                                    else if (AI_player.Player.Level >= 100 && AI_player.Player.Level < 110)
                                        _Class = 24;
                                    else if (AI_player.Player.Level >= 70 && AI_player.Player.Level < 100)
                                        _Class = 23;
                                    else if (AI_player.Player.Level >= 40 && AI_player.Player.Level < 70)
                                        _Class = 22;
                                    else if (AI_player.Player.Level >= 15 && AI_player.Player.Level < 40)
                                        _Class = 21;
                                    else
                                        _Class = 20;
                                break;
                        }
                    }
                    switch (Reborn)
                    {
                        case "1stReborn":
                            AI_player.Player.Class = (AI_player.Player.FirstClass = _Class);
                            AI_player.Player.Reborn = 1;
                            break;
                        case "2ndReborn":
                            AI_player.Player.Class = (AI_player.Player.FirstClass = (AI_player.Player.SecondClass = _Class));
                            AI_player.Player.Reborn = 2;
                            break;
                        case "Newbie":
                            AI_player.Player.Class = _Class;
                            AI_player.Player.Reborn = 0;
                            break;
                    }
                    AI_player.Player.Vitality += 500;
                    AI_player.Player.Strength += 500;
                    AI_player.Player.Spirit += 500;
                    AI_player.Player.Agility += 500;
                    AI_player.Player.SendUpdate(stream, AI_player.Player.Strength, MsgUpdate.DataType.Strength);
                    AI_player.Player.SendUpdate(stream, AI_player.Player.Agility, MsgUpdate.DataType.Agility);
                    AI_player.Player.SendUpdate(stream, AI_player.Player.Spirit, MsgUpdate.DataType.Spirit);
                    AI_player.Player.SendUpdate(stream, AI_player.Player.Vitality, MsgUpdate.DataType.Vitality);
                    AI_player.Player.GuildBattlePower = 15u;
                    AI_player.Player.Face = (ushort)Face;
                    switch (Rank)
                    {
                        case "King":
                            AI_player.Player.NobilityRank = Nobility.NobilityRank.King;
                            break;
                        case "Prince":
                            AI_player.Player.NobilityRank = Nobility.NobilityRank.Prince;
                            break;
                        case "Duke":
                            AI_player.Player.NobilityRank = Nobility.NobilityRank.Duke;
                            break;
                        case "Earl":
                            AI_player.Player.NobilityRank = Nobility.NobilityRank.Earl;
                            break;
                        case "Serf":
                            AI_player.Player.NobilityRank = Nobility.NobilityRank.Serf;
                            break;
                    }
                    AI_Creating(AI_player, stream);
                    AI_player.Player.SetPkMode(Flags.PKMode.Peace);
                    AI_player.Player.HitPoints = AI_player.CalculateHitPoint();
                    AI_player.Status.MaxHitpoints = AI_player.CalculateHitPoint();
                    AI_player.Player.Mana = AI_player.CalculateMana();
                    AI_player.Status.MaxMana = AI_player.CalculateMana();
                    AI_player.AIStatus = AIEnum.AIStatus.Idle;
                    switch (Type)
                    {
                        case "Hunting":
                            AI_player.AIType = AIEnum.AIType.Hunting;
                            if (checkBox4.Checked)
                            {
                                AI_player.Player.AddFlag(MsgUpdate.Flags.Hunting, 2592000, false);
                                //AI_player.Player.AddFlag(MsgUpdate.Flags.VIPSpecial_Jump, 2592000, false, 0, 10u, 10u);
                            }
                            AI_player.Player.AIBotExpire = DateTime.Now.AddHours((int)(byte)LeftOn);
                            break;
                        case "PK-Fighting":
                            AI_player.AIType = AIEnum.AIType.PKFighting;
                            if (checkBox4.Checked)
                            {
                                AI_player.Player.AddFlag(MsgUpdate.Flags.Hunting, 2592000, false);
                                //AI_player.Player.AddFlag(MsgUpdate.Flags.VIPSpecial_Jump, 2592000, false, 0, 10u, 10u);
                            }
                            AI_player.Player.SetPkMode(Flags.PKMode.PK);
                            AI_player.Player.AIBotExpire = DateTime.Now.AddHours((int)(byte)LeftOn);
                            break;
                        case "Training":
                            AI_player.AIType = AIEnum.AIType.Training;
                            if (checkBox4.Checked)
                            {
                                AI_player.Player.AddFlag(MsgUpdate.Flags.Hunting, 2592000, false);
                                //AI_player.Player.AddFlag(MsgUpdate.Flags.VIPSpecial_Jump, 2592000, false, 0, 10u, 10u);
                            }
                            AI_player.Player.AIBotExpire = DateTime.Now.AddHours((int)(byte)LeftOn);
                            break;
                        case "Buffers":
                            AI_player.AIType = AIEnum.AIType.BufferBot;
                            AI_player.Player.AddFlag(MsgUpdate.Flags.Stigma, 2592000, false);
                            AI_player.Player.AddFlag(MsgUpdate.Flags.StarOfAccuracy, 2592000, false);
                            AI_player.Player.AIBotExpire = DateTime.Now.AddHours((int)(byte)LeftOn);
                            break;
                    }
                    if (checkBox10.Checked)
                    {
                        AI_player.Team = new Team(AI_player);
                        AI_player.Team.PickupItems = (AI_player.Team.PickupMoney = (AI_player.Team.AutoInvite = true));
                    }

                    using (new RecycledPacket())
                    {
                        AI_player.Player.AddMapEffect(stream, AI_player.Player.X, AI_player.Player.Y, "eddy");
                        AI_player.Map = Server.ServerMaps[AI_player.Player.Map];
                        AI_player.Map.Enquer(AI_player);
                        Server.GamePoll.TryAdd(AI_player.Player.UID, AI_player);
                        AI_player.Player.View.SendView(AI_player.Player.GetArray(stream, false), false);
                        GetDefault(MainFlag.Botslist);

                    }
                    ServerKernel.Log.SaveLog(string.Format("AI[{0}] has logged in  {1}({2},{3}) will leave at: {4}", AI_player.Player.Name, AI_player.Map.Name, AI_player.Player.X, AI_player.Player.Y, AI_player.Player.AIBotExpire.ToString("d/M/yyyy (H:mm)")), true, LogType.WARNING);
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("something error please check fields!", true, LogType.WARNING);
            }
        }

        public void AI_Creating(GameClient client, Packet stream)
        {
            try
            {
                decimal.TryParse(textBox9.Text, out var HeadId);
                decimal.TryParse(textBox10.Text, out var RingID);
                decimal.TryParse(TxtBotArmors.Text, out var ArmorID);
                decimal.TryParse(TxtBotRightWeapon.Text, out var WeaponID);
                decimal.TryParse(TxtBotLeftWeapon.Text, out var LeftWeaponID);
                decimal.TryParse(TxtBotBoots.Text, out var BootID);
                decimal.TryParse(TxtBotStarTower.Text, out var FanID);
                decimal.TryParse(TxtBotHeavenFan.Text, out var TowerID);
                decimal.TryParse(TxtBotRidingCrop.Text, out var CropID);
                decimal.TryParse(textBox11.Text, out var NecID);
                byte HeadPlus;
                HeadPlus = (byte)numericUpDown11.Value;
                byte HeadBless;
                HeadBless = (byte)numericUpDown1.Value;
                byte NecBless;
                NecBless = (byte)numericUpDown2.Value;
                byte NecPlus;
                NecPlus = (byte)numericUpDown12.Value;
                byte RingBless;
                RingBless = (byte)numericUpDown3.Value;
                byte RingPlus;
                RingPlus = (byte)numericUpDown13.Value;
                byte ArmorBless;
                ArmorBless = (byte)numericUpDown4.Value;
                byte ArmorPlus;
                ArmorPlus = (byte)numericUpDown14.Value;
                byte WeaponBless;
                WeaponBless = (byte)numericUpDown5.Value;
                byte WeaponPlus;
                WeaponPlus = (byte)numericUpDown15.Value;
                byte LeftWeaponBless;
                LeftWeaponBless = (byte)numericUpDown6.Value;
                byte LeftWeaponPlus;
                LeftWeaponPlus = (byte)numericUpDown16.Value;
                byte BootBless;
                BootBless = (byte)numericUpDown7.Value;
                byte BootPlus;
                BootPlus = (byte)numericUpDown20.Value;
                byte FanBless;
                FanBless = (byte)numericUpDown7.Value;
                byte FanPlus;
                FanPlus = (byte)numericUpDown20.Value;
                byte TowerBless;
                TowerBless = (byte)numericUpDown9.Value;
                byte TowerPlus;
                TowerPlus = (byte)numericUpDown18.Value;
                byte CropBless;
                CropBless = (byte)numericUpDown10.Value;
                byte CropPlus;
                CropPlus = (byte)numericUpDown17.Value;
                //if (AtributesStatus.IsTrojan(client.Player.Class))
                //{
                //    List<SkillLearn> Trojan;
                //    Trojan = Server.SkillForLearning[Flags.BaseClassType.Trojan];
                //    foreach (SkillLearn skill7 in Trojan)
                //    {
                //        client.MySpells.Add(stream, (ushort)skill7.ID, skill7.Lvl, 0, 0);
                //    }
                //}
                //else if (AtributesStatus.IsWarrior(client.Player.Class))
                //{
                //    List<SkillLearn> Warrior;
                //    Warrior = Server.SkillForLearning[Flags.BaseClassType.Warrior];
                //    foreach (SkillLearn skill6 in Warrior)
                //    {
                //        client.MySpells.Add(stream, (ushort)skill6.ID, skill6.Lvl, 0, 0);
                //    }
                //}
                //else if (AtributesStatus.IsArcher(client.Player.Class))
                //{
                //    List<SkillLearn> Archer;
                //    Archer = Server.SkillForLearning[Flags.BaseClassType.Archer];
                //    foreach (SkillLearn skill5 in Archer)
                //    {
                //        client.MySpells.Add(stream, (ushort)skill5.ID, skill5.Lvl, 0, 0);
                //    }
                //}
                //else if (AtributesStatus.IsNinja(client.Player.Class))
                //{
                //    List<SkillLearn> Ninja;
                //    Ninja = Server.SkillForLearning[Flags.BaseClassType.Ninja];
                //    foreach (SkillLearn skill4 in Ninja)
                //    {
                //        client.MySpells.Add(stream, (ushort)skill4.ID, skill4.Lvl, 0, 0);
                //    }
                //}
                //else if (AtributesStatus.IsMonk(client.Player.Class))
                //{
                //    List<SkillLearn> Monk;
                //    Monk = Server.SkillForLearning[Flags.BaseClassType.Monk];
                //    foreach (SkillLearn skill3 in Monk)
                //    {
                //        client.MySpells.Add(stream, (ushort)skill3.ID, skill3.Lvl, 0, 0);
                //    }
                //}
                //else if (AtributesStatus.IsFire(client.Player.Class))
                //{
                //    List<SkillLearn> Fire;
                //    Fire = Server.SkillForLearning[Flags.BaseClassType.Fire];
                //    foreach (SkillLearn skill2 in Fire)
                //    {
                //        client.MySpells.Add(stream, (ushort)skill2.ID, skill2.Lvl, 0, 0);
                //    }
                //}
                //else if (AtributesStatus.IsWater(client.Player.Class))
                //{
                //    List<SkillLearn> Water;
                //    Water = Server.SkillForLearning[Flags.BaseClassType.Water];
                //    foreach (SkillLearn skill in Water)
                //    {
                //        client.MySpells.Add(stream, (ushort)skill.ID, skill.Lvl, 0, 0);
                //    }
                //}
                Flags.Gem SetGemOne;
                SetGemOne = Flags.Gem.EmptySocket;
                Flags.Gem SetGemTwo;
                SetGemTwo = Flags.Gem.EmptySocket;
                byte Health;
                Health = (byte)ServerKernel.NextAsync(1, 255);
                if (GemsChecked.Checked)
                {
                    SetGemOne = ((!AtributesStatus.IsTaoist(client.Player.Class)) ? (SetGemTwo = Flags.Gem.SuperDragonGem) : (SetGemTwo = Flags.Gem.SuperPhoenixGem));
                    client.Equipment.Add(stream, (uint)HeadId, Flags.ConquerItem.Head, HeadPlus, HeadBless, Health, SetGemOne, SetGemTwo);
                    client.Equipment.Add(stream, (uint)NecID, Flags.ConquerItem.Necklace, NecPlus, NecBless, Health, SetGemOne, SetGemTwo);
                    client.Equipment.Add(stream, (uint)ArmorID, Flags.ConquerItem.Armor, ArmorPlus, ArmorBless, Health, SetGemOne, SetGemTwo);
                    client.Equipment.Add(stream, (uint)RingID, Flags.ConquerItem.Ring, RingPlus, RingBless, Health, SetGemOne, SetGemTwo);
                    client.Equipment.Add(stream, (uint)WeaponID, Flags.ConquerItem.RightWeapon, WeaponPlus, WeaponBless, Health, SetGemOne, SetGemTwo);
                    if (client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon).ITEM_ID / 1000u == 500)
                        client.Equipment.Add(stream, (uint)LeftWeaponID, Flags.ConquerItem.LeftWeapon, 0, 0, 0);
                    else
                        client.Equipment.Add(stream, (uint)LeftWeaponID, Flags.ConquerItem.LeftWeapon, LeftWeaponPlus, LeftWeaponBless, Health, SetGemOne, SetGemTwo);
                    client.Equipment.Add(stream, (uint)BootID, Flags.ConquerItem.Boots, BootPlus, BootBless, Health, SetGemOne, SetGemTwo);
                    client.Equipment.Add(stream, (uint)FanID, Flags.ConquerItem.Fan, FanPlus, FanBless, Health, Flags.Gem.SuperThunderGem, Flags.Gem.SuperThunderGem);
                    client.Equipment.Add(stream, (uint)TowerID, Flags.ConquerItem.Tower, TowerPlus, TowerBless, Health, Flags.Gem.SuperGloryGem, Flags.Gem.SuperGloryGem);
                    client.Equipment.Add(stream, (uint)CropID, Flags.ConquerItem.Garment, 0, 0, 0);
                    client.Equipment.Add(stream, 30000u, Flags.ConquerItem.Steed, CropPlus, 0, 0);
                    client.Equipment.Add(stream, 2100025u, Flags.ConquerItem.Bottle, 0, 0, 0);
                }
                else
                {
                    client.Equipment.Add(stream, (uint)HeadId - 2, Flags.ConquerItem.Head, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false);
                    client.Equipment.Add(stream, (uint)NecID - 2, Flags.ConquerItem.Necklace, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false);
                    client.Equipment.Add(stream, (uint)ArmorID - 2, Flags.ConquerItem.Armor, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false);
                    client.Equipment.Add(stream, (uint)RingID - 2, Flags.ConquerItem.Ring, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false);
                    if (AtributesStatus.IsTaoist(client.Player.Class))
                    {
                        client.Equipment.Add(stream, (uint)WeaponID - 2, Flags.ConquerItem.RightWeapon, 0, 0, 0, Flags.Gem.RefinedPhoenixGem, Flags.Gem.RefinedPhoenixGem, false);
                    }
                    else
                    client.Equipment.Add(stream, (uint)WeaponID - 2, Flags.ConquerItem.RightWeapon, 0, 0, 0, Flags.Gem.RefinedDragonGem, Flags.Gem.RefinedDragonGem, false);
                    if (client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon).ITEM_ID / 1000u == 500)
                        client.Equipment.Add(stream, (uint)LeftWeaponID, Flags.ConquerItem.LeftWeapon, 0, 0, 0);
                    else
                        client.Equipment.Add(stream, (uint)LeftWeaponID - 2, Flags.ConquerItem.LeftWeapon, 0, 0, 0, Flags.Gem.RefinedDragonGem, Flags.Gem.RefinedDragonGem, false);
                    client.Equipment.Add(stream, (uint)BootID - 2, Flags.ConquerItem.Boots, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false);
                    //client.Equipment.Add(stream, (uint)FanID, Flags.ConquerItem.Fan, FanPlus, FanBless, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket);
                    //client.Equipment.Add(stream, (uint)TowerID, Flags.ConquerItem.Tower, TowerPlus, TowerBless, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket);
                    client.Equipment.Add(stream, (uint)CropID, Flags.ConquerItem.Garment, 0, 0, 0);
                    //client.Equipment.Add(stream, 30000u, Flags.ConquerItem.Steed, CropPlus, 0, 0);
                    //client.Equipment.Add(stream, 2100025, Flags.ConquerItem.Bottle, 0, 0, 0);
                }
                client.Send(stream.HeroInfo(client.Player));
                client.Equipment.Show(stream);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Cannot creating item equipments & skills bot!", true, LogType.WARNING);
            }
        }

        private void UpdateConfigration(object sender, EventArgs e)
        {
            //if (!RefrshBeforeUpdate)
            //{
            //    MessageBox.Show("Refresh page first!");
            //    return;
            //}
            IniFile _panel;
            _panel = new IniFile(Directory.GetCurrentDirectory() + "\\ServerConfiguration.ini", true);
            IniFile _shell;
            _shell = new IniFile(Directory.GetCurrentDirectory() + "\\shell.ini", true);
            ServerKernel.EncryptFiles.Decrypt(Directory.GetCurrentDirectory() + "\\ServerConfiguration.ini");
            _shell.Write("drop_chances", "TEST_CENTER", checkBox5.Checked.ToString());
            _shell.Write("drop_chances", "MonsterFromText", checkBox12.Checked.ToString());
            _shell.Write("drop_chances", "Translator", checkBox13.Checked.ToString());
            _panel.Write("drop_chances", "CHANCE_LETTERS", textBox25.Text);
            _panel.Write("drop_chances", "CHANCE_PLUS_ONE", textBox27.Text);
            _panel.Write("drop_chances", "CHANCE_METEOR", textBox32.Text);
            _panel.Write("drop_chances", "CHANCE_GEMS", textBox31.Text);
            _panel.Write("drop_chances", "CHANCE_STONE_ONE_ITEM", textBox39.Text);
            _panel.Write("drop_chances", "CHANCE_STONE_TWO_ITEM", textBox37.Text);
            _panel.Write("drop_chances", "CHANCE_DRAGONBALL_ITEM", textBox40.Text);
            _panel.Write("drop_chances", "CHANCE_EXPBALL", textBox33.Text);
            _panel.Write("drop_chances", "CHANCE_KEY", textBox330.Text);
            _panel.Write("drop_chances", "SPELL_RATE", textBox34.Text);
            _panel.Write("drop_chances", "PROF_RATE", textBox35.Text);
            _panel.Write("drop_chances", "EXP_RATE", textBox36.Text);
            _panel.Write("drop_chances", "MAX_USER_LOGIN_ON_PC", textBox38.Text);
            _panel.Write("drop_chances", "AWARED_EXPERINCE_FROM_BOT", textBox86.Text);
            _panel.Write("drop_chances", "Max_PLUS", textBox87.Text);
            _panel.Write("drop_chances", "Max_Bless", textBox89.Text);
            _panel.Write("drop_chances", "Max_Enchant", textBox90.Text);
            _panel.Write("drop_chances", "MAX_UPLEVEL", textBox91.Text);
            _panel.Write("drop_chances", "Bound_Equipments_Plus", textBox92.Text);
            _panel.Write("drop_chances", "ViewThreshold", textBox96.Text);
            _panel.Write("drop_rate", "MAXIMUM_LETTER_DAILY_TIMES", textBox73.Text);
            _panel.Write("drop_rate", "CP_MONSTER_DROP_RATE0", textBox54.Text);
            _panel.Write("drop_rate", "CP_MONSTER_DROP_RATE1", textBox53.Text);
            _panel.Write("drop_rate", "NAME_CHANGE", textBox52.Text);
            _panel.Write("drop_rate", "NAME_CHANGE_RESET_LIMIT", textBox51.Text);
            _panel.Write("drop_rate", "GENDAR_CHANGE", textBox50.Text);
            _panel.Write("drop_rate", "CREATE_CLAN", textBox46.Text);
            _panel.Write("drop_rate", "STAY_ONLINE", textBox44.Text);
            _panel.Write("drop_rate", "CONQUER_LETTER_REWARD", textBox43.Text);
            _panel.Write("drop_rate", "TWO_SOC_RATE", textBox42.Text);
            _panel.Write("drop_rate", "MONTHLY_PK_REWARD", textBox49.Text);
            _panel.Write("drop_rate", "QUALIFIER_PK_REWARD", textBox48.Text);
            _panel.Write("drop_rate", "QUALIFIER_HONOR_REWARD", textBox47.Text);
            _panel.Write("drop_rate", "ELITE_GUILD_WAR_Reward", textBox45.Text);
            _panel.Write("drop_rate", "GUILD_WAR_REWARD", textBox56.Text);
            _panel.Write("drop_rate", "CLASS_PK_WAR_REWARD", textBox55.Text);
            _panel.Write("drop_rate", "CLASSIC_CLAN_WAR_REWARD", textBox57.Text);
            _panel.Write("drop_rate", "CAPTURE_THE_FLAG_WAR_REWARD_CPS", textBox58.Text);
            _panel.Write("drop_rate", "CAPTURE_THE_FLAG_WAR_REWARD_MONEY", textBox59.Text);
            _panel.Write("drop_rate", "ONE_SOC_RATE", textBox41.Text);
            _panel.Write("drop_rate", "ELITE_PK_TOURNAMENT_REWARD4", textBox63.Text);
            _panel.Write("drop_rate", "ELITE_PK_TOURNAMENT_REWARD3", textBox64.Text);
            _panel.Write("drop_rate", "ELITE_PK_TOURNAMENT_REWARD2", textBox61.Text);
            _panel.Write("drop_rate", "ELITE_PK_TOURNAMENT_REWARD1", textBox62.Text);
            _panel.Write("drop_rate", "TEAM_PK_TOURNAMENT_REWARD4", textBox65.Text);
            _panel.Write("drop_rate", "TEAM_PK_TOURNAMENT_REWARD3", textBox66.Text);
            _panel.Write("drop_rate", "TEAM_PK_TOURNAMENT_REWARD2", textBox67.Text);
            _panel.Write("drop_rate", "TEAM_PK_TOURNAMENT_REWARD1", textBox68.Text);
            _panel.Write("drop_rate", "SKILL_PK_TOURNAMENT_REWARD4", textBox69.Text);
            _panel.Write("drop_rate", "SKILL_PK_TOURNAMENT_REWARD3", textBox70.Text);
            _panel.Write("drop_rate", "SKILL_PK_TOURNAMENT_REWARD2", textBox71.Text);
            _panel.Write("drop_rate", "SKILL_PK_TOURNAMENT_REWARD1", textBox72.Text);
            _panel.Write("drop_rate", "KILLER_SYSTEM_REWARD4", textBox74.Text);
            _panel.Write("drop_rate", "KILLER_SYSTEM_REWARD3", textBox75.Text);
            _panel.Write("drop_rate", "KILLER_SYSTEM_REWARD2", textBox76.Text);
            _panel.Write("drop_rate", "KILLER_SYSTEM_REWARD1", textBox60.Text);
            _panel.Write("drop_rate", "TREASURE_THIEF_MIN", textBox85.Text);
            _panel.Write("drop_rate", "TREASURE_THIEF_MAX", textBox77.Text);
            _panel.Write("drop_rate", "ARENA_DAILY_RANKING_1ST", textBox95.Text);
            _panel.Write("drop_rate", "ARENA_DAILY_RANKING_2ND", textBox94.Text);
            _panel.Write("drop_rate", "ARENA_DAILY_RANKING_3TH", textBox93.Text);
            _panel.Write("drop_rate", "MONEY_MONSTER_DROP_RATE_VIP", textBox108.Text);
            _panel.Write("drop_rate", "MONEY_MONSTER_DROP_RATE_NORMAL", textBox109.Text);
            _panel.Write("monsters", "MONSTER_SPWANS", textBox110.Text);
            ServerKernel.EncryptFiles.Encrypt(Directory.GetCurrentDirectory() + "\\ServerConfiguration.ini");
            //ServerKernel.AutoMaintenance = checkBox8.Checked;
            //ServerKernel.Maintenance = dateTimePicker1.Value;
            TestUser = checkBox9.Checked;
            MonsterDBSpawn = checkBox14.Checked;
            //if (ServerKernel.AutoMaintenance)
            //    ServerKernel.Log.AppendServerLog($"[AutoMaintenance] Server will Maintenance at {ServerKernel.Maintenance:d/M/yyyy (H:mm)}");
            Server.LoadFileConfiguration();
            //GetDefault(MainFlag.Configrationlist);
            if (trackBar1.Value < 50)
                trackBar1.Value++;
        }

        private void StartMonthlyPk(object sender, EventArgs e)
        {
            MsgSchedules.MonthlyPKWar.Open();
        }

        private void UpdateStaticMessage(object sender, EventArgs e)
        {
            ServerString.GameUpdateMessages[0] = textBox85.Text;
            ServerString.GameUpdateMessages[1] = textBox81.Text;
            ServerString.GameUpdateMessages[2] = textBox80.Text;
            ServerString.GameUpdateMessages[3] = textBox82.Text;
            ServerString.GameUpdateMessages[4] = textBox83.Text;
            ServerString.GameUpdateMessages[5] = textBox84.Text;
            ServerKernel.StaticGUIType = checkBox6.Checked;
            if (trackBar1.Value < 50)
                trackBar1.Value++;
        }

        private void AllowMessageChecked(object sender, EventArgs e)
        {
            ServerKernel.StaticGUIType = checkBox6.Checked;
        }

        private void ClearNobilityUser(object sender, EventArgs e)
        {
            GameClient User;
            User = CurrentlySelectedUser();
            if (User == null || checkBox3.Checked)
                return;
            using (RecycledPacket rec = new RecycledPacket())
            {
                if (User.Player.Nobility != null && User.Player.Nobility.Donation > 50000)
                {
                    Packet stream;
                    stream = rec.GetStream();
                    uint donation;
                    donation = (uint)User.Player.Nobility.Donation / 50000;
                    User.Player.Nobility.Donation = 0;
                    ServerKernel.Log.AppendServerLog($"{User.Player.Name} has update his rank to zero(0) was donate {donation} cps");
                    User.Send(stream.NobilityIconCreate(User.Player.Nobility));
                    Program.NobilityRanking.UpdateRank(User.Player.Nobility);
                    User.Socket.Disconnect();
                    if (trackBar1.Value < 50)
                        trackBar1.Value++;
                }
                else
                    ServerKernel.Log.AppendServerLog(User.Player.Name + " he dosn't have donation higher 50,000");
            }
        }

        private void SaveBossManager(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null)
                {
                    foreach (Boss mob in BossDatabase.Bosses.Values)
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            var row = dataGridView1.Rows[i];
                            string id = row.Cells[0].Value.ToString();
                            if (mob.MonsterID.ToString() == id)
                            {
                                try
                                {
                                    ushort.TryParse(row.Cells[2].Value.ToString(), out var StudyPoints);
                                    mob.StudyPoints = StudyPoints;
                                    uint.TryParse(row.Cells[3].Value.ToString(), out var ConquerPointDropped);
                                    mob.ConquerPointDropped = ConquerPointDropped;
                                    byte.TryParse(row.Cells[4].Value.ToString(), out var SoulDropped);
                                    mob.SoulDropped = SoulDropped;
                                    byte.TryParse(row.Cells[5].Value.ToString(), out var MaxSoulDropped);
                                    mob.MaxSoulDropped = MaxSoulDropped;
                                    byte.TryParse(row.Cells[6].Value.ToString(), out var RefinaryDropped);
                                    mob.RefinaryDropped = RefinaryDropped;
                                    byte.TryParse(row.Cells[7].Value.ToString(), out var MaxRefienryDropped);
                                    mob.MaxRefienryDropped = MaxRefienryDropped;
                                    mob.Items.Clear();
                                    mob.Items.AddRange(row.Cells[8].Value.ToString().Split(','));
                                    if (string.IsNullOrEmpty(row.Cells[8].Value.ToString()))
                                    {
                                        MessageBox.Show("Error: Items");
                                        break;
                                    }
                                    uint.TryParse(row.Cells[9].Value.ToString(), out var ConquerPointScores);
                                    mob.ConquerPointScores = ConquerPointScores;
                                    uint.TryParse(row.Cells[10].Value.ToString(), out var BossPointScores);
                                    mob.BossPointScores = BossPointScores;
                                    uint.TryParse(row.Cells[11].Value.ToString(), out var five);
                                    mob.ItemDropScores = five;
                                    ushort.TryParse(row.Cells[12].Value.ToString(), out var MapID);
                                    if (MapID <= 0)
                                    {
                                        MessageBox.Show("Error: MapID");
                                        break;
                                    }
                                    mob.MapID = MapID;
                                    if (string.IsNullOrEmpty(row.Cells[13].Value.ToString()))
                                    {
                                        MessageBox.Show("Error: X1");
                                        break;
                                    }
                                    mob.X.Clear();
                                    mob.X.AddRange(row.Cells[13].Value.ToString().Split(','));
                                    if (string.IsNullOrEmpty(row.Cells[14].Value.ToString()))
                                    {
                                        MessageBox.Show("Error: X2");
                                        break;
                                    }
                                    mob.Y.Clear();
                                    mob.Y.AddRange(row.Cells[14].Value.ToString().Split(','));
                                    if (string.IsNullOrEmpty(row.Cells[15].Value.ToString()))
                                    {
                                        MessageBox.Show("Error: X3");
                                        break;
                                    }
                                    mob.SpawnMinutes.Clear();
                                    mob.SpawnMinutes.AddRange(row.Cells[15].Value.ToString().Split(','));
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    MessageBox.Show("Done");
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to save monster value", true, LogType.WARNING);
            }
        }

        private void RefreshBossAlive(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            listBox3.Text = "";
            FillBossInformationEmpty();
            foreach (Boss boss in BossDatabase.Bosses.Values)
            {
                if (boss.Alive)
                    listBox3.Items.Add(boss.Name ?? "");
            }
            if (trackBar1.Value < 50)
                trackBar1.Value++;
        }

        private void SelectedBossIndex(object sender, EventArgs e)
        {
            MonsterRole monsterRole = this.CurrentlySelectedBoss();
            if (monsterRole == null)
                return;
            this.textBox100.Text = monsterRole.Family.MinAttack.ToString();
            this.textBox99.Text = monsterRole.Family.MaxAttack.ToString();
            this.textBox102.Text = monsterRole.Family.Defense.ToString();
            this.textBox101.Text = monsterRole.Family.Defense2.ToString();
            this.textBox103.Text = monsterRole.HitPoints.ToString();
            this.textBox104.Text = monsterRole.Family.Level.ToString();
            TextBox textBox105 = this.textBox105;
            ushort num = monsterRole.X;
            string str1 = num.ToString();
            textBox105.Text = str1;
            this.textBox106.Text = monsterRole.Map.ToString();
            TextBox textBox107 = this.textBox107;
            num = monsterRole.Y;
            string str2 = num.ToString();
            textBox107.Text = str2;
            try
            {
                this.pictureBox3.Image = Image.FromFile("Resources/boss/" + monsterRole.Name + ".png");
            }
            catch
            {
                ServerKernel.Log.SaveLog("Boss PictureID not found in Resources/boss/" + monsterRole.Name + ".png", true, LogType.WARNING);
            }
        }

        private void KickoutBoss(object sender, EventArgs e)
        {
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
                Packet stream = recycledPacket.GetStream();
                MonsterRole monsterRole = this.CurrentlySelectedBoss();
                if (monsterRole == null)
                    return;
                GameMap serverMap = Server.ServerMaps[monsterRole.Map];
                serverMap.SetMonsterOnTile(monsterRole.X, monsterRole.Y, false);
                monsterRole.Dead(stream, (GameClient)null, monsterRole.UID, serverMap, BossID: monsterRole.UID);
                this.textBox100.Text = "0";
                this.textBox99.Text = "0";
                this.textBox102.Text = "0";
                this.textBox101.Text = "0";
                this.textBox103.Text = "0";
                this.textBox104.Text = "0";
                this.textBox105.Text = "0";
                this.textBox106.Text = "0";
                this.textBox107.Text = "0";
                this.pictureBox3.Image = (Image)null;
                this.RefreshBossAlive((object)null, (EventArgs)null);
            }
        }

        private void SaveBossStats(object sender, EventArgs e)
        {
            MonsterRole monsterRole = this.CurrentlySelectedBoss();
            if (monsterRole == null)
                return;
            int result1;
            int.TryParse(this.textBox100.Text.ToString(), out result1);
            int result2;
            int.TryParse(this.textBox99.Text.ToString(), out result2);
            ushort result3;
            ushort.TryParse(this.textBox102.Text.ToString(), out result3);
            int result4;
            int.TryParse(this.textBox101.Text.ToString(), out result4);
            monsterRole.Family.MinAttack = result1;
            monsterRole.Family.MaxAttack = result2;
            monsterRole.Family.Defense = result3;
            monsterRole.Family.Defense2 = result4;
            IniFile write;
            write = new IniFile("\\Monsters\\" + monsterRole.Name + ".ini");
            write.Write<int>("cq_monstertype", "attack_min", monsterRole.Family.MinAttack);
            write.Write<int>("cq_monstertype", "attack_max", monsterRole.Family.MaxAttack);
            write.Write<int>("cq_monstertype", "defence", monsterRole.Family.Defense);
            write.Write<int>("cq_monstertype", "defence2", monsterRole.Family.Defense2);
            if (this.trackBar1.Value >= 50)
                return;
            ++this.trackBar1.Value;
        }

        private void AddingBoss(object sender, EventArgs e)
        {
            try
            {
                string Name = this.textBox98.Text;
                string text = this.textBox97.Text;
                uint MonsterID;
                uint.TryParse(text.ToString(), out MonsterID);
                if (string.IsNullOrEmpty(text))
                {
                    int num1 = (int)MessageBox.Show("Boss ID IsNullOrEmpty! try other id");
                }
                else if (string.IsNullOrEmpty(Name))
                {
                    int num2 = (int)MessageBox.Show("Boss Name IsNullOrEmpty! try other name");
                }
                else
                {
                    MonsterRole monsterRole1 = Server.MonsterRole.Values.Where<MonsterRole>((Func<MonsterRole, bool>)(p => (int)p.UID == (int)MonsterID)).FirstOrDefault<MonsterRole>();
                    MonsterRole monsterRole2 = Server.MonsterRole.Values.Where<MonsterRole>((Func<MonsterRole, bool>)(p => p.Name == Name)).FirstOrDefault<MonsterRole>();
                    if (monsterRole1 != null)
                    {
                        int num3 = (int)MessageBox.Show("Boss ID Founded! try other id");
                    }
                    else if (monsterRole2 != null)
                    {
                        int num4 = (int)MessageBox.Show("Boss Name Founded! try other name");
                    }
                    else
                    {
                        Boss boss = new Boss()
                        {
                            Name = Name,
                            Type = Boss.SpawnType.Hourly,
                            MonsterID = MonsterID
                        };
                        string str1 = "0,0,0";
                        string str2 = "0,0,0";
                        string str3 = "0,0";
                        boss.X.AddRange((IEnumerable<string>)str1.Split(','));
                        boss.Y.AddRange((IEnumerable<string>)str2.Split(','));
                        boss.SpawnHours.AddRange((IEnumerable<string>)str3.Split(','));
                        boss.SpawnMinutes.AddRange((IEnumerable<string>)str3.Split(','));
                        boss.StudyPoints = (ushort)0;
                        boss.ConquerPointDropped = 0U;
                        boss.SoulDropped = (byte)0;
                        boss.MaxSoulDropped = (byte)0;
                        boss.RefinaryDropped = (byte)0;
                        boss.MaxRefienryDropped = (byte)0;
                        string str4 = "0,0,0";
                        boss.Items.AddRange((IEnumerable<string>)str4.Split(','));
                        boss.ConquerPointScores = 0U;
                        boss.BossPointScores = 0U;
                        boss.ItemDropScores = 0U;
                        BossDatabase.Bosses.Add(boss.MonsterID, boss);
                        this.GetDefault(ServerManager.MainFlag.Bosseslist);
                        if (this.trackBar1.Value >= 50)
                            return;
                        ++this.trackBar1.Value;
                    }
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to add boss!", true, LogType.WARNING);
            }
        }

        private void ChangeWeather(object sender, EventArgs e)
        {
            try
            {
                bool done;
                done = true;
                MsgWeather.WeatherType Weather;
                Weather = MsgWeather.WeatherType.Nothing;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet packet;
                    packet = rec.GetStream();
                    string text;
                    text = comboBox14.Text;
                    if (text == null)
                        goto IL_0150;
                    switch (text.Length)
                    {
                        case 4:
                            break;
                        case 7:
                            goto IL_008e;
                        case 8:
                            goto IL_00ca;
                        case 13:
                            goto IL_00da;
                        case 23:
                            goto IL_00ea;
                        case 19:
                            goto IL_00fa;
                        case 5:
                            goto IL_010a;
                        case 12:
                            goto IL_011a;
                        default:
                            goto IL_0150;
                    }
                    char c;
                    c = text[0];
                    if (c != 'R')
                    {
                        if (c != 'S' || !(text == "Snow"))
                            goto IL_0150;
                        Weather = MsgWeather.WeatherType.Snow;
                    }
                    else
                    {
                        if (!(text == "Rain"))
                            goto IL_0150;
                        Weather = MsgWeather.WeatherType.Rain;
                    }
                    goto IL_0152;
                IL_00ca:
                    if (!(text == "RainWind"))
                        goto IL_0150;
                    Weather = MsgWeather.WeatherType.RainWind;
                    goto IL_0152;
                IL_00ea:
                    if (!(text == "CherryBlossomPetalsWind"))
                        goto IL_0150;
                    Weather = MsgWeather.WeatherType.CherryBlossomPetalsWind;
                    goto IL_0152;
                IL_0152:
                    if (done)
                    {
                        foreach (GameMap map in Server.ServerMaps.Values)
                        {
                            map.Weather = Weather;
                        }
                        ServerKernel.Log.AppendServerLog($"Game Weather is updated to {Weather}");
                    }
                    else
                        ServerKernel.Log.AppendServerLog("Failed to update game weather");
                    return;
                IL_00fa:
                    if (!(text == "CherryBlossomPetals"))
                        goto IL_0150;
                    Weather = MsgWeather.WeatherType.CherryBlossomPetals;
                    goto IL_0152;
                IL_010a:
                    if (!(text == "Atoms"))
                        goto IL_0150;
                    Weather = MsgWeather.WeatherType.Atoms;
                    goto IL_0152;
                IL_00da:
                    if (!(text == "BlowingCotten"))
                        goto IL_0150;
                    Weather = MsgWeather.WeatherType.BlowingCotten;
                    goto IL_0152;
                IL_0150:
                    done = false;
                    goto IL_0152;
                IL_011a:
                    if (!(text == "AutumnLeaves"))
                        goto IL_0150;
                    Weather = MsgWeather.WeatherType.AutumnLeaves;
                    goto IL_0152;
                IL_008e:
                    if (!(text == "Nothing"))
                        goto IL_0150;
                    Weather = MsgWeather.WeatherType.Nothing;
                    goto IL_0152;
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Weather cannot change!", true, LogType.WARNING);
            }
        }

        private void FixedGameMap(object sender, EventArgs e)
        {
            try
            {
                Dictionary<int, string> maps;
                maps = new Dictionary<int, string>();
                using (BinaryReader gamemap = new BinaryReader(new FileStream(Path.Combine(ServerKernel.CO2FOLDER, "ini/gamemap.dat"), FileMode.Open)))
                {
                    int amount;
                    amount = gamemap.ReadInt32();
                    for (int i = 0; i < amount; i++)
                    {
                        int id;
                        id = gamemap.ReadInt32();
                        string fileName;
                        fileName = Encoding.ASCII.GetString(gamemap.ReadBytes(gamemap.ReadInt32()));
                        int puzzleSize;
                        puzzleSize = gamemap.ReadInt32();
                        ServerKernel.Log.AppendServerLog($"Map id: {id} --> Size: {puzzleSize} loaded..");
                        if (!maps.ContainsKey(id))
                            maps.Add(id, fileName);
                        else
                            maps[id] = fileName;
                    }
                    if (trackBar1.Value < 50)
                        trackBar1.Value++;
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to fixed gamemap!", true, LogType.WARNING);
            }
        }

        public void FetchEventRewards()
        {
            try
            {
                dataGridView3.Rows.Clear();
                dataGridView3.Columns.Clear();
                dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView3.ColumnCount = 6;
                dataGridView3.Columns[0].Name = "ID";
                dataGridView3.Columns[1].Name = "Event_Name";
                dataGridView3.Columns[2].Name = "Reward(CPS)";
                dataGridView3.Columns[3].Name = "Reward(Silver)";
                dataGridView3.Columns[4].Name = "Reward(Item)";
                dataGridView3.Columns[5].Name = "PlayerName(Time)";
                dataGridView3.Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Italic);
                string[] row;
                row = new string[6];
                foreach (IEventRewards bkh in ServerKernel.EventRewards.Values)
                {
                    if (bkh != null)
                    {
                        row[0] = bkh.ID.ToString();
                        row[1] = bkh.Name.ToString();
                        row[2] = bkh.ClaimConquerPoint.ToString();
                        row[3] = bkh.ClaimMoney.ToString();
                        row[4] = bkh.ClamedItem.ToString();
                        row[5] = bkh.Data.ToString();
                        DataGridViewRowCollection rows;
                        rows = dataGridView3.Rows;
                        object[] values;
                        values = row;
                        rows.Add(values);
                    }
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to adding IEventRewards list!", true, LogType.WARNING);
            }
        }

        private void TournamentSchedules(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView2.CurrentRow == null)
                    return;
                foreach (ISchedule tour in ISchedule.Schedules.Values)
                {
                    if (!(tour.Name == dataGridView2.CurrentRow.Cells[1].Value.ToString()))
                        continue;
                    string StartDay;
                    StartDay = dataGridView2.CurrentRow.Cells[2].Value.ToString().ToLower();
                    string StartHour;
                    StartHour = dataGridView2.CurrentRow.Cells[3].Value.ToString();
                    string StartMinute;
                    StartMinute = dataGridView2.CurrentRow.Cells[4].Value.ToString();
                    string EndHour;
                    EndHour = dataGridView2.CurrentRow.Cells[5].Value.ToString();
                    string EndMinute;
                    EndMinute = dataGridView2.CurrentRow.Cells[6].Value.ToString();
                    if (StartDay == null)
                        goto IL_028b;
                    switch (StartDay.Length)
                    {
                        case 6:
                            break;
                        case 8:
                            goto IL_0156;
                        case 7:
                            goto IL_01bb;
                        case 9:
                            goto IL_01cd;
                        default:
                            goto IL_028b;
                    }
                    char c;
                    c = StartDay[0];
                    if (c != 'f')
                    {
                        if (c != 'm')
                        {
                            if (c != 's' || !(StartDay == "sunday"))
                                goto IL_028b;
                            tour.EveryDay = false;
                            tour.DayOfWeek = DayOfWeek.Sunday;
                        }
                        else
                        {
                            if (!(StartDay == "monday"))
                                goto IL_028b;
                            tour.EveryDay = false;
                            tour.DayOfWeek = DayOfWeek.Monday;
                        }
                    }
                    else
                    {
                        if (!(StartDay == "friday"))
                            goto IL_028b;
                        tour.EveryDay = false;
                        tour.DayOfWeek = DayOfWeek.Friday;
                    }
                    goto IL_0296;
                IL_01bb:
                    if (!(StartDay == "tuesday"))
                        goto IL_028b;
                    tour.EveryDay = false;
                    tour.DayOfWeek = DayOfWeek.Tuesday;
                    goto IL_0296;
                IL_028b:
                    MessageBox.Show("Error: Start day");
                    goto IL_0296;
                IL_0156:
                    c = StartDay[0];
                    if (c != 'e')
                    {
                        if (c != 's')
                        {
                            if (c != 't' || !(StartDay == "thursday"))
                                goto IL_028b;
                            tour.EveryDay = false;
                            tour.DayOfWeek = DayOfWeek.Thursday;
                        }
                        else
                        {
                            if (!(StartDay == "saturday"))
                                goto IL_028b;
                            tour.EveryDay = false;
                            tour.DayOfWeek = DayOfWeek.Saturday;
                        }
                    }
                    else
                    {
                        if (!(StartDay == "everyday"))
                            goto IL_028b;
                        tour.EveryDay = true;
                    }
                    goto IL_0296;
                IL_01cd:
                    if (!(StartDay == "wednesday"))
                        goto IL_028b;
                    tour.EveryDay = false;
                    tour.DayOfWeek = DayOfWeek.Wednesday;
                    goto IL_0296;
                IL_0296:
                    if (string.IsNullOrEmpty(StartDay))
                    {
                        MessageBox.Show("Error: StartDay should be right name like: 'sunday' or 'everyday' ");
                        break;
                    }
                    if (string.IsNullOrEmpty(StartHour))
                    {
                        MessageBox.Show("Error: StartHour max: 24");
                        break;
                    }
                    if (string.IsNullOrEmpty(StartMinute))
                    {
                        MessageBox.Show("Error: StartMinute max: 59");
                        break;
                    }
                    if (string.IsNullOrEmpty(EndHour))
                    {
                        MessageBox.Show("Error: EndMinute max: 24");
                        break;
                    }
                    if (string.IsNullOrEmpty(EndMinute))
                    {
                        MessageBox.Show("Error: EndMinute max: 59");
                        break;
                    }
                    uint.TryParse(StartHour, out var start_hour);
                    uint.TryParse(StartMinute, out var start_minute);
                    uint.TryParse(EndHour, out var end_hour);
                    uint.TryParse(EndMinute, out var end_minute);
                    tour.StartDay = StartDay;
                    tour.StartHour = start_hour;
                    tour.StartMinute = start_minute;
                    tour.EndHour = end_hour;
                    tour.EndMinute = end_minute;
                    GetDefault(MainFlag.TournamentTimerlist);
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to Save Schedules!", true, LogType.WARNING);
            }
        }

        private void ResetValueFromUser(object sender, EventArgs e)
        {
            string _Name;
            _Name = dataGridView4.Columns[dataGridView4.CurrentCell.ColumnIndex].Name;
            string TableValue;
            TableValue = dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells[dataGridView4.CurrentCell.ColumnIndex].Value.ToString();
            try
            {
                string TableName;
                TableName = _Name;
                int current;
                current = Server.GamePoll.Count;
                if (current > 0)
                {
                    ServerKernel.Log.AppendServerLog(string.Format("cannot reset {1} someone stay online :{0}", current, TableName));
                    return;
                }
                IniFile ini;
                ini = new IniFile("");
                int Count;
                Count = 0;
                string[] files;
                files = Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\");
                foreach (string fname in files)
                {
                    ini.FileName = fname;
                    ulong.TryParse(TableValue, out var perc);
                    if (perc == 0)
                        return;
                    ulong value;
                    value = ini.ReadUInt64("Character", TableName, 0uL);
                    string Name;
                    Name = ini.ReadString("Character", "Name", "");
                    value -= value * perc / 100uL;
                    ini.Write("Character", TableName, value);
                    if (!string.IsNullOrEmpty(Name))
                        ServerKernel.Log.AppendGameLog(string.Format("{0} has rest his {2} {1}%", Name, perc, TableName));
                    Count++;
                }
                dataGridView4.CurrentRow.Cells[1].Value = 0;
                ServerKernel.Log.AppendServerLog(string.Format("{2} was rested {1}% : Count :{0}", Count, TableValue, TableName));
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to Reset " + _Name + "!", true, LogType.WARNING);
            }
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex;
                selectedIndex = LbxOnScreen.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < LbxOnScreen.Items.Count)
                {
                    string Name;
                    Name = LbxOnScreen.Items[selectedIndex].ToString();
                    GameClient client;
                    client = GameClient.CharacterFromName(Name);
                    if (client != null)
                        FillUserInformation(client.Player);
                }
            }
            catch
            {
            }
        }

        private void GetLiveTournament(object sender, EventArgs e)
        {
            int selectedIndex;
            selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < listBox1.Items.Count)
            {
                string _tournament_selected;
                _tournament_selected = listBox1.Items[selectedIndex].ToString();
                string[] _get_tournament;
                _get_tournament = _tournament_selected.Split(' ').ToArray();
                byte _found_tournament;
                _found_tournament = byte.Parse(_get_tournament[0]);
                ITournamentsAlive.ID TournamentID;
                TournamentID = (ITournamentsAlive.ID)_found_tournament;
                uint DinamicID;
                DinamicID = 0u;
                uint MapID;
                MapID = 0u;
                switch (TournamentID)
                {
                    case ITournamentsAlive.ID.CaptureTheFlag:
                        MapID = MsgSchedules.CaptureTheFlag.Map.ID;
                        DinamicID = 255u;
                        break;
                    case ITournamentsAlive.ID.ClanWar:
                        MapID = MsgSchedules.ClanWar.Map.ID;
                        DinamicID = 255u;
                        break;
                    case ITournamentsAlive.ID.ElitePkTournament:
                        {
                            MsgEliteGroup EPK_Lvl130Plus;
                            EPK_Lvl130Plus = MsgEliteTournament.EliteGroups[3];
                            DinamicID = EPK_Lvl130Plus.DinamycID;
                            MapID = EPK_Lvl130Plus.Map.ID;
                            break;
                        }
                    case ITournamentsAlive.ID.LastmanStand:
                        MapID = MsgLastManStand.MapID;
                        DinamicID = MsgLastManStand.DinimycID;
                        break;
                    case ITournamentsAlive.ID.TeamPkTournament:
                        {
                            MsgTeamEliteGroup EPK_Lvl130Plus2;
                            EPK_Lvl130Plus2 = MsgTeamPkTournament.EliteGroups[3];
                            DinamicID = EPK_Lvl130Plus2.DinamycID;
                            MapID = EPK_Lvl130Plus2.Map.ID;
                            break;
                        }
                    case ITournamentsAlive.ID.FB_SS_Tournament:
                        MapID = 1505u;
                        DinamicID = 255u;
                        break;
                    case ITournamentsAlive.ID.SkillTeamPkTournament:
                        {
                            MsgTeamEliteGroup EPK_Lvl130Plus3;
                            EPK_Lvl130Plus3 = MsgSkillTeamPkTournament.EliteGroups[3];
                            DinamicID = EPK_Lvl130Plus3.DinamycID;
                            MapID = EPK_Lvl130Plus3.Map.ID;
                            break;
                        }
                    case ITournamentsAlive.ID.ClassPkWar:
                        MapID = 1764u;
                        DinamicID = 255u;
                        break;
                    case ITournamentsAlive.ID.GuildWar:
                        MapID = 1038u;
                        DinamicID = 255u;
                        break;
                    case ITournamentsAlive.ID.EliteGuildWar:
                        MapID = 2071u;
                        DinamicID = 255u;
                        break;
                    case ITournamentsAlive.ID.TreasureThief:
                        MapID = MsgTreasureThief.MapID;
                        DinamicID = 255u;
                        break;
                    //case ITournamentsAlive.ID.FindTheBox:
                    //    MapID = MsgFindTheBox.MapID;
                    //    DinamicID = 255u;
                    //    break;
                    case ITournamentsAlive.ID.WeeklyPK:
                        MapID = 1508u;
                        DinamicID = 255u;
                        break;
                    case ITournamentsAlive.ID.MonthlyPK:
                        MapID = 1505u;
                        DinamicID = 255u;
                        break;
                    case ITournamentsAlive.ID.HeroOfGame:
                        MapID = MsgHeroOfGame.MapID;
                        DinamicID = MsgHeroOfGame.DinimycID;
                        break;
                }
                Live form;
                form = new Live(TournamentID.ToString(), MapID, DinamicID, Live.LiveList.Tournament);
                form.Show();
            }
        }

        private void RefreshAllTournaments(object sender, EventArgs e)
        {
            GetDefault(MainFlag.Tournamentlist);
        }

        private void ScoreboardAliveMonster(object sender, EventArgs e)
        {
            MonsterRole role;
            role = CurrentlySelectedBoss();
            if (role == null)
                return;
            Live form;
            form = new Live(role.Name, role.Map, role.DynamicID, Live.LiveList.Boss);
            form.Show();
            try
            {
                pictureBox3.Image = Image.FromFile("Resources/boss/" + role.Name + ".png");
            }
            catch
            {
                ServerKernel.Log.SaveLog("Boss PictureID not found in Resources/boss/" + role.Name + ".png", true, LogType.WARNING);
            }
        }

        private void FetchEventRewards(object sender, EventArgs e)
        {
            ServerKernel.ServerManager.FetchEventRewards();
        }

        private void RefreshBotAlives(object sender, EventArgs e)
        {
            GetDefault(MainFlag.Botslist);
        }

        private void ViewEquipmentUser(object sender, EventArgs e)
        {
            GameClient client;
            client = CurrentlySelectedUser();
            if (client != null)
            {
                PlayerEquipments form;
                form = new PlayerEquipments(client);
                form.Show();
            }
        }

        private void SaveTournamentPrizes(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView5.CurrentRow == null)
                    return;
                foreach (ISchedule tour in ISchedule.Schedules.Values)
                {
                    if (tour.Name == dataGridView5.CurrentRow.Cells[1].Value.ToString())
                    {
                        string ItemReward1;
                        ItemReward1 = dataGridView5.CurrentRow.Cells[2].Value.ToString();
                        string ItemReward2;
                        ItemReward2 = dataGridView5.CurrentRow.Cells[3].Value.ToString();
                        string ItemReward3;
                        ItemReward3 = dataGridView5.CurrentRow.Cells[4].Value.ToString();
                        uint.TryParse(ItemReward1, out var itemone);
                        uint.TryParse(ItemReward2, out var itemtwo);
                        uint.TryParse(ItemReward3, out var itemthree);
                        tour.ItemOne = itemone;
                        tour.ItemTwo = itemtwo;
                        tour.ItemThree = itemthree;
                    }
                }
                ISchedule.Save();
            }
            catch
            {
                ServerKernel.Log.SaveLog("Faild to Save Schedules Prizes!", true, LogType.WARNING);
            }
        }

        public string GetImagePath(string UID)
        {
            IniFile ItemMinIcon;
            ItemMinIcon = new IniFile("\\ItemMinIcon.ini");
            string ItemIDIcon;
            ItemIDIcon = "Item" + UID;
            string ItemID;
            ItemID = ItemMinIcon.ReadString(ItemIDIcon, "Frame0", "ItemDefault");
            return ServerKernel.CO2FOLDER + ItemID;
        }

        private void GiveItemsSelected(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox2.Text))
            {
                string[] accessory;
                accessory = comboBox2.Text.Split(' ').ToArray();
                pictureBox8.ImageLocation = GetImagePath(uint.Parse(accessory[1]).ToString());
            }
            if (!string.IsNullOrEmpty(comboBox3.Text))
            {
                string[] itemid;
                itemid = comboBox3.Text.Split(' ').ToArray();
                pictureBox6.ImageLocation = GetImagePath(uint.Parse(itemid[1]).ToString());
            }
            if (!string.IsNullOrEmpty(comboBox4.Text))
            {
                string[] garment;
                garment = comboBox4.Text.Split(' ').ToArray();
                pictureBox7.ImageLocation = GetImagePath(uint.Parse(garment[1]).ToString());
            }
        }

        private void ReqLev70_CheckedChanged(object sender, EventArgs e)
        {
            byte Class;
            Class = 0;
            string text;
            text = comboBox15.Text;
            if (text != null)
            {
                switch (text.Length)
                {
                    case 6:
                        switch (text[0])
                        {
                            case 'T':
                                if (text == "Trojan")
                                    Class = 15;
                                break;
                            case 'A':
                                if (text == "Archer")
                                    Class = 45;
                                break;
                        }
                        break;
                    case 5:
                        switch (text[0])
                        {
                            case 'N':
                                if (text == "Ninja")
                                    Class = 55;
                                break;
                            case 'W':
                                if (text == "Water")
                                    Class = 135;
                                break;
                        }
                        break;
                    case 4:
                        switch (text[0])
                        {
                            case 'M':
                                if (text == "Monk")
                                    Class = 65;
                                break;
                            case 'F':
                                if (text == "Fire")
                                    Class = 145;
                                break;
                        }
                        break;
                    case 7:
                        if (text == "Warrior")
                            Class = 25;
                        break;
                }
            }
            var RndAccessory = Database.ItemType.RareGarments2;
            var Position = Program.GetRandom.Next(0, RndAccessory.Count);
            var ReceiveItem = RndAccessory.ToArray()[Position];
            //client.Inventory.Add(stream, ReceiveItem);
            string[] Troj;
            Troj = new string[10] { "120129", "160139", "150139", "118069", "130069", "480139", "410139", "0", "0", ""+ ReceiveItem +""};
            if (AtributesStatus.IsTrojan(Class))
            {
                textBox11.Text = Troj[0];
                TxtBotBoots.Text = Troj[1];
                textBox10.Text = Troj[2];
                textBox9.Text = Troj[3];
                TxtBotArmors.Text = Troj[4];
                TxtBotRightWeapon.Text = Troj[5];
                TxtBotLeftWeapon.Text = Troj[6];
                TxtBotStarTower.Text = Troj[7];
                TxtBotHeavenFan.Text = Troj[8];
                TxtBotRidingCrop.Text = Troj[9];
            }
            string[] Warr;
            Warr = new string[10] { "120129", "160139", "150139", "111069", "131069", "560139", "0", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsWarrior(Class))
            {
                textBox11.Text = Warr[0];
                TxtBotBoots.Text = Warr[1];
                textBox10.Text = Warr[2];
                textBox9.Text = Warr[3];
                TxtBotArmors.Text = Warr[4];
                TxtBotRightWeapon.Text = Warr[5];
                TxtBotLeftWeapon.Text = Warr[6];
                TxtBotStarTower.Text = Warr[7];
                TxtBotHeavenFan.Text = Warr[8];
                TxtBotRidingCrop.Text = Warr[9];
            }
            string[] Arch;
            Arch = new string[10] { "120129", "160139", "150139", "113039", "133069", "500139", "1050002", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsArcher(Class))
            {
                textBox11.Text = Arch[0];
                TxtBotBoots.Text = Arch[1];
                textBox10.Text = Arch[2];
                textBox9.Text = Arch[3];
                TxtBotArmors.Text = Arch[4];
                TxtBotRightWeapon.Text = Arch[5];
                TxtBotLeftWeapon.Text = Arch[6];
                TxtBotStarTower.Text = Arch[7];
                TxtBotHeavenFan.Text = Arch[8];
                TxtBotRidingCrop.Text = Arch[9];
            }
            string[] Ninja;
            Ninja = new string[10] { "120129", "160139", "150139", "112069", "135069", "601139", "601139", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsNinja(Class))
            {
                textBox11.Text = Ninja[0];
                TxtBotBoots.Text = Ninja[1];
                textBox10.Text = Ninja[2];
                textBox9.Text = Ninja[3];
                TxtBotArmors.Text = Ninja[4];
                TxtBotRightWeapon.Text = Ninja[5];
                TxtBotLeftWeapon.Text = Ninja[6];
                TxtBotStarTower.Text = Ninja[7];
                TxtBotHeavenFan.Text = Ninja[8];
                TxtBotRidingCrop.Text = Ninja[9];
            }
            string[] Monk;
            Monk = new string[10] { "120129", "160139", "150139", "143069", "136069", "610139", "610139", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsMonk(Class))
            {
                textBox11.Text = Monk[0];
                TxtBotBoots.Text = Monk[1];
                textBox10.Text = Monk[2];
                textBox9.Text = Monk[3];
                TxtBotArmors.Text = Monk[4];
                TxtBotRightWeapon.Text = Monk[5];
                TxtBotLeftWeapon.Text = Monk[6];
                TxtBotStarTower.Text = Monk[7];
                TxtBotHeavenFan.Text = Monk[8];
                TxtBotRidingCrop.Text = Monk[9];
            }
            string[] Taoist;
            Taoist = new string[10] { "121129", "160139", "152129", "117069", "134069", "421139", "0", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsTaoist(Class))
            {
                textBox11.Text = Taoist[0];
                TxtBotBoots.Text = Taoist[1];
                textBox10.Text = Taoist[2];
                textBox9.Text = Taoist[3];
                TxtBotArmors.Text = Taoist[4];
                TxtBotRightWeapon.Text = Taoist[5];
                TxtBotLeftWeapon.Text = Taoist[6];
                TxtBotStarTower.Text = Taoist[7];
                TxtBotHeavenFan.Text = Taoist[8];
                TxtBotRidingCrop.Text = Taoist[9];
            }
        }

        private void ReqLev100_CheckedChanged(object sender, EventArgs e)
        {
            byte Class;
            Class = 0;
            string text;
            text = comboBox15.Text;
            if (text != null)
            {
                switch (text.Length)
                {
                    case 6:
                        switch (text[0])
                        {
                            case 'T':
                                if (text == "Trojan")
                                    Class = 15;
                                break;
                            case 'A':
                                if (text == "Archer")
                                    Class = 45;
                                break;
                        }
                        break;
                    case 5:
                        switch (text[0])
                        {
                            case 'N':
                                if (text == "Ninja")
                                    Class = 55;
                                break;
                            case 'W':
                                if (text == "Water")
                                    Class = 135;
                                break;
                        }
                        break;
                    case 4:
                        switch (text[0])
                        {
                            case 'M':
                                if (text == "Monk")
                                    Class = 65;
                                break;
                            case 'F':
                                if (text == "Fire")
                                    Class = 145;
                                break;
                        }
                        break;
                    case 7:
                        if (text == "Warrior")
                            Class = 25;
                        break;
                }
            }
            var RndAccessory = Database.ItemType.RareGarments2;
            var Position = Program.GetRandom.Next(0, RndAccessory.Count);
            var ReceiveItem = RndAccessory.ToArray()[Position];
            string[] Troj;
            Troj = new string[10] { "120189", "160199", "150199", "118089", "130089", "480199", "420199", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsTrojan(Class))
            {
                textBox11.Text = Troj[0];
                TxtBotBoots.Text = Troj[1];
                textBox10.Text = Troj[2];
                textBox9.Text = Troj[3];
                TxtBotArmors.Text = Troj[4];
                TxtBotRightWeapon.Text = Troj[5];
                TxtBotLeftWeapon.Text = Troj[6];
                TxtBotStarTower.Text = Troj[7];
                TxtBotHeavenFan.Text = Troj[8];
                TxtBotRidingCrop.Text = Troj[9];
            }
            string[] Warr;
            Warr = new string[10] { "120189", "160199", "150199", "111089", "131089", "560199", "0", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsWarrior(Class))
            {
                textBox11.Text = Warr[0];
                TxtBotBoots.Text = Warr[1];
                textBox10.Text = Warr[2];
                textBox9.Text = Warr[3];
                TxtBotArmors.Text = Warr[4];
                TxtBotRightWeapon.Text = Warr[5];
                TxtBotLeftWeapon.Text = Warr[6];
                TxtBotStarTower.Text = Warr[7];
                TxtBotHeavenFan.Text = Warr[8];
                TxtBotRidingCrop.Text = Warr[9];
            }
            string[] Arch;
            Arch = new string[10] { "120189", "160199", "150199", "113079", "133089", "500189", "1050002", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsArcher(Class))
            {
                textBox11.Text = Arch[0];
                TxtBotBoots.Text = Arch[1];
                textBox10.Text = Arch[2];
                textBox9.Text = Arch[3];
                TxtBotArmors.Text = Arch[4];
                TxtBotRightWeapon.Text = Arch[5];
                TxtBotLeftWeapon.Text = Arch[6];
                TxtBotStarTower.Text = Arch[7];
                TxtBotHeavenFan.Text = Arch[8];
                TxtBotRidingCrop.Text = Arch[9];
            }
            string[] Ninja;
            Ninja = new string[10] { "120189", "160199", "150199", "112089", "135089", "601199", "601199", "201009", "202009", "" + ReceiveItem + "" };
            if (AtributesStatus.IsNinja(Class))
            {
                textBox11.Text = Ninja[0];
                TxtBotBoots.Text = Ninja[1];
                textBox10.Text = Ninja[2];
                textBox9.Text = Ninja[3];
                TxtBotArmors.Text = Ninja[4];
                TxtBotRightWeapon.Text = Ninja[5];
                TxtBotLeftWeapon.Text = Ninja[6];
                TxtBotStarTower.Text = Ninja[7];
                TxtBotHeavenFan.Text = Ninja[8];
                TxtBotRidingCrop.Text = Ninja[9];
            }
            string[] Monk;
            Monk = new string[10] { "120189", "160199", "150199", "143089", "136089", "610199", "610199", "201009", "202009", "" + ReceiveItem + "" };
            if (AtributesStatus.IsMonk(Class))
            {
                textBox11.Text = Monk[0];
                TxtBotBoots.Text = Monk[1];
                textBox10.Text = Monk[2];
                textBox9.Text = Monk[3];
                TxtBotArmors.Text = Monk[4];
                TxtBotRightWeapon.Text = Monk[5];
                TxtBotLeftWeapon.Text = Monk[6];
                TxtBotStarTower.Text = Monk[7];
                TxtBotHeavenFan.Text = Monk[8];
                TxtBotRidingCrop.Text = Monk[9];
            }
            string[] Taoist;
            Taoist = new string[10] { "121189", "160199", "152189", "114089", "134089", "421199", "0", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsTaoist(Class))
            {
                textBox11.Text = Taoist[0];
                TxtBotBoots.Text = Taoist[1];
                textBox10.Text = Taoist[2];
                textBox9.Text = Taoist[3];
                TxtBotArmors.Text = Taoist[4];
                TxtBotRightWeapon.Text = Taoist[5];
                TxtBotLeftWeapon.Text = Taoist[6];
                TxtBotStarTower.Text = Taoist[7];
                TxtBotHeavenFan.Text = Taoist[8];
                TxtBotRidingCrop.Text = Taoist[9];
            }
        }

        private void ReqLev120_CheckedChanged(object sender, EventArgs e)
        {
            byte Class;
            Class = 0;
            string text;
            text = comboBox15.Text;
            if (text != null)
            {
                switch (text.Length)
                {
                    case 6:
                        switch (text[0])
                        {
                            case 'T':
                                if (text == "Trojan")
                                    Class = 15;
                                break;
                            case 'A':
                                if (text == "Archer")
                                    Class = 45;
                                break;
                        }
                        break;
                    case 5:
                        switch (text[0])
                        {
                            case 'N':
                                if (text == "Ninja")
                                    Class = 55;
                                break;
                            case 'W':
                                if (text == "Water")
                                    Class = 135;
                                break;
                        }
                        break;
                    case 4:
                        switch (text[0])
                        {
                            case 'M':
                                if (text == "Monk")
                                    Class = 65;
                                break;
                            case 'F':
                                if (text == "Fire")
                                    Class = 145;
                                break;
                        }
                        break;
                    case 7:
                        if (text == "Warrior")
                            Class = 25;
                        break;
                }
            }
            var RndAccessory = Database.ItemType.RareGarments2;
            var Position = Program.GetRandom.Next(0, RndAccessory.Count);
            var ReceiveItem = RndAccessory.ToArray()[Position];
            string[] Troj;
            Troj = new string[10] { "120229", "160229", "150229", "118109", "130109", "480239", "420239", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsTrojan(Class))
            {
                textBox11.Text = Troj[0];
                TxtBotBoots.Text = Troj[1];
                textBox10.Text = Troj[2];
                textBox9.Text = Troj[3];
                TxtBotArmors.Text = Troj[4];
                TxtBotRightWeapon.Text = Troj[5];
                TxtBotLeftWeapon.Text = Troj[6];
                TxtBotStarTower.Text = Troj[7];
                TxtBotHeavenFan.Text = Troj[8];
                TxtBotRidingCrop.Text = Troj[9];
            }
            string[] Warr;
            Warr = new string[10] { "120229", "160229", "150229", "111109", "131109", "560239", "0", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsWarrior(Class))
            {
                textBox11.Text = Warr[0];
                TxtBotBoots.Text = Warr[1];
                textBox10.Text = Warr[2];
                textBox9.Text = Warr[3];
                TxtBotArmors.Text = Warr[4];
                TxtBotRightWeapon.Text = Warr[5];
                TxtBotLeftWeapon.Text = Warr[6];
                TxtBotStarTower.Text = Warr[7];
                TxtBotHeavenFan.Text = Warr[8];
                TxtBotRidingCrop.Text = Warr[9];
            }
            string[] Arch;
            Arch = new string[10] { "120229", "160229", "150229", "113109", "133109", "500229", "1050002", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsArcher(Class))
            {
                textBox11.Text = Arch[0];
                TxtBotBoots.Text = Arch[1];
                textBox10.Text = Arch[2];
                textBox9.Text = Arch[3];
                TxtBotArmors.Text = Arch[4];
                TxtBotRightWeapon.Text = Arch[5];
                TxtBotLeftWeapon.Text = Arch[6];
                TxtBotStarTower.Text = Arch[7];
                TxtBotHeavenFan.Text = Arch[8];
                TxtBotRidingCrop.Text = Arch[9];
            }
            string[] Ninja;
            Ninja = new string[10] { "120229", "160229", "150229", "112109", "135109", "601239", "601239", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsNinja(Class))
            {
                textBox11.Text = Ninja[0];
                TxtBotBoots.Text = Ninja[1];
                textBox10.Text = Ninja[2];
                textBox9.Text = Ninja[3];
                TxtBotArmors.Text = Ninja[4];
                TxtBotRightWeapon.Text = Ninja[5];
                TxtBotLeftWeapon.Text = Ninja[6];
                TxtBotStarTower.Text = Ninja[7];
                TxtBotHeavenFan.Text = Ninja[8];
                TxtBotRidingCrop.Text = Ninja[9];
            }
            string[] Monk;
            Monk = new string[10] { "120229", "160229", "150229", "143109", "136109", "610239", "610239", "201009", "202009", "" + ReceiveItem + "" };
            if (AtributesStatus.IsMonk(Class))
            {
                textBox11.Text = Monk[0];
                TxtBotBoots.Text = Monk[1];
                textBox10.Text = Monk[2];
                textBox9.Text = Monk[3];
                TxtBotArmors.Text = Monk[4];
                TxtBotRightWeapon.Text = Monk[5];
                TxtBotLeftWeapon.Text = Monk[6];
                TxtBotStarTower.Text = Monk[7];
                TxtBotHeavenFan.Text = Monk[8];
                TxtBotRidingCrop.Text = Monk[9];
            }
            string[] Taoist;
            Taoist = new string[10] { "120229", "160229", "150229", "114109", "134109", "421239", "0", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsTaoist(Class))
            {
                textBox11.Text = Taoist[0];
                TxtBotBoots.Text = Taoist[1];
                textBox10.Text = Taoist[2];
                textBox9.Text = Taoist[3];
                TxtBotArmors.Text = Taoist[4];
                TxtBotRightWeapon.Text = Taoist[5];
                TxtBotLeftWeapon.Text = Taoist[6];
                TxtBotStarTower.Text = Taoist[7];
                TxtBotHeavenFan.Text = Taoist[8];
                TxtBotRidingCrop.Text = Taoist[9];
            }
        }

        private void ReqLev140_CheckedChanged(object sender, EventArgs e)
        {
            byte Class;
            Class = 0;
            string text;
            text = comboBox15.Text;
            if (text != null)
            {
                switch (text.Length)
                {
                    case 6:
                        switch (text[0])
                        {
                            case 'T':
                                if (text == "Trojan")
                                    Class = 15;
                                break;
                            case 'A':
                                if (text == "Archer")
                                    Class = 45;
                                break;
                        }
                        break;
                    case 5:
                        switch (text[0])
                        {
                            case 'N':
                                if (text == "Ninja")
                                    Class = 55;
                                break;
                            case 'W':
                                if (text == "Water")
                                    Class = 135;
                                break;
                        }
                        break;
                    case 4:
                        switch (text[0])
                        {
                            case 'M':
                                if (text == "Monk")
                                    Class = 65;
                                break;
                            case 'F':
                                if (text == "Fire")
                                    Class = 145;
                                break;
                        }
                        break;
                    case 7:
                        if (text == "Warrior")
                            Class = 25;
                        break;
                }
            }
            var RndAccessory = Database.ItemType.RareGarments2;
            var Position = Program.GetRandom.Next(0, RndAccessory.Count);
            var ReceiveItem = RndAccessory.ToArray()[Position];
            string[] Troj;
            Troj = new string[10] { "120229", "160229", "150229", "118109", "130109", "480339", "420339", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsTrojan(Class))
            {
                textBox11.Text = Troj[0];
                TxtBotBoots.Text = Troj[1];
                textBox10.Text = Troj[2];
                textBox9.Text = Troj[3];
                TxtBotArmors.Text = Troj[4];
                TxtBotRightWeapon.Text = Troj[5];
                TxtBotLeftWeapon.Text = Troj[6];
                TxtBotStarTower.Text = Troj[7];
                TxtBotHeavenFan.Text = Troj[8];
                TxtBotRidingCrop.Text = Troj[9];
            }
            string[] Warr;
            Warr = new string[10] { "120229", "160229", "150229", "111109", "131109", "560339", "0", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsWarrior(Class))
            {
                textBox11.Text = Warr[0];
                TxtBotBoots.Text = Warr[1];
                textBox10.Text = Warr[2];
                textBox9.Text = Warr[3];
                TxtBotArmors.Text = Warr[4];
                TxtBotRightWeapon.Text = Warr[5];
                TxtBotLeftWeapon.Text = Warr[6];
                TxtBotStarTower.Text = Warr[7];
                TxtBotHeavenFan.Text = Warr[8];
                TxtBotRidingCrop.Text = Warr[9];
            }
            string[] Arch;
            Arch = new string[10] { "120229", "160229", "150229", "113109", "133109", "500329", "1050002", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsArcher(Class))
            {
                textBox11.Text = Arch[0];
                TxtBotBoots.Text = Arch[1];
                textBox10.Text = Arch[2];
                textBox9.Text = Arch[3];
                TxtBotArmors.Text = Arch[4];
                TxtBotRightWeapon.Text = Arch[5];
                TxtBotLeftWeapon.Text = Arch[6];
                TxtBotStarTower.Text = Arch[7];
                TxtBotHeavenFan.Text = Arch[8];
                TxtBotRidingCrop.Text = Arch[9];
            }
            string[] Ninja;
            Ninja = new string[10] { "120229", "160229", "150229", "112109", "135109", "601239", "601239", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsNinja(Class))
            {
                textBox11.Text = Ninja[0];
                TxtBotBoots.Text = Ninja[1];
                textBox10.Text = Ninja[2];
                textBox9.Text = Ninja[3];
                TxtBotArmors.Text = Ninja[4];
                TxtBotRightWeapon.Text = Ninja[5];
                TxtBotLeftWeapon.Text = Ninja[6];
                TxtBotStarTower.Text = Ninja[7];
                TxtBotHeavenFan.Text = Ninja[8];
                TxtBotRidingCrop.Text = Ninja[9];
            }
            string[] Monk;
            Monk = new string[10] { "120229", "160229", "150229", "143109", "136109", "610239", "610239", "201009", "202009", "" + ReceiveItem + "" };
            if (AtributesStatus.IsMonk(Class))
            {
                textBox11.Text = Monk[0];
                TxtBotBoots.Text = Monk[1];
                textBox10.Text = Monk[2];
                textBox9.Text = Monk[3];
                TxtBotArmors.Text = Monk[4];
                TxtBotRightWeapon.Text = Monk[5];
                TxtBotLeftWeapon.Text = Monk[6];
                TxtBotStarTower.Text = Monk[7];
                TxtBotHeavenFan.Text = Monk[8];
                TxtBotRidingCrop.Text = Monk[9];
            }
            string[] Taoist;
            Taoist = new string[10] { "120229", "160229", "150229", "114109", "134109", "421339", "0", "0", "0", "" + ReceiveItem + "" };
            if (AtributesStatus.IsTaoist(Class))
            {
                textBox11.Text = Taoist[0];
                TxtBotBoots.Text = Taoist[1];
                textBox10.Text = Taoist[2];
                textBox9.Text = Taoist[3];
                TxtBotArmors.Text = Taoist[4];
                TxtBotRightWeapon.Text = Taoist[5];
                TxtBotLeftWeapon.Text = Taoist[6];
                TxtBotStarTower.Text = Taoist[7];
                TxtBotHeavenFan.Text = Taoist[8];
                TxtBotRidingCrop.Text = Taoist[9];
            }
        }

        private void StartHeroOfGame(object sender, EventArgs e)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgSchedules.CurrentTournament = MsgSchedules.Tournaments[TournamentType.HeroOfGame];
                MsgSchedules.CurrentTournament.Open();
                GetDefault(MainFlag.Tournamentlist);
            }
        }

        private void UpdateUsers(object sender, EventArgs e)
        {
            //if (textBox14.Text == ServerKernel.Allow_User && textBox13.Text == ServerKernel.Allow_Password)
            //{
            //    ServerKernel.Allow_User = textBox16.Text;
            //    ServerKernel.Allow_Password = textBox15.Text;
            //    IniFile _shell;
            //    _shell = new IniFile(Directory.GetCurrentDirectory() + "\\StartupUser.ini", true);
            //    ServerKernel.EncryptFiles.Decrypt(Directory.GetCurrentDirectory() + "\\StartupUser.ini");
            //    _shell.Write("config", "User", ServerKernel.Allow_User);
            //    _shell.Write("config", "Password", ServerKernel.Allow_Password);
            //    ServerKernel.EncryptFiles.Encrypt(Directory.GetCurrentDirectory() + "\\StartupUser.ini");
            //    MessageBox.Show("Done");
            //}
            //else
            //    MessageBox.Show("Invaild data");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.TxtLogger = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button66 = new System.Windows.Forms.Button();
            this.button64 = new System.Windows.Forms.Button();
            this.button35 = new System.Windows.Forms.Button();
            this.button29 = new System.Windows.Forms.Button();
            this.button28 = new System.Windows.Forms.Button();
            this.button27 = new System.Windows.Forms.Button();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.NumPlayerCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtConnectIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnStart = new System.Windows.Forms.Button();
            this.LbxCharacters = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.comboBox10 = new System.Windows.Forms.ComboBox();
            this.button49 = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.comboBox9 = new System.Windows.Forms.ComboBox();
            this.button46 = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.comboBox8 = new System.Windows.Forms.ComboBox();
            this.button45 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.LblClanName = new System.Windows.Forms.Label();
            this.LblNameChange = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.LbxOnPCCount = new System.Windows.Forms.ListBox();
            this.CheckBoxOfflineVending = new System.Windows.Forms.CheckBox();
            this.CheckBoxOfflineHunting = new System.Windows.Forms.CheckBox();
            this.CheckBoxOfflineTraining = new System.Windows.Forms.CheckBox();
            this.LblGuildName = new System.Windows.Forms.Label();
            this.ClanRank = new System.Windows.Forms.Label();
            this.LblClanRank = new System.Windows.Forms.Label();
            this.ClanName = new System.Windows.Forms.Label();
            this.GuildRank = new System.Windows.Forms.Label();
            this.LblGuildRank = new System.Windows.Forms.Label();
            this.GuildName = new System.Windows.Forms.Label();
            this.LblClass = new System.Windows.Forms.Label();
            this.ExtraAtributes = new System.Windows.Forms.Label();
            this.LblExtraAtributes = new System.Windows.Forms.Label();
            this.NameChange = new System.Windows.Forms.Label();
            this.LblFirstClass = new System.Windows.Forms.Label();
            this.WHMoney = new System.Windows.Forms.Label();
            this.LblWHMoney = new System.Windows.Forms.Label();
            this.Class = new System.Windows.Forms.Label();
            this.LblNobilityRank = new System.Windows.Forms.Label();
            this.SecondClass = new System.Windows.Forms.Label();
            this.LblSecondClass = new System.Windows.Forms.Label();
            this.FirstClass = new System.Windows.Forms.Label();
            this.LblChampionPoints = new System.Windows.Forms.Label();
            this.NobilityRank = new System.Windows.Forms.Label();
            this.LblVIPTime = new System.Windows.Forms.Label();
            this.OnlinePoints = new System.Windows.Forms.Label();
            this.LblOnlinePoints = new System.Windows.Forms.Label();
            this.ChampionPoints = new System.Windows.Forms.Label();
            this.LblMoney = new System.Windows.Forms.Label();
            this.SpecialTitles = new System.Windows.Forms.Label();
            this.LblSpecialTitles = new System.Windows.Forms.Label();
            this.NewbieProtection = new System.Windows.Forms.Label();
            this.LblNewbieProtection = new System.Windows.Forms.Label();
            this.SecurityPass = new System.Windows.Forms.Label();
            this.LblSecurityPass = new System.Windows.Forms.Label();
            this.VIPTime = new System.Windows.Forms.Label();
            this.LblConquerPoints = new System.Windows.Forms.Label();
            this.VIPLevel = new System.Windows.Forms.Label();
            this.LblVIPLevel = new System.Windows.Forms.Label();
            this.Money = new System.Windows.Forms.Label();
            this.UserClamCP = new System.Windows.Forms.Label();
            this.LblUserClamCP = new System.Windows.Forms.Label();
            this.ConquerPoints = new System.Windows.Forms.Label();
            this.CheckBoxAutoHunt = new System.Windows.Forms.CheckBox();
            this.BtnDisconnectUser = new System.Windows.Forms.Button();
            this.BtnRefreshPC = new System.Windows.Forms.Button();
            this.LbxOnScreen = new System.Windows.Forms.ListBox();
            this.LblUserManaPoints = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.LblUserHealthPoints = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.LblUserPkPoints = new System.Windows.Forms.Label();
            this.LblUserAdditionalPoints = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.LblUserSpirit = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.LblUserVitality = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.LblUserAgility = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.LblUserStrength = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.LblUserLocation = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.LblUserExperience = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.LblUserLevel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.LblUserMateName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.LblUserName = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.LblUserId = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button16 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.button8 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.numericUpDown21 = new System.Windows.Forms.NumericUpDown();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.button6 = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button67 = new System.Windows.Forms.Button();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.label157 = new System.Windows.Forms.Label();
            this.label155 = new System.Windows.Forms.Label();
            this.button26 = new System.Windows.Forms.Button();
            this.button23 = new System.Windows.Forms.Button();
            this.button54 = new System.Windows.Forms.Button();
            this.button48 = new System.Windows.Forms.Button();
            this.button36 = new System.Windows.Forms.Button();
            this.button44 = new System.Windows.Forms.Button();
            this.button43 = new System.Windows.Forms.Button();
            this.button42 = new System.Windows.Forms.Button();
            this.button41 = new System.Windows.Forms.Button();
            this.button40 = new System.Windows.Forms.Button();
            this.button39 = new System.Windows.Forms.Button();
            this.button309 = new System.Windows.Forms.Button();
            this.button38 = new System.Windows.Forms.Button();
            this.button37 = new System.Windows.Forms.Button();
            this.button34 = new System.Windows.Forms.Button();
            this.button33 = new System.Windows.Forms.Button();
            this.button31 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.comboBox7 = new System.Windows.Forms.ComboBox();
            this.button32 = new System.Windows.Forms.Button();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.RecList = new System.Windows.Forms.ListBox();
            this.button4 = new System.Windows.Forms.Button();
            this.Send = new System.Windows.Forms.Button();
            this.SendText = new System.Windows.Forms.TextBox();
            this.ClientList = new System.Windows.Forms.ListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label160 = new System.Windows.Forms.Label();
            this.textBox78 = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.button24 = new System.Windows.Forms.Button();
            this.comboBox6 = new System.Windows.Forms.ComboBox();
            this.button18 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.button25 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button19 = new System.Windows.Forms.Button();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.button20 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.button22 = new System.Windows.Forms.Button();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.button56 = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.button50 = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.GemsChecked = new System.Windows.Forms.CheckBox();
            this.RebornBox = new System.Windows.Forms.ComboBox();
            this.comboBox15 = new System.Windows.Forms.ComboBox();
            this.ReqLev120 = new System.Windows.Forms.RadioButton();
            this.ReqLev140 = new System.Windows.Forms.RadioButton();
            this.ReqLev100 = new System.Windows.Forms.RadioButton();
            this.ReqLev70 = new System.Windows.Forms.RadioButton();
            this.comboBox13 = new System.Windows.Forms.ComboBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.comboBox12 = new System.Windows.Forms.ComboBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.comboBox11 = new System.Windows.Forms.ComboBox();
            this.button47 = new System.Windows.Forms.Button();
            this.label45 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.textBox28 = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.textBox26 = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.textBox24 = new System.Windows.Forms.TextBox();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.textBox22 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.textBox20 = new System.Windows.Forms.TextBox();
            this.textBox21 = new System.Windows.Forms.TextBox();
            this.label89 = new System.Windows.Forms.Label();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.TxtBotRidingCrop = new System.Windows.Forms.TextBox();
            this.TxtBotStarTower = new System.Windows.Forms.TextBox();
            this.TxtBotHeavenFan = new System.Windows.Forms.TextBox();
            this.TxtBotBoots = new System.Windows.Forms.TextBox();
            this.TxtBotRightWeapon = new System.Windows.Forms.TextBox();
            this.TxtBotLeftWeapon = new System.Windows.Forms.TextBox();
            this.TxtBotArmors = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.numericUpDown17 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown18 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown19 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown20 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown16 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown15 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown14 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown13 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown12 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown11 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown10 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown9 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown8 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown7 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label46 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.label87 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.label84 = new System.Windows.Forms.Label();
            this.label83 = new System.Windows.Forms.Label();
            this.label82 = new System.Windows.Forms.Label();
            this.label81 = new System.Windows.Forms.Label();
            this.label80 = new System.Windows.Forms.Label();
            this.label79 = new System.Windows.Forms.Label();
            this.label76 = new System.Windows.Forms.Label();
            this.label75 = new System.Windows.Forms.Label();
            this.label74 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.label72 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.label68 = new System.Windows.Forms.Label();
            this.label69 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label63 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.label149 = new System.Windows.Forms.Label();
            this.textBox108 = new System.Windows.Forms.TextBox();
            this.label151 = new System.Windows.Forms.Label();
            this.textBox109 = new System.Windows.Forms.TextBox();
            this.label103 = new System.Windows.Forms.Label();
            this.textBox60 = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.textBox93 = new System.Windows.Forms.TextBox();
            this.textBox41 = new System.Windows.Forms.TextBox();
            this.label138 = new System.Windows.Forms.Label();
            this.textBox74 = new System.Windows.Forms.TextBox();
            this.textBox94 = new System.Windows.Forms.TextBox();
            this.textBox75 = new System.Windows.Forms.TextBox();
            this.label119 = new System.Windows.Forms.Label();
            this.textBox95 = new System.Windows.Forms.TextBox();
            this.textBox76 = new System.Windows.Forms.TextBox();
            this.label112 = new System.Windows.Forms.Label();
            this.textBox69 = new System.Windows.Forms.TextBox();
            this.label113 = new System.Windows.Forms.Label();
            this.textBox70 = new System.Windows.Forms.TextBox();
            this.label114 = new System.Windows.Forms.Label();
            this.textBox71 = new System.Windows.Forms.TextBox();
            this.label115 = new System.Windows.Forms.Label();
            this.textBox72 = new System.Windows.Forms.TextBox();
            this.label108 = new System.Windows.Forms.Label();
            this.textBox65 = new System.Windows.Forms.TextBox();
            this.label109 = new System.Windows.Forms.Label();
            this.textBox66 = new System.Windows.Forms.TextBox();
            this.label110 = new System.Windows.Forms.Label();
            this.textBox67 = new System.Windows.Forms.TextBox();
            this.label111 = new System.Windows.Forms.Label();
            this.textBox85 = new System.Windows.Forms.TextBox();
            this.textBox68 = new System.Windows.Forms.TextBox();
            this.label120 = new System.Windows.Forms.Label();
            this.label106 = new System.Windows.Forms.Label();
            this.textBox77 = new System.Windows.Forms.TextBox();
            this.textBox63 = new System.Windows.Forms.TextBox();
            this.label107 = new System.Windows.Forms.Label();
            this.textBox64 = new System.Windows.Forms.TextBox();
            this.label104 = new System.Windows.Forms.Label();
            this.textBox61 = new System.Windows.Forms.TextBox();
            this.label105 = new System.Windows.Forms.Label();
            this.textBox62 = new System.Windows.Forms.TextBox();
            this.label101 = new System.Windows.Forms.Label();
            this.textBox58 = new System.Windows.Forms.TextBox();
            this.label102 = new System.Windows.Forms.Label();
            this.textBox59 = new System.Windows.Forms.TextBox();
            this.label100 = new System.Windows.Forms.Label();
            this.textBox57 = new System.Windows.Forms.TextBox();
            this.label98 = new System.Windows.Forms.Label();
            this.textBox55 = new System.Windows.Forms.TextBox();
            this.label99 = new System.Windows.Forms.Label();
            this.textBox56 = new System.Windows.Forms.TextBox();
            this.textBox42 = new System.Windows.Forms.TextBox();
            this.label59 = new System.Windows.Forms.Label();
            this.textBox43 = new System.Windows.Forms.TextBox();
            this.label60 = new System.Windows.Forms.Label();
            this.label73 = new System.Windows.Forms.Label();
            this.label78 = new System.Windows.Forms.Label();
            this.textBox45 = new System.Windows.Forms.TextBox();
            this.textBox46 = new System.Windows.Forms.TextBox();
            this.label90 = new System.Windows.Forms.Label();
            this.textBox47 = new System.Windows.Forms.TextBox();
            this.label91 = new System.Windows.Forms.Label();
            this.textBox48 = new System.Windows.Forms.TextBox();
            this.label92 = new System.Windows.Forms.Label();
            this.textBox49 = new System.Windows.Forms.TextBox();
            this.label93 = new System.Windows.Forms.Label();
            this.textBox50 = new System.Windows.Forms.TextBox();
            this.label94 = new System.Windows.Forms.Label();
            this.textBox51 = new System.Windows.Forms.TextBox();
            this.label95 = new System.Windows.Forms.Label();
            this.textBox52 = new System.Windows.Forms.TextBox();
            this.label96 = new System.Windows.Forms.Label();
            this.textBox53 = new System.Windows.Forms.TextBox();
            this.label97 = new System.Windows.Forms.Label();
            this.textBox54 = new System.Windows.Forms.TextBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.checkBox13 = new System.Windows.Forms.CheckBox();
            this.label126 = new System.Windows.Forms.Label();
            this.label117 = new System.Windows.Forms.Label();
            this.checkBox12 = new System.Windows.Forms.CheckBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.textBox110 = new System.Windows.Forms.TextBox();
            this.label152 = new System.Windows.Forms.Label();
            this.textBox96 = new System.Windows.Forms.TextBox();
            this.label139 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.textBox92 = new System.Windows.Forms.TextBox();
            this.label135 = new System.Windows.Forms.Label();
            this.textBox91 = new System.Windows.Forms.TextBox();
            this.label134 = new System.Windows.Forms.Label();
            this.textBox90 = new System.Windows.Forms.TextBox();
            this.label133 = new System.Windows.Forms.Label();
            this.textBox89 = new System.Windows.Forms.TextBox();
            this.label132 = new System.Windows.Forms.Label();
            this.textBox87 = new System.Windows.Forms.TextBox();
            this.label131 = new System.Windows.Forms.Label();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.textBox86 = new System.Windows.Forms.TextBox();
            this.label129 = new System.Windows.Forms.Label();
            this.label116 = new System.Windows.Forms.Label();
            this.textBox73 = new System.Windows.Forms.TextBox();
            this.textBox40 = new System.Windows.Forms.TextBox();
            this.label57 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.textBox37 = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.textBox38 = new System.Windows.Forms.TextBox();
            this.textBox39 = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.textBox36 = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.textBox35 = new System.Windows.Forms.TextBox();
            this.label51 = new System.Windows.Forms.Label();
            this.textBox34 = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.label505 = new System.Windows.Forms.Label();
            this.textBox33 = new System.Windows.Forms.TextBox();
            this.textBox330 = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.textBox31 = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.textBox32 = new System.Windows.Forms.TextBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox14 = new System.Windows.Forms.CheckBox();
            this.label47 = new System.Windows.Forms.Label();
            this.textBox27 = new System.Windows.Forms.TextBox();
            this.button52 = new System.Windows.Forms.Button();
            this.label40 = new System.Windows.Forms.Label();
            this.textBox25 = new System.Windows.Forms.TextBox();
            this.label88 = new System.Windows.Forms.Label();
            this.textBox44 = new System.Windows.Forms.TextBox();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.button57 = new System.Windows.Forms.Button();
            this.label121 = new System.Windows.Forms.Label();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.label153 = new System.Windows.Forms.Label();
            this.textBox88 = new System.Windows.Forms.TextBox();
            this.label130 = new System.Windows.Forms.Label();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.button69 = new System.Windows.Forms.Button();
            this.label170 = new System.Windows.Forms.Label();
            this.button68 = new System.Windows.Forms.Button();
            this.comboBox14 = new System.Windows.Forms.ComboBox();
            this.label171 = new System.Windows.Forms.Label();
            this.button65 = new System.Windows.Forms.Button();
            this.label156 = new System.Windows.Forms.Label();
            this.textBox84 = new System.Windows.Forms.TextBox();
            this.textBox83 = new System.Windows.Forms.TextBox();
            this.textBox82 = new System.Windows.Forms.TextBox();
            this.textBox81 = new System.Windows.Forms.TextBox();
            this.textBox80 = new System.Windows.Forms.TextBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.button55 = new System.Windows.Forms.Button();
            this.label123 = new System.Windows.Forms.Label();
            this.label124 = new System.Windows.Forms.Label();
            this.label125 = new System.Windows.Forms.Label();
            this.label127 = new System.Windows.Forms.Label();
            this.label128 = new System.Windows.Forms.Label();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.label154 = new System.Windows.Forms.Label();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.button51 = new System.Windows.Forms.Button();
            this.button62 = new System.Windows.Forms.Button();
            this.button63 = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.textBox107 = new System.Windows.Forms.TextBox();
            this.textBox105 = new System.Windows.Forms.TextBox();
            this.textBox106 = new System.Windows.Forms.TextBox();
            this.label150 = new System.Windows.Forms.Label();
            this.textBox104 = new System.Windows.Forms.TextBox();
            this.label148 = new System.Windows.Forms.Label();
            this.textBox103 = new System.Windows.Forms.TextBox();
            this.label147 = new System.Windows.Forms.Label();
            this.textBox101 = new System.Windows.Forms.TextBox();
            this.label145 = new System.Windows.Forms.Label();
            this.textBox102 = new System.Windows.Forms.TextBox();
            this.label146 = new System.Windows.Forms.Label();
            this.textBox99 = new System.Windows.Forms.TextBox();
            this.label143 = new System.Windows.Forms.Label();
            this.textBox100 = new System.Windows.Forms.TextBox();
            this.label144 = new System.Windows.Forms.Label();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.textBox98 = new System.Windows.Forms.TextBox();
            this.label142 = new System.Windows.Forms.Label();
            this.textBox97 = new System.Windows.Forms.TextBox();
            this.label141 = new System.Windows.Forms.Label();
            this.button59 = new System.Windows.Forms.Button();
            this.button58 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.button61 = new System.Windows.Forms.Button();
            this.tabPage11 = new System.Windows.Forms.TabPage();
            this.label159 = new System.Windows.Forms.Label();
            this.label158 = new System.Windows.Forms.Label();
            this.button30 = new System.Windows.Forms.Button();
            this.dataGridView5 = new System.Windows.Forms.DataGridView();
            this.label140 = new System.Windows.Forms.Label();
            this.button60 = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.button53 = new System.Windows.Forms.Button();
            this.label36 = new System.Windows.Forms.Label();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.checkBox14 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumPlayerCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown21)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.tabPage5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabPage7.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.tabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.tabPage10.SuspendLayout();
            this.groupBox17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage8.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TxtLogger
            // 
            this.TxtLogger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLogger.Location = new System.Drawing.Point(4, 12);
            this.TxtLogger.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtLogger.MaxLength = 1000000;
            this.TxtLogger.Multiline = true;
            this.TxtLogger.Name = "TxtLogger";
            this.TxtLogger.ReadOnly = true;
            this.TxtLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtLogger.Size = new System.Drawing.Size(1068, 492);
            this.TxtLogger.TabIndex = 65535;
            this.TxtLogger.Text = "Console";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button66);
            this.groupBox1.Controls.Add(this.button64);
            this.groupBox1.Controls.Add(this.button35);
            this.groupBox1.Controls.Add(this.button29);
            this.groupBox1.Controls.Add(this.button28);
            this.groupBox1.Controls.Add(this.button27);
            this.groupBox1.Controls.Add(this.textBox6);
            this.groupBox1.Controls.Add(this.NumPlayerCount);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.TxtConnectIp);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.BtnStart);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(1813, 53);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // button66
            // 
            this.button66.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button66.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.button66.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button66.Location = new System.Drawing.Point(979, 16);
            this.button66.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button66.Name = "button66";
            this.button66.Size = new System.Drawing.Size(148, 31);
            this.button66.TabIndex = 65539;
            this.button66.Text = "LoaderPanel";
            this.button66.UseVisualStyleBackColor = true;
            this.button66.Click += new System.EventHandler(this.LOADERPanel);
            // 
            // button64
            // 
            this.button64.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button64.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.button64.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button64.Location = new System.Drawing.Point(1161, 16);
            this.button64.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button64.Name = "button64";
            this.button64.Size = new System.Drawing.Size(148, 31);
            this.button64.TabIndex = 65539;
            this.button64.Text = "Attack Panel";
            this.button64.UseVisualStyleBackColor = true;
            this.button64.Click += new System.EventHandler(this.AttackPanel);
            // 
            // button35
            // 
            this.button35.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button35.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.button35.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button35.Location = new System.Drawing.Point(1333, 15);
            this.button35.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button35.Name = "button35";
            this.button35.Size = new System.Drawing.Size(148, 31);
            this.button35.TabIndex = 65539;
            this.button35.Text = "Update Website";
            this.button35.UseVisualStyleBackColor = true;
            this.button35.Click += new System.EventHandler(this.UpdateWebsite);
            // 
            // button29
            // 
            this.button29.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button29.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.button29.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button29.Location = new System.Drawing.Point(1653, 15);
            this.button29.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button29.Name = "button29";
            this.button29.Size = new System.Drawing.Size(75, 31);
            this.button29.TabIndex = 65538;
            this.button29.Text = "Save";
            this.button29.UseVisualStyleBackColor = true;
            this.button29.Click += new System.EventHandler(this.SaveServer);
            // 
            // button28
            // 
            this.button28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button28.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.button28.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button28.Location = new System.Drawing.Point(1488, 15);
            this.button28.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(75, 31);
            this.button28.TabIndex = 11;
            this.button28.Text = "Restart";
            this.button28.UseVisualStyleBackColor = true;
            this.button28.Click += new System.EventHandler(this.MaintenanceServer);
            // 
            // button27
            // 
            this.button27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button27.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.button27.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button27.Location = new System.Drawing.Point(1570, 15);
            this.button27.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button27.Name = "button27";
            this.button27.Size = new System.Drawing.Size(75, 31);
            this.button27.TabIndex = 10;
            this.button27.Text = "Exit";
            this.button27.UseVisualStyleBackColor = true;
            this.button27.Click += new System.EventHandler(this.CloseServer);
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(469, 22);
            this.textBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox6.MaxLength = 32;
            this.textBox6.Name = "textBox6";
            this.textBox6.PasswordChar = '*';
            this.textBox6.Size = new System.Drawing.Size(51, 23);
            this.textBox6.TabIndex = 8;
            // 
            // NumPlayerCount
            // 
            this.NumPlayerCount.Location = new System.Drawing.Point(559, 21);
            this.NumPlayerCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NumPlayerCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NumPlayerCount.Name = "NumPlayerCount";
            this.NumPlayerCount.Size = new System.Drawing.Size(56, 23);
            this.NumPlayerCount.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(406, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Password:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(330, 21);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox3.MaxLength = 32;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(68, 23);
            this.textBox3.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(259, 24);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "User Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(525, 24);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Key";
            // 
            // TxtConnectIp
            // 
            this.TxtConnectIp.Location = new System.Drawing.Point(121, 21);
            this.TxtConnectIp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtConnectIp.MaxLength = 32;
            this.TxtConnectIp.Name = "TxtConnectIp";
            this.TxtConnectIp.Size = new System.Drawing.Size(125, 23);
            this.TxtConnectIp.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Connect address:";
            // 
            // BtnStart
            // 
            this.BtnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.BtnStart.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.BtnStart.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BtnStart.Location = new System.Drawing.Point(1733, 15);
            this.BtnStart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(75, 31);
            this.BtnStart.TabIndex = 5;
            this.BtnStart.Text = "Login";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // LbxCharacters
            // 
            this.LbxCharacters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LbxCharacters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LbxCharacters.FormattingEnabled = true;
            this.LbxCharacters.ItemHeight = 15;
            this.LbxCharacters.Location = new System.Drawing.Point(12, 75);
            this.LbxCharacters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LbxCharacters.Name = "LbxCharacters";
            this.LbxCharacters.Size = new System.Drawing.Size(219, 677);
            this.LbxCharacters.TabIndex = 65536;
            this.LbxCharacters.Click += new System.EventHandler(this.LbxCharacters_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tabControl1);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.groupBox2.Location = new System.Drawing.Point(243, 75);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(1582, 750);
            this.groupBox2.TabIndex = 65537;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server information";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage9);
            this.tabControl1.Controls.Add(this.tabPage10);
            this.tabControl1.Controls.Add(this.tabPage11);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(4, 16);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1574, 731);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.LightCyan;
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage1.Controls.Add(this.trackBar1);
            this.tabPage1.Controls.Add(this.pictureBox5);
            this.tabPage1.Controls.Add(this.checkBox7);
            this.tabPage1.Controls.Add(this.checkBox3);
            this.tabPage1.Controls.Add(this.groupBox10);
            this.tabPage1.Controls.Add(this.groupBox9);
            this.tabPage1.Controls.Add(this.groupBox8);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.label34);
            this.tabPage1.Controls.Add(this.label35);
            this.tabPage1.Controls.Add(this.checkBox2);
            this.tabPage1.Controls.Add(this.label28);
            this.tabPage1.Controls.Add(this.label29);
            this.tabPage1.Controls.Add(this.label30);
            this.tabPage1.Controls.Add(this.label31);
            this.tabPage1.Controls.Add(this.checkBox1);
            this.tabPage1.Controls.Add(this.LblClanName);
            this.tabPage1.Controls.Add(this.LblNameChange);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label21);
            this.tabPage1.Controls.Add(this.LbxOnPCCount);
            this.tabPage1.Controls.Add(this.CheckBoxOfflineVending);
            this.tabPage1.Controls.Add(this.CheckBoxOfflineHunting);
            this.tabPage1.Controls.Add(this.CheckBoxOfflineTraining);
            this.tabPage1.Controls.Add(this.LblGuildName);
            this.tabPage1.Controls.Add(this.ClanRank);
            this.tabPage1.Controls.Add(this.LblClanRank);
            this.tabPage1.Controls.Add(this.ClanName);
            this.tabPage1.Controls.Add(this.GuildRank);
            this.tabPage1.Controls.Add(this.LblGuildRank);
            this.tabPage1.Controls.Add(this.GuildName);
            this.tabPage1.Controls.Add(this.LblClass);
            this.tabPage1.Controls.Add(this.ExtraAtributes);
            this.tabPage1.Controls.Add(this.LblExtraAtributes);
            this.tabPage1.Controls.Add(this.NameChange);
            this.tabPage1.Controls.Add(this.LblFirstClass);
            this.tabPage1.Controls.Add(this.WHMoney);
            this.tabPage1.Controls.Add(this.LblWHMoney);
            this.tabPage1.Controls.Add(this.Class);
            this.tabPage1.Controls.Add(this.LblNobilityRank);
            this.tabPage1.Controls.Add(this.SecondClass);
            this.tabPage1.Controls.Add(this.LblSecondClass);
            this.tabPage1.Controls.Add(this.FirstClass);
            this.tabPage1.Controls.Add(this.LblChampionPoints);
            this.tabPage1.Controls.Add(this.NobilityRank);
            this.tabPage1.Controls.Add(this.LblVIPTime);
            this.tabPage1.Controls.Add(this.OnlinePoints);
            this.tabPage1.Controls.Add(this.LblOnlinePoints);
            this.tabPage1.Controls.Add(this.ChampionPoints);
            this.tabPage1.Controls.Add(this.LblMoney);
            this.tabPage1.Controls.Add(this.SpecialTitles);
            this.tabPage1.Controls.Add(this.LblSpecialTitles);
            this.tabPage1.Controls.Add(this.NewbieProtection);
            this.tabPage1.Controls.Add(this.LblNewbieProtection);
            this.tabPage1.Controls.Add(this.SecurityPass);
            this.tabPage1.Controls.Add(this.LblSecurityPass);
            this.tabPage1.Controls.Add(this.VIPTime);
            this.tabPage1.Controls.Add(this.LblConquerPoints);
            this.tabPage1.Controls.Add(this.VIPLevel);
            this.tabPage1.Controls.Add(this.LblVIPLevel);
            this.tabPage1.Controls.Add(this.Money);
            this.tabPage1.Controls.Add(this.UserClamCP);
            this.tabPage1.Controls.Add(this.LblUserClamCP);
            this.tabPage1.Controls.Add(this.ConquerPoints);
            this.tabPage1.Controls.Add(this.CheckBoxAutoHunt);
            this.tabPage1.Controls.Add(this.BtnDisconnectUser);
            this.tabPage1.Controls.Add(this.BtnRefreshPC);
            this.tabPage1.Controls.Add(this.LbxOnScreen);
            this.tabPage1.Controls.Add(this.LblUserManaPoints);
            this.tabPage1.Controls.Add(this.label18);
            this.tabPage1.Controls.Add(this.LblUserHealthPoints);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this.LblUserPkPoints);
            this.tabPage1.Controls.Add(this.LblUserAdditionalPoints);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.LblUserSpirit);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.LblUserVitality);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.LblUserAgility);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.LblUserStrength);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.LblUserLocation);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.LblUserExperience);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.LblUserLevel);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.LblUserMateName);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.LblUserName);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.LblUserId);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Size = new System.Drawing.Size(1566, 705);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Character";
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.SystemColors.Control;
            this.trackBar1.Location = new System.Drawing.Point(781, 620);
            this.trackBar1.Maximum = 50;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(261, 45);
            this.trackBar1.TabIndex = 84;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.Value = 1;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.White;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox5.ErrorImage = null;
            this.pictureBox5.InitialImage = null;
            this.pictureBox5.Location = new System.Drawing.Point(739, 477);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(34, 34);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox5.TabIndex = 83;
            this.pictureBox5.TabStop = false;
            // 
            // checkBox7
            // 
            this.checkBox7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox7.AutoSize = true;
            this.checkBox7.BackColor = System.Drawing.Color.Transparent;
            this.checkBox7.ForeColor = System.Drawing.Color.Black;
            this.checkBox7.Location = new System.Drawing.Point(1020, 330);
            this.checkBox7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(88, 21);
            this.checkBox7.TabIndex = 82;
            this.checkBox7.Text = "Clear Rank";
            this.checkBox7.UseVisualStyleBackColor = false;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.ClearNobilityUser);
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox3.AutoSize = true;
            this.checkBox3.BackColor = System.Drawing.Color.Transparent;
            this.checkBox3.ForeColor = System.Drawing.Color.Black;
            this.checkBox3.Location = new System.Drawing.Point(1020, 303);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(77, 21);
            this.checkBox3.TabIndex = 81;
            this.checkBox3.Text = "In BotJail";
            this.checkBox3.UseVisualStyleBackColor = false;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.BotJailChecked);
            // 
            // groupBox10
            // 
            this.groupBox10.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox10.BackColor = System.Drawing.Color.Transparent;
            this.groupBox10.Controls.Add(this.comboBox10);
            this.groupBox10.Controls.Add(this.button49);
            this.groupBox10.ForeColor = System.Drawing.Color.Black;
            this.groupBox10.Location = new System.Drawing.Point(781, 517);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox10.Size = new System.Drawing.Size(278, 76);
            this.groupBox10.TabIndex = 80;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Remove Inventory";
            // 
            // comboBox10
            // 
            this.comboBox10.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox10.Location = new System.Drawing.Point(10, 33);
            this.comboBox10.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox10.Name = "comboBox10";
            this.comboBox10.Size = new System.Drawing.Size(180, 25);
            this.comboBox10.TabIndex = 42;
            // 
            // button49
            // 
            this.button49.BackColor = System.Drawing.Color.White;
            this.button49.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button49.Location = new System.Drawing.Point(198, 32);
            this.button49.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button49.Name = "button49";
            this.button49.Size = new System.Drawing.Size(72, 25);
            this.button49.TabIndex = 54;
            this.button49.Text = "Remove";
            this.button49.UseVisualStyleBackColor = true;
            this.button49.Click += new System.EventHandler(this.RemoveItem);
            // 
            // groupBox9
            // 
            this.groupBox9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox9.BackColor = System.Drawing.Color.Transparent;
            this.groupBox9.Controls.Add(this.comboBox9);
            this.groupBox9.Controls.Add(this.button46);
            this.groupBox9.ForeColor = System.Drawing.Color.Black;
            this.groupBox9.Location = new System.Drawing.Point(533, 517);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox9.Size = new System.Drawing.Size(240, 76);
            this.groupBox9.TabIndex = 79;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Remove Spell";
            // 
            // comboBox9
            // 
            this.comboBox9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox9.Location = new System.Drawing.Point(10, 33);
            this.comboBox9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox9.Name = "comboBox9";
            this.comboBox9.Size = new System.Drawing.Size(142, 25);
            this.comboBox9.TabIndex = 42;
            // 
            // button46
            // 
            this.button46.BackColor = System.Drawing.Color.White;
            this.button46.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button46.Location = new System.Drawing.Point(160, 32);
            this.button46.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button46.Name = "button46";
            this.button46.Size = new System.Drawing.Size(72, 25);
            this.button46.TabIndex = 54;
            this.button46.Text = "Remove";
            this.button46.UseVisualStyleBackColor = true;
            this.button46.Click += new System.EventHandler(this.RemoveSpell);
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox8.BackColor = System.Drawing.Color.Transparent;
            this.groupBox8.Controls.Add(this.comboBox8);
            this.groupBox8.Controls.Add(this.button45);
            this.groupBox8.ForeColor = System.Drawing.Color.Black;
            this.groupBox8.Location = new System.Drawing.Point(261, 517);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox8.Size = new System.Drawing.Size(264, 76);
            this.groupBox8.TabIndex = 78;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Give Special Titles";
            // 
            // comboBox8
            // 
            this.comboBox8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox8.Location = new System.Drawing.Point(10, 33);
            this.comboBox8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox8.Name = "comboBox8";
            this.comboBox8.Size = new System.Drawing.Size(189, 25);
            this.comboBox8.TabIndex = 42;
            // 
            // button45
            // 
            this.button45.BackColor = System.Drawing.Color.White;
            this.button45.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button45.Location = new System.Drawing.Point(211, 32);
            this.button45.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button45.Name = "button45";
            this.button45.Size = new System.Drawing.Size(45, 25);
            this.button45.TabIndex = 54;
            this.button45.Text = "Give";
            this.button45.UseVisualStyleBackColor = true;
            this.button45.Click += new System.EventHandler(this.GiveSpecialTitles);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.ErrorImage = global::TheChosenProject.Properties.Resources._0;
            this.pictureBox1.Image = global::TheChosenProject.Properties.Resources._0;
            this.pictureBox1.InitialImage = global::TheChosenProject.Properties.Resources._296;
            this.pictureBox1.Location = new System.Drawing.Point(1137, 536);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 77;
            this.pictureBox1.TabStop = false;
            // 
            // label34
            // 
            this.label34.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(786, 452);
            this.label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(48, 17);
            this.label34.TabIndex = 76;
            this.label34.Text = "Reborn";
            // 
            // label35
            // 
            this.label35.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(916, 452);
            this.label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(15, 17);
            this.label35.TabIndex = 75;
            this.label35.Text = "0";
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.Color.Transparent;
            this.checkBox2.ForeColor = System.Drawing.Color.Black;
            this.checkBox2.Location = new System.Drawing.Point(1020, 273);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(96, 21);
            this.checkBox2.TabIndex = 74;
            this.checkBox2.Text = "Use Vending";
            this.checkBox2.UseVisualStyleBackColor = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.VendorChecked);
            // 
            // label28
            // 
            this.label28.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(917, 394);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(38, 17);
            this.label28.TabIndex = 71;
            this.label28.Text = "None";
            // 
            // label29
            // 
            this.label29.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(788, 424);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(65, 17);
            this.label29.TabIndex = 70;
            this.label29.Text = "IP Address";
            // 
            // label30
            // 
            this.label30.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(917, 424);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(38, 17);
            this.label30.TabIndex = 68;
            this.label30.Text = "None";
            // 
            // label31
            // 
            this.label31.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(788, 394);
            this.label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(79, 17);
            this.label31.TabIndex = 69;
            this.label31.Text = "Mac Address";
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.ForeColor = System.Drawing.Color.Black;
            this.checkBox1.Location = new System.Drawing.Point(1020, 243);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(149, 21);
            this.checkBox1.TabIndex = 67;
            this.checkBox1.Text = "Project Manager [PM]";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.MakeUserPMChecked);
            // 
            // LblClanName
            // 
            this.LblClanName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblClanName.AutoSize = true;
            this.LblClanName.Location = new System.Drawing.Point(917, 334);
            this.LblClanName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblClanName.Name = "LblClanName";
            this.LblClanName.Size = new System.Drawing.Size(38, 17);
            this.LblClanName.TabIndex = 66;
            this.LblClanName.Text = "None";
            // 
            // LblNameChange
            // 
            this.LblNameChange.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblNameChange.AutoSize = true;
            this.LblNameChange.Location = new System.Drawing.Point(917, 244);
            this.LblNameChange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblNameChange.Name = "LblNameChange";
            this.LblNameChange.Size = new System.Drawing.Size(15, 17);
            this.LblNameChange.TabIndex = 65;
            this.LblNameChange.Text = "0";
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.Location = new System.Drawing.Point(1238, 276);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(137, 32);
            this.button1.TabIndex = 64;
            this.button1.Text = "Refresh screen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.BtnRefreshScreen_Click);
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(1244, 332);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(87, 17);
            this.label21.TabIndex = 62;
            this.label21.Text = "Players On PC";
            // 
            // LbxOnPCCount
            // 
            this.LbxOnPCCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbxOnPCCount.FormattingEnabled = true;
            this.LbxOnPCCount.ItemHeight = 17;
            this.LbxOnPCCount.Location = new System.Drawing.Point(1238, 358);
            this.LbxOnPCCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LbxOnPCCount.Name = "LbxOnPCCount";
            this.LbxOnPCCount.Size = new System.Drawing.Size(137, 123);
            this.LbxOnPCCount.TabIndex = 63;
            // 
            // CheckBoxOfflineVending
            // 
            this.CheckBoxOfflineVending.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CheckBoxOfflineVending.AutoSize = true;
            this.CheckBoxOfflineVending.BackColor = System.Drawing.Color.Transparent;
            this.CheckBoxOfflineVending.Enabled = false;
            this.CheckBoxOfflineVending.ForeColor = System.Drawing.Color.Black;
            this.CheckBoxOfflineVending.Location = new System.Drawing.Point(1020, 213);
            this.CheckBoxOfflineVending.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBoxOfflineVending.Name = "CheckBoxOfflineVending";
            this.CheckBoxOfflineVending.Size = new System.Drawing.Size(138, 21);
            this.CheckBoxOfflineVending.TabIndex = 60;
            this.CheckBoxOfflineVending.Text = "Use Offline-Vending";
            this.CheckBoxOfflineVending.UseVisualStyleBackColor = false;
            // 
            // CheckBoxOfflineHunting
            // 
            this.CheckBoxOfflineHunting.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CheckBoxOfflineHunting.AutoSize = true;
            this.CheckBoxOfflineHunting.BackColor = System.Drawing.Color.Transparent;
            this.CheckBoxOfflineHunting.Enabled = false;
            this.CheckBoxOfflineHunting.ForeColor = System.Drawing.Color.Black;
            this.CheckBoxOfflineHunting.Location = new System.Drawing.Point(1020, 183);
            this.CheckBoxOfflineHunting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBoxOfflineHunting.Name = "CheckBoxOfflineHunting";
            this.CheckBoxOfflineHunting.Size = new System.Drawing.Size(137, 21);
            this.CheckBoxOfflineHunting.TabIndex = 59;
            this.CheckBoxOfflineHunting.Text = "Use Offline-Hunting";
            this.CheckBoxOfflineHunting.UseVisualStyleBackColor = false;
            // 
            // CheckBoxOfflineTraining
            // 
            this.CheckBoxOfflineTraining.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CheckBoxOfflineTraining.AutoSize = true;
            this.CheckBoxOfflineTraining.BackColor = System.Drawing.Color.Transparent;
            this.CheckBoxOfflineTraining.Enabled = false;
            this.CheckBoxOfflineTraining.ForeColor = System.Drawing.Color.Black;
            this.CheckBoxOfflineTraining.Location = new System.Drawing.Point(1019, 153);
            this.CheckBoxOfflineTraining.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBoxOfflineTraining.Name = "CheckBoxOfflineTraining";
            this.CheckBoxOfflineTraining.Size = new System.Drawing.Size(139, 21);
            this.CheckBoxOfflineTraining.TabIndex = 58;
            this.CheckBoxOfflineTraining.Text = "Use Offline-Training";
            this.CheckBoxOfflineTraining.UseVisualStyleBackColor = false;
            // 
            // LblGuildName
            // 
            this.LblGuildName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblGuildName.AutoSize = true;
            this.LblGuildName.Location = new System.Drawing.Point(917, 274);
            this.LblGuildName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblGuildName.Name = "LblGuildName";
            this.LblGuildName.Size = new System.Drawing.Size(38, 17);
            this.LblGuildName.TabIndex = 56;
            this.LblGuildName.Text = "None";
            // 
            // ClanRank
            // 
            this.ClanRank.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ClanRank.AutoSize = true;
            this.ClanRank.Location = new System.Drawing.Point(788, 364);
            this.ClanRank.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ClanRank.Name = "ClanRank";
            this.ClanRank.Size = new System.Drawing.Size(65, 17);
            this.ClanRank.TabIndex = 57;
            this.ClanRank.Text = "Clan Rank";
            // 
            // LblClanRank
            // 
            this.LblClanRank.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblClanRank.AutoSize = true;
            this.LblClanRank.Location = new System.Drawing.Point(917, 364);
            this.LblClanRank.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblClanRank.Name = "LblClanRank";
            this.LblClanRank.Size = new System.Drawing.Size(38, 17);
            this.LblClanRank.TabIndex = 54;
            this.LblClanRank.Text = "None";
            // 
            // ClanName
            // 
            this.ClanName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ClanName.AutoSize = true;
            this.ClanName.Location = new System.Drawing.Point(788, 334);
            this.ClanName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ClanName.Name = "ClanName";
            this.ClanName.Size = new System.Drawing.Size(71, 17);
            this.ClanName.TabIndex = 55;
            this.ClanName.Text = "Clan Name";
            // 
            // GuildRank
            // 
            this.GuildRank.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GuildRank.AutoSize = true;
            this.GuildRank.Location = new System.Drawing.Point(788, 304);
            this.GuildRank.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GuildRank.Name = "GuildRank";
            this.GuildRank.Size = new System.Drawing.Size(69, 17);
            this.GuildRank.TabIndex = 53;
            this.GuildRank.Text = "Guild Rank";
            // 
            // LblGuildRank
            // 
            this.LblGuildRank.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblGuildRank.AutoSize = true;
            this.LblGuildRank.Location = new System.Drawing.Point(917, 304);
            this.LblGuildRank.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblGuildRank.Name = "LblGuildRank";
            this.LblGuildRank.Size = new System.Drawing.Size(38, 17);
            this.LblGuildRank.TabIndex = 50;
            this.LblGuildRank.Text = "None";
            // 
            // GuildName
            // 
            this.GuildName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GuildName.AutoSize = true;
            this.GuildName.Location = new System.Drawing.Point(788, 274);
            this.GuildName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GuildName.Name = "GuildName";
            this.GuildName.Size = new System.Drawing.Size(75, 17);
            this.GuildName.TabIndex = 51;
            this.GuildName.Text = "Guild Name";
            // 
            // LblClass
            // 
            this.LblClass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblClass.AutoSize = true;
            this.LblClass.Location = new System.Drawing.Point(917, 184);
            this.LblClass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblClass.Name = "LblClass";
            this.LblClass.Size = new System.Drawing.Size(15, 17);
            this.LblClass.TabIndex = 48;
            this.LblClass.Text = "0";
            // 
            // ExtraAtributes
            // 
            this.ExtraAtributes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ExtraAtributes.AutoSize = true;
            this.ExtraAtributes.Location = new System.Drawing.Point(511, 394);
            this.ExtraAtributes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ExtraAtributes.Name = "ExtraAtributes";
            this.ExtraAtributes.Size = new System.Drawing.Size(85, 17);
            this.ExtraAtributes.TabIndex = 49;
            this.ExtraAtributes.Text = "ExtraAtributes";
            // 
            // LblExtraAtributes
            // 
            this.LblExtraAtributes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblExtraAtributes.AutoSize = true;
            this.LblExtraAtributes.Location = new System.Drawing.Point(641, 394);
            this.LblExtraAtributes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblExtraAtributes.Name = "LblExtraAtributes";
            this.LblExtraAtributes.Size = new System.Drawing.Size(15, 17);
            this.LblExtraAtributes.TabIndex = 46;
            this.LblExtraAtributes.Text = "0";
            // 
            // NameChange
            // 
            this.NameChange.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NameChange.AutoSize = true;
            this.NameChange.Location = new System.Drawing.Point(788, 244);
            this.NameChange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NameChange.Name = "NameChange";
            this.NameChange.Size = new System.Drawing.Size(88, 17);
            this.NameChange.TabIndex = 47;
            this.NameChange.Text = "Name Change";
            // 
            // LblFirstClass
            // 
            this.LblFirstClass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblFirstClass.AutoSize = true;
            this.LblFirstClass.Location = new System.Drawing.Point(916, 124);
            this.LblFirstClass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblFirstClass.Name = "LblFirstClass";
            this.LblFirstClass.Size = new System.Drawing.Size(15, 17);
            this.LblFirstClass.TabIndex = 44;
            this.LblFirstClass.Text = "0";
            // 
            // WHMoney
            // 
            this.WHMoney.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.WHMoney.AutoSize = true;
            this.WHMoney.Location = new System.Drawing.Point(788, 214);
            this.WHMoney.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WHMoney.Name = "WHMoney";
            this.WHMoney.Size = new System.Drawing.Size(66, 17);
            this.WHMoney.TabIndex = 45;
            this.WHMoney.Text = "WHMoney";
            // 
            // LblWHMoney
            // 
            this.LblWHMoney.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblWHMoney.AutoSize = true;
            this.LblWHMoney.Location = new System.Drawing.Point(917, 214);
            this.LblWHMoney.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblWHMoney.Name = "LblWHMoney";
            this.LblWHMoney.Size = new System.Drawing.Size(15, 17);
            this.LblWHMoney.TabIndex = 42;
            this.LblWHMoney.Text = "0";
            // 
            // Class
            // 
            this.Class.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Class.AutoSize = true;
            this.Class.Location = new System.Drawing.Point(788, 184);
            this.Class.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Class.Name = "Class";
            this.Class.Size = new System.Drawing.Size(36, 17);
            this.Class.TabIndex = 43;
            this.Class.Text = "Class";
            // 
            // LblNobilityRank
            // 
            this.LblNobilityRank.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblNobilityRank.AutoSize = true;
            this.LblNobilityRank.Location = new System.Drawing.Point(641, 482);
            this.LblNobilityRank.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblNobilityRank.Name = "LblNobilityRank";
            this.LblNobilityRank.Size = new System.Drawing.Size(15, 17);
            this.LblNobilityRank.TabIndex = 40;
            this.LblNobilityRank.Text = "0";
            // 
            // SecondClass
            // 
            this.SecondClass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SecondClass.AutoSize = true;
            this.SecondClass.Location = new System.Drawing.Point(788, 154);
            this.SecondClass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SecondClass.Name = "SecondClass";
            this.SecondClass.Size = new System.Drawing.Size(75, 17);
            this.SecondClass.TabIndex = 41;
            this.SecondClass.Text = "SecondClass";
            // 
            // LblSecondClass
            // 
            this.LblSecondClass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblSecondClass.AutoSize = true;
            this.LblSecondClass.Location = new System.Drawing.Point(916, 154);
            this.LblSecondClass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblSecondClass.Name = "LblSecondClass";
            this.LblSecondClass.Size = new System.Drawing.Size(15, 17);
            this.LblSecondClass.TabIndex = 38;
            this.LblSecondClass.Text = "0";
            // 
            // FirstClass
            // 
            this.FirstClass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FirstClass.AutoSize = true;
            this.FirstClass.Location = new System.Drawing.Point(786, 124);
            this.FirstClass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FirstClass.Name = "FirstClass";
            this.FirstClass.Size = new System.Drawing.Size(59, 17);
            this.FirstClass.TabIndex = 39;
            this.FirstClass.Text = "FirstClass";
            // 
            // LblChampionPoints
            // 
            this.LblChampionPoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblChampionPoints.AutoSize = true;
            this.LblChampionPoints.Location = new System.Drawing.Point(641, 424);
            this.LblChampionPoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblChampionPoints.Name = "LblChampionPoints";
            this.LblChampionPoints.Size = new System.Drawing.Size(15, 17);
            this.LblChampionPoints.TabIndex = 36;
            this.LblChampionPoints.Text = "0";
            // 
            // NobilityRank
            // 
            this.NobilityRank.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NobilityRank.AutoSize = true;
            this.NobilityRank.Location = new System.Drawing.Point(511, 482);
            this.NobilityRank.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NobilityRank.Name = "NobilityRank";
            this.NobilityRank.Size = new System.Drawing.Size(83, 17);
            this.NobilityRank.TabIndex = 37;
            this.NobilityRank.Text = "Nobility Rank";
            // 
            // LblVIPTime
            // 
            this.LblVIPTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblVIPTime.AutoSize = true;
            this.LblVIPTime.Location = new System.Drawing.Point(641, 274);
            this.LblVIPTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblVIPTime.Name = "LblVIPTime";
            this.LblVIPTime.Size = new System.Drawing.Size(15, 17);
            this.LblVIPTime.TabIndex = 34;
            this.LblVIPTime.Text = "0";
            // 
            // OnlinePoints
            // 
            this.OnlinePoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OnlinePoints.AutoSize = true;
            this.OnlinePoints.Location = new System.Drawing.Point(511, 454);
            this.OnlinePoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.OnlinePoints.Name = "OnlinePoints";
            this.OnlinePoints.Size = new System.Drawing.Size(81, 17);
            this.OnlinePoints.TabIndex = 35;
            this.OnlinePoints.Text = "Online Points";
            // 
            // LblOnlinePoints
            // 
            this.LblOnlinePoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblOnlinePoints.AutoSize = true;
            this.LblOnlinePoints.Location = new System.Drawing.Point(641, 454);
            this.LblOnlinePoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblOnlinePoints.Name = "LblOnlinePoints";
            this.LblOnlinePoints.Size = new System.Drawing.Size(15, 17);
            this.LblOnlinePoints.TabIndex = 32;
            this.LblOnlinePoints.Text = "0";
            // 
            // ChampionPoints
            // 
            this.ChampionPoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ChampionPoints.AutoSize = true;
            this.ChampionPoints.Location = new System.Drawing.Point(511, 424);
            this.ChampionPoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ChampionPoints.Name = "ChampionPoints";
            this.ChampionPoints.Size = new System.Drawing.Size(102, 17);
            this.ChampionPoints.TabIndex = 33;
            this.ChampionPoints.Text = "Champion Points";
            // 
            // LblMoney
            // 
            this.LblMoney.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblMoney.AutoSize = true;
            this.LblMoney.Location = new System.Drawing.Point(641, 214);
            this.LblMoney.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblMoney.Name = "LblMoney";
            this.LblMoney.Size = new System.Drawing.Size(15, 17);
            this.LblMoney.TabIndex = 28;
            this.LblMoney.Text = "0";
            // 
            // SpecialTitles
            // 
            this.SpecialTitles.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SpecialTitles.AutoSize = true;
            this.SpecialTitles.Location = new System.Drawing.Point(511, 364);
            this.SpecialTitles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SpecialTitles.Name = "SpecialTitles";
            this.SpecialTitles.Size = new System.Drawing.Size(78, 17);
            this.SpecialTitles.TabIndex = 29;
            this.SpecialTitles.Text = "Special Titles";
            // 
            // LblSpecialTitles
            // 
            this.LblSpecialTitles.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblSpecialTitles.AutoSize = true;
            this.LblSpecialTitles.Location = new System.Drawing.Point(641, 364);
            this.LblSpecialTitles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblSpecialTitles.Name = "LblSpecialTitles";
            this.LblSpecialTitles.Size = new System.Drawing.Size(15, 17);
            this.LblSpecialTitles.TabIndex = 26;
            this.LblSpecialTitles.Text = "0";
            // 
            // NewbieProtection
            // 
            this.NewbieProtection.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NewbieProtection.AutoSize = true;
            this.NewbieProtection.Location = new System.Drawing.Point(511, 334);
            this.NewbieProtection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NewbieProtection.Name = "NewbieProtection";
            this.NewbieProtection.Size = new System.Drawing.Size(109, 17);
            this.NewbieProtection.TabIndex = 27;
            this.NewbieProtection.Text = "Newbie Protection";
            // 
            // LblNewbieProtection
            // 
            this.LblNewbieProtection.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblNewbieProtection.AutoSize = true;
            this.LblNewbieProtection.Location = new System.Drawing.Point(641, 334);
            this.LblNewbieProtection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblNewbieProtection.Name = "LblNewbieProtection";
            this.LblNewbieProtection.Size = new System.Drawing.Size(15, 17);
            this.LblNewbieProtection.TabIndex = 24;
            this.LblNewbieProtection.Text = "0";
            // 
            // SecurityPass
            // 
            this.SecurityPass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SecurityPass.AutoSize = true;
            this.SecurityPass.Location = new System.Drawing.Point(511, 304);
            this.SecurityPass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SecurityPass.Name = "SecurityPass";
            this.SecurityPass.Size = new System.Drawing.Size(79, 17);
            this.SecurityPass.TabIndex = 25;
            this.SecurityPass.Text = "Security Pass";
            // 
            // LblSecurityPass
            // 
            this.LblSecurityPass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblSecurityPass.AutoSize = true;
            this.LblSecurityPass.Location = new System.Drawing.Point(641, 304);
            this.LblSecurityPass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblSecurityPass.Name = "LblSecurityPass";
            this.LblSecurityPass.Size = new System.Drawing.Size(15, 17);
            this.LblSecurityPass.TabIndex = 22;
            this.LblSecurityPass.Text = "0";
            // 
            // VIPTime
            // 
            this.VIPTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.VIPTime.AutoSize = true;
            this.VIPTime.Location = new System.Drawing.Point(511, 274);
            this.VIPTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.VIPTime.Name = "VIPTime";
            this.VIPTime.Size = new System.Drawing.Size(53, 17);
            this.VIPTime.TabIndex = 23;
            this.VIPTime.Text = "VIPTime";
            // 
            // LblConquerPoints
            // 
            this.LblConquerPoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblConquerPoints.AutoSize = true;
            this.LblConquerPoints.Location = new System.Drawing.Point(641, 154);
            this.LblConquerPoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblConquerPoints.Name = "LblConquerPoints";
            this.LblConquerPoints.Size = new System.Drawing.Size(15, 17);
            this.LblConquerPoints.TabIndex = 20;
            this.LblConquerPoints.Text = "0";
            // 
            // VIPLevel
            // 
            this.VIPLevel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.VIPLevel.AutoSize = true;
            this.VIPLevel.Location = new System.Drawing.Point(511, 244);
            this.VIPLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.VIPLevel.Name = "VIPLevel";
            this.VIPLevel.Size = new System.Drawing.Size(53, 17);
            this.VIPLevel.TabIndex = 21;
            this.VIPLevel.Text = "VIPLevel";
            // 
            // LblVIPLevel
            // 
            this.LblVIPLevel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblVIPLevel.AutoSize = true;
            this.LblVIPLevel.Location = new System.Drawing.Point(641, 244);
            this.LblVIPLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblVIPLevel.Name = "LblVIPLevel";
            this.LblVIPLevel.Size = new System.Drawing.Size(15, 17);
            this.LblVIPLevel.TabIndex = 18;
            this.LblVIPLevel.Text = "0";
            // 
            // Money
            // 
            this.Money.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Money.AutoSize = true;
            this.Money.Location = new System.Drawing.Point(511, 214);
            this.Money.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Money.Name = "Money";
            this.Money.Size = new System.Drawing.Size(45, 17);
            this.Money.TabIndex = 19;
            this.Money.Text = "Money";
            // 
            // UserClamCP
            // 
            this.UserClamCP.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.UserClamCP.AutoSize = true;
            this.UserClamCP.Location = new System.Drawing.Point(511, 184);
            this.UserClamCP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.UserClamCP.Name = "UserClamCP";
            this.UserClamCP.Size = new System.Drawing.Size(73, 17);
            this.UserClamCP.TabIndex = 17;
            this.UserClamCP.Text = "ClaimedCPs";
            // 
            // LblUserClamCP
            // 
            this.LblUserClamCP.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserClamCP.AutoSize = true;
            this.LblUserClamCP.Location = new System.Drawing.Point(641, 184);
            this.LblUserClamCP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserClamCP.Name = "LblUserClamCP";
            this.LblUserClamCP.Size = new System.Drawing.Size(15, 17);
            this.LblUserClamCP.TabIndex = 14;
            this.LblUserClamCP.Text = "0";
            // 
            // ConquerPoints
            // 
            this.ConquerPoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ConquerPoints.AutoSize = true;
            this.ConquerPoints.Location = new System.Drawing.Point(511, 154);
            this.ConquerPoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ConquerPoints.Name = "ConquerPoints";
            this.ConquerPoints.Size = new System.Drawing.Size(88, 17);
            this.ConquerPoints.TabIndex = 15;
            this.ConquerPoints.Text = "ConquerPoints";
            // 
            // CheckBoxAutoHunt
            // 
            this.CheckBoxAutoHunt.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CheckBoxAutoHunt.AutoSize = true;
            this.CheckBoxAutoHunt.BackColor = System.Drawing.Color.Transparent;
            this.CheckBoxAutoHunt.ForeColor = System.Drawing.Color.Black;
            this.CheckBoxAutoHunt.Location = new System.Drawing.Point(1019, 123);
            this.CheckBoxAutoHunt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckBoxAutoHunt.Name = "CheckBoxAutoHunt";
            this.CheckBoxAutoHunt.Size = new System.Drawing.Size(126, 21);
            this.CheckBoxAutoHunt.TabIndex = 13;
            this.CheckBoxAutoHunt.Text = "Use Auto-Hunting";
            this.CheckBoxAutoHunt.UseVisualStyleBackColor = false;
            this.CheckBoxAutoHunt.CheckedChanged += new System.EventHandler(this.CheckBoxAutoHunt_CheckedChanged);
            // 
            // BtnDisconnectUser
            // 
            this.BtnDisconnectUser.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnDisconnectUser.Location = new System.Drawing.Point(1128, 606);
            this.BtnDisconnectUser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnDisconnectUser.Name = "BtnDisconnectUser";
            this.BtnDisconnectUser.Size = new System.Drawing.Size(101, 30);
            this.BtnDisconnectUser.TabIndex = 12;
            this.BtnDisconnectUser.Text = "Disconnect";
            this.BtnDisconnectUser.UseVisualStyleBackColor = true;
            this.BtnDisconnectUser.Click += new System.EventHandler(this.BtnDisconnectUser_Click);
            // 
            // BtnRefreshPC
            // 
            this.BtnRefreshPC.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnRefreshPC.Location = new System.Drawing.Point(1238, 494);
            this.BtnRefreshPC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnRefreshPC.Name = "BtnRefreshPC";
            this.BtnRefreshPC.Size = new System.Drawing.Size(136, 30);
            this.BtnRefreshPC.TabIndex = 11;
            this.BtnRefreshPC.Text = "Refresh PC";
            this.BtnRefreshPC.UseVisualStyleBackColor = true;
            this.BtnRefreshPC.Click += new System.EventHandler(this.BtnRefreshPC_Click);
            // 
            // LbxOnScreen
            // 
            this.LbxOnScreen.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbxOnScreen.FormattingEnabled = true;
            this.LbxOnScreen.ItemHeight = 17;
            this.LbxOnScreen.Location = new System.Drawing.Point(1238, 119);
            this.LbxOnScreen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LbxOnScreen.Name = "LbxOnScreen";
            this.LbxOnScreen.Size = new System.Drawing.Size(137, 140);
            this.LbxOnScreen.TabIndex = 10;
            this.LbxOnScreen.Click += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // LblUserManaPoints
            // 
            this.LblUserManaPoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserManaPoints.AutoSize = true;
            this.LblUserManaPoints.Location = new System.Drawing.Point(388, 482);
            this.LblUserManaPoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserManaPoints.Name = "LblUserManaPoints";
            this.LblUserManaPoints.Size = new System.Drawing.Size(15, 17);
            this.LblUserManaPoints.TabIndex = 8;
            this.LblUserManaPoints.Text = "0";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(258, 482);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(40, 17);
            this.label18.TabIndex = 9;
            this.label18.Text = "Mana";
            // 
            // LblUserHealthPoints
            // 
            this.LblUserHealthPoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserHealthPoints.AutoSize = true;
            this.LblUserHealthPoints.Location = new System.Drawing.Point(388, 452);
            this.LblUserHealthPoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserHealthPoints.Name = "LblUserHealthPoints";
            this.LblUserHealthPoints.Size = new System.Drawing.Size(15, 17);
            this.LblUserHealthPoints.TabIndex = 6;
            this.LblUserHealthPoints.Text = "0";
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(258, 452);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(44, 17);
            this.label20.TabIndex = 7;
            this.label20.Text = "Health";
            // 
            // LblUserPkPoints
            // 
            this.LblUserPkPoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserPkPoints.AutoSize = true;
            this.LblUserPkPoints.Location = new System.Drawing.Point(388, 424);
            this.LblUserPkPoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserPkPoints.Name = "LblUserPkPoints";
            this.LblUserPkPoints.Size = new System.Drawing.Size(15, 17);
            this.LblUserPkPoints.TabIndex = 5;
            this.LblUserPkPoints.Text = "0";
            // 
            // LblUserAdditionalPoints
            // 
            this.LblUserAdditionalPoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserAdditionalPoints.AutoSize = true;
            this.LblUserAdditionalPoints.Location = new System.Drawing.Point(388, 394);
            this.LblUserAdditionalPoints.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserAdditionalPoints.Name = "LblUserAdditionalPoints";
            this.LblUserAdditionalPoints.Size = new System.Drawing.Size(15, 17);
            this.LblUserAdditionalPoints.TabIndex = 5;
            this.LblUserAdditionalPoints.Text = "0";
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(258, 424);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(60, 17);
            this.label16.TabIndex = 5;
            this.label16.Text = "PK Points";
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(258, 394);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(101, 17);
            this.label15.TabIndex = 5;
            this.label15.Text = "Additional points";
            // 
            // LblUserSpirit
            // 
            this.LblUserSpirit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserSpirit.AutoSize = true;
            this.LblUserSpirit.Location = new System.Drawing.Point(388, 364);
            this.LblUserSpirit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserSpirit.Name = "LblUserSpirit";
            this.LblUserSpirit.Size = new System.Drawing.Size(15, 17);
            this.LblUserSpirit.TabIndex = 5;
            this.LblUserSpirit.Text = "0";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(258, 364);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 17);
            this.label14.TabIndex = 5;
            this.label14.Text = "Spirit";
            // 
            // LblUserVitality
            // 
            this.LblUserVitality.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserVitality.AutoSize = true;
            this.LblUserVitality.Location = new System.Drawing.Point(388, 334);
            this.LblUserVitality.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserVitality.Name = "LblUserVitality";
            this.LblUserVitality.Size = new System.Drawing.Size(15, 17);
            this.LblUserVitality.TabIndex = 5;
            this.LblUserVitality.Text = "0";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(258, 334);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 17);
            this.label13.TabIndex = 5;
            this.label13.Text = "Vitality";
            // 
            // LblUserAgility
            // 
            this.LblUserAgility.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserAgility.AutoSize = true;
            this.LblUserAgility.Location = new System.Drawing.Point(388, 304);
            this.LblUserAgility.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserAgility.Name = "LblUserAgility";
            this.LblUserAgility.Size = new System.Drawing.Size(15, 17);
            this.LblUserAgility.TabIndex = 5;
            this.LblUserAgility.Text = "0";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(258, 304);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(42, 17);
            this.label12.TabIndex = 5;
            this.label12.Text = "Agility";
            // 
            // LblUserStrength
            // 
            this.LblUserStrength.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserStrength.AutoSize = true;
            this.LblUserStrength.Location = new System.Drawing.Point(388, 274);
            this.LblUserStrength.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserStrength.Name = "LblUserStrength";
            this.LblUserStrength.Size = new System.Drawing.Size(15, 17);
            this.LblUserStrength.TabIndex = 5;
            this.LblUserStrength.Text = "0";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(258, 274);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 17);
            this.label11.TabIndex = 5;
            this.label11.Text = "Strength";
            // 
            // LblUserLocation
            // 
            this.LblUserLocation.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserLocation.AutoSize = true;
            this.LblUserLocation.Location = new System.Drawing.Point(635, 124);
            this.LblUserLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserLocation.Name = "LblUserLocation";
            this.LblUserLocation.Size = new System.Drawing.Size(67, 17);
            this.LblUserLocation.TabIndex = 4;
            this.LblUserLocation.Text = "None (0,0)";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(1234, 97);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(108, 17);
            this.label17.TabIndex = 4;
            this.label17.Text = "Players On Screen";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(511, 124);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 17);
            this.label10.TabIndex = 4;
            this.label10.Text = "Location";
            // 
            // LblUserExperience
            // 
            this.LblUserExperience.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserExperience.AutoSize = true;
            this.LblUserExperience.Location = new System.Drawing.Point(388, 244);
            this.LblUserExperience.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserExperience.Name = "LblUserExperience";
            this.LblUserExperience.Size = new System.Drawing.Size(45, 17);
            this.LblUserExperience.TabIndex = 3;
            this.LblUserExperience.Text = "0 (0%)";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(258, 244);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 17);
            this.label9.TabIndex = 3;
            this.label9.Text = "Experience";
            // 
            // LblUserLevel
            // 
            this.LblUserLevel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserLevel.AutoSize = true;
            this.LblUserLevel.Location = new System.Drawing.Point(388, 214);
            this.LblUserLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserLevel.Name = "LblUserLevel";
            this.LblUserLevel.Size = new System.Drawing.Size(15, 17);
            this.LblUserLevel.TabIndex = 3;
            this.LblUserLevel.Text = "1";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(258, 214);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 17);
            this.label8.TabIndex = 3;
            this.label8.Text = "Level";
            // 
            // LblUserMateName
            // 
            this.LblUserMateName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserMateName.AutoSize = true;
            this.LblUserMateName.Location = new System.Drawing.Point(388, 184);
            this.LblUserMateName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserMateName.Name = "LblUserMateName";
            this.LblUserMateName.Size = new System.Drawing.Size(38, 17);
            this.LblUserMateName.TabIndex = 2;
            this.LblUserMateName.Text = "None";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(258, 184);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Mate name";
            // 
            // LblUserName
            // 
            this.LblUserName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserName.AutoSize = true;
            this.LblUserName.Location = new System.Drawing.Point(388, 154);
            this.LblUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserName.Name = "LblUserName";
            this.LblUserName.Size = new System.Drawing.Size(38, 17);
            this.LblUserName.TabIndex = 1;
            this.LblUserName.Text = "None";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(258, 154);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "Name";
            // 
            // LblUserId
            // 
            this.LblUserId.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblUserId.AutoSize = true;
            this.LblUserId.Location = new System.Drawing.Point(388, 124);
            this.LblUserId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblUserId.Name = "LblUserId";
            this.LblUserId.Size = new System.Drawing.Size(15, 17);
            this.LblUserId.TabIndex = 0;
            this.LblUserId.Text = "0";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(258, 124);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Identity";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage2.Controls.Add(this.button16);
            this.tabPage2.Controls.Add(this.button15);
            this.tabPage2.Controls.Add(this.button9);
            this.tabPage2.Controls.Add(this.button17);
            this.tabPage2.Controls.Add(this.button14);
            this.tabPage2.Controls.Add(this.button13);
            this.tabPage2.Controls.Add(this.button12);
            this.tabPage2.Controls.Add(this.button11);
            this.tabPage2.Controls.Add(this.button10);
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Size = new System.Drawing.Size(1566, 705);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Inventory";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button16
            // 
            this.button16.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button16.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.button16.ForeColor = System.Drawing.Color.Black;
            this.button16.Location = new System.Drawing.Point(880, 455);
            this.button16.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(188, 45);
            this.button16.TabIndex = 76;
            this.button16.Text = "Surprise Box";
            this.button16.UseVisualStyleBackColor = false;
            this.button16.Click += new System.EventHandler(this.GiveSurpriseBox);
            // 
            // button15
            // 
            this.button15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button15.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.button15.ForeColor = System.Drawing.Color.Black;
            this.button15.Location = new System.Drawing.Point(880, 92);
            this.button15.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(188, 39);
            this.button15.TabIndex = 75;
            this.button15.Text = "VIPToken(30)";
            this.button15.UseVisualStyleBackColor = false;
            this.button15.Click += new System.EventHandler(this.VIPToken30);
            // 
            // button9
            // 
            this.button9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button9.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.button9.ForeColor = System.Drawing.Color.Black;
            this.button9.Location = new System.Drawing.Point(880, 45);
            this.button9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(188, 39);
            this.button9.TabIndex = 74;
            this.button9.Text = "VIPToken(1)";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button17
            // 
            this.button17.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button17.BackColor = System.Drawing.Color.CadetBlue;
            this.button17.ForeColor = System.Drawing.Color.Black;
            this.button17.Location = new System.Drawing.Point(880, 247);
            this.button17.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(188, 51);
            this.button17.TabIndex = 73;
            this.button17.Text = "Equipments Taoist";
            this.button17.UseVisualStyleBackColor = false;
            this.button17.Click += new System.EventHandler(this.GiveEquipmentTaoist);
            // 
            // button14
            // 
            this.button14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button14.BackColor = System.Drawing.Color.CadetBlue;
            this.button14.ForeColor = System.Drawing.Color.Black;
            this.button14.Location = new System.Drawing.Point(880, 137);
            this.button14.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(188, 48);
            this.button14.TabIndex = 72;
            this.button14.Text = "Equipments Monk";
            this.button14.UseVisualStyleBackColor = false;
            this.button14.Click += new System.EventHandler(this.GiveEquipmentMonk);
            // 
            // button13
            // 
            this.button13.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button13.BackColor = System.Drawing.Color.CadetBlue;
            this.button13.ForeColor = System.Drawing.Color.Black;
            this.button13.Location = new System.Drawing.Point(880, 193);
            this.button13.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(188, 48);
            this.button13.TabIndex = 71;
            this.button13.Text = "Equipments Ninja";
            this.button13.UseVisualStyleBackColor = false;
            this.button13.Click += new System.EventHandler(this.GiveEquipmentNinja);
            // 
            // button12
            // 
            this.button12.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button12.BackColor = System.Drawing.Color.CadetBlue;
            this.button12.ForeColor = System.Drawing.Color.Black;
            this.button12.Location = new System.Drawing.Point(880, 404);
            this.button12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(188, 45);
            this.button12.TabIndex = 70;
            this.button12.Text = "Equipments Archer";
            this.button12.UseVisualStyleBackColor = false;
            this.button12.Click += new System.EventHandler(this.GiveEquipmentArchera);
            // 
            // button11
            // 
            this.button11.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button11.BackColor = System.Drawing.Color.CadetBlue;
            this.button11.ForeColor = System.Drawing.Color.Black;
            this.button11.Location = new System.Drawing.Point(880, 356);
            this.button11.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(188, 42);
            this.button11.TabIndex = 69;
            this.button11.Text = "Equipments Warrior";
            this.button11.UseVisualStyleBackColor = false;
            this.button11.Click += new System.EventHandler(this.GiveEquipmentWarrior);
            // 
            // button10
            // 
            this.button10.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button10.BackColor = System.Drawing.Color.CadetBlue;
            this.button10.ForeColor = System.Drawing.Color.Black;
            this.button10.Location = new System.Drawing.Point(880, 302);
            this.button10.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(188, 51);
            this.button10.TabIndex = 68;
            this.button10.Text = "Equipments Trojan";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.GiveEquipmentTrojan);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox5.BackColor = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.pictureBox7);
            this.groupBox5.Controls.Add(this.comboBox4);
            this.groupBox5.Controls.Add(this.button8);
            this.groupBox5.ForeColor = System.Drawing.Color.Black;
            this.groupBox5.Location = new System.Drawing.Point(294, 45);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox5.Size = new System.Drawing.Size(284, 455);
            this.groupBox5.TabIndex = 59;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Rare Accessories";
            // 
            // pictureBox7
            // 
            this.pictureBox7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox7.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox7.ErrorImage = global::TheChosenProject.Properties.Resources._0;
            this.pictureBox7.InitialImage = null;
            this.pictureBox7.Location = new System.Drawing.Point(110, 318);
            this.pictureBox7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(64, 64);
            this.pictureBox7.TabIndex = 78;
            this.pictureBox7.TabStop = false;
            // 
            // comboBox4
            // 
            this.comboBox4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(10, 33);
            this.comboBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(265, 25);
            this.comboBox4.TabIndex = 42;
            this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.GiveItemsSelected);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.White;
            this.button8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button8.Location = new System.Drawing.Point(5, 392);
            this.button8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(275, 33);
            this.button8.TabIndex = 54;
            this.button8.Text = "Send Inventory";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.pictureBox8);
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Controls.Add(this.button7);
            this.groupBox3.ForeColor = System.Drawing.Color.Black;
            this.groupBox3.Location = new System.Drawing.Point(585, 45);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Size = new System.Drawing.Size(284, 455);
            this.groupBox3.TabIndex = 58;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Rare Garments";
            // 
            // pictureBox8
            // 
            this.pictureBox8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox8.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox8.ErrorImage = global::TheChosenProject.Properties.Resources._0;
            this.pictureBox8.InitialImage = null;
            this.pictureBox8.Location = new System.Drawing.Point(104, 318);
            this.pictureBox8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(64, 64);
            this.pictureBox8.TabIndex = 78;
            this.pictureBox8.TabStop = false;
            // 
            // comboBox2
            // 
            this.comboBox2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(10, 33);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(265, 25);
            this.comboBox2.TabIndex = 42;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.GiveItemsSelected);
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.White;
            this.button7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button7.Location = new System.Drawing.Point(4, 392);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(276, 33);
            this.button7.TabIndex = 54;
            this.button7.Text = "Send Inventory";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.pictureBox6);
            this.groupBox4.Controls.Add(this.numericUpDown21);
            this.groupBox4.Controls.Add(this.comboBox3);
            this.groupBox4.Controls.Add(this.button6);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.ForeColor = System.Drawing.Color.Black;
            this.groupBox4.Location = new System.Drawing.Point(3, 45);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox4.Size = new System.Drawing.Size(284, 455);
            this.groupBox4.TabIndex = 57;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Gived Items";
            // 
            // pictureBox6
            // 
            this.pictureBox6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox6.BackColor = System.Drawing.Color.White;
            this.pictureBox6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox6.ErrorImage = global::TheChosenProject.Properties.Resources._0;
            this.pictureBox6.InitialImage = null;
            this.pictureBox6.Location = new System.Drawing.Point(113, 318);
            this.pictureBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(64, 64);
            this.pictureBox6.TabIndex = 78;
            this.pictureBox6.TabStop = false;
            // 
            // numericUpDown21
            // 
            this.numericUpDown21.Location = new System.Drawing.Point(63, 73);
            this.numericUpDown21.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown21.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown21.Name = "numericUpDown21";
            this.numericUpDown21.Size = new System.Drawing.Size(213, 25);
            this.numericUpDown21.TabIndex = 77;
            this.numericUpDown21.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // comboBox3
            // 
            this.comboBox3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(10, 33);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(265, 25);
            this.comboBox3.TabIndex = 42;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.GiveItemsSelected);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.White;
            this.button6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button6.Location = new System.Drawing.Point(4, 392);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(272, 33);
            this.button6.TabIndex = 54;
            this.button6.Text = "Send Inventory";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label22.Location = new System.Drawing.Point(2, 75);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(39, 17);
            this.label22.TabIndex = 44;
            this.label22.Text = "+Plus";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage3.Controls.Add(this.button67);
            this.tabPage3.Controls.Add(this.dataGridView3);
            this.tabPage3.Controls.Add(this.label157);
            this.tabPage3.Controls.Add(this.label155);
            this.tabPage3.Controls.Add(this.button26);
            this.tabPage3.Controls.Add(this.button23);
            this.tabPage3.Controls.Add(this.button54);
            this.tabPage3.Controls.Add(this.button48);
            this.tabPage3.Controls.Add(this.button36);
            this.tabPage3.Controls.Add(this.button44);
            this.tabPage3.Controls.Add(this.button43);
            this.tabPage3.Controls.Add(this.button42);
            this.tabPage3.Controls.Add(this.button41);
            this.tabPage3.Controls.Add(this.button40);
            this.tabPage3.Controls.Add(this.button39);
            this.tabPage3.Controls.Add(this.button309);
            this.tabPage3.Controls.Add(this.button38);
            this.tabPage3.Controls.Add(this.button37);
            this.tabPage3.Controls.Add(this.button34);
            this.tabPage3.Controls.Add(this.button33);
            this.tabPage3.Controls.Add(this.button31);
            this.tabPage3.Controls.Add(this.listBox1);
            this.tabPage3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1566, 705);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Schedules Events";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button67
            // 
            this.button67.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button67.BackColor = System.Drawing.Color.Maroon;
            this.button67.ForeColor = System.Drawing.Color.Black;
            this.button67.Location = new System.Drawing.Point(704, 200);
            this.button67.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button67.Name = "button67";
            this.button67.Size = new System.Drawing.Size(159, 39);
            this.button67.TabIndex = 65607;
            this.button67.Text = "HOGAME[DONT]";
            this.button67.UseVisualStyleBackColor = false;
            this.button67.Click += new System.EventHandler(this.StartHeroOfGame);
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataGridView3.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView3.ColumnHeadersHeight = 29;
            this.dataGridView3.Location = new System.Drawing.Point(247, 440);
            this.dataGridView3.MultiSelect = false;
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowHeadersWidth = 51;
            this.dataGridView3.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView3.Size = new System.Drawing.Size(1071, 169);
            this.dataGridView3.TabIndex = 65555;
            // 
            // label157
            // 
            this.label157.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label157.AutoSize = true;
            this.label157.BackColor = System.Drawing.Color.Black;
            this.label157.Font = new System.Drawing.Font("Tahoma", 25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label157.ForeColor = System.Drawing.Color.FloralWhite;
            this.label157.Location = new System.Drawing.Point(81, 55);
            this.label157.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label157.Name = "label157";
            this.label157.Size = new System.Drawing.Size(441, 41);
            this.label157.TabIndex = 65543;
            this.label157.Text = "RUNNING TOURNAMNET";
            this.label157.UseMnemonic = false;
            // 
            // label155
            // 
            this.label155.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label155.AutoSize = true;
            this.label155.BackColor = System.Drawing.Color.Black;
            this.label155.Font = new System.Drawing.Font("Tahoma", 25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label155.ForeColor = System.Drawing.Color.FloralWhite;
            this.label155.Location = new System.Drawing.Point(883, 91);
            this.label155.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label155.Name = "label155";
            this.label155.Size = new System.Drawing.Size(384, 41);
            this.label155.TabIndex = 65542;
            this.label155.Text = "START TOURNAMENT";
            this.label155.UseMnemonic = false;
            // 
            // button26
            // 
            this.button26.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button26.BackColor = System.Drawing.Color.SteelBlue;
            this.button26.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button26.ForeColor = System.Drawing.SystemColors.Control;
            this.button26.Location = new System.Drawing.Point(530, 364);
            this.button26.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(149, 31);
            this.button26.TabIndex = 95;
            this.button26.Text = "LIVE TOURNAMENT";
            this.button26.UseVisualStyleBackColor = false;
            this.button26.Click += new System.EventHandler(this.GetLiveTournament);
            // 
            // button23
            // 
            this.button23.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button23.BackColor = System.Drawing.Color.LightSlateGray;
            this.button23.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button23.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button23.Location = new System.Drawing.Point(249, 396);
            this.button23.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(441, 39);
            this.button23.TabIndex = 94;
            this.button23.Text = "REFRESH";
            this.button23.UseVisualStyleBackColor = true;
            this.button23.Click += new System.EventHandler(this.RefreshAllTournaments);
            // 
            // button54
            // 
            this.button54.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button54.ForeColor = System.Drawing.Color.Black;
            this.button54.Location = new System.Drawing.Point(1133, 190);
            this.button54.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button54.Name = "button54";
            this.button54.Size = new System.Drawing.Size(159, 39);
            this.button54.TabIndex = 93;
            this.button54.Text = "MONTHLY PK";
            this.button54.UseVisualStyleBackColor = true;
            this.button54.Click += new System.EventHandler(this.StartMonthlyPk);
            // 
            // button48
            // 
            this.button48.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button48.ForeColor = System.Drawing.Color.Black;
            this.button48.Location = new System.Drawing.Point(1133, 370);
            this.button48.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button48.Name = "button48";
            this.button48.Size = new System.Drawing.Size(159, 38);
            this.button48.TabIndex = 91;
            this.button48.Text = "WEEKLY PK TOURNAMENT";
            this.button48.UseVisualStyleBackColor = true;
            this.button48.Click += new System.EventHandler(this.StartWeeklyPK);
            // 
            // button36
            // 
            this.button36.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button36.BackColor = System.Drawing.Color.Maroon;
            this.button36.ForeColor = System.Drawing.Color.Black;
            this.button36.Location = new System.Drawing.Point(704, 155);
            this.button36.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button36.Name = "button36";
            this.button36.Size = new System.Drawing.Size(159, 39);
            this.button36.TabIndex = 79;
            this.button36.Text = "LMSDONTWORK";
            this.button36.UseVisualStyleBackColor = false;
            this.button36.Click += new System.EventHandler(this.StartFootBall);
            // 
            // button44
            // 
            this.button44.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button44.ForeColor = System.Drawing.Color.Black;
            this.button44.Location = new System.Drawing.Point(938, 280);
            this.button44.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button44.Name = "button44";
            this.button44.Size = new System.Drawing.Size(171, 39);
            this.button44.TabIndex = 87;
            this.button44.Text = "CAPTURE THE FLAG";
            this.button44.UseVisualStyleBackColor = true;
            this.button44.Click += new System.EventHandler(this.StartCaptureTheFlag);
            // 
            // button43
            // 
            this.button43.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button43.ForeColor = System.Drawing.Color.Black;
            this.button43.Location = new System.Drawing.Point(938, 235);
            this.button43.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button43.Name = "button43";
            this.button43.Size = new System.Drawing.Size(171, 39);
            this.button43.TabIndex = 86;
            this.button43.Text = "GUILD WAR";
            this.button43.UseVisualStyleBackColor = true;
            this.button43.Click += new System.EventHandler(this.StartGuildWar);
            // 
            // button42
            // 
            this.button42.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button42.ForeColor = System.Drawing.Color.Black;
            this.button42.Location = new System.Drawing.Point(938, 190);
            this.button42.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button42.Name = "button42";
            this.button42.Size = new System.Drawing.Size(171, 39);
            this.button42.TabIndex = 85;
            this.button42.Text = "ELITE GUILD WAR";
            this.button42.UseVisualStyleBackColor = true;
            this.button42.Click += new System.EventHandler(this.StartEliteGuildWar);
            // 
            // button41
            // 
            this.button41.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button41.ForeColor = System.Drawing.Color.Black;
            this.button41.Location = new System.Drawing.Point(938, 325);
            this.button41.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button41.Name = "button41";
            this.button41.Size = new System.Drawing.Size(171, 39);
            this.button41.TabIndex = 84;
            this.button41.Text = "CLAN PK WAR";
            this.button41.UseVisualStyleBackColor = true;
            this.button41.Click += new System.EventHandler(this.StartClanWar);
            // 
            // button40
            // 
            this.button40.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button40.ForeColor = System.Drawing.Color.Black;
            this.button40.Location = new System.Drawing.Point(1133, 145);
            this.button40.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button40.Name = "button40";
            this.button40.Size = new System.Drawing.Size(159, 39);
            this.button40.TabIndex = 83;
            this.button40.Text = "CLASS PK TOURNAMENT";
            this.button40.UseVisualStyleBackColor = true;
            this.button40.Click += new System.EventHandler(this.StartClassPkWar);
            // 
            // button39
            // 
            this.button39.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button39.BackColor = System.Drawing.Color.Maroon;
            this.button39.ForeColor = System.Drawing.Color.Black;
            this.button39.Location = new System.Drawing.Point(704, 112);
            this.button39.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button39.Name = "button39";
            this.button39.Size = new System.Drawing.Size(159, 39);
            this.button39.TabIndex = 82;
            this.button39.Text = "TREASURE THIEF";
            this.button39.UseVisualStyleBackColor = false;
            this.button39.Click += new System.EventHandler(this.StartTreasureThief);
            // 
            // button309
            // 
            this.button309.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button309.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button309.ForeColor = System.Drawing.Color.DarkGreen;
            this.button309.Location = new System.Drawing.Point(938, 146);
            this.button309.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button309.Name = "button309";
            this.button309.Size = new System.Drawing.Size(171, 38);
            this.button309.TabIndex = 82;
            this.button309.Text = "TREASURE TC";
            this.button309.UseVisualStyleBackColor = false;
            this.button309.Click += new System.EventHandler(this.StartFindTheBox);
            // 
            // button38
            // 
            this.button38.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button38.BackColor = System.Drawing.Color.Maroon;
            this.button38.ForeColor = System.Drawing.Color.Black;
            this.button38.Location = new System.Drawing.Point(704, 67);
            this.button38.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button38.Name = "button38";
            this.button38.Size = new System.Drawing.Size(159, 39);
            this.button38.TabIndex = 81;
            this.button38.Text = "FB/SSNEEDSFIXED";
            this.button38.UseVisualStyleBackColor = false;
            this.button38.Click += new System.EventHandler(this.StartSkillTournament);
            // 
            // button37
            // 
            this.button37.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button37.ForeColor = System.Drawing.Color.Black;
            this.button37.Location = new System.Drawing.Point(1133, 280);
            this.button37.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button37.Name = "button37";
            this.button37.Size = new System.Drawing.Size(159, 39);
            this.button37.TabIndex = 80;
            this.button37.Text = "SKILL PK TORNAMENT";
            this.button37.UseVisualStyleBackColor = true;
            this.button37.Click += new System.EventHandler(this.StartSkillTeamPkTournament);
            // 
            // button34
            // 
            this.button34.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button34.ForeColor = System.Drawing.Color.Black;
            this.button34.Location = new System.Drawing.Point(1133, 325);
            this.button34.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button34.Name = "button34";
            this.button34.Size = new System.Drawing.Size(159, 39);
            this.button34.TabIndex = 77;
            this.button34.Text = "TEAM PK TOURNAMENT";
            this.button34.UseVisualStyleBackColor = true;
            this.button34.Click += new System.EventHandler(this.StartTeamPkTournament);
            // 
            // button33
            // 
            this.button33.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button33.ForeColor = System.Drawing.Color.Black;
            this.button33.Location = new System.Drawing.Point(1133, 235);
            this.button33.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button33.Name = "button33";
            this.button33.Size = new System.Drawing.Size(159, 39);
            this.button33.TabIndex = 76;
            this.button33.Text = "ELITE PK TOURNAMENT";
            this.button33.UseVisualStyleBackColor = true;
            this.button33.Click += new System.EventHandler(this.StartElitePkTournament);
            // 
            // button31
            // 
            this.button31.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button31.BackColor = System.Drawing.Color.Black;
            this.button31.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button31.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button31.Location = new System.Drawing.Point(255, 364);
            this.button31.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(149, 31);
            this.button31.TabIndex = 68;
            this.button31.Text = "STOP TOURNAMENT";
            this.button31.UseVisualStyleBackColor = false;
            this.button31.Click += new System.EventHandler(this.FinishTournament);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 17;
            this.listBox1.Location = new System.Drawing.Point(139, 112);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(441, 157);
            this.listBox1.TabIndex = 66;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged_1);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage4.Controls.Add(this.pictureBox4);
            this.tabPage4.Controls.Add(this.comboBox7);
            this.tabPage4.Controls.Add(this.button32);
            this.tabPage4.Controls.Add(this.textBox8);
            this.tabPage4.Controls.Add(this.button5);
            this.tabPage4.Controls.Add(this.button2);
            this.tabPage4.Controls.Add(this.comboBox1);
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Controls.Add(this.label26);
            this.tabPage4.Controls.Add(this.textBox1);
            this.tabPage4.Controls.Add(this.RecList);
            this.tabPage4.Controls.Add(this.button4);
            this.tabPage4.Controls.Add(this.Send);
            this.tabPage4.Controls.Add(this.SendText);
            this.tabPage4.Controls.Add(this.ClientList);
            this.tabPage4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1566, 705);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Chat System";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.ErrorImage = global::TheChosenProject.Properties.Resources._0;
            this.pictureBox4.Image = global::TheChosenProject.Properties.Resources._0;
            this.pictureBox4.InitialImage = global::TheChosenProject.Properties.Resources._296;
            this.pictureBox4.Location = new System.Drawing.Point(1244, 164);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(64, 64);
            this.pictureBox4.TabIndex = 78;
            this.pictureBox4.TabStop = false;
            // 
            // comboBox7
            // 
            this.comboBox7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBox7.FormattingEnabled = true;
            this.comboBox7.Items.AddRange(new object[] {
            "Broadcast",
            "World",
            "Discord"});
            this.comboBox7.Location = new System.Drawing.Point(882, 545);
            this.comboBox7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox7.Name = "comboBox7";
            this.comboBox7.Size = new System.Drawing.Size(198, 25);
            this.comboBox7.TabIndex = 26;
            this.comboBox7.Text = "message id";
            // 
            // button32
            // 
            this.button32.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button32.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button32.Location = new System.Drawing.Point(1088, 544);
            this.button32.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(143, 25);
            this.button32.TabIndex = 25;
            this.button32.Text = "Send";
            this.button32.UseVisualStyleBackColor = true;
            this.button32.Click += new System.EventHandler(this.ChatSystemSendMessage);
            // 
            // textBox8
            // 
            this.textBox8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox8.Location = new System.Drawing.Point(253, 545);
            this.textBox8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox8.Multiline = true;
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(621, 25);
            this.textBox8.TabIndex = 24;
            this.textBox8.Text = "write here your message to send  (Broadcast - world and discord)";
            // 
            // button5
            // 
            this.button5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button5.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button5.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button5.Location = new System.Drawing.Point(253, 575);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(479, 33);
            this.button5.TabIndex = 23;
            this.button5.Text = "END CHAT";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.RemoveUserFromChat);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button2.Location = new System.Drawing.Point(753, 575);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(479, 33);
            this.button2.TabIndex = 22;
            this.button2.Text = "START CHAT";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.StartChatPanel);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "WelcomeMessage1",
            "WelcomeMessage2"});
            this.comboBox1.Location = new System.Drawing.Point(879, 513);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(353, 25);
            this.comboBox1.TabIndex = 21;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button3.Location = new System.Drawing.Point(1233, 234);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(85, 33);
            this.button3.TabIndex = 20;
            this.button3.Text = "Remove";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.RemoveFromChat);
            // 
            // label26
            // 
            this.label26.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label26.AutoSize = true;
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(247, 517);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(36, 17);
            this.label26.TabIndex = 19;
            this.label26.Text = "Copy";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox1.Location = new System.Drawing.Point(286, 513);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(588, 25);
            this.textBox1.TabIndex = 18;
            // 
            // RecList
            // 
            this.RecList.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RecList.FormattingEnabled = true;
            this.RecList.ItemHeight = 17;
            this.RecList.Location = new System.Drawing.Point(251, 126);
            this.RecList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.RecList.Name = "RecList";
            this.RecList.Size = new System.Drawing.Size(774, 276);
            this.RecList.TabIndex = 16;
            this.RecList.DoubleClick += new System.EventHandler(this.RecList_MouseClick);
            // 
            // button4
            // 
            this.button4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button4.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button4.Location = new System.Drawing.Point(1233, 126);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(85, 33);
            this.button4.TabIndex = 15;
            this.button4.Text = "Messages";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.ViewMessageslist);
            // 
            // Send
            // 
            this.Send.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Send.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Send.Location = new System.Drawing.Point(955, 453);
            this.Send.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(70, 55);
            this.Send.TabIndex = 14;
            this.Send.Text = "Send";
            this.Send.UseVisualStyleBackColor = true;
            this.Send.Click += new System.EventHandler(this.SendMessageToUser);
            // 
            // SendText
            // 
            this.SendText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SendText.Location = new System.Drawing.Point(251, 453);
            this.SendText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SendText.Multiline = true;
            this.SendText.Name = "SendText";
            this.SendText.Size = new System.Drawing.Size(697, 55);
            this.SendText.TabIndex = 13;
            // 
            // ClientList
            // 
            this.ClientList.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ClientList.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ClientList.FormattingEnabled = true;
            this.ClientList.ItemHeight = 17;
            this.ClientList.Location = new System.Drawing.Point(1029, 126);
            this.ClientList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ClientList.Name = "ClientList";
            this.ClientList.Size = new System.Drawing.Size(202, 361);
            this.ClientList.TabIndex = 12;
            this.ClientList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.ClientList.DoubleClick += new System.EventHandler(this.ClientList_MouseClick);
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage5.Controls.Add(this.label160);
            this.tabPage5.Controls.Add(this.textBox78);
            this.tabPage5.Controls.Add(this.groupBox7);
            this.tabPage5.Controls.Add(this.groupBox6);
            this.tabPage5.Controls.Add(this.button20);
            this.tabPage5.Controls.Add(this.button21);
            this.tabPage5.Controls.Add(this.button22);
            this.tabPage5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage5.Size = new System.Drawing.Size(1566, 705);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Loader Protection";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label160
            // 
            this.label160.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label160.AutoSize = true;
            this.label160.BackColor = System.Drawing.Color.Teal;
            this.label160.Font = new System.Drawing.Font("Tahoma", 20.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label160.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label160.Location = new System.Drawing.Point(659, 317);
            this.label160.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label160.Name = "label160";
            this.label160.Size = new System.Drawing.Size(230, 34);
            this.label160.TabIndex = 65610;
            this.label160.Text = "CHEATERS LOG";
            // 
            // textBox78
            // 
            this.textBox78.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox78.BackColor = System.Drawing.Color.Teal;
            this.textBox78.Location = new System.Drawing.Point(261, 391);
            this.textBox78.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox78.MaxLength = 1000000;
            this.textBox78.Multiline = true;
            this.textBox78.Name = "textBox78";
            this.textBox78.ReadOnly = true;
            this.textBox78.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox78.Size = new System.Drawing.Size(1042, 205);
            this.textBox78.TabIndex = 65541;
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox7.BackColor = System.Drawing.Color.Transparent;
            this.groupBox7.Controls.Add(this.button24);
            this.groupBox7.Controls.Add(this.comboBox6);
            this.groupBox7.Controls.Add(this.button18);
            this.groupBox7.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox7.ForeColor = System.Drawing.Color.Black;
            this.groupBox7.Location = new System.Drawing.Point(924, 118);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox7.Size = new System.Drawing.Size(379, 267);
            this.groupBox7.TabIndex = 59;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Banned PC list";
            this.groupBox7.UseCompatibleTextRendering = true;
            // 
            // button24
            // 
            this.button24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button24.Location = new System.Drawing.Point(10, 71);
            this.button24.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(159, 29);
            this.button24.TabIndex = 61;
            this.button24.Text = "Remove PC";
            this.button24.UseVisualStyleBackColor = true;
            this.button24.Click += new System.EventHandler(this.RemoveBannedPC);
            // 
            // comboBox6
            // 
            this.comboBox6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox6.FormattingEnabled = true;
            this.comboBox6.Location = new System.Drawing.Point(10, 36);
            this.comboBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox6.Name = "comboBox6";
            this.comboBox6.Size = new System.Drawing.Size(361, 25);
            this.comboBox6.TabIndex = 42;
            // 
            // button18
            // 
            this.button18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button18.Location = new System.Drawing.Point(198, 70);
            this.button18.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(159, 31);
            this.button18.TabIndex = 15;
            this.button18.Text = "Banned PC";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.AddBannedPC);
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox6.BackColor = System.Drawing.Color.Transparent;
            this.groupBox6.Controls.Add(this.textBox7);
            this.groupBox6.Controls.Add(this.label27);
            this.groupBox6.Controls.Add(this.button25);
            this.groupBox6.Controls.Add(this.textBox2);
            this.groupBox6.Controls.Add(this.textBox5);
            this.groupBox6.Controls.Add(this.textBox4);
            this.groupBox6.Controls.Add(this.button19);
            this.groupBox6.Controls.Add(this.comboBox5);
            this.groupBox6.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox6.ForeColor = System.Drawing.Color.Black;
            this.groupBox6.Location = new System.Drawing.Point(261, 118);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox6.Size = new System.Drawing.Size(366, 267);
            this.groupBox6.TabIndex = 58;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Banned Accounts list";
            this.groupBox6.UseCompatibleTextRendering = true;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(10, 139);
            this.textBox7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(348, 25);
            this.textBox7.TabIndex = 58;
            this.textBox7.Text = "Reason";
            // 
            // label27
            // 
            this.label27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Italic);
            this.label27.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.label27.Location = new System.Drawing.Point(8, 170);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(91, 23);
            this.label27.TabIndex = 17;
            this.label27.Text = "Add Hours:";
            // 
            // button25
            // 
            this.button25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button25.Location = new System.Drawing.Point(8, 222);
            this.button25.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(158, 31);
            this.button25.TabIndex = 57;
            this.button25.Text = "Remove Account";
            this.button25.UseVisualStyleBackColor = true;
            this.button25.Click += new System.EventHandler(this.RemoveBannedAccount);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.textBox2.Location = new System.Drawing.Point(127, 170);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox2.MaxLength = 3;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(231, 25);
            this.textBox2.TabIndex = 16;
            this.textBox2.Text = "1";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(10, 75);
            this.textBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(348, 25);
            this.textBox5.TabIndex = 56;
            this.textBox5.Text = "PlayerName";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(10, 107);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(348, 25);
            this.textBox4.TabIndex = 55;
            this.textBox4.Text = "UID";
            // 
            // button19
            // 
            this.button19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button19.Location = new System.Drawing.Point(200, 222);
            this.button19.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(158, 31);
            this.button19.TabIndex = 14;
            this.button19.Text = "Banned Account";
            this.button19.UseVisualStyleBackColor = true;
            this.button19.Click += new System.EventHandler(this.AddBannedAccount);
            // 
            // comboBox5
            // 
            this.comboBox5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Location = new System.Drawing.Point(10, 36);
            this.comboBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(348, 25);
            this.comboBox5.TabIndex = 42;
            this.comboBox5.SelectedIndexChanged += new System.EventHandler(this.GetPlayerBanned);
            // 
            // button20
            // 
            this.button20.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button20.BackColor = System.Drawing.Color.Teal;
            this.button20.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button20.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button20.Location = new System.Drawing.Point(635, 240);
            this.button20.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(281, 42);
            this.button20.TabIndex = 12;
            this.button20.Text = "Send Close Client";
            this.button20.UseVisualStyleBackColor = false;
            this.button20.Click += new System.EventHandler(this.SendCloseClient);
            // 
            // button21
            // 
            this.button21.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button21.BackColor = System.Drawing.Color.Teal;
            this.button21.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button21.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button21.Location = new System.Drawing.Point(635, 192);
            this.button21.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(281, 42);
            this.button21.TabIndex = 13;
            this.button21.Text = "Send Scan Files";
            this.button21.UseVisualStyleBackColor = false;
            this.button21.Click += new System.EventHandler(this.SendScanFiles);
            // 
            // button22
            // 
            this.button22.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button22.BackColor = System.Drawing.Color.Teal;
            this.button22.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button22.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button22.Location = new System.Drawing.Point(635, 146);
            this.button22.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(281, 40);
            this.button22.TabIndex = 11;
            this.button22.Text = "Send Check Processes";
            this.button22.UseVisualStyleBackColor = false;
            this.button22.Click += new System.EventHandler(this.button22_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage6.Controls.Add(this.button56);
            this.tabPage6.Controls.Add(this.listBox2);
            this.tabPage6.Controls.Add(this.button50);
            this.tabPage6.Controls.Add(this.label25);
            this.tabPage6.Controls.Add(this.groupBox13);
            this.tabPage6.Controls.Add(this.label89);
            this.tabPage6.Controls.Add(this.groupBox12);
            this.tabPage6.Controls.Add(this.groupBox11);
            this.tabPage6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic);
            this.tabPage6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage6.Size = new System.Drawing.Size(1566, 705);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "AI-Bot Management";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // button56
            // 
            this.button56.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button56.BackColor = System.Drawing.Color.White;
            this.button56.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button56.Location = new System.Drawing.Point(1002, 567);
            this.button56.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button56.Name = "button56";
            this.button56.Size = new System.Drawing.Size(33, 27);
            this.button56.TabIndex = 65553;
            this.button56.Text = "R";
            this.button56.UseVisualStyleBackColor = true;
            this.button56.Click += new System.EventHandler(this.RefreshBotAlives);
            // 
            // listBox2
            // 
            this.listBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 17;
            this.listBox2.Location = new System.Drawing.Point(884, 135);
            this.listBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(151, 395);
            this.listBox2.TabIndex = 65548;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.SelectedBotIndex);
            // 
            // button50
            // 
            this.button50.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button50.BackColor = System.Drawing.Color.White;
            this.button50.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button50.Location = new System.Drawing.Point(884, 567);
            this.button50.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button50.Name = "button50";
            this.button50.Size = new System.Drawing.Size(109, 27);
            this.button50.TabIndex = 65547;
            this.button50.Text = "Remove";
            this.button50.UseVisualStyleBackColor = true;
            this.button50.Click += new System.EventHandler(this.KickoutBot);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.DarkOrange;
            this.label25.Location = new System.Drawing.Point(627, 86);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(118, 21);
            this.label25.TabIndex = 65540;
            this.label25.Text = "Bot Equipments";
            // 
            // groupBox13
            // 
            this.groupBox13.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox13.BackColor = System.Drawing.Color.Transparent;
            this.groupBox13.Controls.Add(this.checkBox11);
            this.groupBox13.Controls.Add(this.GemsChecked);
            this.groupBox13.Controls.Add(this.RebornBox);
            this.groupBox13.Controls.Add(this.comboBox15);
            this.groupBox13.Controls.Add(this.ReqLev120);
            this.groupBox13.Controls.Add(this.ReqLev140);
            this.groupBox13.Controls.Add(this.ReqLev100);
            this.groupBox13.Controls.Add(this.ReqLev70);
            this.groupBox13.Controls.Add(this.comboBox13);
            this.groupBox13.Controls.Add(this.checkBox10);
            this.groupBox13.Controls.Add(this.comboBox12);
            this.groupBox13.Controls.Add(this.checkBox4);
            this.groupBox13.Controls.Add(this.comboBox11);
            this.groupBox13.Controls.Add(this.button47);
            this.groupBox13.Controls.Add(this.label45);
            this.groupBox13.Controls.Add(this.label44);
            this.groupBox13.Controls.Add(this.label43);
            this.groupBox13.Controls.Add(this.textBox28);
            this.groupBox13.Controls.Add(this.label42);
            this.groupBox13.Controls.Add(this.label41);
            this.groupBox13.Controls.Add(this.textBox26);
            this.groupBox13.Controls.Add(this.label39);
            this.groupBox13.Controls.Add(this.label38);
            this.groupBox13.Controls.Add(this.label37);
            this.groupBox13.Controls.Add(this.label33);
            this.groupBox13.Controls.Add(this.textBox24);
            this.groupBox13.Controls.Add(this.textBox19);
            this.groupBox13.Controls.Add(this.textBox22);
            this.groupBox13.Controls.Add(this.label24);
            this.groupBox13.Controls.Add(this.textBox20);
            this.groupBox13.Controls.Add(this.textBox21);
            this.groupBox13.ForeColor = System.Drawing.Color.Black;
            this.groupBox13.Location = new System.Drawing.Point(253, 106);
            this.groupBox13.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox13.Size = new System.Drawing.Size(316, 486);
            this.groupBox13.TabIndex = 65543;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Add bot";
            // 
            // checkBox11
            // 
            this.checkBox11.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox11.AutoSize = true;
            this.checkBox11.Location = new System.Drawing.Point(176, 405);
            this.checkBox11.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(109, 21);
            this.checkBox11.TabIndex = 65561;
            this.checkBox11.Text = "Set Target(Bot)";
            this.checkBox11.UseVisualStyleBackColor = true;
            // 
            // GemsChecked
            // 
            this.GemsChecked.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GemsChecked.AutoSize = true;
            this.GemsChecked.Location = new System.Drawing.Point(13, 405);
            this.GemsChecked.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.GemsChecked.Name = "GemsChecked";
            this.GemsChecked.Size = new System.Drawing.Size(109, 21);
            this.GemsChecked.TabIndex = 65560;
            this.GemsChecked.Text = "Set SuperGems";
            this.GemsChecked.UseVisualStyleBackColor = true;
            // 
            // RebornBox
            // 
            this.RebornBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.RebornBox.FormattingEnabled = true;
            this.RebornBox.Items.AddRange(new object[] {
            "1stReborn",
            "2ndReborn",
            "Newbie"});
            this.RebornBox.Location = new System.Drawing.Point(82, 285);
            this.RebornBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.RebornBox.Name = "RebornBox";
            this.RebornBox.Size = new System.Drawing.Size(226, 25);
            this.RebornBox.TabIndex = 65559;
            // 
            // comboBox15
            // 
            this.comboBox15.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox15.FormattingEnabled = true;
            this.comboBox15.Items.AddRange(new object[] {
            "Trojan",
            "Warrior",
            "Archer",
            "Ninja",
            "Monk",
            "Water",
            "Fire"});
            this.comboBox15.Location = new System.Drawing.Point(81, 316);
            this.comboBox15.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox15.Name = "comboBox15";
            this.comboBox15.Size = new System.Drawing.Size(227, 25);
            this.comboBox15.TabIndex = 65558;
            // 
            // ReqLev120
            // 
            this.ReqLev120.AutoSize = true;
            this.ReqLev120.Location = new System.Drawing.Point(14, 378);
            this.ReqLev120.Name = "ReqLev120";
            this.ReqLev120.Size = new System.Drawing.Size(130, 21);
            this.ReqLev120.TabIndex = 65557;
            this.ReqLev120.TabStop = true;
            this.ReqLev120.Text = "Lev.120 Equipment";
            this.ReqLev120.UseVisualStyleBackColor = true;
            this.ReqLev120.CheckedChanged += new System.EventHandler(this.ReqLev120_CheckedChanged);
            // 
            // ReqLev140
            // 
            this.ReqLev140.AutoSize = true;
            this.ReqLev140.Location = new System.Drawing.Point(176, 378);
            this.ReqLev140.Name = "ReqLev140";
            this.ReqLev140.Size = new System.Drawing.Size(130, 21);
            this.ReqLev140.TabIndex = 65556;
            this.ReqLev140.TabStop = true;
            this.ReqLev140.Text = "Lev.140 Equipment";
            this.ReqLev140.UseVisualStyleBackColor = true;
            this.ReqLev140.CheckedChanged += new System.EventHandler(this.ReqLev140_CheckedChanged);
            // 
            // ReqLev100
            // 
            this.ReqLev100.AutoSize = true;
            this.ReqLev100.Location = new System.Drawing.Point(176, 354);
            this.ReqLev100.Name = "ReqLev100";
            this.ReqLev100.Size = new System.Drawing.Size(130, 21);
            this.ReqLev100.TabIndex = 65555;
            this.ReqLev100.TabStop = true;
            this.ReqLev100.Text = "Lev.100 Equipment";
            this.ReqLev100.UseVisualStyleBackColor = true;
            this.ReqLev100.CheckedChanged += new System.EventHandler(this.ReqLev100_CheckedChanged);
            // 
            // ReqLev70
            // 
            this.ReqLev70.AutoSize = true;
            this.ReqLev70.Location = new System.Drawing.Point(14, 354);
            this.ReqLev70.Name = "ReqLev70";
            this.ReqLev70.Size = new System.Drawing.Size(123, 21);
            this.ReqLev70.TabIndex = 65554;
            this.ReqLev70.TabStop = true;
            this.ReqLev70.Text = "Lev.70 Equipment";
            this.ReqLev70.UseVisualStyleBackColor = true;
            this.ReqLev70.CheckedChanged += new System.EventHandler(this.ReqLev70_CheckedChanged);
            // 
            // comboBox13
            // 
            this.comboBox13.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox13.FormattingEnabled = true;
            this.comboBox13.Items.AddRange(new object[] {
            "AgileMale",
            "MuscularMale",
            "AgileFemale",
            "MuscularFemale"});
            this.comboBox13.Location = new System.Drawing.Point(81, 160);
            this.comboBox13.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox13.Name = "comboBox13";
            this.comboBox13.Size = new System.Drawing.Size(226, 25);
            this.comboBox13.TabIndex = 65553;
            // 
            // checkBox10
            // 
            this.checkBox10.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox10.AutoSize = true;
            this.checkBox10.Location = new System.Drawing.Point(13, 427);
            this.checkBox10.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(161, 21);
            this.checkBox10.TabIndex = 65552;
            this.checkBox10.Text = "Create Team(AutoInvite)";
            this.checkBox10.UseVisualStyleBackColor = true;
            // 
            // comboBox12
            // 
            this.comboBox12.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox12.FormattingEnabled = true;
            this.comboBox12.Items.AddRange(new object[] {
            "King",
            "Prince",
            "Duke",
            "Earl",
            "Serf"});
            this.comboBox12.Location = new System.Drawing.Point(81, 222);
            this.comboBox12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox12.Name = "comboBox12";
            this.comboBox12.Size = new System.Drawing.Size(226, 25);
            this.comboBox12.TabIndex = 65551;
            // 
            // checkBox4
            // 
            this.checkBox4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(176, 427);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(136, 21);
            this.checkBox4.TabIndex = 65550;
            this.checkBox4.Text = "Flag Effect(Hunting)";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // comboBox11
            // 
            this.comboBox11.AutoCompleteCustomSource.AddRange(new string[] {
            "Hunting",
            "Training"});
            this.comboBox11.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox11.FormattingEnabled = true;
            this.comboBox11.Items.AddRange(new object[] {
            "Hunting",
            "Training",
            "PK-Fighting",
            "Buffers"});
            this.comboBox11.Location = new System.Drawing.Point(81, 90);
            this.comboBox11.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox11.Name = "comboBox11";
            this.comboBox11.Size = new System.Drawing.Size(226, 25);
            this.comboBox11.TabIndex = 65549;
            // 
            // button47
            // 
            this.button47.BackColor = System.Drawing.Color.White;
            this.button47.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button47.Location = new System.Drawing.Point(8, 451);
            this.button47.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button47.Name = "button47";
            this.button47.Size = new System.Drawing.Size(300, 29);
            this.button47.TabIndex = 65548;
            this.button47.Text = "Add Bot";
            this.button47.UseVisualStyleBackColor = true;
            this.button47.Click += new System.EventHandler(this.AddingBot);
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(8, 319);
            this.label45.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(36, 17);
            this.label45.TabIndex = 83;
            this.label45.Text = "Class";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(8, 288);
            this.label44.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(51, 17);
            this.label44.TabIndex = 81;
            this.label44.Text = "Reborn:";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(8, 257);
            this.label43.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(33, 17);
            this.label43.TabIndex = 79;
            this.label43.Text = "Face";
            // 
            // textBox28
            // 
            this.textBox28.Location = new System.Drawing.Point(81, 254);
            this.textBox28.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox28.Name = "textBox28";
            this.textBox28.Size = new System.Drawing.Size(227, 25);
            this.textBox28.TabIndex = 78;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(8, 226);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(39, 17);
            this.label42.TabIndex = 77;
            this.label42.Text = "Rank:";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(8, 194);
            this.label41.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(44, 17);
            this.label41.TabIndex = 75;
            this.label41.Text = "Hours:";
            // 
            // textBox26
            // 
            this.textBox26.Location = new System.Drawing.Point(81, 191);
            this.textBox26.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox26.Name = "textBox26";
            this.textBox26.Size = new System.Drawing.Size(227, 25);
            this.textBox26.TabIndex = 74;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(8, 158);
            this.label39.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(38, 17);
            this.label39.TabIndex = 71;
            this.label39.Text = "Body:";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(8, 124);
            this.label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(62, 17);
            this.label38.TabIndex = 70;
            this.label38.Text = "Map(X,Y):";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(8, 62);
            this.label37.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(38, 17);
            this.label37.TabIndex = 69;
            this.label37.Text = "Level:";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(8, 31);
            this.label33.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(45, 17);
            this.label33.TabIndex = 68;
            this.label33.Text = "Name:";
            // 
            // textBox24
            // 
            this.textBox24.Location = new System.Drawing.Point(247, 121);
            this.textBox24.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new System.Drawing.Size(60, 25);
            this.textBox24.TabIndex = 66;
            this.textBox24.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox19
            // 
            this.textBox19.Location = new System.Drawing.Point(81, 121);
            this.textBox19.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new System.Drawing.Size(88, 25);
            this.textBox19.TabIndex = 64;
            this.textBox19.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox22
            // 
            this.textBox22.Location = new System.Drawing.Point(176, 121);
            this.textBox22.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new System.Drawing.Size(63, 25);
            this.textBox22.TabIndex = 63;
            this.textBox22.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(8, 92);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(37, 17);
            this.label24.TabIndex = 62;
            this.label24.Text = "Type:";
            // 
            // textBox20
            // 
            this.textBox20.Location = new System.Drawing.Point(81, 26);
            this.textBox20.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new System.Drawing.Size(227, 25);
            this.textBox20.TabIndex = 60;
            // 
            // textBox21
            // 
            this.textBox21.Location = new System.Drawing.Point(81, 59);
            this.textBox21.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new System.Drawing.Size(227, 25);
            this.textBox21.TabIndex = 59;
            // 
            // label89
            // 
            this.label89.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label89.AutoSize = true;
            this.label89.Font = new System.Drawing.Font("Segoe UI", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Strikeout))));
            this.label89.Location = new System.Drawing.Point(921, 115);
            this.label89.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(72, 17);
            this.label89.TabIndex = 65538;
            this.label89.Text = "AI-Bot Alive";
            // 
            // groupBox12
            // 
            this.groupBox12.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox12.BackColor = System.Drawing.Color.Transparent;
            this.groupBox12.BackgroundImage = global::TheChosenProject.Properties.Resources.information;
            this.groupBox12.Controls.Add(this.TxtBotRidingCrop);
            this.groupBox12.Controls.Add(this.TxtBotStarTower);
            this.groupBox12.Controls.Add(this.TxtBotHeavenFan);
            this.groupBox12.Controls.Add(this.TxtBotBoots);
            this.groupBox12.Controls.Add(this.TxtBotRightWeapon);
            this.groupBox12.Controls.Add(this.TxtBotLeftWeapon);
            this.groupBox12.Controls.Add(this.TxtBotArmors);
            this.groupBox12.Controls.Add(this.textBox11);
            this.groupBox12.Controls.Add(this.textBox10);
            this.groupBox12.Controls.Add(this.textBox9);
            this.groupBox12.Controls.Add(this.label23);
            this.groupBox12.Controls.Add(this.label19);
            this.groupBox12.Controls.Add(this.numericUpDown17);
            this.groupBox12.Controls.Add(this.numericUpDown18);
            this.groupBox12.Controls.Add(this.numericUpDown19);
            this.groupBox12.Controls.Add(this.numericUpDown20);
            this.groupBox12.Controls.Add(this.numericUpDown16);
            this.groupBox12.Controls.Add(this.numericUpDown15);
            this.groupBox12.Controls.Add(this.numericUpDown14);
            this.groupBox12.Controls.Add(this.numericUpDown13);
            this.groupBox12.Controls.Add(this.numericUpDown12);
            this.groupBox12.Controls.Add(this.numericUpDown11);
            this.groupBox12.Controls.Add(this.numericUpDown10);
            this.groupBox12.Controls.Add(this.numericUpDown9);
            this.groupBox12.Controls.Add(this.numericUpDown8);
            this.groupBox12.Controls.Add(this.numericUpDown7);
            this.groupBox12.Controls.Add(this.numericUpDown6);
            this.groupBox12.Controls.Add(this.numericUpDown5);
            this.groupBox12.Controls.Add(this.numericUpDown4);
            this.groupBox12.Controls.Add(this.numericUpDown3);
            this.groupBox12.Controls.Add(this.numericUpDown2);
            this.groupBox12.Controls.Add(this.numericUpDown1);
            this.groupBox12.ForeColor = System.Drawing.Color.Black;
            this.groupBox12.Location = new System.Drawing.Point(577, 117);
            this.groupBox12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox12.Size = new System.Drawing.Size(294, 488);
            this.groupBox12.TabIndex = 80;
            this.groupBox12.TabStop = false;
            // 
            // TxtBotRidingCrop
            // 
            this.TxtBotRidingCrop.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBotRidingCrop.Location = new System.Drawing.Point(71, 409);
            this.TxtBotRidingCrop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtBotRidingCrop.MaxLength = 32;
            this.TxtBotRidingCrop.Name = "TxtBotRidingCrop";
            this.TxtBotRidingCrop.Size = new System.Drawing.Size(98, 23);
            this.TxtBotRidingCrop.TabIndex = 65580;
            // 
            // TxtBotStarTower
            // 
            this.TxtBotStarTower.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBotStarTower.Location = new System.Drawing.Point(72, 330);
            this.TxtBotStarTower.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtBotStarTower.MaxLength = 32;
            this.TxtBotStarTower.Name = "TxtBotStarTower";
            this.TxtBotStarTower.Size = new System.Drawing.Size(98, 23);
            this.TxtBotStarTower.TabIndex = 65579;
            // 
            // TxtBotHeavenFan
            // 
            this.TxtBotHeavenFan.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBotHeavenFan.Location = new System.Drawing.Point(71, 370);
            this.TxtBotHeavenFan.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtBotHeavenFan.MaxLength = 32;
            this.TxtBotHeavenFan.Name = "TxtBotHeavenFan";
            this.TxtBotHeavenFan.Size = new System.Drawing.Size(98, 23);
            this.TxtBotHeavenFan.TabIndex = 65578;
            // 
            // TxtBotBoots
            // 
            this.TxtBotBoots.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBotBoots.Location = new System.Drawing.Point(71, 290);
            this.TxtBotBoots.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtBotBoots.MaxLength = 32;
            this.TxtBotBoots.Name = "TxtBotBoots";
            this.TxtBotBoots.Size = new System.Drawing.Size(98, 23);
            this.TxtBotBoots.TabIndex = 65577;
            // 
            // TxtBotRightWeapon
            // 
            this.TxtBotRightWeapon.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBotRightWeapon.Location = new System.Drawing.Point(72, 210);
            this.TxtBotRightWeapon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtBotRightWeapon.MaxLength = 32;
            this.TxtBotRightWeapon.Name = "TxtBotRightWeapon";
            this.TxtBotRightWeapon.Size = new System.Drawing.Size(98, 23);
            this.TxtBotRightWeapon.TabIndex = 65576;
            // 
            // TxtBotLeftWeapon
            // 
            this.TxtBotLeftWeapon.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBotLeftWeapon.Location = new System.Drawing.Point(71, 250);
            this.TxtBotLeftWeapon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtBotLeftWeapon.MaxLength = 32;
            this.TxtBotLeftWeapon.Name = "TxtBotLeftWeapon";
            this.TxtBotLeftWeapon.Size = new System.Drawing.Size(98, 23);
            this.TxtBotLeftWeapon.TabIndex = 65575;
            // 
            // TxtBotArmors
            // 
            this.TxtBotArmors.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBotArmors.Location = new System.Drawing.Point(71, 170);
            this.TxtBotArmors.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TxtBotArmors.MaxLength = 32;
            this.TxtBotArmors.Name = "TxtBotArmors";
            this.TxtBotArmors.Size = new System.Drawing.Size(98, 23);
            this.TxtBotArmors.TabIndex = 65574;
            // 
            // textBox11
            // 
            this.textBox11.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox11.Location = new System.Drawing.Point(72, 90);
            this.textBox11.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox11.MaxLength = 32;
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(98, 23);
            this.textBox11.TabIndex = 65573;
            // 
            // textBox10
            // 
            this.textBox10.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox10.Location = new System.Drawing.Point(71, 130);
            this.textBox10.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox10.MaxLength = 32;
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(98, 23);
            this.textBox10.TabIndex = 65572;
            // 
            // textBox9
            // 
            this.textBox9.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox9.Location = new System.Drawing.Point(71, 50);
            this.textBox9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox9.MaxLength = 32;
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(98, 23);
            this.textBox9.TabIndex = 65571;
            // 
            // label23
            // 
            this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.label23.ForeColor = System.Drawing.Color.Gold;
            this.label23.Location = new System.Drawing.Point(227, 34);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(40, 13);
            this.label23.TabIndex = 65541;
            this.label23.Text = "(+)Plus";
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.label19.ForeColor = System.Drawing.Color.Gold;
            this.label19.Location = new System.Drawing.Point(180, 34);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(39, 13);
            this.label19.TabIndex = 65540;
            this.label19.Text = "(-)Bless";
            // 
            // numericUpDown17
            // 
            this.numericUpDown17.Font = new System.Drawing.Font("Segoe UI", 8.9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown17.Location = new System.Drawing.Point(231, 409);
            this.numericUpDown17.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown17.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown17.Name = "numericUpDown17";
            this.numericUpDown17.Size = new System.Drawing.Size(34, 23);
            this.numericUpDown17.TabIndex = 65570;
            this.numericUpDown17.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown17.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown18
            // 
            this.numericUpDown18.Font = new System.Drawing.Font("Segoe UI", 8.9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown18.Location = new System.Drawing.Point(231, 370);
            this.numericUpDown18.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown18.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown18.Name = "numericUpDown18";
            this.numericUpDown18.Size = new System.Drawing.Size(34, 23);
            this.numericUpDown18.TabIndex = 65569;
            this.numericUpDown18.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown18.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown19
            // 
            this.numericUpDown19.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDown19.Font = new System.Drawing.Font("Segoe UI", 8.9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown19.Location = new System.Drawing.Point(231, 329);
            this.numericUpDown19.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown19.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown19.Name = "numericUpDown19";
            this.numericUpDown19.Size = new System.Drawing.Size(34, 23);
            this.numericUpDown19.TabIndex = 65568;
            this.numericUpDown19.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown19.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown20
            // 
            this.numericUpDown20.Font = new System.Drawing.Font("Segoe UI", 8.9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown20.Location = new System.Drawing.Point(231, 290);
            this.numericUpDown20.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown20.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown20.Name = "numericUpDown20";
            this.numericUpDown20.Size = new System.Drawing.Size(34, 23);
            this.numericUpDown20.TabIndex = 65567;
            this.numericUpDown20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown20.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown16
            // 
            this.numericUpDown16.Font = new System.Drawing.Font("Segoe UI", 8.9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown16.Location = new System.Drawing.Point(231, 250);
            this.numericUpDown16.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown16.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown16.Name = "numericUpDown16";
            this.numericUpDown16.Size = new System.Drawing.Size(34, 23);
            this.numericUpDown16.TabIndex = 65566;
            this.numericUpDown16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown16.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown15
            // 
            this.numericUpDown15.Font = new System.Drawing.Font("Segoe UI", 8.9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown15.Location = new System.Drawing.Point(231, 211);
            this.numericUpDown15.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown15.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown15.Name = "numericUpDown15";
            this.numericUpDown15.Size = new System.Drawing.Size(34, 23);
            this.numericUpDown15.TabIndex = 65565;
            this.numericUpDown15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown15.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown14
            // 
            this.numericUpDown14.Font = new System.Drawing.Font("Segoe UI", 8.9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown14.Location = new System.Drawing.Point(231, 170);
            this.numericUpDown14.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown14.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown14.Name = "numericUpDown14";
            this.numericUpDown14.Size = new System.Drawing.Size(34, 23);
            this.numericUpDown14.TabIndex = 65564;
            this.numericUpDown14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown14.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown13
            // 
            this.numericUpDown13.Font = new System.Drawing.Font("Segoe UI", 8.9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown13.Location = new System.Drawing.Point(231, 130);
            this.numericUpDown13.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown13.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown13.Name = "numericUpDown13";
            this.numericUpDown13.Size = new System.Drawing.Size(34, 23);
            this.numericUpDown13.TabIndex = 65563;
            this.numericUpDown13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown13.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown12
            // 
            this.numericUpDown12.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Italic);
            this.numericUpDown12.Location = new System.Drawing.Point(231, 89);
            this.numericUpDown12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown12.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown12.Name = "numericUpDown12";
            this.numericUpDown12.Size = new System.Drawing.Size(34, 20);
            this.numericUpDown12.TabIndex = 65562;
            this.numericUpDown12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown12.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown11
            // 
            this.numericUpDown11.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.numericUpDown11.Location = new System.Drawing.Point(231, 50);
            this.numericUpDown11.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown11.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown11.Name = "numericUpDown11";
            this.numericUpDown11.Size = new System.Drawing.Size(34, 22);
            this.numericUpDown11.TabIndex = 65561;
            this.numericUpDown11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown11.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // numericUpDown10
            // 
            this.numericUpDown10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown10.Location = new System.Drawing.Point(178, 410);
            this.numericUpDown10.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown10.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown10.Name = "numericUpDown10";
            this.numericUpDown10.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown10.TabIndex = 65560;
            this.numericUpDown10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown10.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown9
            // 
            this.numericUpDown9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown9.Location = new System.Drawing.Point(178, 370);
            this.numericUpDown9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown9.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown9.Name = "numericUpDown9";
            this.numericUpDown9.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown9.TabIndex = 65559;
            this.numericUpDown9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown9.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown8
            // 
            this.numericUpDown8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown8.Location = new System.Drawing.Point(178, 330);
            this.numericUpDown8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown8.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown8.Name = "numericUpDown8";
            this.numericUpDown8.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown8.TabIndex = 65558;
            this.numericUpDown8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown8.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown7
            // 
            this.numericUpDown7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown7.Location = new System.Drawing.Point(178, 290);
            this.numericUpDown7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown7.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown7.Name = "numericUpDown7";
            this.numericUpDown7.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown7.TabIndex = 65557;
            this.numericUpDown7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown7.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown6
            // 
            this.numericUpDown6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown6.Location = new System.Drawing.Point(178, 250);
            this.numericUpDown6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown6.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown6.TabIndex = 65556;
            this.numericUpDown6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown6.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown5
            // 
            this.numericUpDown5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown5.Location = new System.Drawing.Point(178, 211);
            this.numericUpDown5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown5.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown5.TabIndex = 65555;
            this.numericUpDown5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown5.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown4.Location = new System.Drawing.Point(178, 170);
            this.numericUpDown4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown4.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown4.TabIndex = 65554;
            this.numericUpDown4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown4.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown3.Location = new System.Drawing.Point(178, 130);
            this.numericUpDown3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown3.TabIndex = 65553;
            this.numericUpDown3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown3.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown2.Location = new System.Drawing.Point(178, 90);
            this.numericUpDown2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown2.TabIndex = 65552;
            this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown2.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.SystemColors.Window;
            this.numericUpDown1.Cursor = System.Windows.Forms.Cursors.Default;
            this.numericUpDown1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.numericUpDown1.Location = new System.Drawing.Point(178, 50);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(45, 23);
            this.numericUpDown1.TabIndex = 65551;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // groupBox11
            // 
            this.groupBox11.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox11.BackColor = System.Drawing.Color.Transparent;
            this.groupBox11.BackgroundImage = global::TheChosenProject.Properties.Resources.equipment;
            this.groupBox11.Controls.Add(this.label46);
            this.groupBox11.Controls.Add(this.label32);
            this.groupBox11.Controls.Add(this.label77);
            this.groupBox11.Controls.Add(this.label87);
            this.groupBox11.Controls.Add(this.label86);
            this.groupBox11.Controls.Add(this.label85);
            this.groupBox11.Controls.Add(this.label84);
            this.groupBox11.Controls.Add(this.label83);
            this.groupBox11.Controls.Add(this.label82);
            this.groupBox11.Controls.Add(this.label81);
            this.groupBox11.Controls.Add(this.label80);
            this.groupBox11.Controls.Add(this.label79);
            this.groupBox11.Controls.Add(this.label76);
            this.groupBox11.Controls.Add(this.label75);
            this.groupBox11.Controls.Add(this.label74);
            this.groupBox11.Controls.Add(this.label70);
            this.groupBox11.Controls.Add(this.label71);
            this.groupBox11.Controls.Add(this.label72);
            this.groupBox11.Controls.Add(this.label67);
            this.groupBox11.Controls.Add(this.label68);
            this.groupBox11.Controls.Add(this.label69);
            this.groupBox11.Controls.Add(this.label66);
            this.groupBox11.Controls.Add(this.label65);
            this.groupBox11.Controls.Add(this.label64);
            this.groupBox11.Controls.Add(this.pictureBox2);
            this.groupBox11.Controls.Add(this.label63);
            this.groupBox11.Controls.Add(this.label62);
            this.groupBox11.Controls.Add(this.label61);
            this.groupBox11.ForeColor = System.Drawing.Color.Black;
            this.groupBox11.Location = new System.Drawing.Point(1047, 110);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox11.Size = new System.Drawing.Size(260, 490);
            this.groupBox11.TabIndex = 79;
            this.groupBox11.TabStop = false;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.label46.ForeColor = System.Drawing.Color.Gold;
            this.label46.Location = new System.Drawing.Point(152, 457);
            this.label46.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(13, 15);
            this.label46.TabIndex = 107;
            this.label46.Text = "0";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label32.ForeColor = System.Drawing.Color.Gold;
            this.label32.Location = new System.Drawing.Point(134, 407);
            this.label32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(15, 17);
            this.label32.TabIndex = 106;
            this.label32.Text = "0";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.label77.ForeColor = System.Drawing.Color.Gold;
            this.label77.Location = new System.Drawing.Point(37, 457);
            this.label77.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(62, 15);
            this.label77.TabIndex = 105;
            this.label77.Text = "Total Skills";
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label87.ForeColor = System.Drawing.Color.Gold;
            this.label87.Location = new System.Drawing.Point(156, 378);
            this.label87.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(15, 17);
            this.label87.TabIndex = 103;
            this.label87.Text = "0";
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label86.ForeColor = System.Drawing.Color.Gold;
            this.label86.Location = new System.Drawing.Point(156, 350);
            this.label86.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(15, 17);
            this.label86.TabIndex = 102;
            this.label86.Text = "0";
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label85.ForeColor = System.Drawing.Color.Gold;
            this.label85.Location = new System.Drawing.Point(156, 318);
            this.label85.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(27, 17);
            this.label85.TabIndex = 101;
            this.label85.Text = "0/0";
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label84.ForeColor = System.Drawing.Color.Gold;
            this.label84.Location = new System.Drawing.Point(156, 290);
            this.label84.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(27, 17);
            this.label84.TabIndex = 100;
            this.label84.Text = "0/0";
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label83.ForeColor = System.Drawing.Color.Gold;
            this.label83.Location = new System.Drawing.Point(156, 262);
            this.label83.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(40, 17);
            this.label83.TabIndex = 99;
            this.label83.Text = "None";
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label82.ForeColor = System.Drawing.Color.Gold;
            this.label82.Location = new System.Drawing.Point(156, 234);
            this.label82.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(15, 17);
            this.label82.TabIndex = 98;
            this.label82.Text = "0";
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label81.ForeColor = System.Drawing.Color.Gold;
            this.label81.Location = new System.Drawing.Point(156, 206);
            this.label81.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(15, 17);
            this.label81.TabIndex = 97;
            this.label81.Text = "0";
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label80.ForeColor = System.Drawing.Color.Gold;
            this.label80.Location = new System.Drawing.Point(156, 180);
            this.label80.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(40, 17);
            this.label80.TabIndex = 96;
            this.label80.Text = "None";
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.label79.ForeColor = System.Drawing.Color.Gold;
            this.label79.Location = new System.Drawing.Point(50, 44);
            this.label79.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(13, 15);
            this.label79.TabIndex = 95;
            this.label79.Text = "0";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label76.ForeColor = System.Drawing.Color.Gold;
            this.label76.Location = new System.Drawing.Point(44, 404);
            this.label76.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(58, 20);
            this.label76.TabIndex = 94;
            this.label76.Text = "Left On";
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Italic);
            this.label75.ForeColor = System.Drawing.Color.Gold;
            this.label75.Location = new System.Drawing.Point(112, 107);
            this.label75.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(37, 20);
            this.label75.TabIndex = 93;
            this.label75.Text = "Map";
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Italic);
            this.label74.ForeColor = System.Drawing.Color.Gold;
            this.label74.Location = new System.Drawing.Point(112, 127);
            this.label74.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(18, 20);
            this.label74.TabIndex = 92;
            this.label74.Text = "X";
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label70.ForeColor = System.Drawing.Color.Gold;
            this.label70.Location = new System.Drawing.Point(44, 346);
            this.label70.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(58, 20);
            this.label70.TabIndex = 90;
            this.label70.Text = "Reborn";
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label71.ForeColor = System.Drawing.Color.Gold;
            this.label71.Location = new System.Drawing.Point(44, 314);
            this.label71.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(49, 20);
            this.label71.TabIndex = 89;
            this.label71.Text = "Mana";
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label72.ForeColor = System.Drawing.Color.Gold;
            this.label72.Location = new System.Drawing.Point(44, 374);
            this.label72.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(99, 20);
            this.label72.TabIndex = 88;
            this.label72.Text = "Battle Power:";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label67.ForeColor = System.Drawing.Color.Gold;
            this.label67.Location = new System.Drawing.Point(44, 258);
            this.label67.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(44, 20);
            this.label67.TabIndex = 87;
            this.label67.Text = "Rank";
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label68.ForeColor = System.Drawing.Color.Gold;
            this.label68.Location = new System.Drawing.Point(44, 230);
            this.label68.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(43, 20);
            this.label68.TabIndex = 86;
            this.label68.Text = "Body";
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label69.ForeColor = System.Drawing.Color.Gold;
            this.label69.Location = new System.Drawing.Point(44, 286);
            this.label69.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(55, 20);
            this.label69.TabIndex = 85;
            this.label69.Text = "Health";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Italic);
            this.label66.ForeColor = System.Drawing.Color.Gold;
            this.label66.Location = new System.Drawing.Point(112, 151);
            this.label66.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(17, 20);
            this.label66.TabIndex = 84;
            this.label66.Text = "Y";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label65.ForeColor = System.Drawing.Color.Gold;
            this.label65.Location = new System.Drawing.Point(44, 179);
            this.label65.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(44, 20);
            this.label65.TabIndex = 83;
            this.label65.Text = "Class";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Font = new System.Drawing.Font("Segoe UI Semibold", 10.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label64.ForeColor = System.Drawing.Color.Gold;
            this.label64.Location = new System.Drawing.Point(44, 202);
            this.label64.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(44, 20);
            this.label64.TabIndex = 82;
            this.label64.Text = "Level";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.ErrorImage = global::TheChosenProject.Properties.Resources._296;
            this.pictureBox2.Image = global::TheChosenProject.Properties.Resources._296;
            this.pictureBox2.InitialImage = global::TheChosenProject.Properties.Resources._296;
            this.pictureBox2.Location = new System.Drawing.Point(41, 107);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(64, 64);
            this.pictureBox2.TabIndex = 81;
            this.pictureBox2.TabStop = false;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, ((System.Drawing.FontStyle)((((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline) 
                | System.Drawing.FontStyle.Strikeout))));
            this.label63.ForeColor = System.Drawing.Color.Gold;
            this.label63.Location = new System.Drawing.Point(21, 23);
            this.label63.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(21, 22);
            this.label63.TabIndex = 9;
            this.label63.Text = "0";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.label62.ForeColor = System.Drawing.Color.Gold;
            this.label62.Location = new System.Drawing.Point(86, 64);
            this.label62.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(115, 19);
            this.label62.TabIndex = 8;
            this.label62.Text = "Bot Type (Action)";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label61.ForeColor = System.Drawing.Color.Gold;
            this.label61.Location = new System.Drawing.Point(112, 83);
            this.label61.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(53, 21);
            this.label61.TabIndex = 7;
            this.label61.Text = "Name";
            // 
            // tabPage7
            // 
            this.tabPage7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage7.Controls.Add(this.groupBox15);
            this.tabPage7.Controls.Add(this.groupBox14);
            this.tabPage7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1566, 705);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Server Configuration";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // groupBox15
            // 
            this.groupBox15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox15.BackColor = System.Drawing.Color.Transparent;
            this.groupBox15.Controls.Add(this.label149);
            this.groupBox15.Controls.Add(this.textBox108);
            this.groupBox15.Controls.Add(this.label151);
            this.groupBox15.Controls.Add(this.textBox109);
            this.groupBox15.Controls.Add(this.label103);
            this.groupBox15.Controls.Add(this.textBox60);
            this.groupBox15.Controls.Add(this.label58);
            this.groupBox15.Controls.Add(this.textBox93);
            this.groupBox15.Controls.Add(this.textBox41);
            this.groupBox15.Controls.Add(this.label138);
            this.groupBox15.Controls.Add(this.textBox74);
            this.groupBox15.Controls.Add(this.textBox94);
            this.groupBox15.Controls.Add(this.textBox75);
            this.groupBox15.Controls.Add(this.label119);
            this.groupBox15.Controls.Add(this.textBox95);
            this.groupBox15.Controls.Add(this.textBox76);
            this.groupBox15.Controls.Add(this.label112);
            this.groupBox15.Controls.Add(this.textBox69);
            this.groupBox15.Controls.Add(this.label113);
            this.groupBox15.Controls.Add(this.textBox70);
            this.groupBox15.Controls.Add(this.label114);
            this.groupBox15.Controls.Add(this.textBox71);
            this.groupBox15.Controls.Add(this.label115);
            this.groupBox15.Controls.Add(this.textBox72);
            this.groupBox15.Controls.Add(this.label108);
            this.groupBox15.Controls.Add(this.textBox65);
            this.groupBox15.Controls.Add(this.label109);
            this.groupBox15.Controls.Add(this.textBox66);
            this.groupBox15.Controls.Add(this.label110);
            this.groupBox15.Controls.Add(this.textBox67);
            this.groupBox15.Controls.Add(this.label111);
            this.groupBox15.Controls.Add(this.textBox85);
            this.groupBox15.Controls.Add(this.textBox68);
            this.groupBox15.Controls.Add(this.label120);
            this.groupBox15.Controls.Add(this.label106);
            this.groupBox15.Controls.Add(this.textBox77);
            this.groupBox15.Controls.Add(this.textBox63);
            this.groupBox15.Controls.Add(this.label107);
            this.groupBox15.Controls.Add(this.textBox64);
            this.groupBox15.Controls.Add(this.label104);
            this.groupBox15.Controls.Add(this.textBox61);
            this.groupBox15.Controls.Add(this.label105);
            this.groupBox15.Controls.Add(this.textBox62);
            this.groupBox15.Controls.Add(this.label101);
            this.groupBox15.Controls.Add(this.textBox58);
            this.groupBox15.Controls.Add(this.label102);
            this.groupBox15.Controls.Add(this.textBox59);
            this.groupBox15.Controls.Add(this.label100);
            this.groupBox15.Controls.Add(this.textBox57);
            this.groupBox15.Controls.Add(this.label98);
            this.groupBox15.Controls.Add(this.textBox55);
            this.groupBox15.Controls.Add(this.label99);
            this.groupBox15.Controls.Add(this.textBox56);
            this.groupBox15.Controls.Add(this.textBox42);
            this.groupBox15.Controls.Add(this.label59);
            this.groupBox15.Controls.Add(this.textBox43);
            this.groupBox15.Controls.Add(this.label60);
            this.groupBox15.Controls.Add(this.label73);
            this.groupBox15.Controls.Add(this.label78);
            this.groupBox15.Controls.Add(this.textBox45);
            this.groupBox15.Controls.Add(this.textBox46);
            this.groupBox15.Controls.Add(this.label90);
            this.groupBox15.Controls.Add(this.textBox47);
            this.groupBox15.Controls.Add(this.label91);
            this.groupBox15.Controls.Add(this.textBox48);
            this.groupBox15.Controls.Add(this.label92);
            this.groupBox15.Controls.Add(this.textBox49);
            this.groupBox15.Controls.Add(this.label93);
            this.groupBox15.Controls.Add(this.textBox50);
            this.groupBox15.Controls.Add(this.label94);
            this.groupBox15.Controls.Add(this.textBox51);
            this.groupBox15.Controls.Add(this.label95);
            this.groupBox15.Controls.Add(this.textBox52);
            this.groupBox15.Controls.Add(this.label96);
            this.groupBox15.Controls.Add(this.textBox53);
            this.groupBox15.Controls.Add(this.label97);
            this.groupBox15.Controls.Add(this.textBox54);
            this.groupBox15.ForeColor = System.Drawing.Color.Black;
            this.groupBox15.Location = new System.Drawing.Point(675, 121);
            this.groupBox15.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox15.Size = new System.Drawing.Size(637, 480);
            this.groupBox15.TabIndex = 65545;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Rewards";
            // 
            // label149
            // 
            this.label149.AutoSize = true;
            this.label149.Location = new System.Drawing.Point(209, 53);
            this.label149.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label149.Name = "label149";
            this.label149.Size = new System.Drawing.Size(25, 15);
            this.label149.TabIndex = 65624;
            this.label149.Text = "VIP";
            // 
            // textBox108
            // 
            this.textBox108.Location = new System.Drawing.Point(234, 50);
            this.textBox108.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox108.Name = "textBox108";
            this.textBox108.Size = new System.Drawing.Size(74, 21);
            this.textBox108.TabIndex = 65623;
            // 
            // label151
            // 
            this.label151.AutoSize = true;
            this.label151.Location = new System.Drawing.Point(10, 52);
            this.label151.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label151.Name = "label151";
            this.label151.Size = new System.Drawing.Size(95, 15);
            this.label151.TabIndex = 65622;
            this.label151.Text = "[DROP] MONEY";
            // 
            // textBox109
            // 
            this.textBox109.Location = new System.Drawing.Point(140, 49);
            this.textBox109.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox109.Name = "textBox109";
            this.textBox109.Size = new System.Drawing.Size(67, 21);
            this.textBox109.TabIndex = 65621;
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(334, 372);
            this.label103.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(126, 15);
            this.label103.TabIndex = 65620;
            this.label103.Text = "[CPS] KILLER LVL 1,2";
            // 
            // textBox60
            // 
            this.textBox60.Location = new System.Drawing.Point(462, 369);
            this.textBox60.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox60.Name = "textBox60";
            this.textBox60.Size = new System.Drawing.Size(83, 21);
            this.textBox60.TabIndex = 65619;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(12, 183);
            this.label58.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(69, 15);
            this.label58.TabIndex = 65618;
            this.label58.Text = "1st soc rate";
            // 
            // textBox93
            // 
            this.textBox93.Location = new System.Drawing.Point(553, 447);
            this.textBox93.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox93.Name = "textBox93";
            this.textBox93.Size = new System.Drawing.Size(77, 21);
            this.textBox93.TabIndex = 65597;
            // 
            // textBox41
            // 
            this.textBox41.Location = new System.Drawing.Point(140, 180);
            this.textBox41.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox41.Name = "textBox41";
            this.textBox41.Size = new System.Drawing.Size(168, 21);
            this.textBox41.TabIndex = 65617;
            // 
            // label138
            // 
            this.label138.AutoSize = true;
            this.label138.Location = new System.Drawing.Point(240, 451);
            this.label138.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label138.Name = "label138";
            this.label138.Size = new System.Drawing.Size(137, 15);
            this.label138.TabIndex = 65592;
            this.label138.Text = "[CPS] ARENA RANKING";
            // 
            // textBox74
            // 
            this.textBox74.Location = new System.Drawing.Point(553, 395);
            this.textBox74.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox74.Name = "textBox74";
            this.textBox74.Size = new System.Drawing.Size(77, 21);
            this.textBox74.TabIndex = 65615;
            // 
            // textBox94
            // 
            this.textBox94.Location = new System.Drawing.Point(462, 447);
            this.textBox94.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox94.Name = "textBox94";
            this.textBox94.Size = new System.Drawing.Size(83, 21);
            this.textBox94.TabIndex = 65595;
            // 
            // textBox75
            // 
            this.textBox75.Location = new System.Drawing.Point(462, 395);
            this.textBox75.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox75.Name = "textBox75";
            this.textBox75.Size = new System.Drawing.Size(83, 21);
            this.textBox75.TabIndex = 65613;
            // 
            // label119
            // 
            this.label119.AutoSize = true;
            this.label119.Location = new System.Drawing.Point(334, 399);
            this.label119.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label119.Name = "label119";
            this.label119.Size = new System.Drawing.Size(126, 15);
            this.label119.TabIndex = 65612;
            this.label119.Text = "[CPS] KILLER LVL 2,4";
            // 
            // textBox95
            // 
            this.textBox95.Location = new System.Drawing.Point(377, 447);
            this.textBox95.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox95.Name = "textBox95";
            this.textBox95.Size = new System.Drawing.Size(77, 21);
            this.textBox95.TabIndex = 65593;
            // 
            // textBox76
            // 
            this.textBox76.Location = new System.Drawing.Point(553, 369);
            this.textBox76.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox76.Name = "textBox76";
            this.textBox76.Size = new System.Drawing.Size(77, 21);
            this.textBox76.TabIndex = 65611;
            // 
            // label112
            // 
            this.label112.AutoSize = true;
            this.label112.Location = new System.Drawing.Point(334, 346);
            this.label112.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label112.Name = "label112";
            this.label112.Size = new System.Drawing.Size(118, 15);
            this.label112.TabIndex = 65610;
            this.label112.Text = "[CPS] 4TH SKILL PK";
            // 
            // textBox69
            // 
            this.textBox69.Location = new System.Drawing.Point(462, 343);
            this.textBox69.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox69.Name = "textBox69";
            this.textBox69.Size = new System.Drawing.Size(168, 21);
            this.textBox69.TabIndex = 65609;
            // 
            // label113
            // 
            this.label113.AutoSize = true;
            this.label113.Location = new System.Drawing.Point(334, 320);
            this.label113.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label113.Name = "label113";
            this.label113.Size = new System.Drawing.Size(120, 15);
            this.label113.TabIndex = 65608;
            this.label113.Text = "[CPS] 3RD SKILL PK";
            // 
            // textBox70
            // 
            this.textBox70.Location = new System.Drawing.Point(462, 317);
            this.textBox70.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox70.Name = "textBox70";
            this.textBox70.Size = new System.Drawing.Size(168, 21);
            this.textBox70.TabIndex = 65607;
            // 
            // label114
            // 
            this.label114.AutoSize = true;
            this.label114.Location = new System.Drawing.Point(334, 294);
            this.label114.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label114.Name = "label114";
            this.label114.Size = new System.Drawing.Size(120, 15);
            this.label114.TabIndex = 65606;
            this.label114.Text = "[CPS] 2ND SKILL PK";
            // 
            // textBox71
            // 
            this.textBox71.Location = new System.Drawing.Point(462, 291);
            this.textBox71.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox71.Name = "textBox71";
            this.textBox71.Size = new System.Drawing.Size(168, 21);
            this.textBox71.TabIndex = 65605;
            // 
            // label115
            // 
            this.label115.AutoSize = true;
            this.label115.Location = new System.Drawing.Point(334, 268);
            this.label115.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label115.Name = "label115";
            this.label115.Size = new System.Drawing.Size(117, 15);
            this.label115.TabIndex = 65604;
            this.label115.Text = "[CPS] 1ST SKILL PK";
            // 
            // textBox72
            // 
            this.textBox72.Location = new System.Drawing.Point(462, 265);
            this.textBox72.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox72.Name = "textBox72";
            this.textBox72.Size = new System.Drawing.Size(168, 21);
            this.textBox72.TabIndex = 65603;
            // 
            // label108
            // 
            this.label108.AutoSize = true;
            this.label108.Location = new System.Drawing.Point(12, 450);
            this.label108.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label108.Name = "label108";
            this.label108.Size = new System.Drawing.Size(118, 15);
            this.label108.TabIndex = 65602;
            this.label108.Text = "[CPS] 4TH TEAM PK";
            // 
            // textBox65
            // 
            this.textBox65.Location = new System.Drawing.Point(140, 447);
            this.textBox65.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox65.Name = "textBox65";
            this.textBox65.Size = new System.Drawing.Size(99, 21);
            this.textBox65.TabIndex = 65601;
            // 
            // label109
            // 
            this.label109.AutoSize = true;
            this.label109.Location = new System.Drawing.Point(12, 424);
            this.label109.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label109.Name = "label109";
            this.label109.Size = new System.Drawing.Size(120, 15);
            this.label109.TabIndex = 65600;
            this.label109.Text = "[CPS] 3RD TEAM PK\r\n";
            // 
            // textBox66
            // 
            this.textBox66.Location = new System.Drawing.Point(140, 421);
            this.textBox66.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox66.Name = "textBox66";
            this.textBox66.Size = new System.Drawing.Size(168, 21);
            this.textBox66.TabIndex = 65599;
            // 
            // label110
            // 
            this.label110.AutoSize = true;
            this.label110.Location = new System.Drawing.Point(12, 398);
            this.label110.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label110.Name = "label110";
            this.label110.Size = new System.Drawing.Size(120, 15);
            this.label110.TabIndex = 65598;
            this.label110.Text = "[CPS] 2ND TEAM PK";
            // 
            // textBox67
            // 
            this.textBox67.Location = new System.Drawing.Point(140, 395);
            this.textBox67.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox67.Name = "textBox67";
            this.textBox67.Size = new System.Drawing.Size(168, 21);
            this.textBox67.TabIndex = 65597;
            // 
            // label111
            // 
            this.label111.AutoSize = true;
            this.label111.Location = new System.Drawing.Point(12, 372);
            this.label111.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label111.Name = "label111";
            this.label111.Size = new System.Drawing.Size(117, 15);
            this.label111.TabIndex = 65596;
            this.label111.Text = "[CPS] 1ST TEAM PK";
            // 
            // textBox85
            // 
            this.textBox85.Location = new System.Drawing.Point(462, 421);
            this.textBox85.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox85.Name = "textBox85";
            this.textBox85.Size = new System.Drawing.Size(83, 21);
            this.textBox85.TabIndex = 65577;
            // 
            // textBox68
            // 
            this.textBox68.Location = new System.Drawing.Point(140, 369);
            this.textBox68.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox68.Name = "textBox68";
            this.textBox68.Size = new System.Drawing.Size(168, 21);
            this.textBox68.TabIndex = 65595;
            // 
            // label120
            // 
            this.label120.AutoSize = true;
            this.label120.Location = new System.Drawing.Point(332, 426);
            this.label120.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label120.Name = "label120";
            this.label120.Size = new System.Drawing.Size(133, 15);
            this.label120.TabIndex = 65576;
            this.label120.Text = "[CPS] TREASURE (F,T)";
            // 
            // label106
            // 
            this.label106.AutoSize = true;
            this.label106.Location = new System.Drawing.Point(334, 238);
            this.label106.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label106.Name = "label106";
            this.label106.Size = new System.Drawing.Size(118, 15);
            this.label106.TabIndex = 65594;
            this.label106.Text = "[CPS] 4TH ELITE PK";
            // 
            // textBox77
            // 
            this.textBox77.Location = new System.Drawing.Point(553, 421);
            this.textBox77.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox77.Name = "textBox77";
            this.textBox77.Size = new System.Drawing.Size(77, 21);
            this.textBox77.TabIndex = 65575;
            // 
            // textBox63
            // 
            this.textBox63.Location = new System.Drawing.Point(462, 235);
            this.textBox63.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox63.Name = "textBox63";
            this.textBox63.Size = new System.Drawing.Size(168, 21);
            this.textBox63.TabIndex = 65593;
            // 
            // label107
            // 
            this.label107.AutoSize = true;
            this.label107.Location = new System.Drawing.Point(334, 212);
            this.label107.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label107.Name = "label107";
            this.label107.Size = new System.Drawing.Size(120, 15);
            this.label107.TabIndex = 65592;
            this.label107.Text = "[CPS] 3RD ELITE PK";
            // 
            // textBox64
            // 
            this.textBox64.Location = new System.Drawing.Point(462, 209);
            this.textBox64.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox64.Name = "textBox64";
            this.textBox64.Size = new System.Drawing.Size(168, 21);
            this.textBox64.TabIndex = 65591;
            // 
            // label104
            // 
            this.label104.AutoSize = true;
            this.label104.Location = new System.Drawing.Point(334, 186);
            this.label104.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label104.Name = "label104";
            this.label104.Size = new System.Drawing.Size(120, 15);
            this.label104.TabIndex = 65590;
            this.label104.Text = "[CPS] 2ND ELITE PK";
            // 
            // textBox61
            // 
            this.textBox61.Location = new System.Drawing.Point(462, 183);
            this.textBox61.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox61.Name = "textBox61";
            this.textBox61.Size = new System.Drawing.Size(168, 21);
            this.textBox61.TabIndex = 65589;
            // 
            // label105
            // 
            this.label105.AutoSize = true;
            this.label105.Location = new System.Drawing.Point(334, 160);
            this.label105.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label105.Name = "label105";
            this.label105.Size = new System.Drawing.Size(120, 15);
            this.label105.TabIndex = 65588;
            this.label105.Text = "[CPS] 1ST ELITE PK ";
            // 
            // textBox62
            // 
            this.textBox62.Location = new System.Drawing.Point(462, 157);
            this.textBox62.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox62.Name = "textBox62";
            this.textBox62.Size = new System.Drawing.Size(168, 21);
            this.textBox62.TabIndex = 65587;
            // 
            // label101
            // 
            this.label101.AutoSize = true;
            this.label101.Location = new System.Drawing.Point(334, 133);
            this.label101.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label101.Name = "label101";
            this.label101.Size = new System.Drawing.Size(106, 15);
            this.label101.TabIndex = 65586;
            this.label101.Text = "[CPS] CTF RANKS";
            // 
            // textBox58
            // 
            this.textBox58.Location = new System.Drawing.Point(462, 130);
            this.textBox58.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox58.Name = "textBox58";
            this.textBox58.Size = new System.Drawing.Size(168, 21);
            this.textBox58.TabIndex = 65585;
            // 
            // label102
            // 
            this.label102.AutoSize = true;
            this.label102.Location = new System.Drawing.Point(334, 107);
            this.label102.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label102.Name = "label102";
            this.label102.Size = new System.Drawing.Size(126, 15);
            this.label102.TabIndex = 65584;
            this.label102.Text = "[MONEY] CTF RANKS";
            // 
            // textBox59
            // 
            this.textBox59.Location = new System.Drawing.Point(462, 104);
            this.textBox59.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox59.Name = "textBox59";
            this.textBox59.Size = new System.Drawing.Size(168, 21);
            this.textBox59.TabIndex = 65583;
            // 
            // label100
            // 
            this.label100.AutoSize = true;
            this.label100.Location = new System.Drawing.Point(334, 78);
            this.label100.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(101, 15);
            this.label100.TabIndex = 65580;
            this.label100.Text = "[CPS] CLAN WAR";
            // 
            // textBox57
            // 
            this.textBox57.Location = new System.Drawing.Point(462, 75);
            this.textBox57.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox57.Name = "textBox57";
            this.textBox57.Size = new System.Drawing.Size(168, 21);
            this.textBox57.TabIndex = 65579;
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(334, 52);
            this.label98.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(127, 15);
            this.label98.TabIndex = 65578;
            this.label98.Text = "[CPS] CLASS PK WAR";
            // 
            // textBox55
            // 
            this.textBox55.Location = new System.Drawing.Point(462, 49);
            this.textBox55.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox55.Name = "textBox55";
            this.textBox55.Size = new System.Drawing.Size(168, 21);
            this.textBox55.TabIndex = 65577;
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(332, 26);
            this.label99.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(107, 15);
            this.label99.TabIndex = 65576;
            this.label99.Text = "[CPS] GUILD WAR";
            // 
            // textBox56
            // 
            this.textBox56.Location = new System.Drawing.Point(462, 23);
            this.textBox56.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox56.Name = "textBox56";
            this.textBox56.Size = new System.Drawing.Size(168, 21);
            this.textBox56.TabIndex = 65575;
            // 
            // textBox42
            // 
            this.textBox42.Location = new System.Drawing.Point(140, 232);
            this.textBox42.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox42.Name = "textBox42";
            this.textBox42.Size = new System.Drawing.Size(168, 21);
            this.textBox42.TabIndex = 65574;
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(12, 235);
            this.label59.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(74, 15);
            this.label59.TabIndex = 65573;
            this.label59.Text = "2nd soc rate";
            // 
            // textBox43
            // 
            this.textBox43.Location = new System.Drawing.Point(140, 206);
            this.textBox43.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new System.Drawing.Size(168, 21);
            this.textBox43.TabIndex = 65572;
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(12, 209);
            this.label60.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(115, 15);
            this.label60.TabIndex = 65571;
            this.label60.Text = "[CPS] CO-LETTERS";
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(12, 157);
            this.label73.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(122, 15);
            this.label73.TabIndex = 65570;
            this.label73.Text = "[NPC] CREATE CLAN";
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(12, 344);
            this.label78.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(96, 15);
            this.label78.TabIndex = 65566;
            this.label78.Text = "[CPS] ELITE GW";
            // 
            // textBox45
            // 
            this.textBox45.Location = new System.Drawing.Point(140, 341);
            this.textBox45.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox45.Name = "textBox45";
            this.textBox45.Size = new System.Drawing.Size(168, 21);
            this.textBox45.TabIndex = 65565;
            // 
            // textBox46
            // 
            this.textBox46.Location = new System.Drawing.Point(140, 154);
            this.textBox46.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new System.Drawing.Size(168, 21);
            this.textBox46.TabIndex = 65567;
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(12, 317);
            this.label90.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(127, 15);
            this.label90.TabIndex = 65564;
            this.label90.Text = "[PTS] ARENA HONOR";
            // 
            // textBox47
            // 
            this.textBox47.Location = new System.Drawing.Point(140, 314);
            this.textBox47.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox47.Name = "textBox47";
            this.textBox47.Size = new System.Drawing.Size(168, 21);
            this.textBox47.TabIndex = 65563;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(12, 291);
            this.label91.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(125, 15);
            this.label91.TabIndex = 65562;
            this.label91.Text = "[CPS] ARENA MATCH";
            // 
            // textBox48
            // 
            this.textBox48.Location = new System.Drawing.Point(140, 288);
            this.textBox48.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox48.Name = "textBox48";
            this.textBox48.Size = new System.Drawing.Size(168, 21);
            this.textBox48.TabIndex = 65561;
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(12, 264);
            this.label92.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(118, 15);
            this.label92.TabIndex = 65560;
            this.label92.Text = "[CPS] MONTHLY PK";
            // 
            // textBox49
            // 
            this.textBox49.Location = new System.Drawing.Point(140, 261);
            this.textBox49.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox49.Name = "textBox49";
            this.textBox49.Size = new System.Drawing.Size(168, 21);
            this.textBox49.TabIndex = 65559;
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Location = new System.Drawing.Point(12, 130);
            this.label93.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label93.Name = "label93";
            this.label93.Size = new System.Drawing.Size(123, 15);
            this.label93.TabIndex = 65558;
            this.label93.Text = "[NPC] CHG-GENDER";
            // 
            // textBox50
            // 
            this.textBox50.Location = new System.Drawing.Point(140, 127);
            this.textBox50.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox50.Name = "textBox50";
            this.textBox50.Size = new System.Drawing.Size(168, 21);
            this.textBox50.TabIndex = 65557;
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(12, 104);
            this.label94.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(120, 15);
            this.label94.TabIndex = 65556;
            this.label94.Text = "[MAX] NPC CHANGE";
            // 
            // textBox51
            // 
            this.textBox51.Location = new System.Drawing.Point(140, 101);
            this.textBox51.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox51.Name = "textBox51";
            this.textBox51.Size = new System.Drawing.Size(168, 21);
            this.textBox51.TabIndex = 65555;
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(12, 78);
            this.label95.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(106, 15);
            this.label95.TabIndex = 65554;
            this.label95.Text = "[NPC] CHG-NAME";
            // 
            // textBox52
            // 
            this.textBox52.Location = new System.Drawing.Point(140, 75);
            this.textBox52.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox52.Name = "textBox52";
            this.textBox52.Size = new System.Drawing.Size(168, 21);
            this.textBox52.TabIndex = 65553;
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(160, 25);
            this.label96.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(78, 15);
            this.label96.TabIndex = 65552;
            this.label96.Text = "[VALUE] CPS";
            // 
            // textBox53
            // 
            this.textBox53.Location = new System.Drawing.Point(249, 23);
            this.textBox53.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox53.Name = "textBox53";
            this.textBox53.Size = new System.Drawing.Size(65, 21);
            this.textBox53.TabIndex = 65551;
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(10, 26);
            this.label97.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(71, 15);
            this.label97.TabIndex = 83;
            this.label97.Text = "[RATE] CPS";
            // 
            // textBox54
            // 
            this.textBox54.Location = new System.Drawing.Point(89, 23);
            this.textBox54.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox54.Name = "textBox54";
            this.textBox54.Size = new System.Drawing.Size(67, 21);
            this.textBox54.TabIndex = 82;
            // 
            // groupBox14
            // 
            this.groupBox14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox14.BackColor = System.Drawing.Color.Transparent;
            this.groupBox14.Controls.Add(this.checkBox14);
            this.groupBox14.Controls.Add(this.checkBox13);
            this.groupBox14.Controls.Add(this.label126);
            this.groupBox14.Controls.Add(this.label117);
            this.groupBox14.Controls.Add(this.checkBox12);
            this.groupBox14.Controls.Add(this.checkBox9);
            this.groupBox14.Controls.Add(this.textBox110);
            this.groupBox14.Controls.Add(this.label152);
            this.groupBox14.Controls.Add(this.textBox96);
            this.groupBox14.Controls.Add(this.label139);
            this.groupBox14.Controls.Add(this.dateTimePicker1);
            this.groupBox14.Controls.Add(this.textBox92);
            this.groupBox14.Controls.Add(this.label135);
            this.groupBox14.Controls.Add(this.textBox91);
            this.groupBox14.Controls.Add(this.label134);
            this.groupBox14.Controls.Add(this.textBox90);
            this.groupBox14.Controls.Add(this.label133);
            this.groupBox14.Controls.Add(this.textBox89);
            this.groupBox14.Controls.Add(this.label132);
            this.groupBox14.Controls.Add(this.textBox87);
            this.groupBox14.Controls.Add(this.label131);
            this.groupBox14.Controls.Add(this.checkBox8);
            this.groupBox14.Controls.Add(this.textBox86);
            this.groupBox14.Controls.Add(this.label129);
            this.groupBox14.Controls.Add(this.label116);
            this.groupBox14.Controls.Add(this.textBox73);
            this.groupBox14.Controls.Add(this.textBox40);
            this.groupBox14.Controls.Add(this.label57);
            this.groupBox14.Controls.Add(this.label56);
            this.groupBox14.Controls.Add(this.textBox37);
            this.groupBox14.Controls.Add(this.label55);
            this.groupBox14.Controls.Add(this.label54);
            this.groupBox14.Controls.Add(this.textBox38);
            this.groupBox14.Controls.Add(this.textBox39);
            this.groupBox14.Controls.Add(this.label53);
            this.groupBox14.Controls.Add(this.textBox36);
            this.groupBox14.Controls.Add(this.label52);
            this.groupBox14.Controls.Add(this.textBox35);
            this.groupBox14.Controls.Add(this.label51);
            this.groupBox14.Controls.Add(this.textBox34);
            this.groupBox14.Controls.Add(this.label50);
            this.groupBox14.Controls.Add(this.label505);
            this.groupBox14.Controls.Add(this.textBox33);
            this.groupBox14.Controls.Add(this.textBox330);
            this.groupBox14.Controls.Add(this.label48);
            this.groupBox14.Controls.Add(this.textBox31);
            this.groupBox14.Controls.Add(this.label49);
            this.groupBox14.Controls.Add(this.textBox32);
            this.groupBox14.Controls.Add(this.checkBox5);
            this.groupBox14.Controls.Add(this.checkBox14);
            this.groupBox14.Controls.Add(this.label47);
            this.groupBox14.Controls.Add(this.textBox27);
            this.groupBox14.Controls.Add(this.button52);
            this.groupBox14.Controls.Add(this.label40);
            this.groupBox14.Controls.Add(this.textBox25);
            this.groupBox14.Controls.Add(this.label88);
            this.groupBox14.Controls.Add(this.textBox44);
            this.groupBox14.ForeColor = System.Drawing.Color.Black;
            this.groupBox14.Location = new System.Drawing.Point(252, 121);
            this.groupBox14.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox14.Size = new System.Drawing.Size(415, 507);
            this.groupBox14.TabIndex = 65544;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = " Configuration";
            // 
            // checkBox13
            // 
            this.checkBox13.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox13.AutoSize = true;
            this.checkBox13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.checkBox13.ForeColor = System.Drawing.Color.Transparent;
            this.checkBox13.Location = new System.Drawing.Point(262, 384);
            this.checkBox13.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new System.Drawing.Size(148, 19);
            this.checkBox13.TabIndex = 65608;
            this.checkBox13.Text = "ACTIVE TRANSLATOR";
            this.checkBox13.UseVisualStyleBackColor = false;
            // 
            // label126
            // 
            this.label126.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label126.AutoSize = true;
            this.label126.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label126.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label126.ForeColor = System.Drawing.Color.FloralWhite;
            this.label126.Location = new System.Drawing.Point(13, 408);
            this.label126.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label126.Name = "label126";
            this.label126.Size = new System.Drawing.Size(237, 13);
            this.label126.TabIndex = 65607;
            this.label126.Text = "MONSTER NEEDED TO RESTART TO ACTIVE";
            this.label126.UseMnemonic = false;
            // 
            // label117
            // 
            this.label117.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label117.AutoSize = true;
            this.label117.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label117.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label117.ForeColor = System.Drawing.Color.FloralWhite;
            this.label117.Location = new System.Drawing.Point(273, 332);
            this.label117.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label117.Name = "label117";
            this.label117.Size = new System.Drawing.Size(134, 16);
            this.label117.TabIndex = 65605;
            this.label117.Text = "AUTO MAINTENANCE";
            this.label117.UseMnemonic = false;
            // 
            // checkBox12
            // 
            this.checkBox12.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox12.AutoSize = true;
            this.checkBox12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.checkBox12.ForeColor = System.Drawing.Color.Transparent;
            this.checkBox12.Location = new System.Drawing.Point(32, 429);
            this.checkBox12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(218, 19);
            this.checkBox12.TabIndex = 65604;
            this.checkBox12.Text = "ACTIVE HIGH-MONSTER SPAWNS";
            this.checkBox12.UseVisualStyleBackColor = false;
            // 
            // checkBox9
            // 
            this.checkBox9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox9.AutoSize = true;
            this.checkBox9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.checkBox9.ForeColor = System.Drawing.Color.Transparent;
            this.checkBox9.Location = new System.Drawing.Point(262, 430);
            this.checkBox9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(148, 19);
            this.checkBox9.TabIndex = 65603;
            this.checkBox9.Text = "CLOSE CONNECTION";
            this.checkBox9.UseVisualStyleBackColor = false;
            // 
            // textBox110
            // 
            this.textBox110.Location = new System.Drawing.Point(141, 288);
            this.textBox110.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox110.Name = "textBox110";
            this.textBox110.Size = new System.Drawing.Size(76, 21);
            this.textBox110.TabIndex = 65602;
            // 
            // label152
            // 
            this.label152.AutoSize = true;
            this.label152.Location = new System.Drawing.Point(13, 291);
            this.label152.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label152.Name = "label152";
            this.label152.Size = new System.Drawing.Size(126, 15);
            this.label152.TabIndex = 65601;
            this.label152.Text = "[RESPAWNS] MOB %";
            // 
            // textBox96
            // 
            this.textBox96.Location = new System.Drawing.Point(333, 232);
            this.textBox96.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox96.Name = "textBox96";
            this.textBox96.Size = new System.Drawing.Size(74, 21);
            this.textBox96.TabIndex = 65600;
            // 
            // label139
            // 
            this.label139.AutoSize = true;
            this.label139.Location = new System.Drawing.Point(222, 235);
            this.label139.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label139.Name = "label139";
            this.label139.Size = new System.Drawing.Size(107, 15);
            this.label139.TabIndex = 65599;
            this.label139.Text = "[MAX] ROLE VIEW";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarTitleBackColor = System.Drawing.SystemColors.ControlText;
            this.dateTimePicker1.CalendarTitleForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.dateTimePicker1.Location = new System.Drawing.Point(13, 340);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(392, 21);
            this.dateTimePicker1.TabIndex = 65598;
            // 
            // textBox92
            // 
            this.textBox92.Location = new System.Drawing.Point(333, 154);
            this.textBox92.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox92.Name = "textBox92";
            this.textBox92.Size = new System.Drawing.Size(74, 21);
            this.textBox92.TabIndex = 65591;
            // 
            // label135
            // 
            this.label135.AutoSize = true;
            this.label135.Location = new System.Drawing.Point(222, 157);
            this.label135.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label135.Name = "label135";
            this.label135.Size = new System.Drawing.Size(112, 15);
            this.label135.TabIndex = 65590;
            this.label135.Text = "[GIFT] NEWBIES(+)";
            // 
            // textBox91
            // 
            this.textBox91.Location = new System.Drawing.Point(333, 128);
            this.textBox91.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox91.Name = "textBox91";
            this.textBox91.Size = new System.Drawing.Size(74, 21);
            this.textBox91.TabIndex = 65589;
            // 
            // label134
            // 
            this.label134.AutoSize = true;
            this.label134.Location = new System.Drawing.Point(222, 131);
            this.label134.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label134.Name = "label134";
            this.label134.Size = new System.Drawing.Size(100, 15);
            this.label134.TabIndex = 65588;
            this.label134.Text = "[MAX] LEVELING";
            // 
            // textBox90
            // 
            this.textBox90.Location = new System.Drawing.Point(333, 102);
            this.textBox90.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox90.Name = "textBox90";
            this.textBox90.Size = new System.Drawing.Size(74, 21);
            this.textBox90.TabIndex = 65587;
            // 
            // label133
            // 
            this.label133.AutoSize = true;
            this.label133.Location = new System.Drawing.Point(222, 105);
            this.label133.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label133.Name = "label133";
            this.label133.Size = new System.Drawing.Size(91, 15);
            this.label133.TabIndex = 65586;
            this.label133.Text = "[MAX] ITEM HP";
            // 
            // textBox89
            // 
            this.textBox89.Location = new System.Drawing.Point(333, 75);
            this.textBox89.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox89.Name = "textBox89";
            this.textBox89.Size = new System.Drawing.Size(74, 21);
            this.textBox89.TabIndex = 65585;
            // 
            // label132
            // 
            this.label132.AutoSize = true;
            this.label132.Location = new System.Drawing.Point(222, 78);
            this.label132.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label132.Name = "label132";
            this.label132.Size = new System.Drawing.Size(113, 15);
            this.label132.TabIndex = 65584;
            this.label132.Text = "[MAX] ITEM BLESS";
            // 
            // textBox87
            // 
            this.textBox87.Location = new System.Drawing.Point(333, 49);
            this.textBox87.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox87.Name = "textBox87";
            this.textBox87.Size = new System.Drawing.Size(74, 21);
            this.textBox87.TabIndex = 65583;
            // 
            // label131
            // 
            this.label131.AutoSize = true;
            this.label131.Location = new System.Drawing.Point(222, 52);
            this.label131.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label131.Name = "label131";
            this.label131.Size = new System.Drawing.Size(106, 15);
            this.label131.TabIndex = 65582;
            this.label131.Text = "[MAX] ITEM PLUS";
            // 
            // checkBox8
            // 
            this.checkBox8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox8.AutoSize = true;
            this.checkBox8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.checkBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox8.ForeColor = System.Drawing.Color.Transparent;
            this.checkBox8.Location = new System.Drawing.Point(30, 381);
            this.checkBox8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(216, 20);
            this.checkBox8.TabIndex = 65581;
            this.checkBox8.Text = "ACTIVE AUTO MAINTENANCE";
            this.checkBox8.UseVisualStyleBackColor = false;
            // 
            // textBox86
            // 
            this.textBox86.Location = new System.Drawing.Point(333, 23);
            this.textBox86.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox86.Name = "textBox86";
            this.textBox86.Size = new System.Drawing.Size(74, 21);
            this.textBox86.TabIndex = 65580;
            // 
            // label129
            // 
            this.label129.AutoSize = true;
            this.label129.Location = new System.Drawing.Point(222, 26);
            this.label129.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label129.Name = "label129";
            this.label129.Size = new System.Drawing.Size(117, 15);
            this.label129.TabIndex = 65579;
            this.label129.Text = "[EXP] AI-SHARES %";
            // 
            // label116
            // 
            this.label116.AutoSize = true;
            this.label116.Location = new System.Drawing.Point(222, 183);
            this.label116.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label116.Name = "label116";
            this.label116.Size = new System.Drawing.Size(114, 15);
            this.label116.TabIndex = 65574;
            this.label116.Text = "[MAX]CO-LETTERS";
            // 
            // textBox73
            // 
            this.textBox73.Location = new System.Drawing.Point(333, 180);
            this.textBox73.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox73.Name = "textBox73";
            this.textBox73.Size = new System.Drawing.Size(74, 21);
            this.textBox73.TabIndex = 65573;
            // 
            // textBox40
            // 
            this.textBox40.Location = new System.Drawing.Point(141, 206);
            this.textBox40.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new System.Drawing.Size(76, 21);
            this.textBox40.TabIndex = 65572;
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(13, 209);
            this.label57.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(130, 15);
            this.label57.TabIndex = 65571;
            this.label57.Text = "[DROP] DragonBall  %\r\n";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(13, 157);
            this.label56.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(109, 15);
            this.label56.TabIndex = 65570;
            this.label56.Text = "[DROP] STONE +1";
            // 
            // textBox37
            // 
            this.textBox37.Location = new System.Drawing.Point(141, 180);
            this.textBox37.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox37.Name = "textBox37";
            this.textBox37.Size = new System.Drawing.Size(76, 21);
            this.textBox37.TabIndex = 65569;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(222, 261);
            this.label55.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(113, 15);
            this.label55.TabIndex = 65566;
            this.label55.Text = "[MAX] PC-LOGGED";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(13, 183);
            this.label54.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(126, 15);
            this.label54.TabIndex = 65568;
            this.label54.Text = "[DROP] STONE +2  %";
            // 
            // textBox38
            // 
            this.textBox38.Location = new System.Drawing.Point(333, 258);
            this.textBox38.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox38.Name = "textBox38";
            this.textBox38.Size = new System.Drawing.Size(74, 21);
            this.textBox38.TabIndex = 65565;
            // 
            // textBox39
            // 
            this.textBox39.Location = new System.Drawing.Point(141, 154);
            this.textBox39.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new System.Drawing.Size(76, 21);
            this.textBox39.TabIndex = 65567;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(13, 263);
            this.label53.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(99, 15);
            this.label53.TabIndex = 65564;
            this.label53.Text = "[EXP] LEVELS %";
            // 
            // textBox36
            // 
            this.textBox36.Location = new System.Drawing.Point(141, 260);
            this.textBox36.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new System.Drawing.Size(76, 21);
            this.textBox36.TabIndex = 65563;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(222, 288);
            this.label52.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(95, 15);
            this.label52.TabIndex = 65562;
            this.label52.Text = "[EXP] PROFS %";
            // 
            // textBox35
            // 
            this.textBox35.Location = new System.Drawing.Point(333, 285);
            this.textBox35.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new System.Drawing.Size(74, 21);
            this.textBox35.TabIndex = 65561;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(13, 236);
            this.label51.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(95, 15);
            this.label51.TabIndex = 65560;
            this.label51.Text = "[EXP] SKILLS %";
            // 
            // textBox34
            // 
            this.textBox34.Location = new System.Drawing.Point(141, 233);
            this.textBox34.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox34.Name = "textBox34";
            this.textBox34.Size = new System.Drawing.Size(76, 21);
            this.textBox34.TabIndex = 65559;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(13, 130);
            this.label50.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(118, 15);
            this.label50.TabIndex = 65558;
            this.label50.Text = "[DROP] EXPBALL %";
            // 
            // label505
            // 
            this.label505.AutoSize = true;
            this.label505.Location = new System.Drawing.Point(13, 314);
            this.label505.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label505.Name = "label505";
            this.label505.Size = new System.Drawing.Size(109, 15);
            this.label505.TabIndex = 65558;
            this.label505.Text = "[DROP] Box Key %";
            // 
            // textBox33
            // 
            this.textBox33.Location = new System.Drawing.Point(141, 127);
            this.textBox33.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox33.Name = "textBox33";
            this.textBox33.Size = new System.Drawing.Size(76, 21);
            this.textBox33.TabIndex = 65557;
            // 
            // textBox330
            // 
            this.textBox330.Location = new System.Drawing.Point(141, 313);
            this.textBox330.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox330.Name = "textBox330";
            this.textBox330.Size = new System.Drawing.Size(76, 21);
            this.textBox330.TabIndex = 65557;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(13, 104);
            this.label48.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(101, 15);
            this.label48.TabIndex = 65556;
            this.label48.Text = "[DROP] GEMS %";
            // 
            // textBox31
            // 
            this.textBox31.Location = new System.Drawing.Point(141, 101);
            this.textBox31.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new System.Drawing.Size(76, 21);
            this.textBox31.TabIndex = 65555;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(13, 78);
            this.label49.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(117, 15);
            this.label49.TabIndex = 65554;
            this.label49.Text = "[DROP] METEOR %";
            // 
            // textBox32
            // 
            this.textBox32.Location = new System.Drawing.Point(141, 75);
            this.textBox32.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new System.Drawing.Size(76, 21);
            this.textBox32.TabIndex = 65553;
            // 
            // checkBox5
            // 
            this.checkBox5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox5.AutoSize = true;
            this.checkBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.checkBox5.ForeColor = System.Drawing.Color.Transparent;
            this.checkBox5.Location = new System.Drawing.Point(262, 407);
            this.checkBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(152, 19);
            this.checkBox5.TabIndex = 65550;
            this.checkBox5.Text = "ACTIVE TEST-CENTER";
            this.checkBox5.UseVisualStyleBackColor = false;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(13, 52);
            this.label47.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(111, 15);
            this.label47.TabIndex = 65552;
            this.label47.Text = "[DROP] ITEM +1 %";
            // 
            // textBox27
            // 
            this.textBox27.Location = new System.Drawing.Point(141, 49);
            this.textBox27.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new System.Drawing.Size(76, 21);
            this.textBox27.TabIndex = 65551;
            // 
            // button52
            // 
            this.button52.BackColor = System.Drawing.Color.White;
            this.button52.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button52.Location = new System.Drawing.Point(32, 468);
            this.button52.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button52.Name = "button52";
            this.button52.Size = new System.Drawing.Size(145, 33);
            this.button52.TabIndex = 65548;
            this.button52.Text = "Update  Configuration";
            this.button52.UseVisualStyleBackColor = true;
            this.button52.Click += new System.EventHandler(this.UpdateConfigration);
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(13, 26);
            this.label40.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(119, 15);
            this.label40.TabIndex = 83;
            this.label40.Text = "[DROP] LETTERS %";
            // 
            // textBox25
            // 
            this.textBox25.Location = new System.Drawing.Point(141, 23);
            this.textBox25.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new System.Drawing.Size(76, 21);
            this.textBox25.TabIndex = 82;
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Location = new System.Drawing.Point(223, 209);
            this.label88.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(106, 15);
            this.label88.TabIndex = 65568;
            this.label88.Text = "[CP]STAY ONLINE";
            // 
            // textBox44
            // 
            this.textBox44.Location = new System.Drawing.Point(333, 206);
            this.textBox44.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox44.Name = "textBox44";
            this.textBox44.Size = new System.Drawing.Size(74, 21);
            this.textBox44.TabIndex = 65569;
            // 
            // tabPage9
            // 
            this.tabPage9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage9.Controls.Add(this.button57);
            this.tabPage9.Controls.Add(this.label121);
            this.tabPage9.Controls.Add(this.textBox12);
            this.tabPage9.Controls.Add(this.label153);
            this.tabPage9.Controls.Add(this.textBox88);
            this.tabPage9.Controls.Add(this.label130);
            this.tabPage9.Controls.Add(this.dataGridView4);
            this.tabPage9.Controls.Add(this.button69);
            this.tabPage9.Controls.Add(this.label170);
            this.tabPage9.Controls.Add(this.button68);
            this.tabPage9.Controls.Add(this.comboBox14);
            this.tabPage9.Controls.Add(this.label171);
            this.tabPage9.Controls.Add(this.button65);
            this.tabPage9.Controls.Add(this.label156);
            this.tabPage9.Controls.Add(this.textBox84);
            this.tabPage9.Controls.Add(this.textBox83);
            this.tabPage9.Controls.Add(this.textBox82);
            this.tabPage9.Controls.Add(this.textBox81);
            this.tabPage9.Controls.Add(this.textBox80);
            this.tabPage9.Controls.Add(this.checkBox6);
            this.tabPage9.Controls.Add(this.button55);
            this.tabPage9.Controls.Add(this.label123);
            this.tabPage9.Controls.Add(this.label124);
            this.tabPage9.Controls.Add(this.label125);
            this.tabPage9.Controls.Add(this.label127);
            this.tabPage9.Controls.Add(this.label128);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(1566, 705);
            this.tabPage9.TabIndex = 8;
            this.tabPage9.Text = "Server Message";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // button57
            // 
            this.button57.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button57.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.button57.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button57.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button57.Location = new System.Drawing.Point(752, 443);
            this.button57.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button57.Name = "button57";
            this.button57.Size = new System.Drawing.Size(563, 27);
            this.button57.TabIndex = 65612;
            this.button57.Text = "Update messages";
            this.button57.UseVisualStyleBackColor = false;
            // 
            // label121
            // 
            this.label121.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label121.AutoSize = true;
            this.label121.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label121.Font = new System.Drawing.Font("Tahoma", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label121.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label121.Location = new System.Drawing.Point(929, 115);
            this.label121.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label121.Name = "label121";
            this.label121.Size = new System.Drawing.Size(200, 25);
            this.label121.TabIndex = 65611;
            this.label121.Text = "WORLD MESSAGE";
            // 
            // textBox12
            // 
            this.textBox12.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox12.BackColor = System.Drawing.Color.Teal;
            this.textBox12.Location = new System.Drawing.Point(752, 147);
            this.textBox12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox12.MaxLength = 1000000;
            this.textBox12.Multiline = true;
            this.textBox12.Name = "textBox12";
            this.textBox12.ReadOnly = true;
            this.textBox12.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox12.Size = new System.Drawing.Size(563, 290);
            this.textBox12.TabIndex = 65610;
            this.textBox12.Text = "World Message";
            // 
            // label153
            // 
            this.label153.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label153.AutoSize = true;
            this.label153.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label153.Font = new System.Drawing.Font("Tahoma", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label153.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label153.Location = new System.Drawing.Point(453, 473);
            this.label153.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label153.Name = "label153";
            this.label153.Size = new System.Drawing.Size(726, 25);
            this.label153.TabIndex = 65609;
            this.label153.Text = "RESET (?%) FROM PLAYERS LIKE CPS SILVER AND NOBILITY RANK";
            // 
            // textBox88
            // 
            this.textBox88.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox88.Location = new System.Drawing.Point(366, 270);
            this.textBox88.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox88.Name = "textBox88";
            this.textBox88.Size = new System.Drawing.Size(373, 20);
            this.textBox88.TabIndex = 65608;
            this.textBox88.Text = "*Check our website for the updates made.\"));";
            // 
            // label130
            // 
            this.label130.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label130.AutoSize = true;
            this.label130.Location = new System.Drawing.Point(258, 275);
            this.label130.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label130.Name = "label130";
            this.label130.Size = new System.Drawing.Size(31, 13);
            this.label130.TabIndex = 65607;
            this.label130.Text = "Body";
            // 
            // dataGridView4
            // 
            this.dataGridView4.AllowUserToAddRows = false;
            this.dataGridView4.AllowUserToDeleteRows = false;
            this.dataGridView4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataGridView4.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Location = new System.Drawing.Point(251, 501);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.RowHeadersWidth = 51;
            this.dataGridView4.Size = new System.Drawing.Size(1058, 61);
            this.dataGridView4.TabIndex = 65606;
            // 
            // button69
            // 
            this.button69.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button69.BackColor = System.Drawing.Color.White;
            this.button69.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button69.Location = new System.Drawing.Point(362, 431);
            this.button69.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button69.Name = "button69";
            this.button69.Size = new System.Drawing.Size(151, 33);
            this.button69.TabIndex = 65605;
            this.button69.Text = "Fixed GameMap";
            this.button69.UseVisualStyleBackColor = true;
            this.button69.Click += new System.EventHandler(this.FixedGameMap);
            // 
            // label170
            // 
            this.label170.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label170.AutoSize = true;
            this.label170.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label170.Font = new System.Drawing.Font("Tahoma", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label170.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label170.Location = new System.Drawing.Point(406, 370);
            this.label170.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label170.Name = "label170";
            this.label170.Size = new System.Drawing.Size(262, 25);
            this.label170.TabIndex = 65603;
            this.label170.Text = "SERVER WEATHER LIVE";
            // 
            // button68
            // 
            this.button68.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button68.BackColor = System.Drawing.Color.White;
            this.button68.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button68.Location = new System.Drawing.Point(573, 431);
            this.button68.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button68.Name = "button68";
            this.button68.Size = new System.Drawing.Size(166, 33);
            this.button68.TabIndex = 65602;
            this.button68.Text = "Update Weather";
            this.button68.UseVisualStyleBackColor = true;
            this.button68.Click += new System.EventHandler(this.ChangeWeather);
            // 
            // comboBox14
            // 
            this.comboBox14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBox14.AutoCompleteCustomSource.AddRange(new string[] {
            "Nothing",
            "Rain",
            "Snow",
            "RainWind",
            "AutumnLeaves",
            "CherryBlossomPetals",
            "CherryBlossomPetalsWind",
            "BlowingCotten",
            "Atoms"});
            this.comboBox14.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.comboBox14.FormattingEnabled = true;
            this.comboBox14.Items.AddRange(new object[] {
            "Nothing",
            "Rain",
            "Snow",
            "RainWind",
            "AutumnLeaves",
            "CherryBlossomPetals",
            "CherryBlossomPetalsWind",
            "BlowingCotten",
            "Atoms"});
            this.comboBox14.Location = new System.Drawing.Point(362, 404);
            this.comboBox14.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox14.Name = "comboBox14";
            this.comboBox14.Size = new System.Drawing.Size(377, 21);
            this.comboBox14.TabIndex = 65601;
            // 
            // label171
            // 
            this.label171.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label171.AutoSize = true;
            this.label171.Location = new System.Drawing.Point(258, 407);
            this.label171.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label171.Name = "label171";
            this.label171.Size = new System.Drawing.Size(51, 13);
            this.label171.TabIndex = 65600;
            this.label171.Text = "Weather:";
            // 
            // button65
            // 
            this.button65.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button65.BackColor = System.Drawing.Color.White;
            this.button65.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button65.Location = new System.Drawing.Point(252, 568);
            this.button65.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button65.Name = "button65";
            this.button65.Size = new System.Drawing.Size(1058, 33);
            this.button65.TabIndex = 65585;
            this.button65.Text = "Reset Selected";
            this.button65.UseVisualStyleBackColor = true;
            this.button65.Click += new System.EventHandler(this.ResetValueFromUser);
            // 
            // label156
            // 
            this.label156.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label156.AutoSize = true;
            this.label156.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label156.Font = new System.Drawing.Font("Tahoma", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label156.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label156.Location = new System.Drawing.Point(361, 115);
            this.label156.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label156.Name = "label156";
            this.label156.Size = new System.Drawing.Size(378, 25);
            this.label156.TabIndex = 65580;
            this.label156.Text = "StaticGUI --> WELCOME MESSAGE";
            // 
            // textBox84
            // 
            this.textBox84.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox84.Location = new System.Drawing.Point(366, 244);
            this.textBox84.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox84.Name = "textBox84";
            this.textBox84.Size = new System.Drawing.Size(373, 20);
            this.textBox84.TabIndex = 65577;
            this.textBox84.Text = "*Check our website for the updates made.\"));";
            // 
            // textBox83
            // 
            this.textBox83.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox83.Location = new System.Drawing.Point(366, 220);
            this.textBox83.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox83.Name = "textBox83";
            this.textBox83.Size = new System.Drawing.Size(373, 20);
            this.textBox83.TabIndex = 65576;
            this.textBox83.Text = "*Released the new Anti Cheat.";
            // 
            // textBox82
            // 
            this.textBox82.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox82.Location = new System.Drawing.Point(366, 196);
            this.textBox82.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox82.Name = "textBox82";
            this.textBox82.Size = new System.Drawing.Size(373, 20);
            this.textBox82.TabIndex = 65575;
            this.textBox82.Text = "*Increase the CPs rate.";
            // 
            // textBox81
            // 
            this.textBox81.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox81.Location = new System.Drawing.Point(366, 147);
            this.textBox81.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox81.Name = "textBox81";
            this.textBox81.Size = new System.Drawing.Size(373, 20);
            this.textBox81.TabIndex = 65574;
            this.textBox81.Text = "*Increase the Exp Rate.";
            // 
            // textBox80
            // 
            this.textBox80.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox80.Location = new System.Drawing.Point(366, 172);
            this.textBox80.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox80.Name = "textBox80";
            this.textBox80.Size = new System.Drawing.Size(373, 20);
            this.textBox80.TabIndex = 65573;
            this.textBox80.Text = "*Riding Spell can\'t use in GuildWar/SuperGuildWar.";
            // 
            // checkBox6
            // 
            this.checkBox6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox6.AutoSize = true;
            this.checkBox6.ForeColor = System.Drawing.Color.Maroon;
            this.checkBox6.Location = new System.Drawing.Point(366, 296);
            this.checkBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(103, 17);
            this.checkBox6.TabIndex = 65572;
            this.checkBox6.Text = "Allow Static GUI";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.AllowMessageChecked);
            // 
            // button55
            // 
            this.button55.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button55.BackColor = System.Drawing.Color.White;
            this.button55.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button55.Location = new System.Drawing.Point(366, 314);
            this.button55.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button55.Name = "button55";
            this.button55.Size = new System.Drawing.Size(373, 33);
            this.button55.TabIndex = 65570;
            this.button55.Text = "Update";
            this.button55.UseVisualStyleBackColor = true;
            this.button55.Click += new System.EventHandler(this.UpdateStaticMessage);
            // 
            // label123
            // 
            this.label123.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label123.AutoSize = true;
            this.label123.Location = new System.Drawing.Point(258, 249);
            this.label123.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label123.Name = "label123";
            this.label123.Size = new System.Drawing.Size(31, 13);
            this.label123.TabIndex = 65569;
            this.label123.Text = "Body";
            // 
            // label124
            // 
            this.label124.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label124.AutoSize = true;
            this.label124.Location = new System.Drawing.Point(258, 223);
            this.label124.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label124.Name = "label124";
            this.label124.Size = new System.Drawing.Size(31, 13);
            this.label124.TabIndex = 65567;
            this.label124.Text = "Body";
            // 
            // label125
            // 
            this.label125.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label125.AutoSize = true;
            this.label125.Location = new System.Drawing.Point(258, 199);
            this.label125.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label125.Name = "label125";
            this.label125.Size = new System.Drawing.Size(31, 13);
            this.label125.TabIndex = 65565;
            this.label125.Text = "Body";
            // 
            // label127
            // 
            this.label127.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label127.AutoSize = true;
            this.label127.Location = new System.Drawing.Point(258, 172);
            this.label127.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label127.Name = "label127";
            this.label127.Size = new System.Drawing.Size(31, 13);
            this.label127.TabIndex = 65562;
            this.label127.Text = "Body";
            // 
            // label128
            // 
            this.label128.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label128.AutoSize = true;
            this.label128.Location = new System.Drawing.Point(258, 152);
            this.label128.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label128.Name = "label128";
            this.label128.Size = new System.Drawing.Size(31, 13);
            this.label128.TabIndex = 65560;
            this.label128.Text = "Body";
            // 
            // tabPage10
            // 
            this.tabPage10.BackColor = System.Drawing.Color.White;
            this.tabPage10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage10.Controls.Add(this.label154);
            this.tabPage10.Controls.Add(this.groupBox17);
            this.tabPage10.Controls.Add(this.groupBox16);
            this.tabPage10.Controls.Add(this.button58);
            this.tabPage10.Controls.Add(this.dataGridView1);
            this.tabPage10.Controls.Add(this.listBox3);
            this.tabPage10.Controls.Add(this.button61);
            this.tabPage10.Location = new System.Drawing.Point(4, 22);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage10.Size = new System.Drawing.Size(1566, 705);
            this.tabPage10.TabIndex = 9;
            this.tabPage10.Text = "Boss Management";
            // 
            // label154
            // 
            this.label154.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label154.AutoSize = true;
            this.label154.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label154.Font = new System.Drawing.Font("Tahoma", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label154.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label154.Location = new System.Drawing.Point(629, 364);
            this.label154.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label154.Name = "label154";
            this.label154.Size = new System.Drawing.Size(370, 25);
            this.label154.TabIndex = 65604;
            this.label154.Text = "BOSSES MANAGER INFORMATION";
            // 
            // groupBox17
            // 
            this.groupBox17.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox17.BackColor = System.Drawing.Color.Transparent;
            this.groupBox17.Controls.Add(this.button51);
            this.groupBox17.Controls.Add(this.button62);
            this.groupBox17.Controls.Add(this.button63);
            this.groupBox17.Controls.Add(this.pictureBox3);
            this.groupBox17.Controls.Add(this.textBox107);
            this.groupBox17.Controls.Add(this.textBox105);
            this.groupBox17.Controls.Add(this.textBox106);
            this.groupBox17.Controls.Add(this.label150);
            this.groupBox17.Controls.Add(this.textBox104);
            this.groupBox17.Controls.Add(this.label148);
            this.groupBox17.Controls.Add(this.textBox103);
            this.groupBox17.Controls.Add(this.label147);
            this.groupBox17.Controls.Add(this.textBox101);
            this.groupBox17.Controls.Add(this.label145);
            this.groupBox17.Controls.Add(this.textBox102);
            this.groupBox17.Controls.Add(this.label146);
            this.groupBox17.Controls.Add(this.textBox99);
            this.groupBox17.Controls.Add(this.label143);
            this.groupBox17.Controls.Add(this.textBox100);
            this.groupBox17.Controls.Add(this.label144);
            this.groupBox17.ForeColor = System.Drawing.Color.Black;
            this.groupBox17.Location = new System.Drawing.Point(537, 102);
            this.groupBox17.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox17.Size = new System.Drawing.Size(603, 264);
            this.groupBox17.TabIndex = 65556;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Boss Alive";
            // 
            // button51
            // 
            this.button51.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button51.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.button51.ForeColor = System.Drawing.Color.Black;
            this.button51.Location = new System.Drawing.Point(415, 214);
            this.button51.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button51.Name = "button51";
            this.button51.Size = new System.Drawing.Size(129, 27);
            this.button51.TabIndex = 65576;
            this.button51.Text = "Score Board";
            this.button51.UseVisualStyleBackColor = true;
            this.button51.Click += new System.EventHandler(this.ScoreboardAliveMonster);
            // 
            // button62
            // 
            this.button62.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button62.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.button62.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button62.Location = new System.Drawing.Point(86, 214);
            this.button62.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button62.Name = "button62";
            this.button62.Size = new System.Drawing.Size(154, 27);
            this.button62.TabIndex = 65573;
            this.button62.Text = "Save";
            this.button62.UseVisualStyleBackColor = true;
            this.button62.Click += new System.EventHandler(this.SaveBossStats);
            // 
            // button63
            // 
            this.button63.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button63.BackColor = System.Drawing.Color.Black;
            this.button63.ForeColor = System.Drawing.Color.Black;
            this.button63.Location = new System.Drawing.Point(285, 214);
            this.button63.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button63.Name = "button63";
            this.button63.Size = new System.Drawing.Size(122, 27);
            this.button63.TabIndex = 65575;
            this.button63.Text = "Disconnect";
            this.button63.UseVisualStyleBackColor = true;
            this.button63.Click += new System.EventHandler(this.KickoutBoss);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox3.Location = new System.Drawing.Point(285, 19);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(259, 189);
            this.pictureBox3.TabIndex = 65574;
            this.pictureBox3.TabStop = false;
            // 
            // textBox107
            // 
            this.textBox107.Enabled = false;
            this.textBox107.Location = new System.Drawing.Point(195, 188);
            this.textBox107.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox107.MaxLength = 32;
            this.textBox107.Name = "textBox107";
            this.textBox107.Size = new System.Drawing.Size(45, 20);
            this.textBox107.TabIndex = 65572;
            // 
            // textBox105
            // 
            this.textBox105.Enabled = false;
            this.textBox105.Location = new System.Drawing.Point(142, 188);
            this.textBox105.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox105.MaxLength = 32;
            this.textBox105.Name = "textBox105";
            this.textBox105.Size = new System.Drawing.Size(45, 20);
            this.textBox105.TabIndex = 65570;
            // 
            // textBox106
            // 
            this.textBox106.Enabled = false;
            this.textBox106.Location = new System.Drawing.Point(86, 188);
            this.textBox106.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox106.MaxLength = 32;
            this.textBox106.Name = "textBox106";
            this.textBox106.Size = new System.Drawing.Size(45, 20);
            this.textBox106.TabIndex = 65568;
            // 
            // label150
            // 
            this.label150.AutoSize = true;
            this.label150.Location = new System.Drawing.Point(15, 191);
            this.label150.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label150.Name = "label150";
            this.label150.Size = new System.Drawing.Size(54, 13);
            this.label150.TabIndex = 65569;
            this.label150.Text = "Map (X,Y)";
            // 
            // textBox104
            // 
            this.textBox104.Enabled = false;
            this.textBox104.Location = new System.Drawing.Point(86, 162);
            this.textBox104.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox104.MaxLength = 32;
            this.textBox104.Name = "textBox104";
            this.textBox104.Size = new System.Drawing.Size(154, 20);
            this.textBox104.TabIndex = 65566;
            // 
            // label148
            // 
            this.label148.AutoSize = true;
            this.label148.Location = new System.Drawing.Point(15, 165);
            this.label148.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label148.Name = "label148";
            this.label148.Size = new System.Drawing.Size(33, 13);
            this.label148.TabIndex = 65567;
            this.label148.Text = "Level";
            // 
            // textBox103
            // 
            this.textBox103.Enabled = false;
            this.textBox103.Location = new System.Drawing.Point(85, 136);
            this.textBox103.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox103.MaxLength = 32;
            this.textBox103.Name = "textBox103";
            this.textBox103.Size = new System.Drawing.Size(154, 20);
            this.textBox103.TabIndex = 65564;
            // 
            // label147
            // 
            this.label147.AutoSize = true;
            this.label147.Location = new System.Drawing.Point(14, 139);
            this.label147.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label147.Name = "label147";
            this.label147.Size = new System.Drawing.Size(24, 13);
            this.label147.TabIndex = 65565;
            this.label147.Text = "Life";
            // 
            // textBox101
            // 
            this.textBox101.Location = new System.Drawing.Point(85, 110);
            this.textBox101.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox101.MaxLength = 32;
            this.textBox101.Name = "textBox101";
            this.textBox101.Size = new System.Drawing.Size(154, 20);
            this.textBox101.TabIndex = 65562;
            // 
            // label145
            // 
            this.label145.AutoSize = true;
            this.label145.Location = new System.Drawing.Point(14, 113);
            this.label145.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label145.Name = "label145";
            this.label145.Size = new System.Drawing.Size(54, 13);
            this.label145.TabIndex = 65563;
            this.label145.Text = "Defence2";
            // 
            // textBox102
            // 
            this.textBox102.Location = new System.Drawing.Point(85, 85);
            this.textBox102.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox102.MaxLength = 32;
            this.textBox102.Name = "textBox102";
            this.textBox102.Size = new System.Drawing.Size(154, 20);
            this.textBox102.TabIndex = 65560;
            // 
            // label146
            // 
            this.label146.AutoSize = true;
            this.label146.Location = new System.Drawing.Point(14, 88);
            this.label146.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label146.Name = "label146";
            this.label146.Size = new System.Drawing.Size(48, 13);
            this.label146.TabIndex = 65561;
            this.label146.Text = "Defence";
            // 
            // textBox99
            // 
            this.textBox99.Location = new System.Drawing.Point(85, 59);
            this.textBox99.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox99.MaxLength = 32;
            this.textBox99.Name = "textBox99";
            this.textBox99.Size = new System.Drawing.Size(154, 20);
            this.textBox99.TabIndex = 65558;
            // 
            // label143
            // 
            this.label143.AutoSize = true;
            this.label143.Location = new System.Drawing.Point(14, 62);
            this.label143.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label143.Name = "label143";
            this.label143.Size = new System.Drawing.Size(61, 13);
            this.label143.TabIndex = 65559;
            this.label143.Text = "Attack Max";
            // 
            // textBox100
            // 
            this.textBox100.Location = new System.Drawing.Point(86, 33);
            this.textBox100.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox100.MaxLength = 32;
            this.textBox100.Name = "textBox100";
            this.textBox100.Size = new System.Drawing.Size(154, 20);
            this.textBox100.TabIndex = 65556;
            // 
            // label144
            // 
            this.label144.AutoSize = true;
            this.label144.Location = new System.Drawing.Point(15, 36);
            this.label144.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label144.Name = "label144";
            this.label144.Size = new System.Drawing.Size(58, 13);
            this.label144.TabIndex = 65557;
            this.label144.Text = "Attack Min";
            // 
            // groupBox16
            // 
            this.groupBox16.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox16.BackColor = System.Drawing.Color.Transparent;
            this.groupBox16.Controls.Add(this.textBox98);
            this.groupBox16.Controls.Add(this.label142);
            this.groupBox16.Controls.Add(this.textBox97);
            this.groupBox16.Controls.Add(this.label141);
            this.groupBox16.Controls.Add(this.button59);
            this.groupBox16.ForeColor = System.Drawing.Color.Black;
            this.groupBox16.Location = new System.Drawing.Point(253, 102);
            this.groupBox16.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox16.Size = new System.Drawing.Size(276, 264);
            this.groupBox16.TabIndex = 65551;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Add Boss";
            // 
            // textBox98
            // 
            this.textBox98.Location = new System.Drawing.Point(82, 73);
            this.textBox98.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox98.MaxLength = 32;
            this.textBox98.Name = "textBox98";
            this.textBox98.Size = new System.Drawing.Size(154, 20);
            this.textBox98.TabIndex = 57;
            // 
            // label142
            // 
            this.label142.AutoSize = true;
            this.label142.Location = new System.Drawing.Point(11, 76);
            this.label142.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label142.Name = "label142";
            this.label142.Size = new System.Drawing.Size(64, 13);
            this.label142.TabIndex = 58;
            this.label142.Text = "Boss Name:";
            // 
            // textBox97
            // 
            this.textBox97.Location = new System.Drawing.Point(82, 37);
            this.textBox97.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox97.MaxLength = 32;
            this.textBox97.Name = "textBox97";
            this.textBox97.Size = new System.Drawing.Size(154, 20);
            this.textBox97.TabIndex = 55;
            // 
            // label141
            // 
            this.label141.AutoSize = true;
            this.label141.Location = new System.Drawing.Point(11, 40);
            this.label141.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label141.Name = "label141";
            this.label141.Size = new System.Drawing.Size(47, 13);
            this.label141.TabIndex = 56;
            this.label141.Text = "Boss ID:";
            // 
            // button59
            // 
            this.button59.BackColor = System.Drawing.Color.White;
            this.button59.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button59.Location = new System.Drawing.Point(82, 110);
            this.button59.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button59.Name = "button59";
            this.button59.Size = new System.Drawing.Size(154, 25);
            this.button59.TabIndex = 54;
            this.button59.Text = "Add";
            this.button59.UseVisualStyleBackColor = true;
            this.button59.Click += new System.EventHandler(this.AddingBoss);
            // 
            // button58
            // 
            this.button58.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button58.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.button58.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button58.Location = new System.Drawing.Point(253, 550);
            this.button58.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button58.Name = "button58";
            this.button58.Size = new System.Drawing.Size(1056, 29);
            this.button58.TabIndex = 65550;
            this.button58.Text = "Save Selected";
            this.button58.UseVisualStyleBackColor = false;
            this.button58.Click += new System.EventHandler(this.SaveBossManager);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(253, 392);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(1056, 170);
            this.dataGridView1.TabIndex = 0;
            // 
            // listBox3
            // 
            this.listBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.listBox3.FormattingEnabled = true;
            this.listBox3.Location = new System.Drawing.Point(1158, 102);
            this.listBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(151, 238);
            this.listBox3.TabIndex = 65555;
            this.listBox3.SelectedIndexChanged += new System.EventHandler(this.SelectedBossIndex);
            // 
            // button61
            // 
            this.button61.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button61.BackColor = System.Drawing.Color.White;
            this.button61.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button61.Location = new System.Drawing.Point(1158, 367);
            this.button61.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button61.Name = "button61";
            this.button61.Size = new System.Drawing.Size(152, 23);
            this.button61.TabIndex = 65553;
            this.button61.Text = "Rerfesh";
            this.button61.UseVisualStyleBackColor = true;
            this.button61.Click += new System.EventHandler(this.RefreshBossAlive);
            // 
            // tabPage11
            // 
            this.tabPage11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage11.Controls.Add(this.label159);
            this.tabPage11.Controls.Add(this.label158);
            this.tabPage11.Controls.Add(this.button30);
            this.tabPage11.Controls.Add(this.dataGridView5);
            this.tabPage11.Controls.Add(this.label140);
            this.tabPage11.Controls.Add(this.button60);
            this.tabPage11.Controls.Add(this.dataGridView2);
            this.tabPage11.Location = new System.Drawing.Point(4, 22);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage11.Size = new System.Drawing.Size(1566, 705);
            this.tabPage11.TabIndex = 10;
            this.tabPage11.Text = "Tournament Management";
            this.tabPage11.UseVisualStyleBackColor = true;
            // 
            // label159
            // 
            this.label159.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label159.AutoSize = true;
            this.label159.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label159.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label159.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label159.Location = new System.Drawing.Point(671, 99);
            this.label159.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label159.Name = "label159";
            this.label159.Size = new System.Drawing.Size(266, 16);
            this.label159.TabIndex = 65609;
            this.label159.Text = "TOURNAMENT TIMER LIST (START/STOP )";
            // 
            // label158
            // 
            this.label158.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label158.AutoSize = true;
            this.label158.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label158.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label158.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label158.Location = new System.Drawing.Point(671, 350);
            this.label158.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label158.Name = "label158";
            this.label158.Size = new System.Drawing.Size(224, 16);
            this.label158.TabIndex = 65608;
            this.label158.Text = "TOURNAMENT PRIZES LIST (ITEMS)";
            // 
            // button30
            // 
            this.button30.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.button30.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button30.Location = new System.Drawing.Point(252, 570);
            this.button30.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button30.Name = "button30";
            this.button30.Size = new System.Drawing.Size(1058, 29);
            this.button30.TabIndex = 65606;
            this.button30.Text = "Save Selected";
            this.button30.UseVisualStyleBackColor = true;
            this.button30.Click += new System.EventHandler(this.SaveTournamentPrizes);
            // 
            // dataGridView5
            // 
            this.dataGridView5.AllowUserToAddRows = false;
            this.dataGridView5.AllowUserToDeleteRows = false;
            this.dataGridView5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataGridView5.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView5.ColumnHeadersHeight = 29;
            this.dataGridView5.Location = new System.Drawing.Point(252, 367);
            this.dataGridView5.Name = "dataGridView5";
            this.dataGridView5.RowHeadersWidth = 51;
            this.dataGridView5.Size = new System.Drawing.Size(1058, 197);
            this.dataGridView5.TabIndex = 65607;
            // 
            // label140
            // 
            this.label140.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label140.AutoSize = true;
            this.label140.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label140.Font = new System.Drawing.Font("Tahoma", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label140.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label140.Location = new System.Drawing.Point(589, 409);
            this.label140.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label140.Name = "label140";
            this.label140.Size = new System.Drawing.Size(370, 25);
            this.label140.TabIndex = 65605;
            this.label140.Text = "BOSSES MANAGER INFORMATION";
            // 
            // button60
            // 
            this.button60.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button60.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.button60.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button60.Location = new System.Drawing.Point(252, 320);
            this.button60.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button60.Name = "button60";
            this.button60.Size = new System.Drawing.Size(1058, 29);
            this.button60.TabIndex = 65553;
            this.button60.Text = "Save Selected";
            this.button60.UseVisualStyleBackColor = true;
            this.button60.Click += new System.EventHandler(this.TournamentSchedules);
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView2.ColumnHeadersHeight = 29;
            this.dataGridView2.Location = new System.Drawing.Point(252, 116);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidth = 51;
            this.dataGridView2.Size = new System.Drawing.Size(1058, 200);
            this.dataGridView2.TabIndex = 65554;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.TxtLogger);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(1566, 705);
            this.tabPage8.TabIndex = 11;
            this.tabPage8.Text = "Console";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // button53
            // 
            this.button53.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button53.BackColor = System.Drawing.Color.SteelBlue;
            this.button53.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button53.ForeColor = System.Drawing.SystemColors.Control;
            this.button53.Location = new System.Drawing.Point(743, 748);
            this.button53.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button53.Name = "button53";
            this.button53.Size = new System.Drawing.Size(222, 31);
            this.button53.TabIndex = 65606;
            this.button53.Text = "SHOW LAST REWARDS LIST";
            this.button53.UseVisualStyleBackColor = false;
            this.button53.Click += new System.EventHandler(this.FetchEventRewards);
            // 
            // label36
            // 
            this.label36.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label36.AutoSize = true;
            this.label36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label36.Font = new System.Drawing.Font("Tahoma", 19F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label36.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label36.Location = new System.Drawing.Point(502, 748);
            this.label36.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(222, 31);
            this.label36.TabIndex = 65605;
            this.label36.Text = "LAST REWARDS";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 883);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1838, 24);
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 65538;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(75, 19);
            this.toolStripStatusLabel3.Text = "Login Status:";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.ActiveLinkColor = System.Drawing.Color.Red;
            this.toolStripStatusLabel4.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel4.ForeColor = System.Drawing.Color.Red;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(43, 19);
            this.toolStripStatusLabel4.Text = "Offline";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(76, 19);
            this.toolStripStatusLabel1.Text = "Game Status:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.Red;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(43, 19);
            this.toolStripStatusLabel2.Text = "Offline";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 18);
            // 
            // checkBox14
            // 
            this.checkBox14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox14.AutoSize = true;
            this.checkBox14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.checkBox14.ForeColor = System.Drawing.Color.Transparent;
            this.checkBox14.Location = new System.Drawing.Point(262, 455);
            this.checkBox14.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox14.Name = "checkBox14";
            this.checkBox14.Size = new System.Drawing.Size(148, 19);
            this.checkBox14.TabIndex = 65609;
            this.checkBox14.Text = "DISABLE MOB SPAWN";
            this.checkBox14.UseVisualStyleBackColor = false;
            this.checkBox14.CheckedChanged += new System.EventHandler(this.checkBox14_CheckedChanged);
            // 
            // ServerManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ClientSize = new System.Drawing.Size(1838, 907);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.button53);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.LbxCharacters);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1364, 726);
            this.Name = "ServerManager";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Server Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseServer);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumPlayerCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown21)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabPage7.ResumeLayout(false);
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            this.tabPage10.ResumeLayout(false);
            this.tabPage10.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage11.ResumeLayout(false);
            this.tabPage11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
