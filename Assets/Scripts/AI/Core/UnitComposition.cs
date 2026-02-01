using System.Collections.Generic;

namespace RTS.AI.Core
{
    /// <summary>
    /// Represents the composition of a military force
    /// </summary>
    public struct UnitComposition
    {
        public int InfantryCount;
        public int ArcherCount;
        public int CavalryCount;
        public Dictionary<int, int> RankDistribution; // Rank (1-3) -> Count

        public int TotalCount => InfantryCount + ArcherCount + CavalryCount;

        public UnitComposition(int infantry, int archer, int cavalry)
        {
            InfantryCount = infantry;
            ArcherCount = archer;
            CavalryCount = cavalry;
            RankDistribution = new Dictionary<int, int>();
        }
    }
}
