using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;

namespace BloodyBoss.Systems;

public enum LogLevel
{
    None = 0,
    Error = 1,
    Warning = 2,
    Info = 3,
    Debug = 4,
    Trace = 5
}

public enum LogCategory
{
    System,
    Boss,
    Damage,
    Mechanic,
    Command,
    Database,
    Hook,
    Timer,
    Spawn,
    Death,
    Reward,
    Debug
}

public class BloodyLogger
{
    private readonly ManualLogSource _logger;
    private LogLevel _globalLevel = LogLevel.Info;
    private readonly Dictionary<LogCategory, LogLevel> _categoryLevels = new();
    private readonly HashSet<LogCategory> _enabledCategories = new();
    
    public BloodyLogger(ManualLogSource logger)
    {
        _logger = logger;
        
        // Enable all categories by default
        foreach (LogCategory category in Enum.GetValues(typeof(LogCategory)))
        {
            _enabledCategories.Add(category);
            _categoryLevels[category] = LogLevel.Info;
        }
    }
    
    public void Configure(LogLevel globalLevel, Dictionary<string, LogLevel> categoryLevels, HashSet<string> disabledCategories)
    {
        _globalLevel = globalLevel;
        
        // Reset all categories to global level
        foreach (LogCategory category in Enum.GetValues(typeof(LogCategory)))
        {
            _categoryLevels[category] = globalLevel;
        }
        
        // Apply specific category levels
        foreach (var kvp in categoryLevels)
        {
            if (Enum.TryParse<LogCategory>(kvp.Key, true, out var category))
            {
                _categoryLevels[category] = kvp.Value;
            }
        }
        
        // Handle disabled categories
        _enabledCategories.Clear();
        foreach (LogCategory category in Enum.GetValues(typeof(LogCategory)))
        {
            if (!disabledCategories.Contains(category.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                _enabledCategories.Add(category);
            }
        }
    }
    
    public void Trace(LogCategory category, string message)
    {
        Log(category, LogLevel.Trace, message);
    }
    
    public void Debug(LogCategory category, string message)
    {
        Log(category, LogLevel.Debug, message);
    }
    
    public void Info(LogCategory category, string message)
    {
        Log(category, LogLevel.Info, message);
    }
    
    public void Warning(LogCategory category, string message)
    {
        Log(category, LogLevel.Warning, message);
    }
    
    public void Error(LogCategory category, string message)
    {
        Log(category, LogLevel.Error, message);
    }
    
    public void Error(LogCategory category, string message, Exception ex)
    {
        Log(category, LogLevel.Error, $"{message}: {ex.Message}\n{ex.StackTrace}");
    }
    
    private void Log(LogCategory category, LogLevel level, string message)
    {
        // Check if category is enabled
        if (!_enabledCategories.Contains(category))
            return;
            
        // Check category-specific level
        var categoryLevel = _categoryLevels.GetValueOrDefault(category, _globalLevel);
        if (level > categoryLevel)
            return;
            
        // Format message with category
        var formattedMessage = $"[{category}] {message}";
        
        // Log based on level
        switch (level)
        {
            case LogLevel.Error:
                _logger.LogError(formattedMessage);
                break;
            case LogLevel.Warning:
                _logger.LogWarning(formattedMessage);
                break;
            case LogLevel.Info:
                _logger.LogInfo(formattedMessage);
                break;
            case LogLevel.Debug:
            case LogLevel.Trace:
                _logger.LogDebug(formattedMessage);
                break;
        }
    }
    
    // Helper method to quickly disable/enable categories at runtime
    public void SetCategoryEnabled(LogCategory category, bool enabled)
    {
        if (enabled)
            _enabledCategories.Add(category);
        else
            _enabledCategories.Remove(category);
    }
    
    // Helper method to change log level at runtime
    public void SetLogLevel(LogLevel level)
    {
        _globalLevel = level;
        foreach (var category in _categoryLevels.Keys.ToList())
        {
            _categoryLevels[category] = level;
        }
    }
    
    // Helper method to change category log level at runtime
    public void SetCategoryLevel(LogCategory category, LogLevel level)
    {
        _categoryLevels[category] = level;
    }
}