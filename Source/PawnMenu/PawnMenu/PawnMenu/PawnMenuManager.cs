using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace PawnMenu {

    public class PawnMenuManager {

        public static Dictionary<ThingDef, ThingFilter> KindSettings;

        static PawnMenuManager() {
            KindSettings = new Dictionary<ThingDef, ThingFilter>();
        }
    }
}
