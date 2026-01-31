# Design Document: Advanced AI System

## Overview

The Advanced AI System is a comprehensive, multi-layered artificial intelligence framework for a Unity 2D RTS game. The system integrates five distinct AI approaches—enhanced basic AI, behavior trees, utility AI, state machines, and ML-Agents—into a cohesive architecture that provides both strategic (Commander) and tactical (Unit) intelligence.

The design follows a modular, incremental approach where each AI layer builds upon the previous one. The Commander AI uses Utility AI for high-level strategic decisions (army composition, resource allocation, attack timing), while Unit AI combines Behavior Trees and State Machines for tactical execution (movement, combat, coordination). ML-Agents integration provides an optional learning layer for adaptive behavior.

Key design principles:
- **Modularity**: Each AI system is independent and can be tested/debugged separately
- **Integration**: Systems communicate through well-defined interfaces
- **Performance**: Designed to support 50+ units with distributed processing
- **Extensibility**: Easy to add new behaviors, states, and decision factors
- **Compatibility**: Works with existing BaseUnit, PlayerUnit, EnemyUnit classes and NavMesh movement

## Architecture

### System Layers

```
┌─────────────────────────────────────────────────────────┐
│                    ML-Agents Layer                       │
│              (Optional Learning & Adaptation)            │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                   Commander AI Layer                     │
│              (Strategic Decision Making)                 │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │           Utility AI System                       │  │
│  │  - Army Composition Scoring                       │  │
│  │  - Attack Timing Evaluation                       │  │
│  │  - Resource Allocation                            │  │
│  │  - Threat Analysis                                │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                            ↓
                    Strategic Orders
                            ↓
┌─────────────────────────────────────────────────────────┐
│                     Unit AI Layer                        │
│               (Tactical Execution)                       │
│                                                          │
│  ┌──────────────┐         ┌──────────────────────────┐ │
│  │ Behavior Tree│ ←────→  │   State Machine          │ │
│  │   System     │         │   (FSM)                  │ │
│  │              │         │                          │ │
│  │ - Conditions │         │ States:                  │ │
│  │ - Actions    │         │ - Idle, Patrol           │ │
│  │ - Sequences  │         │ - Chase, Attack          │ │
│  │ - Selectors  │         │ - Retreat, Defend        │ │
│  │              │         │ - Regroup                │ │
│  └──────────────┘         └──────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
                            ↓
                    Movement Commands
                            ↓
┌─────────────────────────────────────────────────────────┐
│              Existing Game Systems                       │
│  - NavMeshAgent (Movement)                              │
│  - BaseUnit/PlayerUnit/EnemyUnit (Unit Management)      │
│  - Rank System (Rookie/Veteran/Elite)                   │
│  - Unit Types (Infantry/Archer/Cavalry)                 │
└─────────────────────────────────────────────────────────┘
```

### Component Interaction Flow

1. **Strategic Level**: Commander AI evaluates game state using Utility AI, scores potential strategic actions, and issues high-level orders
2. **Tactical Level**: Unit AI receives orders, uses Behavior Trees to decompose into actions, and State Machine to manage execution
3. **Execution Level**: Actions translate to NavMeshAgent commands and unit ability usage
4. **Learning Level** (Optional): ML-Agents observe outcomes, receive rewards, and adapt behavior over time

## Components and Interfaces

### 1. Enhanced Basic AI Components

#### ThreatAnalyzer
Analyzes enemy force composition and calculates threat levels.

```csharp
public class ThreatAnalyzer
{
    // Analyzes enemy units and returns threat assessment
    public ThreatAssessment AnalyzeEnemyThreat(List<EnemyUnit> enemies, Vector2 position)
    
    // Calculates threat score based on unit composition
    public float CalculateThreatScore(UnitComposition composition)
    
    // Identifies counter-units needed based on enemy composition
    public Dictionary<UnitType, int> GetCounterComposition(UnitComposition enemyComp)
}

public struct ThreatAssessment
{
    public float ThreatScore;           // Overall threat level (0-1)
    public UnitComposition Composition;  // Enemy unit breakdown
    public Vector2 ThreatCenter;        // Center of enemy force
    public float AverageRank;           // Average experience level
}

public struct UnitComposition
{
    public int InfantryCount;
    public int ArcherCount;
    public int CavalryCount;
    public Dictionary<Rank, int> RankDistribution;
}
```

