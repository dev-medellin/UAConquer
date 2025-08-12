using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Game.MsgNpc
{
    using ActionInvoker = CachedAttributeInvocation<ProcessAction, NpcAttribute, NpcID>;
    using Extensions;
    using TheChosenProject.Client;
    using TheChosenProject.Game.MsgTournaments;
    using TheChosenProject.Role;
    using TheChosenProject.ServerSockets;
    using TheChosenProject.Database;
    using TheChosenProject.Ai;

    public unsafe delegate void ProcessAction(Client.GameClient user, ServerSockets.Packet stream, byte Option, string Input, uint id);



    public class Procesor
    {
        public class InvokerClient
        {
            public GameClient client;

            public byte InteractType;

            public byte option;

            public string input;

            public uint npcid;

            public InvokerClient(GameClient Client, uint _npcid, byte _InteractType, byte _option, string _input)
            {
                client = Client;
                option = _option;
                InteractType = _InteractType;
                input = _input;
                npcid = _npcid;
            }
        }

        public class ExecuteNpcInvoker : ConcurrentSmartThreadQueue<InvokerClient>
        {
            public ExecuteNpcInvoker()
                : base(3)
            {
                Start(5);
            }

            public void TryEnqueue(InvokerClient action)
            {
                Enqueue(action);
            }

            protected unsafe override void OnDequeue(InvokerClient action, int time)
            {
                try
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        if (!action.client.Player.VerifiedPassword)
                        {
                            NpcHandler.VerifiedPassword(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }

                        if (action.InteractType == 6)
                        {
                            if (action.client.Player.StartMessageBox > Time32.Now)
                            {
                                if (action.option == byte.MaxValue && action.client.Player.MessageOK != null)
                                    action.client.Player.MessageOK(action.client);
                                else
                                    action.client.Player.MessageCancel?.Invoke(action.client);
                            }
                            ActionQuery actionQuery;
                            actionQuery = default(ActionQuery);
                            actionQuery.ObjId = action.client.Player.UID;
                            actionQuery.Type = ActionType.CountDown;
                            actionQuery.dwParam = 3;
                            ActionQuery nTime;
                            nTime = actionQuery;
                            action.client.Send(stream.ActionCreate(&nTime));
                            action.client.Player.MessageOK = null;
                            action.client.Player.MessageCancel = null;
                            return;
                        }
                        if (action.npcid == 3124)
                        {
                            if (action.client.MyHouse != null && action.client.Player.DynamicID == action.client.Player.UID)
                            {
                                ActionQuery actionQuery;
                                actionQuery = default(ActionQuery);
                                actionQuery.Type = ActionType.OpenDialog;
                                actionQuery.ObjId = action.client.Player.UID;
                                actionQuery.dwParam = 4;
                                actionQuery.wParam1 = action.client.Player.X;
                                actionQuery.wParam2 = action.client.Player.Y;
                                ActionQuery query;
                                query = actionQuery;
                                action.client.Send(stream.ActionCreate(&query));
                                return;
                            }
                            action.client.SendSysMesage("I'm sorry but you dont own this house !");
                        }
                        if (action.npcid == (uint)NpcID.BlueMouse || action.npcid == (uint)NpcID.BlueMouse2 
                            || action.npcid == (uint)NpcID.BlueMouse3 || action.npcid == (uint)NpcID.BlueMouse4)
                        {
                            NpcHandler.BlueMouse(action.client, stream, action.option, action.input, action.npcid);
                        }
                        if (action.npcid == (uint)NpcID.MikeSkypass1 || action.npcid == (uint)NpcID.MikeSkypass2
                            || action.npcid == (uint)NpcID.MikeSkypass3 || action.npcid == (uint)NpcID.MikeSkypass4
                            || action.npcid == (uint)NpcID.MikeSkypass5)
                        {
                            NpcHandler.MikeSkypass(action.client, stream, action.option, action.input, action.npcid);
                        }
                        if (action.npcid == (uint)NpcID.MikeMain || action.npcid == (uint)NpcID.Mikesnake || action.npcid == (uint)NpcID.Mikesnake1
                            || action.npcid == (uint)NpcID.Mikesnake2 || action.npcid == (uint)NpcID.Mikesnake3
                            || action.npcid == (uint)NpcID.Mikesnake4 || action.npcid == (uint)NpcID.Mikesnake5 || action.npcid == (uint)NpcID.Mikesnake6
                            || action.npcid == (uint)NpcID.Mikesnake7 || action.npcid == (uint)NpcID.Mikesnake8 || action.npcid == (uint)NpcID.Mikesnake9
                            || action.npcid == (uint)NpcID.Mikesnake10 || action.npcid == (uint)NpcID.Mikesnake11 || action.npcid == (uint)NpcID.Mikesnake12
                            || action.npcid == (uint)NpcID.Mikesnake13 || action.npcid == (uint)NpcID.Mikesnake14 || action.npcid == (uint)NpcID.Mikesnake15
                            || action.npcid == (uint)NpcID.Mikesnake16 || action.npcid == (uint)NpcID.Mikesnake17 || action.npcid == (uint)NpcID.Mikesnake18
                            || action.npcid == (uint)NpcID.Mikesnake19 || action.npcid == (uint)NpcID.Mikesnake20 || action.npcid == (uint)NpcID.Mikesnake21
                            || action.npcid == (uint)NpcID.Mikesnake22 || action.npcid == (uint)NpcID.Mikesnake23)
                        {
                            NpcHandler.MikeMain(action.client, stream, action.option, action.input, action.npcid);
                        }
                        
                        if (action.client.Player.Map == 1038 && action.client.Player.View.TryGetValue(action.npcid, out var inpc2, MapObjectType.SobNpc))
                        {
                            SobNpc npc2;
                            npc2 = inpc2 as SobNpc;
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out Tuple<NpcAttribute, ProcessAction> processFolded4))
                                processFolded4.Item2(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        if (action.npcid == (uint)NpcID.DataVendor)
                        {
                            action.client.ActiveNpc = action.npcid;
                            DataVendor.Script(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        switch ((NpcID)action.npcid)
                        {
                            case NpcID.VIPBook:
                            case NpcID.VIPStorage:
                            case NpcID.RoyalPassManager:
                            case NpcID.VIPStorageBook:
                            case NpcID.AutoCompose:
                            case NpcID.WelcomeMessage:
                            case NpcID.DemonBox:
                            case NpcID.AuroraDemonBox:
                            case NpcID.SacredDemonBox:
                            case NpcID.ChaosDemonBox:
                            case NpcID.HeavenDemonBox:
                            case NpcID.OPBox:
                            case NpcID.Steed1Pack:
                            case NpcID.GoldPrizeToken:
                            case NpcID.NobleSteedPack:
                            case NpcID.Steed6:
                            case NpcID.Steed3:
                            case NpcID.Steed1:
                            case NpcID.WelcomeGearPack:
                                {
                                    if (invoker.TryGetInvoker((NpcID)action.npcid, out Tuple<NpcAttribute, ProcessAction> processFolded3))
                                        processFolded3.Item2(action.client, stream, action.option, action.input, action.npcid);
                                    break;
                                }
                        }
                        Game.MsgNpc.Npc obj;
                        if (action.client.Map.SearchNpcInScreen(action.npcid, action.client.Player.X, action.client.Player.Y, out obj))
                        {

                            if (action.client.ProjectManager)
                                action.client.SendSysMesage("Active Npc [" + action.npcid + "] X[" + obj.X + "] Y[" + obj.Y + "]", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out Tuple<NpcAttribute, ProcessAction> processFolded2))
                            {
                                processFolded2.Item2(action.client, stream, action.option, action.input, action.npcid);
                                if (!action.client.ProjectManager)
                                    return;

                                try
                                {
                                    GameMap map2;
                                    map2 = Server.ServerMaps[obj.Map];
                                    int count;
                                    count = action.client.Get_NpcOption.Count;
                                    for (int x2 = 0; x2 < count; x2++)
                                    {
                                        string option2;
                                        option2 = action.client.Get_NpcOption[x2];
                                        string dailog2;
                                        dailog2 = action.client.Get_NpcDailog[x2];
                                        action.client.SendSysMesage($"{action.npcid}: [{obj.X},{obj.Y},{map2.Name}], [{obj.Mesh}], [{dailog2}],[{option2}].", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
                                    }
                                    return;
                                }
                                catch
                                {
                                    ServerKernel.Log.SaveLog("Error: NPC dailog count", true, LogType.WARNING);
                                    return;
                                }
                            }
                            switch (action.client.Player.Map)
                            {
                                case 1126:
                                    {

                                        if (MsgSchedules.CurrentTournament.Type == TournamentType.FindTheBox)
                                        {
                                            if (Role.Core.GetDistance(action.client.Player.X, action.client.Player.Y, obj.X, obj.Y) <= 13)
                                            {
                                                MsgFindTheBox tournament;
                                                tournament = MsgSchedules.CurrentTournament as MsgFindTheBox;
                                                if (tournament.Process == ProcesType.Alive)
                                                    tournament.Reward(action.client, obj, stream);
                                                else
                                                    action.client.Teleport(428, 386, 1002);
                                            }
                                            else
                                            {
                                                action.client.CreateBoxDialog("The distance is too big between you and me.");
                                            }
                                        }
                                        

                                        return;
                                    }
                                //case 1737u:
                                //    if (MsgSchedules.CurrentTournament.Type == TournamentType.TreasureThief)
                                //    {
                                //        MsgTreasureThief tournament;
                                //        tournament = MsgSchedules.CurrentTournament as MsgTreasureThief;
                                //        if (tournament.Process == ProcesType.Alive)
                                //            tournament.Reward(action.client, obj, stream);
                                //        else
                                //            action.client.Teleport(428, 386, 1002);
                                //    }
                                //    return;

                                case 1038u:
                                    switch ((NpcID)action.npcid)
                                    {
                                        case NpcID.GuildConductor4:
                                        case NpcID.GuildConductor3:
                                        case NpcID.GuildConductor2:
                                        case NpcID.GuildConductor1:
                                            NpcHandler.GuildConductorsProces(action.client, stream, action.option, action.input, action.npcid);
                                            break;
                                        case NpcID.TeleGuild3:
                                        case NpcID.TeleGuild2:
                                        case NpcID.TeleGuild1:
                                            break;
                                    }
                                    return;
                                case 700u:
                                    {
                                        NpcID npcid;
                                        npcid = (NpcID)action.npcid;
                                        if (npcid - 925 <= NpcID.BlackSmith)
                                            NpcHandler.LotteryBoxes(action.client, stream, action.option, action.input, action.npcid);
                                        return;
                                    }
                            }
                            switch ((NpcID)action.npcid)
                            {
                                case NpcID.WHTwin:
                                case NpcID.WHMarket:
                                case NpcID.WHPoker:
                                case NpcID.WHDesert:
                                case NpcID.wHPheonix:
                                case NpcID.WHBird:
                                case NpcID.WHApe:
                                case NpcID.WHStone:
                                    NpcHandler.Warehause(action.client, stream, action.option, action.input, action.npcid);
                                    return;
                                case NpcID.WHMarket2:
                                case NpcID.WHMarket3:
                                case NpcID.WHMarket4:
                                case NpcID.WHMarket5:
                                    NpcHandler.Warehause2(action.client, stream, action.option, action.input, action.npcid);
                                    return;
                                case NpcID.TeleGuild4:
                                case NpcID.TeleGuild3:
                                case NpcID.TeleGuild2:
                                case NpcID.TeleGuild1:
                                    NpcHandler.GuildCondTeleBack(action.client, stream, action.option, action.input, action.npcid);
                                    return;
                            }
                            if (action.client.ProjectManager)
                                ServerKernel.Log.SaveLog($"not found npc data id:{action.npcid}", true, LogType.WARNING);
                        }
                        else if (action.npcid == 12)
                        {
                            if (action.client.Player.VipLevel > 1 
                                && !(Program.BlockTeleportMap.Contains(action.client.Player.Map) 
                                || action.client.Player.Map == 1038 
                                || Program.EventsMaps.Contains(action.client.Player.Map) || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && action.client.Player.Map == 8250)))
                            {
                                ActionQuery actionQuery;
                                actionQuery = default(ActionQuery);
                                actionQuery.Type = ActionType.OpenDialog;
                                actionQuery.ObjId = action.client.Player.UID;
                                actionQuery.dwParam = 341;
                                actionQuery.wParam1 = action.client.Player.X;
                                actionQuery.wParam2 = action.client.Player.Y;
                                ActionQuery query2;
                                query2 = actionQuery;
                                action.client.Send(stream.ActionCreate(&query2));
                            }
                        }
                        
                        
                            if (!action.client.Player.View.TryGetValue(action.npcid, out var inpc, MapObjectType.SobNpc))
                                return;
                            SobNpc npc;
                            npc = inpc as SobNpc;
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out Tuple<NpcAttribute, ProcessAction> processFolded))
                                processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            if (!action.client.ProjectManager)
                                return;
                            try
                            {
                                GameMap map;
                                map = Server.ServerMaps[npc.Map];
                                for (int x = 0; x < action.client.Get_NpcDailog.Count; x++)
                                {
                                    string option;
                                    option = action.client.Get_NpcOption[x];
                                    string dailog;
                                    dailog = action.client.Get_NpcDailog[x];
                                    action.client.SendSysMesage($"{action.npcid}: [{npc.X},{npc.Y},{map.Name}], [{npc.Mesh}], [{dailog}],[{option}].", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white);
                                }
                                return;
                            }
                            catch
                            {
                                ServerKernel.Log.SaveLog("Error: NPC dailog count", true, LogType.WARNING);
                                return;
                            }
                        
                    }
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
            }
        }

        public static ExecuteNpcInvoker ExecuteNpc = new ExecuteNpcInvoker();

        public static CachedAttributeInvocation<ProcessAction, NpcAttribute, NpcID> invoker = new CachedAttributeInvocation<ProcessAction, NpcAttribute, NpcID>(NpcAttribute.Translator);

        [Packet(2031)]
        private unsafe static void NpcServerReplay(GameClient user, Packet stream)
        {
            if (user.InTrade || user.IsVendor || !user.Socket.Alive || !user.Player.Alive)
                return;
            stream.NpcDialog(out var npcid, out var _, out var option, out var type, out var Action, out var input);
            Npc value;
            switch (Action)
            {
                case TheChosenProject.Game.MsgNpc.NpcServerReplay.Mode.PlaceFurniture:
                    if (!user.Inventory.HaveSpace(1))
                        user.CreateBoxDialog("Please make 1 more space your inventory.");
                    else
                    {
                        if (!user.MyHouse.Furnitures.TryGetValue(npcid, out var furniture))
                            break;
                        NpcServer.Furniture npc;
                        npc = NpcServer.GetNpcFromMesh(furniture.Mesh);
                        if (npc != null)
                        {
                            user.Inventory.Add(stream, npc.ItemID, 1, 0, 0, 0);
                            user.MyHouse.Furnitures.TryRemove(npcid, out value);
                            if (Server.ItemsBase.TryGetValue(npc.ItemID, out var item))
                                user.SendSysMesage("You got a " + item.Name + "!");
                            ActionQuery actionQuery;
                            actionQuery = default(ActionQuery);
                            actionQuery.ObjId = npcid;
                            actionQuery.Type = ActionType.RemoveEntity;
                            ActionQuery action;
                            action = actionQuery;
                            user.Send(stream.ActionCreate(&action));
                        }
                    }
                    break;
                case TheChosenProject.Game.MsgNpc.NpcServerReplay.Mode.Statue:
                    if (user.MyHouse.Furnitures.TryGetValue(npcid, out value))
                    {
                        user.MyHouse.Furnitures.TryRemove(npcid, out value);
                        ActionQuery actionQuery;
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = npcid;
                        actionQuery.Type = ActionType.RemoveEntity;
                        ActionQuery action2;
                        action2 = actionQuery;
                        user.Send(stream.ActionCreate(&action2));
                    }
                    break;
                default:
                    if (option != byte.MaxValue)
                    {
                        user.ActiveNpc = npcid;
                        ExecuteNpc.Enqueue(new InvokerClient(user, npcid, type, option, input));
                    }
                    break;
            }
        }

        [Packet(2032)]
        private unsafe static void NpcServerRequest(GameClient user, Packet stream)
        {
            if (user.InTrade || user.IsVendor || !user.Socket.Alive || !user.Player.Alive)
                return;
            stream.NpcDialog(out var npcid, out var _, out var option, out var type, out var _, out var input);
            switch (type)
            {
                case 6:
                    if (Program.BlockTeleportMap.Contains(user.Player.Map) || user.InFIveOut || user.InTDM || user.InLastManStanding || user.InPassTheBomb || user.InST)
                        return;
                    if (user.Player.StartMessageBox > Time32.Now)
                    {
                        if (option == 0 && user.Player.MessageOK != null)
                            user.Player.MessageOK(user);
                        else
                            user.Player.MessageCancel?.Invoke(user);
                        ActionQuery actionQuery;
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = user.Player.UID;
                        actionQuery.Type = ActionType.CountDown;
                        actionQuery.dwParam = 3;
                        ActionQuery nTime;
                        nTime = actionQuery;
                        user.Send(stream.ActionCreate(&nTime));
                    }
                    user.Player.MessageOK = null;
                    user.Player.MessageCancel = null;
                    return;
                case 102:
                    if ((user.Player.GuildRank == Flags.GuildMemberRank.GuildLeader || user.Player.GuildRank == Flags.GuildMemberRank.DeputyLeader) && user.Player.MyGuild != null)
                    {
                        user.Player.MyGuild.Quit(input, true, stream);
                        return;
                    }
                    break;
            }
            if (option != byte.MaxValue && option != 0 && !user.InTrade)
            {
                if (user.ActiveNpc == 9999997 && user.Player.WaitingKillCaptcha)
                {
                    if (option == 255) return;
                    if (input == user.Player.KillCountCaptcha)
                    {
                        user.Player.SolveCaptcha();
                    }
                    else
                    {
                        Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(user, stream);
                        dialog.Text("Input the current text: " + user.Player.KillCountCaptcha + " to verify your humanity.");
                        dialog.AddInput("Captcha message:", (byte)user.Player.KillCountCaptcha.Length);
                        dialog.Option("No thank you.", 255);
                        dialog.AddAvatar(39);
                        dialog.FinalizeDialog();
                    }
                    return;
                }
                npcid = user.ActiveNpc;
                ExecuteNpc.Enqueue(new InvokerClient(user, npcid, type, option, input));
            }
        }
    }
}
