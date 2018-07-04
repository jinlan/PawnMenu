using System.Text;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PawnMenu {
    public class Comp_PawnMenu : ThingComp, IStoreSettingsParent {

        private StorageSettings setting;
        private ThingFilter localFilter;
        private ThingFilter localFilterBeforeSleep;
        private PawnMenuManager pawnMenuManager;

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
            return setting;
        }
        public void syncForAndWhen(bool kind, bool supper) {
            if(kind) {
                ThingDef def = parent.def;
                Dictionary<ThingDef, ThingFilter> filters;
                if(supper) {
                    filters = pawnMenuManager.KindFilterBeforeSleep;
                } else {
                    filters = pawnMenuManager.KindFilter;
                }
                if(!filters.ContainsKey(def)) {
                    filters[def] = new ThingFilter(() => {
                        checkDef();
                    });
                }
                syncSetting(filters[def]);
            } else {
                if(supper) {
                    if(localFilterBeforeSleep == null) {
                        localFilterBeforeSleep = new ThingFilter(() => {
                            checkDef();
                        });
                    }
                    syncSetting(localFilterBeforeSleep);
                } else {
                    if(localFilter == null) {
                        localFilter = new ThingFilter(() => {
                            checkDef();
                        });
                    }
                    syncSetting(localFilter);
                }
            }
        }
        public override void PostExposeData() {
            base.PostExposeData();
            Scribe_Deep.Look<ThingFilter>(ref localFilter, "lF");
            Scribe_Deep.Look<ThingFilter>(ref localFilterBeforeSleep, "lFBS");
            syncSetting();
        }

        public bool activated() {
            if(setting == null) {
                return false;
            }
            if(parent.Faction == null || !parent.Faction.IsPlayer) {
                return false;
            }
            if(localBeforeSleepActivated()) {
                return true;
            }
            if(localNormalActivated()) {
                return true;
            }
            if(kindBeforeSleepActivated()) {
                return true;
            }
            if(kindNormalActivated()) {
                return true;
            }
            return false;
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
        private bool kindNormalActivated() {
            return pawnMenuManager.KindFilter.ContainsKey(parent.def) && pawnMenuManager.KindFilter[parent.def].AllowedDefCount > 0;
        }
        private bool kindBeforeSleepActivated() {
            return closeToSleepNow() && pawnMenuManager.KindFilterBeforeSleep.ContainsKey(parent.def) && pawnMenuManager.KindFilterBeforeSleep[parent.def].AllowedDefCount > 0;
        }
        private bool localNormalActivated() {
            return localFilter != null && localFilter.AllowedDefCount > 0; ;
        }
        private bool localBeforeSleepActivated() {
            return closeToSleepNow() && localFilterBeforeSleep != null && localFilterBeforeSleep.AllowedDefCount > 0;
        }
        private void syncSetting(ThingFilter filter = null) {
            if(setting == null) {
                setting = new StorageSettings();
            }
            if(filter != null) {
                setting.filter = filter;
                return;
            }
            if(localBeforeSleepActivated()) {
                setting.filter = localFilterBeforeSleep;
                return;
            }
            if(localNormalActivated()) {
                setting.filter = localFilter;
                return;
            }
            if(kindBeforeSleepActivated()) {
                setting.filter = pawnMenuManager.KindFilterBeforeSleep[parent.def];
                return;
            }
            if(kindNormalActivated()) {
                setting.filter = pawnMenuManager.KindFilter[parent.def];
                return;
            }
        }
        private void checkDef(ThingFilter filter = null) {
            if(filter == null) {
                filter = setting.filter;
            }
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
        private bool closeToSleepNow() {
            if(!(parent is Pawn) || ((Pawn)parent).needs == null || ((Pawn)parent).needs.rest == null) {
                return false;
            }
            Need_Rest rest = ((Pawn)parent).needs.rest;
            if(rest.MaxLevel >= 0.8) {
                return rest.CurLevel <= 0.4;
            } else {
                return rest.CurLevelPercentage <= 0.5;
            }
        }
    }
}
