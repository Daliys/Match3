using UnityEngine;

namespace ObjectPool
{
    /// <summary>
    ///  Scriptable object that holds prefabs for object pooling.
    /// </summary>
    [CreateAssetMenu(fileName = "ObjectPoolingPrefabsSO", menuName = "ScriptableObjects/ObjectPoolingPrefabsSO")]
    public class ObjectPoolingPrefabsSO : ScriptableObject
    {
        public PoolObjectHolder<TileType>[] tilesHolder;
    }
}