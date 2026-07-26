using System;
using Core.DI;
using HotUpdate.Common.Config.Inventory;

namespace HotUpdate.Game.InventoryModule.Sorts
{
    /// <summary>
    /// 背包排序器
    /// </summary>
    public abstract class InventorySorter
    {
        public static Func<int, Comparison<Item>> Default { get; private set; }

        static InventorySorter()
        {
            Default = i => DIContainer.Create<DefaultIDSorter>().GetSorter(i);
        }

        protected abstract Comparison<Item> GetSorter(int i);

        public static Comparison<Item> Get<T>(int i) where T : InventorySorter
        {
            return DIContainer.Create<T>().GetSorter(i);
        }
    }
}