#### ResourceManager
Manages resource allocation for unit production.

```csharp
public class ResourceManager
{
    // Prioritizes unit production based on tactical needs
    public List<UnitProductionOrder> PrioritizeProduction(
        int availableResources,
        UnitComposition currentForces,
        UnitComposition enemyForces)
    
    // Checks if resources are sufficient for production
    public bool CanAfford(UnitType type, int quantity)
    
    // Allocates resources for specific production
    public bool AllocateResources(UnitProductionOrder order)
}

public struct UnitProductionOrder
{
    public UnitType Type;
    public int Quantity;
    public int Priority;      // Higher = more urgent
    public int ResourceCost;
}
```

#### TargetPrioritizer
Selects optimal targets for units based on multiple factors.

```csharp
public class TargetPrioritizer
{
    // Selects best target from available enemies
    public EnemyUnit SelectTarget(
        BaseUnit attacker,
        List<EnemyUnit> potentialTargets)
    
    // Calculates priority score for a target
    public float CalculateTargetPriority(
        BaseUnit attacker,
        EnemyUnit target)
    
    // Factors: type advantage, threat level, distance, health
}
```

### 2. Behavior Tree System

#### Core Behavior Tree Classes

```csharp
// Base node class
public abstract class BTNode
{
    public abstract BTStatus Execute(AIContext context);
}

public enum BTStatus
{
    Success,
    Failure,
    Running
}

// Composite nodes
public class BTSequence : BTNode
{
    private List<BTNode> children;
    private int currentChild = 0;
    
    public override BTStatus Execute(AIContext context)
    // Executes children in order until one fails or all succeed
}

public class BTSelector : BTNode
{
    private List<BTNode> children;
    private int currentChild = 0;
    
    public override BTStatus Execute(AIContext context)
    // Executes children in order until one succeeds or all fail
}

// Decorator node
public class BTInverter : BTNode
{
    private BTNode child;
    
    public override BTStatus Execute(AIContext context)
    // Inverts child result (Success ↔ Failure)
}

// Leaf nodes
public abstract class BTCondition : BTNode
{
    public abstract bool Check(AIContext context);
    
    public override BTStatus Execute(AIContext context)
    {
        return Check(context) ? BTStatus.Success : BTStatus.Failure;
    }
}

public abstract class BTAction : BTNode
{
    public abstract BTStatus Perform(AIContext context);
    
    public override BTStatus Execute(AIContext context)
    {
        return Perform(context);
    }
}
```

#### AI Context
Shared context for behavior tree execution.

```csharp
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
    
    // Blackboard for sharing data between nodes
    public Dictionary<string, object> Blackboard;
}
```

#### Reusable Condition Nodes

```csharp
public class IsEnemyInRange : BTCondition
{
    private float range;
    
    public override bool Check(AIContext context)
    // Returns true if enemy within specified range
}

public class IsHealthBelowThreshold : BTCondition
{
    private float threshold;
    
    public override bool Check(AIContext context)
    // Returns true if unit health below threshold (0-1)
}

public class HasAmmo : BTCondition
{
    public override bool Check(AIContext context)
    // Returns true if unit has ammunition/charges
}

public class IsAllyNearby : BTCondition
{
    private float range;
    
    public override bool Check(AIContext context)
    // Returns true if ally within range
}

public class HasTacticalAdvantage : BTCondition
{
    public override bool Check(AIContext context)
    // Returns true if unit type counters target type
}
```

#### Reusable Action Nodes

```csharp
public class MoveToPosition : BTAction
{
    private Vector2 targetPosition;
    
    public override BTStatus Perform(AIContext context)
    // Commands NavMeshAgent to move, returns Running until arrived
}

public class AttackTarget : BTAction
{
    public override BTStatus Perform(AIContext context)
    // Attacks current target, returns Success when attack executed
}

public class RetreatFromEnemies : BTAction
{
    private float retreatDistance;
    
    public override BTStatus Perform(AIContext context)
    // Moves away from enemies, returns Running until safe
}

public class RequestSupport : BTAction
{
    public override BTStatus Perform(AIContext context)
    // Signals nearby allies for assistance
}

public class UseAbility : BTAction
{
    private string abilityName;
    
    public override BTStatus Perform(AIContext context)
    // Uses specified unit ability if available
}
```

