using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using Bloody.Core.API.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using UnityEngine;
using ProjectM.Shared;
using ProjectM.Gameplay.Scripting;
using Stunlock.Network;
using Bloody.Core.Helper.v1;
using BloodyBoss.Data;

namespace BloodyBoss.Systems.Mechanics
{
    public class StunMechanic : IMechanic
    {
        public string Type => "stun";
        
        // Visual effect PrefabGUIDs
        private readonly PrefabGUID HOLY_BEAM_TARGET = PrefabConstants.HolyBeamTarget;
        private readonly PrefabGUID STUN_IMPACT = PrefabConstants.StunImpact;
        private readonly PrefabGUID STUN_DEBUFF = PrefabConstants.Stun;
        
        // Track active marks
        private readonly Dictionary<Entity, Entity> _activeMarks = new Dictionary<Entity, Entity>();

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var target = GetParameter<string>(parameters, "target", "nearest");
            var stunDuration = GetParameter<float>(parameters, "duration", 3f);
            var markDuration = GetParameter<float>(parameters, "mark_duration", 2.5f);
            var radius = GetParameter<float>(parameters, "radius", 0f);
            var maxTargets = GetParameter<int>(parameters, "max_targets", 1);
            var announcement = GetParameter<string>(parameters, "announcement", "👁️ The boss's psychic gaze locks onto its target!");
            var flashBeforeStun = GetParameter<bool>(parameters, "flash_before_stun", true);
            var canBeCleansed = GetParameter<bool>(parameters, "can_be_cleansed", true);
            var markEffect = GetParameter<string>(parameters, "mark_effect", "auto");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            var targets = GetTargets(bossPos, target, radius, maxTargets);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            foreach (var targetEntity in targets)
            {
                ApplyPsychicMark(targetEntity, markDuration, stunDuration, flashBeforeStun, markEffect);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Psychic stun mechanic executed: {targets.Count} targets marked for {markDuration}s before {stunDuration}s stun");
        }

