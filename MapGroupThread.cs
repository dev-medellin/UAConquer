using Extensions;
using Extensions.ThreadGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.Ai;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Role.Bot;

namespace TheChosenProject
{
    public class MapGroupThread
    {
        public const int AI_Buffers = 500;

        public const int AI_Guards = 700;

        public const int AUTO_HUNTING = 500;

        public const int AI_Bot = 500;

        public const int AI_Monsters = 400;

        public const int AI_Monsters_Revive = 800;

        public const int User_Buffers = 500;

        public const int User_Stamina = 500;

        public const int User_StampXPCount = 3000;

        public const int User_AutoAttack = 220; //220

        public const int User_CheckSeconds = 1000;

        public const int User_FloorSpell = 300;

        public const int User_CheckItems = 1000;
        public const int User_Mining = 3000;

        public ThreadItem Thread;
        public ThreadItem AiThread;
        public ThreadItem AiThread2;
        public ThreadItem AiThread3;

        public MapGroupThread(int interval, string name)
        {
            Thread = new ThreadItem(interval, name, OnProcess);
            AiThread = new ThreadItem(interval, name, AiOnProcess);
            AiThread2 = new ThreadItem(100, name, BotProcessring.OnAttacker);
            AiThread3 = new ThreadItem(50, name, BotProcessring.OnJump);

        }

        public void Start()
        {
            Thread.Open();
            AiThread2.Open();
            AiThread3.Open();
        }
        public void AiOnProcess()
        {
            try
            {
                foreach (var ai in Ai.DataVendor.AIPoll.Values)
                    ai.Thread();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }//bahaa
        }

        public void OnProcess()
        {
            Time32 clock;
            clock = Time32.Now;
            foreach (GameClient user in Server.GamePoll.Values)
            {
                if (user.Fake)
                {
                    if (user.AIType != 0 && clock > user.AI_Bot_Stamp)
                    {
                        AIThread.AIStartAsync(user);
                        user.AI_Bot_Stamp.Value = clock.Value + 500;
                    }
                    continue;
                }
                user.Player.View.CheckUpMonsters(clock);
                if (clock > user.BuffersStamp)
                {
                    PoolProcesses.BuffersCallback(user, clock);
                    user.BuffersStamp.Value = clock.Value + 500;
                }
                if (clock > user.StaminStamp)
                {
                    foreach (var ai in Ai.DataVendor.AIPoll.Values)
                        ai.Thread();
                    if(user.Player.VipLevel >= 6)
                    {
                        PoolProcesses.StaminaCallback(user, clock);
                        user.StaminStamp.Value = clock.Value + 500;
                    }
                    else
                    {
                        PoolProcesses.StaminaCallback(user, clock);
                        user.StaminStamp.Value = clock.Value + 800;
                    }
                }
                if (clock > user.XPCountStamp)
                {
                    PoolProcesses.StampXPCountCallback(user, clock);
                    user.XPCountStamp.Value = clock.Value + 3000;
                }
                if (clock > user.AttackStamp)
                {
                    Client.PoolProcesses.AutoAttackCallback(user, clock);
                    user.AttackStamp.Value = clock.Value + User_AutoAttack;
                }
                if (clock > user.CheckSecondsStamp)
                {
                    PoolProcesses.CheckSeconds(user, clock);
                    user.CheckSecondsStamp.Value = clock.Value + 1000;
                }
                if (clock > user.CheckItemsView)
                {
                    Client.PoolProcesses.AiThread(user);
                    FloorTraps.StartAsync(user, clock);
                    user.CheckItemsView.Value = clock.Value + 1000;
                }
                if (clock > user.Player.NextMine)
                {
                    Client.PoolProcesses.MiningProcess(user);
                    user.Player.NextMine.Value = clock.Value + User_Mining;
                }
                if (clock > user.AutoHuntingStamp)
                {
                    AutoTheard.StartAsync(user, clock);
                    user.AutoHuntingStamp.Value = clock.Value + 500;
                }
            }
        }
    }
}
