using Bloody.Core.Models.v1;
using Bloody.Core;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Exceptions;
using System;
using Unity.Transforms;
using VampireCommandFramework;
using Bloody.Core.GameData.v1;
using Stunlock.Core;
using ProjectM;
using Bloody.Core.Helper.v1;
using Unity.Entities;
using System.Linq;
using BloodyBoss.Systems;
using Bloody.Core.API.v1;
using Unity.Collections;
using ProjectM.Network;
using BloodyBoss.Configuration;
using Unity.Mathematics;

namespace BloodyBoss.Command
{
    [CommandGroup("bb")]
    public static class BossCommand
    {

        [Command("list", usage: "", description: "List of Boss", adminOnly: true)]
        public static void ListBoss(ChatCommandContext ctx)
        {

            var Boss = Database.BOSSES;

            if (Boss.Count == 0)
            {
                throw ctx.Error($"There are no boss created");
            }
            ctx.Reply($"Boss List");
            ctx.Reply($"----------------------------");
            ctx.Reply($"--");
            foreach (var boss in Boss)
            {
                ctx.Reply($"Boss {boss.name}");
                ctx.Reply($"--");
            }
            ctx.Reply($"----------------------------");
        }

        [Command("test", usage: "", description: "Test Boss", adminOnly: true)]
        public static void Test(ChatCommandContext ctx, string bossName)
        {

            SetLocation(ctx,bossName);
            DateTime currentTime = DateTime.Now;
            DateTime x1MinLater = currentTime.AddMinutes(1);
            SetHour(ctx,bossName, x1MinLater.ToString("HH:mm"));
        }

