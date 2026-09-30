using CasualtiesExtra;
using HarmonyLib;
using UnityEngine;

namespace CasualtiesJiggle
{
    [HarmonyPatch]
    internal static class BodyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), "Start")]
        private static void Body_Start(Body __instance)
        {
            if (!JiggleConfig.Enabled.Value)
                return;
            if (__instance.GetComponent<JiggleBody>() == null)
                __instance.gameObject.AddComponent<JiggleBody>();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AnimBody), "Footstep")]
        private static void AnimBody_Footstep(AnimBody __instance)
        {
            Body body = __instance.body;
            if (body != null && body.standing && body.grounded)
                JiggleBody.ForBody(body)?.OnFootStep();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AnimBody), "FootstepForced")]
        private static void AnimBody_FootstepForced(AnimBody __instance)
        {
            AnimBody_Footstep(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), "Jump")]
        private static void Body_Jump(Body __instance)
        {
            JiggleBody.ForBody(__instance)?.OnJump();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), "Eat")]
        private static void Body_Eat(Body __instance, float weightGain)
        {
            JiggleBody.ForBody(__instance)?.OnEat(weightGain);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Body), "Update")]
        [HarmonyAfter("thesofteeveeboy.mods.CasualtiesExtra")]
        private static void Body_Update(Body __instance)
        {
            if (!JiggleConfig.NeutralizeStuckScale.Value)
                return;
            try
            {
                if (__instance.limbs == null || __instance.limbs.Length <= 2)
                    return;
                int stage = CasualtiesExtraApi.GetWeightStage(__instance);
                if (stage < 2 || !CasualtiesExtraApi.GetStuck(__instance))
                    return;
                Transform t1 = __instance.limbs[1].transform;
                Transform t2 = __instance.limbs[2].transform;
                float f1 = stage == 2 ? 0.9f : 1f;
                if (f1 != 1f)
                    t1.localScale = new Vector3(
                        t1.localScale.x / f1,
                        t1.localScale.y,
                        t1.localScale.z
                    );
                t2.localScale = new Vector3(
                    t2.localScale.x / 0.75f,
                    t2.localScale.y,
                    t2.localScale.z
                );
            }
            catch
            {
                // never let the undo kill the game's update
            }
        }
    }
}
