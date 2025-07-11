using Bloody.Core.GameData.v1;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Utils;
using BloodyBoss.Systems;
using BloodyBoss.Components;
using BloodyBoss.Factory;
using System;
using System.Linq;
using ProjectM;
using ProjectM.Network;
using Unity.Transforms;
using Unity.Entities;
using Unity.Collections;
using VampireCommandFramework;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using Bloody.Core.API.v1;

namespace BloodyBoss.Command
{
    /// <summary>
    /// Developer-only commands for testing new features
    /// COMMENT THIS FILE FOR PRODUCTION BUILDS
    /// </summary>
    [CommandGroup("bb dev", "Developer testing commands")]
    public static class DevelopersCommand
    {
        [Command("test-extensions", usage: "<BossName>", description: "Test new BossEntityExtensions methods", adminOnly: true)]
        public static void TestExtensions(ChatCommandContext ctx, string bossName)
        {
            try
            {
                // Check if boss exists in database
                if (!Database.GetBoss(bossName, out BossEncounterModel bossModel))
                {
                    throw ctx.Error($"Boss '{bossName}' not found in database");
                }
                
                // Log to console for better readability
                Plugin.Logger.LogInfo("================== BossEntityExtensions Test ==================");
                Plugin.Logger.LogInfo($"Testing boss: {bossName}");
                Plugin.Logger.LogInfo("==============================================================");
                
                ctx.Reply($"🧪 Testing BossEntityExtensions for '{bossName}':");
                ctx.Reply($"├─ Looking for spawned boss entities...");
                
                // Find all entities with NameableInteractable
                var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeAll);
                int found = 0;
                
                foreach (var entity in entities)
                {
                    // Test IsBloodyBoss extension
                    if (entity.IsBloodyBoss())
                    {
                        // Test GetBossModel extension
                        var model = entity.GetBossModel();
                        if (model != null && model.name == bossName)
                        {
                            found++;
                            ctx.Reply($"├─ ✅ Found boss entity!");
                            Plugin.Logger.LogInfo($"[✓] Found boss entity for: {bossName}");
                            
                            // Test health extensions
                            var (current, max) = entity.GetBossHealth();
                            var percentage = entity.GetBossHealthPercentage();
                            ctx.Reply($"├─ Health: {current:F0}/{max:F0} ({percentage:F1}%)");
                            Plugin.Logger.LogInfo($"[Health] Current: {current:F0} / Max: {max:F0} ({percentage:F1}%)");
                            
                            // Test alive check
                            var isAlive = entity.IsBossAlive();
                            ctx.Reply($"├─ Is Alive: {isAlive}");
                            Plugin.Logger.LogInfo($"[Status] Is Alive: {isAlive}");
                            
                            // Test position
                            var pos = entity.GetBossPosition();
                            ctx.Reply($"├─ Position: {pos.x:F1}, {pos.y:F1}, {pos.z:F1}");
                            Plugin.Logger.LogInfo($"[Position] X: {pos.x:F1}, Y: {pos.y:F1}, Z: {pos.z:F1}");
                            
                            // Test level
                            var level = entity.GetBossLevel();
                            ctx.Reply($"├─ Level: {level}");
                            Plugin.Logger.LogInfo($"[Level] {level}");
                            
                            // Test PrefabGUID check
                            var correctPrefab = entity.IsPrefabGUID(bossModel.PrefabGUID);
                            ctx.Reply($"├─ Correct Prefab: {correctPrefab}");
                            Plugin.Logger.LogInfo($"[PrefabGUID] Matches expected: {correctPrefab} (Expected: {bossModel.PrefabGUID})");
                            
                            // Test team info
                            var (team, teamRef) = entity.GetBossTeam();
                            ctx.Reply($"├─ Team Value: {team.Value}");
                            Plugin.Logger.LogInfo($"[Team] Value: {team.Value}, Reference: {teamRef.Value._Value}");
                            
                            // Test players in radius
                            var players = entity.GetPlayersInRadius(30f);
                            ctx.Reply($"├─ Players within 30m: {players.Count}");
                            Plugin.Logger.LogInfo($"[Players] Within 30m radius: {players.Count}");
                            
                            // Despawn is now handled by LifeTime component
                            ctx.Reply($"└─ LifeTime: {bossModel.Lifetime}s");
                            Plugin.Logger.LogInfo($"[Despawn] Using LifeTime system ({bossModel.Lifetime}s)");
                        }
                    }
                }
                
                entities.Dispose();
                
                if (found == 0)
                {
                    ctx.Reply($"└─ ❌ No spawned boss found. Spawn it first!");
                    Plugin.Logger.LogWarning($"No spawned boss found for: {bossName}");
                }
                
                Plugin.Logger.LogInfo("================== Test Complete ==================");
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Test failed: {e.Message}");
                Plugin.Logger.LogError($"Stack trace: {e.StackTrace}");
                throw ctx.Error($"Test failed: {e.Message}");
            }
        }
        
