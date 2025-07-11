using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Exceptions;
using System;
using System.Linq;
using VampireCommandFramework;

namespace BloodyBoss.Command
{
    [CommandGroup("bb schedule", "Boss scheduling and timer commands")]
    public static class BossScheduleCommand
    {
        [Command("set", usage: "<BossName> <HH:mm>", description: "Set spawn time for a boss", adminOnly: true)]
        public static void SetSchedule(ChatCommandContext ctx, string bossName, string time)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    boss.SetHour(time);
                    ctx.Reply($"⏰ Spawn time '{time}' set for boss '{bossName}'");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        [Command("pause", usage: "<BossName>", description: "Pause boss timer", adminOnly: true)]
        public static void PauseBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (boss.IsPaused)
                    {
                        throw ctx.Error($"Boss '{bossName}' is already paused");
                    }
                    
                    boss.IsPaused = true;
                    boss.PausedAt = DateTime.Now;
                    Database.saveDatabase();
                    
                    ctx.Reply($"⏸️ Boss '{bossName}' timer paused at {DateTime.Now:HH:mm:ss}");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("resume", usage: "<BossName>", description: "Resume boss timer", adminOnly: true)]
        public static void ResumeBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.IsPaused)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not paused");
                    }
                    
                    // Calculate pause duration
                    var pauseDuration = DateTime.Now - boss.PausedAt.Value;
                    
                    boss.IsPaused = false;
                    boss.PausedAt = null;
                    Database.saveDatabase();
                    
                    ctx.Reply($"▶️ Boss '{bossName}' timer resumed (was paused for {pauseDuration.TotalMinutes:F1} minutes)");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("list", usage: "", description: "List all scheduled bosses", adminOnly: true)]
        public static void ListSchedules(ChatCommandContext ctx)
        {
            try
            {
                var bosses = Database.BOSSES.Where(b => !string.IsNullOrEmpty(b.Hour)).OrderBy(b => b.Hour).ToList();
                
                if (bosses.Count == 0)
                {
                    ctx.Reply("No bosses have scheduled spawn times");
                    return;
                }
                
                ctx.Reply($"📅 Scheduled Bosses ({bosses.Count}):");
                ctx.Reply("----------------------------");
                
                foreach (var boss in bosses)
                {
                    var status = boss.IsPaused ? "⏸️ PAUSED" : (boss.bossSpawn ? "🟢 ACTIVE" : "⭕ SCHEDULED");
                    ctx.Reply($"{boss.Hour} - {boss.name} {status}");
                }
                
                ctx.Reply("----------------------------");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        [Command("clear", usage: "<BossName>", description: "Clear boss schedule", adminOnly: true)]
        public static void ClearSchedule(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    boss.Hour = "";
                    boss.IsPaused = false;
                    boss.PausedAt = null;
                    Database.saveDatabase();
                    
                    ctx.Reply($"🗓️ Schedule cleared for boss '{bossName}'");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }
    }
}