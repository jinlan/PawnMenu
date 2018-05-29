using System;
using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace PawnMenu {
    [HarmonyPatch(typeof(JobGiver_PackFood), "TryGiveJob")]
    public class JobGiver_PackFood_TryGiveJob {
        static bool Prefix(Pawn pawn, ref Job __result) {
            if(pawn == null || pawn.Faction == null || !pawn.Faction.IsPlayer) {
                return true;
            }
            Comp_PawnMenu comp = pawn.GetComp<Comp_PawnMenu>();
            if(comp == null && !comp.activated()) {
                return true;
            }
            if(pawn.inventory == null) {
                return false;
            }
            ThingOwner<Thing> innerContainer = pawn.inventory.innerContainer;
            for(int i = 0; i < innerContainer.Count; i++) {
                Thing thing = innerContainer[i];
                if(thing.def.ingestible != null && thing.def.ingestible.nutrition > 0.3f && thing.def.ingestible.preferability >= FoodPreferability.MealAwful && pawn.RaceProps.CanEverEat(thing)) {
                    return false;
                }
            }
            if(pawn.Map.resourceCounter.TotalHumanEdibleNutrition < (float)pawn.Map.mapPawns.ColonistsSpawnedCount * 1.5f) {
                return false;
            }
            Thing thing2 = GenClosest.ClosestThing_Regionwise_ReachablePrioritized(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 20f, delegate (Thing t) {
                if(!comp.contains(t.def)) {
                    return false;
                }
                if(t.def.category != ThingCategory.Item || t.def.ingestible == null || t.def.ingestible.nutrition < 0.3f || t.IsForbidden(pawn) || t is Corpse || !pawn.CanReserve(t, 1, -1, null, false) || !t.IsSociallyProper(pawn) || !pawn.RaceProps.CanEverEat(t)) {
                    return false;
                }
                List<ThoughtDef> list = FoodUtility.ThoughtsFromIngesting(pawn, t, FoodUtility.GetFinalIngestibleDef(t, false));
                for(int j = 0; j < list.Count; j++) {
                    if(list[j].stages[0].baseMoodEffect < 0f) {
                        return false;
                    }
                }
                return true;
            }, (Thing x) => FoodUtility.FoodOptimality(pawn, x, FoodUtility.GetFinalIngestibleDef(x, false), 0f, false), 24, 30);
            if(thing2 == null) {
                return false;
            }
            __result = new Job(JobDefOf.TakeInventory, thing2) {
                count = 1
            };
            return false;
        }
    }
}