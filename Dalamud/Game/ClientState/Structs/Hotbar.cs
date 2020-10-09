using Dalamud.Game.ClientState.Hotbars.Types;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public struct HotbarSlotCollection
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = HotbarConsts.NumSlots)]
        public HotbarSlot[] Slots;

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
