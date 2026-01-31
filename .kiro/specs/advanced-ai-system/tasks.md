# Implementation Plan: Advanced AI System

## Overview

This implementation plan breaks down the Advanced AI System into incremental, testable steps. The approach follows a layered implementation strategy:

1. **Level 1**: Enhanced basic AI (threat analysis, resource management, target prioritization)
2. **Level 2**: Behavior Tree framework and reusable nodes
3. **Level 3**: Utility AI system for strategic decision-making
4. **Level 4**: State Machine system for unit behavioral states
5. **Level 5**: ML-Agents integration (optional)
6. **Integration**: Wire all systems together with Commander and Unit AI controllers

Each level builds upon the previous one, ensuring incremental progress with testable milestones.

## Tasks

- [x] 1. Set up AI system foundation and data structures
  - Create directory structure: `Assets/Scripts/AI/` with subdirectories for each system
  - Define core data structures: `AIContext`, `UnitComposition`, `ThreatAssessment`
  - Create `AIConfiguration` ScriptableObject for tunable parameters
  - Set up FsCheck or CsCheck for property-based testing
  - _Requirements: 8.4, 8.5, 8.7, 10.4_

- [x] 2. Implement Level 1: Enhanced Basic AI
  - [x] 2.1 Implement ThreatAnalyzer class
    - Create `ThreatAnalyzer.cs` with threat calculation methods
    - Implement `AnalyzeEnemyThreat()` considering composition, rank, and positioning
    - Implement `CalculateThreatScore()` with weighted factors
    - Implement `GetCounterComposition()` using rock-paper-scissors logic
    - _Requirements: 1.1, 1.2, 12.1_
  
  - [x] 2.2 Write property test for threat score calculation
    - **Property 1: Threat score considers all factors**
    - **Validates: Requirements 1.1**
  
  - [x] 2.3 Write property test for counter-composition
    - **Property 2: Counter-composition follows rock-paper-scissors balance**
    - **Validates: Requirements 1.2, 12.1**
  
  - [x] 2.4 Implement ResourceManager class
    - Create `ResourceManager.cs` with resource allocation methods
    - Implement `PrioritizeProduction()` based on tactical needs
    - Implement `CanAfford()` and `AllocateResources()` methods
    - _Requirements: 1.3, 1.5_
  
  - [x] 2.5 Write property test for production prioritization
    - **Property 3: Production prioritization respects tactical needs**
    - **Validates: Requirements 1.3**
  
  - [x] 2.6 Write property test for low resource deferral
    - **Property 5: Low resources defer non-critical production**
    - **Validates: Requirements 1.5**
  
  - [x] 2.7 Implement TargetPrioritizer class
    - Create `TargetPrioritizer.cs` with target selection methods
    - Implement `SelectTarget()` considering threat, type advantage, and positioning
    - Implement `CalculateTargetPriority()` with multi-factor scoring
    - _Requirements: 1.4_
  
  - [x] 2.8 Write property test for target selection
    - **Property 4: Target selection considers multiple factors**
    - **Validates: Requirements 1.4**

- [ ] 3. Checkpoint - Verify Level 1 functionality
  - Ensure all Level 1 tests pass
  - Verify threat analysis produces sensible results with test scenarios
  - Ask the user if questions arise

- [ ] 4. Implement Level 2: Behavior Tree System
  - [ ] 4.1 Create behavior tree core classes
    - Create `BTNode.cs` abstract base class with `Execute()` method
    - Define `BTStatus` enum (Success, Failure, Running)
    - Create `AIContext.cs` with unit, agent, and blackboard data
    - _Requirements: 2.1, 2.2_
  
  - [ ] 4.2 Implement composite nodes
    - Create `BTSequence.cs` that executes children until one fails
    - Create `BTSelector.cs` that executes children until one succeeds
    - Create `BTInverter.cs` decorator node
    - Implement state tracking for Running status across frames
    - _Requirements: 2.3, 2.4, 2.7_
  
  - [ ] 4.3 Write property test for Sequence execution
    - **Property 6: Sequence execution order and termination**
    - **Validates: Requirements 2.3**
  
  - [ ] 4.4 Write property test for Selector execution
    - **Property 7: Selector execution order and termination**
    - **Validates: Requirements 2.4**
  
  - [ ] 4.5 Write property test for tree state persistence
    - **Property 10: Behavior tree state persistence across frames**
    - **Validates: Requirements 2.7**
  
  - [ ] 4.6 Implement leaf node base classes
    - Create `BTCondition.cs` abstract class with `Check()` method
    - Create `BTAction.cs` abstract class with `Perform()` method
    - Ensure conditions return only Success/Failure
    - Ensure actions can return Success/Failure/Running
    - _Requirements: 2.5, 2.6_
  
  - [ ] 4.7 Write property test for condition nodes
    - **Property 8: Condition nodes return only Success or Failure**
    - **Validates: Requirements 2.5**
  
  - [ ] 4.8 Write property test for action nodes
    - **Property 9: Action nodes perform actions and return status**
    - **Validates: Requirements 2.6**