### 3. Utility AI System

#### Utility AI Core

```csharp
public class UtilityAI
{
    private List<UtilityAction> availableActions;
    
    // Evaluates all actions and selects best one
    public UtilityAction SelectBestAction(UtilityContext context)
    
    // Calculates scores for all actions
    public Dictionary<UtilityAction, float> ScoreActions(UtilityContext context)
}

public abstract class UtilityAction
{
    public string Name;
    public List<UtilityConsideration> Considerations;
    
    // Calculates overall score for this action
    public float CalculateScore(UtilityContext context)
    
    // Executes the action
    public abstract void Execute(UtilityContext context);
}

public abstract class UtilityConsideration
{
    public float Weight;
    public AnimationCurve ResponseCurve;
    
    // Evaluates this consideration (returns 0-1)
    public abstract float Evaluate(UtilityContext context);
    
    // Applies response curve to raw value
    public float ApplyCurve(float rawValue)
}
```

#### Utility Context

```csharp
public class UtilityContext
{
    // Commander-level context
    public int AvailableResources;
    public UnitComposition OwnForces;
    public UnitComposition EnemyForces;
    public float EnemyThreatLevel;
    public List<Vector2> StrategicPoints;
    public float TimeSinceLastAttack;
    
    // Unit-level context (if used for units)
    public BaseUnit Unit;
    public List<EnemyUnit> VisibleEnemies;
    public float HealthPercentage;
    public bool HasAlliesNearby;
}
```

#### Strategic Actions for Commander

```csharp
public class ProduceInfantryAction : UtilityAction
{
    // Considerations: resource availability, need for infantry, counter-composition
}

public class ProduceArchersAction : UtilityAction
{
    // Considerations: resource availability, need for archers, counter-composition
}

public class ProduceCavalryAction : UtilityAction
{
    // Considerations: resource availability, need for cavalry, counter-composition
}

public class LaunchAttackAction : UtilityAction
{
    // Considerations: force strength, enemy vulnerability, strategic timing
}

public class DefendPositionAction : UtilityAction
{
    // Considerations: force weakness, enemy strength, strategic value
}

public class ExpansionAction : UtilityAction
{
    // Considerations: resource availability, map control, enemy pressure
}
```

#### Utility Considerations

```csharp
public class ResourceAvailabilityConsideration : UtilityConsideration
{
    public override float Evaluate(UtilityContext context)
    // Returns 0-1 based on available resources
}

public class ForceStrengthConsideration : UtilityConsideration
{
    public override float Evaluate(UtilityContext context)
    // Returns 0-1 based on own force strength vs enemy
}

public class ThreatLevelConsideration : UtilityConsideration
{
    public override float Evaluate(UtilityContext context)
    // Returns 0-1 based on enemy threat level
}

public class CompositionNeedConsideration : UtilityConsideration
{
    private UnitType neededType;
    
    public override float Evaluate(UtilityContext context)
    // Returns 0-1 based on need for specific unit type
}
```

### 4. State Machine System

#### FSM Core Classes

```csharp
public class StateMachine
{
    private Dictionary<string, State> states;
    private State currentState;
    private State previousState;
    
    public void AddState(string name, State state)
    public void SetState(string stateName)
    public void Update(float deltaTime)
    
    // Checks transitions and changes state if needed
    private void CheckTransitions()
}

public abstract class State
{
    public string Name;
    public List<StateTransition> Transitions;
    
    // Called when entering this state
    public abstract void OnEnter(AIContext context);
    
    // Called every frame while in this state
    public abstract void OnUpdate(AIContext context, float deltaTime);
    
    // Called when exiting this state
    public abstract void OnExit(AIContext context);
}

public class StateTransition
{
    public string TargetStateName;
    public Func<AIContext, bool> Condition;
}
```

