using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BobDestroyParticles : MonoBehaviour {
  [SerializeField] [Range(0, 1)] private float lifetime;
  private ParticleSystem particles;

  private void Awake() {
    particles = GetComponent<ParticleSystem>();
    particles.Stop();
  }

  public void Play(Color color) {
    var particlesMain = particles.main;
    particlesMain.startColor = color;
    particles.Play();

    CoroutineUtilities.WaitForSecondsAndDoAction(
      this,
      lifetime,
      () => {
        particles.Stop();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
      }
    );
  }
}