- [ ] 5. Implement reusable behavior tree nodes
  - [ ] 5.1 Create condition nodes
    - Implement `IsEnemyInRange.cs` condition
    - Implement `IsHealthBelowThreshold.cs` condition
    - Implement `HasAmmo.cs` condition
    - Implement `IsAllyNearby.cs` condition
    - Implement `HasTacticalAdvantage.cs` condition
    - _Requirements: 3.1_
  
  - [ ] 5.2 Create action nodes
    - Implement `MoveToPosition.cs` action (uses NavMeshAgent)
    - Implement `AttackTarget.cs` action
    - Implement `RetreatFromEnemies.cs` action
    - Implement `RequestSupport.cs` action
    - Implement `UseAbility.cs` action
    - _Requirements: 3.2, 8.5_
  
  - [ ] 5.3 Add parameterization support
    - Ensure nodes accept configuration values (ranges, thresholds)
    - Make nodes reusable across different unit types
    - _Requirements: 3.3, 3.4_
  
  - [ ] 5.4 Write unit tests for reusable nodes
    - Test each condition node with various game states
    - Test each action node with various contexts
    - Verify parameterization works correctly
    - _Requirements: 3.1, 3.2, 3.3, 3.4_
  
  - [ ] 5.5 Implement behavior tree debugging
    - Add logging for node execution when debugging enabled
    - Track active nodes and execution flow
    - Store debug info in `AIDebugInfo` class
    - _Requirements: 3.5, 10.1_

- [ ] 6. Checkpoint - Verify Level 2 functionality
  - Ensure all behavior tree tests pass
  - Create sample behavior trees and verify execution
  - Test tree state persistence across multiple frames
  - Ask the user if questions arise

- [ ] 7. Implement Level 3: Utility AI System
  - [ ] 7.1 Create utility AI core classes
    - Create `UtilityAI.cs` with action selection logic
    - Create `UtilityAction.cs` abstract base class
    - Create `UtilityConsideration.cs` abstract base class
    - Create `UtilityContext.cs` with commander and unit context data
    - _Requirements: 4.1_
  
  - [ ] 7.2 Implement action scoring system
    - Implement `CalculateScore()` in UtilityAction (combines considerations)
    - Implement `Evaluate()` in UtilityConsideration (returns 0-1)
    - Implement `ApplyCurve()` with support for linear, exponential, logistic curves
    - Add randomization for similar scores
    - _Requirements: 4.2, 4.3, 4.6_
  
  - [ ] 7.3 Write property test for action scoring
    - **Property 11: All actions receive scores**
    - **Validates: Requirements 4.1**
  
  - [ ] 7.4 Write property test for multi-factor scoring
    - **Property 12: Scoring considers all specified factors**
    - **Validates: Requirements 4.2**
  
  - [ ] 7.5 Write property test for score randomization
    - **Property 13: Similar scores include randomization**
    - **Validates: Requirements 4.3**
  
  - [ ] 7.6 Implement strategic actions for Commander AI
    - Create `ProduceInfantryAction.cs`, `ProduceArchersAction.cs`, `ProduceCavalryAction.cs`
    - Create `LaunchAttackAction.cs`, `DefendPositionAction.cs`, `ExpansionAction.cs`
    - Each action should have relevant considerations
    - _Requirements: 4.4, 12.2, 12.3, 12.4, 12.5_
  
  - [ ] 7.7 Implement utility considerations
    - Create `ResourceAvailabilityConsideration.cs`
    - Create `ForceStrengthConsideration.cs`
    - Create `ThreatLevelConsideration.cs`
    - Create `CompositionNeedConsideration.cs`
    - _Requirements: 4.2_
  
  - [ ] 7.8 Implement score caching and recalculation
    - Add caching mechanism for action scores
    - Detect significant game state changes
    - Invalidate cache and recalculate when needed
    - _Requirements: 4.5, 9.4_
  
  - [ ] 7.9 Write property test for state change recalculation
    - **Property 14: State changes trigger score recalculation**
    - **Validates: Requirements 4.5**
  
  - [ ] 7.10 Write property test for score caching
    - **Property 26: Action scores cached when state unchanged**
    - **Validates: Requirements 9.4**
  
  - [ ] 7.11 Implement utility AI debugging
    - Add logging for action scores and selection reasoning
    - Display consideration values when debugging enabled
    - _Requirements: 10.3_

