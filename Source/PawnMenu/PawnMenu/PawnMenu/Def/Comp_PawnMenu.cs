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
                syncSetting();
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
            syncSetting();
            return setting;
        }

        public override void PostExposeData() {
            base.PostExposeData();
            if(localFilterActivated()) {
                Scribe_Deep.Look<ThingFilter>(ref localFilter, "lF");
            }
        }

        public bool activated() {
            return parent.Faction != null && parent.Faction.IsPlayer && (!useWholeKindSetting && localFilterActivated()) || (useWholeKindSetting && kindFilterActivated());
        }
        public bool contains(ThingDef food) {
            if(!activated()) {
                return false;
            }
            syncSetting();
            return setting.filter.Allows(food);
        }
        public bool canHaveMenu(Thing thing) {
            return thing != null && thing is Pawn && thing.Faction != null && thing.Faction.IsPlayer;
        }
        private bool kindFilterActivated() {
            return pawnMenuManager.KindFilter.ContainsKey(parent.def) && pawnMenuManager.KindFilter[parent.def].AllowedDefCount > 0;
        }
        private bool localFilterActivated() {
            return localFilter != null && localFilter.AllowedDefCount > 0;
        }
        private void syncSetting() {
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
            Thing selectedThing = Find.Selector.SingleSelectedThing;
            Pawn selectedPawn = selectedThing as Pawn;
            if(selectedPawn == null) {
                return;
            }
            foreach(ThingDef def in filter.AllowedThingDefs) {
                if(!selectedPawn.def.race.CanEverEat(def)) {
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
                MoteMaker.ThrowText(selectedPawn.Position.ToVector3Shifted(), selectedPawn.Map, selectedPawn.def.defName + " can not eat: " + sb);
            }
        }
    }
}
