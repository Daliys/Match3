using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectPool
{
    /// <summary>
    ///  Class that handle object pooling.
    /// </summary>
    public class ObjectPooling : MonoBehaviour
    {
        public static ObjectPooling Instance;

        [SerializeField] private GameObject gameParentToSpawn;
        [SerializeField] private ObjectPoolingPrefabsSO prefabsSo;

        // dictionary that contains queues for each type of object (in our case we have only one type of object)
        private Dictionary<TileType, Queue<GameObject>> _tilePool;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _tilePool = new Dictionary<TileType, Queue<GameObject>>();

            // creating queues for each type of type (1 type because it just a demo)
            foreach (var obstaclePool in Enum.GetValues(typeof(TileType)))
            {
                _tilePool.Add((TileType)obstaclePool, new Queue<GameObject>());
            }
        }
        
        private GameObject CreateObject(TileType obstacleType)
        {
            return CreateObject(obstacleType, prefabsSo.tilesHolder);
        }

        /// <summary>
        /// Creates and initializes a GameObject based on the specified object type.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="objectType">The type of the object to create.</param>
        /// <param name="itemList">An array of PoolObjectHolder containing type-prefab mappings.</param>
        /// <returns>The instantiated GameObject associated with the provided object type.</returns>
        /// <exception cref="System.Exception">Thrown when the prefab for the specified type is not found.</exception>
        private GameObject CreateObject<T>(T objectType, PoolObjectHolder<T>[] itemList)
        {
            // Get the prefab from the Scriptable Object by type of object
            GameObject prefab = itemList.FirstOrDefault(item => item.type.Equals(objectType))?.prefab;

            if (prefab == null)
            {
                throw new Exception($"Prefab for type {objectType} not found");
            }

            GameObject gm = Instantiate(prefab, gameParentToSpawn.transform);
            gm.SetActive(false);
            return gm;
        }
       
        public GameObject GetObject(TileType tileType)
        {
            // if we don't have any available object in the pool, we create a new one otherwise we dequeue the first object in the queue
            GameObject obj = _tilePool[tileType].Count == 0
                ? CreateObject(tileType)
                : _tilePool[tileType].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        
        public void ReturnToPool(GameObject obj)
        {
            PoolObjectType type = obj.GetComponent<PoolObjectType>();

            if (type == null)
            {
                throw new Exception("The object you are trying to return to the pool does not have a PoolObjectType component");
            }

            switch (type)
            {
                case PoolObjectTileComponent tile:
                    _tilePool[tile.type].Enqueue(obj);
                    break;
            }

            obj.SetActive(false);
        }
    }
}