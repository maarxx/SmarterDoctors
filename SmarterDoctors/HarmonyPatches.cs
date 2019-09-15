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
            //Log.Message("Hello from Harmony in scope: com.github.harmony.rimworld.maarx.smarterdoctors");
            var harmony = HarmonyInstance.Create("com.github.harmony.rimworld.maarx.smarterdoctors");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    class Computations
    {
        public static float computeTendPriority(Pawn worker, Pawn target)
        {
            bool isSick = target.health.hediffSet.HasImmunizableNotImmuneHediff();
            int ticksToBleedOut = HealthUtility.TicksUntilDeathDueToBloodLoss(target);
            float delta = float.MaxValue;
            float priority = 0;
            if (isSick)
            {
                delta = FindMostSevereHediffDelta(target);
                priority = ((float) Math.Pow(10, 20)) + delta;
            }
            else if (ticksToBleedOut < int.MaxValue)
            {
                priority = ((float)Math.Pow(10, 10)) - ticksToBleedOut;
            }
            //Log.Message("Hello from computeTendPriority with pawn: " + target.Name.ToStringShort + ", priority: " + priority);

            return priority;
        }

        public static float computeFeedPriority(Pawn worker, Pawn target)
        {
            float priority = 0;
            Hediff malnutrition = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition);
            if (target.needs.food.CurLevel == 0f && malnutrition != null)
            {
                priority = malnutrition.Severity;
            }
            return priority;
        }


        //https://github.com/Mehni/kNumbers/blob/fc9320b45bc0a8c41b2f2c67f68107fb89e84848/Numbers/PawnColumnWorkers/PawnColumnWorker_DiseaseProgression.cs#L152
        private static float FindMostSevereHediffDelta(Pawn pawn)
        {
            IEnumerable<HediffWithComps> tmplist =
                pawn.health.hediffSet.hediffs.Where(x => x.Visible && x is HediffWithComps && !x.FullyImmune()).Cast<HediffWithComps>();

            float delta = float.MinValue;
            HediffWithComps mostSevereHediff = null;

            foreach (HediffWithComps hediff in tmplist)
            {
                HediffComp_Immunizable hediffCompImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();

                if (hediffCompImmunizable == null)
                    continue;

                if (hediffCompImmunizable.Immunity - hediff.Severity > delta)
                {
                    delta = hediffCompImmunizable.Immunity - hediff.Severity;
                    mostSevereHediff = hediff;
                }
            }

            return delta;
        }

        static Dictionary<string, float> trainmap = new Dictionary<string, float>
        {
            { "Tameness", 500f },
            { "Haul", 400f },
            { "Rescue", 300f },
            { "Release", 200f },
            { "Obedience", 100f }
        };
        public static float computeTrainPriority(Pawn worker, Pawn target)
        {

            Pawn_TrainingTracker train = target.training;
            TrainableDef nextTrain = train.NextTrainableToTrain();
            MethodInfo dynMethod = train.GetType().GetMethod("GetSteps", BindingFlags.Instance | BindingFlags.NonPublic);
            int steps = (int) dynMethod.Invoke(train, new object[] { nextTrain });
            Log.Message("Hello from computeTrainPriority with pawn: " + target.Name.ToStringShort + ", " + nextTrain.defName + ", " + steps);
            float priority = trainmap[nextTrain.defName] + steps;
            return priority;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Scanner))]
    [HarmonyPatch("GetPriority")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(TargetInfo) })]
    class Patch_WorkGiver_Scanner_GetPriority
    {
        static void Postfix(WorkGiver_Scanner __instance, ref Pawn pawn, ref TargetInfo t, ref float __result)
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
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Scanner))]
    [HarmonyPatch("Prioritized", MethodType.Getter)]
    class Patch_WorkGiver_Scanner_Prioritized
    {
        static void Postfix(WorkGiver_Scanner __instance, ref bool __result)
        {
            if (__instance is WorkGiver_Tend || __instance is WorkGiver_FeedPatient || __instance is WorkGiver_Train)
            {
                __result = true;
            }
        }
    }
}
