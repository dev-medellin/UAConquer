using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

 
namespace TheChosenProject
{
  public class Live : Form
  {
    private IContainer components;
    private DataGridView dataGridView1;

    public Live() => this.InitializeComponent();

    public Live(string name, uint mapid, uint dinamic, Live.LiveList type)
    {
      try
      {
        this.InitializeComponent();
        switch (type)
        {
          case Live.LiveList.Boss:
            this.Text = "ALive " + name + " Boss";
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnCount = 3;
            this.dataGridView1.Columns[0].Name = "Boss";
            this.dataGridView1.Columns[1].Name = "Player Name";
            this.dataGridView1.Columns[2].Name = "Player Damage";
            this.dataGridView1.Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Italic);
            string[] strArray1 = new string[3];
            using (IEnumerator<KeyValuePair<uint, MonsterRole.ScoreBoard>> enumerator = Server.MonsterRole.Values.Where<MonsterRole>((Func<MonsterRole, bool>) (p => p.Name.ToLower().Contains(name.ToLower()) && p.Alive)).FirstOrDefault<MonsterRole>().Scores.OrderByDescending<KeyValuePair<uint, MonsterRole.ScoreBoard>, uint>((Func<KeyValuePair<uint, MonsterRole.ScoreBoard>, uint>) (e => e.Value.ScoreDmg)).Take<KeyValuePair<uint, MonsterRole.ScoreBoard>>(5).GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<uint, MonsterRole.ScoreBoard> current = enumerator.Current;
                if (current.Value != null)
                {
                  strArray1[0] = name;
                  strArray1[1] = current.Value.Name;
                  strArray1[2] = current.Value.ScoreDmg.ToString();
                  this.dataGridView1.Rows.Add((object[]) strArray1);
                }
              }
              break;
            }
          case Live.LiveList.Tournament:
            this.Text = "ALive " + name + " Tournament";
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnCount = 5;
            this.dataGridView1.Columns[0].Name = "Name";
            this.dataGridView1.Columns[1].Name = "Rank";
            this.dataGridView1.Columns[2].Name = "Life";
            this.dataGridView1.Columns[3].Name = "Mana";
            this.dataGridView1.Columns[4].Name = "Class";
            this.dataGridView1.Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Italic);
            string[] strArray2 = new string[5];
            IEnumerable<GameClient> gameClients = Server.GamePoll.Values.Where<GameClient>((Func<GameClient, bool>) (p => (int) p.Map.ID == (int) mapid));
            if (dinamic != (uint) byte.MaxValue)
              gameClients = Server.GamePoll.Values.Where<GameClient>((Func<GameClient, bool>) (p => (int) p.Map.ID == (int) mapid && (int) p.Player.DynamicID == (int) dinamic)).Take<GameClient>(20);
            using (IEnumerator<GameClient> enumerator = gameClients.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                GameClient current = enumerator.Current;
                if (current != null)
                {
                  strArray2[0] = current.Player.Name;
                  strArray2[1] = current.Player.NobilityRank.ToString();
                  strArray2[2] = current.Player.HitPoints.ToString();
                  strArray2[3] = current.Player.Mana.ToString();
                  strArray2[4] = ((Flags.ProfessionType) current.Player.Class).ToString();
                  this.dataGridView1.Rows.Add((object[]) strArray2);
                }
              }
              break;
            }
        }
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
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Live));
      this.dataGridView1 = new DataGridView();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.SuspendLayout();
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.dataGridView1.BackgroundColor = SystemColors.ControlLightLight;
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Location = new Point(12, 12);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ReadOnly = true;
      this.dataGridView1.Size = new Size(647, 333);
      this.dataGridView1.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.ControlLightLight;
      this.ClientSize = new Size(675, 360);
      this.Controls.Add((Control) this.dataGridView1);
      this.Font = new Font("Segoe UI", 8.25f, FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.Name = "Alive";
      this.Text = "FrmAlive";
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.ResumeLayout(false);
    }

    public enum LiveList
    {
      Boss,
      Tournament,
    }
  }
}
