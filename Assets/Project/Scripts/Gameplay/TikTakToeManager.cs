using System;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class TikTakToeManager : Singleton<TikTakToeManager> {
  #region events

  private event Action<Color> winConditionReachedEvent;
  private void InvokeWinConditionReached(Color color) => winConditionReachedEvent?.Invoke(color);
  public void RegisterWinConditionReachedListener(Action<Color> callback) => winConditionReachedEvent += callback;

  #endregion

  private const int CAPACITY = 3;

  [SerializeField] private LayerMask bobLayer;
  [SerializeField] private ColorPoints[] colorPoints;
  [SerializeField] private BoxCastScanner2D[] scanners = new BoxCastScanner2D[CAPACITY];
  [SerializeField] private float bobCollectionDelay = 1.5f;

  private PendulumBob[,] bobs;
  public int Score { get; set; }

  private void Start() {
    PendulumBob.RegisterBobDisconnectedListener(OnBobDisconnected);
  }

  private void OnBobDisconnected() {
    CoroutineUtilities.WaitForSecondsAndDoAction(
      this,
      bobCollectionDelay,
      () => {
        CollectBobs();
        var winColor = ResolveWinConditions();
        if (winColor != default) {
          InvokeWinConditionReached(winColor);
          return;
        }
        
        const int maxBobs = CAPACITY * CAPACITY;
        var bobCount = bobs.Cast<PendulumBob>().Count(bob => bob != null);
        if (bobCount >= maxBobs) InvokeWinConditionReached(default);
      }
    );
  }

  private void CollectBobs() {
    bobs = new PendulumBob[CAPACITY, CAPACITY];

    for (var scannerIndex = 0; scannerIndex < scanners.Length; scannerIndex++) {
      var collidedBobs = scanners[scannerIndex].ScanForLayer<PendulumBob>(CAPACITY, bobLayer).OrderBy(bob => bob.transform.position.y).ToArray();

      for (var resultIndex = 0; resultIndex < collidedBobs.Length; resultIndex++) {
        bobs[scannerIndex, resultIndex] = collidedBobs[resultIndex];
      }
    }
  }

  private Color ResolveWinConditions() {
    foreach (var color in colorPoints.Select(cp => cp.Color)) {
      if (ResolveWinCondition(color) != null) return color;
    }

    return default;
  }

  private int[,] ResolveWinCondition(Color color) {
    for (var row = 0; row < CAPACITY; row++) {
      if (bobs[0, row] == null || bobs[1, row] == null || bobs[2, row] == null) continue;

      if (bobs[0, row].GetColor() == color && bobs[1, row].GetColor() == color && bobs[2, row].GetColor() == color) {
        return new[,] { { 0, 1, 2 }, { row, row, row } };
      }
    }

    for (var column = 0; column < CAPACITY; column++) {
      if (bobs[column, 0] == null || bobs[column, 1] == null || bobs[column, 2] == null) continue;

      if (bobs[column, 0].GetColor() == color && bobs[column, 1].GetColor() == color && bobs[column, 2].GetColor() == color) {
        return new[,] { { column, column, column }, { 0, 1, 2 } };
      }
    }

    if (bobs[0, 0] != null && bobs[1, 1] != null && bobs[2, 2] != null) {
      if (bobs[0, 0].GetColor() == color && bobs[1, 1].GetColor() == color && bobs[2, 2].GetColor() == color) {
        return new[,] { { 0, 1, 2 }, { 0, 1, 2 } };
      }
    }

    if (bobs[2, 0] != null && bobs[1, 1] != null && bobs[0, 2] != null) {
      if (bobs[2, 0].GetColor() == color && bobs[1, 1].GetColor() == color && bobs[0, 2].GetColor() == color) {
        return new[,] { { 2, 1, 0 }, { 0, 1, 2 } };
      }
    }

    return null;
  }

  public void DestroyBobsOfColor(Color color) {
    var points = colorPoints.FirstOrDefault(cp => cp.Color == color);
    if (points == null) return;

    var winBobs = ResolveWinCondition(color);
    bobs[winBobs[0, 0], winBobs[1, 0]].Destroy();
    bobs[winBobs[0, 1], winBobs[1, 1]].Destroy();
    bobs[winBobs[0, 2], winBobs[1, 2]].Destroy();

    Score += points.Points;
  }

  public bool ContainsBob(PendulumBob bob) {
    foreach (var collectedBob in bobs) {
      if (bob == collectedBob) return true;
    }

    return false;
  }
}