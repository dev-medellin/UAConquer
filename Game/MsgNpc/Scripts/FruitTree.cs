using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TheChosenProject.Game.MsgNpc.Scripts.Quests
{
    public class FruitTree
    {
        [NpcAttribute(NpcID.FruitTree)]
        public static void Handle(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            data.AddAvatar(0);
            return;
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Prepare your self for the ultimate rewards!");
                        data.AddText("\n# ======================================================= #.");
                        data.AddText("\nCan you find some fruit for me,I am thinking of preparing a nice cocktail, what are you waiting for find some fruit");
                        data.AddText("\n    (Tomato)/(Guava)/(Watermelon)/(Pear)/(Grape).");
                        data.AddText("\n# ======================================================= #.");
                        data.AddText("\n Quest attempts : unlimited");
                        data.AddText("\n# ======================================================= #.");
                        data.AddText("\n Quest Requirements : level 137, 2nd reborn");
                        data.AddText("\n# ======================================================= #.");
                        data.AddText("\n             # Quest Rewards #");
                        data.AddText("\n 4,300 ConquerPoints | 5,000,000 Silver | Stone +3 ");
                        data.AddText("\n            VIP 1-Hour | random super gem");
                        data.AddText("\n# ======================================================= #.");                       
                        data.Option("Exchange Fruits.", 1);
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
                        if (client.Player.Level != 137 && client.Player.Reborn != 2)
                        {
                            data.AddText("------------------------------------------------------\n")
                                          .AddText("Sorry, you need to get level 137, 2nd reborn\n")
                                          .AddText("------------------------------------------------------")
                                          .AddOption("I~see.", 255).AddAvatar(53).FinalizeDialog();
                            return;
                        }
                        if (client.Inventory.Contain(711301, 1) && client.Inventory.Contain(711302, 1) && client.Inventory.Contain(711303, 1) && client.Inventory.Contain(711304, 1) && client.Inventory.Contain(711305, 1))
                        {
                            if (Anniversary.AnniversaryQuest.ContainsKey(client.Player.Name))
                                Anniversary.AnniversaryQuest[client.Player.Name]++;
                            else
                                Anniversary.AnniversaryQuest.Add(client.Player.Name, 1);

                            client.Inventory.RemoveStackItem(711301, 1, stream);
                            client.Inventory.RemoveStackItem(711302, 1, stream);
                            client.Inventory.RemoveStackItem(711303, 1, stream);
                            client.Inventory.RemoveStackItem(711304, 1, stream);
                            client.Inventory.RemoveStackItem(711305, 1, stream);

                            if (MyMath.ChanceSuccess(25))
                            {
                                client.Inventory.Add(stream, 730002, 1);
                                client.CreateBoxDialog("You has received a Stone +2!");
                                Program.SendGlobalPackets.SendMsgToAll(client.Player.Name + " has found all fruits and received a +2 Stone from FruitTree!", 2011);
                            }
                            else if (MyMath.ChanceSuccess(10))
                            {
                                client.Player.Money += 5000000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.CreateBoxDialog("You has received a  5,000,000 Silvers!");
                                Program.SendGlobalPackets.SendMsgToAll(client.Player.Name + " has found all fruits and received a 5,000,000 Silvers from FruitTree!", 2011);
                            }     
                            else if (MyMath.ChanceSuccess(10))
                            {
                                client.Inventory.Add(stream, 730003, 1);
                                client.CreateBoxDialog("You has received a Stone +3!");
                                Program.SendGlobalPackets.SendMsgToAll(client.Player.Name + " has found all fruits and received a +3 Stone from FruitTree!", 2011);
                            }                           
                            else
                            {
                                //client.Player.ConquerPoints += 4300;
                                client.CreateBoxDialog($"You have received 4,300 ConquerPoints!");
                                Program.SendGlobalPackets.SendMsgToAll(client.Player.Name + " has found all fruits and received a 4,300 ConquerPoints from FruitTree!", 2011);
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
                        else
                        {
                            data.AddText("Sorry, You need to have 5 fruits , you have to get [(Tomato)/(Guava)/(Watermelon)/(Pear)/(Grape)].")
                                .AddOption("Oops.", 255).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
    }
}