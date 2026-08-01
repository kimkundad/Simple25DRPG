namespace Simple25DRPG.Items
{
    /// <summary>
    /// Defines an object that can accept collected item pickups.
    /// </summary>
    public interface IItemCollector
    {
        /// <summary>
        /// Attempts to collect an item amount.
        /// </summary>
        /// <param name="item">Item data being collected.</param>
        /// <param name="amount">Amount being collected.</param>
        /// <returns>True when the item was accepted.</returns>
        bool TryCollect(ItemData item, int amount);
    }
}
