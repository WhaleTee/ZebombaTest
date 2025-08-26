using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PendulumBob : MonoBehaviour {
  #region events

  private static event Action bobDisconnectedEvent;
  private static void InvokeBobDisconnected() => bobDisconnectedEvent?.Invoke();
  public static void RegisterBobDisconnectedListener(Action callback) => bobDisconnectedEvent += callback;

  #endregion

  [SerializeField] private PendulumMathConfig config;
  [SerializeField] private ColorPoints[] bobColors;
  [SerializeField] private GameObject destroyParticlesPrefab;
  [field: SerializeField] public float DisconnectedBobDisappearingDelay { get; private set; } = 1.75f;
  [field: SerializeField] public float DisconnectedBobDisappearingDuration { get; private set; } = .75f;

  private Rigidbody2D mainRigidbody;
  private CircleCollider2D mainCollider;
  private HingeJoint2D mainJoint;
  private SpriteRenderer mainRenderer;
  private StateMachine stateMachine;
  private BobDisconnectedState bobDisconnectedState;
  private BobConnectedState bobConnectedState;

  private void Awake() {
    mainRigidbody = GetComponent<Rigidbody2D>();
    mainCollider = GetComponent<CircleCollider2D>();
    mainRenderer = GetComponent<SpriteRenderer>();
    mainJoint = GetComponent<HingeJoint2D>();
    stateMachine = new StateMachine();
  }

  private void Update() => stateMachine.Update();

  private void FixedUpdate() => stateMachine.FixedUpdate();

  public void Initialize(Rigidbody2D anchor, BobState state) {
    bobDisconnectedState = new BobDisconnectedState(this, mainRigidbody, mainJoint, mainCollider);
    bobConnectedState = new BobConnectedState(anchor, mainJoint, mainCollider);
    mainRenderer.color = bobColors.Select(bc => bc.Color).ToArray()[Random.Range(0, bobColors.Length)];

    SetState(state);
  }

  private void SetState(BobState state) {
    switch (state) {
      case BobState.Connected:
        stateMachine.SetState(bobConnectedState);
        break;
      case BobState.Disconnected:
        stateMachine.SetState(bobDisconnectedState);
        break;
    }
  }

  public void Destroy() {
    var particles = ObjectPoolManager.SpawnObject(
      destroyParticlesPrefab,
      transform.position,
      Quaternion.identity,
      ObjectPoolManager.PoolType.ParticleSystems
    );

    particles.GetComponent<BobDestroyParticles>().Play(mainRenderer.color);
    ObjectPoolManager.ReturnObjectToPool(gameObject);
  }

  public void ConnectBob() => stateMachine.SetState(bobConnectedState);

  public void DisconnectBob() {
    stateMachine.SetState(bobDisconnectedState);
    InvokeBobDisconnected();
  }

  public Color GetColor() => mainRenderer.color;
}