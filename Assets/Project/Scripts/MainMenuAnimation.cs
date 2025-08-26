using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainMenuAnimation : MonoBehaviour, GameStateMainMenuHandler, GameStateGameplayHandler {
  [SerializeField] private GameObject bobPrefab;
  [SerializeField] private float bobLifetime;
  [SerializeField] private float bobSpawnInterval;
  [SerializeField] private Vector2 mainMenuStagePosition;
  [SerializeField] private Vector2 gameplayStagePosition;
  [SerializeField] private float swipeDuration;

  private readonly Dictionary<GameObject, float> bobs = new Dictionary<GameObject, float>();

  private bool active;
  
  private void Update() {
    foreach (var bob in bobs.Keys.ToList()) {
      bobs[bob] += Time.deltaTime;
    }
  }

  private IEnumerator InstantiateBob() {
    while (active) {
      var bob = ObjectPoolManager.SpawnObject(bobPrefab, transform, Quaternion.identity).GetComponent<PendulumBob>();
      bob.transform.position += new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
      bob.Initialize(null, BobState.Disconnected);
      bobs.Add(bob.gameObject, 0f);
      yield return new WaitForSeconds(bobSpawnInterval);

      foreach (var key in bobs.Keys.ToList()) {
        if (bobs[key] >= bobLifetime) {
          bobs.Remove(key);
          key.GetComponent<PendulumBob>().Destroy();
        }
      }
    }
  }

  public void HandleMainMenu() {
    active = true;
    StartCoroutine(InstantiateBob());
    LerpPosition(mainMenuStagePosition);
  }

  public void HandleGameplay() {
    active = false;

    CoroutineUtilities
    .Lerp(
      this,
      swipeDuration,
      t => {
        transform.position = Vector3.Lerp(transform.position, gameplayStagePosition, t);

        if (t == 1f) {
          foreach (var key in bobs.Keys.ToList()) {
            bobs.Remove(key);
            ObjectPoolManager.ReturnObjectToPool(key);
          }
        }
      }
    );
  }
  
  private void LerpPosition(Vector3 position) {
    CoroutineUtilities.Lerp(this, swipeDuration, t => transform.position = Vector3.Lerp(transform.position, position, t));
  }
}