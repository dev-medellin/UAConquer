using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

 
namespace TheChosenProject.Game.MsgFloorItem
{
    internal class FloorTraps
    {
        public static void Dissappear(GameClient client, MsgItem item)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                if (item.IsTrap())
                    item.SendAll(stream, MsgDropID.RemoveEffect);
                else
                    item.SendAll(stream, MsgDropID.Remove);
                client.Map.View.LeaveMap((IMapObj)item);
                //Server.ActiveSquamas.Remove(item.UID);
            }
        }

        public static void StartAsync(GameClient client, Time32 Now)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || !client.Player.CompleteLogin)
                    return;
                foreach (IMapObj item in client.Player.View.Roles(MapObjectType.Item))
                {
                    MsgItem FloorItem;
                    FloorItem = item as MsgItem;
                    if (!item.Alive)
                    {
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream4;
                            stream4 = recycledPacket.GetStream();
                            MsgItem PItem;
                            PItem = item as MsgItem;
                            if (PItem.IsTrap())
                                PItem.SendAll(stream4, MsgDropID.RemoveEffect);
                            else
                                PItem.SendAll(stream4, MsgDropID.Remove);
                            client.Map.View.LeaveMap((IMapObj)PItem);
                        }
                    }
                    else
                    {
                        if (!item.IsTrap())//bahaa
                            continue;
                        if (item.Map == 1002 && item.X == 372 && item.Y == 335)
                            continue;
                        if (/*Server.ActiveSquamas.Contains(FloorItem.MsgFloor.m_UID) && */item.Map == client.Player.Map)
                        {
                            //if (client.Player.Position.Distance(item.Position) <= 1)
                            //{
                            //    using (RecycledPacket recycledPacket2 = new RecycledPacket())
                            //    {
                            //        Packet stream3;
                            //        stream3 = recycledPacket2.GetStream();
                            //        Dissappear(client, FloorItem);
                            //        switch (Program.GetRandom.Next(20))
                            //        {
                            //            case 0:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession");
                            //                    uint value;
                            //                    value = 10000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 1:
                            //                {
                            //                    uint value2;
                            //                    value2 = 25000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value2);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value2.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 2:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession1");
                            //                    uint value3;
                            //                    value3 = 50000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value3);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value3.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 4:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession2");
                            //                    uint value4;
                            //                    value4 = 75000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value4);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value4.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 5:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession3");
                            //                    uint value5;
                            //                    value5 = 100000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value5);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value5.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 6:
                            //                {
                            //                    uint value6;
                            //                    value6 = 125000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value6);
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession4");
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value6.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 7:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession");
                            //                    uint value7;
                            //                    value7 = 150000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value7);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value7.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 8:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession1");
                            //                    uint value8;
                            //                    value8 = 175000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value8);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value8.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 9:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession2");
                            //                    uint value9;
                            //                    value9 = 200000u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.Silver, Flags.CurrentPointAction.Add, value9);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value9.ToString(",0") + " silvers!");
                            //                    break;
                            //                }
                            //            case 10:
                            //                {
                            //                    uint value10;
                            //                    value10 = 25u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value10);
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession3");
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value10.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 11:
                            //                {
                            //                    uint value11;
                            //                    value11 = 50u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value11);
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession4");
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value11.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 12:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession");
                            //                    uint value12;
                            //                    value12 = 75u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value12);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value12.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 13:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession1");
                            //                    uint value13;
                            //                    value13 = 100u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value13);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value13.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 14:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession2");
                            //                    uint value14;
                            //                    value14 = 125u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value14);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value14.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 15:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession3");
                            //                    uint value15;
                            //                    value15 = 150u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value15);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value15.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 16:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession4");
                            //                    uint value16;
                            //                    value16 = 175u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value16);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value16.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 18:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession");
                            //                    uint value17;
                            //                    value17 = 200u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value17);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value17.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 19:
                            //                {
                            //                    client.Player.SendString(stream3, MsgStringPacket.StringID.Effect, true, "accession1");
                            //                    uint value18;
                            //                    value18 = 215u;
                            //                    client.Player.CurrentPoint(Flags.CurrentPoint.CPS, Flags.CurrentPointAction.Add, value18);
                            //                    client.CreateBoxDialog("Congratulations! You have found a Squama and received " + value18.ToString(",0") + " conquer points!");
                            //                    break;
                            //                }
                            //            case 3:
                            //            case 17:
                            //                break;
                            //        }
                            //    }
                            //}

                            if (client.Player.Map == 2060 && client.Player.ClanUID == MsgSchedules.ClanWar.Winner.ClanID)
                            {
                                if (client.Player.Alive)
                                {
                                    using (RecycledPacket recycledPacket3 = new RecycledPacket())
                                    {
                                        Packet stream2;
                                        stream2 = recycledPacket3.GetStream();
                                        if (!client.Player.ContainFlag(MsgUpdate.Flags.Stigma))
                                        {
                                            client.Player.AddFlag(MsgUpdate.Flags.Stigma, 60, true);
                                            client.Player.SendString(stream2, MsgStringPacket.StringID.Effect, true, "fam_gain_special");
                                        }
                                    }
                                }
                                else
                                {
                                    using (RecycledPacket rec = new RecycledPacket())
                                    {
                                        Packet stream;
                                        stream = rec.GetStream();
                                        MsgSpellAnimation MsgSpell;
                                        MsgSpell = new MsgSpellAnimation(client.Player.UID, 0u, client.Player.X, client.Player.Y, 1100, 0, 0);
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        AnimationObj = new MsgSpellAnimation.SpellObj(client.Player.UID, 0u, MsgAttackPacket.AttackEffect.None);
                                        client.Player.Revive(stream);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(client);
                                    }
                                }
                            }
                            if (client.Player.Map == 1005 /*&& client.Player.ClanUID == MsgSchedules.ClanWar.Winner.ClanID*/)
                            {
                                if (client.Player.Alive)
                                {
                                    using (RecycledPacket recycledPacket3 = new RecycledPacket())
                                    {
                                        Packet stream2;
                                        stream2 = recycledPacket3.GetStream();
                                        if (!client.Player.ContainFlag(MsgUpdate.Flags.Stigma))
                                        {
                                            client.Player.AddFlag(MsgUpdate.Flags.Stigma, 60, true);
                                            //client.Player.SendString(stream2, MsgStringPacket.StringID.Effect, true, "fam_gain_special");
                                        }
                                    }
                                }
                                //else
                                //{
                                //    using (RecycledPacket rec = new RecycledPacket())
                                //    {
                                //        Packet stream;
                                //        stream = rec.GetStream();
                                //        MsgSpellAnimation MsgSpell;
                                //        MsgSpell = new MsgSpellAnimation(client.Player.UID, 0u, client.Player.X, client.Player.Y, 1100, 0, 0);
                                //        MsgSpellAnimation.SpellObj AnimationObj;
                                //        AnimationObj = new MsgSpellAnimation.SpellObj(client.Player.UID, 0u, MsgAttackPacket.AttackEffect.None);
                                //        client.Player.Revive(stream);
                                //        MsgSpell.Targets.Enqueue(AnimationObj);
                                //        MsgSpell.SetStream(stream);
                                //        MsgSpell.Send(client);
                                //    }
                                //}
                            }
                            if (client.Player.Map == 1002 && Role.Core.GetDistance(client.Player.X, client.Player.Y, 441, 348) <= 5)
                            {
                                if (!client.Player.Alive)
                                {
                                    using (RecycledPacket rec = new RecycledPacket())
                                    {
                                        Packet stream;
                                        stream = rec.GetStream();
                                        MsgSpellAnimation MsgSpell;
                                        MsgSpell = new MsgSpellAnimation(client.Player.UID, 0u, client.Player.X, client.Player.Y, 1100, 0, 0);
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        AnimationObj = new MsgSpellAnimation.SpellObj(client.Player.UID, 0u, MsgAttackPacket.AttackEffect.None);
                                        client.Player.Revive(stream);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(client);
                                    }
                                }
                            }
                        }
                        _ = FloorItem.ItemBase;
                    }
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Check Items dropped theards problem", false, LogType.EXCEPTION);
            }
        }
    }
}