- [ ] 8. Checkpoint - Verify Level 3 functionality
  - Ensure all utility AI tests pass
  - Test strategic action selection with various game states
  - Verify score caching improves performance
  - Ask the user if questions arise

- [ ] 9. Implement Level 4: State Machine System
  - [ ] 9.1 Create state machine core classes
    - Create `StateMachine.cs` with state management and transitions
    - Create `State.cs` abstract base class with lifecycle methods
    - Create `StateTransition.cs` with condition and target state
    - _Requirements: 5.1_
  
  - [ ] 9.2 Implement state lifecycle management
    - Implement `OnEnter()`, `OnUpdate()`, `OnExit()` in State base class
    - Ensure entry logic executes on state transition
    - Ensure exit logic executes before leaving state
    - Ensure update logic executes every frame while in state
    - _Requirements: 5.2, 5.3, 5.4_
  
  - [ ] 9.3 Write property test for state entry logic
    - **Property 15: State entry logic executes on transition**
    - **Validates: Requirements 5.2**
  
  - [ ] 9.4 Write property test for state exit logic
    - **Property 16: State exit logic executes on transition**
    - **Validates: Requirements 5.3**
  
  - [ ] 9.5 Write property test for state update logic
    - **Property 17: State update logic executes while active**
    - **Validates: Requirements 5.4**
  
  - [ ] 9.6 Implement state transition system
    - Implement `CheckTransitions()` to evaluate conditions
    - Implement `SetState()` to perform transitions
    - Validate transitions and prevent invalid ones
    - _Requirements: 5.5, 5.6_
  
  - [ ] 9.7 Write property test for valid transitions
    - **Property 18: Valid transitions occur when conditions met**
    - **Validates: Requirements 5.5**
  
  - [ ] 9.8 Write property test for invalid transition prevention
    - **Property 19: Invalid transitions are prevented**
    - **Validates: Requirements 5.6**
  
  - [ ] 9.9 Implement hierarchical state support
    - Add support for nested sub-states
    - Implement parent-child state relationships
    - _Requirements: 5.7_

- [ ] 10. Implement unit states
  - [ ] 10.1 Create combat states
    - Implement `IdleState.cs` with basic waiting behavior
    - Implement `PatrolState.cs` with waypoint navigation
    - Implement `ChaseState.cs` with enemy pursuit
    - Implement `AttackState.cs` with combat engagement
    - _Requirements: 5.1, 6.1, 6.2, 6.4_
  
  - [ ] 10.2 Create tactical states
    - Implement `RetreatState.cs` with escape behavior
    - Implement `DefendState.cs` with position holding
    - Implement `RegroupState.cs` with rally point movement
    - _Requirements: 5.1, 6.3, 6.5, 6.6, 6.7_
  
  - [ ] 10.3 Define state transitions
    - Add transitions for Patrol → Chase (enemy detected)
    - Add transitions for Chase → Attack (in range)
    - Add transitions for Attack → Retreat (low health)
    - Add transitions for Retreat → Regroup (safe distance)
    - Add transitions for Regroup → Patrol (regrouped)
    - Add transitions for Defend → Idle (no threats, timeout)
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7_
  
  - [ ] 10.4 Write unit tests for state transitions
    - Test each specific transition scenario
    - Verify transition conditions work correctly
    - Test timeout-based transitions
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7_
  
  - [ ] 10.5 Implement state machine debugging
    - Add logging for state changes when debugging enabled
    - Display current state and available transitions
    - Track state history
    - _Requirements: 10.2_

- [ ] 11. Checkpoint - Verify Level 4 functionality
  - Ensure all state machine tests pass
  - Test state transitions with various game scenarios
  - Verify state lifecycle methods execute correctly
  - Ask the user if questions arise

