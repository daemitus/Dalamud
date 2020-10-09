namespace Dalamud.Game.ClientState.Hotbars.Types
{
    /// <summary>
    ///     This class represents the usability of hotbar slots.
    /// </summary>
    public class HotbarSlot
    {
        /// <summary>
        ///     The memory representation of the hotbar structure.
        /// </summary>
        protected Structs.HotbarSlot hotbarStruct;

        /// <summary>
        ///     The memory representation of the recast structure.
        /// </summary>
        protected Structs.RecastSlot recastStruct;

        /// <summary>
        ///     Initialize a representation of a recast bar.
        /// </summary>
        /// <param name="hotbarStruct">The memory representation of the hotbar structure.</param>
        /// <param name="recastStruct">The memory representation of the recast structure.</param>
        public HotbarSlot(ref Structs.HotbarSlot hotbarStruct, ref Structs.RecastSlot recastStruct)
        {
            this.hotbarStruct = hotbarStruct;
            this.recastStruct = recastStruct;
        }

        /// <summary>
        ///     Name of the object contained in this slot.
        /// </summary>
        public string Name => string.IsNullOrEmpty(hotbarStruct.Keybind)  // Breaks if oldValue == ""
            ? hotbarStruct.Name : hotbarStruct.Name.Replace(hotbarStruct.Keybind, "");

        /// <summary>
        ///     When you hover over the slot, the contents between the [...]
        /// </summary>
        public string Keybind => hotbarStruct.Keybind.Trim(' ', '[', ']');

        /// <summary>
        ///     The primary key of the keybind as an integer.
        /// </summary>
        public int KeybindVK => hotbarStruct.KeybindVK == 0 ? hotbarStruct.KeybindModifiersVK : hotbarStruct.KeybindVK;

        /// <summary>
        ///     The modifier keys of the keybind as integers from <see cref="System.Windows.Forms.Keys"/>.
        ///     SHIFT = 16, CTRL = 17, ALT = 18.
        /// </summary>
        public int[] KeybindModifiersVK
        {
            get
            {
                // From System.Windows.Forms.Keys
                int ShiftKey = 16, CtrlKey = 17, AltKey = 18;
                return hotbarStruct.KeybindModifiersVK switch
                {
                    41666 => new int[1] { CtrlKey },
                    42946 => new int[1] { ShiftKey },
                    43714 => new int[1] { AltKey },
                    48322 => new int[2] { ShiftKey, AltKey },
                    48578 => new int[2] { CtrlKey, ShiftKey },
                    48834 => new int[2] { CtrlKey, AltKey },
                    46786 => new int[3] { CtrlKey, ShiftKey, AltKey },
                    _ => new int[0] { },
                };
            }
        }

        /// <summary>
        ///     The modifier keys of the keybind as an integer mask from <see cref="System.Windows.Forms.Keys"/>.
        ///     SHIFT = 65536, CTRL = 131072, ALT = 262144
        /// </summary>
        public int KeybindModifiersVKMask
        {
            get
            {
                // From System.Windows.Forms.Keys
                int ShiftMask = 65536, CtrlMask = 131072, AltMask = 262144;
                switch (hotbarStruct.KeybindModifiersVK)
                {
                    case 41666: return CtrlMask;
                    case 42946: return ShiftMask;
                    case 43714: return AltMask;
                    case 46786: return CtrlMask | ShiftMask | AltMask;
                    case 48322: return ShiftMask | AltMask;
                    case 48578: return CtrlMask | ShiftMask;
                    case 48834: return CtrlMask | AltMask;
                    default: return 0;
                }
            }
        }

        /// <summary>
        ///     The category of this slot.
        /// </summary>
        public int Category => this.recastStruct.Category;

        /// <summary>
        ///     The type of this slot.
        /// </summary>
        public int Type => this.recastStruct.Type;

        /// <summary>
        ///     The ID of this slot.
        /// </summary>
        public int ID => this.recastStruct.ID;

        /// <summary>
        ///     The icon of this slot.
        /// </summary>
        public int Icon => this.recastStruct.Icon;

        /// <summary>
        ///     The availability of this slot.
        /// </summary>
        public bool IsAvailable => this.recastStruct.IsAvailable;

        /// <summary>
        ///     If this slot has charges, if any are availabile. If this slot doesn't, true.
        /// </summary>
        public bool HasChargesAvailable => this.recastStruct.HasChargesAvailable;

        /// <summary>
        ///     The GCD cooldown percent of this slot.
        /// </summary>
        public int GcdCooldownPercent => this.recastStruct.GcdCooldownPercent;

        /// <summary>
        ///     The cooldown percent of this slot.
        /// </summary>
        public int CooldownPercent => this.recastStruct.CooldownPercent;

        /// <summary>
        ///     The Mp/Cp/Gp cost of this slot.
        /// </summary>
        public int Cost => this.recastStruct.CostAndCooldownSeconds;

        /// <summary>
        ///     The cooldown of this slot.
        /// </summary>
        public int CooldownSeconds => this.recastStruct.CostAndCooldownSeconds;

        /// <summary>
        ///     The charges/quantity of this slot.
        /// </summary>
        public int ItemQuantity => this.recastStruct.ItemQuantity;

        /// <summary>
        ///     If this slot is any sort of proc or combo.
        /// </summary>
        public bool IsProcOrCombo => this.recastStruct.IsProcOrCombo > 0;

        /// <summary>
        ///     If this slot is in range of use.
        /// </summary>
        public bool InRange => this.recastStruct.InRange;
    }
}
