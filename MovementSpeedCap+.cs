using HarmonyLib;
using MelonLoader;
using System;
using Il2CppVampireSurvivors.Framework.NumberTypes;
using Il2CppVampireSurvivors.Objects.Characters;
using UnityEngine;

namespace Movement_Speed_Cap_Plus
{
    public static class ModInfo
    {
        public const string Name = "Move Speed Cap+";
        public const string Description = "Adds a configurable min and max to your Movement Speed.";
        public const string Author = "Farukon";
        public const string Company = "";
        public const string Version = "1.1.1";
        public const string Download = "";
    }
    public class MoveSpeedCap : MelonMod
    {
        private static MelonPreferences_Category preferences;
        private static MelonPreferences_Entry<bool> enabled;
        private static MelonPreferences_Entry<bool> minEnabled;
        private static MelonPreferences_Entry<float> minMoveSpeed;
        private static MelonPreferences_Entry<bool> maxEnabled;
        private static MelonPreferences_Entry<float> maxMoveSpeed;

        //This *MAY* be pointless, impractical, and silly. But I don't care, it's fun.
        private static readonly string[] loadMessages ={
            "The Move Speed Cap+ Mod has been loaded!",
            "Move Speed Cap+ has loaded your boots with lead! Enjoy!",
            "Move Speed Cap+: The New Mod To Slow Your Day!",
            "Successfully Loaded Move Speed Cap+!",
            "Move Speed Cap+ is better than the rest! Until it's not.",
            "Move Speed has been Capped+!",
            "Move Speed Cap+: Better Slows Than Quicksand!",
            "Move Speed Cap+: Fully Configurable!",
            "If Move Speed Cap+ isn't good enough, Do it yourself then.",
            "Too much movement? Try Move Speed Cap+!",
            "With Move Speed Cap+, you can finally talk to Trousers again!",
            "Avatar Infernas *may* be overpowered with Move Speed Cap+",
            "In the MoveSpeedPlusConfig.json, you can also set a minimum!\n (WARNING: If the minimum is changed, it HAS to be smaller than the max!)",
            "If you so choose, you can set the cap to a *negative!\n (*The negative value shown in game is shown when below 1, this will slow you to a halt at 0.)",
            "The point where your controls are inverted is at a negative for the config, below -100% ingame!",
            "REJECT SPEED, EMBRACE AVERAGE!!! Coutesy of Move Speed Cap+.",
            "'MoveSpeedCapPlus' Loaded",
            "A Move Speed Capper shouldn't have this number of random load messages. \n[LMAO]But why not?",
            "A mod to add basic configurable stuff to Move Speed shouldn't have taken me a week to make.",
            "Thanks to Takacomic for making this mod work!",
            "Now you can disable the Min and Max in the Config!",
            "!dedaol neeb sah doM +paC deepS evoM ehT"
        };

        public override void OnInitializeMelon()
        {
            System.Random random = new System.Random(DateTime.Now.Millisecond);
            int index = random.Next(loadMessages.Length);
            Debug.Log(loadMessages[index]);

            preferences = MelonPreferences.CreateCategory("movementspeedcap_preferences");
            enabled = preferences.CreateEntry("enabled", true);
            minEnabled = preferences.CreateEntry("minEnabled", true);
            maxEnabled = preferences.CreateEntry("maxEnabled", true);
            minMoveSpeed = preferences.CreateEntry("minMoveSpeed", -1.0f);
            maxMoveSpeed = preferences.CreateEntry("maxMoveSpeed", 3.0f);
        }

        [HarmonyPatch(typeof(CharacterController), "OnUpdate")]
        public class ModifierStatsMovespeed_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(CharacterController __instance)
            {
                if (enabled.Value)
                {
                    EggFloat _MoveSpeed = __instance.PlayerStats._MoveSpeed_k__BackingField;
                    if (maxEnabled.Value)
                    {
                        if (_MoveSpeed > maxMoveSpeed.Value) _MoveSpeed = new EggFloat(maxMoveSpeed.Value);
                    }
                    if (minEnabled.Value)
                    {
                        if (_MoveSpeed < minMoveSpeed.Value) _MoveSpeed = new EggFloat(minMoveSpeed.Value);
                    }
                    __instance.PlayerStats._MoveSpeed_k__BackingField = _MoveSpeed;
                }
            }
        }
    }
}