        [Command("reload", usage: "", description: "Reload Database Boss", adminOnly: true)]
        public static void ReloadDatabase(ChatCommandContext ctx)
        {
            try
            {
                Database.loadDatabase();
                ctx.Reply($"Boss database reload successfully");
            } catch(Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
            
        }

        // .bb Test -1391546313 200 2 60 
        // .bb Boss items add Test ItemName -257494203 20 1
        // 
        // .bb Boss set hour Test 13:20
        // .bb Boss start Test
        // 
        //
        // .bb create "Alpha Wolf" -1905691330 90 1 1800
        // .bb set location "Alpha Wolf"
        // .bb set hour "Alpha Wolf" 22:00
        // .bb items add "Alpha Wolf" "Blood Rose Potion" 429052660 25 1
        // .bb start "Alpha Wolf"
        //
        // 
        [Command("create", usage: "<NameOfBOSS> <PrefabGUIDOfBOSS> <Level> <Multiplier> <LifeTimeSeconds>", description: "Create a Boss", adminOnly: true)]
        public static void CreateBOSS(ChatCommandContext ctx, string bossName, int prefabGUID, int level, int multiplier, int lifeTime)
        {
            try
            {
                var entityUnit = Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap[new PrefabGUID(prefabGUID)];

                //if(!entityUnit.Has<VBloodUnit>()) throw ctx.Error($"The PrefabGUID entered does not correspond to a VBlood Unit.");

                if (Database.AddBoss(bossName, prefabGUID, level, multiplier, lifeTime))
                {
                    ctx.Reply($"Boss '{bossName}' created successfully");
                }
            }
            catch (BossExistException)
            {
                throw ctx.Error($"Boss with name '{bossName}' exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("remove", usage: "", description: "Remove a Boss", adminOnly: true)]
        public static void RemoveBoss(ChatCommandContext ctx, string bossName)
        {

            try
            {
                if (Database.RemoveBoss(bossName))
                {
                    ctx.Reply($"Boss '{bossName}' remove successfully");
                }
            }
            catch (NPCDontExistException)
            {
                throw ctx.Error($"Boss with name '{bossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("set location", usage: "<NameOfBoss>", description: "Adds the current location of the player who sets it to the Boss.", adminOnly: true)]
        public static void SetLocation(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                var pos = Plugin.SystemsCore.EntityManager.GetComponentData<LocalToWorld>(user).Position;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetLocation(pos);
                    ctx.Reply($"Position {pos.x},{pos.y},{pos.z} successfully set to Boss '{BossName}'");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("set hour", usage: "<NameOfBoss> <Hour>", description: "Adds the hour and minutes in which the Boss spawn.", adminOnly: true)]
        public static void SetHour(ChatCommandContext ctx, string BossName, string hour)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetHour(hour);
                    ctx.Reply($"Hour {hour} successfully set to Boss '{BossName}'");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("start", usage: "<NameOfBoss>", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void start(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetHourDespawn();
                    Boss.Spawn(user);
                    
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("clearicon", usage: "<NameOfBoss>", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void ClearIcon(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    UserModel userModel = GameData.Users.GetUserByCharacterName(ctx.Event.User.CharacterName.Value);
                    Boss.RemoveIcon(userModel.Entity);
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("clearallicons", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void ClearAllIcons(ChatCommandContext ctx)
        {
            try
            {
                var userModel = GameData.Users.All.FirstOrDefault();
                var user = userModel.Entity;
                var entities = QueryComponents.GetEntitiesByComponentTypes<MapIconData>(EntityQueryOptions.IncludeDisabledEntities);
                var i = 0;
                foreach (var entity in entities)
                {
                    var prefab = entity.Read<PrefabGUID>();
                    var mapicon = Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup[prefab].AssetName;
                    if (prefab.GuidHash == Prefabs.MapIcon_POI_VBloodSource.GuidHash)
                    {
                        i++;
                        StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, entity, user, user, 0, StatChangeReason.Any, true);
                    }
                }
                entities.Dispose();
                ctx.Reply($"Removed icons ['{i}']");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        // ===== ADVANCED COMMANDS =====

        [Command("despawn", usage: "<BossName>", description: "Despawn a boss immediately", adminOnly: true)]
        public static void DespawnBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.bossSpawn)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not currently spawned");
                    }

                    var userModel = GameData.Users.GetUserByCharacterName(ctx.Event.User.CharacterName.Value);
                    
                    // Send manual despawn message
                    var message = $"⚡ Boss {FontColorChatSystem.Yellow(bossName)} has been manually despawned by admin";
                    var refMessage = (FixedString512Bytes)FontColorChatSystem.Red(message);
                    ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref refMessage);
                    
                    // Remove icon and entity
                    boss.RemoveIcon(userModel.Entity);
                    boss.DespawnBoss(userModel.Entity);
                    boss.bossSpawn = false;
                    
                    // Clear killers and save
                    boss.RemoveKillers();
                    Database.saveDatabase();
                    
                    ctx.Reply($"Boss '{bossName}' despawned successfully");
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

        [Command("status", usage: "<BossName>", description: "Show detailed boss status", adminOnly: true)]
        public static void BossStatus(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"📊 Status for Boss '{bossName}':");
                    ctx.Reply($"├─ Currently Spawned: {(boss.bossSpawn ? "✅ Yes" : "❌ No")}");
                    ctx.Reply($"├─ Timer Status: {(boss.IsPaused ? "⏸️ Paused" : "▶️ Running")}");
                    ctx.Reply($"├─ Spawn Time: {boss.Hour}");
                    ctx.Reply($"├─ Despawn Time: {boss.HourDespawn}");
                    ctx.Reply($"├─ Level: {boss.level} | Multiplier: {boss.multiplier}x");
                    ctx.Reply($"├─ Lifetime: {boss.Lifetime}s ({boss.Lifetime/60}min)");
                    ctx.Reply($"├─ Position: ({boss.x:F1}, {boss.y:F1}, {boss.z:F1})");
                    ctx.Reply($"├─ Current Killers: {boss.GetKillers().Count}");
                    ctx.Reply($"├─ Consecutive Spawns: {boss.ConsecutiveSpawns}");
                    ctx.Reply($"├─ Difficulty Multiplier: {boss.CurrentDifficultyMultiplier:F2}x");
                    
                    if (boss.bossSpawn && boss.GetBossEntity())
                    {
                        var health = boss.bossEntity.Read<Health>();
                        var healthPercent = (health.Value / health.MaxHealth.Value) * 100;
                        ctx.Reply($"├─ Health: {health.Value:F0}/{health.MaxHealth.Value:F0} ({healthPercent:F1}%)");
                    }
                    
                    ctx.Reply($"├─ Items: {boss.items.Count} configured");
                    ctx.Reply($"└─ Last Spawn: {(boss.LastSpawn != DateTime.MinValue ? boss.LastSpawn.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}");
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

        [Command("debug", usage: "<BossName>", description: "Show technical debug info", adminOnly: true)]
        public static void DebugBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"🔧 Debug Info for '{bossName}':");
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
                        ctx.Reply($"├─ Entity: ❌ Not found/Invalid");
                    }
                    
                    // Technical stats
                    if (boss.unitStats != null)
                    {
                        ctx.Reply($"├─ Physical Power: {boss.unitStats.PhysicalPower}");
                        ctx.Reply($"├─ Spell Power: {boss.unitStats.SpellPower}");
                        ctx.Reply($"├─ Physical Resistance: {boss.unitStats.PhysicalResistance}");
                    }
                    
                    ctx.Reply($"└─ Database Index: {Database.BOSSES.IndexOf(boss)}");
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

        [Command("simulate", usage: "<BossName> [killer]", description: "Simulate boss death for testing", adminOnly: true)]
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
                    
                    ctx.Reply($"🎭 Simulating death of boss '{bossName}' killed by '{killer}'...");
                    
                    // Simulate death process
                    boss.AddKiller(killer);
                    boss.BuffKillers();
                    boss.SendAnnouncementMessage();
                    
                    ctx.Reply($"✅ Death simulation completed:");
                    ctx.Reply($"├─ Killer added: {killer}");
                    ctx.Reply($"├─ Buffs applied: {(PluginConfig.BuffAfterKillingEnabled.Value ? "Yes" : "No")}");
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

        [Command("resetkills", usage: "<BossName>", description: "Clear boss killers list", adminOnly: true)]
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
                    
                    ctx.Reply($"🧹 Cleared {previousKillers} killers from boss '{bossName}'");
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

        [Command("forcedrop", usage: "<BossName> [player]", description: "Force boss to drop items", adminOnly: true)]
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
                    
                    ctx.Reply($"💰 Forcing item drop for boss '{bossName}'...");
                    
                    var dropped = boss.DropItems();
                    
                    if (dropped)
                    {
                        ctx.Reply($"✅ Items dropped successfully to:");
                        foreach (var killer in boss.GetKillers())
                        {
                            ctx.Reply($"└─ {killer}");
                        }
                    }
                    else
                    {
                        ctx.Reply($"❌ No items were dropped (check boss configuration)");
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

        [Command("teleport", usage: "<BossName>", description: "Teleport to boss location")]
        public static void TeleportToBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                var playerName = ctx.Event.User.CharacterName.Value;
                var userModel = GameData.Users.GetUserByCharacterName(playerName);
                
                // Verify if the command is enabled
                if (!PluginConfig.EnableTeleportCommand.Value)
                {
                    throw ctx.Error("Teleport command is disabled");
                }
                
                // Verify permissions (admin only if configured)
                if (PluginConfig.TeleportAdminOnly.Value && !ctx.Event.User.IsAdmin)
                {
                    throw ctx.Error("🚫 Teleport command is restricted to administrators only");
                }
                
                // Verify if the boss exists
                if (!Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    throw ctx.Error($"Boss '{bossName}' does not exist");
                }
                
                // Verify if only allows teleport to active bosses
                if (PluginConfig.TeleportOnlyToActiveBosses.Value && !boss.bossSpawn)
                {
                    throw ctx.Error($"Boss '{bossName}' is not currently active");
                }
                
                // Verify cooldown
                if (!TeleportManager.CanPlayerTeleport(playerName, out string cooldownReason))
                {
                    throw ctx.Error($"⏰ {cooldownReason}");
                }
                
                // Verify cost (only for non-admins)
                if (!ctx.Event.User.IsAdmin && !TeleportManager.HasTeleportCost(userModel, out string costInfo))
                {
                    throw ctx.Error($"💰 {costInfo}");
                }
                
                // Determine teleport position
                float3 targetPosition;
                bool isBossAlive = false;
                
                if (boss.bossSpawn && boss.GetBossEntity())
                {
                    // Verify if the boss is alive
                    if (boss.bossEntity.Has<Health>())
                    {
                        var health = boss.bossEntity.Read<Health>();
                        isBossAlive = !health.IsDead && health.Value > 0;
                        
                        if (PluginConfig.TeleportRequireBossAlive.Value && !isBossAlive)
                        {
                            throw ctx.Error($"💀 Boss '{bossName}' is dead");
                        }
                    }
                    
                    // Teleport to current boss position
                    var bossTranslation = boss.bossEntity.Read<Translation>();
                    targetPosition = bossTranslation.Value;
                    
                    var statusInfo = isBossAlive ? "alive" : "dead";
                    ctx.Reply($"🌀 Teleporting to {statusInfo} boss '{bossName}'...");
                }
                else
                {
                    // Teleport to configured boss position
                    targetPosition = new float3(boss.x, boss.y, boss.z);
                    ctx.Reply($"🌀 Teleporting to boss '{bossName}' spawn location...");
                }
                
                // Consume cost (only for non-admins)
                if (!ctx.Event.User.IsAdmin)
                {
                    if (!TeleportManager.ConsumeTeleportCost(userModel))
                    {
                        throw ctx.Error("Failed to consume teleport cost");
                    }
                }
                
                // Apply teleport
                var playerEntity = userModel.Character.Entity;
                var translation = playerEntity.Read<Translation>();
                translation.Value = targetPosition;
                playerEntity.Write(translation);
                
                // Apply cooldown
                TeleportManager.SetTeleportCooldown(playerName);
                
                // Confirmation message
                ctx.Reply($"✅ Teleported to {targetPosition.x:F1}, {targetPosition.y:F1}, {targetPosition.z:F1}");
                
                if (PluginConfig.TeleportCooldown.Value > 0)
                {
                    ctx.Reply($"⏰ Next teleport available in {PluginConfig.TeleportCooldown.Value:F0} seconds");
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Teleport command failed: {ex.Message}");
                throw ctx.Error($"Teleport failed: {ex.Message}");
            }
        }
    }
}
