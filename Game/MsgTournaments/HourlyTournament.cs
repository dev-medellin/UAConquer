using System;
using System.Collections.Generic;
using System.Linq;
using TheChosenProject.Game.MsgEvents;

namespace TheChosenProject.Game.MsgTournaments
{
    public class HourlyTournament
    {
        private static readonly Random random = new Random();
        private static readonly Dictionary<int, string> scheduledEvents = new Dictionary<int, string>();
        private static int lastScheduledDay = -1;

        private static readonly string[] allEvents = {
            "ClashOfLegends", "WarOfTitans", "FiveFuryTrial", "InfernoDetonation",
            "CrownConquest", "TempestRush", "ShadowHeist"
        };

        public static void Tick(DateTime Now64)
        {
            ScheduleDailyEvents(Now64);
            TriggerEvent(Now64);
        }

        private static void ScheduleDailyEvents(DateTime Now64)
        {
            if (Now64.DayOfYear == lastScheduledDay)
                return;

            scheduledEvents.Clear();
            lastScheduledDay = Now64.DayOfYear;

            Dictionary<string, int> lastUsedHour = allEvents.ToDictionary(e => e, e => -2);
            string lastEvent = string.Empty;

            for (int hour = 0; hour < 24; hour++)
            {
                var eligible = allEvents
                    .Where(e => e != lastEvent && (hour - lastUsedHour[e] + 24) % 24 >= 1)
                    .ToList();

                if (eligible.Count == 0)
                    eligible = allEvents.Where(e => e != lastEvent).ToList(); // fallback

                string chosen = eligible[random.Next(eligible.Count)];
                scheduledEvents[hour] = chosen;
                lastUsedHour[chosen] = hour;
                lastEvent = chosen;

                Console.WriteLine($"[Scheduler] Scheduled {chosen} at {hour:00}:00");
            }

            Console.WriteLine($"[Scheduler] Daily schedule created for {Now64:yyyy-MM-dd}.");
        }

        private static void TriggerEvent(DateTime Now64)
        {
            if (Now64.Minute == 20 && Now64.Second < 10)
            {
                int currentHour = Now64.Hour;

                if (scheduledEvents.TryGetValue(currentHour, out string eventName))
                {
                    switch (eventName)
                    {
                        case "ClashOfLegends":
                            if (!SkillsTournament.Started) new SkillsTournament();
                            break;
                        case "WarOfTitans":
                            if (!TeamDeathMatch.Started) new TeamDeathMatch();
                            break;
                        case "FiveFuryTrial":
                            if (!Get5HitOut.Started) new Get5HitOut();
                            break;
                        case "InfernoDetonation":
                            if (!PassTheBomb.Started) new PassTheBomb();
                            break;
                        case "CrownConquest":
                            if (!LastManStanding.Started) new LastManStanding();
                            break;
                        case "TempestRush":
                            {
                                Game.MsgEvents.Events e = new CycloneRace();
                                e.StartTournament();
                            }
                            break;
                        case "ShadowHeist":
                            {
                                Game.MsgEvents.Events e = new WhackTheThief();
                                e.StartTournament();
                            }
                            break;
                    }
                    //Program.DiscordEventsAPI.Enqueue($"{eventName} Has Started");
                }
            }
        }
    }
}
