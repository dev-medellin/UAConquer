using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Npc
    {
        public static uint Execute(Packet stream, MsgSpellAnimation.SpellObj obj, GameClient client, SobNpc attacked)
        {
            if (attacked.Map == 1002 && attacked.UID >= 6003 && attacked.UID <= 6006)
            {
                client.TargetObjBot = attacked.UID;
                if (client.MaxHitStackes == 0)
                    client.MaxHitStackes = Program.GetRandom.Next(750, 900);
                if (client.Player.Away == 1)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var apacket = rec.GetStream();
                        client.Player.Away = 0;
                        client.Player.View.SendView(client.Player.GetArray(apacket, false), false);
                    }
                }
                client.HitStackes++;
                if (client.HitStackes >= client.MaxHitStackes)
                {
                    client.HitStackes = 0;
                    client.MaxHitStackes = Program.GetRandom.Next(750, 900);
                    //using (var rec = new ServerSockets.RecycledPacket())
                    //{
                    //    var apacket = rec.GetStream();
                    //    client.Player.Away = 0;
                    //    client.Player.View.SendView(client.Player.GetArray(apacket, false), false);

                    //}
                    if (Role.MyMath.Success(2))//(int)0.05)
                    {
                        int val = 50000;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var apacket = rec.GetStream();
                            client.Player.Away = 0;
                            client.Player.View.SendView(client.Player.GetArray(apacket, false), false);

                        }
                        client.SendSysMesage($"You got {val} Money!", MsgMessage.ChatMode.TopLeft);
                        client.Player.Money += val;
                        //Program.DiscordStakeTC.Enqueue($"Player {client.Player.Name} recieved {val} GOLD from TwinCity Stake!");
                    }
                    else
                    {
                        double chance = 1;
                        if(client.Player.Owner.Status.AgilityAtack > 250 
                            && (Database.AtributesStatus.IsTrojan(client.Player.Class) || Database.AtributesStatus.IsWarrior(client.Player.Class)) 
                            && client.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45
                            && client.Player.Agility >= 420)
                        {
                            chance = 0.5;
                        }
                        if (Database.AtributesStatus.IsArcher(client.Player.Class) && client.Player.Owner.GemValues(Flags.Gem.NormalFuryGem) >= 45
                            && client.Player.Agility >= 420)
                        {
                            chance = 0.5;
                        }
                        if (AtributesStatus.IsFire(client.Player.Class))
                        {
                            chance = 0.8;
                        }
                        if (Role.MyMath.Success(chance) && client.Player.Reborn == 2)
                        {
                            if (client.Inventory.HaveSpace(1) /*&& client.Player.LootDragonBall*/)
                            {
                                client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);

                                client.SendSysMesage($"You got a DragonBall!", MsgMessage.ChatMode.TopLeft);
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + client.Player.Name + " got DragonBall while hitting Stake at TwinCity!.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                                //Program.DiscordStakeTC.Enqueue($"Player {client.Player.Name} recieved Dragonball from TwinCity Stake!");
                            }
                        }
                    }
                }
                if (client.Player.Level >= 1 && client.Player.Reborn >= 0)
                {
                    uint exp = 0;

                    DBLevExp nextlevel;
                    nextlevel = Server.LevelInfo[DBLevExp.Sort.User][(byte)client.Player.Level];
                    double LoseExpPercent;
                    if (client.Player.Level >= 130)
                    {
                        LoseExpPercent = nextlevel.Experience * 2 / 100000uL;
                        exp = (uint)LoseExpPercent;
                        if (exp < 1)
                            exp = 1;
                    }
                    else if (client.Player.Level >= 100 && client.Player.Level < 130)
                    {
                        LoseExpPercent = nextlevel.Experience * 1 / 4000uL;
                        exp = (uint)LoseExpPercent;
                        if (exp < 1)
                            exp = 1;
                    }
                    else
                    {
                        LoseExpPercent = nextlevel.Experience * 1 / 3000uL; //Player.Experience * (uint)(nextlevel.UpLevTime * nextlevel.MentorUpLevTime) / nextlevel.Experience;

                        //loseexp = (ulong)(Player.Experience * 0.10);
                        exp = (uint)LoseExpPercent; //(double)loseexp / (double)nextlevel.Experience;
                        if (exp < 1)
                            exp = 1;
                    }

                    return exp;
                }
            }
            if (client.Pet != null) client.Pet.Target = attacked;

            //obj.Damage = 98989885;
            if ((long)obj.Damage >= (long)attacked.HitPoints)
            {
                uint hitPoints = (uint)attacked.HitPoints;
                attacked.Die(stream, client);
                if (attacked.Map == 1039)
                    return hitPoints / 10;
            }
            else
            {
                attacked.HitPoints -= (int)obj.Damage;
                if ((int)attacked.UID == (int)MsgSchedules.GuildWar.Furnitures[SobNpc.StaticMesh.Pole].UID)
                    MsgSchedules.GuildWar.UpdateScore(client.Player, obj.Damage);
                if ((int)attacked.UID == MsgTournaments.MsgEliteGuildWar.PoleUID)
                    MsgSchedules.EliteGuildWar.UpdateScore(client.Player, obj.Damage);
                if ((int)attacked.UID == MsgTournaments.MsgScoresWar.PoleUID)
                    MsgSchedules.ScoresWar.UpdateScore(client.Player, obj.Damage);
                //if ((int)attacked.UID == (int)MsgSchedules.EliteGuildWar.Furnitures[SobNpc.StaticMesh.Pole].UID)
                //    MsgSchedules.EliteGuildWar.UpdateScore(client.Player, obj.Damage);
                if ((int)attacked.UID == (int)MsgSchedules.ClanWar.Furnitures[SobNpc.StaticMesh.Pole].UID)
                    MsgSchedules.ClanWar.UpdateScore(client.Player, obj.Damage);
                if (MsgSchedules.CaptureTheFlag.Bases.ContainsKey(attacked.UID))
                    MsgSchedules.CaptureTheFlag.UpdateFlagScore(client.Player, attacked, obj.Damage, stream);
                else if (MsgSchedules.CityWar.Bases.ContainsKey(attacked.UID))
                    MsgSchedules.CityWar.UpdateScore(client.Player, attacked, obj.Damage);
                if (attacked.UID == TheChosenProject.Game.MsgGuildDeathMatch.PoleUID)
                    MsgSchedules.GuildDeathMatch.AddScore(obj.Damage, client.Player.MyGuild);

                if (attacked.UID == TheChosenProject.Game.MsgGuildDeathMatch.PoleUID)
                    MsgSchedules.GuildWar.UpdateScore(client.Player, obj.Damage);
                //if (attacked.UID == TheChosenProject.Game.MsgGuildDeathMatch.PoleUID)
                //    MsgSchedules.CitywarTC.UpdateScore(client.Player, obj.Damage);

                //if (attacked.UID == Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //        Game.MsgTournaments.MsgSchedules.GuildWar.UpdateScore(client.Player, obj.Damage);

                //if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarAC.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    Game.MsgTournaments.MsgSchedules.CitywarAC.UpdateScore(client.Player, obj.Damage);
                //if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarBI.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    Game.MsgTournaments.MsgSchedules.CitywarBI.UpdateScore(client.Player, obj.Damage);
                //if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarDC.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                //    Game.MsgTournaments.MsgSchedules.CitywarDC.UpdateScore(client.Player, obj.Damage);
                if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarPC.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.CitywarPC.UpdateScore(client.Player, obj.Damage);
                if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarTC.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                        Game.MsgTournaments.MsgSchedules.CitywarTC.UpdateScore(client.Player, obj.Damage);

                if (MsgTournaments.MsgNobilityPole.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == 22340)
                    MsgTournaments.MsgNobilityPole.UpdateScore(stream, obj.Damage, client.Player);
                if (MsgTournaments.MsgNobilityPole1.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == 22341)
                    MsgTournaments.MsgNobilityPole1.UpdateScore(stream, obj.Damage, client.Player);
                if (MsgTournaments.MsgNobilityPole2.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == 22342)
                    MsgTournaments.MsgNobilityPole2.UpdateScore(stream, obj.Damage, client.Player);
                if (MsgTournaments.MsgNobilityPole3.Proces == MsgTournaments.ProcesType.Alive && attacked.UID == 22343)
                    MsgTournaments.MsgNobilityPole3.UpdateScore(stream, obj.Damage, client.Player);

                if (attacked.Map == 1039)
                {
                    if (obj.Damage < 100)
                        return obj.Damage / 10;
                    if (obj.Damage < 1000)
                        return obj.Damage / 100;
                    return obj.Damage / 1000;
                }
                if (attacked.Map == 1038)
                    return obj.Damage / 1000;
            }
            return 0;
        }
    }
}
