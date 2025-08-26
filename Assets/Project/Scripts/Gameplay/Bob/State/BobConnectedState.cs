using UnityEngine;

public class BobConnectedState : BaseState {
  private readonly Rigidbody2D anchor;
  private readonly HingeJoint2D hingeJoint;
  private readonly CircleCollider2D collider;
  
  public BobConnectedState(Rigidbody2D anchor, HingeJoint2D hingeJoint, CircleCollider2D collider) {
    this.anchor = anchor;
    this.hingeJoint = hingeJoint;
    this.collider = collider;
  }
  
  public override void OnEnter() {
    hingeJoint.connectedBody = anchor;
    hingeJoint.enabled = true;
    collider.enabled = false;
  }
}