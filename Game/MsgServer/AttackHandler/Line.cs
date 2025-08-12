using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.XtraEditors.Filtering.Templates;
using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Role.Bot;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public struct coords
    {
        public int X;
        public int Y;

        public coords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public class Line
    {
        public static List<coords> LineCoords(ushort userx, ushort usery, ushort shotx, ushort shoty, byte length)
        {

            double dir = Math.Atan2(shoty - usery, shotx - userx);
            double f_x = (Math.Cos(dir) * length) + userx;
            double f_y = (Math.Sin(dir) * length) + usery;

            return bresenham(userx, usery, (int)Math.Round(f_x), (int)Math.Round(f_y));
        }
        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }
        public static List<coords> bresenham(int x0, int y0, int x1, int y1)
        {
            List<coords> ThisLine = new List<coords>();


            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if (steep)
                    ThisLine.Add(y, x);
                else
                    ThisLine.Add(x, y);

                err = err - dY;
                if (err < 0) { y += ystep; err += dX; }
            }
            return ThisLine;
        }
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                if (Program.MapCounterHits.Contains(user.Player.Map))
                    user.Player.MisShoot += 1;
                if (UnlimitedArenaRooms.Maps.ContainsValue(user.Player.DynamicID))
                    user.Player.MisShoot += 1;
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.FastBlader:
                    case (ushort)Role.Flags.SpellID.DragonTail:
                    case (ushort)Role.Flags.SpellID.ScrenSword:
                    case (ushort)Role.Flags.SpellID.ViperFang:
                        {
                            if (user.Player.OnTransform)
                            {
                                user.SendSysMesage("You can't use this skill right now!");
                                break;
                            }

                            if (Database.ItemType.IsTwoHand(user.Equipment.RightWeapon) && user.Equipment.LeftWeapon != 0 && Database.ItemType.IsShield(user.Equipment.LeftWeapon) && !Database.ItemType.IsArrow(user.Equipment.LeftWeapon) && user.Inventory.HaveSpace(1))
                            {
                                using (RecycledPacket recycledPacket = new RecycledPacket())
                                {
                                    Packet stream2;
                                    stream2 = recycledPacket.GetStream();
                                    if (!user.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream2))
                                        user.Equipment.Remove(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream2);
                                    user.Equipment.LeftWeapon = 0u;
                                    user.Send(stream2);
                                }
                                user.SendSysMesage("Sorry you can`t use Two handle and Shield, this feature currently fixing!");
                                break;
                            }
                            bool pass = false;

                            user.Player.TotalHits++;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            byte Range = DBSpell.Range;
                            user.Player.Angle = Role.Core.GetAngle(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            Range += 4;
                            var LineRe = LineCoords(user.Player.X, user.Player.Y, Attack.X, Attack.Y, (byte)(Range));
                            byte LineRange = (byte)((ClientSpell.UseSpellSoul > 0) ? 0 : 0);

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster).Where(e => LineRe.Contains((ushort)(e.X), (ushort)(e.Y))))
                            {

                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                               
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 9)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            bool hitSomeone = false;
                            var PointLine = LineCoords(user.Player.X, user.Player.Y, Attack.X, Attack.Y, 9);
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player)
                                .Where(e => LineRe.Contains((ushort)(e.X), (ushort)(e.Y))))
                            {
                                var attacked = targer as Role.Player;
                                hitSomeone = true;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= 9)
                                    {
                                        //if (user.Player.Class >= 11 && user.Player.Class <= 15)
                                        //{
                                        //    user.Player.Stamina = (ushort)Math.Min((int)(user.Player.Stamina + 30), 150);
                                        //    user.Player.SendUpdate(stream, user.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                                        //}
                                        if (!pass)
                                        {
                                            user.Player.Hits++;
                                            user.Player.Chains++;
                                            if (user.Player.Chains > user.Player.MaxChains)
                                                user.Player.MaxChains = user.Player.Chains;
                                            if (user.Player.Map == 700 && user.Player.DynamicID != 0)
                                            {
                                                if (user.Player.LimitHits > 0)
                                                    if (user.Player.LimitHits == user.Player.Hits)
                                                    {
                                                        user.Teleport(214, 203, 1036);
                                                        var bot = BotProcessring.Bots.Values.FirstOrDefault(x => x.Bot.Player.Target.UID == user.Player.UID && x.Bot.Player.DynamicID == user.Player.DynamicID);
                                                        if (bot != null)
                                                        {
                                                            bot.Dispose();

                                                            double Rate = 0;
                                                            Rate = Math.Round((double)(user.Player.Hits * 100.0 / Math.Max(1, user.Player.TotalHits)), 2);
                                                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{user.Player.Name} defeat by {attacked.Name} with Chain: {user.Player.MaxChains} Hits: {user.Player.TotalHits} Miss: {user.Player.TotalHits - user.Player.Hits} Accuracy: {Rate.ToString()} / 100", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                                            return;
                                                        }
                                                    }
                                            }
                                            pass = true;
                                        }
                                        if (Program.MapCounterHits.Contains(user.Player.Map) || UnlimitedArenaRooms.Maps.ContainsValue(user.Player.DynamicID))
                                            user.Player.HitShoot += 1;
                                        
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        if (UnlimitedArenaRooms.Maps.ContainsValue(user.Player.DynamicID) && user.Player.Map == 700)
                                        {
                                            AnimationObj.Damage = (uint)user.Player.Hits;
                                        }
                                        if (user.Player.Map == 700 && user.Player.DynamicID != 0)
                                        {
                                            user.SendSysMesage("Accuracy Rates.", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                                            foreach (var player in user.Map.Values.Where(e => e.Player.DynamicID == user.Player.DynamicID))
                                            {

                                                user.SendSysMesage(player.Player.Name + " " + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) + "%, H: " + player.Player.Hits + ", M: " + (player.Player.TotalHits - player.Player.Hits) + ", M.C: " + player.Player.MaxChains, MsgMessage.ChatMode.ContinueRightCorner);
                                            }
                                        }
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            if (!hitSomeone)
                                user.Player.Chains = 0;
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc).Where(e => LineRe.Contains((ushort)(e.X), (ushort)(e.Y))))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= 9)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FreezingArrow:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            byte Range = (byte)(Math.Min(7, (int)DBSpell.Range));
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, Range, 0, ClientSpell.ID);

                            byte LineRange = 1;
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                            if (attacked.Boss == 9) { AnimationObj.Damage = 1; }
                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SpeedGun:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            byte Range = (byte)(Math.Min(7, (int)DBSpell.Range));
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, Range, 0, ClientSpell.ID);

                            byte LineRange = 1;
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}