#### Unit States

```csharp
public class IdleState : State
{
    // Unit is inactive, waiting for orders or stimulus
    // Transitions: to Patrol (on patrol order), to Chase (enemy detected)
}

public class PatrolState : State
{
    private List<Vector2> patrolPoints;
    private int currentPointIndex;
    
    // Unit moves between patrol points
    // Transitions: to Chase (enemy detected), to Idle (patrol cancelled)
}

public class ChaseState : State
{
    // Unit pursues detected enemy
    // Transitions: to Attack (in range), to Patrol (lost target)
}

public class AttackState : State
{
    // Unit engages enemy in combat
    // Transitions: to Retreat (low health), to Chase (target out of range), to Patrol (target destroyed)
}

public class RetreatState : State
{
    private Vector2 retreatDirection;
    
    // Unit moves away from danger
    // Transitions: to Regroup (safe distance reached), to Attack (cornered)
}

public class DefendState : State
{
    private Vector2 defendPosition;
    
    // Unit holds position and engages approaching enemies
    // Transitions: to Attack (enemy in range), to Idle (no threats)
}

public class RegroupState : State
{
    private Vector2 rallyPoint;
    
    // Unit moves to rally point to join allies
    // Transitions: to Patrol (regrouped), to Chase (new orders)
}
```

### 5. ML-Agents Integration

#### Agent Implementation

```csharp
public class UnitMLAgent : Agent
{
    private BaseUnit unit;
    private StateMachine stateMachine;
    private BehaviorTree behaviorTree;
    
    // Observation space (what the agent sees)
    public override void CollectObservations(VectorSensor sensor)
    
    // Action space (what the agent can do)
    public override void OnActionReceived(ActionBuffers actions)
    
    // Reward function
    private void CalculateRewards()
    
    // Heuristic for testing (manual control)
    public override void Heuristic(in ActionBuffers actionsOut)
}
```

#### Observations

The agent observes:
- Own position (normalized)
- Own health (0-1)
- Own unit type (one-hot encoded)
- Nearest enemy position (normalized)
- Nearest enemy health (0-1)
- Nearest enemy type (one-hot encoded)
- Nearest ally position (normalized)
- Number of nearby allies (normalized)
- Number of nearby enemies (normalized)
- Current state (one-hot encoded)

Total: ~20-25 observations

#### Actions

Discrete action space:
- **Movement**: 8 directions + stay (9 options)
- **Combat**: attack, retreat, defend (3 options)
- **Coordination**: request support, regroup (2 options)

Total: 3 discrete branches

#### Rewards

```csharp
// Positive rewards
+1.0  Destroy enemy unit
+0.5  Damage enemy unit
+0.3  Assist ally in combat
+0.2  Maintain tactical advantage
+0.1  Survive combat encounter

// Negative rewards
-1.0  Unit destroyed
-0.5  Take damage
-0.3  Lose tactical advantage
-0.1  Waste resources (unnecessary movement)

// Shaped rewards
+0.01 per frame when maintaining good positioning
-0.01 per frame when isolated from allies
```

### 6. Integration Layer

#### AIController
Main controller that coordinates all AI systems.

```csharp
public class AIController : MonoBehaviour
{
    // AI System Components
    private UtilityAI commanderAI;
    private ThreatAnalyzer threatAnalyzer;
    private ResourceManager resourceManager;
    
    // Unit AI Management
    private Dictionary<BaseUnit, UnitAIController> unitControllers;
    
    // Strategic decision-making (runs less frequently)
    private void UpdateStrategicAI()
    
    // Tactical updates (runs every frame for active units)
    private void UpdateTacticalAI()
    
    // Distributes processing across frames
    private void DistributeProcessing()
}

public class UnitAIController
{
    public BaseUnit Unit;
    public BehaviorTree BehaviorTree;
    public StateMachine StateMachine;
    public UnitMLAgent MLAgent;  // Optional
    
    public AIContext Context;
    
    // Updates unit AI (called by AIController)
    public void UpdateAI(float deltaTime)
    
    // Receives orders from Commander AI
    public void ReceiveOrder(StrategicOrder order)
}

public struct StrategicOrder
{
    public OrderType Type;  // Attack, Defend, Patrol, Regroup
    public Vector2 TargetPosition;
    public List<BaseUnit> AssignedUnits;
    public int Priority;
}
```

