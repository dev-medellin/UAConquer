using TheChosenProject.Game.MsgServer;
using System.Linq;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgNpc.Scripts.Quests
{
    public class SantaClaus
    {
        private static uint[] RewardItems = new uint[] { 1200006, 115003, 184315, 723694, 1200005, 5, 723694, 188405, 115003, 1200006, 721259, 115003, 723694, 5, 723701, 1200006, 115003 };
        private static Database.ItemType.DBItem[] Weap = null;
        [NpcAttribute(NpcID.SantaClaus)]
        public static void Handle(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            data.AddAvatar(304);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("hello dear, Phoenix Conquer is celebrating its anniversary ! do you wanna change the ingredients from city quest?");
                        data.AddText("~i can give amazing reward from this job random (ChristmasSuit, ChristmasCap, PermanentStone, Thought Drill, Stardrill, items(+5), Celestialstone, Exmptoontoken)..");
                        data.AddText("~You have to give me a (BeanStalk, FatPumpkin, Shampoo, Chocolate, XmasCandy) to get your reward.");
                        data.Option("Exchange Items.", 1);
                        data.AddOption("Anniversary Ranking.", 100);
                        data.Option("Nevermind", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 100:
                    {
                        var lists = Anniversary.GetRanking();
                        if (lists.Count > 0)
                        {
                            foreach (var text in lists)
                                data.AddText(text);
                        }
                        else data.AddText("Come later.");
                        data.AddOption("Ok!", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(720364, 1) && client.Inventory.Contain(720362, 1) && client.Inventory.Contain(720365, 1) && client.Inventory.Contain(710968, 1) && client.Inventory.Contain(720157, 1))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.RemoveStackItem(720364, 1, stream);
                                client.Inventory.RemoveStackItem(720362, 1, stream);
                                client.Inventory.RemoveStackItem(720365, 1, stream);
                                client.Inventory.RemoveStackItem(710968, 1, stream);
                                client.Inventory.RemoveStackItem(720157, 1, stream);

                                if (Anniversary.AnniversaryQuest.ContainsKey(client.Player.Name))
                                    Anniversary.AnniversaryQuest[client.Player.Name]++;
                                else
                                    Anniversary.AnniversaryQuest.Add(client.Player.Name, 1);

                                var id_item = RewardItems[Program.GetRandom.Next(0, RewardItems.Length)];

                                if (id_item != 5)
                                {
                                    Database.ItemType.DBItem dBItem;
                                    if (Server.ItemsBase.TryGetValue(id_item, out dBItem))
                                    {
                                        client.Inventory.Add(stream, id_item, 1);
                                        client.CreateBoxDialog($"You have received {dBItem.Name}!");
                                        Program.SendGlobalPackets.SendMsgToAll(client.Player.Name + $" has found all the city quest items and received a {dBItem.Name} from SantaClaus!", MsgMessage.ChatMode.System);
                                    }
                                }
                                else
                                {
                                    if (Weap == null)
                                        Weap = Server.ItemsBase.Values.Where(e => (TheChosenProject.Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.LeftWeapon || TheChosenProject.Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.RightWeapon) && e.Level < 100 && (e.ID % 10) == 5).ToArray();
                                    var CIBI = Weap[Role.Core.Random.Next(0, Weap.Length)];
                                    client.Inventory.Add(stream, CIBI.ID, 1, 5);
                                    client.CreateBoxDialog($"You have received {CIBI.Name}(+5)!");
                                    Program.SendGlobalPackets.SendMsgToAll(client.Player.Name + $" has found all the city quest items and received a {CIBI.Name}(+5) from SantaClaus!", MsgMessage.ChatMode.System);
                                }

                                Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                packet.ID = MsgStringPacket.StringID.Effect;
                                packet.UID = client.Player.UID;
                                packet.Strings = new string[1] { "lottery" };
                                client.Player.View.SendView(stream.StringPacketCreate(packet), true);

                                data.AddText($"Hurray! You have delivered all the items and received your reward!");
                                data.AddOption("Thanks", 255);
                                data.FinalizeDialog();
                            }
                            else client.CreateBoxDialog("Please make 1 more space in your inventory.");
                        }
                        else
                        {
                            data.AddText("Sorry, You need to have (BeanStalk, FatPumpkin, Shampoo, Chocolate, XmasCandy).").AddOption("Oops.", 255).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}