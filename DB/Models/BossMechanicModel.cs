using System;
using System.Collections.Generic;

namespace BloodyBoss.DB.Models
{
    public class BossMechanicModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; }
        public bool Enabled { get; set; } = true;
        public MechanicTriggerModel Trigger { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public bool HasTriggered { get; set; }
        public DateTime? LastTriggered { get; set; }
        public int TriggerCount { get; set; }
        
        // For one-time mechanics that have been used
        public bool IsExpired => Trigger?.OneTime == true && HasTriggered;
        
        // For time-based repeating mechanics
        public bool CanTriggerAgain()
        {
            if (!HasTriggered || !Trigger.OneTime)
                return true;
                
            if (Trigger.RepeatInterval > 0 && LastTriggered.HasValue)
            {
                var timeSinceLastTrigger = (DateTime.Now - LastTriggered.Value).TotalSeconds;
                return timeSinceLastTrigger >= Trigger.RepeatInterval;
            }
            
            return false;
        }
        
        public void MarkTriggered()
        {
            HasTriggered = true;
            LastTriggered = DateTime.Now;
            TriggerCount++;
        }
        
        public void Reset()
        {
            HasTriggered = false;
            LastTriggered = null;
            TriggerCount = 0;
        }
        
        public string GetDescription()
        {
            var desc = $"{Type} mechanic";
            
            if (Trigger != null)
            {
                switch (Trigger.Type)
                {
                    case "hp_threshold":
                        desc += $" @ {Trigger.Value}% HP";
                        break;
                    case "time":
                        desc += $" @ {Trigger.Value}s";
                        if (Trigger.RepeatInterval > 0)
                            desc += $" (repeats every {Trigger.RepeatInterval}s)";
                        break;
                    case "player_count":
                        desc += $" when {Trigger.Comparison} {Trigger.Value} players";
                        break;
                }
            }
            
            if (!Enabled)
                desc += " [DISABLED]";
                
            return desc;
        }
    }
    
    public class MechanicTriggerModel
    {
        public string Type { get; set; } // "hp_threshold", "time", "player_count", "add_death", "damage_taken"
        public float Value { get; set; }
        public string Comparison { get; set; } = "less_than"; // "less_than", "greater_than", "equals"
        public bool OneTime { get; set; } = true;
        public float RepeatInterval { get; set; } = 0;
        
        // For compound triggers
        public List<MechanicTriggerModel> CompoundConditions { get; set; }
        public string CompoundOperator { get; set; } = "AND"; // "AND", "OR"
        
        // Additional context
        public float TimeWindow { get; set; } = 0; // For damage_taken triggers
        public string AddName { get; set; } // For add_death triggers
        public int AddCount { get; set; } = 1; // For add_death triggers
        
        public bool EvaluateCondition(float currentValue)
        {
            return Comparison?.ToLower() switch
            {
                "less_than" or "lt" or "<" => currentValue < Value,
                "greater_than" or "gt" or ">" => currentValue > Value,
                "equals" or "eq" or "==" => Math.Abs(currentValue - Value) < 0.01f,
                "less_than_or_equals" or "lte" or "<=" => currentValue <= Value,
                "greater_than_or_equals" or "gte" or ">=" => currentValue >= Value,
                _ => false
            };
        }
    }
}