using System;
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
            if(!comp.canHaveMenu(eater)) {
                return true;
            }
            ThingOwner<Thing> innerContainer = holder.inventory.innerContainer;
            for(int i = 0; i < innerContainer.Count; i++) {
                Thing thing = innerContainer[i];
                if(thing.def.IsNutritionGivingIngestible && thing.IngestibleNow && eater.RaceProps.CanEverEat(thing) && thing.def.ingestible.preferability >= minFoodPref && thing.def.ingestible.preferability <= maxFoodPref && (allowDrug || !thing.def.IsDrug)) {
                    float num = thing.def.ingestible.nutrition * (float)thing.stackCount;
                    if(num >= minStackNutrition && comp.contains(thing.def)) {
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
            if(!comp.canHaveMenu(eater)) {
                return;
            }
            if(comp.contains(foodDef)) {
                __result += 10000;
            }
        }
    }
}