- [ ] 12. Implement tactical behavior properties
  - [ ] 12.1 Implement outnumbered retreat behavior
    - Add logic to detect outnumbered situations
    - Implement retreat coordination toward allies
    - _Requirements: 11.1_
  
  - [ ] 12.2 Write property test for outnumbered retreat
    - **Property 27: Outnumbered units retreat to allies**
    - **Validates: Requirements 11.1**
  
  - [ ] 12.3 Implement type advantage engagement
    - Add logic to detect type advantage
    - Implement pursuit behavior when advantaged
    - _Requirements: 11.2_
  
  - [ ] 12.4 Write property test for type advantage engagement
    - **Property 28: Type advantage encourages engagement**
    - **Validates: Requirements 11.2**
  
  - [ ] 12.5 Implement flanking behavior
    - Add logic to detect flanking opportunities
    - Implement flanking movement patterns
    - _Requirements: 11.3_
  
  - [ ] 12.6 Write property test for flanking
    - **Property 29: Flanking opportunities are exploited**
    - **Validates: Requirements 11.3**
  
  - [ ] 12.7 Implement ally support behavior
    - Add logic to detect allies in combat
    - Implement support movement based on unit type
    - _Requirements: 11.4_
  
  - [ ] 12.8 Write property test for ally support
    - **Property 30: Units support engaged allies**
    - **Validates: Requirements 11.4**
  
  - [ ] 12.9 Implement defensive formation behavior
    - Add logic to maintain formation positions
    - Implement high-value target protection
    - _Requirements: 11.5_
  
  - [ ] 12.10 Write property test for defensive formation
    - **Property 31: Defensive units maintain formation**
    - **Validates: Requirements 11.5**
  
  - [ ] 12.11 Implement resource depletion retreat
    - Add logic to detect depleted ammunition/abilities
    - Implement retreat behavior when depleted
    - _Requirements: 11.6_
  
  - [ ] 12.12 Write property test for depleted retreat
    - **Property 32: Depleted units retreat**
    - **Validates: Requirements 11.6**

- [ ] 13. Implement strategic decision properties
  - [ ] 13.1 Implement balanced composition logic
    - Add logic to maintain balanced army composition
    - Integrate with ResourceManager and UtilityAI
    - _Requirements: 12.2_
  
  - [ ] 13.2 Write property test for balanced composition
    - **Property 33: Sufficient resources enable balanced composition**
    - **Validates: Requirements 12.2**
  
  - [ ] 13.3 Implement vulnerability detection and offensive operations
    - Add logic to detect enemy vulnerability
    - Implement offensive order generation
    - _Requirements: 12.3_
  
  - [ ] 13.4 Write property test for offensive operations
    - **Property 34: Vulnerable enemies trigger offensive operations**
    - **Validates: Requirements 12.3**
  
  - [ ] 13.5 Implement weakness detection and defensive posture
    - Add logic to detect own force weakness
    - Implement defensive order generation
    - _Requirements: 12.4_
  
  - [ ] 13.6 Write property test for defensive posture
    - **Property 35: Weak forces adopt defensive posture**
    - **Validates: Requirements 12.4**
  
  - [ ] 13.7 Implement attack timing logic
    - Add logic to evaluate force readiness
    - Combine readiness and vulnerability for timing decisions
    - _Requirements: 12.5_
  
  - [ ] 13.8 Write property test for attack timing
    - **Property 36: Attack timing considers readiness and vulnerability**
    - **Validates: Requirements 12.5**
  
  - [ ] 13.9 Implement strategy adaptation
    - Add logic to track engagement outcomes
    - Implement strategy modification based on results
    - _Requirements: 12.6_
  
  - [ ] 13.10 Write property test for strategy adaptation
    - **Property 37: Strategy adapts to engagement outcomes**
    - **Validates: Requirements 12.6**

- [ ] 14. Checkpoint - Verify tactical and strategic behaviors
  - Ensure all tactical and strategic property tests pass
  - Test behaviors in realistic game scenarios
  - Verify AI makes intelligent decisions
  - Ask the user if questions arise

