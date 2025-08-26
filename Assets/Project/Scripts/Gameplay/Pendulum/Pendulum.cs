using UnityEngine;

[DisallowMultipleComponent]
public class Pendulum : MonoBehaviour, GameStateGameplayHandler, GameStateGameOverHandler {
  [field: SerializeField] public PendulumMathConfig Config { get; private set; }
  [field: SerializeField] public Transform Anchor { get; private set; }
  [field: SerializeField] public Rigidbody2D BobAnchor { get; private set; }
  [field: SerializeField] public GameObject BobPrefab { get; private set; }
  [field: SerializeField] public Vector2 BobInitPosition { get; private set; }
  [field: SerializeField] public LineRenderer StringRenderer { get; private set; }
  private StateMachine stateMachine;
  private PendulumInactiveState pendulumInactiveState;
  private PendulumActiveState pendulumActiveState;

  private void Awake() {
    stateMachine = new StateMachine();
    pendulumInactiveState = new PendulumInactiveState(this);
    pendulumActiveState = new PendulumActiveState(this);
  }

  private void Start() => stateMachine.SetState(pendulumInactiveState);

  private void Update() => stateMachine.Update();

  private void FixedUpdate() => stateMachine.FixedUpdate();

  public void HandleGameplay() => stateMachine.SetState(pendulumActiveState);

  public void HandleGameOver() => stateMachine.SetState(pendulumInactiveState);
}