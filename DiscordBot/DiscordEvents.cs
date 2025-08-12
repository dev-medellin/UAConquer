using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheChosenProject
{
    public class DiscordEvents
    {
        string API = "https://discord.com/api/webhooks/1328842658357182474/bbpaQp9PH9n4CwHugdTTcjP_SprboduQBQk_MgcV9183gA-BSfvki7ewZLWXjUBVAGjZ";
        Queue<string> Msgs;
        Uri webhook;

        public DiscordEvents(string API)
        {
            this.API = API;
            Msgs = new Queue<string>();
            webhook = new Uri(API);
            Console.WriteLine("DiscordEvents Server Ready.");
            var thread = new Thread(Dequeue);
            thread.Start();
        }
        private void Dequeue()
        {
            postToDiscordEvents("DiscordEvents Thread started");
            while (true)
            {
                try
                {
                    while (Msgs.Count != 0)
                    {
                        var msg = Msgs.Dequeue();
                        postToDiscordEvents(msg);
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        string lastmsg = "";
        public void Enqueue(string str)
        {
            Msgs.Enqueue(/*$"[{DateTime.Now.ToString()}]: */$"{str}");
        }
        private void postToDiscordEvents(string Text)
        {
            //if (Program.TestServer)
            //    return;
            if (lastmsg == Text)
                return;
            lastmsg = Text;
            Text = Text.Replace("@everyone", "");
            HttpClient client = new HttpClient();

            Dictionary<string, string> DiscordEventsToPost = new Dictionary<string, string>();
            DiscordEventsToPost.Add("content", Text);

            var content = new FormUrlEncodedContent(DiscordEventsToPost);

            var res = client.PostAsync(webhook, content).Result;
            //If you want to check result value
            if (res.IsSuccessStatusCode)
            {
                //Console.WriteLine($"ent {Text}!");
            }
        }
    }
}