- [ ] 15. Implement Level 5: ML-Agents Integration (Optional)
  - [ ] 15.1 Set up Unity ML-Agents package
    - Install ML-Agents package via Package Manager
    - Configure ML-Agents settings
    - Create training configuration YAML file
    - _Requirements: 7.7_
  
  - [ ] 15.2 Implement UnitMLAgent class
    - Create `UnitMLAgent.cs` inheriting from Agent
    - Implement observation space (positions, health, types, states)
    - Implement action space (movement, combat, coordination)
    - _Requirements: 7.1, 7.2_
  
  - [ ] 15.3 Implement reward system
    - Implement positive rewards (destroy enemy, damage, assist, survive)
    - Implement negative rewards (destroyed, take damage, lose advantage)
    - Implement shaped rewards (positioning, isolation)
    - _Requirements: 7.3, 7.4_
  
  - [ ] 15.4 Write property test for positive rewards
    - **Property 20: Beneficial actions receive positive rewards**
    - **Validates: Requirements 7.3**
  
  - [ ] 15.5 Write property test for negative rewards
    - **Property 21: Detrimental actions receive negative rewards**
    - **Validates: Requirements 7.4**
  
  - [ ] 15.6 Implement heuristic for testing
    - Create manual control heuristic for debugging
    - Allow switching between ML and heuristic modes
    - _Requirements: 7.5_
  
  - [ ] 15.7 Integrate ML-Agent with existing AI
    - Ensure ML-Agent works alongside behavior trees and state machines
    - Implement training data collection
    - Ensure non-ML units continue working
    - _Requirements: 7.5, 7.6, 7.7_
  
  - [ ] 15.8 Write unit tests for ML-Agent integration
    - Test observation space correctness
    - Test action space execution
    - Test reward calculation
    - Verify coexistence with non-ML units
    - _Requirements: 7.1, 7.2, 7.5, 7.7_

- [ ] 16. Implement integration layer
  - [ ] 16.1 Create AIController class
    - Create `AIController.cs` as main coordinator
    - Implement strategic AI update loop (runs less frequently)
    - Implement tactical AI update loop (runs every frame)
    - Integrate ThreatAnalyzer, ResourceManager, UtilityAI
    - _Requirements: 8.1, 8.4_
  
  - [ ] 16.2 Create UnitAIController class
    - Create `UnitAIController.cs` for individual unit AI
    - Integrate BehaviorTree and StateMachine
    - Implement order reception from Commander AI
    - Maintain AIContext for each unit
    - _Requirements: 8.2, 8.3_
  
  - [ ] 16.3 Implement Commander-to-Unit communication
    - Define `StrategicOrder` struct (type, position, units, priority)
    - Implement order issuing from Commander AI
    - Implement order translation to tactical actions in Unit AI
    - _Requirements: 8.1, 8.3_
  
  - [ ] 16.4 Write property test for order translation
    - **Property 22: Commander orders translate to unit actions**
    - **Validates: Requirements 8.3**
  
  - [ ] 16.5 Implement rank system integration
    - Ensure AI considers unit rank in decision-making
    - Modify behavior parameters based on rank
    - _Requirements: 8.6_
  
  - [ ] 16.6 Write property test for rank integration
    - **Property 23: Rank affects AI decision-making**
    - **Validates: Requirements 8.6**
  
  - [ ] 16.7 Verify compatibility with existing classes
    - Test integration with BaseUnit, PlayerUnit, EnemyUnit
    - Ensure NavMeshAgent is used for all movement
    - Verify unit type balance is respected
    - _Requirements: 8.4, 8.5, 8.7_
  
  - [ ] 16.8 Write unit tests for system integration
    - Test Commander AI uses UtilityAI correctly
    - Test Unit AI uses BehaviorTree and StateMachine correctly
    - Test compatibility with existing classes
    - _Requirements: 8.1, 8.2, 8.4, 8.5, 8.7_

- [ ] 17. Implement performance optimizations
  - [ ] 17.1 Implement distributed processing
    - Add frame distribution for unit AI updates
    - Ensure not all units update in same frame
    - Implement update scheduling based on priority
    - _Requirements: 9.2_
  
  - [ ] 17.2 Write property test for distributed processing
    - **Property 24: Processing distributed across frames**
    - **Validates: Requirements 9.2**
  
  - [ ] 17.3 Implement behavior tree depth limiting
    - Add maximum depth tracking during tree traversal
    - Abort execution if depth limit exceeded
    - Log errors for debugging
    - _Requirements: 9.3_
  
  - [ ] 17.4 Write property test for depth limiting
    - **Property 25: Behavior tree depth limits enforced**
    - **Validates: Requirements 9.3**
  
  - [ ] 17.5 Implement performance monitoring
    - Add AI processing time tracking
    - Report timing data when monitoring enabled
    - Implement automatic frequency reduction if needed
    - _Requirements: 9.6_
  
  - [ ] 17.6 Write unit tests for performance features
    - Test distributed processing works correctly
    - Test depth limiting prevents infinite recursion
    - Test performance monitoring reports data
    - _Requirements: 9.2, 9.3, 9.6_

