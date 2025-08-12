using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheChosenProject
{
    public class DiscordWorld
    {
        string API = "https://discord.com/api/webhooks/1371383612590657536/8agvV3QSnbRk4pHtlhJ3nVh4eUNFGMmJtm74pvUkB5ZbEsHvjPuWApfvQGnPX9I3ZgMR";
        Queue<string> Msgs;
        Uri webhook;

        public DiscordWorld(string API)
        {
            this.API = API;
            Msgs = new Queue<string>();
            webhook = new Uri(API);
            Console.WriteLine("DiscordWorld Server Ready.");
            var thread = new Thread(Dequeue);
            thread.Start();
        }
        private void Dequeue()
        {
            postToDiscordWorld("DiscordWorld Thread started");
            while (true)
            {
                try
                {
                    while (Msgs.Count != 0)
                    {
                        var msg = Msgs.Dequeue();
                        postToDiscordWorld(msg);
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
        private void postToDiscordWorld(string Text)
        {
            //if (Program.TestServer)
            //    return;
            if (lastmsg == Text)
                return;
            lastmsg = Text;
            Text = Text.Replace("@everyone", "");
            HttpClient client = new HttpClient();

            Dictionary<string, string> DiscordWorldToPost = new Dictionary<string, string>();
            DiscordWorldToPost.Add("content", Text);

            var content = new FormUrlEncodedContent(DiscordWorldToPost);

            var res = client.PostAsync(webhook, content).Result;
            //If you want to check result value
            if (res.IsSuccessStatusCode)
            {
                //Console.WriteLine($"ent {Text}!");
            }
        }
    }
}
