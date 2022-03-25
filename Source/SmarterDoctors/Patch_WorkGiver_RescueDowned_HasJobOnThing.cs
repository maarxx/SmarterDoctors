using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SmarterDoctors;

[HarmonyPatch(typeof(WorkGiver_RescueDowned))]
[HarmonyPatch("HasJobOnThing")]
internal class Patch_WorkGiver_RescueDowned_HasJobOnThing
{
    private static void Postfix(WorkGiver_RescueDowned __instance, ref Pawn pawn, ref Thing t, ref bool forced,
        ref bool __result)
    {
        // mostly the original method, repeated, with three commented changes

        if (__result)
        {
            return; //new guard clause
        }

        //if (pawn2 != null && pawn2.Downed && pawn2.Faction == pawn.Faction && !pawn2.InBed())
        if (t is Pawn { Downed: true } pawn2 &&
            pawn.Map.designationManager.DesignationOn(pawn2, DesignationDefOf.Tame) != null && !pawn2.InBed())
        {
            LocalTargetInfo target = pawn2;
            var ignoreOtherReservations = forced;
            if (pawn.CanReserve(target, 1, -1, null, ignoreOtherReservations) && !GenAI.EnemyIsNear(pawn2, 40f))
            {
                //Thing thing = FindBed(pawn, pawn2);
                Thing thing = RestUtility.FindBedFor(pawn2, pawn, pawn2.HostFaction == pawn.Faction);
                if (thing != null && pawn2.CanReserve(thing))
                {
                    __result = true;
                    return;
                }

                __result = false;
                return;
            }
        }

        __result = false;
    }
}