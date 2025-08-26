using System;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

[DisallowMultipleComponent]
[RequireComponent(typeof(UIDocument))]
public class GameUI : Singleton<GameUI>, GameStateGameOverHandler {
  private UIDocument gameUIDocument;
  private VisualElement root;
  private VisualElement playButtonGroup;
  private Button playButton;
  private VisualElement gameOverButtonGroup;
  private Button returnGameplayButton;
  private Button returnMainMenuButton;
  private VisualElement gameOverLabelGroup;
  private Label scoreText;

  private event Action playButtonPerformedEvent;
  private event Action restartButtonPerformedEvent;
  private event Action mainMenuButtonPerformedEvent;

  protected override void Awake() {
    base.Awake();

    gameUIDocument = GetComponent<UIDocument>();

    root = gameUIDocument.rootVisualElement;
    playButtonGroup = root.Q<VisualElement>("PlayButtonGroup");
    playButton = playButtonGroup.Q<Button>("PlayButton");
    gameOverButtonGroup = root.Q<VisualElement>("GameOverButtonGroup");
    returnGameplayButton = gameOverButtonGroup.Q<Button>("ReturnGameplayButton");
    returnMainMenuButton = gameOverButtonGroup.Q<Button>("ReturnMainMenuButton");
    gameOverLabelGroup = root.Q<VisualElement>("GameOverLabelGroup");
    scoreText = gameOverLabelGroup.Q<Label>("Score");
  }

  private void Start() {
    RegisterPlayButtonGroupTransitionEndListener(OnPlayButtonGroupTransitionEnd);
    RegisterPlayButtonClickedListener(OnPlayButtonClicked);
    RegisterGameOverButtonGroupTransitionEndListener(OnGameOverButtonGroupTransitionEnd);
    RegisterReturnGameplayButtonClickedListener(OnReturnGameplayButtonClicked);
    RegisterReturnMainMenuButtonClickedListener(OnReturnMainMenuButtonClicked);
  }

  public void RegisterPlayButtonGroupTransitionEndListener(EventCallback<TransitionEndEvent> callback) => playButtonGroup.RegisterCallback(callback);
  public void RegisterPlayButtonClickedListener(EventCallback<PointerUpEvent> callback) => playButton.RegisterCallback(callback);

  public void RegisterGameOverButtonGroupTransitionEndListener(EventCallback<TransitionEndEvent> callback) {
    gameOverButtonGroup.RegisterCallback(callback);
  }

  public void RegisterReturnGameplayButtonClickedListener(EventCallback<PointerUpEvent> callback) => returnGameplayButton.RegisterCallback(callback);
  public void RegisterReturnMainMenuButtonClickedListener(EventCallback<PointerUpEvent> callback) => returnMainMenuButton.RegisterCallback(callback);

  public void RegisterPlayButtonPerformedListener(Action action) => playButtonPerformedEvent += action;
  public void RegisterRestartButtonPerformedListener(Action action) => restartButtonPerformedEvent += action;
  public void RegisterMainMenuButtonPerformedListener(Action action) => mainMenuButtonPerformedEvent += action;

  private void OnPlayButtonGroupTransitionEnd(TransitionEndEvent ctx) {
    var visualElement = (VisualElement)ctx.target;
    if (visualElement.name == "PlayButtonGroup" && !playButtonGroup.ClassListContains("play-button-group--in")) playButtonPerformedEvent?.Invoke();
  }

  private void OnGameOverButtonGroupTransitionEnd(TransitionEndEvent ctx) {
    var visualElement = (VisualElement)ctx.target;

    if (visualElement.name == "GameOverButtonGroup" && !visualElement.ClassListContains("game-over-button-group--in")) {
      if (!playButtonGroup.ClassListContains("play-button-group--in")) restartButtonPerformedEvent?.Invoke();
      else mainMenuButtonPerformedEvent?.Invoke();
    }
  }

  private void OnPlayButtonClicked(PointerUpEvent ctx) {
    root.RemoveFromClassList("global-container--shadowed");
    playButtonGroup.RemoveFromClassList("play-button-group--in");
  }

  private void OnReturnGameplayButtonClicked(PointerUpEvent ctx) {
    gameOverButtonGroup.RemoveFromClassList("game-over-button-group--in");
    gameOverLabelGroup.RemoveFromClassList("game-over-label-group--in");
  }

  private void OnReturnMainMenuButtonClicked(PointerUpEvent ctx) {
    root.AddToClassList("global-container--shadowed");
    playButtonGroup.AddToClassList("play-button-group--in");
    gameOverButtonGroup.RemoveFromClassList("game-over-button-group--in");
    gameOverLabelGroup.RemoveFromClassList("game-over-label-group--in");
  }

  public void HandleGameOver() {
    gameOverButtonGroup.AddToClassList("game-over-button-group--in");
    gameOverLabelGroup.AddToClassList("game-over-label-group--in");
    scoreText.text = TikTakToeManager.Instance.Score.ToString();
  }
}