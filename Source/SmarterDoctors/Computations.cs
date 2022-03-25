using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace SmarterDoctors;

internal class Computations
{
    private static readonly Dictionary<string, float> trainmap = new Dictionary<string, float>
    {
        { "Tameness", 500f },
        { "Haul", 400f },
        { "Rescue", 300f },
        { "Release", 200f },
        { "Obedience", 100f }
    };

    public static float computeTendPriority(Pawn worker, Pawn target)
    {
        var isSick = target.health.hediffSet.HasImmunizableNotImmuneHediff();
        var ticksToBleedOut = HealthUtility.TicksUntilDeathDueToBloodLoss(target);
        float priority = 0;
        if (isSick)
        {
            var delta = FindMostSevereHediffDelta(target);
            priority = (float)Math.Pow(10, 20) + delta;
        }
        else if (ticksToBleedOut < int.MaxValue)
        {
            priority = (float)Math.Pow(10, 10) - ticksToBleedOut;
        }
        //Log.Message("Hello from computeTendPriority with pawn: " + target.Name.ToStringShort + ", priority: " + priority);

        return priority;
    }

    public static float computeFeedPriority(Pawn worker, Pawn target)
    {
        float priority = 0;
        var malnutrition = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition);
        if (target.needs.food.CurLevel == 0f && malnutrition != null)
        {
            priority = malnutrition.Severity;
        }

        return priority;
    }


    //https://github.com/Mehni/kNumbers/blob/fc9320b45bc0a8c41b2f2c67f68107fb89e84848/Numbers/PawnColumnWorkers/PawnColumnWorker_DiseaseProgression.cs#L152
    private static float FindMostSevereHediffDelta(Pawn pawn)
    {
        var tmplist =
            pawn.health.hediffSet.hediffs.Where(x => x.Visible && x is HediffWithComps && !x.FullyImmune())
                .Cast<HediffWithComps>();

        var delta = float.MinValue;
        HediffWithComps mostSevereHediff = null;

        foreach (var hediff in tmplist)
        {
            var hediffCompImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();

            if (hediffCompImmunizable == null)
            {
                continue;
            }

            if (!(hediffCompImmunizable.Immunity - hediff.Severity > delta))
            {
                continue;
            }

            delta = hediffCompImmunizable.Immunity - hediff.Severity;
            mostSevereHediff = hediff;
        }

        return delta;
    }

    public static float computeTrainPriority(Pawn worker, Pawn target)
    {
        var train = target.training;
        var nextTrain = train.NextTrainableToTrain();
        var dynMethod = train.GetType().GetMethod("GetSteps", BindingFlags.Instance | BindingFlags.NonPublic);
        if (dynMethod == null)
        {
            return 0;
        }

        var steps = (int)dynMethod.Invoke(train, new object[] { nextTrain });
        var priority = trainmap[nextTrain.defName];
        if (nextTrain.defName == "Tameness")
        {
            priority += 10 - steps;
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

        return 100f;
    }

    public static float computeGrowerPriority(Pawn p, TargetInfo t)
    {
        var myGrid = new FertilityGrid(t.Map);
        var priority = myGrid.FertilityAt(t.Cell);
        var zone = p.Map.zoneManager.ZoneAt(t.Cell);
        if (zone is Zone_Growing growing && growing.GetPlantDefToGrow().defName == "Plant_Haygrass")
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

        if (f.def.entityDefToBuild is TerrainDef)
        {
            return 50f;
        }

        return 100f;
    }

    public static float computeRescuePriority(Pawn p, Pawn t)
    {
        if (t.IsColonist)
        {
            return 50f;
        }

        if (t.AnimalOrWildMan())
        {
            return 25f;
        }

        return 0f;
    }
}