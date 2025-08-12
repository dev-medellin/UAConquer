using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestWebhook
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string newWebhookUrl = "https://discord.com/api/webhooks/1328842658357182474/bbpaQp9PH9n4CwHugdTTcjP_SprboduQBQk_MgcV9183gA-BSfvki7ewZLWXjUBVAGjZ";
            string message = "Test message to verify new webhook functionality.";

            using (HttpClient client = new HttpClient())
            {
                Dictionary<string, string> discordMessage = new Dictionary<string, string>
                {
                    { "content", message }
                };

                var content = new FormUrlEncodedContent(discordMessage);

                try
                {
                    HttpResponseMessage response = await client.PostAsync(newWebhookUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Message sent to Discord!");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
        }
    }
}
