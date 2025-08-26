using UnityEngine;

public class BobDisconnectedState : BaseState {
  private readonly PendulumBob bob;
  private readonly Rigidbody2D rigidbody;
  private readonly HingeJoint2D hingeJoint;
  private readonly CircleCollider2D collider;

  public BobDisconnectedState(PendulumBob bob, Rigidbody2D rigidbody, HingeJoint2D hingeJoint, CircleCollider2D collider) {
    this.bob = bob;
    this.rigidbody = rigidbody;
    this.hingeJoint = hingeJoint;
    this.collider = collider;
  }

  public override void OnEnter() {
    rigidbody.bodyType = RigidbodyType2D.Dynamic;
    hingeJoint.enabled = false;
    collider.enabled = true;

    if (GameStateManager.Instance.CurrentState == GameState.Gameplay) {
      CoroutineUtilities.WaitForSecondsAndDoAction(
        bob,
        bob.DisconnectedBobDisappearingDelay,
        () => {
          if (!TikTakToeManager.Instance.ContainsBob(bob)) {
            CoroutineUtilities.Lerp(
              bob,
              bob.DisconnectedBobDisappearingDuration,
              t => {
                bob.transform.localScale = Vector3.Lerp(bob.transform.localScale, Vector3.zero, t);
                if (t == 1) ObjectPoolManager.ReturnObjectToPool(bob.gameObject);
              }
            );
          }
        }
      );
    }
  }
}