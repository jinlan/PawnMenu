using System;
using Verse;
using RimWorld;

namespace PawnMenu {
    public class CompProperties_PawnMenu : CompProperties {
        public StorageSettings fixedStorageSettings;
        public override void ResolveReferences(ThingDef parentDef) {
            base.ResolveReferences(parentDef);
            this.fixedStorageSettings.filter.ResolveReferences();
            this.fixedStorageSettings.filter.DisplayRootCategory = new TreeNode_ThingCategory(ThingCategoryDefOf.Foods);
        }
    }
}
