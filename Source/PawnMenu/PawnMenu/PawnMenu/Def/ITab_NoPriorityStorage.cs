using System;
using System.Collections;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace PawnMenu {
    public class ITab_NoPriorityStorage : ITab_Storage {
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
            StorageSettings settings = selStoreSettingsParent.GetStoreSettings();
            Rect position = new Rect(0f, 0f, 300, 30);
            GUI.BeginGroup(position);
            Text.Font = GameFont.Small;
            Rect rect = new Rect(0f, 0f, 180f, 30f).ContractedBy(2f);
            string labelWholeKindSetting = "Use whole kind setting";
            string labelLocalSetting = "Use personal setting";
            string buttonLabel;
            if(comp.UseWholeKindSetting) {
                buttonLabel = labelWholeKindSetting;
            } else {
                buttonLabel = labelLocalSetting;
            }
            if(Widgets.ButtonText(rect, buttonLabel, true, false, true)) {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption(labelWholeKindSetting, delegate {
                    comp.UseWholeKindSetting = true;
                }));
                list.Add(new FloatMenuOption(labelLocalSetting, delegate {
                    comp.UseWholeKindSetting = false;
                }));
                Find.WindowStack.Add(new FloatMenu(list));
            }
            UIHighlighter.HighlightOpportunity(rect, buttonLabel);
            GUI.EndGroup();
            base.FillTab();
        }
    }
}
