using System;
using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace PawnMenu {
    [HarmonyPatch(typeof(FoodUtility), "BestFoodInInventory")]
    public class FoodUtility_BestFoodInInventory {
        static bool Prefix(ref Thing __result, Pawn holder, Pawn eater = null, FoodPreferability minFoodPref = FoodPreferability.NeverForNutrition, FoodPreferability maxFoodPref = FoodPreferability.MealLavish, float minStackNutrition = 0f, bool allowDrug = false) {
            if(holder.inventory == null) {
                return true;
            }
            if(eater == null) {
                eater = holder;
            }
            if(eater == null) {
                return true;
            }
            Comp_PawnMenu comp = eater.GetComp<Comp_PawnMenu>();
            if(comp == null) {
                return true;
            }
            if(!comp.activated()) {
                return true;
            }
            ThingOwner<Thing> innerContainer = holder.inventory.innerContainer;
            for(int i = 0; i < innerContainer.Count; i++) {
                Thing thing = innerContainer[i];
                if(thing.def.IsNutritionGivingIngestible && thing.IngestibleNow && eater.RaceProps.CanEverEat(thing) && thing.def.ingestible.preferability >= minFoodPref && thing.def.ingestible.preferability <= maxFoodPref && (allowDrug || !thing.def.IsDrug) && comp.contains(thing.def)) {
                    float num = thing.def.ingestible.nutrition * (float)thing.stackCount;
                    if(num >= minStackNutrition) {
                        __result = thing;
                        return false;
                    }
                }
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(FoodUtility), "FoodOptimality")]
    public class FoodUtility_FoodOptimality {
        static void Postfix(Pawn eater, ThingDef foodDef, ref float __result) {
            if(eater == null || foodDef == null) {
                return;
            }
            Comp_PawnMenu comp = eater.GetComp<Comp_PawnMenu>();
            if(comp == null) {
                return;
            }
            if(!comp.activated()) {
                return;
            }
            if(comp.contains(foodDef)) {
                __result += 100000;
            }
        }
    }

    [HarmonyPatch(typeof(FoodUtility), "BestFoodSourceOnMap")]
    public class FoodUtility_BestFoodSourceOnMap {
        static bool Prefix(Pawn eater, Pawn getter, bool allowPlant, bool allowForbidden, bool desperate, FoodPreferability maxPref, bool allowCorpse, bool allowDrug, out ThingDef foodDef, ref Thing __result) {
            foodDef = null;
            if(eater.def.race.Humanlike) {
                return true;
            }
            Comp_PawnMenu comp = eater.GetComp<Comp_PawnMenu>();
            if(comp == null) {
                return true;
            }
            if(!comp.activated()) {
                return true;
            }
            FoodPreferability minPref = FoodPreferability.NeverForNutrition;
            Predicate<Thing> foodValidator = delegate (Thing t) {
                if(!allowForbidden && t.IsForbidden(getter)) {
                    return false;
                }
                if(t.def.ingestible.preferability < minPref) {
                    return false;
                }
                if(t.def.ingestible.preferability > maxPref) {
                    return false;
                }
                if(!t.IngestibleNow || !t.def.IsNutritionGivingIngestible || (!allowCorpse && t is Corpse) || (!allowDrug && t.def.IsDrug) || (!desperate && t.IsNotFresh()) || t.IsDessicated() || !eater.RaceProps.WillAutomaticallyEat(t) || !getter.AnimalAwareOf(t) || !getter.CanReserve(t, 1, -1, null, false)) {
                    return false;
                }
                return comp.contains(t.def);
            };
            ThingRequest thingRequest;
            if((eater.RaceProps.foodType & (FoodTypeFlags.Plant | FoodTypeFlags.Tree)) != FoodTypeFlags.None && allowPlant) {
                thingRequest = ThingRequest.ForGroup(ThingRequestGroup.FoodSource);
            } else {
                thingRequest = ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
            }
            int searchRegionsMax = 100;
            HashSet<Thing> ___filtered = new HashSet<Thing>();
            foreach(Thing thing2 in GenRadial.RadialDistinctThingsAround(getter.Position, getter.Map, 2f, true)) {
                Pawn pawn = thing2 as Pawn;
                if(pawn != null && pawn != getter && pawn.RaceProps.Animal && pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Ingest && pawn.CurJob.GetTarget(TargetIndex.A).HasThing) {
                    ___filtered.Add(pawn.CurJob.GetTarget(TargetIndex.A).Thing);
                }
            }
            bool ignoreEntirelyForbiddenRegions = !allowForbidden && ForbidUtility.CaresAboutForbidden(getter, true) && getter.playerSettings != null && getter.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap != null;
            Predicate<Thing> predicate = (Thing t) => foodValidator(t) && !___filtered.Contains(t) && !t.IsNotFresh();
            IntVec3 position = getter.Position;
            Map map = getter.Map;
            PathEndMode peMode = PathEndMode.ClosestTouch;
            TraverseParms traverseParams = TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false);
            Thing bestThing = GenClosest.ClosestThingReachable(position, map, thingRequest, peMode, traverseParams, 9999f, predicate, null, 0, searchRegionsMax, false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions);
            if(bestThing != null) {
                if(bestThing != null) {
                    foodDef = FoodUtility.GetFinalIngestibleDef(bestThing, false);
                }
                __result = bestThing;
                return false;
            }
            return true;
        }
    }
}
