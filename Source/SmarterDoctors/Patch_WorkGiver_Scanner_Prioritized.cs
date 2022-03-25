using HarmonyLib;
using RimWorld;

namespace SmarterDoctors;

[HarmonyPatch(typeof(WorkGiver_Scanner))]
[HarmonyPatch("Prioritized", MethodType.Getter)]
internal class Patch_WorkGiver_Scanner_Prioritized
{
    private static void Postfix(WorkGiver_Scanner __instance, ref bool __result)
    {
        if (__instance is WorkGiver_Tend ||
            __instance is WorkGiver_FeedPatient ||
            __instance is WorkGiver_Train ||
            __instance is WorkGiver_PlantsCut ||
            __instance is WorkGiver_GrowerSow ||
            __instance is WorkGiver_GrowerHarvest ||
            __instance is WorkGiver_ConstructFinishFrames ||
            __instance is WorkGiver_RescueDowned)
        {
            __result = true;
        }
    }
}