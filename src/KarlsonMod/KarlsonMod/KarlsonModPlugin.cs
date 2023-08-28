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
        public static string NoclipKey = "Noclip";
        public static string NativeUIKeyCodeKey = "Shortcut for the native UI of the mod";
        public static string ToggleCursorLockKeyCodeKey = "Shortcut toggling the mouse lock";
        public static string ToggleNoclipKeyCodeKey = "Shortcut for toggling noclip";
        public static string SpawnEnemyKeyCodeKey = "Shortcut for spawning an enemy";

        // Configuration entries. Static, so can be accessed directly elsewhere in code via
        // e.g.
        // float myFloat = KarlsonModPlugin.FloatExample.Value;
        // TODO Change this code or remove the code if not required.
        public static ConfigEntry<float> JumpingForceEntry;
        public static ConfigEntry<float> SlideSlowDownEntry;
        public static ConfigEntry<float> MoveSpeedEntry;
        public static ConfigEntry<bool> ExplosiveBulletsEntry;
        public static ConfigEntry<bool> GodmodeEntry;
        public static ConfigEntry<bool> NoclipEntry;
        public static ConfigEntry<KeyboardShortcut> NativeUIKeyCodeEntry;
        public static ConfigEntry<KeyboardShortcut> ToggleCursorLockKeyCodeEntry;
        public static ConfigEntry<KeyboardShortcut> ToggleNoclipKeyCodeEntry;
        public static ConfigEntry<KeyboardShortcut> SpawnEnemyKeyCodeEntry;

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        private static Rect windowRect = new Rect(20, 20, 350, 620);
        public static ManualLogSource Log = new ManualLogSource(PluginName);
        public static bool menuEnabled;

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
                windowRect = GUI.Window(0, windowRect, UI.DoMyWindow, "Karlson Mod");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(NativeUIKeyCodeEntry.Value.MainKey))
            {
                ToggleMenu(true);
            }
            
            if (Input.GetKeyDown(ToggleCursorLockKeyCodeEntry.Value.MainKey))
            {
                ToggleMenu(true, true);
            }            
            
            if (Input.GetKeyDown(SpawnEnemyKeyCodeEntry.Value.MainKey))
            {
                PlayerPatches.SpawnEnemy();
            }            
            
            if (Input.GetKeyDown(ToggleNoclipKeyCodeEntry.Value.MainKey))
            {
                NoclipEntry.Value = !NoclipEntry.Value;
            }
        }

        public static void ToggleMenu(bool enable, bool mouseOnly = false)
        {
            if(enable && menuEnabled)
                enable = !enable;

            if (!mouseOnly) { menuEnabled = enable; }

            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enable;
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

            NoclipEntry = Config.Bind("General",
                NoclipKey,
                PlayerPatches.noclip,
                    new ConfigDescription("Toggles Noclip"));

            NoclipEntry.SettingChanged += ConfigSettingChanged;

            NativeUIKeyCodeEntry = Config.Bind("General",
                NativeUIKeyCodeKey,
                new KeyboardShortcut(KeyCode.F2));

            NativeUIKeyCodeEntry.SettingChanged += ConfigSettingChanged;

            ToggleCursorLockKeyCodeEntry = Config.Bind("General",
                ToggleCursorLockKeyCodeKey,
                    new KeyboardShortcut(KeyCode.F3));

            ToggleCursorLockKeyCodeEntry.SettingChanged += ConfigSettingChanged;    
            
            ToggleNoclipKeyCodeEntry = Config.Bind("General",
                ToggleNoclipKeyCodeKey,
                    new KeyboardShortcut(KeyCode.F5));

            ToggleNoclipKeyCodeEntry.SettingChanged += ConfigSettingChanged;        
            
            SpawnEnemyKeyCodeEntry = Config.Bind("General",
                SpawnEnemyKeyCodeKey,
                    new KeyboardShortcut(KeyCode.F4));

            SpawnEnemyKeyCodeEntry.SettingChanged += ConfigSettingChanged;
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
            
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == NoclipKey)
            {
                PlayerPatches.noclip = NoclipEntry.Value;
            }                 
            
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == ToggleCursorLockKeyCodeKey)
            {
                ToggleCursorLockKeyCodeEntry.Value = (KeyboardShortcut)settingChangedEventArgs.ChangedSetting.BoxedValue;
            }               
            
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == ToggleNoclipKeyCodeKey)
            {
                ToggleNoclipKeyCodeEntry.Value = (KeyboardShortcut)settingChangedEventArgs.ChangedSetting.BoxedValue;
            }      
            
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == NativeUIKeyCodeKey)
            {
                NativeUIKeyCodeEntry.Value = (KeyboardShortcut)settingChangedEventArgs.ChangedSetting.BoxedValue;
            }               
            
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == SpawnEnemyKeyCodeKey)
            {
                SpawnEnemyKeyCodeEntry.Value = (KeyboardShortcut)settingChangedEventArgs.ChangedSetting.BoxedValue;
            }  
        }
    }
}
