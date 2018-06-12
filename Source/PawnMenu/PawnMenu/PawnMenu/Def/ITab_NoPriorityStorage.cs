using System;
using System.Collections;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace PawnMenu {
    public class ITab_NoPriorityStorage : ITab_Storage {
        private bool kind = true;
        private bool beforeSleep = false;
        public ITab_NoPriorityStorage() : base() {
            this.labelKey = "Menu";
            this.tutorTag = "Menu";
        }
        protected override bool IsPrioritySettingVisible {
            get {
                return false;
            }
        }
        protected override void FillTab() {
            IStoreSettingsParent selStoreSettingsParent = this.SelStoreSettingsParent;
            Comp_PawnMenu comp = selStoreSettingsParent as Comp_PawnMenu;
            comp.activateForAndWhen(kind, beforeSleep);
            Rect rect = new Rect(8f, 0f, 300f, 30f);
            rect = rect.ContractedBy(2f);
            GUI.BeginGroup(rect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.LowerLeft;
            string labelKind = "Kind";
            string labelSelf = "Self";
            string labelNormal = "Breakfast";
            string labelBeforeSleep = "Supper";
            string buttonLabel;
            if(kind) {
                buttonLabel = labelKind;
            } else {
                buttonLabel = labelSelf;
            }
            rect = new Rect(0f, 0f, 30f, 30f);
            Widgets.Label(rect, "For:");
            rect = new Rect(30f, 0f, 60f, 30f);
            if(Widgets.ButtonText(rect, buttonLabel, true, false, true)) {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption(labelKind, delegate {
                    kind = true;
                    comp.activateForAndWhen(kind, beforeSleep);
                }));
                list.Add(new FloatMenuOption(labelSelf, delegate {
                    kind = false;
                    comp.activateForAndWhen(kind, beforeSleep);
                }));
                Find.WindowStack.Add(new FloatMenu(list));
            }
            UIHighlighter.HighlightOpportunity(rect, buttonLabel);
            rect = new Rect(100f, 0f, 45f, 30f);
            Widgets.Label(rect, "When:");
            rect = new Rect(145f, 0f, 100f, 30f);
            if(beforeSleep) {
                buttonLabel = labelBeforeSleep;
            } else {
                buttonLabel = labelNormal;
            }
            if(Widgets.ButtonText(rect, buttonLabel, true, false, true)) {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption(labelBeforeSleep, delegate {
                    beforeSleep = true;
                    comp.activateForAndWhen(kind, beforeSleep);
                }));
                list.Add(new FloatMenuOption(labelNormal, delegate {
                    beforeSleep = false;
                    comp.activateForAndWhen(kind, beforeSleep);
                }));
                Find.WindowStack.Add(new FloatMenu(list));
            }
            UIHighlighter.HighlightOpportunity(rect, buttonLabel);
            GUI.EndGroup();
            base.FillTab();
        }
    }
}
