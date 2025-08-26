using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager> {
  private GameObject emptyHolder;
  private static GameObject particleSystemsEmpty;
  private static GameObject gameObjectsEmpty;
  private static GameObject soundFXEmpty;

  private static Dictionary<GameObject, ObjectPool<GameObject>> objectPools;
  private static Dictionary<GameObject, GameObject> cloneToPrefabMap;

  public enum PoolType {
    ParticleSystems, GameObjects, SoundFX
  }

  protected override void Awake() {
    base.Awake();
    objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
    cloneToPrefabMap = new Dictionary<GameObject, GameObject>();
    SetupEmpties();
  }

  private void SetupEmpties() {
    emptyHolder = new GameObject("ObjectPools");

    particleSystemsEmpty = new GameObject("ParticleEffects");
    particleSystemsEmpty.transform.SetParent(emptyHolder.transform);

    gameObjectsEmpty = new GameObject("GameObjects");
    gameObjectsEmpty.transform.SetParent(emptyHolder.transform);

    soundFXEmpty = new GameObject("SoundFX");
    soundFXEmpty.transform.SetParent(emptyHolder.transform);

    DontDestroyOnLoad(particleSystemsEmpty.transform.root);
  }

  private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType) {
    var pool = new ObjectPool<GameObject>(
      createFunc: () => CreateObject(prefab, pos, rot, poolType),
      actionOnGet: OnGetObject,
      actionOnRelease: OnReleaseObject,
      actionOnDestroy: OnDestroyObject
    );

    objectPools.Add(prefab, pool);
  }

  private static void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType) {
    var pool = new ObjectPool<GameObject>(
      createFunc: () => CreateObject(prefab, parent, rot, poolType),
      actionOnGet: OnGetObject,
      actionOnRelease: OnReleaseObject,
      actionOnDestroy: OnDestroyObject
    );

    objectPools.Add(prefab, pool);
  }

  private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects) {
    prefab.SetActive(false);
    var go = Instantiate(prefab, pos, rot);

    prefab.SetActive(true);

    var parentObject = SetParentObject(poolType);
    go.transform.SetParent(parentObject.transform);

    return go;
  }

  private static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects) {
    prefab.SetActive(false);
    var go = Instantiate(prefab, parent);

    go.transform.localPosition = Vector3.zero;
    go.transform.localRotation = rot;
    go.transform.localScale = Vector3.one;

    prefab.SetActive(true);

    var parentObject = SetParentObject(poolType);
    go.transform.SetParent(parentObject.transform);

    return go;
  }

  private static void OnGetObject(GameObject go) {
    // optional logic
  }

  private static void OnReleaseObject(GameObject go) {
    go.transform.localScale = Vector3.one;
    go.SetActive(false);
  }

  private static void OnDestroyObject(GameObject go) => cloneToPrefabMap.Remove(go);

  private static GameObject SetParentObject(PoolType poolType) {
    return poolType switch {
             PoolType.ParticleSystems => particleSystemsEmpty,
             PoolType.GameObjects => gameObjectsEmpty,
             PoolType.SoundFX => soundFXEmpty,
             var _ => null
           };
  }

  private static T SpawnObject<T>(
    GameObject objectToSpawn,
    Vector3 spawnPos,
    Quaternion spawnRotation,
    PoolType poolType = PoolType.GameObjects
  ) where T : Object {
    if (!objectPools.ContainsKey(objectToSpawn)) CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);

    var go = objectPools[objectToSpawn].Get();

    if (go) {
      cloneToPrefabMap.TryAdd(go, objectToSpawn);

      go.transform.position = spawnPos;
      go.transform.rotation = spawnRotation;
      go.SetActive(true);

      if (typeof(T) == typeof(GameObject)) return go as T;

      var component = go.GetComponent<T>();

      if (!component) {
        Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
        return null;
      }

      return component;
    }

    return null;
  }

  private static T SpawnObject<T>(
    GameObject objectToSpawn,
    Transform parent,
    Quaternion spawnRotation,
    PoolType poolType = PoolType.GameObjects
  ) where T : Object {
    if (!objectPools.ContainsKey(objectToSpawn)) CreatePool(objectToSpawn, parent, spawnRotation, poolType);

    var go = objectPools[objectToSpawn].Get();

    if (go) {
      cloneToPrefabMap.TryAdd(go, objectToSpawn);

      go.transform.SetParent(parent);
      go.transform.localPosition = Vector3.zero;
      go.transform.localRotation = spawnRotation;
      go.SetActive(true);

      var result = go as T;

      if (!result) {
        Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}.");
        return null;
      }

      return result;
    }

    return null;
  }

  public static T SpawnObject<T>(
    T typePrefab,
    Vector3 spawnPos,
    Quaternion spawnRotation,
    PoolType poolType = PoolType.GameObjects
    ) where T : Component => SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);

  public static GameObject SpawnObject(
    GameObject objectToSpawn,
    Vector3 spawnPos,
    Quaternion spawnRotation,
    PoolType poolType = PoolType.GameObjects
  ) => SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);

  public static T SpawnObject<T>(
    T typePrefab,
    Transform parent,
    Quaternion spawnRotation,
    PoolType poolType = PoolType.GameObjects
  ) where T : Component => SpawnObject<T>(typePrefab.gameObject, parent, spawnRotation, poolType);

  public static GameObject SpawnObject(
    GameObject objectToSpawn,
    Transform parent,
    Quaternion spawnRotation,
    PoolType poolType = PoolType.GameObjects
  ) => SpawnObject<GameObject>(objectToSpawn, parent, spawnRotation, poolType);

  public static void ReturnObjectToPool(GameObject go, PoolType poolType = PoolType.GameObjects) {
    if (cloneToPrefabMap.TryGetValue(go, out var prefab)) {
      var parentObject = SetParentObject(poolType);

      if (go.transform.parent != parentObject.transform) go.transform.SetParent(parentObject.transform);
      if (objectPools.TryGetValue(prefab, out var pool)) pool.Release(go);
    } else Debug.LogWarning("Trying to return an object that is not pooled: " + go.name);
  }

  public static void ClearPool(GameObject prefab, PoolType poolType = PoolType.GameObjects) {
    var gos = cloneToPrefabMap
    .Where(map => map.Value == prefab && map.Key.activeInHierarchy)
    .Select(map => map.Key);

    foreach (var go in gos) {
      var parentObject = SetParentObject(poolType);
      if (go.transform.parent != parentObject.transform) go.transform.SetParent(parentObject.transform);
      if (objectPools.TryGetValue(prefab, out var pool)) pool.Release(go);
    }
  }
}