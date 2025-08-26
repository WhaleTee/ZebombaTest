using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class CastScanner2D : MonoBehaviour {
  [SerializeField] protected Vector2 center;
  [SerializeField] protected Vector2Int direction;

  public IEnumerable<T> ScanForLayer<T>(uint expectedObjectsCount, LayerMask layer) where T : Object {
    var hits = new RaycastHit2D[expectedObjectsCount];
    CastNonAlloc(hits, layer);
    if (typeof(GameObject).IsAssignableFrom(typeof(T))) return SelectGameObject(hits) as IEnumerable<T>;

    return SelectComponent<T>(hits);
  }

  private IEnumerable<GameObject> SelectGameObject(RaycastHit2D[] hits) {
    return hits is { Length: > 0 } ? hits.Where(hit => hit.collider).Select(hit => hit.collider.gameObject).ToArray() : Array.Empty<GameObject>();
  }

  private IEnumerable<T> SelectComponent<T>(RaycastHit2D[] hits) {
    return hits is { Length: > 0 } ? hits.Where(hit => hit.collider).Select(hit => hit.collider.GetComponent<T>()).ToArray() : Array.Empty<T>();
  }

  protected abstract void CastNonAlloc(in RaycastHit2D[] hits, LayerMask layer);
}