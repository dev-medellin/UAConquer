using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;

namespace TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanAttackNpc
    {
        public static bool Verified(GameClient client, SobNpc attacked, MagicType.Magic DBSpell)
        {
            foreach (var item in client.Equipment.CurentEquip)
            {
                if (item.Position == (ushort)Role.Flags.ConquerItem.RightWeapon
                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
                    || item.Position == (ushort)Role.Flags.ConquerItem.Ring
                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRing
                    || item.Position == (ushort)Role.Flags.ConquerItem.Fan
                    || item.Position == (ushort)Role.Flags.ConquerItem.RidingCrop)
                {
                    if (item.Durability <= 0)
                        return false;
                }
            }

            if (attacked.HitPoints == 0)
                return false;
            if (client.Player.OnTransform)
                return false;
            if (attacked.IsStatue)
            {
                if (attacked.HitPoints == 0)
                    return false;
                if (attacked.Map == 1002)
                    return false;
                if (MsgSchedules.GuildWar.Winner.GuildID != client.Player.GuildID)
                    return false;
                if ((int)client.Player.GuildRank < 990)
                    return false;
                
            }
            if (attacked.UID == 123456)
            {
                if (client.Player.MyGuild == null)
                    return false;
                if (MsgSchedules.PoleDomination.KillerGuildID == client.Player.MyGuild.Info.GuildID)
                    return false;
                if (attacked.HitPoints == 0)
                    return false;
            }
            if (MsgSchedules.CityWar.Bases.TryGetValue(attacked.UID, out var Bas))
            {
                if (MsgSchedules.CityWar.Proces != ProcesType.Alive)
                    return false;
                if (client.Player.MyGuild == null)
                    return false;
                if (Bas.Npc.HitPoints == 0)
                    return false;
                if (Bas.CapturerID == client.Player.GuildID)
                    return false;
            }
            if (attacked.UID == 1234567)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (client.Player.MyGuild == null)
                    return false;
                if (MsgSchedules.SmallCityGuilWar.KillerGuildID == client.Player.MyGuild.Info.GuildID)
                    return false;
                if (attacked.HitPoints == 0)
                    return false;
            }
            if (attacked.UID == 22340)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (MsgTournaments.MsgNobilityPole.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.Name == MsgTournaments.MsgNobilityPole.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                    return false;
                if (MsgTournaments.MsgNobilityPole.Proces != MsgTournaments.ProcesType.Alive)
                    return false;
                if (client.Player.NobilityRank != Role.Instance.Nobility.NobilityRank.King)
                    return false;
            }
            if (attacked.UID == 22341)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (MsgTournaments.MsgNobilityPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.Name == MsgTournaments.MsgNobilityPole1.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                    return false;
                if (MsgTournaments.MsgNobilityPole1.Proces != MsgTournaments.ProcesType.Alive)
                    return false;
                if (client.Player.NobilityRank != Role.Instance.Nobility.NobilityRank.Prince)
                    return false;
            }
            if (attacked.UID == 22342)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (MsgTournaments.MsgNobilityPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.Name == MsgTournaments.MsgNobilityPole2.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                    return false;
                if (MsgTournaments.MsgNobilityPole2.Proces != MsgTournaments.ProcesType.Alive)
                    return false;
                if (client.Player.NobilityRank != Role.Instance.Nobility.NobilityRank.Duke)
                    return false;
            }
            if (attacked.UID == 22343)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (MsgTournaments.MsgNobilityPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.Name == MsgTournaments.MsgNobilityPole3.Furnitures[Role.SobNpc.StaticMesh.Pole].Name)
                    return false;
                if (MsgTournaments.MsgNobilityPole3.Proces != MsgTournaments.ProcesType.Alive)
                    return false;
                if (client.Player.NobilityRank != Role.Instance.Nobility.NobilityRank.Earl)
                    return false;
            }
            // if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarAC.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
            // {
            //     if (client.Player.MyGuild == null)
            //         return false;
            //     if (Game.MsgTournaments.MsgSchedules.CitywarAC.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
            //         return false;
            //     if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.CitywarAC.Winner.GuildID)
            //         return false;
            //     if (Game.MsgTournaments.MsgSchedules.CitywarAC.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.CitywarAC.Proces == MsgTournaments.ProcesType.Idle)
            //         return false;
            // }
            // if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarBI.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
            // {
            //     if (client.Player.MyGuild == null)
            //         return false;
            //     if (Game.MsgTournaments.MsgSchedules.CitywarBI.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
            //         return false;
            //     if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.CitywarBI.Winner.GuildID)
            //         return false;
            //     if (Game.MsgTournaments.MsgSchedules.CitywarBI.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.CitywarBI.Proces == MsgTournaments.ProcesType.Idle)
            //         return false;
            // }
            if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarPC.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
            {
                if (client.Player.MyGuild == null)
                    return false;
                if (Game.MsgTournaments.MsgSchedules.CitywarPC.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.CitywarPC.Winner.GuildID)
                    return false;
                if (Game.MsgTournaments.MsgSchedules.CitywarPC.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.CitywarPC.Proces == MsgTournaments.ProcesType.Idle)
                    return false;
            }
            // if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarDC.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
            // {
            //     if (client.Player.MyGuild == null)
            //         return false;
            //     if (Game.MsgTournaments.MsgSchedules.CitywarDC.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
            //         return false;
            //     if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.CitywarDC.Winner.GuildID)
            //         return false;
            //     if (Game.MsgTournaments.MsgSchedules.CitywarDC.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.CitywarDC.Proces == MsgTournaments.ProcesType.Idle)
            //         return false;
            // }
            if (attacked.UID == Game.MsgTournaments.MsgSchedules.CitywarTC.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
            {
                if (client.Player.MyGuild == null)
                    return false;
                if (Game.MsgTournaments.MsgSchedules.CitywarTC.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.CitywarTC.Winner.GuildID)
                    return false;
                if (Game.MsgTournaments.MsgSchedules.CitywarTC.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.CitywarTC.Proces == MsgTournaments.ProcesType.Idle)
                    return false;
            }
            if (Game.MsgTournaments.MsgSchedules.MsgCityPole != null &&
                Game.MsgTournaments.MsgSchedules.MsgCityPole.Pole != null &&
                attacked.UID == Game.MsgTournaments.MsgSchedules.MsgCityPole.Pole.UID)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (client.Player.MyGuild == null)
                    return false;
                if (Game.MsgTournaments.MsgSchedules.MsgCityPole.Winner != null &&
                    Game.MsgTournaments.MsgSchedules.MsgCityPole.Winner.GuildID == client.Player.GuildID)
                    return false;
            }



            if (attacked.UID == MsgSchedules.ClanWar.Furnitures[SobNpc.StaticMesh.Pole].UID)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (client.Player.MyClan == null)
                    return false;
                if (MsgSchedules.ClanWar.Furnitures[SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.ClanUID == MsgSchedules.ClanWar.Winner.ClanID)
                    return false;
                if (MsgSchedules.ClanWar.Proces == ProcesType.Dead || MsgSchedules.ClanWar.Proces == ProcesType.Idle)
                    return false;
            }
            if (attacked.UID == MsgSchedules.GuildWar.Furnitures[SobNpc.StaticMesh.Pole].UID)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (client.Player.MyGuild == null)
                    return false;
                if (MsgSchedules.GuildWar.Furnitures[SobNpc.StaticMesh.Pole].HitPoints == 0)
                    return false;
                if (client.Player.GuildID == MsgSchedules.GuildWar.Winner.GuildID)
                    return false;
                if (MsgSchedules.GuildWar.Proces == ProcesType.Dead || MsgSchedules.GuildWar.Proces == ProcesType.Idle)
                    return false;
            }
            if (attacked.UID == Game.MsgTournaments.MsgEliteGuildWar.PoleUID)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (client.Player.MyGuild == null)
                    return false;
                if (Game.MsgTournaments.MsgEliteGuildWar.Pole.HitPoints == 0)
                    return false;
                if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Winner.GuildID)
                    return false;
                if (Game.MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Idle)
                    return false;
            }
            if (attacked.UID == Game.MsgTournaments.MsgScoresWar.PoleUID)
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (client.Player.MyGuild == null)
                    return false;
                if (Game.MsgTournaments.MsgScoresWar.Pole.HitPoints == 0)
                    return false;
                //if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.ScoresWar.Winner.GuildID)
                //    return false;
                if (Game.MsgTournaments.MsgSchedules.ScoresWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.ScoresWar.Proces == MsgTournaments.ProcesType.Idle)
                    return false;
            }
            //if (attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
            //{
            //    if (client.Player.MyGuild == null)
            //        return false;
            //    if (Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints == 0)
            //        return false;
            //    if (client.Player.GuildID == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Winner.GuildID)
            //        return false;
            //    if (Game.MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Dead || Game.MsgTournaments.MsgSchedules.EliteGuildWar.Proces == MsgTournaments.ProcesType.Idle)
            //        return false;
            //}
            if (MsgSchedules.CaptureTheFlag.Bases.TryGetValue(attacked.UID, out var Bass))
            {
                if (client.Player.MyGuild.Info.SilverFund <= 1000)
                {
                    client.SendSysMesage("You can't hit pole because your guild fund is low, donate gold to able hit the pole! #27");
                    return false;
                }
                if (MsgSchedules.CaptureTheFlag.Proces != ProcesType.Alive)
                    return false;
                if (client.Player.MyGuild == null)
                    return false;
                if (Bass.Npc.HitPoints == 0)
                    return false;
                if (Bass.CapturerID == client.Player.GuildID)
                    return false;
            }
            return true;
        }
    }
}