## Data Models

### Unit Data

```csharp
// Existing classes (reference only)
public class BaseUnit
{
    public UnitType Type;
    public Rank Rank;
    public float Health;
    public float MaxHealth;
    public NavMeshAgent Agent;
    // ... existing properties
}

public enum UnitType
{
    Infantry,  // Strong vs Cavalry
    Archer,    // Strong vs Infantry
    Cavalry    // Strong vs Archers
}

public enum Rank
{
    Rookie,   // Base stats
    Veteran,  // +20% stats
    Elite     // +40% stats
}
```

### AI Configuration Data

```csharp
[CreateAssetMenu(fileName = "AIConfig", menuName = "AI/Configuration")]
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
```

### Debugging Data

```csharp
public class AIDebugInfo
{
    // Behavior Tree
    public List<string> ActiveBTNodes;
    public BTStatus LastBTStatus;
    
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
}
```


## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Basic AI Enhancement Properties

**Property 1: Threat score considers all factors**
*For any* enemy force composition, the calculated threat score should reflect changes in unit type composition, rank distribution, and positioning—modifying any of these factors should result in a different threat score.
**Validates: Requirements 1.1**

**Property 2: Counter-composition follows rock-paper-scissors balance**
*For any* enemy unit composition, the generated counter-composition should produce more Infantry when enemies have more Cavalry, more Cavalry when enemies have more Archers, and more Archers when enemies have more Infantry.
**Validates: Requirements 1.2, 12.1**

**Property 3: Production prioritization respects tactical needs**
*For any* game state with available resources, unit production orders should be prioritized such that higher-priority orders address more critical tactical needs (counter-units, force balance) before lower-priority orders.
**Validates: Requirements 1.3**

**Property 4: Target selection considers multiple factors**
*For any* set of potential targets, the selected target should have a higher combined score (threat level + type advantage + positioning) than non-selected targets.
**Validates: Requirements 1.4**

**Property 5: Low resources defer non-critical production**
*For any* game state where resources are below a threshold, non-critical unit production orders should be deferred or deprioritized compared to critical orders.
**Validates: Requirements 1.5**

### Behavior Tree Properties

**Property 6: Sequence execution order and termination**
*For any* Sequence node with multiple children, execution should proceed in order, stopping at the first child that returns Failure, or returning Success only when all children succeed.
**Validates: Requirements 2.3**

**Property 7: Selector execution order and termination**
*For any* Selector node with multiple children, execution should proceed in order, stopping at the first child that returns Success, or returning Failure only when all children fail.
**Validates: Requirements 2.4**

**Property 8: Condition nodes return only Success or Failure**
*For any* Condition node evaluation, the returned status should be either Success or Failure, never Running.
**Validates: Requirements 2.5**

**Property 9: Action nodes perform actions and return status**
*For any* Action node execution, the node should perform its designated action and return a valid status (Success, Failure, or Running).
**Validates: Requirements 2.6**

**Property 10: Behavior tree state persistence across frames**
*For any* behavior tree that returns Running status, resuming execution in the next frame should continue from the same node rather than restarting from the root.
**Validates: Requirements 2.7**

### Utility AI Properties

**Property 11: All actions receive scores**
*For any* set of available actions, the Utility AI should calculate and return a score for every action in the set.
**Validates: Requirements 4.1**

**Property 12: Scoring considers all specified factors**
*For any* action being scored, changing any of the factors (safety, effectiveness, resource cost, strategic value) should result in a different final score.
**Validates: Requirements 4.2**

**Property 13: Similar scores include randomization**
*For any* set of actions where multiple actions have scores within a small threshold, repeated evaluations should not always select the same action (demonstrating randomization).
**Validates: Requirements 4.3**

**Property 14: State changes trigger score recalculation**
*For any* significant game state change, action scores calculated after the change should differ from scores calculated before the change (when state actually affects scoring).
**Validates: Requirements 4.5**

