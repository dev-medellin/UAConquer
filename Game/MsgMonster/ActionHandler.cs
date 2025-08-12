using DevExpress.Utils;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using static TheChosenProject.Database.ClientSpells;
using static TheChosenProject.Role.Flags;
using static TheChosenProject.Role.Instance.Flowers;

namespace TheChosenProject.Game.MsgMonster
{
    public class ActionHandler
    {
        public MSRandom Random = new MSRandom();

        public Random SystemRandom = new Random();

        public bool Rate(int value)
        {
            return value > Random.Next() % 100;
        }

        public void ExecuteAction(TheChosenProject.Role.Player client, MonsterRole monster)
        {
            if (monster.Family.ID == 40121
                || monster.Family.ID == 40122
                || monster.Family.ID == 40123
                || monster.Family.ID == 40124
                //|| monster.Family.ID == 20070 || monster.Family.ID == 20060 || monster.Family.ID == 20160 || monster.Family.ID == 6643 || monster.Family.ID == 20300
                || monster.Family.ID == 21060)//bahaa
                return;
            if (monster.UID >= 700000)
                return;
            switch (monster.State)
            {
                case MobStatus.Idle:
                    {
                        monster.Target = null;
                        CheckTarget(client, monster);
                        break;
                    }
                case MobStatus.SearchTarget: SearchTarget(client, monster); break;
                case MobStatus.Attacking: AttackingTarget(client, monster); break;
            }
        }

        public unsafe void CheckGuardPosition(TheChosenProject.Role.Player client, MonsterRole monster)
        {
            if (monster.Alive && (monster.X != monster.RespawnX || monster.Y != monster.RespawnY) && Time32.Now > monster.MoveStamp.AddMilliseconds(300))
            {
                monster.MoveStamp = Time32.Now;
                Flags.ConquerAngle dir;
                dir = GetAngle(monster.X, monster.Y, monster.RespawnX, monster.RespawnY);
                ushort WalkX;
                WalkX = monster.X;
                ushort WalkY;
                WalkY = monster.Y;
                IncXY(dir, ref WalkX, ref WalkY);
                client.Owner.Map.View.MoveTo((IMapObj)monster, (int)WalkX, (int)WalkY);
                monster.X = WalkX;
                monster.Y = WalkY;
                WalkQuery walkQuery;
                walkQuery = default(WalkQuery);
                walkQuery.Direction = (uint)dir;
                walkQuery.Running = 1;
                walkQuery.UID = monster.UID;
                WalkQuery walk;
                walk = walkQuery;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    monster.Send(stream.MovementCreate(&walk));
                }
            }
        }
        public static uint PhysicalAttackPet(Client.GameClient client, Client.GameClient target)
        {

            if (!client.Socket.Alive)
                return 0;
            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return 1;
            if (client.Pet == null) return 0;

            if (client.ProjectManager)
                return 1;
            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);

            uint power = 0;
            if (client.Pet.monster.Family.MaxAttack > client.Pet.monster.Family.MinAttack)
                power = (uint)Program.GetRandom.Next(client.Pet.monster.Family.MinAttack, client.Pet.monster.Family.MaxAttack);
            else
                power = (uint)Program.GetRandom.Next(client.Pet.monster.Family.MaxAttack, client.Pet.monster.Family.MinAttack);

            // power += (uint)(power * Program.ServerConfig.PhysicalDamage / 100);

            power = new ActionHandler().DecreaseBless(power, (uint)target.Status.ItemBless);


            if (target.Player.Reborn == 1)
                power = (uint)Math.Round((double)(power * 0.7));
            else if (target.Player.Reborn == 2)
                power = (uint)Math.Round((double)(power * 0.5));

            if (power > target.Status.Defence)
                power -= target.Status.Defence;

            if (power > target.Status.MDefence)
                power -= target.Status.MDefence;

