using UnityEngine;

namespace RTS.AI.Core
{
    /// <summary>
    /// Configuration for AI system parameters
    /// </summary>
    [CreateAssetMenu(fileName = "AIConfig", menuName = "RTS/AI/Configuration")]
    public class AIConfiguration : ScriptableObject
    {
        [Header("Commander AI")]
        public float strategicUpdateInterval = 2.0f;
        public float aggressionLevel = 0.5f;
        public float cautionLevel = 0.5f;
        
        [Header("Unit AI")]
        public float tacticalUpdateInterval = 0.1f;
        public float retreatHealthThreshold = 0.3f;
        public float engagementRange = 10f;
        public float supportRange = 15f;
        
        [Header("Behavior Tree")]
        public int maxTreeDepth = 10;
        public bool enableBTDebugging = false;
        
        [Header("Utility AI")]
        public float randomizationFactor = 0.1f;
        public bool cacheActionScores = true;
        public float scoreCacheDuration = 0.5f;
        
        [Header("State Machine")]
        public bool enableFSMDebugging = false;
        public float stateTransitionDelay = 0.1f;
        
        [Header("ML-Agents")]
        public bool useMLAgents = false;
        public int maxSteps = 5000;
        public float decisionInterval = 0.2f;
        
        [Header("Performance")]
        public int maxUnitsPerFrame = 10;
        public bool distributeProcessing = true;
    }
}
