public class PendulumInactiveState : BaseState {
  private readonly Pendulum pendulum;

  public PendulumInactiveState(Pendulum pendulum) => this.pendulum = pendulum;

  public override void OnEnter() {
    ObjectPoolManager.ClearPool(pendulum.BobPrefab);
    pendulum.gameObject.SetActive(false);
  }
}