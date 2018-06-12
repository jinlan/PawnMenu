using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace PawnMenu {

    public class PawnMenuManager : WorldComponent {

        public Dictionary<ThingDef, ThingFilter> KindFilter;
        public Dictionary<ThingDef, ThingFilter> KindFilterBeforeSleep;
        public StorageSettings ParentSetting;

        public PawnMenuManager(World world) : base(world) {
            KindFilter = new Dictionary<ThingDef, ThingFilter>();
            KindFilterBeforeSleep = new Dictionary<ThingDef, ThingFilter>();
            ParentSetting = new StorageSettings();
            ParentSetting.filter.SetAllow(ThingCategoryDefOf.Foods, true);
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Collections.Look<ThingDef, ThingFilter>(ref KindFilter, "KF", LookMode.Def, LookMode.Deep);
            Scribe_Collections.Look<ThingDef, ThingFilter>(ref KindFilterBeforeSleep, "KFBS", LookMode.Def, LookMode.Deep);
        }
    }
}
