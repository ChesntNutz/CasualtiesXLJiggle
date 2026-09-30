using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CasualtiesJiggle
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("CasualtiesUnknown.exe")]
    [BepInDependency("thesofteeveeboy.mods.CasualtiesExtra")]
    public class JigglePlugin : BaseUnityPlugin
    {
        private const string PluginGuid = "casualtiesjiggle.wgaddon";
        private const string PluginName = "CasualtiesJiggle";
        private const string PluginVersion = "0.2.0";

        internal static ManualLogSource Log;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            JiggleConfig.BindAll(Config);

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll(typeof(BodyPatches));
            
            try
            {
                _harmony.PatchAll(typeof(BloatingNoisesPatches));
            }
            catch (Exception e)
            {
                Log.LogWarning($"[Rumble] BloatingNoises hooks failed to apply: {e.Message}");
            }

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
