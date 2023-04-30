using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Il2CppVampireSurvivors.UI;
using Il2CppVampireSurvivors.Objects.Characters;

namespace Movement_Speed_Cap_Plus
{
    public static class ModInfo
    {
        public const string Name = "Move Speed Cap+";
        public const string Description = "Adds a configurable min, max, and added bonus to your Movement Speed";
        public const string Author = "Farukon";
        public const string Company = "";
        public const string Version = "0.1.0";
        public const string Download = "";
    }

    public class ConfigData
    {
        public bool Enabled { get; set; } = true;
        public ConfigSettingData MoveSpeedAdd { get; set; } = new ConfigSettingData("Increases or descreases the movement speed of all characters by a set amount. E.g. 0.11 adds 11% to your movespeed. (Opposite effect on negative.)", "Default = 0.0", 0.0f);
        public ConfigSettingData MinMoveSpeed { get; set; } = new ConfigSettingData("The minimum your movement speed can be. E.g. -0.55 means you'll always have more than -155% movement speed ingame.", "Default = -1.0", -1.0f);
        public ConfigSettingData MaxMoveSpeed { get; set; } = new ConfigSettingData("The maximum your movement speed can be. E.g. 1.58 you'll never have more than +58% movement speed ingame.", "Default = 3.0", 3.0f);
    }

    public class ConfigSettingData
    {
        public ConfigSettingData(string _description, string _default, float _value)
        {
            Description = _description;
            Default = _default;
            Value = _value;
        }
        public string Description { get; set; }
        public string Default { get; set; }
        public float Value { get; set; }
    }

    public class MoveSpeedCap : MelonMod
    {
        //Config Folder and File Paths
        static readonly string _ConfigFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Farukon's Mods");
        static readonly string _ConfigPath = Path.Combine(_ConfigFolderPath, "MoveSpeedPlusConfig.json");

        static readonly string EnabledKey = "Enabled";
        static readonly string MoveSpeedAddKey = "MoveSpeedAdd";
        static readonly string MinMoveSpeedKey = "MinMoveSpeed";
        static readonly string MaxMoveSpeedKey = "MaxMoveSpeed";
        static bool _Enabled;
        static float _MoveSpeedAdd;
        static float _MinMoveSpeed;
        static float _MaxMoveSpeed;

        static void UpdateToggleDebug(bool value) => UpdateEnabled(value);

        static bool SettingAdded = false;

        static System.Action<bool> SettingChanged = UpdateToggleDebug;

        //This *MAY* be pointless, impractical, and silly. But I don't care, it's fun.
        private static readonly string[] loadMessages ={
            "The Move Speed Cap+ Mod has been loaded!",
            "Move Speed Cap+ has loaded your boots with lead! Enjoy!",
            "Move Speed Cap+: The New Mod To Slow Your Day!",
            "Successfully Loaded Move Speed Cap+!",
            "Move Speed Cap+ is better than the rest! Until it's not.",
            "Move Speed has beed Capped+!",
            "Move Speed Cap+: Better Slows Than Quicksand!",
            "Move Speed Cap+: Fully Configurable!",
            "If Move Speed Cap+ isn't good enough, Do it yourself then.",
            "Too much movement? Try Move Speed Cap+!",
            "With Move Speed Cap+, you can finally talk to Trousers again!",
            "Avatar Infernas *may* be overpowered with Move Speed Cap+",
            "In MoveSpeedConfig.json, you can also set a minimum!\n (WARNING: If the minimum is changed, it HAS to be smaller than the max!)",
            "If you so choose, you can set the cap to a *negative!\n (*The negative value shown in game is shown when below 1, this will slow you to a halt at 0.)",
            "The point where your controls are inverted is at a negative for the config, below -100% ingame!",
            "REJECT SPEED, EMBRACE AVERAGE!!! Coutesy of Move Speed Cap+.",
            "'MoveSpeedCapPlus' Loaded",
            "You can also add base speed!",
            "You can also remove base speed!",
            "A Move Speed Capper shouldn't have this number of random load messages. \n[LMAO]But why not?",
            "!dedaol neeb sah doM +paC deepS evoM ehT"
        };

        public override void OnInitializeMelon()
        {
            System.Random random = new();
            int index = random.Next(loadMessages.Length);
            LoggerInstance.Msg($"{loadMessages[index]}");
            {
                ValidateConfig();
            }
        }

        [HarmonyPatch(typeof(CharacterController), "OnUpdate")]
        public class ModifierStatsMovespeed_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(CharacterController __instance)
            {
                if (_Enabled)
                {
                    float _MoveSpeed = __instance._playerStats._MoveSpeed_k__BackingField + _MoveSpeedAdd;
                    if (_MoveSpeed > _MaxMoveSpeed) _MoveSpeed = _MaxMoveSpeed;
                    if (_MoveSpeed < _MinMoveSpeed) _MoveSpeed = _MinMoveSpeed;
                    __instance._playerStats._MoveSpeed_k__BackingField = _MoveSpeed;
                }
            }
        }

        [HarmonyPatch(typeof(OptionsController), nameof(OptionsController.BuildGameplayPage))]
        static class PatchBuildGameplayPage
        {
            static void Postfix(OptionsController __instance)
            {
                if (!SettingAdded)
                {
                    __instance.AddTickBox("Move Speed Cap+", _Enabled, SettingChanged, false);
                }
                SettingAdded = true;
            }
        }

        [HarmonyPatch(typeof(OptionsController), nameof(OptionsController.AddVisibleJoysticks))]
        static class PatchAddVisibleJoysticks { static void Postfix() => SettingAdded = false; }
        private static void UpdateEnabled(bool value)
        {
            ModifyConfigValue(EnabledKey, value);
            _Enabled = value;
        }

        private static void ValidateConfig()
        {
            try
            {
                if (!Directory.Exists(_ConfigFolderPath)) Directory.CreateDirectory(_ConfigFolderPath);
                if (!File.Exists(_ConfigPath)) File.WriteAllText(_ConfigPath, JsonConvert.SerializeObject(new ConfigData { }, Formatting.Indented));

                LoadConfig();
            }
            catch (System.Exception ex) { MelonLogger.Msg($"Error: {ex}"); }
        }

        private static void ModifyConfigValue(string key, object value)
        {
            string file = File.ReadAllText(_ConfigPath);
            JObject json = JObject.Parse(file);

            if (!json.ContainsKey(key)) json.Add(key, JToken.FromObject(value));
            else json[key] = JToken.FromObject(value);

            string finalJson = JsonConvert.SerializeObject(json, Formatting.Indented);
            File.WriteAllText(_ConfigPath, finalJson);
        }
        private static void LoadConfig()
        {
            JObject configJson = JObject.Parse(File.ReadAllText(_ConfigPath) ?? "{}");
            _Enabled = configJson.Value<bool>(EnabledKey);
            _MoveSpeedAdd = configJson.Value<float>(MoveSpeedAddKey);
            _MinMoveSpeed = configJson.Value<float>(MinMoveSpeedKey);
            _MaxMoveSpeed = configJson.Value<float>(MaxMoveSpeedKey);
        }
    }
}
