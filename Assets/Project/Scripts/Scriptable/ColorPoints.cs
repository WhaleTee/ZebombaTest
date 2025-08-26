using UnityEngine;

[CreateAssetMenu(fileName = "EmptyColorPoints", menuName = "Gameplay/ColorPoints")]
public class ColorPoints : ScriptableObject {
  [field: SerializeField] public Color Color { get; private set; }
  [field: SerializeField] public int Points { get; private set; }
}