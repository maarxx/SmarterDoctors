using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace SmarterDoctors
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.github.harmony.rimworld.maarx.smarterdoctors");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    class Computations
    {
        public static float computePriority(Pawn p)
        {
            return 1f;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Tend))]
    [HarmonyPatch("GetPriority")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(TargetInfo) })]
    class ThingLabel
    {
        static void Postfix(WorkGiver_Tend __instance, ref Pawn pawn, ref TargetInfo t, ref float __result)
        {
            Log.Message("Hello from Harmony WorkGiver_Tend GetPriority Postfix with result:" + __result);
            if (__result == 0f)
            {
                __result = Computations.computePriority((Pawn) t.Thing);
            }
        }
    }
}
