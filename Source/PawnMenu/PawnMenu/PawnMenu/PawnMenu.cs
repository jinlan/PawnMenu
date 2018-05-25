using System;
using System.Collections.Generic;
using System.Reflection;
using HugsLib;
using HugsLib.Settings;
using Harmony;

namespace PawnMenu {
    public class PawnMenu : ModBase {

        public override string ModIdentifier {
            get {
                return "PawnMenu";
            }
        }

        public override void DefsLoaded() {
            HarmonyInstance harmony = HarmonyInstance.Create("PawnMenu");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
