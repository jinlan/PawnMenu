using System;
using RimWorld;

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
    }
}
