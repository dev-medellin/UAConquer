using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.ServerCore;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgNpc
{
    public class Dialog
    {
        private readonly GameClient client;

        public Packet stream;

        public Dialog(GameClient Client, Packet _stream)
        {
            stream = _stream;
            client = Client;
        }

        public Dialog CreateMessageBox(string Text)
        {
            Text = Translator.GetTranslatedString(Text, Translator.Language.EN, client.Language);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.MessageBox, Text, ushort.MaxValue, 0));
            return this;
        }

        public Dialog AddText(string text)
        {
            client.Get_NpcDailog.Clear();
            text = Translator.GetTranslatedString(text, Translator.Language.EN, client.Language);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Dialog, text, 0, 0));
            client.Get_NpcDailog.Add(text);
            return this;
        }

        public Dialog Text(string text)
        {
            client.Get_NpcDailog.Clear();
            text = Translator.GetTranslatedString(text, Translator.Language.EN, client.Language);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Dialog, text, 0, 0));
            client.Get_NpcDailog.Add(text);
            return this;
        }

        public Dialog AddAvatar(ushort id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Avatar, "", id, 0));
            return this;
        }

        public Dialog AddOption(string text, byte id)
        {
            client.Get_NpcOption.Clear();
            text = Translator.GetTranslatedString(text, Translator.Language.EN, client.Language);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, id));
            client.Get_NpcOption.Add(text);
            return this;
        }

        public Dialog Option(string text, byte id)
        {
            client.Get_NpcOption.Clear();
            text = Translator.GetTranslatedString(text, Translator.Language.EN, client.Language);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, id));
            client.Get_NpcOption.Add(text);
            return this;
        }

        public Dialog AddOption(string text)
        {
            client.Get_NpcOption.Clear();
            text = Translator.GetTranslatedString(text, Translator.Language.EN, client.Language);
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, byte.MaxValue));
            client.Get_NpcOption.Add(text);
            return this;
        }

        public Dialog AddInput(string text, byte id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Input, text, 16, id));
            return this;
        }

        public void FinalizeDialog(bool messagebox = false)
        {
            if (!messagebox)
                client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Finish, "", 0, 0, false));
        }
    }
}
