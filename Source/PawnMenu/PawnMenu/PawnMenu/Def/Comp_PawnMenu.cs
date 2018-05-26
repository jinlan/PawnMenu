﻿using System;
using RimWorld;
using Verse;

namespace PawnMenu {
    public class Comp_PawnMenu : ThingComp, IStoreSettingsParent {

        private StorageSettings setting;

        public override void PostSpawnSetup(bool respawningAfterLoad) {
            if(setting == null) {
                setting = new StorageSettings(this);
            }
        }

        bool IStoreSettingsParent.StorageTabVisible {
            get {
                return canHaveMenu(parent);
            }
        }

        StorageSettings IStoreSettingsParent.GetParentStoreSettings() {
            return ((CompProperties_PawnMenu)props).fixedStorageSettings; ;
        }

        StorageSettings IStoreSettingsParent.GetStoreSettings() {
            return setting;
        }

        public override void PostExposeData() {
            base.PostExposeData();
            Scribe_Deep.Look<StorageSettings>(ref setting, "s");
        }

        public bool contains(ThingDef food) {
            return setting.filter.Allows(food);
        }
        public bool canHaveMenu(Thing thing) {
            return thing != null && thing is Pawn && thing.Faction != null && thing.Faction.IsPlayer;
        }
    }
}
