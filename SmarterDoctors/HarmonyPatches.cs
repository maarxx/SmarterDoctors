﻿using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SmarterDoctors
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            //Log.Message("Hello from Harmony in scope: com.github.harmony.rimworld.maarx.smarterdoctors");
            var harmony = new Harmony("com.github.harmony.rimworld.maarx.smarterdoctors");
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
            float priority = trainmap[nextTrain.defName];
            if (nextTrain.defName == "Tameness")
            {
                priority += (10 - steps);
            }
            else
            {
                priority += steps;
            }
            //Log.Message("Hello from computeTrainPriority with pawn: " + target.Name.ToStringShort + ", " + nextTrain.defName + ", Steps:" + steps + ", Priority" + priority);
            return priority;
        }

        public static float computePlantCutPriority(Pawn p, TargetInfo t)
        {
            if (t.HasThing && t.Thing.def.defName == "Plant_Haygrass")
            {
                return 80f;
            }
            else
            {
                return 100f;
            }
        }

        public static float computeGrowerPriority(Pawn p, TargetInfo t)
        {
            FertilityGrid myGrid = new FertilityGrid(t.Map);
            float priority = myGrid.FertilityAt(t.Cell);
            Zone zone = p.Map.zoneManager.ZoneAt(t.Cell);
            if (zone != null &&
                zone is Zone_Growing &&
                (zone as Zone_Growing).GetPlantDefToGrow().defName == "Plant_Haygrass")
            {
                priority -= 1;
            }
            return priority;
        }

        public static float computeConstructPriority(Pawn p, Frame f)
        {
            if (f.def.entityDefToBuild.defName == "TrapSpike")
            {
                return 25f;
            }
            else if(f.def.entityDefToBuild is TerrainDef)
            {
                return 50f;
            }
            else 
            {
                return 100f;
            }
            
        }

        public static float computeRescuePriority(Pawn p, Pawn t)
        {
            if (t.IsColonist)
            {
                return 50f;
            }
            else if (t.AnimalOrWildMan())
            {
                return 25f;
            }
            else
            {
                return 0f;
            }

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

    [HarmonyPatch(typeof(WorkGiver_Scanner))]
    [HarmonyPatch("Prioritized", MethodType.Getter)]
    class Patch_WorkGiver_Scanner_Prioritized
    {
        static void Postfix(WorkGiver_Scanner __instance, ref bool __result)
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

    [HarmonyPatch(typeof(WorkGiver_RescueDowned))]
    [HarmonyPatch("HasJobOnThing")]
    class Patch_WorkGiver_RescueDowned_HasJobOnThing
    {
        static void Postfix(WorkGiver_RescueDowned __instance, ref Pawn pawn, ref Thing t, ref bool forced, ref bool __result)
        {
            // mostly the original method, repeated, with three commented changes

            if (__result == true) return; //new guard clause

            Pawn pawn2 = t as Pawn;
          //if (pawn2 == null || !pawn2.Downed || pawn2.Faction != pawn.Faction                                                   || pawn2.InBed() || pawn2.IsCharging() || !pawn.CanReserve(pawn2, 1, -1, null, forced) || GenAI.EnemyIsNear(pawn2, 40f) || CaravanFormingUtility.IsFormingCaravanOrDownedPawnToBeTakenByCaravan(pawn2))
            if (pawn2 == null || !pawn2.Downed || pawn.Map.designationManager.DesignationOn(pawn2, DesignationDefOf.Tame) != null || pawn2.InBed() || pawn2.IsCharging() || !pawn.CanReserve(pawn2, 1, -1, null, forced) || GenAI.EnemyIsNear(pawn2, 40f) || CaravanFormingUtility.IsFormingCaravanOrDownedPawnToBeTakenByCaravan(pawn2))
            {
                __result = false;
                return;
            }
            Thing thing = null;
            if (ChildcareUtility.CanSuckle(pawn2, out ChildcareUtility.BreastfeedFailReason? _))
            {
                if (!HealthAIUtility.ShouldSeekMedicalRest(pawn2))
                {
                    __result = false;
                    return;
                }
                Building_Bed building_Bed;
                if ((building_Bed = (ChildcareUtility.SafePlaceForBaby(pawn2, pawn).Thing as Building_Bed)) != null)
                {
                    thing = building_Bed;
                }
            }
            else
            {
                //thing = FindBed(pawn, pawn2);
                thing = RestUtility.FindBedFor(pawn2, pawn, false, false, pawn2.GuestStatus);
            }
            if (thing != null && pawn2.CanReserve(thing))
            {
                __result = true;
                return;
            }
            __result = false;
            return;
        }
    }
}
