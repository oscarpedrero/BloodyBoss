using Bloody.Core.GameData.v1;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Exceptions;
using BloodyBoss.Configuration;
using BloodyBoss.Systems;
using System;
using ProjectM;
using Unity.Transforms;
using Unity.Entities;
using VampireCommandFramework;
using Bloody.Core;
using Bloody.Core.API.v1;

namespace BloodyBoss.Command
{
    [CommandGroup("bb debug", "Boss debugging and testing commands")]
    public static class BossDebugCommand
    {
        [Command("test", usage: "<BossName>", description: "Quick test boss setup (set location + spawn in 1 min)", adminOnly: true)]
        public static void TestBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                // First check if boss exists
                if (!Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    throw ctx.Error($"Boss '{bossName}' does not exist");
                }
                
                // Set location to player's current position
                var user = ctx.Event.SenderUserEntity;
                var pos = Plugin.SystemsCore.EntityManager.GetComponentData<LocalToWorld>(user).Position;
                boss.SetLocation(pos);
                
                // Set spawn time to 1 minute from now
                DateTime currentTime = DateTime.Now;
                DateTime oneMinLater = currentTime.AddMinutes(1);
                boss.SetHour(oneMinLater.ToString("HH:mm"));
                
                ctx.Reply($"{FontColorChatSystem.Blue("[TEST]")} Test setup for boss '{bossName}':");
                ctx.Reply($"├─ Location: {pos.x:F1}, {pos.y:F1}, {pos.z:F1}");
                ctx.Reply($"└─ Spawn time: {oneMinLater:HH:mm} (in 1 minute)");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        [Command("info", usage: "<BossName>", description: "Show technical debug info", adminOnly: true)]
        public static void DebugBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"{FontColorChatSystem.Blue("[DEBUG]")} Debug Info for '{bossName}':");
                    ctx.Reply($"├─ Asset Name: {boss.AssetName}");
                    ctx.Reply($"├─ PrefabGUID: {boss.PrefabGUID}");
                    ctx.Reply($"├─ Name Hash: {boss.nameHash}");
                    ctx.Reply($"├─ VBlood First Kill: {boss.vbloodFirstKill}");
                    
                    if (boss.GetBossEntity())
                    {
                        ctx.Reply($"├─ Entity ID: {boss.bossEntity.Index}.{boss.bossEntity.Version}");
                        ctx.Reply($"├─ Has VBloodUnit: {boss.bossEntity.Has<VBloodUnit>()}");
                        ctx.Reply($"├─ Has Health: {boss.bossEntity.Has<Health>()}");
                        ctx.Reply($"├─ Has UnitStats: {boss.bossEntity.Has<UnitStats>()}");
                    }
                    else
                    {
                        ctx.Reply($"├─ Entity: {FontColorChatSystem.Red("Not found/Invalid")}");
                    }
                    
                    // Technical stats
                    if (boss.unitStats != null)
                    {
                        ctx.Reply($"├─ Physical Power: {boss.unitStats.PhysicalPower}");
                        ctx.Reply($"├─ Spell Power: {boss.unitStats.SpellPower}");
                        ctx.Reply($"├─ Physical Resistance: {boss.unitStats.PhysicalResistance}");
                        ctx.Reply($"└─ Spell Resistance: {boss.unitStats.SpellResistance}");
                    }
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

        [Command("simulate-death", usage: "<BossName> [killer]", description: "Simulate boss death for testing", adminOnly: true)]
        public static void SimulateBossDeath(ChatCommandContext ctx, string bossName, string killerName = null)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.bossSpawn)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not currently spawned");
                    }
                    
                    var killer = killerName ?? ctx.Event.User.CharacterName.Value;
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[SIMULATE]")} Simulating death of boss '{bossName}' killed by '{killer}'...");
                    
                    // Simulate death process
                    boss.AddKiller(killer);
                    boss.BuffKillers();
                    boss.SendAnnouncementMessage();
                    
                    ctx.Reply($"{FontColorChatSystem.Green("[SUCCESS]")} Death simulation completed:");
                    ctx.Reply($"├─ Killer added: {killer}");
                    ctx.Reply($"├─ Buffs applied: {(PluginConfig.BuffAfterKillingEnabled.Value ? FontColorChatSystem.Green("Yes") : FontColorChatSystem.Red("No"))}");
                    ctx.Reply($"├─ Items dropped: {boss.items.Count} configured");
                    ctx.Reply($"└─ Boss despawned: Yes");
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

        [Command("force-drop", usage: "<BossName> [player]", description: "Force boss to drop items", adminOnly: true)]
        public static void ForceBossDrop(ChatCommandContext ctx, string bossName, string playerName = null)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    var targetPlayer = playerName ?? ctx.Event.User.CharacterName.Value;
                    
                    if (boss.GetKillers().Count == 0)
                    {
                        boss.AddKiller(targetPlayer);
                    }
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[ITEMS]")} Forcing item drop for boss '{bossName}'...");
                    
                    var dropped = boss.DropItems();
                    
                    if (dropped)
                    {
                        ctx.Reply($"{FontColorChatSystem.Green("[SUCCESS]")} Items dropped successfully to:");
                        foreach (var killer in boss.GetKillers())
                        {
                            ctx.Reply($"└─ {killer}");
                        }
                    }
                    else
                    {
                        ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} No items were dropped (check boss configuration)");
                    }
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

        [Command("reset-kills", usage: "<BossName>", description: "Clear boss killers list", adminOnly: true)]
        public static void ResetBossKills(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    var previousKillers = boss.GetKillers().Count;
                    boss.RemoveKillers();
                    boss.vbloodFirstKill = false;
                    Database.saveDatabase();
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[RESET]")} Cleared {previousKillers} killers from boss '{bossName}'");
                    ctx.Reply($"└─ VBlood first kill flag reset");
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

        [Command("ability-info", usage: "<BossName>", description: "Show ability debug info for a boss", adminOnly: true)]
        public static void DebugBossAbilities(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.bossSpawn)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not currently spawned");
                    }
                    
                    if (!boss.GetBossEntity())
                    {
                        throw ctx.Error($"Could not find boss entity for '{bossName}'");
                    }
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[ABILITIES]")} Ability Debug Info for '{bossName}':");
                    var debugInfo = AbilitySwapSystem.GetAbilityDebugInfo(boss.bossEntity);
                    
                    foreach (var line in debugInfo.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            ctx.Reply(line);
                        }
                    }
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