        private void ApplyPsychicMark(Entity target, float markDuration, float stunDuration, bool flashBeforeStun, string markEffect)
        {
            try
            {
                // Use floating eye as default mark effect
                var markBuff = PrefabConstants.FloatingEyeMark;
                
                // If specific effect requested, use that instead
                if (markEffect != "auto" && int.TryParse(markEffect, out int specificGuid))
                {
                    markBuff = new PrefabGUID(specificGuid);
                    Plugin.BLogger.Debug(LogCategory.Mechanic, $"Using custom mark effect: {specificGuid}");
                }
                
                // Apply the mark
                try
                {
                    BuffCharacter(target, markBuff);
                    Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied mark buff {markBuff} to player for {markDuration}s");
                    
                    // Remove the mark just before applying stun
                    CoroutineHandler.StartGenericCoroutine(() =>
                    {
                        RemoveBuff(target, markBuff);
                    }, markDuration - 0.1f);
                }
                catch (Exception ex)
                {
                    Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to apply mark buff: {ex.Message}");
                }
                
                // Track the mark
                _activeMarks[target] = target;
                
                // Send warning message to all players
                if (target.Has<PlayerCharacter>())
                {
                    var playerName = target.Read<PlayerCharacter>().Name.ToString();
                    var warningMsg = $"⚠️ {playerName} is being targeted! Stun incoming in {markDuration:F1} seconds!";
                    var warningRef = (FixedString512Bytes)warningMsg;
                    ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref warningRef);
                    
                    Plugin.BLogger.Info(LogCategory.Mechanic, warningMsg);
                }
                
                // Schedule the stun application
                CoroutineHandler.StartGenericCoroutine(() =>
                {
                    Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applying stun now after {markDuration}s delay");
                    ApplyStunWithImpact(target, stunDuration);
                    
                    // Clean up mark
                    if (_activeMarks.ContainsKey(target))
                    {
                        _activeMarks.Remove(target);
                    }
                }, markDuration);
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, "Failed to apply psychic mark", ex);
            }
        }
        
        private void ApplyStunWithImpact(Entity target, float duration)
        {
            try
            {
                if (!target.Has<Health>() || target.Read<Health>().IsDead)
                    return;
                
                var targetPos = target.Read<LocalToWorld>().Position;
                
                // Apply stun with impact effect
                BuffCharacter(target, STUN_IMPACT);
                
                // Apply screen shake to nearby players
                ApplyScreenShakeNearby(targetPos, 15f, 0.5f, 0.3f);
                
                // Apply the actual stun buff
                CoroutineHandler.StartGenericCoroutine(() =>
                {
                    BuffCharacter(target, STUN_DEBUFF);
                }, 0.1f);
                
                Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied stun to entity for {duration}s with impact effects");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, "Failed to apply stun with impact", ex);
            }
        }
        
        
        private void ApplyScreenShakeNearby(float3 position, float radius, float intensity, float duration)
        {
            try
            {
                var radiusSq = radius * radius;
                var users = GameData.Users.Online.ToList();
                
                foreach (var user in users)
                {
                    if (user.Character.Entity.Has<LocalToWorld>())
                    {
                        var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                        if (math.distancesq(position, playerPos) <= radiusSq)
                        {
                            // Apply screen shake effect
                            // This would need proper implementation based on V Rising's camera system
                            Plugin.BLogger.Trace(LogCategory.Mechanic, $"Applied screen shake to player at distance {math.distance(position, playerPos)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to apply screen shake: {ex.Message}");
            }
        }

        private List<Entity> GetTargets(float3 bossPos, string targetType, float radius, int maxTargets)
        {
            var targets = new List<Entity>();
            var users = GameData.Users.Online.ToList();

            switch (targetType.ToLower())
            {
                case "all":
                    if (radius > 0)
                    {
                        var radiusSq = radius * radius;
                        foreach (var user in users)
                        {
                            if (user.Character.Entity.Has<LocalToWorld>())
                            {
                                var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                                if (math.distancesq(bossPos, pos) <= radiusSq)
                                {
                                    targets.Add(user.Character.Entity);
                                }
                            }
                        }
                    }
                    else
                    {
                        targets.AddRange(users.Select(u => u.Character.Entity));
                    }
                    break;

                case "nearest":
                    var nearestPlayer = GetNearestPlayer(bossPos, users);
                    if (nearestPlayer != Entity.Null)
                    {
                        targets.Add(nearestPlayer);
                    }
                    break;

                case "farthest":
                    var farthestPlayer = GetFarthestPlayer(bossPos, users);
                    if (farthestPlayer != Entity.Null)
                    {
                        targets.Add(farthestPlayer);
                    }
                    break;

                case "random":
                    var randomTargets = GetRandomPlayers(users, maxTargets);
                    targets.AddRange(randomTargets);
                    break;
            }

            // Limit to max targets
            if (targets.Count > maxTargets)
            {
                targets = targets.Take(maxTargets).ToList();
            }

            return targets;
        }

        private Entity GetNearestPlayer(float3 position, List<UserModel> users)
        {
            Entity nearest = Entity.Null;
            float nearestDist = float.MaxValue;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var dist = math.distance(position, pos);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = user.Character.Entity;
                    }
                }
            }

            return nearest;
        }

        private Entity GetFarthestPlayer(float3 position, List<UserModel> users)
        {
            Entity farthest = Entity.Null;
            float farthestDist = 0;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var dist = math.distance(position, pos);
                    if (dist > farthestDist)
                    {
                        farthestDist = dist;
                        farthest = user.Character.Entity;
                    }
                }
            }

            return farthest;
        }

        private List<Entity> GetRandomPlayers(List<UserModel> users, int count)
        {
            var random = new System.Random();
            return users.OrderBy(x => random.Next())
                       .Take(count)
                       .Select(u => u.Character.Entity)
                       .ToList();
        }

        private void BuffCharacter(Entity character, PrefabGUID buffGuid)
        {
            var des = new ApplyBuffDebugEvent()
            {
                BuffPrefabGUID = buffGuid,
            };

            var fromCharacter = new FromCharacter()
            {
                User = character,
                Character = character,
            };

            var debugSystem = Core.SystemsCore.DebugEventsSystem;
            debugSystem.ApplyBuff(fromCharacter, des);
        }
        
        private void RemoveBuff(Entity character, PrefabGUID buffGuid)
        {
            try
            {
                if (BuffUtility.TryGetBuff(Core.SystemsCore.EntityManager, character, buffGuid, out Entity buffEntity))
                {
                    DestroyUtility.Destroy(Core.SystemsCore.EntityManager, buffEntity);
                    Plugin.BLogger.Trace(LogCategory.Mechanic, $"Removed buff {buffGuid} from character");
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to remove buff: {ex.Message}");
            }
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var duration = GetParameter<float>(parameters, "duration", 3f);
            if (duration <= 0 || duration > 10)
                return false;

            var markDuration = GetParameter<float>(parameters, "mark_duration", 2.5f);
            if (markDuration <= 0 || markDuration > 10)
                return false;

            return true;
        }

        public string GetDescription()
        {
            return "Marks players with a psychic eye before stunning them";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<LocalToWorld>();
        }

        private T GetParameter<T>(Dictionary<string, object> parameters, string key, T defaultValue)
        {
            if (parameters.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
    }
}