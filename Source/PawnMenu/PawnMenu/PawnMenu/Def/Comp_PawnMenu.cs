using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    pawnMenuManager.KindFilter[def] = new ThingFilter(() => {
                        checkDef(pawnMenuManager.KindFilter[def]);
                    });
                }
                setting.filter = pawnMenuManager.KindFilter[def];
            } else {
                if(localFilter == null) {
                    localFilter = new ThingFilter(() => {
                        checkDef(localFilter);
                    });
                }
                setting.filter = localFilter;
            }
        }
        private void checkDef(ThingFilter filter) {
            List<ThingDef> illegalDefs = new List<ThingDef>();
            foreach(ThingDef def in filter.AllowedThingDefs) {
                if(parent.def.race.CanEverEat(def)) {
                    illegalDefs.Add(def);
                }
            }
            if(illegalDefs.Count != 0) {
                StringBuilder sb = new StringBuilder();
                int i = 0;
                illegalDefs.ForEach((ThingDef arg) => {
                    sb.Append(arg);
                    i++;
                    if(i <= illegalDefs.Count - 1) {
                        sb.Append(',');
                        sb.Append(' ');
                    }
                    filter.SetAllow(arg, false);
                });
                MoteMaker.ThrowText(parent.Position.ToVector3Shifted(), parent.Map, parent.def.defName + " can not eat: " + sb);
            }
        }
    }
}
