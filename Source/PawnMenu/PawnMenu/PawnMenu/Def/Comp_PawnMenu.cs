using System;
using RimWorld;
using Verse;

namespace PawnMenu {
    public class Comp_PawnMenu : ThingComp, IStoreSettingsParent {

        private StorageSettings setting;
        private ThingFilter localFilter;

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

        bool IStoreSettingsParent.StorageTabVisible {
            get {
                return canHaveMenu(parent);
            }
        }

        StorageSettings IStoreSettingsParent.GetParentStoreSettings() {
            return null;
        }

        StorageSettings IStoreSettingsParent.GetStoreSettings() {
            if(setting == null) {
                setting = new StorageSettings(this);
                localFilter = initFilter(setting.filter);
                syncFilter();
            }
            return setting;
        }

        public override void PostExposeData() {
            base.PostExposeData();
            if(setting != null) {
                Scribe_Deep.Look<StorageSettings>(ref setting, "s");
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
            if(useWholeKindSetting) {
                ThingDef def = parent.def;
                if(!PawnMenuManager.KindSettings.ContainsKey(def)) {
                    PawnMenuManager.KindSettings[def] = initFilter(new ThingFilter());
                }
                setting.filter = PawnMenuManager.KindSettings[def];
            } else {
                setting.filter = localFilter;
            }
        }
        private ThingFilter initFilter(ThingFilter filter) {
            filter.DisplayRootCategory = new TreeNode_ThingCategory(ThingCategoryDefOf.Foods);
            return filter;
        }
    }
}
