using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ConfigurationManager;
using HarmonyLib;
using KarlsonMod.Patches;
using UnityEngine;

namespace KarlsonMod
{
    // TODO Review this file and update to your own requirements.

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class KarlsonModPlugin : BaseUnityPlugin
    {
        // Mod specific details. MyGUID should be unique, and follow the reverse domain pattern
        // e.g.
        // com.mynameororg.pluginname
        // Version should be a valid version string.
        // e.g.
        // 1.0.0
        public const string MyGUID = "com.glumboi.KarlsonMod";
        public const string PluginName = "KarlsonMod";
        public const string VersionString = "1.0.0";

        // Config entry key strings
        // These will appear in the config file created by BepInEx and can also be used
        // by the OnSettingsChange event to determine which setting has changed.
        public static string JumpingForceKey = "Jumping Force";
        public static string SlideSlowDownKey = "Slide SlowDown";
        public static string MoveSpeedKey = "Moving speed";
        public static string ExplosiveBulletsKey = "Explosive Bullets";
        public static string GodmodeKey = "Godmode";
        public static string NativeUICodeKey = "Shortcut for the native UI of the mod";

        // Configuration entries. Static, so can be accessed directly elsewhere in code via
        // e.g.
        // float myFloat = KarlsonModPlugin.FloatExample.Value;
        // TODO Change this code or remove the code if not required.
        public static ConfigEntry<float> JumpingForceEntry;
        public static ConfigEntry<float> SlideSlowDownEntry;
        public static ConfigEntry<float> MoveSpeedEntry;
        public static ConfigEntry<bool> ExplosiveBulletsEntry;
        public static ConfigEntry<bool> GodmodeEntry;
        public static ConfigEntry<KeyboardShortcut> NativeUICodeEntry;

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        bool menuEnabled;

        private void Awake()
        {
            LoadConfig();
            Config.Reload();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        void OnGUI()
        {
            if (menuEnabled)
            {
                UI.CustomMenu();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(NativeUICodeEntry.Value.MainKey))
            {
                if(menuEnabled)
                {
                    menuEnabled = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    
                }
                else
                {
                    menuEnabled = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                
            }
        }

        void LoadConfig()
        {
            JumpingForceEntry = Config.Bind("General",    // The section under which the option is shown
                JumpingForceKey,                            // The key of the configuration option
                PlayerPatches.newJumpingForce,                            // The default value
                new ConfigDescription("Changes the jumping strength",         // Description that appears in Configuration Manager
                    new AcceptableValueRange<float>(550f, 5000)));     // Acceptable range, enabled slider and validation in Configuration Manager

            JumpingForceEntry.SettingChanged += ConfigSettingChanged;

            SlideSlowDownEntry = Config.Bind("General",    
                SlideSlowDownKey,                            
                PlayerPatches.newSlideSlowdown,                            
                new ConfigDescription("Changes the slide slowdown strength",         
                    new AcceptableValueRange<float>(0.0f, 0.2f)));     

            JumpingForceEntry.SettingChanged += ConfigSettingChanged;

            MoveSpeedEntry = Config.Bind("General",    
                MoveSpeedKey,                            
                PlayerPatches.newMoveSpeed,                           
                new ConfigDescription("Changes the slide slowdown strength",        
                    new AcceptableValueRange<float>(20f, 100f)));     

            MoveSpeedEntry.SettingChanged += ConfigSettingChanged;

            ExplosiveBulletsEntry = Config.Bind("General",    
                ExplosiveBulletsKey,                           
                BulletPatches.explosiveBullets,                            
                    new ConfigDescription("Toggles explosions on all Bullets (this makes enemy bullets explode as well)"));    

            ExplosiveBulletsEntry.SettingChanged += ConfigSettingChanged;

            GodmodeEntry = Config.Bind("General",
                GodmodeKey,
                PlayerPatches.godMode,
                    new ConfigDescription("Toggles Godmode"));

            GodmodeEntry.SettingChanged += ConfigSettingChanged;

            NativeUICodeEntry = Config.Bind("General",
                NativeUICodeKey,
                new KeyboardShortcut(KeyCode.F2));

            NativeUICodeEntry.SettingChanged += ConfigSettingChanged;
        }

        private void ConfigSettingChanged(object sender, System.EventArgs e)
        {
            SettingChangedEventArgs settingChangedEventArgs = e as SettingChangedEventArgs;

            // Check if null and return
            if (settingChangedEventArgs == null)
            {
                return;
            }

            // Float Shortcut setting changed handler
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == JumpingForceKey)
            {
                PlayerPatches.newJumpingForce = JumpingForceEntry.Value;
            }
            
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == SlideSlowDownKey)
            {
                PlayerPatches.newSlideSlowdown = SlideSlowDownEntry.Value;
            }

            if (settingChangedEventArgs.ChangedSetting.Definition.Key == MoveSpeedKey)
            {
                PlayerPatches.newMoveSpeed = MoveSpeedEntry.Value;
            }

            if (settingChangedEventArgs.ChangedSetting.Definition.Key == ExplosiveBulletsKey)
            {
                BulletPatches.explosiveBullets = ExplosiveBulletsEntry.Value;
            }    
            
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == GodmodeKey)
            {
                PlayerPatches.godMode = GodmodeEntry.Value;
            }                             
        }
    }
}
