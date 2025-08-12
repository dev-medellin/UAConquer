using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraEditors.Filtering.Templates;
using System;
using System.IO;
using System.Linq;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgNpc.Scripts.Quests
{
    public class Solomon
    {
        public static bool RealMonsterIsSpawn = false;
        public static DateTime TimeRealMonsterIsDead = DateTime.Now;

        [NpcAttribute(NpcID.Solomon)]
        public static void Handle(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(114);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Prepare your self for the ultimate rewards!");
                        dialog.AddText("\n# ======================================================= #.");
                        dialog.AddText("\nare you ready to go through this difficult experience you will go to a land full of fake monsters you will kill them with one hit but there is only one real monster");
                        dialog.AddText("\nthat you hit with 2,000 damage and its HP reaches 150,000 this is the monster that is needed to kill it and you will get a nice drop.");
                        dialog.AddText("\n# ======================================================= #.");
                        dialog.AddText("\n Quest Schedule : Real Monster spawn eveny 1-Hour");
                        dialog.AddText("\n# ======================================================= #.");
                        dialog.AddText("\n Quest Requirements : level 137, 2nd reborn");
                        dialog.AddText("\n# ======================================================= #.");
                        dialog.AddText("\n             # Quest Rewards #");
                        dialog.AddText("\nDbScroll | Stone +4 | 3 MeteorScroll | Super Tortois Gem");
                        dialog.AddText("\n# ======================================================= #.");                       
                        dialog.AddOption("Ok, teleport me now.", 1);
                        dialog.AddOption("Just passing by", 255);
                        dialog.AddAvatar(53);
                        dialog.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.Level != 137 || client.Player.Reborn != 2 || !client.Inventory.HaveSpace(5))
                        {
                            dialog.AddText("------------------------------------------------------\n")
                                          .AddText("Sorry, you need to get level 137, 2nd reborn\n")
                                          .AddText("you need atleast 5 free spaces in your inventory.\n")
                                          .AddText("------------------------------------------------------")
                                          .AddOption("I~see.", 255).AddAvatar(53).FinalizeDialog();
                        }
                        else
                        {
                            CheackTargetToSolomon();
                            if (RealMonsterIsSpawn)
                            {
                                var map = Server.ServerMaps[3845];
                                ushort x = 0, y = 0;
                                map.GetRandCoord(ref x, ref y);
                                client.Teleport(x, y, 3845);
                            }
                            else
                            {
                                dialog.AddText($"Sorry, I can`t teleport you now, come in {TimeRealMonsterIsDead.ToString("HH:mm")}.");
                                dialog.AddOption("Ok.", 255);
                                dialog.FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }

        public static void CheackTargetToSolomon()
        {
            if (!RealMonsterIsSpawn && DateTime.Now > TimeRealMonsterIsDead)
            {
                Role.GameMap.EnterMap(3845);
                var map = Server.ServerMaps[3845];
                var arrays = map.View.GetAllMapRoles(Role.MapObjectType.Monster).ToArray();
                foreach (var checkmobs in arrays)
                {
                    var temp = checkmobs as MsgMonster.MonsterRole;
                    if (temp.isTargetToSolomon)
                        RealMonsterIsSpawn = true;
                }
                if (!RealMonsterIsSpawn)
                {
                    var mob = arrays[Role.Core.Random.Next(0, arrays.Length - 1)];
                    var monster = mob as MsgMonster.MonsterRole;
                    monster.Family.MaxHealth = 150000;
                    monster.HitPoints = (uint)monster.Family.MaxHealth;
                    Console.WriteLine($"Spawn Real Monster at ({monster.X},{monster.Y}).");
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"[LandOfGhosts]: real monster has been spawn at ({monster.X},{monster.Y})", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"[LandOfGhosts]: real monster has been spawn at ({monster.X},{monster.Y})", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"[LandOfGhosts]: real monster has been spawn at ({monster.X},{monster.Y})", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"[LandOfGhosts]: real monster has been spawn at ({monster.X},{monster.Y})", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.CrosTheServer).GetArray(stream));
                    }
                    RealMonsterIsSpawn = monster.isTargetToSolomon = true;
                }
            }
        }

        public static void isTargetToSolomonDead(Client.GameClient client, MsgMonster.MonsterRole mob)
        {
            if (mob.isTargetToSolomon)
            {
                RealMonsterIsSpawn = mob.isTargetToSolomon = false;
                TimeRealMonsterIsDead = DateTime.Now.AddHours(1);
                string msg = "";
                return;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (Role.MyMath.ChanceSuccess(1))
                    {
                        client.Inventory.Add(stream, 720135, 1);
                        msg = "The real monster " + mob.Name + " [in Land of Ghosts] has been destroyed by " + client.Player.Name.ToString() + " and he reward Random Super Gem! for more info go to Land of Ghosts in Twincity at (414, 384).";
                    }
                    else if (Role.MyMath.ChanceSuccess(2))
                    {
                        client.Inventory.Add(stream, 730004, 1);
                        msg = "The real monster " + mob.Name + " [in Land of Ghosts] has been destroyed by " + client.Player.Name.ToString() + " and he reward +4Stone! for more info go to Land of Ghosts in Twincity at (414, 384).";
                    }
                    else if (Role.MyMath.ChanceSuccess(10))
                    {
                        client.Inventory.Add(stream, 720028, 1);
                        msg = "The real monster " + mob.Name + " [in Land of Ghosts] has been destroyed by " + client.Player.Name.ToString() + " and he reward DBscroll! for more info go to Land of Ghosts in Twincity at (414, 384).";
                    }
                    else
                    {
                        client.Inventory.Add(stream, 730003, 1);
                        msg = "The real monster " + mob.Name + " [in Land of Ghosts] has been destroyed by " + client.Player.Name.ToString() + " and he reward +3Stone! for more info go to Land of Ghosts in Twincity at (414, 384).";
                    }
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                    var map = Server.ServerMaps[3845];
                    var arrays = map.View.GetAllMapRoles(Role.MapObjectType.Player).ToArray();
                    foreach (var obj in arrays)
                    {
                        var player = obj as Role.Player;
                        player.Owner.ExitToTwin();
                    }
                }
            }
        }
    }
}