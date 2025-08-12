using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace DiscordOnline
{
    public class Discord
    {
        string API;
        Queue<string> Msgs;
        Uri webhook;

        public Discord(string API)
        {
            this.API = API;
            Msgs = new Queue<string>();
            webhook = new Uri(API);
           //System.Console.WriteLine("Discord Server Ready.");
            var thread = new Thread(Dequeue);
            thread.Start();

            // Send a test message
            Enqueue("Test message to check if webhook is working.");
        }

        private void Dequeue()
        {
            //while (true)
            //{
            //    try
            //    {
            //        while (Msgs.Count != 0)
            //        {
            //            var msg = Msgs.Dequeue();
            //            System.Console.WriteLine($"
            //            Dequeuing message: {msg}");
            //            postToDiscord(msg);
            //        }
            //        Thread.Sleep(1000);
            //    }
            //    catch (Exception e)
            //    {
            //        System.Console.WriteLine($"Error in Dequeue: {e}");
            //    }
            //}
        }

        public void Enqueue(string str)
        {
            Msgs.Enqueue($"{str} ``{DateTime.Now}``");
            //System.Console.WriteLine($"Enqueued message: {str}");
        }

        private void postToDiscord(string Text)
        {
            try
            {
                Text = Text.Replace("@everyone", "");
                HttpClient client = new HttpClient();
           
                Dictionary<string, string> discordToPost = new Dictionary<string, string>
                {
                    { "content", Text }
                };

                var content = new FormUrlEncodedContent(discordToPost);
                var res = client.PostAsync(webhook, content).Result;

                if (res.IsSuccessStatusCode)
                {
                    System.Console.WriteLine("Message sent to Discord!");
                }
                else
                {
                    System.Console.WriteLine($"Failed to send message. Status code: {res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error posting to Discord: {ex.Message}");
            }
        }
    }

    public static class DiscordAPIs
    {
        public static Discord DiscordOnlineAPI = new Discord("https://discord.com/api/webhooks/1328842658357182474/bbpaQp9PH9n4CwHugdTTcjP_SprboduQBQk_MgcV9183gA-BSfvki7ewZLWXjUBVAGjZ");
        public static Discord DiscordChangeNameAPI = new Discord("https://discord.com/api/webhooks/1328842658357182474/bbpaQp9PH9n4CwHugdTTcjP_SprboduQBQk_MgcV9183gA-BSfvki7ewZLWXjUBVAGjZ");
    }
}
