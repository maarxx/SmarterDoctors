using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SmarterDoctors;

internal class Patch_JobDriver_Repair_MakeNewToils
{
    // I wanted them to repair forbidden doors, but can't get it working.
    private static bool Prefix(JobDriver_Repair __instance, ref IEnumerable<Toil> __result)
    {
        var new_result = new List<Toil>();

        var bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        var field_ticksToNextRepair = typeof(JobDriver_Repair).GetField("ticksToNextRepair", bindFlags);

        var field_baseTargetThingA = typeof(JobDriver_Repair).GetProperty("TargetThingA", bindFlags);
        var new_baseTargetThingA = (Thing)field_baseTargetThingA?.GetValue(__instance);

        var field_baseMap = typeof(JobDriver_Repair).GetProperty("Map", bindFlags);
        var new_baseMap = (Map)field_baseMap?.GetValue(__instance);


        //new_this.FailOnDespawnedOrNull(TargetIndex.A);
        new_result.Add(Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch));
        var repair = new Toil
        {
            initAction = delegate
            {
                //ticksToNextRepair = 80f;
                field_ticksToNextRepair?.SetValue(__instance, 80f);
            }
        };
        repair.tickAction = delegate
        {
            var actor = repair.actor;
            actor.skills.Learn(SkillDefOf.Construction, 0.05f);
            var num = actor.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f;
            //ticksToNextRepair -= num;
            field_ticksToNextRepair?.SetValue(__instance, (float)field_ticksToNextRepair.GetValue(__instance) - num);

            if (field_ticksToNextRepair != null && !((float)field_ticksToNextRepair.GetValue(__instance) <= 0f))
            {
                return;
            }

            //ticksToNextRepair += 20f;
            field_ticksToNextRepair?.SetValue(__instance, (float)field_ticksToNextRepair.GetValue(__instance) + 20f);

            if (new_baseTargetThingA == null)
            {
                return;
            }

            new_baseTargetThingA.HitPoints++;
            new_baseTargetThingA.HitPoints =
                Mathf.Min(new_baseTargetThingA.HitPoints, new_baseTargetThingA.MaxHitPoints);
            new_baseMap?.listerBuildingsRepairable.Notify_BuildingRepaired((Building)new_baseTargetThingA);
            if (new_baseTargetThingA.HitPoints != new_baseTargetThingA.MaxHitPoints)
            {
                return;
            }

            actor.records.Increment(RecordDefOf.ThingsRepaired);
            actor.jobs.EndCurrentJob(JobCondition.Succeeded);
        };
        repair.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        repair.WithEffect(new_baseTargetThingA?.def.repairEffect, TargetIndex.A);
        repair.defaultCompleteMode = ToilCompleteMode.Never;
        repair.activeSkill = () => SkillDefOf.Construction;
        new_result.Add(repair);
        __result = new_result.AsEnumerable();
        return false;
    }
}