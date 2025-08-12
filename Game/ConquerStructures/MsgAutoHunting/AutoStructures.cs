using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using System;

 
namespace TheChosenProject.Game.MsgAutoHunting
{
    public class AutoStructures
    {
        [Flags]
        public enum Mode
        {
            NotActive = 0,
            Enable = 1,
            Disable = 2,
            Recived = 3
        }

        public static bool Validated(GameClient client)
        {
            bool canuse;
            canuse = true;
            if (Program.NotAllowAutoHunting.Contains(client.Player.Map))
            {
                client.CreateBoxDialog("Auto-hunting is not available here!");
                canuse = false;
            }
            //else if (client.Player.VipLevel < 4)
            //{
            //    client.CreateBoxDialog("Note: It`ll be available when you are VIP4");
            //    canuse = false;
            //}
            else if (!client.Player.Alive)
            {
                client.CreateBoxDialog("Auto-hunting can`t be used if you are dead.");
                canuse = false;
            }
            //if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
            //    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
            return canuse;
        }
    }
}
