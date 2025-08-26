using UnityEngine;

[DisallowMultipleComponent]
public class GameplayScreen : MonoBehaviour, GameStateMainMenuHandler, GameStateGameplayHandler, GameStateGameOverHandler {
  [SerializeField] private Vector2 mainMenuStagePosition;
  [SerializeField] private Vector2 gameplayStagePosition;
  [SerializeField] private Vector2 gameOverStagePosition;
  [SerializeField] private float swipeDuration;

  public void HandleMainMenu() => transform.position = mainMenuStagePosition;

  public void HandleGameplay() => LerpPosition(gameplayStagePosition);

  public void HandleGameOver() => LerpPosition(gameOverStagePosition);

  private void LerpPosition(Vector3 position) {
    CoroutineUtilities.Lerp(this, swipeDuration, t => transform.position = Vector3.Lerp(transform.position, position, t));
  }
}