            if (power > 300)// 5las ya 3m mustafa msh lazm fzlaka 
            {
                power /= 3;
            }
            return power;// (uint)new Random().Next(100, 1000);
        }
        public static uint PhysicalAttackPet(Client.GameClient client, MonsterRole monster)
        {

            if (!client.Socket.Alive)
                return 0;
            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return 1;

            if (client.ProjectManager)
                return 1;
            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);

            uint power = 0;
            if (client.Pet.monster.Family.MaxAttack > client.Pet.monster.Family.MinAttack)
                power = (uint)Program.GetRandom.Next(client.Pet.monster.Family.MinAttack, client.Pet.monster.Family.MaxAttack);
            else
                power = (uint)Program.GetRandom.Next(client.Pet.monster.Family.MaxAttack, client.Pet.monster.Family.MinAttack);

            power += (uint)(power * ServerKernel.PhysicalDamage / 100);

            // power = DecreaseBless(power, (uint)client.Status.ItemBless);

            if (power > monster.Family.Defense)
                power -= monster.Family.Defense;
            else
                power = 1;


            return power;
        }
        public bool GuardAttackPlayer(TheChosenProject.Role.Player client, MonsterRole monster)
        {
            if (!monster.Alive)
            {
                return false;
            }

            if (client.ContainFlag(MsgUpdate.Flags.FlashingName) && client.Alive)
            {
                short distance;
                distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (distance < monster.Family.AttackRange && !CheckRespouseDamage(client, monster))
                {
                    uint Damage;
                    Damage = MagicAttack(client.Owner, monster);
                    Damage = 200000;
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        MsgSpellAnimation SpellPacket;
                        SpellPacket = new MsgSpellAnimation(monster.UID, 0, client.X, client.Y, 5020, 9, 0);
                        SpellPacket.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.UID, Damage, MsgAttackPacket.AttackEffect.None));
                        SpellPacket.SetStream(stream);
                        SpellPacket.Send(monster);
                    }
                    CheckForOponnentDead(client, Damage, monster);
                    return true;
                }
            }
                return false;
        }

        public bool GuardAttackMonster(GameMap map, MonsterRole attacked, MonsterRole monster)
        {
            if (monster.Family.ID == 21060)
                return false;

            if (attacked.Family.Name.Contains("Guard"))
                return false;

            if (!monster.Alive)
                return false;
            if (attacked.Alive)
            {
                if (monster.Family.ID == 21060)
                    return false;
                short distance;
                distance = MonsterView.GetDistance(attacked.X, attacked.Y, monster.X, monster.Y);
                if (distance < monster.Family.AttackRange)
                {
                    
                    uint Damage;
                    Damage = MagicAttack(attacked, monster);
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        MsgSpellAnimation SpellPacket;
                        SpellPacket = new MsgSpellAnimation(monster.UID, 0, attacked.X, attacked.Y, (ushort)monster.Family.SpellId, 9, 0);
                        SpellPacket.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, Damage, MsgAttackPacket.AttackEffect.None));
                        SpellPacket.SetStream(stream);
                        SpellPacket.Send(monster);
                        if (Damage >= attacked.HitPoints)
                        {
                            map.SetMonsterOnTile(attacked.X, attacked.Y, false);
                            attacked.Dead(stream, null, monster.UID, map);
                        }
                        else
                            attacked.HitPoints -= Damage;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool ExtraBoss(MonsterRole monster)
        {
            if (monster.Family.MaxHealth > 300000)
                return monster.Family.MaxHealth < 7000000;
            return false;
        }

        public unsafe void AttackingTarget(TheChosenProject.Role.Player client, MonsterRole monster)
        {
            if (monster.Family.ID == 21060)
                return;
            if (!monster.Alive)
                return;
            if (monster.Target.Owner.ProjectManager)
                return;
            short distance;
            if (monster.Family.SpellId > 0 && (monster.Family.ID == 3134 || monster.Family.ID == 3130) || monster.Family.AttackRange == 15)
            {
                monster.Family.AttackRange = 3;
                monster.Family.AttackSpeed = 2000;
            }
            distance = MonsterView.GetDistance(monster.Target.X, monster.Target.Y, monster.X, monster.Y);
            if (monster.Boss == 1 && monster.HitPoints >= 2000000)
                monster.Family.AttackRange = 18;
            if (distance > monster.Family.AttackRange || monster.Target == null || !monster.Target.Alive || (monster.Target.ContainFlag(MsgUpdate.Flags.Fly) && monster.Family.SpellId == 0)
                || !monster.Target.Owner.Socket.Alive)
                monster.State = MobStatus.SearchTarget;
            else
            {
                if (!(Time32.Now > monster.AttackSpeed.AddMilliseconds(monster.Family.AttackSpeed)))
                    return;
                monster.AttackSpeed = Time32.Now;
                if (ExtraBoss(monster))
                {
                    if (!CheckRespouseDamage(client, monster))
                    {
                        ushort SpellID;
                        SpellID = 9999;
                        if (monster.Level < 80)
                            SpellID = 9998;
                        else if (monster.Level < 60)
                        {
                            SpellID = 9966;
                        }
                        uint Damage10;
                        Damage10 = MagicAttack(monster.Target.Owner, monster);
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream8;
                            stream8 = recycledPacket.GetStream();
                            MsgSpellAnimation SpellPacket9;
                            SpellPacket9 = new MsgSpellAnimation(monster.UID, 0, monster.Target.X, monster.Target.Y, SpellID, 0, 0);
                            SpellPacket9.Targets.Enqueue(new MsgSpellAnimation.SpellObj(monster.Target.UID, Damage10, MsgAttackPacket.AttackEffect.None));
                            SpellPacket9.SetStream(stream8);
                            SpellPacket9.Send(monster);
                        }
                        CheckForOponnentDead(monster.Target, Damage10, monster);
                    }
                }
                else if (monster.Boss == 0 && monster.Family.SpellId == 0)
                {
                    if (!CheckRespouseDamage(client, monster))
                    {
                        uint Damage9;
                        Damage9 = PhysicalAttack(monster.Target.Owner, monster);
                        Damage9 = (CheckDodge(monster.Target.Owner.Status.Dodge) ? Damage9 : 0);
                        if (Damage9 >= 1)
                            CheckItems.RespouseDurability(monster.Target.Owner);
                        InteractQuery interactQuery;
                        interactQuery = default(InteractQuery);
                        interactQuery.AtkType = MsgAttackPacket.AttackID.Physical;
                        interactQuery.X = monster.Target.X;
                        interactQuery.Y = monster.Target.Y;
                        interactQuery.UID = monster.UID;
                        interactQuery.OpponentUID = monster.Target.UID;
                        interactQuery.Damage = (int)Damage9;
                        InteractQuery action;
                        action = interactQuery;
                        using (RecycledPacket recycledPacket2 = new RecycledPacket())
                        {
                            Packet stream7;
                            stream7 = recycledPacket2.GetStream();
                            monster.Send(stream7.InteractionCreate(&action));
                        }
                        CheckForOponnentDead(monster.Target, Damage9, monster);
                    }
                }
                else if (monster.Family.SpellId != 0 && monster.Boss == 0)
                {
                    if (!CheckRespouseDamage(client, monster))
                    {
                        uint Damage8;
                        Damage8 = MagicAttack(monster.Target.Owner, monster);
                        using (RecycledPacket recycledPacket3 = new RecycledPacket())
                        {
                            Packet stream6;
                            stream6 = recycledPacket3.GetStream();
                            MsgSpellAnimation SpellPacket8;
                            SpellPacket8 = new MsgSpellAnimation(monster.UID, 0, monster.Target.X, monster.Target.Y, (ushort)monster.Family.SpellId, 0, 0);
                            SpellPacket8.Targets.Enqueue(new MsgSpellAnimation.SpellObj(monster.Target.UID, Damage8, MsgAttackPacket.AttackEffect.None));
                            SpellPacket8.SetStream(stream6);
                            SpellPacket8.Send(monster);
                        }
                        CheckForOponnentDead(monster.Target, Damage8, monster);
                    }
                }
                else
                {
                    if (monster.Boss == 0)
                        return;
                    if (!monster.Target.Alive || Core.GetDistance(monster.X, monster.Y, monster.Target.X, monster.Target.Y) > 18)
                    {
                        monster.State = MobStatus.SearchTarget;
                        return;
                    }
                    switch (monster.Family.ID)
                    {
                        case 9111:
                            {
                                List<ushort> Spells = new List<ushort>() { 1002, 1120, 1290 };
                                ushort rand = (byte)Program.GetRandom.Next(0, Spells.Count);
                                switch (Spells[rand])
                                {
                                    case 1120:
                                    case 1002:
                                    case 1290:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();

                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                   , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 7)
                                                    {
                                                        uint Damage = MagicAttack(player.Owner, monster);
                                                        //SpellPacket.Targets.Enqueue(new MsgSpellAnimation.SpellObj(player.UID, Damage));
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        //if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                        //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            break;
                                        }

                                }
                                break;
                            }

                        case 19890:
                        case 19892:
                        case 19893:
                        case 19894:
                        case 19895:
                            {
                                List<ushort> Spells = new List<ushort>() { 1046, 1045, 1002, 8001, 5010, 5030 };
                                ushort rand = (byte)Program.GetRandom.Next(0, Spells.Count);
                                switch (Spells[rand])
                                {
                                    case 1046:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 4, 0);
                                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);


                                                CheckForOponnentDead(monster.Target, Damage, monster);
                                            }
                                            break;
                                        }
                                    case 1045:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 4, 0);
                                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);


                                                CheckForOponnentDead(monster.Target, Damage, monster);
                                            }
                                            break;
                                        }

                                    case 1002:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 3, 0);
                                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.ResistEarth));
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);


                                                CheckForOponnentDead(monster.Target, Damage, monster);
                                            }
                                            break;
                                        }
                                    case 8001:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();

                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                   , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 5, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 7)
                                                    {
                                                        uint Damage = PhysicalAttack(player.Owner, monster);
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        //if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                        //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            break;
                                        }
                                    case 5010:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 9, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 3)
                                                    {
                                                        uint Damage = PhysicalAttack(player.Owner, monster);
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        //if (Rate(10) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                        //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            break;
                                        }
                                    case 5030:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 9, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 3)
                                                    {
                                                        uint Damage = PhysicalAttack(player.Owner, monster);
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        //if (Rate(10) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                        //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        //case 3134://titan
                        //case 3130://gandorma
                        case 3102://snake
                            {
                                List<ushort> Spells = new List<ushort>() { 1046, 1045, 1002, 8001, 5010, 5030 };
                                ushort rand = (byte)Program.GetRandom.Next(0, Spells.Count);
                                switch (Spells[rand])
                                {
                                    case 1046:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 4, 0);
                                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);


                                                CheckForOponnentDead(monster.Target, Damage, monster);
                                            }
                                            break;
                                        }
                                    case 1045:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 4, 0);
                                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);


                                                CheckForOponnentDead(monster.Target, Damage, monster);
                                            }
                                            break;
                                        }

                                    case 1002:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 3, 0);
                                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.ResistEarth));
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);


                                                CheckForOponnentDead(monster.Target, Damage, monster);
                                            }
                                            break;
                                        }
                                    case 8001:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();

                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                   , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 5, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 7)
                                                    {
                                                        uint Damage = PhysicalAttack(player.Owner, monster);
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        //if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                        //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            break;
                                        }
                                    case 5010:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 9, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 3)
                                                    {
                                                        uint Damage = PhysicalAttack(player.Owner, monster);
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        //if (Rate(10) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                        //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            break;
                                        }
                                    case 5030:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 9, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 3)
                                                    {
                                                        uint Damage = PhysicalAttack(player.Owner, monster);
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        //if (Rate(10) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                        //    player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                    switch ((LookFace)monster.Family.Mesh)
                    {
                        case LookFace.CornDevil:
                        case LookFace.ThrillingSpook:
                        case LookFace.GuildBast:
                            {
                                List<ushort> Spells3;
                                Spells3 = new List<ushort> { 10360, 10361, 10362, 10363 };
                                ushort rand3;
                                rand3 = (byte)Program.GetRandom.Next(0, Spells3.Count);
                                using (RecycledPacket recycledPacket7 = new RecycledPacket())
                                {
                                    Packet stream3;
                                    stream3 = recycledPacket7.GetStream();
                                    MagicType.Magic DBSpell3;
                                    DBSpell3 = Server.Magic[Spells3[rand3]][0];
                                    MsgSpellAnimation SpellPacket4;
                                    SpellPacket4 = new MsgSpellAnimation(monster.UID, 0u, monster.Target.X, monster.Target.Y, DBSpell3.ID, 0, 0);
                                    foreach (IMapObj targent3 in monster.View.Roles(client.Owner.Map, MapObjectType.Player))
                                    {
                                        if (!targent3.Alive)
                                            continue;
                                        TheChosenProject.Role.Player player3;
                                        player3 = targent3 as TheChosenProject.Role.Player;
                                        if (Core.GetDistance(monster.Target.X, monster.Target.Y, player3.X, player3.Y) <= DBSpell3.Range)
                                        {
                                            uint Damage4;
                                            Damage4 = PhysicalAttack(player3.Owner, monster);
                                            SpellPacket4.Targets.Enqueue(new MsgSpellAnimation.SpellObj(player3.UID, Damage4, MsgAttackPacket.AttackEffect.ResistEarth));
                                            CheckForOponnentDead(player3, Damage4, monster);
                                            if (Rate(5) && !monster.Target.ContainFlag(MsgUpdate.Flags.Confused))
                                                monster.Target.AddFlag(MsgUpdate.Flags.Confused, 5, true);
                                        }
                                    }
                                    SpellPacket4.SetStream(stream3);
                                    SpellPacket4.Send(monster);
                                    break;
                                }
                            }
                        case LookFace.SnowBanshee:
                            {
                                List<ushort> Spells4;
                                Spells4 = new List<ushort> { 30010, 30011, 30012, 30013, 30014, 10372, 10373 };
                                ushort rand4;
                                rand4 = (byte)Program.GetRandom.Next(0, Spells4.Count);
                                using (RecycledPacket recycledPacket6 = new RecycledPacket())
                                {
                                    Packet stream4;
                                    stream4 = recycledPacket6.GetStream();
                                    MagicType.Magic DBSpell4;
                                    DBSpell4 = Server.Magic[Spells4[rand4]][0];
                                    ushort iD2;
                                    iD2 = DBSpell4.ID;
                                    ushort num2;
                                    num2 = iD2;
                                    if (num2 == 10372)
                                    {
                                        uint Damage5;
                                        Damage5 = 80000u;
                                        monster.HitPoints = (uint)Math.Min(monster.Family.MaxHealth, (int)(monster.HitPoints + Damage5));
                                        MsgSpellAnimation SpellPacket5;
                                        SpellPacket5 = new MsgSpellAnimation(monster.UID, 0u, monster.X, monster.Y, Spells4[rand4], 0, 0);
                                        SpellPacket5.Targets.Enqueue(new MsgSpellAnimation.SpellObj(monster.UID, Damage5, MsgAttackPacket.AttackEffect.None));
                                        SpellPacket5.SetStream(stream4);
                                        SpellPacket5.Send(monster);
                                        break;
                                    }
                                    MsgSpellAnimation SpellPacket6;
                                    SpellPacket6 = new MsgSpellAnimation(monster.UID, 0u, monster.Target.X, monster.Target.Y, DBSpell4.ID, 0, 0);
                                    foreach (IMapObj targent4 in monster.View.Roles(client.Owner.Map, MapObjectType.Player))
                                    {
                                        if (!targent4.Alive)
                                            continue;
                                        TheChosenProject.Role.Player player4;
                                        player4 = targent4 as TheChosenProject.Role.Player;
                                        if (Core.GetDistance(monster.Target.X, monster.Target.Y, player4.X, player4.Y) <= DBSpell4.Range)
                                        {
                                            uint Damage7;
                                            Damage7 = PhysicalAttack(player4.Owner, monster);
                                            SpellPacket6.Targets.Enqueue(new MsgSpellAnimation.SpellObj(player4.UID, Damage7, MsgAttackPacket.AttackEffect.ResistWater));
                                            CheckForOponnentDead(player4, Damage7, monster);
                                            if (Rate(5) && !monster.Target.ContainFlag(MsgUpdate.Flags.Freeze))
                                                monster.Target.AddFlag(MsgUpdate.Flags.Freeze, 5, true);
                                        }
                                    }
                                    SpellPacket6.SetStream(stream4);
                                    SpellPacket6.Send(monster);
                                    break;
                                }
                            }
                        case LookFace.Gibbon:
                        case LookFace.NagaLord:
                        case LookFace.Talon:
                        case LookFace.Howler:
                        case LookFace.Titan:
                        case LookFace.LavaBeast:
                            {
                                List<ushort> Spells;
                                Spells = new List<ushort> { 10000, 10001, 10003, 1045 };
                                ushort rand;
                                rand = (byte)Program.GetRandom.Next(0, Spells.Count);
                                using (RecycledPacket rec = new RecycledPacket())
                                {
                                    Packet stream;
                                    stream = rec.GetStream();
                                    MagicType.Magic DBSpell;
                                    DBSpell = Server.Magic[Spells[rand]][0];
                                    MsgSpellAnimation SpellPacket;
                                    SpellPacket = new MsgSpellAnimation(monster.UID, 0u, monster.Target.X, monster.Target.Y, DBSpell.ID, 0, 0);
                                    foreach (IMapObj targent in monster.View.Roles(client.Owner.Map, MapObjectType.Player))
                                    {
                                        if (!targent.Alive)
                                            continue;
                                        TheChosenProject.Role.Player player;
                                        player = targent as TheChosenProject.Role.Player;
                                        if (Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= DBSpell.Range)
                                        {
                                            uint Damage;
                                            Damage = PhysicalAttack(player.Owner, monster);
                                            SpellPacket.Targets.Enqueue(new MsgSpellAnimation.SpellObj(player.UID, Damage, MsgAttackPacket.AttackEffect.ResistWood));
                                            CheckForOponnentDead(player, Damage, monster);
                                            if (Rate(5) && !monster.Target.ContainFlag(MsgUpdate.Flags.Confused))
                                                monster.Target.AddFlag(MsgUpdate.Flags.Confused, 5, true);
                                        }
                                    }
                                    SpellPacket.SetStream(stream);
                                    SpellPacket.Send(monster);
                                    break;
                                }
                            }
                        case LookFace.DarkmoonDemon:
                        case LookFace.TeratoDragon:
                            {
                                List<ushort> Spells = new List<ushort>() { 7013, 7014, 7015,7016, 7017 };
                                ushort rand = (byte)Program.GetRandom.Next(0, Spells.Count);
                                switch (Spells[rand])
                                {
                                    case 7017:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();

                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                   , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 8)
                                                    {
                                                        uint Damage = PhysicalAttack(player.Owner, monster);
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                            player.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            break;
                                        }
                                    case 7014:
                                    case 7013:
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                {
                                                    if (!targent.Alive)
                                                        continue;
                                                    var player = targent as Role.Player;
                                                    if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 6)
                                                    {
                                                        uint Damage = PhysicalAttack(player.Owner, monster);
                                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                            , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                        CheckForOponnentDead(player, Damage, monster);

                                                        if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                            player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                    }
                                                }
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);

                                            }
                                            break;
                                        }
                                    case 7015:
                                        {
                                            uint Damage = PhysicalAttack(monster.Target.Owner, monster);

                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                 , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);
                                            }
                                            CheckForOponnentDead(monster.Target, Damage, monster);

                                            if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                            break;
                                        }
                                    case 7016:
                                        {
                                            uint Damage = 80000;
                                            monster.HitPoints = (uint)Math.Min(monster.Family.MaxHealth, (int)(monster.HitPoints + Damage));
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                , 0, monster.X, monster.Y, (ushort)Spells[rand], 0, 0);
                                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                SpellPacket.SetStream(stream);
                                                SpellPacket.Send(monster);


                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case LookFace.Minotaurs:
                        case LookFace.SwordMaster:
                            {
                                List<ushort> Spells5;
                                Spells5 = new List<ushort> { 10500, 10502, 10503, 10504, 10506, 10506 };
                                ushort rand5;
                                rand5 = (byte)Program.GetRandom.Next(0, Spells5.Count);
                                using (RecycledPacket recycledPacket4 = new RecycledPacket())
                                {
                                    Packet stream6;
                                    stream6 = recycledPacket4.GetStream();
                                    MagicType.Magic DBSpell5;
                                    DBSpell5 = Server.Magic[Spells5[rand5]][0];
                                    MsgSpellAnimation SpellPacket8;
                                    SpellPacket8 = new MsgSpellAnimation(monster.UID, 0u, monster.Target.X, monster.Target.Y, DBSpell5.ID, 0    , 0);
                                    foreach (IMapObj targent5 in monster.View.Roles(client.Owner.Map, MapObjectType.Player))
                                    {
                                        if (!targent5.Alive)
                                            continue;
                                        TheChosenProject.Role.Player player5;
                                        player5 = targent5 as TheChosenProject.Role.Player;
                                        if (Core.GetDistance(monster.Target.X, monster.Target.Y, player5.X, player5.Y) <= DBSpell5.Range)
                                        {
                                            uint Damage8;
                                            Damage8 = PhysicalAttack(player5.Owner, monster);
                                            SpellPacket8.Targets.Enqueue(new MsgSpellAnimation.SpellObj(player5.UID, Damage8, MsgAttackPacket.AttackEffect.ResistMetal));
                                            CheckForOponnentDead(player5, Damage8, monster);
                                            if (Rate(5) && !monster.Target.ContainFlag(MsgUpdate.Flags.Dizzy))
                                                monster.Target.AddFlag(MsgUpdate.Flags.Dizzy, 5, true);
                                        }
                                    }
                                    SpellPacket8.SetStream(stream6);
                                    SpellPacket8.Send(monster);
                                    break;
                                }
                            }
                        case LookFace.Gano:
                            {
                                Dictionary<ushort, byte> skillMaxLevels = new Dictionary<ushort, byte>
                                {
                                    { 1115, 4 },  // Example: skill ID 1115 has max level 5
                                    { 1150, 7 },
                                    { 1165, 3 },
                                };
                                ushort rand5;
                                rand5 = (byte)Program.GetRandom.Next(0, skillMaxLevels.Count);
                                using (RecycledPacket recycledPacket4 = new RecycledPacket())
                                {
                                    Packet stream6;
                                    stream6 = recycledPacket4.GetStream();
                                    MagicType.Magic DBSpell5;
                                    ushort skillId;
                                    byte skillLevel = 1; // default fallback

                                    // Pick a random skill ID from the dictionary
                                    var keys = skillMaxLevels.Keys.ToList();
                                    ushort randKey = keys[Program.GetRandom.Next(0, keys.Count)];
                                    skillId = randKey;

                                    // Fetch the magic definition
                                    DBSpell5 = Server.Magic[skillId][0];

                                    // Determine max level
                                    if (skillMaxLevels.TryGetValue(skillId, out byte maxLevel))
                                    {
                                        skillLevel = maxLevel; // Or calculate based on your logic
                                    }
                                    MsgSpellAnimation SpellPacket8;
                                    SpellPacket8 = new MsgSpellAnimation(monster.UID, 0u, monster.Target.X, monster.Target.Y, DBSpell5.ID, 4, 0);
                                    foreach (IMapObj targent5 in monster.View.Roles(client.Owner.Map, MapObjectType.Player))
                                    {
                                        if (!targent5.Alive)
                                            continue;
                                        TheChosenProject.Role.Player player5;
                                        player5 = targent5 as TheChosenProject.Role.Player;
                                        if (Core.GetDistance(monster.Target.X, monster.Target.Y, player5.X, player5.Y) <= DBSpell5.Range)
                                        {
                                            uint Damage8;
                                            Damage8 = PhysicalAttack(player5.Owner, monster);
                                            SpellPacket8.Targets.Enqueue(new MsgSpellAnimation.SpellObj(player5.UID, Damage8, MsgAttackPacket.AttackEffect.ResistMetal));
                                            CheckForOponnentDead(player5, Damage8, monster);
                                            if (Rate(5) && !monster.Target.ContainFlag(MsgUpdate.Flags.Dizzy))
                                                monster.Target.AddFlag(MsgUpdate.Flags.Dizzy, 5, true);
                                        }
                                    }
                                    SpellPacket8.SetStream(stream6);
                                    SpellPacket8.Send(monster);
                                    break;
                                }
                            }
                    }
                }
            }
        }

        public unsafe void SearchTarget(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (client == null) return;

            if (monster.Boss == 9) return;

            if (monster == null)
                return;
            if (!monster.Alive)
                return;
            try
            {
                if (monster.Target != null)
                {
                    if (!monster.Target.Alive || monster.Target.Owner == null || (monster.Target.ContainFlag(MsgUpdate.Flags.Fly) && monster.Family.SpellId == 0) || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Invisibility)
                        || !monster.Target.Owner.Socket.Alive)
                    {
                        monster.State = MobStatus.Idle;
                        return;
                    }
                }
                if (monster.Target == null)
                {
                    monster.State = MobStatus.Idle;
                    return;
                }
                if (monster.Family == null)
                    return;
                if (monster.Family.SpellId > 0)
                    monster.Family.AttackRange = 15;
                short distance = MonsterView.GetDistance(monster.Target.X, monster.Target.Y, monster.X, monster.Y);
                if (distance <= monster.Family.AttackRange || (monster.Boss == 1 && monster.HitPoints > 2000000 && distance <= 18))
                {
                    monster.State = MobStatus.Attacking;
                }
                else
                {
                    monster.State = MobStatus.Idle;
                }
                if (distance <= monster.Family.ViewRange && monster.Target != null && monster.Target.Alive)
                {
                    try
                    {
                        if (Extensions.Time32.Now > monster.MoveStamp.AddMilliseconds(monster.Family.MoveSpeed))
                        {
                            if (monster == null)
                                return;
                            if (!monster.Alive)
                                return;
                            monster.MoveStamp = Extensions.Time32.Now;
                            bool Walk = Random.Next() % 100 < 70;
                            if (Walk)
                            {
                                if (monster.Target != null)
                                {
                                    Role.Flags.ConquerAngle dir = GetAngle(monster.X, monster.Y, monster.Target.X, monster.Target.Y);
                                    ushort WalkX = monster.X; ushort WalkY = monster.Y;
                                    IncXY(dir, ref WalkX, ref WalkY);
                                    if (monster.Target.Owner == null)
                                    {
                                        monster.State = MobStatus.Idle;
                                        return;
                                    }
                                    if (monster.Target.Owner.Map == null)
                                    {
                                        monster.State = MobStatus.Idle;
                                        return;
                                    }
                                    var Map = monster.Target.Owner.Map;
                                    if (Map.ValidLocation(WalkX, WalkY) && !Map.MonsterOnTile(WalkX, WalkY))
                                    {
                                        try
                                        {
                                            Map.SetMonsterOnTile(monster.X, monster.Y, false);
                                            Map.SetMonsterOnTile(WalkX, WalkY, true);
                                            Map.View.MoveTo<Role.IMapObj>(monster, WalkX, WalkY);
                                            monster.X = WalkX; monster.Y = WalkY;
                                            WalkQuery action = new WalkQuery()
                                            {
                                                Direction = (byte)dir,
                                                Running = 1,
                                                UID = monster.UID
                                            };
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                monster.Send(stream.MovementCreate(&action));
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Report1 : " + e.Message);
                                            monster.State = MobStatus.Idle;
                                            Console.WriteLine("Null1 >> Monster1 : " + monster.Family.Name + " Target : " + monster.Target.Name);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            dir = (Role.Flags.ConquerAngle)(Random.Next() % 8);
                                            WalkX = monster.X; WalkY = monster.Y;
                                            IncXY(dir, ref WalkX, ref WalkY);
                                            if (Map.ValidLocation(WalkX, WalkY) && !Map.MonsterOnTile(WalkX, WalkY))
                                            {
                                                Map.SetMonsterOnTile(monster.X, monster.Y, false);
                                                Map.SetMonsterOnTile(WalkX, WalkY, true);
                                                Map.View.MoveTo<Role.IMapObj>(monster, WalkX, WalkY);
                                                monster.X = WalkX; monster.Y = WalkY;
                                                WalkQuery action = new WalkQuery()
                                                {
                                                    Direction = (byte)dir,
                                                    Running = 1,
                                                    UID = monster.UID
                                                };
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    monster.Send(stream.MovementCreate(&action));
                                                    monster.UpdateMonsterView(monster.Target.View, stream);
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Report2 : " + e.Message);
                                            monster.State = MobStatus.Idle;
                                            Console.WriteLine("Null2 >> Monster2 : " + monster.Family.Name + " Target : " + monster.Target.Name);
                                        }
                                    }
                                }
                                else
                                {
                                    monster.State = MobStatus.Idle;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Report3 : " + e.Message);
                        monster.State = MobStatus.Idle;
                        Console.WriteLine("Null3 >> Monster3 : " + monster.Family.Name + " Target : " + monster.Target.Name);
                    }
                }
                else
                    monster.State = MobStatus.Idle;
            }
            catch (Exception e)
            {
                Console.WriteLine("Report4 : " + e.Message);
                monster.State = MobStatus.Idle;
                Console.WriteLine("Null3 >> Monster3 : " + monster.Family.Name + " Target : " + monster.Target.Name);
            }
        }

        //public unsafe void SearchTarget(MonsterRole monster)
        //{
        //    if (monster == null || !monster.Alive)
        //        return;
        //    try
        //    {
        //        if (monster.Target != null && (!monster.Target.Alive || (monster.Target.ContainFlag(MsgUpdate.Flags.Fly) && monster.Boss == 1) || (monster.Boss != 1 && monster.Target.ContainFlag(MsgUpdate.Flags.Invisibility))))
        //            monster.State = MobStatus.Idle;
        //        else if (monster.Target == null)
        //        {
        //            monster.State = MobStatus.Idle;
        //        }
        //        else
        //        {
        //            if (monster.Family == null)
        //                return;
        //            short distance;
        //            distance = MonsterView.GetDistance(monster.Target.X, monster.Target.Y, monster.X, monster.Y);
        //            if (distance <= monster.Family.AttackRange || (monster.Boss == 1 && monster.HitPoints > 2000000 && distance <= 18))
        //                monster.State = MobStatus.Attacking;
        //            else
        //                monster.State = MobStatus.Idle;
        //            if (distance <= monster.Family.ViewRange && monster.Target != null && monster.Target.Alive)
        //                try
        //                {
        //                    if ((monster.Family.Settings & MonsterSettings.HasPlayerOwner) != MonsterSettings.HasPlayerOwner && Time32.Now > monster.MoveStamp.AddMilliseconds(monster.Family.MoveSpeed))
        //                    {
        //                        monster.MoveStamp = Time32.Now;
        //                        if (Random.Next() % 100 < 70)
        //                        {
        //                            Flags.ConquerAngle dir;
        //                            dir = GetAngle(monster.X, monster.Y, monster.Target.X, monster.Target.Y);
        //                            ushort WalkX;
        //                            WalkX = monster.X;
        //                            ushort WalkY;
        //                            WalkY = monster.Y;
        //                            IncXY(dir, ref WalkX, ref WalkY);
        //                            GameMap Map;
        //                            Map = monster.Target.Owner.Map;
        //                            if (Map.ValidLocation(WalkX, WalkY) && !Map.MonsterOnTile(WalkX, WalkY))
        //                            {
        //                                Map.SetMonsterOnTile(monster.X, monster.Y, false);
        //                                Map.SetMonsterOnTile(WalkX, WalkY, true);
        //                                Map.View.MoveTo((IMapObj)monster, (int)WalkX, (int)WalkY);
        //                                monster.X = WalkX;
        //                                monster.Y = WalkY;
        //                                WalkQuery walkQuery;
        //                                walkQuery = default(WalkQuery);
        //                                walkQuery.Direction = (uint)dir;
        //                                walkQuery.Running = 1;
        //                                walkQuery.UID = monster.UID;
        //                                WalkQuery action;
        //                                action = walkQuery;
        //                                using (RecycledPacket recycledPacket = new RecycledPacket())
        //                                {
        //                                    Packet stream2;
        //                                    stream2 = recycledPacket.GetStream();
        //                                    monster.Send(stream2.MovementCreate(&action));
        //                                    return;
        //                                }
        //                            }
        //                            dir = (Flags.ConquerAngle)(Random.Next() % 8);
        //                            WalkX = monster.X;
        //                            WalkY = monster.Y;
        //                            IncXY(dir, ref WalkX, ref WalkY);
        //                            if (Map.ValidLocation(WalkX, WalkY) && !Map.MonsterOnTile(WalkX, WalkY))
        //                            {
        //                                Map.SetMonsterOnTile(monster.X, monster.Y, false);
        //                                Map.SetMonsterOnTile(WalkX, WalkY, true);
        //                                Map.View.MoveTo((IMapObj)monster, (int)WalkX, (int)WalkY);
        //                                monster.X = WalkX;
        //                                monster.Y = WalkY;
        //                                WalkQuery walkQuery;
        //                                walkQuery = default(WalkQuery);
        //                                walkQuery.Direction = (uint)dir;
        //                                walkQuery.Running = 1;
        //                                walkQuery.UID = monster.UID;
        //                                WalkQuery action2;
        //                                action2 = walkQuery;
        //                                using (RecycledPacket rec = new RecycledPacket())
        //                                {
        //                                    Packet stream;
        //                                    stream = rec.GetStream();
        //                                    monster.Send(stream.MovementCreate(&action2));
        //                                    monster.UpdateMonsterView(stream);
        //                                    return;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    return;
        //                }
        //                catch (Exception e2)
        //                {
        //                    ServerKernel.Log.SaveLog(e2.ToString(), false, LogType.EXCEPTION);
        //                    return;
        //                }
        //            monster.State = MobStatus.Idle;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
        //    }
        //}

        public void CheckTarget(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (!monster.Alive || !client.Alive)
                return;
            if (monster.Target == null && !client.ContainFlag(MsgServer.MsgUpdate.Flags.Fly) && client.Alive)
            {
                if (monster.Family.SpellId > 0)
                    monster.Family.ViewRange = 15;
                short distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (distance <= monster.Family.ViewRange && client.Alive)
                {
                    var targ = monster.View.GetTarget(client.Owner.Map, Role.MapObjectType.Player);
                    if (targ != null)
                    {
                        monster.Target = targ as Role.Player;
                        if (!monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                        {
                            monster.State = MobStatus.SearchTarget;
                        }
                        else
                        {
                            foreach (var OtherTarget in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                            {
                                var obj = OtherTarget as Role.Player;
                                if (!obj.ContainFlag(MsgServer.MsgUpdate.Flags.Fly) || obj.Alive)
                                {
                                    monster.Target = obj;
                                    monster.State = MobStatus.SearchTarget;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                short distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (monster.Target == null)
                {
                    var targ = monster.View.GetTarget(client.Owner.Map, Role.MapObjectType.Player);
                    if (targ != null)
                    {
                        monster.Target = targ as Role.Player;
                        if (!monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly) || monster.Boss == 1)
                        {
                            monster.State = MobStatus.SearchTarget;
                        }
                        else
                        {
                            foreach (var OtherTarget in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                            {
                                var obj = OtherTarget as Role.Player;
                                if (!obj.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                                {
                                    monster.Target = obj;
                                    monster.State = MobStatus.SearchTarget;
                                }
                            }
                        }
                    }
                }
                if (monster.Target != null && monster.Boss == 0 && (distance > monster.Family.ViewRange || (monster.Target.ContainFlag(MsgUpdate.Flags.Fly) && monster.Family.SpellId == 0) || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Invisibility)
                    || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Invisible) || monster.Target.Owner.Socket == null || !monster.Target.Owner.Socket.Alive))
                {
                    monster.Target = null;
                }
                else if (monster.Target != null && monster.Boss == 1 && (distance > monster.Family.ViewRange || (monster.Target.ContainFlag(MsgUpdate.Flags.Fly) && monster.Family.SpellId == 0) || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Invisibility)
                    || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Invisible) || monster.Target.Owner.Socket == null || !monster.Target.Owner.Socket.Alive))
                {
                    monster.Target = null;
                }
                else if (monster.Target == null || monster.Target.Alive)
                    monster.State = MobStatus.SearchTarget;
            }
        }

        #region oldCheckTarget
        //public void CheckTarget(TheChosenProject.Role.Player client, MonsterRole monster)
        //{
        //    if (!monster.Alive)
        //        return;
        //    if (monster.Target == null && !client.ContainFlag(MsgUpdate.Flags.Fly))
        //    {
        //        short distance2;
        //        distance2 = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
        //        if (distance2 > monster.Family.ViewRange || !client.Alive)
        //            return;
        //        IMapObj targ2;
        //        targ2 = monster.View.GetTarget(client.Owner.Map, MapObjectType.Player);
        //        if (targ2 == null)
        //            return;
        //        monster.Target = targ2 as TheChosenProject.Role.Player;
        //        if (!monster.Target.ContainFlag(MsgUpdate.Flags.Fly) || !monster.Target.Alive)
        //        {
        //            monster.State = MobStatus.SearchTarget;
        //            return;
        //        }
        //        {
        //            foreach (IMapObj OtherTarget2 in monster.View.Roles(client.Owner.Map, MapObjectType.Player))
        //            {
        //                TheChosenProject.Role.Player obj2;
        //                obj2 = OtherTarget2 as TheChosenProject.Role.Player;
        //                if (!obj2.ContainFlag(MsgUpdate.Flags.Fly) || obj2.Alive)
        //                {
        //                    monster.Target = obj2;
        //                    monster.State = MobStatus.SearchTarget;
        //                }
        //            }
        //            return;
        //        }
        //    }
        //    short distance;
        //    distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
        //    if (monster.Target == null)
        //    {
        //        IMapObj targ;
        //        targ = monster.View.GetTarget(client.Owner.Map, MapObjectType.Player);
        //        if (targ != null)
        //        {
        //            monster.Target = targ as TheChosenProject.Role.Player;
        //            if (!monster.Target.ContainFlag(MsgUpdate.Flags.Fly) || monster.Boss == 1)
        //                monster.State = MobStatus.SearchTarget;
        //            else
        //            {
        //                foreach (IMapObj OtherTarget in monster.View.Roles(client.Owner.Map, MapObjectType.Player))
        //                {
        //                    TheChosenProject.Role.Player obj;
        //                    obj = OtherTarget as TheChosenProject.Role.Player;
        //                    if (!obj.ContainFlag(MsgUpdate.Flags.Fly))
        //                    {
        //                        monster.Target = obj;
        //                        monster.State = MobStatus.SearchTarget;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (monster.Target != null && (distance > monster.Family.ViewRange || monster.Target.ContainFlag(MsgUpdate.Flags.Fly) && monster.Boss == 1 || !monster.Target.Alive ) || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Invisibility))
        //        monster.Target = null;
        //    else if (monster.Target == null || monster.Target.Alive)
        //    {
        //        monster.State = MobStatus.SearchTarget;
        //    }
        //}
        #endregion

        public bool CheckDodge(uint Dodge)
        {
            bool allow;
            allow = true;
            uint Noumber;
            Noumber = (uint)Random.Next() % 150;
            if (Noumber > 60)
                Noumber = (uint)Random.Next() % 150;
            if (Noumber < Dodge)
                allow = false;
            return allow;
        }

        public void CheckForOponnentDead(TheChosenProject.Role.Player Player, uint Damage, MonsterRole monster)
        {
            if (!Player.Alive)
                return;
            if (Player.DelayedTask)
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream2;
                    stream2 = recycledPacket.GetStream();
                    Player.RemovePick(stream2);
                }
            }
            if (!Player.Owner.Fake && !Player.Owner.Socket.Alive)
                return;
            if (Damage >= Player.HitPoints)
            {
                ushort X;
                X = Player.X;
                ushort Y;
                Y = Player.Y;
                Player.Dead(null, X, Y, monster.UID);
                //Player.Owner.SendSysMesage("" + Player.Name.ToString() + " was killed by " + monster.Name + "!", MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red, true);

                return;
            }
            if (Player.Action == Flags.ConquerAction.Sit)
            {
                if (Player.Stamina >= 20)
                    Player.Stamina -= 20;
                else
                    Player.Stamina = 0;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Player.SendUpdate(stream, Player.Stamina, MsgUpdate.DataType.Stamina);
                }
                Player.Action = Flags.ConquerAction.None;
            }
            Player.HitPoints -= (int)Damage;
        }

        public unsafe bool CheckRespouseDamage(TheChosenProject.Role.Player player, MonsterRole Monster)
        {
            if (Rate(15))
            {
                if (player.ContainReflect)
                {
                    MsgSpellAnimation.SpellObj DmgObj2;
                    DmgObj2 = new MsgSpellAnimation.SpellObj
                    {
                        Damage = PhysicalAttack(player.Owner, Monster)
                    };
                    DmgObj2.Damage /= 10;
                    if (DmgObj2.Damage == 0)
                        DmgObj2.Damage = 1;
                    InteractQuery interactQuery;
                    interactQuery = default(InteractQuery);
                    interactQuery.Damage = (int)DmgObj2.Damage;
                    interactQuery.ResponseDamage = DmgObj2.Damage;
                    interactQuery.AtkType = MsgAttackPacket.AttackID.Reflect;
                    interactQuery.X = player.X;
                    interactQuery.Y = player.Y;
                    interactQuery.OpponentUID = Monster.UID;
                    interactQuery.UID = player.UID;
                    interactQuery.Effect = DmgObj2.Effect;
                    InteractQuery action;
                    action = interactQuery;
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        Monster.Send(stream2.InteractionCreate(&action));
                        TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream2, DmgObj2, player.Owner, Monster);
                    }
                    return true;
                }
                if (player.ActivateCounterKill)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        if (player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                            return false;
                        if (player.Owner.MySpells.ClientSpells.TryGetValue(6003, out var ClientSpell) && Server.Magic.TryGetValue(6003, out var DBSpells) && DBSpells.TryGetValue(ClientSpell.Level, out var DBSpell))
                        {
                            new MsgSpellAnimation.SpellObj();
                            Physical.OnMonster(player, Monster, DBSpell, out var DmgObj, 0);
                            if (ClientSpell.Level < DBSpells.Count - 1)
                            {
                                ClientSpell.Experience += (int)(DmgObj.Damage / ServerKernel.SPELL_RATE);
                                if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                {
                                    ClientSpell.Level++;
                                    ClientSpell.Experience = 0;
                                }
                                player.Send(stream.SpellCreate(ClientSpell));
                            }
                            InteractQuery interactQuery;
                            interactQuery = default(InteractQuery);
                            interactQuery.ResponseDamage = DmgObj.Damage;
                            interactQuery.AtkType = MsgAttackPacket.AttackID.Scapegoat;
                            interactQuery.X = player.X;
                            interactQuery.Y = player.Y;
                            interactQuery.OpponentUID = Monster.UID;
                            interactQuery.UID = player.UID;
                            interactQuery.Effect = DmgObj.Effect;
                            InteractQuery action2;
                            action2 = interactQuery;
                            Monster.Send(stream.InteractionCreate(&action2));
                            TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, DmgObj, player.Owner, Monster, true);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public uint MagicAttack(MonsterRole attacked, MonsterRole monster)
        {
            uint power = (uint)monster.Family.MaxAttack;
            if (power > attacked.Family.Defense)
                power -= attacked.Family.Defense;
            else power = 1;
            return power;
        }
        //public uint MagicAttack(Client.GameClient client, MonsterRole monster)
        //{

        //    Double Reborn = 1.00;
        //    if (client.Player.Reborn == 1)
        //        Reborn -= 0.30; //30%
        //    else if (client.Player.Reborn >= 2)
        //        Reborn -= 0.50; //50%

        //    if (!client.Socket.Alive)
        //        return 0;
        //    if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
        //        return 1;
        //    MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);

        //    //uint power = (uint)monster.Family.MaxAttack;
        //    uint power = (uint)((monster.Family.MaxAttack <= monster.Family.MinAttack) ? SystemRandom.Next(monster.Family.MaxAttack, monster.Family.MinAttack) : SystemRandom.Next(monster.Family.MinAttack, monster.Family.MaxAttack));
        //    power = DecreaseBless(power, client.Status.ItemBless);
        //    power = power * (uint)Reborn;
        //    if (power > client.Status.MDefence)
        //        power -= ((uint)(power - client.Status.MagicDefence) / 100);
        //    else power = 1;
        //    if (power > client.Status.MagicDefence)
        //        power -= client.Status.MagicDefence;
        //    else power = 1;
        //    if (power > client.Status.MagicDamageDecrease)
        //        power -= client.Status.MagicDamageDecrease;
        //    else power = 1;
        //    return power;
        //}

        //public uint MagicAttack(MonsterRole attacked, MonsterRole monster)
        //{
        //    uint power;
        //    power = (uint)monster.Family.MaxAttack;
        //    if (power > attacked.Family.Defense)
        //        return power - attacked.Family.Defense;
        //    return 1;
        //}
        public uint MagicAttack(Client.GameClient client, MonsterRole monster)
        {
            if (!client.Socket.Alive)
                return 0;
            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return 1;
            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);

            uint power = (uint)monster.Family.MaxAttack;
            power = DecreaseBless(power, client.Status.ItemBless);
            if (power > client.Status.MDefence)
            {
                double reduction = client.Status.MDefence / 100.0;
                power -= (uint)(power * reduction);
            }
            else power = 1;
            if (power > client.Status.MagicDefence)
                power -= client.Status.MagicDefence;
            else power = 1;
            if (power > client.Status.MagicDamageDecrease)
                power -= client.Status.MagicDamageDecrease;
            else power = 1;
            return power;
        }
        //public uint MagicAttack(GameClient client, MonsterRole monster)
        //{

        //    Double Reborn = 1.00;
        //    if (client.Player.Reborn == 1)
        //        Reborn -= 0.30; //30%
        //    else if (client.Player.Reborn >= 2)
        //        Reborn -= 0.50; //50%

        //    if (!client.Socket.Alive)
        //        return 0;
        //    if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
        //        return 1;

        //    //if (monster.Boss == 0 && !monster.Name.Contains("Guard") && client.AIType == AIEnum.AIType.Leveling);
        //    //{
        //    //   return 1;
        //    //}
        //    CheckGemEffects.CheckRespouseDamage(client);
        //    double power;
        //    if (monster.Name == "Guard2")
        //        power = 200;
        //    else if (monster.Name == "AncientDevil")
        //    {
        //        power = (uint)Program.GetRandom.Next(monster.Family.MinAttack, monster.Family.MaxAttack);
        //    }

        //    //if(client.Status.MDefence >= 94 && !monster.Family.Name.Contains("Guard"))
        //    //{
        //    //    //power = (uint)monster.Family.MaxAttack;
        //    //    power = (uint)((monster.Family.MaxAttack <= monster.Family.MinAttack) ? SystemRandom.Next(monster.Family.MaxAttack, monster.Family.MinAttack) : SystemRandom.Next(monster.Family.MinAttack, monster.Family.MaxAttack));
        //    //    power = DecreaseBless((uint)power, client.Status.ItemBless);
        //    //    power *= (uint)Reborn;
        //    //    uint defMagic = (client.Status.MagicDefence + client.Status.MDefence);
        //    //    power = ((power <= (defMagic)) ? 1u : (power - (client.Status.MagicDefence + client.Status.MDefence)));
        //    //    power *= 0.75;
        //    //    if (power > client.Status.MagicDamageDecrease)
        //    //        return (uint)power - client.Status.MagicDamageDecrease;
        //    //    return 1;
        //    //}
        //    //else
        //    //{
        //        power = (uint)((monster.Family.MaxAttack <= monster.Family.MinAttack) ? SystemRandom.Next(monster.Family.MaxAttack, monster.Family.MinAttack) : SystemRandom.Next(monster.Family.MinAttack, monster.Family.MaxAttack));
        //        power = DecreaseBless((uint)power, client.Status.ItemBless);
        //        uint defMagic = (client.Status.MagicDefence + client.Status.MDefence);
        //        uint MagicDefemce = client.Status.MagicDefence;
        //        uint MagicPercent = client.Status.MDefence;
        //        MagicDefemce += MagicDefemce * MagicPercent / 100;
        //        power = ((power <= (MagicDefemce)) ? 1u : (power - (client.Status.MagicDefence + client.Status.MDefence)));
        //        if (power > client.Status.MagicDamageDecrease)
        //            return (uint)power - client.Status.MagicDamageDecrease;
        //        return 1;
        //    //}
        //}

        public uint PhysicalAttack(GameClient client, MonsterRole monster)
        {
            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return 1;
            //if (monster.Boss == 0 && !monster.Name.Contains("Guard") && (/*client.Player.NewbieProtection == Flags.NewbieExperience.Enable ||*/ client.AIType == AIEnum.AIType.Leveling))
            //    return 1;
            
            CheckGemEffects.CheckRespouseDamage(client);
            uint power;
            power = (uint)((monster.Family.MaxAttack <= monster.Family.MinAttack) ? SystemRandom.Next(monster.Family.MaxAttack, monster.Family.MinAttack) : SystemRandom.Next(monster.Family.MinAttack, monster.Family.MaxAttack));
            //power += power * ServerKernel.PhysicalDamage / 100;
            power = DecreaseBless(power, client.Status.ItemBless);
            power = ((power <= client.AjustDefense) ? 1u : (power - client.AjustDefense));
            if (client.Player.ContainFlag(MsgUpdate.Flags.MagicShield) || client.Player.ContainFlag(MsgUpdate.Flags.Shield))
                power = ((power <= client.AjustDefense) ? 1u : (power - client.AjustDefense + (uint)(Base.MulDiv((int)client.AjustDefense, 120, 100) - (int)client.AjustDefense)));
            if (power > client.AjustPhysicalDamageDecrease())
                return power - client.AjustPhysicalDamageDecrease();
            return 1;
        }

        public uint DecreaseBless(uint Damage, uint bless)
        {
            uint power;
            power = Damage;
            power = power * bless / 100;
            return Damage - power;
        }

        public Flags.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double AddX;
            AddX = X2 - X;
            double AddY;
            AddY = Y2 - Y;
            double r;
            r = Math.Atan2(AddY, AddX);
            if (r < 0.0)
                r += Math.PI * 2.0;
            double direction;
            direction = 360.0 - r * 180.0 / Math.PI;
            byte Dir;
            Dir = (byte)(7.0 - Math.Floor(direction) / 45.0 % 8.0 - 1.0);
            return (Flags.ConquerAngle)((int)Dir % 8);
        }

        public static void IncXY(Flags.ConquerAngle Facing, ref ushort x, ref ushort y)
        {
            sbyte xi;
            sbyte yi;
            xi = (yi = 0);
            switch (Facing)
            {
                case Flags.ConquerAngle.North:
                    xi = -1;
                    yi = -1;
                    break;
                case Flags.ConquerAngle.South:
                    xi = 1;
                    yi = 1;
                    break;
                case Flags.ConquerAngle.East:
                    xi = 1;
                    yi = -1;
                    break;
                case Flags.ConquerAngle.West:
                    xi = -1;
                    yi = 1;
                    break;
                case Flags.ConquerAngle.NorthWest:
                    xi = -1;
                    break;
                case Flags.ConquerAngle.SouthWest:
                    yi = 1;
                    break;
                case Flags.ConquerAngle.NorthEast:
                    yi = -1;
                    break;
                case Flags.ConquerAngle.SouthEast:
                    xi = 1;
                    break;
            }
            x = (ushort)(x + xi);
            y = (ushort)(y + yi);
        }
    }
}
