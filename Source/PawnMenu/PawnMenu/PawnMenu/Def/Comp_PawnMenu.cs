using System;
using RimWorld;
using Verse;

namespace PawnMenu {
    public class Comp_PawnMenu : ThingComp, IStoreSettingsParent {

        private StorageSettings setting;
        private ThingFilter localFilter;
        private PawnMenuManager pawnMenuManager;

        private bool useWholeKindSetting = true;

        public bool UseWholeKindSetting {
            get {
                return useWholeKindSetting;
            }
            set {
                useWholeKindSetting = value;
                syncFilter();
            }
        }

        public override void Initialize(CompProperties props) {
            base.Initialize(props);
            pawnMenuManager = Find.World.GetComponent<PawnMenuManager>();
        }

        bool IStoreSettingsParent.StorageTabVisible {
            get {
                return canHaveMenu(parent);
            }
        }

        StorageSettings IStoreSettingsParent.GetParentStoreSettings() {
            return pawnMenuManager.ParentSetting;
        }

        StorageSettings IStoreSettingsParent.GetStoreSettings() {
            syncFilter();
            return setting;
        }

        public override void PostExposeData() {
            base.PostExposeData();
            if(activated()) {
                Scribe_Deep.Look<ThingFilter>(ref localFilter, "lF");
            }
        }

        public bool activated() {
            if(setting == null) {
                return false;
            }
            return setting.filter.AllowedDefCount > 0;
        }
        public bool contains(ThingDef food) {
            if(!activated()) {
                return false;
            }
            return setting.filter.Allows(food);
        }
        public bool canHaveMenu(Thing thing) {
            return thing != null && thing is Pawn && thing.Faction != null && thing.Faction.IsPlayer;
        }
        private void syncFilter() {
            if(setting == null) {
                setting = new StorageSettings();
            }
            if(useWholeKindSetting) {
                ThingDef def = parent.def;
                if(!pawnMenuManager.KindFilter.ContainsKey(def)) {
                    pawnMenuManager.KindFilter[def] = new ThingFilter();
                }
                setting.filter = pawnMenuManager.KindFilter[def];
            } else {
                if(localFilter == null) {
                    localFilter = new ThingFilter();
                }
                setting.filter = localFilter;
            }
        }
    }
}
