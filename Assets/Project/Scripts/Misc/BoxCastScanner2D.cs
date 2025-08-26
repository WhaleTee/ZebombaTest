using UnityEngine;

public class BoxCastScanner2D : CastScanner2D {
  [SerializeField] protected Vector2 size;
  [Space]
  [SerializeField] protected bool debug;

  private Vector2 Position => transform.position + new Vector3(center.x, center.y);

  private void OnDrawGizmosSelected() {
    if (debug) {
      Gizmos.color = Color.green;
      Gizmos.DrawWireCube(Position, size);
    }
  }

  protected override void CastNonAlloc(in RaycastHit2D[] hits, LayerMask layer) {
    var filter = new ContactFilter2D();
    filter.SetLayerMask(layer);
    Physics2D.BoxCast(
      Position,
      size,
      0f,
      direction,
      filter,
      hits
    );
  }
}