### State Machine Properties

**Property 15: State entry logic executes on transition**
*For any* state transition, the entry logic of the new state should execute exactly once when the transition occurs.
**Validates: Requirements 5.2**

**Property 16: State exit logic executes on transition**
*For any* state transition, the exit logic of the old state should execute exactly once before the transition completes.
**Validates: Requirements 5.3**

**Property 17: State update logic executes while active**
*For any* state, while the state machine remains in that state, the update logic should execute every frame.
**Validates: Requirements 5.4**

**Property 18: Valid transitions occur when conditions met**
*For any* state with defined transitions, when a transition condition becomes true, the state machine should transition to the specified target state.
**Validates: Requirements 5.5**

**Property 19: Invalid transitions are prevented**
*For any* state, attempting to transition to a state that is not in the defined transitions list should be rejected or ignored.
**Validates: Requirements 5.6**

### ML-Agents Properties

**Property 20: Beneficial actions receive positive rewards**
*For any* ML-Agent action that results in beneficial outcomes (damage to enemy, survival, tactical advantage), the agent should receive a positive reward value.
**Validates: Requirements 7.3**

**Property 21: Detrimental actions receive negative rewards**
*For any* ML-Agent action that results in detrimental outcomes (taking damage, unit loss, tactical disadvantage), the agent should receive a negative reward value.
**Validates: Requirements 7.4**

### Integration Properties

**Property 22: Commander orders translate to unit actions**
*For any* strategic order issued by Commander AI, the receiving Unit AI should generate and execute tactical actions that align with the order's intent.
**Validates: Requirements 8.3**

**Property 23: Rank affects AI decision-making**
*For any* unit with different rank values (Rookie, Veteran, Elite), AI decisions should reflect the rank difference through modified behavior parameters or capabilities.
**Validates: Requirements 8.6**

### Performance Properties

**Property 24: Processing distributed across frames**
*For any* frame with multiple units requiring AI updates, not all units should update in the same frame when distribution is enabled.
**Validates: Requirements 9.2**

**Property 25: Behavior tree depth limits enforced**
*For any* behavior tree, the maximum traversal depth during execution should not exceed the configured depth limit.
**Validates: Requirements 9.3**

**Property 26: Action scores cached when state unchanged**
*For any* Utility AI evaluation, if game state has not changed since the last evaluation, the returned action scores should be retrieved from cache rather than recalculated.
**Validates: Requirements 9.4**

### Tactical Behavior Properties

**Property 27: Outnumbered units retreat to allies**
*For any* unit that is outnumbered by enemies, the unit should move toward the nearest ally group rather than engaging.
**Validates: Requirements 11.1**

**Property 28: Type advantage encourages engagement**
*For any* unit with type advantage over nearby enemies (Infantry vs Cavalry, Cavalry vs Archers, Archers vs Infantry), the unit should pursue and engage rather than avoid.
**Validates: Requirements 11.2**

**Property 29: Flanking opportunities are exploited**
*For any* unit with a flanking opportunity (enemy engaged with ally, clear path to enemy flank), the unit should attempt to move to the flanking position.
**Validates: Requirements 11.3**

**Property 30: Units support engaged allies**
*For any* unit with allies currently in combat within support range, the unit should move to assist based on its unit type capabilities.
**Validates: Requirements 11.4**

**Property 31: Defensive units maintain formation**
*For any* unit in defensive posture, the unit should maintain its position relative to other defending units and prioritize protecting high-value targets.
**Validates: Requirements 11.5**

**Property 32: Depleted units retreat**
*For any* unit with depleted ammunition or abilities, the unit should transition to retreat behavior rather than continuing to engage.
**Validates: Requirements 11.6**

### Strategic Decision Properties

**Property 33: Sufficient resources enable balanced composition**
*For any* game state with sufficient resources, the Commander AI should produce units to maintain a balanced composition across all three unit types rather than focusing on a single type.
**Validates: Requirements 12.2**

**Property 34: Vulnerable enemies trigger offensive operations**
*For any* game state where enemy forces are vulnerable (outnumbered, poor composition, low health), the Commander AI should issue offensive orders rather than defensive ones.
**Validates: Requirements 12.3**

