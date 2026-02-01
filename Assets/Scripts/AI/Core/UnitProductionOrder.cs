namespace RTS.AI.Core
{
    /// <summary>
    /// Represents a unit production order with priority
    /// </summary>
    public struct UnitProductionOrder
    {
        public UnitType Type;
        public int Quantity;
        public int Priority;      // Higher = more urgent
        public int ResourceCost;
    }
}
