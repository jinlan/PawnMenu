using System;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace PawnMenu {
    [HarmonyPatch(typeof(JobGiver_EatRandom), "TryGiveJob")]
    public class JobGiver_EatRandom_TryGiveJob {
        static bool Prefix(Pawn pawn, ref Job __result) {
            if(pawn == null || pawn.Faction == null || !pawn.Faction.IsPlayer) {
                return true;
            }
            Comp_PawnMenu comp = pawn.GetComp<Comp_PawnMenu>();
            if(comp != null && comp.activated()) {
                Predicate<Thing> validator = (Thing t) => t.def.category == ThingCategory.Item && t.IngestibleNow && pawn.RaceProps.CanEverEat(t) && pawn.CanReserve(t, 1, -1, null, false) && comp.contains(t.def);
                Thing food = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableAlways), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 10f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
                if(food != null) {
                    __result = new Job(JobDefOf.Ingest, food);
                    return false;
                }
            }
            return true;
        }
    }
}