**Property 35: Weak forces adopt defensive posture**
*For any* game state where own forces are significantly weaker than enemy forces, the Commander AI should issue defensive orders and prioritize unit production over attacks.
**Validates: Requirements 12.4**

**Property 36: Attack timing considers readiness and vulnerability**
*For any* game state, the Commander AI should score attack actions higher when both force readiness is high and enemy vulnerability is high, compared to when either factor is low.
**Validates: Requirements 12.5**

**Property 37: Strategy adapts to engagement outcomes**
*For any* sequence of engagements, if previous engagements failed, the Commander AI should modify its strategy (composition, timing, positioning) in subsequent decisions.
**Validates: Requirements 12.6**

## Error Handling

### Behavior Tree Error Handling

**Null Reference Protection**:
- All behavior tree nodes must check for null context, unit, or agent references
- Return Failure status if critical references are missing
- Log warnings for debugging when null references are encountered

**Infinite Loop Prevention**:
- Enforce maximum tree depth limit (configurable, default 10)
- Track node execution count per frame
- Abort tree execution if depth limit exceeded
- Log error with tree structure for debugging

**Invalid State Handling**:
- Validate game state before node execution
- Handle destroyed units gracefully (abort tree execution)
- Handle missing targets by returning Failure and allowing tree to select alternative behavior

### State Machine Error Handling

**Invalid Transition Handling**:
- Log warning when invalid transition is attempted
- Remain in current state rather than entering undefined state
- Provide debugging information about attempted transition

**Missing State Handling**:
- Validate all transition target states exist during initialization
- Throw exception during setup if invalid state references found
- Prevent runtime errors by catching issues early

**State Execution Errors**:
- Wrap state update logic in try-catch blocks
- Log errors but maintain state machine stability
- Optionally transition to safe "Error" state on critical failures

### Utility AI Error Handling

**Division by Zero Protection**:
- Validate consideration values before normalization
- Handle zero-sum scores by falling back to default action
- Clamp values to valid ranges (0-1) after curve application

**No Valid Actions**:
- Handle case where all actions score zero or negative
- Provide fallback "Idle" action as safety net
- Log warning when no valid actions available

**Consideration Evaluation Errors**:
- Catch exceptions in consideration evaluation
- Return neutral score (0.5) for failed considerations
- Log errors for debugging without crashing AI

### ML-Agents Error Handling

**Observation Space Errors**:
- Validate observation vector size matches configured space
- Clamp observation values to valid ranges
- Handle missing data by providing default observations

**Action Space Errors**:
- Validate action indices are within valid ranges
- Ignore invalid actions and maintain previous behavior
- Log warnings for debugging training issues

**Reward Calculation Errors**:
- Clamp rewards to reasonable ranges (-10 to +10)
- Handle edge cases (division by zero, null references)
- Ensure rewards are finite (no NaN or Infinity)

### Integration Error Handling

**NavMesh Errors**:
- Check if NavMeshAgent is enabled before issuing movement commands
- Handle unreachable destinations by selecting alternative positions
- Fall back to direct movement if NavMesh fails

**Unit Reference Errors**:
- Validate unit references before accessing properties
- Remove destroyed units from tracking lists
- Handle race conditions where units are destroyed mid-update

**Performance Degradation**:
- Monitor AI processing time per frame
- Automatically reduce update frequency if frame time exceeds threshold
- Log performance warnings for optimization

## Testing Strategy

### Dual Testing Approach

This AI system requires both **unit tests** and **property-based tests** for comprehensive coverage:

- **Unit tests** verify specific examples, edge cases, and integration points
- **Property-based tests** verify universal properties across all inputs
- Together they ensure both concrete correctness and general behavior

### Unit Testing Focus

Unit tests should cover:

1. **Specific Examples**:
   - Specific threat analysis scenarios (e.g., 5 Infantry + 3 Archers)
   - Specific state transitions (e.g., Patrol → Chase when enemy detected)
   - Specific behavior tree compositions (e.g., "Attack if enemy in range, else patrol")

2. **Edge Cases**:
   - Empty enemy lists
   - Zero resources
   - Maximum unit counts
   - Destroyed units mid-execution
   - NavMesh unreachable positions

