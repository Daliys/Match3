using System;
using UnityEngine;

namespace ObjectPool
{
    /// <summary>
    ///  Class that holds prefab for object pooling.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PoolObjectHolder<T>
    {
        public GameObject prefab;
        public T type;

    }
}