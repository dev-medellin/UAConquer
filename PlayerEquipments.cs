using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Properties;
using TheChosenProject.ServerCore;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheChosenProject.WindowsAPI;


namespace TheChosenProject
{
    public class PlayerEquipments : Form
    {
        private IContainer components;
        private PictureBox Headger;
        private PictureBox Necklace;
        private PictureBox Ring;
        private PictureBox Armor;
        private PictureBox RightWeapon;
        private PictureBox LeftAccessory;
        private PictureBox HeavenFan;
        private PictureBox StarTower;
        private PictureBox RidingCrop;
        private PictureBox Boot;
        private PictureBox Garment;
        private PictureBox Steed;
        private PictureBox Bottle;
        private Label Spouse;
        private Label PlayerName;
        private Label Title;
        private Label BattlePower;

        public PlayerEquipments() => this.InitializeComponent();

        public string GetImagePath(string UID)
        {
            string str = new IniFile("\\ItemMinIcon.ini").ReadString("Item" + UID, "Frame0", "ItemDefault");
            return ServerKernel.CO2FOLDER + str;
        }

        public void FetchPlayerEquipments(GameClient client)
        {
            foreach (MsgGameItem msgGameItem in client.Equipment.CurentEquip)
            {
                if (msgGameItem.Position == (ushort)1)
                    this.Headger.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)2)
                    this.Necklace.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)3)
                    this.Armor.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)4)
                    this.RightWeapon.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)15)
                    this.LeftAccessory.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)6)
                    this.Ring.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)8)
                    this.Boot.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)10)
                    this.HeavenFan.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)11)
                    this.StarTower.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)18)
                    this.RidingCrop.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)7)
                    this.Bottle.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)9)
                    this.Garment.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
                if (msgGameItem.Position == (ushort)12)
                    this.Steed.ImageLocation = this.GetImagePath(msgGameItem.ITEM_ID.ToString());
            }
        }

        public PlayerEquipments(GameClient client)
        {
            try
            {
                this.InitializeComponent();
                this.Text = client.Player.Name + " Equipment`s";
                this.FetchPlayerEquipments(client);
                this.PlayerName.Text = client.Player.Name;
                this.Title.Text = ((MsgTitle.TitleType)client.Player.MyTitle).ToString();
                this.Spouse.Text = client.Player.Spouse;
                this.BattlePower.Text = client.Player.BattlePower.ToString();
            }
            catch
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PlayerEquipments));
            this.Headger = new PictureBox();
            this.Necklace = new PictureBox();
            this.Ring = new PictureBox();
            this.Armor = new PictureBox();
            this.RightWeapon = new PictureBox();
            this.LeftAccessory = new PictureBox();
            this.HeavenFan = new PictureBox();
            this.StarTower = new PictureBox();
            this.RidingCrop = new PictureBox();
            this.Boot = new PictureBox();
            this.Garment = new PictureBox();
            this.Steed = new PictureBox();
            this.Bottle = new PictureBox();
            this.Spouse = new Label();
            this.PlayerName = new Label();
            this.Title = new Label();
            this.BattlePower = new Label();
            ((ISupportInitialize)this.Headger).BeginInit();
            ((ISupportInitialize)this.Necklace).BeginInit();
            ((ISupportInitialize)this.Ring).BeginInit();
            ((ISupportInitialize)this.Armor).BeginInit();
            ((ISupportInitialize)this.RightWeapon).BeginInit();
            ((ISupportInitialize)this.LeftAccessory).BeginInit();
            ((ISupportInitialize)this.HeavenFan).BeginInit();
            ((ISupportInitialize)this.StarTower).BeginInit();
            ((ISupportInitialize)this.RidingCrop).BeginInit();
            ((ISupportInitialize)this.Boot).BeginInit();
            ((ISupportInitialize)this.Garment).BeginInit();
            ((ISupportInitialize)this.Steed).BeginInit();
            ((ISupportInitialize)this.Bottle).BeginInit();
            this.SuspendLayout();
            this.Headger.BackColor = Color.Transparent;
            this.Headger.Location = new Point(189, 91);
            this.Headger.Name = "Headger";
            this.Headger.Size = new Size(40, 40);
            this.Headger.TabIndex = 0;
            this.Headger.TabStop = false;
            this.Necklace.BackColor = Color.Transparent;
            this.Necklace.Location = new Point(189, 140);
            this.Necklace.Name = "Necklace";
            this.Necklace.Size = new Size(40, 40);
            this.Necklace.TabIndex = 1;
            this.Necklace.TabStop = false;
            this.Ring.BackColor = Color.Transparent;
            this.Ring.Location = new Point(43, 140);
            this.Ring.Name = "Ring";
            this.Ring.Size = new Size(40, 40);
            this.Ring.TabIndex = 2;
            this.Ring.TabStop = false;
            this.Armor.BackColor = Color.Transparent;
            this.Armor.Location = new Point(189, 291);
            this.Armor.Name = "Armor";
            this.Armor.Size = new Size(40, 40);
            this.Armor.TabIndex = 3;
            this.Armor.TabStop = false;
            this.RightWeapon.BackColor = Color.Transparent;
            this.RightWeapon.Location = new Point(189, 196);
            this.RightWeapon.Name = "RightWeapon";
            this.RightWeapon.Size = new Size(40, 40);
            this.RightWeapon.TabIndex = 4;
            this.RightWeapon.TabStop = false;
            this.LeftAccessory.BackColor = Color.Transparent;
            this.LeftAccessory.Location = new Point(189, 235);
            this.LeftAccessory.Name = "LeftAccessory";
            this.LeftAccessory.Size = new Size(40, 40);
            this.LeftAccessory.TabIndex = 5;
            this.LeftAccessory.TabStop = false;
            this.HeavenFan.BackColor = Color.Transparent;
            this.HeavenFan.Location = new Point(43, 391);
            this.HeavenFan.Name = "HeavenFan";
            this.HeavenFan.Size = new Size(40, 40);
            this.HeavenFan.TabIndex = 6;
            this.HeavenFan.TabStop = false;
            this.StarTower.BackColor = Color.Transparent;
            this.StarTower.Location = new Point(91, 391);
            this.StarTower.Name = "StarTower";
            this.StarTower.Size = new Size(40, 40);
            this.StarTower.TabIndex = 7;
            this.StarTower.TabStop = false;
            this.RidingCrop.BackColor = Color.Transparent;
            this.RidingCrop.Location = new Point(141, 391);
            this.RidingCrop.Name = "RidingCrop";
            this.RidingCrop.Size = new Size(40, 40);
            this.RidingCrop.TabIndex = 8;
            this.RidingCrop.TabStop = false;
            this.Boot.BackColor = Color.Transparent;
            this.Boot.Location = new Point(43, 241);
            this.Boot.Name = "Boot";
            this.Boot.Size = new Size(40, 40);
            this.Boot.TabIndex = 9;
            this.Boot.TabStop = false;
            this.Garment.BackColor = Color.Transparent;
            this.Garment.Location = new Point(189, 330);
            this.Garment.Name = "Garment";
            this.Garment.Size = new Size(40, 40);
            this.Garment.TabIndex = 10;
            this.Garment.TabStop = false;
            this.Steed.BackColor = Color.Transparent;
            this.Steed.Location = new Point(43, 291);
            this.Steed.Name = "Steed";
            this.Steed.Size = new Size(40, 40);
            this.Steed.TabIndex = 11;
            this.Steed.TabStop = false;
            this.Bottle.BackColor = Color.Transparent;
            this.Bottle.Location = new Point(43, 191);
            this.Bottle.Name = "Bottle";
            this.Bottle.Size = new Size(40, 40);
            this.Bottle.TabIndex = 12;
            this.Bottle.TabStop = false;
            this.Spouse.Anchor = AnchorStyles.None;
            this.Spouse.AutoSize = true;
            this.Spouse.BackColor = Color.Transparent;
            this.Spouse.Font = new Font("Segoe UI", 9f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte)0);
            this.Spouse.ForeColor = Color.Gold;
            this.Spouse.Location = new Point(102, 115);
            this.Spouse.Margin = new Padding(4, 0, 4, 0);
            this.Spouse.Name = "Spouse";
            this.Spouse.Size = new Size(37, 15);
            this.Spouse.TabIndex = 16;
            this.Spouse.Text = "None";
            this.PlayerName.Anchor = AnchorStyles.None;
            this.PlayerName.AutoSize = true;
            this.PlayerName.BackColor = Color.Transparent;
            this.PlayerName.Font = new Font("Segoe UI", 9f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte)0);
            this.PlayerName.ForeColor = Color.Gold;
            this.PlayerName.Location = new Point(92, 95);
            this.PlayerName.Margin = new Padding(4, 0, 4, 0);
            this.PlayerName.Name = "PlayerName";
            this.PlayerName.Size = new Size(37, 15);
            this.PlayerName.TabIndex = 17;
            this.PlayerName.Text = "None";
            this.Title.Anchor = AnchorStyles.None;
            this.Title.AutoSize = true;
            this.Title.BackColor = Color.Transparent;
            this.Title.Font = new Font("Segoe UI", 9f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte)0);
            this.Title.ForeColor = Color.Gold;
            this.Title.Location = new Point(92, 75);
            this.Title.Margin = new Padding(4, 0, 4, 0);
            this.Title.Name = "Title";
            this.Title.Size = new Size(37, 15);
            this.Title.TabIndex = 18;
            this.Title.Text = "None";
            this.BattlePower.Anchor = AnchorStyles.None;
            this.BattlePower.AutoSize = true;
            this.BattlePower.BackColor = Color.Transparent;
            this.BattlePower.Font = new Font("Segoe UI Black", 9.75f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte)0);
            this.BattlePower.ForeColor = Color.Gold;
            this.BattlePower.Location = new Point(13, 23);
            this.BattlePower.Margin = new Padding(4, 0, 4, 0);
            this.BattlePower.Name = "BattlePower";
            this.BattlePower.Size = new Size(32, 17);
            this.BattlePower.TabIndex = 19;
            this.BattlePower.Text = "000";
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.BackgroundImage = (Image)Resources.DialogNpcEquip1BG;
            this.ClientSize = new Size(264, 502);
            this.Controls.Add((Control)this.BattlePower);
            this.Controls.Add((Control)this.Title);
            this.Controls.Add((Control)this.PlayerName);
            this.Controls.Add((Control)this.Spouse);
            this.Controls.Add((Control)this.Bottle);
            this.Controls.Add((Control)this.Steed);
            this.Controls.Add((Control)this.Garment);
            this.Controls.Add((Control)this.Boot);
            this.Controls.Add((Control)this.RidingCrop);
            this.Controls.Add((Control)this.StarTower);
            this.Controls.Add((Control)this.HeavenFan);
            this.Controls.Add((Control)this.LeftAccessory);
            this.Controls.Add((Control)this.RightWeapon);
            this.Controls.Add((Control)this.Armor);
            this.Controls.Add((Control)this.Ring);
            this.Controls.Add((Control)this.Necklace);
            this.Controls.Add((Control)this.Headger);
            this.Font = new Font("Segoe UI", 8.25f, FontStyle.Italic, GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.Name = nameof(PlayerEquipments);
            this.Text = "PlayerEquipment`s";
            ((ISupportInitialize)this.Headger).EndInit();
            ((ISupportInitialize)this.Necklace).EndInit();
            ((ISupportInitialize)this.Ring).EndInit();
            ((ISupportInitialize)this.Armor).EndInit();
            ((ISupportInitialize)this.RightWeapon).EndInit();
            ((ISupportInitialize)this.LeftAccessory).EndInit();
            ((ISupportInitialize)this.HeavenFan).EndInit();
            ((ISupportInitialize)this.StarTower).EndInit();
            ((ISupportInitialize)this.RidingCrop).EndInit();
            ((ISupportInitialize)this.Boot).EndInit();
            ((ISupportInitialize)this.Garment).EndInit();
            ((ISupportInitialize)this.Steed).EndInit();
            ((ISupportInitialize)this.Bottle).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
