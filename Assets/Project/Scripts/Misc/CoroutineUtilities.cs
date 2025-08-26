using System;
using System.Collections;
using UnityEngine;

public static class CoroutineUtilities {
  public static IEnumerator Lerp(
    float duration,
    Action<float> action,
    bool fixedTime = false,
    bool smooth = false,
    AnimationCurve curve = null,
    bool inverse = false
  ) {
    float time = 0;
    Func<float, float> evaluateTime = t => t;
    if (smooth) evaluateTime = t => Mathf.SmoothStep(0, 1, t);
    if (curve != null) evaluateTime = curve.Evaluate;

    while (time < duration) {
      var delta = fixedTime ? Time.fixedDeltaTime : Time.deltaTime;
      var elapsedTime = time + delta > duration ? 1 : time / duration;

      if (inverse) elapsedTime = 1 - elapsedTime;

      action(evaluateTime(elapsedTime));
      time += delta;
      yield return null;
    }

    action(evaluateTime(inverse ? 0 : 1));
  }

  public static Coroutine Lerp(MonoBehaviour go, float duration, Action<float> action) => go.StartCoroutine(Lerp(duration, action));

  public static Coroutine WaitForSecondsAndDoAction(MonoBehaviour go, float seconds, Action action) {
    return go.StartCoroutine(WaitForSecondsAndDoAction(seconds, action));
  }

  private static IEnumerator WaitForSecondsAndDoAction(float seconds, Action action) {
    yield return new WaitForSeconds(seconds);

    action.Invoke();
  }
}