using TheChosenProject.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.WindowsAPI;
using TheChosenProject;
//using TheChosenProject.Game.Rayzo;
using System.IO;
using MySql.Data.MySqlClient;
using MySqlCommand = TheChosenProject.Database.MySqlCommand;
using DevExpress.XtraEditors.TextEditController;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using TheChosenProject.Game.Rayzo;

namespace TheChosenProject
{
    public partial class Rayzo_Panle : DevExpress.XtraEditors.XtraForm
    {
        public Rayzo_Panle()
        {
            InitializeComponent();
            loadePlayerName();
        }
        #region old
        //#region Buttn Panle
        //// accuont data
        //private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage1;
        //    LoadeAllUserName();//لود لاسماء الحسابات

        //}
        ////new accuont
        //private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage2;
        //}
        //private void barButtonItem19_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage3;
        //}
        //private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage4;
        //}


        //private void PuttnSpill_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage5;
        //    loadePlayerName();
        //    using (var rec = new ServerSockets.RecycledPacket())
        //    {
        //        var stream = rec.GetStream();
        //        foreach (var spell in Database.Server.Magic.Values)
        //        {
        //            if (spell.Keys.Count > 0)
        //            {
        //                var sp = spell[(ushort)(spell.Keys.Count - 1)];

        //                SpellList.Items.Add(sp.Name + "-" + sp.ID);

        //            }
        //        }

        //    }
        //}

        //private void SkillTrojan_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage11;
        //}
        //private void SkillFire_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage12;
        //}

        //private void SkillArcher_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage13;
        //}

        //private void SkillWarrior_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage14;
        //}

        //private void SkillMonk_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage16;
        //}

        //private void SkillNinja_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage15;
        //}

        //private void SkillParite_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage17;
        //}
        //private void BtnWaterSkll_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage18;
        //}
        //private void BtnWater_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage18;
        //}

        //private void DragonWarrir_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage19;
        //}

        //private void WindWilker_Click(object sender, EventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage20;
        //}
        //private void AllChar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    navigationFrame1.SelectedPage = navigationPage10;
        //    LoadAttack();
        //    UpdateTxt(); // اضافه القيم فى التيكست
        //}
        ////تحميل اسماء الاعبين
        //private void loadePlayerName()
        //{
        //    SpellUserName.Items.Clear();
        //    Status_AllUser.Items.Clear();
        //    CBPlayerName.Items.Clear();

        //    if (Database.Server.GamePoll != null)
        //    {
        //        foreach (var user in Database.Server.GamePoll.Values)
        //        {
        //            SpellUserName.Items.Add(user.Player.Name);
        //            Status_AllUser.Items.Add(user.Player.Name);
        //            CBPlayerName.Items.Add(user.Player.Name);


