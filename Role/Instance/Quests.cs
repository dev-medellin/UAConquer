using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Role.Instance
{
    public class Quests
    {
        public Player Player;

        public Dictionary<uint, MsgQuestList.QuestListItem> src;

        public Dictionary<uint, MsgQuestList.QuestListItem> AcceptedQuests;

        public Quests(Player _owner)
        {
            Player = _owner;
            AcceptedQuests = new Dictionary<uint, MsgQuestList.QuestListItem>();
            src = new Dictionary<uint, MsgQuestList.QuestListItem>();
        }

        public void IncreaseQuestObjectives(Packet stream, Flags.MissionsFlag UID, params uint[] Intentions)
        {
            if (!IsActiveQuest((uint)UID) || !AcceptedQuests.ContainsKey((uint)UID))
                return;
            MsgQuestList.QuestListItem item;
            item = AcceptedQuests[(uint)UID];
            if (Intentions.Length > item.Intentions.Length)
            {
                uint[] _Intentions;
                _Intentions = new uint[item.Intentions.Length];
                for (int x3 = 0; x3 < _Intentions.Length; x3++)
                {
                    _Intentions[x3] = item.Intentions[x3];
                }
                item.Intentions = new uint[Intentions.Length];
                for (int x2 = 0; x2 < _Intentions.Length; x2++)
                {
                    item.Intentions[x2] = _Intentions[x2];
                }
            }
            for (int x = 0; x < Intentions.Length; x++)
            {
                item.Intentions[x] += Intentions[x];
            }
            AcceptedQuests[(uint)UID] = item;
            Player.Owner.Send(stream.MsgQuestDataCreate(0, (uint)UID, item.Intentions));
        }

        public unsafe void SendAutoPatcher(string Text, uint map, ushort x, ushort y, NpcID NpcUid)
        {
            Player.MessageBox(Text, delegate (GameClient p)
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream;
                    stream = recycledPacket.GetStream();
                    ActionQuery actionQuery;
                    actionQuery = default(ActionQuery);
                    actionQuery.ObjId = p.Player.UID;
                    actionQuery.Type = ActionType.AutoPatcher;
                    actionQuery.Timestamp = (int)NpcUid;
                    actionQuery.wParam1 = x;
                    actionQuery.wParam2 = y;
                    actionQuery.dwParam = map;
                    actionQuery.dwParam3 = map;
                    ActionQuery actionQuery2;
                    actionQuery2 = actionQuery;
                    p.Send(stream.ActionCreate(&actionQuery2));
                }
            }, null);
        }

        public unsafe void SendAutoPatcher(uint map, ushort x, ushort y, uint NpcUid)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.ObjId = Player.UID;
                actionQuery.Type = ActionType.AutoPatcher;
                actionQuery.Timestamp = (int)NpcUid;
                actionQuery.wParam1 = x;
                actionQuery.wParam2 = y;
                actionQuery.dwParam = map;
                ActionQuery action;
                action = actionQuery;
                Player.Send(stream.ActionCreate(&action));
            }
        }

        public void RemoveQuest(uint UID)
        {
            if (src.ContainsKey(UID))
                src.Remove(UID);
            if (AcceptedQuests.ContainsKey(UID))
                AcceptedQuests.Remove(UID);
            MsgQuestList.QuestListItem n_quest;
            n_quest = new MsgQuestList.QuestListItem
            {
                UID = UID,
                Status = MsgQuestList.QuestListItem.QuestStatus.Available,
                Time = 0u
            };
            SendSinglePacket(n_quest, MsgQuestList.QuestMode.Review);
        }

        public bool IsActiveQuest(uint UID)
        {
            return CheckQuest(UID, MsgQuestList.QuestListItem.QuestStatus.Accepted);
        }

        public bool CheckObjectives(Flags.MissionsFlag UID, params uint[] Intentions)
        {
            if (AcceptedQuests.ContainsKey((uint)UID))
                try
                {
                    MsgQuestList.QuestListItem quest;
                    quest = AcceptedQuests[(uint)UID];
                    bool isdone;
                    isdone = true;
                    for (int x2 = 0; x2 < Intentions.Length; x2++)
                    {
                        if (quest.Intentions[x2] < Intentions[x2])
                            isdone = false;
                    }
                    return isdone;
                }
                catch
                {
                    for (int x = 0; x < 10; x++)
                    {
                        ServerKernel.Log.SaveLog("Error on quest : " + UID, true, LogType.WARNING);
                    }
                }
            return false;
        }

        public bool CheckQuest(uint UID, MsgQuestList.QuestListItem.QuestStatus status)
        {
            if (status == MsgQuestList.QuestListItem.QuestStatus.Accepted)
                return AcceptedQuests.ContainsKey(UID);
            return src.Values.Where((MsgQuestList.QuestListItem p) => p.UID == UID && p.Status == status).Count() == 1;
        }

        public bool FinishQuest(uint UID)
        {
            if (src.ContainsKey(UID))
            {
                MsgQuestList.QuestListItem item;
                item = src[UID];
                if (item.Status != MsgQuestList.QuestListItem.QuestStatus.Finished)
                {
                    item.Status = MsgQuestList.QuestListItem.QuestStatus.Finished;
                    src[UID] = item;
                    if (AcceptedQuests.ContainsKey(UID))
                        AcceptedQuests.Remove(UID);
                    SendSinglePacket(item, MsgQuestList.QuestMode.FinishQuest);
                    return true;
                }
            }
            return false;
        }

        public unsafe bool Accept(QuestInfo.DBQuest quest, uint time)
        {
            if (!src.ContainsKey(quest.MissionId))
            {
                MsgQuestList.QuestListItem questListItem;
                questListItem = new MsgQuestList.QuestListItem();
                questListItem.UID = quest.MissionId;
                questListItem.Status = MsgQuestList.QuestListItem.QuestStatus.Accepted;
                questListItem.Time = time;
                questListItem.Intentions = new uint[quest.Intentions];
                MsgQuestList.QuestListItem n_quest;
                n_quest = questListItem;
                AcceptedQuests.Add(n_quest.UID, n_quest);
                src.Add(n_quest.UID, n_quest);
                SendSinglePacket(n_quest, MsgQuestList.QuestMode.AcceptQuest);
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    ActionQuery actionQuery;
                    actionQuery = default(ActionQuery);
                    actionQuery.Type = ActionType.OpenCustom;
                    actionQuery.ObjId = Player.Owner.Player.UID;
                    actionQuery.dwParam = 3147;
                    ActionQuery action;
                    action = actionQuery;
                    Player.Owner.Send(stream.ActionCreate(&action));
                }
            }
            return true;
        }

        public void SendSinglePacket(MsgQuestList.QuestListItem data, MsgQuestList.QuestMode mode)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                stream.QuestListCreate(mode, 1);
                stream.AddItemQuestList(data);
                Player.Owner.Send(stream.QuestListFinalize());
                if (mode == MsgQuestList.QuestMode.FinishQuest && Player.Level < 130)
                    Player.Owner.GainExpBall(600.0, false, Flags.ExperienceEffect.None, false, false);
            }
        }

        public int AcceptQuestsCount()
        {
            return src.Values.Where((MsgQuestList.QuestListItem p) => p.Status == MsgQuestList.QuestListItem.QuestStatus.Accepted).Count();
        }

        public bool AllowAccept()
        {
            return AcceptQuestsCount() < 5;
        }

        public void SendFullGUI(Packet stream)
        {
            if (!src.ContainsKey(20199))
            {
                MsgQuestList.QuestListItem n_quest2;
                n_quest2 = new MsgQuestList.QuestListItem
                {
                    UID = 20199u,
                    Status = MsgQuestList.QuestListItem.QuestStatus.Accepted,
                    Time = 0u
                };
                AcceptedQuests.Add(n_quest2.UID, n_quest2);
                src.Add(n_quest2.UID, n_quest2);
            }
            if (!src.ContainsKey(20198))
            {
                MsgQuestList.QuestListItem n_quest;
                n_quest = new MsgQuestList.QuestListItem
                {
                    UID = 20198u,
                    Status = MsgQuestList.QuestListItem.QuestStatus.Accepted,
                    Time = 0u
                };
                AcceptedQuests.Add(n_quest.UID, n_quest);
                src.Add(n_quest.UID, n_quest);
            }
            Dictionary<int, Queue<MsgQuestList.QuestListItem>> Collection;
            Collection = new Dictionary<int, Queue<MsgQuestList.QuestListItem>> {
            {
                0,
                new Queue<MsgQuestList.QuestListItem>()
            } };
            int count;
            count = 0;
            QuestInfo.DBQuest[] Array;
            Array = QuestInfo.AllQuests.Values.ToArray();
            for (uint x2 = 0; x2 < Array.Length; x2++)
            {
                if (x2 % 80u == 0)
                {
                    count++;
                    Collection.Add(count, new Queue<MsgQuestList.QuestListItem>());
                }
                if (src.ContainsKey(Array[x2].MissionId))
                {
                    Collection[count].Enqueue(src[Array[x2].MissionId]);
                    continue;
                }
                MsgQuestList.QuestListItem quest;
                quest = new MsgQuestList.QuestListItem
                {
                    UID = Array[x2].MissionId,
                    Status = MsgQuestList.QuestListItem.QuestStatus.Available,
                    Time = 0u
                };
                Collection[count].Enqueue(quest);
            }
            foreach (Queue<MsgQuestList.QuestListItem> aray in Collection.Values)
            {
                Queue<MsgQuestList.QuestListItem> ItemArray;
                ItemArray = aray;
                ushort CCount;
                CCount = (ushort)ItemArray.Count;
                stream.QuestListCreate(MsgQuestList.QuestMode.Review, CCount);
                for (byte x = 0; x < CCount; x = (byte)(x + 1))
                {
                    stream.AddItemQuestList(ItemArray.Dequeue());
                }
                Player.Owner.Send(stream.QuestListFinalize());
            }
        }
    }
}
