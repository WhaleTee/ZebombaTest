using UnityEngine;
using UnityEngine.InputSystem;

public class PendulumActiveState : BaseState {
  private readonly Pendulum pendulum;
  private PendulumBob bob;
  private float bobAnchorCurrentAngle;
  private float bobAnchorAngularVelocity;

  public PendulumActiveState(Pendulum pendulum) => this.pendulum = pendulum;

  public override void OnEnter() {
    pendulum.gameObject.SetActive(true);
    pendulum.StringRenderer.positionCount = 2;
    bobAnchorCurrentAngle = pendulum.Config.InitialAngle * Mathf.Deg2Rad;
    pendulum.BobAnchor.bodyType = RigidbodyType2D.Kinematic;
    UserInput.Instance.RegisterPointerUpListener(OnPointerUp);

    CoroutineUtilities.WaitForSecondsAndDoAction(
      pendulum,
      1f,
      () => {
        AttachBob();
        bob.transform.localScale = Vector3.zero;
        CoroutineUtilities.Lerp(pendulum, .5f, t => bob.transform.localScale = Vector3.Lerp(bob.transform.localScale, Vector3.one, t));
      }
    );
  }

  public override void Update() {
    UpdatePosition();
    DrawString();
  }

  public override void FixedUpdate() => CalculatePhysics();

  private void CalculatePhysics() {
    var dt = Time.fixedDeltaTime;
    var angularAcceleration = pendulum.Config.Gravity / pendulum.Config.Length * Mathf.Sin(bobAnchorCurrentAngle);
    bobAnchorAngularVelocity += angularAcceleration * dt * pendulum.Config.Speed;
    bobAnchorCurrentAngle += bobAnchorAngularVelocity * dt;
    Debug.Log($"velocity: {bobAnchorAngularVelocity * Mathf.Rad2Deg}, angle : {bobAnchorCurrentAngle * Mathf.Rad2Deg}");
  }

  private void UpdatePosition() {
    Vector2 targetPosition = pendulum.Anchor.localPosition;
    targetPosition.x += pendulum.Config.Length * Mathf.Sin(bobAnchorCurrentAngle);
    targetPosition.y -= pendulum.Config.Length * Mathf.Cos(bobAnchorCurrentAngle);

    pendulum.BobAnchor.MovePosition(targetPosition);
  }

  private void DrawString() {
    pendulum.StringRenderer.SetPosition(0, pendulum.Anchor.localPosition);
    pendulum.StringRenderer.SetPosition(1, pendulum.BobAnchor.transform.localPosition);
  }

  public override void OnExit() {
    ObjectPoolManager.ReturnObjectToPool(bob.gameObject);
    bob = null;
    pendulum.StringRenderer.positionCount = 0;
    UserInput.Instance.UnregisterPointerUpListener(OnPointerUp);
  }

  private void InstantiateBob() {
    bob = ObjectPoolManager.SpawnObject(pendulum.BobPrefab, pendulum.transform, Quaternion.identity).GetComponent<PendulumBob>();
    bob.transform.localPosition = pendulum.BobAnchor.position + pendulum.BobInitPosition;
    bob.Initialize(pendulum.BobAnchor, BobState.Connected);
  }

  private void AttachBob() {
    InstantiateBob();
  }

  private void OnPointerUp(InputAction.CallbackContext ctx) {
    bob.DisconnectBob();
    AttachBob();
  }
}