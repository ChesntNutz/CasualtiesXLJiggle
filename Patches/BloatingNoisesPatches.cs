using CasualtiesExtra;
using HarmonyLib;

namespace CasualtiesJiggle
{
    [HarmonyPatch]
    internal static class BloatingNoisesPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BloatingNoises), nameof(BloatingNoises.Gurgle))]
        private static void Gurgle(float bloat, Body body, float __result)
        {
            if (body != null)
                JiggleBody.ForBody(body)?.OnGurgle(bloat, __result);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BloatingNoises), nameof(BloatingNoises.Burp))]
        private static void Burp(float bloat, Body body)
        {
            if (body != null)
                JiggleBody.ForBody(body)?.OnBurp(bloat);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BloatingNoises), nameof(BloatingNoises.Slosh))]
        private static void Slosh(float bloat, Body body)
        {
            if (body != null)
                JiggleBody.ForBody(body)?.OnSlosh(bloat);
        }
    }
}
