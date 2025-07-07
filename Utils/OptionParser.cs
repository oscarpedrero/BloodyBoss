using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BloodyBoss.Utils
{
    /// <summary>
    /// Parser for command-line style options (--option value)
    /// </summary>
    public class OptionParser
    {
        private readonly Dictionary<string, string> _options;
        private readonly List<string> _positionalArgs;

        public OptionParser(string[] args)
        {
            _options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _positionalArgs = new List<string>();
            
            ParseArguments(args);
        }

        private void ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    var key = args[i].Substring(2);
                    
                    // Check if next arg is a value or another option
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        _options[key] = args[i + 1];
                        i++; // Skip next arg as it's the value
                    }
                    else
                    {
                        _options[key] = "true"; // Boolean flag
                    }
                }
                else if (args[i].StartsWith("-") && args[i].Length == 2)
                {
                    // Support single dash shortcuts like -h for --hp
                    var key = args[i].Substring(1);
                    
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        _options[key] = args[i + 1];
                        i++;
                    }
                    else
                    {
                        _options[key] = "true";
                    }
                }
                else
                {
                    // Positional argument
                    _positionalArgs.Add(args[i]);
                }
            }
        }

        // Get methods with defaults
        public float GetFloat(string key, float defaultValue = 0f)
        {
            if (TryGetValue(key, out var value) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (TryGetValue(key, out var value) && int.TryParse(value, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            if (TryGetValue(key, out var value))
            {
                if (string.IsNullOrEmpty(value) || value.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
                    return false;
                if (int.TryParse(value, out var intValue))
                    return intValue != 0;
            }
            return defaultValue;
        }

        public string GetString(string key, string defaultValue = "")
        {
            return TryGetValue(key, out var value) ? value : defaultValue;
        }

        public T GetEnum<T>(string key, T defaultValue) where T : struct, Enum
        {
            if (TryGetValue(key, out var value) && Enum.TryParse<T>(value, true, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        // Check methods
        public bool HasOption(string key)
        {
            return ContainsKey(key);
        }

        public bool HasFlag(string key)
        {
            return HasOption(key);
        }

        // Get all options
        public Dictionary<string, string> GetAllOptions()
        {
            return new Dictionary<string, string>(_options);
        }

        public List<string> GetPositionalArgs()
        {
            return new List<string>(_positionalArgs);
        }

        // Internal helpers
        private bool TryGetValue(string key, out string value)
        {
            // Try exact match first
            if (_options.TryGetValue(key, out value))
                return true;

            // Try with common aliases
            var aliases = GetAliases(key);
            foreach (var alias in aliases)
            {
                if (_options.TryGetValue(alias, out value))
                    return true;
            }

            value = null;
            return false;
        }

        private bool ContainsKey(string key)
        {
            if (_options.ContainsKey(key))
                return true;

            var aliases = GetAliases(key);
            foreach (var alias in aliases)
            {
                if (_options.ContainsKey(alias))
                    return true;
            }

            return false;
        }

        private string[] GetAliases(string key)
        {
            // Common aliases for mechanic options
            return key.ToLower() switch
            {
                "hp" => new[] { "health", "hp-threshold", "threshold" },
                "damage" => new[] { "dmg", "damage-multiplier" },
                "movement" => new[] { "move", "movement-speed", "speed" },
                "attack-speed" => new[] { "as", "attack", "atk-speed" },
                "cast-speed" => new[] { "cs", "cast", "casting-speed" },
                "cooldown-reduction" => new[] { "cdr", "cooldown", "cd" },
                _ => Array.Empty<string>()
            };
        }

        // Validation
        public void RequireOptions(params string[] requiredOptions)
        {
            var missing = new List<string>();
            foreach (var option in requiredOptions)
            {
                if (!HasOption(option))
                {
                    missing.Add(option);
                }
            }

            if (missing.Count > 0)
            {
                throw new ArgumentException($"Missing required options: {string.Join(", ", missing.Select(m => $"--{m}"))}");
            }
        }

        // Pretty print for debugging
        public override string ToString()
        {
            var parts = new List<string>();
            
            if (_positionalArgs.Count > 0)
                parts.Add($"Positional: [{string.Join(", ", _positionalArgs)}]");
            
            if (_options.Count > 0)
            {
                var opts = _options.Select(kvp => $"--{kvp.Key}={kvp.Value}");
                parts.Add($"Options: {string.Join(" ", opts)}");
            }

            return string.Join(" | ", parts);
        }
    }
}