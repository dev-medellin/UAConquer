using TheChosenProject.Game.MsgMonster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Role.Flags;

namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public class SummonGuard
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream,
            Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            if (user.Pet != null)
            {
                user.Pet.DeAtach(stream);
            }
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            string Name = "";
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    // later i will add them
                    case 4010:
                        {
                      
                            user.SendSysMesage("You can't summon this guards");
                            break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            var skill = Database.Server.Magic[ClientSpell.ID];
                            switch (ClientSpell.Level)
                            {
                                case 0: Name = "GhostBat"; break;
                                case 1: Name = "FastBat"; break;
                                case 2: Name = "SwiftBat"; break;
                                case 3: Name = "MagicBat"; break;
                            }
                            new MonsterPet(user.Player, Name, stream);
                            user.Pet.Attach(stream);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case 4020:
                        {
                            //if (user.Player.Map == 1002 || user.Player.Map == 1005 || user.Player.Map == 1036 || user.Player.Map == 1038 || user.Player.Map == 1121)
                            //{
                            //    user.SendSysMesage("You can't summon guards");
                            //    break;
                            //}
                            user.SendSysMesage("You can't summon this guards");
                            break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            var skill = Database.Server.Magic[ClientSpell.ID];
                            switch (ClientSpell.Level)
                            {
                                case 0: Name = "GhostBatBoss"; break;
                                case 1: Name = "FastBatBoss"; break;
                                case 2: Name = "SwiftBatBoss"; break;
                                case 3: Name = "MagicBatBoss"; break;
                            }
                            new MonsterPet(user.Player, Name, stream);
                            user.Pet.Attach(stream);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case 4050:
                        {
                            //if (user.Player.Map == 1002 || user.Player.Map == 1005 || user.Player.Map == 1036 || user.Player.Map == 1038 || user.Player.Map == 1121)
                            //{
                            //    user.SendSysMesage("You can't summon guards");
                            //    break;
                            //}
                            user.SendSysMesage("You can't summon this guards");
                            break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            var skill = Database.Server.Magic[ClientSpell.ID];
                            switch (ClientSpell.Level)
                            {
                                case 0: Name = "FireRatA"; break;
                                case 1: Name = "FireRatB"; break;
                                case 2: Name = "FireRatC"; break;
                                case 3: Name = "FireRatD"; break;
                            }

                            new MonsterPet(user.Player, Name, stream);
                            user.Pet.Attach(stream);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case 4060:
                        {
                            //if (user.Player.Map == 1002 || user.Player.Map == 1005 || user.Player.Map == 1036 || user.Player.Map == 1038 || user.Player.Map == 1121)
                            //{
                            //    user.SendSysMesage("You can't summon guards");
                            //    break;
                            //}
                            user.SendSysMesage("You can't summon this guards");
                            break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            var skill = Database.Server.Magic[ClientSpell.ID];
                            switch (ClientSpell.Level)
                            {
                                case 0: Name = "EvilBatA"; break;
                                case 1: Name = "EvilBatB"; break;
                                case 2: Name = "EvilBatC"; break;
                                case 3: Name = "EvilBatD"; break;
                            }
                            new MonsterPet(user.Player, Name, stream);
                            user.Pet.Attach(stream);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case 4070:
                        {
                            //if (user.Player.Map == 1002 || user.Player.Map == 1005 || user.Player.Map == 1036 || user.Player.Map == 1038 || user.Player.Map == 1121)
                            //{
                            //    user.SendSysMesage("You can't summon guards");
                            //    break;
                            //}
                            user.SendSysMesage("You can't summon this guards");
                            break;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            var skill = Database.Server.Magic[ClientSpell.ID];
                            switch (ClientSpell.Level)
                            {
                                case 0: Name = "SkeletonA"; break;
                                case 1: Name = "SkeletonB"; break;
                                case 2: Name = "SkeletonC"; break;
                                case 3: Name = "SkeletonD"; break;
                            }
                            new MonsterPet(user.Player, Name, stream);
                            user.Pet.Attach(stream);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case 4000:
                        {
                            //if (user.Player.Map == 1002 || user.Player.Map == 1005 || user.Player.Map == 1036 || user.Player.Map == 1038 || user.Player.Map == 1121)
                            //{
                            //    user.SendSysMesage("You can't summon guards");
                            //    break;
                            //}
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            var skill = Database.Server.Magic[ClientSpell.ID];
                            switch (ClientSpell.Level)
                            {
                                case 0: Name = "IronGuard"; break;
                                case 1: Name = "CopperGuard"; break;
                                case 2: Name = "SilverGuard"; break;
                                case 3: Name = "GoldGuard"; break;
                            }
                            new MonsterPet(user.Player, Name, stream);
                            user.Pet.Attach(stream);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case 12000:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            var skill = Database.Server.Magic[ClientSpell.ID];
                            switch (ClientSpell.Level)
                            {
                                case 0: Name = "CubPanda"; break;
                                case 1: Name = "TinyPanda"; break;
                                case 2: Name = "FluffyPanda"; break;
                                case 3: Name = "LittlePanda"; break;
                            }
                            new MonsterPet(user.Player, Name, stream);
                            user.Pet.Attach(stream);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}
