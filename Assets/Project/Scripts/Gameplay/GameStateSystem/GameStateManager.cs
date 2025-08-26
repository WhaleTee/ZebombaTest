using System;
using System.Collections;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager> {
  private event Action<GameState> gameStateChanged;

  public GameState CurrentState { get;  private set; }

  private void Start() {
    CurrentState = GameState.MainMenu;
    StartCoroutine(WaitForSecondsAndDoAction(1f, () => gameStateChanged?.Invoke(CurrentState)));
    RegisterCallbacks();
  }

  private IEnumerator WaitForSecondsAndDoAction(float seconds, Action action) {
    yield return new WaitForSeconds(seconds);

    action.Invoke();
  }

  private void RegisterCallbacks() {
    GameUI.instance.RegisterPlayButtonPerformedListener(() => ChangeState(GameState.Gameplay));

    GameUI.instance.RegisterMainMenuButtonPerformedListener(() => {
                                                              ChangeState(GameState.MainMenu);
                                                              TikTakToeManager.Instance.Score = 0;
                                                            }
    );

    GameUI.instance.RegisterRestartButtonPerformedListener(() => {
                                                             ChangeState(GameState.Gameplay);
                                                             TikTakToeManager.Instance.Score = 0;
                                                           }
    );

    TikTakToeManager.instance.RegisterWinConditionReachedListener(OnWinConditionReached);
  }

  private void OnWinConditionReached(Color color) {
    if (color != default) TikTakToeManager.Instance.DestroyBobsOfColor(color);
    else ChangeState(GameState.GameOver);
  }

  private void ChangeState(GameState newState) {
    CurrentState = newState;
    gameStateChanged?.Invoke(CurrentState);
  }

  public void RegisterGameStateChangedListener(Action<GameState> listener) => gameStateChanged += listener;

  public void UnregisterGameStateChangedListener(Action<GameState> listener) => gameStateChanged -= listener;
}