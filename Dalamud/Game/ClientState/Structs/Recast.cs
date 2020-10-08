using Dalamud.Game.ClientState.Hotbars.Types;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Dalamud.Game.ClientState.Structs
{
    public static class RecastConsts
    {
        public const int BarSize = NumSlots * SlotSize;
        public const int NumBars = 20;
        public const int NumSlots = 16;
        public const int SlotSize = 64;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RecastSlotCollectionContainer
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RecastConsts.NumBars)]
        public RecastSlotCollection[] Bars;

        internal List<List<string>> ToStringComponents()
        {
            return Bars.SelectMany(x => x.ToStringComponents()).ToList();
        }

        public override string ToString()
        {
            var barComponents = ToStringComponents();

            int[] paddings = (
                from col in barComponents.Transpose()
                select col.Aggregate("", (max, curr) => max.Length > curr.Length ? max : curr).Length
            ).ToArray();

            StringBuilder sb = new StringBuilder();
            for (int slotIdx = 0; slotIdx < 20 * 16; slotIdx++)
            {
                int hotbarIdx = slotIdx / 16;
                switch ((HotbarType)hotbarIdx)
                {
                    case HotbarType.HOTBAR_1: sb.Append("[HB1] "); break;
                    case HotbarType.HOTBAR_2: sb.Append("[HB2] "); break;
                    case HotbarType.HOTBAR_3: sb.Append("[HB3] "); break;
                    case HotbarType.HOTBAR_4: sb.Append("[HB4] "); break;
                    case HotbarType.HOTBAR_5: sb.Append("[HB5] "); break;
                    case HotbarType.HOTBAR_6: sb.Append("[HB6] "); break;
                    case HotbarType.HOTBAR_7: sb.Append("[HB7] "); break;
                    case HotbarType.HOTBAR_8: sb.Append("[HB8] "); break;
                    case HotbarType.HOTBAR_9: sb.Append("[HB9] "); break;
                    case HotbarType.HOTBAR_10: sb.Append("[HB0] "); break;
                    case HotbarType.CROSS_HOTBAR_1: sb.Append("[XB1] "); break;
                    case HotbarType.CROSS_HOTBAR_2: sb.Append("[XB2] "); break;
                    case HotbarType.CROSS_HOTBAR_3: sb.Append("[XB3] "); break;
                    case HotbarType.CROSS_HOTBAR_4: sb.Append("[XB4] "); break;
                    case HotbarType.CROSS_HOTBAR_5: sb.Append("[XB5] "); break;
                    case HotbarType.CROSS_HOTBAR_6: sb.Append("[XB6] "); break;
                    case HotbarType.CROSS_HOTBAR_7: sb.Append("[XB7] "); break;
                    case HotbarType.CROSS_HOTBAR_8: sb.Append("[XB8] "); break;
                    case HotbarType.PETBAR: sb.Append("[PET] "); break;
                    case HotbarType.CROSS_PETBAR: sb.Append("[XPT] "); break;
                }

                var slotComponents = barComponents[slotIdx];
                sb.Append($"[Slot{slotIdx % 16 + 1:00}] ");
                for (int componentIdx = 0; componentIdx < slotComponents.Count; componentIdx++)
                {
                    var component = slotComponents[componentIdx].PadRight(paddings[componentIdx]);
                    sb.Append(component);
                    if (componentIdx < slotComponents.Count - 1)
                        sb.Append("  ");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RecastSlotCollection
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RecastConsts.NumSlots)]
        public RecastSlot[] Slots;

        internal List<List<string>> ToStringComponents()
        {
            return (from slot in Slots select slot.ToStringList()).ToList();
        }

        public override string ToString()
        {
            var barComponents = ToStringComponents();

            int[] paddings = (
                from col in barComponents.Transpose()
                select col.Aggregate("", (max, curr) => max.Length > curr.Length ? max : curr).Length
            ).ToArray();

            StringBuilder sb = new StringBuilder();
            for (int slotIdx = 0; slotIdx < 16; slotIdx++)
            {
                var slotComponents = barComponents[slotIdx];
                sb.Append($"[Slot{slotIdx + 1:00}] ");
                for (int componentIdx = 0; componentIdx < slotComponents.Count; componentIdx++)
                {
                    var component = slotComponents[componentIdx].PadRight(paddings[componentIdx]);
                    sb.Append(component);
                    if (componentIdx < slotComponents.Count - 1)
                        sb.Append("  ");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RecastSlot
    {
        public int Category;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Unknown1;

        public int Type;

        public int ID;

        public int Icon;

        public bool IsAvailable;

        public bool HasChargesAvailable;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Unknown2;

        public int GcdCooldownPercent;

        public int CooldownPercent;

        public int CostAndCooldownSeconds;

        public int ItemQuantity;  // For items, this is the same value as CostOrCooldown.

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Unknown3;

        public int IsProcOrCombo;

        public bool InRange;

        internal List<string> ToStringList()
        {
            List<string> components = new List<string>();
            foreach (FieldInfo fieldInfo in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var data = fieldInfo.GetValue(this);
                if (fieldInfo.FieldType == typeof(byte[]))
                {
                    var joined = string.Join(", ", (byte[])data);
                    var bstr = $"new byte[] {{ {joined} }}";
                    components.Add($"{fieldInfo.Name}={bstr}");
                }
                else
                {
                    components.Add($"{fieldInfo.Name}={data}");
                }
            }
            return components;
        }

        public override string ToString()
        {
            return string.Join("  ", ToStringList());
        }
    }
}
