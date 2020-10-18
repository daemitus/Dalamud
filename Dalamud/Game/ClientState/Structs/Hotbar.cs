using Dalamud.Game.ClientState.Hotbars.Types;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Dalamud.Game.ClientState.Structs
{
    public static class HotbarConsts
    {
        public const int BarSize = NumSlots * SlotSize;
        public const int NumBars = 20;
        public const int NumSlots = 16;
        public const int SlotSize = 224;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HotbarSlotCollectionContainer
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = HotbarConsts.NumBars)]
        public HotbarSlotCollection[] Bars;

        public override string ToString()
        {
            List<List<string>> barComponents = Bars.SelectMany(bar => bar.Slots.Select(slot => Util.StructToComponents(slot).ToList()).ToList()).ToList();
            int[] paddings = barComponents.Transpose().Select(col => col.Aggregate("", (max, curr) => max.Length > curr.Length ? max : curr).Length).ToArray();

            StringBuilder sb = new StringBuilder();

            int slotIdx = 0;
            foreach (var slotComponents in barComponents)
            {
                var paddedSlotComponents = Enumerable.Zip(slotComponents, paddings, (slotComponent, padding) => slotComponent.PadRight(padding)).ToList();
                slotComponents.Insert(0, $"[Slot{slotIdx % 16 + 1:00}]");
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
    public struct HotbarSlotCollection
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = HotbarConsts.NumSlots)]
        public HotbarSlot[] Slots;

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

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct HotbarSlot
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 102)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Keybind;

        /*
        Results after setting a slot to various permutations of Ctrl+Alt+Shift+1
        It's definitly the modifier key, but how its packed is beyond me.
            C   1 = { 194, 162, 49, 0 } = 41666, 49
             S  1 = { 194, 167, 49, 0 } = 42946, 49
              A 1 = { 194, 170, 49, 0 } = 43714, 49
            CSA 1 = { 194, 182, 49, 0 } = 46786, 49
             SA 1 = { 194, 188, 49, 0 } = 48322, 49
            CS  1 = { 194, 189, 49, 0 } = 48578, 49
            C A 1 = { 194, 190, 49, 0 } = 48834, 49
                1 = {  49,   0, 49, 0 } =    49, 49
        */
        public ushort KeybindModifiersVK;

        public ushort KeybindVK;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] padding;

        public int ID;

        public int ID2;     // Repeated?

        public int IconID;  // Likely only changes from ID if using XIVCombo

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 62)]
        public byte[] Unknown;

        public override string ToString()
        {
            return Util.StructToString(this);
        }
    }
}