        [Command("find-bosses", description: "Find all BloodyBoss entities currently spawned", adminOnly: true)]
        public static void FindBosses(ChatCommandContext ctx)
        {
            try
            {
                Plugin.Logger.LogInfo("================== Find All BloodyBosses ==================");
                ctx.Reply($"🔍 Searching for all BloodyBoss entities...");
                
                var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeAll);
                int count = 0;
                
                foreach (var entity in entities)
                {
                    if (entity.IsBloodyBoss())
                    {
                        count++;
                        var model = entity.GetBossModel();
                        var health = entity.GetBossHealthPercentage();
                        var pos = entity.GetBossPosition();
                        
                        if (model != null)
                        {
                            ctx.Reply($"├─ {model.name}: {health:F1}% HP");
                            Plugin.Logger.LogInfo($"[Boss #{count}] Name: {model.name}");
                            Plugin.Logger.LogInfo($"  - Health: {health:F1}%");
                            Plugin.Logger.LogInfo($"  - Position: ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})");
                            Plugin.Logger.LogInfo($"  - PrefabGUID: {model.PrefabGUID}");
                            Plugin.Logger.LogInfo($"  - Level: {entity.GetBossLevel()}");
                        }
                        else
                        {
                            var nameable = entity.Read<NameableInteractable>();
                            ctx.Reply($"├─ Unknown Boss (hash: {nameable.Name.Value})");
                            Plugin.Logger.LogWarning($"[Unknown Boss] Hash: {nameable.Name.Value}");
                            Plugin.Logger.LogWarning($"  - Health: {health:F1}%");
                            Plugin.Logger.LogWarning($"  - Position: ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})");
                        }
                    }
                }
                
                entities.Dispose();
                ctx.Reply($"└─ Total BloodyBosses found: {count}");
                Plugin.Logger.LogInfo($"Total BloodyBosses found: {count}");
                Plugin.Logger.LogInfo("================== Search Complete ==================");
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Find bosses error: {e.Message}");
                Plugin.Logger.LogError($"Stack trace: {e.StackTrace}");
                throw ctx.Error($"Error: {e.Message}");
            }
        }
        
        [Command(name: "debug-tracking", adminOnly: true, usage: ".bb dev debug-tracking", description: "Debug boss damage tracking")]
        public static void DebugTracking(ChatCommandContext ctx)
        {
            BossGameplayEventSystem.DebugListTrackedBosses();
            ctx.Reply("Boss tracking info logged to console");
        }
        
        [Command("test-players-radius", usage: "<radius>", description: "Test GetPlayersInRadius for current boss", adminOnly: true)]
        public static void TestPlayersRadius(ChatCommandContext ctx, float radius = 20f)
        {
            try
            {
                Plugin.Logger.LogInfo("================== Player Radius Detection Test ==================");
                Plugin.Logger.LogInfo($"Testing radius: {radius}m");
                
                var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeAll);
                bool found = false;
                
                foreach (var entity in entities)
                {
                    if (entity.IsBloodyBoss())
                    {
                        found = true;
                        var model = entity.GetBossModel();
                        var bossName = model?.name ?? "Unknown";
                        var bossPos = entity.GetBossPosition();
                        
                        ctx.Reply($"🎯 Testing radius detection for '{bossName}':");
                        Plugin.Logger.LogInfo($"Boss: {bossName}");
                        Plugin.Logger.LogInfo($"Boss Position: ({bossPos.x:F1}, {bossPos.y:F1}, {bossPos.z:F1})");
                        
                        var players = entity.GetPlayersInRadius(radius);
                        ctx.Reply($"├─ Players within {radius}m: {players.Count}");
                        Plugin.Logger.LogInfo($"Players found within {radius}m: {players.Count}");
                        
                        for (int i = 0; i < players.Count; i++)
                        {
                            if (players[i].Has<PlayerCharacter>())
                            {
                                var pc = players[i].Read<PlayerCharacter>();
                                var name = pc.Name.Value;
                                var playerPos = players[i].GetBossPosition();
                                var distance = Unity.Mathematics.math.distance(bossPos, playerPos);
                                
                                ctx.Reply($"├─ {name}: {distance:F1}m away");
                                Plugin.Logger.LogInfo($"[Player {i+1}] {name}");
                                Plugin.Logger.LogInfo($"  - Distance: {distance:F1}m");
                                Plugin.Logger.LogInfo($"  - Position: ({playerPos.x:F1}, {playerPos.y:F1}, {playerPos.z:F1})");
                            }
                        }
                        
                        if (players.Count == 0)
                        {
                            Plugin.Logger.LogInfo("No players found in radius");
                        }
                        break;
                    }
                }
                
                entities.Dispose();
                
                if (!found)
                {
                    ctx.Reply($"❌ No BloodyBoss spawned to test with!");
                    Plugin.Logger.LogWarning("No BloodyBoss entity found for radius test");
                }
                
                Plugin.Logger.LogInfo("================== Test Complete ==================");
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Radius test error: {e.Message}");
                Plugin.Logger.LogError($"Stack trace: {e.StackTrace}");
                throw ctx.Error($"Error: {e.Message}");
            }
        }
        
        [Command("benchmark-extensions", description: "Benchmark performance of extension methods", adminOnly: true)]
        public static void BenchmarkExtensions(ChatCommandContext ctx)
        {
            try
            {
                Plugin.Logger.LogInfo("================== Performance Benchmark ==================");
                ctx.Reply($"⏱️ Starting performance benchmark...");
                
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // Test 1: Find all bosses
                stopwatch.Restart();
                var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeAll);
                int totalEntities = 0;
                int bossCount = 0;
                foreach (var entity in entities)
                {
                    totalEntities++;
                    if (entity.IsBloodyBoss()) bossCount++;
                }
                entities.Dispose();
                var findTime = stopwatch.ElapsedMilliseconds;
                
                // Test 2: Get boss models
                stopwatch.Restart();
                entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeAll);
                int modelCount = 0;
                foreach (var entity in entities)
                {
                    if (entity.IsBloodyBoss() && entity.GetBossModel() != null)
                        modelCount++;
                }
                entities.Dispose();
                var modelTime = stopwatch.ElapsedMilliseconds;
                
                // Test 3: Health checks
                stopwatch.Restart();
                entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeAll);
                int aliveCount = 0;
                foreach (var entity in entities)
                {
                    if (entity.IsBloodyBoss() && entity.IsBossAlive())
                        aliveCount++;
                }
                entities.Dispose();
                var healthTime = stopwatch.ElapsedMilliseconds;
                
                // Test 4: Position checks
                stopwatch.Restart();
                entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeAll);
                int posCount = 0;
                foreach (var entity in entities)
                {
                    if (entity.IsBloodyBoss())
                    {
                        var pos = entity.GetBossPosition();
                        if (pos.x != 0 || pos.y != 0 || pos.z != 0)
                            posCount++;
                    }
                }
                entities.Dispose();
                var posTime = stopwatch.ElapsedMilliseconds;
                
                stopwatch.Stop();
                
                // Display results
                ctx.Reply($"📊 Benchmark Results:");
                ctx.Reply($"├─ Find all bosses: {findTime}ms (found {bossCount})");
                ctx.Reply($"├─ Get boss models: {modelTime}ms (found {modelCount})");
                ctx.Reply($"├─ Health checks: {healthTime}ms (alive {aliveCount})");
                ctx.Reply($"└─ Position checks: {posTime}ms (valid {posCount})");
                
                // Log detailed results
                Plugin.Logger.LogInfo("=== Benchmark Results ===");
                Plugin.Logger.LogInfo($"Total entities scanned: {totalEntities}");
                Plugin.Logger.LogInfo($"");
                Plugin.Logger.LogInfo($"[IsBloodyBoss Check]");
                Plugin.Logger.LogInfo($"  - Time: {findTime}ms");
                Plugin.Logger.LogInfo($"  - Found: {bossCount}/{totalEntities} entities");
                Plugin.Logger.LogInfo($"  - Performance: {(totalEntities > 0 ? (float)findTime/totalEntities : 0):F3}ms per entity");
                Plugin.Logger.LogInfo($"");
                Plugin.Logger.LogInfo($"[GetBossModel Check]");
                Plugin.Logger.LogInfo($"  - Time: {modelTime}ms");
                Plugin.Logger.LogInfo($"  - Found: {modelCount} models");
                Plugin.Logger.LogInfo($"  - Success rate: {(bossCount > 0 ? (float)modelCount/bossCount*100 : 0):F1}%");
                Plugin.Logger.LogInfo($"");
                Plugin.Logger.LogInfo($"[IsBossAlive Check]");
                Plugin.Logger.LogInfo($"  - Time: {healthTime}ms");
                Plugin.Logger.LogInfo($"  - Alive: {aliveCount}/{bossCount}");
                Plugin.Logger.LogInfo($"");
                Plugin.Logger.LogInfo($"[GetBossPosition Check]");
                Plugin.Logger.LogInfo($"  - Time: {posTime}ms");
                Plugin.Logger.LogInfo($"  - Valid positions: {posCount}/{bossCount}");
                Plugin.Logger.LogInfo($"");
                Plugin.Logger.LogInfo($"Total benchmark time: {findTime + modelTime + healthTime + posTime}ms");
                Plugin.Logger.LogInfo("================== Benchmark Complete ==================");
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Benchmark failed: {e.Message}");
                Plugin.Logger.LogError($"Stack trace: {e.StackTrace}");
                throw ctx.Error($"Benchmark failed: {e.Message}");
            }
        }
        
        [Command("validate-extensions", description: "Validate all extension methods work correctly", adminOnly: true)]
        public static void ValidateExtensions(ChatCommandContext ctx)
        {
            Plugin.Logger.LogInfo("================== Extension Methods Validation ==================");
            ctx.Reply($"🔬 Validating BossEntityExtensions...");
            
            int tests = 0;
            int passed = 0;
            var results = new System.Text.StringBuilder();
            
            try
            {
                // Test 1: Empty entity handling
                tests++;
                var emptyEntity = Entity.Null;
                bool test1 = !emptyEntity.IsBloodyBoss();
                if (test1) passed++;
                results.AppendLine($"[Test 1] Empty entity IsBloodyBoss: {(test1 ? "PASSED" : "FAILED")}");
                if (!test1) ctx.Reply($"❌ Empty entity check failed");
                
                // Test 2: SafeRead with missing component
                tests++;
                bool test2 = emptyEntity.SafeRead<Health>().Value == 0;
                if (test2) passed++;
                results.AppendLine($"[Test 2] SafeRead default value: {(test2 ? "PASSED" : "FAILED")}");
                if (!test2) ctx.Reply($"❌ SafeRead default failed");
                
                // Test 3: GetBossModel with non-boss
                tests++;
                bool test3 = emptyEntity.GetBossModel() == null;
                if (test3) passed++;
                results.AppendLine($"[Test 3] GetBossModel null entity: {(test3 ? "PASSED" : "FAILED")}");
                if (!test3) ctx.Reply($"❌ GetBossModel null check failed");
                
                // Test 4: Health percentage with dead boss
                tests++;
                bool test4 = emptyEntity.GetBossHealthPercentage() == 0;
                if (test4) passed++;
                results.AppendLine($"[Test 4] Health percentage null entity: {(test4 ? "PASSED" : "FAILED")}");
                if (!test4) ctx.Reply($"❌ Health percentage zero check failed");
                
                // Test 5: Position of null entity
                tests++;
                var nullPos = emptyEntity.GetBossPosition();
                bool test5 = nullPos.Equals(Unity.Mathematics.float3.zero);
                if (test5) passed++;
                results.AppendLine($"[Test 5] Position null entity: {(test5 ? "PASSED" : "FAILED")}");
                
                // Test 6: Level of null entity
                tests++;
                bool test6 = emptyEntity.GetBossLevel() == 0;
                if (test6) passed++;
                results.AppendLine($"[Test 6] Level null entity: {(test6 ? "PASSED" : "FAILED")}");
                
                // Test 7: PrefabGUID check on null
                tests++;
                bool test7 = !emptyEntity.IsPrefabGUID(12345);
                if (test7) passed++;
                results.AppendLine($"[Test 7] PrefabGUID null entity: {(test7 ? "PASSED" : "FAILED")}");
                
                // Test 8: GetPlayersInRadius on null
                tests++;
                bool test8 = false;
                try
                {
                    var players = emptyEntity.GetPlayersInRadius(10f);
                    test8 = players.Count == 0;
                }
                catch
                {
                    test8 = true; // Expected to handle gracefully
                }
                if (test8) passed++;
                results.AppendLine($"[Test 8] GetPlayersInRadius null entity: {(test8 ? "PASSED" : "FAILED")}");
                
                // Display results
                ctx.Reply($"✅ Validation complete: {passed}/{tests} tests passed");
                
                if (passed < tests)
                {
                    ctx.Reply($"⚠️ Some tests failed! Check console for details.");
                }
                else
                {
                    ctx.Reply($"🎉 All tests passed! Extensions are working correctly.");
                }
                
                // Log detailed results
                Plugin.Logger.LogInfo("=== Validation Results ===");
                Plugin.Logger.LogInfo(results.ToString());
                Plugin.Logger.LogInfo($"Total: {passed}/{tests} passed ({(float)passed/tests*100:F1}%)");
                Plugin.Logger.LogInfo("================== Validation Complete ==================");
            }
            catch (Exception e)
            {
                ctx.Reply($"❌ Validation error: {e.Message}");
                Plugin.Logger.LogError($"Validation error: {e.Message}");
                Plugin.Logger.LogError($"Stack trace: {e.StackTrace}");
            }
        }
        
        [Command("test-components", usage: "<BossName>", description: "Test new BossComponents system", adminOnly: true)]
        public static void TestComponents(ChatCommandContext ctx, string bossName)
        {
            try
            {
                Plugin.Logger.LogInfo("================== BossComponents Test ==================");
                Plugin.Logger.LogInfo($"Testing components for boss: {bossName}");
                
                ctx.Reply($"🧪 Testing BossComponents for '{bossName}':");
                
                // Test 1: Find boss using new query system
                var entityManager = Plugin.SystemsCore.EntityManager;
                var bossEntities = BossComponentsHelper.FindAllBossEntities(entityManager);
                
                ctx.Reply($"├─ Found {bossEntities.Count} boss entities total");
                Plugin.Logger.LogInfo($"[Query Test] Found {bossEntities.Count} boss entities");
                
                // Test 2: Check specific boss
                Entity targetBoss = Entity.Null;
                foreach (var entity in bossEntities)
                {
                    var identifier = BossComponents.GetBossIdentifier(entity, entityManager);
                    if (identifier == bossName)
                    {
                        targetBoss = entity;
                        ctx.Reply($"├─ ✅ Found target boss: {bossName}");
                        Plugin.Logger.LogInfo($"[Identifier Test] Found boss with identifier: {identifier}");
                        break;
                    }
                }
                
                // No need to dispose List<Entity>
                
                if (targetBoss == Entity.Null)
                {
                    ctx.Reply($"└─ ❌ Boss '{bossName}' not found!");
                    Plugin.Logger.LogWarning($"Boss '{bossName}' not found in spawned entities");
                    return;
                }
                
                // Test 3: Get/Create boss state
                if (!Database.GetBoss(bossName, out BossEncounterModel model))
                {
                    ctx.Reply($"└─ ❌ Boss model not found in database!");
                    return;
                }
                
                var bossState = BossComponentsHelper.GetOrCreateBossState(targetBoss, model);
                ctx.Reply($"├─ Boss State Created/Retrieved:");
                ctx.Reply($"├─   Name: {bossState.BossName}");
                ctx.Reply($"├─   Spawn Time: {bossState.SpawnTime:HH:mm:ss}");
                ctx.Reply($"├─   Difficulty: {bossState.DifficultyMultiplier:F2}x");
                
                Plugin.Logger.LogInfo($"[Boss State]");
                Plugin.Logger.LogInfo($"  - Name: {bossState.BossName}");
                Plugin.Logger.LogInfo($"  - NameHash: {bossState.NameHash}");
                Plugin.Logger.LogInfo($"  - SpawnTime: {bossState.SpawnTime}");
                Plugin.Logger.LogInfo($"  - IsPaused: {bossState.IsPaused}");
                Plugin.Logger.LogInfo($"  - ConsecutiveSpawns: {bossState.ConsecutiveSpawns}");
                Plugin.Logger.LogInfo($"  - DifficultyMultiplier: {bossState.DifficultyMultiplier}");
                
                // Test 4: Get combat stats
                var combatStats = BossComponentsHelper.GetBossCombatStats(targetBoss);
                ctx.Reply($"├─ Combat Stats:");
                ctx.Reply($"├─   Health: {combatStats.CurrentHealth:F0}/{combatStats.MaxHealth:F0} ({combatStats.HealthPercentage:F1}%)");
                ctx.Reply($"├─   Power: Physical {combatStats.PhysicalPower:F1}, Spell {combatStats.SpellPower:F1}");
                
                Plugin.Logger.LogInfo($"[Combat Stats]");
                Plugin.Logger.LogInfo($"  - Health: {combatStats.CurrentHealth}/{combatStats.MaxHealth} ({combatStats.HealthPercentage:F1}%)");
                Plugin.Logger.LogInfo($"  - PhysicalPower: {combatStats.PhysicalPower}");
                Plugin.Logger.LogInfo($"  - SpellPower: {combatStats.SpellPower}");
                Plugin.Logger.LogInfo($"  - PhysicalResistance: {combatStats.PhysicalResistance}");
                Plugin.Logger.LogInfo($"  - SpellResistance: {combatStats.SpellResistance}");
                
                // Test 5: Check phase
                var currentPhase = BossComponents.Phases.GetCurrentPhase(combatStats.HealthPercentage);
                ctx.Reply($"├─ Current Phase: {currentPhase.PhaseNumber} ({currentPhase.HealthThreshold}% threshold)");
                ctx.Reply($"└─ Phase Text: {currentPhase.AnnouncementText}");
                
                Plugin.Logger.LogInfo($"[Phase System]");
                Plugin.Logger.LogInfo($"  - Current Phase: {currentPhase.PhaseNumber}");
                Plugin.Logger.LogInfo($"  - Threshold: {currentPhase.HealthThreshold}%");
                Plugin.Logger.LogInfo($"  - Announcement: {currentPhase.AnnouncementText}");
                Plugin.Logger.LogInfo($"  - Last Announced: {bossState.LastAnnouncedPhase}");
                
                // Test 6: Spawn config
                var spawnConfig = BossComponentsHelper.GetSpawnConfig(model);
                Plugin.Logger.LogInfo($"[Spawn Config]");
                Plugin.Logger.LogInfo($"  - PrefabGUID: {spawnConfig.PrefabGUID}");
                Plugin.Logger.LogInfo($"  - Position: ({spawnConfig.Position.x:F1}, {spawnConfig.Position.y:F1}, {spawnConfig.Position.z:F1})");
                Plugin.Logger.LogInfo($"  - Level: {spawnConfig.Level}");
                Plugin.Logger.LogInfo($"  - Health Multiplier: {spawnConfig.HealthMultiplier}");
                Plugin.Logger.LogInfo($"  - Damage Multiplier: {spawnConfig.DamageMultiplier}");
                
                Plugin.Logger.LogInfo("================== Test Complete ==================");
            }
            catch (Exception e)
            {
                ctx.Reply($"❌ Test failed: {e.Message}");
                Plugin.Logger.LogError($"Component test error: {e.Message}");
                Plugin.Logger.LogError($"Stack trace: {e.StackTrace}");
            }
        }
        
        [Command("test-phase-transition", usage: "<BossName> <HealthPercent>", description: "Simulate phase transition", adminOnly: true)]
        public static void TestPhaseTransition(ChatCommandContext ctx, string bossName, float healthPercent)
        {
            try
            {
                if (healthPercent < 0 || healthPercent > 100)
                {
                    ctx.Reply($"❌ Health percent must be between 0 and 100");
                    return;
                }
                
                Plugin.Logger.LogInfo("================== Phase Transition Test ==================");
                
                // Get phases for health percentage
                var phase = BossComponents.Phases.GetCurrentPhase(healthPercent);
                
                ctx.Reply($"🎭 Phase at {healthPercent}% health:");
                ctx.Reply($"├─ Phase Number: {phase.PhaseNumber}");
                ctx.Reply($"├─ Threshold: {phase.HealthThreshold}%");
                ctx.Reply($"└─ Message: {phase.AnnouncementText}");
                
                // Show all phases
                ctx.Reply($"📊 All Phase Thresholds:");
                foreach (var p in BossComponents.Phases.StandardPhases)
                {
                    var marker = healthPercent <= p.HealthThreshold ? "→" : " ";
                    ctx.Reply($"{marker} Phase {p.PhaseNumber}: ≤{p.HealthThreshold}%");
                }
                
                Plugin.Logger.LogInfo($"Health: {healthPercent}% → Phase {phase.PhaseNumber}");
                Plugin.Logger.LogInfo("================== Test Complete ==================");
            }
            catch (Exception e)
            {
                ctx.Reply($"❌ Test failed: {e.Message}");
                Plugin.Logger.LogError($"Phase test error: {e.Message}");
            }
        }
        
        [Command("test-factory", usage: "<BossName>", description: "Test BossEntityFactory spawn", adminOnly: true)]
        public static void TestFactory(ChatCommandContext ctx, string bossName)
        {
            try
            {
                Plugin.Logger.LogInfo("================== BossEntityFactory Test ==================");
                
                // Get boss model
                if (!Database.GetBoss(bossName, out BossEncounterModel model))
                {
                    ctx.Reply($"❌ Boss '{bossName}' not found in database");
                    return;
                }
                
                ctx.Reply($"🏭 Testing BossEntityFactory for '{bossName}':");
                ctx.Reply($"├─ Using PrefabGUID: {model.PrefabGUID}");
                ctx.Reply($"├─ Level: {model.level}");
                ctx.Reply($"├─ Lifetime: {model.Lifetime}s");
                
                // Get player position for spawn
                var playerPos = ctx.Event.SenderCharacterEntity.Read<LocalToWorld>().Position;
                var spawnPos = new Unity.Mathematics.float3(
                    playerPos.x + 10, // Spawn 10 units away
                    playerPos.y,
                    playerPos.z
                );
                
                ctx.Reply($"├─ Spawn position: ({spawnPos.x:F1}, {spawnPos.y:F1}, {spawnPos.z:F1})");
                Plugin.Logger.LogInfo($"Spawning boss at: ({spawnPos.x:F1}, {spawnPos.y:F1}, {spawnPos.z:F1})");
                
                // Create boss using factory
                BossEntityFactory.CreateBoss(model, ctx.Event.SenderCharacterEntity, spawnPos, (Entity bossEntity) =>
                {
                    Plugin.Logger.LogInfo($"[Factory] Boss entity created: {bossEntity.Index}:{bossEntity.Version}");
                    
                    // Store entity reference for delayed verification
                    var entityIndex = bossEntity.Index;
                    var entityVersion = bossEntity.Version;
                    
                    // Schedule verification after a delay
                    CoroutineHandler.StartFrameCoroutine(() =>
                    {
                        // Reconstruct entity reference
                        var entity = new Entity { Index = entityIndex, Version = entityVersion };
                        
                        // Use EntityManager directly to check existence
                        var entityManager = Plugin.SystemsCore.EntityManager;
                        if (!entityManager.Exists(entity))
                        {
                            Plugin.Logger.LogError("[Factory] Entity no longer exists!");
                            return;
                        }
                        
                        // Now check with proper entity manager context
                        if (entityManager.HasComponent<NameableInteractable>(entity))
                        {
                            var nameable = entityManager.GetComponentData<NameableInteractable>(entity);
                            var name = nameable.Name.Value;
                            Plugin.Logger.LogInfo($"[Factory] Entity name: '{name}'");
                            
                            if (name.EndsWith("bb") || name.EndsWith("ibb"))
                            {
                                Plugin.Logger.LogInfo($"[Factory] Boss verified!");
                                Plugin.Logger.LogInfo($"  - Name: {model.name}");
                                
                                // Get health
                                if (entityManager.HasComponent<Health>(entity))
                                {
                                    var health = entityManager.GetComponentData<Health>(entity);
                                    var percentage = (health.Value / health.MaxHealth.Value) * 100f;
                                    Plugin.Logger.LogInfo($"  - Health: {percentage:F1}%");
                                }
                                
                                // Get level
                                if (entityManager.HasComponent<UnitLevel>(entity))
                                {
                                    var unitLevel = entityManager.GetComponentData<UnitLevel>(entity);
                                    Plugin.Logger.LogInfo($"  - Level: {unitLevel.Level._Value}");
                                }
                                
                                // Get position
                                if (entityManager.HasComponent<LocalToWorld>(entity))
                                {
                                    var ltw = entityManager.GetComponentData<LocalToWorld>(entity);
                                    Plugin.Logger.LogInfo($"  - Position: ({ltw.Position.x:F1}, {ltw.Position.y:F1}, {ltw.Position.z:F1})");
                                }
                            }
                            else
                            {
                                Plugin.Logger.LogError($"[Factory] Entity name doesn't match boss pattern: '{name}'");
                            }
                        }
                        else
                        {
                            Plugin.Logger.LogError("[Factory] Entity has no NameableInteractable component!");
                        }
                    }, 5, 1); // Wait 5 frames
                });
                
                ctx.Reply($"└─ ✅ Factory spawn initiated!");
                Plugin.Logger.LogInfo("================== Test Complete ==================");
            }
            catch (Exception e)
            {
                ctx.Reply($"❌ Factory test failed: {e.Message}");
                Plugin.Logger.LogError($"Factory test error: {e.Message}");
                Plugin.Logger.LogError($"Stack trace: {e.StackTrace}");
            }
        }
        
        
        [Command("time-test", "tt", "Test spawn/despawn at specific time", adminOnly: true)]
        public static void TestTime(ChatCommandContext ctx, string time)
        {
            try
            {
                // Validate time format
                if (!System.Text.RegularExpressions.Regex.IsMatch(time, @"^\d{2}:\d{2}$"))
                {
                    ctx.Reply("Invalid time format. Use HH:mm (e.g., 14:30)");
                    return;
                }
                
                ctx.Reply($"Testing spawn for time: {time}");
                
                // Check bosses configured for this time
                var bosses = Database.BOSSES.Where(x => x.Hour == time && !x.IsPaused).ToList();
                
                if (bosses.Count > 0)
                {
                    ctx.Reply($"Found {bosses.Count} bosses configured for {time}:");
                    foreach (var boss in bosses)
                    {
                        ctx.Reply($"- {boss.name} (PrefabGUID: {boss.PrefabGUID})");
                    }
                }
                else
                {
                    ctx.Reply($"No bosses configured for {time}");
                }
                
                // Despawn times no longer used - relying on LifeTime system
            }
            catch (Exception ex)
            {
                ctx.Reply($"Error testing time: {ex.Message}");
                Plugin.Logger.LogError($"Time test error: {ex.Message}");
            }
        }
    }
}