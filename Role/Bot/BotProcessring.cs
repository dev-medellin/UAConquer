using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace TheChosenProject.Role.Bot
{
    public class BotProcessring
    {
        public static ConcurrentDictionary<uint, AI> Bots = new ConcurrentDictionary<uint, AI>();

        public static void OnJump()
        {
            try
            {
                foreach (var bot in Bots.Values)
                {
                    lock (bot)
                    {
                        if (bot.ToStart > DateTime.Now)
                        {
                            if (bot.msgStart.AddSeconds(1) < DateTime.Now)
                            {
                                bot.Bot.Player.Target.Owner.SendSysMesage($"Bot will start after {(bot.ToStart - DateTime.Now).Seconds} Seconds");
                                bot.msgStart = DateTime.Now;
                            }
                            continue;
                        }
                        if (bot.Bot?.Player?.Alive ?? false)
                        {
                            bot.HandleJump();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public static void OnAttacker()
        {
            try
            {
                foreach (var bot in Bots.Values)
                {
                    lock (bot)
                    {
                        if (!bot.Bot.Player.Alive) continue;
                        if (bot.ToStart > DateTime.Now) continue;
                        bot.Attack();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