3. **Integration Points**:
   - Commander AI → Unit AI communication
   - Behavior Tree ↔ State Machine coordination
   - AI System → NavMeshAgent interaction
   - ML-Agents → existing AI integration

4. **Error Conditions**:
   - Null references
   - Invalid state transitions
   - Malformed behavior trees
   - Out-of-range values

### Property-Based Testing Configuration

**Testing Library**: Use appropriate PBT library for C#:
- **FsCheck** (recommended for C#/Unity)
- **CsCheck** (alternative)

**Test Configuration**:
- Minimum **100 iterations** per property test
- Each test must reference its design document property
- Tag format: `// Feature: advanced-ai-system, Property {N}: {property text}`

**Example Property Test Structure**:

```csharp
[Test]
public void Property_2_CounterComposition_FollowsRockPaperScissors()
{
    // Feature: advanced-ai-system, Property 2: Counter-composition follows rock-paper-scissors balance
    
    Prop.ForAll<UnitComposition>(enemyComp =>
    {
        var counterComp = threatAnalyzer.GetCounterComposition(enemyComp);
        
        // More enemy Cavalry → More friendly Infantry
        if (enemyComp.CavalryCount > enemyComp.InfantryCount + enemyComp.ArcherCount)
            return counterComp.InfantryCount > counterComp.CavalryCount;
        
        // More enemy Archers → More friendly Cavalry
        if (enemyComp.ArcherCount > enemyComp.InfantryCount + enemyComp.CavalryCount)
            return counterComp.CavalryCount > counterComp.ArcherCount;
        
        // More enemy Infantry → More friendly Archers
        if (enemyComp.InfantryCount > enemyComp.CavalryCount + enemyComp.ArcherCount)
            return counterComp.ArcherCount > counterComp.InfantryCount;
        
        return true; // Balanced composition
    }).QuickCheckThrowOnFailure();
}
```

### Test Data Generators

Property tests require generators for:

1. **Unit Compositions**: Random distributions of Infantry, Archer, Cavalry with various ranks
2. **Game States**: Random resource levels, unit positions, health values
3. **Behavior Trees**: Random valid tree structures with various node types
4. **State Machines**: Random state configurations with valid transitions
5. **AI Contexts**: Random combat scenarios with units, enemies, and tactical situations

### Testing Priorities

**Phase 1: Core Systems** (Must have property tests)
- Behavior Tree execution (Properties 6-10)
- State Machine transitions (Properties 15-19)
- Utility AI scoring (Properties 11-14)

**Phase 2: AI Logic** (Must have property tests)
- Threat analysis and counter-composition (Properties 1-2)
- Target selection and prioritization (Properties 3-5)
- Tactical behaviors (Properties 27-32)

**Phase 3: Integration** (Mix of unit and property tests)
- Commander ↔ Unit communication (Property 22)
- System integration (Property 23)
- Performance optimizations (Properties 24-26)

**Phase 4: Strategic AI** (Property tests with complex scenarios)
- Strategic decision-making (Properties 33-37)
- ML-Agents rewards (Properties 20-21)

### Performance Testing

While not covered by property tests, performance should be validated through:

- **Load tests**: 50+ units with AI enabled
- **Profiling**: Measure AI processing time per frame
- **Stress tests**: Maximum unit counts with all AI systems active
- **Frame time monitoring**: Ensure <16ms for 60 FPS target

### Integration Testing

Integration tests should verify:

- Complete gameplay scenarios (spawn units → engage → retreat → regroup)
- Commander issues orders → Units execute → Feedback to Commander
- ML-Agents training loop (observation → action → reward → learning)
- All AI systems working together in realistic game situations

### Debugging Support

All AI systems include debugging capabilities:

- **Behavior Tree Visualization**: Active nodes, execution flow, status
- **State Machine Display**: Current state, available transitions, history
- **Utility AI Scores**: Action scores, consideration values, selection reasoning
- **Performance Metrics**: Processing time, update frequency, cache hit rates

Enable debugging through `AIConfiguration.enableBTDebugging` and similar flags.
