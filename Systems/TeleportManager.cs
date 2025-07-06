using System;
using System.Collections.Generic;
using System.Linq;
using BloodyBoss.Configuration;
using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using Stunlock.Core;

namespace BloodyBoss.Systems
{
    internal static class TeleportManager
    {
        private static readonly Dictionary<string, DateTime> _lastTeleports = new();

        public static bool CanPlayerTeleport(string playerName, out string reason)
        {
            reason = string.Empty;

            // Verify if the command is enabled
            if (!PluginConfig.EnableTeleportCommand.Value)
            {
                reason = "Teleport command is disabled";
                return false;
            }

            // Verify cooldown
            if (PluginConfig.TeleportCooldown.Value > 0 && _lastTeleports.ContainsKey(playerName))
            {
                var timeSinceLastTeleport = DateTime.Now - _lastTeleports[playerName];
                var cooldownRemaining = PluginConfig.TeleportCooldown.Value - timeSinceLastTeleport.TotalSeconds;

                if (cooldownRemaining > 0)
                {
                    reason = $"Teleport on cooldown for {cooldownRemaining:F0} seconds";
                    return false;
                }
            }

            return true;
        }

        public static bool HasTeleportCost(UserModel player, out string costInfo)
        {
            costInfo = string.Empty;

            if (PluginConfig.TeleportCostItemGUID.Value == "0")
                return true; // No cost required

            try
            {
                var itemGUID = new PrefabGUID(int.Parse(PluginConfig.TeleportCostItemGUID.Value));
                var requiredAmount = PluginConfig.TeleportCostAmount.Value;

                // Check if player has the required items
                var hasEnoughItems = true; // Simplified for now - need to implement proper item checking
                costInfo = hasEnoughItems ? string.Empty : $"Requires {requiredAmount}x items";
                return hasEnoughItems;
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error checking teleport cost: {ex.Message}");
                costInfo = "Error checking teleport cost";
                return false;
            }
        }

        public static bool ConsumeTeleportCost(UserModel player)
        {
            if (PluginConfig.TeleportCostItemGUID.Value == "0")
                return true;

            try
            {
                var itemGUID = new PrefabGUID(int.Parse(PluginConfig.TeleportCostItemGUID.Value));
                var amount = PluginConfig.TeleportCostAmount.Value;

                // Simplified item removal - should be implemented with proper BloodyCore API
                return true; // For now, always succeed
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error consuming teleport cost: {ex.Message}");
                return false;
            }
        }

        public static void SetTeleportCooldown(string playerName)
        {
            _lastTeleports[playerName] = DateTime.Now;
        }

        public static Dictionary<string, DateTime> GetTeleportCooldowns()
        {
            return _lastTeleports.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static void ClearCooldown(string playerName)
        {
            _lastTeleports.Remove(playerName);
        }

        public static void ClearAllCooldowns()
        {
            _lastTeleports.Clear();
        }
    }
}