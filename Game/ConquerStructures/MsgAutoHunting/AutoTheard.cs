using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Game.ConquerStructures.MsgAutoHunting;
using TheChosenProject.Game.MsgServer;
using System;


namespace TheChosenProject.Game.MsgAutoHunting
{
    public class AutoTheard
    {
        public static void StartAsync(GameClient client, Time32 Clock)
        {
            try
            {
                if (client != null && client.FullLoading && client.Player != null && client.Player.CompleteLogin && client.Player.AutoHunting == AutoStructures.Mode.Enable && client.Player.ContainFlag(MsgUpdate.Flags.AutoHunting))
                    AutoHunting.HuntingAsync(client, Clock);
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
