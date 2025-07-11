using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.Methods;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Exceptions;
using BloodyBoss.Configuration;
using BloodyBoss.Systems;
using System;
using Unity.Transforms;
using Unity.Mathematics;
using VampireCommandFramework;

namespace BloodyBoss.Command
{
    [CommandGroup("bb location", "Boss location management commands")]
    public static class BossLocationCommand
    {
        [Command("set", usage: "<BossName>", description: "Set boss location to your current position", adminOnly: true)]
        public static void SetLocation(ChatCommandContext ctx, string bossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                var pos = Plugin.SystemsCore.EntityManager.GetComponentData<LocalToWorld>(user).Position;
                
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    boss.SetLocation(pos);
                    ctx.Reply($"✅ Position {pos.x:F1}, {pos.y:F1}, {pos.z:F1} set for boss '{bossName}'");
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
                
                // Verify if the boss has a location
                if (boss.x == 0 && boss.y == 0 && boss.z == 0)
                {
                    throw ctx.Error($"Boss '{bossName}' does not have a location set");
                }
                
                // Teleport the player
                userModel.TeleportTo(new float3(boss.x, boss.y, boss.z));
                
                ctx.Reply($"📍 Teleporting to boss '{bossName}' at {boss.x:F1}, {boss.y:F1}, {boss.z:F1}");
            }
            catch (Exception e)
            {
                Plugin.BLogger.Error(LogCategory.Command, $"Error in teleport command: {e}");
                throw;
            }
        }

        [Command("show", usage: "<BossName>", description: "Show boss location", adminOnly: true)]
        public static void ShowLocation(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (!Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    throw ctx.Error($"Boss '{bossName}' does not exist");
                }
                
                if (boss.x == 0 && boss.y == 0 && boss.z == 0)
                {
                    ctx.Reply($"⚠️ Boss '{bossName}' does not have a location set");
                }
                else
                {
                    ctx.Reply($"📍 Boss '{bossName}' location: {boss.x:F1}, {boss.y:F1}, {boss.z:F1}");
                }
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }
    }
}