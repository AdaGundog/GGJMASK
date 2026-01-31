using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.AI.Core
{
    /// <summary>
    /// Shared context for AI decision-making, containing unit state and environment information
    /// </summary>
    public class AIContext
    {
        public BaseUnit Unit;
        public Transform Transform;
        public NavMeshAgent Agent;
        public List<BaseUnit> NearbyAllies;
        public List<EnemyUnit> NearbyEnemies;
        public EnemyUnit CurrentTarget;
        public Vector2 LastKnownEnemyPosition;
        public float DeltaTime;
        
        // Blackboard for sharing data between AI components
        public Dictionary<string, object> Blackboard;

        public AIContext()
        {
            NearbyAllies = new List<BaseUnit>();
            NearbyEnemies = new List<EnemyUnit>();
            Blackboard = new Dictionary<string, object>();
        }
    }
}
