using System.Collections.Generic;
using Unity.Entities;

namespace BloodyBoss.Systems
{
    /// <summary>
    /// Base interface for all boss mechanics
    /// </summary>
    public interface IMechanic
    {
        /// <summary>
        /// Unique identifier for this mechanic type
        /// </summary>
        string Type { get; }
        
        /// <summary>
        /// Execute the mechanic on the boss entity
        /// </summary>
        void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world);
        
        /// <summary>
        /// Validate that the parameters are correct for this mechanic
        /// </summary>
        bool Validate(Dictionary<string, object> parameters);
        
        /// <summary>
        /// Get a human-readable description of this mechanic
        /// </summary>
        string GetDescription();
        
        /// <summary>
        /// Check if this mechanic can be applied to the given entity
        /// </summary>
        bool CanApply(Entity bossEntity);
    }
}