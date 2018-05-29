using System;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace PawnMenu {
    [HarmonyPatch(typeof(JobGiver_EatInPartyArea), "FindFood")]
    public class JobGiver_EatInPartyArea_FindFood {
        static bool Prefix(Pawn pawn, IntVec3 partySpot, ref Thing __result) {
            if(pawn == null || pawn.Faction == null || !pawn.Faction.IsPlayer) {
                return true;
            }
            Comp_PawnMenu comp = pawn.GetComp<Comp_PawnMenu>();
            if(comp != null && comp.activated()) {
                Predicate<Thing> validator = (Thing x) => x.IngestibleNow && x.def.IsNutritionGivingIngestible && PartyUtility.InPartyArea(x.Position, partySpot, pawn.Map) && !x.def.IsDrug && x.def.ingestible.preferability > FoodPreferability.RawBad && pawn.RaceProps.WillAutomaticallyEat(x) && !x.IsForbidden(pawn) && x.IsSociallyProper(pawn) && pawn.CanReserve(x, 1, -1, null, false) && comp.contains(x.def);
                __result = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree), PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 14f, validator, null, 0, 12, false, RegionType.Set_Passable, false);
                return __result == null;
            }
            return true;
        }
    }
}
