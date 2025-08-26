using System;
using UnityEngine;

[Serializable]
public class PendulumMathConfig {
  [field: SerializeField] public float Length { get; private set; } = 2f;
  [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
  [field: SerializeField] public float InitialAngle { get; private set; } = 45f;
  [field: SerializeField] public float Speed { get; private set; } = 1f;
}