        //        }
        //    }
        //}
        //private void SendMessagebox(string Msg, string Snedr = "MrRayzo", MessageBoxButtons Buttun = MessageBoxButtons.OK, MessageBoxIcon Icon = MessageBoxIcon.Information)
        //{
        //    MessageBox.Show(Msg, Snedr, Buttun, Icon);
        //}
        //#endregion
        #endregion
        #region Buttn Panle
        // accuont data
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage1;
            LoadeAllUserName();//لود لاسماء الحسابات

        }
        //new accuont
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage2;
        }
        private void barButtonItem19_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage3;
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage4;
        }
        private void ButtanBan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage6;
        }
        private void BtnChatAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage8;
        }
        private void PuttnSpill_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage5;
            loadePlayerName();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (var spell in Database.Server.Magic.Values)
                {
                    if (spell.Keys.Count > 0)
                    {
                        var sp = spell[(ushort)(spell.Keys.Count - 1)];

                        SpellList.Items.Add(sp.Name + "-" + sp.ID);

                    }
                }

            }
        }
        private void ButtnChatPlaer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage7;
            loadChatPanl();
        }
        
        private void SkillTrojan_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage11;
        }
        private void SkillFire_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage12;
        }

        private void SkillArcher_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage13;
        }

        private void SkillWarrior_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage14;
        }

        private void SkillMonk_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage16;
        }

        private void SkillNinja_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage15;
        }

        private void SkillParite_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage17;
        }
        private void BtnWaterSkll_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage18;
        }
        private void BtnWater_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage18;
        }

        private void DragonWarrir_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage19;
        }

        private void WindWilker_Click(object sender, EventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage20;
        }
        private void AllChar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage10;
            LoadAttack();
            UpdateTxt(); // اضافه القيم فى التيكست
        }
        //تحميل اسماء الاعبين
        private void loadePlayerName()
        {
            SpellUserName.Items.Clear();
            Status_AllUser.Items.Clear();
            CBPlayerName.Items.Clear();
            BanPlayerName.Items.Clear();
            ComName.Items.Clear();
            ComName.Items.Clear();
            ChiPlayerName.Items.Clear();
            if (Database.Server.GamePoll != null)
            {
                foreach (var user in Database.Server.GamePoll.Values)
                {
                    SpellUserName.Items.Add(user.Player.Name);
                    Status_AllUser.Items.Add(user.Player.Name);
                    CBPlayerName.Items.Add(user.Player.Name);
                    BanPlayerName.Items.Add(user.Player.Name);
                    ComName.Items.Add(user.Player.Name);
                    ChiPlayerName.Items.Add(user.Player.Name);

                }
            }
        }
        private void SendMessagebox(string Msg, string Snedr = "MrRayzo", MessageBoxButtons Buttun = MessageBoxButtons.OK, MessageBoxIcon Icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(Msg, Snedr, Buttun, Icon);
        }
        #endregion
        #region Class Accuont

        private void LoadeAllUserName()
        {
            try
            {
                Accuont_AllUser.Items.Clear();
                MySqlReader reader = new MySqlReader(new Database.MySqlCommand(MySqlCommandType.SELECT).Select("accounts"));
                while (reader.Read())
                {
                    Accuont_AllUser.Items.Add(reader.ReadLongString("Username"));

                }
            }
            catch (Exception e) { SendMessagebox(e.Message); }
        }
        private void AddNewAcc_Click(object sender, EventArgs e)
        {
            try
            {
                if (AccName.Text != "")
                {
                    var cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("accounts").Insert("Username", AccName.Text).Insert("Password", AccPass.Text).Insert("Email", AccEmail.Text).Execute();
                    System.Windows.Forms.MessageBox.Show("Done");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void AccDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (Accuont_AllUser.SelectedItem != null)
                {
                    var cmd = new MySqlCommand(MySqlCommandType.DELETE);
                    cmd.Delete("accounts", "Username", Accuont_AllUser.SelectedItem.ToString());
                    cmd.Execute2();
                    SendMessagebox("Done Delete");
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void Change_Click(object sender, EventArgs e)
        {
            try
            {
                if (NewID.Text != "")
                {
                    var cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmd.Update("accounts").Set("Username", NewName.Text).Set("Password", NewPass.Text).Set("Email", NewEmil.Text)
                    .Where("EntityID", NewID.Text);
                    if (cmd.Execute2() > 0)
                    {
                        SendMessagebox("Done Save");

                    }
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void Accuont_AllUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Accuont_AllUser.SelectedItem != null)
                {
                    using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
                    {
                        cmd.Select("accounts").Where("Username", Accuont_AllUser.SelectedItem.ToString());
                        using (MySqlReader rdr = new MySqlReader(cmd))
                        {
                            if (rdr.Read())
                            {

                                Acc_UID.Text = rdr.ReadInt32("EntityID").ToString();
                                Acc_UserName.Text = rdr.ReadString("Username");
                                Acc_Pass.Text = rdr.ReadString("Password");
                                Acc_Emil.Text = rdr.ReadString("Email");
                                //Acc_Mac.Text = rdr.ReadString("EarthID");
                                Acc_Ip.Text = rdr.ReadString("IP");

                                //
                                NewID.Text = rdr.ReadInt32("EntityID").ToString();
                                NewName.Text = rdr.ReadString("Username");
                                NewPass.Text = rdr.ReadString("Password");
                                NewEmil.Text = rdr.ReadString("Email");
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Username not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        #endregion

        #region Class Stauts

        private void Status_Ref_Click(object sender, EventArgs e)
        {
            loadePlayerName();
        }
        private void Status_AllUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCharInfo();
        }
        private void LoadCharInfo()
        {
            try
            {
                TheChosenProject.Client.GameClient client = null;
                client = TheChosenProject.Client.GameClient.CharacterFromName(Status_AllUser.SelectedItem.ToString());
                if (client == null)
                    return;
                CPS.Text = client.Player.ConquerPoints.ToString();
                BCps.Text = client.Player.ChampionPoints.ToString();
                banck_Cps.Text = client.Player.ChampionPoints.ToString();
                Money.Text = client.Player.Money.ToString();
                Level.Text = client.Player.Level.ToString();
                WhPass.Text = client.Player.SecurityPassword.ToString();
                if (client.Player.MyGuild != null)
                    GuildName.Text = client.Player.MyGuild.GuildName;
                if (client.Player.MyClan != null)
                    Clan.Text = client.Player.ClanName;
                else
                    Clan.Text = "None";
                Sex.Text = client.Player.GetGender.ToString();
                switch (client.Player.Reborn)
                {
                    case 2: Reborn.Text = "2nd Reborn"; break;
                    case 1: Reborn.Text = "1st Reborn"; break;
                    default: Reborn.Text = "None"; break;
                }
                switch (client.Player.Class)
                {
                    #region Get Class
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        {
                            Class.Text = "Trojan";
                            break;
                        }
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                        {
                            Class.Text = "Warrior";
                            break;
                        }
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                        {
                            Class.Text = "Archer";
                            break;
                        }
                    case 50:
                    case 51:
                    case 52:
                    case 53:
                    case 54:
                    case 55:
                        {
                            Class.Text = "Ninja";
                            break;
                        }
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                    case 65:
                        {
                            Class.Text = "Monk";
                            break;
                        }
                    case 130:
                    case 131:
                    case 132:
                    case 133:
                    case 134:
                    case 135:
                        {
                            Class.Text = "Water";
                            break;
                        }
                    case 140:
                    case 141:
                    case 142:
                    case 143:
                    case 144:
                    case 145:
                        {
                            Class.Text = "Fire";
                            break;
                        }
                    default: Class.Text = "Taoist"; break;
                        #endregion
                }
                double x = 0;
                if ((client.Player.ExpireVip > DateTime.Now))
                {
                    x = (client.Player.ExpireVip - DateTime.Now).TotalDays;
                }
                VIP.Text = client.Player.VipLevel.ToString();
                VipDay.Text = x.ToString();
                OnlinePoint.Text = client.Player.OnlinePoints.ToString();
                Status_Sbouse.Text = client.Player.Spouse;
                BanckMoney.Text = client.Player.WHMoney.ToString();
                VotePoint.Text = client.Player.ChampionPoints.ToString();
                PkPoint.Text = client.Player.PKPoints.ToString();
                Donation.Text = client.Player.Nobility.Donation.ToString();
                Rank.Text = client.Player.NobilityRank.ToString();
                Donate.Text = client.Player.ChampionPoints.ToString();
                Agility.Text = client.Player.Agility.ToString();
                Strength.Text = client.Player.Strength.ToString();
                Status_Vit.Text = client.Player.Vitality.ToString();
                Spirit.Text = client.Player.Spirit.ToString();
                Atributes.Text = client.Player.Atributes.ToString();
                Attack.Text = client.Status.MaxAttack.ToString();
                MinAttack.Text = client.Status.MinAttack.ToString();
                Defnce.Text = client.Status.Defence.ToString();
                HitPoint.Text = client.Status.MaxHitpoints.ToString();
                Mana.Text = client.Status.MaxMana.ToString();
                Power.Text = client.Player.BattlePower.ToString();
                Mg_Attack.Text = client.Status.MagicAttack.ToString();
                Mg_Defen.Text = client.Status.MagicDefence.ToString();
                Bless.Text = client.Status.ItemBless.ToString();
                Map.Text = client.Player.Map.ToString();
                X.Text = client.Player.X.ToString();
                Y.Text = client.Player.Y.ToString();
                Mac.Text = client.OnLogin.MacAddress.ToString();
                UID.Text = client.Player.UID.ToString();
                UID.Text = client.Player.UID.ToString();
                using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
                {
                    cmd.Select("accounts").Where("EntityID", client.Player.UID);
                    using (MySqlReader rdr = new MySqlReader(cmd))
                    {
                        if (rdr.Read())
                        {

                            UserName.Text = rdr.ReadUInt32("Username").ToString();
                            Pass.Text = rdr.ReadUInt32("Password").ToString();
                            IP.Text = rdr.ReadUInt32("IP").ToString();
                            //Mac.Text = rdr.ReadUInt32("EarthID").ToString();

                        }

                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
        private void SaveCharInfo()
        {
            try
            {
                TheChosenProject.Client.GameClient client = null;
                if (Status_AllUser.SelectedItem == null) return;
                client = TheChosenProject.Client.GameClient.CharacterFromName(Status_AllUser.SelectedItem.ToString());
                if (client == null)
                    return;
                client.Player.Map = Convert.ToUInt16(Map.Text);
                client.Player.X = Convert.ToUInt16(X.Text);
                client.Player.Y = Convert.ToUInt16(Y.Text);
                client.Player.ConquerPoints = (int)Convert.ToUInt32(CPS.Text);
                client.Player.ChampionPoints = Convert.ToUInt16(BCps.Text);
                client.Player.ChampionPoints = (int)Convert.ToUInt32(banck_Cps.Text);
                client.Player.Money = (int)Convert.ToUInt32(Money.Text);
                client.Player.Level = Convert.ToByte(Level.Text);
                client.Player.SecurityPassword = Convert.ToUInt32(WhPass.Text);
                if (Class.Text == "15" || Class.Text == "25" || Class.Text == "45" || Class.Text == "55" || Class.Text == "65" || Class.Text == "75" || Class.Text == "135" || Class.Text == "145")
                    client.Player.Class = Convert.ToByte(Class.Text);
                if (Reborn.Text == "1" || Reborn.Text == "2")
                    client.Player.Reborn = Convert.ToByte(Reborn.Text);
                client.Player.ExpireVip = DateTime.Now.AddDays(double.Parse(VipDay.Text));
                client.Player.VipLevel = byte.Parse(VIP.Text);
                client.Player.ChampionPoints = (int)Convert.ToUInt32(OnlinePoint.Text);
                client.Player.WHMoney = Convert.ToUInt32(BanckMoney.Text);
                client.Player.ChampionPoints = (int)Convert.ToUInt32(VotePoint.Text);
                client.Player.PKPoints = Convert.ToUInt16(PkPoint.Text);
                client.Player.Nobility.Donation = Convert.ToUInt64(Donation.Text);
                client.Player.ChampionPoints = (int)Convert.ToUInt32(Donate.Text);
                client.Player.Agility = Convert.ToUInt16(Agility.Text);
                client.Player.Strength = Convert.ToUInt16(Strength.Text);
                client.Player.Spirit = Convert.ToUInt16(Spirit.Text);
                client.Player.Vitality = Convert.ToUInt16(Vitality.Text);
                client.Player.Atributes = Convert.ToUInt16(Atributes.Text);
                client.Status.MinAttack = Convert.ToUInt32(MinAttack.Text);
                client.Status.MaxAttack = Convert.ToUInt32(Attack.Text);
                client.Status.Defence = Convert.ToUInt32(Defnce.Text);
                client.Status.MaxHitpoints = Convert.ToUInt32(HitPoint.Text);
                client.Status.MaxMana = Convert.ToUInt32(Mana.Text);
                client.Status.MagicAttack = Convert.ToUInt32(Mg_Attack.Text);
                client.Status.MagicDefence = Convert.ToUInt32(Mg_Defen.Text);
                client.Status.ItemBless = Convert.ToUInt32(Bless.Text);
                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Player.SendUpdate(stream, client.Player.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                    client.Player.SendUpdate(stream, client.Player.ChampionPoints, Game.MsgServer.MsgUpdate.DataType.RaceShopPoints);
                    client.Player.SendUpdate(stream, client.Player.ConquerPoints, Game.MsgServer.MsgUpdate.DataType.ConquerPoints);
                    client.Player.SendUpdate(stream, client.Player.OnlinePoints, Game.MsgServer.MsgUpdate.DataType.BoundConquerPoints);
                    client.Player.SendUpdate(stream, client.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                    client.Player.SendUpdate(stream, client.Player.Level, Game.MsgServer.MsgUpdate.DataType.Level);
                    client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                    client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                    client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                    client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                    client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                    client.Player.SendUpdate(stream, client.Player.WHMoney, Game.MsgServer.MsgUpdate.DataType.WHMoney);
                    client.Send(stream.NobilityIconCreate(client.Player.Nobility));
                    Program.NobilityRanking.UpdateRank(client.Player.Nobility);
                }


            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void Save_Click(object sender, EventArgs e)
        {
            SaveCharInfo();
        }
        #endregion

        #region Class Items
        private void ButtunItems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                navigationFrame1.SelectedPage = navigationPage4;
                loadePlayerName();
                CBAddItems.Items.Clear();
                //لود الايتمز
                foreach (var item in TheChosenProject.Database.Server.ItemsBase.Values)
                {
                    CBAddItems.Items.Add(item.Name + "_" + item.ID);
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }

        }
        //اضافه ايتمز
        private void AddItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (CBPlayerName.SelectedItem != null)
                {
                    TheChosenProject.Client.GameClient client = null;
                    client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                    if (client == null)
                        return;

                    TheChosenProject.Database.ItemType.DBItem DBItem;
                    //string[] id = CBAddItems.SelectedItem.ToString().Split(' ').ToArray();
                    string[] id = CBAddItems.SelectedItem.ToString().Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (TheChosenProject.Database.Server.ItemsBase.TryGetValue(uint.Parse(id[1]), out DBItem))
                    {
                        using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            byte bound = byte.Parse(this.AddBound.Text);
                            bool Newbound = false;
                            TheChosenProject.Role.Flags.Gem soc1;
                            TheChosenProject.Role.Flags.Gem soc2;
                            if (bound == 1) Newbound = true; else Newbound = false;
                            if (AddSoc1.Text == "1") soc1 = TheChosenProject.Role.Flags.Gem.SuperDragonGem; else soc1 = TheChosenProject.Role.Flags.Gem.NoSocket;
                            if (AddSoc2.Text == "1") soc2 = TheChosenProject.Role.Flags.Gem.SuperDragonGem; else soc2 = TheChosenProject.Role.Flags.Gem.NoSocket;
                            client.Inventory.Add(stream, uint.Parse(id[1]), byte.Parse(this.AddCount.Text), byte.Parse(this.AddPlus.Text), byte.Parse(this.AddBless.Text), byte.Parse(this.AddHP.Text), soc1, soc2, Newbound);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Plase Chose PlayerName");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
        private void Load_Click(object sender, EventArgs e)
        {
            if (CBPlayerName.SelectedItem != null)
            {
                TheChosenProject.Client.GameClient client = null;
                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                if (client == null)
                    return;
                listBox2.Items.Clear();
                foreach (var itemx in client.Equipment.ClientItems.Values)
                {
                    if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                        continue;
                    listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Equipment]", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID));
                }
                foreach (var itemx in client.Inventory.ClientItems.Values)
                {
                    if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                        continue;
                    listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Inventory]", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID));
                }
                foreach (var ware in client.Warehouse.ClientItems.Values)
                {
                    foreach (var itemx in ware.Values)
                    {
                        if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                            continue;
                        listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Warehouse] @@ {2}", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID, itemx.WH_ID));
                    }
                }
            }
        }
        private void RemoveItem()
        {
            var c = listBox2.SelectedItem.ToString();
            string[] data = c.Split(new string[] { "@@", " " }, StringSplitOptions.RemoveEmptyEntries);
            Client.GameClient client = null;
            client = Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
            if (client == null)
                return;
            uint UID = Convert.ToUInt32(data[1]);
            MsgGameItem item = null;
            if (data[2] == "[Equipment]")
            {
                if (client.Equipment.ClientItems.ContainsKey(UID))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Equipment.ClientItems.TryRemove(UID, out item);
                        client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item.UID, item.Position, 0, 0, 0, 0));
                        client.Equipment.QueryEquipment(client.Equipment.Alternante);

                    }
                }
            }
            else if (data[2] == "[Inventory]")
            {
                client.Inventory.TryGetItem(UID, out item);
                if (item == null)
                    return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.Inventory.Remove(item.ITEM_ID, 1, stream);
                }

            }
            else if (data[2] == "[Warehouse]")
            {
                foreach (var WH in client.Warehouse.ClientItems.Values)
                {
                    if (WH.ContainsKey(UID))
                    {
                        WH.TryRemove(UID, out item);
                    }
                }
            }

            listBox2.Items.Clear();
            foreach (var itemx in client.Equipment.ClientItems.Values)
            {
                if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                    continue;
                listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Equipment]", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID));
            }
            foreach (var itemx in client.Inventory.ClientItems.Values)
            {
                if (itemx == null || !Server.ItemsBase.ContainsKey(itemx.ITEM_ID))
                    continue;
                listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Inventory]", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID));
            }
            foreach (var ware in client.Warehouse.ClientItems.Values)
            {
                foreach (var itemx in ware.Values)
                {
                    if (item == null || !Server.ItemsBase.ContainsKey(item.ITEM_ID))
                        continue;
                    listBox2.Items.Add(String.Format(" {0} @@ {1} @@ [Warehouse] @@ {2}", Server.ItemsBase[itemx.ITEM_ID].Name, itemx.UID, itemx.WH_ID));
                }
            }

        }

        private void RemoveSp_Click(object sender, EventArgs e)
        {
            RemoveItem();
        }
        private void AddStuff(byte Class)
        {
            byte Plus = byte.Parse(FullPlus.Text);
            byte Bless = byte.Parse(FullBless.Text);
            byte HP = byte.Parse(FullHP.Text);
            byte bound = byte.Parse(this.FullBond.Text);
            bool Newbound = false;
            TheChosenProject.Role.Flags.Gem soc1;
            TheChosenProject.Role.Flags.Gem soc2;
            if (bound == 1) Newbound = true; else Newbound = false;
            if (FullSoc1.Text == "1") soc1 = TheChosenProject.Role.Flags.Gem.SuperDragonGem; else soc1 = TheChosenProject.Role.Flags.Gem.NoSocket;
            if (FullSoc2.Text == "1") soc2 = TheChosenProject.Role.Flags.Gem.SuperDragonGem; else soc2 = TheChosenProject.Role.Flags.Gem.NoSocket;

            switch (Class)
            {
                case 15:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                TheChosenProject.Client.GameClient client = null;
                                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 410339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SkyBlade                                    
                                    client.Inventory.Add(stream, 420339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SquallSword                                    
                                    client.Inventory.Add(stream, 480339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //NirvanaClub                                    
                                    client.Inventory.Add(stream, 130109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //ObsidianArmor                                    
                                    client.Inventory.Add(stream, 118109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //PeerlessCoronet                                   
                                                                                                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 25:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                TheChosenProject.Client.GameClient client = null;
                                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 900109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //CelestialShield                                    
                                    client.Inventory.Add(stream, 561339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //OccultWand                                    
                                    client.Inventory.Add(stream, 560339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SpearOfWrath                                    
                                    client.Inventory.Add(stream, 131109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //ImperiousArmor                                    
                                    client.Inventory.Add(stream, 111109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SteelHelmet                                   
                                    }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 45:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                TheChosenProject.Client.GameClient client = null;
                                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 500329, 1, Plus, Bless, HP, soc1, soc2, Newbound); //HeavenlyBow                                    
                                    client.Inventory.Add(stream, 133109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //WelkinCoat                                    
                                    client.Inventory.Add(stream, 113109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //WhiteTigerHat                                   
                                    }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 55:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                TheChosenProject.Client.GameClient client = null;
                                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 601439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //HanzoKatana                                    
                                    client.Inventory.Add(stream, 601439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //HanzoKatana                                    
                                    client.Inventory.Add(stream, 511439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SilenceScythe                                    
                                    client.Inventory.Add(stream, 135309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //NightmareVest                                    
                                    client.Inventory.Add(stream, 112309, 1, Plus, Bless, HP, soc1, soc2, false); //RambleVeil                                   
                                    }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 65:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                TheChosenProject.Client.GameClient client = null;
                                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 610439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //LazuritePrayerBeads                                    
                                    client.Inventory.Add(stream, 610439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //LazuritePrayerBeads                                    
                                    client.Inventory.Add(stream, 136309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //WhiteLotusFrock                                    
                                    client.Inventory.Add(stream, 143309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //XumiCap                                   
                                    }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 75:
                    {
                        try
                        {
                            if (CBPlayerName.SelectedItem != null)
                            {
                                TheChosenProject.Client.GameClient client = null;
                                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 612439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //LordPistol                                    
                                    client.Inventory.Add(stream, 611439, 1, Plus, Bless, HP, soc1, soc2, Newbound); //CaptainRapier                                    
                                    client.Inventory.Add(stream, 139309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //DarkDragonCoat                                    
                                    client.Inventory.Add(stream, 144309, 1, Plus, Bless, HP, soc1, soc2, Newbound); //DominatorHat                                   
                                    }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 135:
                    {
                        try
                        {
                            if (FullSoc1.Text == "1") soc1 = Role.Flags.Gem.SuperTortoiseGem; else soc1 = Role.Flags.Gem.NoSocket;
                            if (FullSoc1.Text == "1") soc2 = Role.Flags.Gem.SuperTortoiseGem; else soc2 = Role.Flags.Gem.NoSocket;
                            if (CBPlayerName.SelectedItem != null)
                            {
                                TheChosenProject.Client.GameClient client = null;
                                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 120269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Necklace                                    
                                    client.Inventory.Add(stream, 150269, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Ring                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot  
                                    client.Inventory.Add(stream, 560339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SpearOfWrath                                    
                                    client.Inventory.Add(stream, 134109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //EternalRobe                                    
                                    client.Inventory.Add(stream, 114109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //DistinctCap                                  
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }
                case 145:
                    {
                        try
                        {
                            if (FullSoc1.Text == "1") soc1 = Role.Flags.Gem.SuperPhoenixGem; else soc1 = Role.Flags.Gem.NoSocket;
                            if (FullSoc2.Text == "1") soc2 = Role.Flags.Gem.SuperPhoenixGem; else soc2 = Role.Flags.Gem.NoSocket;
                            if (CBPlayerName.SelectedItem != null)
                            {
                                TheChosenProject.Client.GameClient client = null;
                                client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                                if (client == null)
                                    return;
                                if (!client.Inventory.HaveSpace(12))
                                {
                                    System.Windows.Forms.MessageBox.Show("Character must have 12 free slots into inventory");
                                    return;
                                }
                                using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 121249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //NiftyBag                                    
                                    client.Inventory.Add(stream, 152259, 1, Plus, Bless, HP, soc1, soc2, Newbound); //WyvernBracelet                                    
                                    client.Inventory.Add(stream, 160249, 1, Plus, Bless, HP, soc1, soc2, Newbound); //Boot   
                                    client.Inventory.Add(stream, 421339, 1, Plus, Bless, HP, soc1, soc2, Newbound); //SupremeSword                                    
                                    client.Inventory.Add(stream, 134109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //EternalRobe                                    
                                    client.Inventory.Add(stream, 114109, 1, Plus, Bless, HP, soc1, soc2, Newbound); //DistinctCap                                   
                                    }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        break;
                    }

            }
        }

        private void StuffNinja_Click(object sender, EventArgs e)
        {
            AddStuff(55);
        }

        private void StuffMonk_Click(object sender, EventArgs e)
        {
            AddStuff(65);
        }

        private void StuffParte_Click(object sender, EventArgs e)
        {
            AddStuff(75);
        }

        private void StuffArcher_Click(object sender, EventArgs e)
        {
            AddStuff(45);
        }

        private void StuffWarrior_Click(object sender, EventArgs e)
        {
            AddStuff(25);
        }

        private void StuffTrojan_Click(object sender, EventArgs e)
        {
            AddStuff(15);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            AddStuff(145);
        }

        private void StuffTaoist_Click(object sender, EventArgs e)
        {
            AddStuff(135);
        }

        private void SpishelItems(int indx)
        {
            switch (indx)
            {
                case 0:
                    {//cup
                        if (CBPlayerName.SelectedItem != null)
                        {
                            TheChosenProject.Client.GameClient client = null;
                            client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                            if (!client.Inventory.HaveSpace(1))
                            {
                                System.Windows.Forms.MessageBox.Show("Character must have 1 free slots into inventory");
                                return;
                            }
                            using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Inventory.Add(stream, 2100095, 1);
                            }
                        }
                        break;
                    }
                case 1:
                    {//vip 1 days
                        if (CBPlayerName.SelectedItem != null)
                        {
                            TheChosenProject.Client.GameClient client = null;
                            client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;

                            using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (DateTime.Now > client.Player.ExpireVip || client.Player.VipLevel != 7)
                                    client.Player.ExpireVip = DateTime.Now.AddDays(7);
                                else
                                    client.Player.ExpireVip = client.Player.ExpireVip.AddDays(7);
                                client.Player.VipLevel = 7;
                                client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                                client.Player.UpdateVip(stream);
                                client.CreateBoxDialog("You`ve received VIP 7 (7 day).");
                            }
                        }
                        break;
                    }
                case 2:
                    {//vip 30 days
                        if (CBPlayerName.SelectedItem != null)
                        {
                            TheChosenProject.Client.GameClient client = null;
                            client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;

                            using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (DateTime.Now > client.Player.ExpireVip || client.Player.VipLevel != 7)
                                    client.Player.ExpireVip = DateTime.Now.AddDays(30);
                                else
                                    client.Player.ExpireVip = client.Player.ExpireVip.AddDays(30);
                                client.Player.VipLevel = 7;
                                client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                                client.Player.UpdateVip(stream);
                                client.CreateBoxDialog("You`ve received VIP 7 (30 day).");
                            }
                        }
                        break;
                    }
                case 3:
                    {//Soul 6
                        if (CBPlayerName.SelectedItem != null)
                        {
                            TheChosenProject.Client.GameClient client = null;
                            client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                            if (!client.Inventory.HaveSpace(25))
                            {
                                System.Windows.Forms.MessageBox.Show("Character must have 25 free slots into inventory");
                                return;
                            }
                            using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                var array = Database.ItemType.PurificationItems[6].Values.ToArray();
                                foreach (var item in array)
                                {
                                    client.Inventory.Add(stream, item.ID, 1);
                                }

                            }
                        }
                        break;
                    }
                case 4:
                    {//Soul 7
                        if (CBPlayerName.SelectedItem != null)
                        {
                            TheChosenProject.Client.GameClient client = null;
                            client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                            if (!client.Inventory.HaveSpace(25))
                            {
                                System.Windows.Forms.MessageBox.Show("Character must have 25 free slots into inventory");
                                return;
                            }
                            using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                var array = Database.ItemType.PurificationItems[7].Values.ToArray();
                                foreach (var item in array)
                                {
                                    client.Inventory.Add(stream, item.ID, 1);
                                }

                            }
                        }
                        break;
                    }
                case 5:
                    {//Ref 6
                        if (CBPlayerName.SelectedItem != null)
                        {
                            TheChosenProject.Client.GameClient client = null;
                            client = TheChosenProject.Client.GameClient.CharacterFromName(CBPlayerName.SelectedItem.ToString());
                            if (client == null)
                                return;
                            if (!client.Inventory.HaveSpace(15))
                            {
                                System.Windows.Forms.MessageBox.Show("Character must have 15 free slots into inventory");
                                return;
                            }
                            using (var rec = new TheChosenProject.ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                var array = Database.ItemType.Refinary[5].Values.ToArray();
                                foreach (var item in array)
                                {
                                    client.Inventory.Add(stream, item.ItemID, 1);
                                }

                            }
                        }
                        break;
                    }

            }
        }

        private void CBSpecial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBSpecial.SelectedIndex >= 0)
            {
                SpishelItems(CBSpecial.SelectedIndex);
            }
        }
        #endregion

        #region Class Spell

        private void AddSpell_Click(object sender, EventArgs e)
        {
            try
            {
                ushort ID = ushort.Parse(AddID.Text);
                byte Level = byte.Parse(AddLevel.Text);
                int exp = int.Parse(AddExp.Text);
                if (SpellUserName.SelectedItem != null)
                {
                    TheChosenProject.Client.GameClient client = null;
                    client = TheChosenProject.Client.GameClient.CharacterFromName(SpellUserName.SelectedItem.ToString());
                    if (client == null)
                        return;
                    using (var rec = new ServerSockets.RecycledPacket())
                        client.MySpells.Add(rec.GetStream(), (ushort)ID, Level, 0, 0, exp);
                    SendMessagebox("Done Add This Spell " + ID + "");
                }
                else
                {
                    MessageBox.Show("Plase Select The Player Name");
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void SpellList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string[] Arry = SpellList.SelectedItem.ToString().Split('-');
                AddSpellName.Text = Arry[0];
                AddID.Text = Arry[1];
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void LoadeRemoveSpell()
        {
            try
            {
                if (SpellUserName.SelectedItem != null)
                {
                    TheChosenProject.Client.GameClient client = null;
                    client = TheChosenProject.Client.GameClient.CharacterFromName(SpellUserName.SelectedItem.ToString());
                    if (client == null)
                        return;
                    PlayerSpell.Items.Clear();
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        foreach (var spell in Database.Server.Magic.Values)
                        {
                            if (spell.Keys.Count > 0)
                            {
                                var sp = spell[(ushort)(spell.Keys.Count - 1)];

                                foreach (var spelle in client.MySpells.ClientSpells.Values)
                                {
                                    if (sp.ID == spelle.ID)
                                    {
                                        PlayerSpell.Items.Add(sp.Name + "-" + sp.ID);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }
        }
        private void SpellUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadeRemoveSpell();
        }
        private bool done = false;
        private void PlayerSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!done)
            {
                LoadeRemoveSpell();
                done = true;
            }
            if (PlayerSpell.SelectedItem != null)
            {
                string[] Arry = PlayerSpell.SelectedItem.ToString().Split('-');
                RemoveName.Text = Arry[0];
                RemoveID.Text = Arry[1];
            }
        }

        private void RemoveSpell_Click(object sender, EventArgs e)
        {
            try
            {
                ushort ID = ushort.Parse(RemoveID.Text);
                if (SpellUserName.SelectedItem != null)
                {
                    TheChosenProject.Client.GameClient client = null;
                    client = TheChosenProject.Client.GameClient.CharacterFromName(SpellUserName.SelectedItem.ToString());
                    if (client == null)
                        return;
                    if (client.MySpells.ClientSpells.ContainsKey((ushort)ID))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.MySpells.Remove((ushort)ID, stream);
                            SendMessagebox("Done Remove This Spell " + ID + "");
                        }

                    }
                }
            }
            catch (Exception ex) { SendMessagebox(ex.Message); }

        }

        private void RefName_Click(object sender, EventArgs e)
        {
            //لود الاسماء
            loadePlayerName();
        }
        #endregion
        #region class Ban
        private void Plyer_Name_Click(object sender, EventArgs e)
        {
            loadePlayerName();
        }
        private void AddBanACC_Click(object sender, EventArgs e)
        {

            //if (BanPlayerName.SelectedItem != null)
            //{
            //    TheChosenProject.Client.GameClient client = null;
            //    client = TheChosenProject.Client.GameClient.CharacterFromName(BanPlayerName.SelectedItem.ToString());
            //    if (client == null)
            //        return;
            //    Database.SystemBannedAccount.AddBan(client.Player.UID, client.Player.Name, uint.Parse(BanAccHours.Text), BanAccRespwen.Text);
            //    client.Socket.Disconnect();
            //    SendMessagebox("Done Ban Accuont " + client.Player.Name + "");
            //    LoadBanList("Acc");
            //}

        }
        private void LoadBanList(string Type)
        {
            switch (Type)
            {
                case "Acc":
                    {
                        listBanAcc.Items.Clear();
                        foreach (var ban in SystemBannedAccount.BannedPoll.Values)
                        {
                            listBanAcc.Items.Add(ban.Name);
                        }
                        break;
                    }
                case "Chat":
                    {
                        listBanChat.Items.Clear();
                        foreach (var User in Server.GamePoll.Values)
                        {
                            if (User.Player.IsBannedChat)
                            {
                                listBanChat.Items.Add(User.Player.Name);
                            }
                        }
                        break;
                    }
                case "IP":
                    {
                        listBanIP.Items.Clear();
                        foreach (var User in SystemBannedPC.BannedPoll.Values)
                        {
                            listBanIP.Items.Add(User.MACAdress);
                        }
                        break;
                    }
            }
        }
        private void LoadListAccBan_Click(object sender, EventArgs e)
        {
            LoadBanList("Acc");
        }

        private void RemoveBanACC_Click(object sender, EventArgs e)
        {
            if (listBanAcc.SelectedItem != null)
            {
                uint UID = 0;
                foreach (var obj in SystemBannedAccount.BannedPoll.Values)
                {
                    if (obj.Name == listBanAcc.SelectedItem.ToString())
                    {
                        UID = obj.UID;
                        break;
                    }
                }
                if (UID != 0)
                {
                    SystemBannedAccount.RemoveBan(UID);
                    LoadBanList("Acc");
                }
            }
        }

        private void AddBanChat_Click(object sender, EventArgs e)
        {
            if (BanPlayerName.SelectedItem == null) return;
            string Name = BanPlayerName.SelectedItem.ToString();
            int Hours = int.Parse(BanChatHours.Text);
            ChatBanned.AddChatBan(Name, Hours);
            SendMessagebox("Done Ban Chat " + Name + "");
            LoadBanList("Chat");

        }

        private void RemoveChatACC_Click(object sender, EventArgs e)
        {
            if (BanPlayerName.SelectedItem == null) return;
            string Name = BanPlayerName.SelectedItem.ToString();
            int Hours = int.Parse(BanChatHours.Text);
            ChatBanned.RemoveChatBan(Name);
            SendMessagebox("Done UnBan Chat " + Name + "");
            LoadBanList("Chat");
        }

        private void LoadListBanChat_Click(object sender, EventArgs e)
        {
            LoadBanList("Chat");
        }

        private void AddBanIP_Click(object sender, EventArgs e)
        {

            if (BanPlayerName.SelectedItem != null)
            {
                TheChosenProject.Client.GameClient client = null;
                client = TheChosenProject.Client.GameClient.CharacterFromName(BanPlayerName.SelectedItem.ToString());
                if (client == null)
                    return;
                Database.SystemBannedPC.AddBan(client);
                client.Socket.Disconnect();
                SendMessagebox("Done Ban IP " + client.Player.Name + "");
                LoadBanList("IP");
            }
        }

        private void LoadListIp_Click(object sender, EventArgs e)
        {
            LoadBanList("IP");
        }

        private void RemoveBanIP_Click(object sender, EventArgs e)
        {
            if (listBanIP.SelectedItem != null)
            {
                Database.SystemBannedPC.RemoveBan(listBanIP.SelectedItem.ToString());
                SendMessagebox("Done UnBan IP " + listBanIP.SelectedItem.ToString() + "");
                LoadBanList("IP");
            }
        }
        private void BanPlayerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BanPlayerName.SelectedItem != null)
            {
                TheChosenProject.Client.GameClient client = null;
                client = TheChosenProject.Client.GameClient.CharacterFromName(BanPlayerName.SelectedItem.ToString());
                if (client == null)
                    return;
                BanUid.Text = client.Player.UID.ToString();
                TxtBanIp.Text = client.Socket.RemoteIp.ToString();
            }
        }
        #endregion

        #region Class ChatPanle
        public class Clientt
        {
            public string Mess;
            public bool Seen = true;
        }
        public static Dictionary<string, Clientt> Clients = new Dictionary<string, Clientt>();
        public string SelectedClient = "";
        public static bool NewMessage = false;
        public static bool On = false;
        //Role.GameMap _map;
        //public Role.GameMap Mapp
        //{
        //    get
        //    {
        //        if (_map == null)
        //            _map = Server.ServerMaps[1002];
        //        return _map;
        //    }
        //}
        public void loadChatPanl()
        {
            Clients.Clear();
            SelectedClient = "";
            ClientList.ForeColor = Color.Black;
            RecList.ForeColor = Color.Black;
            this.Text = "ChatBox " + ServerKernel.ServerName;
            //base.Closing += this.Form1_Closing;
        }
        //private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    try
        //    {
        //        On = false;
        //        Clients.Clear();
        //        SelectedClient = "";
        //    }
        //    catch
        //    {
        //    }
        //    e.Cancel = false;
        //}
        private void SendMes()//الارسال
        {
            try
            {

                foreach (var user in Server.GamePoll.Values)
                {
                    if (user.Player.Name == SelectedClient)
                    {
                        Clientt msg = new Clientt();
                        //user.SendWhisper(SendText.Text, "Sauron[PM]", SelectedClient);
                        msg.Mess = (Environment.NewLine + "Sauron[PM]--->" + SendText.Text + ",");
                        if (Clients.ContainsKey(SelectedClient))
                        {
                            Clients[SelectedClient].Mess += msg.Mess;

                        }
                        else
                        {
                            Clients.Add(SelectedClient, msg);
                        }
                        RecList.Items.Add(msg.Mess);
                        SendText.Text = "";
                    }

                    RefreshData();
                }
            }
            catch (Exception d)
            {
                Console.WriteLine(d.ToString());
            }
        }
        private void ButtnSend_Click(object sender, EventArgs e)
        {
            //زر الارسال
            SendMes();
        }

        private void ClientList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Clientt client;
                SelectedClient = ClientList.Text;
                if (SelectedClient.Contains("(UnSeen)"))
                {
                    SelectedClient = SelectedClient.Remove(SelectedClient.Length - 8, 8);
                    ClientList.Text = SelectedClient;
                    ClientList.Items[ClientList.SelectedIndex] = SelectedClient;
                }
                if (Clients.TryGetValue(SelectedClient, out client))
                {
                    client.Seen = true;
                    RecList.Items.Clear();
                    string items = client.Mess;
                    string[] item = items.Split(',');
                    for (int x = 0; x < item.Length; x++)
                    {
                        RecList.Items.Add(item[x]);
                    }

                }
            }
            catch
            {

            }
        }

        private void MisReady_SelectedIndexChanged(object sender, EventArgs e)
        {
            //رسائل جاهزه
            try
            {
                foreach (var user in Server.GamePoll.Values)
                {
                    if (user.Player.Name == SelectedClient)
                    {
                        Clientt msg = new Clientt();
                        //user.SendWhisper(MisReady.Text, "Sauron[PM]", SelectedClient);
                        msg.Mess = ("Sauron[PM]--->" + MisReady.Text + ",");
                        if (Clients.ContainsKey(SelectedClient))
                        {
                            Clients[SelectedClient].Mess += msg.Mess;
                        }
                        else
                        {
                            Clients.Add(SelectedClient, msg);
                        }
                        RecList.Items.Add(msg.Mess);
                        MisReady.SelectedItem = null;
                    }
                    if (ComName.SelectedItem != null && SelectedClient == "")
                    {
                        if (user.Player.Name == ComName.SelectedItem.ToString())
                        {
                            Clientt msg = new Clientt();
                            //user.SendWhisper(MisReady.Text, "Sauron[PM]", ComName.SelectedItem.ToString());
                            msg.Mess = ("Sauron[PM]--->" + MisReady.Text + ",");
                            if (Clients.ContainsKey(ComName.SelectedItem.ToString()))
                            {
                                Clients[ComName.SelectedItem.ToString()].Mess += msg.Mess;
                            }
                            else
                            {
                                Clients.Add(ComName.SelectedItem.ToString(), msg);
                            }
                            RecList.Items.Add(msg.Mess);
                            MisReady.SelectedItem = null;
                            ComName.SelectedItem = null;
                            RefreshData();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void RemovePlaer_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clients.ContainsKey(ClientList.Items[ClientList.SelectedIndex].ToString()))
                {
                    Clients.Remove(ClientList.Items[ClientList.SelectedIndex].ToString());
                }
                ClientList.Items.RemoveAt(ClientList.SelectedIndex);
            }
            catch
            {

            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void ComName_Click(object sender, EventArgs e)
        {
            loadePlayerName();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (NewMessage)
            {
                RefreshData();
                NewMessage = false;
            }
        }
        private void RefreshData()
        {
            try
            {
                ClientList.Items.Clear();
                foreach (var x in Clients)
                {
                    if (x.Value.Seen)
                    {
                        ClientList.Items.Add(x.Key);
                    }
                    else
                    {
                        if (SelectedClient != x.Key)
                        {
                            ClientList.Items.Add(x.Key + "(UnSeen)");
                        }
                        else
                        {
                            ClientList.Items.Add(x.Key);
                        }
                    }
                }
                RecList.Items.Clear();
                Clientt client;
                if (SelectedClient != "")
                {
                    if (Clients.TryGetValue(SelectedClient, out client))
                    {
                        client.Seen = true;
                        string items = client.Mess;
                        string[] item = items.Split(',');
                        for (int x = 0; x < item.Length; x++)
                        {
                            RecList.Items.Add(item[x]);
                        }
                    }
                }
            }
            catch (Exception d)
            {
                MessageBox.Show(d.ToString());
            }
        }

        public static void WelcomeMessage(Client.GameClient user, Game.MsgNpc.Npc npc, ServerSockets.Packet stream)
        {
            var msg = new MsgMessage("Welcome and enjoy our server, how can i help you? ", user.Player.Name, "[PM]", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Whisper);
            Program.SendGlobalPackets.Enqueue(msg.GetArray(new ServerSockets.RecycledPacket().GetStream()));
        }

        private void ButtnSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMes();
            }
        }

        private void ON_Off_GM_Click(object sender, EventArgs e)

        {
            TheChosenProject.Client.GameClient pclient = new TheChosenProject.Client.GameClient(null);
            if (!Database.Server.GamePoll.ContainsKey(2000000000))
            {

                pclient.Fake = true;
                pclient.Player = new Role.Player(pclient);
                pclient.Inventory = new Role.Instance.Inventory(pclient);
                pclient.Equipment = new Role.Instance.Equip(pclient);
                pclient.Warehouse = new Role.Instance.Warehouse(pclient);
                pclient.MyProfs = new Role.Instance.Proficiency(pclient);
                pclient.MySpells = new Role.Instance.Spell(pclient);
                pclient.Status = new Game.MsgServer.MsgStatus();
                pclient.Player.Name = "Sauron[PM]";
                pclient.Player.Body = 1003;
                pclient.Player.UID = 2000000000;
                pclient.Player.HitPoints = ushort.MaxValue;
                pclient.Status.MaxHitpoints = ushort.MaxValue;
                pclient.Player.X = (ushort)312;
                pclient.Player.Y = (ushort)288;
                pclient.Player.Map = 1002;
                pclient.Player.Level = 140;
                pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                pclient.Player.Face = 153;
                pclient.Player.CountryID = 1;
                pclient.Player.Action = Role.Flags.ConquerAction.Sit;
                pclient.Player.Angle = Role.Flags.ConquerAngle.SouthWest;
                pclient.Player.Vitality = (ushort)((pclient.Player.Level + pclient.Player.BattlePower) * (pclient.Player.Reborn + 1));

                pclient.Player.Hair = 774;
                pclient.Player.GarmentId = 194065;
                pclient.Map = Database.Server.ServerMaps[1002];
                DataCore.AtributeStatus.GetStatus(pclient.Player);
                DataCore.SetCharacterSides(pclient.Player);
                DataCore.CreateHairStyle(pclient);
                DataCore.LoadClient(pclient.Player);
                pclient.Map.Enquer(pclient);

                Database.Server.GamePoll.TryAdd(pclient.Player.UID, pclient);


                using (var p = new ServerSockets.RecycledPacket())
                {
                    var stream = p.GetStream();
                    pclient.Player.View.SendView(pclient.Player.GetArray(stream, false), false);
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("Sauron[PM] Has Login In TwinCity", "ALLUSERS", "System", MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));

                }
                Rayzo_Panle.On = true;

            }
            else
            {
                if (!Rayzo_Panle.On)
                {
                    Rayzo_Panle.On = true;
                    using (var p = new ServerSockets.RecycledPacket())
                    {
                        var stream = p.GetStream();
                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Sauron[PM] Has Login In TwinCity", "ALLUSERS", "System", MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                    }

                }
                else
                {
                    using (var p = new ServerSockets.RecycledPacket())
                    {
                        var stream = p.GetStream();
                        Program.SendGlobalPackets.Enqueue(new MsgMessage("Sauron[PM] Has Close", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                        Rayzo_Panle.On = false;
                    }
                }

            }
        }

        private void SendText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMes();
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            RecList.Items.Clear();
        }

        private void SendMessAll_Click(object sender, EventArgs e)
        {
            if (ChatType.SelectedItem != null)
            {
                switch (ChatType.SelectedIndex)
                {
                    case 0:
                        {
                            foreach (var cleint in Server.GamePoll.Values)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage(ChatAllMess.Text, MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));
                                }

                            }
                            break;
                        }
                    case 1:
                        {

                            foreach (var cleint in Server.GamePoll.Values)
                            {
                                if (cleint.Fake)
                                    return;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    var msg = new MsgMessage(ChatAllMess.Text, cleint.Player.Name, "[PM]", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Whisper);
                                    Program.SendGlobalPackets.Enqueue(msg.GetArray(new ServerSockets.RecycledPacket().GetStream()));
                                }

                            }
                            break;
                        }
                    case 2:
                        {
                            foreach (var cleint in Server.GamePoll.Values)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    cleint.SendSysMesage(ChatAllMess.Text, MsgMessage.ChatMode.Center);

                                }

                            }
                            break;
                        }

                    case 3:
                        {
                            foreach (var cleint in Server.GamePoll.Values)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    cleint.SendSysMesage(ChatAllMess.Text, MsgMessage.ChatMode.BroadcastMessage);

                                }

                            }
                            break;
                        }
                    case 4:
                        {
                            foreach (var cleint in Server.GamePoll.Values)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    cleint.SendSysMesage(ChatAllMess.Text, MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);

                                }

                            }
                            break;
                        }
                }
            }
        }
        #endregion
        #region Class Attack
        #region Ninja
        #region TwofoldBlade
        public static uint TwofoldBladeSkilUID = 0;
        public static double TwofoldBladePlayers;
        public static void TwofoldBladeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "6000");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                TwofoldBladeSkilUID = r.ReadUInt32("SkilUID");
                TwofoldBladePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region BloodyScythe
        public static uint BloodyScytheSkilUID = 0;
        public static double BloodyScythePlayers;
        public static void BloodyScytheTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11170");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                BloodyScytheSkilUID = r.ReadUInt32("SkilUID");
                BloodyScythePlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region CounterKill
        public static uint CounterKillSkilUID = 0;
        public static double CounterKillPlayers;
        public static void CounterKillTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "6003");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                CounterKillSkilUID = r.ReadUInt32("SkilUID");
                CounterKillPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ShurikenV
        public static uint ShurikenVSkilUID = 0;
        public static double ShurikenVPlayers;
        public static void ShurikenVTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "6010");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ShurikenVSkilUID = r.ReadUInt32("SkilUID");
                ShurikenVPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region GapingWounds
        public static uint GapingWoundsSkilUID = 0;
        public static double GapingWoundsPlayers;
        public static void GapingWoundsTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11230");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                GapingWoundsSkilUID = r.ReadUInt32("SkilUID");
                GapingWoundsPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region TwilightDance
        public static uint TwilightDanceUID = 0;
        public static double TwilightDancePlayers;
        public static void TwilightDanceTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12070");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                TwilightDanceUID = r.ReadUInt32("SkilUID");
                TwilightDancePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SuperTwBlade
        public static uint SuperTwBladeUID = 0;
        public static double SuperTwBladePlayers;
        public static void SuperTwBladeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12080");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SuperTwBladeUID = r.ReadUInt32("SkilUID");
                SuperTwBladePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FatalSpin
        public static uint FatalSpinUID = 0;
        public static double FatalSpinPlayers;
        public static void FatalSpinTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12110");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                FatalSpinUID = r.ReadUInt32("SkilUID");
                FatalSpinPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Monk
        #region RadiantPalm
        public static uint RadiantPalmSkilUID = 0;
        public static double RadiantPalmPlayers;

        public static void RadiantPalmTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "10381");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                RadiantPalmSkilUID = r.ReadUInt32("SkilUID");
                RadiantPalmPlayers = r.ReadDouble("Damge");
                ;
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region WhirlwindKick
        public static uint WhirlwindKickSkilUID = 0;
        public static double WhirlwindKickPlayers;
        public static void WhirlwindKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "10415");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                WhirlwindKickSkilUID = r.ReadUInt32("SkilUID");
                WhirlwindKickPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region TripleAttack
        public static uint TripleAttackSkilUID = 0;
        public static double TripleAttackPlayers;
        public static void TripleAttackTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "10490");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                TripleAttackSkilUID = r.ReadUInt32("SkilUID");
                TripleAttackPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Oblivion
        public static uint OblivionSkilUID = 0;
        public static double OblivionPlayers;
        public static void OblivionTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "10390");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                OblivionSkilUID = r.ReadUInt32("SkilUID");
                OblivionPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region WrathoftheEmperor
        public static uint WrathoftheEmperorSkilUID = 0;
        public static double WrathoftheEmperorPlayers;
        public static void WrathoftheEmperorTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12570");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                WrathoftheEmperorSkilUID = r.ReadUInt32("SkilUID");
                WrathoftheEmperorPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region InfernalEcho
        public static uint InfernalEchoSkillUID = 0;
        public static double InfernalEchoPlayers;
        public static void InfernalEchoTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12550");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                InfernalEchoSkillUID = r.ReadUInt32("SkilUID");
                InfernalEchoPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Strike
        public static uint StrikeSkilUID = 0;
        public static double StrikePlayers;
        public static void StrikeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12600");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                StrikeSkilUID = r.ReadUInt32("SkilUID");
                StrikePlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Warrior
        #region SpeedGun
        public static uint SpeedGunSkilUID = 0;
        public static double SpeedGunPlayers;
        public static void SpeedGunTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1260");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SpeedGunSkilUID = r.ReadUInt32("SkilUID");
                SpeedGunPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Snow
        public static uint SnowSkilUID = 0;
        public static double SnowPlayers;
        public static void SnowTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "5010");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SnowSkilUID = r.ReadUInt32("SkilUID");
                SnowPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Boom
        public static uint BoomSkilUID = 0;
        public static double BoomPlayers;
        public static void BoomTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "5040");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                BoomSkilUID = r.ReadUInt32("SkilUID");
                BoomPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Seizer
        public static uint SeizerSkilUID = 0;
        public static double SeizerPlayers;
        public static void SeizerTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "7000");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SeizerSkilUID = r.ReadUInt32("SkilUID");
                SeizerPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ViperFang
        public static uint ViperFangSkilUID = 0;
        public static double ViperFangPlayers;
        public static void ViperFangTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11005");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ViperFangSkilUID = r.ReadUInt32("SkilUID");
                ViperFangPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Penetration
        public static uint PenetrationSkilUID = 0;
        public static double PenetrationPlayers;
        public static void PenetrationTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1290");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                PenetrationSkilUID = r.ReadUInt32("SkilUID");
                PenetrationPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonTail
        public static uint DragonTailSkilUID = 0;
        public static double DragonTailPlayers;
        public static void DragonTailTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11000");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                DragonTailSkilUID = r.ReadUInt32("SkilUID");
                DragonTailPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Celestial
        public static uint CelestialSkilUID = 0;
        public static double CelestialPlayers;
        public static void CelestialTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "7030");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                CelestialSkilUID = r.ReadUInt32("SkilUID");
                CelestialPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ChargingVortex
        public static uint ChargingVortexUID = 0;
        public static double ChargingVortexPlayers;
        public static void ChargingVortexTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11190");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ChargingVortexUID = r.ReadUInt32("SkilUID");
                ChargingVortexPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ScarofEarth
        public static uint ScarofEarthSkilUID = 0;
        public static double ScarofEarthPlayers;
        public static void ScarofEarthTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12670");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ScarofEarthSkilUID = r.ReadUInt32("SkilUID");
                ScarofEarthPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region WaveofBlood
        public static uint WaveofBloodSkilUID = 0;
        public static double WaveofBloodPlayers;
        public static void WaveofBloodTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12690");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                WaveofBloodSkilUID = r.ReadUInt32("SkilUID");
                WaveofBloodPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ManiacDance
        public static uint ManiacDanceSkilUID = 0;
        public static double ManiacDancePlayers;
        public static void ManiacDanceTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12700");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ManiacDanceSkilUID = r.ReadUInt32("SkilUID");
                ManiacDancePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Pirate
        #region BladeTempest
        public static uint BladeTempestSkilUID = 0;
        public static double BladeTempestPlayers;
        public static void BladeTempestTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11110");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                BladeTempestSkilUID = r.ReadUInt32("SkilUID");
                BladeTempestPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region GaleBomb
        public static uint GaleBombSkilUID = 0;
        public static double GaleBombPlayers;
        public static void GaleBombTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11070");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                GaleBombSkilUID = r.ReadUInt32("SkilUID");
                GaleBombPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Windstorm
        public static uint WindstormSkilUID = 0;
        public static double WindstormPlayers;
        public static void WindstormTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11140");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                WindstormSkilUID = r.ReadUInt32("SkilUID");
                WindstormPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ScurvyBomb
        public static uint ScurvyBombSkilUID = 0;
        public static double ScurvyBombPlayers;
        public static void ScurvyBombTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11040");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ScurvyBombSkilUID = r.ReadUInt32("SkilUID");
                ScurvyBombPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region CannonBarrage
        public static uint CannonBarrageSkilUID = 0;
        public static double CannonBarragePlayers;
        public static void CannonBarrageTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11050");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                CannonBarrageSkilUID = r.ReadUInt32("SkilUID");
                CannonBarragePlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region KrakensRevenge
        public static uint KrakensRevengeSkilUID = 0;
        public static double KrakensRevengePlayers;
        public static void KrakensRevengeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11100");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                KrakensRevengeSkilUID = r.ReadUInt32("SkilUID");
                KrakensRevengePlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Trojan
        #region FastBlade
        public static uint FastBladeSkilUID = 0;
        public static double FastBladePlayers;
        public static void FastBladeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1045");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                FastBladeSkilUID = r.ReadUInt32("SkilUID");
                FastBladePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ScentSword
        public static uint ScentSwordSkilUID = 0;
        public static double ScentSwordPlayers;
        public static void ScentSwordTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1046");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ScentSwordSkilUID = r.ReadUInt32("SkilUID");
                ScentSwordPlayers = r.ReadDouble("Damge");
            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Rage
        public static uint RageSkilUID = 0;
        public static double RagePlayers;
        public static void RageTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "7020");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                RageSkilUID = r.ReadUInt32("SkilUID");
                RagePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Phoenix
        public static uint PhoenixSkilUID = 0;
        public static double PhoenixPlayers;

        public static void PhoenixTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "5030");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                PhoenixSkilUID = r.ReadUInt32("SkilUID");
                PhoenixPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Hercules
        public static uint HerculesSkilUID = 0;
        public static double HerculesPlayers;
        public static void HerculesTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1115");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                HerculesSkilUID = r.ReadUInt32("SkilUID");
                HerculesPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonWhirl
        public static uint DragonWhirlSkilUID = 0;
        public static double DragonWhirlPlayers;
        public static void DragonWhirlTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "10315");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                DragonWhirlSkilUID = r.ReadUInt32("SkilUID");
                DragonWhirlPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FatalCross
        public static uint FatalCrossSkilUID = 0;
        public static double FatalCrossPlayers;
        public static void FatalCrossTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11980");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                FatalCrossSkilUID = r.ReadUInt32("SkilUID");
                FatalCrossPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region MortalStrike
        public static uint MortalStrikeSkilUID = 0;
        public static double MortalStrikePlayers;
        public static void MortalStrikeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11990");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                MortalStrikeSkilUID = r.ReadUInt32("SkilUID");
                MortalStrikePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Fire
        #region Thunder
        public static uint ThunderSkilUID = 0;
        public static double ThunderPlayers;
        public static void ThunderTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1000");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ThunderSkilUID = r.ReadUInt32("SkilUID");
                ThunderPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Fire
        public static uint FireSkilUID = 0;
        public static double FirePlayers;
        public static void FireTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1001");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                FireSkilUID = r.ReadUInt32("SkilUID");
                FirePlayers = r.ReadDouble("Damge");


            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Tornado
        public static uint TornadoSkilUID = 0;
        public static double TornadoPlayers;
        public static void TornadoTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1002");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                TornadoSkilUID = r.ReadUInt32("SkilUID");
                TornadoPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Bomb
        public static uint BombSkilUID = 0;
        public static double BombPlayers;
        public static void BombTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1160");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                BombSkilUID = r.ReadUInt32("SkilUID");
                BombPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ChainBolt
        public static uint ChainBoltSkilUID = 0;
        public static double ChainBoltPlayers;
        public static void ChainBoltTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "10309");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ChainBoltSkilUID = r.ReadUInt32("SkilUID");
                ChainBoltPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FireRing
        public static uint FireRingSkilUID = 0;
        public static double FireRingPlayers;
        public static void FireRingTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1150");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                FireRingSkilUID = r.ReadUInt32("SkilUID");
                FireRingPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FireMeteor
        public static uint FireMeteorSkilUID = 0;
        public static double FireMeteorPlayers;
        public static void FireMeteorTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1180");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                FireMeteorSkilUID = r.ReadUInt32("SkilUID");
                FireMeteorPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FireCircle
        public static uint FireCircleSkilUID = 0;
        public static double FireCirclePlayers;
        public static void FireCircleTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1120");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                FireCircleSkilUID = r.ReadUInt32("SkilUID");
                FireCirclePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region FireofHell
        public static uint FireofHellSkilUID = 0;
        public static double FireofHellPlayers;
        public static void FireofHellTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1165");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                FireofHellSkilUID = r.ReadUInt32("SkilUID");
                FireofHellPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region HeavenBlade
        public static uint HeavenBladeSkilUID = 0;
        public static double HeavenBladePlayers;
        public static void HeavenBladeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "10310");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                HeavenBladeSkilUID = r.ReadUInt32("SkilUID");
                HeavenBladePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion

        #endregion
        #region Archer
        #region Scatter
        public static uint ScatterSkilUID = 0;
        public static double ScatterPlayers;
        public static void ScatterTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "8001");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ScatterSkilUID = r.ReadUInt32("SkilUID");
                ScatterPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region RapidFire
        public static uint RapidFireSkilUID = 0;
        public static double RapidFirePlayers;
        public static void RapidFireTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "8000");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                RapidFireSkilUID = r.ReadUInt32("SkilUID");
                RapidFirePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DaggerStorm
        public static uint DaggerStormSkilUID = 0;
        public static double DaggerStormPlayers;
        public static void DaggerStormTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11600");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                DaggerStormSkilUID = r.ReadUInt32("SkilUID");
                DaggerStormPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region BladeFlurry
        public static uint BladeFlurrySkilUID = 0;
        public static double BladeFlurryPlayers;
        public static void BladeFlurryTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11610");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                BladeFlurrySkilUID = r.ReadUInt32("SkilUID");
                BladeFlurryPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region BlisteringWave
        public static uint BlisteringWaveSkilUID = 0;
        public static double BlisteringWavePlayers;
        public static void BlisteringWaveTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11650");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                BlisteringWaveSkilUID = r.ReadUInt32("SkilUID");
                BlisteringWavePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region MortalWound
        public static uint MortalWoundSkilUID = 0;
        public static double MortalWoundPlayers;
        public static void MortalWoundTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "11660");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                MortalWoundSkilUID = r.ReadUInt32("SkilUID");
                MortalWoundPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region Water
        #region Vulcano
        public static uint VulcanoSkilUID = 0;
        public static double VulcanoPlayers;
        public static void VulcanoTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1125");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                VulcanoSkilUID = r.ReadUInt32("SkilUID");
                VulcanoPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Lightning
        public static uint LightningSkilUID = 0;
        public static double LightningPlayers;
        public static void LightningTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "1010");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                LightningSkilUID = r.ReadUInt32("SkilUID");
                LightningPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SpeedLightning
        public static uint SpeedLightningSkilUID = 0;
        public static double SpeedLightningPlayers;
        public static void SpeedLightningTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "5001");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SpeedLightningSkilUID = r.ReadUInt32("SkilUID");
                SpeedLightningPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Prevade
        public static uint PrevadeSkilUID = 0;
        public static double PrevadePlayers;
        public static void PrevadeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "3090");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                PrevadeSkilUID = r.ReadUInt32("SkilUID");
                PrevadePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SearingTouch
        public static uint SearingTouchSkilUID = 0;
        public static double SearingTouchPlayers;
        public static void SearingTouchTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12400");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SearingTouchSkilUID = r.ReadUInt32("SkilUID");
                SearingTouchPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region DragonWarrer
        #region CrackingSwipe
        public static uint CrackingSwipeSkilUID = 0;
        public static double CrackingSwipePlayers;
        public static void CrackingSwipeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12160");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                CrackingSwipeSkilUID = r.ReadUInt32("SkilUID");
                CrackingSwipePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SplittingSwipe
        public static uint SplittingSwipeSkilUID = 0;
        public static double SplittingSwipePlayers;
        public static void SplittingSwipeTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12170");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SplittingSwipeSkilUID = r.ReadUInt32("SkilUID");
                SplittingSwipePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SpeedKick
        public static uint SpeedKickSkilUID = 0;
        public static double SpeedKickPlayers;
        public static void SpeedKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12120");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SpeedKickSkilUID = r.ReadUInt32("SkilUID");
                SpeedKickPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ViolentKick
        public static uint ViolentKickSkilUID = 0;
        public static double ViolentKickPlayers;
        public static void ViolentKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12130");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ViolentKickSkilUID = r.ReadUInt32("SkilUID");
                ViolentKickPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region StormKick
        public static uint StormKickSkilUID = 0;
        public static double StormKickPlayers;
        public static void StormKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12140");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                StormKickSkilUID = r.ReadUInt32("SkilUID");
                StormKickPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonPunch
        public static uint DragonPunchSkilUID = 0;
        public static double DragonPunchPlayers;
        public static void DragonPunchTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12240");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                DragonPunchSkilUID = r.ReadUInt32("SkilUID");
                DragonPunchPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonCyclone
        public static uint DragonCycloneSkilUID = 0;
        public static double DragonCyclonePlayers;
        public static void DragonCycloneTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12290");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                DragonCycloneSkilUID = r.ReadUInt32("SkilUID");
                DragonCyclonePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonFury
        public static uint DragonFurySkilUID = 0;
        public static double DragonFuryPlayers;
        public static void DragonFuryTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12300");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                DragonFurySkilUID = r.ReadUInt32("SkilUID");
                DragonFuryPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region AirKick
        public static uint AirKickSkilUID = 0;
        public static double AirKickPlayers;
        public static void AirKickTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12320");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                AirKickSkilUID = r.ReadUInt32("SkilUID");
                AirKickPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region AirSweep
        public static uint AirSweepSkilUID = 0;
        public static double AirSweepPlayers;
        public static void AirSweepTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12330");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                AirSweepSkilUID = r.ReadUInt32("SkilUID");
                AirSweepPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region AirRaid
        public static uint AirRaidSkilUID = 0;
        public static double AirRaidPlayers;
        public static void AirRaidTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12340");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                AirRaidSkilUID = r.ReadUInt32("SkilUID");
                AirRaidPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region DragonSlash
        public static uint DragonSlashSkilUID = 0;
        public static double DragonSlashPlayers;
        public static void DragonSlashTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12350");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                DragonSlashSkilUID = r.ReadUInt32("SkilUID");
                DragonSlashPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #endregion
        #region WindWalker
        #region TripleBlasts
        public static uint TripleBlastsSkilUID = 0;
        public static double TripleBlastsPlayers;
        public static void TripleBlastsTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12850");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                TripleBlastsSkilUID = r.ReadUInt32("SkilUID");
                TripleBlastsPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Omnipotence
        public static uint OmnipotenceSkilUID = 0;
        public static double OmnipotencePlayers;
        public static void OmnipotenceTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12860");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                OmnipotenceSkilUID = r.ReadUInt32("SkilUID");
                OmnipotencePlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region SwirlingStorm
        public static uint SwirlingStormSkilUID = 0;
        public static double SwirlingStormPlayers;
        public static void SwirlingStormTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12890");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                SwirlingStormSkilUID = r.ReadUInt32("SkilUID");
                SwirlingStormPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region RageofWar
        public static uint RageofWarSkilUID = 0;
        public static double RageofWarPlayers;
        public static void RageofWarTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12930");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                RageofWarSkilUID = r.ReadUInt32("SkilUID");
                RageofWarPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region BurntFrost
        public static uint BurntFrostSkilUID = 0;
        public static double BurntFrostPlayers;
        public static void BurntFrostTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12940");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                BurntFrostSkilUID = r.ReadUInt32("SkilUID");
                BurntFrostPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region AngerofStomper
        public static uint AngerofStomperSkilUID = 0;
        public static double AngerofStomperPlayers;
        public static void AngerofStomperTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12980");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                AngerofStomperSkilUID = r.ReadUInt32("SkilUID");
                AngerofStomperPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region Thundercloud
        public static uint ThundercloudSkilUID = 0;
        public static double ThundercloudPlayers;
        public static void ThundercloudTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "12840");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ThundercloudSkilUID = r.ReadUInt32("SkilUID");
                ThundercloudPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion
        #region ThundercloudAttack
        public static uint ThundercloudAttackSkilUID = 0;
        public static double ThundercloudAttackPlayers;
        public static void ThundercloudAttackTime()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack").Where("SkilUID", "13190");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                ThundercloudAttackSkilUID = r.ReadUInt32("SkilUID");
                ThundercloudAttackPlayers = r.ReadDouble("Damge");

            }
            r.Close();
            r.Dispose();
        }
        #endregion

        #endregion
        public static void LoadAttack()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("RayzoAttack"))
            using (var reader = new MySqlReader(cmd))
            {
                while (reader.Read())
                {

                    #region Ninja
                    TwofoldBladeTime();
                    CounterKillTime();
                    BloodyScytheTime();
                    ShurikenVTime();
                    GapingWoundsTime();
                    TwilightDanceTime();
                    SuperTwBladeTime();
                    FatalSpinTime();
                    #endregion
                    #region Monk
                    RadiantPalmTime();
                    WhirlwindKickTime();
                    TripleAttackTime();
                    OblivionTime();
                    WrathoftheEmperorTime();
                    StrikeTime();
                    InfernalEchoTime();
                    #endregion
                    #region Warrior
                    SpeedGunTime();
                    SnowTime();
                    BoomTime();
                    SeizerTime();
                    ViperFangTime();
                    PenetrationTime();
                    DragonTailTime();
                    CelestialTime();
                    ChargingVortexTime();
                    ScarofEarthTime();
                    WaveofBloodTime();
                    ManiacDanceTime();
                    #endregion
                    #region Pirate
                    BladeTempestTime();
                    GaleBombTime();
                    WindstormTime();
                    ScurvyBombTime();
                    CannonBarrageTime();
                    KrakensRevengeTime();
                    #endregion
                    #region Trojan
                    FastBladeTime();
                    ScentSwordTime();
                    PhoenixTime();
                    RageTime();
                    HerculesTime();
                    DragonWhirlTime();
                    FatalCrossTime();
                    MortalStrikeTime();
                    #endregion
                    #region Taoist fire
                    ThunderTime();
                    FireTime();
                    TornadoTime();
                    BombTime();
                    ChainBoltTime();
                    FireRingTime();
                    FireMeteorTime();
                    FireCircleTime();
                    FireofHellTime();
                    HeavenBladeTime();
                    #endregion
                    #region Archer
                    ScatterTime();
                    RapidFireTime();
                    DaggerStormTime();
                    BladeFlurryTime();
                    BlisteringWaveTime();
                    MortalWoundTime();
                    #endregion
                    #region Water
                    VulcanoTime();
                    LightningTime();
                    SpeedLightningTime();
                    PrevadeTime();
                    SearingTouchTime();
                    #endregion
                    #region DragonWarrer
                    CrackingSwipeTime();
                    SplittingSwipeTime();
                    SpeedKickTime();
                    ViolentKickTime();
                    StormKickTime();
                    DragonPunchTime();
                    DragonCycloneTime();
                    DragonFuryTime();
                    AirKickTime();
                    AirSweepTime();
                    AirRaidTime();
                    DragonSlashTime();
                    #endregion
                    #region Wind Wailker
                    TripleBlastsTime();
                    OmnipotenceTime();
                    SwirlingStormTime();
                    RageofWarTime();
                    BurntFrostTime();
                    AngerofStomperTime();
                    ThundercloudTime();
                    ThundercloudAttackTime();
                    #endregion 
                    return;
                }
            }

        }
        private void UpdateTxt()
        {
            // WindWalker
            Txt_TripleBlasts.Text = TripleBlastsPlayers.ToString("0.00");
            Txt_Omnipotence.Text = OmnipotencePlayers.ToString("0.00");
            Txt_SwirlingStorm.Text = SwirlingStormPlayers.ToString("0.00");
            Txt_RageofWar.Text = RageofWarPlayers.ToString("0.00");
            Txt_BurntFrost.Text = BurntFrostPlayers.ToString("0.00");
            Txt_AngerofStomper.Text = AngerofStomperPlayers.ToString("0.00");
            Txt_Thundercloud.Text = ThundercloudPlayers.ToString("0.00");
            Txt_ThundercloudAttack.Text = ThundercloudAttackPlayers.ToString("0.00");

            //Dragon Warrior
            Txt_CrackingSwipe.Text = CrackingSwipePlayers.ToString("0.00");
            Txt_SplittingSwipe.Text = SplittingSwipePlayers.ToString("0.00");
            Txt_SpeedKick.Text = SpeedKickPlayers.ToString("0.00");
            Txt_ViolentKick.Text = ViolentKickPlayers.ToString("0.00");
            Txt_StormKick.Text = StormKickPlayers.ToString("0.00");
            Txt_DragonPunch.Text = DragonPunchPlayers.ToString("0.00");
            Txt_DragonCyclone.Text = DragonCyclonePlayers.ToString("0.00");
            Txt_DragonFury.Text = DragonFuryPlayers.ToString("0.00");
            Txt_AirKick.Text = AirKickPlayers.ToString("0.00");
            Txt_AirRaid.Text = AirRaidPlayers.ToString("0.00");
            Txt_ViolentKick.Text = ViolentKickPlayers.ToString("0.00");
            Txt_AirSweep.Text = AirSweepPlayers.ToString("0.00");
            Txt_DragonSlash.Text = DragonSlashPlayers.ToString("0.00");

            //Parite
            textBladeTempest.Text = BladeTempestPlayers.ToString("0.00");
            textGaleBomb.Text = GaleBombPlayers.ToString("0.00");
            textWindstorm.Text = WindstormPlayers.ToString("0.00");
            textScurvyBomb.Text = ScurvyBombPlayers.ToString("0.00");
            textCannonBarrage.Text = CannonBarragePlayers.ToString("0.00");
            textKrakensRevenge.Text = KrakensRevengePlayers.ToString("0.00");
            //Water
            textVulcano.Text = VulcanoPlayers.ToString("0.00");
            textLightning.Text = LightningPlayers.ToString("0.00");
            textSpeedLightning.Text = SpeedLightningPlayers.ToString("0.00");
            textPrevade.Text = PrevadePlayers.ToString("0.00");
            Txt_SearingTouch.Text = SearingTouchPlayers.ToString("0.00");
            //Trojan
            HercDameg.Text = HerculesPlayers.ToString("0.00");
            FastDamge.Text = FastBladePlayers.ToString("0.00");
            SSdamge.Text = ScentSwordPlayers.ToString("0.00");
            RageDamge.Text = RagePlayers.ToString("0.00");
            PhoenixDmg.Text = PhoenixPlayers.ToString("0.00");
            textDragonWhirl.Text = DragonWhirlPlayers.ToString("0.00");
            Txt_FatalCross.Text = FatalCrossPlayers.ToString("0.00");
            Txt_MortalStrike.Text = MortalStrikePlayers.ToString("0.00");
            //Fire
            textThunder.Text = ThunderPlayers.ToString("0.00");
            textFire.Text = FirePlayers.ToString("0.00");
            textTornado.Text = TornadoPlayers.ToString("0.00");
            textBomb.Text = BombPlayers.ToString("0.00");
            textChainBolt.Text = ChainBoltPlayers.ToString("0.00");
            textFireRing.Text = FireRingPlayers.ToString("0.00");
            textFireMeteor.Text = FireMeteorPlayers.ToString("0.00");
            textFireCircle.Text = FireCirclePlayers.ToString("0.00");
            textFireofHell.Text = FireofHellPlayers.ToString("0.00");
            textHeavenBlade.Text = HeavenBladePlayers.ToString("0.00");
            //ninja
            textTwofoldBlade.Text = TwofoldBladePlayers.ToString("0.00");
            textBloodyScythe.Text = BloodyScythePlayers.ToString("0.00");
            textCounterKill.Text = CounterKillPlayers.ToString("0.00");
            textShurikenVortex.Text = ShurikenVPlayers.ToString("0.00");
            textGapingWounds.Text = GapingWoundsPlayers.ToString("0.00");
            Txt_TwilightDance.Text = TwilightDancePlayers.ToString("0.00");
            Txt_SuperTwBlade.Text = SuperTwBladePlayers.ToString("0.00");
            Txt_FatalSpin.Text = FatalSpinPlayers.ToString("0.00");

            //Monk
            textPalm.Text = RadiantPalmPlayers.ToString("0.00");
            textWhirlwindKick.Text = WhirlwindKickPlayers.ToString("0.00");
            textTripleAttack.Text = TripleAttackPlayers.ToString("0.00");
            textOblivion.Text = OblivionPlayers.ToString("0.00");
            Txt_WrathoftheEmperor.Text = WrathoftheEmperorPlayers.ToString("0.00");
            Txt_BladeFlurry.Text = BladeFlurryPlayers.ToString("0.00");
            Txt_Strike.Text = StrikePlayers.ToString("0.00");
            //Archer
            textScatter.Text = ScatterPlayers.ToString("0.00");
            textRapidFire.Text = RapidFirePlayers.ToString("0.00");
            Txt_DaggerStorm.Text = DaggerStormPlayers.ToString("0.00");
            Txt_InfernalEcho.Text = InfernalEchoPlayers.ToString("0.00");
            Txt_BlisteringWave.Text = BlisteringWavePlayers.ToString("0.00");
            Txt_MortalWound.Text = MortalWoundPlayers.ToString("0.00");
            //Warrior
            textSpeedGun.Text = SpeedGunPlayers.ToString("0.00");
            textSnow.Text = SnowPlayers.ToString("0.00");
            textBoom.Text = BoomPlayers.ToString("0.00");
            textSeizer.Text = SeizerPlayers.ToString("0.00");
            textViperFang.Text = ViperFangPlayers.ToString("0.00");
            textPenetration.Text = PenetrationPlayers.ToString("0.00");
            textDragonTail.Text = DragonTailPlayers.ToString("0.00");
            textCelestial.Text = CelestialPlayers.ToString("0.00");
            textChargingVortex.Text = ChargingVortexPlayers.ToString("0.00");
            Txt_ScarofEarth.Text = ScarofEarthPlayers.ToString("0.00");
            Txt_WaveofBlood.Text = WaveofBloodPlayers.ToString("0.00");
            Txt_Strike.Text = StrikePlayers.ToString("0.00");

        }
        public static void ExtraPanleDamge(Database.MagicType.Magic DBSpell, MsgSpellAnimation.SpellObj Spell)
        {
            switch (DBSpell.ID)
            {
                case 11600:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DaggerStormPlayers);
                        break;
                    }
                case 11610:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BladeFlurryPlayers);
                        break;
                    }
                case 11650:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BlisteringWavePlayers);
                        break;
                    }
                case 11660:
                    {
                        Spell.Damage = (uint)(Spell.Damage * MortalWoundPlayers);
                        break;
                    }
                case 12840:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ThundercloudPlayers);
                        break;
                    }
                case 12940:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BurntFrostPlayers);
                        break;
                    }
                case 13190:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ThundercloudAttackPlayers);
                        break;
                    }
                case 12850:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TripleBlastsPlayers);
                        break;
                    }
                case 12680:
                    {
                        Spell.Damage = (uint)(Spell.Damage * StrikePlayers);
                        break;
                    }

                case 11980:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FatalCrossPlayers);
                        break;
                    }
                case 11990:
                    {
                        Spell.Damage = (uint)(Spell.Damage * MortalStrikePlayers);
                        break;
                    }

                case 12400:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SearingTouchPlayers);
                        break;
                    }
                case 12550:
                    {
                        Spell.Damage = (uint)(Spell.Damage * InfernalEchoPlayers);
                        break;
                    }
                case 12570:
                    {
                        Spell.Damage = (uint)(Spell.Damage * WrathoftheEmperorPlayers);
                        break;
                    }
                case 12600:
                    {
                        Spell.Damage = (uint)(Spell.Damage * StrikePlayers);
                        break;
                    }
                case 12670:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ScarofEarthPlayers);
                        break;
                    }
                case 12690:
                    {
                        Spell.Damage = (uint)(Spell.Damage * WaveofBloodPlayers);
                        break;
                    }
                case 12700:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ManiacDancePlayers);
                        break;
                    }

                case 12130:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ViolentKickPlayers);
                        break;
                    }
                case 12140:
                    {
                        Spell.Damage = (uint)(Spell.Damage * StormKickPlayers);
                        break;
                    }
                case 12240:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonPunchPlayers);
                        break;
                    }
                case 12290:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonCyclonePlayers);
                        break;
                    }
                case 12300:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonFuryPlayers);
                        break;
                    }
                case 12320:
                    {
                        Spell.Damage = (uint)(Spell.Damage * AirKickPlayers);
                        break;
                    }
                case 12330:
                    {
                        Spell.Damage = (uint)(Spell.Damage * AirSweepPlayers);
                        break;
                    }
                case 12350:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonSlashPlayers);
                        break;
                    }
                case 12070:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TwilightDancePlayers);
                        break;
                    }
                case 12080:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SuperTwBladePlayers);
                        break;
                    }
                case 12110:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FatalSpinPlayers);
                        break;
                    }
                case 12160:
                    {
                        Spell.Damage = (uint)(Spell.Damage * CrackingSwipePlayers);
                        break;
                    }
                case 12170:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SplittingSwipePlayers);
                        break;
                    }
                case 12120:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SpeedKickPlayers);
                        break;
                    }
                case 11100:
                    {
                        Spell.Damage = (uint)(Spell.Damage * KrakensRevengePlayers);
                        break;
                    }
                case 11110:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BladeTempestPlayers);
                        break;
                    }
                case 11070:
                    {
                        Spell.Damage = (uint)(Spell.Damage * GaleBombPlayers);
                        break;
                    }
                case 11140:
                    {
                        Spell.Damage = (uint)(Spell.Damage * WindstormPlayers);
                        break;
                    }
                case 11040:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ScurvyBombPlayers);
                        break;
                    }
                case 11050:
                    {
                        Spell.Damage = (uint)(Spell.Damage * CannonBarragePlayers);
                        break;
                    }
                case 5040:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BoomPlayers);
                        break;
                    }
                case 7000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SeizerPlayers);
                        break;
                    }
                case 11005:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ViperFangPlayers);
                        break;
                    }
                case 1290:
                    {
                        Spell.Damage = (uint)(Spell.Damage * PenetrationPlayers);
                        break;
                    }
                case 5030:
                    {
                        Spell.Damage = (uint)(Spell.Damage * PhoenixPlayers);
                        break;
                    }
                case 10315:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonWhirlPlayers);
                        break;
                    }
                case 10381:
                    {
                        Spell.Damage = (uint)(Spell.Damage * RadiantPalmPlayers);
                        break;
                    }
                case 1045:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FastBladePlayers);
                        break;
                    }
                case 1046:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ScentSwordPlayers);
                        break;
                    }
                case 1115:
                    {
                        Spell.Damage = (uint)(Spell.Damage * HerculesPlayers);
                        break;
                    }
                case 7020:
                    {
                        Spell.Damage = (uint)(Spell.Damage * RagePlayers);
                        break;
                    }
                case 11000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * DragonTailPlayers);
                        break;
                    }
                case 7030:
                    {
                        Spell.Damage = (uint)(Spell.Damage * CelestialPlayers);
                        break;
                    }
                case 11190:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ChargingVortexPlayers);
                        break;
                    }
                case 10415:
                    {
                        Spell.Damage = (uint)(Spell.Damage * WhirlwindKickPlayers);
                        break;
                    }
                case 10490:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TripleAttackPlayers);
                        break;
                    }
                case 10390:
                    {
                        Spell.Damage = (uint)(Spell.Damage * OblivionPlayers);
                        break;
                    }
                case 1260:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SpeedGunPlayers);
                        break;
                    }
                case 5010:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SnowPlayers);
                        break;
                    }
                case 1000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ThunderPlayers);
                        break;
                    }
                case 1001:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FirePlayers);
                        break;
                    }
                case 1125:
                    {
                        Spell.Damage = (uint)(Spell.Damage * VulcanoPlayers);
                        break;
                    }
                case 1010:
                    {
                        Spell.Damage = (uint)(Spell.Damage * LightningPlayers);
                        break;
                    }
                case 5001:
                    {
                        Spell.Damage = (uint)(Spell.Damage * SpeedLightningPlayers);
                        break;
                    }
                case 3090:
                    {
                        Spell.Damage = (uint)(Spell.Damage * PrevadePlayers);
                        break;
                    }
                case 6000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TwofoldBladePlayers);
                        break;
                    }
                case 11170:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BloodyScythePlayers);
                        break;
                    }
                case 6003:
                    {
                        Spell.Damage = (uint)(Spell.Damage * CounterKillPlayers);
                        break;
                    }
                case 6010:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ShurikenVPlayers);
                        break;
                    }
                case 11230:
                    {
                        Spell.Damage = (uint)(Spell.Damage * GapingWoundsPlayers);
                        break;
                    }
                case 1002:
                    {
                        Spell.Damage = (uint)(Spell.Damage * TornadoPlayers);
                        break;
                    }
                case 1160:
                    {
                        Spell.Damage = (uint)(Spell.Damage * BombPlayers);
                        break;
                    }
                case 10309:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ChainBoltPlayers);
                        break;
                    }
                case 1150:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FireRingPlayers);
                        break;
                    }
                case 1180:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FireMeteorPlayers);
                        break;
                    }
                case 1120:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FireCirclePlayers);
                        break;
                    }
                case 1165:
                    {
                        Spell.Damage = (uint)(Spell.Damage * FireofHellPlayers);
                        break;
                    }
                case 10310:
                    {
                        Spell.Damage = (uint)(Spell.Damage * HeavenBladePlayers);
                        break;
                    }
                case 8001:
                    {
                        Spell.Damage = (uint)(Spell.Damage * ScatterPlayers);
                        break;
                    }
                case 8000:
                    {
                        Spell.Damage = (uint)(Spell.Damage * RapidFirePlayers);
                        break;
                    }
            }
        }
        #region Save
        public static void Update(uint SkillUID, double Damge)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
            {
                cmd.Update("RayzoAttack").Set("Damge", Damge)
                .Where("SkilUID", SkillUID);
                if (cmd.Execute2() > 0)
                {
                }
            }
        }
        private void SaveTrojan_Click(object sender, EventArgs e)
        {
            FatalCrossPlayers = double.Parse(Txt_FatalCross.Text);
            MortalStrikePlayers = double.Parse(Txt_MortalStrike.Text);
            HerculesPlayers = double.Parse(HercDameg.Text);
            FastBladePlayers = double.Parse(FastDamge.Text);
            ScentSwordPlayers = double.Parse(SSdamge.Text);
            RagePlayers = double.Parse(RageDamge.Text);
            PhoenixPlayers = double.Parse(PhoenixDmg.Text);
            DragonWhirlPlayers = double.Parse(textDragonWhirl.Text);
            //////////////////////
            Update(FatalCrossSkilUID, FatalCrossPlayers);
            Update(MortalStrikeSkilUID, MortalStrikePlayers);
            Update(FastBladeSkilUID, FastBladePlayers);
            Update(ScentSwordSkilUID, ScentSwordPlayers);
            Update(RageSkilUID, RagePlayers);
            Update(PhoenixSkilUID, PhoenixPlayers);
            Update(DragonWhirlSkilUID, DragonWhirlPlayers);
            Update(HerculesSkilUID, HerculesPlayers);
        }
        private void SaveFire_Click(object sender, EventArgs e)
        {
            ThunderPlayers = double.Parse(textThunder.Text);
            FirePlayers = double.Parse(textFire.Text);
            TornadoPlayers = double.Parse(textTornado.Text);
            BombPlayers = double.Parse(textBomb.Text);
            ChainBoltPlayers = double.Parse(textChainBolt.Text);
            FireRingPlayers = double.Parse(textFireRing.Text);
            FireMeteorPlayers = double.Parse(textFireMeteor.Text);
            FireCirclePlayers = double.Parse(textFireCircle.Text);
            FireofHellPlayers = double.Parse(textFireofHell.Text);
            HeavenBladePlayers = double.Parse(textHeavenBlade.Text);
            //
            Update(ThunderSkilUID, ThunderPlayers);
            Update(FireSkilUID, FirePlayers);
            Update(TornadoSkilUID, TornadoPlayers);
            Update(BombSkilUID, BombPlayers);
            Update(ChainBoltSkilUID, ChainBoltPlayers);
            Update(FireRingSkilUID, FireRingPlayers);
            Update(FireMeteorSkilUID, FireMeteorPlayers);
            Update(FireCircleSkilUID, FireCirclePlayers);
            Update(FireofHellSkilUID, FireofHellPlayers);
            Update(HeavenBladeSkilUID, HeavenBladePlayers);

        }

        private void SaveArcher_Click(object sender, EventArgs e)
        {
            BlisteringWavePlayers = double.Parse(Txt_BlisteringWave.Text);
            DaggerStormPlayers = double.Parse(Txt_DaggerStorm.Text);
            BladeFlurryPlayers = double.Parse(Txt_BladeFlurry.Text);
            MortalWoundPlayers = double.Parse(Txt_MortalWound.Text);
            ScatterPlayers = double.Parse(textScatter.Text);
            RapidFirePlayers = double.Parse(textRapidFire.Text);
            //
            Update(BladeFlurrySkilUID, BlisteringWavePlayers);
            Update(DaggerStormSkilUID, DaggerStormPlayers);
            Update(BladeFlurrySkilUID, BladeFlurryPlayers);
            Update(MortalWoundSkilUID, MortalWoundPlayers);
            Update(ScatterSkilUID, ScatterPlayers);
            Update(RapidFireSkilUID, RapidFirePlayers);

        }

        private void SaveWater_Click(object sender, EventArgs e)
        {
            SearingTouchPlayers = double.Parse(Txt_SearingTouch.Text);
            VulcanoPlayers = double.Parse(textVulcano.Text);
            LightningPlayers = double.Parse(textLightning.Text);
            SpeedLightningPlayers = double.Parse(textSpeedLightning.Text);
            PrevadePlayers = double.Parse(textPrevade.Text);
            //
            Update(SearingTouchSkilUID, SearingTouchPlayers);
            Update(VulcanoSkilUID, VulcanoPlayers);
            Update(LightningSkilUID, LightningPlayers);
            Update(SpeedLightningSkilUID, SpeedLightningPlayers);
            Update(PrevadeSkilUID, PrevadePlayers);
        }

        private void SaveWarrior_Click(object sender, EventArgs e)
        {

            ScarofEarthPlayers = double.Parse(Txt_ScarofEarth.Text);
            WaveofBloodPlayers = double.Parse(Txt_WaveofBlood.Text);
            ManiacDancePlayers = double.Parse(Txt_ManiacDance.Text);
            SpeedGunPlayers = double.Parse(textSpeedGun.Text);
            SnowPlayers = double.Parse(textSnow.Text);
            BoomPlayers = double.Parse(textBoom.Text);
            SeizerPlayers = double.Parse(textSeizer.Text);
            ViperFangPlayers = double.Parse(textViperFang.Text);
            PenetrationPlayers = double.Parse(textPenetration.Text);
            DragonTailPlayers = double.Parse(textDragonTail.Text);
            CelestialPlayers = double.Parse(textCelestial.Text);
            ChargingVortexPlayers = double.Parse(textChargingVortex.Text);


            Update(ScarofEarthSkilUID, ScarofEarthPlayers);
            Update(WaveofBloodSkilUID, WaveofBloodPlayers);
            Update(ManiacDanceSkilUID, ManiacDancePlayers);
            Update(SpeedGunSkilUID, SpeedGunPlayers);
            Update(SnowSkilUID, SnowPlayers);
            Update(BoomSkilUID, BoomPlayers);
            Update(SeizerSkilUID, SeizerPlayers);
            Update(ViperFangSkilUID, ViperFangPlayers);
            Update(PenetrationSkilUID, PenetrationPlayers);
            Update(DragonTailSkilUID, DragonTailPlayers);
            Update(CelestialSkilUID, CelestialPlayers);
            Update(ChargingVortexUID, ChargingVortexPlayers);
        }

        private void SaveNinja_Click(object sender, EventArgs e)
        {
            TwofoldBladePlayers = double.Parse(textTwofoldBlade.Text);
            SuperTwBladePlayers = double.Parse(Txt_SuperTwBlade.Text);
            BloodyScythePlayers = double.Parse(textBloodyScythe.Text);
            CounterKillPlayers = double.Parse(textCounterKill.Text);
            ShurikenVPlayers = double.Parse(textShurikenVortex.Text);
            GapingWoundsPlayers = double.Parse(textGapingWounds.Text);

            Update(TwofoldBladeSkilUID, TwofoldBladePlayers);
            Update(SuperTwBladeUID, SuperTwBladePlayers);
            Update(CounterKillSkilUID, BloodyScythePlayers);
            Update(ShurikenVSkilUID, ShurikenVPlayers);
            Update(GapingWoundsSkilUID, GapingWoundsPlayers);
            Update(BloodyScytheSkilUID, BloodyScythePlayers);

        }

        private void SaveMonk_Click(object sender, EventArgs e)
        {
            RadiantPalmPlayers = double.Parse(textPalm.Text);
            WhirlwindKickPlayers = double.Parse(textWhirlwindKick.Text);
            TripleAttackPlayers = double.Parse(textTripleAttack.Text);
            OblivionPlayers = double.Parse(textOblivion.Text);
            WrathoftheEmperorPlayers = double.Parse(Txt_WrathoftheEmperor.Text);
            InfernalEchoPlayers = double.Parse(Txt_InfernalEcho.Text);
            StrikePlayers = double.Parse(Txt_Strike.Text);


            Update(WrathoftheEmperorSkilUID, WrathoftheEmperorPlayers);
            Update(InfernalEchoSkillUID, InfernalEchoPlayers);
            Update(StrikeSkilUID, StrikePlayers);
            Update(RadiantPalmSkilUID, RadiantPalmPlayers);
            Update(WhirlwindKickSkilUID, WhirlwindKickPlayers);
            Update(TripleAttackSkilUID, TripleAttackPlayers);
            Update(OblivionSkilUID, OblivionPlayers);

        }

        private void SavePatite_Click(object sender, EventArgs e)
        {
            BladeTempestPlayers = double.Parse(textBladeTempest.Text);
            GaleBombPlayers = double.Parse(textGaleBomb.Text);
            WindstormPlayers = double.Parse(textWindstorm.Text);
            ScurvyBombPlayers = double.Parse(textScurvyBomb.Text);
            CannonBarragePlayers = double.Parse(textCannonBarrage.Text);
            KrakensRevengePlayers = double.Parse(textKrakensRevenge.Text);

            Update(BladeTempestSkilUID, BladeTempestPlayers);
            Update(GaleBombSkilUID, GaleBombPlayers);
            Update(WindstormSkilUID, WindstormPlayers);
            Update(ScurvyBombSkilUID, ScurvyBombPlayers);
            Update(CannonBarrageSkilUID, CannonBarragePlayers);
            Update(KrakensRevengeSkilUID, KrakensRevengePlayers);

        }
        private void Btn_Save_DragonW_Click(object sender, EventArgs e)
        {

            CrackingSwipePlayers = double.Parse(Txt_CrackingSwipe.Text);
            SplittingSwipePlayers = double.Parse(Txt_SplittingSwipe.Text);
            SpeedKickPlayers = double.Parse(Txt_SpeedKick.Text);
            ViolentKickPlayers = double.Parse(Txt_ViolentKick.Text);
            StormKickPlayers = double.Parse(Txt_StormKick.Text);
            DragonPunchPlayers = double.Parse(Txt_DragonPunch.Text);
            DragonCyclonePlayers = double.Parse(Txt_DragonCyclone.Text);
            DragonSlashPlayers = double.Parse(Txt_DragonSlash.Text);
            AirKickPlayers = double.Parse(Txt_AirKick.Text);
            AirSweepPlayers = double.Parse(Txt_AirSweep.Text);
            AirRaidPlayers = double.Parse(Txt_AirRaid.Text);
            DragonFuryPlayers = double.Parse(Txt_DragonFury.Text);

            ////////////////////////////////////UPDATE////////////

            Update(CrackingSwipeSkilUID, CrackingSwipePlayers);
            Update(SplittingSwipeSkilUID, SplittingSwipePlayers);
            Update(SpeedKickSkilUID, SpeedKickPlayers);
            Update(ViolentKickSkilUID, ViolentKickPlayers);
            Update(StormKickSkilUID, StormKickPlayers);
            Update(DragonPunchSkilUID, DragonPunchPlayers);
            Update(DragonCycloneSkilUID, DragonCyclonePlayers);
            Update(DragonFurySkilUID, DragonFuryPlayers);
            Update(AirKickSkilUID, AirKickPlayers);
            Update(AirSweepSkilUID, AirSweepPlayers);
            Update(AirRaidSkilUID, AirRaidPlayers);
            Update(DragonSlashSkilUID, DragonSlashPlayers);
        }

        private void Btn_Save_Wind_Click(object sender, EventArgs e)
        {


            TripleBlastsPlayers = double.Parse(Txt_TripleBlasts.Text);
            OmnipotencePlayers = double.Parse(Txt_Omnipotence.Text);
            SwirlingStormPlayers = double.Parse(Txt_SwirlingStorm.Text);
            RageofWarPlayers = double.Parse(Txt_RageofWar.Text);
            BurntFrostPlayers = double.Parse(Txt_BurntFrost.Text);
            AngerofStomperPlayers = double.Parse(Txt_AngerofStomper.Text);
            ThundercloudPlayers = double.Parse(Txt_Thundercloud.Text);
            ThundercloudAttackPlayers = double.Parse(Txt_ThundercloudAttack.Text);


            ////////////////////////////////////UPDATE////////////
            Update(TripleBlastsSkilUID, TripleBlastsPlayers);
            Update(OmnipotenceSkilUID, OmnipotencePlayers);
            Update(SwirlingStormSkilUID, SwirlingStormPlayers);
            Update(RageofWarSkilUID, RageofWarPlayers);
            Update(BurntFrostSkilUID, BurntFrostPlayers);
            Update(AngerofStomperSkilUID, AngerofStomperPlayers);
            Update(ThundercloudSkilUID, ThundercloudPlayers);
            Update(ThundercloudAttackSkilUID, ThundercloudAttackPlayers);




        }
        #endregion

        private void SaveChar(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }


            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


        #endregion
    }
}
