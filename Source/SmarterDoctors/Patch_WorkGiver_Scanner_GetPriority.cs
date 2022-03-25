using HarmonyLib;
using RimWorld;
using Verse;

namespace SmarterDoctors;

[HarmonyPatch(typeof(WorkGiver_Scanner))]
[HarmonyPatch("GetPriority")]
[HarmonyPatch(new[] { typeof(Pawn), typeof(TargetInfo) })]
internal class Patch_WorkGiver_Scanner_GetPriority
{
    private static void Postfix(WorkGiver_Scanner __instance, ref Pawn pawn, ref TargetInfo t, ref float __result)
    {
        if (__instance is WorkGiver_Tend)
        {
            if (__result == 0f)
            {
                __result = Computations.computeTendPriority(pawn, (Pawn)t.Thing);
            }
        }
        else if (__instance is WorkGiver_FeedPatient)
        {
            if (__result == 0f)
            {
                __result = Computations.computeFeedPriority(pawn, (Pawn)t.Thing);
            }
        }
        else if (__instance is WorkGiver_Train)
        {
            if (__result == 0f)
            {
                __result = Computations.computeTrainPriority(pawn, (Pawn)t.Thing);
            }
        }
        else if (__instance is WorkGiver_PlantsCut)
        {
            if (__result == 0f)
            {
                __result = Computations.computePlantCutPriority(pawn, t);
            }
        }
        else if (__instance is WorkGiver_GrowerSow)
        {
            if (__result == 0f)
            {
                __result = Computations.computeGrowerPriority(pawn, t);
            }
        }
        else if (__instance is WorkGiver_GrowerHarvest)
        {
            if (__result == 0f)
            {
                __result = Computations.computeGrowerPriority(pawn, t);
            }
        }
        else if (__instance is WorkGiver_ConstructFinishFrames)
        {
            if (__result == 0f)
            {
                __result = Computations.computeConstructPriority(pawn, (Frame)t);
            }
        }
        else if (__instance is WorkGiver_RescueDowned)
        {
            if (__result == 0f)
            {
                __result = Computations.computeRescuePriority(pawn, (Pawn)t);
            }
        }
    }
}