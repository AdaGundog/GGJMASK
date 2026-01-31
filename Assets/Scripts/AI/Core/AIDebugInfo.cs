using System.Collections.Generic;

namespace RTS.AI.Core
{
    /// <summary>
    /// Debug information for AI systems
    /// </summary>
    public class AIDebugInfo
    {
        // Behavior Tree
        public List<string> ActiveBTNodes;
        public string LastBTStatus;
        
        // State Machine
        public string CurrentState;
        public string PreviousState;
        public List<string> AvailableTransitions;
        
        // Utility AI
        public Dictionary<string, float> ActionScores;
        public string SelectedAction;
        public string SelectionReason;
        
        // General
        public float ProcessingTime;
        public int FramesSinceUpdate;

        public AIDebugInfo()
        {
            ActiveBTNodes = new List<string>();
            AvailableTransitions = new List<string>();
            ActionScores = new Dictionary<string, float>();
        }
    }
}
