using Dalamud.Game.ClientState.Hotbars.Types;
using Dalamud.Game.Internal;
using JetBrains.Annotations;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Dalamud.Game.ClientState.Hotbars
{
    /// <summary>
    ///     This collection represents Hotbars with Recast data.
    /// </summary>
    public class HotbarTable : IReadOnlyCollection<((HotbarType, SlotType), HotbarSlot)>, IDisposable
    {
        #region HotbarTable Cache

        internal class HotbarMap : Dictionary<(HotbarType, SlotType), HotbarSlot> { }

        private HotbarMap HotbarCache { get; } = new HotbarMap();

        private void ResetCache() => HotbarCache.Clear();

        #endregion

        private readonly Dalamud Dalamud;
        private bool loaded = false;
        private IntPtr HotbarAddress = IntPtr.Zero;
        private IntPtr RecastAddress = IntPtr.Zero;


        /// <summary>
        ///     Set up the hotbar table collection.
        /// </summary>
        /// <param name="dalamud">Dalamud instance.</param>
        public HotbarTable(Dalamud dalamud)
        {
            this.Dalamud = dalamud;

            dalamud.Framework.OnUpdateEvent += Framework_OnUpdateEvent;
        }

        /// <summary>
        ///     The pointer paths don't actually work until the player is logged in
        ///     so don't reference anything until then.
        /// </summary>
        public void LoadPointers()
        {
            if (!loaded)
            {
                loaded = true;

                // Make sure to add the sig length to the address

                // https://github.com/FFXIVAPP/sharlayan-resources/blob/master/signatures/5.3/x64.json#L113
                var hotbarBase = Dalamud.SigScanner.ScanText("48 8B 47 08 49 8B 5D 00 49 8B 77 28 48 8B 3D") + 15;
                HotbarAddress = ResolvePointerPath(hotbarBase, new List<int>() { 0, 0, 56, 48, 40, 32, 0, 0 }, true);
                Log.Verbose($"Hotbar table address {HotbarAddress}");

                // https://github.com/FFXIVAPP/sharlayan-resources/blob/master/signatures/5.3/x64.json#L128
                var recastBase = Dalamud.SigScanner.ScanText("48 8B 47 08 49 8B 5D 00 49 8B 77 28 48 8B 3D") + 15;
                RecastAddress = ResolvePointerPath(recastBase, new List<int>() { 0, 0, 56, 24, 48, 32, 60 }, true);
                Log.Verbose($"Recast table address {RecastAddress}");
            }
        }

        private void Framework_OnUpdateEvent(Framework framework)
        {
            this.ResetCache();
        }

        /// <summary>
        ///     Get the specified hotbar and slot.
        /// </summary>
        /// <param name="hotbar">Hotbar type.</param>
        /// <param name="slot">Hotbar slot.</param>
        [CanBeNull]
        public HotbarSlot this[HotbarType hotbar, SlotType slot] => GetHotbarSlot(hotbar, slot);

        /// <summary>
        ///     Get the specified hotbar and slot.
        /// </summary>
        /// <param name="location">A (HotbarType, SlotType) tuple.</param>
        [CanBeNull]
        public HotbarSlot this[ValueTuple<HotbarType, SlotType> location] => GetHotbarSlot(location.Item1, location.Item2);

        /// <summary>
        ///     Get the specified hotbar and slot.
        /// </summary>
        /// <param name="location">A Tuple&lt;HotbarType, SlotType&gt; tuple.</param>
        [CanBeNull]
        public HotbarSlot this[Tuple<HotbarType, SlotType> location] => GetHotbarSlot(location.Item1, location.Item2);

        /// <summary>
        ///     Get all slots from the specified hotbar.
        /// </summary>
        /// <param name="hotbar">Hotbar type.</param>
        [CanBeNull]
        public IEnumerable<HotbarSlot> this[HotbarType hotbar]
        {
            get
            {
                if (!Enum.IsDefined(typeof(HotbarType), hotbar)) return null;
                return from SlotType slot in Enum.GetValues(typeof(SlotType)) select this[hotbar, slot];
            }
        }

        /// <summary>
        ///     Load the entire cache at once.
        /// </summary>
        public void LoadHotbarCache()
        {
            var hotbarData = Marshal.PtrToStructure<Structs.HotbarSlotCollectionContainer>(HotbarAddress);
            var recastData = Marshal.PtrToStructure<Structs.RecastSlotCollectionContainer>(RecastAddress);
            foreach (HotbarType hotbar in Enum.GetValues(typeof(HotbarType)))
                foreach (SlotType slot in Enum.GetValues(typeof(SlotType)))
                    if (!HotbarCache.TryGetValue((hotbar, slot), out HotbarSlot _))
                        HotbarCache[(hotbar, slot)] = new HotbarSlot(
                            ref hotbarData.Bars[(int)hotbar].Slots[(int)slot],
                            ref recastData.Bars[(int)hotbar].Slots[(int)slot]);
        }

        /// <summary>
        ///     Get a single Hotbar slot on demand.
        /// </summary>
        /// <param name="hotbar">Hotbar type</param>
        /// <param name="slot">Slot number</param>
        [CanBeNull]
        private HotbarSlot GetHotbarSlot(HotbarType hotbar, SlotType slot)
        {

            if (!Enum.IsDefined(typeof(HotbarType), hotbar)) return null;
            if (!Enum.IsDefined(typeof(SlotType), slot)) return null;

            LoadPointers();

            if (!HotbarCache.TryGetValue((hotbar, slot), out HotbarSlot hotbarSlot))
            {
                IntPtr hotbarAddress = HotbarAddress + (((int)hotbar * Structs.HotbarConsts.BarSize) + ((int)slot * Structs.HotbarConsts.SlotSize));
                IntPtr recastAddress = RecastAddress + (((int)hotbar * Structs.RecastConsts.BarSize) + ((int)slot * Structs.RecastConsts.SlotSize));
                Structs.HotbarSlot hotbarData = Marshal.PtrToStructure<Structs.HotbarSlot>(hotbarAddress);
                Structs.RecastSlot recastData = Marshal.PtrToStructure<Structs.RecastSlot>(recastAddress);
                hotbarSlot = HotbarCache[(hotbar, slot)] = new HotbarSlot(ref hotbarData, ref recastData);
            }
            return hotbarSlot;
        }

        #region IReadOnlyCollection Pattern

        public int Count => HotbarCache.Count;

        public IEnumerator<((HotbarType, SlotType), HotbarSlot)> GetEnumerator()
        {
            foreach (HotbarType hotbar in Enum.GetValues(typeof(HotbarType)))
                foreach (SlotType slot in Enum.GetValues(typeof(SlotType)))
                    yield return ((hotbar, slot), this[hotbar, slot]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDisposable Pattern

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            ResetCache();
            Dalamud.Framework.OnUpdateEvent -= Framework_OnUpdateEvent;
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HotbarTable()
        {
            Dispose(false);
        }

        #endregion

        private IntPtr ResolvePointerPath(IntPtr baseAddress, IEnumerable<int> pointerPath, bool isASM)
        {
            // Modified from https://github.com/FFXIVAPP/sharlayan/blob/master/Sharlayan/MemoryHandler.cs#L215

            // Log.Verbose($"Base address is {baseAddress}");
            IntPtr nextAddress = baseAddress;
            foreach (var offset in pointerPath)
            {
                Log.Verbose($"");
                baseAddress = new IntPtr(nextAddress.ToInt64() + offset);
                if (baseAddress == IntPtr.Zero)
                {
                    return baseAddress;
                }

                if (isASM)
                {
                    isASM = false;
                    var asmOffset = Marshal.ReadInt32(new IntPtr(baseAddress.ToInt64()));
                    nextAddress = baseAddress + asmOffset + 4;
                    // Log.Verbose($"New address is {nextAddress} (ASM)");
                }
                else
                {
                    nextAddress = Marshal.ReadIntPtr(baseAddress);
                    // Log.Verbose($"New address is {nextAddress}");
                }
            }
            return baseAddress;
        }
    }
}