- [ ] 18. Implement debugging and tuning features
  - [ ] 18.1 Implement AIDebugInfo class
    - Create `AIDebugInfo.cs` with debug data structures
    - Track active BT nodes, current state, action scores
    - Track processing time and update frequency
    - _Requirements: 10.1, 10.2, 10.3_
  
  - [ ] 18.2 Implement difficulty presets
    - Create difficulty preset configurations (Easy, Medium, Hard)
    - Implement preset application to AI parameters
    - Allow runtime difficulty adjustment
    - _Requirements: 10.5_
  
  - [ ] 18.3 Implement decision logging
    - Add optional logging for AI decisions
    - Log decision rationale for analysis
    - Include context and reasoning in logs
    - _Requirements: 10.6_
  
  - [ ] 18.4 Write unit tests for debugging features
    - Test debug info is populated correctly
    - Test difficulty presets change parameters
    - Test decision logging works when enabled
    - _Requirements: 10.1, 10.2, 10.3, 10.5, 10.6_

- [ ] 19. Implement error handling
  - [ ] 19.1 Add behavior tree error handling
    - Add null reference checks in all nodes
    - Implement depth limit enforcement
    - Handle destroyed units gracefully
    - Add try-catch blocks for node execution
  
  - [ ] 19.2 Add state machine error handling
    - Validate transitions during initialization
    - Handle invalid transition attempts
    - Wrap state update logic in try-catch
    - Implement optional error state
  
  - [ ] 19.3 Add utility AI error handling
    - Add division by zero protection
    - Handle no valid actions scenario
    - Catch consideration evaluation errors
    - Clamp values to valid ranges
  
  - [ ] 19.4 Add ML-Agents error handling
    - Validate observation vector size
    - Validate action indices
    - Clamp rewards to reasonable ranges
    - Handle NaN and Infinity values
  
  - [ ] 19.5 Add integration error handling
    - Check NavMeshAgent before movement commands
    - Handle unreachable destinations
    - Validate unit references before access
    - Monitor and respond to performance degradation
  
  - [ ] 19.6 Write unit tests for error handling
    - Test null reference handling
    - Test invalid state transitions
    - Test division by zero protection
    - Test NavMesh error handling

- [ ] 20. Final integration and testing
  - [ ] 20.1 Create sample behavior trees for each unit type
    - Create Infantry behavior tree
    - Create Archer behavior tree
    - Create Cavalry behavior tree
    - Wire trees to unit AI controllers
  
  - [ ] 20.2 Configure Commander AI with strategic actions
    - Add all strategic actions to Commander's UtilityAI
    - Configure considerations and weights
    - Test strategic decision-making
  
  - [ ] 20.3 Wire all systems together
    - Connect AIController to game manager
    - Initialize all AI systems on game start
    - Ensure proper cleanup on game end
  
  - [ ]* 20.4 Write integration tests
    - Test complete gameplay scenarios
    - Test Commander issues orders → Units execute
    - Test all AI systems working together
    - Test with 50+ units for performance validation
  
  - [ ] 20.5 Create AI configuration presets
    - Create default configuration
    - Create aggressive AI configuration
    - Create defensive AI configuration
    - Create balanced AI configuration

- [ ] 21. Final checkpoint - Complete system verification
  - Run all unit tests and property tests
  - Test complete gameplay with all AI systems active
  - Verify performance with 50+ units
  - Verify debugging features work correctly
  - Ensure all requirements are met
  - Ask the user if questions arise

## Notes

- All tasks are required for comprehensive AI system implementation
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation at each level
- Property tests validate universal correctness properties (minimum 100 iterations each)
- Unit tests validate specific examples, edge cases, and integration points
- ML-Agents (Level 5) is entirely optional and can be skipped
- All AI systems are designed to work independently and together
- Performance optimizations are critical for supporting 50+ units
