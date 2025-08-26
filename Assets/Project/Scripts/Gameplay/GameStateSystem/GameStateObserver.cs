using System;
using System.Linq;
using UnityEngine;

public class GameStateObserver : Singleton<GameStateObserver> {
  private GameStateMainMenuHandler[] mainMenuHandlers;
  private GameStateGameplayHandler[] gameplayHandlers;
  private GameStateGameOverHandler[] gameOverHandlers;

  protected override void Awake() {
    base.Awake();
    FindHandlers();
  }

  private void Start() {
    SubscribeGameStateChanged();
  }

  private void FindHandlers() {
    mainMenuHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
    .OfType<GameStateMainMenuHandler>()
    .ToArray();

    gameplayHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
    .OfType<GameStateGameplayHandler>()
    .ToArray();

    gameOverHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
    .OfType<GameStateGameOverHandler>()
    .ToArray();
  }

  private void SubscribeGameStateChanged() {
    GameStateManager.instance.RegisterGameStateChangedListener(OnGameStateChanged);
  }

  private void OnGameStateChanged(GameState gameState) {
    switch (gameState) {
      case GameState.MainMenu:
        foreach (var handler in mainMenuHandlers) {
          handler.HandleMainMenu();
        }

        break;
      case GameState.Gameplay:
        foreach (var handler in gameplayHandlers) {
          handler.HandleGameplay();
        }

        break;
      case GameState.GameOver:
        foreach (var handler in gameOverHandlers)
        {
          handler.HandleGameOver();
        }

        break;
    }
  }
}