using Dalamud.Game.ClientState.Hotbars.Types;
using System.Collections.Generic;
using System.Linq;
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

        public override string ToString()
        {
            List<List<string>> barComponents = Bars.SelectMany(bar => bar.Slots.Select(slot => Util.StructToComponents(slot).ToList()).ToList()).ToList();
            int[] paddings = barComponents.Transpose().Select(col => col.Aggregate("", (max, curr) => max.Length > curr.Length ? max : curr).Length).ToArray();

            StringBuilder sb = new StringBuilder();

            int slotIdx = 0;
            foreach (var slotComponents in barComponents)
            {
                var paddedSlotComponents = Enumerable.Zip(slotComponents, paddings, (slotComponent, padding) => slotComponent.PadRight(padding)).ToList();
                paddedSlotComponents.Insert(0, $"[Slot{slotIdx + 1:00}]");
                switch ((HotbarType)(slotIdx / 16))
                {
                    case HotbarType.HOTBAR_1: slotComponents.Insert(0, "[HB1]"); break;
                    case HotbarType.HOTBAR_2: slotComponents.Insert(0, "[HB2]"); break;
                    case HotbarType.HOTBAR_3: slotComponents.Insert(0, "[HB3]"); break;
                    case HotbarType.HOTBAR_4: slotComponents.Insert(0, "[HB4]"); break;
                    case HotbarType.HOTBAR_5: slotComponents.Insert(0, "[HB5]"); break;
                    case HotbarType.HOTBAR_6: slotComponents.Insert(0, "[HB6]"); break;
                    case HotbarType.HOTBAR_7: slotComponents.Insert(0, "[HB7]"); break;
                    case HotbarType.HOTBAR_8: slotComponents.Insert(0, "[HB8]"); break;
                    case HotbarType.HOTBAR_9: slotComponents.Insert(0, "[HB9]"); break;
                    case HotbarType.HOTBAR_10: slotComponents.Insert(0, "[HB0]"); break;
                    case HotbarType.CROSS_HOTBAR_1: slotComponents.Insert(0, "[XB1]"); break;
                    case HotbarType.CROSS_HOTBAR_2: slotComponents.Insert(0, "[XB2]"); break;
                    case HotbarType.CROSS_HOTBAR_3: slotComponents.Insert(0, "[XB3]"); break;
                    case HotbarType.CROSS_HOTBAR_4: slotComponents.Insert(0, "[XB4]"); break;
                    case HotbarType.CROSS_HOTBAR_5: slotComponents.Insert(0, "[XB5]"); break;
                    case HotbarType.CROSS_HOTBAR_6: slotComponents.Insert(0, "[XB6]"); break;
                    case HotbarType.CROSS_HOTBAR_7: slotComponents.Insert(0, "[XB7]"); break;
                    case HotbarType.CROSS_HOTBAR_8: slotComponents.Insert(0, "[XB8]"); break;
                    case HotbarType.PETBAR: slotComponents.Insert(0, "[PET]"); break;
                    case HotbarType.CROSS_PETBAR: slotComponents.Insert(0, "[XPT]"); break;
                }
                sb.Append(string.Join("  ", paddedSlotComponents));
                sb.Append("\n");
                slotIdx++;
            }
            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RecastSlotCollection
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RecastConsts.NumSlots)]
        public RecastSlot[] Slots;

        public override string ToString()
        {
            List<List<string>> barComponents = Slots.Select(slot => Util.StructToComponents(slot)).ToList();
            int[] paddings = barComponents.Transpose().Select(col => col.Aggregate("", (max, curr) => max.Length > curr.Length ? max : curr).Length).ToArray();

            StringBuilder sb = new StringBuilder();

            int slotIdx = 0;
            foreach (var slotComponents in barComponents)
            {
                var paddedSlotComponents = Enumerable.Zip(slotComponents, paddings, (slotComponent, padding) => slotComponent.PadRight(padding)).ToList();
                paddedSlotComponents.Insert(0, $"[Slot{slotIdx + 1:00}]");
                sb.Append(string.Join("  ", paddedSlotComponents));
                sb.Append("\n");
                slotIdx++;
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

        public override string ToString()
        {
            return string.Join("  ", Util.StructToComponents(this));
        }
    }
}
