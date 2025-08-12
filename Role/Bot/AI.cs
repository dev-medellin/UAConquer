using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors.Filtering.Templates;
using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Role.Bot
{
    /// <summary>
    /// Description of AIBot.
    /// </summary>
    public class AI
    {

        private int JumpSpeed = 0;
        private int BaseJumpSpeed = 0;
        private int Accuracy = 0;
        private string Name = string.Empty;

        private List<string> NameUsed = new List<string>();

        private DateTime LastAtttack = DateTime.Now;

        private SkillType SkillType;

        /// <summary>
        /// Set Level of AI
        /// </summary>
        /// <param name="Level">Difficult of AI</param>
        public void SetLevel(BotLevel Level)
        {
            this.Level = Level;
            switch (Level)
            {
                case BotLevel.Easy:
                    BaseJumpSpeed = 400;
                    Accuracy = 50;
                    Name = "EasyBOT";
                    break;

                case BotLevel.Normal:
                    BaseJumpSpeed = 300;
                    Accuracy = 75;
                    Name = "MidBOT";
                    break;

                case BotLevel.Medium:
                    BaseJumpSpeed = 200;
                    Accuracy = 90;
                    Name = "HardBOT";
                    break;

                case BotLevel.Hard:
                    BaseJumpSpeed = 100;
                    Accuracy = 95;
                    Name = "InsaneBOT";
                    break;

                case BotLevel.Insane:
                    BaseJumpSpeed = 20;
                    Accuracy = 98;
                    Name = "DivineBOT";
                    break;
            }
        }

        // Fields and Properties 
        public Client.GameClient Bot;
        public Client.GameClient Oppenents;
        public DateTime ToStart;
        public DateTime msgStart;
        public BotLevel Level;
        public BotType BotType;

        public DateTime PotStamp = DateTime.Now;

        public AI()
        {
            Bot = new Client.GameClient(null) { Fake = true };
        }

        public List<ushort> Spells = new List<ushort>();
        public void LoadBot(BotType BotType, Client.GameClient Oppenent, SkillType skillType)
        {
            Oppenents = Oppenent;
            SkillType = skillType;
            this.BotType = BotType;
            if (Oppenent is null) return;


            Bot.Player = new Player(Bot);
            Bot.Inventory = new Instance.Inventory(Bot);
            Bot.Equipment = new Instance.Equip(Bot);
            Bot.Player.SubClass = new Instance.SubClass();
            Bot.MySpells = new Instance.Spell(Bot);

            switch (BotType)
            {
                case BotType.DuelBot:
                    {
                        Bot.Player.Name = Name;

                        Bot.Player.UID = Database.Server.BotCounter.Next;

                        Bot.Player.Avatar = Oppenent.Player.Avatar;//face
                        Bot.Player.Body = 1003;
                        Bot.Player.Hair = 935;
                        Bot.Player.TransformationID = 0;
                        Bot.Player.Strength = 176;
                        Bot.Player.Agility = 36;
                        Bot.Player.Vitality = 500;
                        Bot.Player.Spirit = 0;
                        Bot.Player.PKPoints = 0;
                        Bot.Player.Level = 140;
                        Bot.Player.Class = 15;
                        Bot.Player.Map = Oppenent.Player.Map;
                        Bot.Player.DynamicID = Oppenent.Player.DynamicID;
                        Bot.Map = Database.Server.ServerMaps[Oppenent.Player.Map];
                        Bot.Player.X = Oppenent.Player.X;
                        Bot.Player.Y = Oppenent.Player.Y;
                        Bot.Player.LimitHits = Oppenent.Player.LimitHits;
                        Bot.Map.Enquer(Bot);

                        Bot.Player.HitPoints = 1000;
                        Bot.Status = new MsgStatus();
                        Bot.Status.MaxHitpoints = 1000;
                        Bot.Player.Action = Flags.ConquerAction.Sit;
                        Bot.Player.Stamina = 100;

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Bot.Equipment.Add(stream, 410301, Flags.ConquerItem.RightWeapon);
                            Bot.Equipment.Add(stream, 420003, Flags.ConquerItem.LeftWeapon);
                            Bot.Equipment.Add(stream, 130109, Flags.ConquerItem.Armor);

                            Bot.Player.RightWeaponId = 410301;
                            Bot.Player.LeftWeaponId = 420003;
                            Bot.Player.ArmorId = 130109;

                            if (!Bot.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.FastBlader))
                                Bot.MySpells.Add(stream, (ushort)Flags.SpellID.FastBlader, 4);
                            if (!Bot.MySpells.ClientSpells.ContainsKey((ushort)Flags.SpellID.ScrenSword))
                                Bot.MySpells.Add(stream, (ushort)Flags.SpellID.ScrenSword, 4);

                            Spells.Add((ushort)Flags.SpellID.FastBlader);
                            Spells.Add((ushort)Flags.SpellID.ScrenSword);


                            Bot.Player.ServerID = 1;
                            Oppenent.Send(Bot.Player.GetArray(stream, false));
                            Bot.Player.View.Role();
                            Bot.Player.SetPkMode(Flags.PKMode.PK);
                            BeginJumpBot(Oppenent);
                            ToStart = DateTime.Now.AddSeconds(3);
                            msgStart = DateTime.Now;
                        }
                        break;
                    }
            }

        }

        #region Jump Bot
        public DateTime LastBotJump = DateTime.Now;
        public void HandleJump()
        {
            if (DateTime.Now < ToStart)
                return;

            if (DateTime.Now >= LastBotJump.AddMilliseconds(JumpSpeed))
            {
                LastBotJump = DateTime.Now;
                Jump_Action();
                
                    if (Level == BotLevel.Insane)
                    {
                        if (!Bot.Player.Target.Alive)
                        {
                            foreach (var target in Bot.Player.View.Roles(MapObjectType.Player))
                            {
                                var attacked = target as Player;
                                if (attacked.Alive)
                                    Bot.Player.Target = attacked;
                            }
                        }
                        if (Bot.Player.Map == Bot.Player.Target.Map && Bot.Player.DynamicID == Bot.Player.Target.DynamicID)
                        {
                            #region fb / ss
                            Shoot(Accuracy);

                            LastAtttack = DateTime.Now;
                            #endregion
                        }
                        else Dispose();

                    }
                
            }
        }
        public void Attack()
        {
            if (DateTime.Now < ToStart)
                return;
            if (Level == BotLevel.Insane) return;
            if (DateTime.Now >= LastAtttack.AddMilliseconds(500 + BaseJumpSpeed))
            {
                //if (Calculations.GetDistance(Bot.Player.X, Bot.Player.Y, Bot.Player.Target.X, Bot.Player.Target.Y) <= 10)
                {
                    if (!Bot.Player.Target.Alive)
                    {
                        foreach (var target in Bot.Player.View.Roles(MapObjectType.Player))
                        {
                            var attacked = target as Player;
                            if (attacked.Alive)
                                Bot.Player.Target = attacked;
                        }
                    }
                    if (Bot.Player.Map == Bot.Player.Target.Map && Bot.Player.DynamicID == Bot.Player.Target.DynamicID)
                    {
                        #region fb / ss
                        Shoot(Accuracy);

                        LastAtttack = DateTime.Now;
                        #endregion
                    }
                    else Dispose();
                }
            }
        }

        public void BeginJumpBot(Client.GameClient target)
        {
            BotProcessring.Bots.TryAdd(Bot.Player.UID, this);

            Bot.Player.Target = target.Player;
        }

        public void StopJumpBot()
        {
            BotProcessring.Bots.TryRemove(Bot.Player.UID, out _);
            if (NameUsed.Contains(Bot.Player.Name))
                NameUsed.Remove(Bot.Player.Name);
        }

        private void Jump_Action()
        {
            if (Bot.Player.Target is null) return;
            Jump();
        }
        private static Random rnd = new Random((int)Time32.Now.Value);
        private unsafe void Jump()
        {
            ushort x = 0;
            ushort y = 0;
            if (Bot == null) return;
            if (Bot.Map == null) return;
            
            {
            jmp:
                
                    x = (ushort)(Bot.Player.Target.X + rnd.Next(-3, 3));
                    y = (ushort)(Bot.Player.Target.Y + rnd.Next(-3, 3));
                    if (!Bot.Map.ValidLocation(x, y))
                        goto jmp;
                

                var distance = Core.GetDistance(Bot.Player.X, Bot.Player.Y, x, y);
                if (BotType == BotType.ArenaBot)
                    JumpSpeed = Math.Max((int)(500 + distance * 100) + BaseJumpSpeed, 500);
                else
                    JumpSpeed = Math.Max((int)(400 + distance * 40) + BaseJumpSpeed, 500);

                var inter = new InterActionWalk()
                {
                    Mode = MsgInterAction.Action.Jump,
                    X = x,
                    Y = y,
                    UID = Bot.Player.UID,
                };

                Bot.Map.View.MoveTo<IMapObj>(Bot.Player, x, y);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Bot.Player.Target.View.SendView(stream.InterActionWalk(&inter), true);
                }
                Bot.Player.X = x;
                Bot.Player.Y = y;
            }
        }

        public void Shoot(int accu)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var interact = new InteractQuery
                {
                    AtkType = MsgAttackPacket.AttackID.Magic,
                    UID = Bot.Player.UID,
                };
                if (BotType == BotType.DuelBot)
                {
                    var spell = (ushort)(SkillType == SkillType.FastBlade ? 1045 : 1046);
                    interact.SpellID = spell;
                    interact.SpellLevel = 4;
                }
                if (Core.Rate(accu))
                {
                    interact.X = Bot.Player.Target.X;
                    interact.Y = Bot.Player.Target.Y;
                    interact.OpponentUID = Bot.Player.Target.UID;
                }
                else
                {
                    interact.X = (ushort)(Bot.Player.Target.X + 1);
                    interact.Y = (ushort)(Bot.Player.Target.Y + 1);
                }
                Bot.Player.TotalHits++;
                bool pass = false;
                bool hitSomeone = false;

                if (BotType == BotType.DuelBot)
                {
                    var magiceffect = new MsgSpellAnimation(Bot.Player.UID, 0, interact.X, interact.Y, interact.SpellID, interact.SpellLevel, 0);
                    if (interact.OpponentUID != 0)
                    {
                        hitSomeone = true;

                        if (!pass)
                        {
                            Bot.Player.Hits++;
                            Bot.Player.Chains++;
                            if (Bot.Player.Chains > Bot.Player.MaxChains)
                                Bot.Player.MaxChains = Bot.Player.Chains;
                            if (Bot.Player.LimitHits > 0)
                                if (Bot.Player.LimitHits <= Bot.Player.Hits)
                                {
                                    var p = Bot.Player.Target;
                                    Dispose();
                                    p.Owner.Teleport(214, 203, 1036);
                                    uint Rate = 0;
                                    Rate = (uint)(((float)p.Owner.Player.HitShoot / (float)p.Owner.Player.MisShoot) * 100);
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{p.Owner.Player.Name} defeat by {Name} with Chain: {p.Owner.Player.MaxChains} Hits: {p.Owner.Player.Hits} Miss: {p.Owner.Player.TotalHits - p.Owner.Player.Hits} Accuracy: {Math.Round((double)(p.Hits * 100.0 / Math.Max(1, p.TotalHits)), 2).ToString()} / 100", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                    return;
                                }
                           
                            pass = true;
                        }
                        magiceffect.Targets.Enqueue(new MsgSpellAnimation.SpellObj(interact.OpponentUID, (uint)Bot.Player.Hits, MsgAttackPacket.AttackEffect.None));
                        magiceffect.SetStream(stream);
                        magiceffect.Send(Bot.Player.Target.Owner, true);
                    }
                    else
                    {
                        if (!hitSomeone)
                            Bot.Player.Chains = 0;

                        MsgAttackPacket.ProcescMagic(Bot, stream, interact);
                        if (Bot.Player.LimitHits > 0)
                            if (Bot.Player.LimitHits <= Bot.Player.Hits)
                            {
                                var p = Bot.Player.Target;
                                Dispose();
                                p.Owner.Teleport(214, 203, 1036);
                                uint Rate = 0;
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{p.Owner.Player.Name} defeat by {Name} with Chain: {p.Owner.Player.MaxChains} Hits: {p.Owner.Player.Hits} Miss: {p.Owner.Player.TotalHits - p.Owner.Player.Hits} Accuracy: {Math.Round((double)(p.Hits * 100.0 / Math.Max(1, p.TotalHits)), 2).ToString()} / 100", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                return;
                            }
                    }
                    if (Bot != null)
                    {
                        Bot.Player.Target.Owner.SendSysMesage("Accuracy Rates.", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                        foreach (var player in Bot.Player.Target.Owner.Map.Values.Where(e => e.Player.DynamicID == Bot.Player.Target.Owner.Player.DynamicID))
                        {

                            Bot.Player.Target.Owner.SendSysMesage(player.Player.Name + " " + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) + "%, H: " + player.Player.Hits + ", M: " + (player.Player.TotalHits - player.Player.Hits) + ", M.C: " + player.Player.MaxChains, MsgMessage.ChatMode.ContinueRightCorner);
                        }
                    }
                }
            }
        }
        #endregion

        public void Dispose()
        {
            StopJumpBot();

            if (Bot == null)
                return;
            if (Bot.Map == null)
                return;
            Bot.Map.Denquer(Bot);
            Bot.Player.View.Role();

            Bot = null;
        }

    }
}
