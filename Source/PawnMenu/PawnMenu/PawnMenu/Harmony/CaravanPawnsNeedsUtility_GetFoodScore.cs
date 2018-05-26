using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using Verse;
using RimWorld.Planet;
using RimWorld;

namespace PawnMenu {
    [HarmonyPatch(typeof(CaravanPawnsNeedsUtility), "GetFoodScore", new Type[] { typeof(ThingDef), typeof(Pawn) })]
    public static class CaravanPawnsNeedsUtility_GetFoodScore {
        static bool Prefix(ThingDef food, Pawn pawn, ref float __result) {
            if(pawn == null) {
                return true;
            }
            Comp_PawnMenu comp = pawn.GetComp<Comp_PawnMenu>();
            if(comp != null && comp.activated() && comp.contains(food)) {
                __result += 10000;
                return false;
            }
            return true;
        }
    }
}
