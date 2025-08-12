using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace TheChosenProject
{
    public class DiscordDIBosses
    {
        string API = "https://discord.com/api/webhooks/1328842658357182474/bbpaQp9PH9n4CwHugdTTcjP_SprboduQBQk_MgcV9183gA-BSfvki7ewZLWXjUBVAGjZ";
        Queue<string> Msgs;
        Uri webhook;

        public DiscordDIBosses(string API)
        {
            this.API = API;
            Msgs = new Queue<string>();
            webhook = new Uri(API);
            Console.WriteLine("DiscordDIB Server Ready.");
            var thread = new Thread(Dequeue);
            thread.Start();
        }
        private void Dequeue()
        {
            postToDiscordDIB("DiscordDIB Thread started");
            while (true)
            {
                try
                {
                    while (Msgs.Count != 0)
                    {
                        var msg = Msgs.Dequeue();
                        postToDiscordDIB(msg);
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        //string lastmsg = "";
        public void Enqueue(string str)
        {
            Msgs.Enqueue(/*$"[{DateTime.Now.ToString()}]: */$"{str}");
        }
        private void postToDiscordDIB(string Text)
        {
            //if (Program.TestServer)
            //    return;
            //if (lastmsg == Text)
            //    return;
            //lastmsg = Text;
            //Text = Text.Replace("@everyone", "");
            HttpClient client = new HttpClient();

            Dictionary<string, string> DiscordDIBToPost = new Dictionary<string, string>();
            DiscordDIBToPost.Add("content", Text);

            var content = new FormUrlEncodedContent(DiscordDIBToPost);

            var res = client.PostAsync(webhook, content).Result;
            //If you want to check result value
            if (res.IsSuccessStatusCode)
            {
                //Console.WriteLine($"ent {Text}!");
            }
        }
    }
}
