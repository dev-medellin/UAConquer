using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Game.MsgNpc.Scripts.Quests.Halloween
{
    internal class Pumpkin
    {
        [NpcAttribute(NpcID.Pumpkin1)]
        public static void Pumpkin1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(195);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("When black cats prowl and pumpkins gleam, may luck be yours on Halloween.");
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Pumpkin2)]
        public static void Pumpkin2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(195);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Say boo and scary on.");
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Pumpkin3)]
        public static void Pumpkin3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(195);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Keep calm and eat more candy.");
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Pumpkin4)]
        public static void Pumpkin4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(195);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("the only thing we have to fear if fear itself.");
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Pumpkin5)]
        public static void Pumpkin5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            dialog.AddAvatar(195);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Everyday is halloween, isn't it? for some of us.");
                        dialog.AddOption("Just passing by", 255);
                        dialog.FinalizeDialog();
                        break;
                    }
            }
        }
    }
}
