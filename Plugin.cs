using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using ServerSync;
using UnityEngine;

namespace GimmeServerSync
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class GimmeServerSyncPlugin : BaseUnityPlugin
    {
        internal const string ModName = "GimmeServerSync";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "coemt";
        private const string ModGUID = $"{Author}.{ModName}";
        private static string ConfigFileName = $"{ModGUID}.cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource GimmeServerSyncLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public void Awake()
        {
            // Uncomment the line below to use the LocalizationManager for localizing your mod.
            // Make sure to populate the English.yml file in the translation folder with your keys to be localized and the values associated before uncommenting!.
            //Localizer.Load(); // Use this to initialize the LocalizationManager (for more information on LocalizationManager, see the LocalizationManager documentation https://github.com/blaxxun-boop/LocalizationManager#example-project).
            torch1 = config("2 - Fuel", "Wood Torch", 6f, "Max fuel", true);
            torch2 = config("2 - Fuel", "Blue Torch", 6f, "Max fuel", true);
            torch3 = config("2 - Fuel", "Green Torch", 6f, "Max fuel");
            torch4 = config("2 - Fuel", "Iron Torch", 6f, "Max fuel");
            torch5 = config("2 - Fuel", "Fire Pit", 10f, "Max fuel");
            torch6 = config("2 - Fuel", "Fire Pit Iron", 10f, "Max fuel");
            torch7 = config("2 - Fuel", "Fire Pit Hildir", 10f, "Max fuel");
            torch8 = config("2 - Fuel", "Fire Pit Haldor", 10f, "Max fuel");
            torch9 = config("2 - Fuel", "Hearth", 20f, "Max fuel");
            torch10 = config("2 - Fuel", "Bonfire", 10f, "Max fuel");
            torch11 = config("2 - Fuel", "Brazier floor", 5f, "Max fuel");
            torch12 = config("2 - Fuel", "Brazier floor blue", 5f, "Max fuel");
            torch13 = config("2 - Fuel", "Brazier Ceiling", 5f, "Max fuel");
            torch14 = config("2 - Fuel", "Wall Torch", 6f, "Max fuel");
            smelt1 = config("2 - Fuel", "Smelter Coal", 20, "Max fuel");
            smeltOre1 = config("2 - Fuel", "Smelter Ore", 10,"Max Ore");
            smelt2 = config("2 - Fuel", "Charcoal Wood", 20, "Max fuel");
            smelt3 = config("2 - Fuel", "Blast furnace Coal", 20, "Max fuel");
            smelt4 = config("2 - Fuel", "Armory Smelter", 40, "Max fuel");
            smeltOre2 = config("2 - Fuel", "Blastfurnace Ore", 10,  "Max Ore");
            smeltOre3 = config("2 - Fuel", "Armory Ore", 20,  "Max Ore");
            smeltOre5 = config("2 - Fuel", "Spinning Wheel", 40,  "Max Flax");
            smeltOre6 = config("2 - Fuel", "Windmill", 50,  "Max Barley");
            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On,
                "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);


            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                GimmeServerSyncLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                GimmeServerSyncLogger.LogError($"There was an issue loading your {ConfigFileName}");
                GimmeServerSyncLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        public static ConfigEntry<float> torch1 = null!;
        public static ConfigEntry<float> torch2 = null!;
        public static ConfigEntry<float> torch3 = null!;
        public static ConfigEntry<float> torch4 = null!;
        public static ConfigEntry<float> torch5 = null!;
        public static ConfigEntry<float> torch6 = null!;
        public static ConfigEntry<float> torch7 = null!;
        public static ConfigEntry<float> torch8 = null!;
        public static ConfigEntry<float> torch9 = null!;
        public static ConfigEntry<float> torch10 = null!;
        public static ConfigEntry<float> torch11 = null!;
        public static ConfigEntry<float> torch12 = null!;
        public static ConfigEntry<float> torch13 = null!;
        public static ConfigEntry<float> torch14 = null!;
        public static ConfigEntry<int> smelt1 = null!;
        public static ConfigEntry<int> smelt2 = null!;
        public static ConfigEntry<int> smelt3 = null!;
        public static ConfigEntry<int> smelt4 = null!;
      
        public static ConfigEntry<int> smeltOre1 = null!;
        public static ConfigEntry<int> smeltOre2 = null!;
        public static ConfigEntry<int> smeltOre3 = null!;
        public static ConfigEntry<int> smeltOre4 = null!;
        public static ConfigEntry<int> smeltOre5 = null!;
        public static ConfigEntry<int> smeltOre6 = null!;
        private static ConfigEntry<Toggle> _serverConfigLocked = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order = null!;
            [UsedImplicitly] public bool? Browsable = null!;
            [UsedImplicitly] public string? Category = null!;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer = null!;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                $"# Acceptable values: {string.Join(", ", UnityInput.Current.SupportedKeyCodes)}";
        }

        #endregion
    }

    public static class KeyboardExtensions
    {
        public static bool IsKeyDown(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKeyDown(shortcut.MainKey) &&
                   shortcut.Modifiers.All(Input.GetKey);
        }

        public static bool IsKeyHeld(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKey(shortcut.MainKey) &&
                   shortcut.Modifiers.All(Input.GetKey);
        }
    }
}